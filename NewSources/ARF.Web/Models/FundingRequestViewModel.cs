using ARF.Data;
using ARF.Web.App_Start.Code;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class FundingRequestViewModel : RequestViewModel
    {
        public List<ReportForm> Forms { set; get; }
        public List<BudgetPerPersonViewModel> BudgetSummaryPerPerson { get; set; } = new List<BudgetPerPersonViewModel>();
        public string SecondPeriodTotalCost { set; get; }
        public string FirstPeriodTotalCost { get; set; }
        public string Signature { set; get; }
        public bool Checkbox { set; get; }
        public ReqFieldType? FieldType { get; set; }
        public PublicationType PubType { get; set; }
        public List<ResearcherViewModel> Researchers { get; set; }
        public List<TimeTableViewModel> timeTable { get; set; }
        public ARF.Data.DbModels.ResearchersCv ResearchersCv { get; set; }
        public ARF.Data.DbModels.ResearchersReward ResearchersReward { get; set; }
        public ARF.Data.DbModels.RequestFile RequestFile { get; set; }
        public ARF.Data.DbModels.RequestMidTermReport requestMidTermReport { get; set; }
        public ARF.Data.DbModels.RewardRequest RewardRequest { get; set; }
        public ARF.Data.DbModels.RequestData RequestData { get; set; }
        public ARF.Data.DbModels.RequestProposalPlan RequestProposal { get; set; }
        public ARF.Data.DbModels.RequestTimeTable RequestTT { get; set; }
        public ARF.Data.DbModels.RequestActivity RequestActivity { get; set; }
        public ARF.Data.DbModels.RequestBenificialAuthority RequestBenificial { get; set; }
        public ARF.Data.DbModels.Material material { get; set; }        
        public RequestSummaryTabbedViewModel RequestSummary { get; set; }
        public List<RefereeFeedbackFormData> RefereeFeedbackForm { get; set; }        
        public AuthorViewModel AuthorDetails { get; set; }                
        public List<RefereeViewModel> Referees { get; set; }
        public List<ARF.Data.DbModels.Referee> Arbitrators { get; set; }
        public ARF.Data.DbModels.RequestReferee RequestReferee { get; set; }
        public List<RefereeViewModel> CouncilMembers { get; set; }

        public List<RequestActionViewModel> AllowedActions { get; set; }

        public double? Price { get; set; }

        public bool CanSelectReferees { get; set; }
        public bool CanViewReferees { get; set; }
        public bool CanSeeCouncilMembers { get; set; }

        public int MinRefereeCount { get; set; }
               

        public int MaxRefereeCount { get; set; }
        public List<ARF.Data.DbModels.Researcher> ResearcherList { get; set; }
        public List<ARF.Data.DbModels.ResearchersCv> ResearchersCvList { get; set; }
        public List<ARF.Data.DbModels.ResearchersReward> ResearchersRewardList { get; set; }
        public List<ARF.Data.DbModels.Request> RequestList { get; set; }
        public List<ARF.Data.DbModels.RewardRequest> RewardRequestList { get; set; }
        public List<ARF.Data.DbModels.RequestData> RequestDataList { get; set; }
        public List<ARF.Data.DbModels.RequestTimeTable> RequestTTList { get; set; }
        public List<ARF.Data.DbModels.RequestActivity> RequestActivityList { get; set; }
        public List<ARF.Data.DbModels.RequestBenificialAuthority> RequestBenificialList { get; set; }        

        // Lookups
        public SelectListItem[] Colleges { get; set; }
        public SelectListItem[] Universities { get; set; }
        public SelectListItem[] Departments { get; set; }
        public SelectListItem[] QualificationTypes { get; set; }
    }

    public class RefereeFeedbackFormData
    {
        public int GivenMarks { get; set; }
        public int FeedbackFormQuestionId { get; set; }
        public string Comments { get; set; }

    }
    public class ReportForm
    {
        public long Id { set; get; }
        public string TaskName { get; set; }
        public byte? CompletionRate { get; set; }
        public string Reasons { get; set; }
    }
}
