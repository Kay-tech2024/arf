using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ARF.Core.Models;
using ARF.Data.DbModels;
using ARF.Web.Models;

namespace ARF.Web.Controllers
{
    public class QuantitativeController : BaseController
    {
        
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RequestIslamicAndOtherJudgementForm RislamJud)
        {
           try
            {
                RislamJud.Id = Guid.NewGuid();
                DbContext.RequestIslamicAndOtherJudgementForms.Add(RislamJud);
                DbContext.SaveChanges();
                return View();
            }
            catch(Exception ex)
            {
                return View();
            }

         
        }
    }
}