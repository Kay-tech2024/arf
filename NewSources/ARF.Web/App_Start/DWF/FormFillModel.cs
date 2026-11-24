using ARF.Data;
using System.Collections.Generic;

namespace ARF.Web.App_Start.DWF
{
    public class FormFillModel
    {
        public long? RequestId { get; set; }
        public long FormId { get; set; }
        public FormType FormType { get; set; }
        public bool IsLTR { get; set; }
        public long SectionId { get; set; }
        public int Step { get; set; }
        public int StepCount { get; set; }
        public List<string> StepTitles { get; set; }

        public List<FormFieldModel> Fields { get; set; }
        public List<FieldData> FieldData { get; set; }
    }

    public class FieldData
    {
        public string FieldId { get; set; }
        public string FieldValue { get; set; }
        public string Fieldkey { get; set; }
        public List<FieldData> FieldArrayData { get; set; }
        public bool IsValid { get; set; }
    }
}