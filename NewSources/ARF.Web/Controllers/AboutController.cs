using ARF.Core;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using ARF.Web.App_Start.Code;
using ARF.Web.Models;
using ARF.Web.Resources;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [AllowAnonymous]
    public class AboutController : BaseController
    {
        // GET: About
        [AcceptVerbs("GET", "POST")]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ContactUs()
        {
            var model = new ContactUsViewModel();
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(ContactUsViewModel model)
        {
            var contactUs = new ContactedU();
            var db = Utils.db;

            if (model == null) return RedirectToAction("ContactUs");


            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

            if (String.IsNullOrEmpty(recaptchaHelper.Response))
            {
                ModelState.AddModelError("", LangHelpers.Instance["robot_required"]);
            }
            else
            {
                RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    ModelState.AddModelError("", "Incorrect captcha answer.");
                }
                else
                {
                    ViewBag.VerifyCaptcha = false;
                }
            }


            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                ModelState.AddModelError("FullName", ArfStrings.fullname_required);
            }

            if (string.IsNullOrWhiteSpace(model.EmailAddress))
            {
                ModelState.AddModelError("EmailAddress", ArfStrings.email_required);
            }
            else if (!Utils.ValidateEmail(model.EmailAddress))
            {
                ModelState.AddModelError("EmailAddress", ArfStrings.invalid_email);
            }
            else
            {
                model.EmailAddress = model.EmailAddress.Trim().ToLower();
            }

            if (string.IsNullOrWhiteSpace(model.Message))
            {
                ModelState.AddModelError("Message", ArfStrings.contactusmessage_required);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    contactUs.FullName = model.FullName;
                    contactUs.EmailAddress = model.EmailAddress;
                    contactUs.Subject = model.Subject;
                    contactUs.Message = model.Message;

                    contactUs.Created = TM.Now;
                    db.ContactedUs.Add(contactUs);
                    db.SaveChanges();

                    var allSystemAdmins = db.Users.Where(u => u.Roles.Count(r => r.Name == ArfUserRole.SystemAdmin.ToString()) > 0).ToList();
                    var isSent = SendContactUsMessage(model);

                    {
                        TempData["message"] = Resources.ArfStrings.you_have_successfully_submitted_and_comunicate_assoon;
                        TempData["message_type"] = "success";

                        return RedirectToAction("Index", "Home");
                    }

                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    Logger.LogExceptionAsync(ex, model);
                    TempData["message"] = ArfStrings.failed + " " + ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
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
                    TempData["message"] = ArfStrings.failed + " " + ex2.Message;
                    TempData["message_type"] = "danger";
                }
            }

            return View(model);
        }

        [AcceptVerbs("GET")]
        public ActionResult UserGuide()
        {
            if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch))
            {
                return File(Utils.MapPath("~/Content/files/ARF_UM.pdf"), MimeMapping.GetMimeMapping("ssdds.pdf"));
            }
            else if (Utils.HasRolesAny(ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects) || Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects))
            {
                return File(Utils.MapPath("~/Content/files/ARF_UM_S_M.pdf"), MimeMapping.GetMimeMapping("ssdds.pdf"));
            }
            else
            {
                return File(Utils.MapPath("~/Content/files/ARF User Manual-الباحث الرئيس.pdf"), MimeMapping.GetMimeMapping("ssdds.pdf"));
            }


        }
    }
}