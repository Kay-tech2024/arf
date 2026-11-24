using ARF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class RequestSummaryTabbedViewModel
    {
        public ReqFieldType Type { get; set; }
        public List<RequestSummaryTabViewModel> tabs { get; set; }

        public RequestSummaryTabbedViewModel()
        {
            tabs = new List<RequestSummaryTabViewModel>();
        }
    }

    public class RequestSummaryTabViewModel
    {
        public string TabName { get; set; }
        public string TabId { get; set; } = "0";
        public long RequestId { get; set; }
    }

}