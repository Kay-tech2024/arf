using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.Web.App_Start.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class RequestViewModel
    {
        public Request Request { get; set; }

        public long? Id { get; set; }

        public string UniqueId { get; set; }

        public long? PublicationRequestId { get; set; }

        public Data.RequestType RequestType { get; set; }

        public string MaterialsTotalCost { get; set; }
        public string TotalCostForProject { get; set; }
        public List<ARF.Data.DbModels.RequestMaterial> RequestMaterial { get; set; }
        public BetterSelectListItem[] MaterialTypes { get; set; }

        public string Type { get; set; }
        public byte TypeNo { get; set; }
        public string Title { get; set; }

        public bool AllowEdit { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public long? StatusNo { get; set; }

        public string CreatedByUserId { get; set; }

        public string CurrentlyWithUserId { get; set; }
        public string CurrentlyWithUserName { get; set; }

        [Editable(false)]
        public string Name { get; set; }

        public string Comments { get; set; }

        public List<CommentViewModel> CommentThread { get; set; } = new List<CommentViewModel>();

        public List<AttachedFile> Files { get; set; } = new List<AttachedFile>();

        public List<RequestHistoryModel> History { get; set; }

        public SelectListItem[] ReturnableTo { get; set; }

        public Data.RequestAction Action { get; set; }
        public string AssignToUserId { get; set; }

        public string AnnouncementStartDate { get; set; }
        public string AnnouncementEndDate { get; set; }

        public string ProjectAdvantages { get; set; }
        public string ProjectDisadvantages { get; set; }
        public SelectListItem[] Recommendations { get; set; }
        public long Recommendation { get; set; }
        public string RecommendationText { get; set; }
        public string ReasonForInvalidRecommendation { get; set; }
        public string MeetingNumber { get; set; }

        public string SecretaryMeetingDate { get; set; }
        public string SecretaryMeetingNumber { get; set; }

        [AllowHtml]
        public string ResearcherContractText { get; set; }
        public string ResearcherContractStartDate { get; set; }

        public List<ARF.Data.DbModels.RequestCoresearcherResponse> RequestCoresearcherResponses { get; set; }

        [LocalizedDisplay("meeting_date")]
        public string MeetingDate { get; set; }

        [LocalizedDisplay("suggested_referees")]
        public List<SuggestedRefereeViewModel> SuggestedReferees { get; set; } = new List<SuggestedRefereeViewModel>();

        public string WithdrawalFromProject { get; set; }
        public string UpdateDuration { get; set; }
        public string UpdateResearchersTeam { get; set; }
        public string UpdateMaterials { get; set; }
    }

    public enum RecommendationOptions : long
    {
        ValidForFunding = 1,
        NotValidForFundingAtAll = 2,
        ValidWithErrors = 3,
    }
}