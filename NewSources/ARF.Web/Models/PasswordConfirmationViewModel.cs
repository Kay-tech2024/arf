using ARF.Web.App_Start.Code;
using System.ComponentModel.DataAnnotations;

namespace ARF.Web.Models
{
    public class PasswordConfirmationViewModel
    {
        [Required(ErrorMessage = " كلمة السر القديمة Requiredة")]
        [StringLength(100, ErrorMessage = "{0} يجب أن يكون {2} حرفا على الأقل.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplay("كلمة السر القديمة")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = " كلمة السر Requiredة")]
        [StringLength(100, ErrorMessage = "{0} يجب أن يكون {2} حرفا على الأقل.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplay("كلمة السر")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplay("تأكيد كلمة السر")]
        [Compare("Password", ErrorMessage = "Password والتأكيد Password غير متطابقة.")]
        public string ConfirmPassword { get; set; }
    }

    public class DeleteConfirmation
    {
        public string Id { get; set; }

        [Required(ErrorMessage = " كلمة السر Requiredة")]
        [DataType(DataType.Password)]
        [LocalizedDisplay("كلمة السر")]
        public string Password { get; set; }
    }
}