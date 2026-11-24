using ARF.Core;
using ARF.Data;
using System;
using System.Collections.Generic;

namespace ARF.Web.Models
{
    public class RequestGridModel
    {
        public int Current { get; set; }
        public int RowCount { get; set; }
        public Dictionary<string, string> Sort { get; set; }
        public string SearchPhrase { get; set; }
        public RequestTab? Status { get; set; }

        public long? p { get; set; }

        public long? j { get; set; }

        public string u { get; set; }

    }
    public class FilterModel
    {
        public List<int?> Field { set; get; }
        public List<int?> College { get; set; }
        public List<int?> Requesttype { get; set; }
        public List<int?> Statustype { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public string Title { get; set; }
        public string Researcher { get; set; }
    }
}