using ARF.Core.Models;
using ARF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ARF.Core.Managers
{
    public class CommonManager : AbstractManager
    {
        public List<NotificationSummaryModel> GetNotifications()
        {
            try
            {
                var model = new List<NotificationSummaryModel>();

                var notifications = RequestUtils.GetRequests(status: RequestTab.Pending).Select(r =>
                           new NotificationModel
                           {
                               Id = r.Id,
                               Tab = NotificationTab.Requests,
                               Type = ((RequestType)r.Type).ToString(),
                           }
                       ).ToList();


                model = notifications.GroupBy(c => "notification_" + (c.Tab == NotificationTab.Requests ? c.Tab + "_" + c.Type : c.Tab.ToString()).ToLower())
                    .Select(c => new NotificationSummaryModel
                    {
                        LangKey = c.Key,
                        Count = c.Count()
                    }).OrderBy(c => c.LangKey).ToList();

                foreach (var m in model)
                {

                    var t = (RequestType)RequestType.Unspecificed;
                    var typeStr = m.LangKey.Split('_').Length == 3 ? m.LangKey.Split('_')[2] : null;


                    if (Enum.TryParse(typeStr, true, out t) && t != RequestType.Unspecificed)
                    {
                        m.Uri = "Requests/?a=1&t=" + (byte)t;
                    }
                    else
                    {
                        var type = (string)null;

                        if (m.LangKey.Contains("attachment") || m.LangKey.Contains("jobseekerconsent"))
                            type = "attachment";
                        else if (m.LangKey.Contains("transfer"))
                            type = "transfer";
                        else if (m.LangKey.Contains("exiting"))
                            type = "exiting";
                        else if (m.LangKey.Contains("placement"))
                            type = "placement";
                        else if (m.LangKey.Contains("extension"))
                            type = "extension";

                        m.Uri = "Requests/?a=1" + (type == null ? "" : "#" + type);
                    }

                }

                return model;

            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsync(ex);
            }

            return new List<NotificationSummaryModel>();
        }



        public static bool GetIsSystemAdmin(string userId = null)
        {
            return Utils.IsSystemAdmin(Utils.GetUserRoles(userId));
        }

        public static UserPreferences GetUserPreferences()
        {
            if (HttpContextManager.Current == null || HttpContextManager.Current.Request == null) return null;

            var _preferences = HttpContextManager.Current.GetOwinContext().Get<UserPreferences>(typeof(UserPreferences).FullName);

            if (_preferences == null)
            {
                _preferences = new UserPreferences { Language = LanguagePreference.Arabic, Menu = MenuPreference.DontCare };
                var userId = Utils.GetUserId();
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var prefs = GetDatabaseContext().UserPreferences.Where(s => s.UserId == userId).ToList();

                    if (prefs.Count(s => s.Key.ToUpper() == "MENU" && s.UserAgent == HttpContextManager.Current.Request.UserAgent) >= 1)
                    {
                        _preferences.Menu = prefs.FirstOrDefault(s => s.Key.ToUpper() == "MENU" && s.UserAgent == HttpContextManager.Current.Request.UserAgent).Value == "COLLARFED" ? MenuPreference.Collapsed : MenuPreference.Expanded;
                    }
                    else if (prefs.Count(s => s.Key.ToUpper() == "MENU") >= 1)
                    {
                        _preferences.Menu = prefs.FirstOrDefault(s => s.Key.ToUpper() == "MENU").Value == "COLLARFED" ? MenuPreference.Collapsed : MenuPreference.Expanded;
                    }

                    if(prefs.Count(s => s.Key.ToUpper() == "LANG") >= 1)
                    {
                        var langStr = prefs.FirstOrDefault(s => s.Key.ToUpper() == "LANG");

                        if (langStr.Value == "EN")
                        {
                            _preferences.Language = LanguagePreference.English;
                        } else
                        {
                            _preferences.Language = LanguagePreference.Arabic;
                        }
                    } else
                    {

                        var culture = Thread.CurrentThread.CurrentUICulture.Name;

                        if (culture == "en-US")
                        {
                            _preferences.Language = LanguagePreference.English;
                        }
                        else
                        {
                            _preferences.Language = LanguagePreference.Arabic;
                        }

                        _preferences.Language = LanguagePreference.Arabic;

                    }
                } 

                HttpContextManager.Current.GetOwinContext().Set<UserPreferences>(typeof(UserPreferences).FullName, _preferences);
            }

            return _preferences;
        }

       

        public static int SortString(string s1, string s2, string sortDirection)
        {
            try
            {
                s1 = string.IsNullOrEmpty(s1) ? "" : s1;
                s2 = string.IsNullOrEmpty(s2) ? "" : s2;
                return sortDirection == "asc" ? s1.CompareTo(s2) : s2.CompareTo(s1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetApplicationUrl()
        {
            //Return variable declaration
            var appPath = string.Empty;

            //Getting the current context of HTTP request
            var context = HttpContextManager.Current;

            //Checking the current context content
            if (context != null)
            {
                //Formatting the fully qualified website url/name
                appPath = string.Format("{0}://{1}{2}{3}",
                                        context.Request.Url.Scheme,
                                        context.Request.Url.Host,
                                        context.Request.Url.Port == 80 || context.Request.Url.Port == 443
                                            ? string.Empty
                                            : ":" + context.Request.Url.Port,
                                        context.Request.ApplicationPath);
            }

            if (!appPath.EndsWith("/"))
                appPath += "/";

            return appPath;
        }


        public void ToGrid<T>(IQueryable<T> query)
        {

        }
    }
}