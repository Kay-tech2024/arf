using ARF.Core;
using ARF.Data;
using ARF.TimeMachine;
using ARF.Web.App_Start.Code;
using ARF.Web.Models;
using ARF.Web.Resources;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [AllowAnonymous]
    public class TempLinksController : BaseController
    {
        // GET: TempLinks
        [HttpGet]
        public ActionResult Index(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Response.Write(LangHelpers.Instance["Invalidkey"]);
                Response.End();

                return Content("");
            }


            var linkData = DbContext.UserTemporaryUrls.FirstOrDefault(l => l.Key == key);

            if (linkData != null)
            {
                if (linkData.IsUsed)
                {
                    return View("~/Views/TempLinks/CustomView.cshtml", (object)Resources.ArfStrings.this_link_is_used);
                }

                if (linkData.Expires < TM.Now)
                {
                    return View("~/Views/TempLinks/CustomView.cshtml", (object)Resources.ArfStrings.this_link_is_no_longer_used);
                }

                var user = DbContext.Users.FirstOrDefault(u => u.Id == linkData.UserId);

                if (user == null)
                {
                    return View("~/Views/TempLinks/CustomView.cshtml", (object)Resources.ArfStrings.this_link_is_invalid);
                }
               

                switch ((TempUrlType)linkData.Type)
                {

                    case TempUrlType.RefereeEmailActivation:
                        if (user.EmailConfirmed == true)
                        {
                            return View("~/Views/TempLinks/CustomView.cshtml", (object)Resources.ArfStrings.this_account_is_already_activated);
                        }

                        linkData.IsUsed = true;
                        user.EmailConfirmed = true;
                        user.Updated = TM.Now;
                        DbContext.SaveChanges();

                        if (user.IsActivated == true)                                                   
                            return View("~/Views/TempLinks/CustomView.cshtml", (object)(Resources.ArfStrings.email_verfication_completed + " <p> <a href='https://arf.srg.edu.sa/'>" + Resources.ArfStrings.click_here_for_homepage + "</p>"));                        
                        else
                            return View("~/Views/TempLinks/CustomView.cshtml", (object)Resources.ArfStrings.email_verfication_completed_but_not_activated);

                    case TempUrlType.ResearcherEmailActivation:
                        if (user.EmailConfirmed == true)
                        {
                            return View("~/Views/TempLinks/CustomView.cshtml", (object)Resources.ArfStrings.this_account_is_already_activated);
                        }

                        linkData.IsUsed = true;
                        user.EmailConfirmed = true;
                        user.Updated = TM.Now;
                        DbContext.SaveChanges();

                        if (user.IsActivated == true)
                            return View("~/Views/TempLinks/CustomView.cshtml", (object)(Resources.ArfStrings.email_verfication_completed+" <p> <a href='https://arf.srg.edu.sa/'>"+ Resources.ArfStrings.click_here_for_homepage+"</p>"));
                        else
                            return View("~/Views/TempLinks/CustomView.cshtml", (object)Resources.ArfStrings.email_verfication_completed_but_not_activated);

                    case TempUrlType.ResetPassword:

                        var model = new ResetPasswordViewModel
                        {
                            Key = key,
                            Email = user.Email
                        };
                        return View("~/Views/TempLinks/ResetPassword.cshtml", model);

                }

                return View("~/Views/TempLinks/CustomView.cshtml", (object)Resources.ArfStrings.this_link_is_invalid_for_use);

            }
            else
            {

                var coresearcher = Utils.db.RequestCoresearchers
                    .Include("Researcher")                    
                    .OrderByDescending(t => t.Updated)
                    .FirstOrDefault(t => t.Key == key && t.IsDeleted == false);

                if (coresearcher == null)
                {
                    Response.Write(LangHelpers.Instance["InvalidLink"]);
                    Response.End();

                    return Content("");
                }

                if (coresearcher.IsKeyUsed)
                {
                    Response.Write(LangHelpers.Instance["LinkisAlreadyused"]);
                    Response.End();

                    return Content("");
                }
                return RedirectToAction("CoResearch", new CoResearcherMessageViewModel
                {
                    Name = coresearcher.Researcher.FirstName + " " + coresearcher.Researcher.SecondName + " " + coresearcher.Researcher.ThirdName + " " + coresearcher.Researcher.LastName,
                    Email = coresearcher.Researcher.Email,
                    mobileNo = coresearcher.Researcher.MobileNo,
                    Key = key,
                    IsAccepted = true,
                });
            }

        }

        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Key)) return RedirectToAction("Login", "Account");

            var linkData = DbContext.UserTemporaryUrls.FirstOrDefault(l => l.Key == model.Key);

            ModelState.Clear();

            if (linkData == null)
            {
                TempData["message"] = Resources.ArfStrings.this_link_is_invalid;
                TempData["message_type"] = "danger";
                return RedirectToAction("Login", "Account");
            }

            if (linkData.IsUsed)
            {
                TempData["message"] = Resources.ArfStrings.this_link_is_used;
                TempData["message_type"] = "danger";
                return RedirectToAction("Login", "Account");

            }

            if (linkData.Expires < TM.Now)
            {
                TempData["message"] = Resources.ArfStrings.this_link_is_invalid_for_use;
                TempData["message_type"] = "danger";
                return RedirectToAction("Login", "Account");
            }

            var user = DbContext.Users.FirstOrDefault(u => u.Id == linkData.UserId);

            if (user == null)
            {
                TempData["message"] = Resources.ArfStrings.this_link_is_invalid;
                TempData["message_type"] = "danger";
                return RedirectToAction("Login", "Account");
            }


            model.Email = user.Email;
            var errorMsg = (string)null;
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", LangHelpers.Instance["password_required"]);

            }
            else if (model.ConfirmPassword != model.Password)
            {
                ModelState.AddModelError("ConfirmPassword", LangHelpers.Instance["password_confirmation_mismatch"]);
            }
            else if (!WebUtils.ValidatePassword(model.Password, IsAdmin, out errorMsg))
            {
                ModelState.AddModelError("Password", errorMsg);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                    linkData.IsUsed = true;
                    DbContext.SaveChanges();

                    TempData["message"] = Resources.ArfStrings.password_changed_successfully;
                    TempData["message_type"] = "success";

                    return RedirectToAction("Login", "Account");
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


            return View("ResetPassword", model);

        }


    }
}