using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARF_Portal.Controllers
{
    public class FormsViewsController : Controller
    {
        // GET: FormsViews
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IslamicScience()
        {
            return View();
        }
        public ActionResult ArabicLangScience()
        {
            return View();
        }
        public ActionResult HumanSocietyScience()
        {
            return View();
        }
        public ActionResult AppliedScience()
        {
            return View();
        }
    }
}