using ARF.Data;
using System;

namespace ARF.Core.Models
{
    public class FieldModel
    {
        public string Name { get; set; }
        public FieldType Type { get; set; }
        public uint? Minimum { get; set; }
        public uint? Maximum { get; set; }
        public bool IsRequired { get; set; }
        public bool IsEditable { get; set; }
        public bool IsFromProfile { get; set; }

        public FieldModel[] Subfields { get; set; }

        public FieldModel(string name, FieldType type, bool isRequired = false, uint? min = null, uint? max = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception("Field name cannot be empty");
            if (type == FieldType.Unspecified) throw new Exception("Invalid field type");
            if (type == FieldType.Array) throw new Exception("This constuctor cannot be used for Array fields");

            this.Name = name;
            this.Type = type;
            this.Minimum = min;
            this.Maximum = max;
        }

        public FieldModel(FieldType type, bool isRequired, uint? min, uint? max, params FieldModel[] subfields)
        {
            if (type != FieldType.Array) throw new Exception("This constuctor can only be used for Array fields");
            if (subfields == null || subfields.Length == 0) throw new Exception("Sub-fields cannot be empty");

            this.Type = type;
            this.Minimum = min;
            this.Maximum = max;

            this.Subfields = subfields;
        }
    }
}