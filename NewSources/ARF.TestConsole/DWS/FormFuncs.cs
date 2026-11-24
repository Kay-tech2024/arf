using ARF.Core;
using ARF.Data;
using ARF.Data.DbModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ARF.TestConsole.DWS
{
    public static class FormFuncs
    {
        public static void SaveForms(string inputFile)
        {
            using (var pkg = new ExcelPackage(new FileInfo(inputFile)))
            {
                var sheet = pkg.Workbook.Worksheets["ARF"];
                var rowCount = sheet.Dimension.End.Row;
                var columnCount = sheet.Dimension.End.Column;

                var columns = new string[] { "ID", "FormNo", "FormName", "NameArabic", "NameEnglish", "Type", "ReadOnly", "Required", "MinimumCharacters", "MaximumCharacters", "Choices", "Section", "Depends on" };
                var columnIndices = new int[columnCount];

                for (var i = 0; i < columnIndices.Length; i++)
                    columnIndices[i] = -1;

                // header indices
                for (var col = 1; col <= columnCount; col++)
                {
                    var text = Utils.NormalizeString(sheet.Cells[1, col].Text);

                    for (var i = 0; i < columns.Length; i++)
                    {
                        if (Utils.NormalizeString(columns[i]) == text)
                        {
                            columnIndices[col - 1] = i;
                            break;
                        }
                    }
                }

                var data = new List<FormExcelModel>();

                for (var row = 2; row <= rowCount; row++)
                {
                    var model = new FormExcelModel();

                    for (var col = 1; col <= columnCount; col++)
                    {
                        if (columnIndices[col - 1] < 0) continue;
                        var colName = columns[columnIndices[col - 1]];
                        var value = sheet.Cells[row, col].Text.Trim();

                        switch (colName)
                        {
                            case "ID":
                                model.ID = value;
                                break;

                            case "FormNo":
                                model.FormNo = value;
                                break;

                            case "FormName":
                                model.FormName = value;
                                break;

                            case "NameArabic":
                                model.NameArabic = value;
                                break;

                            case "NameEnglish":
                                model.NameEnglish = value;
                                break;

                            case "Type":
                                model.Type = value;
                                break;

                            case "ReadOnly":
                                model.ReadOnly = value;
                                break;

                            case "Required":
                                model.Required = value;
                                break;

                            case "MinimumCharacters":
                                model.MinimumCharacters = value;
                                break;

                            case "MaximumCharacters":
                                model.MaximumCharacters = value;
                                break;

                            case "Choices":
                                model.Choices = value;
                                break;

                            case "Section":
                                model.Section = value;
                                break;

                            case "Depends on":
                            case "DependsOn":
                                model.DependsOn = value;
                                break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.FormName) && !string.IsNullOrWhiteSpace(model.ID))
                        data.Add(model);
                }

                var db = Utils.db;

                // delete all forms & fields

                db.Database.ExecuteSqlCommand("update FormSections set DependsOnFieldID = null");
                db.Database.ExecuteSqlCommand("update FormFields set LookUpId = null");
                db.Database.ExecuteSqlCommand("delete LookupValues");
                db.Database.ExecuteSqlCommand("delete Lookups");
                db.Database.ExecuteSqlCommand("delete FormFields");
                db.Database.ExecuteSqlCommand("delete FormSections");
                db.Database.ExecuteSqlCommand("delete Forms");

                var forms = data.Select(m => m.FormName).Distinct().ToList();

                foreach (var formName in forms)
                {
                    //if (formName != "SocialForm") continue;
                    var form = new Form
                    {
                        Key = formName,
                        NameArabic = Utils.IsArabicString(formName) ? formName : null,
                        NameEnglish = !Utils.IsArabicString(formName) ? formName : null,
                        IsActive = true,
                    };

                    db.Forms.Add(form);
                    db.SaveChanges();

                    var addedSections = new Dictionary<string, FormSection>();

                    foreach (var sectionName in data.Where(f => f.FormName == formName).Select(f => f.Section).Distinct().ToList())
                    {
                        var sec = "";
                        if (string.IsNullOrWhiteSpace(sectionName))
                            sec = "A";
                        else
                            sec = sectionName.ToUpper().Trim();

                        var section = (FormSection)null;

                        if (addedSections.ContainsKey(sec))
                        {
                            section = addedSections[sec];
                        }
                        else
                        {
                            section = new FormSection
                            {
                                Key = sec,
                                FormId = form.Id,
                                NameEnglish = sec,
                                NameArabic = sec,
                            };

                            db.FormSections.Add(section);
                            db.SaveChanges();
                        }

                        var addedFields = new Dictionary<string, FormField>();
                        var addedFieldsNum = new Dictionary<string, FormField>();

                        var isTableStarted = false;
                        var isTableHeadingEnded = false;
                        var tableColumnCount = 0;
                        var currentTableColumnCount = 0;

                        var order = 1;
                        foreach (var fieldModel in data.Where(f => f.FormName == formName && f.Section == sectionName))
                        {
                            if (string.IsNullOrWhiteSpace(fieldModel.NameArabic) && string.IsNullOrWhiteSpace(fieldModel.NameEnglish)) continue;
                            if (string.IsNullOrWhiteSpace(fieldModel.Type)) continue;

                            fieldModel.Type = fieldModel.Type.Replace(" ", "").Replace("&", "").ToUpper();

                            var key = !string.IsNullOrWhiteSpace(fieldModel.NameEnglish) ? fieldModel.NameEnglish.Replace(" ", "") : fieldModel.NameArabic.Trim();

                            var k = 0;
                            while (addedFields.ContainsKey(key))
                            {
                                key = key + "_" + (++k);
                            }

                            var type = FormFieldType.Unspecified;

                            if (!Enum.TryParse(fieldModel.Type, true, out type)) continue;

                            if (!isTableStarted)
                            {
                                if (type == FormFieldType.TableHeading)
                                {
                                    isTableStarted = true;
                                    isTableHeadingEnded = false;
                                    tableColumnCount = 0;
                                    currentTableColumnCount = 0;

                                    var nfield = new FormField
                                    {
                                        Key = key,
                                        FormId = form.Id,
                                        SectionId = section.Id,
                                        Order = order++,
                                        IsRequired = false,
                                        IsReadOnly = true,
                                        NameEnglish = "table_start",
                                        NameArabic = "table_start",
                                        FieldType = (byte)FormFieldType.Table,
                                    };

                                    db.FormFields.Add(nfield);
                                }
                            }
                            else
                            {
                                if (!isTableHeadingEnded)
                                {
                                    if (type == FormFieldType.TableHeading)
                                    {
                                        tableColumnCount++;
                                    }
                                    else
                                    {
                                        isTableHeadingEnded = true;
                                    }
                                }
                                else
                                {
                                    if (currentTableColumnCount + 1 > tableColumnCount)
                                    {
                                        isTableStarted = false;

                                        var nfield = new FormField
                                        {
                                            Key = key,
                                            FormId = form.Id,
                                            SectionId = section.Id,
                                            Order = order++,
                                            IsRequired = false,
                                            IsReadOnly = true,
                                            NameEnglish = "table_end",
                                            NameArabic = "table_end",
                                            FieldType = (byte)FormFieldType.TableEnd,
                                        };

                                        db.FormFields.Add(nfield);
                                    }

                                    currentTableColumnCount++;
                                }
                            }

                            var minStr = (string)null;
                            var maxStr = (string)null;

                            if (type == FormFieldType.Date || type == FormFieldType.DateTime)
                            {
                            }
                            else
                            {
                                int min = 0;
                                int max = 0;

                                if (int.TryParse(fieldModel.MinimumCharacters, out min) && min >= 0)
                                {
                                    minStr = min + "";
                                }

                                if (int.TryParse(fieldModel.MaximumCharacters, out max) && max > 0)
                                {
                                    maxStr = max + "";
                                }
                            }

                            if (type == FormFieldType.TableHeading)
                            {
                                type = FormFieldType.ColumnHeading;
                            }
                            else if (type == FormFieldType.checkbox)
                            {
                                type = FormFieldType.Dropdown;
                                if (string.IsNullOrWhiteSpace(fieldModel.Choices))
                                {
                                    fieldModel.Choices = "نعم|لا";
                                }
                            }

                            var field = new FormField
                            {
                                Key = key,
                                FormId = form.Id,
                                SectionId = section.Id,
                                Order = order++,
                                IsRequired = Utils.StringToBoolean(fieldModel.Required) == true,
                                IsReadOnly = Utils.StringToBoolean(fieldModel.ReadOnly) == true,
                                NameEnglish = fieldModel.NameEnglish,
                                NameArabic = fieldModel.NameArabic,
                                FieldType = (byte)type,
                                MinValue = minStr,
                                MaxValue = maxStr,
                            };

                            if (!string.IsNullOrWhiteSpace(fieldModel.Choices))
                            {
                                var tabs = fieldModel.Choices.Split('|').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                                if (tabs.Count > 1)
                                {
                                    var predicate = PredicateBuilder.BaseOr<LookupValue>();

                                    foreach (var t in tabs)
                                    {
                                        predicate = predicate.Or(x => x.NameArabic == t || x.NameEnglish == t);
                                    }

                                    var lookupM = db.LookupValues
                                        .Where(predicate)
                                        .GroupBy(x => x.Lookup).
                                        Select(x => new { LookupId = x.Key.Id, Count = x.Count() })
                                        .ToList()
                                        .FirstOrDefault(x => x.Count == tabs.Count);

                                    if (lookupM != null)
                                    {
                                        field.LookupId = lookupM.LookupId;
                                    }
                                    else
                                    {
                                        var lookup = new Lookup
                                        {
                                            Key = key.ToString(),
                                            IsActive = true,
                                        };

                                        db.Lookups.Add(lookup);
                                        db.SaveChanges();

                                        foreach (var t in tabs)
                                        {
                                            var value = new LookupValue
                                            {
                                                LookupId = lookup.Id,
                                                Key = t,
                                                NameArabic = Utils.IsArabicString(t) ? t : null,
                                                NameEnglish = !Utils.IsArabicString(t) ? t : null,
                                                IsActive = true,
                                            };

                                            db.LookupValues.Add(value);
                                        }

                                        db.SaveChanges();

                                        field.LookupId = lookup.Id;
                                    }
                                }
                            }

                            db.FormFields.Add(field);

                            db.SaveChanges();

                            if (!string.IsNullOrWhiteSpace(fieldModel.DependsOn) && addedFieldsNum.ContainsKey(fieldModel.DependsOn.Trim()))
                            {
                                var otherField = addedFieldsNum[fieldModel.DependsOn.Trim()];

                                if (otherField != null)
                                {
                                    field.DependsOnFieldId = otherField.Id;
                                }
                            }

                            db.SaveChanges();

                            addedFields.Add(key, field);
                            addedFieldsNum.Add(fieldModel.ID, field);
                        }

                        if (isTableStarted)
                        {
                            isTableStarted = false;

                            var nfield = new FormField
                            {
                                Key = "table_end",
                                FormId = form.Id,
                                SectionId = section.Id,
                                Order = order++,
                                IsRequired = false,
                                IsReadOnly = true,
                                NameEnglish = "table_end",
                                NameArabic = "table_end",
                                FieldType = (byte)FormFieldType.TableEnd,
                            };

                            db.FormFields.Add(nfield);
                        }

                        db.SaveChanges();
                    }
                }
            }
        }

        public static void GenerateCodeForForms(string path, string controllerName)
        {
            if (!Directory.Exists(path) || !Directory.Exists(path + "\\Views") || !Directory.Exists(path + "\\Controllers") || !Directory.Exists(path + "\\Models")) return;

            var viewsPath = path + "\\Views\\GeneratedForms";
            var controllerPath = path + "\\Controllers\\GeneratedFormsController.cs";
            var modelsPath = path + "\\Models\\GeneratedForms.cs";

            // Structure:
            //       - Controller: GeneratedFormsController, each form has two actions: GET & POST, the section number/step is passed as a query string
            //       - Models: each form section has a model
            //       - Views: each form section has a view
            // Example link: http://localhost:50603/GeneratedForms/IslamicForm/?step=1

            if (!Directory.Exists(viewsPath))
                Directory.CreateDirectory(viewsPath);

            var db = Utils.db;
            var forms = db.Forms.ToList();
        }
    }
}