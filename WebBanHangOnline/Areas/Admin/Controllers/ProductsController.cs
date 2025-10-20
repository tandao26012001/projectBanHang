using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,Employee")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Products
        //public ActionResult Index(int? page)
        //{
        //    IEnumerable<Product> items = db.Products.OrderByDescending(x => x.Id);
        //    var pageSize = 10;
        //    if (page == null)
        //    {
        //        page = 1;
        //    }
        //    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
        //    items = items.ToPagedList(pageIndex, pageSize);
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.Page = page;
        //    return View(items);
        //}
        //public ActionResult Index()
        //{
        //    var item = db.Products.ToList();
        //    return View(item);
        //}
        public ActionResult Index(string searchText)
        {
            var items = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                items = items.Where(x => x.Title.Contains(searchText) || x.ProductCode.Contains(searchText));
            }

            ViewBag.SearchText = searchText; // để giữ giá trị khi submit
            return View(items.OrderByDescending(x => x.Id).ToList());
        }

        [HttpGet]
        public JsonResult GetSuggestions(string term)
        {
            var suggestions = db.Products
                .Where(p => p.Title.Contains(term) || p.ProductCode.Contains(term))
                .Select(p => new
                {
                    label = p.Title + " (" + p.ProductCode + ")",
                    value = p.Title // giá trị điền vào ô tìm kiếm
                })
                .Take(10)
                .ToList();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            // Danh sách màu cố định
            ViewBag.Colors = db.Colors.ToList(); // Lấy danh sách màu
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product model, List<string> Images, List<int> rDefault, List<int> SelectedColors)
        {
            if (ModelState.IsValid)
            {
                if (Images != null && Images.Count > 0)
                {
                    for (int i = 0; i < Images.Count; i++)
                    {
                        if (i + 1 == rDefault[0])
                        {
                            model.Image = Images[i];
                            model.ProductImage.Add(new ProductImage
                            {
                                ProductId = model.Id,
                                ImageUrl = Images[i],
                                IsDefault = true
                            });
                        }
                        else
                        {
                            model.ProductImage.Add(new ProductImage
                            {
                                ProductId = model.Id,
                                ImageUrl = Images[i],
                                IsDefault = false
                            });
                        }
                    }
                }
                //  Gán màu cho sản phẩm
                if (SelectedColors != null && SelectedColors.Count > 0)
                {
                    model.ProductColors = new List<ProductColor>();
                    foreach (var colorId in SelectedColors)
                    {
                        model.ProductColors.Add(new ProductColor
                        {
                            ColorId = colorId
                        });
                    }
                }
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                if (string.IsNullOrEmpty(model.SeoTitle))
                {
                    model.SeoTitle = model.Title;
                }
                if (string.IsNullOrEmpty(model.Alias))
                    model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);
                db.Products.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            ViewBag.Colors = db.Colors.ToList();
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");

            var item = db.Products
                .Include("ProductColors") // để load danh sách màu sản phẩm
                .FirstOrDefault(x => x.Id == id);

            if (item == null) return HttpNotFound();

            var images = db.ProductImages.Where(x => x.ProductId == id).ToList();
            ViewBag.ProductImages = images;

            ViewBag.Colors = db.Colors.ToList(); // ✅ tất cả màu
            ViewBag.SelectedColorIds = item.ProductColors.Select(pc => pc.ColorId).ToList(); // ✅ màu đã chọn

            return View(item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Product model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                if (model.CreatedDate < new DateTime(1753, 1, 1))
                {
                    model.CreatedDate = DateTime.Now;
                }

                model.ModifiedDate = DateTime.Now;
                model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);

                db.Products.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                // --- Cập nhật ảnh ---
                var productImages = db.ProductImages.Where(x => x.ProductId == model.Id);
                db.ProductImages.RemoveRange(productImages);
                db.SaveChanges();

                var images = form.GetValues("Images");
                var defaultIndex = form["rDefault"];

                if (images != null)
                {
                    for (int i = 0; i < images.Length; i++)
                    {
                        var img = new ProductImage
                        {
                            ProductId = model.Id,
                            ImageUrl = images[i],
                            IsDefault = (defaultIndex == (i + 1).ToString()) // vì index trong table bắt đầu từ 1
                        };
                        db.ProductImages.Add(img);
                    }
                    db.SaveChanges();
                }
                // Cập nhật danh sách màu
                var selectedColors = form.GetValues("SelectedColors");
                var oldColors = db.ProductColors.Where(x => x.ProductId == model.Id);
                db.ProductColors.RemoveRange(oldColors);
                db.SaveChanges();

                if (selectedColors != null)
                {
                    foreach (var colorId in selectedColors)
                    {
                        db.ProductColors.Add(new ProductColor
                        {
                            ProductId = model.Id,
                            ColorId = int.Parse(colorId)
                        });
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            ViewBag.ProductImages = db.ProductImages.Where(x => x.ProductId == model.Id).ToList();
            ViewBag.Colors = db.Colors.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                var checkImg = item.ProductImage.Where(x => x.ProductId == item.Id);
                if (checkImg != null)
                {
                    foreach (var img in checkImg)
                    {
                        db.ProductImages.Remove(img);
                        db.SaveChanges();
                    }
                }
                db.Products.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    var idList = ids.Split(',').Select(int.Parse).ToList();

                    foreach (var id in idList)
                    {
                        var product = db.Products.Find(id);
                        if (product != null)
                        {
                            db.Products.Remove(product);
                        }
                    }

                    db.SaveChanges();

                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Danh sách rỗng" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isAcive = item.IsActive });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult IsHome(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsHome = !item.IsHome;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsHome = item.IsHome });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsSale(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsSale = !item.IsSale;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsSale = item.IsSale });
            }

            return Json(new { success = false });
        }
    }
}