using ARF.Core;
using ARF.Core.Managers;
using ARF.Data.DbModels;
using ARF.Web.App_Start.Code;
using ARF.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using System.Linq;
using ARF.Data;
using ARF.Services;
using ARF.TimeMachine;
using OpenHtmlToPdf;
using System.IO;
using System.Text;
using MimeTypes;

namespace ARF.Web.Controllers
{    
    public class SettingsController : BaseController
    {
        [CustomAuthorize(Privilages.settings_email, Privilages.settings_sms, Privilages.settings_tawafuq, Privilages.settings_webservices, Privilages.settings_change_rag)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [CustomAuthorize(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers)]
        public ActionResult Contract()
        {
            var model = new ContractTemplateViewModel();            
            var contract_arabic = Utils.db.Settings.FirstOrDefault(x => x.Key == Constants.FUNDING_REQUEST_CONTRACT_TEMPLATE_ARABIC);
            var contract_start_date = Utils.db.Settings.FirstOrDefault(x => x.Key == Constants.FUNDING_REQUEST_CONTRACT_START_DATE);

            model.ContractArabic = contract_arabic != null ? (contract_arabic.Value.StartsWith("<div dir='rtl'>") ? contract_arabic.Value : "<div dir='rtl'>" + contract_arabic.Value + "</div>") : "";
            model.ContractStartDate = contract_start_date != null ? contract_start_date.Value : "";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contract(ContractTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Utils.SaveSettingKeyData(Constants.FUNDING_REQUEST_CONTRACT_TEMPLATE_ARABIC, model.ContractArabic);
                Utils.SaveSettingKeyData(Constants.FUNDING_REQUEST_CONTRACT_START_DATE, model.ContractStartDate);
                if (model.DownloadPDF == 1)
                {
                    string FileName = DateTime.Now.Ticks.ToString() + "--sample-contract.pdf";
                    string FilePath = Path.Combine(Server.MapPath("~/Content/files/"), FileName);

                    string baseURL = System.Configuration.ConfigurationManager.AppSettings["ApplicationUrl"];
                    string contractHeaderURL = baseURL + "/ContractHeader.html";

                    string pdfHtml = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>" + FileName + "</title></head><body>" + model.ContractArabic + "</body></html>";


                    var pdf = Pdf.From(pdfHtml)
                                    .OfSize(PaperSize.A4)
                                    .WithoutOutline()
                                    .WithTitle(FileName)                                    
                                    .Portrait()
                                    .Comressed()
                                    .WithObjectSetting("header.htmlUrl", contractHeaderURL)
                                    .WithObjectSetting("header.spacing", "2.5")
                                    .WithObjectSetting("header.line", "1")
                                    .Content();
                    FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
                    fs.Write(pdf, 0, pdf.Length);
                    fs.Close();
                    return File(FilePath, MimeTypeMap.GetMimeType(Path.GetExtension(FilePath)), FileName);
                }
                else
                {
                    TempData["message"] = LangHelpers.Instance["data_saved_successfully"];
                }

                TempData["message_type"] = "success";
                return RedirectToAction("Contract");
            }

            return Contract();
        }

        [AllowAnonymous]
        public ActionResult Translate(string key, string value, string lang)
        {
            if (!Utils.IsLocalhost && !Utils.HasActualRolesAny(ArfUserRole.SystemAdmin))
            {
                return Content("Not authorized");
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    ModelState.AddModelError("value", Resources.ArfStrings.you_must_enter_text);
                }

                if (lang != "ar" && lang != "en")
                {
                    ModelState.AddModelError("lang", Resources.ArfStrings.you_must_choose_language);
                }

                if (key != null && key.StartsWith("untranslated_"))
                    key = key.Substring("untranslated_".Length);

                if (ModelState.IsValid)
                {


                    var originalKey = key.Trim();
                    key = key.Replace(" ", "");

                    key = Utils.PascalCaseToUnderscore(key);

                    var kv = DbContext.Translations.FirstOrDefault(k => k.Key == key && k.Lang == lang);

                    if (kv == null)
                    {


                        var kvs = DbContext.Translations.Where(k => k.Value.StartsWith(key) && k.Value.EndsWith(key) && ((lang == "en" && k.Lang == "ar") || (lang == "ar" && k.Lang == "en"))).ToList();

                        if (kvs.Count == 1)
                        {
                            kv = kvs[0];
                        }



                    }

                    var userId = CurrentUser != null ? CurrentUser.Id : Utils.SystemAdminUserId;
                    if (kv != null)
                    {
                        kv.Value = value.Trim();
                        kv.Updated = TM.Now;
                        kv.UpdatedBy = userId;
                    }
                    else
                    {
                        kv = new Translation
                        {
                            Key = key,
                            Value = value.Trim(),
                            Lang = lang,
                            CreatedBy = userId,
                            UpdatedBy = userId,
                            Created = TM.Now,
                            Updated = TM.Now
                        };

                        DbContext.Translations.Add(kv);
                    }

                    DbContext.SaveChanges();
                }
            }
            return View();
        }


