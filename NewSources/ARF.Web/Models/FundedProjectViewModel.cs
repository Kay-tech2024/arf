using ARF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class FundedProjectViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long? MidTermReportRequestId { get; set; }
        public long? FullTermReportRequestId { get; set; }
        public long? MaterialRequestRequestId { get; set; }
        public long? EditRequestRequestId { get; set; }
        public int RequestedMaterialCount { get; set; }
        public ReqFieldType? FieldType { get; set; }
    }
}