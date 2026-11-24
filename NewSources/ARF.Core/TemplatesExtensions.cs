using ARF.Data;
using ARF.Data.DbModels;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ARF.Core
{
    public static class TemplatesExtensions
    {
        private static Regex rx_variables = new Regex(@"(\{[\w_\-0-9]+\})", RegexOptions.Compiled);

        public static bool IsNotifiable(this Request request, User user)
        {
            var cond1 = request.CreatedBy == user.Id;
            //var cond2 = request.noti

            return true;
        }

        public static List<string> ExtractTemplateVariables(this string str)
        {
            var results = new List<string>();

            foreach (var match in rx_variables.Matches(str))
            {
                if (results.Contains(((Match)match).Value)) continue;
                results.Add(((Match)match).Value);
            }

            return results;
        }

        public static string UpdateUserTemplateVariables(this string str, User user)
        {
            var variables = ExtractTemplateVariables(str);

            foreach (var variable in variables)
            {
                str = str.UpdateUserTemplateVariable(user, variable);
            }

            return str;
        }

        public static string UpdateUserTemplateVariable(this string str, User user, string variable)
        {
            if (!string.IsNullOrWhiteSpace(variable))
            {
                switch (variable.Trim().ToUpper())
                {
                    case "{NAME}":
                        if (user.UserProfile != null)
                            str = str.Replace(variable, user.UserProfile.GetFullName());
                        else
                            str = str.Replace(variable, user.UserName);
                        break;
                    case "{USERNAME}":
                    case "{USER_NAME}":
                        str = str.Replace(variable, user.UserName);
                        break;

                    case "{EMAIL}":
                        str = str.Replace(variable, user.Email);
                        break;
                }
            }

            return str;
        }

        public static string UpdateRequestTemplateVariables(this string str, Request request, LanguagePreference lang = LanguagePreference.English)
        {
            var variables = ExtractTemplateVariables(str);

            foreach (var variable in variables)
            {
                str = str.UpdateRequestTemplateVariable(request, variable, lang);
            }

            return str;
        }

        public static string UpdateRequestTemplateVariable(this string str, Request request, string variable, LanguagePreference lang = LanguagePreference.English)
        {
            if (!string.IsNullOrWhiteSpace(variable))
            {
                switch (variable.Trim().ToUpper())
                {
                    case "{TYPE}":
                        str = str.Replace(variable, ((Data.RequestType)request.Type).ToString());
                        break;



                    case "{SUBMITTED_BY}":
                    case "{SUBMITTEDBY}":
                        if (request.CreatedByUser != null && request.CreatedByUser.UserProfile != null)
                        {
                            str = str.Replace(variable, request.CreatedByUser.UserProfile.GetFullName() + " (" + request.CreatedByUser.UserName + ")");
                        }
                        else
                            str = str.Replace(variable, "");

                        break;



                    case "{COMMENTS}":
                        str = str.Replace(variable, request.Comments);
                        break;

                    case "{URL}":
                        str = str.Replace(variable, Utils.GetApplicationUrl() + "Requests/Edit/" + request.Id);
                        break;


                    case "{SUBMISSIONDATE}":
                    case "{SUBMISSION_DATE}":
                        str = str.Replace(variable, request.Created.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.GetCultureInfo("en-US")));
                        break;


                }
            }

            return str;
        }
    }
}