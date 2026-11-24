using ARF.Auth;
using ARF.Core;
using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using ARF.Web.App_Start.Code;
using ARF.Web.Models;
using EntityFramework.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using PhoneNumbers;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [CustomAuthorize(Privilages.account_login, Privilages.account_profile)]
    public class AccountController : BaseController
    {
        public AccountController()
        {
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult Index()
        {
            return RedirectToAction("Profile");
        }

        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public ActionResult RequestCoResearcherPublicLink(string _h)
        {
            if (!String.IsNullOrEmpty(_h))
            {
                string _publicHashLink = Utils.Decrypt(_h);
                if (!String.IsNullOrEmpty(_publicHashLink))
                {
                    string[] _publicHashLinkParts = _publicHashLink.Split(Core.Constants.ExternalRefereeParamsConnector);
                    if (_publicHashLinkParts.Count() >= 3)
                    {
                        int RequestCoResearcherResponseId = -1;
                        Int64 requestId = -1;
                        string email = String.Empty;
                        try
                        {
                            requestId = Convert.ToInt64(_publicHashLinkParts[0]);
                            RequestCoResearcherResponseId = Convert.ToInt32(_publicHashLinkParts[1]);
                            email = _publicHashLinkParts[2];
                        }
                        catch (Exception ex) { }

                        if (requestId != -1 && RequestCoResearcherResponseId != -1 && !String.IsNullOrEmpty(email))
                        {
                            var request = Utils.db.Requests.Where(r => (r.Id == requestId) && r.IsDeleted == false).FirstOrDefault();
                            if (request != null)
                            {
                                var response = Utils.db.RequestCoresearcherResponses.Find(RequestCoResearcherResponseId);
                                if (response != null)
                                {
                                    var coResearcher = Utils.db.RequestCoresearchers.Include("Researcher").Where(x => x.Id == response.RequestCoresearcherId && x.RequestId == requestId && x.IsDeleted == false).FirstOrDefault();
                                    if (coResearcher != null)
                                    {
                                        var coResearcherResponse = response;
                                        if (coResearcherResponse != null && coResearcherResponse.SubmittedOn == null)
                                        {
                                            if (coResearcherResponse.ReceivedOn == null)
                                            {
                                                coResearcherResponse.ReceivedOn = TM.Now;
                                                Utils.db.Entry(coResearcherResponse).State = EntityState.Modified;
                                                Utils.db.SaveChanges();
                                            }
                                            string welcomeMessage = String.Empty;
                                            if (request.Type == (byte)RequestType.EditFundingRequest)
                                            {
                                                welcomeMessage = LangHelpers.Instance["coresearcher_request_update_welcome_page_message_edit_request_page"];
                                            }
                                            else
                                            {
                                                welcomeMessage = LangHelpers.Instance["coresearcher_request_update_welcome_page_message"];
                                            }
                                            welcomeMessage = welcomeMessage.Replace("{FullName}", coResearcher.Researcher.GetFullName()).Replace("{RequestTitle}", request.Title);

                                            ViewBag.RequestId = requestId;
                                            ViewBag.RequestCoResearcherResponseId = RequestCoResearcherResponseId;
                                            ViewBag.ParamsHash = _h;
                                            ViewBag.FullName = coResearcher.Researcher.GetFullName();
                                            ViewBag.WelcomeMessage = welcomeMessage;
                                            ViewBag.FieldType = request.FieldType;
                                            return View("~/Views/Requests/CoResearcherRequestUpdateResponsePage.cshtml");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult ExternalReferee(string _h)
        {
            if (!String.IsNullOrEmpty(_h))
            {
                string _publicHashLink = Utils.Decrypt(_h);
                if (!String.IsNullOrEmpty(_publicHashLink))
                {
                    string[] _publicHashLinkParts = _publicHashLink.Split(Core.Constants.ExternalRefereeParamsConnector);
                    if (_publicHashLinkParts.Count() >= 3)
                    {
                        int refereeId = -1;
                        Int64 requestId = -1;
                        string email = String.Empty;
                        try
                        {
                            requestId = Convert.ToInt64(_publicHashLinkParts[0]);
                            refereeId = Convert.ToInt32(_publicHashLinkParts[1]);
                            email = _publicHashLinkParts[2];
                        }
                        catch (Exception ex) { }

                        if (requestId != -1 && refereeId != -1 && !String.IsNullOrEmpty(email))
                        {
                            var request = Utils.db.Requests.Where(r => (r.Id == requestId) && r.IsDeleted == false).FirstOrDefault();
                            if (request != null)
                            {
                                var referee = Utils.db.RequestReferees.Include("Referee").Where(r => r.RequestId == requestId && r.RefereeId == refereeId).FirstOrDefault();
                                if (referee != null)
                                {
                                    int externalRefereeResponseTimeDays = (new RefereeUtil()).GetExtrnalRefereeResponseTimeDays();
                                    int totalHrs = externalRefereeResponseTimeDays * 24;
                                    if (TM.Now.Subtract(referee.RequestReviewSentOn.Value).TotalHours < totalHrs)
                                    {
                                        string welcomeMessage = LangHelpers.Instance["external_referee_welcome_page_message"];
                                        welcomeMessage = welcomeMessage.Replace("{FullName}", referee.Referee.GetFullName());

                                        ViewBag.RequestId = requestId;
                                        ViewBag.RefereeId = refereeId;
                                        ViewBag.Email = email;
                                        ViewBag.ParamsHash = _h;
                                        ViewBag.FullName = referee.Referee.GetFullName();
                                        ViewBag.WelcomeMessage = welcomeMessage;
                                        if (email.EndsWith("@imamu.edu.sa"))
                                        {
                                            ViewBag.RoleCode = 1;
                                        }
                                        else
                                        {
                                            ViewBag.RoleCode = 5;
                                        }
                                        if (referee.Action == null)
                                        {
                                            return View("~/Views/Requests/ExternalRefereeView.cshtml");
                                        }
                                        else if (referee.Action == (byte)RequestAction.AcceptToReview && (referee.Referee != null && referee.Referee.UserId == null))
                                        {
                                            var session = UserSession;
                                            var db = Utils.db;
                                            var data = db.UserSessionDatas.Where(x => x.SessionId == UserSession.Id).ToList();
                                            SaveKeyValue<string>("Email", referee.Referee.Email, data);
                                            SaveKeyValue<string>("ExternalRefereeHashParams", _h, data);

                                            return RedirectToAction("Register", new { role = ViewBag.RoleCode });
                                        }
                                    }
                                    else
                                    {
                                        TempData["message"] = LangHelpers.Instance["external_referee_welcome_page_link_expire"];
                                        TempData["message_type"] = "warning";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToLocal(returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View("~/Views/Account/Login.cshtml");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
           
            if (Request.IsAuthenticated)
            {
                return RedirectToLocal(returnUrl);
            }
            if (model == null || string.IsNullOrWhiteSpace(model.UserName) || ModelState == null || !ModelState.IsValid)
            {
                return View("~/Views/Account/Login.cshtml", model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true

            model.UserName = model.UserName.Trim().ToLower();
            var user = (ApplicationUser)null;
            if (model.UserName.Contains("@"))
            {
                user = UserManager.FindByEmail(model.UserName);
            }

            if (user == null)
            {
                user = UserManager.FindByName(model.UserName);
            }

            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();
            if (Utils.IsLocalhost == false)
            {
                if (String.IsNullOrEmpty(recaptchaHelper.Response))
                {
                    //ModelState.AddModelError("", LangHelpers.Instance["robot_required"]);

                    TempData["message"] = LangHelpers.Instance["robot_required"];
                    TempData["message_type"] = "warning";
                    return View("~/Views/Account/Login.cshtml", model);
                }
                else
                {
                    RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
                    if (recaptchaResult != RecaptchaVerificationResult.Success)
                    {
                        //ModelState.AddModelError("", "Incorrect captcha answer.");

                        TempData["message"] = LangHelpers.Instance["Incorrect captcha answer."];
                        TempData["message_type"] = "warning";
                        return View("~/Views/Account/Login.cshtml", model);
                    }
                    else
                    {
                        ViewBag.VerifyCaptcha = false;
                    }

                }


            }
            if (user != null && user.IsDeleted)
            {
                TempData["message"] = Resources.ArfStrings.this_account_is_deleted;
                TempData["message_type"] = "warning";
                return View("~/Views/Account/Login.cshtml", model);
            }
            else if (user != null && user.IsActivated != true)
            {
                TempData["message"] = Resources.ArfStrings.this_account_is_not_activated_yet;
                TempData["message_type"] = "warning";
                return View("~/Views/Account/Login.cshtml", model);
            }
            else if (user != null && user.EmailConfirmed != true)
            {
                TempData["message"] = Resources.ArfStrings.email_not_activated_please_click + " <a href=\"" + Utils.GetApplicationUrl() + "Account/ResendEmailActivation?username=" + HttpUtility.UrlEncode(user.UserName) + "\">" + Resources.ArfStrings.here + "</a>" + Resources.ArfStrings.resending_activation_message_for_email;
                TempData["message_type"] = "warning";
                return View("~/Views/Account/Login.cshtml", model);
            }
            else if (user != null && user.PhoneNumberConfirmed != true)
            {
                TempData["message"] = Resources.ArfStrings.mobile_number_not_activated_please_click + "<a href=\"" + Utils.GetApplicationUrl() + "Account/SendMobileActivation?username=" + HttpUtility.UrlEncode(user.UserName) + "\">" + Resources.ArfStrings.here + "</a>" + Resources.ArfStrings.resending_activation_message_for_mobile_number;
                TempData["message_type"] = "warning";

                return View("~/Views/Account/Login.cshtml", model);
            }

            var isSuccess = !string.IsNullOrWhiteSpace(model.Password) && UserManager.CheckPassword(user, model.Password);

            if (!isSuccess && !string.IsNullOrWhiteSpace(model.Password))
            {
                model.Password = model.Password.Trim();
                isSuccess = UserManager.CheckPassword(user, model.Password);
            }

            if (Utils.IsLocalhost)
            {
                if (user != null)
                {
                    isSuccess = true;
                }
            }

            if (isSuccess)
            {
                user.AccessFailedCount = 0;
                user.LockoutEnabled = false;
                user.LockoutEndDateUtc = null;

                AuthDbContext.SaveChanges();

                var user2 = DbContext.Users.Include("Roles").Include("Researchers").SingleOrDefault(u => u.Id == user.Id);

                var roles = user2.Roles.ToArray();

                if (!user.IsActivated)
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    ModelState.AddModelError("", "هذا الحساب معطل");
                    return View("~/Views/Account/Login.cshtml", model);
                }
                if (Session != null)
                {
                    Session.Abandon();
                }
                if (Response != null)
                {
                    Response.Cookies.Add(new HttpCookie("id", Guid.NewGuid().ToString()));
                }

                var identity = SignInManager.CreateUserIdentity(user);

                var sessionId = Guid.NewGuid().ToString();

                var cr = identity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

                if (user2.Researchers != null && user2.Researchers.Count > 0)
                {
                    identity.AddClaim(new Claim("RESEARCHER_ID", user2.Researchers.First().Id.ToString()));
                }

                identity.AddClaim(new Claim("SESSION_ID", sessionId));

                AuthenticationManager.SignIn(identity);

                if (!Utils.IsLocalhost)
                {
                    var userSessions = DbContext.UserSessions.Where(s => s.UserId == user.Id && s.Lockdate == null).ToList();

                    foreach (var s in userSessions)
                    {
                        s.Locked = true;
                        s.Lockdate = TM.Now;
                    }
                }

                var session = new UserSession
                {
                    UserId = user.Id,
                    IpAddress = Utils.GetUserIP(),
                    Expires = model.RememberMe ? TM.Now.AddDays(7) : TM.Now.AddHours(3),
                    Locked = false,
                    Sessionid = sessionId,
                    Updated = TM.Now,
                    Created = TM.Now,
                };

                DbContext.UserSessions.Add(session);
                DbContext.SaveChanges();

                var currentUrl = Utils.GetFullUrl();

                if (currentUrl.ToLower().EndsWith("account/login") || currentUrl.ToLower().EndsWith("account/logoff"))
                    return RedirectToAction("Index", "Home");

                return Redirect(currentUrl);
            }
            else
            {
                if (user != null)
                {
                    user.AccessFailedCount++;

                    //if (user.AccessFailedCount >= 3)
                    //{
                    //    user.LockoutEnabled = true;
                    //  user.LockoutEndDateUtc = DateTime.UtcNow.AddHours(1);
                    //}

                    AuthDbContext.SaveChanges();

                    //if (user.AccessFailedCount >= 3)
                    //{
                    //  ModelState.AddModelError("", Resources.ArfStrings.sorry_this_account_is_deactivated_for + " " + Math.Round((user.LockoutEndDateUtc.Value - DateTime.UtcNow).TotalMinutes).ToString() + " " + Resources.ArfStrings.minute );
                    //}
                    //else
                    //{
                    ModelState.AddModelError("", string.Format(Resources.ArfStrings.entry_informations_is_incorrect_please_register_first_by_clicking_new_user_register, 3 - user.AccessFailedCount));
                    //}
                }
                else
                {
                    ModelState.AddModelError("", Resources.ArfStrings.entry_informations_is_incorrect_please_register_first_by_clicking_new_user_register);
                }

                return View("~/Views/Account/Login.cshtml", model);
            }
        }

        public new ActionResult Profile()
        {
            if (CurrentUser == null) return RedirectToAction("LogOff");

            if (UserUtils.IsImpersonating)
            {
                TempData["message"] = Resources.ArfStrings.You_can_not_edit_your_profile_without_going_back_to_your_baseline;
                TempData["message_type"] = "warning";
                return RedirectToAction("Index", "Home");
            }

            User user = DbContext.Users
                .Include("UserProfile")
                .Include("Department")
                .Include("College")
                .Include("Roles")
                .Include("Researchers")
                .Where(u => u.Id == CurrentUser.Id)
                .FirstOrDefault();

            if (user == null)
            {
                return HttpNotFound();
            }
            var model = new EditUserViewModel
            {
                FullName = user.UserProfile == null ? user.UserName : user.UserProfile.GetFullName(),
                NationalId = user.UserProfile != null ? user.UserProfile.NationalId : "",
                UserId = user.Id,
                Password = "**********",
                ConfirmPassword = "**********",
                Email = user.Email,
                MobileNo = user.UserProfile == null ? "" : user.UserProfile.MobileNo,
                UserName = user.UserName,
                CollegeId = user.CollegeId ?? 0,
                CollegeName = user.CollegeId == null ? null : (IsArabic ? user.College.NameArabic : user.College.NameEnglish),
                RoleId = user.Roles.Count() == 0 ? null : user.Roles.FirstOrDefault().Id,
                RoleName = LangHelpers.Instance[user.Roles.Count() == 0 ? null : user.Roles.FirstOrDefault().Name],
                IsActivated = user.IsActivated,
                NationalityId = user.UserProfile.NationalityId,
                BirthDate = user.UserProfile.BirthDate == null ? null : user.UserProfile.BirthDate.Value.FormatDate(false, false, false),
                CountryId = user.CountryId ?? 0,
                CityId = user.UserProfile.CityId ?? 0,
                Address = user.Address ?? "",
                UniversityId = user.UniversityId ?? 0,
                UniversityName = user.University.LocalizedName(),
                UniversityOther = user.UniversityOther ?? "",
                CollegeOther = user.CollegeOther ?? "",
                DepartmentID = user.DepartmentId ?? 0,
                Bank = user.UserProfile.BankId ?? 0,
                BankBranch = user.UserProfile.BankBranch ?? "",
                BankAccountHolderName = user.UserProfile.BankAccountHolder ?? "",
                BankAccountIBAN = user.UserProfile.BankIban ?? "",
                BankOther = user.UserProfile.BankName ?? "",
                SectionPublishedPapers = user.UserProfile.CvSectionPublishedPapers == null ? "" : Regex.Replace(user.UserProfile.CvSectionPublishedPapers, "<.*?>", String.Empty),
                SectionInterests = user.UserProfile.CvSectionInterests == null ? "" : Regex.Replace(user.UserProfile.CvSectionInterests, "<.*?>", String.Empty),
                SectionCareerHistory = user.UserProfile.CvSectionCareerHistory == null ? "" : Regex.Replace(user.UserProfile.CvSectionCareerHistory, "<.*?>", String.Empty),
                SectionTrainingCourses = user.UserProfile.CvSectionTrainingCourses == null ? "" : Regex.Replace(user.UserProfile.CvSectionTrainingCourses, "<.*?>", String.Empty),
                SectionAwards = user.UserProfile.CvSectionAwards == null ? "" : Regex.Replace(user.UserProfile.CvSectionAwards, "<.*?>", String.Empty),
                SectionMemberships = user.UserProfile.CvSectionMemberships == null ? "" : Regex.Replace(user.UserProfile.CvSectionMemberships, "<.*?>", String.Empty),
                SectionAcademicHistory = user.UserProfile.CvSectionAcademicHistory == null ? "" : Regex.Replace(user.UserProfile.CvSectionAcademicHistory, "<.*?>", String.Empty)
            };
            if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher))
            {
                var researcher = user.Researchers.Where(x => x.UserId == user.Id).FirstOrDefault();
                model.NationalId = researcher.NationalId;
                model.UniversityId = researcher.UniversityId;
                model.UniversityOther = researcher.UniversityOther;
                model.CollegeId = researcher.CollegeId;
                model.CollegeOther = researcher.CollegeOther;
                model.DepartmentID = researcher.DepartmentId;
                model.DepartmentOther = researcher.DepartmentOther ?? "";

                if (researcher.Qualification != null)
                {
                    model.Qualification = (QualificationType)researcher.Qualification > 0 ? (QualificationType)researcher.Qualification : QualificationType.Unspecified;
                }
                model.Specialization = researcher.Specialization ?? "";
                model.CountryId = researcher.CountryId ?? 0;
                model.CityId = researcher.CityId ?? 0;
                model.BirthDate = researcher.DateOfBirth == null ? null : researcher.DateOfBirth.Value.FormatDate(false, false, false);
                model.NationalityId = researcher.NationalityId ?? 0;
                model.BankIbanPhotocopyReadonly = user.UserFiles
                                                .Where(f => f.Type == (byte)FileType.IbanPhotocopy)
                                                .OrderByDescending(f => f.Created)
                                                .Select(c => new AttachedFile
                                                {
                                                    FileId = c.Id.ToString(),
                                                    Type = (FileType)c.Type,
                                                    FileName = Path.GetFileName(c.FileName)
                                                })
                                                .FirstOrDefault();

                model.UniversityId = 1;
            }
            else if (Utils.HasRolesAny(ArfUserRole.Referee))
            {
                var referee = user.Referees.Where(x => x.UserId == user.Id).FirstOrDefault();
                if (referee.Qualification != null)
                {
                    model.Qualification = (QualificationType)referee.Qualification > 0 ? (QualificationType)referee.Qualification : QualificationType.Unspecified;
                }
            }
            FillOutLookups(model);
            ViewBag.IncludeCKEditor = true;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(EditUserViewModel model)
        {
            if (CurrentUser == null)
                return RedirectToAction("Index", "Home");

            int? universityId = model.UniversityId;
            if (UserUtils.IsImpersonating)
            {
                TempData["message"] = Resources.ArfStrings.You_can_not_edit_your_profile_without_going_back_to_your_baseline;
                TempData["message_type"] = "warning";
                return RedirectToAction("Index", "Home");
            }

            var userId = CurrentUser.Id;
            var user = (User)null;

            if (!string.IsNullOrWhiteSpace(model.UserId))
            {
                user = DbContext.Users
                        .Include("Roles")
                        .Include("UserProfile")
                        .Include("College")
                        .SingleOrDefault(u => u.Id == model.UserId);
                if (user == null)
                {
                    ModelState.AddModelError("UserId", Resources.ArfStrings.this_user_does_not_exist);
                }
            }
            if (model.UserName != user.UserName)
            {
                var isExists = DbContext.Users.Where(x => x.UserName == model.UserName).Count() > 0;
                if (isExists)
                {
                    ModelState.AddModelError("UserName", LangHelpers.Instance["username_must_be_unique"]);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != "**********" && model.Password == model.ConfirmPassword)
            {
                if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", Resources.ArfStrings.you_must_enter_your_current_password);
                }
                else if (!UserManager.CheckPassword(CurrentUser, model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", Resources.ArfStrings.current_password_does_not_match_registered);
                }
                else
                {
                    var errorMsg = (string)null;
                    if (!WebUtils.ValidatePassword(model.Password, IsAdmin, out errorMsg))
                    {
                        ModelState.AddModelError("Password", errorMsg);
                    }
                }
            }

            var birthDate = DateTime.MinValue;

            model.CollegeName = user.CollegeId == null ? null : user.College.LocalizedName();
            model.RoleId = user.Roles.Count() == 0 ? null : user.Roles.FirstOrDefault().Id;
            model.RoleName = LangHelpers.Instance[user.Roles.Count() == 0 ? null : user.Roles.FirstOrDefault().Name];

            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                ModelState.AddModelError("FullName", LangHelpers.Instance["fullname_required"]);
            }
            else
            {
                var comps = Utils.GetNameComponents(model.FullName);

                if (comps.FirstName == null || comps.LastName == null || comps.ThirdName == null || comps.SecondName == null)
                {
                    ModelState.AddModelError("FullName", LangHelpers.Instance["quadname_required"]);
                }
            }

            var number = (PhoneNumber)null;
            var lang = (string)null;

            if (!string.IsNullOrWhiteSpace(model.MobileNo))
            {
                var util = PhoneNumberUtil.GetInstance();

                try
                {
                    number = util.Parse(model.MobileNo, "SA");
                }
                catch (NumberParseException)
                {
                    ModelState.AddModelError("MobileNo", LangHelpers.Instance["invalid_mobile_number"]);
                }
            }
            else
            {
                ModelState.AddModelError("MobileNo", LangHelpers.Instance["mobile_number_required"]);
            }

            if (!string.IsNullOrWhiteSpace(model.NationalId))
            {
                if (!IsValidNationalId(model.NationalId))
                {
                    ModelState.AddModelError("NationalId", LangHelpers.Instance["invalid_national_id"]);
                }
            }
            else
            {
                ModelState.AddModelError("NationalId", LangHelpers.Instance["national_id_required"]);
            }

            if (Utils.HasRolesAny(ArfUserRole.Researcher, ArfUserRole.PrincipalResearcher, ArfUserRole.Referee))
            {
                if (string.IsNullOrWhiteSpace(model.BirthDate) || !DateTime.TryParseExact(model.BirthDate.Trim(), "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out birthDate))
                {
                    ModelState.AddModelError("BirthDate", LangHelpers.Instance["birthdate_required"]);
                }

                if (!(model.NationalityId > 0))
                {
                    ModelState.AddModelError("NationalityId", LangHelpers.Instance["nationality_required"]);
                }
                if (!(model.CountryId > 0))
                {
                    ModelState.AddModelError("CountryId", LangHelpers.Instance["country_required"]);
                }

                if ((model.CityId == null))
                {
                    ModelState.AddModelError("CityId", LangHelpers.Instance["country_required"]);
                }

                if (string.IsNullOrWhiteSpace(model.Address))
                {
                    ModelState.AddModelError("Address", LangHelpers.Instance["address_required"]);
                }
                if (model.UniversityId == 0 && string.IsNullOrWhiteSpace(model.UniversityOther))
                {
                    ModelState.AddModelError("UniversityOther", LangHelpers.Instance["university_required"]);
                }
                else if (!(model.UniversityId >= 0))
                {
                    ModelState.AddModelError("UniversityId", LangHelpers.Instance["university_required"]);
                }

                if (model.CollegeId == 0 && string.IsNullOrWhiteSpace(model.CollegeOther))
                {
                    ModelState.AddModelError("CollegeOther", LangHelpers.Instance["college_required"]);
                }
                else if (!(model.CollegeId >= 0))
                {
                    ModelState.AddModelError("CollegeId", LangHelpers.Instance["college_required"]);
                }
                if (model.DepartmentID == 0 && string.IsNullOrWhiteSpace(model.DepartmentOther))
                {
                    ModelState.AddModelError("DepartmentOther", LangHelpers.Instance["department_required"]);
                }
                else if (!(model.DepartmentID >= 0))
                {
                    ModelState.AddModelError("DepartmentID", LangHelpers.Instance["department_required"]);
                }
                if (string.IsNullOrWhiteSpace(model.Specialization))
                {
                    ModelState.AddModelError("Specialization", LangHelpers.Instance["specialization_required"]);
                }
                if (model.Bank == 0 && string.IsNullOrWhiteSpace(model.BankOther))
                {
                    ModelState.AddModelError("BankOther", LangHelpers.Instance["bank_required"]);
                }
                else if (!(model.Bank >= 0))
                {
                    ModelState.AddModelError("Bank", LangHelpers.Instance["bank_required"]);
                }

                if (string.IsNullOrWhiteSpace(model.BankAccountHolderName))
                {
                    ModelState.AddModelError("BankAccountHolderName", LangHelpers.Instance["account_holder_name_required"]);
                }
                if (string.IsNullOrWhiteSpace(model.BankAccountIBAN))
                {
                    ModelState.AddModelError("BankAccountIBAN", LangHelpers.Instance["iban_required"]);
                }
                else if (!Utils.ValidateIBAN(model.BankAccountIBAN))
                {
                    ModelState.AddModelError("BankAccountIBAN", LangHelpers.Instance["invalid_iban"]);
                }

                if (model.BankIbanPhotocopy == null || model.BankIbanPhotocopy.ContentLength == 0)
                {
                    var oldFile = DbContext.UserFiles
                                        .Where(f => f.User.Researchers.Count(r => r.UserId == CurrentUser.Id) > 0)
                                        .Where(f => f.Type == (byte)FileType.IbanPhotocopy)
                                        .OrderByDescending(f => f.Created)
                                        .FirstOrDefault();

                    if (oldFile == null)
                    {
                        ModelState.AddModelError("BankIbanPhotocopy", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("BankIbanPhotocopy", false, lang)));
                    }
                }
                else
                {
                    var extension = model.BankIbanPhotocopy.FileName == null ? null : Path.GetExtension(model.BankIbanPhotocopy.FileName).ToLower();
                    if (extension == null || (extension != ".pdf" && extension != ".doc" && extension != ".docx" && extension != ".jpeg" && extension != ".jpg" && extension != ".png"))
                    {
                        ModelState.AddModelError("BankIbanPhotocopy", LangHelpers.Instance["file_type_pdf_doc"]);
                    }
                }
            }

            if (Utils.HasRolesAny(ArfUserRole.Researcher, ArfUserRole.PrincipalResearcher))
            {
                model.UniversityId = 1;
                if (!string.IsNullOrWhiteSpace(model.Email) && (!model.Email.Trim().ToLower().EndsWith("@imamu.edu.sa") && !model.Email.Trim().ToLower().EndsWith(".imamu.edu.sa") && !model.Email.Trim().ToLower().EndsWith("@kay-ict.com")))
                {
                    ModelState.AddModelError("Email", LangHelpers.Instance.GetLangValueByString("email_must_be_imam_edu"));
                }

                if (string.IsNullOrWhiteSpace(model.SectionPublishedPapers) || Utils.StripHTML(model.SectionPublishedPapers).Trim().Length == 0)
                {
                    ModelState.AddModelError("SectionPublishedPapers", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionPublishedPapers", false, lang)));
                }

                if (string.IsNullOrWhiteSpace(model.SectionInterests) || Utils.StripHTML(model.SectionInterests).Trim().Length == 0)
                {
                    ModelState.AddModelError("SectionInterests", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionInterests", false, lang)));
                }

                if (string.IsNullOrWhiteSpace(model.SectionCareerHistory) || Utils.StripHTML(model.SectionCareerHistory).Trim().Length == 0)
                {
                    ModelState.AddModelError("SectionCareerHistory", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionCareerHistory", false, lang)));
                }

                if (string.IsNullOrWhiteSpace(model.SectionAcademicHistory) || Utils.StripHTML(model.SectionAcademicHistory).Trim().Length == 0)
                {
                    ModelState.AddModelError("SectionAcademicHistory", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionAcademicHistory", false, lang)));
                }

                if (string.IsNullOrWhiteSpace(model.SectionTrainingCourses) || Utils.StripHTML(model.SectionTrainingCourses).Trim().Length == 0)
                {
                    ModelState.AddModelError("SectionTrainingCourses", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionTrainingCourses", false, lang)));
                }

                if (string.IsNullOrWhiteSpace(model.SectionAwards) || Utils.StripHTML(model.SectionAwards).Trim().Length == 0)
                {
                    ModelState.AddModelError("SectionAwards", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionAwards", false, lang)));
                }

                if (string.IsNullOrWhiteSpace(model.SectionMemberships) || Utils.StripHTML(model.SectionMemberships).Trim().Length == 0)
                {
                    ModelState.AddModelError("SectionMemberships", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionMemberships", false, lang)));
                }

            }
            else
            {
                model.UniversityId = CurrentUser.UniversityId;
            }
            universityId = model.UniversityId;

            if (ModelState.IsValid)
            {
                try
                {
                    if (user.UserProfile == null)
                    {
                        user.UserProfile = new UserProfile
                        {
                            UserId = user.Id,
                            Created = TM.Now,
                            CreatedBy = CurrentUser.Id,
                            Updated = TM.Now,
                            UpdatedBy = CurrentUser.Id,
                        };

                        DbContext.UserProfiles.Add(user.UserProfile);
                        DbContext.SaveChanges();
                    }
                    if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != "**********" && model.Password == model.ConfirmPassword)
                    {
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                        user.ForcePasswordChange = false;
                    }
                    else if (CurrentUser.ForcePasswordChange)
                    {
                        TempData["message"] = Resources.ArfStrings.must_change_password;
                        TempData["message_type"] = "danger";

                        FillOutLookups(model);

                        return View(model);
                    }
                    // User Profile Data
                    if (number != null)
                    {
                        user.UserProfile.MobileNo = "0" + number.NationalNumber;
                    }

                    var cmps = Utils.GetNameComponents(model.FullName);

                    user.UserProfile.FirstName = cmps.FirstName;
                    user.UserProfile.MiddleName = cmps.SecondName;
                    user.UserProfile.ThirdName = cmps.ThirdName;
                    user.UserProfile.LastName = cmps.LastName;
                    user.UserProfile.NationalId = model.NationalId;
                    user.UserProfile.Updated = TM.Now;
                    user.UserProfile.NationalityId = model.NationalityId;
                    user.UserProfile.BirthDate = birthDate;
                    user.UserProfile.BankId = model.Bank;
                    user.UserProfile.BankAccountHolder = model.BankAccountHolderName;
                    user.UserProfile.BankBranch = model.BankBranch;
                    user.UserProfile.BankIban = model.BankAccountIBAN;

                    // CVs
                    user.UserProfile.CvSectionPublishedPapers = model.SectionPublishedPapers;
                    user.UserProfile.CvSectionInterests = model.SectionInterests;
                    user.UserProfile.CvSectionCareerHistory = model.SectionCareerHistory;
                    user.UserProfile.CvSectionTrainingCourses = model.SectionTrainingCourses;
                    user.UserProfile.CvSectionAwards = model.SectionAwards;
                    user.UserProfile.CvSectionMemberships = model.SectionMemberships;
                    user.UserProfile.CvSectionAcademicHistory = model.SectionAcademicHistory;

                    // User Data
                    user.UserName = model.UserName;
                    user.CountryId = model.CountryId;
                    user.UniversityId = model.UniversityId == 0 ? null : model.UniversityId;
                    user.CollegeId = model.CollegeId;
                    user.DepartmentId = model.DepartmentID;
                    user.MajorId = model.MajorId;
                    user.Email = model.Email;

                    //user.IdentityNumber = model.NationalId;
                    user.Address = model.Address;
                    user.PhoneNumber = model.MobileNo;
                    user.Updated = TM.Now;
                    user.NationalId = model.NationalId;
                    user.FullName = model.FullName;
                    user.UniversityOther = model.UniversityOther;
                    user.CollegeOther = model.CollegeOther;
                    user.MajorOther = model.MajorName;
                    if (user.CollegeId == 0)
                    {
                        user.CollegeId = null;
                    }
                    if (user.DepartmentId == 0)
                    {
                        user.DepartmentId = null;
                    }

                    if (user.Created == null)
                    {
                        user.Created = DbContext.UserLogs.Count(u => u.UserId == user.Id) == 0 ? TM.Now : DbContext.UserLogs.OrderByDescending(u => u.Created).FirstOrDefault(u => u.UserId == user.Id).Created;
                    }

                    DbContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    DbContext.Entry(user.UserProfile).State = System.Data.Entity.EntityState.Modified;

                    var researcher = DbContext.Researchers.Where(x => x.UserId == user.Id).FirstOrDefault();
                    if (researcher != null)
                    {
                        researcher.NationalId = model.NationalId;
                        researcher.Qualification = (byte)model.Qualification;
                        researcher.DepartmentOther = model.DepartmentOther;
                        researcher.Specialization = model.Specialization;
                        researcher.CollegeOther = model.CollegeOther;
                        researcher.CollegeId = model.CollegeId;
                        researcher.FirstName = cmps.FirstName;
                        researcher.LastName = cmps.LastName;
                        researcher.SecondName = cmps.SecondName;
                        researcher.ThirdName = cmps.ThirdName;
                        researcher.NationalityId = model.NationalityId;
                        researcher.NationalId = model.NationalId;
                        researcher.CountryId = model.CountryId;
                        researcher.CityId = model.CityId;
                        researcher.UniversityId = model.UniversityId;
                        researcher.CollegeId = model.CollegeId;
                        researcher.DepartmentId = model.DepartmentID;
                        researcher.DepartmentOther = model.DepartmentOther;
                        researcher.Address = model.Address;
                        researcher.Email = model.Email;
                        researcher.MobileNo = model.MobileNo;
                        researcher.Updated = TM.Now;
                        researcher.UpdatedBy = CurrentUser.Id;
                        researcher.DateOfBirth = birthDate;

                        if (model.BankIbanPhotocopy != null && model.BankIbanPhotocopy.ContentLength > 0)
                        {
                            var file = model.BankIbanPhotocopy;
                            var oldFile = researcher.User.UserFiles
                                       .Where(f => f.Type == (byte)FileType.IbanPhotocopy)
                                       .OrderByDescending(f => f.Created)
                                       .FirstOrDefault();

                            var fileId = oldFile == null ? Guid.NewGuid() : oldFile.Id;
                            var fileName = Path.GetFileName(file.FileName);
                            var extension = Path.GetExtension(fileName);
                            var path = Path.Combine(Server.MapPath("~/Content/files/"), fileId + extension);

                            file.EnhancedSaveAs(path);

                            if (oldFile == null)
                            {
                                oldFile = new Data.DbModels.UserFile
                                {
                                    Id = fileId,
                                    UserId = user.Id,
                                    Type = (byte)FileType.IbanPhotocopy,
                                    CreatedBy = Utils.CurrentUser.Id,
                                    Updated = TM.Now,
                                    UpdatedBy = CurrentUser.Id,

                                    FileName = Path.GetFileName(file.FileName),
                                    Extension = extension,
                                    Created = TM.Now,
                                    FilePath = path
                                };

                                DbContext.UserFiles.Add(oldFile);
                            }
                            else
                            {
                                oldFile.FilePath = path;
                                oldFile.Extension = extension;
                                oldFile.FileName = Path.GetFileName(file.FileName);
                                oldFile.Updated = TM.Now;
                                oldFile.UpdatedBy = CurrentUser.Id;
                            }
                        }

                        // CVs
                        DbContext.Entry(researcher).State = System.Data.Entity.EntityState.Modified;
                    }
                    var Referee = DbContext.Referees.Where(x => x.UserId == user.Id).FirstOrDefault();
                    if (Referee != null)
                    {
                        Referee.Qualification = (byte)model.Qualification;
                        DbContext.Entry(Referee).State = System.Data.Entity.EntityState.Modified;
                    }

                    DbContext.SaveChanges();

                    TempData["message"] = Resources.ArfStrings.file_updated_successfully;
                    TempData["message_type"] = "success";

                    return RedirectToAction("Index", "Account");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    Logger.LogExceptionAsync(ex, model);
                    TempData["message"] = LangHelpers.Instance["failed"] + " " + ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
                    TempData["message_type"] = "danger";
                }
                catch (Exception ex)
                {
                    Logger.LogExceptionAsync(ex, model);

                    var ex2 = ex;
                    while (ex2.InnerException != null)
                    {
                        ex2 = ex2.InnerException;
                    }

                    TempData["message"] = LangHelpers.Instance["failed"] + " " + ex2.Message;
                    TempData["message_type"] = "danger";
                }
            }

            if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher))
            {
                model.BankIbanPhotocopyReadonly = user.UserFiles
                                             .Where(f => f.Type == (byte)FileType.IbanPhotocopy)
                                             .OrderByDescending(f => f.Created)
                                             .Select(c => new AttachedFile
                                             {
                                                 FileId = c.Id.ToString(),
                                                 Type = (FileType)c.Type,
                                                 FileName = Path.GetFileName(c.FileName)
                                             })
                                             .FirstOrDefault();
            }

            model.IsActivated = user == null ? false : user.IsActivated;
            ViewBag.IncludeCKEditor = true;
            FillOutLookups(model);
            model.UniversityId = universityId;

            return View(model);
        }

        public ActionResult LogOff()
        {
            if (User != null && User.Identity != null && CurrentUser != null && CurrentUser.Id != null)
            {
                var sessionClaim = SignInManager.AuthenticationManager.User.Claims.FirstOrDefault(c => c.Type == "SESSION_ID");

                if (sessionClaim != null)
                {
                    var session = DbContext.UserSessions.OrderByDescending(v => v.Created).FirstOrDefault(s => s.Sessionid == sessionClaim.Value);
                    if (session != null)
                    {
                        session.Locked = true;
                        session.Lockdate = TM.Now;
                    }
                }

                DbContext.UserLogs.Add(new UserLog
                {
                    Created = TM.Now,
                    UserId = CurrentUser.Id,
                    OperationCode = "LOGOUT"
                });

                DbContext.SaveChanges();
            }

            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

            if (CurrentUser != null)
            {
                var userConenctions = DbContext.UserConnections.Where(u => u.UserId == CurrentUser.Id).ToList();

                foreach (var u in userConenctions)
                {
                    u.IsConnected = false;
                    context.Clients.All.renderUnregister(u.ConnectionId);
                }
            }

            DbContext.SaveChanges();

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("id", Guid.NewGuid().ToString()));
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register(ArfUserRole? role, int? step)
        {
            var model = new RegistrationViewModel()
            {
                Step = step
            };

            if (role == null)
            {
                role = ArfUserRole.Researcher;
            }

            if (!(model.Step >= 1))
            {
                model.Step = 1;
            }

            GetDataFromSession(ref model, true);
            ViewBag.ResearcherPageTitle = LangHelpers.Instance["register_as_researcher"];

            if (!String.IsNullOrEmpty(model.ExternalRefereeHashParams) && !String.IsNullOrEmpty(model.Email))
            {
                UserSession.EmailVerified = model.Email;
                ViewBag.ResearcherPageTitle = LangHelpers.Instance["register_as_referee"];
            }

            if (!(model.LastSavedStep >= 1))
            {
                model.LastSavedStep = 0;
            }

            if (model.Step > 5 || model.Step > model.LastSavedStep + 1)
            {
                return Redirect(Utils.GetRegisterLink(role.Value, model.LastSavedStep + 1));
            }

            ViewBag.Countries = new SelectList(DbContext.Countries
                .OrderBy(c => c.NameArabic == "المملكة العربية السعودية" ? 1 : 2)
                .ThenBy(c => c.NameArabic), "Id", "NameArabic");

            model.CountryId = 1;

            if (role == ArfUserRole.Researcher)
                role = ArfUserRole.PrincipalResearcher;

            model.Role = role;

            FillOutLookups(model);
            ViewBag.IncludeCKEditor = true;

            if (model.Step == 4)
            {
                ViewBag.IsMobileVerified = UserSession != null && UserSession.VerifiedMobile != null && UserSession.VerifiedMobile == model.MobileNumber;
                ViewBag.IsEmailVerified = UserSession != null && UserSession.EmailVerified != null && UserSession.EmailVerified == model.Email;
            }

            if (role == null || (role != ArfUserRole.PrincipalResearcher && role != ArfUserRole.Referee))
            {
                return View();
            }
            else if (role == ArfUserRole.PrincipalResearcher)
            {
                return View("RegisterResearcherSteps", model);
            }
            else
            {
                return View("RegisterReferee", model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegistrationViewModel model)
        {
            if (model == null)
            {
                return RedirectToAction("Register", "Account");
            }

            var res = this.Register(model.Role, model.Step);
            if (res is RedirectResult || res is RedirectToRouteResult)
            {
                return res;
            }

            var model2 = (RegistrationViewModel)((ViewResult)res).Model;

            model.Step = model2.Step;
            model.LastSavedStep = model2.LastSavedStep;

            if (model.Role == ArfUserRole.Researcher)
                model.Role = ArfUserRole.PrincipalResearcher;

            if (model.Role == null || (model.Role != ArfUserRole.PrincipalResearcher && model.Role != ArfUserRole.Referee))
            {
                return RedirectToAction("Register", "Account");
            }

            if (model.Role == ArfUserRole.Referee)
            {
                model.Role = model2.Role;
                model2 = model;
            }

            var db = Utils.db;
            var birthDate = DateTime.MinValue;

            if (!string.IsNullOrWhiteSpace(model2.BirthDate) && DateTime.TryParseExact(model2.BirthDate.Trim(), "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out birthDate))
            {
            }

            switch (model.Step)
            {
                case 1:
                    {
                        if (!(model.CountryId > 0))
                        {
                            ModelState.AddModelError("CountryId", LangHelpers.Instance["residence_country_required"]);
                        }

                        if (!(model.CityId > 0))
                        {
                            ModelState.AddModelError("CityId", LangHelpers.Instance["residence_city_required"]);
                        }

                        if (!(model.Qualification > 0))
                        {
                            ModelState.AddModelError("Qualification", LangHelpers.Instance["qualification_required"]);
                        }

                        if (string.IsNullOrWhiteSpace(model.Specialization))
                        {
                            ModelState.AddModelError("Specialization", LangHelpers.Instance["specialization_required"]);
                        }

                        if (string.IsNullOrWhiteSpace(model.FullName))
                        {
                            ModelState.AddModelError("FullName", LangHelpers.Instance["quadname_required"]);
                        }
                        else
                        {
                            var comps = Utils.GetNameComponents(model.FullName);

                            if (comps.FirstName == null || comps.LastName == null || comps.ThirdName == null || comps.SecondName == null)
                            {
                                ModelState.AddModelError("FullName", LangHelpers.Instance["quadname_required"]);
                            }
                        }

                        birthDate = DateTime.MinValue;

                        if (string.IsNullOrWhiteSpace(model.BirthDate) || !DateTime.TryParseExact(model.BirthDate.Trim(), "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out birthDate))
                        {
                            ModelState.AddModelError("BirthDate", LangHelpers.Instance["birthdate_required"]);
                        }
                        else
                        {
                            var age = Utils.CalculateAge(birthDate);
                            if (age < 17)
                            {
                                ModelState.AddModelError("BirthDate", LangHelpers.Instance["age_is_too_young"]);
                            }
                        }

                        if (string.IsNullOrWhiteSpace(model.NationalId))
                        {
                            ModelState.AddModelError("NationalId", LangHelpers.Instance["national_id_required"]);
                        }
                        else if (!IsValidNationalId(model.NationalId))
                        {
                            ModelState.AddModelError("NationalId", LangHelpers.Instance["invalid_national_id"]);
                        }

                        if (model.UniversityId == 0 && string.IsNullOrWhiteSpace(model.UniversityOther))
                        {
                            ModelState.Remove("UniversityId");
                            ModelState.Remove("CollegeId");
                            ModelState.Remove("UniversityId");
                            ModelState.AddModelError("UniversityOther", LangHelpers.Instance["university_required"]);
                        }
                        else if (!(model.UniversityId >= 0))
                        {
                            ModelState.AddModelError("UniversityId", LangHelpers.Instance["university_required"]);
                        }

                        if (model.CollegeId == 0 && string.IsNullOrWhiteSpace(model.CollegeOther))
                        {
                            ModelState.Remove("CollegeId");
                            ModelState.AddModelError("CollegeOther", LangHelpers.Instance["college_required"]);
                        }
                        else if (!(model.CollegeId >= 0))
                        {
                            ModelState.AddModelError("CollegeId", LangHelpers.Instance["college_required"]);
                        }

                        if (model.DepartmentID == 0 && string.IsNullOrWhiteSpace(model.DepartmentOther))
                        {
                            ModelState.Remove("DepartmentID");
                            ModelState.AddModelError("DepartmentOther", LangHelpers.Instance["department_required"]);
                        }
                        else if (!(model.DepartmentID >= 0))
                        {
                            ModelState.AddModelError("DepartmentID", LangHelpers.Instance["department_required"]);
                        }

                        if (!(model.NationalityId > 0))
                        {
                            ModelState.AddModelError("NationalityId", LangHelpers.Instance["nationality_required"]);
                        }

                        if (model.Role == ArfUserRole.Referee)
                        {
                            if (string.IsNullOrWhiteSpace(model.MobileNumber))
                            {
                                ModelState.AddModelError("MobileNumber", LangHelpers.Instance["mobile_required"]);
                            }
                            else
                            {
                                model.MobileNumber = model.MobileNumber.Trim().ToLower();
                                var phone = "";
                                if (!Utils.ValidateMobile(model.MobileNumber, out phone))
                                {
                                    ModelState.AddModelError("MobileNumber", LangHelpers.Instance["invalid_mobile"]);
                                }
                                else
                                {
                                    model.MobileNumber = phone;

                                    ViewBag.IsMobileVerified = UserSession != null && UserSession.VerifiedMobile != null && UserSession.VerifiedMobile == model.MobileNumber;

                                    if (ViewBag.IsMobileVerified != true)
                                    {
                                        ModelState.AddModelError("MobileNumber", LangHelpers.Instance["mobile_not_verified"]);
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 2:
                    {
                        if (model.Bank == 0 && string.IsNullOrWhiteSpace(model.BankOther))
                        {
                            ModelState.AddModelError("BankOther", LangHelpers.Instance["bank_required"]);
                        }
                        else if (!(model.Bank >= 0))
                        {
                            ModelState.AddModelError("Bank", LangHelpers.Instance["bank_required"]);
                        }

                        if (string.IsNullOrWhiteSpace(model.BankAccountHolderName))
                        {
                            ModelState.AddModelError("BankAccountHolderName", LangHelpers.Instance["account_holder_name_required"]);
                        }

                        if (string.IsNullOrWhiteSpace(model.BankAccountIBAN))
                        {
                            ModelState.AddModelError("BankAccountIBAN", LangHelpers.Instance["iban_required"]);
                        }
                        else if (!Utils.ValidateIBAN(model.BankAccountIBAN))
                        {
                            ModelState.AddModelError("BankAccountIBAN", LangHelpers.Instance["invalid_iban"]);
                        }

                        {
                            var foundIban = false;

                            var uploadedIban = (HttpPostedFileBase)null;

                            uploadedIban = Request.Files["BankIbanPhotocopy"];

                            if (uploadedIban != null && uploadedIban.ContentLength > 0)
                            {
                                foundIban = true;
                            }

                            if (!foundIban)
                            {
                                if (model2.BankIbanPhotocopyReadonly == null)
                                    ModelState.AddModelError("BankIbanPhotocopy", LangHelpers.Instance["iban_photo_required"]);
                            }
                            else
                            {
                                var extension = Path.GetExtension(uploadedIban.FileName).ToLower();
                                if (extension == null || (extension != ".pdf" && extension != ".doc" && extension != ".docx" && extension != ".jpeg" && extension != ".jpg" && extension != ".png"))
                                {
                                    ModelState.AddModelError("BankIbanPhotocopy", LangHelpers.Instance["file_type_pdf_doc"]);
                                }
                            }
                        }
                    }
                    break;

                case 3:
                    {
                        if (model.Role == ArfUserRole.PrincipalResearcher)
                        {
                            var lang = (string)null;
                            if (string.IsNullOrWhiteSpace(model.SectionPublishedPapers) || Utils.StripHTML(model.SectionPublishedPapers).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionPublishedPapers", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionPublishedPapers", false, lang)));
                            }

                            if (string.IsNullOrWhiteSpace(model.SectionInterests) || Utils.StripHTML(model.SectionInterests).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionInterests", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionInterests", false, lang)));
                            }

                            if (string.IsNullOrWhiteSpace(model.SectionCareerHistory) || Utils.StripHTML(model.SectionCareerHistory).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionCareerHistory", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionCareerHistory", false, lang)));
                            }

                            if (string.IsNullOrWhiteSpace(model.SectionAcademicHistory) || Utils.StripHTML(model.SectionAcademicHistory).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionAcademicHistory", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionAcademicHistory", false, lang)));
                            }

                            if (string.IsNullOrWhiteSpace(model.SectionTrainingCourses) || Utils.StripHTML(model.SectionTrainingCourses).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionTrainingCourses", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionTrainingCourses", false, lang)));
                            }

                            if (string.IsNullOrWhiteSpace(model.SectionAwards) || Utils.StripHTML(model.SectionAwards).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionAwards", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionAwards", false, lang)));
                            }

                            if (string.IsNullOrWhiteSpace(model.SectionMemberships) || Utils.StripHTML(model.SectionMemberships).Trim().Length == 0)
                            {
                                ModelState.AddModelError("SectionMemberships", string.Format(LangHelpers.Instance.GetLangValueByString("field_is_required", false, lang), LangHelpers.Instance.GetLangValueByString("SectionMemberships", false, lang)));
                            }
                        }
                    }
                    break;

                case 4:
                    {
                        if (string.IsNullOrWhiteSpace(model.Email))
                        {
                            ModelState.AddModelError("Email", LangHelpers.Instance["email_required"]);
                        }
                        else
                        {
                            model.Email = model.Email.Trim().ToLower();

                            if (!Utils.ValidateEmail(model.Email))
                            {
                                ModelState.AddModelError("Email", LangHelpers.Instance["invalid_email"]);
                            }
                            else
                            {
                                if (model2.UniversityId == 1 && (!model.Email.Trim().ToLower().EndsWith("@imamu.edu.sa") && !model.Email.Trim().ToLower().EndsWith("@kay-ict.com")))
                                {
                                    ModelState.AddModelError("Email", LangHelpers.Instance.GetLangValueByString("email_must_be_imam_edu"));
                                }

                                if (model.EmailConfirmation != model.Email)
                                {
                                    ModelState.AddModelError("EmailConfirmation", LangHelpers.Instance["email_confirmation_mismatch"]);
                                }
                                else if (db.Users.FirstOrDefault(u => u.Email == model.Email) != null)
                                {
                                    ModelState.AddModelError("Email", LangHelpers.Instance["email_exists"]);
                                }
                                else
                                {
                                    ViewBag.IsEmailVerified = UserSession != null && UserSession.EmailVerified != null && UserSession.EmailVerified == model.Email;

                                    if (ViewBag.IsEmailVerified != true)
                                    {
                                        ModelState.AddModelError("Email", LangHelpers.Instance["email_not_verified"]);
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(model.MobileNumber))
                        {
                            ModelState.AddModelError("MobileNumber", LangHelpers.Instance["mobile_required"]);
                        }
                        else
                        {
                            model.MobileNumber = model.MobileNumber.Trim().ToLower();
                            var phone = "";
                            if (!Utils.ValidateMobile(model.MobileNumber, out phone))
                            {
                                ModelState.AddModelError("MobileNumber", LangHelpers.Instance["invalid_mobile"]);
                            }
                            else
                            {
                                model.MobileNumber = phone;

                                ViewBag.IsMobileVerified = UserSession != null && UserSession.VerifiedMobile != null && UserSession.VerifiedMobile == model.MobileNumber;

                                if (ViewBag.IsMobileVerified != true)
                                {
                                    ModelState.AddModelError("MobileNumber", LangHelpers.Instance["mobile_not_verified"]);
                                }
                            }
                        }
                    }

                    break;

                case 5:
                    {
                        if (model.Password == null)
                        {
                            ModelState.AddModelError("Password", LangHelpers.Instance["password_required"]);
                        }
                    }
                    break;
            }
            var user = Utils.db.Users.Where(x => x.Email == model.Email || x.UserName == model2.Email);

            if (user.Count() > 0)
            {
                ModelState.AddModelError("Email", LangHelpers.Instance["user_already_exists_with_provided_email"]);
            }

            if (ModelState.IsValid)
            {
                if (model.Step == 1)
                {
                    if (model.UniversityId == 0) model.UniversityId = null;
                    if (model.CollegeId == 0) model.CollegeId = null;
                    if (model.DepartmentID == 0) model.DepartmentID = null;
                    if (model.Bank == 0) model.Bank = null;
                }

                if (model.Step < 5 && model.Role != ArfUserRole.Referee)
                {
                    SaveDataToSession(model);
                    TempData["message"] = LangHelpers.Instance["data_saved_successfully"];
                    TempData["message_type"] = "success";
                    return Redirect(Utils.GetRegisterLink(model.Role.Value, model.Step.Value + 1));
                }
                else
                {
                    using (var trans2 = DbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            var hasher = new PasswordHasher();
                            var appUser = new User
                            {
                                Id = Guid.NewGuid().ToString(),
                                FullName = model2.FullName,
                                UserName = model2.Email,
                                Email = model2.Email,
                                PhoneNumber = model2.MobileNumber,
                                CountryId = model2.CountryId == 0 ? null : model2.CountryId,
                                CollegeId = model2.CollegeId == 0 ? null : model2.CollegeId,
                                CollegeOther = model2.CollegeOther,
                                UniversityOther = model2.UniversityOther,
                                MajorOther = model2.MajorOther,
                                Address = model2.Address,
                                PasswordHash = hasher.HashPassword(model.Password),
                                SecurityStamp = "a14ee95e-c73e-466e-a786-aaf4378237d3",
                                AccessFailedCount = 0,
                                CreatedBy = UserId,
                                UpdatedBy = UserId,
                                DepartmentId = model2.DepartmentID,
                                UniversityId = model2.UniversityId,

                                Roles = db.Roles.Where(r => r.Name == model.Role.Value.ToString()).ToList(),
                                Created = TM.Now,
                                Updated = TM.Now,
                                IsActivated = (model2.Role == ArfUserRole.PrincipalResearcher || model2.Role == ArfUserRole.Referee),
                                EmailConfirmed = UserSession.EmailVerified == model2.Email,
                                PhoneNumberConfirmed = UserSession.VerifiedMobile == model2.MobileNumber
                            };

                            db.Users.Add(appUser);
                            db.SaveChanges();

                            var gKey = "";
                            var err = "";

                            {
                                var comps = Utils.GetNameComponents(model2.FullName);

                                var profile = new UserProfile
                                {
                                    UserId = appUser.Id,
                                    FirstName = comps.FirstName,
                                    MiddleName = comps.SecondName,
                                    ThirdName = comps.ThirdName,
                                    LastName = comps.LastName,
                                    AddressLine1 = model2.Address,
                                    BirthDate = birthDate,
                                    NationalityId = model2.NationalityId,
                                    BankName = model2.BankName,
                                    BankBranch = model2.BankBranch,
                                    BankIban = model2.BankAccountIBAN,
                                    BankAccountHolder = model2.BankAccountHolderName,
                                    NationalId = model2.NationalId,
                                    MobileNo = model2.MobileNumber,
                                    MobileNumber = model2.MobileNumber,
                                    BankId = model2.Bank,

                                    Created = TM.Now,
                                    Updated = TM.Now,
                                    CreatedBy = appUser.Id,
                                    UpdatedBy = appUser.Id,

                                    CvSectionPublishedPapers = model2.SectionPublishedPapers,
                                    CvSectionInterests = model2.SectionInterests,
                                    CvSectionCareerHistory = model2.SectionCareerHistory,
                                    CvSectionTrainingCourses = model2.SectionTrainingCourses,
                                    CvSectionAwards = model2.SectionAwards,
                                    CvSectionMemberships = model2.SectionMemberships,
                                    CvSectionAcademicHistory = model2.SectionAcademicHistory
                                };

                                db.UserProfiles.Add(profile);

                                db.SaveChanges();
                                if (model2.Role == ArfUserRole.PrincipalResearcher)
                                {
                                    var researcher = new Researcher
                                    {
                                        UserId = appUser.Id,
                                        FirstName = comps.FirstName,
                                        SecondName = comps.SecondName,
                                        ThirdName = comps.ThirdName,
                                        LastName = comps.LastName,
                                        NationalId = model2.NationalId,
                                        AccountOwner = model2.BankAccountHolderName,

                                        NationalityId = model2.NationalityId,
                                        Email = model2.Email,
                                        MobileNo = model2.MobileNumber,
                                        CollegeId = model2.CollegeId,
                                        DateOfBirth = birthDate,
                                        UniversityId = model2.UniversityId,

                                        MajorId = model2.MajorId,
                                        Address = model2.Address,
                                        Created = TM.Now,
                                        Updated = TM.Now,
                                        CreatedBy = appUser.Id,
                                        UpdatedBy = appUser.Id,
                                        CountryId = model2.CountryId,
                                        CityId = model2.CityId,
                                        DepartmentId = model2.DepartmentID,
                                        Qualification = model2.Qualification == null ? (byte)QualificationType.Unspecified : (byte)model2.Qualification,
                                        UniversityOther = model2.UniversityOther,
                                        CollegeOther = model2.CollegeOther,
                                        DepartmentOther = model2.DepartmentOther,
                                        Specialization = model2.Specialization
                                    };

                                    db.Researchers.Add(researcher);

                                    db.SaveChanges();

                                    var refereeRoleId = db.Roles.FirstOrDefault(r => r.Name == ArfUserRole.Referee.ToString()).Id;

                                    if (researcher.UniversityId == 1)
                                    {
                                        var u = appUser;
                                        var extraRole = new UserExtraRole
                                        {
                                            UserId = u.Id,
                                            RoleId = refereeRoleId,
                                            IsActive = true,
                                            Created = TM.Now,
                                            Updated = TM.Now,
                                            CreatedBy = Utils.SystemAdminUserId,
                                            UpdatedBy = Utils.SystemAdminUserId,

                                        };

                                        db.UserExtraRoles.Add(extraRole);

                                        db.SaveChanges();

                                        var referee = db.Referees.FirstOrDefault(r => r.Email == u.Email);
                                        if (referee == null)
                                        {

                                            var userReferee = new Referee
                                            {
                                                UserId = u.Id,
                                                Email = researcher.Email,
                                                MobileNo = researcher.MobileNo,
                                                FirstName = researcher.FirstName,
                                                SecondName = researcher.SecondName,
                                                ThirdName = researcher.ThirdName,
                                                LastName = researcher.LastName,
                                                UniversityId = researcher.UniversityId,
                                                CollegeId = researcher.CollegeId,
                                                Address = researcher.Address,
                                                CityId = researcher.CityId,
                                                CountryId = researcher.CountryId,
                                                NationalityId = researcher.NationalityId,
                                                MajorId = researcher.MajorId,
                                                Qualification = researcher.Qualification,
                                                RegionId = researcher.RegionId,
                                                IsDeleted = false,


                                                Created = TM.Now,
                                                Updated = TM.Now,
                                                CreatedBy = Utils.SystemAdminUserId,
                                                UpdatedBy = Utils.SystemAdminUserId,
                                            };
                                            db.Referees.Add(userReferee);
                                        }
                                        else
                                        {
                                            referee.UserId = appUser.Id;
                                        }
                                        db.SaveChanges();
                                    }
                                }
                                else if (model2.Role == ArfUserRole.Referee)
                                {
                                    var _referee = db.Referees.Where(r => r.Email == model2.Email).FirstOrDefault();
                                    if (_referee != null)
                                    {
                                        _referee.UserId = appUser.Id;
                                    }
                                    else
                                    {
                                        var referee = new Referee
                                        {
                                            UserId = appUser.Id,
                                            FirstName = comps.FirstName,
                                            SecondName = comps.SecondName,
                                            ThirdName = comps.ThirdName,
                                            LastName = comps.LastName,
                                            Email = model2.Email,
                                            MobileNo = model2.MobileNumber,
                                            CollegeId = model2.CollegeId,
                                            UniversityId = model2.UniversityId,
                                            MajorId = model2.MajorId,
                                            Address = model2.Address,
                                            Created = TM.Now,
                                            Updated = TM.Now,
                                            CreatedBy = appUser.Id,
                                            UpdatedBy = appUser.Id
                                        };

                                        db.Referees.Add(referee);
                                    }

                                    db.SaveChanges();
                                }

                                if (model2.BankIbanPhotocopyReadonly != null)
                                {
                                    var sessFile = db.UserSessionFiles.FirstOrDefault(f => f.Id.ToString() == model2.BankIbanPhotocopyReadonly.FileId);

                                    if (sessFile != null)
                                    {
                                        var file = DbContext.UserFiles.FirstOrDefault(f => f.Id == sessFile.Id);

                                        var id = file == null ? sessFile.Id : Guid.NewGuid();

                                        file = new Data.DbModels.UserFile
                                        {
                                            Id = id,
                                            Type = sessFile.Type,
                                            CreatedBy = appUser.Id,
                                            FileName = sessFile.FileName,
                                            Extension = sessFile.Extension,
                                            UserId = appUser.Id,
                                            Created = TM.Now,
                                            Updated = TM.Now,
                                            UpdatedBy = appUser.Id,
                                            FilePath = sessFile.FilePath
                                        };

                                        db.UserFiles.Add(file);
                                    }
                                }

                                db.SaveChanges();
                                bool linkCreated = true;
                                if (model2.Role == ArfUserRole.Researcher || (model2.Role == ArfUserRole.Referee && !appUser.EmailConfirmed))
                                {
                                    linkCreated = CreateTemporaryLink(model2.Role == ArfUserRole.Referee ? TempUrlType.RefereeEmailActivation : TempUrlType.WelcomeMessageResearcher, appUser.Id, model2.Email, appUser.PhoneNumber, out err, out gKey);
                                }

                                if (!linkCreated)
                                {
                                    TempData["message"] = LangHelpers.Instance["failed_to_send_activation_email"] + " " + err;
                                    TempData["message_type"] = "danger";
                                }
                                else
                                {
                                    try
                                    {
                                        db.UserSessionDatas.Where(x => x.SessionId == UserSession.Id).Delete();
                                        db.UserSessionFiles.Where(x => x.SessionId == UserSession.Id).Delete();
                                    }
                                    catch
                                    {
                                    }

                                    trans2.Commit();

                                    if (model2.Role == ArfUserRole.Referee && !appUser.EmailConfirmed)
                                    {
                                        TempData["message"] = LangHelpers.Instance["registration_successful_pending_approval"];
                                    }
                                    else
                                    {
                                        TempData["message"] = LangHelpers.Instance["registration_successul"];
                                    }

                                    TempData["message_type"] = "success";

                                    return RedirectToAction("Login", "Account");
                                }
                            }
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                        {
                            try { trans2.Rollback(); } catch { }

                            model2.SmsCode = null;

                            Logger.LogExceptionAsync(ex, model2);
                            TempData["message"] = LangHelpers.Instance["failed"] + " " + ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
                            TempData["message_type"] = "danger";
                        }
                        catch (Exception ex)
                        {
                            try { trans2.Rollback(); } catch { }

                            model2.SmsCode = null;

                            Logger.LogExceptionAsync(ex, model2);

                            TempData["message"] = LangHelpers.Instance["failed"] + " " + ex.SanitizeException();
                            TempData["message_type"] = "danger";
                        }
                    }
                }
            }

            ViewBag.Countries = new SelectList(DbContext.Countries.OrderBy(c => c.NameArabic), "Id", "NameArabic");
            FillOutLookups(model);
            ViewBag.IncludeCKEditor = true;

            var role = model.Role;

            if (role == ArfUserRole.PrincipalResearcher)
            {
                return View("RegisterResearcherSteps", model);
            }
            else
            {
                return View("RegisterReferee", model);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResendEmailActivation(string username)
        {
            var user = (User)null;
            if (string.IsNullOrWhiteSpace(username) || (user = DbContext.Users.FirstOrDefault(u => u.Email == username || u.UserName == username)) == null)
                return RedirectToAction("Login");

            var gKey = "";
            var err = "";

            if (!CreateTemporaryLink(user.Roles.Count(r => r.Name == ArfUserRole.Referee.ToString()) > 0 ? TempUrlType.RefereeEmailActivation : TempUrlType.ResearcherEmailActivation, user.Id, user.Email, user.PhoneNumber, out err, out gKey))
            {
                TempData["message"] = Resources.ArfStrings.activation_message_sent_failed + err;
                TempData["message_type"] = "danger";

                return RedirectToAction("Login");
            }
            else
            {
                TempData["message"] = Resources.ArfStrings.activation_message_sent;
                TempData["message_type"] = "success";

                return RedirectToAction("Login");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult SendResetPasswordEmail()
        {
            var model = new ForgotPasswordViewModel { };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendResetPasswordEmail(ForgotPasswordViewModel model)
        {
            var username = model == null ? "" : model.Email;
            var user = (User)null;
            if (string.IsNullOrWhiteSpace(username) || (user = DbContext.Users.FirstOrDefault(u => u.Email == username || u.UserName == username)) == null)
            {
                TempData["message"] = Resources.ArfStrings.this_user_does_not_exist + username;
                TempData["message_type"] = "danger";

                return RedirectToAction("Login");
            }

            var gKey = "";
            var err = "";

            if (!CreateTemporaryLink(TempUrlType.ResetPassword, user.Id, user.Email, user.PhoneNumber, out err, out gKey))
            {
                TempData["message"] = Resources.ArfStrings.activation_message_sent_failed + err;
                TempData["message_type"] = "danger";

                return RedirectToAction("Login");
            }
            else
            {
                TempData["message"] = Resources.ArfStrings.activation_message_sent;
                TempData["message_type"] = "success";

                return RedirectToAction("Login");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult SendMobileActivation(string username)
        {
            var user = (User)null;
            if (string.IsNullOrWhiteSpace(username) || (user = DbContext.Users.FirstOrDefault(u => u.Email == username || u.UserName == username)) == null)
                return RedirectToAction("Login");

            var model = new ResetPasswordViewModel
            {
                Email = user.Email,
                MobileNumber = user.PhoneNumber
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMobileActivation(ResetPasswordViewModel model)
        {
            var username = model == null ? "" : model.Email;
            var user = (User)null;
            if (string.IsNullOrWhiteSpace(username) || (user = DbContext.Users.FirstOrDefault(u => u.Email == username || u.UserName == username)) == null)
                return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(model.MobileNumber))
            {
                ModelState.AddModelError("MobileNumber", LangHelpers.Instance["mobile_no_required"]);
            }

            var db = Utils.db;

            var key = model.SmsKey == null ? null : db.UserTemporaryUrls.FirstOrDefault(u => model.SmsKey == null ? false : u.Key == model.SmsKey);

            if (key != null)
            {
                if (model.SmsCode != key.ShortCode.ToString())
                {
                    ModelState.AddModelError("ShortCode", LangHelpers.Instance["sms_code_mismatch"]);
                }
                else if (ModelState.IsValid)
                {
                    if (UserSession != null)
                        UserSession.VerifiedMobile = model.MobileNumber;
                    key.IsUsed = true;
                    user.PhoneNumber = model.MobileNumber;
                    user.PhoneNumberConfirmed = true;
                    db.SaveChanges();

                    TempData["message"] = "تم تفعيل رقم جوالك بنجاح!";
                    TempData["message_type"] = "success";

                    return RedirectToAction("Login");
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var gKey = "";
                    var err = "";

                    if (!CreateTemporaryLink(TempUrlType.SmsActivation, user.Id, user.Email, model.MobileNumber, out err, out gKey))
                    {
                        TempData["message"] = Resources.ArfStrings.activation_message_sent_failed + err;
                        TempData["message_type"] = "danger";

                        return View(model);
                    }
                    else
                    {
                        TempData["message"] = Resources.ArfStrings.activation_message_sent_to_phone;
                        TempData["message_type"] = "success";

                        model.SmsKey = gKey;

                        return View(model);
                    }
                }
            }

            model.MobileNumber = user.PhoneNumber;
            return View(model);
        }

        [NonAction]
        private void FillOutLookups(RegistrationViewModel model)
        {
            {
                var list = DbContext.Departments
                            .Where(c => c.CollegeId == model.CollegeId)
                            .ToArray()
                            .Select(c => new SelectListItem
                            {
                                Text = c.LocalizedName(),
                                Value = c.Id.ToString()
                            })
                            .OrderBy(c => c.Text)
                            .ToList();
                list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, null), Value = "0" });
                model.Departments = list.ToArray();
            }

            if (model.Role == ArfUserRole.Researcher || model.Role == ArfUserRole.PrincipalResearcher)
            {
                var list = DbContext.Universities
                               .Where(c => c.Id == 1)
                               .ToArray()
                               .OrderBy(c => c.Id == 1 ? 0 : 1)
                               .ThenBy(c => c.LocalizedName())
                               .Select(c => new SelectListItem
                               {
                                   Text = c.LocalizedName(),
                                   Value = c.Id.ToString()
                               })
                               .ToList();

                model.Universities = list.ToArray();

                model.UniversityId = 1;
                model.UniversityName = list.FirstOrDefault().Text;
            }
            else
            {
                var list = DbContext.Universities
                               .ToArray()
                               .OrderBy(c => c.Id == 1 ? 0 : 1)
                               .ThenBy(c => c.LocalizedName())
                               .Select(c => new SelectListItem
                               {
                                   Text = c.LocalizedName(),
                                   Value = c.Id.ToString()
                               })
                               .ToList();

                list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, null), Value = "0" });

                model.Universities = list.ToArray();
            }

            {
                ViewBag.IncludeCKEditor = true;
                ViewBag.LargerWidth = true;

                var list = DbContext.Colleges
                               .Where(c => c.UniversityId == model.UniversityId)
                               .ToArray()
                               .Select(c => new SelectListItem
                               {
                                   Text = c.LocalizedName(),
                                   Value = c.Id.ToString()
                               })
                               .OrderBy(c => c.Text)
                               .ToList();

                list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, null), Value = "0" });

                model.Colleges = list.ToArray();
            }

            {
                var values = Utils.QualificationTypesByResearcherType();
                var list = new List<SelectListItem>();

                var i = 0;
                foreach (var v in values)
                {
                    if (v.ToString() == "Unspecified") continue;

                    list.Add(new SelectListItem
                    {
                        Value = v.ToString(),
                        Text = LangHelpers.Instance[v.ToString()]
                    });

                    i++;
                }
                model.QualificationTypes = list.ToArray();
            }

            {
                var list = DbContext.Countries
                    .ToArray()
                    .OrderBy(c => c.Id == 1 ? 0 : 1)
                    .ThenBy(c => c.LocalizedName())
                     .Select(c => new SelectListItem
                     {
                         Text = c.LocalizedName(),
                         Value = c.Id.ToString()
                     })
                    .ToList();

                model.Countries = list.ToArray();
            }

            {
                var list = DbContext.Cities

                    .ToArray()
                    .Select(c => new CityViewModel
                    {
                        Id = c.Id,
                        CountryId = c.CountryId,
                        Name = c.LocalizedName(),
                    })
                    .OrderBy(c => c.Id == 1 ? 0 : 1)
                    .ThenBy(c => c.LocalizedName())
                    .ToList();

                list.Add(new CityViewModel { Name = LangHelpers.Instance.GetLangValueByString("other", false, null), Id = 0 });

                model.Cities = list;
            }

            {
                var list = DbContext.Banks
                    .ToArray()
                    .Select(c => new SelectListItem
                    {
                        Text = c.LocalizedName(),
                        Value = c.Id.ToString()
                    })
                    .OrderBy(c => c.Text)
                    .ToList();

                list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, null), Value = "0" });

                model.Banks = list.ToArray();
            }

            Utils.SwitchCulture(null);
        }

        [NonAction]
        private void FillOutLookups(EditUserViewModel model)
        {
            var isResearcher = Utils.HasRolesAny(ArfUserRole.Researcher, ArfUserRole.PrincipalResearcher);
            var isCommitteeMember = Utils.HasRolesAny(ArfUserRole.ResearchUnitMember);
            {
                var list = DbContext.Departments
                            .Where(c => c.CollegeId == model.CollegeId)
                            .ToArray()
                            .Select(c => new SelectListItem
                            {
                                Text = c.LocalizedName(),
                                Value = c.Id.ToString()
                            })
                            .OrderBy(c => c.Text)
                            .ToList();

                if (!isResearcher && !isCommitteeMember)
                {
                    list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, null), Value = "0" });
                }
                model.Departments = list.ToArray();
            }

            if (isResearcher)
            {
                var list = DbContext.Universities
                               .Where(c => c.Id == 1)
                               .ToArray()
                               .OrderBy(c => c.Id == 1 ? 0 : 1)
                               .ThenBy(c => c.LocalizedName())
                               .Select(c => new SelectListItem
                               {
                                   Text = c.LocalizedName(),
                                   Value = c.Id.ToString()
                               })
                               .ToList();

                model.Universities = list.ToArray();

                model.UniversityId = 1;
                model.UniversityName = list.FirstOrDefault().Text;
            }
            else
            {
                var list = DbContext.Universities
                               .ToArray()
                               .OrderBy(c => c.Id == 1 ? 0 : 1)
                               .ThenBy(c => c.LocalizedName())
                               .Select(c => new SelectListItem
                               {
                                   Text = c.LocalizedName(),
                                   Value = c.Id.ToString()
                               })
                               .ToList();

                if (!isResearcher && !isCommitteeMember)
                {
                    list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, null), Value = "0" });
                }
                model.Universities = list.ToArray();
            }

            {
                ViewBag.IncludeCKEditor = true;
                ViewBag.LargerWidth = true;

                var list = DbContext.Colleges
                               .Where(c => c.UniversityId == model.UniversityId)
                               .ToArray()
                               .Select(c => new CollegeViewModel
                               {
                                   Name = c.LocalizedName(),
                                   Id = c.Id,
                               })
                               .OrderBy(c => c.Name)
                               .ToList();

                if (!isResearcher && !isCommitteeMember)
                {
                    list.Add(new CollegeViewModel { Name = LangHelpers.Instance.GetLangValueByString("other", false, null), Id = 0 });
                }

                model.Colleges = list;
            }

            {
                var values = Utils.QualificationTypesByResearcherType();
                var list = new List<SelectListItem>();

                var i = 0;
                foreach (var v in values)
                {
                    if (v.ToString() == "Unspecified") continue;

                    list.Add(new SelectListItem
                    {
                        Value = v.ToString(),
                        Text = LangHelpers.Instance[v.ToString()]
                    });

                    i++;
                }
                model.QualificationTypes = list.ToArray();
            }

            {
                var list = DbContext.Countries
                    .ToArray()
                    .OrderBy(c => c.Id == 1 ? 0 : 1)
                    .ThenBy(c => c.LocalizedName())
                     .Select(c => new SelectListItem
                     {
                         Text = c.LocalizedName(),
                         Value = c.Id.ToString()
                     })
                    .ToList();

                model.Countries = list.ToArray();
            }

            {
                var list = DbContext.Cities

                    .ToArray()
                    .Select(c => new CityViewModel
                    {
                        Id = c.Id,
                        CountryId = c.CountryId,
                        Name = c.LocalizedName(),
                    })
                    .OrderBy(c => c.Name)
                    .ToList();

                list.Add(new CityViewModel { Name = LangHelpers.Instance.GetLangValueByString("other", false, null), Id = 0 });

                model.Cities = list;
            }

            {
                var list = DbContext.Banks
                    .ToArray()
                    .Select(c => new SelectListItem
                    {
                        Text = c.LocalizedName(),
                        Value = c.Id.ToString()
                    })
                    .OrderBy(c => c.Text)
                    .ToList();

                list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, null), Value = "0" });

                model.Banks = list.ToArray();
            }

            Utils.SwitchCulture(null);
        }

        private void GetDataFromSession(ref RegistrationViewModel model, bool overwriteCurrentStep = false)
        {
            if (model == null) model = new RegistrationViewModel();

            var db = Utils.db;
            var session = UserSession;
            var data = db.UserSessionDatas.Where(x => x.SessionId == UserSession.Id).ToList();

            model.LastSavedStep = GetKeyValue<int?>("LastSavedStep", data);
            model.ExternalRefereeHashParams = GetKeyValue<string>("ExternalRefereeHashParams", data);

            if (overwriteCurrentStep || model.Step != 1)
            {
                model.NationalId = GetKeyValue<string>("NationalId", data);
                model.FullName = GetKeyValue<string>("FullName", data);
                model.UniversityOther = GetKeyValue<string>("UniversityOther", data);
                model.CollegeOther = GetKeyValue<string>("CollegeOther", data);
                model.DepartmentOther = GetKeyValue<string>("DepartmentOther", data);
                model.MajorOther = GetKeyValue<string>("MajorOther", data);
                model.BirthDate = GetKeyValue<string>("BirthDate", data);
                model.UniversityId = GetKeyValue<int?>("UniversityId", data);
                model.UniversityName = GetKeyValue<string>("UniversityName", data);
                model.MajorId = GetKeyValue<int?>("MajorId", data);
                model.NationalityId = GetKeyValue<int?>("NationalityId", data);
                model.CountryId = GetKeyValue<int?>("CountryId", data);
                model.CollegeId = GetKeyValue<int?>("CollegeId", data);
                model.Address = GetKeyValue<string>("Address", data);
                model.CityId = GetKeyValue<int?>("CityId", data);
                model.Qualification = GetKeyValue<QualificationType?>("Qualification", data);
                model.DepartmentID = GetKeyValue<int?>("DepartmentID", data);
                model.Specialization = GetKeyValue<string>("Specialization", data);
            }

            if (overwriteCurrentStep || model.Step != 2)
            {
                model.Bank = GetKeyValue<int?>("Bank", data);
                var fileId = GetKeyValue<string>("BankIbanPhotocopyReadonly", data);
                if (fileId != null)
                {
                    model.BankIbanPhotocopyReadonly = db.UserSessionFiles
                                                    .Where(f => f.Id.ToString() == fileId && f.SessionId == UserSession.Id)
                                                    .ToList()
                                                    .Select(c => new AttachedFile
                                                    {
                                                        FileId = c.Id.ToString(),
                                                        Type = (FileType)c.Type,
                                                        FileName = Path.GetFileName(c.FileName)
                                                    })
                                                    .FirstOrDefault();
                }

                model.BankOther = GetKeyValue<string>("BankOther", data);
                model.BankName = GetKeyValue<string>("BankName", data);
                model.BankBranch = GetKeyValue<string>("BankBranch", data);
                model.BankAccountHolderName = GetKeyValue<string>("BankAccountHolderName", data);
                model.BankAccountIBAN = GetKeyValue<string>("BankAccountIBAN", data);
            }

            if (overwriteCurrentStep || model.Step != 3)
            {
                model.SectionPublishedPapers = GetKeyValue<string>("SectionPublishedPapers", data);
                model.SectionInterests = GetKeyValue<string>("SectionInterests", data);
                model.SectionCareerHistory = GetKeyValue<string>("SectionCareerHistory", data);
                model.SectionAcademicHistory = GetKeyValue<string>("SectionAcademicHistory", data);
                model.SectionTrainingCourses = GetKeyValue<string>("SectionTrainingCourses", data);
                model.SectionAwards = GetKeyValue<string>("SectionAwards", data);
                model.SectionMemberships = GetKeyValue<string>("SectionMemberships", data);
                model.CvText = GetKeyValue<string>("CvText", data);
            }

            if (overwriteCurrentStep || model.Step != 4)
            {
                model.Email = GetKeyValue<string>("Email", data);
                model.MobileNumber = GetKeyValue<string>("MobileNumber", data);
            }
        }

        private void SaveDataToSession(RegistrationViewModel model)
        {
            if (model == null) return;

            var session = UserSession;

            var db = Utils.db;
            var data = db.UserSessionDatas.Where(x => x.SessionId == UserSession.Id).ToList();

            if (model.Step == 1)
            {
                SaveKeyValue<string>("NationalId", model.NationalId, data);
                SaveKeyValue<string>("FullName", model.FullName, data);
                SaveKeyValue<string>("UniversityOther", model.UniversityOther, data);
                SaveKeyValue<string>("CollegeOther", model.CollegeOther, data);
                SaveKeyValue<string>("DepartmentOther", model.DepartmentOther, data);
                SaveKeyValue<string>("MajorOther", model.MajorOther, data);
                SaveKeyValue<string>("BirthDate", model.BirthDate, data);
                SaveKeyValue<int?>("UniversityId", model.UniversityId, data);
                SaveKeyValue<string>("UniversityName", model.UniversityName, data);
                SaveKeyValue<int?>("MajorId", model.MajorId, data);
                SaveKeyValue<int?>("NationalityId", model.NationalityId, data);
                SaveKeyValue<int?>("CountryId", model.CountryId, data);
                SaveKeyValue<int?>("CollegeId", model.CollegeId, data);
                SaveKeyValue<string>("Address", model.Address, data);
                SaveKeyValue<int?>("CityId", model.CityId, data);
                SaveKeyValue<QualificationType?>("Qualification", model.Qualification, data);
                SaveKeyValue<int?>("DepartmentID", model.DepartmentID, data);
                SaveKeyValue<string>("Specialization", model.Specialization, data);
            }
            else if (model.Step == 2)
            {
                SaveKeyValue<int?>("Bank", model.Bank, data);

                var file = (UserSessionFile)null;
                var files = Request.Files;
                for (var i = 0; i < files.AllKeys.Count(); i++)
                {
                    var ff = Request.Files[i];

                    if (!(ff.ContentLength > 0)) continue;

                    var fileName = Path.GetFileName(ff.FileName);
                    var extension = Path.GetExtension(fileName);
                    var fileType = (byte)FileType.IbanPhotocopy;

                    file = new Data.DbModels.UserSessionFile
                    {
                        Id = Guid.NewGuid(),
                        Type = fileType,
                        CreatedBy = UserId,
                        FileName = Path.GetFileName(ff.FileName),
                        Extension = extension,
                        SessionId = UserSession.Id,
                        Created = TM.Now
                    };

                    file.FilePath = Path.Combine(Server.MapPath("~/Content/files/"), file.Id + extension);
                    ff.EnhancedSaveAs(file.FilePath);

                    db.UserSessionFiles.Add(file);

                    db.SaveChanges();

                    break;
                }

                if (file != null)
                {
                    SaveKeyValue<string>("BankIbanPhotocopyReadonly", file.Id.ToString(), data);
                }

                SaveKeyValue<string>("BankOther", model.BankOther, data);
                SaveKeyValue<string>("BankName", model.BankName, data);
                SaveKeyValue<string>("BankBranch", model.BankBranch, data);
                SaveKeyValue<string>("BankAccountHolderName", model.BankAccountHolderName, data);
                SaveKeyValue<string>("BankAccountIBAN", model.BankAccountIBAN, data);
            }
            else if (model.Step == 3)
            {
                SaveKeyValue<string>("SectionPublishedPapers", model.SectionPublishedPapers, data);
                SaveKeyValue<string>("SectionInterests", model.SectionInterests, data);
                SaveKeyValue<string>("SectionCareerHistory", model.SectionCareerHistory, data);
                SaveKeyValue<string>("SectionAcademicHistory", model.SectionAcademicHistory, data);
                SaveKeyValue<string>("SectionTrainingCourses", model.SectionTrainingCourses, data);
                SaveKeyValue<string>("SectionAwards", model.SectionAwards, data);
                SaveKeyValue<string>("SectionMemberships", model.SectionMemberships, data);
                SaveKeyValue<string>("CvText", model.CvText, data);
            }
            else if (model.Step == 4)
            {
                SaveKeyValue<string>("Email", model.Email, data);
                SaveKeyValue<string>("MobileNumber", model.MobileNumber, data);
            }

            SaveKeyValue<int?>("LastSavedStep", model.Step, data);
        }
    }
}