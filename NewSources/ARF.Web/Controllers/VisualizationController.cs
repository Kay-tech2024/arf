using ARF.Core;
using ARF.Data.DbModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    public class VisualizationController : BaseController
    {
        private ArfDbEntities db = Utils.db;

        [HttpGet]
        public ActionResult Index()
        {
            var model = db.WorkFlows.Where(e => e.IsActive).OrderBy(e => e.NameEnglish).ToList();

            return View(model);
        }

        public ActionResult WorkFlow(int id)
        {
            var list = db.WorkflowStateActions.Where(e => e.State.WorkFlow.Id == id && e.IsActive && e.State.IsActive).ToList();
            var first = new WorkflowStateAction();

            var list2 = new List<WorkflowStateAction>();
            list2 = list.ToList();

            foreach (var item in list)
            {
                if (list.Where(e => e.NextStateId == item.State.Id).Count() > 0)
                {
                }
                else
                {
                    first = item;
                    list2.Remove(first);
                    break;
                }
            }

            var availableactions = db.WorkflowStateActions.Where(e => e.StateId == first.StateId && e.IsActive).ToList();

            list.Clear();
            list.Add(first);
            list.AddRange(list2);

            ViewBag.First = true;
            ViewBag.BackKey = id;
            return View("ViewTransition", availableactions);
        }

        public ActionResult FundingRequestWorkflow()
        {
            var list = db.WorkflowStateActions.Where(e => e.State.WorkFlow.Key == "FundingRequestWorkflow").ToList();
            var first = new WorkflowStateAction();
            foreach (var item in list)
            {
                if (list.Where(e => e.NextStateId == item.Id).Count() > 0)
                {
                }
                else
                {
                    first = item;
                    break;
                }
            }

            list.Clear();
            list.Add(first);

            ViewBag.First = true;
            ViewBag.BackKey = "FundingRequestWorkflow";
            return View("ViewTransition", list);
        }

        public ActionResult AdverisementLetterRequestWorkflow()
        {
            var list = db.WorkflowStateActions.Where(e => e.State.WorkFlow.Key == "AdverisementLetterRequestWorkflow").ToList();
            var first = new WorkflowStateAction();
            foreach (var item in list)
            {
                if (list.Where(e => e.NextStateId == item.Id).Count() > 0)
                {
                }
                else
                {
                    first = item;
                    break;
                }
            }

            list.Clear();
            list.Add(first);
            ViewBag.First = true;
            ViewBag.BackKey = "AdverisementLetterRequestWorkflow";
            return View("ViewTransition", list);
        }

        public ActionResult DeansLetterRequestWorkflow()
        {
            var list = db.WorkflowStateActions.Where(e => e.State.WorkFlow.Key == "DeansLetterRequestWorkflow").ToList();
            var first = new WorkflowStateAction();
            foreach (var item in list)
            {
                if (list.Where(e => e.NextStateId == item.Id).Count() > 0)
                {
                }
                else
                {
                    first = item;
                    break;
                }
            }

            list.Clear();
            list.Add(first);

            ViewBag.First = true;
            ViewBag.BackKey = "DeansLetterRequestWorkflow";
            return View("ViewTransition", list);
        }

        public ActionResult DesignRequestWorkflow()
        {
            var list = db.WorkflowStateActions.Where(e => e.State.WorkFlow.Key == "DesignRequestWorkflow").ToList();
            var first = new WorkflowStateAction();
            foreach (var item in list)
            {
                if (list.Where(e => e.NextStateId == item.Id).Count() > 0)
                {
                }
                else
                {
                    first = item;
                    break;
                }
            }

            list.Clear();
            list.Add(first);
            ViewBag.First = true;
            ViewBag.BackKey = "DesignRequestWorkflow";
            return View("ViewTransition", list);
        }

        public ActionResult FinalTermRequestWorkflow()
        {
            var list = db.WorkflowStateActions.Where(e => e.State.WorkFlow.Key == "FinalTermRequestWorkflow").ToList();
            var first = new WorkflowStateAction();
            foreach (var item in list)
            {
                if (list.Where(e => e.NextStateId == item.Id).Count() > 0)
                {
                }
                else
                {
                    first = item;
                    break;
                }
            }

            list.Clear();
            list.Add(first);
            ViewBag.First = true;
            ViewBag.BackKey = "FinalTermRequestWorkflow";
            return View("ViewTransition", list);
        }

        public ActionResult MaterialRequestWorkflow()
        {
            var list = db.WorkflowStateActions.Where(e => e.State.WorkFlow.Key == "MaterialRequestWorkflow").ToList();
            var first = new WorkflowStateAction();
            foreach (var item in list)
            {
                if (list.Where(e => e.NextStateId == item.Id).Count() > 0)
                {
                }
                else
                {
                    first = item;
                    break;
                }
            }

            list.Clear();
            list.Add(first);
            ViewBag.First = true;
            ViewBag.BackKey = "MaterialRequestWorkflow";
            return View("ViewTransition", list);
        }

        public ActionResult MidTermRequestWorkflow()
        {
            var list = db.WorkflowStateActions.Where(e => e.State.WorkFlow.Key == "MidTermRequestWorkflow").ToList();
            var first = new WorkflowStateAction();
            foreach (var item in list)
            {
                if (list.Where(e => e.NextStateId == item.Id).Count() > 0)
                {
                }
                else
                {
                    first = item;
                    break;
                }
            }

            list.Clear();
            list.Add(first);
            ViewBag.First = true;
            ViewBag.BackKey = "MidTermRequestWorkflow";
            return View("ViewTransition", list);
        }

        public ActionResult NewspaperLetterRequestWorkflow()
        {
            var list = db.WorkflowStateActions.Where(e => e.State.WorkFlow.Key == "NewspaperLetterRequestWorkflow").ToList();
            var first = new WorkflowStateAction();
            foreach (var item in list)
            {
                if (list.Where(e => e.NextStateId == item.Id).Count() > 0)
                {
                }
                else
                {
                    first = item;
                    break;
                }
            }
            ViewBag.First = true;
            ViewBag.BackKey = "NewspaperLetterRequestWorkflow";
            list.Clear();
            list.Add(first);

            return View("ViewTransition", list);
        }

        public ActionResult RefereeRewardWorkflow()
        {
            var list = db.WorkflowStateActions.Where(e => e.State.WorkFlow.Key == "RefereeRewardWorkflow").ToList();
            var first = new WorkflowStateAction();
            foreach (var item in list)
            {
                if (list.Where(e => e.NextStateId == item.Id).Count() > 0)
                {
                }
                else
                {
                    first = item;
                    break;
                }
            }
            ViewBag.First = true;
            ViewBag.BackKey = "RefereeRewardWorkflow";
            list.Clear();
            list.Add(first);

            return View("ViewTransition", list);
        }

        public JsonResult GetTransitons(long nextstate, string role)
        {
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewTransition(long? Id)
        {
            if (Id == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.First = false;
            var model = db.WorkflowStateActions.Where(e => e.StateId == Id).ToList();
            return View(model);
        }
    }
}