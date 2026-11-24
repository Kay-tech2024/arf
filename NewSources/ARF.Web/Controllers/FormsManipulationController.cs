using ARF.Core;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    public class FormsManipulationController : BaseController
    {
        ArfDbEntities db = Utils.db;

        [HttpGet]
        public ActionResult Index()
        {
            var model = db.Forms.OrderBy(e => e.NameEnglish).ToList();

            ViewBag.Roles = getRoles();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int? FormId, string NameArabic, string NameEnglish, bool? IsActive, string[] VisibleToRoles, string[] InvisibleToRoles)
        {

            string visibletoroles;
            string invisibletoroles;

            if (VisibleToRoles != null && VisibleToRoles.Length > 0)
            {
                visibletoroles = string.Join(",", VisibleToRoles);
            }
            else
            {
                visibletoroles = null;
            }

            if (InvisibleToRoles != null && InvisibleToRoles.Length > 0)
            {
                invisibletoroles = string.Join(",", InvisibleToRoles);
            }
            else
            {
                invisibletoroles = null;
            }

            if (FormId < 1)
            {
                //new Form
                db.Forms.Add(new Form()
                {
                    Key = NameEnglish.Trim(' '),
                    NameArabic = NameArabic,
                    NameEnglish = NameEnglish,
                    IsActive = (bool)IsActive,
                    InvisibleToRoles = invisibletoroles,
                    VisibleToRoles = visibletoroles
                });
            }
            else
            {
                var form = db.Forms.Where(e => e.Id == FormId).FirstOrDefault();

                form.NameArabic = NameArabic;
                form.NameEnglish = NameEnglish;
                form.IsActive = (bool)IsActive;
                form.VisibleToRoles = visibletoroles;
                form.InvisibleToRoles = invisibletoroles;

            }

            db.SaveChanges();
            ViewBag.Roles = getRoles();

            var model = db.Forms.OrderBy(e => e.NameEnglish).ToList();

            return View(model);


        }

        [HttpGet]
        public ActionResult Sections(int? id)
        {
            ViewBag.Roles = getRoles();
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var s = Utils.db.Forms.Where(e => e.Id == id).Select(e => new { Name = e.NameArabic, e.Id }).FirstOrDefault();
            ViewBag.FormId = s.Id;
            ViewBag.FormName = s.Name;

            var model = Utils.db.FormSections.Where(e => e.FormId == id).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sections(int FormId, int? SId, string NameArabic, string NameEnglish, bool? IsActive, string[] VisibleToRoles, string[] InvisibleToRoles)
        {
            string visibletoroles;
            string invisibletoroles;

            if (VisibleToRoles != null && VisibleToRoles.Length > 0)
            {
                visibletoroles = string.Join(",", VisibleToRoles);
            }
            else
            {
                visibletoroles = null;
            }

            if (InvisibleToRoles != null && InvisibleToRoles.Length > 0)
            {
                invisibletoroles = string.Join(",", InvisibleToRoles);
            }
            else
            {
                invisibletoroles = null;
            }

            if (SId < 1)
            {
                //new Section
                var maxkey = db.FormSections.Where(e => e.FormId == FormId).Max(e => e.Key);

                var maxkeychar = ' ';
                if (maxkey == null)
                {
                    maxkeychar = 'A';
                }
                else
                {
                    maxkeychar = maxkey[0];
                    maxkeychar++;
                }

                db.FormSections.Add(new FormSection()
                {
                    Key = maxkeychar.ToString(),
                    NameArabic = NameArabic,
                    NameEnglish = NameEnglish,
                    IsActive = (bool)IsActive,
                    InvisibleToRoles = invisibletoroles,
                    VisibleToRoles = visibletoroles,
                    DependsOnFieldId = null,
                    DependsOnFieldValue = null,
                    FormId = FormId
                });
            }
            else
            {
                //edit section
                var section = db.FormSections.Where(e => e.Id == SId).FirstOrDefault();

                section.NameArabic = NameArabic;
                section.NameEnglish = NameEnglish;
                section.IsActive = (bool)IsActive;
                section.VisibleToRoles = visibletoroles;
                section.InvisibleToRoles = invisibletoroles;
            }

            db.SaveChanges();
            ViewBag.Roles = getRoles();
            var s = Utils.db.Forms.Where(e => e.Id == FormId).Select(e => new { Name = e.NameArabic, e.Id }).FirstOrDefault();
            ViewBag.FormId = s.Id;
            ViewBag.FormName = s.Name;
            var model = Utils.db.FormSections.Where(e => e.FormId == FormId).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult EditSections(int? id, string NameArabic, string NameEnglish)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var s = Utils.db.Forms.Where(e => e.Id == id).Select(x => x.NameArabic).FirstOrDefault();

            ViewBag.FormName = s;

            var model = Utils.db.FormSections.Where(e => e.FormId == id).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult FormFields(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            else
            {
                var ids = id.Split(',');

                var FormId = int.Parse(ids[0]);

                var SectionId = int.Parse(ids[1]);

                var f = Utils.db.Forms.Where(e => e.Id == FormId).FirstOrDefault();
                var s = Utils.db.FormSections.Where(e => e.Id == SectionId).FirstOrDefault();

                ViewBag.FormName = f.NameArabic;
                ViewBag.SectionName = s.NameArabic;

                var fields = db.FormFields
                    .Where(e => e.FormId == FormId && e.SectionId == SectionId && e.IsActive).OrderBy(x => x.Order)
                    .ToList();

                foreach (var item in fields)
                {
                    var idd = item.DependsOnFieldId;
                    var iddv = db.FormFields.Where(e => e.SectionId == SectionId && e.IsActive && e.Id == idd).Select(e => e.Order).FirstOrDefault();
                    if (iddv > 0)
                    {
                        item.DependsOnFieldId = iddv;
                    }

                }

                List<FormField> lff = new List<FormField>();
                lff.Add(new FormField()
                {
                    FormId = FormId,
                    SectionId = SectionId,
                    Id = -1,
                    NameArabic = "",
                    NameEnglish = "",
                    DependsOnFieldId = null,
                    DependsOnFieldValue = null,
                    IsActive = false,
                    IsReadOnly = false,
                    IsRequired = false,
                    IsMultipleSelection = false,
                    Key = null,
                    MaxValue = null,
                    MinValue = null
                });
                lff.AddRange(fields);
                var model = new FormFieldEditModel
                {
                    Fields = lff
                };

                return View("FormFields", model);
            }
        }

        [HttpGet]
        public ActionResult Lookups(int id, int formid, int sectionid)
        {
            var lk = new Lookup();
            var field = db.FormFields.Where(e => e.Id == id).FirstOrDefault();
            if (field.LookupId == null)
            {
                lk.Key = field.Key + field.Id;
                lk.IsActive = true;
                db.Lookups.Add(lk);
                field.LookupId = lk.Id;
                db.SaveChanges();
                var model = new List<LookupValue>();
                model.Add(new LookupValue()
                {
                    LookupId = lk.Id,
                    Id = -1
                });
                return View(new LookupValuesEditModel()
                {
                    LookupValues = model
                });
            }
            else
            {
                var model = db.LookupValues.Where(e => e.LookupId == field.LookupId).ToList();
                if (model == null || model.Count < 1)
                {
                    model.Add(new LookupValue()
                    {
                        LookupId = (long)field.LookupId,
                        Id = -1
                    });
                }
                return View(new LookupValuesEditModel()
                {
                    LookupValues = model
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Lookups(LookupValuesEditModel f, int id, int formid, int sectionid)
        {
            var transid = formid.ToString() + ',' + sectionid.ToString();

            for (int i = 0; i < f.LookupValues.Count; i++)
            {
                if (f.LookupValues[i].Id == -1)
                {
                    continue;
                }
                if (f.LookupValues[i].NameArabic == null)
                {
                    ModelState.AddModelError("LookupValues[" + i + "].NameArabic", "NameArabic is required");
                    return View(f);
                }
                else if (f.LookupValues[i].NameEnglish == null)
                {
                    ModelState.AddModelError("LookupValues[" + i + "].NameEnglish", "NameEnglish is required");
                    return View(f);
                }
                else if(f.LookupValues[i].Id == 0)
                {
                    db.LookupValues.Add(new LookupValue()
                    {
                        Key = f.LookupValues[i].LookupId + f.LookupValues[i].NameEnglish.Trim(' '),
                        LookupId = f.LookupValues[i].LookupId,
                        NameArabic = f.LookupValues[i].NameArabic,
                        NameEnglish = f.LookupValues[i].NameEnglish
                    });
                }
                else
                {
                    var lvid = f.LookupValues[i].Id;
                    var val = db.LookupValues.Where(e => e.Id == lvid).FirstOrDefault();
                    val.NameArabic = f.LookupValues[i].NameArabic;
                    val.NameEnglish = f.LookupValues[i].NameEnglish;
                    db.SaveChanges();
                }
            }
            db.SaveChanges();
            return RedirectToAction("FormFields", new { id = transid });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormFields(FormFieldEditModel model, string id)
        {
            var ids = id.Split(',');
            var FormId = int.Parse(ids[0]);
            var SectionId = int.Parse(ids[1]);

            var fr = Utils.db.Forms.Where(e => e.Id == FormId).FirstOrDefault();
            var s = Utils.db.FormSections.Where(e => e.Id == SectionId).FirstOrDefault();

            ViewBag.FormName = fr.NameArabic;
            ViewBag.SectionName = s.NameArabic;

            if (model.Fields.Count > 0)
            {
                foreach (var item in model.Fields)
                {
                    if (item.Id > 0)
                    {
                        var currid = item.Id;
                        var curritem = db.FormFields.Where(e => e.Id == currid).FirstOrDefault();
                        curritem.InvisibleToRoles = item.InvisibleToRoles;
                        curritem.IsActive = item.IsActive;
                        curritem.IsMultipleSelection = item.IsMultipleSelection;
                        curritem.LookupId = item.LookupId;
                        curritem.MaxValue = item.MaxValue;
                        curritem.MinValue = item.MinValue;
                        curritem.NameArabic = item.NameArabic;
                        curritem.NameEnglish = item.NameEnglish;
                        curritem.Order = item.Order;
                        curritem.VisibleToRoles = item.VisibleToRoles;
                        curritem.FieldType = item.FieldType;
                        curritem.IsRequired = item.IsRequired;
                        curritem.DependsOnFieldValue = item.DependsOnFieldValue;
                        curritem.IsReadOnly = item.IsReadOnly;
                        curritem.IsSummable = item.IsSummable;
                        curritem.DependsOnFieldId = null;
                        db.SaveChanges();
                    }
                    else if (item.Id == 0)
                    {
                        var f = new FormField()
                        {
                            Key = item.NameEnglish.Trim(' '),
                            SectionId = SectionId,
                            FormId = FormId,
                            InvisibleToRoles = item.InvisibleToRoles,
                            IsActive = item.IsActive,
                            IsMultipleSelection = item.IsMultipleSelection,
                            LookupId = item.LookupId,
                            MaxValue = item.MaxValue,
                            MinValue = item.MinValue,
                            NameArabic = item.NameArabic,
                            NameEnglish = item.NameEnglish,
                            Order = item.Order,
                            VisibleToRoles = item.VisibleToRoles,
                            FieldType = item.FieldType,
                            IsRequired = item.IsRequired,
                            DependsOnFieldValue = item.DependsOnFieldValue,
                            IsReadOnly = item.IsReadOnly,
                            IsSummable = item.IsSummable,
                            DependsOnFieldId = null
                        };
                        db.FormFields.Add(f);
                        db.SaveChanges();
                        item.Id = f.Id;
                    }
                    if (item.Id > -1)
                    {
                        var currid2 = item.Id;
                        var curritem2 = db.FormFields.Where(e => e.Id == currid2).FirstOrDefault();
                        curritem2.DependsOnFieldId = model.Fields.Where(e => e.Order == item.DependsOnFieldId && item.IsActive).Select(x => (int?)x.Id).FirstOrDefault();
                    }
                    db.SaveChanges();
                }


                //foreach (var item in model.Fields.Where(e => e.Id > 0))
                //{
                //    var currid = item.Id;
                //    var curritem = db.FormFields.Where(e => e.Id == currid).FirstOrDefault();
                //    curritem.InvisibleToRoles = item.InvisibleToRoles;
                //    curritem.IsActive = item.IsActive;
                //    curritem.IsMultipleSelection = item.IsMultipleSelection;
                //    curritem.LookupId = item.LookupId;
                //    curritem.MaxValue = item.MaxValue;
                //    curritem.MinValue = item.MinValue;
                //    curritem.NameArabic = item.NameArabic;
                //    curritem.NameEnglish = item.NameEnglish;
                //    curritem.Order = item.Order;
                //    curritem.VisibleToRoles = item.VisibleToRoles;
                //    //curritem.DependsOnFieldId = model.Fields.Where(e => e.Order == item.DependsOnFieldId && item.IsActive).Select(x => x.Id).FirstOrDefault();
                //    curritem.FieldType = item.FieldType;
                //    curritem.IsRequired = item.IsRequired;
                //    curritem.DependsOnFieldValue = item.DependsOnFieldValue;
                //    curritem.IsReadOnly = item.IsReadOnly;
                //    curritem.IsSummable = item.IsSummable;
                //    db.SaveChanges();
                //}

                //foreach (var item in model.Fields.Where(e => e.Id < 1).OrderBy(e => e.Order))
                //{
                //    db.FormFields.Add(new FormField()
                //    {
                //        SectionId = SectionId,
                //        FormId = FormId,
                //        InvisibleToRoles = item.InvisibleToRoles,
                //        IsActive = item.IsActive,
                //        IsMultipleSelection = item.IsMultipleSelection,
                //        LookupId = item.LookupId,
                //        MaxValue = item.MaxValue,
                //        MinValue = item.MinValue,
                //        NameArabic = item.NameArabic,
                //        NameEnglish = item.NameEnglish,
                //        Order = item.Order,
                //        VisibleToRoles = item.VisibleToRoles,
                //        //DependsOnFieldId = model.Fields.Where(e => e.Order == item.DependsOnFieldId && item.IsActive).Select(x => x.Id).FirstOrDefault(),
                //        FieldType = item.FieldType,
                //        IsRequired = item.IsRequired,
                //        DependsOnFieldValue = item.DependsOnFieldValue,
                //        IsReadOnly = item.IsReadOnly,
                //        IsSummable = item.IsSummable
                //    });
                //    db.SaveChanges();
                //}
            }

            return RedirectToAction("FormFields", new { id = id });


        }

        public MultiSelectList getRoles()
        {
            var le = db.Roles.ToList();

            return new MultiSelectList(le, "Id", "NameArabic");
        }

    }
}