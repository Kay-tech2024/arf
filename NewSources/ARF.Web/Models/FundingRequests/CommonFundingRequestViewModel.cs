using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.Web.App_Start.Code;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public partial class CommonFundingRequestViewModel
    {
        [LocalizedDisplay("request_id")]
        public long? Id { get; set; }

        public bool HasCoresearchers { get; set; } = false;

        public string Comments { get; set; }

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
        [Step(Step = 1, IsRequired = true)]
        public int? CollegeID { get; set; }

        [LocalizedDisplay("department")]
        [Step(Step = 1, IsRequired = true)]
        public int? DepartmentId { get; set; }

        [LocalizedDisplay("title_in_arabic")]
        [Step(Step = 1, IsRequired = true, FieldType = Data.FormFieldType.Text, IsArabicOnly = true, Min = 10, Max = 300)]
        public string TitleArabic { get; set; }

        [LocalizedDisplay("title_in_english")]
        [Step(Step = 1, IsRequired = false, FieldType = Data.FormFieldType.Text, IsEnglishOnly = true, Min = 10, Max = 300)]
        public string TitleEnglish { get; set; }

        [LocalizedDisplay("duration")]
        [Step(Step = 1, IsRequired = true, FieldType = Data.FormFieldType.Integer, Min = 1, Max = 36)]
        public int? Duration { get; set; }

        [LocalizedDisplay("primary_category")]
        [Step(Step = 1, IsRequired = true, RequiredIfOtherField = "PrimaryCategoryOther")]
        public long? PrimaryCategory { get; set; }

        [LocalizedDisplay("secondary_category")]
        [Step(Step = 1, IsRequired = false, RequiredIfOtherField = "SecondaryCategoryOther")]
        public long? SecondaryCategory { get; set; }

        [LocalizedDisplay("other_primary_category")]
        [Step(Step = 1, IsRequired = true, RequiredIfOtherField = "PrimaryCategory", RequiredIfClearField = false)]
        public string PrimaryCategoryOther { get; set; }

        [LocalizedDisplay("other_secondary_category")]
        [Step(Step = 1, IsRequired = true, RequiredIfClearField = false)]
        public string SecondaryCategoryOther { get; set; }

        [LocalizedDisplay("is_previously_applied")]
        [Step(Step = 1, IsRequired = true)]
        public bool? IsPreviouslyApplied { get; set; }

        [LocalizedDisplay("WasProposalAccepted")]
        [Step(Step = 1, IsRequired = true, RequiredIfOtherField = "IsPreviouslyApplied", RequiredIfOtherFieldValue = "True")]
        public bool? WasProposalAccepted { get; set; }

        [LocalizedDisplay("RefusalReason")]
        [Step(Step = 1, IsRequired = true, RequiredIfOtherField = "WasProposalAccepted", RequiredIfOtherFieldValue = "False")]
        public string RefusalReason { get; set; }

        [LocalizedDisplay("application_no")]
        [Step(Step = 1, IsRequired = true, RequiredIfOtherField = "IsPreviouslyApplied", RequiredIfOtherFieldValue = "True")]
        public string ApplicationNumber { get; set; }

        [LocalizedDisplay("number_of_funded_projects")]
        [Step(Step = 1, IsRequired = true)]
        public int? NumberOfFundedProjects { get; set; }

        [LocalizedDisplay("number_of_expected_publications")]
        [Step(Step = 1, IsRequired = true)]
        public int? NumberOfExpectedPublications { get; set; }

        [LocalizedDisplay("is_within_priority")]
        [Step(Step = 1, IsRequired = true)]
        public bool? IsWithinPriority { get; set; }

        [LocalizedDisplay("priority")]
        [Step(Step = 1, IsRequired = true, RequiredIfOtherField = "IsWithinPriority", RequiredIfOtherFieldValue = "True")]
        public long? PriorityId { get; set; }


        [LocalizedDisplay("other_priority")]
        [Step(Step = 1, IsRequired = true, RequiredIfOtherField = "PriorityId", RequiredIfClearField = false)]
        public string PriorityOther { get; set; }

        // Step 2
        [LocalizedDisplay("project_type")]
        [Step(Step = 2, IsRequired = false)]
        public ProjectType ProjectType { get; set; }

        [LocalizedDisplay("researchers")]
        [Step(Step = 2, IsRequired = true, Min = 1, Max = 10)]
        public List<CoresearcherViewModel> Researchers { get; set; } = new List<CoresearcherViewModel>();

        // Step 3
        [LocalizedDisplay("the_abstract_in_arabic")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, Min = 100)]
        public string Abstract { get; set; }

        [LocalizedDisplay("Keywords")]
        [Step(Step = 3, IsRequired = true)]
        public List<string> Keywords { get; set; } = new List<string>();

        [LocalizedDisplay("the_abstract_in_english")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = false)]
        public string AbstractEnglish { get; set; }

        [LocalizedDisplay("keywords_english")]
        [Step(Step = 3, IsRequired = false)]
        public List<string> KeywordsEnglish { get; set; } = new List<string>();

        [LocalizedDisplay("section_project_importance")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField", RequiredIfNot = true)]
        public string SectionImportance { get; set; }

        [LocalizedDisplay("section_problem")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField", RequiredIfNot = true)]
        public string SectionProblem { get; set; }

        [LocalizedDisplay("section_goals")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField", RequiredIfNot = true)]
        public string SectionGoals { get; set; }

        [LocalizedDisplay("section_assumptions")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = false)]
        public string SectionAssumptions { get; set; }

        [LocalizedDisplay("section_terms")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField", RequiredIfNot = true)]
        public string SectionTerms { get; set; }

        [LocalizedDisplay("section_objective_limitations")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = false)]

        public string SectionObjectiveLimitations { get; set; }

        [LocalizedDisplay("section_spatial_limitations")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = false)]
        public string SectionSpatialLimitations { get; set; }

        [LocalizedDisplay("section_temporal_limitations")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = false)]
        public string SectionTemporalLimitations { get; set; }

        [LocalizedDisplay("section_theoritical_framework")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = false)]
        public string SectionTheoriticalFramework { get; set; }

        [LocalizedDisplay("section_previous_studies")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField", RequiredIfNot = true)]
        public string SectionPreviousStudies { get; set; }

        [LocalizedDisplay("section_procedures")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField", RequiredIfNot = true)]
        public string SectionProcedures { get; set; }

        [LocalizedDisplay("section_society")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = false)]
        public string SectionSociety { get; set; }

        [LocalizedDisplay("section_tools")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = false, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField", RequiredIfNot = true)]
        public string SectionTools { get; set; }

        [LocalizedDisplay("section_chapters")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField", RequiredIfNot = true)]
        public string SectionChapters { get; set; }

        [LocalizedDisplay("section_references")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField", RequiredIfNot = true)]
        public string SectionReferences { get; set; }

        // For applied only

        [LocalizedDisplay("applied_section_objectives")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField")]
        public string SectionObjectives { get; set; }
        [LocalizedDisplay("applied_section_relevance")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField")]
        public string SectionRelevance { get; set; }
        [LocalizedDisplay("applied_section_background")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField")]
        public string SectionBackground { get; set; }
        [LocalizedDisplay("applied_section_methodology")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField")]
        public string SectionMethodology { get; set; }
        [LocalizedDisplay("applied_section_appendices")]
        [AllowHtml]
        [Step(Step = 3, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField")]
        public string SectionAppendices { get; set; }


        [LocalizedDisplay("applied_section_other_funding")]
        [Step(Step = 4, IsRequired = true, Min = 0, Max = 100)]
        public List<FundingSourceViewModel> FundingSources { get; set; } = new List<FundingSourceViewModel>();



        // Step 4
        [LocalizedDisplay("timeline")]
        [Step(Step = 4, IsRequired = true, Min = 0, Max = 100)]
        public List<TimelineViewModel> Timeline { get; set; } = new List<TimelineViewModel>();

        [LocalizedDisplay("timeline")]
        [Step(Step = 4, IsRequired = false, Min = 0, Max = 100)]
        public List<MaterialsViewModel> Materials { get; set; } = new List<MaterialsViewModel>();

        [Step(Step = 4, IsRequired = true, RequiredIfOtherField = "Type", RequiredIfOtherFieldValue = "AppliedScienceField")]
        [LocalizedDisplay("applied_budget_rationale")]
        [AllowHtml]
        public string BudgetRationale { get; set; }

        // Step 5

        [LocalizedDisplay("suggested_referees")]
        [Step(Step = 5, IsRequired = true, Min = 5, Max = 10)]
        public List<SuggestedRefereeViewModel> SuggestedReferees { get; set; } = new List<SuggestedRefereeViewModel>();

        // Step 6
        [LocalizedDisplay("suggested_activities")]
        [Step(Step = 6, IsRequired = false, Min = 0, Max = 100)]
        public List<SuggestedActivityViewModel> SuggestedActivities { get; set; } = new List<SuggestedActivityViewModel>();

        [LocalizedDisplay("beneficiaries")]
        [Step(Step = 6, IsRequired = true, Min = 1, Max = 10)]
        public List<BeneficiaryViewModel> Beneficiaries { get; set; } = new List<BeneficiaryViewModel>();

        [LocalizedDisplay("publish_containers")]
        [Step(Step = 6, IsRequired = true, Min = 1, Max = 10)]
        public List<PublishContainerViewModel> PublishContainers { get; set; } = new List<PublishContainerViewModel>();

        // Lookups
        public BetterSelectListItem[] Colleges { get; set; }
        public List<ReportForm> Forms { set; get; }
        public BetterSelectListItem[] Priorities { get; set; }
        public BetterSelectListItem[] Durations { get; set; }
        public BetterSelectListItem[] PrimaryCategories { get; set; }
        public BetterSelectListItem[] SecondaryCategories { get; set; }
        public BetterSelectListItem[] ResearcherRoles { get; set; }
        public BetterSelectListItem[] Universities { get; set; }
        public BetterSelectListItem[] Departments { get; set; }
        public BetterSelectListItem[] QualificationTypes { get; set; }
        public BetterSelectListItem[] Countries { get; set; }
        public List<CityViewModel> Cities { get; set; }
        public BetterSelectListItem[] Banks { get; set; }
        public BetterSelectListItem[] ActivityTypes { get; set; }
        public BetterSelectListItem[] ResearchersLookup { get; set; }
        public BetterSelectListItem[] Months { get; set; }
        public BetterSelectListItem[] MaterialTypes { get; set; }
        public BetterSelectListItem[] FundingStatuses { get; set; }
        public BetterSelectListItem[] OptionSelection { get; set; }
        public List<TimelineGanttViewModel> TimelinePreview { get; set; } = new List<TimelineGanttViewModel>();

        public List<BudgetPerPersonViewModel> BudgetSummaryPerPerson { get; set; } = new List<BudgetPerPersonViewModel>();

        public string FirstPeriodTotalCost { get; set; }
        public string SecondPeriodTotalCost { get; set; }
        public string MaterialsTotalCost { get; set; }
        public string TotalCostForProject { get; set; }        

        public int UpdateDuration { get; set; }
        public int UpdateResearchersTeam { get; set; }
        public int UpdateMaterials { get; set; }
        public int WithdrawalFromProject { get; set; }        
    }

    public class CoresearcherViewModel
    {
        public long? Id { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("researcher_type")]
        public ResearcherRole? RoleInProject { get; set; }

        [Step(Step = 2, IsRequired = true, FieldType = FormFieldType.Name)]
        [LocalizedDisplay("quad_name")]
        public string FullName { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("degree_or_qualification")]
        public QualificationType? Qualification { get; set; }

        [Step(Step = 2, IsRequired = true, RequiredIfOtherField = "UniversityOther")]
        [LocalizedDisplay("university_or_workplace")]
        public int? UniversityId { get; set; }

        [Step(Step = 2, IsRequired = true, RequiredIfOtherField = "CollegeOther")]
        [LocalizedDisplay("college")]
        public int? CollegeId { get; set; }

        [Step(Step = 2, IsRequired = true, RequiredIfOtherField = "DepartmentOther")]
        [LocalizedDisplay("department")]
        public int? DepartmentID { get; set; }

        [Step(Step = 2, IsRequired = true, RequiredIfOtherField = "UniversityId", RequiredIfClearField = false)]
        [LocalizedDisplay("other_university")]
        public string UniversityOther { get; set; }

        [Step(Step = 2, IsRequired = true, RequiredIfOtherField = "CollegeId", RequiredIfClearField = false)]
        [LocalizedDisplay("other_college")]
        public string CollegeOther { get; set; }

        [Step(Step = 2, IsRequired = true, RequiredIfOtherField = "DepartmentID", RequiredIfClearField = false)]
        [LocalizedDisplay("other_department")]
        public string DepartmentOther { get; set; }

        [Step(Step = 2, IsRequired = true, FieldType = FormFieldType.Phone)]
        [LocalizedDisplay("mobile")]
        public string MobileNumber { get; set; }

        [Step(Step = 2, IsRequired = true, FieldType = FormFieldType.Email)]
        [LocalizedDisplay("email")]
        public string Email { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("national_id_or_passport")]
        public string NationalID { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("nationality")]
        public int? NationalityId { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("birthdate")]
        public string Birthdate { get; set; }



        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("residence_country")]
        public int? CountryId { get; set; }


        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("residence_city")]
        public int? CityId { get; set; }


        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("addresss")]
        public string Addresss { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("account_holder_name")]
        public string BankAccountHolder { get; set; }

        [Step(Step = 2, IsRequired = true, FieldType = FormFieldType.IBAN)]
        [LocalizedDisplay("iban")]
        public string BankIban { get; set; }

        [Step(Step = 2, IsRequired = false, FieldType = FormFieldType.AttachedFile)]
        [LocalizedDisplay("iban_photocopy")]
        public HttpPostedFileBase BankIbanPhotocopy { get; set; }

        public AttachedFile BankIbanPhotocopyReadonly { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("SectionPublishedPapers")]
        [AllowHtml]
        public string SectionPublishedPapers { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("SectionInterests")]
        [AllowHtml]
        public string SectionInterests { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("SectionCareerHistory")]
        [AllowHtml]
        public string SectionCareerHistory { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("SectionAcademicHistory")]
        [AllowHtml]
        public string SectionAcademicHistory { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("SectionTrainingCourses")]
        [AllowHtml]
        public string SectionTrainingCourses { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("SectionAwards")]
        [AllowHtml]
        public string SectionAwards { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("SectionMemberships")]
        [AllowHtml]
        public string SectionMemberships { get; set; }



        [Step(Step = 2, IsRequired = true, RequiredIfOtherField = "BankOther")]
        [LocalizedDisplay("Bank")]
        public int? Bank { get; set; }

        [Step(Step = 2, IsRequired = true)]
        [LocalizedDisplay("researcher_specialization")]
        public string Specialization { get; set; }

        [Step(Step = 2, IsRequired = true, RequiredIfOtherField = "Bank", RequiredIfClearField = false)]
        [LocalizedDisplay("other_bank")]
        public string BankOther { get; set; }

        [Step(Step = 2, IsRequired = false)]
        [LocalizedDisplay("branch_name")]
        public string BankBranch { get; set; }

        [Step(Step = 2, IsRequired = false, FieldType = FormFieldType.AttachedFile)]
        [LocalizedDisplay("researcher_type")]
        public HttpPostedFileBase CvFile { get; set; }

        [Step(Step = 2, IsRequired = false, FieldType = FormFieldType.LongText)]
        [AllowHtml]
        [LocalizedDisplay("cv_text")]
        public string CvText { get; set; }        
    }

    public class BudgetPerPersonViewModel
    {
        public string Name { get; set; }
        public bool IsConsultant { get; set; }
        public string ResearcherRole { get; set; }
        public string FirstPeriodActiveMonths { get; set; }
        public string FirstPeriodCost { get; set; }
        public string SecondPeriodActiveMonths { get; set; }
        public string SecondPeriodCost { get; set; }
        public string TotalActiveMonths { get; set; }
        public string TotalCost { get; set; }
    }

    public class TimelineViewModel
    {
        public long? Id { get; set; }

        [Step(Step = 4, IsRequired = true, OnlyIfPropertyIsNotNull = "TaskName")]
        [LocalizedDisplay("name")]
        public string TaskName { get; set; }

        [Step(Step = 4, IsRequired = true, OnlyIfPropertyIsNotNull = "TaskName")]
        [LocalizedDisplay("responsible_person")]
        public long? ResponsibleResearcherId { get; set; }

        [Step(Step = 4, IsRequired = false)]
        [LocalizedDisplay("responsible_person")]
        public string ResponsibleResearcherName { get; set; }

        [Step(Step = 4, IsRequired = true, OnlyIfPropertyIsNotNull = "TaskName")]
        [LocalizedDisplay("StartingMonth")]
        public int? StartingMonth { get; set; }

        [Step(Step = 4, IsRequired = true, OnlyIfPropertyIsNotNull = "TaskName")]
        [LocalizedDisplay("number_of_visits")]
        public int? NumberOfVisits { get; set; }

        [Step(Step = 4, IsRequired = true, OnlyIfPropertyIsNotNull = "TaskName")]
        [LocalizedDisplay("EndingMonth")]
        public int? EndingMonth { get; set; }

        public bool IsConsultant { get; set; }
    }

    public class MaterialsViewModel
    {
        public long? Id { get; set; }

        [Step(Step = 4, IsRequired = false)]
        [LocalizedDisplay("material_name")]
        public string Name { get; set; }

        [Step(Step = 4, IsRequired = true, FieldType = FormFieldType.Float, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("amount_in_grams")]
        public double? Amount { get; set; }

        [Step(Step = 4, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("material_type")]
        public MaterialType? Type { get; set; }

        [Step(Step = 4, IsRequired = true, OnlyIfPropertyIsNotNull = "Name", RequiredIfOtherField = "Type", RequiredIfClearField = false)]
        [LocalizedDisplay("other_type")]
        public string OtherType { get; set; }


        [Step(Step = 4, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("material_usage")]
        public string Usage { get; set; }

        [Step(Step = 4, IsRequired = true, FieldType = FormFieldType.Monetary, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("material_cost")]
        public double? Cost { get; set; }
    }

    public class FundingSourceViewModel
    {
        public long? Id { get; set; }

        [Step(Step = 4, IsRequired = false)]
        [LocalizedDisplay("applied_funding_agency")]
        public string Name { get; set; }

        [Step(Step = 4, IsRequired = true, FieldType = FormFieldType.Float, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("applied_funding_amount")]
        public double? Amount { get; set; }


        [Step(Step = 4, IsRequired = true, FieldType = FormFieldType.Integer, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("applied_funding_duration")]
        public int? Duration { get; set; }

        [Step(Step = 4, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("applied_funding_status")]
        public FundingStatus? Status { get; set; }

    }


    public class SuggestedRefereeViewModel
    {
        public long? Id { get; set; }

        [Step(Step = 5, IsRequired = true, FieldType = FormFieldType.Name)]
        [LocalizedDisplay("name")]
        public string Name { get; set; }

        [Step(Step = 5, IsRequired = true)]
        [LocalizedDisplay("Degree")]
        public QualificationType? Degree { get; set; }

        [Step(Step = 5, IsRequired = true, RequiredIfOtherField = "UniversityOther")]
        [LocalizedDisplay("university")]
        public int? UniversityId { get; set; }

        [Step(Step = 5, IsRequired = true, RequiredIfOtherField = "CollegeOther")]
        [LocalizedDisplay("college")]
        public int? CollegeId { get; set; }

        [Step(Step = 5, IsRequired = true, RequiredIfOtherField = "DepartmentOther")]
        [LocalizedDisplay("department")]
        public int? DepartmentID { get; set; }

        [Step(Step = 5, IsRequired = true, RequiredIfOtherField = "UniversityId", RequiredIfClearField = false)]
        [LocalizedDisplay("other_university")]
        public string UniversityOther { get; set; }

        [Step(Step = 5, IsRequired = true, RequiredIfOtherField = "CollegeId", RequiredIfClearField = false)]
        [LocalizedDisplay("other_college")]
        public string CollegeOther { get; set; }

        [Step(Step = 5, IsRequired = true, RequiredIfOtherField = "DepartmentID", RequiredIfClearField = false)]
        [LocalizedDisplay("other_department")]
        public string DepartmentOther { get; set; }

        [Step(Step = 5, IsRequired = true, FieldType = FormFieldType.Phone)]
        [LocalizedDisplay("mobile")]
        public string MobileNumber { get; set; }

        [Step(Step = 5, IsRequired = true)]
        [LocalizedDisplay("precise_specialization")]
        public string Specialization { get; set; }

        [Step(Step = 5, IsRequired = true, FieldType = FormFieldType.Email)]
        [LocalizedDisplay("email")]
        public string Email { get; set; }

        [Step(Step = 5, IsRequired = true)]
        [LocalizedDisplay("addresss")]
        public string Addresss { get; set; }
    }

    public class SuggestedActivityViewModel
    {
        public long? Id { get; set; }

        [Step(Step = 6, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("name")]
        public string Name { get; set; }

        [Step(Step = 6, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("activity_type")]
        public ActivityType? Type { get; set; }

        [Step(Step = 6, IsRequired = false, OnlyIfPropertyIsNotNull = "Name", RequiredIfOtherField = "activity_type", RequiredIfClearField = false)]
        [LocalizedDisplay("other_type")]
        public string OtherType { get; set; }

        [Step(Step = 6, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("address")]
        public string Address { get; set; }

        [Step(Step = 6, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("benefit_description")]
        public string Description { get; set; }
    }

    public class BeneficiaryViewModel
    {
        public long? Id { get; set; }

        [Step(Step = 6, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("beneficiary")]
        public string Name { get; set; }

        [Step(Step = 6, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("address")]
        public string Address { get; set; }

        [Step(Step = 6, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("benefit_description")]
        public string Description { get; set; }
    }

    public class PublishContainerViewModel
    {
        public long? Id { get; set; }

        [Step(Step = 6, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("container_name")]
        public string Name { get; set; }

        [Step(Step = 6, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("address")]
        public string Address { get; set; }

        [Step(Step = 6, IsRequired = true, OnlyIfPropertyIsNotNull = "Name")]
        [LocalizedDisplay("selection_reason")]
        public string SelectionReason { get; set; }
    }

    public enum ProjectType : byte
    {
        Unspecified = 0,
        Single = 1,
        Team = 2
    }

    public enum CoresearcherType : byte
    {
        Unspecified = 0,
        Single = 1,
        Team = 2
    }

    public enum ActivityType : byte
    {
        Unspecified = 0,
        Seminar = 1,
        Lecture = 2,
        Competition = 3,
        Article = 4,
        AcademicPoster = 5,
        AcademicJourney = 6,
        Conference = 7,
        Workshop = 8,
        TrainingCourse = 9,
        MediaActivity = 10,
    }
}