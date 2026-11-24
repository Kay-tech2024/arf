using ARF.Core;
using ARF.Data;
using ARF.Web.App_Start.Code;
using ARF.Web.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class PublishOrderViewModel
    {

        public long? RequestId { get; set; }

        public class NameComponents

        {

            public string FirstName { get; set; }

            public string SecondName { get; set; }

            public string ThirdName { get; set; }

            public string LastName { get; set; }

        }
        public IEnumerable<DropDown> FacultyOptions
        {
            get
            {
                return Lookups.FacultyOptions;
            }
        }

        public IEnumerable<DropDown> MajorOptions
        {
            get
            {
                return Lookups.MajorOptions;
            }
        }
        public IEnumerable<DropDown> ScientificMaterialTypeOptions
        {
            get
            {
                return Lookups.ScientificMaterialTypeOptions;
            }
        }

        [Required(ErrorMessageResourceName = "researcher_name_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("researcher_name")]
        public string ResearcherName { get; set; }

        [Required(ErrorMessageResourceName = "select_faculty", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("faculity")]
        public int? FacultyId { get; set; }

        [LocalizedDisplay("major")]
        public int? MajorId { get; set; }

        [Required(ErrorMessageResourceName = "scientific_material_type_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("scientific_material_type")]
        public string ScientificMaterialTypeId { get; set; }

        [Required(ErrorMessageResourceName = "publish_type_required", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("publish_type")]
        public PublishTimeType PublishType { get; set; }


        [LocalizedDisplay("related_funding_request")]
        public long? FundingRequestId { get; set; }

        public SelectList PreviousFundingRequests { get; set; }
    }

}