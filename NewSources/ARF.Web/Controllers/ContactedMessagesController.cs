using ARF.Core;
using ARF.Data;
using ARF.Web.App_Start.Code;
using ARF.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [CustomAuthorize(ArfUserRole.SystemAdmin)]
    public class ContactedMessagesController : BaseController
    {
        // GET: ContactedMessages
        public ActionResult Index()
        {
            var list = Utils.db.ContactedUs
                .Where(x => x.IsDeleted == false)
                .Select(t => new ContactMessagesViewModel
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    EmailAddress = t.EmailAddress,
                    Created = t.Created,
                    Message = t.Message,
                    Subject = t.Subject,
                    IsDeleted = t.IsDeleted
                });

            return View(list);
        }
        [HttpGet]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var message = DbContext.ContactedUs.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            var contacted = new ContactMessagesViewModel
            {
                Id = message.Id,
                Created = message.Created,
                EmailAddress = message.EmailAddress,
                FullName = message.FullName,
                Message = message.Message,
                Subject = message.Subject
            };
            return View(contacted);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ContactMessagesViewModel contacted)
        {
            if (contacted == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                var message = DbContext.ContactedUs.Find(contacted.Id);
                message.IsDeleted = true;
                DbContext.Entry(message).State = EntityState.Modified;
                DbContext.SaveChanges();
                TempData["message"] = Resources.ArfStrings.message_deleted_successfully;
                TempData["message_type"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsync(ex, contacted.Id);
                TempData["message"] = Resources.ArfStrings.message_deletion_failed + " " + contacted.Id + " (" + ex.SanitizeException() + ")";
                TempData["message_type"] = "danger";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult View(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var message = DbContext.ContactedUs.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            var contacted = new ContactMessagesViewModel
            {
                Id = message.Id,
                Created = message.Created,
                EmailAddress = message.EmailAddress,
                FullName = message.FullName,
                Message = message.Message,
                Subject = message.Subject
            };
            return View(contacted);
        }
    }
}