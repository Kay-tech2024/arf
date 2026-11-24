using ARF.Core;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.App_Start.DWF
{
    public static class DwfHelpers
    {
       
        public static IHtmlString RenderField(this HtmlHelper htmlHelper, FormFieldModel field, string value, bool enableValidation, ref bool tablestarted, ref bool tableHeadingStarted, ref int index, ref int tableFieldIndex)
        {
            var html = "";

            var newIndex = -1;
            var newIndexTable = -1;

            if ((FormFieldType)field.FieldType == FormFieldType.Text || (FormFieldType)field.FieldType == FormFieldType.ShortText || (FormFieldType)field.FieldType == FormFieldType.NormalText || field.FieldType == FormFieldType.Float || field.FieldType == FormFieldType.Phone || field.FieldType == FormFieldType.Email || field.FieldType == FormFieldType.Date || field.FieldType == FormFieldType.DateTime)
            {
                // done
                if (tablestarted)
                {
                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr class='template'>";
                    }

                    newIndex = index;
                    newIndexTable = ++tableFieldIndex;

                    if (field.IsReadOnly)
                    {
                        html += "<td>" +
                        "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>" +
                        "</td>";
                    }
                    else
                    {
                        html += "<td>" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldArrayData[" + (newIndexTable) + "].FieldId\" type=\"hidden\" value=\"" + field.Id + "\" />" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldArrayData[" + (newIndexTable) + "].FieldKey\" type=\"hidden\" value=\"" + field.Key + "\" />" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldArrayData[" + (newIndexTable) + "].FieldValue\" type=\"text\" class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\" value=\"" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "\"/></td>";
                    }
                }
                else
                {
                    newIndex = ++index;

                    if (field.IsReadOnly)
                    {
                        html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                        html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                        html += "<div class=\"col-md-8\">";
                        html += "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>";
                        html += "</div>";
                        html += "</div>";
                    }
                    else
                    {
                        html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                        html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                        html += "<div class=\"col-md-8\">";
                        html += "" +
                            "<input name=\"FieldData[" + (newIndex) + "].FieldId\" type=\"hidden\" value=\"" + field.Id + "\"/ >" +
                            "<input name=\"FieldData[" + (newIndex) + "].FieldKey\" type=\"hidden\" value=\"" + field.Key + "\"/ >" +
                            "<input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"text\" class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\"  value=\"" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "\"/>";
                        html += "</div>";
                        html += "</div>";
                    }
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.Heading)
            {
                html += "<h2 class=\"field-type-heading\">" + field.Name + "</h2>";
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.Integer)
            {
                newIndex = ++index;
                // done
                if (tablestarted)
                {
                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr>";
                    }

                    if (field.IsReadOnly)
                    {
                        html += "<td>" +
                        "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>" +
                        "</td>";
                    }
                    else
                    {
                        html += "<td>" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldId\" type=\"hidden\" value=\"" + field.Id + "\"/ >" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldKey\" type=\"hidden\" value=\"" + field.Key + "\"/ >" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"number\" class=\"form-control col-md-8 field-type-integer\" /></td>";
                    }
                }
                else
                {
                    html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                    html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                    html += "<div class=\"col-md-8\">";
                    html += "" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldId\" type=\"hidden\" value=\"" + field.Id + "\"/ >" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldKey\" type=\"hidden\" value=\"" + field.Key + "\"/ >" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"number\" class=\"form-control col-md-8 field-type-integer\" />";
                    html += "</div>";
                    html += "</div>";
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.Date)
            {
                newIndex = ++index;
                // done
                if (tablestarted)
                {
                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr>";
                    }

                    if (field.IsReadOnly)
                    {
                        html += "<td>" +
                        "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>" +
                        "</td>";
                    }
                    else
                    {
                        html += "<td>" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldId\" type=\"hidden\" value=\"" + field.Id + "\"/ >" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldKey\" type=\"hidden\" value=\"" + field.Key + "\"/ >" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"date\" class=\"form-control col-md-8 field-type-date\" /></td>";
                    }
                }
                else
                {
                    html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                    html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                    html += "<div class=\"col-md-8\"><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"date\" class=\"form-control col-md-8 field-type-date\" /></div>";
                    html += "</div>";
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.DateTime)
            {
                // done
                if (tablestarted)
                {
                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr>";
                    }

                    if (field.IsReadOnly)
                    {
                        html += "<td>" +
                        "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>" +
                        "</td>";
                    }
                    else
                    {
                        html += "<td><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"datetime-local\" class=\"form-control col-md-8 field-type-datetime\" /></td>";
                    }
                }
                else
                {
                    html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                    html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                    html += "<div class=\"col-md-8\"><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"datetime-local\" class=\"form-control col-md-8 field-type-datetime\" /></div>";
                    html += "</div>";
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.Monetary)
            {
                // done
                if (tablestarted)
                {
                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr>";
                    }

                    if (field.IsReadOnly)
                    {
                        html += "<td>" +
                        "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>" +
                        "</td>";
                    }
                    else
                    {
                        html += "<td><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"number\" step=\"0.01\" class=\"form-control col-md-8 field-type-monetary\" /></td>";
                    }
                }
                else
                {
                    html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                    html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                    html += "<div class=\"col-md-8\"><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"number\" step=\"0.01\" class=\"form-control col-md-8 field-type-monetary\" /></div>";
                    html += "</div>";
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.Float)
            {
                // done
                if (tablestarted)
                {
                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr>";
                    }

                    if (field.IsReadOnly)
                    {
                        html += "<td>" +
                        "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>" +
                        "</td>";
                    }
                    else
                    {
                        html += "<td><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"number\" step=\"0.01\" class=\"form-control col-md-8 field-type-float\" /></td>";
                    }
                }
                else
                {
                    html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                    html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                    html += "<div class=\"col-md-8\"><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"number\" step=\"0.01\" class=\"form-control col-md-8 field-type-float\" /></div>";
                    html += "</div>";
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.checkbox)
            {
                // done
                if (tablestarted)
                {
                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr>";
                    }

                    if (field.IsReadOnly)
                    {
                        html += "<td>" +
                        "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>" +
                        "</td>";
                    }
                    else
                    {
                        html += "<td>";

                        if (field.Lookups != null && field.Lookups.Count > 0)
                        {
                            foreach (var opt in field.Lookups)
                            {
                                if (field.IsMultipleSelection == true)
                                {
                                    html += "<label class=\"checkbox-inline field-type-checkbox\"><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"checkbox\" name=\"checbox-" + field.Id + "\" value=\"" + opt.Value + "\"  /> " + opt.Text + "</label>";
                                }
                                else
                                {
                                    html += "<label class=\"checkbox-inline field-type-checkbox\"><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"radio\" name=\"checbox-" + field.Id + "\" value=\"" + opt.Value + "\" /> " + opt.Text + "</label>";
                                }
                            }
                        }

                        html += "</td>";
                    }
                }
                else
                {
                    html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                    html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";

                    if (field.Lookups != null && field.Lookups.Count > 0)
                    {
                        foreach (var opt in field.Lookups)
                        {
                            if (field.IsMultipleSelection == true)
                            {
                                html += "<label class=\"checkbox-inline field-type-checkbox\"><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"checkbox\" name=\"checbox-" + field.Id + "\" value=\"" + opt.Value + "\"  /> " + opt.Text + "</label>";
                            }
                            else
                            {
                                html += "<label class=\"checkbox-inline field-type-checkbox\"><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"radio\" name=\"checbox-" + field.Id + "\" value=\"" + opt.Value + "\" /> " + opt.Text + "</label>";
                            }
                        }
                    }

                    html += "</div>";
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.Dropdown)
            {
                // done
                if (tablestarted)
                {
                    newIndex = index;
                    newIndexTable = ++tableFieldIndex;

                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr>";
                    }

                    if (field.IsReadOnly)
                    {
                        html += "<td>" +
                        "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>" +
                        "</td>";
                    }
                    else
                    {
                        html += "<td><select class=\"form-control field-type-dropdown\">";
                        if (field.Lookups != null && field.Lookups.Count > 0)
                        {
                            html += "<option value=\"\"></option>";

                            foreach (var opt in field.Lookups)
                            {
                                html += "<option value=\"" + opt.Value + "\">" + opt.Text + "</option>";
                            }
                        }

                        html += "</select></td>";
                    }
                }
                else
                {
                    newIndex = ++index;

                    html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                    html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                    html += "<div class=\"col-md-8\">";
                    html += "" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldId\" type=\"hidden\" value=\"" + field.Id + "\"/ >" +
                        "<input name=\"FieldData[" + (newIndex) + "].FieldKey\" type=\"hidden\" value=\"" + field.Key + "\"/ >";

                    html += "<select name=\"FieldData[" + (newIndex) + "].FieldValue\" class=\"form-control field-type-dropdown\">";
                    if (field.Lookups == null || field.Lookups.Count == 0)
                    {
                        if (field.Key.EndsWith("CollegeId"))
                        {
                            field.Lookups = Utils.db.Colleges
                                .ToList()
                                .OrderBy(f => f.LocalizedName())
                                .Select(f => new SelectListItem
                                {
                                    Value = f.Id + "",
                                    Text = f.LocalizedName()
                                })
                                .ToList();
                        }
                    }
                    if (field.Lookups != null && field.Lookups.Count > 0)
                    {
                        html += "<option value=\"\"></option>";

                        foreach (var opt in field.Lookups)
                        {
                            if (value != null && opt.Value == opt.Value)
                                html += "<option selected value=\"" + opt.Value + "\">" + opt.Text + "</option>";
                            else
                                html += "<option value=\"" + opt.Value + "\">" + opt.Text + "</option>";
                        }
                    }

                    html += "</select>";
                    html += "</div>";
                    html += "</div>";
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.Email)
            {
                // done
                if (tablestarted)
                {
                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr>";
                    }

                    if (field.IsReadOnly)
                    {
                        html += "<td>" +
                        "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>" +
                        "</td>";
                    }
                    else
                    {
                        html += "<td><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"email\" class=\"form-control field-type-email\" /></td>";
                    }
                }
                else
                {
                    html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                    html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                    html += "<div class=\"col-md-8\">";
                    html += "<input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"email\" class=\"form-control field-type-email\" />";
                    html += "</div>";
                    html += "</div>";
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.LongText)
            {
                // done
                if (tablestarted)
                {
                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr>";
                    }

                    if (field.IsReadOnly)
                    {
                        html += "<td>" +
                        "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>" +
                        "</td>";
                    }
                    else
                    {
                        html += "<td><textarea rows=\"6\" cols=\"5\" class=\"form-control field-type-longtext\"></textarea></td>";
                    }
                }
                else
                {
                    html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                    html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                    html += "<div class=\"col-md-8\">";
                    html += "<textarea rows=\"6\" cols=\"5\" class=\"form-control field-type-longtext\"></textarea>";
                    html += "</div>";
                    html += "</div>";
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.TableHeading)
            {
                html += "<h4 class=\"field-type-tableheading\">" + field.Name + "</h4>";
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.SubHeading)
            {
                html += "<h4 class=\"field-type-tableheading\">" + field.Name + "</h4>";
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.Phone)
            {
                // done
                if (tablestarted)
                {
                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr>";
                    }

                    if (field.IsReadOnly)
                    {
                        html += "<td>" +
                        "<div class=\"form-control field-type-" + field.FieldType.ToString().ToLower() + "\">" + (string.IsNullOrWhiteSpace(value) ? "" : value) + "</div>" +
                        "</td>";
                    }
                    else
                    {
                        html += "<td><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"text\" class=\"form-control field-type-phone\" /></td>";
                    }
                }
                else
                {
                    html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                    html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                    html += "<div class=\"col-md-8\">";
                    html += "<input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"text\" class=\"form-control field-type-phone\" />";
                    html += "</div>";
                    html += "</div>";
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.AttachedFile)
            {
                // done
                if (tablestarted)
                {
                    if (tableHeadingStarted)
                    {
                        tableHeadingStarted = false;
                        html += "</tr></thead>";
                        html += "<tbody><tr>";
                    }

                    html += "<td><input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"file\" class=\"form-control field-type-attachedfile\" /></td>";
                }
                else
                {
                    html += "<div class=\"form-group" + (field.IsRequired == true ? " required" : "") + "\">";
                    html += "<label class=\"control-label col-md-4\">" + field.Name + "</label>";
                    html += "<div class=\"col-md-8\">";
                    html += "<input name=\"FieldData[" + (newIndex) + "].FieldValue\" type=\"file\" class=\"form-control field-type-attachedfile\" />";
                    html += "</div>";
                    html += "</div>";
                }
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.Table || (FormFieldType)field.FieldType == FormFieldType.TabContainer)
            {
                tablestarted = true;
                tableFieldIndex = 0;

                newIndex = ++index;
                html += "" +
                    "<input name=\"FieldData[" + (newIndex) + "].FieldId\" type=\"hidden\" value=\"" + field.Id + "\"/ >" +
                    "<input name=\"FieldData[" + (newIndex) + "].FieldKey\" type=\"hidden\" value=\"" + field.Key + "\"/ >" +
                    "<div class='table-container'><table class='table'><thead><tr>";
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.ColumnHeading)
            {
                tableHeadingStarted = true;

                html += "<th>" + field.Name + "</th>";
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.TableEnd)
            {
                tablestarted = false;
                html += "</tr></tbody></table><a class=\"btn btn-success btn-xs btn-add-row\">Add</a></div>";
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.Paragraph)
            {
                html += "<p>" + field.LocalizedName() + "</p>";
            }
            else if ((FormFieldType)field.FieldType == FormFieldType.HTML)
            {
                html += field.LocalizedName();
            }

            return MvcHtmlString.Create(html);
        }

        public static Nullable<T> GetDataValue<T>(this Request request, int fieldId) where T : struct
        {
            if (!(fieldId > 0)) return default(T);
            return GetDataValue<T>(request, "FIELD_" + fieldId);
        }

        public static Nullable<T> GetDataValue<T>(this Request request, string key) where T : struct
        {
            if (request == null || !(request.Id > 0)) return default(T);
            if (string.IsNullOrWhiteSpace(key)) return default(T);

            var data = request
                .RequestDatas
                .Where(d => d.Name == key)
                .OrderByDescending(d => d.Created)
                .FirstOrDefault();

            if (data == null) return default(T);

            if (typeof(T) == typeof(Int16))
            {
                Int16 val = 0;
                if (Int16.TryParse(data.Value, out val))
                {
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                else
                {
                    return null;
                }
            }
            else if (typeof(T) == typeof(Int32))
            {
                Int32 val = 0;
                if (Int32.TryParse(data.Value, out val))
                {
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                else
                {
                    return null;
                }
            }
            else if (typeof(T) == typeof(Int64))
            {
                Int64 val = 0;
                if (Int64.TryParse(data.Value, out val))
                {
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                else
                {
                    return null;
                }
            }
            else if (typeof(T) == typeof(float))
            {
                float val = 0;
                if (float.TryParse(data.Value, out val))
                {
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                else
                {
                    return null;
                }
            }
            else if (typeof(T) == typeof(double))
            {
                double val = 0;
                if (double.TryParse(data.Value, out val))
                {
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                else
                {
                    return null;
                }
            }
            else if (typeof(T) == typeof(DateTime))
            {
                DateTime val = default(DateTime);
                if (DateTime.TryParseExact(data.Value, "yyyy-MM-dd hh:mm:ss tt", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out val))
                {
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(data.Value)) return null;
                return Utils.DeerializeObject<T>(data.Value);
            }
        }

        public static RequestData SetDataValue(this Request request, long fieldId, string value, bool saveToRequestData = true)
        {
            if (!(fieldId > 0)) return null;

            var data = SetDataValue(request, "FIELD_" + fieldId, value, saveToRequestData);
            data.FieldId = fieldId;
            return data;
        }

        public static RequestData SetDataValue(this Request request, string key, string value, bool saveToRequestData = true)
        {
            if (request == null || !(request.Id > 0)) return null;
            if (string.IsNullOrWhiteSpace(key) || value == null) return null;

            var data = request
                .RequestDatas
                .Where(d => d.Name == key)
                .OrderByDescending(d => d.Created)
                .FirstOrDefault();

            if (data == null)
            {
                data = new RequestData()
                {
                    Name = key,
                    Created = TM.Now,
                    CreatedBy = Utils.CurrentUser.Id,
                };

                if (saveToRequestData)
                {
                    request.RequestDatas.Add(data);
                }
            }

            data.Value = value;

            data.Updated = TM.Now;
            data.UpdatedBy = Utils.CurrentUser.Id;

            return data;
        }

        private static string RenderTable(DwfFieldEnumerator enumerator, Request request)
        {
            // Sanity checks!
            if (enumerator == null || enumerator.Current == null) return "";

            var field = enumerator.Current;
            if (field == null || !(field.Id > 0)) return "";
            if (field.FieldType != FormFieldType.Table && field.FieldType != FormFieldType.TabContainer) return "";
            if (request == null) request = new Request();
            if (request.RequestDatas == null || request.RequestDatas.Count == 0) request.RequestDatas = new List<RequestData> { };
            if (!(field.MinValue > 0)) field.MinValue = 0;
            if (!(field.MaxValue > 0)) field.MaxValue = int.MaxValue;
            if (field.MaxValue < field.MinValue) field.MaxValue = field.MaxValue;

            var data = request.RequestDatas.ToList();
            var builder = new StringBuilder(20000);
            var isTable = field.FieldType == FormFieldType.Table;
            var isDynamic = field.MinValue == field.MaxValue;

            var headingFields = new List<FormFieldModel>();
            var dataFields = new List<FormFieldModel>();
            var dataFieldValues = new List<string[]>();

            while (enumerator.MoveNext() && enumerator.Current.FieldType == FormFieldType.TableHeading)
            {
                headingFields.Add(enumerator.Current);
            }

            while (enumerator.MoveNext() && enumerator.Current.FieldType != FormFieldType.TableEnd)
            {
                dataFields.Add(enumerator.Current);
            }

            if (data.Count > 0)
            {
                foreach (var dfield in dataFields)
                {
                    var values = data.Where(x => x.Name.StartsWith("FIELD_" + dfield.Id + "_")).ToList();

                }
            }

            builder.Append("<div class='table-container'>");

            // heading
            if (isTable)
            {
                builder.Append("<table class='table'><thead><tr>");

                foreach (var hfield in headingFields)
                {
                    builder.Append("<th><h4 class=\"field-type-tableheading\">" + hfield.Name + "</h4></th>");
                }

                builder.Append("</tr>");
            }
            else
            {
                builder.Append("<ul class=\"nav nav-tabs nav-justified\">");

                builder.Append("</ul>");
            }

            // template row

            // ending
            if (isTable)
            {
                builder.Append("</tbody></table>");
            }
            else
            {
            }

            builder.Append("</div>");

            return builder.ToString();
        }

        public static IHtmlString RenderFormSection(this HtmlHelper html, FormFillModel section, Request request, bool enableValidation = true)
        {
            // Sanity checks!
            if (section == null || !(section.FormId > 0) || section.Fields == null || section.Fields.Count == 0) return new HtmlString("");
            if (request == null) request = new Request();
            if (request.RequestDatas == null || request.RequestDatas.Count == 0)
                request.RequestDatas = new List<RequestData> { };

            var builder = new StringBuilder(10000);
            var enumerator = new DwfFieldEnumerator(section.Fields);

            var data = request.RequestDatas.ToList();

            var tablestarted = false;
            var headingstarted = false;
            var index = -1;
            var tableFieldIndex = -1;

            while (enumerator.MoveNext())
            {
                var field = enumerator.Current;

                if (field.FieldType == FormFieldType.Table || field.FieldType == FormFieldType.TabContainer)
                {
                    var str = RenderTable(enumerator, request);
                    builder.Append(str);
                }
                else
                {
                    var rd = data.FirstOrDefault(f => f.Name == "FIELD_" + field.Id);
                    var str = RenderField(html, field, rd == null ? null : rd.Value, enableValidation, ref tablestarted, ref headingstarted, ref index, ref tableFieldIndex);

                    builder.Append(str);
                }
            }

            return new HtmlString(builder.ToString());
        }
    }
}