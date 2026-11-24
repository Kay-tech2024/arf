using ARF.Auth;
using ARF.Data;
using ARF.Data.DbModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace ARF.TimeMachine
{

    public static class TmUtils
    {

        internal static ArfDbEntities db
        {
            get
            {
                var _db = TmUtils.HasOwinContext ? HttpContext.Current.GetOwinContext().Get<ArfDbEntities>(typeof(ArfDbEntities).FullName) : null;
                if (_db == null)
                {
                    if (TmUtils.HasOwinContext)
                    {
                        _db = new ArfDbEntities();

                        _db.Configuration.AutoDetectChangesEnabled = true;
                        _db.Configuration.LazyLoadingEnabled = true;
                        _db.Configuration.ValidateOnSaveEnabled = false;

                        HttpContext.Current.GetOwinContext().Set<ArfDbEntities>(typeof(ArfDbEntities).FullName, _db);
                    }
                    else
                    {
                        if (_staticObjs == null) _staticObjs = new Dictionary<string, object>();

                        if (_staticObjs.ContainsKey(typeof(ArfDbEntities).FullName))
                            return _staticObjs[typeof(ArfDbEntities).FullName] as ArfDbEntities;

                        _db = new ArfDbEntities();
                        _db.Configuration.AutoDetectChangesEnabled = true;
                        _db.Configuration.LazyLoadingEnabled = true;
                        _db.Configuration.ValidateOnSaveEnabled = false;

                        _staticObjs[typeof(ArfDbEntities).FullName] = _db;
                    }
                }

                return _db;
            }

            set
            {
                value.Configuration.AutoDetectChangesEnabled = true;
                value.Configuration.LazyLoadingEnabled = true;
                value.Configuration.ValidateOnSaveEnabled = false;

                if (TmUtils.HasOwinContext)
                    HttpContext.Current.GetOwinContext().Set<ArfDbEntities>(typeof(ArfDbEntities).FullName, value);
                else
                {
                    if (_staticObjs == null) _staticObjs = new Dictionary<string, object>();

                    if (_staticObjs.ContainsKey(typeof(ArfDbEntities).FullName))
                        _staticObjs[typeof(ArfDbEntities).FullName] = value;
                }
            }
        }

        internal static ApplicationDbContext auth_db
        {
            get
            {
                var _db = TmUtils.HasOwinContext ? HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>(typeof(ApplicationDbContext).FullName) : null;
                if (_db == null)
                {
                    if (TmUtils.HasOwinContext)
                    {
                        _db = new ApplicationDbContext();

                        _db.Configuration.AutoDetectChangesEnabled = true;
                        _db.Configuration.LazyLoadingEnabled = true;
                        _db.Configuration.ValidateOnSaveEnabled = false;

                        HttpContext.Current.GetOwinContext().Set<ApplicationDbContext>(typeof(ApplicationDbContext).FullName, _db);
                    }
                    else
                    {
                        if (_staticObjs == null) _staticObjs = new Dictionary<string, object>();

                        if (_staticObjs.ContainsKey(typeof(ApplicationDbContext).FullName))
                            return _staticObjs[typeof(ApplicationDbContext).FullName] as ApplicationDbContext;

                        _db = new ApplicationDbContext();
                        _db.Configuration.AutoDetectChangesEnabled = true;
                        _db.Configuration.LazyLoadingEnabled = true;
                        _db.Configuration.ValidateOnSaveEnabled = false;

                        _staticObjs[typeof(ApplicationDbContext).FullName] = _db;
                    }
                }

                return _db;
            }

            set
            {
                value.Configuration.AutoDetectChangesEnabled = true;
                value.Configuration.LazyLoadingEnabled = true;
                value.Configuration.ValidateOnSaveEnabled = false;

                if (TmUtils.HasOwinContext)
                    HttpContext.Current.GetOwinContext().Set<ApplicationDbContext>(typeof(ApplicationDbContext).FullName, value);
                else
                {
                    if (_staticObjs == null) _staticObjs = new Dictionary<string, object>();

                    if (_staticObjs.ContainsKey(typeof(ApplicationDbContext).FullName))
                        _staticObjs[typeof(ApplicationDbContext).FullName] = value;
                }
            }
        }

        internal static ArfUserRole[] GetActualUserRoles(HttpContextBase context = null, string userId = null)
        {
            if (userId == null)
            {
                if (context == null)
                {
                    context = TmHttpContextManager.Current;
                }

                if (context != null && context.User != null && context.User.Identity != null)
                {
                    if (context.User.Identity is ClaimsIdentity)
                    {
                        var identity = (ClaimsIdentity)context.User.Identity;
                        var roleClaims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

                        var roles = new List<ArfUserRole>();

                        foreach (var c in roleClaims)
                        {
                            var role = ArfUserRole.None;

                            if (Enum.TryParse(c.Value, out role) && role != ArfUserRole.None)
                            {
                                roles.Add(role);
                            }
                        }

                        return roles.ToArray();
                    }
                    else
                    {
                        var username = context.User.Identity.Name;
                        var user = TmUtils.db.Users.FirstOrDefault(u => u.UserName == username);

                        if (user != null)
                        {
                            var roles = new List<ArfUserRole>();

                            foreach (var c in user.Roles)
                            {
                                var role = ArfUserRole.None;

                                if (Enum.TryParse(c.Name, out role) && role != ArfUserRole.None)
                                {
                                    roles.Add(role);
                                }
                            }

                            return roles.ToArray();
                        }
                    }
                }
            }
            else
            {
                var user = TmUtils.db.Users.SingleOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    var userRoles = user.Roles.Select(r => r.Name).ToList();

                    var roles = new List<ArfUserRole>();

                    foreach (var r in userRoles)
                    {
                        var role = ArfUserRole.None;

                        if (Enum.TryParse(r, out role) && role != ArfUserRole.None)
                        {
                            roles.Add(role);
                        }
                    }

                    return roles.ToArray();
                }
            }

            return new ArfUserRole[0];
        }


        public static string EnhancedGetSessionId()
        {
            if (TmHttpContextManager.Current.Items.Contains("is_unit_testing"))
            {
                var db = TmUtils.db;
                var username = TmUtils.GetUsername();
                var user = db.Users.FirstOrDefault(u => u.UserName == username);

                if (user == null) return null;

                var sessionClaim = db.UserSessions.OrderBy(c => c.Created).FirstOrDefault(e => e.UserId == user.Id);

                if (sessionClaim != null)
                {
                    return sessionClaim.Sessionid;
                }

                return null;
            }
            else
            {
                return GetSessionId();
            }
        }

        public static string GetSessionId()
        {
            //Get the current claims principal
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            // Get the claims values
            var name = identity.Claims.Where(c => c.Type == "SESSION_ID")
                               .Select(c => c.Value).SingleOrDefault();

            if (name != null) return name;
            return HttpContext.Current.Session.SessionID;
        }

        internal static UserSession GetCurrentUserSession(ArfDbEntities db = null)
        {
            if (TmHttpContextManager.Current == null) return null;

            if (db == null)
            {
                db = TmUtils.db;
            }

            var _session = TmHttpContextManager.Current.GetOwinContext().Get<UserSession>(typeof(UserSession).FullName);

            if (_session == null)
            {
                var sessionId = EnhancedGetSessionId();
                _session = db.UserSessions.OrderByDescending(s => s.Created).FirstOrDefault(s => s.Sessionid == sessionId);

                TmHttpContextManager.Current.GetOwinContext().Set<UserSession>(typeof(UserSession).FullName, _session);
            }

            return _session;
        }


        internal static ArfUserRole[] GetUserRoles(HttpContextBase context = null, string userId = null)
        {
            if (userId == null)
            {
                if (context == null)
                {
                    context = TmHttpContextManager.Current;
                }

                if (context != null && context.User != null && context.User.Identity != null)
                {
                    if (context.User.Identity is ClaimsIdentity)
                    {
                        var identity = (ClaimsIdentity)context.User.Identity;
                        var roleClaims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

                        var roles = new List<ArfUserRole>();

                        var UserSession = GetCurrentUserSession();
                        if (UserSession != null && UserSession.CurrentRole != null)
                        {
                            roles = new List<ArfUserRole>() { (ArfUserRole)UserSession.CurrentRole };
                        }
                        else
                        {
                            foreach (var c in roleClaims)
                            {
                                var role = ArfUserRole.None;

                                if (Enum.TryParse(c.Value, out role) && role != ArfUserRole.None)
                                {
                                    roles.Add(role);
                                }
                            }
                        }

                        return roles.ToArray();
                    }
                    else
                    {
                        var username = context.User.Identity.Name;
                        var user = TmUtils.db.Users.FirstOrDefault(u => u.UserName == username);

                        if (user != null)
                        {
                            var roles = new List<ArfUserRole>();

                            foreach (var c in user.Roles)
                            {
                                var role = ArfUserRole.None;

                                if (Enum.TryParse(c.Name, out role) && role != ArfUserRole.None)
                                {
                                    roles.Add(role);
                                }
                            }

                            return roles.ToArray();
                        }
                    }
                }
            }
            else
            {
                var user = TmUtils.db.Users.Include("Roles").SingleOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    var userRoles = user.Roles.Select(r => r.Name).ToList();

                    var roles = new List<ArfUserRole>();

                    foreach (var r in userRoles)
                    {
                        var role = ArfUserRole.None;

                        if (Enum.TryParse(r, out role) && role != ArfUserRole.None)
                        {
                            roles.Add(role);
                        }
                    }

                    return roles.ToArray();
                }
            }

            return new ArfUserRole[0];
        }


        [ThreadStatic]
        private static Dictionary<string, dynamic> _staticObjs = new Dictionary<string, dynamic>();

        public static bool IsSystemWide
        {
            get
            {
                return GetDictVal("TimeMachineIsSystemWide") == "True";
            }
            set
            {
                SetDictVal("TimeMachineIsSystemWide", value.ToString());
            }
        }

        public static dynamic GetDictVal(string key)
        {

            if (_staticObjs == null) _staticObjs = new Dictionary<string, dynamic>();

            if (_staticObjs.ContainsKey(key))
            {
                return _staticObjs[key];
            }

            return null;

        }

        public static void SetDictVal(string key, dynamic value)
        {

            if (_staticObjs == null) _staticObjs = new Dictionary<string, dynamic>();

            if (_staticObjs.ContainsKey(key))
            {
                _staticObjs[key] = value;
            }
            else
            {
                _staticObjs.Add(key, value);
            }

        }



        public static DateTime CurrentDateTimeStatic
        {
            get
            {
                var isEnabled = ConfigurationManager.AppSettings["TimeMachineEnabled"] != null && ConfigurationManager.AppSettings["TimeMachineEnabled"].ToLower().Trim() == "true";

                if (!isEnabled) return DateTime.Now;

                var _currentDateTime = DateTime.MinValue;

                if (HasOwinContext)
                {
                    _currentDateTime = TmHttpContextManager.Current.GetOwinContext().Get<DateTime>("CurrentDateTime");
                }

                if (_currentDateTime == DateTime.MinValue)
                {
                    _currentDateTime = DateTime.Now;

                    //if (UserUtils.CurrentUser != null && UserUtils.CurrentUser.Id != null)
                    //{

                    //SG fix for Transactional Linq

                    using (var con = new ArfDbEntities())
                    using (var tx = con.Database.BeginTransaction())
                    {

                        try
                        {
                            double diff = 0;
                            if (UserUtils.CurrentUser != null && UserUtils.CurrentUser.Id != null)
                            {
                                var userPreferences = UserUtils.DbContext.UserPreferences
                                .Where(u => u.UserId == UserUtils.CurrentUser.Id)
                                .Where(u => u.Key == "TIME")
                                .FirstOrDefault();

                                tx.Commit();
                                con.SaveChanges();

                                if (userPreferences != null && double.TryParse(userPreferences.Value, out diff))
                                {
                                    _currentDateTime = DateTime.Now.AddMinutes(-diff);
                                }
                                else
                                {
                                    var setting = UserUtils.DbContext.Settings
                                                  .Where(u => u.Key == "TIME")
                                                  .FirstOrDefault();


                                    if (setting != null && double.TryParse(setting.Value, out diff))
                                    {
                                        _currentDateTime = DateTime.Now.AddMinutes(-diff);

                                    }
                                }
                            }
                            else
                            {
                                var setting = UserUtils.DbContext.Settings
                                                 .Where(u => u.Key == "TIME")
                                                 .FirstOrDefault();

                                if (setting != null && double.TryParse(setting.Value, out diff))
                                {
                                    _currentDateTime = DateTime.Now.AddMinutes(-diff);

                                }
                            }

                        }

                        catch (Exception)
                        {
                            tx.Rollback();
                        }
                    }

                    if (HasOwinContext)
                    {
                        TmHttpContextManager.Current.GetOwinContext().Set<DateTime>("CurrentDateTime", _currentDateTime);
                    }
                }

                return _currentDateTime;
            }

            set
            {
                if (IsSystemWide)
                {
                    var setting = UserUtils.DbContext.Settings
                        .Where(u => u.Key == "TIME")
                        .FirstOrDefault();


                    if (setting == null)
                    {
                        setting = new Setting()
                        {
                            Key = "TIME",
                            Value = (DateTime.Now - value).TotalMinutes + "",
                            Created = DateTime.Now,
                            Updated = DateTime.Now,
                            CreatedBy = UserUtils.CurrentUser.Id,
                            UpdatedBy = UserUtils.CurrentUser.Id

                        };

                        UserUtils.DbContext.Settings.Add(setting);
                    }
                    else
                    {
                        setting.Value = (DateTime.Now - value).TotalMinutes + "";
                        setting.Updated = DateTime.Now;
                        setting.UpdatedBy = UserUtils.CurrentUser.Id;
                        UserUtils.DbContext.Entry(setting).State = System.Data.Entity.EntityState.Modified;

                    }

                    UserUtils.DbContext.SaveChanges();
                }
                else if (UserUtils.CurrentUser != null && UserUtils.CurrentUser.Id != null)
                {
                    var userPreferences = UserUtils.DbContext.UserPreferences
                        .Where(u => u.UserId == UserUtils.CurrentUser.Id)
                        .Where(u => u.Key == "TIME")
                        .FirstOrDefault();


                    if (userPreferences == null)
                    {
                        userPreferences = new UserPreference()
                        {
                            Key = "TIME",
                            Value = (DateTime.Now - value).TotalMinutes + "",
                            UserId = UserUtils.CurrentUser.Id,

                        };

                        UserUtils.DbContext.UserPreferences.Add(userPreferences);
                    }
                    else
                    {
                        userPreferences.Value = (DateTime.Now - value).TotalMinutes + "";
                        UserUtils.DbContext.Entry(userPreferences).State = System.Data.Entity.EntityState.Modified;

                    }

                    UserUtils.DbContext.SaveChanges();
                }


                TmHttpContextManager.Current.GetOwinContext().Set<DateTime>("CurrentDateTime", value);
            }
        }


        internal static bool HasOwinContext
        {
            get
            {
                if (TmHttpContextManager.Current == null || TmHttpContextManager.Current.Request == null) return false;

                try
                {
                    var owinContext = TmHttpContextManager.Current.GetOwinContext();
                    if (owinContext == null) return false;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        internal static bool IsCollegeRole(params ArfUserRole[] roles)
        {
            if (roles == null || roles.Length == 0) roles = GetUserRoles();

            if (roles.Count(role => role == ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch || role.ToString().StartsWith("ResearchUnit")) > 0) return true;

            return false;
        }


        internal static List<string> GetCollegeRoles()
        {
            var roles = TmUtils.db.Roles
                .Where(r => r.Name == ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch.ToString() || r.Name.StartsWith("ResearchUnit"))
                .Select(r => r.Id)
                .ToList();

            return roles;
        }

        internal static bool IsMajorRole(params ArfUserRole[] roles)
        {
            if (roles == null || roles.Length == 0) roles = GetUserRoles();

            if (roles.Count(role => role.ToString().Contains("Referee")) > 0) return true;

            return false;
        }


        private static TmUsersManager UserUtils
        {
            get
            {
                return TmUtils.GetInstance<TmUsersManager>();
            }
        }

        public static T GetInstance<T>() where T : new()
        {
            if (!HasOwinContext)
            {
                if (ConfigurationManager.AppSettings["migration_files"] != null)
                {

                    var instance = new T();


                    return instance;
                }

                return new T();
            }
            else
            {
                var instance = TmHttpContextManager.Current.GetOwinContext().Get<T>(typeof(T).FullName);
                if (instance == null)
                {
                    instance = new T();
                    TmHttpContextManager.Current.GetOwinContext().Set<T>(typeof(T).FullName, instance);
                }
                return instance;
            }



        }

        public static ArfDbEntities GetDbContext()
        {
            var _db = TmUtils.HasOwinContext ? HttpContext.Current.GetOwinContext().Get<ArfDbEntities>(typeof(ArfDbEntities).FullName) : TmUtils.GetDictVal(typeof(ArfDbEntities).FullName);

            if (_db == null)
            {
                if (TmUtils.HasOwinContext)
                {
                    _db = new ArfDbEntities();

                    HttpContext.Current.GetOwinContext().Set<ArfDbEntities>(typeof(ArfDbEntities).FullName, _db);
                }
                else
                {
                    if (TmUtils.GetDictVal(typeof(ArfDbEntities).FullName) != null)
                        return TmUtils.GetDictVal(typeof(ArfDbEntities).FullName);


                    _db = new ArfDbEntities();

                    TmUtils.SetDictVal(typeof(ArfDbEntities).FullName, _db);

                }
            }

            return _db;
        }

        internal static string EnhancedGetUserId(this IIdentity p)
        {


            if (TmHttpContextManager.Current.Items.Contains("is_unit_testing"))
            {
                var db = TmUtils.GetDbContext();
                var username = TmUtils.GetUsername();
                var user = db.Users.FirstOrDefault(u => u.UserName == username && u.IsDeleted == false);

                if (user == null) return null;

                return user.Id;
            }
            else
            {
                var userId = TmHttpContextManager.Current.GetOwinContext().Get<string>("UserId");

                if (!string.IsNullOrWhiteSpace(userId)) return userId;

                userId = p.GetUserId();

                TmHttpContextManager.Current.GetOwinContext().Set("UserId", userId);

                return userId;
            }
        }

        public static string GetUsername()
        {
            if (TmHttpContextManager.Current != null && TmHttpContextManager.Current.Request != null && TmHttpContextManager.Current.User != null && TmHttpContextManager.Current.User.Identity != null && TmHttpContextManager.Current.User.Identity.IsAuthenticated)
            {
                return TmHttpContextManager.Current.User.Identity.Name;
            }

            return "NOT_AUTHENTICATED";
        }

        public static bool TimeMachineEnabled
        {



            get
            {
                var isEnabled = ConfigurationManager.AppSettings["TimeMachineEnabled"] != null && ConfigurationManager.AppSettings["TimeMachineEnabled"].ToLower().Trim() == "true";

                if (!isEnabled) return false;

                var realTime = DateTime.Now;
                var tmTime = TM.Now;
                if (realTime.Year != tmTime.Year || realTime.Month != tmTime.Month || realTime.Day != tmTime.Day || realTime.Hour != tmTime.Hour || realTime.Minute != tmTime.Minute)
                    return true;

                return false;
            }
        }


        public static string GetUserIP()
        {
            string strIP = String.Empty;
            if (TmHttpContextManager.Current == null || TmHttpContextManager.Current.Request == null) return null;

            var httpReq = TmHttpContextManager.Current.Request;

            //test for non-standard proxy server designations of client's IP
            if (httpReq.ServerVariables["HTTP_CLIENT_IP"] != null)
            {
                strIP = httpReq.ServerVariables["HTTP_CLIENT_IP"].ToString();
            }
            else if (httpReq.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                strIP = httpReq.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            //test for host address reported by the server
            else if
            (
                //if exists
                (httpReq.UserHostAddress.Length != 0)
                &&
                //and if not localhost IPV6 or localhost name
                ((httpReq.UserHostAddress != "::1") || (httpReq.UserHostAddress != "localhost"))
            )
            {
                strIP = httpReq.UserHostAddress;
            }
            //finally, if all else fails, get the IP from a web scrape of another server
            else
            {
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                using (WebResponse response = request.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    strIP = sr.ReadToEnd();
                }
                //scrape ip from the html
                int i1 = strIP.IndexOf("Address: ") + 9;
                int i2 = strIP.LastIndexOf("</body>");
                strIP = strIP.Substring(i1, i2 - i1);
            }
            return strIP;
        }


        public static string GetUserId()
        {
            if (TmHttpContextManager.Current == null || TmHttpContextManager.Current.User == null) return null;

            return TmHttpContextManager.Current.User.Identity.EnhancedGetUserId();
        }

    }
}
