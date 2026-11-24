using ARF.Data;
using System.Collections.Generic;
using System.Web;

namespace ARF.Web.Models
{
    public class ImportViewModel
    {
        public ImportFormat Format { get; set; }

        public bool Simulate { get; set; } = false;
        public long? ProviderId { get; set; }
        public long? JpcId { get; set; }

        public HttpPostedFileBase File { get; set; }

        public string Email { get; set; }

        public List<string> ImportFormats { get; set; } = new List<string> { ImportFormat.Referees.ToString(), ImportFormat.Researchers.ToString() };
    }
}