using System;
using System.ComponentModel.DataAnnotations;

namespace ARF.Web.App_Start.Code
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    internal class BeforeDateAttribute : ValidationAttribute
    {
        public string OtherDateProperty { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(OtherDateProperty)) return ValidationResult.Success;

            var date = (DateTime?)value;
            if (date == null)
            {
                return ValidationResult.Success;
            }

            var otherDate = (DateTime?)validationContext.ObjectInstance.GetType().GetProperty(OtherDateProperty).GetValue(validationContext.ObjectInstance, null);

            if (otherDate == null) return ValidationResult.Success;

            if (date.Value <= otherDate.Value) return ValidationResult.Success;

            return new ValidationResult(string.Format("{0} must be before {1}", validationContext.DisplayName, OtherDateProperty));
        }
    }
}