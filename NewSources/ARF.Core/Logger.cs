using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ARF.Core
{
    public class Logger
    {
        private static Regex rx_alphanumeric = new Regex("[^a-zA-Z0-9 -]", RegexOptions.Compiled);

        public static void FakeLogExceptionAsync(Exception ex, object obj = null, bool sendImmediately = false)
        {
        }

        public static void LogExceptionAsync(Exception ex, object obj = null, bool sendImmediately = false, bool writeOnly = false)
        {

            try
            {
                var objStr = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new MyContractResolver()
                });

                var headerStr = "";

                if (HttpContextManager.Current != null && HttpContextManager.Current.Request != null)
                {
                    headerStr += "<br/>&nbsp;&nbsp;&nbsp;<b>url</b>:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + Utils.GetFullUrl();
                    headerStr += "<br/>&nbsp;&nbsp;&nbsp;<b>Application URL</b>:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + Utils.GetApplicationUrl();
                    headerStr += "<br/>&nbsp;&nbsp;&nbsp;<b>Method</b>:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + HttpContextManager.Current.Request.HttpMethod;
                    headerStr += "<br/>&nbsp;&nbsp;&nbsp;<b>referrer</b>:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + Utils.GetReferrerUrl();
                    headerStr += "<br/>&nbsp;&nbsp;&nbsp;<b>username</b>:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + Utils.GetUsername();
                    headerStr += "<br/>&nbsp;&nbsp;&nbsp;<b>ip_address</b>:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + Utils.GetUserIP();
                    headerStr += "<br/>&nbsp;&nbsp;&nbsp;<b>browser</b>:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + HttpContextManager.Current.Request.UserAgent;
                }
                if (Utils.IsLocalhost)
                {
                    Logger.LogException(ex, objStr, headerStr, true);
                }
                else
                {
                    Hangfire.BackgroundJob.Enqueue(() => Logger.LogException(ex, objStr, headerStr, sendImmediately, writeOnly));
                }


            }
            catch
            {
            }
        }

        public static void LogException(Exception ex, string objStr = null, string headerStr = null, bool sendImmediately = false, bool writeOnly = false)
        {
            if (ex.Message.Contains("__browserLink")) return;
            try
            {
                var Request = HttpContextManager.Current == null ? null : HttpContextManager.Current.Request;

                var str = ex.Message;

                if (objStr != null)
                {
                    str += "<br/>&nbsp;&nbsp;&nbsp;data:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + objStr;
                }

                if (headerStr != null)
                {
                    str += headerStr;
                }

                str += "<br/>&nbsp;&nbsp;&nbsp;<b>exception</b>:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ex.ToString().Replace("<br/>", "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                str += "<br/>&nbsp;&nbsp;&nbsp;<b>exception_more</b>:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + Utils.GetSpecificExceptionData(ex).Replace("\r\n", "<br/>").Replace("<br/>", "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;").Replace(" at ", " at<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");

                var key = ex.Message.Replace("\r\n", "<br/>");
                key = rx_alphanumeric.Replace(key, "");

                if (!writeOnly)
                {


                    //#if !DEBUG
                    if (!sendImmediately)
                        Hangfire.BackgroundJob.Enqueue(() => SendException(str, true));
                    else
                        SendException(str);
                }
                //#else
                LogWriter.Instance.WriteToLog("exception", str);
                //#endif
            }
            catch (Exception ex2)
            {
                Logger.FakeLogExceptionAsync(ex2);
            }
        }

        public static void SendException(string msg, bool useMailGun = true)
        {
            if (msg.Contains("Thread was being aborted")) return;

            string subject = "Subject";
            useMailGun = false;
            string body = msg;

#if DEBUG
            subject = "(DEBUG) ARF Error - " + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.GetCultureInfo("en-US"));
#else
            subject = "ARF Error - " + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.GetCultureInfo("en-US"));
#endif

            if (!Utils.IsLocalhost)
            {
                try
                {
                    EmailService.SendMessage(Utils.GetSmtpSettings(), subject, body, 
                        new MailAddress("omar.shaaban@kay-ict.com", "Omar Shaaban"), 
                        new MailAddress[] {
                            new MailAddress("a.abujalila@kay-ict.com", "Abdulrahman Abu Jalila"),
                            new MailAddress("awadalamri@kay-ict.com", "Awad Alamri"),
                        }, 
                    null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public class LogWriter
        {
            private static LogWriter instance;
            private static string logDir = ConfigurationManager.AppSettings["LogPath"] + "\\";
            private static DateTime LastFlushed = DateTime.Now;

            /// <summary>
            /// Private constructor to prevent instance creation
            /// </summary>
            private LogWriter() { }

            /// <summary>
            /// An LogWriter instance that exposes a single instance
            /// </summary>
            public static LogWriter Instance
            {
                get
                {
                    // If the instance is null then create one and init the Queue
                    if (instance == null)
                    {
                        instance = new LogWriter();
                    }
                    return instance;
                }
            }

            /// <summary>
            /// The single instance method that writes to the log file
            /// </summary>
            /// <param name="message">The message to write to the log</param>
            public void WriteToLog(string key, string message)
            {
                var time = DateTime.Now;
                logDir = "C:\\ARF_Logs\\";
                // Lock the queue while writing to prevent contention for the log file
                lock (instance)
                {
                    // Create the entry and push to the Queue
                    Log entry = new Log(message);

                    var dir = logDir + time.ToString("yyyy-MM-dd", CultureInfo.GetCultureInfo("en-US"));
                    var logPath = dir + "\\" + key + ".txt";

                    if (!Directory.Exists(logDir))
                        Directory.CreateDirectory(logDir);

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    // This could be optimised to prevent opening and closing the file for each write
                    using (FileStream fs = File.Open(logPath, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter log = new StreamWriter(fs))
                        {
                            log.WriteLine(string.Format("{0}\t{1}", time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), entry.Message));
                            log.WriteLine("****************************************");
                            log.Close();
                        }
                    }

                    logPath = dir + "\\" + time.ToString("yyyy-MM-dd", CultureInfo.GetCultureInfo("en-US")) + ".txt";
                    using (FileStream fs = File.Open(logPath, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter log = new StreamWriter(fs))
                        {
                            log.WriteLine(string.Format("{0}\t{1}", time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), entry.Message));
                            log.WriteLine("****************************************");
                            log.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A Log class to store the message and the Date and Time the log entry was created
        /// </summary>
        public class Log
        {
            public string Message { get; set; }

            public Log(string message)
            {
                Message = message;
            }
        }
    }

    public class MyContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var members = base.GetSerializableMembers(objectType);
            var filteredMembers = new List<MemberInfo>();
            members.ForEach(m =>
            {
                if (m.MemberType == MemberTypes.Property)
                {
                    PropertyInfo info = (PropertyInfo)m;
                    var type = info.PropertyType;
                    if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
                    {
                        filteredMembers.Add(m);
                    }
                }
            });
            return filteredMembers;
        }
    }
}