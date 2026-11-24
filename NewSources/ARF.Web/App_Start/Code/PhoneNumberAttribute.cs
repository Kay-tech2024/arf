using PhoneNumbers;
using System.ComponentModel.DataAnnotations;

namespace ARF.Web.App_Start.Code
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var valueString = value as string;
            if (string.IsNullOrEmpty(valueString))
            {
                return true;
            }

            var util = PhoneNumberUtil.GetInstance();
            try
            {
                var number = util.Parse(valueString, "SA");
                return util.IsValidNumber(number);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}