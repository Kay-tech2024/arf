using ARF.Core;
using ARF.Web.App_Start.Code;
using EntityFramework.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [CustomAuthorize]
    public class TimeMachineController : BaseController
    {
        public ActionResult Index(int? yy, int? mo, int? dd, int? mm, int? hh, bool? IsSystemWide)
        {
            if (!Utils.TimeMachineEnabled)
            {
                return Content("تم تعطيل هذه الخاصية لأسباب أمنية");
            }

            if (yy > 0 && mo > 0 && dd > 0 && mo >= 0 && hh >= 0)
            {

                try
                {
                    IsSystemWide = true;
                    var date = new DateTime(yy.Value, mo.Value, dd.Value, hh.Value, mm.Value, 0);

                    if (IsSystemWide == null) IsSystemWide = false;

                    TimeMachine.TmUtils.IsSystemWide = IsSystemWide.Value;
                    TimeMachine.TmUtils.CurrentDateTimeStatic = date;

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                }
            }


            ViewBag.yy = TimeMachine.TmUtils.CurrentDateTimeStatic.Year;
            ViewBag.mo = TimeMachine.TmUtils.CurrentDateTimeStatic.Month;
            ViewBag.dd = TimeMachine.TmUtils.CurrentDateTimeStatic.Day;

            ViewBag.hh = TimeMachine.TmUtils.CurrentDateTimeStatic.Hour;
            ViewBag.mm = TimeMachine.TmUtils.CurrentDateTimeStatic.Minute;
            ViewBag.IsSystemWide = DbContext.Settings.Count(x => x.Key == "TIME") > 0;

            return View();
        }

        public ActionResult Reset()
        {
            if (!Utils.TimeMachineEnabled)
            {
                return Content("تم تعطيل هذه الخاصية لأسباب أمنية");
            }


            TimeMachine.TmUtils.CurrentDateTimeStatic = DateTime.Now;

            DbContext.UserPreferences
                .Where(up => up.Key.StartsWith("TIME"))
                .Delete();

            DbContext.Settings.Where(x => x.Key == "TIME").Delete();

            DbContext.SaveChanges();

            return RedirectToAction("Index");

        }
    }
}