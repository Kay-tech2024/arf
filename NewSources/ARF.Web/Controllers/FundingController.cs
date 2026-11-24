using ARF.Core;
using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using ARF.Web.App_Start.Code;
using ARF.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Linq;

namespace ARF.Web.Controllers
{
    public class FundingController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult New(ReqFieldType? type, int? step, long? id)
        {

            if (type == ReqFieldType.AppliedScienceField)
            {
                Utils.SwitchCulture("EN");
            }

            ViewBag.LargerWidth = true;

            if (!(id > 0)) id = 0;
            if (!(step > 0)) step = 1;
            var request = (Request)null;
            var lang = (string)null;

            if (type == null)
                type = ReqFieldType.Unspecified;

            var model = (CommonFundingRequestViewModel)null;

            var stepCount = (byte)1;
            var announcement = (Announcement)null;

            if (id > 0)
            {
                request = RequestUtils.GetRequests().FirstOrDefault(r => r.Id == id && (r.Type == (byte)Data.RequestType.FundingRequest || type == null || type == ReqFieldType.Unspecified || r.FieldType == (byte)type.Value));

                if (request == null)
                {
                    TempData["message"] = LangHelpers.Instance.GetLangValueByString("unknown_request", false, lang) + id;
                    TempData["message_type"] = "danger";

                    return RedirectToAction("Index", "Home");
                }

                if (request.Status != (long)RequestStatus.NotSubmitted && request.Status != (long)RequestStatus.FundingRequestReturnedToResearcher && request.Status != (long)RequestStatus.FundingRequestPendingModificationsByResearcher)
                {
                    TempData["message"] = LangHelpers.Instance.GetLangValueByString("cannot_edit_this_request", false, lang) + id;
                    TempData["message_type"] = "danger";

                    return Redirect(Utils.GetApplicationUrl() + "Requests/View/" + id);
                }
                request.RequestCoresearchers = request.RequestCoresearchers.Where(x => x.IsDeleted == false).ToList();

                if (type == null && request.FieldType > 0)
                {
                    type = (ReqFieldType)request.FieldType.Value;
                }
            }
            else
            {
                step = 1;
            }

            if (type == ReqFieldType.Unspecified)
            {
                if (request != null)
                {
                    type = (ReqFieldType)request.FieldType;
                }
                else
                {
                    TempData["message"] = LangHelpers.Instance.GetLangValueByString("unknown_page", false, lang);
                    TempData["message_type"] = "danger";
                    return RedirectToAction("Index", "Home");
                }
            }

            switch (type)
            {
                case ReqFieldType.ArabicLanguageField:
                    model = new ArabicFundingRequestViewModel();

                    stepCount = 6;

                    if (step > stepCount)
                        step = stepCount;

                    break;

                case ReqFieldType.IslamicField:
                    model = new IslamicFundingRequestViewModel();
                    stepCount = 6;

                    if (step > stepCount)
                        step = stepCount;

                    break;

                case ReqFieldType.AppliedScienceField:
                    model = new ScientificFundingRequestViewModel();
                    stepCount = 6;

                    if (step > stepCount)
                        step = stepCount;

                    break;

                case ReqFieldType.SocialField:
                    model = new SocialScienceFundingRequestViewModel();
                    stepCount = 6;

                    if (step > stepCount)
                        step = stepCount;

                    break;
            }

            if (model != null)
            {
                model.HasCoresearchers = request != null && request.RequestCoresearchers.Count(r => r.Researcher.UserId != r.CreatedBy && r.IsDeleted == false) > 0;
            }

            // making sure that previous sections have been filled
            if (step > 1)
            {
                var origStep = step;
                var lastUpdatedStep = request.LastUpdatedStep;

                if (lastUpdatedStep == null)
                {
                    lastUpdatedStep = 1;
                }

                if (!(step <= lastUpdatedStep + 1))
                {
                    step = lastUpdatedStep + 1;
                }

                if (step == 0)
                    step = 1;
                else if (step > stepCount)
                {
                    step = stepCount;
                }

                if (step != origStep)
                {
                    return Redirect(Utils.GetFundingRequestLink(type.Value, step, id));
                }
            }

            lang = type == ReqFieldType.AppliedScienceField ? "en" : null;

            var mainResearcher = DbContext.Researchers.Where(r => r.UserId == CurrentUser.Id).FirstOrDefault();

            if (mainResearcher == null)
            {
                TempData["message"] = LangHelpers.Instance.GetLangValueByString("researcher_data_is_not_found", false, lang);
                TempData["message_type"] = "danger";

                return RedirectToAction("Index", "Home");
            }

            if (!Utils.HasRolesAny(ArfUserRole.PrincipalResearcher))
            {
                TempData["message"] = LangHelpers.Instance.GetLangValueByString("access_denied", false, lang);
                TempData["message_type"] = "danger";
                return RedirectToAction("Index", "Home");
            }

            announcement = Utils.GetActiveAnnouncement();

            if (announcement == null)
            {
                var @continue = false;
                if (Constants.AllowSubmissionOfPendingRequestsForHoursAfterEndOfAnnouncement > 0 && request != null)
                {
                    var ann = Utils.db.Announcements
                           .Where(a => a.StartDateGreg <= TM.Now && DbFunctions.AddHours(a.EndDateGreg, Constants.AllowSubmissionOfPendingRequestsForHoursAfterEndOfAnnouncement) > TM.Now)
                           .OrderByDescending(a => a.Created)
                           .FirstOrDefault();

                    if (ann == null)
                    {
                        // users
                        {
                            var _allowed_to_submit = Utils.db.Settings.Where(x => x.Key == "arf.users.allowed_to_submit").FirstOrDefault();
                            var userId = CurrentUser.Id;
                            if (_allowed_to_submit != null)
                            {
                                var _list = _allowed_to_submit.Value.Split(',').ToList();
                                string result = _list.FirstOrDefault(x => x == userId);
                                if (result != null)
                                {
                                    ann = Utils.db.Announcements.OrderByDescending(a => a.Created).FirstOrDefault();
                                }
                            }
                        }

                        // requests
                        {
                            var _allowed_to_submit = Utils.db.Settings.Where(x => x.Key == "arf.request.allowed_to_submit").FirstOrDefault();
                            var requestId = id.ToString();
                            if (_allowed_to_submit != null)
                            {
                                var _list = _allowed_to_submit.Value.Split(',').ToList();
                                string result = _list.FirstOrDefault(x => x == requestId);
                                if (result != null)
                                {
                                    ann = Utils.db.Announcements.OrderByDescending(a => a.Created).FirstOrDefault();
                                }
                            }
                        }

                    }

                    if (ann != null)
                    {
                        announcement = ann;
                        @continue = true;
                    }
                    else if (request != null && request.Status == (long)RequestStatus.FundingRequestReturnedToResearcher)
                    {
                        @continue = true;
                        announcement = request.Announcement;
                    }


                }

                if (!@continue)
                {
                    TempData["message"] = LangHelpers.Instance.GetLangValueByString("no_active_announcement", false, lang);
                    TempData["message_type"] = "danger";
                    return RedirectToAction("Index", "Home");
                }
            }

            if (model == null)
            {
                TempData["message"] = LangHelpers.Instance.GetLangValueByString("unknown_page", false, lang);
                TempData["message_type"] = "danger";

                return RedirectToAction("Index", "Home");
            }

            model.HasCoresearchers = request != null && request.RequestCoresearchers.Count(r => r.Researcher != null && r.Researcher.UserId != r.CreatedBy && r.IsDeleted == false) > 0;
            model.Researchers = new List<CoresearcherViewModel>();
            model.ProjectType = ProjectType.Single;

