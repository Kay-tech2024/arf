using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;

namespace ARF.Services
{
    public class SmsServiceMobily
    {
        public static bool IsLocalhost
        {
            get
            {
                return ConfigurationManager.AppSettings["IsLocalhost"] != null && ConfigurationManager.AppSettings["IsLocalhost"].ToLower().Trim() == "true";
            }
        }

        private string GetBalance(string username, string password)
        {
            HttpWebRequest req = (HttpWebRequest)
            WebRequest.Create("http://www.mobily.ws/api/balance.php");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            string postData = "mobile=" + username + "&password=" + password;
            req.ContentLength = postData.Length;

            StreamWriter stOut = new
            StreamWriter(req.GetRequestStream(),
            System.Text.Encoding.ASCII);
            stOut.Write(postData);
            stOut.Close();
            // Do the request to get the response
            string strResponse;
            StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
            strResponse = stIn.ReadToEnd();
            stIn.Close();
            switch (strResponse)
            {
                case "1": return "إن إسم المستخدم الذي إستخدمته للدخول غير صحيح (تأكد من أن إسم المستخدم الذي إستخدمته هو نفسه الذي تدخله عند دخولك إلى موقع موبايلي).";
                case "2": return "هناك خطأ في كلمة المرور (تأكد من أن كلمة المرور التي تستخدمها هي نفسها التي تستخدمها عند دخولك موقع موبايلي).";
                case "-1": return "لم يتم التواصل مع خادم (Server) الإرسال موبايلي بنجاح. (قد يكون هناك محاولات إرسال كثيرة تمت معا , أو قد يكون هناك عطل مؤقت طرأ على الخادم إذا إستمرت المشكلة يرجى التواصل مع الدعم الفني)";
                case "-2": return "لم يتم الربط مع قاعدة البيانات (Database) التي تحتوي على حسابك وبياناتك لدى موبايلي. (قد يكون هناك محاولات إرسال كثيرة تمت معا , أو قد يكون هناك عطل مؤقت طرأ على الخادم إذا إستمرت المشكلة يرجى التواصل مع الدعم الفني)";
            }

            return strResponse;
        }

        public static bool SendMessage(SmsSettings settings, string message, params string[] to)
        {
            string error = null;
            return SendMessage(settings, message, to, out error);
        }

