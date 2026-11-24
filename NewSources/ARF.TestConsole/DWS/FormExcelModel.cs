using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARF.TestConsole.DWS
{
    public class FormExcelModel
    {
        public string ID { get; set; }
        public string FormNo { get; set; }
        public string FormName { get; set; }
        public string NameArabic { get; set; }
        public string NameEnglish { get; set; }
        public string Type { get; set; }
        public string ReadOnly { get; set; }
        public string Required { get; set; }
        public string MinimumCharacters { get; set; }
        public string MaximumCharacters { get; set; }
        public string Choices { get; set; }
        public string Section { get; set; }
        public string DependsOn { get; set; }

    }
}
