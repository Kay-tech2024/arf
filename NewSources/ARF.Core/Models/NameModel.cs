using ARF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARF.Core.Models
{
    public class NameModel
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public ReqFieldType? Field { get; set; }
        public int Code { get; set; }
    }
}
