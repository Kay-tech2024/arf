using ARF.Core;
using ARF.Data;
using ARF.Web.App_Start.Code;
using ARF.Web.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ARF.Web.Models
{
    public class ScientificLetterInformationViewModel
    {
        [Required(ErrorMessageResourceName = "sci_letterinformation_title_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("sci_letterinformation_title")]
        public string LetterTitle { get; set; }

        [Required(ErrorMessageResourceName = "degree_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("degree")]
        public string Degree { get; set; }

        [Required(ErrorMessageResourceName = "supervisor_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("supervisor")]
        public string Supervisor { get; set; }

        [Required(ErrorMessageResourceName = "discussion_committeemembers_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("discussion_committeemembers")]
        public string CommitteeMember { get; set; }

        [Required(ErrorMessageResourceName = "scientific_department_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("scientific_department")]
        public string ScientificDepartment { get; set; }

        [Required(ErrorMessageResourceName = "evaluation_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("evaluation")]
        public string Evaluation { get; set; }

        [Required(ErrorMessageResourceName = "discussiondate_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("discussion_date")]
        public DateTime? DateOfDiscussion { get; set; }


        [Required(ErrorMessageResourceName = "scientific_material_type_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("scientific_material_type")]
        public ScientificMaterialType ScientificMaterialType { get; set; }

        [Required(ErrorMessageResourceName = "publisher_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("publisher")]
        public string Publisher { get; set; }

        [Required(ErrorMessageResourceName = "publisher_place_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("publisher_place")]
        public string PublisherPlace { get; set; }

        [Required(ErrorMessageResourceName = "publisher_date_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("publisher_date")]
        public DateTime DateOfPublish { get; set; }

        [LocalizedDisplay("reason_for_reject")]
        public string ReasonForRejection { get; set; }



    }
}