using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.Web.App_Start.Code;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class HomeViewModel
    {
        public List<RequestSummaryViewModel> Requests { get; set; }
    }

    public class RequestSummaryViewModel
    {
        public long Id { get; set; }
        public string UniqueId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
    }

    public class RequestSummaryViewModel2
    {        
        public string TabName { get; set; }
        public string TabId { get; set; } = "1";

        [LocalizedDisplay("request_id")]
        public long? Id { get; set; }

        [LocalizedDisplay("preview")]
        public string Preview { get; set; }

        [LocalizedDisplay("announcement")]
        public long? AnnouncementId { get; set; }

        [LocalizedDisplay("request_type")]
        public ReqFieldType Type { get; set; }

        [LocalizedDisplay("step_count")]
        public int? StepCount { get; set; }

        [LocalizedDisplay("step_no")]
        public int? Step { get; set; }

        [LocalizedDisplay("request_details")]
        public Request Request { get; set; }

        // Step 1
        [LocalizedDisplay("research_college")]
        
        public int? CollegeID { get; set; }

        [LocalizedDisplay("department")]
        
        public int? DepartmentId { get; set; }

        [LocalizedDisplay("title_in_arabic")]
        
        public string TitleArabic { get; set; }

        [LocalizedDisplay("title_in_english")]
        
        public string TitleEnglish { get; set; }

        [LocalizedDisplay("duration")]
        
        public int? Duration { get; set; }

        [LocalizedDisplay("primary_category")]
        
        public long? PrimaryCategory { get; set; }

        [LocalizedDisplay("secondary_category")]
        
        public long? SecondaryCategory { get; set; }

        [LocalizedDisplay("other_primary_category")]
        
        public string PrimaryCategoryOther { get; set; }

        [LocalizedDisplay("other_secondary_category")]
        
        public string SecondaryCategoryOther { get; set; }

        [LocalizedDisplay("is_previously_applied")]
        
        public bool? IsPreviouslyApplied { get; set; }

        [LocalizedDisplay("WasProposalAccepted")]
        
        public bool? WasProposalAccepted { get; set; }

        [LocalizedDisplay("RefusalReason")]
        
        public string RefusalReason { get; set; }

        [LocalizedDisplay("application_no")]
        
        public string ApplicationNumber { get; set; }

        [LocalizedDisplay("number_of_funded_projects")]
        
        public int? NumberOfFundedProjects { get; set; }

        [LocalizedDisplay("number_of_expected_publications")]
        
        public int? NumberOfExpectedPublications { get; set; }

        [LocalizedDisplay("is_within_priority")]
        
        public bool? IsWithinPriority { get; set; }

        [LocalizedDisplay("priority")]
        
        public long? PriorityId { get; set; }


        [LocalizedDisplay("other_priority")]
        
        public string PriorityOther { get; set; }

        // Step 2
        [LocalizedDisplay("project_type")]
        
        public ProjectType ProjectType { get; set; }

        [LocalizedDisplay("researchers")]
        
        public List<CoresearcherViewModel> Researchers { get; set; } = new List<CoresearcherViewModel>();

        // Step 3
        [LocalizedDisplay("the_abstract_in_arabic")]
        [AllowHtml]
        
        public string Abstract { get; set; }

        [LocalizedDisplay("Keywords")]
        
        public List<string> Keywords { get; set; } = new List<string>();

        [LocalizedDisplay("the_abstract_in_english")]
        [AllowHtml]
        
        public string AbstractEnglish { get; set; }

        [LocalizedDisplay("keywords_english")]
        
        public List<string> KeywordsEnglish { get; set; } = new List<string>();

        [LocalizedDisplay("section_project_importance")]
        [AllowHtml]
        
        public string SectionImportance { get; set; }

        [LocalizedDisplay("section_problem")]
        [AllowHtml]
        
        public string SectionProblem { get; set; }

        [LocalizedDisplay("section_goals")]
        [AllowHtml]
        
        public string SectionGoals { get; set; }

        [LocalizedDisplay("section_assumptions")]
        [AllowHtml]
        
        public string SectionAssumptions { get; set; }

        [LocalizedDisplay("section_terms")]
        [AllowHtml]
        
        public string SectionTerms { get; set; }

        [LocalizedDisplay("section_objective_limitations")]
        [AllowHtml]
        

        public string SectionObjectiveLimitations { get; set; }

        [LocalizedDisplay("section_spatial_limitations")]
        [AllowHtml]
        
        public string SectionSpatialLimitations { get; set; }

        [LocalizedDisplay("section_temporal_limitations")]
        [AllowHtml]
        
        public string SectionTemporalLimitations { get; set; }

        [LocalizedDisplay("section_theoritical_framework")]
        [AllowHtml]
        
        public string SectionTheoriticalFramework { get; set; }

        [LocalizedDisplay("section_previous_studies")]
        [AllowHtml]
        
        public string SectionPreviousStudies { get; set; }

        [LocalizedDisplay("section_procedures")]
        [AllowHtml]
        
        public string SectionProcedures { get; set; }

        [LocalizedDisplay("section_society")]
        [AllowHtml]
        
        public string SectionSociety { get; set; }

        [LocalizedDisplay("section_tools")]
        [AllowHtml]
        
        public string SectionTools { get; set; }

        [LocalizedDisplay("section_chapters")]
        [AllowHtml]
        
        public string SectionChapters { get; set; }

        [LocalizedDisplay("section_references")]
        [AllowHtml]
        
        public string SectionReferences { get; set; }

        // For applied only

        [LocalizedDisplay("applied_section_objectives")]
        [AllowHtml]
        
        public string SectionObjectives { get; set; }
        [LocalizedDisplay("applied_section_relevance")]
        [AllowHtml]
        
        public string SectionRelevance { get; set; }
        [LocalizedDisplay("applied_section_background")]
        [AllowHtml]
        
        public string SectionBackground { get; set; }
        [LocalizedDisplay("applied_section_methodology")]
        [AllowHtml]
        
        public string SectionMethodology { get; set; }
        [LocalizedDisplay("applied_section_appendices")]
        [AllowHtml]
        
        public string SectionAppendices { get; set; }


        [LocalizedDisplay("applied_section_other_funding")]
        
        public List<FundingSourceViewModel> FundingSources { get; set; } = new List<FundingSourceViewModel>();



        // Step 4
        [LocalizedDisplay("timeline")]
        
        public List<TimelineViewModel> Timeline { get; set; } = new List<TimelineViewModel>();

        [LocalizedDisplay("timeline")]
        
        public List<MaterialsViewModel> Materials { get; set; } = new List<MaterialsViewModel>();

        
        [LocalizedDisplay("applied_budget_rationale")]
        [AllowHtml]
        public string BudgetRationale { get; set; }

        // Step 5

        [LocalizedDisplay("suggested_referees")]
        
        public List<SuggestedRefereeViewModel> SuggestedReferees { get; set; } = new List<SuggestedRefereeViewModel>();

        // Step 6
        [LocalizedDisplay("suggested_activities")]
        
        public List<SuggestedActivityViewModel> SuggestedActivities { get; set; } = new List<SuggestedActivityViewModel>();

        [LocalizedDisplay("beneficiaries")]
        
        public List<BeneficiaryViewModel> Beneficiaries { get; set; } = new List<BeneficiaryViewModel>();

        [LocalizedDisplay("publish_containers")]
        
        public List<PublishContainerViewModel> PublishContainers { get; set; } = new List<PublishContainerViewModel>();

        // Lookups
        public SelectListItem[] Colleges { get; set; }

        public SelectListItem[] Priorities { get; set; }
        public SelectListItem[] Durations { get; set; }
        public SelectListItem[] PrimaryCategories { get; set; }
        public SelectListItem[] SecondaryCategories { get; set; }
        public SelectListItem[] ResearcherRoles { get; set; }
        public SelectListItem[] Universities { get; set; }
        public SelectListItem[] Departments { get; set; }
        public SelectListItem[] QualificationTypes { get; set; }
        public SelectListItem[] Countries { get; set; }
        public List<CityViewModel> Cities { get; set; }
        public SelectListItem[] Banks { get; set; }
        public SelectListItem[] ActivityTypes { get; set; }
        public SelectListItem[] ResearchersLookup { get; set; }
        public SelectListItem[] Months { get; set; }
        public SelectListItem[] MaterialTypes { get; set; }
        public SelectListItem[] FundingStatuses { get; set; }
        public List<TimelineGanttViewModel> TimelinePreview { get; set; } = new List<TimelineGanttViewModel>();

        public List<BudgetPerPersonViewModel> BudgetSummaryPerPerson { get; set; } = new List<BudgetPerPersonViewModel>();

        public string FirstPeriodTotalCost { get; set; }
        public string SecondPeriodTotalCost { get; set; }
        public string MaterialsTotalCost { get; set; }
        public string TotalCostForProject { get; set; }
    }

    public class CoresearcherSummaryViewModel
    {
        public long? Id { get; set; }

        [LocalizedDisplay("researcher_type")]
        public string RoleInProject { get; set; }

        [LocalizedDisplay("quad_name")]
        public string Name { get; set; }

        [LocalizedDisplay("Qualification")]
        public string Qualification { get; set; }

        [LocalizedDisplay("university_or_workplace")]
        public string University { get; set; }

        [LocalizedDisplay("college")]
        public string College { get; set; }

        [LocalizedDisplay("department")]
        public string Department { get; set; }

        [LocalizedDisplay("other_university")]
        public string UniversityOther { get; set; }

        [LocalizedDisplay("other_college")]
        public string CollegeOther { get; set; }

        [LocalizedDisplay("other_department")]
        public string DepartmentOther { get; set; }

        [LocalizedDisplay("mobile")]
        public string MobileNumber { get; set; }

        [LocalizedDisplay("email")]
        public string Email { get; set; }

        [LocalizedDisplay("national_id_or_passport")]
        public string NationalID { get; set; }

        [LocalizedDisplay("nationality")]
        public string Nationality { get; set; }

        [LocalizedDisplay("birthdate")]
        public string Birthdate { get; set; }

        [LocalizedDisplay("addresss")]
        public string Addresss { get; set; }

        [LocalizedDisplay("account_holder_name")]
        public string BankAccountHolder { get; set; }

        [LocalizedDisplay("iban")]
        public string BankIban { get; set; }

        [LocalizedDisplay("iban_photocopy")]
        public HttpPostedFileBase BankIbanPhotocopy { get; set; }

        public AttachedFile BankIbanPhotocopyReadonly { get; set; }

        [LocalizedDisplay("SectionPublishedPapers")]
        public string SectionPublishedPapers { get; set; }

        [LocalizedDisplay("SectionInterests")]
        public string SectionInterests { get; set; }

        [LocalizedDisplay("SectionCareerHistory")]
        public string SectionCareerHistory { get; set; }

        [LocalizedDisplay("SectionTrainingCourses")]
        public string SectionTrainingCourses { get; set; }

        [LocalizedDisplay("SectionAwards")]
        public string SectionAwards { get; set; }

        [LocalizedDisplay("SectionMemberships")]
        public string SectionMemberships { get; set; }

        [LocalizedDisplay("Bank")]
        public string Bank { get; set; }

        [LocalizedDisplay("other_bank")]
        public string BankOther { get; set; }

        [LocalizedDisplay("BankBranch")]
        public string BankBranch { get; set; }

        [LocalizedDisplay("researcher_type")]
        public HttpPostedFileBase CvFile { get; set; }

        [LocalizedDisplay("cv_text")]
        public string CvText { get; set; }
    }

    public class TimelineSummaryViewModel
    {
        public long? Id { get; set; }

        [LocalizedDisplay("name")]
        public string TaskName { get; set; }

        [LocalizedDisplay("responsible_person")]
        public string ResponsibleResearcher { get; set; }

        [LocalizedDisplay("responsible_person")]
        public string ResponsibleResearcherName { get; set; }

        [LocalizedDisplay("StartingMonth")]
        public string StartingMonth { get; set; }

        [LocalizedDisplay("number_of_visits")]
        public string NumberOfVisits { get; set; }

        [LocalizedDisplay("EndingMonth")]
        public string EndingMonth { get; set; }

        public bool IsConsultant { get; set; }
    }

    public class SuggestedRefereeSumamryViewModel
    {
        public long? Id { get; set; }

        [LocalizedDisplay("name")]
        public string Name { get; set; }

        [LocalizedDisplay("Degree")]
        public string DegreeType { get; set; }

        [LocalizedDisplay("university")]
        public string University { get; set; }

        [LocalizedDisplay("college")]
        public string College { get; set; }

        [LocalizedDisplay("department")]
        public string Department { get; set; }

        [LocalizedDisplay("other_university")]
        public string UniversityOther { get; set; }

        [LocalizedDisplay("other_college")]
        public string CollegeOther { get; set; }

        [LocalizedDisplay("other_department")]
        public string DepartmentOther { get; set; }

        [LocalizedDisplay("mobile")]
        public string MobileNumber { get; set; }

        [LocalizedDisplay("email")]
        public string Email { get; set; }

        [LocalizedDisplay("addresss")]
        public string Addresss { get; set; }
    }

    public class SuggestedActivitySummaryViewModel
    {
        public long? Id { get; set; }

        [LocalizedDisplay("name")]
        public string Name { get; set; }

        [LocalizedDisplay("activity_type")]
        public string Type { get; set; }

        [LocalizedDisplay("other_type")]
        public string OtherType { get; set; }

        [LocalizedDisplay("address")]
        public string Address { get; set; }

        [LocalizedDisplay("benefit_description")]
        public string Description { get; set; }
    }

    public class BeneficiarySummaryViewModel
    {
        public long? Id { get; set; }

        [LocalizedDisplay("beneficiary")]
        public string Name { get; set; }

        [LocalizedDisplay("address")]
        public string Address { get; set; }

        [LocalizedDisplay("benefit_description")]
        public string Description { get; set; }
    }

    public class PublishContainerSummaryViewModel
    {
        public long? Id { get; set; }

        [LocalizedDisplay("container_name")]
        public string Name { get; set; }

        [LocalizedDisplay("address")]
        public string Address { get; set; }

        [LocalizedDisplay("selection_reason")]
        public string SelectionReason { get; set; }
    }
}