using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class CoResearcherRequestReviewResponseViewModel
    {
        public long RequestId { get; set; }
        public long RequestCoResearcherResponseId { get; set; }       
        public string Email { get; set; }
        public string Action { get; set; }
        public string ParamsHash { get; set; }
    }
}