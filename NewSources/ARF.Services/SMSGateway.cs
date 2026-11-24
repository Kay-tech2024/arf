using Newtonsoft.Json;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ARF.Services
{
    public class MessagePart
    {
        public string MessageId { get; set; }
        public int MessagePartId { get; set; }
        public string MessageText { get; set; }
    }

    public class MessageData
    {
        public string MobileNumber { get; set; }
        public List<MessagePart> MessageParts { get; set; }
    }

    public class Response
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string JobId { get; set; }
        public List<MessageData> MessageData { get; set; }
    }
    public class SMSGateway
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
                to = new string[] { "0595487539", "0569003570" };
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
            //var MESSAGE = message;
            var MESSAGE = ConvertToUnicode(message);
            try
            {                
                HttpWebRequest req = (HttpWebRequest)
                WebRequest.Create("https://apps.gateway.sa/vendorsms/pushsms.aspx");
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                string postData = "user=" + USER_NAME + "&password="__SET_ME__"&msisdn=" + RECIEVER + "&sid=" + SENDER + "&msg=" + MESSAGE + "&fl=0&dc=8";
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
                var result = JsonConvert.DeserializeObject<Response>(strResponse);
                if (result.ErrorCode == "000")
                    return true;

                error = result.ErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
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

    }
}
