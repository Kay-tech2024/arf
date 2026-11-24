using ARF.Core;
using ARF.TimeMachine;
using System;
using System.IO;

namespace ARF.Daemon
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string path = @"C:\ARF_Daemon\Logs\ARF_Daemon_Info.log";
                File.AppendAllText(path, Environment.NewLine + "*** Running on: " + TM.Now.ToString());

                (new RefereeUtil()).CheckRefereeFundingReviewRequestExpiryAndSendNotificationToNextReferee();

                (new RequestUtil()).UpdateCoResearcherResponseFoReviewRequest();
            }
            catch (Exception ex)
            {
                string _exception = "******************** /n/n Message: " + ex.Message + "\n InnerExceptionMessae: "+ ex.InnerException.Message + " \n StackTrack: " + ex.StackTrace + "\n\n\n\n";
                Console.WriteLine(_exception);
                string path = @"C:\ARF_Daemon\Logs\ARF_Daemon_Error.log";
                File.AppendAllText(path, Environment.NewLine + _exception);
            }
        }
    }
}