            if (request != null)
            {
                // correcting coresearchers

                var mrr = request.RequestCoresearchers.FirstOrDefault(rr => rr.ResearcherId == mainResearcher.Id && rr.IsDeleted == false);

                if (mrr == null)
                {
                    var i = 2;
                    foreach (var rr in request.RequestCoresearchers)
                    {
                        rr.Order = i++;
                    }

                    mrr = new RequestCoresearcher
                    {
                        ResearcherId = mainResearcher.Id,
                        Order = 1,
                        RequestId = request.Id,
                        ResearcherType = (byte)ResearcherRole.MainResearcher,
                        Created = TM.Now,
                        Updated = TM.Now,
                        CreatedBy = CurrentUser.Id,
                        UpdatedBy = CurrentUser.Id
                    };

                    DbContext.RequestCoresearchers.Add(mrr);
                    DbContext.SaveChanges();
                }
                else if (mrr.ResearcherType != (byte)ResearcherRole.MainResearcher)
                {
                    mrr.ResearcherType = (byte)ResearcherRole.MainResearcher;
                    DbContext.SaveChanges();
                }

                model.Id = request.Id;
                model.Request = request;

                switch (step)
                {
                    case 1:
                        {
                            model.CollegeID = request.CollegeId;
                            model.DepartmentId = request.DepartmentId;
                            model.TitleArabic = request.Title;
                            model.TitleEnglish = request.TitleEnglish;
                            model.Duration = request.Duration;
                            model.PrimaryCategory = request.PrimaryCategoryId;
                            model.SecondaryCategory = request.SecondaryCategoryId;
                            model.PrimaryCategoryOther = request.PrimaryCategoryOther;
                            model.SecondaryCategoryOther = request.SecondaryCategoryOther;
                            model.IsPreviouslyApplied = request.IsPreviouslyApplied;
                            model.ApplicationNumber = request.ApplicationNumber;
                            model.NumberOfFundedProjects = request.NumberOfFundedProjects;
                            model.NumberOfExpectedPublications = request.NumberOfExpectedPublications;
                            model.WasProposalAccepted = request.WasProposalAccepted;
                            model.RefusalReason = request.RefusalReason;

                            if (request.PrimaryCategoryOther != null)
                            {
                                model.PrimaryCategory = 0;
                                model.SecondaryCategory = 0;
                            }
                            else if (request.SecondaryCategoryOther != null)
                            {
                                model.SecondaryCategory = 0;
                            }

                            model.IsWithinPriority = request.IsWithinPriority;
                            model.PriorityId = request.PriorityId;
                            model.PriorityOther = request.PriorityOther;

                            if (model.PriorityId > 0 || !string.IsNullOrWhiteSpace(model.PriorityOther))
                            {
                                model.IsWithinPriority = true;

                                if (model.PriorityId == null)
                                    model.PriorityId = 0;
                            }


                            break;
                        }

                    case 2:
                        {
                            if (request.RequestCoresearchers.Count(x => x.IsDeleted == false) > 0)
                            {
                                model.ProjectType = ProjectType.Team;

                                var tResearchers = request.RequestCoresearchers.Where(x => x.IsDeleted == false)
                                    .ToList()
                                    .OrderBy(r => r.Order)
                                    .Select(r =>
                                        new CoresearcherViewModel
                                        {
                                            Id = r.Researcher.Id,
                                            RoleInProject = r.ResearcherType == null ? ResearcherRole.AssistantResearcher : (ResearcherRole)r.ResearcherType,
                                            FullName = r.Researcher.GetFullName(),
                                            Qualification = r.Researcher.Qualification == null ? QualificationType.Unspecified : (QualificationType)r.Researcher.Qualification,
                                            UniversityId = r.Researcher.UniversityId == null && r.Researcher.UniversityOther != null ? 0 : r.Researcher.UniversityId,
                                            CollegeId = r.Researcher.CollegeId == null && r.Researcher.CollegeOther != null ? 0 : r.Researcher.CollegeId,
                                            DepartmentID = r.Researcher.DepartmentId == null && r.Researcher.DepartmentOther != null ? 0 : r.Researcher.DepartmentId,
                                            UniversityOther = r.Researcher.UniversityOther,
                                            CollegeOther = r.Researcher.CollegeOther,
                                            DepartmentOther = r.Researcher.DepartmentOther,
                                            MobileNumber = r.Researcher.MobileNo,
                                            Email = r.Researcher.Email,
                                            NationalID = r.Researcher.NationalId,
                                            Addresss = r.Researcher.Address,
                                            Birthdate = r.Researcher.DateOfBirth == null ? null : r.Researcher.DateOfBirth.Value.FormatDate(false, false, false),
                                            NationalityId = r.Researcher.NationalityId,
                                            CountryId = r.Researcher.CountryId,
                                            CityId = r.Researcher.CityId,
                                            Specialization = r.Researcher.Specialization,
                                            BankAccountHolder = r.Researcher.User.UserProfile.BankAccountHolder,
                                            BankIban = r.Researcher.User.UserProfile.BankIban,
                                            BankIbanPhotocopyReadonly = r.Researcher.User.UserFiles
                                                .Where(f => f.Type == (byte)FileType.IbanPhotocopy)
                                                .OrderByDescending(f => f.Created)
                                                .Select(c => new AttachedFile
                                                {
                                                    FileId = c.Id.ToString(),
                                                    Type = (FileType)c.Type,
                                                    FileName = Path.GetFileName(c.FileName)
                                                })
                                                .FirstOrDefault(),
                                            Bank = r.Researcher.User.UserProfile.BankId,
                                            BankOther = r.Researcher.User.UserProfile.BankName,
                                            BankBranch = r.Researcher.User.UserProfile.BankBranch,
                                            SectionPublishedPapers = r.Researcher.User.UserProfile.CvSectionPublishedPapers,
                                            SectionInterests = r.Researcher.User.UserProfile.CvSectionInterests,
                                            SectionCareerHistory = r.Researcher.User.UserProfile.CvSectionCareerHistory,
                                            SectionAcademicHistory = r.Researcher.User.UserProfile.CvSectionAcademicHistory,
                                            SectionTrainingCourses = r.Researcher.User.UserProfile.CvSectionTrainingCourses,
                                            SectionAwards = r.Researcher.User.UserProfile.CvSectionAwards,
                                            SectionMemberships = r.Researcher.User.UserProfile.CvSectionMemberships,
                                        }
                                    ).ToList();

                                model.Researchers.AddRange(tResearchers);
                            }

                            break;
                        }

                    case 3:
                        {
                            model.Abstract = request.Abstract;
                            model.AbstractEnglish = request.AbstractEnglish;

                            if (request.KeyWords != null)
                            {
                                var keywords = request.KeyWords.Split('|').Where(k => !string.IsNullOrWhiteSpace(k)).ToList();
                                if (keywords.Count > 0)
                                {
                                    model.Keywords.AddRange(keywords);
                                }
                            }

                            if (request.KeywordsEnglish != null)
                            {
                                var keywords = request.KeywordsEnglish.Split('|').Where(k => !string.IsNullOrWhiteSpace(k)).ToList();
                                if (keywords.Count > 0)
                                {
                                    model.KeywordsEnglish.AddRange(keywords);
                                }
                            }

                            if (type == ReqFieldType.AppliedScienceField)
                            {
                                model.SectionObjectives = request.SectionObjectives;
                                model.SectionRelevance = request.SectionRelevance;
                                model.SectionBackground = request.SectionBackground;
                                model.SectionMethodology = request.SectionMethodology;
                                model.SectionAppendices = request.SectionAppendices;

                                if (request.RequestFundingSources.Count() > 0)
                                {
                                    var tFundingSources = request.RequestFundingSources
                                        .OrderBy(r => r.Order)
                                        .ToList()
                                        .Select(r =>
                                            new FundingSourceViewModel
                                            {
                                                Id = r.Id,
                                                Name = r.Name,
                                                Duration = r.Duration,
                                                Amount = r.Amount,
                                                Status = r.Status > 0 ? (FundingStatus)r.Status : (FundingStatus?)null
                                            }
                                        ).ToList();

                                    model.FundingSources.AddRange(tFundingSources);
                                }
                            }
                            else
                            {
                                model.SectionImportance = request.SectionImportance;
                                model.SectionProblem = request.SectionProblem;
                                model.SectionGoals = request.SectionGoals;
                                model.SectionAssumptions = request.SectionAssumptions;
                                model.SectionTerms = request.SectionTerms;
                                model.SectionObjectiveLimitations = request.SectionObjectiveLimitations;
                                model.SectionSpatialLimitations = request.SectionSpatialLimitations;
                                model.SectionTemporalLimitations = request.SectionTemporalLimitations;
                                model.SectionTheoriticalFramework = request.SectionTheoriticalFramework;
                                model.SectionPreviousStudies = request.SectionPreviousStudies;
                                model.SectionProcedures = request.SectionProcedures;
                                model.SectionSociety = request.SectionSociety;
                                model.SectionTools = request.SectionTools;
                                model.SectionChapters = request.SectionChapters;
                                model.SectionReferences = request.SectionReferences;
                            }
                        }
                        break;

                    case 4:
                        {
                            if (request.RequestTimelines.Count() > 0)
                            {
                                var tActivities = request.RequestTimelines
                                    .OrderBy(r => r.Order)
                                    .ToList()
                                    .Select(r =>
                                        new TimelineViewModel
                                        {
                                            Id = r.Id,
                                            TaskName = r.TaskName,
                                            StartingMonth = r.StartingMonth,
                                            EndingMonth = r.EndingMonth,
                                            NumberOfVisits = r.NumberOfVisits,
                                            ResponsibleResearcherId = r.ResearcherId,
                                            ResponsibleResearcherName = r.Researcher.FirstName + ' ' + r.Researcher.LastName + GetResearcherRoleString(request, r.ResearcherId),
                                            IsConsultant = GetResearcherRole(request, r.ResearcherId) == ResearcherRole.Consultant
                                        }
                                    ).ToList();

                                model.Timeline.AddRange(tActivities);
                            }

                            if (Request["preview"] == "1")
                            {
                                var usCulture = System.Globalization.CultureInfo.GetCultureInfo("en-SA");

                                model.TimelinePreview = model.Timeline.Select(x => new TimelineGanttViewModel
                                {
                                    task = x.TaskName,
                                    type = "",
                                    startTimeInt = x.StartingMonth.Value,
                                    endTimeInt = x.EndingMonth.Value,
                                    startTime = new DateTime(TM.Now.Year + 1, 1, 1).AddMonths(x.StartingMonth.Value - 1).FirstOfMonth().ToString("yyyy-M-d", usCulture),
                                    endTime = new DateTime(TM.Now.Year + 1, 1, 1).AddMonths(x.EndingMonth.Value).AddSeconds(-1).ToString("yyyy-M-d", usCulture),
                                    duration = x.EndingMonth.Value - x.StartingMonth.Value + 1,
                                    startTimeStr = "",
                                    endTimeStr = "",
                                    details = x.ResponsibleResearcherName
                                }).ToList();

                                var midMonth = request.Duration / 2;

                                model.BudgetSummaryPerPerson = request.RequestCoresearchers.Where(x => x.IsDeleted == false)
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

                                model.FirstPeriodTotalCost = string.Format("{0:c}", request.FirstPeriodTotalCost).Replace("$", LangHelpers.Instance["sar"] + " ");
                                model.SecondPeriodTotalCost = string.Format("{0:c}", request.SecondPeriodTotalCost).Replace("$", LangHelpers.Instance["sar"] + " ");
                                model.MaterialsTotalCost = string.Format("{0:c}", request.MaterialsTotalCost).Replace("$", LangHelpers.Instance["sar"] + " ");
                                model.TotalCostForProject = string.Format("{0:c}", request.TotalCostForProject).Replace("$", LangHelpers.Instance["sar"] + " ");

                                model.TimelinePreview.Add(new TimelineGanttViewModel
                                {
                                    task = LangHelpers.Instance.GetLangValueByString("project_start", false, lang),
                                    type = "",
                                    startTimeInt = 1,
                                    endTimeInt = 1,
                                    startTime = new DateTime(TM.Now.Year + 1, 1, 1).ToString("yyyy-M-d", usCulture),
                                    endTime = new DateTime(TM.Now.Year + 1, 1, 1).ToString("yyyy-M-d", usCulture),
                                    startTimeStr = "",
                                    endTimeStr = "",
                                    details = ""
                                });
                                var random = new Random();

                                random.Next(0, 100);

                                model.TimelinePreview.Add(new TimelineGanttViewModel
                                {
                                    task = LangHelpers.Instance.GetLangValueByString("project_end", false, lang),
                                    type = "",
                                    startTimeInt = model.Request.Duration.Value,
                                    endTimeInt = model.Request.Duration.Value,
                                    startTime = new DateTime(TM.Now.Year + 1, 1, 1).AddMonths(model.Request.Duration.Value - 1).EndOfMonth().ToString("yyyy-M-d", usCulture),
                                    endTime = new DateTime(TM.Now.Year + 1, 1, 1).AddMonths(model.Request.Duration.Value - 1).EndOfMonth().ToString("yyyy-M-d", usCulture),
                                    startTimeStr = "",
                                    endTimeStr = "",
                                    details = ""
                                });

                                midMonth = model.Request.Duration / 2;
                                foreach (var t in model.TimelinePreview)
                                {
                                    var i = t.startTimeInt - 1;

                                    if (i < midMonth)
                                        t.startTimeStr = LangHelpers.Instance.GetLangValueByString("first_period", false, lang) + " - " + LangHelpers.Instance.GetLangValueByString("the_month", false, lang) + " " + (i + 1);
                                    else
                                        t.startTimeStr = LangHelpers.Instance.GetLangValueByString("second_period", false, lang) + " - " + LangHelpers.Instance.GetLangValueByString("the_month", false, lang) + " " + ((i - midMonth) + 1);

                                    i = t.endTimeInt - 1;

                                    if (i < midMonth)
                                        t.endTimeStr = LangHelpers.Instance.GetLangValueByString("first_period", false, lang) + " - " + LangHelpers.Instance.GetLangValueByString("the_month", false, lang) + " " + (i + 1);
                                    else
                                        t.endTimeStr = LangHelpers.Instance.GetLangValueByString("second_period", false, lang) + " - " + LangHelpers.Instance.GetLangValueByString("the_month", false, lang) + " " + ((i - midMonth) + 1);
                                }
                            }

                            if (type == ReqFieldType.AppliedScienceField)
                            {
                                model.BudgetRationale = request.BudgetRationale;

                                if (request.RequestMaterials.Count() > 0)
                                {
                                    var tActivities = request.RequestMaterials
                                        .OrderBy(r => r.Order)
                                        .Select(r =>
                                            new MaterialsViewModel
                                            {
                                                Id = r.Id,
                                                Name = r.Name,
                                                Amount = r.Amount,
                                                Cost = r.Cost,
                                                Usage = r.Usage,
                                                Type = (MaterialType?)r.Type,
                                                OtherType = r.OtherType
                                            }
                                        ).ToList();

                                    model.Materials.AddRange(tActivities);
                                }
                            }
                        }

                        break;

                    case 5:
                        {
                            if (request.RequestSuggestedReferees
                                .Where(x => x.CreatedBy == Utils.CurrentUser.Id)
                                .Count() > 0)
                            {
                                var tReferees = request.RequestSuggestedReferees
                                    .Where(x => x.CreatedBy == Utils.CurrentUser.Id)
                                    .OrderBy(r => r.Order)
                                    .Select(r =>
                                        new SuggestedRefereeViewModel
                                        {
                                            Id = r.Id,
                                            Name = r.Name,
                                            Degree = r.Qualification == null ? QualificationType.Unspecified : (QualificationType)r.Qualification,
                                            UniversityId = r.UniversityId,
                                            CollegeId = r.CollegeId,
                                            Specialization = r.Specialization,
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

                            break;
                        }

                    case 6:
                        {
                            if (request.RequestActivities.Count() > 0)
                            {
                                var tActivities = request.RequestActivities
                                    .OrderBy(r => r.Order)
                                    .Select(r =>
                                        new SuggestedActivityViewModel
                                        {
                                            Id = r.Id,
                                            Name = r.Name,
                                            Description = r.Description,
                                            Type = (ActivityType?)r.ActivityType,
                                            OtherType = r.ActivityTypeOther,
                                            Address = r.Address,
                                        }
                                    ).ToList();

                                model.SuggestedActivities.AddRange(tActivities);
                            }
                        }

                        {
                            if (request.RequestBeneficiaries.Count() > 0)
                            {
                                var tBeneficiaries = request.RequestBeneficiaries
                                    .OrderBy(r => r.Order)
                                    .Select(r =>
                                        new BeneficiaryViewModel
                                        {
                                            Id = r.Id,
                                            Name = r.Name,
                                            Description = r.Description,
                                            Address = r.Address,
                                        }
                                    ).ToList();

                                model.Beneficiaries.AddRange(tBeneficiaries);
                            }
                        }

                        {
                            if (request.RequestContainers.Count() > 0)
                            {
                                var tContainers = request.RequestContainers
                                    .OrderBy(r => r.Order)
                                    .Select(r =>
                                        new PublishContainerViewModel
                                        {
                                            Id = r.Id,
                                            Name = r.Name,
                                            SelectionReason = r.Description,
                                            Address = r.Address,
                                        }
                                    ).ToList();

                                model.PublishContainers.AddRange(tContainers);
                            }
                        }
                        break;
                }
            }

            if (step == 3 && model.Keywords == null)
            {
                model.Keywords = new List<string>();
            }

            if (step == 3 && model.Keywords.Count < 6)
            {
                for (var i = model.Keywords.Count; i < 6; i++)
                {
                    model.Keywords.Add("");
                }
            }

            if (step == 3 && type == ReqFieldType.AppliedScienceField && model.FundingSources.Count < 5)
            {
                for (var i = model.FundingSources.Count; i < 5; i++)
                {
                    model.FundingSources.Add(new FundingSourceViewModel { });
                }
            }

            if (step == 4 && model.Timeline.Count < 5)
            {
                for (var i = model.Timeline.Count; i < 5; i++)
                {
                    model.Timeline.Add(new TimelineViewModel { });
                }
            }

            if (step == 4 && model.Materials.Count < 5)
            {
                for (var i = model.Materials.Count; i < 5; i++)
                {
                    model.Materials.Add(new MaterialsViewModel { });
                }
            }

            if (step == 5 && model.SuggestedReferees.Count < 5)
            {
                for (var i = model.SuggestedReferees.Count; i < 5; i++)
                {
                    model.SuggestedReferees.Add(new SuggestedRefereeViewModel { });
                }
            }

            if (step == 6 && model.Beneficiaries.Count < 1)
            {
                for (var i = model.Beneficiaries.Count; i < 1; i++)
                {
                    model.Beneficiaries.Add(new BeneficiaryViewModel { });
                }
            }

            if (step == 6 && model.Beneficiaries.Count < 1)
            {
                for (var i = model.Beneficiaries.Count; i < 1; i++)
                {
                    model.Beneficiaries.Add(new BeneficiaryViewModel { });
                }
            }

            model.AnnouncementId = announcement.Id;

            model.Type = type.Value;
            model.StepCount = stepCount;
            model.Step = step;

            FillOutLookups(model);

            model.PrimaryCategory = 0;
            model.SecondaryCategory = 0;
            ViewBag.agreement = LangHelpers.Instance["agreement"];
            if (model.Request != null && model.Request.Status == (long)RequestStatus.FundingRequestPendingModificationsByResearcher)
            {
                ViewBag.agreement = LangHelpers.Instance["agreement_resubmit"];
            }


            if (model.Step == 1 && model.Priorities.Length == 1)
            {
                model.PriorityId = 0;
            }


            Utils.SwitchCulture(null);

            if (request != null)
            {
                if (request.Status == (long)RequestStatus.FundingRequestReturnedToResearcher)
                {
                    var requestUser = request.RequestUsers
                    .Where(ru => ru.Action == null && ru.UserId == request.CreatedBy)
                    .OrderByDescending(a => a.AssignedOn)
                    .FirstOrDefault();
                    if (requestUser != null)
                    {
                        TempData["message"] = LangHelpers.Instance.GetLangValueByString("returned_text", false, lang) + ": " + requestUser.Comments;
                        TempData["message_type"] = "danger";
                    }

                }
            }

            return View("~/Views/Requests/FundingRequests/New.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(CommonFundingRequestViewModel model)
        {
            if (model == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = this.New(model.Type, model.Step, model.Id);
            if (result is RedirectResult || result is RedirectToRouteResult)
            {
                return result;
            }

            var model2 = (CommonFundingRequestViewModel)((ViewResult)result).Model;

            if (model2.Request == null && model.CollegeID > 0 && model2.Step > 1)
            {
                model2.Request = new Request
                {
                    Type = (byte)Data.RequestType.FundingRequest,
                    Status = 0,
                    AnnouncementId = model2.AnnouncementId,
                    FieldType = (byte)model2.Type,
                    Created = TM.Now,
                    Updated = TM.Now,
                    CreatedBy = CurrentUser.Id,
                    UpdatedBy = CurrentUser.Id,
                };

                DbContext.Requests.Add(model2.Request);
                DbContext.SaveChanges();
            }


            model.HasCoresearchers = model2.HasCoresearchers;

            var lang = model.Type == ReqFieldType.AppliedScienceField ? "en" : null;

            if (Request.QueryString["preview"] == "1")
            {
                if (model.Preview == LangHelpers.Instance.GetLangValueByString("edit", false, lang))
                {
                    var link = Utils.GetFundingRequestLink(model2.Type, model2.Step, model2.Request.Id);

                    return Redirect(link);
                }
                else
                {

                    var link = Utils.GetFundingRequestLink(model2.Type, model2.Step + 1, model2.Request.Id);

                    return Redirect(link);
                }
            }

            var mainResearcher = DbContext.Researchers.Where(r => r.UserId == CurrentUser.Id).FirstOrDefault();

            if (mainResearcher == null)
            {
                TempData["message"] = LangHelpers.Instance.GetLangValueByString("researcher_data_is_not_found", false, lang);
                TempData["message_type"] = "danger";

                return RedirectToAction("Index", "Home");
            }

            // additional validation
            switch (model2.Step)
            {
                case 1:
                    {
                        if (model.Type == ReqFieldType.AppliedScienceField && string.IsNullOrWhiteSpace(model.TitleEnglish))
                        {
                            ModelState.AddModelError("TitleEnglish", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("title_in_english", false, lang)));
                        }

                        ModelState.Remove("PrimaryCategory");
                        ModelState.Remove("SecondaryCategory");
                        ModelState.Remove("PrimaryCategoryOther");
                        ModelState.Remove("SecondaryCategoryOther");

                        model.PrimaryCategory = null;
                        model.SecondaryCategory = null;



                        if (model.IsWithinPriority == false)
                        {
                            ModelState.Remove("PriorityOther");
                            ModelState.Remove("Priority");
                        }

                        if (model.IsPreviouslyApplied == false)
                        {
                            ModelState.Remove("RefusalReason");
                            ModelState.Remove("ApplicationNumber");
                        }


                        if (model.WasProposalAccepted == false)
                        {
                            ModelState.Remove("ApplicationNumber");
                        }

                        if (string.IsNullOrWhiteSpace(model.SecondaryCategoryOther))
                        {
                            ModelState.AddModelError("SecondaryCategoryOther", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("other_secondary_category", false, lang)));
                        }

                        if (string.IsNullOrWhiteSpace(model.PriorityOther))
                        {
                            if (model.PriorityId == 0)
                            {
                                ModelState.AddModelError("PriorityOther", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("priority", false, lang)));
                            }
                        }
                    }
                    break;

                case 2:
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
                    break;

                case 3:
                    {
                        if (string.IsNullOrWhiteSpace(model.Abstract) || Utils.StripHTML(model.Abstract).Trim().Length == 0)
                        {
                            ModelState.AddModelError("Abstract", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("abstract_in_arabic", false, lang)));
                        }

                        if (model.Keywords == null || model.Keywords.Count(k => !string.IsNullOrWhiteSpace(k)) < 6)
                        {
                            TempData["message"] = LangHelpers.Instance.GetLangValueByString("must_provide_6_keywords", false, lang);
                            TempData["message_type"] = "danger";

                            ModelState.AddModelError("Keywords", LangHelpers.Instance.GetLangValueByString("must_provide_6_keywords", false, lang));
                        }

                        if (model.Type == ReqFieldType.AppliedScienceField)
                        {
                            if ((model.KeywordsEnglish == null || model.KeywordsEnglish.Count(k => !string.IsNullOrWhiteSpace(k)) < 6))
                            {
                                TempData["message"] = LangHelpers.Instance.GetLangValueByString("must_provide_6_keywords", false, lang);
                                TempData["message_type"] = "danger";

                                ModelState.AddModelError("KeywordsEnglish", LangHelpers.Instance.GetLangValueByString("must_provide_6_keywords", false, lang));
                            }

                            if (string.IsNullOrWhiteSpace(model.AbstractEnglish) || Utils.StripHTML(model.AbstractEnglish).Trim().Length == 0)
                            {
                                ModelState.AddModelError("AbstractEnglish", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("abstract_in_english", false, lang)));
                            }
                        }

                        if (model2.Type == ReqFieldType.AppliedScienceField)
                        {
                            if (string.IsNullOrWhiteSpace(model.SectionObjectives) || Utils.StripHTML(model.SectionObjectives).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionObjectives", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("applied_section_objectives", false, lang)));
                            }

                            if (string.IsNullOrWhiteSpace(model.SectionRelevance) || Utils.StripHTML(model.SectionRelevance).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionRelevance", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("applied_section_relevance", false, lang)));
                            }

                            if (string.IsNullOrWhiteSpace(model.SectionBackground) || Utils.StripHTML(model.SectionBackground).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionBackground", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("applied_section_background", false, lang)));
                            }

                            if (string.IsNullOrWhiteSpace(model.SectionMethodology) || Utils.StripHTML(model.SectionMethodology).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionMethodology", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("applied_section_methodology", false, lang)));
                            }

                            if (string.IsNullOrWhiteSpace(model.SectionAppendices) || Utils.StripHTML(model.SectionAppendices).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionAppendices", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("applied_section_appendices", false, lang)));
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(model.SectionImportance) || Utils.StripHTML(model.SectionImportance).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionImportance", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("section_project_importance", false, lang)));
                            }
                            if (string.IsNullOrWhiteSpace(model.SectionProblem) || Utils.StripHTML(model.SectionProblem).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionProblem", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("section_problem", false, lang)));
                            }
                            if (string.IsNullOrWhiteSpace(model.SectionGoals) || Utils.StripHTML(model.SectionGoals).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionGoals", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("section_goals", false, lang)));
                            }
                            if (string.IsNullOrWhiteSpace(model.SectionTerms) || Utils.StripHTML(model.SectionTerms).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionTerms", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("section_terms", false, lang)));
                            }
                            if (string.IsNullOrWhiteSpace(model.SectionPreviousStudies) || Utils.StripHTML(model.SectionPreviousStudies).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionPreviousStudies", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("section_previous_studies", false, lang)));
                            }
                            if (string.IsNullOrWhiteSpace(model.SectionChapters) || Utils.StripHTML(model.SectionChapters).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionChapters", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("section_chapters", false, lang)));
                            }
                            if (string.IsNullOrWhiteSpace(model.SectionReferences) || Utils.StripHTML(model.SectionReferences).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionReferences", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("section_references", false, lang)));
                            }
                        }
                    }
                    break;

                case 4:
                    if (model.Timeline == null || model.Timeline.Count(t => t.TaskName != null) < 1)
                    {
                        TempData["message"] = LangHelpers.Instance.GetLangValueByString("must_provide_1_task", false, lang);
                        TempData["message_type"] = "danger";

                        ModelState.AddModelError("Timeline", LangHelpers.Instance.GetLangValueByString("must_provide_1_task", false, lang));
                    }
                    else
                    {
                        var i = 0;

                        var sumPerConsultant = new Dictionary<long, int>();

                        foreach (var t in model.Timeline)
                        {
                            var role = GetResearcherRole(model2.Request, t.ResponsibleResearcherId);

                            if (role == ResearcherRole.Consultant)
                            {
                                t.IsConsultant = true;
                                ModelState.Remove("Timeline[" + i + "].EndingMonth");

                                if (!(t.NumberOfVisits > 0))
                                {
                                    ModelState.AddModelError("Timeline[" + i + "].NumberOfVisits", LangHelpers.Instance.GetLangValueByString("number_of_visits_is_required", false, lang));
                                }
                                else
                                {
                                    if (sumPerConsultant.ContainsKey(t.ResponsibleResearcherId.Value))
                                    {
                                        sumPerConsultant[t.ResponsibleResearcherId.Value] += t.NumberOfVisits.Value;
                                    }
                                    else
                                    {
                                        sumPerConsultant.Add(t.ResponsibleResearcherId.Value, t.NumberOfVisits.Value);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.Remove("Timeline[" + i + "].NumberOfVisits");

                                if (t.EndingMonth < t.StartingMonth)
                                {
                                    ModelState.AddModelError("Timeline[" + i + "].StartingMonth", LangHelpers.Instance.GetLangValueByString("end_before_start", false, lang));
                                }
                            }

                            i++;
                        }

                        i = 0;
                        foreach (var t in model.Timeline)
                        {
                            var role = GetResearcherRole(model2.Request, t.ResponsibleResearcherId);

                            if (role == ResearcherRole.Consultant)
                            {
                                var isInsideSaudi = model2.Request.RequestCoresearchers.Count(cr => cr.ResearcherId == t.ResponsibleResearcherId && cr.CountryId == 1 && cr.IsDeleted == false) > 0;

                                if (isInsideSaudi)
                                {
                                    if (sumPerConsultant[t.ResponsibleResearcherId.Value] > 14)
                                    {
                                        var name = DbContext.Researchers.FirstOrDefault(r => r.Id == t.ResponsibleResearcherId.Value).GetFullName();
                                        ModelState.AddModelError("Timeline_" + i + "_", string.Format(LangHelpers.Instance.GetLangValueByString("number_of_visits_exceeded_14", false, lang), name));

                                        // to display one error per consultant
                                        sumPerConsultant[t.ResponsibleResearcherId.Value] = 0;
                                    }
                                }
                                else
                                {
                                    if (sumPerConsultant[t.ResponsibleResearcherId.Value] > 10)
                                    {
                                        var name = DbContext.Researchers.FirstOrDefault(r => r.Id == t.ResponsibleResearcherId.Value).GetFullName();
                                        ModelState.AddModelError("Timeline_" + i + "_", string.Format(LangHelpers.Instance.GetLangValueByString("number_of_visits_exceeded_10", false, lang), name));

                                        // to display one error per consultant
                                        sumPerConsultant[t.ResponsibleResearcherId.Value] = 0;
                                    }
                                }
                            }

                            i++;
                        }

                    }

                    if (model2.Type == ReqFieldType.AppliedScienceField)
                    {
                        if (model.Materials.Count(r => !string.IsNullOrWhiteSpace(r.Name)) > 0 && (string.IsNullOrWhiteSpace(model.BudgetRationale) || Utils.StripHTML(model.BudgetRationale).Trim().Length == 0))
                        {
                            ModelState.AddModelError("BudgetRationale", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("applied_budget_rationale", false, lang)));
                        }


                    }

                    break;

                case 5:
                    {
                        if (model.SuggestedReferees == null || model.SuggestedReferees.Count < 5)
                        {
                            TempData["message"] = LangHelpers.Instance.GetLangValueByString("must_provide_5_referees", false, lang);
                            TempData["message_type"] = "danger";

                            ModelState.AddModelError("SuggestedReferes", LangHelpers.Instance.GetLangValueByString("must_provide_5_referees", false, lang));
                        }
                        else
                        {
                            var encounteredMobileNumbers = new Dictionary<string, int>();
                            var encounteredEmails = new Dictionary<string, int>();
                            // Verify Email, Phone number uniqueness in Referees
                            var totalEmails = 0;
                            var totalPhones = 0;
                            for (var i = 0; i < model.SuggestedReferees.Count; i++)
                            {
                                if (string.IsNullOrWhiteSpace(model.SuggestedReferees[i].Name)) continue;

                                if (model.SuggestedReferees[i].MobileNumber != null)
                                {
                                    totalPhones++;
                                    var phone = "";
                                    if (Utils.ValidateMobile(model.SuggestedReferees[i].MobileNumber, out phone))
                                    {
                                        model.SuggestedReferees[i].MobileNumber = phone;

                                        if (encounteredMobileNumbers.ContainsKey(model.SuggestedReferees[i].MobileNumber))
                                        {
                                            encounteredMobileNumbers[model.SuggestedReferees[i].MobileNumber]++;
                                        }
                                        else
                                        {
                                            encounteredMobileNumbers.Add(model.SuggestedReferees[i].MobileNumber, 1);
                                        }
                                    }
                                }

                                if (model.SuggestedReferees[i].Email != null)
                                {
                                    model.SuggestedReferees[i].Email = model.SuggestedReferees[i].Email.ToLower().Trim();
                                    totalEmails++;
                                    if (encounteredEmails.ContainsKey(model.SuggestedReferees[i].Email))
                                    {
                                        encounteredEmails[model.SuggestedReferees[i].Email]++;
                                    }
                                    else
                                    {
                                        encounteredEmails.Add(model.SuggestedReferees[i].Email, 1);
                                    }
                                }
                            }
                            if (totalEmails > 0 || totalPhones > 0)
                            {
                                // Verify Email, Phone number uniqueness in Researchers
                                var researchers = DbContext.RequestCoresearchers.Where(r => r.RequestId == model.Id && r.IsDeleted == false).ToList();
                                if (researchers.Count() > 0)
                                {
                                    for (var i = 0; i < model.Researchers.Count; i++)
                                    {
                                        if (string.IsNullOrWhiteSpace(model.Researchers[i].FullName)) continue;

                                        if (model.Researchers[i].MobileNumber != null)
                                        {
                                            var phone = "";
                                            Utils.ValidateMobile(model.Researchers[i].MobileNumber, out phone);
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

                                        if (model.Researchers[i].Email != null)
                                        {
                                            model.Researchers[i].Email = model.Researchers[i].Email.ToLower().Trim();

                                            if (encounteredEmails.ContainsKey(model.Researchers[i].Email))
                                            {
                                                encounteredEmails[model.Researchers[i].Email]++;
                                            }
                                            else
                                            {
                                                encounteredEmails.Add(model.Researchers[i].Email, 1);
                                            }
                                        }
                                    }
                                }

                            }

                            if (encounteredMobileNumbers.Count(k => k.Value > 1) > 0)
                            {
                                ModelState.AddModelError("SuggestedReferees", LangHelpers.Instance.GetLangValueByString("request_contains_duplicate_phones", false, lang));
                            }

                            if (encounteredEmails.Count(k => k.Value > 1) > 0)
                            {
                                ModelState.AddModelError("SuggestedReferees", LangHelpers.Instance.GetLangValueByString("request_contains_duplicate_emails", false, lang));
                            }
                        }
                    }
                    break;

                case 6:
                    {
                        {
                            if (model.SuggestedActivities == null || model.SuggestedActivities.Count(t => t.Name != null) < 1)
                            {
                                ModelState.AddModelError("SuggestedActivities", LangHelpers.Instance.GetLangValueByString("must_provide_1_activity", false, lang));
                            }


                            if (model.Beneficiaries == null || model.Beneficiaries.Count(t => t.Name != null) < 1)
                            {
                                ModelState.AddModelError("Beneficiaries", LangHelpers.Instance.GetLangValueByString("must_provide_1_beneficiary", false, lang));
                            }


                            if (model.PublishContainers == null || model.PublishContainers.Count(t => t.Name != null) < 1)
                            {
                                ModelState.AddModelError("PublishContainers", LangHelpers.Instance.GetLangValueByString("must_provide_1_publish_container", false, lang));
                            }
                        }
                        break;
                    }
            }

            if (ModelState.IsValid)
            {
                bool redirectToRequestPage = false;
                var request = model2.Request;
                if (request != null)
                {
                    if (request.Status == (long)RequestStatus.FundingRequestPendingModificationsByResearcher)
                    {
                        (new RefereeUtil()).CreateRequestRevision(request.Id, false);
                        redirectToRequestPage = true;
                    }
                    else if (request.Status == (long)RequestStatus.FundingRequestReturnedToResearcher)
                    {
                        redirectToRequestPage = true;
                    }

                    if (request.RequestCoresearchers != null)
                    {
                        if (request.RequestCoresearchers.Count > 0)
                        {
                            request.RequestCoresearchers = request.RequestCoresearchers.Where(x => x.IsDeleted == false).ToList();
                        }
                    }
                }

                var trans = (DbContextTransaction)null;
                try
                {
                    using (trans = DbContext.Database.BeginTransaction())
                    {
                        if (request == null)
                        {
                            request = new Data.DbModels.Request
                            {
                                Type = (byte)Data.RequestType.FundingRequest,
                                CreatedBy = CurrentUser.Id,
                                Created = TM.Now
                            };

                            DbContext.Requests.Add(request);
                        }

                        switch (model.Step)
                        {
                            case 1:
                                {
                                    request.CollegeId = model.CollegeID;
                                    request.DepartmentId = model.DepartmentId;
                                    request.Title = model.TitleArabic;
                                    request.TitleEnglish = model.TitleEnglish;
                                    request.Duration = model.Duration;
                                    request.PrimaryCategoryId = !string.IsNullOrWhiteSpace(model.PrimaryCategoryOther) ? null : model.PrimaryCategory;
                                    request.PrimaryCategoryOther = string.IsNullOrWhiteSpace(model.PrimaryCategoryOther) ? null : model.PrimaryCategoryOther;
                                    request.SecondaryCategoryId = !string.IsNullOrWhiteSpace(model.SecondaryCategoryOther) ? null : model.SecondaryCategory;
                                    request.SecondaryCategoryOther = string.IsNullOrWhiteSpace(model.SecondaryCategoryOther) ? null : model.SecondaryCategoryOther;
                                    request.IsPreviouslyApplied = model.IsPreviouslyApplied;
                                    request.ApplicationNumber = model.IsPreviouslyApplied != true ? null : model.ApplicationNumber;
                                    request.NumberOfFundedProjects = model.NumberOfFundedProjects;
                                    request.NumberOfExpectedPublications = model.NumberOfExpectedPublications;
                                    request.WasProposalAccepted = model.WasProposalAccepted;
                                    request.RefusalReason = model.RefusalReason;
                                    request.IsWithinPriority = model.IsWithinPriority == true;
                                    request.PriorityId = model.IsWithinPriority == true && model.PriorityId > 0 ? model.PriorityId : null;
                                    request.PriorityOther = model.PriorityOther;
                                }
                                break;

                            case 2:
                                {
                                    request.NoOfParticipant = model.Researchers.Count;

                                    var encounteredResearcherIds = new HashSet<long>();
                                    var coresearchers = request.RequestCoresearchers.Where(x => x.IsDeleted == false).ToList();

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
                                }

                                break;

                            case 3:
                                {
                                    request.KeyWords = string.Join("|", model.Keywords.Where(k => !string.IsNullOrWhiteSpace(k)));
                                    request.KeywordsEnglish = string.Join("|", model.KeywordsEnglish.Where(k => !string.IsNullOrWhiteSpace(k)));
                                    request.Abstract = model.Abstract;
                                    request.AbstractEnglish = model.AbstractEnglish;
                                    if (model2.Type == ReqFieldType.AppliedScienceField)
                                    {
                                        request.SectionObjectives = model.SectionObjectives;
                                        request.SectionRelevance = model.SectionRelevance;
                                        request.SectionBackground = model.SectionBackground;
                                        request.SectionMethodology = model.SectionMethodology;
                                        request.SectionAppendices = model.SectionAppendices;

                                        request.SectionImportance = null;
                                        request.SectionProblem = null;
                                        request.SectionGoals = null;
                                        request.SectionAssumptions = null;
                                        request.SectionTerms = null;
                                        request.SectionObjectiveLimitations = null;
                                        request.SectionSpatialLimitations = null;
                                        request.SectionTemporalLimitations = null;
                                        request.SectionTheoriticalFramework = null;
                                        request.SectionPreviousStudies = null;
                                        request.SectionProcedures = null;
                                        request.SectionSociety = null;
                                        request.SectionTools = null;
                                        request.SectionChapters = null;
                                        request.SectionReferences = null;

                                        var encounteredIds = new HashSet<long>();
                                        model.FundingSources = model.FundingSources.Where(r => r.Name != null).ToList();

                                        var tFundingSources = request.RequestFundingSources.ToList();

                                        for (var i = 0; i < model.FundingSources.Count; i++)
                                        {
                                            var rmodel = model.FundingSources[i];

                                            var fs = DbContext.RequestFundingSources
                                                .OrderByDescending(r => r.Id)
                                                .Where(r => r.RequestId == request.Id)
                                                .FirstOrDefault(r => rmodel.Id > 0 && rmodel.Id == r.Id);

                                            if (fs == null)
                                            {
                                                fs = new RequestFundingSource
                                                {
                                                    RequestId = request.Id,
                                                    Created = TM.Now,
                                                    CreatedBy = CurrentUser.Id
                                                };

                                                DbContext.RequestFundingSources.Add(fs);
                                            }

                                            fs.Name = rmodel.Name;
                                            fs.Amount = rmodel.Amount;
                                            fs.Duration = rmodel.Duration;
                                            fs.Status = (byte?)rmodel.Status;
                                            fs.Order = i + 1;

                                            fs.Updated = TM.Now;
                                            fs.UpdatedBy = CurrentUser.Id;

                                            DbContext.SaveChanges();

                                            encounteredIds.Add(fs.Id);
                                        }

                                        foreach (var cr in tFundingSources)
                                        {
                                            if (!encounteredIds.Contains(cr.Id))
                                            {
                                                DbContext.Entry(cr).State = EntityState.Deleted;
                                            }
                                        }

                                        DbContext.SaveChanges();
                                    }
                                    else
                                    {
                                        request.SectionImportance = model.SectionImportance;
                                        request.SectionProblem = model.SectionProblem;
                                        request.SectionGoals = model.SectionGoals;
                                        request.SectionAssumptions = model.SectionAssumptions;
                                        request.SectionTerms = model.SectionTerms;
                                        request.SectionObjectiveLimitations = model.SectionObjectiveLimitations;
                                        request.SectionSpatialLimitations = model.SectionSpatialLimitations;
                                        request.SectionTemporalLimitations = model.SectionTemporalLimitations;
                                        request.SectionTheoriticalFramework = model.SectionTheoriticalFramework;
                                        request.SectionPreviousStudies = model.SectionPreviousStudies;
                                        request.SectionProcedures = model.SectionProcedures;
                                        request.SectionSociety = model.SectionSociety;
                                        request.SectionTools = model.SectionTools;
                                        request.SectionChapters = model.SectionChapters;
                                        request.SectionReferences = model.SectionReferences;

                                        request.SectionObjectives = null;
                                        request.SectionRelevance = null;
                                        request.SectionBackground = null;
                                        request.SectionMethodology = null;
                                        request.SectionAppendices = null;
                                    }

                                    break;
                                }

                            case 4:
                                var totalCost = (double)0;

                                {
                                    var encounteredIds = new HashSet<long>();
                                    model.Timeline = model.Timeline.Where(r => r.TaskName != null).ToList();

                                    var tasks = request.RequestTimelines.ToList();

                                    for (var i = 0; i < model.Timeline.Count; i++)
                                    {
                                        var rmodel = model.Timeline[i];

                                        var task = DbContext.RequestTimelines
                                            .OrderByDescending(r => r.Id)
                                            .Where(r => r.RequestId == request.Id)
                                            .FirstOrDefault(r => rmodel.Id > 0 && rmodel.Id == r.Id);

                                        if (task == null)
                                        {
                                            task = new RequestTimeline
                                            {
                                                RequestId = request.Id,
                                                Created = TM.Now,
                                                CreatedBy = CurrentUser.Id
                                            };

                                            DbContext.RequestTimelines.Add(task);
                                        }

                                        task.TaskName = rmodel.TaskName;
                                        task.ResearcherId = rmodel.ResponsibleResearcherId.Value;
                                        task.Order = i + 1;
                                        task.StartingMonth = rmodel.StartingMonth;

                                        var role = GetResearcherRole(model2.Request, rmodel.ResponsibleResearcherId);

                                        if (role == ResearcherRole.Consultant)
                                        {
                                            task.EndingMonth = rmodel.StartingMonth;
                                            task.NumberOfVisits = rmodel.NumberOfVisits;
                                        }
                                        else
                                        {
                                            task.EndingMonth = rmodel.EndingMonth;
                                            task.NumberOfVisits = null;
                                        }

                                        task.Updated = TM.Now;
                                        task.UpdatedBy = CurrentUser.Id;

                                        DbContext.SaveChanges();

                                        encounteredIds.Add(task.Id);
                                    }

                                    foreach (var cr in tasks)
                                    {
                                        if (!encounteredIds.Contains(cr.Id))
                                        {
                                            DbContext.Entry(cr).State = EntityState.Deleted;
                                        }
                                    }

                                    DbContext.SaveChanges();

                                    var firstPeriodTotalCost = (double)0;
                                    var secondPeriodTotalCost = (double)0;

                                    var midMonth = request.Duration / 2;

                                    foreach (var r in request.RequestCoresearchers)
                                    {
                                        var rtasks = request.RequestTimelines
                                           .Where(rt => rt.ResearcherId == r.ResearcherId)
                                           .ToList();

                                        var role = GetResearcherRole(request, r.ResearcherId);
                                        if (role == ResearcherRole.Consultant)
                                        {
                                            r.FirstPeriodActiveMonths = rtasks
                                                .Where(rt => rt.StartingMonth <= midMonth)
                                                .Select(rt => rt.NumberOfVisits)
                                                .Sum();

                                            r.SecondPeriodActiveMonths = rtasks
                                                .Where(rt => rt.StartingMonth > midMonth)
                                                .Select(rt => rt.NumberOfVisits)
                                                .Sum();
                                        }
                                        else
                                        {
                                            r.FirstPeriodActiveMonths = 0;
                                            r.SecondPeriodActiveMonths = 0;

                                            for (var i = 1; i <= request.Duration; i++)
                                            {
                                                if (rtasks.Count(x => i >= x.StartingMonth && i <= x.EndingMonth) > 0)
                                                {
                                                    if (i <= midMonth)
                                                    {
                                                        r.FirstPeriodActiveMonths++;
                                                    }
                                                    else
                                                    {
                                                        r.SecondPeriodActiveMonths++;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    var isMainResearcherFromAhsaShariaCollege = Utils.db.Colleges.Count(c => c.Id == mainResearcher.CollegeId && c.NameArabic == "الشريعة بالأحساء") > 0;

                                    foreach (var r in request.RequestCoresearchers)
                                    {
                                        var isConsultant = r.ResearcherType == (byte)ResearcherRole.Consultant;

                                        r.TotalActiveMonths = r.FirstPeriodActiveMonths + r.SecondPeriodActiveMonths;

                                        r.FirstPeriodCost = Utils.CalculateCostForPerson(
                                            (ResearcherRole)r.ResearcherType,
                                            (QualificationType)r.Researcher.Qualification,
                                            r.CountryId > 0 ? r.CountryId.Value : 0,
                                            isConsultant && isMainResearcherFromAhsaShariaCollege ? 0 : (r.CityId > 0 ? r.CityId.Value : 0),
                                            r.FirstPeriodActiveMonths.Value
                                        );

                                        r.SecondPeriodCost = Utils.CalculateCostForPerson(
                                            (ResearcherRole)r.ResearcherType,
                                            (QualificationType)r.Researcher.Qualification,
                                            r.CountryId > 0 ? r.CountryId.Value : 0,
                                            isConsultant && isMainResearcherFromAhsaShariaCollege ? 0 : (r.CityId > 0 ? r.CityId.Value : 0),
                                            r.SecondPeriodActiveMonths.Value);

                                        r.TotalCost = r.FirstPeriodCost + r.SecondPeriodCost;

                                        firstPeriodTotalCost += r.FirstPeriodCost.Value;
                                        secondPeriodTotalCost += r.SecondPeriodCost.Value;
                                    }

                                    request.FirstPeriodTotalCost = firstPeriodTotalCost;
                                    request.SecondPeriodTotalCost = secondPeriodTotalCost;
                                    totalCost += firstPeriodTotalCost;
                                    totalCost += secondPeriodTotalCost;
                                }

                                if (request.FieldType == (byte)ReqFieldType.AppliedScienceField)
                                {
                                    request.BudgetRationale = model.BudgetRationale;
                                    model.Materials = model.Materials.Where(r => r.Name != null).ToList();

                                    var totalMaterialCost = (double)0;
                                    var encounteredIds = new HashSet<long>();
                                    var tasks = request.RequestMaterials.ToList();

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

                                        totalMaterialCost += material.Cost;

                                        encounteredIds.Add(material.Id);
                                    }

                                    request.MaterialsTotalCost = totalMaterialCost;
                                    totalCost += totalMaterialCost;

                                    foreach (var cr in tasks)
                                    {
                                        if (!encounteredIds.Contains(cr.Id))
                                        {
                                            DbContext.Entry(cr).State = EntityState.Deleted;
                                        }
                                    }

                                    DbContext.SaveChanges();
                                }

                                request.TotalCostForProject = totalCost;
                                break;

                            case 5:
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
                                                RequestId = request.Id,
                                                Created = TM.Now,
                                                CreatedBy = CurrentUser.Id
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
                                        referee.Specialization = rmodel.Specialization;
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

                                    break;
                                }

                            case 6:
                                {
                                    model.SuggestedActivities = model.SuggestedActivities.Where(r => r.Name != null).ToList();
                                    var encounteredActivityIds = new HashSet<long>();
                                    var referees = request.RequestActivities.ToList();

                                    for (var i = 0; i < model.SuggestedActivities.Count; i++)
                                    {
                                        var rmodel = model.SuggestedActivities[i];

                                        var activity = DbContext.RequestActivities
                                            .OrderByDescending(r => r.Id)
                                            .Where(r => r.RequestId == request.Id)
                                            .FirstOrDefault(r => rmodel.Id > 0 && rmodel.Id == r.Id);

                                        if (activity == null)
                                        {
                                            activity = new RequestActivity
                                            {
                                                RequestId = request.Id,
                                                Created = TM.Now,
                                                CreatedBy = CurrentUser.Id
                                            };

                                            DbContext.RequestActivities.Add(activity);
                                        }

                                        activity.Name = rmodel.Name;
                                        activity.Description = rmodel.Name;
                                        activity.Order = i + 1;
                                        activity.Address = rmodel.Address;
                                        activity.ActivityType = !string.IsNullOrWhiteSpace(rmodel.OtherType) ? null : (byte?)rmodel.Type;
                                        activity.ActivityTypeOther = rmodel.Type > 0 ? null : rmodel.OtherType;

                                        activity.Updated = TM.Now;
                                        activity.UpdatedBy = CurrentUser.Id;

                                        DbContext.SaveChanges();

                                        encounteredActivityIds.Add(activity.Id);
                                    }

                                    foreach (var cr in referees)
                                    {
                                        if (!encounteredActivityIds.Contains(cr.Id))
                                        {
                                            DbContext.Entry(cr).State = EntityState.Deleted;
                                        }
                                    }

                                    DbContext.SaveChanges();
                                }

                                {
                                    model.Beneficiaries = model.Beneficiaries.Where(r => r.Name != null).ToList();
                                    var encounteredBeneficiaryIds = new HashSet<long>();
                                    var referees = request.RequestBeneficiaries.ToList();

                                    for (var i = 0; i < model.Beneficiaries.Count; i++)
                                    {
                                        var rmodel = model.Beneficiaries[i];

                                        var activity = DbContext.RequestBeneficiaries
                                            .OrderByDescending(r => r.Id)
                                            .Where(r => r.RequestId == request.Id)
                                            .FirstOrDefault(r => rmodel.Id > 0 && rmodel.Id == r.Id);

                                        if (activity == null)
                                        {
                                            activity = new RequestBeneficiary
                                            {
                                                RequestId = request.Id,
                                                Created = TM.Now,
                                                CreatedBy = CurrentUser.Id
                                            };

                                            DbContext.RequestBeneficiaries.Add(activity);
                                        }

                                        activity.Name = rmodel.Name;
                                        activity.Description = rmodel.Name;
                                        activity.Order = i + 1;
                                        activity.Address = rmodel.Address;

                                        activity.Updated = TM.Now;
                                        activity.UpdatedBy = CurrentUser.Id;

                                        DbContext.SaveChanges();

                                        encounteredBeneficiaryIds.Add(activity.Id);
                                    }

                                    foreach (var cr in referees)
                                    {
                                        if (!encounteredBeneficiaryIds.Contains(cr.Id))
                                        {
                                            DbContext.Entry(cr).State = EntityState.Deleted;
                                        }
                                    }

                                    DbContext.SaveChanges();
                                }

                                {
                                    model.PublishContainers = model.PublishContainers.Where(r => r.Name != null).ToList();
                                    var encounteredContainerIds = new HashSet<long>();
                                    var referees = request.RequestContainers.ToList();

                                    for (var i = 0; i < model.PublishContainers.Count; i++)
                                    {
                                        var rmodel = model.PublishContainers[i];

                                        var container = DbContext.RequestContainers
                                            .OrderByDescending(r => r.Id)
                                            .Where(r => r.RequestId == request.Id)
                                            .FirstOrDefault(r => rmodel.Id > 0 && rmodel.Id == r.Id);

                                        if (container == null)
                                        {
                                            container = new RequestContainer
                                            {
                                                RequestId = request.Id,
                                                Created = TM.Now,
                                                CreatedBy = CurrentUser.Id
                                            };

                                            DbContext.RequestContainers.Add(container);
                                        }

                                        container.Name = rmodel.Name;
                                        container.Description = rmodel.Name;
                                        container.Order = i + 1;
                                        container.Address = rmodel.Address;

                                        container.Updated = TM.Now;
                                        container.UpdatedBy = CurrentUser.Id;

                                        DbContext.SaveChanges();

                                        encounteredContainerIds.Add(container.Id);
                                    }

                                    foreach (var cr in referees)
                                    {
                                        if (!encounteredContainerIds.Contains(cr.Id))
                                        {
                                            DbContext.Entry(cr).State = EntityState.Deleted;
                                        }
                                    }

                                    DbContext.SaveChanges();
                                }

                                break;
                        }

                        request.AnnouncementId = model2.AnnouncementId;

                        if (request.Status != (long)RequestStatus.FundingRequestReturnedToResearcher && request.Status != (long)RequestStatus.FundingRequestPendingModificationsByResearcher)
                        {
                            request.Status = (long)Data.RequestStatus.NotSubmitted;
                        }
                        request.Updated = TM.Now;

                        request.UpdatedBy = CurrentUser.Id;
                        request.FieldType = (byte)model2.Type;

                        if (!(request.LastUpdatedStep > model.Step))
                            request.LastUpdatedStep = (byte)model.Step;

                        if (model.Step == model2.StepCount)
                        {
                            if (String.IsNullOrEmpty(request.UniqueId))
                            {
                                request.UniqueId = RequestUtils.GenerateUniqueId(request.Id, request.AnnouncementId.Value, request.CollegeId.Value);
                            }


                            if (Constants.EnableCoresearchersNotifications)
                            {
                                if (model.HasCoresearchers)
                                {
                                    foreach (var rr in request.RequestCoresearchers)
                                    {
                                        if (rr.ResearcherType == (byte)ResearcherRole.MainResearcher)
                                            rr.DidResearcherAgree = true;
                                        else
                                            rr.DidResearcherAgree = null;
                                    }

                                    if (request.Status != (long)RequestStatus.FundingRequestPendingModificationsByResearcher && request.Status != (long)RequestStatus.FundingRequestReturnedToResearcher)
                                    {
                                        request.Status = (long)RequestStatus.FundingRequestPendingCoresearchers;
                                    }
                                }
                                else
                                {
                                    if (request.Status != (long)RequestStatus.FundingRequestPendingModificationsByResearcher && request.Status != (long)RequestStatus.FundingRequestReturnedToResearcher)
                                    {
                                        request.Status = (long)RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies;
                                    }
                                }
                            }
                            else
                            {
                                if (request.Status != (long)RequestStatus.FundingRequestPendingModificationsByResearcher && request.Status != (long)RequestStatus.FundingRequestReturnedToResearcher)
                                {
                                    request.Status = (long)RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies;
                                }
                            }

                            var isResubmitted = request.RequestHistoryLogs.Count(r => r.Action == (byte)RequestAction.Submit && r.CreatedBy == CurrentUser.Id) > 0;

                            var db = Utils.db;
                            db.RequestHistoryLogs.Add(new RequestHistoryLog
                            {
                                RequestId = request.Id,
                                Role = (byte)Roles.FirstOrDefault(),
                                CollegeId = CurrentUser.CollegeId,
                                IsImpersonating = UserUtils.IsImpersonating,

                                Url = Utils.GetFullUrl(),
                                Status = request.Status,
                                Action = (byte)(isResubmitted ? RequestAction.Resubmit : RequestAction.Submit),
                                UserAgent = Request.UserAgent,
                                IpAddress = Utils.GetUserIP(),
                                Created = TM.Now,
                                CreatedBy = UserId,
                            });
                        }

                        trans.Commit();
                        DbContext.SaveChanges();


                        if (model.Step == 4 && Request.QueryString["preview"] != "1")
                        {
                            var link = Utils.GetFundingRequestLink(model2.Type, model2.Step, request.Id);
                            link += "&preview=1";

                            return Redirect(link);
                        }

                        if (model.Step == model2.StepCount)
                        {
                            TempData["message"] = "تم إرسال الطلب بنجاح";
                            TempData["message_type"] = "success";
                            if (redirectToRequestPage)
                            {
                                return Redirect(Utils.GetApplicationUrl() + "Requests/View/" + request.Id);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            TempData["message"] = "تم حفظ البيانات بنجاح";
                            TempData["message_type"] = "success";
                            return Redirect(Utils.GetFundingRequestLink(model2.Type, model2.Step + 1, request.Id));
                        }
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    Logger.LogExceptionAsync(ex, model);
                    if (trans != null)
                    {
                        try
                        {
                            trans.Rollback();
                        }
                        catch
                        {
                        }
                    }
                    string error = ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;

                    TempData["message"] = error;
                    TempData["message_type"] = "danger";
                }
                catch (Exception ex)
                {
                    Logger.LogExceptionAsync(ex, model);
                    if (trans != null)
                    {
                        try
                        {
                            trans.Rollback();
                        }
                        catch
                        {
                        }
                    }
                    TempData["message"] = ex.SanitizeException();
                    TempData["message_type"] = "danger";
                }
            }

            model.AnnouncementId = model2.AnnouncementId;
            model.Type = model2.Type;
            model.StepCount = model2.StepCount;
            model.Step = model2.Step;
            model.Request = model2.Request;

            FillOutLookups(model);

            model.SecondaryCategory = 0;
            model.PrimaryCategory = 0;

            if (model.Step == 1 && model.Priorities.Length == 1)
            {
                model.PriorityId = 0;
            }


            Utils.SwitchCulture(null);

            return View("~/Views/Requests/FundingRequests/New.cshtml", model);
        }

        [NonAction]
        public void FillOutLookups(CommonFundingRequestViewModel model)
        {
            var lang = model.Type == ReqFieldType.AppliedScienceField ? "en" : null;

            var listYesNo = new List<BetterSelectListItem>();
            listYesNo.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("No", false, lang), Value = "0" });
            listYesNo.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("Yes", false, lang), Value = "1" });
            model.OptionSelection = listYesNo.ToArray();


            if (model.Step == 1 || model.Step == 0)
            {
                {
                    var list = DbContext.Departments
                        .Where(d => model.CollegeID > 0 && d.CollegeId == model.CollegeID)
                        .Where(d => d.Field == null || d.Field == (byte)model.Type)
                        .ToArray()
                        .Select(c => new BetterSelectListItem
                        {
                            Text = c.LocalizedName(),
                            Value = c.Id.ToString()
                        })
                        .OrderBy(c => c.Text)
                        .ToList();

                    model.Departments = list.ToArray();
                }
                {
                    var list = DbContext.Colleges
                        .Where(c => c.Field == null || c.Field == (byte)model.Type)
                   .ToArray()
                   .Select(c => new BetterSelectListItem
                   {
                       Text = c.LocalizedName(),
                       Value = c.Id.ToString()
                   })
                   .OrderBy(c => c.Text)
                   .ToList();

                    model.Colleges = list.ToArray();
                }

                {
                    var list = DbContext.Universities
               .ToArray()
               .OrderBy(c => c.Id == 1 ? 0 : 1)
               .ThenBy(c => c.LocalizedName())
               .Select(c => new BetterSelectListItem
               {
                   Text = c.LocalizedName(),
                   Value = c.Id.ToString()
               })
               .ToList();

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Universities = list.ToArray();
                }

                {
                    var list = DbContext.Categories
              .Where(c => c.Level == 1)
              .ToList()
              .Select(c => new BetterSelectListItem
              {
                  Text = c.LocalizedName(),
                  Value = c.Id.ToString()
              })
              .OrderBy(c => c.Text)
              .ToList();

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.PrimaryCategories = list.ToArray();
                }

                {
                    if (model.PrimaryCategory > 0)
                    {
                        var list = DbContext.Categories
                           .Where(c => c.ParentId == model.PrimaryCategory && c.Level == 2)
                           .ToArray()
                           .Select(c => new BetterSelectListItem
                           {
                               Text = c.LocalizedName(),
                               Value = c.Id.ToString()
                           })
                           .OrderBy(c => c.Text)
                           .ToList();

                        list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                        model.SecondaryCategories = list.ToArray();
                    }
                    else
                    {
                        model.SecondaryCategories = new BetterSelectListItem[0];
                    }
                }
                {
                    var list = new List<BetterSelectListItem>();
                    for (var i = 6; i <= 36; i += 6) list.Add(new BetterSelectListItem { Value = i.ToString(), Text = i.ToString() });

                    model.Durations = list.ToArray();
                }

                {
                    var list = new List<BetterSelectListItem>();
                    var priority = DbContext.DepartmentPriorities.FirstOrDefault(p => model.PriorityId > 0 && p.Id == model.PriorityId);

                    var prios = (model.DepartmentId > 0 ? DbContext.DepartmentPriorities.Where(dp => dp.DepartmentId == model.DepartmentId).ToList() : new List<DepartmentPriority>());

                    list = prios
                        .Select(c => new BetterSelectListItem
                        {
                            Text = c.Priority.LocalizedName(),
                            Value = c.PriorityId.ToString()
                        })
                        .OrderBy(c => c.Text)
                        .ToList();

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Priorities = list.ToArray();
                }
            }

            if (model.Step == 2 || model.Step == 0)
            {
                {
                    ViewBag.IncludeCKEditor = true;
                    ViewBag.LargerWidth = true;

                    var list = DbContext.Colleges
                   .ToArray()
                   .Select(c => new BetterSelectListItem
                   {
                       Text = c.LocalizedName(),
                       Value = c.Id.ToString(),
                       ParentId = c.UniversityId.ToString()
                   })
                   .OrderBy(c => c.Text)
                   .ToList();

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Colleges = list.ToArray();
                }

                {
                    var list = DbContext.Universities
               .ToArray()
               .OrderBy(c => c.Id == 1 ? 0 : 1)
               .ThenBy(c => c.LocalizedName())
               .Select(c => new BetterSelectListItem
               {
                   Text = c.LocalizedName(),
                   Value = c.Id.ToString()
               })
               .ToList();

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Universities = list.ToArray();
                }

                {
                    var list = DbContext.Departments
               .ToArray()
               .Select(c => new BetterSelectListItem
               {
                   Text = c.LocalizedName(),
                   Value = c.Id.ToString(),
                   ParentId = c.CollegeId.ToString()
               })
               .OrderBy(c => c.Text)
               .ToList();

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Departments = list.ToArray();
                }

                {
                    var values = Enum.GetValues(typeof(ResearcherRole));
                    var list = new List<BetterSelectListItem>();

                    var i = 0;
                    foreach (var v in values)
                    {
                        if (v.ToString() == ResearcherRole.Unspecified.ToString() || v.ToString() == ResearcherRole.MainResearcher.ToString()) continue;

                        list.Add(new BetterSelectListItem
                        {
                            Value = v.ToString(),
                            Text = LangHelpers.Instance[v.ToString()]
                        });

                        i++;
                    }

                    model.ResearcherRoles = list.ToArray();
                }

                {
                    var values = Utils.QualificationTypesByResearcherType();
                    var list = new List<BetterSelectListItem>();

                    var i = 0;
                    foreach (var v in values)
                    {
                        if (v.ToString() == "Unspecified") continue;

                        list.Add(new BetterSelectListItem
                        {
                            Value = v.ToString(),
                            Text = LangHelpers.Instance[v.ToString()]
                        });

                        i++;
                    }

                    model.QualificationTypes = list.ToArray();
                }

                {
                    var list = DbContext.Countries
                        .ToArray()
                        .OrderBy(c => c.Id == 1 ? 0 : 1)
                        .ThenBy(c => c.LocalizedName())
                         .Select(c => new BetterSelectListItem
                         {
                             Text = c.LocalizedName(),
                             Value = c.Id.ToString()
                         })
                        .ToList();

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Countries = list.ToArray();
                }

                {
                    var list = DbContext.Cities
                        .ToArray()
                        .Select(c => new CityViewModel
                        {
                            Id = c.Id,
                            CountryId = c.CountryId,
                            Name = c.LocalizedName(),
                        })
                        .OrderBy(c => c.Name)
                        .ToList();

                    list.Add(new CityViewModel { Name = LangHelpers.Instance.GetLangValueByString("other", false, lang), Id = 0 });

                    model.Cities = list;
                }
                {
                    var list = DbContext.Banks
                        .ToArray()
                        .Select(c => new BetterSelectListItem
                        {
                            Text = c.LocalizedName(),
                            Value = c.Id.ToString()
                        })
                        .OrderBy(c => c.Text)
                        .ToList();

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Banks = list.ToArray();
                }

            }

            if (model.Step == 3 || model.Step == 0)
            {

                {
                    ViewBag.IncludeCKEditor = true;
                    ViewBag.LargerWidth = true;
                    ViewBag.WidthMaximized = false;
                }

                {
                    var values = Enum.GetValues(typeof(FundingStatus));
                    var list = new List<BetterSelectListItem>();

                    var i = 0;
                    foreach (var v in values)
                    {
                        if (v.ToString() == FundingStatus.Unspecified.ToString()) continue;

                        list.Add(new BetterSelectListItem
                        {
                            Value = v.ToString(),
                            Text = LangHelpers.Instance[v.ToString()]
                        });

                        i++;
                    }

                    model.FundingStatuses = list.ToArray();
                }

            }

            if (model.Step == 4 || model.Step == 0)
            {
                {
                    ViewBag.WidthMaximized = true;
                    ViewBag.AllowPreview = true;

                    var id = model.Request == null ? (long?)null : model.Request.Id;
                    var userId = model.Request == null ? (string)null : model.Request.CreatedBy;
                    var list = DbContext.Researchers
                        .Where(r => id != null && r.RequestCoresearchers.Count(cr => cr.RequestId == id && cr.IsDeleted == false) > 0)
                   .ToArray()
                   .Select(c => new BetterSelectListItem
                   {
                       Text = c.GetFullName() + GetResearcherRoleString(model.Request, c.Id),
                       Value = c.Id.ToString()
                   })
                   .OrderBy(c => c.Text)
                   .ToList();

                    model.ResearchersLookup = list.ToArray();
                }

                if (model.Type == ReqFieldType.AppliedScienceField)
                {
                    ViewBag.IncludeCKEditor = true;
                }

                {
                    if (model.Request == null || !(model.Request.Duration > 0))
                    {
                        model.Months = new BetterSelectListItem[0];
                    }
                    else
                    {
                        var list = new List<BetterSelectListItem>();
                        for (var i = 0; i < model.Request.Duration; i++)
                        {
                            if (i < (model.Request.Duration / 2))
                                list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("first_period", false, lang) + " - " + LangHelpers.Instance.GetLangValueByString("the_month", false, lang) + " " + (i + 1), Value = (i + 1) + "" });
                            else
                                list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("second_period", false, lang) + " - " + LangHelpers.Instance.GetLangValueByString("the_month", false, lang) + " " + (i + 1), Value = (i + 1) + "" });
                        }

                        model.Months = list.ToArray();
                    }
                }

                if (model.Type == ReqFieldType.AppliedScienceField)
                {
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
                }
            }

