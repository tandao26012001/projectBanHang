using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Role
        public ActionResult Index(string searchText)
        {
            var items = db.Roles.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                items = items.Where(x => x.Name.Contains(searchText));
            }

            ViewBag.SearchText = searchText; // để giữ giá trị khi submit
            return View(items.OrderByDescending(x => x.Id).ToList());
        }

        [HttpGet]
        public JsonResult GetSuggestions(string term)
        {
            var suggestions = db.Roles
                .Where(p => p.Name.Contains(term))
                .Select(p => new
                {
                    label = p.Name,
                    value = p.Name // giá trị điền vào ô tìm kiếm
                })
                .Take(10)
                .ToList();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                roleManager.Create(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            var item = db.Roles.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                roleManager.Update(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            var role = db.Roles.Find(id);
            if (role != null && role.Name != "Admin") // Không cho xoá quyền Admin
            {
                db.Roles.Remove(role);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Không thể xoá quyền hoặc quyền không tồn tại." });
        }
        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var idList = ids.Split(',');
                foreach (var id in idList)
                {
                    var role = db.Roles.Find(id);
                    if (role != null && role.Name != "Admin") // Bảo vệ quyền Admin
                    {
                        db.Roles.Remove(role);
                    }
                }
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Danh sách quyền cần xoá rỗng." });
        }

    }
}