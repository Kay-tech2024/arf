using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARF.Core.Models
{
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
