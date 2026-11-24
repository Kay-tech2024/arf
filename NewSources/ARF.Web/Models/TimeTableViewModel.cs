using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class TimeTableViewModel
    {
        public string Task { get; set; }
        public System.Guid Id { get; set; }
        public float Percentage { get; set; }
        public string DelayReasons { get; set; }
    }
}