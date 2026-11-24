using ARF.Core;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    public class WorkFlowManipulationController : BaseController
    {
        ArfDbEntities db = Utils.db;

        [HttpGet]
        public ActionResult Index()
        {
            var model = db.WorkFlows.OrderBy(e => e.NameEnglish).ToList();

            ViewBag.RequestTypes = getRequestTypes();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int? WorkFlowId, string NameArabic, string NameEnglish, bool? IsActive, int RequestTypeId)
        {

            if (WorkFlowId < 1)
            {
                //new Form
                db.WorkFlows.Add(new WorkFlow()
                {
                    Key = NameEnglish.Trim(' '),
                    NameArabic = NameArabic,
                    NameEnglish = NameEnglish,
                    IsActive = (bool)IsActive,
                    RequestTypeId = RequestTypeId
                });
            }
            else
            {
                var workflow = db.WorkFlows.Where(e => e.Id == WorkFlowId).FirstOrDefault();

                workflow.NameArabic = NameArabic;
                workflow.NameEnglish = NameEnglish;
                workflow.IsActive = (bool)IsActive;
                workflow.RequestTypeId = RequestTypeId;
            }
            //TempData["message"] = "تم إرسال الطلب بنجاح";
            //TempData["message_type"] = "success";
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["message"] = e.SanitizeException();
                TempData["message_type"] = "danger";

            }
            ViewBag.RequestTypes = getRequestTypes();

            var model = db.WorkFlows.OrderBy(e => e.NameEnglish).ToList();

            return View(model);

        }

        [HttpGet]
        public ActionResult ViewStates(int id)
        {
            var model = db.WorkflowStates.Where(e => e.WorkflowId == id).ToList();
            ViewBag.WorkFlowId = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewStates(long? WorkFlowId, int? Id, string NameArabic, string NameEnglish, bool? IsActive)
        {
            if (Id < 1 || Id == null)
            {
                //new state
                db.WorkflowStates.Add(new WorkflowState()
                {
                    Key = NameEnglish.Trim(' '),
                    NameArabic = NameArabic,
                    NameEnglish = NameEnglish,
                    IsActive = (bool)IsActive,
                    WorkflowId = (long)WorkFlowId
                });
            }
            else
            {
                var state = db.WorkflowStates.Where(e => e.Id == Id).FirstOrDefault();

                state.NameArabic = NameArabic;
                state.NameEnglish = NameEnglish;
                state.IsActive = (bool)IsActive;
            }

            db.SaveChanges();
            ViewBag.WorkFlowId = WorkFlowId;

            var model = db.WorkflowStates.Where(e => e.WorkflowId == WorkFlowId).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult ViewStateActions(int id, long? WorkFlowId)
        {
            //id, stateid, roleid, actionid, formid, isactive, formscope, nextstateid
            var stateactions = db.WorkflowStateActions.Where(e => e.StateId == id && e.IsActive).ToList();
            if (stateactions.Count < 1)
            {
                stateactions.Add(new WorkflowStateAction()
                {
                    Id = -1
                });
            }
            var model = new StateActionEditModel()
            {
                StateActions = stateactions,
            };
            ViewBag.Roles = getRoles();
            ViewBag.Forms = getForms();
            ViewBag.RequestAction = getActions();
            ViewBag.States = getStates((long)WorkFlowId);
            ViewBag.WorkFlowName = db.WorkFlows.Where(e => e.Id == WorkFlowId).Select(e => e.NameArabic).FirstOrDefault();
            ViewBag.StateName = db.WorkflowStates.Where(e => e.Id == id).Select(e => e.NameArabic).FirstOrDefault();
            ViewBag.StateId = id;
            ViewBag.WorkFlowId = WorkFlowId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewStateActions(StateActionEditModel s, int id, long? WorkFlowId)
        {
            //modelstate.addmodelerror
            for (int i = 0; i < s.StateActions.Count; i++)
            {
                if (s.StateActions[i].Id == -1)
                {
                    continue;
                }
                if (s.StateActions[i].RoleId == null)
                {
                    ModelState.AddModelError("StateActions[" + i + "].RoleId", "Role is required");
                }
                else if (s.StateActions[i].Id == 0)
                {
                    //new action
                    //Id StateId RoleId ActionId FormId FormScope NextStateId IsActive
                    db.WorkflowStateActions.Add(new WorkflowStateAction()
                    {
                        FormId = s.StateActions[i].FormId,
                        StateId = s.StateActions[i].StateId,
                        RoleId = s.StateActions[i].RoleId,
                        ActionId = s.StateActions[i].ActionId,
                        FormScope = s.StateActions[i].FormScope,
                        NextStateId = s.StateActions[i].NextStateId,
                        IsActive = s.StateActions[i].IsActive
                    });
                }
                else
                {
                    //edit action
                    var itemid = s.StateActions[i].Id;
                    var a = db.WorkflowStateActions.Where(e => e.Id == itemid).FirstOrDefault();
                    a.FormId = s.StateActions[i].FormId;
                    a.RoleId = s.StateActions[i].RoleId;
                    a.ActionId = s.StateActions[i].ActionId;
                    a.FormScope = s.StateActions[i].FormScope;
                    a.NextStateId = s.StateActions[i].NextStateId;
                    a.IsActive = s.StateActions[i].IsActive;
                }
                db.SaveChanges();
            }

            var model = new StateActionEditModel()
            {
                StateActions = s.StateActions.Where(e => e.IsActive).ToList()
            };

            ViewBag.Roles = getRoles();
            ViewBag.Forms = getForms();
            ViewBag.RequestAction = getActions();
            ViewBag.States = getStates((long)WorkFlowId);
            ViewBag.WorkFlowName = db.WorkFlows.Where(e => e.Id == WorkFlowId).Select(e => e.NameArabic).FirstOrDefault();
            ViewBag.StateName = db.WorkflowStates.Where(e => e.Id == id).Select(e => e.NameArabic).FirstOrDefault();
            ViewBag.StateId = id;
            ViewBag.WorkFlowId = WorkFlowId;

            return View(model);
        }


        public List<SelectListItem> getRequestTypes()
        {
            return null;
        }

        public List<SelectListItem> getRoles()
        {
            var le = db.Roles.ToList().Select(e => new SelectListItem { Text = e.LocalizedName(), Value = e.Id.ToString() }).ToList();
            return le;
        }

        public List<SelectListItem> getForms()
        {
            var le = db.Forms.ToList().Select(e => new SelectListItem { Text = e.LocalizedName(), Value = e.Id.ToString() }).ToList();

            return le;
        }

        public List<SelectListItem> getActions()
        {
            return null;
        }

        public List<SelectListItem> getStates(long WorkFlowId)
        {
            var le = db.WorkflowStates.Where(e => e.WorkflowId == WorkFlowId).ToList().Select(e => new SelectListItem { Text = e.LocalizedName(), Value = e.Id.ToString() }).ToList();

            return le;
        }

    }
}