            if (model.Step == 5 || model.Step == 0)
            {
                {
                    var list = DbContext.Colleges
                   .ToArray()
                   .Select(c => new BetterSelectListItem
                   {
                       Text = c.LocalizedName(),
                       Value = c.Id.ToString()
                   })
                   .OrderBy(c => c.Text)
                   .ToList();

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Colleges = list.ToArray();
                }

                {
                    var list = DbContext.Universities
               .ToArray()
               .OrderBy(c => c.Id == 1 ? 0 : 1)
               .ThenBy(c => c.LocalizedName())
               .Select(c => new BetterSelectListItem
               {
                   Text = c.LocalizedName(),
                   Value = c.Id.ToString()
               })
               .ToList();

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Universities = list.ToArray();
                }

                {
                    var list = DbContext.Departments
               .ToArray()
               .Select(c => new BetterSelectListItem
               {
                   Text = c.LocalizedName(),
                   Value = c.Id.ToString()
               })
               .OrderBy(c => c.Text)
               .ToList();

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Departments = list.ToArray();
                }

                {
                    var values = Utils.QualificationTypesByResearcherType(role: ArfUserRole.Referee);
                    var list = new List<BetterSelectListItem>();

                    var i = 0;
                    foreach (var v in values)
                    {
                        if (v.ToString() == "Unspecified") continue;

                        list.Add(new BetterSelectListItem
                        {
                            Value = v.ToString(),
                            Text = LangHelpers.Instance[v.ToString()]
                        });

                        i++;
                    }

                    model.QualificationTypes = list.ToArray();
                }

            }

