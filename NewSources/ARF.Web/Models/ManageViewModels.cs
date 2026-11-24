using ARF.Web.App_Start.Code;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ARF.Web.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplay("New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplay("Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeUserPasswordViewModel
    {


        [Editable(false)]
        public int? Id { get; set; }


        [Editable(false)]
        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [StringLength(100, ErrorMessage = "{0} يجب أن يكون {2} حرفا على الأقل.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplay("Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplay("تأكيد Password")]
        [Compare("Password", ErrorMessage = "Password والتأكيد Password غير متطابقة.")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Send credentials to email")]
        public bool SendCredentialsViaEmail { get; set; } = true;
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [DataType(DataType.Password)]
        [LocalizedDisplay("Current password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplay("New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplay("Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [Phone]
        [LocalizedDisplay("Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [LocalizedDisplay("Code")]
        public string Code { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [Phone]
        [LocalizedDisplay("Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

    public class UserRoleViewModel
    {
        public string UserId { get; set; }

        public string RoleId { get; set; }
    }

    public class UsersInfoViewModel
    {
        public List<UserInfo> Users { get; set; }

        public List<RoleViewModel> AllRoles { get; set; }
    }

    public class UserInfo
    {
        public UserViewModel User { get; set; }

        public RoleViewModel Role { get; set; }
    }

    public class RoleViewModel
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public int VirtualRoleCode { get; set; }
    }
}