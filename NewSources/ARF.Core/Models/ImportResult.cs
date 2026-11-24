using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARF.Core.Models
{
    public class ImportResult
    {
        public long Total { get; set; }
        public List<string> ImportedNins { get; set; } = new List<string>();
        public List<string> UpdatedNins { get; set; } = new List<string>();
        public long Skipped { get; set; }

        public Dictionary<string, string> ErrorsNins { get; set; } = new Dictionary<string, string>();


    }
}
