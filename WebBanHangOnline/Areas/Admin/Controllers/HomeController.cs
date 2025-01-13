using System.Web.Mvc;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            ViewBag.Message = "Bạn không có quyền truy cập vào chức năng này.";
            return View();
        }
    }
}