using ARF.Data;
using System;

namespace ARF.Core.Models
{
    public class ViewRequestModel
    {
        public long Id { get; set; }
        public long? CollegeId { get; set; }
        public string CollegeName { get; set; }
        public string JpcName { get; set; }
        public long JobSeekerId { get; set; }
        public string JobSeekerNin { get; set; }
        public string JobSeekerName { get; set; }
        public RequestType Type { get; set; }
        public DateTime? Date { get; set; }
        public string Employer { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? Approved { get; set; }

        public bool? IsPmApproved { get; set; }
        public DateTime? PmApproved { get; set; }

        public bool? IsBmApproved { get; set; }
        public DateTime? BmApproved { get; set; }
        public RequestViewStatus Status { get; set; }
        public bool IsActionable { get; set; }
        public bool IsPending { get; set; }

        public long? SourceProviderId { get; set; }
        public long? SourceJpcId { get; set; }

        public long? TargetProviderId { get; set; }
        public long? TargetJpcId { get; set; }

        public bool IsCanceled { get; set; }

        public bool IsReturn { get; set; }

        public bool? DidJsConsent { get; set; }

        public bool? IsApprovedBySource { get; set; }

        public bool? IsSourceBmApproved { get; set; }

        public bool? IsSourcePmApproved { get; set; }

        public bool? IsApprovedByTarget { get; set; }

        public bool? IsTargetBmApproved { get; set; }

        public bool? IsTargetPmApproved { get; set; }


        public long? SubmittedByJpcId { get; set; }
    }
}
