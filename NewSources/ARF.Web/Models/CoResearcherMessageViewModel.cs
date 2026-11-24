using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class CoResearcherMessageViewModel
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public bool? IsAccepted { get; set; }
        public int type;
        
        public string ScientificType { get; set; }
        public string Supervisor { get; set; }
        public int MajorId { get; set; }
        public int languageId { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string mobileNo { get; set; }
        public string Email { get; set; }
        
        public int MajorAuthorId { get; set; }
        public string Recommendation { get; set; }
        public int NoOfRecommendations { get; set; }
        public bool IsGovernmental { get; set; }
        public bool IsArabic { get; set; }
        public string PublisherName { get; set; }
        public string PublisherPlace { get; set; }
        public DateTime? PublisherDate { get; set; }
        public DateTime? RecommendationDate { get; set; }
        public DateTime? DateOfDiscussion { get; set; }
        public int? Degree { get; set; }
        public string Gpa { get; set; }
        public int? PublishType { get; set; }
        public string ReasonOfReject { get; set; }
        public string TranslatedTo { get; set; }
        public string AuthersName { get; set; }
        public string CommitteeMembers { get; set; }
        public string SelectedAction { get; set; }

    }
}