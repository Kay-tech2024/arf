using ARF.Web.Models;
using System.Collections.Generic;

namespace ARF.Web.App_Start.Code
{
    public class Lookups
    {
        public static IEnumerable<DropDown> FacultyOptions = new List<DropDown>
        {
            new DropDown {ID = 0, Value = "Faculty 1"},
            new DropDown {ID = 1, Value = "Faculty 2"}
        };
        public static IEnumerable<DropDown> MajorOptions = new List<DropDown>
       {
            new DropDown {ID = 0, Value = "Major 1"},
            new DropDown {ID = 1, Value = "Major 2"}
       };
        public static IEnumerable<DropDown> ScientificMaterialTypeOptions = new List<DropDown>
       {
            new DropDown {ID = 0, Value = "Scientific Material Type 1"},
            new DropDown {ID = 1, Value = "Scientific Material Type 2"}
       };

        public static IEnumerable<DropDown> DepartmentOptions = new List<DropDown>
       {
            new DropDown {ID = 0, Value = "Department 1"},
            new DropDown {ID = 1, Value = "Department 2"}
       };

        public static IEnumerable<DropDown> SciDepartmentOptions = new List<DropDown>
       {
            new DropDown {ID = 0, Value = "Scientific Department 1"},
            new DropDown {ID = 1, Value = "Scientific Department 2"}
       };
        public static IEnumerable<DropDown> CountryOptions = new List<DropDown>
        {
            new DropDown {ID = 0, Value = "اختار..."},
            new DropDown {ID = 1, Value = "المملكة العربية السعودية"},
            new DropDown {ID = 2, Value = "الامارات"},
            new DropDown {ID = 3, Value = "بحرين"},
            new DropDown {ID = 4, Value = "عمن"},
            new DropDown {ID = 5, Value = "الخ"}
        };

    }
}