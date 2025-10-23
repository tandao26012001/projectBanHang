using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ShoppingCartController()
        {
        }

        public ShoppingCartController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // Getter cho giỏ hàng từ session
        private ShoppingCart Cart
        {
            get
            {
                var cart = Session["Cart"] as ShoppingCart;
                if (cart == null)
                {
                    cart = new ShoppingCart();
                    Session["Cart"] = cart;
                }
                return cart;
            }
            set
            {
                Session["Cart"] = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.CheckCart = Cart.Items.Any() ? Cart : null;
            return View();
        }

        [AllowAnonymous]
        public ActionResult CheckOut()
        {
            var cart = Cart; // Cart là property đã xử lý sẵn từ Session["Cart"]
            return View(cart.Items); // ✅ Lấy đúng danh sách sản phẩm từ Cart
        }

        [AllowAnonymous]
        public ActionResult CheckOutSuccess()
        {
            string orderCode = TempData["OrderCode"] as string;
            ViewBag.OrderCode = orderCode;
            return View();
        }

        [AllowAnonymous]
        public PartialViewResult Partial_Item_Cart() => PartialView(Cart.Items);

        [AllowAnonymous]
        public JsonResult ShowCount()
        {
            int count = Cart?.Items.Sum(x => x.Quantity) ?? 0;
            return Json(new { Count = count }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public PartialViewResult Partial_CheckOut() => PartialView();

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewModel req)
        {
            var code = new { Success = false, Code = -1 };
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    Order order = new Order();
                    order.CustomerName = req.CustomerName;
                    order.Phone = req.Phone;
                    order.Address = req.Address;
                    order.Email = req.Email;
                    cart.Items.ForEach(x => order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        Price = x.Price
                    }));
                    order.TotalAmount = cart.Items.Sum(x => x.Price * x.Quantity);
                    order.TypePayment = req.TypePayment;
                    order.CreatedDate = DateTime.Now;
                    order.ModifiedDate = DateTime.Now;
                    order.Status = 1;
                    Random rd = new Random();
                    order.Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                    //order.E = req.CustomerName;
                    db.Orders.Add(order);
                    db.SaveChanges();
                    //send mail cho khachs hang
                    var strSanPham = "";
                    var thanhtien = decimal.Zero;
                    var TongTien = decimal.Zero;
                    foreach (var sp in cart.Items)
                    {
                        strSanPham += "<tr>";
                        strSanPham += "<td>" + sp.ProductName + "</td>";
                        strSanPham += "<td>" + sp.Quantity + "</td>";
                        strSanPham += "<td>" + WebBanHangOnline.Common.Common.FormatNumber(sp.TotalPrice, 0) + "</td>";
                        strSanPham += "</tr>";
                        thanhtien += sp.Price * sp.Quantity;
                    }
                    TongTien = thanhtien;
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code);
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.CustomerName);
                    contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
                    contentCustomer = contentCustomer.Replace("{{Email}}", req.Email);
                    contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
                    contentCustomer = contentCustomer.Replace("{{ThanhTien}}", WebBanHangOnline.Common.Common.FormatNumber(thanhtien, 0));
                    contentCustomer = contentCustomer.Replace("{{TongTien}}", WebBanHangOnline.Common.Common.FormatNumber(TongTien, 0));
                    WebBanHangOnline.Common.Common.SendMail("ShopOnline", "Đơn hàng #" + order.Code, contentCustomer.ToString(), req.Email);

                    string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                    contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code);
                    contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                    contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.CustomerName);
                    contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
                    contentAdmin = contentAdmin.Replace("{{Email}}", req.Email);
                    contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
                    contentAdmin = contentAdmin.Replace("{{ThanhTien}}", WebBanHangOnline.Common.Common.FormatNumber(thanhtien, 0));
                    contentAdmin = contentAdmin.Replace("{{TongTien}}", WebBanHangOnline.Common.Common.FormatNumber(TongTien, 0));
                    WebBanHangOnline.Common.Common.SendMail("ShopOnline", "Đơn hàng mới #" + order.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
                    cart.ClearCart();
                    TempData["OrderCode"] = order.Code;
                    return RedirectToAction("CheckOutSuccess");
                }
            }
            return Json(code);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult AddToCart(int id, int quantity = 1, string size = "", int colorId = 0)
        {
            if (quantity <= 0) quantity = 1;

            try
            {
                var product = db.Products.Find(id);
                if (product == null)
                    return Json(new { success = false, message = "Sản phẩm không tồn tại!" });

                // Tìm ColorId theo alias (nếu có)
                var colorEntity = db.Colors.FirstOrDefault(c => c.Id == colorId);
                string colorName = colorEntity != null ? colorEntity.Name : "Không xác định";
                string colorHex = colorEntity != null ? colorEntity.HexColor : "#FFFFFF";

                //  Kiểm tra item trùng: id + size + color
                var existing = Cart.Items.FirstOrDefault(x =>
                    x.ProductId == id &&
                    x.Size == size &&
                    x.ColorId == colorId
                );

                if (existing != null)
                {
                    // Nếu đã có sản phẩm trùng (cùng size + color) thì cộng dồn số lượng
                    existing.Quantity += quantity;
                    //existing.TotalPrice = existing.Quantity * existing.Price;
                }
                else
                {
                    // Nếu là sản phẩm mới hoàn toàn (khác màu hoặc size)
                    var price = product.PriceSale > 0 ? (decimal)product.PriceSale : product.Price;
                    Cart.Items.Add(new ShoppingCartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Title,
                        CategoryName = product.ProductCategory?.Title ?? "",
                        Alias = product.Alias,
                        Size = size,
                        ColorId = colorId,       // lưu ID màu
                        ColorName = colorName,   // tên hiển thị
                        ColorHex = colorHex,     // mã HEX
                        Quantity = quantity,
                        Price = price,
                        ProductImg = product.ProductImage.FirstOrDefault(x => x.IsDefault)?.ImageUrl ?? product.Image,
                        //TotalPrice = price * quantity
                    });
                }

                int count = Cart.Items.Sum(x => x.Quantity);
                return Json(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng!", count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult Update(int id, int quantity)
        {
            Cart.UpdateQuantity(id, quantity);
            return Json(new { Success = true });
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var existing = Cart.Items.FirstOrDefault(x => x.ProductId == id);
            if (existing != null)
            {
                Cart.Remove(id);
                return Json(new { Success = true, code = 1, Count = Cart.Items.Count });
            }
            return Json(new { Success = false });
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult UpdateQuantity(int id, int quantity)
        {
            var item = Cart.Items.FirstOrDefault(x => x.ProductId == id);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                //item.TotalPrice = item.Quantity * item.Price;
            }

            decimal total = Cart.Items.Sum(x => x.TotalPrice);
            int count = Cart.Items.Sum(x => x.Quantity);

            return Json(new { success = true, totalPrice = item?.TotalPrice, cartTotal = total, cartCount = count });
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult RemoveItem(int id)
        {
            Cart.Items.RemoveAll(x => x.ProductId == id);
            decimal total = Cart.Items.Sum(x => x.TotalPrice);
            int count = Cart.Items.Sum(x => x.Quantity);

            return Json(new { success = true, cartTotal = total, cartCount = count });
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult DeleteAll()
        {
            Cart.ClearCart();
            return Json(new { Success = true });
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult DeleteSelected(List<int> ids)
        {
            Cart.Items.RemoveAll(x => ids.Contains(x.ProductId));
            return Json(new { success = true });
        }
        public ActionResult GeneratePaymentQR(decimal amount, string orderCode, string method = "bank")
        {
            string qrUrl = "";

            if (method == "momo")
            {
                // 👉 QR Momo
                string momoPhone = "0347363130"; // số Momo của bạn
                string message = Uri.EscapeDataString($"Thanh toan don hang {orderCode}");
                qrUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=2|99|{momoPhone}|||0|0|{amount}|{message}";
            }
            else
            {
                // 👉 QR ngân hàng VietQR
                string bankCode = "VCB"; // Vietcombank
                string accountNumber = "0123456789";
                string accountName = "DAO NGOC TAN";
                string addInfo = Uri.EscapeDataString($"Thanh toan don hang {orderCode}");

                qrUrl = $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-compact2.png?amount={amount}&addInfo={addInfo}&accountName={Uri.EscapeDataString(accountName)}";
            }

            return Redirect(qrUrl);
        }
    }
}