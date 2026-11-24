using ARF.Core.Models;
using ARF.Data;
using ARF.TimeMachine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARF.Core.Managers
{
    public partial class ImportManager
    {
        public static string SaveImportedFile(string path, string fileName, ImportFormat format)
        {
            var folder = @"C:\ARF-Data\Imported-" + format.ToString() + "\\";
            var newPath = folder + TM.Now.ToString("yyyy-MM-dd hh_mm_ss tt", CultureInfo.GetCultureInfo("en-US")) + Path.GetExtension(fileName);

            var i = 1;
            while (File.Exists(newPath))
            {
                var hash1 = Utils.ComputeMD5Hash(path);
                var hash2 = Utils.ComputeMD5Hash(newPath);

                if (hash1 == hash2) return newPath;

                i++;

                newPath = folder + TM.Now.ToString("yyyy-MM-dd hh_mm_ss tt_" + i, CultureInfo.GetCultureInfo("en-US")) + Path.GetExtension(fileName);

            }

            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                File.Copy(path, newPath, true);
                path = newPath;

                return newPath;
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsync(ex, newPath);

                return path;
            }
        }

        public GenericImportModel GenericImportJobseeker(Dictionary<string, string> parameters, ImportFormat format)
        {
            if (parameters == null || parameters.Count == 0) return null;

            var model = new GenericImportModel();



            foreach (var kv in parameters)
            {
                if (string.IsNullOrWhiteSpace(kv.Key)) continue;
                var key = Utils.NormalizeString(new string(kv.Key.Where(s => s != '-' && s != '.' && s != '\t' && s != ' ' && s != '_').ToArray()).ToUpper());

                switch (key)
                {
                    case "الاسم":
                    case "اسم الباحث":
                    case "اسم المحكم":
                    case "NAME":
                        {
                            if (string.IsNullOrWhiteSpace(kv.Value)) continue;
                            model.Name = kv.Value.Trim();

                            model.Name = model.Name.Replace("ـ", "").Replace("  ", " ");


                            var comps = Utils.GetNameComponents(model.Name);

                            if (comps != null)
                            {
                                if (comps.FirstName != null && model.FirstName == null) model.FirstName = comps.FirstName;
                                if (comps.SecondName != null && model.MiddleName == null) model.MiddleName = comps.SecondName;
                                if (comps.ThirdName != null && model.ThirdName == null) model.ThirdName = comps.ThirdName;
                                if (comps.LastName != null && model.LastName == null) model.LastName = comps.LastName;

                            }
                        }
                        break;

                    case "الاسم الاول":
                        {
                            if (string.IsNullOrWhiteSpace(kv.Value)) continue;
                            model.Name = kv.Value.Trim();

                            model.Name = model.Name.Replace("ـ", "").Replace("  ", " ");

                        }
                        break;
                    default:
                        break;
                }
            }

            return model;
        }


    }
}
