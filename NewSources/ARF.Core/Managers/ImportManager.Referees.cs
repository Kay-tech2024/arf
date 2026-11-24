using ARF.Core.Models;
using ARF.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ARF.Core.Managers
{
    public partial class ImportManager
    {
		// WIP
        //public static ImportResult ImportReferees(string path, string fileName, bool simulate, string sendResultsToEmail, string userId, out string errorMsg)
        //{
        //    var extDb = Utils.db;
        //    var currentUserId = string.IsNullOrWhiteSpace(userId) ? Utils.GetCurrentUserDetails(extDb).Id : userId;

        //    extDb.Configuration.AutoDetectChangesEnabled = true;
        //    extDb.Configuration.LazyLoadingEnabled = false;
        //    extDb.Configuration.ValidateOnSaveEnabled = false;

        //    errorMsg = (string)null;

        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(path))
        //        {
        //            errorMsg = "MISSING_FILE_PATH";
        //            return null;
        //        }

        //        if (string.IsNullOrWhiteSpace(fileName))
        //        {
        //            errorMsg = "MISSING_FILE_NAME";
        //            return null;
        //        }

        //        if (!File.Exists(path))
        //        {
        //            errorMsg = "FILE_DOES_NOT_EXIST";
        //            return null;
        //        }

        //        path = SaveImportedFile(path, fileName, ImportFormat.Referees);

        //        var regions = extDb.Regions.ToArray();
        //        var cities = extDb.Cities.ToArray();
        //        var educationLevels = extDb.EducationLevels.ToArray();
        //        var referralSources = extDb.ReferralSources.ToArray();

        //        var result = new ImportResult();

        //        var resultsPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "_results" + Path.GetExtension(path);

        //        File.Copy(path, resultsPath, true);

        //        if (extDb.ImportOperations.Count(o => o.OriginalFilePath == path && o.Imported == -1) > 0)
        //        {
        //            errorMsg = "ALREADY_PROCESSING";
        //            return null;
        //        }

        //        if (!simulate)
        //        {
        //            operation = new ImportOperation
        //            {
        //                Errors = -1,
        //                Imported = -1,
        //                MissingData = -1,
        //                OriginalFilePath = path,
        //                FileName = fileName,
        //                Hash = Utils.ComputeMD5Hash(path),
        //                Created = TimeMachine.TmUtils.CurrentDateTimeStatic,
        //                CreatedBy = currentUserId,
        //            };

        //            extDb.ImportOperations.Add(operation);

        //            extDb.SaveChanges();
        //        }

        //        ExcelPackage packageResults = new ExcelPackage(new FileInfo(resultsPath));

        //        GetWorksheets(packageResults);

        //        var data = new List<OsrModel>();
        //        var batchId = Path.GetFileName(fileName);
        //        using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
        //        {
        //            bool isConformant = false;
        //            var sheetIndex = 0;
        //            foreach (var ws in GetWorksheets(package))
        //            {
        //                sheetIndex++;

        //                var ws2 = packageResults == null ? null : packageResults.Workbook.Worksheets[sheetIndex];

        //                string[] columns = new string[] { "Source of Referral", "NIN", "JS_Full_Name", "MOBILE_NUMBER", "EMAIL", "Gender", "Age", "Education", "Location", "Work Experience", "Disability", "Eligibility Check", "Provider", "Date of Referral" };
        //                int[] columnIndices = new int[columns.Length];

        //                var headerRowIndex = 0;

        //                for (var i = 1; i <= 3; i++)
        //                {
        //                    var isFound = true;
        //                    for (var j = 1; j <= columns.Length; j++)
        //                    {
        //                        var isColFound = false;
        //                        for (var k = 1; k <= ws.Dimension.End.Column; k++)
        //                        {
        //                            if (ws.Cells[i, k].Value != null && ws.Cells[i, k].Value.ToString().Trim().ToLower().StartsWith(columns[j - 1].Trim().ToLower()))
        //                            {
        //                                columnIndices[j - 1] = k;
        //                                isColFound = true;
        //                                break;
        //                            }
        //                        }
        //                        if (!isColFound)
        //                        {
        //                            isFound = false;
        //                            break;
        //                        }
        //                    }

        //                    if (isFound)
        //                    {
        //                        headerRowIndex = i;
        //                        break;
        //                    }
        //                }

        //                if (headerRowIndex == 0)
        //                {
        //                    if (!isConformant)
        //                    {
        //                        errorMsg = "Uploaded file doesn't conform to the Out-of-System Referrals format";
        //                        return null;
        //                    }
        //                }
        //                else
        //                {
        //                    isConformant = true;

        //                    ws.Calculate();

        //                    var resultCol = ws.Dimension.End.Column + 1;

        //                    ws2.Cells[headerRowIndex, resultCol].Value = "Import result";
        //                    ws2.Cells[headerRowIndex, resultCol].Style.Border = ws2.Cells[headerRowIndex, resultCol - 1].Style.Border;
        //                    ws2.Cells[headerRowIndex, resultCol].Style.Font = ws2.Cells[headerRowIndex, resultCol - 1].Style.Font;
        //                    ws2.Cells[headerRowIndex, resultCol].Style.Fill = ws2.Cells[headerRowIndex, resultCol - 1].Style.Fill;

        //                    var maxRowIndex = ws.Dimension.End.Row;

        //                    for (var row = headerRowIndex + 1; row <= maxRowIndex; row++)
        //                    {
        //                        var refSourceCell = ws.Cells[row, columnIndices[0]].Value;
        //                        var ninCell = ws.Cells[row, columnIndices[1]].Value;
        //                        var fullnameCell = Utils.CleanObject(ws.Cells[row, columnIndices[2]].Value, true, false, true, true, false, true, true);
        //                        var mobileCell = ws.Cells[row, columnIndices[3]].Value;
        //                        var emailCell = ws.Cells[row, columnIndices[4]].Value;
        //                        var genderCell = ws.Cells[row, columnIndices[5]].Value;
        //                        var ageCell = ws.Cells[row, columnIndices[6]].Value;
        //                        var educationCell = Utils.CleanObjectNoPunctuation(ws.Cells[row, columnIndices[7]].Value);
        //                        var locationCell = Utils.CleanObjectNoPunctuation(ws.Cells[row, columnIndices[8]].Value);
        //                        var workExperienceCell = ws.Cells[row, columnIndices[9]].Value;
        //                        var disabilityCell = ws.Cells[row, columnIndices[10]].Value;
        //                        var eligiblityCheckRequiredCell = ws.Cells[row, columnIndices[11]].Value;
        //                        var providerCell = ws.Cells[row, columnIndices[12]].Value;

        //                        var referralDateCell = Utils.SheetCellToDate(ws.Cells[row, columnIndices[13]]);

        //                        if (ninCell == null || string.IsNullOrWhiteSpace(ninCell.ToString()))
        //                        {
        //                            continue;
        //                        }

        //                        data.Add(new OsrModel
        //                        {
        //                            FilePath = path,
        //                            refSourceCell = refSourceCell,
        //                            ninCell = ninCell,
        //                            fullnameCell = fullnameCell,
        //                            mobileCell = mobileCell,
        //                            emailCell = emailCell,
        //                            genderCell = genderCell,
        //                            ageCell = ageCell,
        //                            educationCell = educationCell,
        //                            locationCell = locationCell,
        //                            workExperienceCell = workExperienceCell,
        //                            disabilityCell = disabilityCell,
        //                            eligiblityCheckRequiredCell = eligiblityCheckRequiredCell,
        //                            providerCell = providerCell,
        //                            referralDateCell = referralDateCell,
        //                            simulate = simulate,
        //                            result = result,
        //                            ws = ws,
        //                            ws2 = ws2,
        //                            row = row,
        //                            resultCol = resultCol,
        //                        });
        //                    }
        //                }
        //            }
        //        }

        //        var threads = (Thread[])null;

        //        if (data.Count > 10)
        //        {
        //            threads = new Thread[10];
        //        }
        //        else
        //        {
        //            threads = new Thread[1];
        //        }

        //        var results = new ImportResult[threads.Length];
        //        var toDeletedJobseekersPerThread = new List<long>[threads.Length];
        //        var toDeletedJourneysPerThread = new List<long>[threads.Length];
        //        var perThread = (int)Math.Ceiling((double)(data.Count / (double)threads.Length));

        //        for (var i = 0; i < threads.Length; i++)
        //        {
        //            var subData = data.Skip(i * perThread).Take(perThread).ToList();
        //            var subResult = new ImportResult { Total = subData.Count };

        //            results[i] = subResult;
        //            toDeletedJobseekersPerThread[i] = new List<long>();
        //            toDeletedJourneysPerThread[i] = new List<long>();

        //            foreach (var d in subData)
        //            {
        //                d.result = subResult;
        //            }

        //            var local_i = i;
        //            threads[i] = new Thread(new ThreadStart(delegate
        //            {
        //                TmUtils.SetDictVal("UserId", userId);

        //                using (var db = new ArfDbEntities())
        //                {
        //                    db.Configuration.AutoDetectChangesEnabled = true;
        //                    db.Configuration.LazyLoadingEnabled = false;
        //                    db.Configuration.ValidateOnSaveEnabled = false;
        //                    db.Database.CommandTimeout = 120;

        //                    var providers = db.Providers.Where(p => !p.IsDeleted).ToArray();
        //                    var jpcs = db.JPCs.Where(j => !j.IsDeleted).ToArray();

        //                    var ci = 0;
        //                    foreach (var d in subData)
        //                    {
        //                        ci++;

        //                        if (ci % 1000 == 0)
        //                        {
        //                            db.DiscardChanges();
        //                        }

        //                        d.result = subResult;

        //                        var errMsg2 = "";
        //                        ImportSingleOsr(
        //                                options,
        //                                d,
        //                                jpcs,
        //                                cities,
        //                                referralSources,
        //                                educationLevels,
        //                                toDeletedJobseekersPerThread[local_i],
        //                                toDeletedJourneysPerThread[local_i],
        //                                batchId,
        //                                currentUserId,
        //                                db,
        //                                out errMsg2
        //                        );
        //                    }
        //                }

        //                lock (toBeRemovedJobseekers)
        //                {
        //                    toBeRemovedJobseekers.AddRange(toDeletedJobseekersPerThread[local_i]);
        //                }

        //                lock (toBeRemovedJourneys)
        //                {
        //                    toBeRemovedJourneys.AddRange(toDeletedJourneysPerThread[local_i]);
        //                }
        //            }));

        //            threads[i].Start();
        //        }

        //        foreach (var thread in threads)
        //        {
        //            thread.Join();
        //        }

        //        result.Total = data.Count;
        //        for (var i = 0; i < data.Count; i++)
        //        {
        //            var ws = data[i].ws;
        //            var ws2 = data[i].ws2;

        //            var row = data[i].row;
        //            var resultCol = data[i].resultCol;

        //            var tIndex = (int)Math.Floor((double)(i / (double)perThread));
        //            if (data[i].errorMsg != null)
        //            {
        //                result.ErrorsNins.SafeAddNin((ws.Name == "" ? "" : ws.Name + "-") + "Row_" + row + ": " + data[i].ninCell.ToString(), data[i].errorMsg, ws2 == null ? null : ws2.Cells[row, resultCol]);
        //            }
        //            else
        //            {
        //                var cell = ws2 == null ? null : ws2.Cells[row, resultCol];

        //                if (cell != null)
        //                {
        //                    cell.Value = "Imported successfully";
        //                }

        //                result.ImportedNins.Add(data[i].ninCell.ToString());
        //            }
        //        }

        //        packageResults.Save();

        //        if (!simulate)
        //        {
        //            operation.Errors = result.ErrorsNins.Count;
        //            operation.Imported = result.ImportedNins.Count;
        //            operation.ResultsFilePath = resultsPath;
        //            extDb.Entry(operation).State = EntityState.Modified;
        //            extDb.SaveChanges();

        //            if (toBeRemovedJourneys != null && toBeRemovedJourneys.Count > 0)
        //                extDb.JobSeekerJourneys.Where(j => toBeRemovedJourneys.Count(x => x == j.Id) > 0).Delete();

        //            if (toBeRemovedJobseekers != null && toBeRemovedJobseekers.Count > 0)
        //                extDb.JobSeekers.Where(j => toBeRemovedJobseekers.Count(x => x == j.Id) > 0).Delete();

        //            extDb.SaveChanges();
        //        }
        //        else
        //        {
        //            extDb.DiscardChanges();
        //        }

        //        if (!string.IsNullOrWhiteSpace(sendResultsToEmail))
        //        {
        //            EmailService.SendMessage(
        //                Utils.GetSmtpSettings(),
        //                "Results of OSR import on " + TimeMachine.TmUtils.CurrentDateTimeStatic.FormatDate(),
        //                "Dear,<br/>Please find the results file, attached.",
        //                new MailAddress[] { new MailAddress(sendResultsToEmail) },
        //                null,
        //                new MailAddress[] { new MailAddress("omar.shaaban@kay.sa") },
        //                new string[] { resultsPath }
        //            );
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogExceptionAsync(ex);

        //        return new ImportResult();
        //    }
        //}
    }
}