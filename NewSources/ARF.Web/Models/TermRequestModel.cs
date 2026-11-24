using ARF.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ARF.Web.Models
{
    public class TermRequestModel : RequestViewModel
    {
        public string ResearchUnitCode;
        public string ResearchYear;
        public string TitleArabic;
        public string TitleEnglish;
        public string FundingAmount;
        public string FundingDurationMonths;
        public bool IsShared;
        public string ContractDate;
        public string ResearchFaculty;
        public string PrimaryResearcherNameArabic;
        public string ApplicationDate;
        public List<ResearcherViewModel> Researchers;

        public List<Task> Tasks;


        public string GoodComments { get; set; }
        public string BadComments { get; set; }
        public List<RefereeSuggestion> Referees { get; set; }

    }

    public class RefereeSuggestion
    {
        public int Order;

        [Required]
        public string Name;

        public string Speciality;
        public string EducationalLevel;
        public string WorkAddress;

        [Required]
        public string Email;

        [Required]
        public string ContactNo;
    }

    public class Task
    {
        public string TaskName;
        public string CompletionPercent;
        public string DelayReason;
    }
}
