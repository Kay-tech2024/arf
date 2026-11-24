using ARF.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class AuthorData
    {
        //[Required(ErrorMessageResourceType = typeof(ArfStrings), ErrorMessageResourceName = "author_name_requerd")]
        public string AuthorName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ArfStrings), ErrorMessageResourceName = "employer_required")]
        public string Employer { get; set; }

        public int? BankAccountType { get; set; }
        public bool IsPartOfScienceProduct { get; set; }
        //[Required(ErrorMessageResourceType =typeof(ArfStrings),ErrorMessageResourceName ="City is Requerd")]
        public string City { get; set; }
        //[Required(ErrorMessageResourceType = typeof(ArfStrings), ErrorMessageResourceName = "Country is Requerd")]
        public string Country { get; set; }
        public string FilePath { get; set; }
        public string IBAN { get; set; }
        public bool IsAgree { get; set; }
        public bool IsNotAgree { get; set; }

        public string PartnerEmail { get; set; }

        public string PartnerMobile { get; set; }

        public string PartnerName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ArfStrings), ErrorMessageResourceName = "city")]
        public string ParticipantName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ArfStrings), ErrorMessageResourceName = "Participant Email Required")]
        public string ParticipantEmail { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ArfStrings), ErrorMessageResourceName = "MobileNumber Required")]
        public string ParticipanMobileNumber { get; set; }








    }
}