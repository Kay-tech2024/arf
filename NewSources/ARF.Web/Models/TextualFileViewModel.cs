using ARF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class TextualFileViewModel
    {
        public long? Id { get; set; }

        public FileType? Type { get; set; }

        public List<TextualFileSectinoViewModel> Sections { get; set; }
        
    }

    public class TextualFileSectinoViewModel
    {
        public long? Id { get; set; }

        public FileType? Type { get; set; }

        public string Html { get; set; }

        public bool IsRequired { get; set; } = false;
    }
}