        public static bool SendMessage(SmsSettings settings, string message, string[] to, out string error)
        {
            error = null;

            if (ConfigurationManager.AppSettings["DontSendSMS"] == "true")
                return true;
            //,"966540864470"
            if (IsLocalhost)
            {
                to = new string[] {  "966595657226", "966540864470","0569003570" };
            }

            if (to == null || to.Length == 0)
            {
                error = "NO_RECEIVERS";
                return false;
            }

            var newTo = new List<string>();

            var util = PhoneNumberUtil.GetInstance();

            foreach (var mobileNumber in to)
            {
                try
                {
                    var number = util.Parse(mobileNumber, "SA");
                    newTo.Add(number.CountryCode + "" + number.NationalNumber);
                }
                catch (NumberParseException)
                {
                }
            }

            if (newTo == null || newTo.Count == 0)
            {
                error = "NO_VALID_RECEIVERS";
                return false;
            }

            /*******************************************************************
            here are the parameters needed to send sms message
            ********************************************************************/

            string USER_NAME = settings.Username;
            string PASSWRD = settings.Password;
            string SENDER = settings.Sender;
            string RECIEVER = string.Join(",", newTo);
            var MESSAGE = ConvertToUnicode(message);

            try
            {
                HttpWebRequest req = (HttpWebRequest)
                WebRequest.Create("http://www.mobily.ws/api/msgSend.php");
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                string postData = "mobile=" + USER_NAME + "&password="__SET_ME__"&numbers=" + RECIEVER + "&sender=" + SENDER + "&msg=" + MESSAGE + "&applicationType=61";
                req.ContentLength = postData.Length;

                StreamWriter stOut = new
                StreamWriter(req.GetRequestStream(),
                System.Text.Encoding.ASCII);
                stOut.Write(postData);
                stOut.Close();
                // Do the request to get the response
                string strResponse;
                StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
                strResponse = stIn.ReadToEnd();
                stIn.Close();

                if (strResponse == "1")
                    return true;

                error = SanitizeResult(strResponse);
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static bool isArabicMessage(string strText)
        {
            string str = "دجحخهعغفقثصضطكمنتالبيسشظزوةىلارؤءئإلإألأآلآ";

            for (int i = 0; i < strText.Length; i++)

                if (str.IndexOf(strText[i]) != -1)

                    return true;

            return false;
        }

        private static string ConvertToUnicode(string val)
        {
            string msg2 = string.Empty;

            for (int i = 0; i < val.Length; i++)
            {
                msg2 += convertToUnicode(System.Convert.ToChar(val.Substring(i, 1)));
            }

            return msg2;
        }

        private static string convertToUnicode(char ch)
        {
            System.Text.UnicodeEncoding class1 = new System.Text.UnicodeEncoding();
            byte[] msg = class1.GetBytes(System.Convert.ToString(ch));

            return fourDigits(msg[1] + msg[0].ToString("X"));
        }

        private static string fourDigits(string val)
        {
            string result = string.Empty;

            switch (val.Length)
            {
                case 1: result = "000" + val; break;
                case 2: result = "00" + val; break;
                case 3: result = "0" + val; break;
                case 4: result = val; break;
            }

            return result;
        }

        private static string SanitizeResult(string res)
        {
            switch (res)
            {
                case "1": return "لقد تمت العملية بنجاح";
                case "2": return "إن رصيدك لدى موبايلي قد إنتهى ولم يعد به أي رسائل. (لحل المشكلة قم بشحن رصيدك من الرسائل لدى موبايلي. لشحن رصيدك إتبع تعليمات شحن الرصيد)";
                case "3": return "إن رصيدك الحالي لا يكفي لإتمام عملية الإرسال. (لحل المشكلة قم بشحن رصيدك من الرسائل لدى موبايلي. لشحن رصيدك إتبع تعليمات شحن الرصيد";
                case "4": return "إن إسم المستخدم الذي إستخدمته للدخول إلى حساب الرسائل غير صحيح (تأكد من أن إسم المستخدم الذي إستخدمته هو نفسه الذي تستخدمه عند دخولك إلى موقع موبايلي).";
                case "5": return "هناك خطأ في كلمة المرور (تأكد من أن كلمة المرور التي تم إستخدامها هي نفسها التي تستخدمها عند دخولك موقع موبايلي,إذا نسيت كلمة المرور إضغط على رابط نسيت كلمة المرور لتصلك رسالة على جوالك برقم المرور الخاص بك)";
                case "6": return "إن صفحة الإرسال لاتجيب في الوقت الحالي (قد يكون هناك طلب كبير على الصفحة أو توقف مؤقت للصفحة فقط حاول مرة أخرى أو تواصل مع الدعم الفني إذا إستمر الخطأ)";
                case "12": return "إن حسابك بحاجة إلى تحديث يرجى مراجعة الدعم الفني";
                case "13": return "إن إسم المرسل الذي إستخدمته في هذه الرسالة لم يتم قبوله. (يرجى إرسال الرسالة بإسم مرسل آخر أو تعريف إسم المرسل لدى موبايلي)";
                case "14": return "إن إسم المرسل الذي إستخدمته غير معرف لدى موبايلي. (يمكنك تعريف إسم المرسل من خلال صفحة إضافة إسم مرسل)";
                case "15": return "يوجد رقم جوال خاطئ في الأرقام التي قمت بالإرسال لها. (تأكد من صحة الأرقام التي تريد الإرسال لها وأنها بالصيغة الدولية)";
                case "16": return "الرسالة التي قمت بإرسالها لا تحتوي على إسم مرسل. (أدخل إسم مرسل عند إرسالك الرسالة)";
                case "17": return "لم يتم إرسال نص الرسالة. الرجاء التأكد من إرسال نص الرسالة والتأكد من تحويل الرسالة الى يوني كود (الرجاء التأكد من استخدام الدالة()";
                case "18": return "الإرسال متوقف حاليا";
                case "19": return "applicationType غير موجودة في الرابط";
                case "-1": return "لم يتم التواصل مع خادم (Server) الإرسال موبايلي بنجاح. (قد يكون هناك محاولات إرسال كثيرة تمت معا , أو قد يكون هناك عطل مؤقت طرأ على الخادم إذا إستمرت المشكلة يرجى التواصل مع الدعم الفني)";
                case "-2": return "لم يتم الربط مع قاعدة البيانات (Database) التي تحتوي على حسابك وبياناتك لدى موبايلي. (قد يكون هناك محاولات إرسال كثيرة تمت معا , أو قد يكون هناك عطل مؤقت طرأ على الخادم إذا إستمرت المشكلة يرجى التواصل مع الدعم الفني)";
                default: return res;
            }
        }
    }
}