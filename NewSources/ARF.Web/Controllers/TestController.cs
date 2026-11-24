using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [AllowAnonymous]
    public class TestController : BaseController
    {
        public ActionResult Index(int? t)
        {
            return View();
        }
    }
}