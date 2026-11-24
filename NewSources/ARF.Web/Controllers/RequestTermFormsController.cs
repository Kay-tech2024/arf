using ARF.Data.DbModels;
using ARF.TimeMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    public class RequestTermFormsController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult MidTermExamination(long id)
        {
            Request request = DbContext.Requests.Find(id);
            if (request == null) {
                return RedirectToAction("index", "Requests");
            }
            ViewBag.request = request;
            return View("ExaminationOfExistingMidTermForm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MidTermExamination(RequestMidTermCheck examinationRequest)
        {
            examinationRequest.RecordDate = TM.Now;
            examinationRequest.Id = Guid.NewGuid();

            try
            {
                DbContext.RequestMidTermChecks.Add(examinationRequest);
                DbContext.SaveChanges();
                return RedirectToAction("index", "Requests");
            }
            catch (Exception e) {
                Request request = DbContext.Requests.Find(examinationRequest.RequestId);
                ViewBag.request = request;
                return View("ExaminationOfExistingMidTermForm", examinationRequest);
            }        

        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: RequestTermForms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: RequestTermForms/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RequestTermForms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: RequestTermForms/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RequestTermForms/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
