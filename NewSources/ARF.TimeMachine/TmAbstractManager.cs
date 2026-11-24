using ARF.Auth;
using ARF.Data;
using ARF.Data.DbModels;

using Microsoft.AspNet.Identity; // Maybe this one too
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.TimeMachine
{
    internal class TmAbstractManager
    {
        
        protected UserSession _session;
        private bool? _isGosiAvailable;
        protected bool? _useRealFinanceDb;
        protected bool? _useFakeServices;
        protected ApplicationUser _user;
        protected ApplicationUser _actualUser;
        protected bool? _isSystemAdmin;
        protected DateTime _currentDateTime;
        protected ArfDbEntities _db;
        protected ApplicationDbContext _authDbContext;
        protected ApplicationSignInManager _signInManager;
        protected ApplicationUserManager _userManager;


        internal ApplicationUserManager UserManager
        {
            get
            {
                if (TmUtils.HasOwinContext)
                {
                    _userManager = TmHttpContextManager.Current.GetOwinContext().Get<ApplicationUserManager>();

                    if (_userManager == null)
                    {
                        _userManager = ApplicationUserManager.Create(new ApplicationDbContext());

                        TmHttpContextManager.Current.GetOwinContext().Set<ApplicationUserManager>(typeof(ApplicationUserManager).FullName, _userManager);
                    }

                    return _userManager;
                }

                return ApplicationUserManager.Create(new ApplicationDbContext());
            }
            private set
            {
                _userManager = value;
            }
        }

        internal ArfDbEntities DbContext
        {
            get
            {
                
                return TmUtils.db;
            }
            set
            {
                TmUtils.db = value;
            }
        }

        internal ApplicationDbContext AuthDbContext
        {
            get
            {

                return TmUtils.auth_db;
            }
            set
            {
                TmUtils.auth_db = value;
            }
        }

        internal ApplicationUser CurrentUser
        {
            get
            {
                if (TmHttpContextManager.Current == null) return null;
                var userId = TmHttpContextManager.Current.User.Identity.EnhancedGetUserId();
                if (string.IsNullOrWhiteSpace(userId)) return null;

                if (TmUtils.HasOwinContext)
                {
                    if (_user == null)
                    {
                        if (TmHttpContextManager.Current != null &&
                            (
                                TmHttpContextManager.Current.User == null ||
                                TmHttpContextManager.Current.User.Identity == null
                            )
                        ) return null;


                        _user = UserManager.FindById(userId);



                        if (UserSession != null && UserSession.CurrentRole != null && UserSession.UserId == _user.Id)
                        {
                            if (TmUtils.IsCollegeRole())
                                _user.CollegeId = (int?)UserSession.CurrentRoleDepartmentId;

                        }

                    }

                    return _user;
                }

                return AuthDbContext.Users.Find("00000000-0000-0000-0000-000000000000");
            }
        }

        internal ApplicationSignInManager SignInManager
        {
            get
            {
                if (_signInManager != null)
                    return _signInManager;

                return _signInManager ?? TmHttpContextManager.Current.GetOwinContext().Get<ApplicationSignInManager>();
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
                return TmUtils.GetSessionId();
            }
        }

        public UserSession UserSession
        {
            get
            {
                if (!TmUtils.HasOwinContext) return null;

                if (_session != null) return _session;

                _session = TmHttpContextManager.Current.GetOwinContext().Get<UserSession>(typeof(UserSession).FullName);

                if (_session == null)
                {
                    var userId = TmUtils.GetUserId();

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
                            IpAddress = TmUtils.GetUserIP(),
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

                    TmHttpContextManager.Current.GetOwinContext().Set<UserSession>(typeof(UserSession).FullName, _session);
                }

                return _session;
            }
        }



    }
}