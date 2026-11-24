using ARF.Core;
using ARF.Web.Resources;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace ARF.Web.App_Start.Code
{
    public class LangHelpers : Translator
    {
        private static Translator _instance;

        private LangHelpers()
        {
        }

        public override string GetLangValueByString(string str, bool returnNullIfNotFound = false, string lang = null)
        {

            if (str == null) return null;
            str = Utils.PascalCaseToUnderscore(str);

            if (str == "0_resubmit") str = "_resubmit";

            if (str == "0")
            {
                str = "not_submitted";
            }

            var key = str;

            if (str == null) return null;
            str = Utils.PascalCaseToUnderscore(str);


            if (lang == null)
            {
                if (!Utils.IsUserLanguageArabic)
                    lang = "EN";
                else
                    lang = "AR";
            }
            else
            {
                lang = lang.Trim().ToUpper();
            }


            CultureInfo Culture = new CultureInfo(lang != "EN" ? "ar-SA" : "en-US");

            var oldCulture = Thread.CurrentThread.CurrentUICulture.Name;

            Thread.CurrentThread.CurrentCulture = Culture;
            Thread.CurrentThread.CurrentUICulture = Culture;

            var t = typeof(ArfStrings);
            var fields = t.GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(fi => fi.Name == str).ToList();

            var tstr = str;
            if (fields.Count > 0) tstr = fields[0].GetValue(null).ToString();
            else if (returnNullIfNotFound) tstr = null;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(oldCulture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(oldCulture);

            if (Utils.ShowTerms)
            {
                if (!key.Contains("css") && key != "lang_code")
                    return tstr + " (" + key + ")";
            }


            return tstr;
        }

        public static string GetKey(string str, string lang)
        {
            if (str == null) return null;
            if (lang != null)
                lang = lang.Trim().ToUpper();

            if (lang != "EN") lang = "AR";

            str = Utils.PascalCaseToUnderscore(str);

            if (str == "0_resubmit") str = "_resubmit";

            CultureInfo Culture = new CultureInfo(lang != "EN" ? "ar-SA" : "en-US");

            var oldCulture = Thread.CurrentThread.CurrentUICulture.Name;

            var key = (string)null;
            Thread.CurrentThread.CurrentCulture = Culture;
            Thread.CurrentThread.CurrentUICulture = Culture;

            var t = typeof(ArfStrings);
            var fields = t.GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(fi => fi.Name == str).ToList();

            if (fields.Count > 0) key = fields[0].Name;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(oldCulture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(oldCulture);

            return key;
        }

        public static string GetKeyByString(string str, string lang)
        {
            if (str == null) return null;
            if (lang != null)
                lang = lang.Trim().ToUpper();

            if (lang != "EN") lang = "AR";

            str = str.Trim();

            if (str == "0_resubmit") str = "_resubmit";

            CultureInfo Culture = new CultureInfo(lang != "EN" ? "ar-SA" : "en-US");

            var oldCulture = Thread.CurrentThread.CurrentUICulture.Name;

            var key = (string)null;
            Thread.CurrentThread.CurrentCulture = Culture;
            Thread.CurrentThread.CurrentUICulture = Culture;

            var t = typeof(ArfStrings);
            var fields = t.GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(fi => fi.GetValue(null) != null && fi.GetValue(null).ToString() == str).ToList();

            if (fields.Count > 0) key = fields[0].Name;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(oldCulture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(oldCulture);

            return key;
        }

        public string GetRequestHistoryStatus(string str)
        {
            return "";
        }

        public static Translator Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new LangHelpers();
                return Instance;
            }
        }
    }
}