using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class ContactMessagesViewModel
    {
        public long Id { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
        public DateTime Created { get; set; }

        public bool IsDeleted { get; set; }
    }
}