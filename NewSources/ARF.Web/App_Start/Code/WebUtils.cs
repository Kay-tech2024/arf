using ARF.Core;
using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using ARF.Web.Controllers;
using ARF.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.App_Start.Code
{
    public static class WebUtils
    {
        public static RequestSummaryTabbedViewModel GetTabbedRequestSummary(long requestId, ReqFieldType Type)
        {
            var RequestSummary = new RequestSummaryTabbedViewModel();
            RequestSummary.Type = Type;

            RequestSummaryTabViewModel _summaryData = new RequestSummaryTabViewModel();
            _summaryData.TabId = "0";
            _summaryData.TabName = LangHelpers.Instance["current_version"];
            _summaryData.RequestId = requestId;
            RequestSummary.tabs.Add(_summaryData);

            var revisions = Utils.db.Requests.Where(x => x.RequestRevisionId == requestId).OrderByDescending(x => x.Created).ToList();
            int tabId = revisions.Count();
            foreach (var revision in revisions)
            {
                _summaryData = new RequestSummaryTabViewModel();
                _summaryData.TabId = tabId.ToString();
                _summaryData.TabName = LangHelpers.Instance["revision"] + " " + tabId.ToString();
                _summaryData.RequestId = revision.Id;
                RequestSummary.tabs.Add(_summaryData);
                tabId--;
            }

            return RequestSummary;
        }

        public static RequestSummaryViewModel2 GetRequestSummary(long requestId, string tabId="1")
        {
            var db = Utils.db;

            var req = Utils.db.Requests.Find(requestId);

            return GetRequestSummary(req, tabId);
        }

        public static RequestSummaryViewModel2 GetRequestSummary(Request req, string tabId = "1")
        {
            var DbContext = Utils.db;
            if (req == null || !(req.Id > 0)) return null;
            var model = new RequestSummaryViewModel2();
            var id = req.Id;
            var type = (ReqFieldType?)req.FieldType;
            model.TabId = tabId;
            if (!(id > 0)) id = 0;
            var request = (Request)null;
            var lang = (string)null;

            if (type == null)
                type = ReqFieldType.Unspecified;

            var Request = HttpContext.Current.Request;
            var CurrentUser = Utils.RequestUtils.CurrentUser;
            var RequestUtils = Utils.RequestUtils;
            var stepCount = (byte)1;
            var announcement = (Announcement)null;

            if (id > 0)
            {
                request = RequestUtils.GetRequests().FirstOrDefault(r => r.Id == id && (r.Type == (byte)Data.RequestType.FundingRequest || type == null || type == ReqFieldType.Unspecified || r.FieldType == (byte)type.Value));

                if (request == null)
                {
                    request = req;
                }

                if (type == null && request.FieldType > 0)
                {
                    type = (ReqFieldType)request.FieldType.Value;
                }
            }

            if (type == ReqFieldType.Unspecified)
            {
                if (request != null && request.FieldType != null)
                {
                    type = (ReqFieldType)request.FieldType;
                }
                else
                {
                    FillOutLookups(model);
                    return model;
                }
            }

            lang = type == ReqFieldType.AppliedScienceField ? "en" : null;

            announcement = req.Announcement;

            model.Researchers = new List<CoresearcherViewModel>();
            model.ProjectType = ProjectType.Single;

            if (request != null)
            {
                model.Id = request.Id;
                model.Request = request;

                {
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
                    }

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
                    }

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


                        {
                            var usCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

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

                            if (model.Request.Duration != null)
                            {
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
                            }
                            else
                            {
                                model.TimelinePreview.Add(new TimelineGanttViewModel
                                {
                                    task = LangHelpers.Instance.GetLangValueByString("project_end", false, lang),
                                    type = "",
                                    startTimeInt = model.Request.Duration.Value,
                                    endTimeInt = model.Request.Duration.Value,
                                    startTime = new DateTime(TM.Now.Year + 1, 1, 1).AddMonths(11).EndOfMonth().ToString("yyyy-M-d", usCulture),
                                    endTime = new DateTime(TM.Now.Year + 1, 1, 1).AddMonths(11).EndOfMonth().ToString("yyyy-M-d", usCulture),
                                    startTimeStr = "",
                                    endTimeStr = "",
                                    details = ""
                                });
                            }

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
                                        }
                                    ).ToList();

                                model.Materials.AddRange(tActivities);
                            }
                        }
                    }

                    {
                        if (request.RequestSuggestedReferees
                            .Where(x => x.CreatedBy == req.CreatedBy)
                            .Count() > 0)
                        {
                            var tReferees = request.RequestSuggestedReferees
                                .Where(x => x.CreatedBy == req.CreatedBy)
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
                    }
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
                }
            }


            if (announcement != null)
                model.AnnouncementId = announcement.Id;

            model.Type = type.Value;
            model.StepCount = stepCount;
            model.Step = 1;

            FillOutLookups(model);

            model.PrimaryCategory = 0;
            model.SecondaryCategory = 0;

            if (model.Step == 1 && model.Priorities.Length == 1)
            {
                model.PriorityId = 0;
            }

            //Utils.db.DiscardChanges();

            return model;
        }

        public static CommonFundingRequestViewModel GetEditRequestForm(long Id)
        {

            Request request = Utils.db.Requests.Where(r =>
                  r.Id == Id &&
                  r.Type == (byte)Data.RequestType.FundingRequest &&
                  r.Status == (long)RequestStatus.Completed

              )
              .OrderByDescending(r => r.Created)
              .FirstOrDefault();

            var user = Utils.GetCurrentUserDetails();
            var model = (CommonFundingRequestViewModel)null;
            ReqFieldType type = (ReqFieldType)request.Type;

            switch (type)
            {
                case ReqFieldType.ArabicLanguageField:
                    model = new ArabicFundingRequestViewModel();
                    break;

                case ReqFieldType.IslamicField:
                    model = new IslamicFundingRequestViewModel();
                    break;

                case ReqFieldType.AppliedScienceField:
                    model = new ScientificFundingRequestViewModel();
                    break;

                case ReqFieldType.SocialField:
                    model = new SocialScienceFundingRequestViewModel();
                    break;
            }
            model.CollegeID = request.CollegeId;
            model.DepartmentId = request.DepartmentId;
            model.HasCoresearchers = request != null && request.RequestCoresearchers.Count(r => r.Researcher != null && r.Researcher.UserId != r.CreatedBy && r.IsDeleted == false) > 0;
            model.Researchers = new List<CoresearcherViewModel>();
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

            if (request.FieldType == (byte)ReqFieldType.AppliedScienceField)
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

            model.Duration = request.Duration;
            model.Type = (ReqFieldType)request.FieldType;
            model.Step = 0;
            FillOutLookups(model);

            var list = new List<BetterSelectListItem>();
            for (var i = 6; i <= 18; i += 6) list.Add(new BetterSelectListItem { Value = i.ToString(), Text = i.ToString() });
            model.Durations = list.ToArray();

            return model;
        }
        private static ResearcherRole? GetResearcherRole(Request request, long? researcherId)
        {
            if (request == null || !(researcherId > 0)) return null;
            var coresearcher = request.RequestCoresearchers.Where(x => x.IsDeleted == false).FirstOrDefault(rr => rr.ResearcherId == researcherId.Value);
            if (coresearcher == null || !(coresearcher.ResearcherType > 0))
            {
                if (coresearcher != null && coresearcher.Researcher.UserId == request.CreatedBy)
                    return ResearcherRole.MainResearcher;
                return null;
            }

            return (ResearcherRole)coresearcher.ResearcherType;
        }

        private static string GetResearcherRoleString(Request request, long? researcherId)
        {
            if (request == null || !(researcherId > 0)) return "";
            var coresearcher = request.RequestCoresearchers.Where(x => x.IsDeleted == false).FirstOrDefault(rr => rr.ResearcherId == researcherId.Value);
            if (coresearcher == null || !(coresearcher.ResearcherType > 0))
            {
                if (coresearcher != null && coresearcher.Researcher.UserId == request.CreatedBy)
                    return " (" + LangHelpers.Instance[(ResearcherRole.MainResearcher).ToString()] + ")";
                return "";
            }

            return " (" + LangHelpers.Instance[((ResearcherRole)coresearcher.ResearcherType).ToString()] + ")";
        }

        private static void FillOutLookups(RequestSummaryViewModel2 model)
        {
            var lang = model.Type == ReqFieldType.AppliedScienceField ? "en" : null;

            if (model.Type == ReqFieldType.AppliedScienceField)
            {
                Utils.SwitchCulture("EN");
            }

            var DbContext = Utils.db;

            {
                {
                    var list = DbContext.Departments
                        .Where(d => model.CollegeID > 0 && d.CollegeId == model.CollegeID)
                        .Where(d => d.Field == null || d.Field == (byte)model.Type)
                        .ToArray()
                        .Select(c => new SelectListItem
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
                   .Select(c => new SelectListItem
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
                    var list = DbContext.Categories
              .Where(c => c.Level == 1)
              .ToList()
              .Select(c => new SelectListItem
              {
                  Text = c.LocalizedName(),
                  Value = c.Id.ToString()
              })
              .OrderBy(c => c.Text)
              .ToList();

                    list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.PrimaryCategories = list.ToArray();
                }

                {
                    if (model.PrimaryCategory > 0)
                    {
                        var list = DbContext.Categories
                           .Where(c => c.ParentId == model.PrimaryCategory && c.Level == 2)
                           .ToArray()
                           .Select(c => new SelectListItem
                           {
                               Text = c.LocalizedName(),
                               Value = c.Id.ToString()
                           })
                           .OrderBy(c => c.Text)
                           .ToList();

                        list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                        model.SecondaryCategories = list.ToArray();
                    }
                    else
                    {
                        model.SecondaryCategories = new SelectListItem[0];
                    }
                }
                {
                    var list = new List<SelectListItem>();
                    for (var i = 6; i <= 36; i += 6) list.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });

                    model.Durations = list.ToArray();
                }

                {
                    var list = new List<SelectListItem>();
                    var priority = DbContext.DepartmentPriorities.FirstOrDefault(p => model.PriorityId > 0 && p.Id == model.PriorityId);

                    var prios = priority != null ? priority.Department.DepartmentPriorities.ToList() : (model.DepartmentId > 0 ? DbContext.DepartmentPriorities.Where(dp => dp.DepartmentId == model.DepartmentId).ToList() : new List<DepartmentPriority>());

                    list = prios
                        .Select(c => new SelectListItem
                        {
                            Text = c.Priority.LocalizedName(),
                            Value = c.PriorityId.ToString()
                        })
                        .OrderBy(c => c.Text)
                        .ToList();

                    list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Priorities = list.ToArray();
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
                    var values = Enum.GetValues(typeof(ResearcherRole));
                    var list = new List<SelectListItem>();

                    var i = 0;
                    foreach (var v in values)
                    {
                        if (v.ToString() == ResearcherRole.Unspecified.ToString() || v.ToString() == ResearcherRole.MainResearcher.ToString()) continue;

                        list.Add(new SelectListItem
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

                {
                    var list = DbContext.Countries
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
                        .Select(c => new SelectListItem
                        {
                            Text = c.LocalizedName(),
                            Value = c.Id.ToString()
                        })
                        .OrderBy(c => c.Text)
                        .ToList();

                    list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.Banks = list.ToArray();
                }

                {
                    var values = Enum.GetValues(typeof(FundingStatus));
                    var list = new List<SelectListItem>();

                    var i = 0;
                    foreach (var v in values)
                    {
                        if (v.ToString() == FundingStatus.Unspecified.ToString()) continue;

                        list.Add(new SelectListItem
                        {
                            Value = v.ToString(),
                            Text = LangHelpers.Instance[v.ToString()]
                        });

                        i++;
                    }

                    model.FundingStatuses = list.ToArray();
                }
                {
                    var id = model.Request == null ? (long?)null : model.Request.Id;
                    var userId = model.Request == null ? (string)null : model.Request.CreatedBy;
                    var list = DbContext.Researchers
                        .Where(r => id != null && r.RequestCoresearchers.Count(cr => cr.RequestId == id && cr.IsDeleted == false) > 0)
                   .ToArray()
                   .Select(c => new SelectListItem
                   {
                       Text = c.GetFullName() + GetResearcherRoleString(model.Request, c.Id),
                       Value = c.Id.ToString()
                   })
                   .OrderBy(c => c.Text)
                   .ToList();

                    model.ResearchersLookup = list.ToArray();
                }

                {
                    if (model.Request == null || !(model.Request.Duration > 0))
                    {
                        model.Months = new SelectListItem[0];
                    }
                    else
                    {
                        var list = new List<SelectListItem>();
                        for (var i = 0; i < model.Request.Duration; i++)
                        {
                            if (i < (model.Request.Duration / 2))
                                list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("first_period", false, lang) + " - " + LangHelpers.Instance.GetLangValueByString("the_month", false, lang) + " " + (i + 1), Value = (i + 1) + "" });
                            else
                                list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("second_period", false, lang) + " - " + LangHelpers.Instance.GetLangValueByString("the_month", false, lang) + " " + (i + 1), Value = (i + 1) + "" });
                        }

                        model.Months = list.ToArray();
                    }
                }

                if (model.Type == ReqFieldType.AppliedScienceField)
                {
                    var values = Enum.GetValues(typeof(MaterialType));
                    var list = new List<SelectListItem>();

                    var i = 0;
                    foreach (var v in values)
                    {
                        if (v.ToString() == MaterialType.Unspecified.ToString()) continue;

                        list.Add(new SelectListItem
                        {
                            Value = v.ToString(),
                            Text = LangHelpers.Instance[v.ToString()]
                        });

                        i++;
                    }


                    list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });


                    model.MaterialTypes = list.ToArray();
                }
                else
                {
                    model.MaterialTypes = new SelectListItem[0];
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
                {
                    var values = Enum.GetValues(typeof(ActivityType));
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

                    list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, lang), Value = "0" });

                    model.ActivityTypes = list.ToArray();
                }
            }

            Utils.SwitchCulture(null);
        }

        private static void FillOutLookups(CommonFundingRequestViewModel model)
        {
            var lang = model.Type == ReqFieldType.AppliedScienceField ? "en" : null;

            if (model.Type == ReqFieldType.AppliedScienceField)
            {
                Utils.SwitchCulture("EN");
            }

            var listYesNo = new List<BetterSelectListItem>();
            listYesNo.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("No", false, lang), Value = "0" });
            listYesNo.Add(new BetterSelectListItem { Text = LangHelpers.Instance.GetLangValueByString("Yes", false, lang), Value = "1" });
            model.OptionSelection = listYesNo.ToArray();

            var DbContext = Utils.db;

            {                               

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

                    var prios = priority != null ? priority.Department.DepartmentPriorities.ToList() : (model.DepartmentId > 0 ? DbContext.DepartmentPriorities.Where(dp => dp.DepartmentId == model.DepartmentId).ToList() : new List<DepartmentPriority>());

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
                {
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
                {
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
                else
                {
                    model.MaterialTypes = new BetterSelectListItem[0];
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
                {
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

            Utils.SwitchCulture(null);
        }

        public static string GetRoleNameOfUser(User _u, ArfUserRole? role)
        {
            string roleName = String.Empty;            
            if (_u != null)
            {
                if (_u.Roles.Count() == 0)
                {
                    roleName = LangHelpers.Instance["none"];
                }
                else
                {
                    roleName = LangHelpers.Instance[_u.Roles.FirstOrDefault().Name];
                    if ((Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch) && role == ArfUserRole.ResearchUnitMember))
                    {
                        roleName = LangHelpers.Instance["ResearchUnitMember"];
                        if (_u.College != null)
                        {
                            if (_u.College.RoleGroupCode == 1)
                            {
                                roleName = LangHelpers.Instance["research_unit_member_new"];
                            }
                        }
                    }
                    else if (Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects) && role == ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects)
                    {
                        roleName = LangHelpers.Instance["MembersOfTheStandingCommitteeForFinancingResearchProjects"];
                    }
                    else if (_u.College != null && _u.Roles.FirstOrDefault().Name == ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch.ToString())
                    {
                        GetViceDeanForHigherStudiesAndScientificResearchSubRoleName(_u.College.RoleGroupCode.Value, _u.Roles.FirstOrDefault().Name);
                    }
                }
            }
            
            return roleName;
        }

        public static string GetViceDeanForHigherStudiesAndScientificResearchSubRoleName(int RoleGroupCode, string RoleKey, int CollegeResearchUnitCode = 0)
        {
            string roleName = String.Empty;
            if (RoleKey == ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch.ToString())
            {
                if (CollegeResearchUnitCode == 0)
                {
                    switch (RoleGroupCode)
                    {
                        case 1:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_college");
                            break;
                        case 2:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_institute");
                            break;
                        default:
                            roleName = LangHelpers.Instance[RoleKey];
                            break;
                    }
                }
                else
                {
                    switch (CollegeResearchUnitCode)
                    {
                        case 15:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_15");
                            break;
                        case 14:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_14");
                            break;
                        case 13:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_13");
                            break;
                        case 12:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_12");
                            break;
                        case 11:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_11");
                            break;
                        case 10:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_10");
                            break;
                        case 9:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_9");
                            break;
                        case 8:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_8");
                            break;
                        case 7:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_7");
                            break;
                        case 6:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_6");
                            break;
                        case 5:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_5");
                            break;
                        case 4:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_4");
                            break;
                        case 3:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_3");
                            break;
                        case 2:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_2");
                            break;
                        case 1:
                            roleName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_rolename_college_code_1");
                            break;
                        default:
                            roleName = LangHelpers.Instance[RoleKey];
                            break;
                    }

                }

            }
            else
            {
                roleName = LangHelpers.Instance[RoleKey];
            }

            return roleName;
        }

        public static string GetViceDeanForHigherStudiesAndScientificResearchRequestStatusName(string StatusKey, int CollegeCode = 0)
        {
            string StatusName = String.Empty;

            switch (CollegeCode)
            {
                case 15:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_15");
                    break;
                case 14:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_14");
                    break;
                case 13:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_13");
                    break;
                case 12:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_12");
                    break;
                case 11:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_11");
                    break;
                case 10:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_10");
                    break;
                case 9:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_9");
                    break;
                case 8:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_8");
                    break;
                case 7:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_7");
                    break;
                case 6:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_6");
                    break;
                case 5:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_5");
                    break;
                case 4:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_4");
                    break;
                case 3:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_3");
                    break;
                case 2:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_2");
                    break;
                case 1:
                    StatusName = LangHelpers.Instance.GetLangValueByString("vice_dean_for_higher_studies_and_scientific_research_statusname_college_code_1");
                    break;
                default:
                    StatusName = LangHelpers.Instance[StatusName];
                    break;

            }

            return StatusName;
        }

        public static RefereeFundingRequestFeedbackPartialViewModel GetRefereeFundingRequestFeedbackDetails(long requestId, int refereeId, ReqFieldType ReqType, bool IsReadonly)
        {
            var _refereeUtil = new RefereeUtil();
            var questions = _refereeUtil.GetRefereeRequestFeedbackForm(requestId, ReqType);
            var answers = Utils.db.RefereeRequestFeedbacks.Where(x => x.RequestId == requestId && x.RefereeId == refereeId && x.IsDeleted == false).ToList();
            var _response = new RefereeFundingRequestFeedbackPartialViewModel();
            _response.FeedbackFormQuestions = new List<RefereeFundingRequestFeedbackViewModel>();
            var isSelectedLangArabic = Utils.IsUserLanguageArabic;
            foreach (var question in questions)
            {
                var ret = new RefereeFundingRequestFeedbackViewModel();
                var _answer = answers.Where(x => x.FeedbackFormQuestionId == question.Id).FirstOrDefault();

                ret.RequestId = requestId;
                ret.RefereeId = refereeId;
                ret.FeedbackFormQuestionId = question.Id;
                ret.MaxMarks = question.MaxMarks;
                ret.OrderNumber = question.OrderNumber;
                ret.QuestionText = isSelectedLangArabic ? question.TextArabic : question.TextEnglish;
                if (_answer != null)
                {
                    ret.Comments = _answer.Comments;
                    ret.GivenMarks = _answer.GivenMarks;
                }

                _response.FeedbackFormQuestions.Add(ret);
            }
            _response.IsReadonly = IsReadonly;
            _response.CutOffMarks = _refereeUtil.GetRefereeRequestFeedbackRejectCutOffMarks();

            return _response;
        }

        private static Regex rx_digits = new Regex("\\d", RegexOptions.Compiled);
        private static Regex rx_letters = new Regex("[a-zA-Z]", RegexOptions.Compiled);
        private static Regex rx_special_characters = new Regex("\\W", RegexOptions.Compiled);

        public static bool ValidatePassword(string password, bool isAdmin, out string errorMsg)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                errorMsg = LangHelpers.Instance["validate_password_password_empty"];
                return false;
            }

            var missingRequirements = new List<string>();

            password = password.Trim();
            if (password.Contains(" ") || password.Contains("\t"))
            {
                missingRequirements.Add(LangHelpers.Instance["validate_password_cannot_contain_spaces"]);
            }

            if (isAdmin)
            {
                if (password.Length < 12)
                {
                    missingRequirements.Add(LangHelpers.Instance["validate_password_must_be_atleast_12_chars"]);
                }
            }
            else
            {
                if (password.Length < 8)
                {
                    missingRequirements.Add(LangHelpers.Instance["validate_password_must_be_atleast_8_chars"]);
                }
            }

            if (password.Length > 30)
            {
                missingRequirements.Add(LangHelpers.Instance["validate_password_must_not_exceed_30_chars"]);
            }

            if (!rx_digits.Match(password).Success)
            {
                missingRequirements.Add(LangHelpers.Instance["validate_password_must_contain_digits"]);
            }

            if (!rx_letters.Match(password).Success)
            {
                missingRequirements.Add(LangHelpers.Instance["validate_password_must_contain_letters"]);
            }

            if (!rx_special_characters.Match(password).Success)
            {
                missingRequirements.Add(LangHelpers.Instance["validate_password_must_contain_special_chars"]);
            }

            if (missingRequirements.Count > 0)
            {
                errorMsg = LangHelpers.Instance["validate_password_password_requirements"];
                foreach (var r in missingRequirements)
                {
                    errorMsg += " - " + r;
                }

                return false;
            }

            errorMsg = null;

            return true;
        }


    }
}