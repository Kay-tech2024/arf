using ARF.Core;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using ARF.Web.App_Start.Code;
using ARF.Web.Models;
using ARF.Web.Resources;
using Humanizer;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [CustomAuthorize]
    public class AjaxController : BaseController
    {

        //[HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult LoadRequestSummaryPartial(long RequestId)
        {
            return PartialView("~/Views/Requests/_RequestSummaryPartial.cshtml", WebUtils.GetRequestSummary(RequestId));
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LoadRefereeRequestFeedbackFormPartial(long RequestId, int RefereeId, ReqFieldType FieldType)
        {
            return PartialView("~/Views/Requests/_RefereeRequestFeedbackFormQuestionsPartial.cshtml", WebUtils.GetRefereeFundingRequestFeedbackDetails(RequestId, RefereeId, FieldType, true));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void SavePreference(UserPreferenceModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Key) || string.IsNullOrWhiteSpace(model.Value))
                return;

            var key = model.Key.Trim().ToUpper();
            var value = model.Value.Trim().ToUpper();
            bool save = false;
            bool ignore = false;
            bool userAgentImportant = false;

            switch (key)
            {
                case "LANG":
                    switch (value)
                    {
                        case "AR":
                        case "EN":

                            save = true;
                            break;
                    }

                    break;

                case "MENU":

                    userAgentImportant = true;

                    switch (value)
                    {
                        case "COLLARFED":
                        case "EXPANDED":
                            save = true;
                            break;

                        case "DONTCARE":
                            ignore = true;
                            break;
                    }
                    break;
            }

            if (ignore) return;

            if (save)
            {
                var prefs = DbContext.UserPreferences.Where(p => p.UserId == CurrentUser.Id && p.Key == key).ToList();

                if (prefs.Count == 0 && !ignore)
                {
                    var pref = new UserPreference
                    {
                        UserId = CurrentUser.Id,
                        Key = key,
                        UserAgent = Request.UserAgent,
                        Value = value
                    };

                    DbContext.UserPreferences.Add(pref);
                }
                else
                {
                    if (userAgentImportant)
                    {
                        var pref = prefs.FirstOrDefault(p => string.IsNullOrWhiteSpace(p.UserAgent) || p.UserAgent == Request.UserAgent);

                        if (pref == null && !ignore)
                        {
                            pref = new UserPreference
                            {
                                UserId = CurrentUser.Id,
                                Key = key,
                                UserAgent = Request.UserAgent,
                                Value = value
                            };

                            DbContext.UserPreferences.Add(pref);
                        }
                        else
                        {
                            pref.UserAgent = Request.UserAgent;
                            pref.Value = value;
                        }
                    }
                    else
                    {
                        prefs.First().Value = value;
                    }
                }

                DbContext.SaveChanges();
            }
        }

        [AllowAnonymous]
        public JsonResult JustSawIt(int id)
        {
            if (!(id > 0)) return Json(false);

            var ad = Utils.db.Advertisements.FirstOrDefault(x => x.Id == id);

            if (ad != null)
            {
                var userId = Utils.UserUtils.CurrentUser == null ? null : Utils.UserUtils.CurrentUser.Id;
                var sessionId = Utils.UserUtils.UserSession.Id;

                ad.AdvertisementViews.Add(new AdvertisementView
                {
                    AdvertisementId = id,
                    UserId = UserId,
                    Date = TM.Now,
                    SessionId = sessionId,
                    Browser = Request.UserAgent,
                    IpAddress = Utils.GetUserIP()
                });

                DbContext.Configuration.ValidateOnSaveEnabled = false;
                DbContext.SaveChanges();

                return Json(true);
            }

            return Json(false);
        }

        [AllowAnonymous]
        public JsonResult SendVerificationCode(string to, CodeType? type, bool? imamOnly)
        {
            if (string.IsNullOrWhiteSpace(to) || !(type > 0)) return Json(new { success = false, msg = LangHelpers.Instance["INVALID_REQUEST"] });

            if (CurrentUser != null)
            {
                return Json(new { success = false, msg = LangHelpers.Instance["ALREADY_LOGGED_IN"] });
            }

            if (type == CodeType.SMS)
            {
                var validPhone = "";
                if (Utils.ValidateMobile(to, out validPhone) && UserSession.VerifiedMobile == validPhone)
                {
                    return Json(new { success = false, msg = LangHelpers.Instance["ALREADY_VERIFIED"], isVerified = true });
                }

                to = validPhone;
            }
            else if (type == CodeType.Email)
            {
                to = to.Trim().ToLower();
                if (Utils.ValidateEmail(to))
                {
                    if (UserSession.EmailVerified == to)
                    {
                        return Json(new { success = false, msg = LangHelpers.Instance["ALREADY_VERIFIED"], isVerified = true });
                    }

                    if (imamOnly == true && !to.EndsWith("@imamu.edu.sa") && !to.EndsWith(".imamu.edu.sa") && !to.EndsWith("kay-ict.com"))
                    {
                        return Json(new { success = false, msg = LangHelpers.Instance["email_must_be_imam_edu"], isVerified = true });
                    }
                }

                if (Utils.db.Users.Count(u => u.Email == to) > 0)
                {
                    return Json(new { success = false, msg = LangHelpers.Instance["EMAIL_EXISTS"] });

                }
            }



            if (!Utils.IsLocalhost)
            {

                if (type == CodeType.SMS && UserSession.LastSmsCodeGenerated != null && UserSession.LastSmsCodeGenerated.Value.AddMinutes(2) > TM.Now)
                {
                    return Json(new { success = false, msg = LangHelpers.Instance["PLEASE_WAIT_FOR_2_MINUTES"] });
                }

                if (type == CodeType.Email && UserSession.LastEmailCodeGenerated != null && UserSession.LastEmailCodeGenerated.Value.AddMinutes(2) > TM.Now)
                {
                    return Json(new { success = false, msg = LangHelpers.Instance["PLEASE_WAIT_FOR_2_MINUTES"] });
                }
            }

            var gKey = "";
            var err = "";
            if (!GenerateAndSendVerificationCode(type.Value, to, out err, out gKey))
            {
                return Json(new { success = false, msg = err });
            }

            return Json(new { success = true, key = gKey });


        }


        [AllowAnonymous]
        public JsonResult VerifyCode(string to, CodeType? type, string key, string code)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(to) || !(type > 0)) return Json(new { success = false, msg = LangHelpers.Instance["INVALID_REQUEST"] });

            if (CurrentUser != null)
            {
                return Json(new { success = false, msg = LangHelpers.Instance["ALREADY_LOGGED_IN"] });
            }


            if (type == CodeType.SMS)
            {
                var validPhone = "";
                if (Utils.ValidateMobile(to, out validPhone) && UserSession.VerifiedMobile == validPhone)
                {
                    return Json(new { success = true, msg = LangHelpers.Instance["ALREADY_VERIFIED"], isVerified = true });
                }

                to = validPhone;
            }
            else if (type == CodeType.Email)
            {
                to = to.Trim().ToLower();
                if (Utils.ValidateEmail(to) && UserSession.EmailVerified == to)
                {
                    return Json(new { success = true, msg = LangHelpers.Instance["ALREADY_VERIFIED"], isVerified = true });
                }
            }

            var db = Utils.db;
            var link = db.UserTemporaryUrls.FirstOrDefault(l => l.Key == key && l.SessionId == UserSession.Id);

            if (link == null)
            {
                return Json(new { success = false, msg = LangHelpers.Instance["NOT_FOUND"] });
            }

            if (link.IsUsed == true)
            {
                return Json(new { success = false, msg = LangHelpers.Instance["ALREADY_USED"] });
            }

            if (link.Expires < TM.Now)
            {
                return Json(new { success = false, msg = LangHelpers.Instance["EXPIRED"] });
            }

            var codeInt = (int)0;
            if (int.TryParse(code, out codeInt) && link.ShortCode != null && link.ShortCode == codeInt)
            {
                link.IsUsed = true;
                if (type == CodeType.SMS)
                    UserSession.VerifiedMobile = to;
                else
                    UserSession.EmailVerified = to;

                db.SaveChanges();

                return Json(new { success = true });
            }



            return Json(new { success = false, msg = LangHelpers.Instance["INVALID_CODE"] });


        }

        [CustomAuthorize(Privilages.requests_view_requests)]
        public JsonResult ListRequests(RequestGridModel model, FilterModel filter)
        {
            if (IsCollegeUser) model.p = null;
            if (IsGroupRole) model.j = null;


            var nin = (string)null;
            if (Request["nin"] == null && IsValidNationalId(Request["nin"]))
            {
                nin = Request["nin"];
            }

            if (model == null) return Json(new
            {
                total = 0,
                rows = new List<RequestViewModel>(),
                current = model.Current,
                rowCount = model.RowCount
            });


            var sortBy = (string)null; ;
            var sortDirection = (string)null;
            if (model.Sort != null && model.Sort.Keys.Count > 0)
            {
                var key = model.Sort.Keys.First();
                var value = model.Sort[key];

                if (model.Sort != null && model.Sort.Count > 0)
                {
                    sortBy = key;
                    sortDirection = value;
                }
            }

            int take = 0;
            if (model.RowCount <= 0)
            {
                take = 20;
            }
            else
            {
                take = model.RowCount;
            }

            if (model.Current <= 0)
            {
                model.Current = 1;
            }

            int from = (model.Current - 1) * model.RowCount;
            long total = 0;

            var query = RequestUtils.GetRequests(
                model.p,
                model.j,
                model.u,
                null,
                model.Status,
                model.SearchPhrase,
                model.Status == RequestTab.Archive ? model.Current : 0,
                model.Status == RequestTab.Archive ? model.RowCount : 0,
                sortBy,
                sortDirection,
                LangHelpers.Instance, new Core.Models.FilterModel
                {
                    Field = filter.Field,
                    College = filter.College,
                    Statustype = filter.Statustype,
                    Requesttype = filter.Requesttype,
                    Startdate = filter.Startdate,
                    Enddate = filter.Enddate,
                    Title = filter.Title,
                    Researcher = filter.Researcher
                },
                out total
            );

            var data = query
                .ToList().Select((r, index) => new
                {
                    Id = r.Id,
                    UniqueId = r.UniqueId == null ? r.Id.ToString() : r.UniqueId,
                    Title = r.Title,
                    DepartmentName = r.CreatedByUser != null && r.CreatedByUser.Researchers.Count > 0 && r.CreatedByUser.Researchers.FirstOrDefault().Department != null ? r.CreatedByUser.Researchers.FirstOrDefault().Department.NameArabic : "",
                    CollegeName = r.College != null ? r.College.LocalizedName() : "",
                    EmployeeId = r.CreatedByUser.UserProfile == null ? "" : r.CreatedByUser.UserProfile.EmployeeId,
                    Name = r.CreatedByUser.UserProfile == null ? "" : r.CreatedByUser.FullName,
                    Type = LangHelpers.Instance[(((Data.RequestType)r.Type).ToString())],
                    DateCreated = r.Created.Humanize(false, null, CultureInfo.GetCultureInfo("ar-SA")),
                    DateCreatedActual = r.Created.FormatDate(true, true),
                    DateUpdated = r.Updated.Humanize(false, null, CultureInfo.GetCultureInfo("ar-SA")),
                    DateUpdatedActual = r.Updated.FormatDate(true, true),
                    StatusCode = r.Status,
                    AllowToDelete = ((Utils.HasRolesAny(ArfUserRole.SystemAdmin) || (r.CreatedBy == CurrentUser.Id && Utils.HasRolesAny(ArfUserRole.PrincipalResearcher))) && r.Type == (byte)Data.RequestType.FundingRequest && (r.Status == 0 || r.Status == (long)RequestStatus.NotSubmitted)),
                    Status = (r.College != null && r.Status == (long)RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies) ? WebUtils.GetViceDeanForHigherStudiesAndScientificResearchRequestStatusName(((RequestStatus)r.Status).ToString(), r.College.ResearchUnitCode.Value) : LangHelpers.Instance[(((RequestStatus)r.Status).ToString())],
                    Actionable = RequestUtils.GetRequestActions(r).Count > 0,
                    Updated = r.Updated,
                    Index = index
                })
                .ToList();

            if (Utils.HasRolesAny(ArfUserRole.ResearchUnitMember))
            {
                data.RemoveAll(x => x.Actionable == false);
            }

            if (model.Status == RequestTab.Pending)
            {
                data = data.OrderBy(r => r.Actionable == true ? 0 : 1)
                            .ThenBy(r => r.Index)
                            .Skip((model.Current - 1) * model.RowCount)
                            .Take(model.RowCount)
                            .ToList();
            }

            return Json(new
            {
                total = total,
                rows = data,
                current = model.Current,
                rowCount = model.RowCount
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult FindRefereeByEmail(string RefereeEmail)
        {
            var _response = new AjaxResponse();
            try
            {
                var referee = Utils.db.Referees.Where(x => x.Email == RefereeEmail && x.IsDeleted == false).FirstOrDefault();
                if (referee != null)
                {
                    _response.Data = new
                    {
                        Id = referee.Id,
                        FirstName = referee.FirstName,
                        SecondName = referee.SecondName,
                        ThirdName = referee.ThirdName,
                        LastName = referee.LastName,
                        MobileNumber = referee.MobileNo,
                        UserId = referee.UserId
                    };
                }
            }
            catch (Exception ex)
            {
                _response.Status = false;
                _response.Message = ex.Message;
            }

            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetUserDetailsByEmail(string Email)
        {
            var _response = new AjaxResponse();
            var userDetails = Utils.db.Users.Include("Roles").Include("UserProfile").SingleOrDefault(u => u.Email == Email);
            if (userDetails != null)
            {
                var userRoles = userDetails.Roles.Select(r => r.Name).ToList();
                var roles = new List<ArfUserRole>();
                foreach (var r in userRoles)
                {
                    var role = ArfUserRole.None;

                    if (Enum.TryParse(r, out role) && role != ArfUserRole.None)
                    {
                        roles.Add(role);
                    }
                }

                dynamic _userDetailsRetObj = new System.Dynamic.ExpandoObject();
                _userDetailsRetObj.FullName = userDetails.UserProfile.GetFullName();
                _userDetailsRetObj.PhoneNumber = userDetails.UserProfile.MobileNo;
                _userDetailsRetObj.Email = userDetails.Email;
                _userDetailsRetObj.UserName = userDetails.UserName;
                _userDetailsRetObj.Id = userDetails.Id;
                _userDetailsRetObj.NationalId = userDetails.UserProfile.NationalId;
                _userDetailsRetObj.Role = roles.Count > 0 ? LangHelpers.Instance[roles[0].ToString()] : "";
                _response.Data = _userDetailsRetObj;
            }
            else
            {
                _response.Status = false;
                _response.Message = LangHelpers.Instance["user_does_not_exists"];
            }
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveExternalReferee(ExternalRefereeViewModel _model)
        {
            var _response = new AjaxResponse();
            try
            {
                _response.Status = true;
                var util = PhoneNumberUtil.GetInstance();
                var number = (PhoneNumber)null;
                try
                {
                    number = util.Parse(_model.MobileNumber, "SA");
                }
                catch (NumberParseException)
                {
                    _response.Status = false;
                    ModelState.AddModelError("MobileNumber", LangHelpers.Instance["invalid_mobile_number"]);
                }

                if (ModelState.IsValid)
                {
                    var cmps = Utils.GetNameComponents(_model.FullName);
                    var referee = Utils.db.Referees.Where(x => x.Email == _model.Email && x.IsDeleted == false).FirstOrDefault();
                    if (referee == null)
                    {
                        referee = new Referee
                        {
                            FirstName = cmps.FirstName,
                            SecondName = cmps.SecondName,
                            ThirdName = cmps.ThirdName,
                            LastName = cmps.LastName,
                            Email = _model.Email,

                            UserId = null,
                            Created = DateTime.Now,
                            CreatedBy = CurrentUser.Id,
                            Updated = DateTime.Now,
                            UpdatedBy = CurrentUser.Id,
                        };

                        if (number != null)
                        {
                            referee.MobileNo = number.CountryCode + "" + number.NationalNumber;
                        }

                        DbContext.Referees.Add(referee);
                        DbContext.SaveChanges();
                    }
                    _response.Data = referee.Id;

                }
                else
                {
                    _response.Status = false;
                    _response.Message = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                }

            }
            catch (Exception ex)
            {
                _response.Status = false;
                _response.Message = ex.Message;
            }

            return Json(_response);
        }

        [CustomAuthorize(ArfUserRole.SystemAdmin, ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch, ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects)]
        public JsonResult ListUsers(UsersGridModel model, ArfUserRole? role)
        {
            if (role == ArfUserRole.Researcher)
            {
                role = ArfUserRole.PrincipalResearcher;
            }

            if (model == null) return Json(new
            {
                total = 0,
                rows = new List<User>(),
                current = model.Current,
                rowCount = model.RowCount
            });

            var sortBy = "userid";
            var sortDirection = SortDirection.Ascending;

            if (model.Sort != null && model.Sort.Count > 0)
            {
                var key = model.Sort.Keys.First();
                var value = model.Sort[key];
                sortBy = key;
                //  if (Enum.TryParse(key.ToPascalCase(), out sortBy))
                //   {
                sortDirection = value == "asc" ? SortDirection.Ascending : SortDirection.Descending;
                //   }
            }

            if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch))
            {
                role = ArfUserRole.ResearchUnitMember;
            }
            else if (Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects))
            {
                role = ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects;
            }

            var collegeId = 0;
            if (Utils.IsCollegeUser)
            {
                if (CurrentUser.CollegeId == null)
                {
                    collegeId = -1;
                }
                else
                {
                    collegeId = CurrentUser.CollegeId.Value;
                }
            }
            string RoleId = String.Empty;
            if (role != null)
            {
                RoleId = Utils.db.Roles.FirstOrDefault(r => r.Name == role.ToString()).Id;
            }
            sortBy = sortBy.ToLower();
            IQueryable<User> query = UserUtils.GetUsers(model.SearchPhrase, false, LangHelpers.Instance, true)
                .Where(u => role == null || u.Roles.Count(r => r.Id == RoleId) > 0 || u.UserExtraRoles.Count(l => l.RoleId == RoleId && l.IsActive == true) > 0)
                .Where(u => (collegeId == 0 || u.CollegeId == collegeId || u.UserExtraRoles.Count(l => l.CollegeId == collegeId) > 0));

            switch (sortBy)
            {
                case "userid":
                    query = sortDirection == SortDirection.Ascending ? query.OrderBy(a => a.Id) : query.OrderByDescending(a => a.Id);
                    break;

                case "username":
                    query = sortDirection == SortDirection.Ascending ? query.OrderBy(a => a.UserName) : query.OrderByDescending(a => a.UserName);
                    break;

                case "email":
                    query = sortDirection == SortDirection.Ascending ? query.OrderBy(a => a.Email) : query.OrderByDescending(a => a.Email);
                    break;

                default:
                case "updated":
                    query = sortDirection == SortDirection.Ascending ? query.OrderBy(a => a.Updated) : query.OrderByDescending(a => a.Updated);
                    break;
            }

            int take = 0;
            if (model.RowCount <= 0)
            {
                take = 20;
            }
            else
            {
                take = model.RowCount;
            }

            if (model.Current <= 0)
            {
                model.Current = 1;
            }
            int from = (model.Current - 1) * model.RowCount;

            var data = query.Skip(from).Take(take).ToList();
            var users = data.Select(a => new
            {
                UserId = a.Id,
                FullName = a.UserProfile.GetFullName(),
                RoleId = a.Roles.Count > 0 ? a.Roles.FirstOrDefault().Name : "",
                UserName = a.UserName,
                Email = a.Email,
                IsActivated = a.IsActivated ? LangHelpers.Instance["activated"] : LangHelpers.Instance["deactivated"],
                CollegeName = a.CollegeId != null ? a.College.NameArabic : "",
                RoleName = WebUtils.GetRoleNameOfUser(a, role),
                CvFileName = a.UserFiles.Where(x => x.Type == (byte)FileType.Resume).OrderByDescending(x => x.Created).FirstOrDefault() == null ? "" : a.UserFiles.Where(x => x.Type == (byte)FileType.Resume).OrderByDescending(x => x.Created).FirstOrDefault().FileName,
                CvKey = a.UserFiles.Where(x => x.Type == (byte)FileType.Resume).OrderByDescending(x => x.Created).FirstOrDefault() == null ? "" : a.UserFiles.Where(x => x.Type == (byte)FileType.Resume).OrderByDescending(x => x.Created).FirstOrDefault().Id.ToString()
            }).ToList();

            return Json(new
            {
                total = query.LongCount(),
                rows = users,
                current = model.Current,
                rowCount = model.RowCount
            });
        }

        [CustomAuthorize]
        public JsonResult ListReferees(UsersGridModel model)
        {

            if (model == null) return Json(new
            {
                total = 0,
                rows = new List<User>(),
                current = model.Current,
                rowCount = model.RowCount
            });

            var sortBy = "userid";
            var sortDirection = SortDirection.Ascending;

            if (model.Sort != null && model.Sort.Count > 0)
            {
                var key = model.Sort.Keys.First();
                var value = model.Sort[key];
                sortBy = key;
                sortDirection = value == "asc" ? SortDirection.Ascending : SortDirection.Descending;
            }


            var collegeId = 0;
            if (Utils.IsCollegeUser)
            {
                if (CurrentUser.CollegeId == null)
                {
                    collegeId = -1;
                }
                else
                {
                    collegeId = CurrentUser.CollegeId.Value;
                }
            }

            sortBy = sortBy.ToLower();

            IQueryable<Referee> query = UserUtils.GetReferees(model.SearchPhrase, false, LangHelpers.Instance, model.FilterUserId);

            switch (sortBy)
            {
                case "userid":
                    query = sortDirection == SortDirection.Ascending ? query.OrderBy(a => a.Id) : query.OrderByDescending(a => a.Id);
                    break;

                case "name":
                    query = sortDirection == SortDirection.Ascending ? query.OrderBy(a => a.FirstName).ThenBy(a => a.SecondName).ThenBy(a => a.ThirdName).ThenBy(a => a.LastName) : query.OrderByDescending(a => a.LastName).ThenByDescending(a => a.ThirdName).ThenByDescending(a => a.SecondName).ThenByDescending(a => a.FirstName);
                    break;

                case "email":
                    query = sortDirection == SortDirection.Ascending ? query.OrderBy(a => a.Email) : query.OrderByDescending(a => a.Email);
                    break;

                default:
                case "updated":
                    query = sortDirection == SortDirection.Ascending ? query.OrderBy(a => a.Updated) : query.OrderByDescending(a => a.Updated);
                    break;
            }

            int take = 0;
            if (model.RowCount <= 0)
            {
                take = 20;
            }
            else
            {
                take = model.RowCount;
            }

            if (model.Current <= 0)
            {
                model.Current = 1;
            }
            int from = (model.Current - 1) * model.RowCount;

            var data = query.Skip(from).Take(take).ToList();
            var users = data.Select(a => new
            {
                Id = a.Id,
                UserId = a.UserId,
                FullName = a.GetFullName(),
                UserName = a.User != null ? a.User.UserName : "",
                Email = a.Email,
                Mobile = a.MobileNo,
                IsActivated = a.User != null && a.User.IsActivated ? LangHelpers.Instance["activated"] : LangHelpers.Instance["deactivated"],
                CollegeName = a.CollegeId != null ? a.College.NameArabic : "",
                RoleName = LangHelpers.Instance["referee"].ToString(),
                CvFileName = a.User == null || a.User.UserFiles.Where(x => x.Type == (byte)FileType.Resume).OrderByDescending(x => x.Created).FirstOrDefault() == null ? "" : a.User.UserFiles.Where(x => x.Type == (byte)FileType.Resume).OrderByDescending(x => x.Created).FirstOrDefault().FileName,
                CvKey = a.User == null || a.User.UserFiles.Where(x => x.Type == (byte)FileType.Resume).OrderByDescending(x => x.Created).FirstOrDefault() == null ? "" : a.User.UserFiles.Where(x => x.Type == (byte)FileType.Resume).OrderByDescending(x => x.Created).FirstOrDefault().Id.ToString(),
                MajorName = a.MajorId == null ? "" : a.Major.NameArabic,

            }).ToList()

            .Select(a => new
            {
                Id = a.Id,
                UserId = a.UserId,
                FullName = a.FullName,
                UserName = a.UserName,
                Email = a.Email,
                Mobile = a.Mobile,
                IsActivated = a.IsActivated,
                CollegeName = a.CollegeName,
                MajorName = a.MajorName,
                RoleName = LangHelpers.Instance[a.RoleName],
                CvFileName = a.CvFileName,
                CvKey = a.CvKey
            }).ToList();

            return Json(new
            {
                total = query.LongCount(),
                rows = users,
                current = model.Current,
                rowCount = model.RowCount
            });
        }

        [AllowAnonymous]
        public ActionResult GetMajors(int? collegeId)
        {
            var college = (College)null;
            if (collegeId <= 0 || (college = DbContext.Colleges.SingleOrDefault(r => r.Id == collegeId)) == null)
            {
                return Json(new List<SelectListItem>());
            }


            return Json(college.Majors.OrderBy(c => IsArabic ? c.NameArabic : c.NameEnglish).Select(c => new { text = IsArabic ? c.NameArabic : c.NameEnglish, value = c.Id }).ToList());

        }

        [AllowAnonymous]
        public ActionResult GetCategories(long? categoryId, int? level)
        {
            if (categoryId > 0)
            {
                var cat = (Category)null;
                if (categoryId <= 0 || (cat = DbContext.Categories.SingleOrDefault(r => r.Id == categoryId)) == null)
                {
                    return Json(new List<SelectListItem>());
                }

                return Json(
                            cat.Categories
                            .Where(c => level == null || c.Level == level.Value)
                            .ToList()
                            .Select(c => new { text = c.LocalizedName(), value = c.Id })
                            .OrderBy(c => c.text)
                            .ToList()
                        );
            }
            else
            {
                return Json(
                           DbContext.Categories
                           .Where(c => level == null || c.Level == level.Value)
                           .ToList()
                           .Select(c => new { text = c.LocalizedName(), value = c.Id })
                           .OrderBy(c => c.text)
                           .ToList()
                       );
            }
        }

        [AllowAnonymous]
        public JsonResult GetFields()
        {
            List<DropDown> list = new List<DropDown>();
            var values = Enum.GetValues(typeof(ReqFieldType));
            foreach (ReqFieldType v in values)
            {
                int x = (int)v;
                string str = v.ToString();
                if (str != ReqFieldType.Unspecified.ToString())
                {
                    list.Add(new DropDown { Value = LangHelpers.Instance[str], ID = x });
                }
            }
            return Json(list);
        }
        [AllowAnonymous]
        public JsonResult GetRequestType()
        {
            List<DropDown> list = new List<DropDown>();
            var values = Enum.GetValues(typeof(RequestType));

            foreach (RequestType v in values)
            {
                int x = (int)v;
                string str = v.ToString();
                if (str != RequestType.Unspecificed.ToString())
                {
                    list.Add(new DropDown { Value = LangHelpers.Instance[str], ID = x });
                }
            }
            return Json(list);
        }
        [AllowAnonymous]
        public JsonResult GetStatusByRequestId(List<int?> RequestId)
        {
            List<DropDown> list = new List<DropDown>();
            if (RequestId.Count(x => x != null && x != -1) > 0)
            {
                var values = Enum.GetValues(typeof(RequestStatus));
                foreach (var id in RequestId)
                {
                    var Requesttype = Enum.GetName(typeof(RequestType), id);
                    Requesttype = Requesttype.Substring(0, 3);
                    if (Requesttype.ToLower() == "adv")
                    {
                        list.Add(new DropDown { Value = LangHelpers.Instance[RequestStatus.DeansLetterRequestPendingSubmission.ToString()], ID = (long)RequestStatus.DeansLetterRequestPendingSubmission });
                        list.Add(new DropDown { Value = LangHelpers.Instance[RequestStatus.DeansLetterRequestRecievedBy_ViceDean_for_research_centers_and_databases.ToString()], ID = (long)RequestStatus.DeansLetterRequestRecievedBy_ViceDean_for_research_centers_and_databases });
                        list.Add(new DropDown { Value = LangHelpers.Instance[RequestStatus.DeansLetterRecievedBy_DeanOfScientificResearch.ToString()], ID = (long)RequestStatus.DeansLetterRecievedBy_DeanOfScientificResearch });
                    }
                    foreach (RequestStatus v in values)
                    {
                        long x = (long)v;
                        string str = v.ToString();
                        if (str.StartsWith(Requesttype))
                        {
                            list.Add(new DropDown { Value = LangHelpers.Instance[str], ID = x });
                        }
                    }
                }
            }
            list.Add(new DropDown { Value = LangHelpers.Instance[RequestStatus.Pending.ToString()], ID = (long)RequestStatus.Pending });
            list.Add(new DropDown { Value = LangHelpers.Instance[RequestStatus.Rejected.ToString()], ID = (long)RequestStatus.Rejected });
            list.Add(new DropDown { Value = LangHelpers.Instance[RequestStatus.Approved.ToString()], ID = (long)RequestStatus.Approved });
            list.Add(new DropDown { Value = LangHelpers.Instance[RequestStatus.RejectedByResearcher.ToString()], ID = (long)RequestStatus.RejectedByResearcher });
            list.Add(new DropDown { Value = LangHelpers.Instance[RequestStatus.Canceled.ToString()], ID = (long)RequestStatus.Canceled });
            list.Add(new DropDown { Value = LangHelpers.Instance[RequestStatus.Completed.ToString()], ID = (long)RequestStatus.Completed });
            list.Add(new DropDown { Value = LangHelpers.Instance[RequestStatus.NotSubmitted.ToString()], ID = (long)RequestStatus.NotSubmitted });
            return Json(list);
        }
        [AllowAnonymous]
        public JsonResult GetRequestStatus()
        {
            List<DropDown> list = new List<DropDown>();
            var values = Enum.GetValues(typeof(RequestStatus));

            foreach (RequestStatus v in values)
            {
                int x = (int)v;
                string str = v.ToString();
                list.Add(new DropDown { Value = LangHelpers.Instance[str], ID = x });
            }
            return Json(list);
        }
        [AllowAnonymous]
        public JsonResult GetCollegesByField(List<long?> fieldid)
        {
            if (fieldid.Count() > 0)
            {
                List<List<DropDown>> list = new List<List<DropDown>>();
                foreach (var f in fieldid)
                {
                    var data = DbContext.Colleges.Where(x => x.Field == f).Select(x => new DropDown { ID = x.Id, Value = Utils.IsUserLanguageArabic ? x.NameArabic : x.NameEnglish }).ToList();
                    list.Add(data);
                }
                return Json(list);
            }
            else
            {
                var data = DbContext.Colleges.Select(x => new DropDown { ID = x.Id, Value = Utils.IsUserLanguageArabic ? x.NameArabic : x.NameEnglish }).ToList();
                return Json(data);
            }
        }
        [AllowAnonymous]
        public ActionResult GetColleges(long? universityId)
        {

            if (universityId > 0)
            {
                var cat = (University)null;
                if (universityId <= 0 || (cat = DbContext.Universities.SingleOrDefault(r => r.Id == universityId)) == null)
                {
                    return Json(new List<SelectListItem>());
                }


                var deps = cat.Colleges.ToList();

                var switchCulture = false;
                if (Request["type"] != null)
                {
                    if (Request["dontFilter"] != "1")
                    {
                        deps = deps.Where(d => d.Field == null || d.Field == (byte)Enum.Parse(typeof(ReqFieldType), Request["type"])).ToList();
                    }

                    if ((ReqFieldType)(Enum.Parse(typeof(ReqFieldType), Request["type"])) == ReqFieldType.AppliedScienceField)
                    {
                        Utils.SwitchCulture("en");
                        switchCulture = true;
                    }
                }

                var res = Json(
                            deps
                            .Select(c => new { text = c.LocalizedName(), value = c.Id })
                            .OrderBy(c => c.text)
                            .ToList()
                        );

                if (switchCulture)
                {
                    Utils.SwitchCulture();
                }


                return res;
            }
            else
            {
                var deps = DbContext.Colleges.Where(f => false).ToList();

                if (Request["type"] != null)
                {
                    deps = deps.Where(d => d.Field == null || d.Field == (byte)Enum.Parse(typeof(ReqFieldType), Request["type"])).ToList();
                }

                return Json(
                           deps
                           .Select(c => new { text = c.LocalizedName(), value = c.Id })
                           .OrderBy(c => c.text)
                           .ToList()
                       );
            }
        }

        [AllowAnonymous]
        public ActionResult GetDepartments(long? collegeId)
        {

            if (collegeId > 0)
            {
                var cat = (College)null;
                if (collegeId <= 0 || (cat = DbContext.Colleges.SingleOrDefault(r => r.Id == collegeId)) == null)
                {
                    return Json(new List<SelectListItem>());
                }


                var deps = cat.Departments.ToList();

                var switchCulture = false;
                if (Request["type"] != null)
                {
                    if (Request["dontFilter"] != "1")
                    {
                        deps = deps.Where(d => d.Field == null || d.Field == (byte)Enum.Parse(typeof(ReqFieldType), Request["type"])).ToList();
                    }

                    if ((ReqFieldType)(Enum.Parse(typeof(ReqFieldType), Request["type"])) == ReqFieldType.AppliedScienceField)
                    {
                        Utils.SwitchCulture("en");
                        switchCulture = true;
                    }
                }

                var res = Json(
                            deps
                            .Select(c => new { text = c.LocalizedName(), value = c.Id })
                            .OrderBy(c => c.text)
                            .ToList()
                        );

                if (switchCulture)
                {
                    Utils.SwitchCulture();
                }


                return res;
            }
            else
            {
                var deps = DbContext.Departments.Where(f => false).ToList();

                if (Request["type"] != null)
                {
                    deps = deps.Where(d => d.Field == null || d.Field == (byte)Enum.Parse(typeof(ReqFieldType), Request["type"])).ToList();
                }

                return Json(
                           deps
                           .Select(c => new { text = c.LocalizedName(), value = c.Id })
                           .OrderBy(c => c.text)
                           .ToList()
                       );
            }
        }

        [AllowAnonymous]
        public ActionResult GetCities(long? countryId)
        {

            if (countryId > 0)
            {
                var cat = (Country)null;
                if (countryId <= 0 || (cat = DbContext.Countries.SingleOrDefault(r => r.Id == countryId)) == null)
                {
                    return Json(new List<SelectListItem>());
                }



                var cities = cat.Cities.ToList();

                var switchCulture = false;
                if (Request["type"] != null)
                {
                    if ((ReqFieldType)(Enum.Parse(typeof(ReqFieldType), Request["type"])) == ReqFieldType.AppliedScienceField)
                    {
                        Utils.SwitchCulture("en");
                        switchCulture = true;
                    }
                }

                var res = Json(
                            cities
                            .Select(c => new { text = c.LocalizedName(), value = c.Id })
                            .OrderBy(c => c.text)
                            .ToList()
                        );

                if (switchCulture)
                {
                    Utils.SwitchCulture();
                }

                return res;
            }
            else
            {
                var deps = DbContext.Cities.Where(c => false).ToList();

                var switchCulture = false;
                if (Request["type"] != null)
                {
                    if ((ReqFieldType)(Enum.Parse(typeof(ReqFieldType), Request["type"])) == ReqFieldType.AppliedScienceField)
                    {
                        Utils.SwitchCulture("en");
                        switchCulture = true;
                    }
                }

                var res = Json(
                           deps
                           .Select(c => new { text = c.LocalizedName(), value = c.Id })
                           .OrderBy(c => c.text)
                           .ToList()
                       );

                if (switchCulture)
                {
                    Utils.SwitchCulture();
                }

                return res;
            }
        }


        [AllowAnonymous]
        public ActionResult GetPriorities(long? departmentId)
        {
            if (departmentId > 0)
            {
                var cat = (Department)null;
                if (departmentId <= 0 || (cat = DbContext.Departments.SingleOrDefault(r => r.Id == departmentId)) == null)
                {
                    return Json(new List<SelectListItem>());
                }

                return Json(
                            cat.DepartmentPriorities
                            .ToList()
                            .Select(c => new { text = c.Priority.LocalizedName(), value = c.PriorityId })
                            .OrderBy(c => c.text)
                            .ToList()
                        );
            }
            else
            {
                return Json(
                           DbContext.DepartmentPriorities
                           .ToList()
                           .Select(c => new { text = c.Priority.LocalizedName(), value = c.PriorityId })
                           .OrderBy(c => c.text)
                           .ToList()
                       );
            }
        }



        [CustomAuthorize]
        public JsonResult GetChatBuddies()
        {
            return null;
        }

        [CustomAuthorize]
        public JsonResult GetChat(string userId)
        {
            return null;

        }

        [AllowAnonymous]
        public dynamic Heartbeat()
        {
            var isLocked = UserSession != null && UserSession.Locked == true;
            var isExpired = UserSession == null || UserSession.Expires < DateTime.UtcNow || CurrentUser == null || Roles == null || Roles.Length == 0;

            var status = isExpired ? "EXPIRED" : (isLocked ? "LOCKED" : "VALID");
            return Json(new { status = status });
        }


        public class Person
        {
            public string userId;
            public string Name;
            public string LastMessageTime;
            public string LastMessagePreview;
            public string ConversationStartDateTime;
            public List<Message> MessagesList;
        }

        public class Message
        {
            public string Direction;
            public string MessageText;
            public string SentTime;
        }

        public class AjaxResponse
        {
            public AjaxResponse()
            {
                Status = true;
                Message = String.Empty;
            }
            public bool Status { set; get; }
            public string Message { set; get; }
            public object Data { set; get; }
        }

    }
}