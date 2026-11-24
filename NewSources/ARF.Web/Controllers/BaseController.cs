using ARF.Auth;
using ARF.Core;
using ARF.Core.Managers;
using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.Services;
using ARF.TimeMachine;
using ARF.Web.App_Start.Code;
using ARF.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace ARF.Web.Controllers
{
    [CustomAuthorize]
    public class BaseController : Controller
    {
        protected ArfDbEntities _db;
        protected SmtpSettings _smtpSettings;
        protected SmsSettings _malathSmsSettings;
        protected SmsSettings _mobilySmsSettings;
        protected UserPreferences _preferences;
        protected ApplicationUser _user;
        protected ArfUserRole[] _roles;
        protected Privilages? _privilages;
        protected bool? _isProviderRole;
        protected bool? _isAdmin;
        protected bool? _isJpcRole;
        protected bool? _isSystemAdmin;
        protected ApplicationDbContext _authDbContext;
        protected ApplicationSignInManager _signInManager;
        protected ApplicationUserManager _userManager;

        public new HttpContextBase HttpContext
        {
            get
            {
                return HttpContextManager.Current;
            }
        }

        public new HttpRequestBase Request
        {
            get
            {
                return HttpContext.Request;
            }
        }

        public bool IsDebug { get { return ((CompilationSection)ConfigurationManager.GetSection(@"system.web/compilation")).Debug; } }

        public bool IsBackgroundJob { get { return HttpContext == null; } }

        public Request GetRequest(long? id = null)
        {
            var req = DbContext.Requests.Where(e => e.Id == id).ToList();
            if (req != null)
            {
                return req.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                if (_signInManager != null)
                    return _signInManager;

                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (HttpContext == null) return null;
                if (_userManager != null) return _userManager;

                _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                if (_userManager == null)
                {
                    _userManager = ApplicationUserManager.Create(AuthDbContext);
                }

                return _userManager;
            }
        }

        public string SessionId
        {
            get
            {
                var sessionClaim = SignInManager.AuthenticationManager.User.Claims.FirstOrDefault(c => c.Type == "SESSION_ID");

                if (sessionClaim != null)
                {
                    return sessionClaim.Value;
                }

                return null;
            }
        }

        public UserSession UserSession
        {
            get
            {
                return UserUtils.UserSession;
            }
        }

        public string UserId
        {
            get
            {
                return CurrentUser == null ? Utils.SystemAdminUserId : CurrentUser.Id;
            }
        }

        public ArfDbEntities DbContext
        {
            get
            {
                return Utils.db;
            }
        }

        public bool IsArabic
        {
            get
            {
                return true;
            }
        }

        public ApplicationDbContext AuthDbContext
        {
            get
            {
                return UserManager.AuthDbContext;
            }
            private set
            {
                _authDbContext = value;
            }
        }

        protected override void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            base.OnAuthenticationChallenge(filterContext);
        }

        public bool ValidateProfile()
        {
            if (CurrentUser == null || !Utils.HasRolesAny(ArfUserRole.PrincipalResearcher)) return true;

            var researcher = DbContext.Researchers.FirstOrDefault(r => r.UserId == CurrentUser.Id);

            if (researcher == null) return false;

            var oldErrors = new List<KeyValuePair<string, ModelState>>();

            foreach (var err in ModelState)
            {
                oldErrors.Add(err);
            }

            ModelState.Clear();

            var model = new CoresearcherViewModel
            {
                RoleInProject = ResearcherRole.MainResearcher,
                Id = researcher.Id,
                FullName = researcher.GetFullName(),
                Qualification = researcher.Qualification == null ? QualificationType.Unspecified : (QualificationType)researcher.Qualification,
                UniversityId = researcher.UniversityId == null && researcher.UniversityOther != null ? 0 : researcher.UniversityId,
                CollegeId = researcher.CollegeId == null && researcher.CollegeOther != null ? 0 : researcher.CollegeId,
                DepartmentID = researcher.DepartmentId == null && researcher.DepartmentOther != null ? 0 : researcher.DepartmentId,
                UniversityOther = researcher.UniversityOther,
                CollegeOther = researcher.CollegeOther,
                DepartmentOther = researcher.DepartmentOther,
                MobileNumber = researcher.MobileNo,
                Email = researcher.Email,
                NationalID = researcher.NationalId,
                Addresss = researcher.Address,
                Birthdate = researcher.DateOfBirth == null ? null : researcher.DateOfBirth.Value.FormatDate(false, false, false),
                NationalityId = researcher.NationalityId,
                CountryId = researcher.CountryId,
                CityId = researcher.CityId,
                Specialization = researcher.Specialization,
                BankAccountHolder = researcher.User.UserProfile.BankAccountHolder,
                BankIban = researcher.User.UserProfile.BankIban,
                BankIbanPhotocopyReadonly = researcher.User.UserFiles
                                                .Where(f => f.Type == (byte)FileType.IbanPhotocopy)
                                                .OrderByDescending(f => f.Created)
                                                .Select(c => new AttachedFile
                                                {
                                                    FileId = c.Id.ToString(),
                                                    Type = (FileType)c.Type,
                                                    FileName = Path.GetFileName(c.FileName)
                                                })
                                                .FirstOrDefault(),
                Bank = researcher.User.UserProfile.BankId,
                BankOther = researcher.User.UserProfile.BankName,
                BankBranch = researcher.User.UserProfile.BankBranch,
                SectionPublishedPapers = researcher.User.UserProfile.CvSectionPublishedPapers,
                SectionInterests = researcher.User.UserProfile.CvSectionInterests,
                SectionCareerHistory = researcher.User.UserProfile.CvSectionCareerHistory,
                SectionAcademicHistory = researcher.User.UserProfile.CvSectionAcademicHistory,
                SectionTrainingCourses = researcher.User.UserProfile.CvSectionTrainingCourses,
                SectionAwards = researcher.User.UserProfile.CvSectionAwards,
                SectionMemberships = researcher.User.UserProfile.CvSectionMemberships,
            };

            var isValid = TryValidateModel(model);

            if (model.UniversityId == null)
            {
                ModelState.Remove("UniversityOther");
            }

            if (model.CollegeId == null)
            {
                ModelState.Remove("CollegeOther");
            }

            if (model.DepartmentID == null)
            {
                ModelState.Remove("DepartmentOther");
            }

            var lang = (string)null;
            var rmodel = model;

            if (rmodel.BankIbanPhotocopy == null || rmodel.BankIbanPhotocopy.ContentLength == 0)
            {
                var oldFile = rmodel.Id > 0 ? DbContext.UserFiles
                                    .Where(f => f.User.Researchers.Count(r => r.Id == rmodel.Id) > 0)
                                    .Where(f => f.Type == (byte)FileType.IbanPhotocopy)
                                    .OrderByDescending(f => f.Created)
                                    .FirstOrDefault() : null;

                if (oldFile == null)
                {
                    ModelState.AddModelError("BankIbanPhotocopy", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("BankIbanPhotocopy", false, lang)));
                }
            }

            if (string.IsNullOrWhiteSpace(rmodel.SectionPublishedPapers) || Utils.StripHTML(rmodel.SectionPublishedPapers).Trim().Length == 0)
            {
                ModelState.AddModelError("SectionPublishedPapers", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionPublishedPapers", false, lang)));
            }

            if (string.IsNullOrWhiteSpace(rmodel.SectionInterests) || Utils.StripHTML(rmodel.SectionInterests).Trim().Length == 0)
            {
                ModelState.AddModelError("SectionInterests", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionInterests", false, lang)));
            }

            if (string.IsNullOrWhiteSpace(rmodel.SectionCareerHistory) || Utils.StripHTML(rmodel.SectionCareerHistory).Trim().Length == 0)
            {
                ModelState.AddModelError("SectionCareerHistory", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionCareerHistory", false, lang)));
            }

            if (string.IsNullOrWhiteSpace(rmodel.SectionAcademicHistory) || Utils.StripHTML(rmodel.SectionAcademicHistory).Trim().Length == 0)
            {
                ModelState.AddModelError("SectionAcademicHistory", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionAcademicHistory", false, lang)));
            }

            if (string.IsNullOrWhiteSpace(rmodel.SectionTrainingCourses) || Utils.StripHTML(rmodel.SectionTrainingCourses).Trim().Length == 0)
            {
                ModelState.AddModelError("SectionTrainingCourses", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionTrainingCourses", false, lang)));
            }

            if (string.IsNullOrWhiteSpace(rmodel.SectionAwards) || Utils.StripHTML(rmodel.SectionAwards).Trim().Length == 0)
            {
                ModelState.AddModelError("SectionAwards", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionAwards", false, lang)));
            }

            if (string.IsNullOrWhiteSpace(rmodel.SectionMemberships) || Utils.StripHTML(rmodel.SectionMemberships).Trim().Length == 0)
            {
                ModelState.AddModelError("SectionMemberships", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionMemberships", false, lang)));
            }

            isValid = ModelState.IsValid;

            foreach (var err in oldErrors)
            {
                if (ModelState.ContainsKey(err.Key)) continue;

                ModelState.Add(err);
            }

            return isValid;
        }

        public SmtpSettings SmtpSettings
        {
            get
            {
                if (_smtpSettings == null)
                {
                    _smtpSettings = Utils.GetSmtpSettings(DbContext);
                }

                return _smtpSettings;
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

                        if (prefs.Count(s => s.Key.ToUpper() == "MENU" && s.UserAgent == Request.UserAgent) >= 1)
                        {
                            _preferences.Menu = prefs.FirstOrDefault(s => s.Key.ToUpper() == "MENU" && s.UserAgent == Request.UserAgent).Value == "COLLARFED" ? MenuPreference.Collapsed : MenuPreference.Expanded;
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

        public IList<string> CurrentUserRoles
        {
            get
            {
                return UserManager.GetRoles(CurrentUser.Id);
            }
        }

        public ApplicationUser CurrentUser
        {
            get
            {
                return UserUtils.CurrentUser;
            }
        }

        public ApplicationUser ActualCurrentUser
        {
            get
            {
                return UserUtils.ActualCurrentUser;
            }
        }

        public void SetCurrentUser(string username)
        {
            _user = AuthDbContext.Users.FirstOrDefault(u => u.UserName == username);
            _roles = DbContext.Roles.Where(ru => ru.Users.Count(u => u.Id == _user.Id) > 0).ToList().Select(r => (ArfUserRole)Enum.Parse(typeof(ArfUserRole), r.Name)).ToArray();
        }

        public ArfUserRole[] Roles
        {
            get
            {
                if (_roles != null) return _roles;

                _roles = UserUtils.Roles;
                return _roles;
            }
        }

        public ArfUserRole[] ActualRoles
        {
            get
            {
                if (_roles != null) return _roles;

                if (CurrentUser == null) return new ArfUserRole[0];

                _roles = Utils.GetUserRoles(CurrentUser.Id);
                return _roles;
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

        public Privilages ActualPrivilages
        {
            get
            {
                if (_privilages != null) return _privilages.Value;

                _privilages = Utils.GetActualUserPrivilages(CurrentUser.Id);

                return _privilages.Value;
            }
        }

        public bool IsAdmin
        {
            get
            {
                if (_isAdmin != null) return _isAdmin.Value;
                _isAdmin = Utils.IsAdmin(Roles);
                return _isAdmin.Value;
            }
        }

        public bool IsCollegeUser
        {
            get
            {
                return CurrentUser.CollegeId != null;
            }
        }

        public bool IsGroupRole
        {
            get
            {
                return CurrentUser.CollegeId != null;
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

        protected List<CollegeViewModel> GetDepartments(bool allowOtherDepartments = false)
        {
            return Utils.GetDepartments(allowOtherDepartments).Select(p => new CollegeViewModel
            {
                Id = p.Id,
                Name = p.Name
            }).OrderBy(p => p.NameArabic).ToList();
        }

        public ActionResult RedirectToLastIndexPage(string action, string controller)
        {
            if (UserSession != null && !string.IsNullOrWhiteSpace(UserSession.LastIndexUrl))
            {
                return Redirect(UserSession.LastIndexUrl);
            }
            else
            {
                return RedirectToAction(action, controller);
            }
        }

        protected List<CollegeViewModel> GetColleges()
        {
            return Utils.GetColleges().Select(j => new CollegeViewModel
            {
                Id = j.Id,
                Name = j.Name,
                RoleGroupCode = j.Code
            }).ToList();
        }

        protected List<MajorViewModel> GetMajors()
        {
            return Utils.GetMajors().Select(j => new MajorViewModel
            {
                Id = j.Id,
                Name = j.Name
            }).ToList();
        }

        protected List<RoleViewModel> GetRoles(bool GetVicePresidentForHigherStudiesAndScientificResearchSubRoles = false)
        {
            var roles = new List<RoleViewModel>();

            roles = DbContext.Roles.Select(r => new RoleViewModel { RoleId = r.Id, RoleName = r.Name }).ToList();

            var ViceDeanForHigherStudiesAndScientificResearchRole = roles.Where(r => r.RoleName == "ViceDeanForHigherStudiesAndScientificResearch").FirstOrDefault();
            string ViceDeanForHigherStudiesAndScientificResearchRoleId = string.Empty;
            if (ViceDeanForHigherStudiesAndScientificResearchRole != null)
            {
                ViceDeanForHigherStudiesAndScientificResearchRoleId = ViceDeanForHigherStudiesAndScientificResearchRole.RoleId;
            }

            roles = roles
                        .Where(r =>
                               r.RoleName == "PrincipalResearcher"
                            || r.RoleName == "ViceDeanForHigherStudiesAndScientificResearch"
                            || r.RoleName == "ResearchUnitMember"
                            || r.RoleName == "DeanofDeanshipofScientificResearch"
                            || r.RoleName == "ViceDeanshipofResearchcCentersAndDatabases"
                            || r.RoleName == "SecretaryOfTheDeanshipOfResearchCentersAndDatabases"
                            || r.RoleName == "DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers"
                            || r.RoleName == "SecretaryOfTheStandingCommitteeForFinancingResearchProjects"
                            || r.RoleName == "MembersOfTheStandingCommitteeForFinancingResearchProjects"
                            || r.RoleName == "Referee"
                            || r.RoleName == "Designer"
                            || r.RoleName == "Accountant"
                            || r.RoleName == "DarWebsiteManager"
                            || r.RoleName == "CollegeDean"
                            || r.RoleName == "VicePresidentForHigherStudiesAndScientificResearch"
                            || r.RoleName == "UniversityRector"
                            || r.RoleName == "DirectorGeneralOfAdministrationAndFinance"
                        )
                        .Select(r => new RoleViewModel { RoleId = r.RoleId, RoleName = LangHelpers.Instance[r.RoleName], VirtualRoleCode = 0 })
                        .ToList();

            if (GetVicePresidentForHigherStudiesAndScientificResearchSubRoles)
            {
                roles.Add(new RoleViewModel { RoleId = ViceDeanForHigherStudiesAndScientificResearchRoleId, VirtualRoleCode = 1, RoleName = WebUtils.GetViceDeanForHigherStudiesAndScientificResearchSubRoleName(1, "ViceDeanForHigherStudiesAndScientificResearch") });
                roles.Add(new RoleViewModel { RoleId = ViceDeanForHigherStudiesAndScientificResearchRoleId, VirtualRoleCode = 2, RoleName = WebUtils.GetViceDeanForHigherStudiesAndScientificResearchSubRoleName(2, "ViceDeanForHigherStudiesAndScientificResearch") });
            }
            return roles.OrderBy(r => r.RoleName).ToList();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = CurrentUser;

            if (user == null && !string.IsNullOrWhiteSpace(filterContext.HttpContext.Request["UserName"]) && filterContext.ActionDescriptor != null && filterContext.ActionDescriptor.ControllerDescriptor != null && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Account")
            {
                var controller = new AccountController();
                var model = new LoginViewModel
                {
                    UserName = filterContext.HttpContext.Request["UserName"],
                    Password = filterContext.HttpContext.Request["Password"],
                    RememberMe = filterContext.HttpContext.Request["RememberMe"] == "true",
                };

                controller.TempData = filterContext.Controller.ControllerContext.Controller.TempData;

                var result = controller.Login(model, null);
                filterContext.Result = result;
            }
            else if (user != null)
            {
                if (filterContext != null && filterContext.ActionDescriptor != null && filterContext.ActionDescriptor.ControllerDescriptor != null && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Account")
                {
                    if (!HttpContext.Request.IsAjaxRequest())
                    {
                        if (!ValidateProfile())
                        {
                            TempData["message"] = LangHelpers.Instance[user.ForcePasswordChange ? "please_complete_your_profile_and_change_password" : "please_complete_your_profile"];
                            TempData["message_type"] = "danger";

                            filterContext.Result = RedirectToAction("Profile", "Account");
                        }
                        else if (user.ForcePasswordChange)
                        {
                            TempData["message"] = LangHelpers.Instance["you_must_change_password"];
                            TempData["message_type"] = "danger";

                            filterContext.Result = RedirectToAction("Profile", "Account");
                        }
                    }
                }
            }


            base.OnActionExecuting(filterContext);
        }

        private static Regex _rx_national_id = new Regex(@"^([1-2][0-9]{9})$", RegexOptions.Compiled);
        private static Regex _rx_national_id_start = new Regex(@"^[1-2]\d{0,9}$", RegexOptions.Compiled);

        public static bool IsValidNationalId(string nin)
        {
            if (string.IsNullOrWhiteSpace(nin)) return false;
            nin = nin.Replace(" ", "");

            var newNin = nin;
            if (nin.Split('-').Length == 2)
            {
                newNin = nin.Split('-')[1];
            }

            var isValidSaudiId = _rx_national_id.Match(newNin).Success && Utils.CalculateLuhnAlgorithm(newNin);

            if (!isValidSaudiId && nin.StartsWith("GCC-"))
            {
                long value = 0;
                return long.TryParse(newNin, out value);
            }

            return isValidSaudiId;
        }

        public static bool IsValidExtensionForFileName(string fileName)
        {
            return AbstractManager.IsValidExtensionForFileName(fileName);
        }

        public static string[] ValidFileExtensions
        {
            get
            {
                return AbstractManager.ValidFileExtensions;
            }
        }

        public static bool IsValidNationalIdStart(string nin)
        {
            if (string.IsNullOrWhiteSpace(nin)) return false;
            nin = nin.Replace(" ", "");

            return _rx_national_id_start.Match(nin).Success;
        }

        public T GetInstance<T>() where T : new()
        {
            if (Utils.HasOwinContext)
            {
                var instance = HttpContext.GetOwinContext().Get<T>(typeof(T).FullName);
                if (instance == null)
                {
                    instance = new T();
                    HttpContext.GetOwinContext().Set<T>(typeof(T).FullName, instance);
                }

                return instance;
            }

            return new T();
        }

        public RequestsManager RequestUtils
        {
            get
            {
                return GetInstance<RequestsManager>();
            }
        }

        public ExportManager ExportUtils
        {
            get
            {
                return GetInstance<ExportManager>();
            }
        }

        public TemplatesManager TemplateUtils
        {
            get
            {
                return GetInstance<TemplatesManager>();
            }
        }

        public CommonManager CommonUtils
        {
            get
            {
                return GetInstance<CommonManager>();
            }
        }

        public UsersManager UserUtils
        {
            get
            {
                return GetInstance<UsersManager>();
            }
        }

        public Department GetDepartment(long? did = null)
        {
            var provider = (Department)null;

            if (!IsCollegeUser)
            {
                if (did != null && did > 0)
                    provider = DbContext.Departments.SingleOrDefault(p => p.Id == did.Value);
            }
            else
            {
                provider = DbContext.Departments.SingleOrDefault(p => p.Id == CurrentUser.CollegeId);
            }

            return provider;
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new CustomJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        public SmsProvider PreferredSmsProvider
        {
            get
            {
                return Utils.PreferredSmsProvider;
            }
        }

        public bool GenerateAndSendVerificationCode(CodeType type, string to, out string err, out string key)
        {
            key = (string)null;
            err = (string)null;

            var db = Utils.db;

            var email = (string)null;
            var mobile = (string)null;

            if (string.IsNullOrWhiteSpace(to))
            {
                err = "MISSING_ADDRESS";
                return false;
            }

            to = to.Trim().ToLower();

            if (type == CodeType.Email)
            {
                email = to;
            }
            else if (type == CodeType.SMS)
            {
                mobile = to;
            }

            try
            {
                var validMobileNumber = (string)null;

                if ((type == CodeType.SMS) && (string.IsNullOrWhiteSpace(mobile) || !Utils.ValidateMobile(mobile, out validMobileNumber)))
                {
                    err = "INVALID_MOBILE";
                    return false;
                }
                else if ((type == CodeType.Email) && (string.IsNullOrWhiteSpace(email) || !Utils.ValidateEmail(email)))
                {
                    err = "invalid_email";
                    return false;
                }

                var uniqueHash = Guid.NewGuid() + TM.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + to;

                var tempUrl = null as UserTemporaryUrl;

                var strHash = Convert.ToBase64String(SHA256CryptoServiceProvider.Create().ComputeHash(Encoding.ASCII.GetBytes(uniqueHash))).Replace("+", "").Replace("/", "").Replace("\\", "");

                key = strHash;

                tempUrl = new UserTemporaryUrl
                {
                    Key = strHash,
                    IsUsed = false,
                    UserId = null,
                    SessionId = UserSession.Id,
                    Type = (byte)(type == CodeType.SMS ? TempUrlType.SmsActivationNoUser : TempUrlType.EmailActivationNoUser),
                    Expires = TM.Now.AddHours(24),
                    ShortCode = Utils.GenerateShortCode(),
                    Generated = TM.Now,
                    GeneratedBy = UserId,
                };

                db.UserTemporaryUrls.Add(tempUrl);
                db.SaveChanges();

                if (type == CodeType.SMS)
                {
                    var smsMessage = TemplateUtils[TemplateKey.SmsActivationNoUser];

                    var msg = !Utils.IsUserLanguageArabic ? smsMessage.BodyEnglish : smsMessage.BodyArabic;

                    msg = msg.Replace("{code}", tempUrl.ShortCode.ToString());

                    var isSent = SmsService.SendMessage(msg, new string[] { validMobileNumber }, out err);

                    if (!isSent)
                    {
                        err = "FAILED_TO_SEND_SMS_" + err;
                    }
                    else
                    {
                        UserSession.LastSmsCodeGenerated = TM.Now;
                        db.SaveChanges();
                    }

                    return isSent;
                }
                else
                {
                    var emailMessage = TemplateUtils[TemplateKey.EmailActivationNoUser];
                    var subject = Utils.IsUserLanguageArabic ? emailMessage.SubjectArabic : emailMessage.SubjectEnglish;

                    var msg = !Utils.IsUserLanguageArabic ? emailMessage.BodyEnglish : emailMessage.BodyArabic;

                    msg = msg.Replace("{code}", tempUrl.ShortCode.ToString());

                    var isSent = EmailService.SendMessage(SmtpSettings, subject, msg, new MailAddress(email), null, null);

                    if (!isSent)
                    {
                        err = "FAILED_TO_SEND_EMAIL_" + err;
                    }
                    else
                    {
                        UserSession.LastEmailCodeGenerated = TM.Now;
                        db.SaveChanges();
                    }

                    return isSent;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Logger.LogExceptionAsync(ex);
                err = ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsync(ex);

                err = ex.SanitizeException();

                return false;
            }
        }

        public bool CreateTemporaryLink(TempUrlType type, string userId, string email, string mobile, out string err, out string key)
        {
            key = (string)null;
            err = (string)null;

            var db = Utils.db;
            var user = (User)null;

            if (string.IsNullOrWhiteSpace(email))
                email = null;
            else
                email = email.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(mobile))
                mobile = null;

            try
            {
                user = db.Users.FirstOrDefault(u => (userId != null && u.Id == userId) || (email != null && u.Email == email));

                if (user != null)
                {
                    if (string.IsNullOrWhiteSpace(email))
                        email = user.Email;
                    if (string.IsNullOrWhiteSpace(mobile))
                        mobile = user.PhoneNumber;
                }

                var validMobileNumber = (string)null;

                if ((type == TempUrlType.SmsActivation || type == TempUrlType.RefereeAccountActivation) && (string.IsNullOrWhiteSpace(mobile) || !Utils.ValidateMobile(mobile, out validMobileNumber)))
                {
                    err = "INVALID_MOBILE";
                    return false;
                }
                else if ((type == TempUrlType.RefereeEmailActivation || type == TempUrlType.ResearcherEmailActivation) && (string.IsNullOrWhiteSpace(email) || !Utils.ValidateEmail(email)))
                {
                    err = "INVALID_EMAIL";
                    return false;
                }
                else if ((type == TempUrlType.RefereeEmailActivation || type == TempUrlType.ResearcherEmailActivation) && user == null)
                {
                    err = "USER_NOT_FOUND";
                    return false;
                }

                var uniqueHash = TM.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    uniqueHash += userId;
                }
                else if (!string.IsNullOrWhiteSpace(email))
                {
                    uniqueHash += email;
                }
                else if (!string.IsNullOrWhiteSpace(mobile))
                {
                    uniqueHash += mobile;
                }

                var tempUrl = null as UserTemporaryUrl;

                var strHash = Convert.ToBase64String(SHA256CryptoServiceProvider.Create().ComputeHash(Encoding.ASCII.GetBytes(uniqueHash))).Replace("+", "").Replace("/", "").Replace("\\", "");

                key = strHash;

                tempUrl = new UserTemporaryUrl
                {
                    Key = strHash,
                    IsUsed = false,
                    UserId = user == null ? null : user.Id,
                    Type = (byte)type,
                    Expires = TM.Now.AddHours(24),
                    ShortCode = Utils.GenerateShortCode(),
                    Generated = TM.Now,
                    GeneratedBy = CurrentUser == null ? Utils.SystemAdminUserId : CurrentUser.Id,
                };

                db.UserTemporaryUrls.Add(tempUrl);
                db.SaveChanges();

                if (type == TempUrlType.SmsActivation)
                {
                    var smsMessage = TemplateUtils[TemplateKey.NewUserSmsActivation];

                    var msg = !Utils.IsUserLanguageArabic ? smsMessage.BodyEnglish : smsMessage.BodyArabic;

                    msg = msg.Replace("{code}", tempUrl.ShortCode.ToString());

                    var isSent = SmsService.SendMessage(msg, new string[] { validMobileNumber }, out err);

                    if (!isSent)
                    {
                        err = "FAILED_TO_SEND_SMS_" + err;
                    }

                    return isSent;
                }
                else if (type == TempUrlType.WelcomeMessageResearcher)
                {
                    var emailMessage = TemplateUtils[TemplateKey.WelcomeMessageResearcher];

                    var subject = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(emailMessage.SubjectEnglish) ? emailMessage.SubjectArabic : emailMessage.SubjectEnglish;
                    var body = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(emailMessage.SubjectEnglish) ? emailMessage.BodyArabic : emailMessage.BodyEnglish;

                    var link = Utils.GetApplicationUrl() + "l/" + tempUrl.Key;
                    body = body
                            .Replace("{username}", user.UserName)
                            .Replace("{name}", user.UserProfile.GetFullName())
                            .Replace("{email}", user.Email)
                            .Replace("{link}", link);

                    var isSent = EmailService.SendMessage(SmtpSettings, subject, body, new MailAddress(user.Email, user.FullName), null, null);
                    if (!isSent)
                    {
                        err = "FAILED_TO_SEND_EMAIL";
                    }

                    return isSent;
                }
                else if (type == TempUrlType.ResearcherEmailActivation || type == TempUrlType.RefereeEmailActivation)
                {
                    var emailMessage = TemplateUtils[type == TempUrlType.ResearcherEmailActivation ? TemplateKey.NewResearcherEmailActivation : TemplateKey.NewRefereeEmailActivation];

                    var subject = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(emailMessage.SubjectEnglish) ? emailMessage.SubjectArabic : emailMessage.SubjectEnglish;
                    var body = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(emailMessage.SubjectEnglish) ? emailMessage.BodyArabic : emailMessage.BodyEnglish;

                    var link = Utils.GetApplicationUrl() + "l/" + tempUrl.Key;
                    body = body
                            .Replace("{username}", user.UserName)
                            .Replace("{name}", user.UserProfile.GetFullName())
                            .Replace("{email}", user.Email)
                            .Replace("{link}", link);

                    var isSent = EmailService.SendMessage(SmtpSettings, subject, body, new MailAddress(user.Email, user.FullName), null, null);
                    if (!isSent)
                    {
                        err = "FAILED_TO_SEND_EMAIL";
                    }

                    return isSent;
                }
                else if (type == TempUrlType.ResetPassword)
                {
                    var emailMessage = TemplateUtils[TemplateKey.ResetPassword];

                    var subject = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(emailMessage.SubjectEnglish) ? emailMessage.SubjectArabic : emailMessage.SubjectEnglish;
                    var body = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(emailMessage.SubjectEnglish) ? emailMessage.BodyArabic : emailMessage.BodyEnglish;

                    var link = Utils.GetApplicationUrl() + "l/" + tempUrl.Key;
                    body = body.Replace("{username}", user.UserName).Replace("{link}", link);

                    var isSent = EmailService.SendMessage(SmtpSettings, subject, body, new MailAddress(user.Email, user.FullName), null, null);
                    if (!isSent)
                    {
                        err = "FAILED_TO_SEND_EMAIL";
                    }

                    return isSent;
                }
                else if (type == TempUrlType.RefereeAccountActivation)
                {
                    var emailMessage = TemplateUtils[TemplateKey.RefereeAccountActivation];

                    var subject = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(emailMessage.SubjectEnglish) ? emailMessage.SubjectArabic : emailMessage.SubjectEnglish;
                    var body = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(emailMessage.SubjectEnglish) ? emailMessage.BodyArabic : emailMessage.BodyEnglish;

                    var link = Utils.GetApplicationUrl();
                    body = body
                            .Replace("{username}", user.UserName)
                            .Replace("{name}", user.UserProfile.GetFullName())
                            .Replace("{email}", user.Email)
                            .Replace("{link}", link);

                    var isSent = EmailService.SendMessage(SmtpSettings, subject, body, new MailAddress(user.Email, user.FullName), null, null);
                    if (!isSent)
                    {
                        err = "FAILED_TO_SEND_EMAIL";
                    }

                    var msg = Utils.MakeSMSFriendly(body);
                    isSent = SmsService.SendMessage(msg, new string[] { validMobileNumber }, out err);

                    if (!isSent)
                    {
                        err = "FAILED_TO_SEND_SMS_" + err;
                    }

                    return isSent;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Logger.LogExceptionAsync(ex);
                err = ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsync(ex);

                err = ex.SanitizeException();

                return false;
            }

            return true;
        }

        public bool SendRefereeNotification(RequestReferee rr, string key = null)
        {
            if (rr == null || !(rr.Id > 0)) return false;

            var email = rr.Referee.Email;
            var mobile = rr.Referee.MobileNo;

            if (!Utils.ValidateEmail(email))
                email = null;

            if (!Utils.ValidateMobile(mobile, out mobile))
                mobile = null;

            if (email == null && mobile == null) return false;

            var fullName = rr.Referee.GetFullName();
            var subject = (string)null;
            var body = (string)null;
            var smsBody = (string)null;

            var tempUrl = (UserTemporaryUrl)null;

            if (rr.Referee.UserId == null)
            {
                if ((string.IsNullOrWhiteSpace(key) || (tempUrl = DbContext.UserTemporaryUrls.OrderByDescending(tu => tu.Generated).FirstOrDefault(tu => tu.Key == key && tu.Expires > TM.Now && !tu.IsUsed)) == null))
                {
                    var err = "";
                    if (!CreateTemporaryLink(TempUrlType.RegisterReferee, null, email, mobile, out err, out key))
                        return false;

                    tempUrl = DbContext.UserTemporaryUrls.OrderByDescending(tu => tu.Generated).FirstOrDefault(tu => tu.Key == key && tu.Expires > TM.Now && !tu.IsUsed);

                    if (tempUrl == null)
                        return false;
                }

                var template = TemplateUtils[TemplateKey.RegisterReferee];

                subject = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(template.SubjectEnglish) ? template.SubjectArabic : template.SubjectEnglish;
                body = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(template.SubjectEnglish) ? template.BodyArabic : template.BodyEnglish;
                var link = Utils.GetApplicationUrl() + "l/" + tempUrl.Key;
                body = body.Replace("{name}", fullName).Replace("{link}", link);
                body = body.Replace("{title}", rr.Request.Title).Replace("{type}", LangHelpers.Instance["type_" + ((PublicationType)rr.Request.PublicationType).ToString()]);

                template = TemplateUtils[TemplateKey.RegisterRefereeSMS];

                smsBody = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(template.SubjectEnglish) ? template.BodyArabic : template.BodyEnglish;
                smsBody = smsBody.Replace("{name}", fullName).Replace("{link}", link);
                smsBody = smsBody.Replace("{title}", rr.Request.Title).Replace("{type}", LangHelpers.Instance["type_" + ((PublicationType)rr.Request.PublicationType).ToString()]);
            }
            else
            {
                var template = TemplateUtils[TemplateKey.InviteReferee];

                subject = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(template.SubjectEnglish) ? template.SubjectArabic : template.SubjectEnglish;
                body = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(template.SubjectEnglish) ? template.BodyArabic : template.BodyEnglish;

                var link = Utils.GetApplicationUrl();
                body = body.Replace("{name}", fullName).Replace("{link}", link);
                body = body.Replace("{title}", rr.Request.Title).Replace("{type}", LangHelpers.Instance["type_" + ((PublicationType)rr.Request.PublicationType).ToString()]);

                template = TemplateUtils[TemplateKey.InviteRefereeSMS];

                smsBody = !Utils.IsUserLanguageArabic || string.IsNullOrWhiteSpace(template.SubjectEnglish) ? template.BodyArabic : template.BodyEnglish;
                smsBody = smsBody.Replace("{name}", fullName).Replace("{link}", link);
                smsBody = smsBody.Replace("{title}", rr.Request.Title).Replace("{type}", LangHelpers.Instance["type_" + ((PublicationType)rr.Request.PublicationType).ToString()]);
            }

            var sent = false;

            if (email != null)
            {
                var isSent = EmailService.SendMessage(SmtpSettings, subject, body, new MailAddress(email, fullName), null, null);
                if (isSent)
                {
                    sent = true;
                }
            }

            if (mobile != null)
            {
                smsBody = Utils.StripHTML(smsBody);
                var isSent = SmsService.SendMessage(smsBody, mobile);
                if (isSent)
                {
                    sent = true;
                }
            }

            return sent;
        }

        public bool SendContactUsMessage(ContactUsViewModel contact)
        {
            if (contact == null || (contact.EmailAddress == null)) return false;
            var subject = (string)null;
            var body = (string)null;
            var emailId = contact.EmailAddress;
            var fullName = contact.FullName;
            var message = contact.Message;
            var subjectC = contact.Subject;
            var template = TemplateUtils[TemplateKey.ContactUsMessage];
            if (!Utils.ValidateEmail(emailId))
                emailId = null;
            if (emailId == null) return false;

            if (Utils.IsTextArabic(message))
            {
                subject = template.SubjectArabic;
                body = template.BodyArabic;
            }
            else
            {
                subject = template.SubjectEnglish;
                body = template.BodyEnglish;
            }
            subject = subject.Replace("{fullname}", fullName);
            body = body.Replace("{fullname}", fullName).Replace("{emailId}", emailId).Replace("{message}", message).Replace("{subject}", string.IsNullOrEmpty(subjectC) ? "" : subjectC);

            var sent = false;

            if (emailId != null)
            {
                try
                {
                    var err = "";
                    var isSent = EmailService.SendMessage(SmtpSettings, subject, body, new MailAddress[] { new MailAddress("info@srg.edu.sa", "SRG Info") }, null, null, new MailAddress(contact.EmailAddress, contact.FullName), null, out err);
                    if (isSent)
                    {
                        sent = true;
                    }
                }
                catch (Exception ex)
                {
                    sent = false;
                    Logger.LogExceptionAsync(ex, contact);
                }
            }
            return sent;
        }

        protected T GetKeyValue<T>(string key, List<UserSessionData> data)
        {
            var db = Utils.db;

            if (data == null)
            {
                data = db.UserSessionDatas.Where(x => x.SessionId == UserSession.Id).ToList();
            }

            var dv = db.UserSessionDatas.FirstOrDefault(x => x.SessionId == UserSession.Id && x.Key == key);
            if (dv != null)
            {
                if (dv.Type.StartsWith("System.Int32"))
                {
                    int a = 0;
                    if (int.TryParse(dv.Value, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else if (dv.Type.StartsWith("System.Nullable`1[[System.Int32"))
                {
                    int a = 0;
                    if (int.TryParse(dv.Value, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else if (dv.Type.StartsWith("System.Int64"))
                {
                    long a = 0;
                    if (long.TryParse(dv.Value, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else if (dv.Type.StartsWith("System.Nullable`1[[System.Int64"))
                {
                    long a = 0;
                    if (long.TryParse(dv.Value, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else if (dv.Type.StartsWith("System.Int16"))
                {
                    Int16 a = 0;
                    if (Int16.TryParse(dv.Value, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else if (dv.Type.StartsWith("System.Nullable`1[[System.Int16"))
                {
                    Int16 a = 0;
                    if (Int16.TryParse(dv.Value, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else if (dv.Type.StartsWith("System.Byte"))
                {
                    byte a = 0;
                    if (byte.TryParse(dv.Value, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else if (dv.Type.StartsWith("System.Nullable`1[[System.Byte"))
                {
                    byte a = 0;
                    if (byte.TryParse(dv.Value, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else if (dv.Type.StartsWith("System.DateTime"))
                {
                    DateTime a = default(DateTime);
                    if (DateTime.TryParseExact(dv.Value, "yyyy-MM-dd hh:mm:ss tt", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else if (dv.Type.StartsWith("System.Nullable`1[[System.DateTime"))
                {
                    DateTime a = default(DateTime);
                    if (DateTime.TryParseExact(dv.Value, "yyyy-MM-dd hh:mm:ss tt", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else if (dv.Type.StartsWith("ARF.Data.QualificationType"))
                {
                    QualificationType a = default(QualificationType);
                    if (Enum.TryParse(dv.Value, true, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else if (dv.Type.StartsWith("System.Nullable`1[[ARF.Data.QualificationType"))
                {
                    QualificationType a = default(QualificationType);
                    if (Enum.TryParse(dv.Value, true, out a))
                    {
                        return (T)((object)a);
                    }

                    return default(T);
                }
                else
                {
                    return (T)((object)dv.Value);
                }
            }

            return default(T);
        }

        protected void SaveKeyValue<T>(string key, T valueObj, List<UserSessionData> data)
        {
            var db = Utils.db;

            if (data == null)
            {
                data = db.UserSessionDatas.Where(x => x.SessionId == UserSession.Id).ToList();
            }

            if (key != null)
            {
                var value = valueObj == null ? null : valueObj.SpecialToString();
                var type = typeof(T).FullName;
                var dv = data.FirstOrDefault(k => k.Key == key && k.Type == type);
                if (dv == null)
                {
                    db.UserSessionDatas.Add(new UserSessionData
                    {
                        Key = key,
                        Value = value,
                        Type = type,
                        SessionId = UserSession.Id,
                    });
                }
                else
                {
                    dv.Value = value;
                }

                db.SaveChanges();
            }
        }
    }

    public class CustomJsonResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;

            try
            {
                response.ContentType = !string.IsNullOrEmpty(this.ContentType) ? this.ContentType : "application/json";

                if (this.ContentEncoding != null)
                {
                    response.ContentEncoding = this.ContentEncoding;
                }

                if (this.Data == null)
                {
                    return;
                }

                response.Write(JsonConvert.SerializeObject(this.Data, Settings));
            }
            catch
            {
            }
        }
    }
}