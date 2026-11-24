using ARF.Auth;
using ARF.Core;
using ARF.Core.Managers;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using ARF.Web.App_Start.Code;
using ARF.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [CustomAuthorize(Privilages.users_view_users_list)]
    public class UsersController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = GetInstance<UsersManager>()
                .GetUsers()
                .Include("Roles")
                .Include("UserProfile")
                .Include("College")
                .Include("Major")
                .SingleOrDefault(u => u.Id == id);

            if (user == null)
            {
                TempData["message"] = Resources.ArfStrings.could_not_find_any_user+ id;
                TempData["message_type"] = "warning";

                return RedirectToAction("Index");
            }

            var model = new EditUserViewModel
            {
                FullName = user.UserProfile.GetFullName(),
                UserId = user.Id,
                Password = "**********",
                ConfirmPassword = "**********",
                Email = user.Email,
                MobileNo = user.UserProfile.MobileNo,
                CollegeId = user.CollegeId,
                CollegeName = user.CollegeId == null ? null : (IsArabic ? user.College.NameArabic : user.College.NameEnglish),
                RoleId = user.Roles.Count() == 0 ? null : user.Roles.FirstOrDefault().Id,
                RoleName = user.Roles.Count() == 0 ? null : user.Roles.FirstOrDefault().Name,
                IsActivated = user.IsActivated,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                ForcePasswordChange = user.ForcePasswordChange
            };
            return View(model);
        }

        // GET: Admin/Users/Create

        public ActionResult Create()
        {
            var model = new EditUserViewModel();
            var userid = User.Identity.EnhancedGetUserId();
            var user = AuthDbContext.Users.Find(userid);

            //model.Colleges = GetColleges();
            //model.Majors = GetMajors();
            //model.Roles = GetRoles();

            FillOutLookups(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EditUserViewModel model)
        {
            var newUserRoleName = (Role)null;
            var newUserRole = ArfUserRole.None;

            var number = (PhoneNumber)null;
            var birthDate = DateTime.MinValue;

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

            if (!string.IsNullOrWhiteSpace(model.RoleId))
            {
                newUserRoleName = DbContext.Roles.SingleOrDefault(r => r.Id == model.RoleId);
                if (newUserRoleName == null || !Enum.TryParse(newUserRoleName.Name, out newUserRole) || newUserRole == ArfUserRole.None)
                {
                    ModelState.AddModelError("RoleID", Resources.ArfStrings.user_role_is_required);
                }
                else

                {
                    if (IsCollegeUser)
                    {
                        model.CollegeId = CurrentUser.CollegeId;

                        if (IsGroupRole)
                        {
                            model.CollegeId = CurrentUser.CollegeId;
                        }
                    }
                    else
                    {
                        var needsCollege = Utils.IsCollegeRole(newUserRole);
                        var needsMajor = Utils.IsMajorRole(newUserRole);

                        if (needsCollege)
                        {
                            if (model.CollegeId == null)
                            {
                                ModelState.AddModelError("CollegeId", Resources.ArfStrings.colleage_field_is_required);
                            }
                        }
                        else
                        {
                            model.CollegeId = null;
                        }

                        if (needsMajor)
                        {
                            if (model.MajorId == null)
                            {
                                ModelState.AddModelError("MajorId",Resources.ArfStrings.specialization_field_is_required);
                            }
                        }
                        else
                        {
                            model.MajorId = null;
                        }
                    }
                }
            }

            var errorMsg = (string)null;
            if (!WebUtils.ValidatePassword(model.Password, Utils.IsAdmin(newUserRole), out errorMsg))
            {
                ModelState.AddModelError("Password", errorMsg);
            }

            model.ExtraRoles = model.ExtraRoles.Where(r => r.RoleId != null).ToList();
            model.Roles = GetRoles();

            {
                var roles = Utils.db.Roles.ToList();
                var i = 0;

                var dupes = model.ExtraRoles.GroupBy(x => new { x.RoleId }).Where(x => x.Skip(1).Any());
                if (dupes.Count() > 0)
                {
                    ModelState.AddModelError("ExtraRoles_0_", string.Format(LangHelpers.Instance["duplicate_additional_roles"]));
                }
                foreach (var t in model.ExtraRoles)
                {

                    if (model.Roles.Count(r => r.RoleId == t.RoleId) == 0)
                    {
                        ModelState.AddModelError("ExtraRoles_" + i + "_", string.Format(LangHelpers.Instance["role_not_found"]));
                    }
                    else if (t.RoleId == model.RoleId)
                    {
                        ModelState.AddModelError("ExtraRoles_" + i + "_", string.Format(LangHelpers.Instance["extra_role_and_main_role_are_same"]));
                    }
                    else
                    {
                        t.Role = (ArfUserRole)Enum.Parse(typeof(ArfUserRole), roles.FirstOrDefault(r => r.Id == t.RoleId).Name);
                        t.EffectiveFromDate = Utils.StringToDate(t.EffectiveFrom);
                        t.EffectiveToDate = Utils.StringToDate(t.EffectiveTo);

                        if (Utils.IsCollegeRole(t.Role))
                        {
                            if (!(t.CollegeId > 0))
                            {
                                ModelState.AddModelError("ExtraRoles_" + i + "_", string.Format(LangHelpers.Instance["college_required"]));
                            }
                        }
                        else
                        {
                            t.CollegeId = null;
                        }

                        if (t.EffectiveToDate < t.EffectiveFromDate)
                        {
                            ModelState.AddModelError("ExtraRoles_" + i + "_", string.Format(LangHelpers.Instance["effective_to_before_effective_from"]));
                        }

                    }

                    i++;
                }
            }

            var db = Utils.db;
            var user = new User();
            user = db.Users.Where(x => x.UserName == model.UserName || x.Email == model.Email).FirstOrDefault();
            if (user != null)
            {
                if (user.IsDeleted == false)
                {
                    ModelState.AddModelError("Username", LangHelpers.Instance["duplicate_username_or_email"]);
                }
                else
                {
                    user.IsDeleted = false;
                    user.IsActivated = true;
                    db.SaveChanges();

                    TempData["message"] = LangHelpers.Instance["username_or_email_already_exists_with_same_user_in_deleted_form"];
                    TempData["message_type"] = "success";
                    return RedirectToAction("Index", "Users");                    
                }
            }

            if (ModelState.IsValid)
            {
                if (model.UniversityId == null || model.UniversityId == 0)
                {
                    model.UniversityId = 1;
                }

                var hasher = new PasswordHasher();                
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = model.FullName,
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.MobileNo,
                    CountryId = model.CountryId == 0 ? null : model.CountryId,
                    CollegeId = model.CollegeId == 0 ? null : model.CollegeId,
                    CollegeOther = model.CollegeOther,
                    UniversityOther = model.UniversityOther,
                    MajorOther = model.MajorOther,
                    Address = model.Address,
                    PasswordHash = hasher.HashPassword(model.Password),
                    SecurityStamp = "a14ee95e-c73e-466e-a786-aaf4378237d3",
                    AccessFailedCount = 0,
                    CreatedBy = UserId,
                    UpdatedBy = UserId,
                    DepartmentId = model.DepartmentID,
                    UniversityId = model.UniversityId,

                    Roles = new List<Role> { newUserRoleName },
                    Created = TM.Now,
                    Updated = TM.Now,
                    IsActivated = model.IsActivated,
                    EmailConfirmed = model.EmailConfirmed,
                    PhoneNumberConfirmed = model.PhoneNumberConfirmed
                };

                db.Users.Add(user);
                db.SaveChanges();




                {
                    var comps = Utils.GetNameComponents(model.FullName);

                    var userProfile = new UserProfile
                    {
                        UserId = user.Id,
                        Created = TM.Now,
                        CreatedBy = CurrentUser.Id,
                        Updated = TM.Now,
                        UpdatedBy = CurrentUser.Id,

                        FirstName = comps.FirstName,
                        MiddleName = comps.SecondName,
                        ThirdName = comps.ThirdName,
                        LastName = comps.LastName,
                        AddressLine1 = model.Address,
                        BirthDate = birthDate,
                        NationalityId = model.NationalityId,
                        BankName = model.BankName,
                        BankBranch = model.BankBranch,
                        BankIban = model.BankAccountIBAN,
                        BankAccountHolder = model.BankAccountHolderName,
                        NationalId = model.NationalId,

                        CvSectionPublishedPapers = model.SectionPublishedPapers,
                        CvSectionInterests = model.SectionInterests,
                        CvSectionCareerHistory = model.SectionCareerHistory,
                        CvSectionTrainingCourses = model.SectionTrainingCourses,
                        CvSectionAwards = model.SectionAwards,
                        CvSectionMemberships = model.SectionMemberships,
                        BankId = model.Bank,
                        CvSectionAcademicHistory = model.SectionAcademicHistory
                    };

                    if (number != null)
                    {
                        userProfile.MobileNo = "0" + number.NationalNumber;
                        userProfile.MobileNumber = "0" + number.NationalNumber;
                    }

                    userProfile.NationalId = model.NationalId;
                    var cmps = Utils.GetNameComponents(model.FullName);

                    userProfile.FirstName = cmps.FirstName;
                    userProfile.MiddleName = cmps.SecondName;
                    userProfile.ThirdName = cmps.ThirdName;
                    userProfile.LastName = cmps.LastName;

                    DbContext.UserProfiles.Add(userProfile);

                    switch (newUserRole)
                    {
                        case ArfUserRole.PrincipalResearcher:
                            {
                                var researcher = new Researcher
                                {
                                    FirstName = cmps.FirstName,
                                    SecondName = cmps.SecondName,
                                    ThirdName = cmps.ThirdName,
                                    LastName = cmps.LastName,
                                    Email = model.Email,

                                    UserId = user.Id,
                                    Created = TM.Now,
                                    CreatedBy = CurrentUser.Id,
                                    Updated = TM.Now,
                                    UpdatedBy = CurrentUser.Id,

                                    //Qualification = (byte)model.Qualification,
                                    //DepartmentOther = model.DepartmentOther,
                                    //Specialization = model.Specialization,
                                    //CollegeOther = model.CollegeOther,
                                    //CollegeId = model.CollegeId,

                                    //NationalityId = model.NationalityId,
                                    NationalId = model.NationalId,
                                    //CountryId = model.CountryId,
                                    //CityId = model.CityId,
                                    //UniversityId = model.UniversityId,
                                    //DepartmentId = model.DepartmentID,
                                    //Address = model.Address,
                                    //DateOfBirth = birthDate,

                                    //MajorId = model.MajorId,
                                    //UniversityOther = model.UniversityOther
                                };

                                if (number != null)
                                {
                                    researcher.MobileNo = "0" + number.NationalNumber;
                                }

                                DbContext.Researchers.Add(researcher);
                            }
                            break;

                        case ArfUserRole.Referee:
                            {
                                var referee = new Referee
                                {
                                    FirstName = cmps.FirstName,
                                    SecondName = cmps.SecondName,
                                    ThirdName = cmps.ThirdName,
                                    LastName = cmps.LastName,
                                    Email = model.Email,

                                    UserId = user.Id,
                                    Created = TM.Now,
                                    CreatedBy = CurrentUser.Id,
                                    Updated = TM.Now,
                                    UpdatedBy = CurrentUser.Id,
                                };

                                if (number != null)
                                {
                                    referee.MobileNo = "0" + number.NationalNumber;
                                }
                                DbContext.Referees.Add(referee);
                            }
                            break;
                    }

                    DbContext.SaveChanges();

                    {
                        var encounteredIds = new HashSet<long>();
                        var extraRoles = user.UserExtraRoles.ToList();

                        for (var i = 0; i < model.ExtraRoles.Count; i++)
                        {
                            var rmodel = model.ExtraRoles[i];

                            var exRole = DbContext.UserExtraRoles
                                .OrderByDescending(r => r.Id)
                                .Where(r => r.UserId == user.Id)
                                .FirstOrDefault(r => rmodel.Id > 0 && rmodel.Id == r.Id);

                            if (exRole == null)
                            {
                                exRole = new UserExtraRole
                                {
                                    UserId = user.Id,
                                    Created = TM.Now,
                                    CreatedBy = CurrentUser.Id,
                                };

                                DbContext.UserExtraRoles.Add(exRole);
                            }

                            exRole.RoleId = rmodel.RoleId;
                            exRole.CollegeId = rmodel.CollegeId;
                            exRole.EffectiveFrom = rmodel.EffectiveFromDate;
                            exRole.EffectiveTo = rmodel.EffectiveToDate;
                            exRole.IsActive = rmodel.IsActive;
                            exRole.Updated = TM.Now;
                            exRole.UpdatedBy = CurrentUser.Id;

                            DbContext.SaveChanges();

                            encounteredIds.Add(exRole.Id);
                        }


                        DbContext.SaveChanges();
                    }


                    TempData["message"] = Resources.ArfStrings.user_created_successfully;
                    TempData["message_type"] = "success";

                    return RedirectToAction("Index", "Users");
                }
            }

            TempData["message"] = Resources.ArfStrings.user_creation_failed;
            TempData["message_type"] = "danger";

            FillOutLookups(model); ;
            return View(model);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            if (id == CurrentUser.Id)
            {
                return RedirectToAction("Profile", "Account");
            }

            var user = UserUtils.GetUsers()
                .Include("Roles")
                .Include("UserProfile")
                .Include("College")
                .Include("Major")
                .SingleOrDefault(u => u.Id == id);

            if (user == null)
            {
                TempData["message"] = Resources.ArfStrings.could_not_find_any_user + id;
                TempData["message_type"] = "warning";

                return RedirectToAction("Index");
            }

            var model = new EditUserViewModel
            {
                UserId = user.Id,
                NationalId = "",
                UserName = user.UserName,
                FullName = user.UserName,
                Password = "**********",
                ConfirmPassword = "**********",
                Email = user.Email,
                MobileNo = "",
                MajorId = user.MajorId ?? 0,
                MajorName = user.MajorId == null ? "" : user.Major.NameEnglish,
                RoleId = user.Roles.Count() == 0 ? null : user.Roles.FirstOrDefault().Id,
                IsActivated = user.IsActivated,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                ForcePasswordChange = user.ForcePasswordChange,

                CollegeId = user.CollegeId ?? 0,
                CollegeName = user.CollegeId == null ? null : (IsArabic ? user.College.NameArabic : user.College.NameEnglish),
                RoleGroupCode = user.CollegeId == null ? 0 : user.College.RoleGroupCode.Value,
                RoleName = LangHelpers.Instance[user.Roles.Count() == 0 ? null : user.Roles.FirstOrDefault().Name],
                NationalityId = 0,
                BirthDate = "",
                CountryId = user.CountryId ?? 0,
                CityId = 0,
                Address = user.Address ?? "",
                UniversityId = user.UniversityId ?? 0,
                UniversityOther = user.UniversityOther ?? "",
                CollegeOther = user.CollegeOther ?? "",
                DepartmentID = user.DepartmentId ?? 0,
                Bank = 0,
                BankBranch = "",
                BankAccountHolderName = "",
                BankAccountIBAN = "",
                BankOther = "",
                SectionPublishedPapers = "",
                SectionInterests = "",
                SectionCareerHistory = "",
                SectionTrainingCourses = "",
                SectionAwards = "",
                SectionMemberships = ""
            };
            if (user.UserProfile != null)
            {
                model.NationalId = user.UserProfile.NationalId ?? "";
                model.FullName = user.UserProfile.GetFullName();
                model.MobileNo = user.UserProfile.MobileNo ?? "";
                model.NationalityId = user.UserProfile.NationalityId ?? 0;
                model.BirthDate = user.UserProfile.BirthDate == null ? "" : user.UserProfile.BirthDate.Value.FormatDate(false, false, false);
                model.CityId = user.UserProfile.CityId ?? 0;
                model.Bank = user.UserProfile.BankId ?? 0;
                model.BankBranch = user.UserProfile.BankBranch ?? "";
                model.BankAccountHolderName = user.UserProfile.BankAccountHolder ?? "";
                model.BankAccountIBAN = user.UserProfile.BankIban ?? "";
                model.BankOther = user.UserProfile.BankName ?? "";
                model.SectionPublishedPapers = user.UserProfile.CvSectionPublishedPapers == null ? "" : Regex.Replace(user.UserProfile.CvSectionPublishedPapers, "<.*?>", String.Empty);
                model.SectionInterests = user.UserProfile.CvSectionInterests == null ? "" : Regex.Replace(user.UserProfile.CvSectionInterests, "<.*?>", String.Empty);
                model.SectionCareerHistory = user.UserProfile.CvSectionCareerHistory == null ? "" : Regex.Replace(user.UserProfile.CvSectionCareerHistory, "<.*?>", String.Empty);
                model.SectionTrainingCourses = user.UserProfile.CvSectionTrainingCourses == null ? "" : Regex.Replace(user.UserProfile.CvSectionTrainingCourses, "<.*?>", String.Empty);
                model.SectionAwards = user.UserProfile.CvSectionAwards == null ? "" : Regex.Replace(user.UserProfile.CvSectionAwards, "<.*?>", String.Empty);
                model.SectionMemberships = user.UserProfile.CvSectionMemberships == null ? "" : Regex.Replace(user.UserProfile.CvSectionMemberships, "<.*?>", String.Empty);
            }
            var researcher = user.Researchers.FirstOrDefault(x => x.UserId == user.Id);
            if (researcher != null)
            {
                model.DepartmentOther = researcher.DepartmentOther ?? "";
                model.Qualification = (QualificationType?)researcher.Qualification > 0 ? (QualificationType?)researcher.Qualification : QualificationType.Unspecified;
                model.Specialization = researcher.Specialization ?? "";
                model.CountryId = researcher.CountryId ?? 0;
            }


            {
                var extraRoles = Utils.db.UserExtraRoles
                        .Where(x => x.UserId == user.Id)
                        .ToList();

                if (extraRoles.Count > 0)
                {
                    var eRoles = extraRoles
                        .OrderBy(r => r.Id)
                        .Select(r =>
                            new ExtraRoleViewModel
                            {
                                Id = r.Id,
                                RoleId = r.RoleId,
                                Role = (ArfUserRole)Enum.Parse(typeof(ArfUserRole), r.Role.Name),
                                CollegeId = r.CollegeId,
                                EffectiveFrom = r.EffectiveFrom == null ? null : r.EffectiveTo.Value.ToString("yyyy-MM-dd", CultureInfo.GetCultureInfo("en-US")),
                                EffectiveTo = r.EffectiveTo == null ? null : r.EffectiveTo.Value.ToString("yyyy-MM-dd", CultureInfo.GetCultureInfo("en-US")),
                                IsActive = r.IsActive
                            }
                        ).ToList();

                    model.ExtraRoles.AddRange(eRoles);
                }
            }


            FillOutLookups(model);
            return View(model);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel model)
        {
            var userId = null as string;
            var user = (User)null;

            if (!string.IsNullOrWhiteSpace(model.UserId) && model.UserId == CurrentUser.Id)
            {
                return RedirectToAction("Profile", "Account");
            }

            var number = (PhoneNumber)null;

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

            if (!string.IsNullOrWhiteSpace(model.UserId))
            {
                user = UserUtils.GetUsers()
                    .Include("Roles")
                    .Include("UserProfile")
                    .Include("Researchers")
                    .Include("Referees")
                    .SingleOrDefault(u => u.Id == model.UserId);

                if (user == null)
                {
                    TempData["message"] = Resources.ArfStrings.could_not_find_any_user + model.UserId;
                    TempData["message_type"] = "warning";

                    return RedirectToAction("Index");
                }
            }

            var birthDate = DateTime.MinValue;            

            var userRoleName = (Role)null;
            var userRole = ArfUserRole.None;

            if (!string.IsNullOrWhiteSpace(model.RoleId))
            {
                userRoleName = DbContext.Roles.SingleOrDefault(r => r.Id == model.RoleId);
                if (userRoleName == null || !Enum.TryParse(userRoleName.Name, out userRole) || userRole == ArfUserRole.None)
                {
                    ModelState.AddModelError("RoleID", Resources.ArfStrings.user_role_is_required);
                }
                else

                {
                    if (IsCollegeUser)
                    {
                        model.CollegeId = CurrentUser.CollegeId;
                    }
                    else
                    {
                        var needsCollege = Utils.IsCollegeRole(userRole);
                        var needsMajor = Utils.IsMajorRole(userRole);

                        if (needsCollege)
                        {
                            if (model.CollegeId == null)
                            {
                                ModelState.AddModelError("CollegeId",Resources.ArfStrings.colleage_field_is_required);
                            }
                        }
                        else
                        {
                            model.CollegeId = null;
                        }

                        if (needsMajor)
                        {
                            if (model.MajorId == null)
                            {
                                ModelState.AddModelError("MajorId", Resources.ArfStrings.specialization_field_is_required);
                            }
                        }
                        else
                        {
                            model.MajorId = null;
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != "**********" && model.Password == model.ConfirmPassword)
            {
                var errorMsg = (string)null;
                if (!WebUtils.ValidatePassword(model.Password, Utils.IsAdmin(userRole), out errorMsg))
                {
                    ModelState.AddModelError("Password", errorMsg);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.NationalId))
            {
                if (!IsValidNationalId(model.NationalId))
                {
                    ModelState.AddModelError("NationalId", LangHelpers.Instance["invalid_national_id"]);
                }
            }

            model.ExtraRoles = model.ExtraRoles.Where(r => r.RoleId != null).ToList();
            model.Roles = GetRoles();

            {
                var roles = Utils.db.Roles.ToList();
                var i = 0;

                var dupes = model.ExtraRoles.GroupBy(x => new { x.RoleId }).Where(x => x.Skip(1).Any());                
                if (dupes.Count() > 0)
                {
                    ModelState.AddModelError("ExtraRoles_0_", string.Format(LangHelpers.Instance["duplicate_additional_roles"]));
                }
                foreach (var t in model.ExtraRoles)
                {

                    if (model.Roles.Count(r => r.RoleId == t.RoleId) == 0)
                    {
                        ModelState.AddModelError("ExtraRoles_" + i + "_", string.Format(LangHelpers.Instance["role_not_found"]));
                    }
                    else if (t.RoleId == model.RoleId)
                    {
                        ModelState.AddModelError("ExtraRoles_" + i + "_", string.Format(LangHelpers.Instance["extra_role_and_main_role_are_same"]));
                    }
                    else
                    {
                        t.Role = (ArfUserRole)Enum.Parse(typeof(ArfUserRole), roles.FirstOrDefault(r => r.Id == t.RoleId).Name);
                        t.EffectiveFromDate = Utils.StringToDate(t.EffectiveFrom);
                        t.EffectiveToDate = Utils.StringToDate(t.EffectiveTo);

                        if (Utils.IsCollegeRole(t.Role))
                        {
                            if (!(t.CollegeId > 0))
                            {
                                ModelState.AddModelError("ExtraRoles_" + i + "_", string.Format(LangHelpers.Instance["college_required"]));
                            }
                        }
                        else
                        {
                            t.CollegeId = null;
                        }

                        if (t.EffectiveToDate < t.EffectiveFromDate)
                        {
                            ModelState.AddModelError("ExtraRoles_" + i + "_", string.Format(LangHelpers.Instance["effective_to_before_effective_from"]));
                        }

                    }



                    i++;
                }
            }

            if (ModelState.IsValid)
            {
                var sendActivationEmail = user.IsActivated != true && model.IsActivated == true;

                try
                {
                    user.Roles.Clear();
                    user.Roles.Add(userRoleName);

                    if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != "**********" && model.Password == model.ConfirmPassword)
                    {
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                    }

                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    user.IsActivated = model.IsActivated;
                    user.EmailConfirmed = model.EmailConfirmed;
                    user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
                    user.ForcePasswordChange = model.ForcePasswordChange;
                    user.CollegeId = model.CollegeId;
                    user.MajorId = model.MajorId;
                    user.Updated = TM.Now;
                    var comps = Utils.GetNameComponents(model.FullName);

                    if (user.UserProfile == null)
                    {
                        user.UserProfile = new UserProfile
                        {
                            UserId = user.Id,
                            Created = TM.Now,
                            CreatedBy = CurrentUser.Id,
                            Updated = TM.Now,
                            UpdatedBy = CurrentUser.Id,

                            FirstName = comps.FirstName,
                            MiddleName = comps.SecondName,
                            ThirdName = comps.ThirdName,
                            LastName = comps.LastName,
                            AddressLine1 = model.Address,
                            BirthDate = birthDate,
                            NationalityId = model.NationalityId,
                            BankName = model.BankName,
                            BankBranch = model.BankBranch,
                            BankIban = model.BankAccountIBAN,
                            BankAccountHolder = model.BankAccountHolderName,
                            NationalId = model.NationalId,
                            MobileNo = model.MobileNo,
                            MobileNumber = model.MobileNo,

                            CvSectionPublishedPapers = model.SectionPublishedPapers,
                            CvSectionInterests = model.SectionInterests,
                            CvSectionCareerHistory = model.SectionCareerHistory,
                            CvSectionTrainingCourses = model.SectionTrainingCourses,
                            CvSectionAwards = model.SectionAwards,
                            CvSectionMemberships = model.SectionMemberships,
                            CvSectionAcademicHistory = model.SectionAcademicHistory,
                            BankId = model.Bank
                        };

                        DbContext.SaveChanges();
                    }


                    {
                        var encounteredIds = new HashSet<long>();
                        var extraRoles = user.UserExtraRoles.ToList();

                        for (var i = 0; i < model.ExtraRoles.Count; i++)
                        {
                            var rmodel = model.ExtraRoles[i];

                            var exRole = DbContext.UserExtraRoles
                                .OrderByDescending(r => r.Id)
                                .Where(r => r.UserId == user.Id)
                                .FirstOrDefault(r => rmodel.Id > 0 && rmodel.Id == r.Id);

                            if (exRole == null)
                            {
                                exRole = new UserExtraRole
                                {
                                    UserId = user.Id,
                                    Created = TM.Now,
                                    CreatedBy = CurrentUser.Id,
                                };

                                DbContext.UserExtraRoles.Add(exRole);
                            }

                            exRole.RoleId = rmodel.RoleId;
                            exRole.CollegeId = rmodel.CollegeId;
                            exRole.EffectiveFrom = rmodel.EffectiveFromDate;
                            exRole.EffectiveTo = rmodel.EffectiveToDate;
                            exRole.IsActive = rmodel.IsActive;
                            exRole.Updated = TM.Now;
                            exRole.UpdatedBy = CurrentUser.Id;

                            DbContext.SaveChanges();

                            encounteredIds.Add(exRole.Id);
                        }

                        foreach (var cr in extraRoles)
                        {
                            if (!encounteredIds.Contains(cr.Id))
                            {
                                DbContext.Entry(cr).State = EntityState.Deleted;
                            }
                        }

                        DbContext.SaveChanges();
                    }


                    var cmps = Utils.GetNameComponents(model.FullName);

                    switch (userRole)
                    {
                        case ArfUserRole.PrincipalResearcher:
                            if (user.Researchers.Count == 0)
                            {
                                var researcher = new Researcher
                                {
                                    FirstName = cmps.FirstName,
                                    SecondName = cmps.SecondName,
                                    ThirdName = cmps.ThirdName,
                                    LastName = cmps.LastName,
                                    Email = model.Email,
                                    CollegeId = model.CollegeId,
                                    MajorId = model.MajorId,
                                    UserId = user.Id,
                                    Created = TM.Now,
                                    CreatedBy = CurrentUser.Id,
                                    Updated = TM.Now,
                                    UpdatedBy = CurrentUser.Id,

                                    Qualification = (byte)model.Qualification,
                                    DepartmentOther = model.DepartmentOther,
                                    Specialization = model.Specialization,
                                    CollegeOther = model.CollegeOther,

                                    NationalityId = model.NationalityId,
                                    NationalId = model.NationalId,
                                    CountryId = model.CountryId,
                                    CityId = model.CityId,
                                    UniversityId = model.UniversityId,
                                    DepartmentId = model.DepartmentID,
                                    Address = model.Address,
                                    MobileNo = model.MobileNo,
                                    DateOfBirth = birthDate
                                };

                                if (number != null)
                                {
                                    researcher.MobileNo = "0" + number.NationalNumber;
                                }

                                DbContext.Researchers.Add(researcher);
                                DbContext.SaveChanges();

                                user.Researchers = new List<Researcher> { researcher };
                            }

                            DbContext.Entry(user.Researchers.First()).State = EntityState.Modified;
                            break;

                        case ArfUserRole.Referee:
                            if (user.Referees.Count == 0)
                            {
                                var referee = new Referee
                                {
                                    FirstName = cmps.FirstName,
                                    SecondName = cmps.SecondName,
                                    ThirdName = cmps.ThirdName,
                                    LastName = cmps.LastName,
                                    Email = model.Email,
                                    CollegeId = model.CollegeId,
                                    MajorId = model.MajorId,
                                    UserId = user.Id,
                                    Created = TM.Now,
                                    CreatedBy = CurrentUser.Id,
                                    Updated = TM.Now,
                                    UpdatedBy = CurrentUser.Id,
                                };

                                if (number != null)
                                {
                                    referee.MobileNo = "0" + number.NationalNumber;
                                }

                                DbContext.Referees.Add(referee);
                                DbContext.SaveChanges();

                                user.Referees = new List<Referee> { referee };
                            }

                            DbContext.Entry(user.Referees.First()).State = EntityState.Modified;
                            break;
                    }

                    if (number != null)
                    {
                        user.UserProfile.MobileNo = "0" + number.NationalNumber;
                    }

                    user.UserProfile.NationalId = model.NationalId;

                    user.UserProfile.FirstName = cmps.FirstName;
                    user.UserProfile.MiddleName = cmps.SecondName;
                    user.UserProfile.ThirdName = cmps.ThirdName;
                    user.UserProfile.LastName = cmps.LastName;

                    switch (userRole)
                    {
                        case ArfUserRole.PrincipalResearcher:
                            {
                                if (model.CollegeId != null)
                                    user.Researchers.First().CollegeId = model.CollegeId;

                            }
                            break;

                        case ArfUserRole.Referee:
                            {
                                if (model.CollegeId != null)
                                    user.Referees.First().CollegeId = model.CollegeId;

                                user.Referees.First().MajorId = model.MajorId;
                            }
                            break;
                    }

                    DbContext.Entry(user).State = EntityState.Modified;
                    DbContext.Entry(user.UserProfile).State = EntityState.Modified;
                    DbContext.SaveChanges();

                    if (sendActivationEmail && userRole == ArfUserRole.Referee)
                    {
                        var err = "";
                        var gKey = "";
                        CreateTemporaryLink(TempUrlType.RefereeAccountActivation, user.Id, model.Email, model.MobileNo, out err, out gKey);
                    }

                    TempData["message"] = Resources.ArfStrings.user_updated_successfully;
                    TempData["message_type"] = "success";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Logger.LogExceptionAsync(ex, model);

                    ModelState.AddModelError("EXCEPTION", ex.SanitizeException());
                }
            }

            //model.Colleges = GetColleges();
            //model.Majors = GetMajors();
            //model.Roles = GetRoles();
            FillOutLookups(model);
            return View(model);
        }

        // GET: Admin/Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id == CurrentUser.Id)
            {
                Response.Write("You can't delete yourself.");
                Response.End();
                return null;
            }

            User aspNetUser = DbContext.Users.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (id == CurrentUser.Id)
            {
                Response.Write(Resources.ArfStrings.you_can_delete_your_self);
                Response.End();
                return RedirectToAction("Index");
            }
            var username = (string)null;
            try
            {
                User aspNetUser = DbContext.Users.Find(id);
                username = aspNetUser.UserName;

                DbContext.Users.Remove(aspNetUser);
                DbContext.SaveChanges();
                TempData["message"] = Resources.ArfStrings.user_deleted_successfully;
                TempData["message_type"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsync(ex, id);
                TempData["message"] = Resources.ArfStrings.user_deleted_failed+ " " + (string.IsNullOrWhiteSpace(username) ? id : username) + " (" + ex.SanitizeException() + ")";
                TempData["message_type"] = "danger";
            }
            return RedirectToAction("Index");
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangeUserPasswordViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserId))
            {
                return RedirectToAction("Index", "Users");
            }

            var user = GetInstance<UsersManager>().GetUsers().SingleOrDefault(u => u.Id == model.UserId);

            if (user == null)
            {
                TempData["message"] = Resources.ArfStrings.could_not_find_any_user;
                TempData["message_type"] = "danger";

                return RedirectToAction("Index", "Users");
            }

            var roles = user.Roles.Select(r => r.Name).ToList();
            var userRole = ArfUserRole.None;

            foreach (var r in roles)
            {
                var role = ArfUserRole.None;

                if (Enum.TryParse<ArfUserRole>(r, out role) && role > userRole)
                {
                    userRole = role;
                }
            }

            var errorMsg = (string)null;
            if (!WebUtils.ValidatePassword(model.Password, Utils.IsAdmin(userRole), out errorMsg))
            {
                ModelState.AddModelError("Password", errorMsg);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                    user.LockoutEnabled = false;
                    user.LockoutEndDateUtc = null;
                    user.AccessFailedCount = 0;

                    DbContext.SaveChanges();

                    if (model.SendCredentialsViaEmail)
                    {
                        var template = GetInstance<TemplatesManager>()[TemplateKey.EditUserPassword];

                        var body = template.BodyEnglish
                            .Replace("{Name}", user.FullName)
                            .Replace("{Username}", user.UserName)
                            .Replace("{Password}", model.Password);

                        EmailService.SendMessage(SmtpSettings, template.SubjectEnglish, body, new MailAddress(user.Email, user.FullName), null, null);
                    }

                    TempData["message"] = "تم تحديث كلمة المرور بنجاح ";
                    TempData["message_type"] = "success";
                }
                catch (Exception ex)
                {
                    Logger.LogExceptionAsync(ex, model);
                }

                return RedirectToAction("Edit", "Users", new { Id = model.UserId });
            }

            var model2 = (EditUserViewModel)((ViewResult)Edit(model.UserId)).Model;

            return View("Edit", model2);
        }

        [AjaxOnly]
        public dynamic AjaxDeleteUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new { success = false, msg = "INVALID_APPLICAITON_ID" };

            if (id == CurrentUser.Id)
            {
                return new { success = false, msg = Resources.ArfStrings.you_can_delete_your_self};
            }
            try
            {
                var user = UserUtils.GetUsers().SingleOrDefault(u => u.Id == id);

                if (user == null)
                {
                    TempData["message"] = Resources.ArfStrings.could_not_find_any_user + id;
                    TempData["message_type"] = "warning";

                    return RedirectToAction("Index");
                }

                user.IsDeleted = true;
                DbContext.Entry(user).State = EntityState.Modified;
                DbContext.SaveChanges();
                return new { success = true, msg = Resources.ArfStrings.user_deleted_successfully };
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsync(ex, id);
                return new { success = false, msg = ex.SanitizeException() };
            }
        }

   
        [NonAction]
        private void FillOutLookups(EditUserViewModel model)
        {
            {
                model.Majors = GetMajors();
            }
            {
                model.Roles = GetRoles(true);
            }
            {
                var list = DbContext.Departments
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
            {
                model.Colleges = GetColleges();
            }
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

                //list.Add(new SelectListItem { Text = LangHelpers.Instance.GetLangValueByString("other", false, null), Value = "0" });

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
    }
}