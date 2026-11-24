using ARF.Core.Models;
using ARF.Web.App_Start.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class CollegeViewModel
    {
        [Editable(false)]
        public long? Id { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ArfStrings), ErrorMessageResourceName = "name_english_required")]
        [LocalizedDisplay("name_english")]
        [Unique(TableName = "Colleges")]
        public string NameEnglish { get; set; }

        [LocalizedDisplay("name_arabic")]
        [Unique(TableName = "Colleges")]
        public string NameArabic { get; set; }

        public int RoleGroupCode { get; set; }

        public List<AttachedFile> Files { get; set; }
    }
}