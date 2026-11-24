using ARF.Core;
using ARF.Data;
using ARF.TimeMachine;
using System;
using System.ComponentModel.DataAnnotations;

namespace ARF.Web.App_Start.Code
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal class StepAttribute : ValidationAttribute
    {
        public int Step { get; set; } = 0;
        public bool IsRequired { get; set; } = false;
        public string RequiredIfOtherField { get; set; } = "";
        public string RequiredIfOtherFieldValue { get; set; } = "";
        public bool RequiredIfClearField { get; set; } = true;
        public bool RequiredIfNot { get; set; } = false;

        public FormFieldType FieldType { get; set; } = FormFieldType.Unspecified;
        public bool IsHijriDate { get; set; } = false;
        public bool MustBeGreaterThanCurrentDate { get; set; }
        public string MustBeGreaterThanOtherProperty { get; set; }
        public bool IsArabicOnly { get; set; }
        public bool IsEnglishOnly { get; set; }
        public double Min { get; set; } = -1;
        public double Max { get; set; } = -1;
        public string OnlyIfPropertyIsNotNull { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.MemberName == "DepartmentID")
            {
            }

            var validate = true;
            string lang = validationContext.ObjectInstance.GetPropValue("Type") != null && validationContext.ObjectInstance.GetPropValue("Type").ToString() == ReqFieldType.AppliedScienceField.ToString() ? "en" : null;

            if (validationContext.ObjectInstance.HasProperty("Step"))
            {
                var obj = validationContext.ObjectInstance.GetPropValue("Step");
                if (obj != null)
                {
                    int step = 0;
                    int.TryParse(obj.ToString(), out step);

                    if (step != Step)
                    {
                        validate = false;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(RequiredIfOtherField) && validationContext.ObjectInstance.HasProperty(RequiredIfOtherField))
            {
                var oval = validationContext.ObjectInstance.GetPropValue(RequiredIfOtherField);

                if (oval != null && !string.IsNullOrWhiteSpace(oval.ToString()))
                {
                    if (string.IsNullOrWhiteSpace(RequiredIfOtherFieldValue) || ((!RequiredIfNot && RequiredIfOtherFieldValue.ToLower() != oval.ToString().ToLower()) || (RequiredIfNot && RequiredIfOtherFieldValue.ToLower() == oval.ToString().ToLower())))
                    {
                        if (RequiredIfClearField)
                        {
                            validationContext.ObjectInstance.SetPropValue(validationContext.MemberName, null);
                        }
                    }
                }
            }

            if (validate)
            {
                var err = (string)null;
                var isValid = true;

                if (IsRequired && (value == null || string.IsNullOrWhiteSpace(value.ToString()) || (value.GetType().BaseType.FullName == "System.Enum" && value.ToString() == "Unspecified")))
                {
                    isValid = false;

                    if (OnlyIfPropertyIsNotNull != null)
                    {
                        var v = validationContext.ObjectInstance.GetPropValue(OnlyIfPropertyIsNotNull);
                        if (v == null)
                        {
                            isValid = true;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(RequiredIfOtherField) && validationContext.ObjectInstance.HasProperty(RequiredIfOtherField))
                    {
                        var oval = validationContext.ObjectInstance.GetPropValue(RequiredIfOtherField);

                        if ((RequiredIfOtherFieldValue != null && oval != null && !string.IsNullOrWhiteSpace(oval.ToString())))
                        {
                            if (string.IsNullOrWhiteSpace(RequiredIfOtherFieldValue) || ((!RequiredIfNot && RequiredIfOtherFieldValue.ToLower() != oval.ToString().ToLower()) || (RequiredIfNot && RequiredIfOtherFieldValue.ToLower() == oval.ToString().ToLower())))
                            {
                                isValid = true;

                            }
                        }
                        //else if ((!RequiredIfNot && (RequiredIfOtherFieldValue != null && (oval == null || string.IsNullOrWhiteSpace(oval.ToString())))) || (RequiredIfNot && !(RequiredIfOtherFieldValue != null && (oval == null || string.IsNullOrWhiteSpace(oval.ToString())))))
                        //{
                        //    isValid = true;
                        //}
                    }

                    if (!isValid)
                        err = LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang);
                }
                else if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    switch (FieldType)
                    {
                        case FormFieldType.IBAN:
                            isValid = Utils.ValidateIBAN(value.ToString());
                            if (!isValid)
                            {
                                err = LangHelpers.Instance.GetLangValueByString("invalid_iban", false, lang);
                            }
                            break;

                        case FormFieldType.Name:
                            {
                                var nameComps = Utils.GetNameComponents(value.ToString());
                                if (nameComps == null || nameComps.FirstName == null || nameComps.LastName == null)
                                {
                                    isValid = false;
                                    err = LangHelpers.Instance.GetLangValueByString("fullname_required", false, lang);
                                }
                            }
                            break;

                        case FormFieldType.Email:
                            isValid = Utils.ValidateEmail(value.ToString());
                            if (!isValid)
                            {
                                err = LangHelpers.Instance.GetLangValueByString("invalid_email", false, lang);
                            }
                            break;

                        case FormFieldType.Phone:
                            isValid = Utils.ValidateMobile(value.ToString());
                            if (!isValid)
                            {
                                err = LangHelpers.Instance.GetLangValueByString("invalid_mobile_number", false, lang);
                            }
                            break;

                        case FormFieldType.Integer:
                            {
                                var num = (int)0;
                                if (!int.TryParse(value.ToString(), out num))
                                {
                                    err = LangHelpers.Instance.GetLangValueByString("invalid_integer", false, lang);
                                }
                                else if ((Min >= 0 && num < Min) && !(Max >= 0 && num > Max))
                                {
                                    err = LangHelpers.Instance.GetLangValueByString("integer_greater_than", false, lang).Replace("{min}", Min + "").Replace("{max}", Max + "");
                                }
                                else if (!(Min >= 0 && num < Min) || (Max >= 0 && num > Max))
                                {
                                    err = LangHelpers.Instance.GetLangValueByString("integer_less_than", false, lang).Replace("{max}", Max + "");
                                }
                                else if ((Min >= 0 && num < Min) || (Max >= 0 && num > Max))
                                {
                                    err = LangHelpers.Instance.GetLangValueByString("integer_between_min_max", false, lang).Replace("{min}", Min + "").Replace("{max}", Max + "");
                                }

                                break;
                            }
                        case FormFieldType.Monetary:
                            {
                                var num = (float)0;
                                if (!float.TryParse(value.ToString(), out num))
                                {
                                    err = LangHelpers.Instance.GetLangValueByString("invalid_money", false, lang);
                                }
                                else if ((Min >= 0 && num < Min) && !(Max >= 0 && num > Max))
                                {
                                    err = LangHelpers.Instance.GetLangValueByString("money_value_greater_than", false, lang).Replace("{min}", Min + "").Replace("{max}", Max + "");
                                }
                                else if (!(Min >= 0 && num < Min) || (Max >= 0 && num > Max))
                                {
                                    err = LangHelpers.Instance.GetLangValueByString("money_value_less_than", false, lang).Replace("{max}", Max + "");
                                }
                                else if ((Min >= 0 && num < Min) || (Max >= 0 && num > Max))
                                {
                                    err = LangHelpers.Instance.GetLangValueByString("money_value_between_min_max", false, lang).Replace("{min}", Min + "").Replace("{max}", Max + "");
                                }

                                break;
                            }
                        case FormFieldType.Date:
                            {
                                var date = (DateTime?)null;
                                var dateComps = Utils.ParseDate(value.ToString(), IsHijriDate ? DateType.Hijri : DateType.Unspecified, out date);

                                if (dateComps == null)
                                {
                                    if (IsHijriDate)
                                        err = LangHelpers.Instance.GetLangValueByString("invalid_hijri_date", false, lang);
                                    else
                                        err = LangHelpers.Instance.GetLangValueByString("invalid_date", false, lang);
                                }
                                else
                                {
                                    if (MustBeGreaterThanCurrentDate && date < TM.Now.Date)
                                    {
                                        err = LangHelpers.Instance.GetLangValueByString("date_greater_than_current_date", false, lang);
                                    }
                                    else if (MustBeGreaterThanOtherProperty != null)
                                    {
                                        var otherProperty = validationContext.ObjectInstance.GetPropValue(MustBeGreaterThanOtherProperty);
                                        if (otherProperty != null)
                                        {
                                            var date2 = (DateTime?)null;
                                            var dateComps2 = Utils.ParseDate(otherProperty.ToString(), IsHijriDate ? DateType.Hijri : DateType.Unspecified, out date2);

                                            if (date2 != null && date2 >= date)
                                            {
                                                isValid = false;
                                                err = LangHelpers.Instance.GetLangValueByString("date_greater_than", false, lang).Replace("{min}", date2.Value.FormatHijriDate(false));
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        default:

                            if (IsArabicOnly)
                            {
                                var isAr = Utils.IsArabicString(value.ToString());

                                if (!isAr)
                                {
                                    isValid = false;
                                    err = LangHelpers.Instance.GetLangValueByString("field_must_be_arabic_only", false, lang);
                                }
                            }
                            else if (IsEnglishOnly)
                            {
                                var isEn = Utils.IsEnglishString(value.ToString());

                                if (!isEn)
                                {
                                    isValid = false;
                                    err = LangHelpers.Instance.GetLangValueByString("field_must_be_english_only", false, lang);
                                }
                            }

                            if (isValid)
                            {
                                var num = value.ToString().Length;
                                if ((Min >= 0 && num < Min) && !(Max >= 0 && num > Max))
                                {
                                    err = LangHelpers.Instance.GetLangValueByString("string_greater_than", false, lang).Replace("{min}", Min + "").Replace("{max}", Max + "");
                                }
                                else if (!(Min >= 0 && num < Min) && (Max >= 0 && num > Max))
                                {
                                    err = LangHelpers.Instance.GetLangValueByString("string_less_than", false, lang).Replace("{max}", Max + "");
                                }
                                else if ((Min >= 0 && num < Min) && (Max >= 0 && num > Max))
                                {
                                    err = LangHelpers.Instance.GetLangValueByString("string_between_min_max", false, lang).Replace("{min}", Min + "").Replace("{max}", Max + "");
                                }
                            }

                            break;
                    }
                }

                if (isValid)
                {
                }
                else
                {
                    if (err != null)
                    {
                        return new ValidationResult(string.Format(err, LangHelpers.Instance.GetLangValueByString(validationContext.MemberName, false, lang)));
                    }

                    return new ValidationResult(string.Format(LangHelpers.Instance.GetLangValueByString("field_is_invalid", false, lang), LangHelpers.Instance.GetLangValueByString(validationContext.MemberName, false, lang)));
                }
            }

            return ValidationResult.Success;
        }
    }
}