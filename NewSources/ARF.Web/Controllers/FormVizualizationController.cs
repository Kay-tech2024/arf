using ARF.Core;
using ARF.Data.DbModels;
using ARF.Web.Models;
using ARF.Web.App_Start.DWF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ARF.Data;

namespace ARF.Web.Controllers
{
    public class FormVizualizationController : BaseController
    {
        ArfDbEntities db = Utils.db;
        public ActionResult Index()
        {
            var model = db.Forms.OrderBy(e => e.NameEnglish).ToList();

            return View(model);


        }

        public ActionResult Form(int? id)
        {
            var form = Utils.db.Forms.Where(e => e.Id == id).FirstOrDefault();

            if (form == null) return RedirectToAction("Index");

            var s = form.NameArabic;

            ViewBag.FormName = s;

            if (id == null)
            {
                var model = db.Forms.OrderBy(e => e.NameEnglish).ToList();

                return View(model);
            }
            else
            {
                var model = db.FormSections.Where(e => e.FormId == id).OrderBy(e => e.Key)
                    .ToList()
                    .Select(e => new SectionsViewModel
                    {
                        FormId = e.FormId,
                        Id = e.Id,
                        IsActive = e.IsActive,
                        Key = e.Key,
                        Name = IsArabic ? e.NameArabic : e.NameEnglish,
                        FormFields = e.FormFields.Where(f => f.IsActive).OrderBy(f => f.Order).ThenBy(f => f.Id).Select(f => new FormFieldModel
                        {
                            Id = f.Id,
                            FormId = f.FormId,
                            LookupId = f.LookupId,
                            SectionId = f.SectionId,
                            Key = f.Key,
                            Name = IsArabic ? f.NameArabic : f.NameEnglish,


                            FieldType = (FormFieldType)f.FieldType,
                            IsRequired = f.IsRequired,
                            IsMultipleSelection = f.IsMultipleSelection,
                            IsActive = f.IsActive,
                            MinValue = f.MinValue == null ? (int?)null : int.Parse(f.MinValue),
                            MaxValue = f.MaxValue == null ? (int?)null : int.Parse(f.MaxValue),
                            DependsOnFieldId = f.DependsOnFieldId,
                            DependsOnFieldValue = f.DependsOnFieldValue,
                            VisibleToRoles = f.VisibleToRoles,
                            InvisibleToRoles = f.InvisibleToRoles,
                            Order = f.Order,
                            IsReadOnly = f.IsReadOnly,
                            Lookups = f.LookupId > 0 ? f.Lookup.LookupValues.Select(v => new SelectListItem
                            {
                                Value = v.Id + "",
                                Text = IsArabic ? v.NameArabic : v.NameEnglish
                            }).ToList() : null

                        }).ToList()
                    })
                    .ToList();


                return View("ViewForms", model);
            }
        }

    }
}