            if (model.Step == 1 || model.Step == 0)
            {
                {
                    ViewBag.LargerWidth = true;

                    var values = Enum.GetValues(typeof(ActivityType));
                    var list = new List<BetterSelectListItem>();

                    var i = 0;
                    foreach (var v in values)
                    {
                        if (v.ToString() == "Unspecified") continue;

                        list.Add(new BetterSelectListItem
                        {
                            Value = v.ToString(),
                            Text = LangHelpers.Instance[v.ToString()]
                        });

                        i++;
                    }

                    list.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.ActivityTypes = list.ToArray();
                }


            }
        }

        [NonAction]
        private ResearcherRole? GetResearcherRole(Request request, long? researcherId)
        {
            if (request == null || !(researcherId > 0)) return null;
            var coresearcher = request.RequestCoresearchers.FirstOrDefault(rr => rr.ResearcherId == researcherId.Value && rr.IsDeleted == false);
            if (coresearcher == null || !(coresearcher.ResearcherType > 0))
            {
                if (coresearcher != null && coresearcher.Researcher.UserId == request.CreatedBy)
                    return ResearcherRole.MainResearcher;
                return null;
            }

            return (ResearcherRole)coresearcher.ResearcherType;
        }

        [NonAction]
        private string GetResearcherRoleString(Request request, long? researcherId)
        {
            if (request == null || !(researcherId > 0)) return "";
            var coresearcher = request.RequestCoresearchers.FirstOrDefault(rr => rr.ResearcherId == researcherId.Value && rr.IsDeleted == false);
            if (coresearcher == null || !(coresearcher.ResearcherType > 0))
            {
                if (coresearcher != null && coresearcher.Researcher.UserId == request.CreatedBy)
                    return " (" + LangHelpers.Instance[(ResearcherRole.MainResearcher).ToString()] + ")";
                return "";
            }

            return " (" + LangHelpers.Instance[((ResearcherRole)coresearcher.ResearcherType).ToString()] + ")";
        }
    }
}