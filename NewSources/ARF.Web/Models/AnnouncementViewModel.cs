using ARF.Core.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class AnnouncementViewModel
    {

        public long Id { get; set; }
        public string Title { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string Status { get; set; }

        public List<AttachedFile> Files { get; set; } = new List<AttachedFile>();
    }
}