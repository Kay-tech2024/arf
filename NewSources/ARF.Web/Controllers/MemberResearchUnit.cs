using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    public class MemberResearchUnitController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FundingRequests()
        {
            return View();
        }

        public ActionResult ViewRequest()
        {
            return View();
        }
    }
}