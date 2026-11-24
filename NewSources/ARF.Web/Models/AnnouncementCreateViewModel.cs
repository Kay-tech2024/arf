using ARF.Core.Models;
using ARF.Web.App_Start.Code;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class AnnouncementCreateViewModel
    {
        [Required(ErrorMessage = "العنوان مطلوب", AllowEmptyStrings = false)]
        [RegularExpression(@"^[\u0621-\u064A0-9\-\(\)\s]+$", ErrorMessage = "يجب أن يكون العنوان عربيًا")]
        [Unique(TableName = "Announcements", ErrorMessage = "هذا العنوان موجود مسبقًا")]
        public string Title { get; set; }

        [Required(ErrorMessage = "المحتوى مطلوب", AllowEmptyStrings = false)]
        [AllowHtml]
        public string Content { get; set; }

        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        public string StartDate { get; set; }

        [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
        public string EndDate { get; set; }

        public List<AttachedFile> Files { get; set; } = new List<AttachedFile>();

    }
}