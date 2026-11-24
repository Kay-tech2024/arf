using ARF.Core;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using ARF.Web.App_Start.Code;
using ARF.Web.App_Start.DWF;
using ARF.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [CustomAuthorize]
    public class FormController : BaseController
    {
        [HttpGet]
        public ActionResult Index(FormType type, int? step, long? id)
        {
            if (!(id > 0)) id = 0;
            if (!(step > 0)) step = 1;

            var request = (Request)null;
            var form = (Form)null;
            var formKey = (string)null;
            var announcement = (Announcement)null;

            var mustBeOfRoles = new List<ArfUserRole>();
            var mustHaveCurrentAnnouncement = false;

            switch (type)
            {
                case FormType.ArabicFundingRequest:
                case FormType.IslamicFundingRequest:
                    mustBeOfRoles = new List<ArfUserRole> { ArfUserRole.PrincipalResearcher };
                    mustHaveCurrentAnnouncement = true;
                    formKey = "Islamic&ArabicForm";
                    break;

                case FormType.ScientificFundingRequest:
                    mustBeOfRoles = new List<ArfUserRole> { ArfUserRole.PrincipalResearcher };
                    mustHaveCurrentAnnouncement = true;
                    formKey = "scientificForm";
                    break;

                case FormType.SocialScienceFundingRequest:
                    mustBeOfRoles = new List<ArfUserRole> { ArfUserRole.PrincipalResearcher };
                    mustHaveCurrentAnnouncement = true;
                    formKey = "SocialForm";
                    break;
            }

            if (formKey == null)
            {
                TempData["message"] = LangHelpers.Instance["unknown_page"];
                TempData["message_type"] = "danger";
                return RedirectToAction("Index", "Home");
            }

            if (mustBeOfRoles.Count > 0 && !Utils.HasRolesAny(mustBeOfRoles.ToArray()))
            {
                TempData["message"] = LangHelpers.Instance["access_denied"];
                TempData["message_type"] = "danger";
                return RedirectToAction("Index", "Home");
            }

            if (mustHaveCurrentAnnouncement)
            {
                announcement = Utils.GetActiveAnnouncement();

                if (announcement == null)
                {
                    TempData["message"] = LangHelpers.Instance["no_active_announcement"];
                    TempData["message_type"] = "danger";
                    return RedirectToAction("Index", "Home");
                }
            }

            // getting the form that must have at least 1 section with 1 field
            form = DbContext.Forms.FirstOrDefault(f => f.Key == formKey && f.IsActive == true && f.FormSections.Count(s => s.IsActive && s.FormFields.Count(x => x.IsActive) > 0) > 0);

            if (form == null)
            {
                TempData["message"] = LangHelpers.Instance["unknown_page"];
                TempData["message_type"] = "danger";

                return RedirectToAction("Index", "Home");
            }

            var activeSections = form.FormSections
                .Where(s => s.IsActive && s.FormFields.Count(x => x.IsActive) > 0)
                .OrderBy(s => s.Key)
                .ThenBy(s => s.Id)
                .ToList();

            // getting the request

            if (!(id > 0))
            {
                switch (type)
                {
                    case FormType.ArabicFundingRequest:
                        {
                            var otherPendingRequest = DbContext.Requests
                               .Where(r => r.CreatedBy == CurrentUser.Id)
                               .Where(r => r.FieldType == (byte)ReqFieldType.ArabicLanguageField)
                               .Where(r => r.Status == (long)RequestStatus.NotSubmitted)
                               .Where(r => r.AnnouncementId == announcement.Id)
                               .OrderByDescending(r => r.Created)
                               .FirstOrDefault();

                            if (otherPendingRequest != null)
                            {
                                id = otherPendingRequest.Id;
                                request = otherPendingRequest;
                            }
                        }
                        break;

                    case FormType.IslamicFundingRequest:
                        {
                            var otherPendingRequest = DbContext.Requests
                               .Where(r => r.CreatedBy == CurrentUser.Id)
                               .Where(r => r.FieldType == (byte)ReqFieldType.IslamicField)
                               .Where(r => r.Status == (long)RequestStatus.NotSubmitted)
                               .Where(r => r.AnnouncementId == announcement.Id)
                               .OrderByDescending(r => r.Created)
                               .FirstOrDefault();

                            if (otherPendingRequest != null)
                            {
                                id = otherPendingRequest.Id;
                                request = otherPendingRequest;
                            }
                        }
                        break;

                    case FormType.ScientificFundingRequest:
                        {
                            var otherPendingRequest = DbContext.Requests
                               .Where(r => r.CreatedBy == CurrentUser.Id)
                               .Where(r => r.FieldType == (byte)ReqFieldType.AppliedScienceField)
                               .Where(r => r.Status == (long)RequestStatus.NotSubmitted)
                               .Where(r => r.AnnouncementId == announcement.Id)
                               .OrderByDescending(r => r.Created)
                               .FirstOrDefault();

                            if (otherPendingRequest != null)
                            {
                                id = otherPendingRequest.Id;
                                request = otherPendingRequest;
                            }
                        }
                        break;

                    case FormType.SocialScienceFundingRequest:
                        {
                            var otherPendingRequest = DbContext.Requests
                                .Where(r => r.CreatedBy == CurrentUser.Id)
                                .Where(r => r.FieldType == (byte)ReqFieldType.SocialField)
                                .Where(r => r.Status == (long)RequestStatus.NotSubmitted)
                                .Where(r => r.AnnouncementId == announcement.Id)
                                .OrderByDescending(r => r.Created)
                                .FirstOrDefault();

                            if (otherPendingRequest != null)
                            {
                                id = otherPendingRequest.Id;
                                request = otherPendingRequest;
                            }
                        }
                        break;
                }
            }

            if (id > 0)
            {
                request = RequestUtils.GetRequests().FirstOrDefault(r => r.Id == id);

                if (request == null)
                {
                    TempData["message"] = LangHelpers.Instance["unknown_request"] + id;
                    TempData["message_type"] = "danger";

                    return RedirectToAction("Index", "Home");
                }


                // making sure that previous sections have been filled
                if (step > 1)
                {
                    var currentlyActivesSection = request.GetDataValue<int>("CURRENT_STEP");

                    if (currentlyActivesSection > 1 && step > currentlyActivesSection)
                    {
                        step = currentlyActivesSection;
                    }

                    if (step > activeSections.Count)
                    {
                        step = activeSections.Count;
                    }
                }
            }

            var section = activeSections[step.Value - 1];

            var model = new FormFillModel
            {
                FormId = form.Id,
                FormType = type,
                IsLTR = section.IsLtr,
                RequestId = id,
                SectionId = section.Id,
                Step = step.Value,
                StepCount = activeSections.Count,
                StepTitles = activeSections.Select(s => s.LocalizedName()).ToList(),
                Fields = section.FormFields.Where(f => f.IsActive).OrderBy(f => f.Order).ThenBy(f => f.Id).Select(f => new FormFieldModel
                {
                    Id = f.Id,
                    FormId = f.FormId,
                    LookupId = f.LookupId,
                    SectionId = f.SectionId,
                    Key = f.Key,
                    Name = IsArabic ? f.NameArabic : f.NameEnglish,
                    IsLTR = f.IsLtr,

                    FieldType = (FormFieldType)f.FieldType,
                    IsRequired = f.IsRequired,
                    IsMultipleSelection = f.IsMultipleSelection,
                    IsActive = f.IsActive,
                    MinValue = f.MinValue == null ? (int?)null : int.Parse(f.MinValue),
                    MaxValue = f.MaxValue == null ? (int?)null : int.Parse(f.MaxValue),
                    DependsOnFieldId = f.DependsOnFieldId,
                    DependsOnFieldValue = f.DependsOnFieldValue,
                    VisibleToRoles = f.VisibleToRoles,
                    InvisibleToRoles = f.InvisibleToRoles,
                    Order = f.Order,
                    IsReadOnly = f.IsReadOnly,
                    Lookups = f.LookupId > 0 ? f.Lookup.LookupValues.Select(v => new SelectListItem
                    {
                        Value = v.Id + "",
                        Text = v.LocalizedName()
                    }).ToList() : null
                }).ToList(),
                FieldData = request == null ? new List<FieldData>() : request.RequestDatas.Where(f => f.Name.StartsWith("FIELD_")).Select(f => new FieldData
                {
                    FieldId = f.Name.Substring("FIELD_".Length),
                    FieldValue = f.Value,
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormType type, int? step, long? id, FormFillModel model)
        {
            var requiredFiledList = new string[] { "RequestCollegeId", "ThetitleoftheproposedprojectinArabic", "Proposedduration(months)", "Proposedbudget(Riyals)", "StartDate", "ProjectStartDate" };

            if (model == null)
            {
                model = new FormFillModel();
            }

            if (model.FieldData == null)
            {
                model.FieldData = new List<FieldData>();
            }

            var result = Index(type, step, id);
            if (result is RedirectResult || result is RedirectToRouteResult)
            {
                return result;
            }

            var model2 = (FormFillModel)((ViewResult)result).Model;

            foreach (var f in model2.Fields)
            {
                if (f.Key == "Proposedbudget(Riyals)")
                {
                }

                var isValid = true;
                var fval = model.FieldData.FirstOrDefault(f2 => f2.FieldId == f.Id.ToString());
                if (fval == null || string.IsNullOrWhiteSpace(fval.FieldValue) && f.IsRequired && !f.IsReadOnly)
                {
                    if (f.DependsOnFieldId == null)
                    {
                        if (requiredFiledList.Count(x => x == f.Key) > 0)
                        {
                            ModelState.AddModelError("FIELD_" + f.Id, string.Format(Resources.ArfStrings.this_field + "{0}" + Resources.ArfStrings.is_required, f.LocalizedName()));
                            isValid = false;
                        }
                    }
                }
                else if (fval != null && !string.IsNullOrWhiteSpace(fval.FieldValue))
                {
                    switch ((FormFieldType)f.FieldType)
                    {
                        case FormFieldType.Integer:
                            {
                                int a = 0;
                                if (!int.TryParse(fval.FieldValue, out a) || (f.MinValue != null && a < f.MinValue) || (f.MaxValue != null && a > f.MaxValue))
                                {
                                    ModelState.AddModelError("FIELD_" + f.Id, string.Format(Resources.ArfStrings.this_field+"{0}"+Resources.ArfStrings.is_required, f.LocalizedName()));
                                    isValid = false;
                                }
                            }
                            break;

                        case FormFieldType.Email:
                            {
                                if (!Utils.ValidateEmail(fval.FieldValue))
                                {
                                    ModelState.AddModelError("FIELD_" + f.Id, string.Format(Resources.ArfStrings.this_field+"{ 0}"+Resources.ArfStrings.is_incorrect, f.LocalizedName()));
                                    isValid = false;
                                }
                            }
                            break;

                        case FormFieldType.Phone:
                            {
                                if (!Utils.ValidateMobile(fval.FieldValue))
                                {
                                    ModelState.AddModelError("FIELD_" + f.Id, string.Format(Resources.ArfStrings.this_field + "{ 0}" + Resources.ArfStrings.is_incorrect, f.LocalizedName()));
                                    isValid = false;
                                }
                            }
                            break;

                        case FormFieldType.DateTime:
                        case FormFieldType.Date:
                            {
                                if (Utils.StringToDate(fval.FieldValue) == null)
                                {
                                    ModelState.AddModelError("FIELD_" + f.Id, string.Format(Resources.ArfStrings.this_field + "{ 0}" + Resources.ArfStrings.is_incorrect, f.LocalizedName()));
                                    isValid = false;
                                }
                            }
                            break;
                    }
                }

                if (fval != null)
                    fval.IsValid = isValid;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var ann = Utils.GetActiveAnnouncement();
                    var title = model.FieldData.FirstOrDefault(f => f.Fieldkey == "ThetitleoftheproposedprojectinArabic");
                    var college = model.FieldData.FirstOrDefault(f => f.Fieldkey == "RequestCollegeId");
                    var duration = model.FieldData.FirstOrDefault(f => f.Fieldkey == "Proposedduration(months)");
                    var amount = model.FieldData.FirstOrDefault(f => f.Fieldkey == "Proposedbudget(Riyals)");
                    var startDate = model.FieldData.FirstOrDefault(f => f.Fieldkey == "StartDate" || f.Fieldkey == "ProjectStartDate");

                    var field = ReqFieldType.Unspecified;

                    switch (model2.FormType)
                    {
                        case FormType.ArabicFundingRequest:
                            field = ReqFieldType.ArabicLanguageField;
                            break;

                        case FormType.IslamicFundingRequest:
                            field = ReqFieldType.IslamicField;
                            break;

                        case FormType.ScientificFundingRequest:
                            field = ReqFieldType.AppliedScienceField;
                            break;

                        case FormType.SocialScienceFundingRequest:
                            field = ReqFieldType.SocialField;
                            break;
                    }

                    var request = (Request)null;

                    if (model2.RequestId > 0)
                    {
                        request = DbContext.Requests.FirstOrDefault(f => f.Id == model2.RequestId);
                    }
                    else if (model2.Step == 1)
                    {
                        request = new Request
                        {
                            Created = TM.Now,
                            Updated = TM.Now,
                            CreatedBy = CurrentUser.Id,
                            UpdatedBy = CurrentUser.Id
                        };

                        DbContext.Requests.Add(request);
                    }

                    if (model2.Step == 1)
                    {

                        request.Type = (byte)Data.RequestType.FundingRequest;
                        request.StartDate = ann.StartDateGreg;
                        request.FieldType = (byte)field;
                        request.IsScientificProduction = model2.FormType == FormType.ScientificFundingRequest;
                        request.Status = (long)RequestStatus.NotSubmitted;
                        request.Title = title == null ? null : title.FieldValue;
                        request.CollegeId = college == null ? (int?)null : int.Parse(college.FieldValue);
                        request.ProjectStartDate = startDate == null ? (DateTime?)null : Utils.StringToDate(startDate.FieldValue);
                        request.ProjectEndDate = request.ProjectStartDate == null || duration == null ? (DateTime?)null : request.StartDate.Value.AddMonths(int.Parse(duration.FieldValue));
                        request.Duration = duration == null ? (int?)null : int.Parse(duration.FieldValue);
                        request.AmountSar = amount == null ? (double?)null : double.Parse(amount.FieldValue);
                        request.AnnouncementId = ann.Id;

                    }

                    DbContext.SaveChanges();

                    foreach (var fval in model.FieldData.Where(f => f.IsValid && f.FieldValue != null))
                    {
                        var x = request.SetDataValue(long.Parse(fval.FieldId), fval.FieldValue, true);
                    }

                    if (step == model2.StepCount && request.Type == (byte)Data.RequestType.FundingRequest)
                    {
                        request.Status = (long)RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies;

                        request.RequestHistoryLogs.Add(new RequestHistoryLog
                        {
                            IpAddress = Utils.GetUserIP(),
                            UserAgent = Request.UserAgent,
                            Url = Utils.GetFullUrl(),
                            Action = (byte)Data.RequestAction.Submit,
                            Created = TM.Now,
                            CreatedBy = CurrentUser.Id
                        });
                    }

                    var stepData = request.SetDataValue("CURRENT_STEP", (step + 1) + "", true);

                    DbContext.SaveChanges();

                    if (step == model2.StepCount)
                    {
                        TempData["message"] = Resources.ArfStrings.request_sent_successfully;
                        TempData["message_type"] = "success";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["message"] = Resources.ArfStrings.data_saved_successfully;
                        TempData["message_type"] = "success";
                        return Redirect(Utils.GetFormSectionLink(type, step + 1, request.Id));
                    }
                }
                catch (Exception ex)
                {
                    TempData["message"] = Resources.ArfStrings.operation_failed + ex.SanitizeException() + ")";
                    TempData["message_type"] = "danger";
                }
            }

            if (model.RequestId == null)
            {
                model.RequestId = model2.RequestId;
            }

            model.IsLTR = model2.IsLTR;
            model.FormType = model2.FormType;
            model.RequestId = model2.RequestId;
            model.FormId = model2.FormId;
            model.SectionId = model2.SectionId;
            model.Step = model2.Step;
            model.StepCount = model2.StepCount;
            model.StepTitles = model2.StepTitles;
            model.Fields = model2.Fields;

            return View(model);
        }
    }
}