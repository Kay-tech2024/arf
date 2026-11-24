using ARF.Core;
using ARF.Web.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Collections.Generic;
using ARF.Web.App_Start.Code;

namespace ARF.Web.Models
{
    public class CoResearcherEncyclopediaModel
    {
        [Required(ErrorMessageResourceName = "encyclopedia_major", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("encyclopedia_major")]
        public int MajorId { get; set; }

        [Required(ErrorMessageResourceName = "encyclopedia_lang", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("encyclopedia_lang")]
        public int languageId { get; set; }

        [Required(ErrorMessageResourceName = "encyclopedia_title", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("encyclopedia_title")]
        public string EncyclopediaTitle { get; set; }

        [Required(ErrorMessageResourceName = "encyclopedia_original_title", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("encyclopedia_original_title")]
        public string EncyclopediaOriginalTitle { get; set; }

        [Required(ErrorMessageResourceName = "author_major", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("author_Major")]
        public int MajorAuthorId { get; set; }

        [Required(ErrorMessageResourceName = "recommendation", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("recommendation")]
        public string Recommendation { get; set; }

        public bool IsGovernmental { get; set; }

        [LocalizedDisplay("publisher")]
        public string PublisherName { get; set; }

        [LocalizedDisplay("publisher_place")]
        public string PublisherPlace { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [LocalizedDisplay("date")]
        public string date { get; set; }

        [LocalizedDisplay("reason_for_reject")]
        public string ReasonOfReject { get; set; }

        public bool ISApproved { get; set; }
        public string SelectedAction { get; set; }

    }
    public class CoResearcherDictionaryModel
    {
        public int RequestId { get; set; }
        [Required(ErrorMessageResourceName = "majorof_dictionary", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("majorof_dictionary")]
        public int MajorId { get; set; }

        [Required(ErrorMessageResourceName = "dictionary_language", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("dictionary_language")]
        public int languageId { get; set; }
        

        [Required(ErrorMessageResourceName = "dictionary_title", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("dictionary_title")]
        public string DictionaryTitle { get; set; }

        [Required(ErrorMessageResourceName = "dictionary_title_original", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("dictionary_title_original")]
        public string OriginalDictionaryTitle { get; set; }

        [Required(ErrorMessageResourceName = "author_major", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("author_Major")]
        public int MajorAuthorId { get; set; }

        [Required(ErrorMessageResourceName = "recommendation_number", ErrorMessageResourceType = typeof(ArfStrings))]
        [LocalizedDisplay("recommendation_number")]
        public long NumberOfRecommendations { get; set; }

        public bool IsGovernmental { get; set; }

        [LocalizedDisplay("publisher")]
        public string PublisherName { get; set; }

        [LocalizedDisplay("publisher_place")]
        public string PublisherPlace { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [LocalizedDisplay("date")]
        public string date { get; set; }

        [LocalizedDisplay("reason_for_reject")]
        public string ReasonOfReject { get; set; }

        public bool ISApproved { get; set; }

        public string SelectedAction { get; set; }

    }


}