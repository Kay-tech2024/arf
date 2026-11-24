using ARF.Web.App_Start.Code;
using ARF.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace ARF.Web.Models
{
    public class registrationtestModel
    {
        [Required(ErrorMessageResourceName = "fullname_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("fullname")]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceName = "mobile_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("mobile")]
        public string MobileNumber { get; set; }

        //[Required(ErrorMessageResourceName = "birthdate_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("birthdate")]
        public DateTime? BirthDate { get; set; }

        //[Required(ErrorMessageResourceName = "university_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("university")]
        public int? UniversityId { get; set; }

        //[Required(ErrorMessageResourceName = "major_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("major")]
        public int? MajorId { get; set; }

        //[Required(ErrorMessageResourceName = "nationality_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("nationality")]
        public int? NationalityId { get; set; }

        //[Required(ErrorMessageResourceName = "country_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("country")]
        public int? CountryId { get; set; }


        //[Required(ErrorMessageResourceName = "college_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("college")]
        public int? CollegeId { get; set; }

        [Required(ErrorMessageResourceName = "address_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("address")]
        public string Address { get; set; }

        [Required(ErrorMessageResourceName = "email_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("email")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "password_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "password_confirmation_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [DataType(DataType.Password)]
        [LocalizedDisplay("password_confirmation")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }



    }


}