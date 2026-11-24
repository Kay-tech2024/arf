using ARF.Core.Models;
using ARF.Data;
using ARF.Web.App_Start.Code;
using ARF.Web.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class RegistrationViewModel
    {
        public IEnumerable<DropDown> CountryOptions
        {
            get
            {
                return Lookups.CountryOptions;
            }
        }


        public int? Step { get; set; }
        public int? LastSavedStep { get; set; }

        public ArfUserRole? Role { get; set; }

        
        [LocalizedDisplay("national_id")]
        public string NationalId { get; set; }

        public string ExternalRefereeHashParams { get; set; }

        [LocalizedDisplay("fullname")]
        public string FullName { get; set; }

        
        [LocalizedDisplay("mobile")]
        public string MobileNumber { get; set; }

        //
        [LocalizedDisplay("university")]
        public string UniversityOther { get; set; }

        //
        [LocalizedDisplay("college")]
        public string CollegeOther { get; set; }

        [LocalizedDisplay("department")]
        public string DepartmentOther { get; set; }

        //
        [LocalizedDisplay("major")]
        public string MajorOther { get; set; }

        //
        [LocalizedDisplay("birthdate")]
        public string BirthDate { get; set; }

        //
        [LocalizedDisplay("university")]
        public int? UniversityId { get; set; }

        [LocalizedDisplay("university")]
        public string UniversityName { get; set; }

        //
        [LocalizedDisplay("major")]
        public int? MajorId { get; set; }

        //
        [LocalizedDisplay("nationality")]
        public int? NationalityId { get; set; }

        //
        [LocalizedDisplay("residence_country")]
        public int? CountryId { get; set; }

        //
        [LocalizedDisplay("college")]
        public int? CollegeId { get; set; }

        
        [LocalizedDisplay("address")]
        public string Address { get; set; }

        
        [LocalizedDisplay("email")]
        public string Email { get; set; }

        [LocalizedDisplay("email_confirmation")]
        [System.ComponentModel.DataAnnotations.Compare("Email")]
        public string EmailConfirmation { get; set; }

        [LocalizedDisplay("password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplay("password_confirmation")]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }

        [LocalizedDisplay("bank_name")]
        public string BankName { get; set; }

        [LocalizedDisplay("bank_branch")]
        public string BankBranch { get; set; }

        [LocalizedDisplay("account_holder_name")]
        public string BankAccountHolderName { get; set; }

        [LocalizedDisplay("iban")]
        public string BankAccountIBAN { get; set; }

        [LocalizedDisplay("resume")]
        public HttpPostedFileBase File { get; set; }

        public string SmsKey { get; set; }

        public string EmailKey { get; set; }

        [LocalizedDisplay("sms_code")]
        public string SmsCode { get; set; }

        [LocalizedDisplay("sms_code")]
        public string hdnIsMobileVerified { get; set; }

        [LocalizedDisplay("email_code")]
        public string EmailCode { get; set; }

        [LocalizedDisplay("residence_city")]
        public int? CityId { get; set; }

        //
        
        [LocalizedDisplay("Qualification")]
        public QualificationType? Qualification { get; set; }

        
        [LocalizedDisplay("department")]
        public int? DepartmentID { get; set; }

        [LocalizedDisplay("researcher_specialization")]
        public string Specialization { get; set; }

        
        [LocalizedDisplay("Bank")]
        public int? Bank { get; set; }

        //
        [LocalizedDisplay("iban_photocopy")]
        public HttpPostedFileBase BankIbanPhotocopy { get; set; }

        public AttachedFile BankIbanPhotocopyReadonly { get; set; }

        //
        [LocalizedDisplay("researcher_type")]
        public ResearcherRole? RoleInProject { get; set; }

        //[Step(IsRequired = true, RequiredIfOtherField = "Bank")]
        [LocalizedDisplay("other_bank")]
        public string BankOther { get; set; }

        //
        [LocalizedDisplay("SectionPublishedPapers")]
        [AllowHtml]
        public string SectionPublishedPapers { get; set; }

        
        [LocalizedDisplay("SectionInterests")]
        [AllowHtml]
        public string SectionInterests { get; set; }

        
        [LocalizedDisplay("SectionCareerHistory")]
        [AllowHtml]
        public string SectionCareerHistory { get; set; }

        
        [LocalizedDisplay("SectionAcademicHistory")]
        [AllowHtml]
        public string SectionAcademicHistory { get; set; }

        
        [LocalizedDisplay("SectionTrainingCourses")]
        [AllowHtml]
        public string SectionTrainingCourses { get; set; }

        
        [LocalizedDisplay("SectionAwards")]
        [AllowHtml]
        public string SectionAwards { get; set; }

        
        [LocalizedDisplay("SectionMemberships")]
        [AllowHtml]
        public string SectionMemberships { get; set; }

        [AllowHtml]
        [LocalizedDisplay("cv_text")]
        public string CvText { get; set; }

        // Lookups
        public List<CityViewModel> Cities { get; set; }

        public SelectListItem[] Universities { get; set; }
        public SelectListItem[] Colleges { get; set; }
        public SelectListItem[] Departments { get; set; }
        public SelectListItem[] Banks { get; set; }
        public SelectListItem[] QualificationTypes { get; set; }
        public SelectListItem[] Countries { get; set; }
    }
}