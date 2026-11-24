using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using PhoneNumbers;

namespace ARF.Services
{
    public class SmsServiceMalath
    {
        public static bool IsLocalhost
        {
            get
            {
                return ConfigurationManager.AppSettings["IsLocalhost"] != null && ConfigurationManager.AppSettings["IsLocalhost"].ToLower().Trim() == "true";
            }
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


            if (IsLocalhost)
            {
                to = new string[] { "966559072407" };
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

            string response = "";
            /*******************************************************************
             This link is used to send message either in arabic or in English
            ********************************************************************/

            string url = "http://www.sms.malath.net.sa/httpSmsProvider.aspx";
            /*******************************************************************
            here are the parameters needed to send sms message
            ********************************************************************/

            string USER_NAME = settings.Username;
            string PASSWRD = settings.Password;
            string SENDER = settings.Sender;
            string RECIEVER = string.Join(",", newTo);
            string MESSAGE = message;
            string UNICOD = "E";

            if (isArabicMessage(message))
            {
                UNICOD = "U"; //this parameter  isfor arabic message
                MESSAGE = ToUnicode(message);
            }

            string query = "username=" + USER_NAME + "&password="__SET_ME__"&mobile=" +
                RECIEVER + "&unicode=" + UNICOD + "&message=" + MESSAGE + "&sender=" + SENDER;

            try
            {
                /*******************************************************************
         here we can make a request for the the required api website by passing parameters to it.
        ********************************************************************/

                HttpWebRequest httpwc = (HttpWebRequest)WebRequest.Create(url);
                httpwc.Method = "POST";
                byte[] data = Encoding.UTF8.GetBytes(query);
                httpwc.ContentType = "application/x-www-form-urlencoded";
                httpwc.ContentLength = data.Length;
                Stream stwriter = httpwc.GetRequestStream();
                stwriter.Write(data, 0, data.Length);
                stwriter.Close();

                WebResponse wr = httpwc.GetResponse();
                stwriter = wr.GetResponseStream();

                StreamReader streader = new StreamReader(stwriter);
                response = streader.ReadLine();
                streader.Close();
                stwriter.Close();

                /*******************************************************************
         This result returned by the response to the request and it takes one of these values
        ********************************************************************/

                switch (response)
                {
                    case "0":
                        error = null;
                        return true;

                    case "101":
                        error = "معاملات خاطئة";
                        break;

                    case "104":
                        error = "قد يكون اسم المستخدم او كلمة الملرور غير صحيح ";
                        break;

                    case "105":
                        error = "بطاقة غير متوفرة";
                        break;

                    case "106":
                        error = "unicode خاطئ";
                        break;

                    case "107":
                        error = "اسم المرسل غير مسموح";
                        break;

                    case "108":
                        error = "اسم المرسل غير موجود";
                        break;

                    default:
                        error = "هناك خطاء في الارسال ";
                        break;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            return false;
        }

        private static bool isArabicMessage(string strText)
        {
            string str = "دجحخهعغفقثصضطكمنتالبيسشظزوةىلارؤءئإلإألأآلآ";

            for (int i = 0; i < strText.Length; i++)

                if (str.IndexOf(strText[i]) != -1)

                    return true;

            return false;
        }

        /*************************************************************
        This function is used to convert arabic message to unicod.
          First  : convert letters to chars (ascii code).
          second : convert chars to hexdecimal numbers .
        **************************************************************/

        private static string ToUnicode(string messg)
        {
            string arabicmessage = string.Empty;

            for (int i = 0; i < messg.Length; i++)
            {
                arabicmessage += ToChar(System.Convert.ToChar(messg.Substring(i, 1)));
            }

            return arabicmessage;
        }

        private static string ToChar(char ch)
        {
            System.Text.UnicodeEncoding class1 = new System.Text.UnicodeEncoding();
            byte[] msg = class1.GetBytes(System.Convert.ToString(ch));

            return ToHexaDecimal(msg[1] + msg[0].ToString("X"));
        }

        private static string ToHexaDecimal(string messg)
        {
            string temp = string.Empty;

            switch (messg.Length)
            {
                case 1: temp = "000" + messg; break;
                case 2: temp = "00" + messg; break;
                case 3: temp = "0" + messg; break;
                case 4: temp = messg; break;
            }

            return temp;
        }
    }
}