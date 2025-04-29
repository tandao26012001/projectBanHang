using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Products
        public ActionResult Index(string search, int? page)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Title.Contains(search));
            }

            ViewBag.Search = search;
            return View(products.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        // Autocomplete Suggestion
        public JsonResult GetSuggestions(string term)
        {
            var suggestions = db.Products
                                .Where(p => p.Title.Contains(term))
                                .Select(p => p.Title)
                                .Take(10)
                                .ToList();
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(string alias)
        {
            var product = db.Products
                .Include("ProductCategory")
                .Include("ProductImage")
                .FirstOrDefault(x => x.Alias == alias);

            if (product == null)
            {
                return HttpNotFound();
            }

            // Lấy sản phẩm cùng loại (khác Id hiện tại)
            var relatedProducts = db.Products
                .Where(x => x.ProductCategoryId == product.ProductCategoryId && x.Id != product.Id && x.IsActive)
                .OrderByDescending(x => x.CreatedDate)
                .Take(8)
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
        public ActionResult ProductCategory(string alias, int id)
        {
            var category = db.ProductCategories.FirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                return HttpNotFound();
            }

            var products = db.Products
                .Where(x => x.ProductCategoryId == category.Id && x.IsActive)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            ViewBag.CategoryName = category.Title;
            ViewBag.CategoryAlias = category.Alias;

            return View(products); // hoặc return PartialView nếu gọi bằng Ajax
        }
        [HttpGet]
        public ActionResult Partial_ItemsByCateId(int? categoryId)
        {
            var products = db.Products.Where(p => p.IsActive);

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.ProductCategoryId == categoryId.Value);
            }

            var result = products
                .OrderByDescending(p => p.CreatedDate)
                .ToList();

            return PartialView("Partial_ItemsByCateId", result); // nên đặt tên rõ ràng
        }


        public ActionResult Partial_ProductSales()
        {
            var items = db.Products.Where(x => x.IsSale && x.IsActive).Take(12).ToList();
            return PartialView(items);
        }
        public ActionResult Partial_ProductHots()
        {
            var items = db.Products.Where(x => x.IsHot && x.IsActive).Take(12).ToList();
            return PartialView(items);
        }
        public ActionResult Partial_ProductInHome()
        {
            var items = db.Products.Where(x => x.IsHome && x.IsActive).Take(12).ToList();
            return PartialView(items);
        }
        public ActionResult Partial_ProductNews()
        {
            var items = db.Products.Where(x => x.IsActive).OrderByDescending(x=>x.CreatedDate).Take(12).ToList();
            return PartialView(items);
        }

    }
}