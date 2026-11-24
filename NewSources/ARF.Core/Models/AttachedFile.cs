using ARF.Data;
using System;
using System.Web;

namespace ARF.Core.Models
{
    public class AttachedFile
    {
        public string FileId { get; set; }

        public FileType Type { get; set; }


        public bool IsDownloaded { get; set; }

        public string FileName { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }


        public string FilePath { get; set; }

        public HttpPostedFileBase Content { get; set; }
    }
}