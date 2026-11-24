using ARF.Services;
using System.Collections.Generic;
using System.Configuration;

namespace ARF.Core
{
    public class SmsService
    {
        public static bool SendMessage(string message, params string[] to)
        {
            string error = null;
            return SendMessage(message, to, out error);
        }

        public static bool SendMessage(string message, string[] to, out string error)
        {
            error = null;

            if (ConfigurationManager.AppSettings["DontSendSMS"] == "true")
                return true;

            var provider = Utils.PreferredSmsProvider;
            if(provider==SmsProvider.MalathSMS)
            {
                var settings = Utils.MalathSmsSettings;
                return SmsServiceMalath.SendMessage(settings, message, to, out error);
            }
            else if (provider == SmsProvider.Msegat)
            {
                var settings = Utils.MsegatSmsSettings;
                return MsegatSms.SendMessageAsync(settings, message, to, out error);

            }
            else if(provider==SmsProvider.MobilyWS)
            {
                var settings = Utils.MalathSmsSettings;
                return SmsServiceMobily.SendMessage(settings, message, to, out error);

            }
            else
            {
                var settings = Utils.SmsSettings;
                return SMSGateway.SendMessage(settings, message, to, out error);
            }            
        }
    }
}