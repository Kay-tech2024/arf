using ARF.Web.App_Start.Code;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ARF.Web.Models
{
    public class GroupViewModel
    {
        public long Id { get; set; }
        public int CollegeId { get; set; }
        public string Name { get; set; }
        public string CollegeName { get; set; }

        [LocalizedDisplay("name_english")]
        public string NameEnglish { get; set; }

        [LocalizedDisplay("name_arabic")]
        public string NameArabic { get; set; }

        public List<CollegeViewModel> Departments { get; set; } = new List<CollegeViewModel>();
    }
}