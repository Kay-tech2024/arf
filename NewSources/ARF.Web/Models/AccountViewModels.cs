using ARF.Core.Models;
using ARF.Data;
using ARF.Web.App_Start.Code;
using ARF.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessageResourceName = "email_required", ErrorMessageResourceType = typeof(Resources.ArfStrings))]
        [LocalizedDisplay("email")]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedDepartment { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Departments { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        public string Department { get; set; }
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "email_required")]
        [EmailAddress]
        [LocalizedDisplay("email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "username_required")]
        [LocalizedDisplay("username")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "password_required")]
        [DataType(DataType.Password)]
        [LocalizedDisplay("password")]
        public string Password { get; set; }

        [LocalizedDisplay("keep_me_signed_in")]
        public bool RememberMe { get; set; } = false;
    }

    public class ResetPasswordViewModel
    {
        [EmailAddress]
        [LocalizedDisplay("email")]
        public string Email { get; set; }

        [LocalizedDisplay("mobile")]
        public string MobileNumber { get; set; }

        public string SmsKey { get; set; }

        [LocalizedDisplay("sms_code")]
        public string SmsCode { get; set; }

        [LocalizedDisplay("password")]
        public string Password { get; set; }

        [LocalizedDisplay("password_confirmation")]
        public string ConfirmPassword { get; set; }

        public string Key { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "email_required")]
        [LocalizedDisplay("email")]
        public string Email { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "name_required")]
        [LocalizedDisplay("fullname")]
        public string FullName { get; set; }

        [EmailAddress]
        [Unique(TableName = "Users")]
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "email_required")]
        [LocalizedDisplay("email")]
        public string Email { get; set; }


        [LocalizedDisplay("mobile")]
        public string MobileNo { get; set; }

        [Unique(TableName = "Users")]
        [Required(ErrorMessageResourceName = "username_required", ErrorMessageResourceType = typeof(Resources.ArfStrings))]
        [LocalizedDisplay("Username")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "role_required")]
        [LocalizedDisplay("Role")]
        public string RoleId { get; set; }


        [LocalizedDisplay("college")]
        public int? CollegeId { get; set; }


        [LocalizedDisplay("college")]
        public string CollegeName { get; set; }

        [LocalizedDisplay("major")]
        public int? MajorId { get; set; }

        [LocalizedDisplay("major")]
        public string MajorName { get; set; }


        [LocalizedDisplay("national_id")]
        public string NationalId { get; set; }


        [LocalizedDisplay("Role")]
        public string RoleName { get; set; }

        [Required(ErrorMessageResourceName = "password_required", ErrorMessageResourceType = typeof(Resources.ArfStrings))]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplay("New password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplay("Password confirmation")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "password_confirmation_mismatch")]
        public string ConfirmPassword { get; set; }

        public string UserRoleId { get; set; }

        [Required]
        [LocalizedDisplay("activate_immediately")]
        public bool IsActivated { get; set; } = true;

        [Required]
        [LocalizedDisplay("Force password change")]
        public bool ForcePasswordChange { get; set; } = true;

        [LocalizedDisplay("Send credentials to email")]
        public bool SendCredentialsViaEmail { get; set; } = true;

        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();

        public List<CollegeViewModel> Colleges { get; set; } = new List<CollegeViewModel>();

        public List<MajorViewModel> Majors { get; set; } = new List<MajorViewModel>();

        [LocalizedDisplay("nationality")]
        public int? NationalityId { get; set; }

        [LocalizedDisplay("birthdate")]
        public string BirthDate { get; set; }

        [LocalizedDisplay("country")]
        public int? CountryId { get; set; }

        [LocalizedDisplay("residence_city")]
        public int? CityId { get; set; }


        [LocalizedDisplay("address")]
        public string Address { get; set; }


        [LocalizedDisplay("Qualification")]
        public QualificationType? Qualification { get; set; }

        [LocalizedDisplay("researcher_type")]
        public ResearcherRole? RoleInProject { get; set; }


        [LocalizedDisplay("university")]
        public int? UniversityId { get; set; }


        [LocalizedDisplay("university")]
        public string UniversityOther { get; set; }


        [LocalizedDisplay("college")]
        public string CollegeOther { get; set; }

        [LocalizedDisplay("department")]
        public string DepartmentOther { get; set; }


        [LocalizedDisplay("department")]
        public int? DepartmentID { get; set; }

        [LocalizedDisplay("researcher_specialization")]
        public string Specialization { get; set; }

        [LocalizedDisplay("Bank")]
        public int? Bank { get; set; }

        [LocalizedDisplay("iban_photocopy")]
        public HttpPostedFileBase BankIbanPhotocopy { get; set; }

        public AttachedFile BankIbanPhotocopyReadonly { get; set; }

        [LocalizedDisplay("other_bank")]
        public string BankOther { get; set; }

        [LocalizedDisplay("bank_name")]
        public string BankName { get; set; }
        [LocalizedDisplay("bank_branch")]
        public string BankBranch { get; set; }
        [LocalizedDisplay("account_holder_name")]
        public string BankAccountHolderName { get; set; }
        [LocalizedDisplay("iban")]
        public string BankAccountIBAN { get; set; }

        [LocalizedDisplay("major")]
        public string MajorOther { get; set; }

        public ArfUserRole? Role { get; set; }



        [Required]
        [LocalizedDisplay("EmailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [Required]
        [LocalizedDisplay("PhoneNumberConfirmed")]
        public bool PhoneNumberConfirmed { get; set; }


        // Lookups
        public List<CityViewModel> Cities { get; set; }
        public SelectListItem[] Universities { get; set; }
        // public SelectListItem[] Colleges { get; set; }
        public SelectListItem[] Departments { get; set; }
        public SelectListItem[] Banks { get; set; }
        public SelectListItem[] QualificationTypes { get; set; }
        public SelectListItem[] Countries { get; set; }

        // CVs
        [LocalizedDisplay("SectionPublishedPapers")]
        [AllowHtml]
        public string SectionPublishedPapers { get; set; }

        [LocalizedDisplay("SectionInterests")]
        [AllowHtml]
        public string SectionInterests { get; set; }

        [LocalizedDisplay("SectionCareerHistory")]
        [AllowHtml]
        public string SectionCareerHistory { get; set; }

        [LocalizedDisplay("SectionTrainingCourses")]
        [AllowHtml]
        public string SectionTrainingCourses { get; set; }

        [LocalizedDisplay("SectionAwards")]
        [AllowHtml]
        public string SectionAwards { get; set; }

        [LocalizedDisplay("SectionMemberships")]
        [AllowHtml]
        public string SectionMemberships { get; set; }

        [LocalizedDisplay("SectionAcademicHistory")]
        [AllowHtml]
        public string SectionAcademicHistory { get; set; }

        [AllowHtml]
        [LocalizedDisplay("cv_text")]
        public string CvText { get; set; }
    }

    public class EditUserViewModel
    {
        [Editable(false)]
        public string Id { get; set; }

        [Editable(false)]
        public string UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [LocalizedDisplay("fullname")]
        public string FullName { get; set; }

        [LocalizedDisplay("national_id")]
        public string NationalId { get; set; }

        //[Unique(TableName = "Users")]
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "email_required")]
        [EmailAddress]
        [LocalizedDisplay("email")]
        public string Email { get; set; }

        [LocalizedDisplay("mobile")]
        public string MobileNo { get; set; }

        //[Unique(TableName = "Users")]
        [LocalizedDisplay("username")]
        public string UserName { get; set; }

        //    [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [LocalizedDisplay("role")]
        public string RoleId { get; set; }


        [LocalizedDisplay("college")]
        public int? CollegeId { get; set; }

        [LocalizedDisplay("college")]
        public string CollegeName { get; set; }


        [LocalizedDisplay("major")]
        public int? MajorId { get; set; }

        [LocalizedDisplay("major")]
        public string MajorName { get; set; }

        [LocalizedDisplay("role")]
        public string RoleName { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplay("password")]
        public string CurrentPassword { get; set; }

        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplay("password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplay("password_confirmation")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "password_confirmation_mismatch")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Send credentials to email")]
        public bool SendCredentialsViaEmail { get; set; } = true;


        public string UserRoleId { get; set; }

        [Required]
        [LocalizedDisplay("activation")]
        public bool IsActivated { get; set; }

        [Required]
        [LocalizedDisplay("PhoneNumberConfirmed")]
        public bool PhoneNumberConfirmed { get; set; }


        [Required]
        [LocalizedDisplay("EmailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [Required]
        [LocalizedDisplay("force_password_change")]
        public bool ForcePasswordChange { get; set; }


        [LocalizedDisplay("nationality")]
        public int? NationalityId { get; set; }


        [LocalizedDisplay("birthdate")]
        public string BirthDate { get; set; }


        [LocalizedDisplay("residence_country")]
        public int? CountryId { get; set; }

        [LocalizedDisplay("residence_city")]
        public int? CityId { get; set; }

        [LocalizedDisplay("address")]
        public string Address { get; set; }


        [LocalizedDisplay("Qualification")]
        public QualificationType? Qualification { get; set; }

        [LocalizedDisplay("researcher_type")]
        public ResearcherRole? RoleInProject { get; set; }


        [LocalizedDisplay("university")]
        public int? UniversityId { get; set; }

        [LocalizedDisplay("university")]
        public string UniversityName { get; set; }


        [LocalizedDisplay("university")]
        public string UniversityOther { get; set; }


        [LocalizedDisplay("college")]
        public string CollegeOther { get; set; }


        [LocalizedDisplay("major")]
        public string MajorOther { get; set; }

        [LocalizedDisplay("department")]
        public string DepartmentOther { get; set; }


        [LocalizedDisplay("department")]
        public int? DepartmentID { get; set; }

        [LocalizedDisplay("researcher_specialization")]
        public string Specialization { get; set; }


        [LocalizedDisplay("Bank")]
        public int? Bank { get; set; }


        [LocalizedDisplay("iban_photocopy")]
        public HttpPostedFileBase BankIbanPhotocopy { get; set; }

        public AttachedFile BankIbanPhotocopyReadonly { get; set; }

        [LocalizedDisplay("other_bank")]
        public string BankOther { get; set; }

        [LocalizedDisplay("bank_name")]
        public string BankName { get; set; }
        [LocalizedDisplay("bank_branch")]
        public string BankBranch { get; set; }
        [LocalizedDisplay("account_holder_name")]
        public string BankAccountHolderName { get; set; }
        [LocalizedDisplay("iban")]
        public string BankAccountIBAN { get; set; }

        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();

        public List<CollegeViewModel> Colleges { get; set; } = new List<CollegeViewModel>();

        public List<MajorViewModel> Majors { get; set; } = new List<MajorViewModel>();

        public int RoleGroupCode { get; set; }
        // Lookups
        public List<CityViewModel> Cities { get; set; }
        public SelectListItem[] Universities { get; set; }
        // public SelectListItem[] Colleges { get; set; }
        public SelectListItem[] Departments { get; set; }
        public SelectListItem[] Banks { get; set; }
        public SelectListItem[] QualificationTypes { get; set; }
        public SelectListItem[] Countries { get; set; }

        // CVs
        [LocalizedDisplay("SectionPublishedPapers")]
        [AllowHtml]
        public string SectionPublishedPapers { get; set; }

        [LocalizedDisplay("SectionInterests")]
        [AllowHtml]
        public string SectionInterests { get; set; }

        [LocalizedDisplay("SectionCareerHistory")]
        [AllowHtml]
        public string SectionCareerHistory { get; set; }

        [LocalizedDisplay("SectionTrainingCourses")]
        [AllowHtml]
        public string SectionTrainingCourses { get; set; }

        [LocalizedDisplay("SectionAwards")]
        [AllowHtml]
        public string SectionAwards { get; set; }

        [LocalizedDisplay("SectionMemberships")]
        [AllowHtml]
        public string SectionMemberships { get; set; }

        [LocalizedDisplay("SectionAcademicHistory")]
        [AllowHtml]
        public string SectionAcademicHistory { get; set; }

        [AllowHtml]
        [LocalizedDisplay("cv_text")]
        public string CvText { get; set; }


        public List<ExtraRoleViewModel> ExtraRoles { get; set; } = new List<ExtraRoleViewModel>();
    }


    public class ExtraRoleViewModel
    {
        public long? Id { get; set; }
        public string RoleId { get; set; }
        public ArfUserRole Role { get; set; }
        public int? CollegeId { get; set; }
        public string EffectiveFrom { get; set; }
        public DateTime? EffectiveFromDate { get; set; }
        public string EffectiveTo { get; set; }
        public DateTime? EffectiveToDate { get; set; }
        public bool IsActive { get; set; } = false;
    }


    public class UserPreferenceModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ContactUsViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ArfStrings), ErrorMessageResourceName = "name_required")]
        [LocalizedDisplay("fullname")]
        public string FullName { get; set; }

        [EmailAddress]
        [Required(ErrorMessageResourceType = typeof(ArfStrings), ErrorMessageResourceName = "email_required")]
        [LocalizedDisplay("email")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(ArfStrings), ErrorMessageResourceName = "email_required")]
        [LocalizedDisplay("subject")]
        public string Subject { get; set; }

        [Required(ErrorMessageResourceType = typeof(ArfStrings), ErrorMessageResourceName = "contactusmessage_required")]
        [LocalizedDisplay("contactusmessage")]
        public string Message { get; set; }
    }

    public class MemberCreateViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "name_required")]
        [LocalizedDisplay("fullname")]
        public string FullName { get; set; }

        [EmailAddress]        
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "email_required")]
        [LocalizedDisplay("email")]
        public string Email { get; set; }
        
        [LocalizedDisplay("password")]
        public string Password { get; set; }

        [LocalizedDisplay("mobile")]
        public string MobileNo { get; set; }

        [Unique(TableName = "Users")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "role_required")]
        [LocalizedDisplay("Role")]
        public string RoleId { get; set; }

        [LocalizedDisplay("college")]
        public int? CollegeId { get; set; }

        public int? UniversityId { get; set; }

        [LocalizedDisplay("college")]
        public string CollegeName { get; set; }

        [LocalizedDisplay("major")]
        public int? MajorId { get; set; }

        [LocalizedDisplay("major")]
        public string MajorName { get; set; }

        [LocalizedDisplay("national_id")]
        public string NationalId { get; set; }

        [LocalizedDisplay("Role")]
        public string RoleName { get; set; }

        public string UserRoleId { get; set; }

        [Required]
        [LocalizedDisplay("activate_immediately")]
        public bool IsActivated { get; set; } = true;

        [Required]
        [LocalizedDisplay("Force password change")]
        public bool ForcePasswordChange { get; set; } = true;

        [LocalizedDisplay("Send credentials to email")]
        public bool SendCredentialsViaEmail { get; set; } = true;

        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();

        public List<CollegeViewModel> Colleges { get; set; } = new List<CollegeViewModel>();

        public List<MajorViewModel> Majors { get; set; } = new List<MajorViewModel>();

        [LocalizedDisplay("nationality")]
        public int? NationalityId { get; set; }
    }

}