        [AllowAnonymous]
        public ActionResult GetTranslation(string key, string value, string lang)
        {
            if (!Utils.IsLocalhost && !Utils.HasActualRolesAny(ArfUserRole.SystemAdmin))
            {
                return new HttpUnauthorizedResult();
            }

            if (key != null && key.StartsWith("untranslated_"))
                key = key.Substring("untranslated_".Length);

            if (!string.IsNullOrWhiteSpace(key))
            {
                var originalKey = key.Trim();
                key = key.Replace(" ", "");

                key = Utils.PascalCaseToUnderscore(key);

                var kv = DbContext.Translations.FirstOrDefault(k => k.Key == key && k.Lang == lang);

                if (kv == null)
                {

                    var kvs = DbContext.Translations.Where(k => k.Key.StartsWith(key) && k.Lang == lang).ToList();

                    if (kvs.Count == 1)
                    {
                        kv = kvs[0];
                    }
                    else
                    {
                        kvs = DbContext.Translations.Where(k => k.Value.StartsWith(originalKey) && k.Lang == lang).ToList();

                        if (kvs.Count >= 1)
                        {
                            kv = kvs[0];
                        }
                    }


                }

                if (kv != null)
                {
                    return Json(kv.Value);
                }

            }

            return Json("");
        }



        [CustomAuthorize(Privilages.settings_email)]
        public ActionResult Email()
        {
            return View(SmtpSettings);
        }

        [CustomAuthorize(Privilages.settings_email)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Email(SmtpSettings model)
        {
            if (ModelState.IsValid)
            {
                var setts = DbContext.Settings.Where(s => s.Key.StartsWith("smtp")).ToList();

                var setting = (Setting)null;

                if ((setting = setts.SingleOrDefault(s => s.Key.ToLower() == "smtp:host")) != null)
                {
                    setting.Value = model.Host;
                }
                else
                {
                    DbContext.Settings.Add(new Setting
                    {
                        Key = "smtp:Host",
                        Value = model.Host
                    });
                }

                if ((setting = setts.SingleOrDefault(s => s.Key.ToLower() == "smtp:port")) != null)
                {
                    setting.Value = model.Port.ToString();
                }
                else
                {
                    DbContext.Settings.Add(new Setting
                    {
                        Key = "smtp:Port",
                        Value = model.Port.ToString()
                    });
                }

                if ((setting = setts.SingleOrDefault(s => s.Key.ToLower() == "smtp:fromemail")) != null)
                {
                    setting.Value = model.FromEmail;
                }
                else
                {
                    DbContext.Settings.Add(new Setting
                    {
                        Key = "smtp:FromEmail",
                        Value = model.FromEmail
                    });
                }

                if ((setting = setts.SingleOrDefault(s => s.Key.ToLower() == "smtp:username")) != null)
                {
                    setting.Value = model.Username;
                }
                else
                {
                    DbContext.Settings.Add(new Setting
                    {
                        Key = "smtp:Username",
                        Value = model.Username
                    });
                }

                if ((setting = setts.SingleOrDefault(s => s.Key.ToLower() == "smtp:emailcc")) != null)
                {
                    setting.Value = model.EmailCC;
                }
                else
                {
                    DbContext.Settings.Add(new Setting
                    {
                        Key = "smtp:EmailCC",
                        Value = model.EmailCC
                    });
                }

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    if ((setting = setts.SingleOrDefault(s => s.Key.ToLower() == "smtp:password")) != null)
                    {
                        setting.Value = model.Password;
                    }
                    else
                    {
                        DbContext.Settings.Add(new Setting
                        {
                            Key = "smtp:Password",
                            Value = model.Password
                        });
                    }
                }

                if ((setting = setts.SingleOrDefault(s => s.Key.ToLower() == "smtp:ssl")) != null)
                {
                    setting.Value = model.SSL.ToString();
                }
                else
                {
                    DbContext.Settings.Add(new Setting
                    {
                        Key = "smtp:SSL",
                        Value = model.SSL.ToString()
                    });
                }

                DbContext.SaveChanges();
                TempData["message"] = "Update is successful";
                TempData["message_type"] = "success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Update failed";
                TempData["message_type"] = "danger";
            }
            return View(model);
        }

