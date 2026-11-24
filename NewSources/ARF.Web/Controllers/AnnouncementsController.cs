using ARF.Core;
using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using ARF.Web.App_Start.Code;
using ARF.Web.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [CustomAuthorize]
    public class AnnouncementsController : BaseController
    {
        // GET: Announcements
        public ActionResult Index()
        {
            var announcements = DbContext.Announcements
                .OrderByDescending(l => l.StartDateGreg)
                .ToList()
                .Select(l => new AnnouncementViewModel
                {
                    Id = l.Id,
                    Title = l.Title,
                    Content = l.Content,
                    StartDate = l.StartDateHijri,
                    EndDate = l.EndDateHijri,
                    Status = LangHelpers.Instance[l.StartDateGreg > TM.Now ? "not_started" : (l.EndDateGreg < TM.Now ? "completed" : "ongoing")]
                })
                .ToList();

            return View(announcements);
        }

        [HttpGet]
        [CustomAuthorize(ArfUserRole.DeanofDeanshipofScientificResearch)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize(ArfUserRole.DeanofDeanshipofScientificResearch)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AnnouncementCreateViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.StartDate) || string.IsNullOrWhiteSpace(model.EndDate))
            {
                return RedirectToAction("Create");
            }


            var startDate = Utils.ToHijriDate(model.StartDate);
            var endDate = Utils.ToHijriDate(model.EndDate);


            if (string.IsNullOrWhiteSpace(model.StartDate) || startDate == null)
            {
                ModelState.AddModelError("StartDate", Resources.ArfStrings.start_date_is_incorrect);
            }

            if (string.IsNullOrWhiteSpace(model.EndDate) || endDate == null)
            {
                ModelState.AddModelError("EndDate", Resources.ArfStrings.finish_date_is_incorrect);
            }


            if (startDate != default(DateTime) && endDate != default(DateTime))
            {



                if (startDate > endDate)
                {
                    ModelState.AddModelError("EndDate", Resources.ArfStrings.finish_date_can_not_be_before_the_start_date);
                }
                else if (endDate < TM.Now)
                {
                    ModelState.AddModelError("EndDate", Resources.ArfStrings.finish_date_can_not_be_in_the_past);
                }
                else
                {
                    var prevAnnouncement = DbContext.Announcements.FirstOrDefault(r => r.StartDateGreg == startDate && r.EndDateGreg == endDate);

                    if (prevAnnouncement != null)
                    {
                        TempData["message"] = Resources.ArfStrings.this_period_is_already_exist;
                        TempData["message_type"] = "info";

                        return Redirect(Utils.GetApplicationUrl() + "Edit/" + prevAnnouncement.Id);
                    }
                    else
                    {
                        prevAnnouncement = DbContext.Announcements.FirstOrDefault(r => (r.StartDateGreg >= startDate && r.EndDateGreg <= endDate) || (r.StartDateGreg < startDate && r.EndDateGreg > startDate));

                        if (prevAnnouncement != null)
                        {

                            ModelState.AddModelError("EndDate", Resources.ArfStrings.there_is_another_period_conflict_with_current_period);
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                DbContextTransaction trans = null;
                try
                {
                    using (trans = DbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                    {
                        var ann = new Announcement
                        {
                            Title = model.Title.Trim(),
                            Content = model.Content.Trim(),
                            StartDateHijri = model.StartDate.Trim(),
                            EndDateHijri = model.EndDate.Trim(),
                            StartDateGreg = startDate.Value,
                            EndDateGreg = endDate.Value,
                            Created = TM.Now,
                            Updated = TM.Now,
                            CreatedBy = CurrentUser.Id,
                            UpdatedBy = CurrentUser.Id
                        };

                        var uniqueId = startDate.Value.FormatHijriDate(false, true) + "/" + endDate.Value.FormatHijriDate(false, true);

                        var req = new Request
                        {
                            Title = model.Title,
                            Status = (long)(startDate > TM.Now ? RequestStatus.NotStarted : RequestStatus.Started),
                            Type = (byte)Data.RequestType.AnnouncementRequest,
                            StartDate = startDate,
                            UniqueId = uniqueId,
                            Created = TM.Now,
                            Updated = TM.Now,
                            CreatedBy = CurrentUser.Id,
                            UpdatedBy = CurrentUser.Id
                        };


                        DbContext.Requests.Add(req);
                        DbContext.Announcements.Add(ann);
                        DbContext.SaveChanges();

                        req.AnnouncementId = ann.Id;
                        ann.RequestId = req.Id;

                        DbContext.Entry(req).State = System.Data.Entity.EntityState.Modified;
                        DbContext.Entry(ann).State = System.Data.Entity.EntityState.Modified;
                        DbContext.SaveChanges();


                        var designerRequest = new Request
                        {
                            Title = model.Title,
                            AnnouncementId = ann.Id,
                            PublicationRequestId = req.Id,
                            Status = (long)RequestStatus.DesignRequestReceivedByDirectorOfProgramOfFinance,
                            Type = (byte)Data.RequestType.DesignRequest,
                            StartDate = startDate,
                            UniqueId = req.Id + "-DS",
                            Created = TM.Now,
                            Updated = TM.Now,
                            CreatedBy = CurrentUser.Id,
                            UpdatedBy = CurrentUser.Id
                        };


                        DbContext.Requests.Add(designerRequest);

                        DbContext.SaveChanges();

                        trans.Commit();
                    }



                    TempData["message"] = Resources.ArfStrings.advertisement_created_successfully;
                    TempData["message_type"] = "success";

                    return RedirectToAction("Index");

                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    Logger.LogExceptionAsync(ex, model);
                    TempData["message"] = Resources.ArfStrings.creation_operation_failed+ ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage + ")";
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

                    TempData["message"] = Resources.ArfStrings.creation_operation_failed + ex2.Message + ")";
                    TempData["message_type"] = "danger";
                }
                finally
                {

                }
            }

            return View(model);
        }


        [HttpGet]
        public ActionResult View(int id)
        {
            var ann = DbContext.Announcements
                .Include("Request")
                .Include("Request.RequestFiles")
                .FirstOrDefault(a => a.Id == id);

            if (ann == null)
            {
                TempData["message"] = Resources.ArfStrings.this_period_is_not_exist;
                TempData["message_type"] = "info";
                return RedirectToAction("Index");
            }



            var model = new AnnouncementEditViewModel
            {
                Id = ann.Id,
                Title = ann.Title,
                Content = ann.Content,
                StartDate = ann.StartDateHijri,
                EndDate = ann.EndDateHijri,
                Files = ann.Request.RequestFiles.Select(c => new AttachedFile
                {
                    FileId = c.Id.ToString(),
                    Type = (FileType)c.Type,
                    FileName = Path.GetFileName(c.FileName)
                }).ToList()
            };

            return View("View", model);
        }


        [HttpGet]
        [CustomAuthorize(ArfUserRole.DeanofDeanshipofScientificResearch)]
        public ActionResult Edit(int id)
        {
            var ann = DbContext.Announcements
                .Include("Request")
                .Include("Request.RequestFiles")
                .FirstOrDefault(a => a.Id == id);

            if (ann == null)
            {
                TempData["message"] =Resources.ArfStrings.this_period_is_not_exist;
                TempData["message_type"] = "info";
                return RedirectToAction("Index");
            }


            if (ann.EndDateGreg < TM.Now)
            {

                TempData["message"] = Resources.ArfStrings.this_period_is_over_and_can_not_be_edit;
                TempData["message_type"] = "info";

                return Redirect(Utils.GetApplicationUrl() + "Announcements/View/" + ann.Id);
            }

            var model = new AnnouncementEditViewModel
            {
                Id = ann.Id,
                Title = ann.Title,
                Content = ann.Content,
                StartDate = ann.StartDateHijri,
                EndDate = ann.EndDateHijri,
                Files = ann.Request.RequestFiles.Select(c => new AttachedFile
                {
                    FileId = c.Id.ToString(),
                    Type = (FileType)c.Type,
                    FileName = Path.GetFileName(c.FileName)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(ArfUserRole.DeanofDeanshipofScientificResearch)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AnnouncementEditViewModel model)
        {
            var ann = (Announcement)null;

            if (model == null || model.Id <= 0 || (ann = DbContext.Announcements
               .Include("Request")
               .Include("Request.RequestFiles")
               .FirstOrDefault(a => a.Id == model.Id)) == null)
            {
                TempData["message"] = Resources.ArfStrings.this_period_is_not_exist;
                TempData["message_type"] = "info";
                return RedirectToAction("Index");
            }



            if (ann.EndDateGreg < TM.Now)
            {

                TempData["message"] = Resources.ArfStrings.this_period_is_over_and_can_not_be_edit;
                TempData["message_type"] = "info";

                return Redirect(Utils.GetApplicationUrl() + "Announcements/View/" + ann.Id);
            }

            var startDate = Utils.ToHijriDate(model.StartDate);
            var endDate = Utils.ToHijriDate(model.EndDate);


            if (string.IsNullOrWhiteSpace(model.StartDate) || startDate == null)
            {
                ModelState.AddModelError("StartDate", Resources.ArfStrings.start_date_is_incorrect);
            }

            if (string.IsNullOrWhiteSpace(model.EndDate) || endDate == null)
            {
                ModelState.AddModelError("EndDate", Resources.ArfStrings.finish_date_is_incorrect);
            }

            if (startDate != default(DateTime) && endDate != default(DateTime))
            {

                if (startDate > endDate)
                {
                    ModelState.AddModelError("EndDate", Resources.ArfStrings.finish_date_can_not_be_before_the_start_date);
                }
                else if (endDate < TM.Now)
                {
                    ModelState.AddModelError("EndDate", Resources.ArfStrings.finish_date_can_not_be_in_the_past);
                }
                else
                {
                    var prevAnnouncement = DbContext.Announcements.FirstOrDefault(r => r.StartDateGreg == startDate && r.EndDateGreg == endDate);

                    if (prevAnnouncement != null && prevAnnouncement.Id != ann.Id)
                    {
                        TempData["message"] = Resources.ArfStrings.this_period_is_already_exist;
                        TempData["message_type"] = "info";
                        return RedirectToAction("Edit", new { id = prevAnnouncement.Id });
                    }
                    else
                    {
                        prevAnnouncement = DbContext.Announcements.FirstOrDefault(r => (r.StartDateGreg >= startDate && r.EndDateGreg <= endDate) || (r.StartDateGreg < startDate && r.EndDateGreg > startDate));

                        if (prevAnnouncement != null && prevAnnouncement.Id != ann.Id)
                        {

                            ModelState.AddModelError("EndDate", Resources.ArfStrings.there_is_another_period_conflict_with_current_period);
                        }
                    }
                }
            }



            if (ann.StartDateGreg <= TM.Now && ann.EndDateGreg > TM.Now)
            {

                if (ann.StartDateGreg != startDate.Value)
                {
                    TempData["message"] = Resources.ArfStrings.this_period_is_running_and_can_not_be_editable;
                    TempData["message_type"] = "info";

                    ModelState.AddModelError("StartDate", Resources.ArfStrings.this_period_is_running_and_can_not_be_editable);
                }
            }

            if (ModelState.IsValid)
            {
                DbContextTransaction trans = null;
                try
                {
                    using (trans = DbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                    {
                        ann.Title = model.Title.Trim();
                        ann.StartDateHijri = model.StartDate.Trim();
                        ann.EndDateHijri = model.EndDate.Trim();
                        ann.StartDateGreg = startDate.Value;
                        ann.EndDateGreg = endDate.Value;
                        ann.Content = model.Content.Trim();
                        ann.Updated = TM.Now;
                        ann.UpdatedBy = CurrentUser.Id;

                        DbContext.Entry(ann).State = EntityState.Modified;



                        var uniqueId = startDate.Value.FormatHijriDate(false, true) + "_" + endDate.Value.FormatHijriDate(false, true);

                        if (uniqueId != ann.Request.UniqueId)
                        {
                            ann.Request.UniqueId = uniqueId;
                            ann.Request.Announcement = ann;

                            DbContext.Entry(ann.Request).State = EntityState.Modified;
                        }


                        DbContext.SaveChanges();

                        trans.Commit();
                    }



                    TempData["message"] = Resources.ArfStrings.advertisement_created_successfully;
                    TempData["message_type"] = "success";

                    return RedirectToAction("Index");

                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    Logger.LogExceptionAsync(ex, model);
                    TempData["message"] = Resources.ArfStrings.creation_operation_failed+ ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage + ")";
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

                    TempData["message"] = Resources.ArfStrings.creation_operation_failed + ex2.Message + ")";
                    TempData["message_type"] = "danger";
                }
                finally
                {

                }
            }

            model.Files = ann.Request.RequestFiles.Select(c => new AttachedFile
            {
                FileId = c.Id.ToString(),
                Type = (FileType)c.Type,
                FileName = Path.GetFileName(c.FileName)
            }).ToList();

            return View(model);
        }
    }
}