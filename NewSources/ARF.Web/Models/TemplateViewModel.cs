using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class TemplateViewModel
    {
        [Required]
        public string TemplateKey { get; set; }

        [AllowHtml]
        public string SubjectEnglish { get; set; }

        [AllowHtml]
        public string BodyEnglish { get; set; }

        [AllowHtml]
        public string SubjectArabic { get; set; }

        [AllowHtml]
        public string BodyArabic { get; set; }

        public bool IsHtml { get; set; }

    }
}