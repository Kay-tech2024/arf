using System.ComponentModel.DataAnnotations;

namespace ARF.Services
{
    public enum SmsProvider : byte
    {
        Unspecificed = 0, MalathSMS = 1, MobilyWS = 2,SmsGateway=3,Msegat=4
    };

    public class SmsSettings
    {
        public SmsProvider Type { get; set; }

        public string Sender { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}