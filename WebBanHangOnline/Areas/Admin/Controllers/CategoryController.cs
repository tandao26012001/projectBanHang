using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Category
        public ActionResult Index(string searchText)
        {
            var items = db.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                items = items.Where(x => x.Title.Contains(searchText));
            }

            ViewBag.SearchText = searchText; // để giữ giá trị khi submit
            return View(items.OrderByDescending(x => x.Id).ToList());
        }
        [HttpGet]
        public JsonResult GetSuggestions(string term)
        {
            var suggestions = db.Categories
                .Where(p => p.Title.Contains(term))
                .Select(p => new
                {
                    label = p.Title,
                    value = p.Title // giá trị điền vào ô tìm kiếm
                })
                .Take(10)
                .ToList();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Category model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.Position = model.Position;
                model.Alias = Models.Common.Filter.FilterChar(model.Title);
                model.Link = Models.Common.Filter.FilterChar(model.Title);
                db.Categories.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var item = db.Categories.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Attach(model);
                model.ModifiedDate = DateTime.Now;
                model.Alias = Models.Common.Filter.FilterChar(model.Title);
                model.Link = Models.Common.Filter.FilterChar(model.Title);

                db.Entry(model).Property(x => x.Title).IsModified = true;
                db.Entry(model).Property(x => x.Description).IsModified = true;
                db.Entry(model).Property(x => x.Link).IsModified = true;
                db.Entry(model).Property(x => x.Alias).IsModified = true;
                db.Entry(model).Property(x => x.SeoKeywords).IsModified = true;
                db.Entry(model).Property(x => x.SeoTitle).IsModified = true;
                db.Entry(model).Property(x => x.Position).IsModified = true;
                db.Entry(model).Property(x => x.ModifiedDate).IsModified = true;
                db.Entry(model).Property(x => x.Modifiedby).IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Categories.Find(id);
            if (item != null)
            {
                db.Categories.Remove(item);
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
                        var product = db.Categories.Find(id);
                        if (product != null)
                        {
                            db.Categories.Remove(product);
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
    }
}