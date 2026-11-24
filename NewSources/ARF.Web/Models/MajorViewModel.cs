using ARF.Core.Models;
using ARF.Web.App_Start.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class MajorViewModel
    {
        [Editable(false)]
        public long? Id { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "name_english_required")]
        [LocalizedDisplay("name_english")]
        [Unique(TableName = "Majors")]
        public string NameEnglish { get; set; }

        [LocalizedDisplay("name_arabic")]
        [Unique(TableName = "Majors")]
        public string NameArabic { get; set; }

       
    }
}