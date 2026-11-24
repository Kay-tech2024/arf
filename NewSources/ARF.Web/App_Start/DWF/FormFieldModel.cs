using ARF.Data;
using ARF.Data.DbModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ARF.Web.App_Start.DWF
{
    public class FormFieldModel
    {
        public long Id { get; set; } // Id (Primary key)
        public long? FormId { get; set; } // FormId
        public long? LookupId { get; set; } // LookupId
        public long? SectionId { get; set; } // SectionId
        public string Key { get; set; } // Key
        public string Name { get; set; } // NameArabic
        public string NameEnglish { get; set; } // NameEnglish
        public FormFieldType FieldType { get; set; } // FieldType
        public bool IsRequired { get; set; } // IsRequired
        public bool IsMultipleSelection { get; set; } // IsMultipleSelection
        public bool IsActive { get; set; } // IsActive
        public int? MinValue { get; set; } // MinValue
        public int? MaxValue { get; set; } // MaxValue
        public long? DependsOnFieldId { get; set; } // DependsOnFieldId
        public string DependsOnFieldValue { get; set; } // DependsOnFieldValue
        public string VisibleToRoles { get; set; } // VisibleToRoles
        public string InvisibleToRoles { get; set; } // InvisibleToRoles
        public int Order { get; set; } // Order
        public bool IsReadOnly { get; set; } // IsReadOnly
        public bool IsLTR { get; set; }


        public FormField OriginalField { get; set; }
        public List<SelectListItem> Lookups { get; set; }
    }
}