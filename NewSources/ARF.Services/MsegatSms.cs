using Newtonsoft.Json;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ARF.Services
{
    public class MsegatSmsResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }

    }
    public class MsegatSms
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
            return SendMessageAsync(settings, message, to, out error);
        }

        public static bool SendMessageAsync(SmsSettings settings, string message, string[] to, out string error)
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
            var MESSAGE = message;
            try
            {
                var postdata = new
                {
                    userName =USER_NAME,
                    numbers = RECIEVER,
                    userSender=SENDER,
                    apiKey=PASSWRD,
                    msg=message
                };
                using (var client = new HttpClient())
                {
                    string json = JsonConvert.SerializeObject(postdata);
                    StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = client.PostAsync("https://www.msegat.com/gw/sendsms.php", data).GetAwaiter().GetResult();
                    string strResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    client.Dispose();
                    var result = JsonConvert.DeserializeObject<MsegatSmsResponse>(strResponse);
                    if (result.Code == "1")
                        return true;
                    error = result.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
