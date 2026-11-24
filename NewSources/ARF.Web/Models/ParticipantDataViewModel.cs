using ARF.Web.App_Start.Code;
using ARF.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class ParticipantDataViewModel
    {
        public long? RequestId { get; set; }

        public List<SingleParticipantDataViewModel> ParticipantDetail { get; set; }
        
    }

    public class SingleParticipantDataViewModel
    {
        [Required(ErrorMessageResourceName = "participant_name_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("participant_name")]
        public string ParticipantName { get; set; }

        [Required(ErrorMessageResourceName = "email_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("book_type")]
        public string ParticipantEmail { get; set; }

        [Required(ErrorMessageResourceName = "mobile_no_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("book_type")]
        public string ParticipanMobileNumber { get; set; }


    }
}