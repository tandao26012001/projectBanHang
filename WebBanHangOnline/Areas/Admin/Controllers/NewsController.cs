using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,Employee")]
    public class NewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/News
        //public ActionResult Index(string Searchtext, int? page)
        //{
        //    var pageSize = 10;
        //    if (page == null)
        //    {
        //        page = 1;
        //    }
        //    IEnumerable<News> items = db.News.OrderByDescending(x => x.Id);
        //    if (!string.IsNullOrEmpty(Searchtext))
        //    {
        //        items= items.Where(x=>x.Alias.Contains(Searchtext) || x.Title.Contains(Searchtext));
        //    }
        //    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
        //     items = items.ToPagedList(pageIndex, pageSize);
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.Page = page;
        //    return View(items);
        //}
        public ActionResult Index(string searchText)
        {
            var items = db.News.AsQueryable();

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
            var suggestions = db.News
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
        [HttpGet]
        public ActionResult Add()
        {
            var model = new News();

            ViewBag.Category = new SelectList(db.Categories.ToList(), "Id", "Title");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(News model)
        {
            if (ModelState.IsValid)
            {
                var news = new News
                {
                    Title = model.Title,
                    Image = model.Image,
                    Alias = Models.Common.Filter.FilterChar(model.Title),
                    SeoTitle = model.SeoTitle,
                    SeoKeywords = model.SeoKeywords,
                    Detail = model.Detail,
                    CategoryId = model.CategoryId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            LoadCategory();
            return View(model);
        }
        private void LoadCategory()
        {
            ViewBag.Category = new SelectList(db.Categories.ToList(), "Id", "Title");
        }
        public ActionResult Edit(int id)
        {
            var item = db.News.Find(id);
            ViewBag.Category = new SelectList(db.Categories.ToList(), "Id", "Title", item.CategoryId); // <-- đây là điểm quan trọng
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(News model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);
                db.News.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category = new SelectList(db.Categories.ToList(), "Id", "Title", model.CategoryId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.News.Find(id);
            if (item != null)
            {
                db.News.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.News.Find(id);
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
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.News.Find(Convert.ToInt32(item));
                        db.News.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

    }
}