        [CustomAuthorize(Privilages.settings_sms)]
        public ActionResult Sms()
        {
            var MalathSmsSettings = Utils.MalathSmsSettings;
            var MobilySmsSettings = Utils.MobilySmsSettings;

            var model = new SmsSettingsViewModel() { Providers = new List<SmsProviderViewModel>(), PreferredProvider = MalathSmsSettings.Type };
            model.Providers.Add(new SmsProviderViewModel
            {
                Type = SmsProvider.MalathSMS,
                SenderName = MalathSmsSettings.Sender,
                Username = MalathSmsSettings.Username,
                Password = MalathSmsSettings.Password
            });

            model.Providers.Add(new SmsProviderViewModel
            {
                Type = SmsProvider.MobilyWS,
                SenderName = MobilySmsSettings.Sender,
                Username = MobilySmsSettings.Username,
                Password = MobilySmsSettings.Password
            });

            return View(model);
        }

        [CustomAuthorize(Privilages.settings_sms)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sms(SmsSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var setts = DbContext.Settings.Where(s => s.Key.StartsWith("sms")).ToList();

                var setting = (Setting)null;

                if ((setting = setts.SingleOrDefault(s => s.Key.ToLower() == "sms:type")) != null)
                {
                    setting.Value = model.PreferredProvider.ToString();
                }
                else
                {
                    DbContext.Settings.Add(new Setting
                    {
                        Key = "sms:type",
                        Value = model.PreferredProvider.ToString()
                    });
                }

                foreach (var p in model.Providers)
                {
                    var suffix = ":" + (p.Type == SmsProvider.MalathSMS ? "malath" : "mobily");

                    if ((setting = setts.SingleOrDefault(s => s.Key.ToLower() == "sms:sender" + suffix)) != null)
                    {
                        setting.Value = p.SenderName;
                    }
                    else
                    {
                        DbContext.Settings.Add(new Setting
                        {
                            Key = "sms:sender" + suffix,
                            Value = p.SenderName
                        });
                    }

                    if ((setting = setts.SingleOrDefault(s => s.Key.ToLower() == "sms:username" + suffix)) != null)
                    {
                        setting.Value = p.Username;
                    }
                    else
                    {
                        DbContext.Settings.Add(new Setting
                        {
                            Key = "sms:Username" + suffix,
                            Value = p.Username
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(p.Password) && p.Password != "**********")
                    {
                        if ((setting = setts.SingleOrDefault(s => s.Key.ToLower() == "sms:password" + suffix)) != null)
                        {
                            setting.Value = p.Password;
                        }
                        else
                        {
                            DbContext.Settings.Add(new Setting
                            {
                                Key = "sms:Password" + suffix,
                                Value = p.Password
                            });
                        }
                    }
                }

                DbContext.SaveChanges();
                TempData["message"] = "SMS settings updated successfully";
                TempData["message_type"] = "success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "SMS settings failed to update";
                TempData["message_type"] = "danger";
            }
            return View(model);
        }

        private bool SendNotification(string email)
        {
            try
            {
                var to = new MailAddress(email);

                return EmailService.SendMessage(SmtpSettings, "Test Email", "This is test message", to, null, null);
            }
            catch (DbEntityValidationException ex)
            {
                Logger.LogExceptionAsync(ex, email);
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsync(ex, email);
                return false;
            }
        }

        [ValidateAntiForgeryToken]
        [CustomAuthorize(Privilages.settings_email)]
        [HttpPost]
        public ActionResult SendTestMessage(string Email)
        {
            if (SendNotification(Email))
            {
                TempData["message"] = "Send test message";
                TempData["message_type"] = "success";
                return RedirectToAction("Email");
            }
            else
            {
                if (TempData["message"] == null)
                {
                    TempData["message"] = "Failed to send test message";
                    TempData["message_type"] = "danger";
                }
                return RedirectToAction("Email");
            }
        }

        [CustomAuthorize(Privilages.settings_sms)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendTestSmsMessage(string mobileNumber, SmsProvider? provider)
        {
            try
            {
                var MalathSmsSettings = Utils.MalathSmsSettings;
                var MobilySmsSettings = Utils.MobilySmsSettings;

                string error = null;
                var status = false;

                if (provider == SmsProvider.MalathSMS)
                {
                    status = SmsServiceMalath.SendMessage(MalathSmsSettings, "This is test message", new string[] { mobileNumber }, out error);
                }
                else
                {
                    status = SmsServiceMobily.SendMessage(MobilySmsSettings, "This is test message", new string[] { mobileNumber }, out error);
                }

                if (!status)
                {
                    TempData["message"] = error;
                    TempData["message_type"] = "danger";
                }
                else
                {
                    TempData["message"] = "Test message was sent successfully";
                    TempData["message_type"] = "success";
                }
            }
            catch (DbEntityValidationException ex)
            {
                Logger.LogExceptionAsync(ex, mobileNumber);

                TempData["message"] = "Failed to send test message (" + ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage + ")";
                TempData["message_type"] = "danger";
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsync(ex, mobileNumber);
                TempData["message"] = "Failed to send test message (exception: " + ex.Message + ")";
                TempData["message_type"] = "danger";
            }

            return RedirectToAction("Sms");
        }

    }

}