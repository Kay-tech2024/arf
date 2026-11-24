using ARF.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ARF.Web.Controllers
{
    public class CoResearcherController : Controller
    {
        public ActionResult Encyclopedia()
        {
            //SC016
            ViewBag.Majors = new SelectList("", "");
            return View();
        }

        public ActionResult Dictionary()
        {
            //SC017
            
            ViewBag.Majors = new SelectList("","");
            return View();
        }

        public ActionResult ScientificLetterInformation()
        {
            var model = new ScientificLetterInformationViewModel();
            return View(model);
        }
    }
}