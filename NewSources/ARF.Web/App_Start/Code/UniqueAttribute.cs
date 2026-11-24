using ARF.Core;
using ARF.Core.Managers;
using ARF.Web.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ARF.Web.App_Start.Code
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    internal class UniqueAttribute : ValidationAttribute
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(validationContext.DisplayName)) return ValidationResult.Success;

            string str = (string)value;
            if (String.IsNullOrWhiteSpace(str))
            {
                return ValidationResult.Success;
            }

            string andClause = "";

            if (validationContext.ObjectInstance.HasProperty("Id"))
            {
                var obj = validationContext.ObjectInstance.GetPropValue("Id");
                if (obj != null)
                    andClause = "AND Id != '" + obj.ToString() + "'";
            }
            else if (validationContext.ObjectInstance.HasProperty("UserId"))
            {
                var obj = validationContext.ObjectInstance.GetPropValue("UserId");
                if (obj != null)
                    andClause = "AND Id != '" + obj.ToString() + "'";
            }

            if (string.IsNullOrWhiteSpace(SchemaName)) SchemaName = "dbo";
            if (string.IsNullOrWhiteSpace(TableName)) TableName = "users";

            var count = CommonManager.GetDatabaseContext().Database.SqlQuery<int>(string.Format("SELECT count(*) from {0}.{1} where {2} = @p0 {3}", SchemaName, TableName, validationContext.MemberName, andClause), value).First();
            if (count == 0)
                return ValidationResult.Success;

            return new ValidationResult(validationContext.DisplayName + " " + LangHelpers.Instance["must_be_unique"]);
        }
    }
}