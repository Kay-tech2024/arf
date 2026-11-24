using ARF.Core;
using ARF.Core.Models;
using ARF.Core.Workflows;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using ARF.Web.App_Start.Code;
using ARF.Web.App_Start.Extensions;
using ARF.Web.Models;
using ARF.Web.Resources;
using OpenHtmlToPdf;
using PhoneNumbers;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    public class RequestsController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Create(long? id, Data.RequestType? type, ReqFieldType? field)
        {
            if (field == null && id == null)
            {
                return base.View("SelectField");
            }

            if (type == null || type == Data.RequestType.Unspecificed)
            {
                TempData["message"] = "يجب اختيار نوع الطلب";
                TempData["message_type"] = "info";

                return RedirectToAction("Index", "Home");
            }

            var currentAnnouncement = DbContext.Announcements.FirstOrDefault(r => r.StartDateGreg <= DateTime.Now && r.EndDateGreg > DateTime.Now);

            if (currentAnnouncement == null && type == Data.RequestType.FundingRequest)
            {
                TempData["message"] = "لم تبدأ دورة التمويل بعد";
                TempData["message_type"] = "info";

                return RedirectToAction("Index", "Home");
            }

            var otherRequest = (Request)null;
            if (type == Data.RequestType.MidTermProgressReport || type == Data.RequestType.FulltermProgressReport || type == Data.RequestType.MaterialRequest || type == RequestType.EditFundingRequest)
            {
                if (id == null) id = 0;

                otherRequest = DbContext.Requests.Where(r =>
                    r.Id == id.Value &&
                    r.Type == (byte)Data.RequestType.FundingRequest &&
                    r.Status == (long)RequestStatus.Completed

                )
                .OrderByDescending(r => r.Created)
                .FirstOrDefault();
                List<SelectListItem> listItems = new List<SelectListItem>();
                for (int ii = 0; ii <= 100; ii++)
                {
                    listItems.Add(new SelectListItem { Text = ii.ToString(), Value = ii.ToString() });
                }
                ViewBag.dropdownlist = listItems;
            }
            ViewBag.ShowMaterialEditForm = true;
            if (type == RequestType.EditFundingRequest && id != null)
            {
                var request = Utils.db.Requests.Where(x => x.PublicationRequestId == id.Value && x.Type == (byte)RequestType.MaterialRequest).FirstOrDefault();
                if (request != null)
                {
                    ViewBag.ShowMaterialEditForm = false;
                }
                ViewBag.IncludeCKEditor = true;
            }
            if (otherRequest == null)
            {
                TempData["message"] = "لا يوجد لديك مشاريع بحثية";
                TempData["message_type"] = "danger";

                return RedirectToAction("Index", "Home");
            }

            var user = Utils.GetCurrentUserDetails();

            var model = new FundingRequestViewModel
            {
                Name = user.DbUser.UserProfile.GetFullName(),
            };


            if (otherRequest.Announcement != null)
            {
                model.AnnouncementStartDate = otherRequest.Announcement.StartDateHijri;
                model.AnnouncementEndDate = otherRequest.Announcement.EndDateHijri;
            }
            if(type == Data.RequestType.MidTermProgressReport)
            {
                var startdatemonth = otherRequest.ProjectStartDate.Value.Month;
                int duration =(otherRequest.Duration / 2).GetValueOrDefault();
                //startdate.Value.AddMonths(duration);
                var midmonth = startdatemonth + duration;                
               model.Forms= otherRequest.RequestTimelines.Where(x=>x.EndingMonth<=midmonth).Select(x => new Models.ReportForm { CompletionRate = x.CompletionRate, Reasons = x.Reasons, TaskName = x.TaskName, Id=x.Id }).ToList();
            }
            if (type == Data.RequestType.FulltermProgressReport)
            {                
                model.Forms = otherRequest.RequestTimelines.Select(x => new Models.ReportForm { CompletionRate = x.CompletionRate, Reasons = x.Reasons, TaskName = x.TaskName }).ToList();
            }
            model.BudgetSummaryPerPerson = otherRequest.RequestCoresearchers.Where(x => x.IsDeleted == false)
                               .OrderBy(rr => rr.Order)
                               .ToList()
                               .Select(rr => new BudgetPerPersonViewModel
                               {
                                   Name = rr.Researcher.GetFullName(),
                                   ResearcherRole = LangHelpers.Instance[((ResearcherRole?)rr.ResearcherType).ToString()],
                                   IsConsultant = rr.ResearcherType == (byte)ResearcherRole.Consultant,
                                   FirstPeriodActiveMonths = rr.FirstPeriodActiveMonths + "",
                                   SecondPeriodActiveMonths = rr.SecondPeriodActiveMonths + "",
                                   FirstPeriodCost = string.Format("{0:c}", rr.FirstPeriodCost).Replace("$", LangHelpers.Instance["sar"] + " "),
                                   SecondPeriodCost = string.Format("{0:c}", rr.SecondPeriodCost).Replace("$", LangHelpers.Instance["sar"] + " "),
                                   TotalActiveMonths = rr.TotalActiveMonths + "",
                                   TotalCost = string.Format("{0:c}", rr.TotalCost).Replace("$", LangHelpers.Instance["sar"] + " "),
                               })
                               .ToList();
            model.FirstPeriodTotalCost = string.Format("{0:c}", otherRequest.FirstPeriodTotalCost).Replace("$", LangHelpers.Instance["sar"] + " ");
            model.SecondPeriodTotalCost = string.Format("{0:c}", otherRequest.SecondPeriodTotalCost).Replace("$", LangHelpers.Instance["sar"] + " ");
            model.FieldType = field;
            model.PublicationRequestId = id;
            model.UniqueId = otherRequest.UniqueId;
            model.Title = otherRequest.Title;
            model.RequestMaterial = otherRequest.RequestMaterials.ToList();
            model.MaterialsTotalCost = string.Format("{0:c}", otherRequest.MaterialsTotalCost).Replace("$", LangHelpers.Instance["sar"] + " ");
            model.TotalCostForProject = string.Format("{0:c}", otherRequest.TotalCostForProject).Replace("$", LangHelpers.Instance["sar"] + " ");
            model.Files = new List<AttachedFile>();
            if (type.Value == RequestType.MaterialRequest)
            {
                model.Files = otherRequest.RequestFiles.Where(rf => rf.Type == (byte)FileType.RequestMaterialsQuotation1 || rf.Type == (byte)FileType.RequestMaterialsQuotation2 || rf.Type == (byte)FileType.RequestMaterialsQuotation3).Select(c => new AttachedFile
                {
                    FileId = c.Id.ToString(),
                    Type = (FileType)c.Type,
                    FileName = Path.GetFileName(c.FileName),
                    Date = c.Created,
                    FilePath = c.FilePath
                }).ToList();
            }
            var lang = otherRequest.Type == (byte)ReqFieldType.AppliedScienceField ? "en" : null;

            #region Populate Lookups
            var values = Enum.GetValues(typeof(MaterialType));
            var list = new List<BetterSelectListItem>();

            var i = 0;
            foreach (var v in values)
            {
                if (v.ToString() == MaterialType.Unspecified.ToString()) continue;

                list.Add(new BetterSelectListItem
                {
                    Value = v.ToString(),
                    Text = LangHelpers.Instance[v.ToString()]
                });



                i++;
            }

            list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });
            model.MaterialTypes = list.ToArray();

            #endregion

            return View("GenericForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CommonFundingRequestViewModel model, long PublicationRequestId, Data.RequestType? type)
        {
           //return Create(PublicationRequestId, type, null);
            if (type == null || type == Data.RequestType.Unspecificed)
            {
                TempData["message"] = "يجب اختيار نوع الطلب";
                TempData["message_type"] = "info";

                return RedirectToAction("Index", "Home");
            }

            var lang = model.Type == ReqFieldType.AppliedScienceField ? "en" : null;
            var foundFiles = 0;
            var otherRequest = (Request)null;
            if (type == Data.RequestType.MidTermProgressReport || type == Data.RequestType.FulltermProgressReport || type == Data.RequestType.MaterialRequest || type == Data.RequestType.EditFundingRequest)
            {
                otherRequest = DbContext.Requests.Where(r =>
                    r.Id == PublicationRequestId &&
                    r.Type == (byte)Data.RequestType.FundingRequest &&
                    r.Status == (long)RequestStatus.Completed

                )
                .OrderByDescending(r => r.Created)
                .FirstOrDefault();

                if (Request.Files.Count > 0)
                {
                    for (var i = 0; i < Request.Files.Count; i++)
                    {
                        var File = Request.Files[i];
                        if (File != null && File.ContentLength > 0)
                        {
                            foundFiles++;
                        }
                    }
                }

                if (
                    (type == Data.RequestType.MidTermProgressReport && foundFiles < 2) ||
                    (type == Data.RequestType.FulltermProgressReport && foundFiles < 3)
                )
                {
                    TempData["message"] = "لم يتم إرفاق جميع الملفات المطلوبة";
                    TempData["message_type"] = "danger";

                    return RedirectToAction("Create", new { id = model.Id, type = type.ToString() });
                }
            }

            ModelState.Clear();

            if (otherRequest == null)
            {
                TempData["message"] = "لا يوجد لديك مشاريع بحثية";
                TempData["message_type"] = "danger";

                return RedirectToAction("Index", "Home");
            }

            var initialState = (RequestStatus?)null;

            switch (type.Value)
            {
                case Data.RequestType.MidTermProgressReport:
                    initialState = RequestStatus.MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearch;
                    break;

                case Data.RequestType.FulltermProgressReport:
                    initialState = RequestStatus.FinalReportRecievedByVice_of_Deanship_Research_Centers_and_databases;
                    break;

                case Data.RequestType.MaterialRequest:
                    initialState = RequestStatus.MaterialRequestReceivedByViceDeanForHigherStudiesAndScientificResearch;
                    break;
                case Data.RequestType.EditFundingRequest:
                    initialState = RequestStatus.EditFundingRequestReceivedByViceDeanForHigherStudiesAndScientificResearch;
                    break;
            }

            if (initialState == null)
            {
                ModelState.AddModelError("", "لم يتم تحديد الحالة الابتدائية");
            }

            if (type == RequestType.EditFundingRequest)
            {
                if (model.WithdrawalFromProject == 0 && model.UpdateDuration == 0 && model.UpdateResearchersTeam == 0 && model.UpdateMaterials == 0)
                {
                    TempData["message"] = LangHelpers.Instance["please_select_some_attribute_to_edit"];
                    TempData["message_type"] = "danger";

                    return Create(PublicationRequestId, type, null);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int? duration = null;
                    int? EditRequestType = null;
                    bool updateResearcherTeam = false;
                    bool updateMaterials = false;
                    if (type == RequestType.EditFundingRequest)
                    {
                        if (model.WithdrawalFromProject == 1)
                        {
                            EditRequestType = (int)EditFundingRequestType.WithdrawalFromProject;
                        }
                        else if (model.UpdateDuration == 1 && model.UpdateResearchersTeam == 0 && model.UpdateMaterials == 0)
                        {
                            EditRequestType = (int)EditFundingRequestType.UpdateDuration;
                            duration = model.Duration;
                        }
                        else if (model.UpdateDuration == 0 && model.UpdateResearchersTeam == 0 && model.UpdateMaterials == 1)
                        {
                            EditRequestType = (int)EditFundingRequestType.UpdateMaterials;
                            updateMaterials = true;
                        }
                        else if (model.UpdateDuration == 0 && model.UpdateResearchersTeam == 1 && model.UpdateMaterials == 0)
                        {
                            EditRequestType = (int)EditFundingRequestType.UpdateResearchersTeam;
                            updateResearcherTeam = true;
                        }
                        else if (model.UpdateDuration == 1 && model.UpdateResearchersTeam == 0 && model.UpdateMaterials == 1)
                        {
                            EditRequestType = (int)EditFundingRequestType.UpdateDurationMaterials;
                            updateMaterials = true;
                            duration = model.Duration;
                        }
                        else if (model.UpdateDuration == 1 && model.UpdateResearchersTeam == 1 && model.UpdateMaterials == 0)
                        {
                            EditRequestType = (int)EditFundingRequestType.UpdateDurationResearchersTeam;
                            updateResearcherTeam = true;
                            duration = model.Duration;
                        }
                        else if (model.UpdateDuration == 0 && model.UpdateResearchersTeam == 1 && model.UpdateMaterials == 1)
                        {
                            EditRequestType = (int)EditFundingRequestType.UpdateResearchersTeamMaterials;
                            updateMaterials = true;
                            updateResearcherTeam = true;
                        }
                        else if (model.UpdateDuration == 1 && model.UpdateResearchersTeam == 1 && model.UpdateMaterials == 1)
                        {
                            EditRequestType = (int)EditFundingRequestType.UpdateDurationResearchersTeamMaterials;
                            updateMaterials = true;
                            duration = model.Duration;
                            updateResearcherTeam = true;
                        }
                    }

                    #region Validation

                    if (updateResearcherTeam)
                    {
                        var mainResearcher = DbContext.Researchers.Where(r => r.UserId == CurrentUser.Id).FirstOrDefault();
                        {
                            if (model.Researchers == null || model.Researchers.Count == 0)
                            {
                                ModelState.AddModelError("Researchers", LangHelpers.Instance.GetLangValueByString("missing_researcher_data", false, lang));
                            }
                            else
                            {
                                model.ProjectType = model.Researchers.Count == 1 ? ProjectType.Single : ProjectType.Team;

                                if (!(model.Researchers[0].Id > 0) || mainResearcher.Id != model.Researchers[0].Id)
                                {
                                    ModelState.AddModelError("Researchers", LangHelpers.Instance.GetLangValueByString("request_must_be_submitted_by_main_researcher", false, lang));
                                }

                                var encounteredMobileNumbers = new Dictionary<string, int>();
                                var encounteredEmails = new Dictionary<string, int>();

                                model.Researchers[0].RoleInProject = ResearcherRole.MainResearcher;

                                var total = 0;
                                var insideImam = 0;
                                for (var i = 0; i < model.Researchers.Count; i++)
                                {
                                    if (string.IsNullOrWhiteSpace(model.Researchers[i].FullName)) continue;

                                    total++;

                                    if (model.Researchers[i].RoleInProject == ResearcherRole.Unspecified || (i > 0 && model.Researchers[i].RoleInProject == ResearcherRole.MainResearcher))
                                        model.Researchers[i].RoleInProject = ResearcherRole.AssistantResearcher;

                                    if (model.Researchers[i].MobileNumber != null)
                                    {
                                        var phone = "";
                                        if (Utils.ValidateMobile(model.Researchers[i].MobileNumber, out phone))
                                        {
                                            model.Researchers[i].MobileNumber = phone;


                                            if (encounteredMobileNumbers.ContainsKey(model.Researchers[i].MobileNumber))
                                            {
                                                encounteredMobileNumbers[model.Researchers[i].MobileNumber]++;
                                            }
                                            else
                                            {
                                                encounteredMobileNumbers.Add(model.Researchers[i].MobileNumber, 1);
                                            }
                                        }
                                    }

                                    if (model.Researchers[i].Email != null)
                                    {
                                        model.Researchers[i].Email = model.Researchers[i].Email.ToLower().Trim();

                                        if (model.Researchers[i].UniversityId == 1)
                                        {
                                            insideImam++;

                                            if (!model.Researchers[i].Email.EndsWith("@imamu.edu.sa") && !model.Researchers[i].Email.EndsWith(".imamu.edu.sa") && !model.Researchers[i].Email.EndsWith("kay-ict.com"))
                                            {
                                                ModelState.AddModelError("Researchers[" + i + "].Email", LangHelpers.Instance.GetLangValueByString("email_must_be_imam_edu", false, lang));
                                            }
                                        }

                                        if (encounteredEmails.ContainsKey(model.Researchers[i].Email))
                                        {
                                            encounteredEmails[model.Researchers[i].Email]++;
                                        }
                                        else
                                        {
                                            encounteredEmails.Add(model.Researchers[i].Email, 1);
                                        }
                                    }

                                    if (model.Researchers[i].UniversityId == null)
                                    {
                                        ModelState.Remove("Researchers[" + i + "].UniversityOther");
                                    }

                                    if (model.Researchers[i].CollegeId == null)
                                    {
                                        ModelState.Remove("Researchers[" + i + "].CollegeOther");
                                    }

                                    if (model.Researchers[i].DepartmentID == null)
                                    {
                                        ModelState.Remove("Researchers[" + i + "].DepartmentOther");
                                    }

                                    var rmodel = model.Researchers[i];

                                    if (rmodel.BankIbanPhotocopy == null || rmodel.BankIbanPhotocopy.ContentLength == 0)
                                    {
                                        var oldFile = rmodel.Id > 0 ? DbContext.UserFiles
                                                            .Where(f => f.User.Researchers.Count(r => r.Id == rmodel.Id) > 0)
                                                            .Where(f => f.Type == (byte)FileType.IbanPhotocopy)
                                                            .OrderByDescending(f => f.Created)
                                                            .FirstOrDefault() : null;

                                        if (oldFile == null)
                                        {
                                            ModelState.AddModelError("Researchers[" + i + "].BankIbanPhotocopy", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("BankIbanPhotocopy", false, lang)));
                                        }
                                    }

                                    if (string.IsNullOrWhiteSpace(rmodel.SectionPublishedPapers) || Utils.StripHTML(rmodel.SectionPublishedPapers).Trim().Length == 0)
                                    {
                                        ModelState.AddModelError("Researchers[" + i + "].SectionPublishedPapers", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionPublishedPapers", false, lang)));
                                    }

                                    if (string.IsNullOrWhiteSpace(rmodel.SectionInterests) || Utils.StripHTML(rmodel.SectionInterests).Trim().Length == 0)
                                    {
                                        ModelState.AddModelError("Researchers[" + i + "].SectionInterests", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionInterests", false, lang)));
                                    }

                                    if (string.IsNullOrWhiteSpace(rmodel.SectionCareerHistory) || Utils.StripHTML(rmodel.SectionCareerHistory).Trim().Length == 0)
                                    {
                                        ModelState.AddModelError("Researchers[" + i + "].SectionCareerHistory", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionCareerHistory", false, lang)));
                                    }

                                    if (string.IsNullOrWhiteSpace(rmodel.SectionAcademicHistory) || Utils.StripHTML(rmodel.SectionAcademicHistory).Trim().Length == 0)
                                    {
                                        ModelState.AddModelError("Researchers[" + i + "].SectionAcademicHistory", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionAcademicHistory", false, lang)));
                                    }

                                    if (string.IsNullOrWhiteSpace(rmodel.SectionTrainingCourses) || Utils.StripHTML(rmodel.SectionTrainingCourses).Trim().Length == 0)
                                    {
                                        ModelState.AddModelError("Researchers[" + i + "].SectionTrainingCourses", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionTrainingCourses", false, lang)));
                                    }

                                    if (string.IsNullOrWhiteSpace(rmodel.SectionAwards) || Utils.StripHTML(rmodel.SectionAwards).Trim().Length == 0)
                                    {
                                        ModelState.AddModelError("Researchers[" + i + "].SectionAwards", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionAwards", false, lang)));
                                    }

                                    if (string.IsNullOrWhiteSpace(rmodel.SectionMemberships) || Utils.StripHTML(rmodel.SectionMemberships).Trim().Length == 0)
                                    {
                                        ModelState.AddModelError("Researchers[" + i + "].SectionMemberships", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionMemberships", false, lang)));
                                    }
                                }

                                if ((insideImam / (double)total) < 0.5)
                                {
                                    ModelState.AddModelError("Researchers", LangHelpers.Instance.GetLangValueByString("imam_researchers_more_than_50", false, lang));
                                }

                                if (encounteredMobileNumbers.Count(k => k.Value > 1) > 0)
                                {
                                    ModelState.AddModelError("Researchers", LangHelpers.Instance.GetLangValueByString("request_contains_duplicate_phones", false, lang));
                                }

                                if (encounteredEmails.Count(k => k.Value > 1) > 0)
                                {
                                    ModelState.AddModelError("Researchers", LangHelpers.Instance.GetLangValueByString("request_contains_duplicate_emails", false, lang));
                                }
                            }
                        }
                    }

                    if (updateMaterials)
                    {
                        if (model.Materials.Count(r => string.IsNullOrWhiteSpace(r.Name)) > 0)
                        {
                            ModelState.AddModelError("Materials", LangHelpers.Instance.GetLangValueByString("material_name_is_required"));
                        }
                    }

                    #endregion

                    if (ModelState.IsValid)
                    {
                        Request request = new Data.DbModels.Request();
                        if (type.Value == RequestType.EditFundingRequest)
                        {
                            request = Utils.db.Requests.Where(x => x.PublicationRequestId == PublicationRequestId && x.Type == (byte)RequestType.EditFundingRequest).FirstOrDefault();
                            if (request == null)
                            {
                                long requestId = (new RefereeUtil()).CreateRequestRevision(PublicationRequestId, true);
                                request = Utils.db.Requests.Where(x => x.Id == requestId).FirstOrDefault();
                                request.RequestRevisionId = null;
                            }
                        }
                        if (updateResearcherTeam)
                        {
                            initialState = RequestStatus.EditFundingRequestPendingCoResearcherApproval;
                        }
                        request.Comments = model.Comments;
                        request.Status = (long)initialState;
                        request.Type = (byte)type.Value;
                        request.CurrentWorkflowLevel = 1;
                        request.Created = DateTime.Now;
                        request.CreatedBy = CurrentUser.Id;

                        request.Recommendation = null;
                        request.ReasonForInvalidRecommendation = null;
                        request.MeetingDate = null;
                        request.MeetingNumber = null;
                        request.SecretaryMeetingNumber = null;
                        request.SecretaryMeetingDate = null;

                        request.PublicationRequestId = otherRequest.Id;
                        request.AnnouncementId = otherRequest.AnnouncementId;
                        request.Title = otherRequest.Title;
                        request.TitleEnglish = otherRequest.TitleEnglish;
                        request.CollegeId = otherRequest.CollegeId;
                        request.DepartmentId = otherRequest.DepartmentId;
                        request.EditRequestType = EditRequestType;
                        request.Updated = DateTime.Now;
                        request.UpdatedBy = CurrentUser.Id;
                        if (model.Forms.Count(x=>x.CompletionRate!=null)>0)
                        {
                            foreach (var f in model.Forms)
                            {
                               var data= DbContext.RequestTimelines.Where(x => x.Id == f.Id).FirstOrDefault();
                                data.CompletionRate = f.CompletionRate;
                                data.Reasons = f.Reasons;
                                DbContext.Entry(data).State = EntityState.Modified;                                
                            }
                         }


                        if (duration != null)
                        {
                            request.Duration = duration;
                        }
                        else
                        {
                            request.Duration = otherRequest.Duration;
                        }


                        DbContext.Requests.Add(request);
                        DbContext.SaveChanges();

                        FileType _fileType = FileType.Other;
                        if (type.Value == RequestType.MaterialRequest)
                        {
                            _fileType = FileType.MaterialsRequest;
                        }
                        else if (type.Value == RequestType.MidTermProgressReport)
                        {
                            _fileType = FileType.MidTermAttachment;
                        }
                        else if (type.Value == RequestType.FulltermProgressReport)
                        {
                            _fileType = FileType.FinalTermAttachment;
                        }

                        for (var i = 0; i < Request.Files.Count; i++)
                        {
                            var File = Request.Files[i];
                            if (File != null && File.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(File.FileName);
                                var extension = Path.GetExtension(fileName);

                                var file = new Data.DbModels.RequestFile
                                {
                                    Id = Guid.NewGuid(),
                                    Type = (byte)_fileType,
                                    CreatedBy = Utils.CurrentUser.Id,
                                    FileName = Path.GetFileName(File.FileName),
                                    Extension = extension,
                                    RequestId = request.Id,
                                    Created = DateTime.Now,
                                    Updated=DateTime.Now,
                                    UpdatedBy=Utils.CurrentUser.Id
                                };

                                file.FilePath = Path.Combine(Server.MapPath("~/Content/files/"), file.Id + extension);
                                File.EnhancedSaveAs(file.FilePath);

                                DbContext.RequestFiles.Add(file);
                            }
                        }

                        DbContext.SaveChanges();

                        #region UpdateResearcherTeam

                        if (updateResearcherTeam)
                        {
                            var encounteredResearcherIds = new HashSet<long>();
                            var coresearchers = Utils.db.RequestCoresearchers.Where(x => x.RequestId == request.Id && x.IsDeleted == false).ToList();

                            for (var i = 0; i < model.Researchers.Count; i++)
                            {
                                var rmodel = model.Researchers[i];

                                var nameComps = Utils.GetNameComponents(rmodel.FullName);

                                var researcher = DbContext.Researchers
                                    .Where(r => r.Id == rmodel.Id)
                                    .FirstOrDefault();

                                if (researcher == null)
                                {
                                    researcher = new Researcher
                                    {
                                        FirstName = nameComps.FirstName,
                                        Email = rmodel.Email,
                                        Created = TM.Now,
                                        CreatedBy = CurrentUser.Id,
                                        Updated = TM.Now,
                                        UpdatedBy = CurrentUser.Id
                                    };

                                    DbContext.Researchers.Add(researcher);
                                }

                                var user = researcher.User;
                                if (user == null)
                                {
                                    var suffix_it = 0;
                                    var suffix = "";
                                    while (Utils.db.Users.Count(u => u.Email == rmodel.Email + suffix) > 0)
                                    {
                                        suffix_it++;
                                        if (suffix_it == 1)
                                            suffix = "_coresearcher";
                                        else
                                            suffix = "_coresearcher_" + (suffix_it + 1);

                                    }

                                    user = new Data.DbModels.User
                                    {
                                        Id = Guid.NewGuid().ToString(),

                                        Email = rmodel.Email + suffix,
                                        UserName = rmodel.Email + suffix,
                                        PhoneNumber = rmodel.MobileNumber,
                                        PhoneNumberConfirmed = false,
                                        EmailConfirmed = false,
                                        Created = TM.Now,
                                        CreatedBy = CurrentUser.Id,
                                        Updated = TM.Now,
                                        UpdatedBy = CurrentUser.Id,
                                        IsDeleted = false,
                                        PasswordHash = UserManager.PasswordHasher.HashPassword(Convert.ToBase64String(Encoding.ASCII.GetBytes(Utils.GenerateShortCode() + ""))),
                                        ForcePasswordChange = true,
                                        UniversityId = string.IsNullOrWhiteSpace(rmodel.UniversityOther) ? null : rmodel.UniversityId,
                                        CollegeId = string.IsNullOrWhiteSpace(rmodel.CollegeOther) ? null : rmodel.CollegeId,
                                        DepartmentId = string.IsNullOrWhiteSpace(rmodel.DepartmentOther) ? null : rmodel.DepartmentID,
                                        CollegeOther = rmodel.CollegeOther,
                                        UniversityOther = rmodel.UniversityOther,
                                        SecurityStamp = "a14ee95e-c73e-466e-a786-aaf4378237d3",
                                    };

                                    DbContext.Users.Add(user);
                                    DbContext.SaveChanges();

                                    researcher.User = user;
                                    researcher.UserId = user.Id;
                                }

                                var userProfile = user.UserProfile;
                                if (userProfile == null)
                                {
                                    user.UserProfile = new Data.DbModels.UserProfile
                                    {
                                        UserId = user.Id,
                                        Created = TM.Now,
                                        Updated = TM.Now,
                                        UpdatedBy = CurrentUser.Id,
                                        CreatedBy = CurrentUser.Id
                                    };
                                }

                                var date = Utils.StringToDate(rmodel.Birthdate);

                                researcher.FirstName = user.UserProfile.FirstName = nameComps.FirstName;
                                researcher.SecondName = user.UserProfile.MiddleName = nameComps.SecondName;
                                researcher.ThirdName = user.UserProfile.ThirdName = nameComps.ThirdName;
                                researcher.LastName = user.UserProfile.LastName = nameComps.LastName;
                                researcher.Qualification = (byte)rmodel.Qualification;
                                researcher.UniversityId = !string.IsNullOrWhiteSpace(rmodel.UniversityOther) ? null : rmodel.UniversityId;
                                researcher.CollegeId = !string.IsNullOrWhiteSpace(rmodel.CollegeOther) ? null : rmodel.CollegeId;
                                researcher.DepartmentId = !string.IsNullOrWhiteSpace(rmodel.DepartmentOther) ? null : rmodel.DepartmentID;
                                researcher.UniversityOther = rmodel.UniversityOther;
                                researcher.CollegeOther = rmodel.CollegeOther;
                                researcher.DepartmentOther = rmodel.DepartmentOther;
                                researcher.MobileNo = rmodel.MobileNumber;
                                researcher.Email = rmodel.Email;
                                researcher.NationalId = rmodel.NationalID;
                                researcher.CountryId = rmodel.CountryId;
                                researcher.CityId = rmodel.CityId;
                                researcher.Address = rmodel.Addresss;
                                researcher.DateOfBirth = date;
                                researcher.NationalityId = rmodel.NationalityId;
                                researcher.Specialization = rmodel.Specialization;
                                researcher.User.UserProfile.FirstName = researcher.FirstName;

                                researcher.User.UserProfile.BankAccountHolder = rmodel.BankAccountHolder;
                                researcher.User.UserProfile.BankIban = rmodel.BankIban;
                                researcher.User.UserProfile.CvSectionPublishedPapers = rmodel.SectionPublishedPapers;
                                researcher.User.UserProfile.CvSectionInterests = rmodel.SectionInterests;
                                researcher.User.UserProfile.CvSectionCareerHistory = rmodel.SectionCareerHistory;
                                researcher.User.UserProfile.CvSectionAcademicHistory = rmodel.SectionAcademicHistory;
                                researcher.User.UserProfile.CvSectionTrainingCourses = rmodel.SectionTrainingCourses;
                                researcher.User.UserProfile.CvSectionAwards = rmodel.SectionAwards;
                                researcher.User.UserProfile.CvSectionMemberships = rmodel.SectionMemberships;

                                if (rmodel.BankIbanPhotocopy != null && rmodel.BankIbanPhotocopy.ContentLength > 0)
                                {
                                    var file = rmodel.BankIbanPhotocopy;
                                    var oldFile = researcher.User.UserFiles
                                               .Where(f => f.Type == (byte)FileType.IbanPhotocopy)
                                               .OrderByDescending(f => f.Created)
                                               .FirstOrDefault();

                                    var fileId = oldFile == null ? Guid.NewGuid() : oldFile.Id;
                                    var fileName = Path.GetFileName(file.FileName);
                                    var extension = Path.GetExtension(fileName);
                                    var path = Path.Combine(Server.MapPath("~/Content/files/"), fileId + extension);

                                    file.EnhancedSaveAs(path);

                                    if (oldFile == null)
                                    {
                                        oldFile = new Data.DbModels.UserFile
                                        {
                                            Id = fileId,
                                            UserId = user.Id,
                                            Type = (byte)FileType.IbanPhotocopy,
                                            CreatedBy = Utils.CurrentUser.Id,
                                            Updated = TM.Now,
                                            UpdatedBy = CurrentUser.Id,

                                            FileName = Path.GetFileName(file.FileName),
                                            Extension = extension,
                                            Created = TM.Now,
                                            FilePath = path
                                        };

                                        DbContext.UserFiles.Add(oldFile);
                                    }
                                    else
                                    {
                                        oldFile.FilePath = path;
                                        oldFile.Extension = extension;
                                        oldFile.FileName = Path.GetFileName(file.FileName);
                                        oldFile.Updated = TM.Now;
                                        oldFile.UpdatedBy = CurrentUser.Id;
                                    }
                                }

                                if (rmodel.Bank > 0)
                                {
                                    researcher.User.UserProfile.BankId = rmodel.Bank;
                                    researcher.User.UserProfile.BankName = null;
                                }
                                else
                                {
                                    researcher.User.UserProfile.BankId = null;
                                    researcher.User.UserProfile.BankName = rmodel.BankOther;
                                }

                                researcher.User.UserProfile.BankBranch = rmodel.BankBranch;
                                researcher.Updated = TM.Now;
                                researcher.UpdatedBy = CurrentUser.Id;

                                DbContext.SaveChanges();

                                var coresearcher = coresearchers.FirstOrDefault(cr => cr.ResearcherId == researcher.Id);

                                if (coresearcher == null)
                                {
                                    coresearcher = new RequestCoresearcher
                                    {
                                        RequestId = request.Id,
                                        ResearcherId = researcher.Id,
                                        Created = TM.Now,
                                        CreatedBy = CurrentUser.Id,
                                        Updated = TM.Now,
                                        UpdatedBy = CurrentUser.Id,
                                    };

                                    DbContext.RequestCoresearchers.Add(coresearcher);
                                }

                                coresearcher.Order = i + 1;
                                coresearcher.CountryId = rmodel.CountryId;
                                coresearcher.CityId = rmodel.CityId;

                                var cvFile = researcher.User.UserFiles
                                    .OrderByDescending(x => x.Id)
                                    .FirstOrDefault(x => x.Type == (byte)FileType.Resume && x.FileTextId > 0);

                                if (cvFile == null)
                                {
                                    cvFile = new UserFile
                                    {
                                        Id = Guid.NewGuid(),
                                        Type = (byte)FileType.Resume,
                                        FileName = "",
                                        Extension = "",
                                        UserId = researcher.UserId,
                                        Created = TM.Now,
                                        CreatedBy = CurrentUser.Id,
                                        Updated = TM.Now,
                                        UpdatedBy = CurrentUser.Id,
                                    };

                                    DbContext.UserFiles.Add(cvFile);
                                    DbContext.SaveChanges();
                                }

                                var ntext = cvFile.FileText;

                                if (ntext == null)
                                {
                                    ntext = new FileText
                                    {
                                        Created = TM.Now,
                                        CreatedBy = CurrentUser.Id,
                                        Updated = TM.Now,
                                        UpdatedBy = CurrentUser.Id,
                                    };

                                    DbContext.FileTexts.Add(ntext);
                                    DbContext.SaveChanges();
                                }

                                DbContext.SaveChanges();

                                cvFile.FileTextId = ntext.Id;

                                coresearcher.ResearcherType = (byte)rmodel.RoleInProject;
                                coresearcher.Updated = TM.Now;
                                coresearcher.UpdatedBy = CurrentUser.Id;

                                encounteredResearcherIds.Add(researcher.Id);
                            }

                            foreach (var cr in coresearchers)
                            {
                                if (!encounteredResearcherIds.Contains(cr.ResearcherId))
                                {
                                    (new RequestUtil()).DeleteRequestCoresearcherResponseRow(cr.RequestId, cr.Id);
                                    cr.IsDeleted = true;
                                    cr.UpdatedBy = CurrentUser.Id;
                                    cr.Updated = TM.Now;
                                    DbContext.Entry(cr).State = EntityState.Modified;
                                }
                            }

                            DbContext.SaveChanges();
                            ((new RequestUtil())).sendNotificationToCoResearcherAfterEditingRequest(request.Id, CurrentUser.Id);
                        }

                        #endregion

                        #region UpdateMaterials

                        if (updateMaterials)
                        {
                            model.Materials = model.Materials.Where(r => r.Name != null).ToList();
                            for (var i = 0; i < model.Materials.Count; i++)
                            {
                                var rmodel = model.Materials[i];
                                var material = DbContext.RequestMaterials
                                    .OrderByDescending(r => r.Id)
                                    .Where(r => r.RequestId == request.Id)
                                    .FirstOrDefault(r => rmodel.Id > 0 && rmodel.Id == r.Id);

                                if (material == null)
                                {
                                    material = new RequestMaterial
                                    {
                                        RequestId = request.Id,
                                        Created = TM.Now,
                                        CreatedBy = CurrentUser.Id
                                    };

                                    DbContext.RequestMaterials.Add(material);
                                }

                                material.Name = rmodel.Name;
                                material.Type = (byte)rmodel.Type.Value;
                                material.Order = i + 1;
                                material.Amount = rmodel.Amount.Value;
                                material.Cost = rmodel.Cost.Value;
                                material.Usage = rmodel.Usage;
                                material.Updated = TM.Now;
                                material.UpdatedBy = CurrentUser.Id;

                                material.Type = !string.IsNullOrWhiteSpace(rmodel.OtherType) ? (byte)0 : (byte)rmodel.Type;
                                material.OtherType = rmodel.Type > 0 ? null : rmodel.OtherType;

                                DbContext.SaveChanges();
                            }
                        }

                        #endregion

                        var log = new RequestHistoryLog
                        {
                            RequestId = request.Id,
                            Status = request.Status,
                            Comments = model.Comments,
                            Action = (byte)(RequestAction.Submit),
                            IpAddress = Utils.GetUserIP(),
                            UserAgent = Request.UserAgent,
                            Url = Utils.GetFullUrl(),
                            Role = (byte)ArfUserRole.SystemAdmin,
                            IsImpersonating = false,
                            CollegeId = request.CollegeId,
                            Created = TM.Now,
                            CreatedBy = CurrentUser.Id,
                        };

                        DbContext.RequestHistoryLogs.Add(log);
                        DbContext.SaveChanges();

                        var notification = new UserNotification
                        {
                            RequestId = request.Id,
                            Actionable = true,
                            IsEmailSent = false,
                            UserId = DbContext.Users.Where(u => (u.Roles.Count(r => r.Name == ArfUserRole.VicePresidentForHigherStudiesAndScientificResearch.ToString()) > 0)).Select(u => u.Id).FirstOrDefault(),
                            Created = DateTime.Now,
                            CreatedBy = CurrentUser.Id,
                        };
                        DbContext.UserNotifications.Add(notification);
                        DbContext.SaveChanges();

                        TempData["message"] = "تم إرسال الطلب بنجاح";
                        TempData["message_type"] = "success";
                        return RedirectToAction("Index", "Requests");
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    Logger.LogExceptionAsync(ex, model);
                    TempData["message"] = LangHelpers.Instance["failed"] + " " + ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
                    TempData["message_type"] = "danger";
                }
                catch (Exception ex)
                {
                    Logger.LogExceptionAsync(ex, model);

                    var ex2 = ex;
                    while (ex2.InnerException != null)
                    {
                        ex2 = ex2.InnerException;
                    }

                    TempData["message"] = LangHelpers.Instance["failed"] + " " + ex2.Message;
                    TempData["message_type"] = "danger";
                }
            }

            return Create(PublicationRequestId, type, null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public JsonResult UpdateCoResearcherRequestReviewResponse(CoResearcherRequestReviewResponseViewModel model)
        {
            var _response = new AjaxRequestResponse();
            string _publicHashLink = Utils.Decrypt(model.ParamsHash);
            var coResearcherResponse = Utils.db.RequestCoresearcherResponses.Find(model.RequestCoResearcherResponseId);
            if (coResearcherResponse != null)
            {
                coResearcherResponse.SubmittedOn = TM.Now;
                coResearcherResponse.Updated = TM.Now;
                coResearcherResponse.Status = (model.Action == RequestCoresearcherResponseStatus.Approved.ToString()) ? (byte)RequestCoresearcherResponseStatus.Approved : (byte)RequestCoresearcherResponseStatus.Rejected;
                Utils.db.Entry(coResearcherResponse).State = EntityState.Modified;
                Utils.db.SaveChanges();

                var coResearcher = Utils.db.RequestCoresearchers.Find(coResearcherResponse.RequestCoresearcherId);
                if (coResearcher != null)
                {
                    coResearcher.DidResearcherAgree = (model.Action == RequestCoresearcherResponseStatus.Approved.ToString()) ? true : false;
                    coResearcher.Updated = TM.Now;
                    Utils.db.SaveChanges();
                }
                string coResearcherUserId = coResearcherResponse.RequestCoresearcher.Researcher.UserId;
                (new RequestUtil()).UpdateRequestStatusAfterCoResearcherResponseForRequestUpdate(model.RequestId, coResearcherUserId, ArfUserRole.Researcher);
            }
            _response.RedirectURL = Url.Action("Login", "Account");
            return Json(_response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public JsonResult UpdateRequestReviewResponse(ExternalRefereeRequestReviewResponseViewModel model)
        {
            var _response = new AjaxRequestResponse();
            string _publicHashLink = Utils.Decrypt(model.ParamsHash);
            bool DidRefereeAgree = model.Action == Data.RequestAction.AcceptToReview.ToString() ? true : false; ;
            if (!String.IsNullOrEmpty(_publicHashLink))
            {
                string[] _publicHashLinkParts = _publicHashLink.Split(Core.Constants.ExternalRefereeParamsConnector);
                string refereeUserId = String.Empty;
                var rr = Utils.db.RequestReferees.Where(r => r.RequestId == model.RequestId && r.RefereeId == model.RefereeId && r.DidRefereeAgree == null).FirstOrDefault();
                if (rr != null)
                {
                    refereeUserId = rr.Referee.UserId;

                    rr.DidRefereeAgree = DidRefereeAgree;
                    rr.RequestReviewAcceptOn = TM.Now;
                    rr.ReceivedOn = TM.Now;
                    rr.Action = (model.Action == RequestAction.AcceptToReview.ToString()) ? (byte)RequestAction.AcceptToReview : (byte)RequestAction.RejectToReview;
                    rr.Comments = String.Empty;
                    if (!rr.DidRefereeAgree.Value)
                    {
                        rr.SubmittedOn = TM.Now;
                    }
                    rr.Updated = TM.Now;
                    rr.UpdatedBy = !String.IsNullOrEmpty(refereeUserId) ? refereeUserId : Utils.SystemAdminUserId;

                    Utils.db.Entry(rr).State = EntityState.Modified;
                    Utils.db.SaveChanges();

                    if (!rr.DidRefereeAgree.Value)
                    {
                        (new RefereeUtil()).SendNotificationToNextRefereeForFundingRequestReview(rr.RequestId, 0,"");

                        var request = Utils.db.Requests.Find(rr.RequestId);
                        if (request != null)
                        {
                            long currentStatus = request.Status;
                            if (request.Status == (long)RequestStatus.FundingRequestPendingMainRefrees)
                            {
                                request.Status = (long)RequestStatus.FundingRequestReceivedBySpareReferee;
                                request.Updated = TM.Now;
                                Utils.db.SaveChanges();

                                Utils.db.RequestHistoryLogs.Add(new RequestHistoryLog
                                {
                                    RequestId = request.Id,
                                    Status = currentStatus,
                                    Action = (byte)(RequestAction.RejectToReview),
                                    IpAddress = Utils.GetUserIP(),
                                    UserAgent = Request.UserAgent,
                                    Url = Utils.GetFullUrl(),
                                    Role = (byte)ArfUserRole.SystemAdmin,
                                    IsImpersonating = false,
                                    CollegeId = request.CollegeId,
                                    Created = TM.Now,
                                    CreatedBy = Utils.SystemAdminUserId,
                                });

                                Utils.db.SaveChanges();

                            }
                        }
                    }
                }

                var session = UserSession;
                var db = Utils.db;
                var data = db.UserSessionDatas.Where(x => x.SessionId == UserSession.Id).ToList();
                SaveKeyValue<string>("Email", model.Email, data);
                SaveKeyValue<string>("ExternalRefereeHashParams", model.ParamsHash, data);

                if (!DidRefereeAgree || !String.IsNullOrEmpty(refereeUserId))
                {
                    _response.RedirectURL = Url.Action("Login", "Account", new { role = model.RoleCode });
                }
                else
                {
                    _response.RedirectURL = Url.Action("Register", "Account", new { role = model.RoleCode });
                }
            }
            else
            {
                _response.Status = false;
            }

            return Json(_response);
        }

        [HttpPost]
        [CustomAuthorize(ArfUserRole.SystemAdmin, ArfUserRole.PrincipalResearcher)]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string RequestId)
        {
            DbContext.Configuration.LazyLoadingEnabled = false;
            var request = Utils.db.Requests.Where(r => (r.UniqueId == RequestId || r.Id.ToString() == RequestId) && r.IsDeleted == false).FirstOrDefault();
            if (request != null)
            {
                if (request.Type == (byte)Data.RequestType.FundingRequest && (request.Status == 0 || request.Status == (long)RequestStatus.NotSubmitted))
                {
                    request.IsDeleted = true;
                    request.Updated = TM.Now;
                    request.UpdatedBy = CurrentUser.Id;
                    Utils.db.SaveChanges();
                }
            }

            return RedirectToLastIndexPage("Index", "Home");
        }

        [HttpGet]
        public ActionResult View(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return RedirectToLastIndexPage("Index", "Requests");

            DbContext.Configuration.LazyLoadingEnabled = true;

            var request = RequestUtils.GetRequests()
                .Where(r => r.UniqueId == id || r.Id.ToString() == id)
                .FirstOrDefault();

            if (request == null)
            {
                TempData["message"] = string.Format(LangHelpers.Instance["request_not_found"], id);
                TempData["message_type"] = "danger";
                return RedirectToLastIndexPage("Index", "Requests");
            }
            ViewBag.ShowRequestHistory = true;
            ViewBag.CanViewRefereeComments = false;

            if (request.CreatedBy == CurrentUser.Id && Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.Type == (byte)Data.RequestType.FundingRequest && (request.Status == 0 || request.Status == (long)RequestStatus.NotSubmitted))
            {
                return Redirect(Utils.GetFundingRequestLink((ReqFieldType)request.FieldType, null, request.Id));
            }
            request.RequestCoresearchers = request.RequestCoresearchers.Where(x => x.IsDeleted == false).ToList();

            var pendingUAction = request.RequestUsers
                .Where(ru => ru.Action == null && ru.UserId == CurrentUser.Id && ru.ReceivedOn == null)
                .OrderByDescending(a => a.AssignedOn)
                .FirstOrDefault();

            var isMember = Roles.First().ToString().EndsWith("Member");

            if (pendingUAction != null)
            {
                pendingUAction.ReceivedOn = TM.Now;
                DbContext.SaveChanges();
            }
            else if (isMember || Utils.HasRolesAny(ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects))
            {
                if (request.RequestUsers.Count(ru => ru.UserId == CurrentUser.Id) == 0)
                {
                    var reqUser = new RequestUser
                    {
                        RequestId = request.Id,
                        UserId = CurrentUser.Id,
                        AssignedOn = TM.Now,
                        AssignedBy = CurrentUser.Id,
                        Action = null,
                        SubmittedOn = null,
                        ReceivedOn = TM.Now,
                    };

                    DbContext.RequestUsers.Add(reqUser);
                    DbContext.SaveChanges();
                }
            }

            ViewBag.AllowToUploadAttachment = true;
            ViewBag.RefereeId = 0;
            ViewBag.ShowRefereeFeedbackForm = false;
            if (Utils.HasRolesAny(ArfUserRole.Referee)&&request.Type==(byte)RequestType.FundingRequest)
            {
                var pendingRAction = request.RequestReferees.Where(ru => ru.Referee.UserId == CurrentUser.Id && ru.RequestId == request.Id).FirstOrDefault();
                if (pendingRAction != null)
                {
                    ViewBag.RefereeId = pendingRAction.RefereeId;
                    if (pendingRAction.Action == null)
                    {
                        pendingRAction.ReceivedOn = TM.Now;
                        DbContext.SaveChanges();
                        if (pendingRAction.DidRefereeAgree == null)
                        {
                            ViewBag.AllowToUploadAttachment = false;
                        }
                    }
                    else if (pendingRAction.Action != null && pendingRAction.Action != (byte)RequestAction.RejectToReview)
                    {
                        ViewBag.ShowRefereeFeedbackForm = true;
                    }
                }
            }

            var viewName = "View";
            var modelType = typeof(RequestViewModel);
            var model = (RequestViewModel)null;
            var type = (Data.RequestType)request.Type;
            var user = Utils.GetCurrentUserDetails();
            var isResearcher = Utils.HasRolesAny(ArfUserRole.PrincipalResearcher);
            var isReferee = Utils.HasRolesAny(ArfUserRole.Referee);
            var otherRequest = (Request)null;
            if (type == Data.RequestType.MidTermProgressReport || type == Data.RequestType.FulltermProgressReport || type == Data.RequestType.MaterialRequest)
            {
                otherRequest = DbContext.Requests.Where(r =>
                    r.Id == request.PublicationRequestId &&
                    r.Type == (byte)Data.RequestType.FundingRequest &&
                    r.Status == (long)RequestStatus.Completed

                )
                .OrderByDescending(r => r.Created)
                .FirstOrDefault();
                List<SelectListItem> listItems = new List<SelectListItem>();
                for (int ii = 0; ii <= 100; ii++)
                {
                    listItems.Add(new SelectListItem { Text = ii.ToString(), Value = ii.ToString() });
                }
                ViewBag.dropdownlist = listItems;
            }

            switch (type)
            {
                case Data.RequestType.FundingRequest:
                default:

                    if (type == Data.RequestType.FundingRequest || type == RequestType.MidTermProgressReport)
                        viewName = "View";
                    else
                        viewName = "ViewGeneric";

                    model = new FundingRequestViewModel();
                    model.Request = request;
                    model.StatusNo = request.Status;
                    model.RequestType = (Data.RequestType)request.Type;
                    modelType = typeof(FundingRequestViewModel);
                    model.Title = request.Title;
                    model.Id = request.Id;
                    model.ProjectAdvantages = request.ProjectAdvantages;
                    model.ProjectDisadvantages = request.ProjectDisadvantages;
                    model.Recommendation = (request.Recommendation == null) ? 0 : request.Recommendation.Value;                   
                    if (otherRequest != null)
                    {
                        model.MaterialsTotalCost = string.Format("{0:c}", otherRequest.MaterialsTotalCost).Replace("$", LangHelpers.Instance["sar"] + " ");
                        model.TotalCostForProject = string.Format("{0:c}", otherRequest.TotalCostForProject).Replace("$", LangHelpers.Instance["sar"] + " ");
                    }

                    if (model.Recommendation == 0)
                    {
                        model.RecommendationText = String.Empty;
                    }
                    else
                    {
                        model.RecommendationText = LangHelpers.Instance[((RecommendationOptions)model.Recommendation).ToString()];
                    }
                    model.ReasonForInvalidRecommendation = request.ReasonForInvalidRecommendation;
                    model.MeetingNumber = request.MeetingNumber;
                    model.MeetingDate = request.MeetingDate == null ? "" : request.MeetingDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-Us"));
                    model.SecretaryMeetingNumber = request.SecretaryMeetingNumber;
                    model.SecretaryMeetingDate = request.SecretaryMeetingDate == null ? "" : request.SecretaryMeetingDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-Us"));

                    if (type == Data.RequestType.MaterialRequest)
                    {
                        model.RequestMaterial = Utils.db.RequestMaterials.Where(x => x.RequestId == otherRequest.Id).ToList();
                    }

                    if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers) && request.Status == (long)RequestStatus.FundingRequestWaitingForContractPreparation)
                    {
                        model.ResearcherContractText = (new RequestUtil()).GetResearcherContractText(request);
                        model.ResearcherContractStartDate = (new RequestUtil()).GetResearcherContractStartDate(request);
                    }
                    if (otherRequest != null)
                    {
                        var tReferees = request.RequestSuggestedReferees
                        .Where(x => x.CreatedByUser.Roles
                                    .Where(r => r.Name == "ViceDeanForHigherStudiesAndScientificResearch")
                                    .Count() > 0
                                    && x.CreatedByUser.CollegeId == request.CollegeId
                               )
                        .OrderBy(r => r.Order)
                        .Select(r =>
                            new SuggestedRefereeViewModel
                            {
                                Id = r.Id,
                                Name = r.Name,
                                Degree = r.Qualification == null ? QualificationType.Unspecified : (QualificationType)r.Qualification,
                                UniversityId = r.UniversityId,
                                CollegeId = r.CollegeId,
                                DepartmentID = r.DepartmentId,
                                UniversityOther = r.UniversityOther,
                                CollegeOther = r.CollegeOther,
                                DepartmentOther = r.DepartmentOther,
                                MobileNumber = r.MobileNo,
                                Email = r.Email,
                                Addresss = r.Address,
                            }
                        ).ToList();

                        model.SuggestedReferees.AddRange(tReferees);
                    }

                    var model2 = (FundingRequestViewModel)model;

                    model2.PubType = (PublicationType)request.PublicationType;

                    model2.RequestList = new List<Request> { request };
                    model2.RequestDataList = request.RequestDatas.ToList();
                    FillOutLookups(model2);
                    model2.RequestTTList = request.RequestTimeTables.ToList();
                    model2.ResearcherList = request.RequestCoresearchers.Where(x => x.IsDeleted == false).Select(rc => rc.Researcher).ToList();
                    model2.RequestActivityList = request.RequestActivities.ToList();
                    model2.RequestBenificialList = request.RequestBenificialAuthorities.ToList();
                    if (type == Data.RequestType.MidTermProgressReport)
                    {
                        model2.BudgetSummaryPerPerson = otherRequest.RequestCoresearchers.Where(x => x.IsDeleted == false)
                                 .OrderBy(rr => rr.Order)
                                 .ToList()
                                 .Select(rr => new BudgetPerPersonViewModel
                                 {
                                     Name = rr.Researcher.GetFullName(),
                                     ResearcherRole = LangHelpers.Instance[((ResearcherRole?)rr.ResearcherType).ToString()],
                                     IsConsultant = rr.ResearcherType == (byte)ResearcherRole.Consultant,
                                     FirstPeriodActiveMonths = rr.FirstPeriodActiveMonths + "",
                                     SecondPeriodActiveMonths = rr.SecondPeriodActiveMonths + "",
                                     FirstPeriodCost = string.Format("{0:c}", rr.FirstPeriodCost).Replace("$", LangHelpers.Instance["sar"] + " "),
                                     SecondPeriodCost = string.Format("{0:c}", rr.SecondPeriodCost).Replace("$", LangHelpers.Instance["sar"] + " "),
                                     TotalActiveMonths = rr.TotalActiveMonths + "",
                                     TotalCost = string.Format("{0:c}", rr.TotalCost).Replace("$", LangHelpers.Instance["sar"] + " "),
                                 })
                                 .ToList();
                        model2.FirstPeriodTotalCost = string.Format("{0:c}", otherRequest.FirstPeriodTotalCost).Replace("$", LangHelpers.Instance["sar"] + " ");
                        model2.SecondPeriodTotalCost = string.Format("{0:c}", otherRequest.SecondPeriodTotalCost).Replace("$", LangHelpers.Instance["sar"] + " ");
                        model2.Forms = otherRequest.RequestTimelines.Select(x => new Models.ReportForm { CompletionRate = x.CompletionRate, Reasons = x.Reasons, TaskName = x.TaskName, Id = x.Id }).ToList();
                    }
                    var workflow = (IRequestWorkflow)null;

                    switch ((Data.RequestType)request.Type)
                    {
                        case Data.RequestType.FundingRequest:
                            workflow = new FundingRequestWorkflow();
                            break;

                        case Data.RequestType.DesignRequest:
                            workflow = new DesignRequestWorkflow();
                            break;

                        case Data.RequestType.MidTermProgressReport:
                            workflow = new MidTermRequestWorkflow();
                            break;

                        case Data.RequestType.FulltermProgressReport:
                            workflow = new FinalTermRequestWorkflow();
                            break;

                        case Data.RequestType.AdvertisementLetterForCollegeDeans:
                        case Data.RequestType.AdvertisementLetterForDARWebsite:
                        case Data.RequestType.AdvertisementLetterForDesigner:
                        case Data.RequestType.AdvertisementLetterForIT:
                        case Data.RequestType.AdvertisementLetterForNewspaper:
                            workflow = new LetterRequestWorkflow();
                            break;

                        case Data.RequestType.MaterialRequest:
                            workflow = new MaterialRequestWorkflow();
                            break;

                        case Data.RequestType.EditFundingRequest:
                            workflow = new EditFundingRequestWorkflow();
                            break;
                        case Data.RequestType.RefereeReward:
                            workflow = new RefereeRewardWorkflow();
                            break;

                        case Data.RequestType.AnnouncementRequest:
                            break;
                    }
                    if (request.Status != (long)RequestStatus.FundingRequestReturnedToResearcher)
                    {
                        model.AllowEdit = false;
                    }
                    else
                    {
                        model.AllowEdit = true;
                    }

                    if (((Data.RequestType)request.Type).ToString().EndsWith("Reward"))
                    {
                        if (request.Status == (long)RequestStatus.RefereeRewardRecievedByDirector_of_Research_Projects_Financing_Program)
                        {
                            model.AllowEdit = true;
                        }

                        model2.Price = request.RewardRequest.Amount;
                    }

                    if (request.Type == (byte)Data.RequestType.AnnouncementRequest)
                        return Redirect(Utils.GetApplicationUrl() + "Announcements/Edit/" + request.AnnouncementId);

                    model2.AllowedActions = workflow.GetAllowedActions(request).Select(a => new RequestActionViewModel
                    {
                        Action = a,
                        Display = LangHelpers.Instance[a.ToString()],
                        Value = a.ToString()
                    }).ToList();

                    model2.AllowedActions.Insert(0, new RequestActionViewModel
                    {
                        Value = "",
                        Display = Resources.ArfStrings.please_choose_action
                    });

                    model2.CanSelectReferees = model2.AllowedActions.Count(a => a.Action == Data.RequestAction.ApproveAndSelectReferees || a.Action == Data.RequestAction.SelectReferees) > 0;

                    if (Utils.HasRolesAny(ArfUserRole.HeadOfTheStandingCommitteeForFinancingResearchProjects, ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects,ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch) && (request.Status == (long)RequestStatus.FundingRequestRecievedBySecretaryOfCommitteeResearchFinance||request.Status == (long)RequestStatus.MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearch))
                    {
                        model2.CanSelectReferees = true;
                    }

                    model2.CanViewReferees = request.RequestReferees.Count(rr => rr.Action != null && rr.Action != (byte)Data.RequestAction.Canceled) > 0 && !isResearcher && !isReferee;

                    if (!model2.CanViewReferees && Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases, ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers, ArfUserRole.HeadOfTheStandingCommitteeForFinancingResearchProjects, ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects, ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects, ArfUserRole.UniversityRector,ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch))
                    {
                        model2.CanViewReferees = true;
                    }
                    //if(model2.CanViewReferees&&Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch)&&request.Status==(long)RequestStatus.MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearch)
                    //{
                    //    model2.Referees=
                    //}

                    if (isReferee)
                    {
                        ViewBag.ShowRequestHistory = false;
                    }

                    if (isResearcher && request.CreatedBy == CurrentUser.Id && request.Status == (long)RequestStatus.FundingRequestPendingModificationsByResearcher)
                    {
                        ViewBag.CanViewRefereeComments = true;
                    }

                    if (type == Data.RequestType.RefereeReward)
                    {
                        model2.AuthorDetails = new AuthorViewModel
                        {
                            Id = request.RewardRequest.User.Id,
                            Name = request.RewardRequest.User.UserProfile.GetFullName(),
                        };

                        if (request.RewardRequest.User.UserProfile != null)
                        {
                            model2.AuthorDetails.AccountBank = request.RewardRequest.User.UserProfile.Bank == null ? request.RewardRequest.User.UserProfile.BankName : request.RewardRequest.User.UserProfile.Bank.NameArabic;
                            model2.AuthorDetails.AccountName = request.RewardRequest.User.UserProfile.BankAccountHolder;
                            model2.AuthorDetails.AccountIBAN = request.RewardRequest.User.UserProfile.BankIban;
                        }
                    }
                    else if (type == Data.RequestType.FundingRequest||type==Data.RequestType.MidTermProgressReport)
                    {
                        model2.AuthorDetails = new AuthorViewModel
                        {
                            Id = request.CreatedByUser.Researchers.First().Id + "",
                            Name = request.CreatedByUser.Researchers.First().GetFullName()
                        };
                    }

                    if (model2.CanSelectReferees && request.Type ==(long)RequestType.FundingRequest)
                    {
                        model2.MinRefereeCount = 3;
                        model2.MaxRefereeCount = 10;
                    }
                    if (model2.CanSelectReferees && request.Type == (long)RequestType.MidTermProgressReport)
                    {
                        model2.MinRefereeCount = 1;
                        model2.MaxRefereeCount = 10;
                    }

                    if (Utils.HasRolesAny(ArfUserRole.Referee) && model2.AllowedActions.Count > 1)
                    {
                        // saving receival time for referees
                        var rr = request.RequestReferees.FirstOrDefault(x => x.Referee.UserId == CurrentUser.Id);
                        if (rr != null && rr.ReceivedOn == null)
                        {
                            rr.ReceivedOn = TM.Now;
                            DbContext.Entry(rr).State = EntityState.Modified;
                            DbContext.SaveChanges();
                        }
                    }

                    model2.CanSeeCouncilMembers = false;

                    var role = (ArfUserRole?)null;

                    if (Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects))
                    {
                        role = ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects;
                    }
                    else if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch))
                    {
                        role = ArfUserRole.ResearchUnitMember;
                    }

                    if (role != null)
                    {
                        model2.CanSeeCouncilMembers = true;

                        model2.CouncilMembers = request.RequestUsers
                            .Where(ru => ru.Action != null && ru.Action != (byte)Data.RequestAction.Canceled && ru.User.Roles.Count(r => r.Name == role.ToString()) > 0)
                            .ToList()
                            .Select(r => new RefereeViewModel
                            {
                                Id = r.UserId,
                                FullName = r.User.UserProfile.GetFullName(),
                                Action = r.Action == null ? "" : LangHelpers.Instance[((Data.RequestAction)r.Action.Value).ToString()],
                                Comments = r.Comments,
                                SubmittedOn = r.SubmittedOn,
                                FileId = r.FileId == null ? "" : r.FileId.ToString(),
                                FileName = string.IsNullOrWhiteSpace(r.FileName) ? (r.RequestFile == null ? LangHelpers.Instance["member_opinion"] : LangHelpers.Instance[((FileType)r.RequestFile.Type).ToString()]) : r.FileName
                            })
                            .OrderByDescending(r => r.SubmittedOn)
                            .ToList();

                        var newList = new List<RefereeViewModel>();

                        foreach (var cm in model2.CouncilMembers)
                        {
                            if (newList.Count(x => x.Id == cm.Id) > 0) continue;

                            newList.Add(cm);
                        }

                        model2.CouncilMembers = newList;
                    }

                    model2.Referees = request.RequestReferees.Select(r =>
                    new RefereeViewModel
                    {
                        Id = r.RefereeId + "",
                        FullName = r.Referee.GetFullName(),
                        MajorName = r.Referee.MajorId == null ? "" : r.Referee.Major.NameArabic,
                        Comments = r.Comments,
                        ReviewSentOnStr = r.RequestReviewSentOn != null ? r.RequestReviewSentOn.Value.FormatDate(true) : "",
                        UserId = r.Referee.UserId,
                        FileId = r.RequestFileId == null ? null : r.RequestFileId.Value.ToString(),
                        FileName = r.RequestFileId == null ? null : r.RequestFile.FileName,
                        RequestReviewOrderNumber = r.RequestReviewOrderNumber.Value,
                        Action = r.Action == null ? "" : LangHelpers.Instance[((Data.RequestAction)r.Action.Value).ToString()],
                    }).ToList();

                    break;
            }

            if (model == null) model = new RequestViewModel();

            model.Request = request;

            var data = request.RequestDatas.ToList();
            foreach (var prop in modelType.GetProperties())
            {
                if (prop.PropertyType.FullName.Contains("List")) continue;
                var entry = data.FirstOrDefault(r => r.Name == prop.Name);
                if (entry == null) continue;

                if (prop.PropertyType.FullName == typeof(uint?).FullName)
                {
                    uint tmp = 0;
                    if (uint.TryParse(entry.Value, out tmp))
                    {
                        model.SetPropValue(prop.Name, tmp);
                    }
                }
                else if (prop.PropertyType.FullName == typeof(int?).FullName)
                {
                    int tmp = 0;
                    if (int.TryParse(entry.Value, out tmp))
                    {
                        model.SetPropValue(prop.Name, tmp);
                    }
                }
                else if (prop.PropertyType.FullName == typeof(DateTime?).FullName)
                {
                    DateTime tmp = default(DateTime);
                    if (DateTime.TryParse(entry.Value, out tmp))
                    {
                        model.SetPropValue(prop.Name, tmp);
                    }
                }
                else if (prop.PropertyType.FullName == typeof(PaymentMethod?).FullName)
                {
                    PaymentMethod tmp = PaymentMethod.Unspecified;
                    if (Enum.TryParse(entry.Value, true, out tmp))
                    {
                        model.SetPropValue(prop.Name, tmp);
                    }
                }
                else if (prop.PropertyType.FullName == typeof(VacationType?).FullName)
                {
                    VacationType tmp = VacationType.Unspecified;
                    if (Enum.TryParse(entry.Value, true, out tmp))
                    {
                        model.SetPropValue(prop.Name, tmp);
                    }
                }
                else
                {
                    model.SetPropValue(prop.Name, entry.Value);
                }
            }

            model.Id = request.Id;
            //model.Comments = request.UpdatedBy == CurrentUser.Id ? request.Comments : null;
            model.Name = user.DbUser.UserProfile.GetFullName();
            model.UniqueId = request.UniqueId;
            model.PublicationRequestId = request.PublicationRequestId;
            model.RequestCoresearcherResponses = new List<RequestCoresearcherResponse>();
            if (isResearcher || request.Status == (long)RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterResearcher)
            {
                model.RequestCoresearcherResponses = Utils.db.RequestCoresearcherResponses.Where(x => x.RequestId.ToString() == id).OrderByDescending(x => x.SentOn).ToList();
            }

            model.Type = LangHelpers.Instance[(((Data.RequestType)request.Type).ToString())];
            if (request.Status == (long)RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies && request.College != null)
            {
                model.Status = WebUtils.GetViceDeanForHigherStudiesAndScientificResearchRequestStatusName(((RequestStatus)request.Status).ToString(), request.College.ResearchUnitCode.Value);
            }
            else
            {
                model.Status = LangHelpers.Instance[(((RequestStatus)request.Status).ToString())];
            }

            model.Date = request.Created.FormatDate();
            model.CurrentlyWithUserId = request.CurrentlyWith;
            model.CreatedByUserId = request.CreatedBy;
            model.WithdrawalFromProject = LangHelpers.Instance["no"];
            model.UpdateDuration = LangHelpers.Instance["no"];
            model.UpdateMaterials = LangHelpers.Instance["no"];
            model.UpdateResearchersTeam = LangHelpers.Instance["no"];

            if (model.Request.Type == (byte)RequestType.EditFundingRequest)
            {
                var EditRequestType = model.Request.EditRequestType.Value;

                if (EditRequestType == (int)EditFundingRequestType.WithdrawalFromProject)
                {
                    model.WithdrawalFromProject = LangHelpers.Instance["yes"];
                }
                else
                {
                    if (EditRequestType == (int)EditFundingRequestType.UpdateMaterials
                        || EditRequestType == (int)EditFundingRequestType.UpdateDurationMaterials
                        || EditRequestType == (int)EditFundingRequestType.UpdateDurationResearchersTeamMaterials
                        || EditRequestType == (int)EditFundingRequestType.UpdateResearchersTeamMaterials
                        )
                    {
                        model.UpdateMaterials = LangHelpers.Instance["yes"];
                    }

                    if (EditRequestType == (int)EditFundingRequestType.UpdateDuration
                        || EditRequestType == (int)EditFundingRequestType.UpdateDurationMaterials
                        || EditRequestType == (int)EditFundingRequestType.UpdateDurationResearchersTeamMaterials
                        || EditRequestType == (int)EditFundingRequestType.UpdateDurationResearchersTeam
                        )
                    {
                        model.UpdateDuration = LangHelpers.Instance["yes"];
                    }
                    if (EditRequestType == (int)EditFundingRequestType.UpdateResearchersTeam
                        || EditRequestType == (int)EditFundingRequestType.UpdateDurationResearchersTeam
                        || EditRequestType == (int)EditFundingRequestType.UpdateDurationResearchersTeamMaterials
                        || EditRequestType == (int)EditFundingRequestType.UpdateResearchersTeamMaterials
                        )
                    {
                        model.UpdateResearchersTeam = LangHelpers.Instance["yes"];
                    }
                }
            }
            model.CommentThread = request.RequestHistoryLogs.ToList().OrderByDescending(h => h.Created).Select
             (
                         h => new CommentViewModel
                         {
                             IsAction = h.Action > 0,
                             Action = LangHelpers.Instance["history_" + ((Data.RequestAction)h.Action).ToString()],
                             Comment = h.Comments,
                             Username = h.CreatedByUser != null ? (h.CreatedByUser.UserProfile.GetFullName() != null ? h.CreatedByUser.UserProfile.GetFullName() : h.CreatedByUser.UserName) : "",
                             Role = LangHelpers.Instance[((ArfUserRole)h.Role).ToString()],
                             RoleKey = ((ArfUserRole)h.Role).ToString(),
                             Hide = (isResearcher && ((ArfUserRole)h.Role).ToString() == ArfUserRole.Referee.ToString()) ? true : false,
                             RoleGroupCode = h.College != null ? h.College.RoleGroupCode.Value : 0,
                             CollegeResearchUnitCode = h.College != null ? h.College.ResearchUnitCode.Value : 0,
                             Date = h.Created,
                             DateStr = h.Created.FormatDate(true, true)
                         }
             ).ToList();

            foreach (var _history in model.CommentThread)
            {
                if (_history.RoleGroupCode != 0 && _history.RoleKey == ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch.ToString())
                {
                    _history.Role = WebUtils.GetViceDeanForHigherStudiesAndScientificResearchSubRoleName(_history.RoleGroupCode, _history.RoleKey, _history.CollegeResearchUnitCode);
                }
            }
            model.CommentThread = model.CommentThread.OrderByDescending(m => m.Date).ThenByDescending(m => m.IsAction ? 0 : 1).ToList();
            //if (!Utils.HasRolesAny(ArfUserRole.ResearchUnitMember))
            //{
            model.CommentThread.RemoveAll(x => x.RoleKey == ArfUserRole.ResearchUnitMember.ToString());
            //}

            model.Files = RequestUtils.GetFiles(request);

            var pusers = request.RequestHistoryLogs.Select(u2 => u2.CreatedBy).ToList();

            model.ReturnableTo = DbContext.Users.Where(u => pusers.Count(u2 => u2 == u.Id) > 0 && u.Id != CurrentUser.Id).ToList().Select(u => new SelectListItem
            {
                Text = u.UserProfile.GetFullName(),
                Value = u.Id
            }).ToArray();

            #region Populate Lookups
            var values = Enum.GetValues(typeof(RecommendationOptions));
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Value = "0",
                Text = LangHelpers.Instance["please_select"]
            });
            var i = 1;
            foreach (var v in values)
            {
                list.Add(new SelectListItem
                {
                    Value = ((long)v).ToString(),
                    Text = LangHelpers.Instance[v.ToString()]
                });
                i++;
            }
            model.Recommendations = list.ToArray();


            var valuesMT = Enum.GetValues(typeof(MaterialType));
            var listMT = new List<BetterSelectListItem>();

            i = 0;
            foreach (var v in valuesMT)
            {
                if (v.ToString() == MaterialType.Unspecified.ToString()) continue;
                listMT.Add(new BetterSelectListItem
                {
                    Value = v.ToString(),
                    Text = LangHelpers.Instance[v.ToString()]
                });
                i++;
            }
            listMT.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, null), Value = "0" });
            model.MaterialTypes = listMT.ToArray();

            #endregion

            if (request.Announcement != null)
            {
                model.AnnouncementStartDate = request.Announcement.StartDateHijri;
                model.AnnouncementEndDate = request.Announcement.EndDateHijri;
            }
            if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch) && model.SuggestedReferees.Count < 5&&type==RequestType.FundingRequest)
            {
                for (var j = model.SuggestedReferees.Count; j < 5; j++)
                {
                    model.SuggestedReferees.Add(new SuggestedRefereeViewModel { });
                }
            }

            return View(viewName, model);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var result = View(id);

            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, FundingRequestViewModel model)
        {
            //return RedirectToAction("View", "Requests", new { id = model.Id }); 
            if (model == null || string.IsNullOrWhiteSpace(id)) return RedirectToLastIndexPage("Index", "Requests");
            var lang = model.FieldType == ReqFieldType.AppliedScienceField ? "en" : null;

            var request = RequestUtils.GetRequests()
                .Include(r => r.RequestCoresearchers)
                .Include("RequestCoresearchers.Researcher")
                .Include("RequestReferees")
                .Include("RequestReferees.Referee")
                .Include("PublicationRequest")
                .Include("PublicationRequest")
                .Include("RequestMaterials")
                .Include("RewardRequest.User")
                .Include("RewardRequest.User.UserProfile")
                .Where(r => r.Id == model.Id)
                .FirstOrDefault();

            if (request == null)
            {
                TempData["message"] = string.Format(LangHelpers.Instance["request_not_found"], model.Id.ToString());
                TempData["message_type"] = "danger";
                return RedirectToLastIndexPage("Index", "Requests");
            }


            var workflow = (IRequestWorkflow)null;

            switch ((Data.RequestType)request.Type)
            {
                case Data.RequestType.FundingRequest:
                    workflow = new FundingRequestWorkflow();
                    break;

                case Data.RequestType.DesignRequest:
                    workflow = new DesignRequestWorkflow();
                    break;

                case Data.RequestType.MidTermProgressReport:
                    workflow = new MidTermRequestWorkflow();
                    break;

                case Data.RequestType.FulltermProgressReport:
                    workflow = new FinalTermRequestWorkflow();
                    break;

                case Data.RequestType.AdvertisementLetterForCollegeDeans:
                case Data.RequestType.AdvertisementLetterForIT:
                case Data.RequestType.AdvertisementLetterForNewspaper:
                case Data.RequestType.AdvertisementLetterForDesigner:
                case Data.RequestType.AdvertisementLetterForDARWebsite:
                    workflow = new LetterRequestWorkflow();
                    break;

                case Data.RequestType.MaterialRequest:
                    workflow = new MaterialRequestWorkflow();
                    break;
                case Data.RequestType.EditFundingRequest:
                    workflow = new EditFundingRequestWorkflow();
                    break;

                case Data.RequestType.RefereeReward:
                    workflow = new RefereeRewardWorkflow();
                    break;
            }

            var allowedActions = workflow.GetAllowedActionsTransitions(request);
            var transition = allowedActions.FirstOrDefault(a => a.Item1 == model.Action);

            if (allowedActions.Count == 0)
            {
                return RedirectToAction("View", new { id = id });
            }
            else if (transition == null)
            {
                if (model.Action == default(Data.RequestAction))
                {
                    TempData["message"] = Resources.ArfStrings.please_choose_action;
                    TempData["message_type"] = "danger";
                }
                else
                {
                    TempData["message"] = Resources.ArfStrings.this_action_is_not_avaliable;
                    TempData["message_type"] = "danger";
                }

                return RedirectToAction("View", new { id = id });
            }


            if (transition.Item1.ToString().StartsWith("Return") || transition.Item1.ToString().StartsWith("Reject"))
            {
                if (string.IsNullOrWhiteSpace(model.Comments))
                {
                    TempData["message"] = Resources.ArfStrings.you_should_tell_the_reason;
                    TempData["message_type"] = "danger";


                    return RedirectToAction("View", new { id = id });
                }

                model.Comments = model.Comments.Trim();
            }


            var canSelectReferees = allowedActions.Count(a => a.Item1 == Data.RequestAction.ApproveAndSelectReferees || a.Item1 == Data.RequestAction.SelectReferees) > 0;
            var RequestCoresearchers = request.RequestCoresearchers.Where(x => x.IsDeleted == false).ToList();

            var isMember = Roles.First().ToString().EndsWith("Member");

            if (canSelectReferees && (model.Action == Data.RequestAction.ForwardToInitialReferee || model.Action == Data.RequestAction.ApproveAndSelectReferees || model.Action == Data.RequestAction.SelectReferees))
            {
                if (request.Type == (byte)RequestType.FundingRequest)
                {
                    model.MinRefereeCount = 3;
                    model.MaxRefereeCount = 10;
                }
                else
                {
                    model.MinRefereeCount = 1;
                    model.MaxRefereeCount = 10;
                }

                if ((model.Referees == null || model.Referees.Count < model.MinRefereeCount || model.Referees.Count > model.MaxRefereeCount))
                {
                    TempData["message"] = string.Format(Resources.ArfStrings.you_should_choose_refree, model.MinRefereeCount, model.MaxRefereeCount);
                    TempData["message_type"] = "danger";

                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }
            }

            var meetingDate = DateTime.MinValue;
            var SecretaryMeetingDate = DateTime.MinValue;

            if ((model.Action != RequestAction.SendToCommiteeMembers && (request.Status == (long)RequestStatus.EditFundingRequestPendingCollegeCommittee || request.Status == (long)RequestStatus.EditFundingRequestReceivedByViceDeanForHigherStudiesAndScientificResearch || request.Status == (long)RequestStatus.FundingRequestPendingCollegeCommittee || request.Status == (long)RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies) && Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch)) && model.Action != RequestAction.ReturnToResearcher)
            {
                if (model.Action != RequestAction.ApproveAndSelectReferees)
                {
                    if (string.IsNullOrWhiteSpace(model.Comments))
                    {
                        TempData["message"] = LangHelpers.Instance.GetLangValueByString("comments_required", false, lang);
                        TempData["message_type"] = "danger";
                        ModelState.AddModelError("Comments", LangHelpers.Instance.GetLangValueByString("comments_required", false, lang));

                        return RedirectToAction("View", "Requests", new { id = model.Id });
                    }
                }


                if (string.IsNullOrWhiteSpace(model.MeetingNumber))
                {
                    TempData["message"] = LangHelpers.Instance.GetLangValueByString("meeting_number_required", false, lang);
                    TempData["message_type"] = "danger";
                    ModelState.AddModelError("MeetingNumber", LangHelpers.Instance.GetLangValueByString("meeting_number_required", false, lang));

                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }

                if (string.IsNullOrWhiteSpace(model.MeetingDate) || !DateTime.TryParseExact(model.MeetingDate.Trim(), "yyyy-MM-dd", new CultureInfo("en-Us"), DateTimeStyles.None, out meetingDate))
                {
                    TempData["message"] = LangHelpers.Instance.GetLangValueByString("meetingdate_required", false, lang);
                    TempData["message_type"] = "danger";
                    ModelState.AddModelError("MeetingDate", LangHelpers.Instance["meetingdate_required"]);
                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }

                if (model.Recommendation == 0)
                {
                    TempData["message"] = LangHelpers.Instance.GetLangValueByString("recommendation_required", false, lang);
                    TempData["message_type"] = "danger";
                    ModelState.AddModelError("Recommendation", LangHelpers.Instance["recommendation_required"]);
                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }
            }

            if ((model.Action == RequestAction.ApproveAndSelectReferees || model.Action == RequestAction.Approve) && Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects))
            {
                if (string.IsNullOrWhiteSpace(model.SecretaryMeetingNumber))
                {
                    TempData["message"] = LangHelpers.Instance.GetLangValueByString("meeting_number_required", false, lang);
                    TempData["message_type"] = "danger";
                    ModelState.AddModelError("SecretaryMeetingNumber", LangHelpers.Instance.GetLangValueByString("meeting_number_required", false, lang));

                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }
                else if (string.IsNullOrWhiteSpace(model.SecretaryMeetingDate) || !DateTime.TryParseExact(model.SecretaryMeetingDate.Trim(), "yyyy-MM-dd", new CultureInfo("en-Us"), DateTimeStyles.None, out SecretaryMeetingDate))
                {
                    TempData["message"] = LangHelpers.Instance.GetLangValueByString("meetingdate_required", false, lang);
                    TempData["message_type"] = "danger";
                    ModelState.AddModelError("SecretaryMeetingDate", LangHelpers.Instance["meetingdate_required"]);
                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }
            }

            if (model.Action == RequestAction.Approve && request.Status == (long)RequestStatus.FundingRequestWaitingForContractPreparation && Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
            {
                model.ResearcherContractText.Replace("<div dir='rtl'><p><br></p>", "").Replace("<div dir='ltr'><p><br></p>", "");
                if (String.IsNullOrEmpty(model.ResearcherContractText))
                {
                    TempData["message"] = LangHelpers.Instance.GetLangValueByString("researcher_contract_text_required", false, lang);
                    TempData["message_type"] = "danger";
                    ModelState.AddModelError("ResearcherContractText", LangHelpers.Instance["researcher_contract_text_required"]);
                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }
            }

            if (Utils.HasRolesAny(ArfUserRole.ResearchUnitMember) || Utils.HasRolesAny(ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects))
            {
                if (string.IsNullOrEmpty(model.Comments) || string.IsNullOrWhiteSpace(model.Comments))
                {
                    TempData["message"] = LangHelpers.Instance.GetLangValueByString("recommendation_required", false, lang);
                    TempData["message_type"] = "danger";
                    ModelState.AddModelError("Comments", LangHelpers.Instance.GetLangValueByString("recommendation_required", false, lang));

                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }
            }

            ModelState.Clear();
            var materialCost = request.RequestMaterials.Sum(x => x.Cost);
            var requireFileUpload = RequestUtils.RequiredFiles(request).Count > 0;

            if (requireFileUpload)
            {
                var missingFiles = RequestUtils.MissingFiles(request, model.Files);

                if (missingFiles.Count > 0)
                {
                    TempData["message"] = Resources.ArfStrings.you_should_upload_files;
                    TempData["message_type"] = "danger";
                    var i = 0;
                    foreach (var f in missingFiles)
                    {
                        TempData["message"] += LangHelpers.Instance[f.ToString()];
                        if (i < missingFiles.Count - 1)
                        {
                            TempData["message"] += "، ";
                        }

                        i++;
                    }

                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }
            }

            var db = Utils.db;

            if (((Data.RequestType)request.Type).ToString().EndsWith("Reward") && request.Status == (long)RequestStatus.RefereeRewardRecievedByDirector_of_Research_Projects_Financing_Program)
            {
                if (model.Price == null || model.Price <= 0)
                {
                    TempData["message"] = Resources.ArfStrings.you_must_specify_reward_value;
                    TempData["message_type"] = "danger";

                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }

                if (model.AuthorDetails == null || string.IsNullOrWhiteSpace(model.AuthorDetails.AccountIBAN) || string.IsNullOrWhiteSpace(model.AuthorDetails.AccountBank) || string.IsNullOrWhiteSpace(model.AuthorDetails.AccountName))
                {
                    TempData["message"] = Resources.ArfStrings.you_must_enter_your_bank_info;
                    TempData["message_type"] = "danger";

                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }
                else if (!Utils.ValidateIBAN(model.AuthorDetails.AccountIBAN))
                {
                    TempData["message"] = Resources.ArfStrings.IBAN_no_is_incorrect;
                    TempData["message_type"] = "danger";

                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }
            }

            if (request.Type == (byte)Data.RequestType.FundingRequest && transition.Item2 == RequestStatus.Completed)
            {
                if (request.Duration == null || request.Duration < 0)
                {
                    TempData["message"] = Resources.ArfStrings.contract_duration_is_unknown;
                    TempData["message_type"] = "danger";

                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }
            }

            if (model.Action == ARF.Data.RequestAction.SendToDirectorOfProgramOfFinance && Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.Status == (long)RequestStatus.FundingRequestPendingModificationsByResearcher)
            {
                int totalRows = Utils.db.Requests.Where(x => x.RequestRevisionId == request.Id).Count();
                if (totalRows == 0)
                {
                    TempData["message"] = Resources.ArfStrings.update_request_before_submitting;
                    TempData["message_type"] = "danger";

                    return RedirectToAction("View", "Requests", new { id = model.Id });
                }
            }

            if (Utils.HasRolesAny(ArfUserRole.Referee))
            {
                if (model.RefereeFeedbackForm != null && model.RefereeFeedbackForm.Count > 0)
                {
                    foreach (var _question in model.RefereeFeedbackForm)
                    {
                        if (_question.GivenMarks >= 0 && String.IsNullOrEmpty(_question.Comments))
                        {
                            TempData["message"] = Resources.ArfStrings.comments_are_required_for_all_questions;
                            TempData["message_type"] = "danger";

                            return RedirectToAction("View", "Requests", new { id = model.Id });
                        }
                    }
                }
            }

            try
            {
                RequestFile submittedFile = null;

                if (model.Files == null)
                {
                    model.Files = new List<AttachedFile>();
                }
                Guid? fileFieldId = null;
                string fileFieldName = null;

                if (model.Files.Count > 0)
                {
                    for (var i = 0; i < model.Files.Count; i++)
                    {
                        var fileModel = model.Files[i];
                        if (fileModel == null || fileModel.Content == null || fileModel.Content.ContentLength <= 0) continue;

                        if (model.Files[i].Type == FileType.Unspecified)
                        {
                            var type = FileType.Other;

                            if (Utils.HasRolesAny(ArfUserRole.Referee))
                            {
                                type = FileType.RefereeOpinion;
                            }

                            if (request.Type == (byte)Data.RequestType.FundingRequest && Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                            {
                                type = FileType.Contract;
                            }

                            model.Files[0].Type = type;
                        }

                        var oldFile = db.RequestFiles.FirstOrDefault(f => fileModel.FileId != null && f.Id.ToString() == fileModel.FileId);

                        var File = model.Files[i].Content;
                        var fileId = oldFile == null ? Guid.NewGuid() : oldFile.Id;
                        var fileName = Path.GetFileName(File.FileName);
                        var extension = Path.GetExtension(fileName);
                        var path = Path.Combine(Server.MapPath("~/Content/files/"), fileId + extension);
                        fileFieldId = fileId;
                        fileFieldName = fileName;

                        File.EnhancedSaveAs(path);

                        if (oldFile == null)
                        {
                            submittedFile = new Data.DbModels.RequestFile
                            {
                                Id = fileId,
                                Type = (byte)model.Files[i].Type,
                                CreatedBy = Utils.CurrentUser.Id,
                                FileName = Path.GetFileName(File.FileName),
                                Extension = extension,
                                RequestId = request.Id,
                                Created = TM.Now,
                                FilePath = path,
                                Updated = TM.Now,
                                UpdatedBy = CurrentUser.Id
                            };

                            db.RequestFiles.Add(submittedFile);
                        }
                        else
                        {
                            oldFile.FilePath = path;
                            oldFile.Extension = extension;
                            oldFile.FileName = Path.GetFileName(File.FileName);
                            oldFile.Updated = TM.Now;
                            oldFile.UpdatedBy = CurrentUser.Id;
                        }
                        db.SaveChanges();
                    }
                }

                if (((Data.RequestType)request.Type).ToString().EndsWith("Reward") && request.Status == (long)RequestStatus.RefereeRewardRecievedByDirector_of_Research_Projects_Financing_Program)
                {
                    request.RewardRequest.Amount = model.Price;
                    request.RewardRequest.User.UserProfile.BankName = model.AuthorDetails.AccountBank;
                    request.RewardRequest.User.UserProfile.BankAccountHolder = model.AuthorDetails.AccountName;
                    request.RewardRequest.User.UserProfile.BankIban = model.AuthorDetails.AccountIBAN;

                    db.Entry(request.RewardRequest).State = EntityState.Modified;
                }

                var currentStatus = request.Status;

                if (model.Referees == null) model.Referees = new List<RefereeViewModel>();

                if (canSelectReferees)
                {
                    var requestReferees = new List<RequestReferee>();

                    if (!isMember)
                    {
                        var oldReferees = db.RequestReferees.Where(rr => rr.RequestId == request.Id && rr.Action == null);

                        foreach (var rr in oldReferees)
                        {
                            if (model.Referees.Count(r => r.Id == rr.RefereeId.ToString()) > 0) continue;
                            rr.Action = (byte)Data.RequestAction.Canceled;
                            rr.SubmittedOn = TM.Now;
                            rr.ReceivedOn = TM.Now;
                            rr.RequestReviewAcceptOn = TM.Now;
                            rr.RequestReviewSentOn = TM.Now;
                            rr.Updated = TM.Now;
                            rr.UpdatedBy = CurrentUser.Id;

                            db.Entry(rr).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        request.RefereesApproved = false;
                    }

                    var index = db.RequestReferees.Where(rr => rr.RequestId == request.Id).Max(p => p.RequestReviewOrderNumber);
                    if (index == null)
                    {
                        index = 0;
                    }
                    foreach (var referee in model.Referees)
                    {
                        var rid = (int)0;

                        if (!int.TryParse(referee.Id, out rid) || rid <= 0) continue;

                        var strHash = Convert.ToBase64String(SHA256CryptoServiceProvider.Create().ComputeHash(Encoding.ASCII.GetBytes(TM.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + referee.Id))).Replace("+", "").Replace("/", "").Replace("\\", "");
                        if (strHash.Length > 128)
                        {
                            strHash = strHash.Substring(0, 128);
                        }

                        if (strHash.EndsWith("="))
                            strHash = strHash.Substring(0, strHash.Length - 1);

                        var roundNo = -1;

                        if (model.Action == Data.RequestAction.ForwardToInitialReferee)
                        {
                            roundNo = 1;
                        }
                        else if (index < 2)
                        {
                            roundNo = 2;
                        }
                        else
                        {
                            roundNo = 3;
                        }

                        var rr = request.RequestReferees.FirstOrDefault(x => x.RefereeId == rid);

                        if (rr == null)
                        {
                            rr = new RequestReferee
                            {
                                RefereeId = rid,
                                RequestId = request.Id,
                                RoundNo = roundNo,
                                Key = strHash,
                                IsKeyUsed = false,
                                NumberOfVotes = 1,
                                Created = TM.Now,
                                CreatedBy = CurrentUser.Id,
                                Updated = TM.Now,
                                UpdatedBy = CurrentUser.Id,
                                RequestReviewOrderNumber = (index + 1)
                            };

                            requestReferees.Add(rr);
                            db.RequestReferees.Add(rr);
                        }
                        index++;
                    }

                    db.SaveChanges();
                }
                var _refreeUtil = new RefereeUtil();
                if (model.Action == ARF.Data.RequestAction.ForwardToMainReferees && Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers)||
                    (request.Type==(byte)RequestType.MidTermProgressReport&&Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch)))
                {
                    _refreeUtil.SendNotificationToNextRefereeForFundingRequestReview(model.Id.Value, 0,((RequestType)request.Type).ToString());

                    int responded = 0;
                    var _requestReferees = Utils.db.RequestReferees.Where(p => p.RequestId == model.Id.Value && p.RequestReviewSentOn != null).ToList();
                    if (_requestReferees.Count > 0)
                    {
                        foreach (var _referees in _requestReferees)
                        {
                            if (_referees.Action == (byte)RequestAction.Approve || _referees.Action == (byte)RequestAction.Reject || _referees.Action == (byte)RequestAction.ApproveWithComments)
                            {
                                responded++;
                            }
                        }
                    }
                    if (responded == 0)
                    {
                        _refreeUtil.SendNotificationToNextRefereeForFundingRequestReview(model.Id.Value, 0,((RequestType)request.Type).ToString());
                    }
                }

                if (Utils.HasRolesAny(ArfUserRole.Referee))
                {
                    var rr = request.RequestReferees.FirstOrDefault(r => r.Referee.UserId == CurrentUser.Id);
                    bool updateRequestStatus = false;
                    bool updateRefereeRequestForm = false;
                    bool sendInvitationToNextReferee = false;

                    if (rr != null)
                    {
                        if (rr.ReceivedOn == null)
                        {
                            rr.ReceivedOn = TM.Now;
                        }
                        if (rr.DidRefereeAgree == null)
                        {
                            rr.DidRefereeAgree = model.Action.ToString() == Data.RequestAction.AcceptToReview.ToString() ? true : false;
                            rr.RequestReviewAcceptOn = TM.Now;
                            rr.Action = (byte)model.Action;
                            if (!rr.DidRefereeAgree.Value)
                            {
                                rr.SubmittedOn = TM.Now;
                                rr.Comments = model.Comments;
                                updateRequestStatus = true;

                                // Send Notification to Next Reserve Referee
                                int MaxRequestReviewOrderNumberReceivedRequest = request.RequestReferees.Where(p => p.RequestReviewSentOn != null).Max(p => p.RequestReviewOrderNumber).Value;
                                _refreeUtil.SendNotificationToNextRefereeForFundingRequestReview(rr.RequestId, MaxRequestReviewOrderNumberReceivedRequest,"");
                            }
                            rr.Updated = TM.Now;
                            rr.UpdatedBy = CurrentUser.Id;
                        }
                        else
                        {
                            updateRefereeRequestForm = true;
                            updateRequestStatus = true;
                            rr.SubmittedOn = TM.Now;
                            rr.Action = (byte)model.Action;
                            rr.DidRefereeAgree = true;
                            rr.Comments = model.Comments;
                            if (submittedFile != null)
                            {
                                rr.RequestFileId = submittedFile.Id;
                            }

                            if (transition.Item2 != RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeDecision)
                            {
                                sendInvitationToNextReferee = true;
                            }
                        }

                        db.Entry(rr).State = EntityState.Modified;
                        db.SaveChanges();
                        if (model.RefereeFeedbackForm != null && updateRefereeRequestForm && model.RefereeFeedbackForm.Count > 0)
                        {
                            foreach (var _question in model.RefereeFeedbackForm)
                            {
                                var _questionRow = db.RefereeRequestFeedbacks.Where(x => x.FeedbackFormQuestionId == _question.FeedbackFormQuestionId && x.RequestId == model.Id && x.RefereeId == rr.RefereeId && x.IsDeleted == false).FirstOrDefault();
                                if (_questionRow == null)
                                {
                                    _questionRow = new RefereeRequestFeedback();
                                    _questionRow.RequestId = model.Id.Value;
                                    _questionRow.RefereeId = rr.RefereeId;
                                    _questionRow.FeedbackFormQuestionId = _question.FeedbackFormQuestionId;
                                    _questionRow.Created = TM.Now;
                                    _questionRow.CreatedBy = CurrentUser.Id;
                                }
                                _questionRow.Comments = _question.Comments;
                                _questionRow.GivenMarks = _question.GivenMarks;
                                _questionRow.Updated = TM.Now;
                                _questionRow.UpdatedBy = CurrentUser.Id;
                                if (_questionRow.Id == 0)
                                {
                                    db.RefereeRequestFeedbacks.Add(_questionRow);
                                }
                                db.SaveChanges();
                            }
                        }

                        if (updateRequestStatus)
                        {
                            request.Status = (long)transition.Item2;
                        }

                        if (sendInvitationToNextReferee)
                        {
                            int pendingToRespond = Utils.db.RequestReferees.Where(p => p.RequestId == rr.RequestId && p.RequestReviewSentOn != null && p.SubmittedOn == null).Count();
                            if (pendingToRespond == 0)
                            {
                                _refreeUtil.SendNotificationToNextRefereeForFundingRequestReview(rr.RequestId, 0,"");
                            }
                        }
                    }
                }
                else
                {
                    request.Status = (long)transition.Item2;
                }

                request.LastStatusChanged = TM.Now;
                request.LastStatusChangedBy = CurrentUser.Id;
                request.CurrentlyWith = transition.Item3 == null || transition.Item3.Length == 0 ? null : transition.Item3[0];

                request.Updated = TM.Now;
                request.UpdatedBy = CurrentUser.Id;
                string baseURL = System.Configuration.ConfigurationManager.AppSettings["ApplicationUrl"];
                string contractHeaderURL = baseURL + "/ContractHeader.html";
                if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch) && (request.Status == (long)RequestStatus.FundingRequestRecievedByDeanOfScientificResearch || request.Status == (long)RequestStatus.FundingRequestPendingCollegeCommittee || request.Status == (long)RequestStatus.EditFundingRequestReceivedByViceDeanForHigherStudiesAndScientificResearch || request.Status == (long)RequestStatus.EditFundingRequestPendingCollegeCommittee))
                {
                    request.ProjectAdvantages = model.ProjectAdvantages;
                    request.ProjectDisadvantages = model.ProjectDisadvantages;
                    request.Recommendation = (long)model.Recommendation;
                    request.ReasonForInvalidRecommendation = model.ReasonForInvalidRecommendation;
                    request.MeetingNumber = model.MeetingNumber;
                    if (meetingDate != DateTime.MinValue && meetingDate != null)
                    {
                        request.MeetingDate = Convert.ToDateTime(model.MeetingDate, new CultureInfo("en-Us"));
                    }
                }
                else if (model.Action == RequestAction.ApproveAndSelectReferees && Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects))
                {
                    request.SecretaryMeetingNumber = model.SecretaryMeetingNumber;
                    if (SecretaryMeetingDate != DateTime.MinValue && SecretaryMeetingDate != null)
                    {
                        request.SecretaryMeetingDate = Convert.ToDateTime(model.SecretaryMeetingDate, new CultureInfo("en-Us"));
                    }
                }
                else if (model.Action == RequestAction.SendToViceDeanForResearchCentersAndDatabases && transition.Item2 == RequestStatus.FundingRequestContractReceivedByResearchCentersAndDatabases && Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                {
                    request.ResearcherContractText = model.ResearcherContractText;
                    var contract_startdate = Utils.db.Settings.FirstOrDefault(x => x.Key == Constants.FUNDING_REQUEST_CONTRACT_START_DATE);
                    if (contract_startdate != null)
                    {
                        request.ResearcherContractStartDate = Convert.ToDateTime(contract_startdate.Value, new CultureInfo("en-Us"));
                    }
                    var oldFile = db.RequestFiles.FirstOrDefault(f => f.RequestId == request.Id && f.Type == (byte)FileType.Contract);
                    string FileName = DateTime.Now.Ticks.ToString() + "--" + request.UniqueId + "-contract.pdf";
                    string extension = ".pdf";
                    string FilePath = Path.Combine(Server.MapPath("~/Content/files/"), FileName);

                    string pdfHtml = (new RequestUtil()).GetFundingRequestContractHeader(request, FileName) + request.ResearcherContractText + (new RequestUtil()).GetFundingRequestContractFooter(request, FileName);
                    var pdf = Pdf.From(pdfHtml)
                                .OfSize(PaperSize.A4)
                                .WithTitle(FileName)
                                .WithoutOutline()
                                .WithMargins(2.0.Centimeters())
                                .Portrait()
                                .Comressed()
                                .WithObjectSetting("header.htmlUrl", contractHeaderURL)
                                .WithObjectSetting("header.spacing", "2.5")
                                .WithObjectSetting("header.line", "1")
                                .Content();

                    FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
                    fs.Write(pdf, 0, pdf.Length);
                    fs.Close();

                    if (oldFile == null)
                    {
                        submittedFile = new Data.DbModels.RequestFile
                        {
                            Id = Guid.NewGuid(),
                            Type = (byte)FileType.Contract,
                            CreatedBy = Utils.CurrentUser.Id,
                            FileName = FileName,
                            Extension = extension,
                            RequestId = request.Id,
                            Created = TM.Now,
                            FilePath = FilePath,
                            Updated = TM.Now,
                            UpdatedBy = CurrentUser.Id
                        };

                        db.RequestFiles.Add(submittedFile);
                    }
                    else
                    {
                        oldFile.FilePath = FilePath;
                        oldFile.Extension = extension;
                        oldFile.FileName = FileName;
                        oldFile.Updated = TM.Now;
                        oldFile.UpdatedBy = CurrentUser.Id;
                    }
                    db.SaveChanges();
                }
                else if (model.Action == RequestAction.Approve && transition.Item2 == RequestStatus.FundingRequestContractReceivedByDeanOfScientificResearchAfterResearcher && Utils.HasRolesAny(ArfUserRole.PrincipalResearcher))
                {
                    if (!string.IsNullOrEmpty(model.Signature) && model.Checkbox)
                    {
                        string _Signature = "<div style='padding-top: 25px;'><div style='float:left'><p>" + LangHelpers.Instance["researcher_name"] + ": <br/>" + CurrentUser.FullName + "</p></div></div>";
                        var oldFile = db.RequestFiles.FirstOrDefault(f => f.RequestId == request.Id && f.Type == (byte)FileType.Contract);
                        string FileName = DateTime.Now.Ticks.ToString() + "--" + request.UniqueId + "-contract.pdf";
                        string extension = ".pdf";
                        string FilePath = Path.Combine(Server.MapPath("~/Content/files/"), FileName);
                        request.ResearcherContractText = request.ResearcherContractText.Replace("{MainResearcherSignature}", model.Signature);

                        string pdfHtml = (new RequestUtil()).GetFundingRequestContractHeader(request, FileName) + request.ResearcherContractText + (new RequestUtil()).GetFundingRequestContractFooter(request, FileName);
                        var pdf = Pdf.From(pdfHtml)
                                    .OfSize(PaperSize.A4)
                                    .WithTitle(FileName)
                                    .WithoutOutline()
                                    .WithMargins(2.0.Centimeters())
                                    .Portrait()
                                    .Comressed()
                                    .WithObjectSetting("header.htmlUrl", contractHeaderURL)
                                    .WithObjectSetting("header.spacing", "2.5")
                                    .WithObjectSetting("header.line", "1")
                                    .Content();

                        FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
                        fs.Write(pdf, 0, pdf.Length);
                        fs.Close();

                        if (oldFile != null)
                        {
                            oldFile.FilePath = FilePath;
                            oldFile.Extension = extension;
                            oldFile.FileName = FileName;
                            oldFile.Updated = TM.Now;
                            oldFile.UpdatedBy = CurrentUser.Id;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (!model.Checkbox)
                        {
                            ModelState.AddModelError("", LangHelpers.Instance["please_check_the_checkbox"]);
                        }
                        if(string.IsNullOrEmpty(model.Signature))
                        {
                            ModelState.AddModelError("", LangHelpers.Instance["please_enter_signature"]);
                        }
                        return RedirectToAction("View", "Requests", new { id = model.Id });
                    }
                }
                else if (model.Action == RequestAction.Approve && transition.Item2 == RequestStatus.Completed && Utils.HasRolesAny(ArfUserRole.DeanofDeanshipofScientificResearch))
                {
                    if (!string.IsNullOrEmpty(model.Signature) && model.Checkbox)
                    {
                        var mainResearcher = Utils.db.Users.Find(request.CreatedBy);
                        string _Signature = "<div style='padding-top: 25px;'><div style='float:right'><p>" + LangHelpers.Instance["dean_name"] + ": <br/>" + CurrentUser.FullName + "</p></div><div style='float:left'><p>" + LangHelpers.Instance["researcher_name"] + ": <br/>" + mainResearcher.FullName + "</p></div></div>";
                        var oldFile = db.RequestFiles.FirstOrDefault(f => f.RequestId == request.Id && f.Type == (byte)FileType.Contract);
                        string FileName = DateTime.Now.Ticks.ToString() + "--" + request.UniqueId + "-contract.pdf";
                        string extension = ".pdf";
                        string FilePath = Path.Combine(Server.MapPath("~/Content/files/"), FileName);
                        string ContractApprovalDate = request.ContractApprovalDate != null ? request.ContractApprovalDate.Value.FormatDate() : DateTime.Now.FormatDate();

                        request.ResearcherContractText = request.ResearcherContractText
                                                                .Replace("{DeanSignature}", model.Signature)
                                                                .Replace("{ContractApprovalDate}", ContractApprovalDate);

                        string pdfHtml = (new RequestUtil()).GetFundingRequestContractHeader(request, FileName) + request.ResearcherContractText + (new RequestUtil()).GetFundingRequestContractFooter(request, FileName);
                        var pdf = Pdf.From(pdfHtml)
                                    .OfSize(PaperSize.A4)
                                    .WithTitle(FileName)
                                    .WithoutOutline()
                                    .WithMargins(2.0.Centimeters())
                                    .Portrait()
                                    .Comressed()
                                    .WithObjectSetting("header.htmlUrl", contractHeaderURL)
                                    .WithObjectSetting("header.spacing", "2.5")
                                    .WithObjectSetting("header.line", "1")
                                    .Content();

                        FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
                        fs.Write(pdf, 0, pdf.Length);
                        fs.Close();

                        if (oldFile != null)
                        {
                            oldFile.FilePath = FilePath;
                            oldFile.Extension = extension;
                            oldFile.FileName = FileName;
                            oldFile.Updated = TM.Now;
                            oldFile.UpdatedBy = CurrentUser.Id;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (!model.Checkbox)
                        {
                            ModelState.AddModelError("", LangHelpers.Instance["please_check_the_checkbox"]);
                        }
                        if (string.IsNullOrEmpty(model.Signature))
                        {
                            ModelState.AddModelError("Signature", LangHelpers.Instance["please_enter_signature"]);
                        }
                        return RedirectToAction("View", "Requests", new { id = model.Id });
                    }
                }

                db.Entry(request).State = EntityState.Modified;

                if (request.Type == (byte)Data.RequestType.FundingRequest)
                {
                    if (Utils.HasRolesAny(ArfUserRole.Referee))
                    {
                        var rr1 = request.RequestReferees.FirstOrDefault(r => r.Referee.UserId == CurrentUser.Id);
                        if (rr1.Action != (byte)RequestAction.AcceptToReview && rr1.Action != (byte)RequestAction.RejectToReview)
                        {
                            var refReward = new Request
                            {
                                PublicationRequestId = request.Id,
                                AnnouncementId = request.AnnouncementId,
                                StartDate = request.StartDate,
                                ProjectStartDate = request.ProjectStartDate,
                                ProjectEndDate = request.ProjectEndDate,

                                CollegeId = request.CollegeId,
                                Title = request.Title,
                                PublicationType = request.PublicationType,
                                Type = (byte)Data.RequestType.RefereeReward,
                                Status = (long)RequestStatus.RefereeRewardRecievedByDirector_of_Research_Projects_Financing_Program,
                                LastStatusChanged = TM.Now,
                                LastStatusChangedBy = CurrentUser.Id,
                                Created = TM.Now,
                                CreatedBy = CurrentUser.Id,
                                Updated = TM.Now,
                                UpdatedBy = CurrentUser.Id,
                            };


                            db.Requests.Add(refReward);
                            db.SaveChanges();
                            var refRewardReq = new RewardRequest
                            {
                                RequestId = refReward.Id,
                                UserId = CurrentUser.Id,
                                Created = TM.Now,
                                CreatedBy = CurrentUser.Id,
                                Updated = TM.Now,
                                UpdatedBy = CurrentUser.Id,
                            };

                            db.RewardRequests.Add(refRewardReq);
                            db.SaveChanges();
                        }
                    }

                    if (isMember && request.Status == (long)RequestStatus.FundingRequestPendingCollegeCommittee)
                    {
                        var pendingUAction = request.RequestUsers.Where(ru => ru.UserId == CurrentUser.Id && ru.RequestId == request.Id).FirstOrDefault();
                        if (pendingUAction != null)
                        {
                            pendingUAction.SubmittedOn = TM.Now;
                            pendingUAction.Action = (byte)model.Action;
                            pendingUAction.Comments = model.Comments;
                            pendingUAction.FileId = fileFieldId;
                            pendingUAction.FileName = fileFieldName;

                            db.SaveChanges();
                        }
                    }

                    if (Utils.HasRolesAny(ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects) && request.Status == (long)RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjects)
                    {
                        var pendingUAction = request.RequestUsers.Where(ru => ru.UserId == CurrentUser.Id && ru.RequestId == request.Id).FirstOrDefault();
                        if (pendingUAction != null)
                        {
                            pendingUAction.SubmittedOn = TM.Now;
                            pendingUAction.Action = (byte)model.Action;
                            pendingUAction.Comments = model.Comments;
                            pendingUAction.FileId = fileFieldId;
                            pendingUAction.FileName = fileFieldName;

                            db.SaveChanges();
                        }
                    }

                }

                if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch) && request.Status == (long)RequestStatus.FundingRequestRecievedByDeanOfScientificResearch)
                {
                    model.SuggestedReferees = model.SuggestedReferees.Where(r => r.Name != null).ToList();
                    var encounteredRefereeIds = new HashSet<long>();
                    var referees = request.RequestSuggestedReferees
                        .Where(x => x.CreatedBy == Utils.CurrentUser.Id)
                        .ToList();

                    for (var i = 0; i < model.SuggestedReferees.Count; i++)
                    {
                        var rmodel = model.SuggestedReferees[i];

                        var referee = DbContext.RequestSuggestedReferees
                            .Where(r => r.RequestId == request.Id && r.CreatedBy == Utils.CurrentUser.Id)
                            .OrderByDescending(r => r.Id)
                            .FirstOrDefault(r => r.Email == rmodel.Email && (!(rmodel.Id > 0) || rmodel.Id == r.Id));

                        if (referee == null)
                        {
                            referee = new RequestSuggestedReferee
                            {
                                Name = model.SuggestedReferees[i].Name,
                                RequestId = request.Id,
                                Created = TM.Now,
                                CreatedBy = CurrentUser.Id,
                                UpdatedBy = CurrentUser.Id,
                                Updated = TM.Now
                            };

                            DbContext.RequestSuggestedReferees.Add(referee);
                        }

                        referee.Name = rmodel.Name;
                        referee.Qualification = (byte)rmodel.Degree;
                        referee.UniversityId = !string.IsNullOrWhiteSpace(rmodel.UniversityOther) ? null : rmodel.UniversityId;
                        referee.CollegeId = !string.IsNullOrWhiteSpace(rmodel.CollegeOther) ? null : rmodel.CollegeId;
                        referee.DepartmentId = !string.IsNullOrWhiteSpace(rmodel.DepartmentOther) ? null : rmodel.DepartmentID;
                        referee.UniversityOther = rmodel.UniversityOther;
                        referee.CollegeOther = rmodel.CollegeOther;
                        referee.DepartmentOther = rmodel.DepartmentOther;
                        referee.MobileNo = rmodel.MobileNumber;
                        referee.Email = rmodel.Email;
                        referee.Address = rmodel.Addresss;
                        referee.Order = i + 1;
                        referee.Updated = TM.Now;
                        referee.UpdatedBy = CurrentUser.Id;

                        DbContext.SaveChanges();

                        encounteredRefereeIds.Add(referee.Id);
                    }

                    foreach (var cr in referees)
                    {
                        if (!encounteredRefereeIds.Contains(cr.Id))
                        {
                            DbContext.Entry(cr).State = EntityState.Deleted;
                        }
                    }
                    DbContext.SaveChanges();
                }

                var history = new RequestHistoryLog
                {
                    RequestId = request.Id,
                    Status = currentStatus,
                    CurrentlyWith = CurrentUser.Id,
                    Role = (byte)Roles.FirstOrDefault(),
                    CollegeId = CurrentUser.CollegeId,
                    IsImpersonating = UserUtils.IsImpersonating,

                    Comments = model.Comments,
                    IpAddress = Utils.GetUserIP(),
                    UserAgent = Request.UserAgent,
                    Url = Utils.GetFullUrl(),
                    Action = (byte)model.Action,
                    Created = TM.Now,
                    CreatedBy = CurrentUser.Id,
                };

                db.RequestHistoryLogs.Add(history);
                db.SaveChanges();

                if (request.Status == (long)RequestStatus.Completed)
                {
                    workflow.OnCompleted(request);
                }

                if (request.Type == (byte)Data.RequestType.DesignRequest && request.Status == (long)RequestStatus.Completed)
                {
                }

                if (request.Type == (byte)Data.RequestType.AdvertisementLetterForCollegeDeans && request.Status == (long)RequestStatus.Completed)
                {
                    var file = db.RequestFiles.OrderByDescending(r => r.Created).Where(r => r.RequestId == request.Id).FirstOrDefault();

                    RequestUtils.SendNotification(
                      request,
                      TemplateKey.RecievedFundingRequestByHeadOfResearchUnit_To_college_Vicedean_and_college_dean_Notification,
                      new ArfUserRole[] { ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch, ArfUserRole.CollegeDean },
                      null,
                      null,
                      null,
                      new string[] { file.FilePath }
                    );
                }

                if (request.Type == (byte)Data.RequestType.AdvertisementLetterForIT && request.Status == (long)RequestStatus.Completed)
                {
                    var file1 = db.RequestFiles.OrderByDescending(r => r.Created).Where(r => r.RequestId == request.Id).FirstOrDefault();
                    var file2 = DbContext.RequestFiles
                    .Where(rf => rf.Request.AnnouncementId == request.AnnouncementId && rf.Request.Type == (byte)Data.RequestType.DesignRequest)
                        .OrderByDescending(rf => rf.Created)
                        .Skip(0)
                        .Take(1)
                        .ToList()
                        .FirstOrDefault();

                    if (file1 == null) file1 = new RequestFile();
                    if (file2 == null) file2 = new RequestFile();

                    RequestUtils.SendNotification(
                      request,
                      TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_To_Dean_of_it_Notification,
                      new ArfUserRole[] { ArfUserRole.DeanOfTheDeanshipOfInformationTechnology },
                      null,
                      null,
                      null,
                      new string[] { file1.FilePath, file2.FilePath }
                    );
                }

                if (request.Type == (byte)Data.RequestType.AdvertisementLetterForNewspaper && request.Status == (long)RequestStatus.Completed)
                {
                    var file1 = db.RequestFiles.OrderByDescending(r => r.Created).Where(r => r.RequestId == request.Id).FirstOrDefault();
                    var file2 = DbContext.RequestFiles
                    .Where(rf => rf.Request.AnnouncementId == request.AnnouncementId && rf.Request.Type == (byte)Data.RequestType.DesignRequest)
                        .OrderByDescending(rf => rf.Created)
                        .Skip(0)
                        .Take(1)
                        .ToList()
                        .FirstOrDefault();

                    if (file1 == null) file1 = new RequestFile();
                    if (file2 == null) file2 = new RequestFile();

                    RequestUtils.SendNotification(
                      request,
                      TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_To_Director_of_University_Mirror_newspaper_Notification,
                      new ArfUserRole[] { ArfUserRole.UniversityMirrorDirector },
                      null,
                      null,
                      null,
                      new string[] { file1.FilePath, file2.FilePath }
                    );
                }

                request.LastActionId = history.Id;

                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();

                if (Utils.HasRolesAny(ArfUserRole.Referee))
                {
                    _refreeUtil.ReturnToSecretaryToSelectMoreRefereeIfRequired(request.Id);
                }
                else if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.Status == (long)RequestStatus.FundingRequestPendingCoresearchersAfterRequestUpdateAfterRefereeDecision && model.Action == RequestAction.SendToCoresearchers)
                {
                    (new RequestUtil()).sendNotificationToCoResearcherForRequestUpdateAfterRefereeDecision(request.Id, CurrentUser.Id);
                }

                if (request.Type == (byte)Data.RequestType.FundingRequest && request.Status == (long)RequestStatus.Completed)
                {
                    request.ProjectStartDate = request.ResearcherContractStartDate;
                    request.ProjectEndDate = request.ProjectStartDate.Value.AddMonths(request.Duration.Value);
                    request.ContractApprovalDate = DateTime.Now;
                    var file1 = db.RequestFiles.OrderByDescending(r => r.Created).Where(r => r.RequestId == request.Id && r.Type == (byte)FileType.Contract).FirstOrDefault();
                    if (file1 == null) file1 = new RequestFile();

                    RequestUtils.SendNotification(
                       request,
                       TemplateKey.RecievedFundingRequestByHeadOfResearchUnit_Notification,
                       new ArfUserRole[] { ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch, ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers, ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases, ArfUserRole.DeanofDeanshipofScientificResearch },
                       null,
                       new string[] { request.CreatedBy },
                       null,
                       new string[] { file1.FilePath },
                       request.CollegeId.Value
                     );
                }
                else if (request.Type == (byte)Data.RequestType.MaterialRequest && request.Status == (long)RequestStatus.MaterialRequestReceivedByResearcherAfterDirectorGeneralOfAdministrationAndFinance)
                {
                    RequestUtils.SendNotification(
                       request,
                       TemplateKey.MaterialRequestChequeReceivedByResearcherSMSNotification,
                       TemplateKey.MaterialRequestChequeReceivedByResearcherEmailNotification,
                       null,
                       null,
                       new string[] { request.CreatedBy },
                       null,
                       null
                     );

                }
                else if (request.Type == (byte)Data.RequestType.EditFundingRequest && request.Status == (long)RequestStatus.Approved && Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects))
                {
                    if (request.EditRequestType != (int)EditFundingRequestType.WithdrawalFromProject)
                    {
                        // Inserting into RequestCoresearchers
                        var publicationTeam = Utils.db.RequestCoresearchers.Where(x => x.RequestId == request.PublicationRequestId.Value).ToList();
                        foreach (var team in publicationTeam)
                        {
                            team.IsDeleted = true;
                            team.Updated = TM.Now;
                            team.UpdatedBy = Utils.SystemAdminUserId;
                            Utils.db.SaveChanges();
                        }

                        var RequestCoresearchersEditReq = Utils.db.RequestCoresearchers.AsNoTracking().Where(x => x.RequestId == request.Id && x.IsDeleted == false).ToList();
                        if (RequestCoresearchersEditReq.Count() > 0)
                        {
                            foreach (var _data in RequestCoresearchersEditReq)
                            {
                                _data.Created = TM.Now;
                                _data.Updated = TM.Now;
                                _data.UpdatedBy = Utils.SystemAdminUserId;
                                _data.RequestId = request.PublicationRequestId.Value;
                                _data.Id = 0;
                                Utils.db.RequestCoresearchers.Add(_data);
                            }
                            Utils.db.SaveChanges();
                        }

                        // Inserting into RequestMaterials

                        var publicationMaterial = Utils.db.RequestMaterials.Where(x => x.RequestId == request.PublicationRequestId.Value).ToList();
                        foreach (var _material in publicationMaterial)
                        {
                            _material.IsDeleted = true;
                            _material.Updated = TM.Now;
                            _material.UpdatedBy = Utils.SystemAdminUserId;
                            Utils.db.SaveChanges();
                        }

                        var RequestMaterials = Utils.db.RequestMaterials.AsNoTracking().Where(x => x.RequestId == request.Id).ToList();
                        if (RequestMaterials.Count() > 0)
                        {
                            foreach (var _data in RequestMaterials)
                            {
                                _data.Created = TM.Now;
                                _data.Updated = TM.Now;
                                _data.UpdatedBy = Utils.SystemAdminUserId;
                                _data.RequestId = request.PublicationRequestId.Value;
                                _data.Id = 0;
                                Utils.db.RequestMaterials.Add(_data);
                            }
                            Utils.db.SaveChanges();
                        }

                        var req = Utils.db.Requests.Find(request.PublicationRequestId.Value);
                        req.Duration = request.Duration;
                        req.Updated = TM.Now;
                        req.UpdatedBy = Utils.SystemAdminUserId;
                        Utils.db.SaveChanges();
                    }
                }
                TempData["message"] = string.Format(LangHelpers.Instance["request_updated_successfully"], model.Id.ToString());
                TempData["message_type"] = "success";

                return RedirectToAction("View", "Requests", new { id = model.Id });
            }

            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Logger.LogExceptionAsync(ex, model);
                TempData["message"] = LangHelpers.Instance["failed"] + " " + ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
                TempData["message_type"] = "danger";
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsync(ex, model);

                var ex2 = ex;
                while (ex2.InnerException != null)
                {
                    ex2 = ex2.InnerException;
                }

                TempData["message"] = LangHelpers.Instance["failed"] + " " + ex2.Message;
                TempData["message_type"] = "danger";
            }

            return RedirectToAction("View", "Requests", new { id = model.Id });
        }

        [HttpGet]
        [CustomAuthorize(ArfUserRole.PrincipalResearcher)]
        public ActionResult FundingRequests()
        {
            var model = DbContext.Requests.Where(r =>
                r.CreatedBy == CurrentUser.Id &&
                r.Type == (byte)Data.RequestType.FundingRequest &&
                r.Status == (long)RequestStatus.Completed &&
                r.ProjectStartDate != null &&
                r.ProjectEndDate != null
            )
            .Select(r => new FundedProjectViewModel
            {
                Id = r.Id,
                StartDate = r.ProjectStartDate.Value,
                EndDate = r.ProjectEndDate.Value,
                Title = r.Title,
                FieldType = (ReqFieldType?)r.FieldType,
                MidTermReportRequestId = r.Requests.Count(r2 => r2.Type == (byte)Data.RequestType.MidTermProgressReport) > 0 ? r.Requests.OrderByDescending(r2 => r2.Created).Where(r2 => r2.Type == (byte)Data.RequestType.MidTermProgressReport).FirstOrDefault().Id : (long?)null,
                FullTermReportRequestId = r.Requests.Count(r2 => r2.Type == (byte)Data.RequestType.FulltermProgressReport) > 0 ? r.Requests.OrderByDescending(r2 => r2.Created).Where(r2 => r2.Type == (byte)Data.RequestType.FulltermProgressReport).FirstOrDefault().Id : (long?)null,
                MaterialRequestRequestId = r.Requests.Count(r2 => r2.Type == (byte)Data.RequestType.MaterialRequest) > 0 ? r.Requests.OrderByDescending(r2 => r2.Created).Where(r2 => r2.Type == (byte)Data.RequestType.MaterialRequest).FirstOrDefault().Id : (long?)null,
                RequestedMaterialCount = r.RequestMaterials.Count(r2 => r2.IsDeleted == false),
                EditRequestRequestId = r.Requests.Count(r2 => r2.Type == (byte)Data.RequestType.EditFundingRequest) > 0 ? r.Requests.OrderByDescending(r2 => r2.Created).Where(r2 => r2.Type == (byte)Data.RequestType.EditFundingRequest).FirstOrDefault().Id : (long?)null,
            })
            .ToList();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitActionPdf(FormCollection collection)
        {
            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertUrl(collection["TxtUrl"]);

            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "Document.pdf";
            return fileResult;
        }



        private string GenerateUniqueRequestId()
        {
            // format 201608220112
            var date = TM.Now;
            var s1 = DbContext.Requests.Where(c => c.Created.Year == date.Year && c.Created.Month == date.Month && c.Created.Day == date.Day).OrderByDescending(c => c.Id).FirstOrDefault();

            var id1 = (long)0;
            if (s1 != null && s1.UniqueId.Length == 12 && long.TryParse(s1.UniqueId.Substring(8), out id1))
            {
                return TM.Now.ToString("yyyyMMdd") + (id1 + 1).ToString().PadLeft(4, '0');
            }

            return date.ToString("yyyyMMdd", new CultureInfo("en-US")) + (1 + "").PadLeft(4, '0');
        }

        [NonAction]
        private void FillOutLookups(FundingRequestViewModel model)
        {
            var lang = model.FieldType == ReqFieldType.AppliedScienceField ? "en" : null;

            if (model.FieldType == ReqFieldType.AppliedScienceField)
            {
                Utils.SwitchCulture("EN");
            }
            {
                var list = DbContext.Colleges
                            .ToArray()
                            .Select(c => new SelectListItem
                            {
                                Text = c.LocalizedName(),
                                Value = c.Id.ToString()
                            })
                           .OrderBy(c => c.Text)
                           .ToList();

                list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });
                model.Colleges = list.ToArray();
            }
            {
                var list = DbContext.Universities
                            .ToArray()
                            .OrderBy(c => c.Id == 1 ? 0 : 1)
                            .ThenBy(c => c.LocalizedName())
                            .Select(c => new SelectListItem
                            {
                                Text = c.LocalizedName(),
                                Value = c.Id.ToString()
                            })
                            .ToList();
                list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });
                model.Universities = list.ToArray();
            }

            {
                var list = DbContext.Departments
                           .ToArray()
                           .Select(c => new SelectListItem
                           {
                               Text = c.LocalizedName(),
                               Value = c.Id.ToString()
                           })
                           .OrderBy(c => c.Text)
                           .ToList();
                list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });
                model.Departments = list.ToArray();
            }

            {
                var values = Utils.QualificationTypesByResearcherType(role: ArfUserRole.Referee);
                var list = new List<SelectListItem>();
                var i = 0;
                foreach (var v in values)
                {
                    if (v.ToString() == "Unspecified") continue;

                    list.Add(new SelectListItem
                    {
                        Value = v.ToString(),
                        Text = LangHelpers.Instance[v.ToString()]
                    });
                    i++;
                }
                model.QualificationTypes = list.ToArray();
            }

            //switch (model.Step)
            //{
            //    case 1:
            //        {
            //            var list = DbContext.Departments
            //                .Where(d => model.CollegeID > 0 && d.CollegeId == model.CollegeID)
            //                .Where(d => d.Field == null || d.Field == (byte)model.Type)
            //                .ToArray()
            //                .Select(c => new SelectListItem
            //                {
            //                    Text = c.LocalizedName(),
            //                    Value = c.Id.ToString()
            //                })
            //                .OrderBy(c => c.Text)
            //                .ToList();

            //            model.Departments = list.ToArray();
            //        }
            //        {
            //            var list = DbContext.Colleges
            //                .Where(c => c.Field == null || c.Field == (byte)model.Type)
            //           .ToArray()
            //           .Select(c => new SelectListItem
            //           {
            //               Text = c.LocalizedName(),
            //               Value = c.Id.ToString()
            //           })
            //           .OrderBy(c => c.Text)
            //           .ToList();

            //            model.Colleges = list.ToArray();
            //        }

            //        {
            //            var list = DbContext.Colleges
            //           .ToArray()
            //           .Select(c => new SelectListItem
            //           {
            //               Text = c.LocalizedName(),
            //               Value = c.Id.ToString()
            //           })
            //           .OrderBy(c => c.Text)
            //           .ToList();

            //            model.FullColleges = list.ToArray();
            //        }

            //        {
            //            var list = DbContext.Universities
            //       .ToArray()
            //       .OrderBy(c => c.Id == 1 ? 0 : 1)
            //       .ThenBy(c => c.LocalizedName())
            //       .Select(c => new SelectListItem
            //       {
            //           Text = c.LocalizedName(),
            //           Value = c.Id.ToString()
            //       })
            //       .ToList();

            //            list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

            //            model.Universities = list.ToArray();
            //        }

            //        {
            //            var list = DbContext.Categories
            //      .Where(c => c.Level == 1)
            //      .ToList()
            //      .Select(c => new SelectListItem
            //      {
            //          Text = c.LocalizedName(),
            //          Value = c.Id.ToString()
            //      })
            //      .OrderBy(c => c.Text)
            //      .ToList();

            //            list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

            //            model.PrimaryCategories = list.ToArray();
            //        }

            //        {
            //            if (model.PrimaryCategory > 0)
            //            {
            //                var list = DbContext.Categories
            //                   .Where(c => c.ParentId == model.PrimaryCategory && c.Level == 2)
            //                   .ToArray()
            //                   .Select(c => new SelectListItem
            //                   {
            //                       Text = c.LocalizedName(),
            //                       Value = c.Id.ToString()
            //                   })
            //                   .OrderBy(c => c.Text)
            //                   .ToList();

            //                list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

            //                model.SecondaryCategories = list.ToArray();
            //            }
            //            else
            //            {
            //                model.SecondaryCategories = new SelectListItem[0];
            //            }
            //        }
            //        {
            //            var list = new List<SelectListItem>();
            //            for (var i = 6; i <= 36; i += 6) list.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });

            //            model.Durations = list.ToArray();
            //        }

            //        {
            //            var list = new List<SelectListItem>();
            //            var priority = DbContext.DepartmentPriorities.FirstOrDefault(p => model.PriorityId > 0 && p.Id == model.PriorityId);

            //            var prios = priority != null ? priority.Department.DepartmentPriorities.ToList() : (model.PriorityDepartmentId > 0 ? DbContext.DepartmentPriorities.Where(dp => dp.DepartmentId == model.PriorityDepartmentId).ToList() : new List<DepartmentPriority>());

            //            list = prios
            //                .Select(c => new SelectListItem
            //                {
            //                    Text = c.Priority.LocalizedName(),
            //                    Value = c.Id.ToString()
            //                })
            //                .OrderBy(c => c.Text)
            //                .ToList();

            //            model.Priorities = list.ToArray();

            //            var deps = priority != null ? priority.Department.College.Departments.ToList() : (model.PriorityCollegeId > 0 ? DbContext.Departments.Where(dp => dp.CollegeId == model.PriorityCollegeId).ToList() : new List<Department>());

            //            list = deps
            //                .Select(c => new SelectListItem
            //                {
            //                    Text = c.LocalizedName(),
            //                    Value = c.Id.ToString()
            //                })
            //                .OrderBy(c => c.Text)
            //                .ToList();

            //            model.FullDepartments = list.ToArray();

            //            if (priority != null)
            //            {
            //                if (!(model.PriorityCollegeId > 0))
            //                {
            //                    model.PriorityCollegeId = priority.Department.CollegeId;
            //                    model.PriorityDepartmentId = priority.DepartmentId;
            //                }
            //                else if (!(model.PriorityDepartmentId > 0))
            //                {
            //                    model.PriorityDepartmentId = priority.DepartmentId;
            //                }
            //            }
            //        }
            //        break;

            //    case 2:
            //        {
            //            ViewBag.IncludeCKEditor = true;
            //            ViewBag.LargerWidth = true;

            //            var list = DbContext.Colleges
            //           .ToArray()
            //           .Select(c => new SelectListItem
            //           {
            //               Text = c.LocalizedName(),
            //               Value = c.Id.ToString()
            //           })
            //           .OrderBy(c => c.Text)
            //           .ToList();

            //            list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

            //            model.Colleges = list.ToArray();
            //        }

            //        {
            //            var list = DbContext.Universities
            //       .ToArray()
            //       .OrderBy(c => c.Id == 1 ? 0 : 1)
            //       .ThenBy(c => c.LocalizedName())
            //       .Select(c => new SelectListItem
            //       {
            //           Text = c.LocalizedName(),
            //           Value = c.Id.ToString()
            //       })
            //       .ToList();

            //            list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

            //            model.Universities = list.ToArray();
            //        }

            //        {
            //            var list = DbContext.Departments
            //       .ToArray()
            //       .Select(c => new SelectListItem
            //       {
            //           Text = c.LocalizedName(),
            //           Value = c.Id.ToString()
            //       })
            //       .OrderBy(c => c.Text)
            //       .ToList();

            //            list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

            //            model.Departments = list.ToArray();
            //        }

            //        {
            //            var values = Enum.GetValues(typeof(ResearcherRole));
            //            var list = new List<SelectListItem>();

            //            var i = 0;
            //            foreach (var v in values)
            //            {
            //                if (v.ToString() == ResearcherRole.Unspecified.ToString() || v.ToString() == ResearcherRole.MainResearcher.ToString()) continue;

            //                list.Add(new SelectListItem
            //                {
            //                    Value = v.ToString(),
            //                    Text = LangHelpers.Instance[v.ToString()]
            //                });

            //                i++;
            //            }

            //            model.ResearcherRoles = list.ToArray();
            //        }

            //        {
            //            var values = Utils.QualificationTypesByResearcherType();
            //            var list = new List<SelectListItem>();

            //            var i = 0;
            //            foreach (var v in values)
            //            {
            //                if (v.ToString() == "Unspecified") continue;

            //                list.Add(new SelectListItem
            //                {
            //                    Value = v.ToString(),
            //                    Text = LangHelpers.Instance[v.ToString()]
            //                });

            //                i++;
            //            }

            //            model.QualificationTypes = list.ToArray();
            //        }

            //        {
            //            var list = DbContext.Countries
            //                .ToArray()
            //                .OrderBy(c => c.Id == 1 ? 0 : 1)
            //                .ThenBy(c => c.LocalizedName())
            //                 .Select(c => new SelectListItem
            //                 {
            //                     Text = c.LocalizedName(),
            //                     Value = c.Id.ToString()
            //                 })
            //                .ToList();

            //            list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

            //            model.Countries = list.ToArray();
            //        }



            //        {
            //            var list = DbContext.Cities
            //                .ToArray()
            //                .Select(c => new CityViewModel
            //                {
            //                    Id = c.Id,
            //                    CountryId = c.CountryId,
            //                    Name = c.LocalizedName(),
            //                })
            //                .OrderBy(c => c.Name)
            //                .ToList();

            //            list.Add(new CityViewModel { Name = LangHelpers.Instance.GetLangValueByString("other", false, lang), Id = 0 });

            //            model.Cities = list;
            //        }
            //        {
            //            var list = DbContext.Banks
            //                .ToArray()
            //                .Select(c => new SelectListItem
            //                {
            //                    Text = c.LocalizedName(),
            //                    Value = c.Id.ToString()
            //                })
            //                .OrderBy(c => c.Text)
            //                .ToList();

            //            list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

            //            model.Banks = list.ToArray();
            //        }

            //        break;

            //    case 3:

            //        {
            //            ViewBag.IncludeCKEditor = true;
            //            ViewBag.LargerWidth = true;
            //            ViewBag.WidthMaximized = false;
            //        }

            //        break;

            //    case 4:
            //        {
            //            ViewBag.WidthMaximized = true;
            //            ViewBag.AllowPreview = true;

            //            var id = model.Request == null ? (long?)null : model.Request.Id;
            //            var userId = model.Request == null ? (string)null : model.Request.CreatedBy;
            //            var list = DbContext.Researchers
            //                .Where(r => id != null && r.RequestCoresearchers.Count(cr => cr.RequestId == id) > 0)
            //           .ToArray()
            //           .Select(c => new SelectListItem
            //           {
            //               Text = c.GetFullName() + GetResearcherRoleString(model.Request, c.Id),
            //               Value = c.Id.ToString()
            //           })
            //           .OrderBy(c => c.Text)
            //           .ToList();

            //            model.ResearchersLookup = list.ToArray();
            //        }

            //        {
            //            if (model.Request == null || !(model.Request.Duration > 0))
            //            {
            //                model.Months = new SelectListItem[0];
            //            }
            //            else
            //            {
            //                var list = new List<SelectListItem>();
            //                for (var i = 0; i < model.Request.Duration; i++)
            //                {
            //                    if (i < (model.Request.Duration / 2))
            //                        list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("first_period", false, lang) + " - " + LangHelpers.Instance.GetLangValueByString("the_month", false, lang) + " " + (i + 1), Value = (i + 1) + "" });
            //                    else
            //                        list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("second_period", false, lang) + " - " + LangHelpers.Instance.GetLangValueByString("the_month", false, lang) + " " + (i + 1), Value = (i + 1) + "" });
            //                }

            //                model.Months = list.ToArray();
            //            }
            //        }

            //        if (model.Type == ReqFieldType.AppliedScienceField)
            //        {
            //            var values = Enum.GetValues(typeof(MaterialType));
            //            var list = new List<SelectListItem>();

            //            var i = 0;
            //            foreach (var v in values)
            //            {
            //                if (v.ToString() == MaterialType.Unspecified.ToString()) continue;

            //                list.Add(new SelectListItem
            //                {
            //                    Value = v.ToString(),
            //                    Text = LangHelpers.Instance[v.ToString()]
            //                });

            //                i++;
            //            }

            //            model.MaterialTypes = list.ToArray();
            //        }
            //        break;

            //    case 5:


            //        break;

            //    case 6:
            //        {
            //            ViewBag.LargerWidth = true;

            //            var values = Enum.GetValues(typeof(ActivityType));
            //            var list = new List<SelectListItem>();

            //            var i = 0;
            //            foreach (var v in values)
            //            {
            //                if (v.ToString() == "Unspecified") continue;

            //                list.Add(new SelectListItem
            //                {
            //                    Value = v.ToString(),
            //                    Text = LangHelpers.Instance[v.ToString()]
            //                });

            //                i++;
            //            }

            //            list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

            //            model.ActivityTypes = list.ToArray();
            //        }

            //        break;
            //}

            Utils.SwitchCulture(null);
        }

        [NonAction]
        private string GetResearcherRoleString(Request request, long? researcherId)
        {
            if (request == null || !(researcherId > 0)) return "";
            var coresearcher = request.RequestCoresearchers.FirstOrDefault(rr => rr.ResearcherId == researcherId.Value && rr.IsDeleted == false);
            if (coresearcher == null || !(coresearcher.ResearcherType > 0))
            {
                if (coresearcher.Researcher.UserId == request.CreatedBy)
                    return " (" + LangHelpers.Instance[(ResearcherRole.MainResearcher).ToString()] + ")";
                return "";
            }

            return " (" + LangHelpers.Instance[((ResearcherRole)coresearcher.ResearcherType).ToString()] + ")";
        }
    }
}