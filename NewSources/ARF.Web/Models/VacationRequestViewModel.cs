using ARF.Core;
using ARF.Core.Models;
using ARF.Data;
using ARF.Web.App_Start.Code;
using ARF.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class VacationRequestViewModel : RequestViewModel
    {

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [LocalizedDisplay("vacation_type")]
        public VacationType? VacationType { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [LocalizedDisplay("job_title")]
        public string JobTitle { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [LocalizedDisplay("rank")]
        public string Rank { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [LocalizedDisplay("vacation_in_days")]
        public uint? VacationInDays { get; set; }


        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [LocalizedDisplay("vacation_start_date")]
        public string VacationStartDate { get; set; }


        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [LocalizedDisplay("vacation_address")]
        public string VacationAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "field_required")]
        [LocalizedDisplay("mobile")]
        public string MobileNumber { get; set; }




    }


}