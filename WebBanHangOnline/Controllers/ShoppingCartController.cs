using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
using WebBanHangOnline.Models.Payments;

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
        public ActionResult CheckOutSuccess() => View();

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
            var cartItems = Session["Cart"] as List<ShoppingCartItem>;
            if (!ModelState.IsValid || cartItems == null || !cartItems.Any())
            {
                return Json(new { Success = false, Message = "Giỏ hàng trống hoặc thông tin không hợp lệ" });
            }

            var order = new Order
            {
                CustomerName = req.CustomerName,
                Phone = req.Phone,
                Address = req.Address,
                Email = req.Email,
                TypePayment = req.TypePayment,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                CreatedBy = req.Phone,
                Code = "DH" + new Random().Next(1000, 9999)
            };

            order.OrderDetails = Cart.Items.Select(x => new OrderDetail
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                Price = x.Price
            }).ToList();

            order.TotalAmount = order.OrderDetails.Sum(x => x.Price * x.Quantity);
            db.Orders.Add(order);
            db.SaveChanges();

            //// gửi mail khách + admin
            //var spHtml = string.Join("", Cart.Items.Select(sp => $@"
            //    <tr>
            //        <td>{sp.ProductName}</td>
            //        <td>{sp.Quantity}</td>
            //        <td>{WebBanHangOnline.Common.Common.FormatNumber(sp.TotalPrice, 0)}</td>
            //    </tr>"));

            //var templatePath = Server.MapPath("~/Content/templates/");
            //var ngayDat = DateTime.Now.ToString("dd/MM/yyyy");
            //var total = WebBanHangOnline.Common.Common.FormatNumber(order.TotalAmount, 0);

            //string contentCustomer = System.IO.File.ReadAllText(templatePath + "send2.html");
            //contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code)
            //    .Replace("{{SanPham}}", spHtml)
            //    .Replace("{{NgayDat}}", ngayDat)
            //    .Replace("{{TenKhachHang}}", order.CustomerName)
            //    .Replace("{{Phone}}", order.Phone)
            //    .Replace("{{Email}}", order.Email)
            //    .Replace("{{DiaChiNhanHang}}", order.Address)
            //    .Replace("{{ThanhTien}}", total)
            //    .Replace("{{TongTien}}", total);

            //string contentAdmin = System.IO.File.ReadAllText(templatePath + "send1.html")
            //    .Replace("{{MaDon}}", order.Code)
            //    .Replace("{{SanPham}}", spHtml)
            //    .Replace("{{NgayDat}}", ngayDat)
            //    .Replace("{{TenKhachHang}}", order.CustomerName)
            //    .Replace("{{Phone}}", order.Phone)
            //    .Replace("{{Email}}", order.Email)
            //    .Replace("{{DiaChiNhanHang}}", order.Address)
            //    .Replace("{{ThanhTien}}", total)
            //    .Replace("{{TongTien}}", total);

            //WebBanHangOnline.Common.Common.SendMail("ShopOnline", $"Đơn hàng #{order.Code}", contentCustomer, req.Email);
            //WebBanHangOnline.Common.Common.SendMail("ShopOnline", $"Đơn hàng mới #{order.Code}", contentAdmin, ConfigurationManager.AppSettings["EmailAdmin"]);

            Cart.ClearCart();
            return RedirectToAction("CheckOutSuccess");
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult AddToCart(int id, int quantity = 1)
        {
            if (quantity <= 0) quantity = 1;

            try
            {
                var product = db.Products.Find(id);
                if (product == null)
                    return Json(new { success = false, message = "Sản phẩm không tồn tại!" });

                var existing = Cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (existing != null)
                {
                    existing.Quantity += quantity;
                    existing.TotalPrice = existing.Quantity * existing.Price;
                }
                else
                {
                    var price = product.PriceSale > 0 ? (decimal)product.PriceSale : product.Price;
                    Cart.Items.Add(new ShoppingCartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Title,
                        CategoryName = product.ProductCategory?.Title ?? "",
                        Alias = product.Alias,
                        Quantity = quantity,
                        Price = price,
                        ProductImg = product.ProductImage.FirstOrDefault(x => x.IsDefault)?.ImageUrl ?? "",
                        TotalPrice = price * quantity
                    });
                }

                int count = Cart.Items.Sum(x => x.Quantity);
                return Json(new { success = true, message = "Đã thêm sản phẩm!", count });
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
                item.TotalPrice = item.Quantity * item.Price;
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

        //#region Thanh toán VNPay
        //public string UrlPayment(int type, string orderCode)
        //{
        //    var order = db.Orders.FirstOrDefault(x => x.Code == orderCode);
        //    if (order == null) return null;

        //    var price = (long)(order.TotalAmount * 100);
        //    var vnp = new VnPayLibrary();

        //    vnp.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        //    vnp.AddRequestData("vnp_Command", "pay");
        //    vnp.AddRequestData("vnp_TmnCode", ConfigurationManager.AppSettings["vnp_TmnCode"]);
        //    vnp.AddRequestData("vnp_Amount", price.ToString());
        //    vnp.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
        //    vnp.AddRequestData("vnp_CurrCode", "VND");
        //    vnp.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
        //    vnp.AddRequestData("vnp_Locale", "vn");
        //    vnp.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);
        //    vnp.AddRequestData("vnp_OrderType", "other");
        //    vnp.AddRequestData("vnp_ReturnUrl", ConfigurationManager.AppSettings["vnp_Returnurl"]);
        //    vnp.AddRequestData("vnp_TxnRef", order.Code);

        //    if (type == 1) vnp.AddRequestData("vnp_BankCode", "VNPAYQR");
        //    else if (type == 2) vnp.AddRequestData("vnp_BankCode", "VNBANK");
        //    else if (type == 3) vnp.AddRequestData("vnp_BankCode", "INTCARD");

        //    return vnp.CreateRequestUrl(ConfigurationManager.AppSettings["vnp_Url"], ConfigurationManager.AppSettings["vnp_HashSecret"]);
        //}
        //#endregion
    }
}