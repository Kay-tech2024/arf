using ARF.Core;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.Web.App_Start.Code;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [CustomAuthorize(ArfUserRole.SystemAdmin, ArfUserRole.DeanofDeanshipofScientificResearch)]
    public class ReportsController : BaseController
    {
        public ActionResult Types()
        {
            List<ViewRequestsByTypeReport> model = Utils.db.Database.SqlQuery<ViewRequestsByTypeReport>("select * from ViewRequestsByTypeReport").ToList();

            return View(model);
        }

        public ActionResult Financial()
        {
            List<ViewRequestsByFinancialTypesReport> model = Utils.db.Database.SqlQuery<ViewRequestsByFinancialTypesReport>("select * from ViewRequestsByFinancialTypesReport").ToList();
            return View(model);
        }

        public ActionResult FinancialResearchTeam() //ViewRequestsByResearchTeamTypesReport
        {
            List<ViewRequestsByResearchTeamTypesReport> model = Utils.db.Database.SqlQuery<ViewRequestsByResearchTeamTypesReport>("select * from ViewRequestsByResearchTeamTypesReport").ToList();
            return View(model);
        }

        public ActionResult Materials()
        {
            List<ViewMaterialRequest> model = Utils.db.Database.SqlQuery<ViewMaterialRequest>("select * from ViewMaterialRequests").ToList();
            return View(model);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(short? ReportNo, short? RequestStatus, short? FieldType, short? UserRole, short? CollegeId)
        {
            if (ReportNo == 3)
            {
                ViewBag.IsMaterial = 1;
            }
            else
            {
                ViewBag.IsMaterial = 0;
            }

            return View();
        }
    }
}