using ARF.Auth;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using Microsoft.AspNet.Identity; // Maybe this one too
using Microsoft.AspNet.Identity.Owin;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ARF.Core.Managers
{
    public class AbstractManager
    {
        protected UserPreferences _preferences;
        protected UserSession _session;
        protected ApplicationUser _user;
        protected ApplicationUser _actualUser;
        protected ArfUserRole[] _roles;
        protected ArfUserRole[] _actualRoles;
        protected Privilages? _privilages;
        protected bool? _isSystemAdmin;
        protected DateTime _currentDateTime;
        protected ArfDbEntities _db;
        protected ApplicationDbContext _authDbContext;
        protected ApplicationSignInManager _signInManager;
        protected ApplicationUserManager _userManager;

        public bool HasOwinContext
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.Request == null) return false;

                try
                {
                    var owinContext = HttpContext.Current.GetOwinContext();
                    if (owinContext == null) return false;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (HasOwinContext)
                {
                    _userManager = HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>();

                    if (_userManager == null)
                    {
                        _userManager = ApplicationUserManager.Create(AuthDbContext);

                        HttpContext.Current.GetOwinContext().Set<ApplicationUserManager>(typeof(ApplicationUserManager).FullName, _userManager);
                    }

                    return _userManager;
                }

                return ApplicationUserManager.Create(AuthDbContext);
            }
            private set
            {
                _userManager = value;
            }
        }

        public ArfDbEntities DbContext
        {
            get
            {
                if (_db != null) return _db;

                return _db = Utils.db;
            }
            set
            {
                _db = Utils.db = value;
            }
        }

        public ApplicationDbContext AuthDbContext
        {
            get
            {
                if (_authDbContext != null) return _authDbContext;

                return _authDbContext = Utils.auth_db;
            }
            set
            {
                _authDbContext = Utils.auth_db = value;
            }
        }


        public ApplicationUser CurrentUser
        {
            get
            {
                if (HttpContextManager.Current == null) return null;
                var userId = HttpContextManager.Current.User.Identity.EnhancedGetUserId();
                if (string.IsNullOrWhiteSpace(userId)) return null;

                if (Utils.HasOwinContext)
                {
                    if (_user == null)
                    {
                        if (HttpContextManager.Current != null &&
                            (
                                HttpContextManager.Current.User == null ||
                                HttpContextManager.Current.User.Identity == null
                            )
                        ) return null;


                        _user = UserManager.FindById(userId);



                        if (UserSession != null && UserSession.CurrentRole != null && UserSession.UserId == _user.Id)
                        {
                            if (Utils.IsCollegeRole())
                                _user.CollegeId = (int?)UserSession.CurrentRoleDepartmentId;

                        }

                    }

                    return _user;
                }

                return AuthDbContext.Users.Find("00000000-0000-0000-0000-000000000000");
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                if (_signInManager != null)
                    return _signInManager;

                return _signInManager ?? HttpContextManager.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public string SessionId
        {
            get
            {
                return Utils.GetSessionId();
            }
        }

        public UserSession UserSession
        {
            get
            {
                if (!HasOwinContext) return null;

                if (_session != null) return _session;

                _session = HttpContextManager.Current.GetOwinContext().Get<UserSession>(typeof(UserSession).FullName);

                if (_session == null)
                {
                    var userId = Utils.GetUserId();

                    if (userId == null)
                    {
                        _session = DbContext.UserSessions.OrderByDescending(s => s.Created).FirstOrDefault(s => s.Sessionid == SessionId);
                    }
                    else
                    {
                        _session = DbContext.UserSessions.OrderByDescending(s => s.Created).FirstOrDefault(s => s.Sessionid == SessionId && (userId == null || userId == s.UserId));
                    }

                    if (_session == null)
                    {
                        _session = new UserSession
                        {
                            UserId = userId,
                            IpAddress = Utils.GetUserIP(),
                            UserAgent = HttpContext.Current.Request.UserAgent,
                            Expires = DateTime.UtcNow.AddMinutes(60),
                            Locked = false,
                            Sessionid = SessionId,
                            Updated = TM.Now,
                            Created = TM.Now,
                        };

                        DbContext.UserSessions.Add(_session);
                        DbContext.SaveChanges();
                    }

                    HttpContextManager.Current.GetOwinContext().Set<UserSession>(typeof(UserSession).FullName, _session);
                }

                return _session;
            }
        }

        public ApplicationUser ActualCurrentUser
        {
            get
            {
                if (Utils.HasOwinContext)
                {
                    if (_actualUser == null)
                    {
                        if (HttpContextManager.Current != null &&
                            (
                                HttpContextManager.Current.User == null ||
                                HttpContextManager.Current.User.Identity == null
                            )
                        ) return null;

                        _actualUser = UserManager.FindById(HttpContextManager.Current.User.Identity.EnhancedGetUserId());
                    }

                    return _actualUser;
                }

                return AuthDbContext.Users.Find("00000000-0000-0000-0000-000000000000");
            }
        }

        public bool IsImpersonating
        {
            get
            {

                return UserSession.CurrentRole != null;
            }
        }


        public ArfUserRole[] Roles
        {
            get
            {
                if (_roles != null) return _roles;
                if (CurrentUser == null) return new ArfUserRole[0];

                if (UserSession != null && UserSession.CurrentRole != null && UserUtils.CanChangeRoleTo((ArfUserRole)UserSession.CurrentRole, UserSession.CurrentRoleDepartmentId))
                {
                    _roles = new ArfUserRole[] { (ArfUserRole)UserSession.CurrentRole };
                }
                else
                {
                    _roles = Utils.GetUserRoles(CurrentUser.Id);
                }



                return _roles;
            }
        }

        public ArfUserRole[] ActualRoles
        {
            get
            {
                if (_actualRoles != null) return _actualRoles;

                if (CurrentUser == null) return new ArfUserRole[0];
                _actualRoles = Utils.GetActualUserRoles(CurrentUser.Id);

                return _actualRoles;
            }
        }

        public Privilages Privilages
        {
            get
            {
                if (_privilages != null) return _privilages.Value;

                _privilages = Utils.GetUserPrivilages(CurrentUser.Id);

                return _privilages.Value;
            }
        }

        public bool IsCollegeUser
        {
            get
            {
                return Utils.IsCollegeRole() && CurrentUser.CollegeId != null;
            }
        }

        public bool IsSystemAdmin
        {
            get
            {
                if (_isSystemAdmin != null) return _isSystemAdmin.Value;
                _isSystemAdmin = Utils.IsSystemAdmin(Roles);
                return _isSystemAdmin.Value;
            }
        }

        public static ArfDbEntities GetDatabaseContext()
        {
            var _db = HttpContextManager.Current.GetOwinContext().Get<ArfDbEntities>(typeof(ArfDbEntities).FullName);
            if (_db == null)
            {
                _db = new ArfDbEntities();
                HttpContextManager.Current.GetOwinContext().Set<ArfDbEntities>(typeof(ArfDbEntities).FullName, _db);
            }

            return _db;
        }

        private static Regex _rx_national_id = new Regex(@"^([1-2][0-9]{9})$", RegexOptions.Compiled);
        private static Regex _rx_national_id_start = new Regex(@"^[1-2]\d{0,9}$", RegexOptions.Compiled);

        public static bool IsValidNationalId(string nin)
        {
            if (string.IsNullOrWhiteSpace(nin)) return false;
            nin = nin.Replace(" ", "");

            return _rx_national_id.Match(nin).Success && Utils.CalculateLuhnAlgorithm(nin);
        }

        public static bool IsSeeminglyValidNationalId(string nin)
        {
            if (string.IsNullOrWhiteSpace(nin)) return false;

            var ninInt = 09;
            if (nin.Length == 10 && int.TryParse(nin, out ninInt)) return true;

            return false;
        }

        public static bool IsValidNationalIdStart(string nin)
        {
            if (string.IsNullOrWhiteSpace(nin)) return false;
            nin = nin.Replace(" ", "");

            return _rx_national_id_start.Match(nin).Success;
        }

        public static UsersManager UserUtils
        {
            get
            {
                return Utils.UserUtils;
            }
        }

        public static RequestsManager RequestUtils
        {
            get
            {
                return Utils.GetInstance<RequestsManager>();
            }
        }

        public static ExportManager ExportUtils
        {
            get
            {
                return Utils.GetInstance<ExportManager>();
            }
        }

        public static TemplatesManager TemplateUtils
        {
            get
            {
                return Utils.GetInstance<TemplatesManager>();
            }
        }

        public static CommonManager CommonUtils
        {
            get
            {
                return Utils.GetInstance<CommonManager>();
            }
        }

        private static string[] _validExtensions = new string[] { ".doc", ".docx", ".pdf", ".png", ".jpg", ".jpeg" };

        public static string[] ValidFileExtensions
        {
            get
            {
                return _validExtensions;
            }
        }

        public static bool IsValidExtensionForFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;

            var ext = Path.GetExtension(fileName);

            if (string.IsNullOrWhiteSpace(ext)) return false;

            ext = ext.Trim().ToLower();

            if (!ext.StartsWith("."))
                ext = "." + ext;

            return _validExtensions.Contains(ext);
        }

        public DateTime CurrentDateTime
        {
            get
            {
                if (_currentDateTime != null && _currentDateTime != DateTime.MinValue) return _currentDateTime;

                if (HasOwinContext)
                {
                    _currentDateTime = HttpContextManager.Current.GetOwinContext().Get<DateTime>("CurrentDateTime");
                }

                if (_currentDateTime == null || _currentDateTime == DateTime.MinValue)
                {
                    _currentDateTime = TM.Now;

                    if (HasOwinContext)
                    {
                        HttpContextManager.Current.GetOwinContext().Set<DateTime>("CurrentDateTime", _currentDateTime);
                    }
                }

                return _currentDateTime;
            }

            set
            {
                _currentDateTime = value;
                HttpContextManager.Current.GetOwinContext().Set<DateTime>("CurrentDateTime", _currentDateTime);
            }
        }

        public static DateTime CurrentDateTimeStatic
        {
            get
            {
                var _currentDateTime = TM.Now;

                if (Utils.HasOwinContext)
                {
                    _currentDateTime = HttpContextManager.Current.GetOwinContext().Get<DateTime>("CurrentDateTime");
                }

                if (_currentDateTime == DateTime.MinValue)
                {
                    _currentDateTime = TM.Now;

                    if (Utils.HasOwinContext)
                    {
                        HttpContextManager.Current.GetOwinContext().Set<DateTime>("CurrentDateTime", _currentDateTime);
                    }
                }

                return _currentDateTime;
            }

            set
            {
                HttpContextManager.Current.GetOwinContext().Set<DateTime>("CurrentDateTime", value);
            }
        }

        public UserPreferences Preferences
        {
            get
            {
                if (_preferences == null)
                {
                    _preferences = new UserPreferences { Language = LanguagePreference.DontCare, Menu = MenuPreference.DontCare };

                    if (CurrentUser != null && CurrentUser.Id != null)
                    {
                        var prefs = DbContext.UserPreferences.Where(s => s.UserId == CurrentUser.Id).ToList();

                        if (prefs.Count(s => s.Key.ToUpper() == "LANG") >= 1)
                        {
                            _preferences.Language = prefs.FirstOrDefault(s => s.Key.ToUpper() == "LANG").Value == "AR" ? LanguagePreference.Arabic : LanguagePreference.English;
                        }

                        if (prefs.Count(s => s.Key.ToUpper() == "MENU" && s.UserAgent == HttpContextManager.Current.Request.UserAgent) >= 1)
                        {
                            _preferences.Menu = prefs.FirstOrDefault(s => s.Key.ToUpper() == "MENU" && s.UserAgent == HttpContextManager.Current.Request.UserAgent).Value == "COLLARFED" ? MenuPreference.Collapsed : MenuPreference.Expanded;
                        }
                        else if (prefs.Count(s => s.Key.ToUpper() == "MENU") >= 1)
                        {
                            _preferences.Menu = prefs.FirstOrDefault(s => s.Key.ToUpper() == "MENU").Value == "COLLARFED" ? MenuPreference.Collapsed : MenuPreference.Expanded;
                        }
                    }
                }

                return _preferences;
            }
        }

        public string QueryString(string key)
        {
            if (HttpContextManager.Current == null || HttpContextManager.Current.Request == null || HttpContextManager.Current.Request.QueryString == null) return null;

            return HttpContextManager.Current.Request.QueryString[key];
        }

        public bool IsArabic
        {
            get
            {
                return Preferences.Language == LanguagePreference.Arabic;
            }
        }
    }
}