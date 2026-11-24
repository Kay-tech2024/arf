using APS.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APS.Web.Models
{
    public class Participant
    {

        [Required(ErrorMessageResourceType = typeof(ApsStrings), ErrorMessageResourceName = "Participant Name Required")]
        public string ParticipantName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ApsStrings), ErrorMessageResourceName = "Participant Email Required")]
        public string ParticipantEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(ApsStrings), ErrorMessageResourceName = "MobileNumber Required")]
        public string ParticipanMobileNumber { get; set; }







    }
}