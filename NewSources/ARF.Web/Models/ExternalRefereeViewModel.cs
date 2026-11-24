using System.ComponentModel.DataAnnotations;


namespace ARF.Web.Models
{
    public class ExternalRefereeViewModel
    {
        [Required(ErrorMessageResourceName = "email_required", ErrorMessageResourceType = typeof(Resources.ArfStrings))]
        [EmailAddress(ErrorMessageResourceName = "email_required", ErrorMessageResourceType = typeof(Resources.ArfStrings))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "name_required")]        
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "mobile_required")]
        public string MobileNumber { get; set; }

    }
}