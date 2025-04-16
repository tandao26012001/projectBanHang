using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Admin/Account
        public ActionResult Index(string searchText)
        {
            var items = db.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                items = items.Where(x => x.FullName.Contains(searchText) || x.UserName.Contains(searchText));
            }

            ViewBag.SearchText = searchText; // để giữ giá trị khi submit
            return View(items.OrderByDescending(x => x.Id).ToList());
        }
        [HttpGet]
        public JsonResult GetSuggestions(string term)
        {
            var suggestions = db.Users
                .Where(p => p.FullName.Contains(term) || p.UserName.Contains(term))
                .Select(p => new
                {
                    label = p.FullName + " (" + p.UserName + ")",
                    value = p.FullName // giá trị điền vào ô tìm kiếm
                })
                .Take(10)
                .ToList();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Xóa thông tin đăng nhập trước đó nếu có
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            // Thực hiện đăng nhập
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    // Người dùng đăng nhập thành công
                    var user = await UserManager.FindByNameAsync(model.UserName);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Người dùng không tồn tại.");
                        return View(model);
                    }

                    // Nếu muốn chỉ cho phép Admin hoặc Employee truy cập hệ thống, kiểm tra vai trò tại đây
                    var roles = await UserManager.GetRolesAsync(user.Id);
                    if (!roles.Contains("Admin") && !roles.Contains("Employee"))
                    {
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        ModelState.AddModelError("", "Tài khoản của bạn không có quyền truy cập vào hệ thống.");
                        return View(model);
                    }

                    // Xử lý returnUrl nếu cần thiết
                    if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                    {
                        returnUrl = Url.Action("Index", "Home");
                    }

                    // Sau đó redirect về trang hợp lệ
                    return RedirectToLocal(returnUrl);


                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                    return View(model);
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Clear();
            return RedirectToAction("Login", "Account");
        }
        //
        // GET: /Account/Register
        public ActionResult Create()
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");

            var model = new CreateAccountViewModel(); // Luôn khởi tạo model
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Image = model.Image,
                    Email = model.Email,
                    FullName = model.FullName,
                    Phone = model.Phone
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.Roles != null)
                    {
                        foreach (var r in model.Roles)
                        {
                            UserManager.AddToRole(user.Id, r);
                        }
                    }

                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }
            var roles = db.Roles?.ToList() ?? new List<IdentityRole>();
            ViewBag.Role = new SelectList(roles, "Name", "Name");
            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult Edit(string id)
        {
            var user = UserManager.FindById(id);
            if (user == null) return HttpNotFound();
            var role = UserManager.GetRoles(user.Id).FirstOrDefault();

            var model = new EditAccountViewModel
            {
                FullName = user.FullName,
                Image = user.Image,
                Email = user.Email,
                Phone = user.Phone,
                UserName = user.UserName,
                Roles = role
            };

            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name", model.Roles);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST
        public async Task<ActionResult> Edit(EditAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name", model.Roles);
                return View(model);
            }

            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user == null) return HttpNotFound();

            user.FullName = model.FullName;
            user.Image = model.Image;
            user.Phone = model.Phone;
            user.Email = model.Email;

            var result = await UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var currentRoles = await UserManager.GetRolesAsync(user.Id);
                await UserManager.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());

                if (!string.IsNullOrEmpty(model.Roles))
                {
                    await UserManager.AddToRoleAsync(user.Id, model.Roles);
                }

                return RedirectToAction("Index");
            }

            AddErrors(result);
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name", model.Roles);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json(new { success = false, message = "ID không hợp lệ." });

            // Không cho người dùng tự xóa chính mình
            if (User.Identity.GetUserId() == id)
                return Json(new { success = false, message = "Bạn không thể tự xoá chính mình." });

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy người dùng." });

            var roles = await UserManager.GetRolesAsync(id);
            if (roles.Any())
                await UserManager.RemoveFromRolesAsync(id, roles.ToArray());

            var result = await UserManager.DeleteAsync(user);
            return Json(new { success = result.Succeeded });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAll(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return Json(new { success = false, message = "Danh sách ID rỗng." });

            var idList = ids.Split(',').ToList();
            string currentUserId = User.Identity.GetUserId();
            bool allDeleted = true;

            foreach (var id in idList)
            {
                // Không cho xóa chính mình
                if (id == currentUserId)
                    continue;

                var user = await UserManager.FindByIdAsync(id);
                if (user != null)
                {
                    var roles = await UserManager.GetRolesAsync(id);
                    if (roles.Any())
                        await UserManager.RemoveFromRolesAsync(id, roles.ToArray());

                    var result = await UserManager.DeleteAsync(user);
                    if (!result.Succeeded)
                        allDeleted = false;
                }
            }

            return Json(new { success = allDeleted, message = allDeleted ? null : "Một số tài khoản không thể xoá." });
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}