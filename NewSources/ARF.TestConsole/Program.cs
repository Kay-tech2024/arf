using ARF.Core;
using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TestConsole.DWS;
using ARF.TimeMachine;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
//using System.Web.Mvc;

namespace ARF.TestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SendMessageToViceDeans(@"c:\Users\omars_ilonsnw\Downloads\vice_deans.txt", false);
            return;            
            //(new RefereeUtil()).CreateRequestRevision(242, false);
            //CreateReferees(@"e:\OneDrive - Kay Technology\Temp\referees.txt");            
            //CleanupTranslations(@"c:\Users\Omar\Desktop\ar.txt", @"c:\Users\Omar\Desktop\ar2.txt");
            //CleanupTranslations(@"c:\Users\Omar\Desktop\en.txt", @"c:\Users\Omar\Desktop\en2.txt");
            //return;

            //ImportPriorities(@"c:\Users\Omar\Downloads\ARF Workflows.xlsx", @"c:\Users\Omar\Desktop\arf_priorities.sql");
            //return;
            //WriteViewsAndModels(@"C:\users\omar\desktop\ArfGen\");
            //return;
            ////WriteTerms("asdf");
            ////return;
            //WriteTranslations(@"c:\users\omars\Desktop\arf_translations.txt", @"c:\users\omars\Desktop\arf_translations2.txt");

            //return;

            //FormFuncs.SaveForms(@"C:\Users\Omars\desktop\APS & ARF Forms3.xlsx");

            //return;
            ////WriteTerms(@"C:\users\omars\desktop\terms.txt");

            ////return;
            //CreateReferees(@"C:\users\omars\desktop\referees.txt");
            //return;

            //InsertFakeReferees(1000);
            //return;
            //InsertFakeData();

            //GenerateProgram.GenerateMain();

            //GenerateStateTransitionMachine(@"C:\users\omars\desktop\state_transiition_machine.txt");

            //var roles = new ArfUserRole[]
            //{
            //};

        }

        public static void SendMessageToViceDeans(string inputFile, bool testMode = true)
        {
            var reader = new StreamReader(inputFile);
            var lines = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            reader.Close();

            var db = Utils.db;
            var emails = new List<string>();
            var names = new List<NameComponents>();
            var role = db.Roles.FirstOrDefault(r => r.Name == ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch.ToString());

            var countries = db.Countries.ToList();
            var colleges = db.Colleges.ToList().Select(x => new { Id = x.Id, Name = Utils.NormalizeString(x.NameArabic) }).ToList();

            var hasher = Utils.UserUtils.UserManager.PasswordHasher;

            var templateSubject = "الدعم الفني لنظام تمويل المشروعات البحثية";
            var templateBody = "<div style=\"direction: rtl;\"><p>سعادة الدكتور مدير مركز /رئيس وحدة البحوث</p><p>السلام عليكم ورحمة الله وبركاته</p><p>بداية نحب أن نرحب بكم في الدعم الفني لنظام تمويل المشروعات البحثية بعمادة البحث العلمي. ونحب أن نذكركم بأنه يمكنكم التواصل معنا في الدعم الفني لأي خلل أو مساعدة قد تحتاجونها في النظام من خلال الطرق التالية: </p><ol><li>الدعم الفني الفوري خلال وقت الدوام الرسمي من خلال المحاثة الفورية في الموقع</li><li>عن طريق إرسال رسالة عن طريق صفحة اتصل بنا https://arf.srg.edu.sa/About/ContactUs</li><li>عبر الإيميل الخاص بالدعم الفني (info@srg.edu.sa) </li></ol><p>والله يحفظكم </p>";


            var rand = new Random();

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var tabs = line.Split('\t');

                if (tabs.Length < 7) continue;

                var no = tabs[0];
                var name = tabs[1];
                var collegeName = tabs[2];
                var mobile = tabs[4];
                var email = tabs[5];

                if (string.IsNullOrWhiteSpace(email))
                    throw new Exception("Email missing");
                else
                {
                    email = email.ToLower().Trim();

                    if (!Utils.ValidateEmail(email))
                    {
                        throw new Exception("Invalid email: " + email);
                    }
                }

                if (string.IsNullOrWhiteSpace(mobile))
                    throw new Exception("Mobile number missing");
                else
                {
                    mobile = mobile.ToLower().Trim();

                    var validPhone = "";
                    if (!Utils.ValidateMobile(mobile, out validPhone))
                    {
                        throw new Exception("Invalid mobile: " + mobile);
                    }
                    else
                    {
                        mobile = validPhone;
                    }
                }

                var comps = Utils.GetNameComponents(name);

                if (comps == null || comps.FirstName == null || comps.LastName == null) continue;
                if (emails.Contains(email)) continue;

                emails.Add(email);
                names.Add(comps);


                var collegeId = (int?)null;

                if (!string.IsNullOrWhiteSpace(collegeName))
                {
                    var m = colleges.SingleOrDefault(x => Utils.NormalizeString(x.Name) == Utils.NormalizeString(collegeName) || Utils.NormalizeString("كلية" + x.Name) == Utils.NormalizeString(collegeName));
                    if (m != null)
                    {
                        collegeId = m.Id;
                    }
                    else
                    {

                        throw new Exception("College not found: " + collegeName);

                    }

                }
                else
                {
                    throw new Exception("College not found: " + collegeName);
                }


                var username = email;
                var password = Helpers.GeneratePassword(12);

                var fullName = comps.FirstName;

                if (comps.SecondName != null)
                    fullName += " " + comps.SecondName;

                if (comps.ThirdName != null)
                    fullName += " " + comps.ThirdName;

                if (comps.LastName != null)
                    fullName += " " + comps.LastName;


                if (comps.Title != null)
                {
                    fullName = comps.Title + " " + fullName;
                }


                if (testMode)
                {
                    mobile = "966549768845";
                    email = "ahmad@kay-ict.com";
                }


                var subject = templateSubject;
                var body = templateBody;


                body = body
                        .Replace("{name}", fullName)
                        .Replace("{email}", email)
                        .Replace("{password}", password);

                var isSent = EmailService.SendMessage(Utils.GetSmtpSettings(), subject, body, new MailAddress(email, fullName), null, new string[] { @"c:\Users\omars_ilonsnw\Downloads\ARF UM.pdf" });

                if (!isSent)
                {
                    throw new Exception("FAILED_TO_SEND_EMAIL");
                }


            }
        }

        //        public static void InsertFakeReferees(int max)
        //        {
        //            var db = Utils.db;

        //            var role = db.Roles.FirstOrDefault(r => r.Name == ArfUserRole.Referee.ToString());
        //            var rand = new Random();
        //            var majors = db.Majors.ToList();

        //            for (var i = 1; i <= max; i++)
        //            {
        //                char gender = ' ';
        //                var name = "";
        //                while (string.IsNullOrWhiteSpace(name) || gender != 'M')
        //                {
        //                    name = FakeGenerator.GenerateName(out gender);
        //                }

        //                var comps = Utils.GetNameComponents(name);

        //                var user = new User
        //                {
        //                    Id = Guid.NewGuid().ToString(),
        //                    Roles = new List<Role> { role },
        //                    UserName = "rf" + i,
        //                    Email = "user" + i + "@gmail.com",
        //                    SecurityStamp = "a14ee95e-c73e-466e-a786-aaf4378237d3",
        //                    PasswordHash = "AFDn7WXfSXvvQ9X29S0t/tqKGG8lS7To/E7zVj7mbaxOoBmbN2xE6JwwPQTz/Td8Fg==",
        //                    IsActivated = true,
        //                    Created = TM.Now,
        //                    CreatedBy = "00000000-0000-0000-0000-000000000000",
        //                    Updated = TM.Now,
        //                    UpdatedBy = "00000000-0000-0000-0000-000000000000",
        //                };

        //                db.Users.Add(user);
        //                db.SaveChanges();

        //                var userP = new UserProfile
        //                {
        //                    UserId = user.Id,
        //                    FirstName = comps.FirstName,
        //                    MiddleName = comps.SecondName,
        //                    ThirdName = comps.ThirdName,
        //                    LastName = comps.LastName,

        //                    Created = TM.Now,
        //                    CreatedBy = "00000000-0000-0000-0000-000000000000",
        //                    Updated = TM.Now,
        //                    UpdatedBy = "00000000-0000-0000-0000-000000000000",
        //                };

        //                db.UserProfiles.Add(userP);
        //                db.SaveChanges();

        //                var @ref = new Referee
        //                {
        //                    UserId = user.Id,
        //                    FirstName = comps.FirstName,
        //                    SecondName = comps.SecondName,
        //                    ThirdName = comps.ThirdName,
        //                    LastName = comps.LastName,
        //                    Email = user.Email,
        //                    MajorId = majors[rand.Next(0, majors.Count)].Id,
        //                    Created = TM.Now,
        //                    CreatedBy = "00000000-0000-0000-0000-000000000000",
        //                    Updated = TM.Now,
        //                    UpdatedBy = "00000000-0000-0000-0000-000000000000",
        //                };

        //                db.Referees.Add(@ref);
        //                db.SaveChanges();
        //            }
        //        }

        //        public static void InsertFakeData()
        //        {
        //            var db = Utils.db;

        //            db.Configuration.AutoDetectChangesEnabled = true;
        //            db.Configuration.LazyLoadingEnabled = true;

        //            var x = new List<dynamic>();
        //            x.Add(new { Title = "أحسن التقاسيم في معرفة الأقاليم (مختارات)", Author = "محمد بن أحمد المقدسي" });
        //            x.Add(new { Title = "أخبار المصحفين", Author = "الحسن بن عبد الله العسكري أبو أحمد" });
        //            x.Add(new { Title = "أخبار النحويين", Author = "عبد الواحد بن عمر بن محمد بن أبي هاشم" });
        //            x.Add(new { Title = "أخبار ملوك بني عبيد وسيرتهم", Author = "محمد بن علي بن حماد أبو عبد الله" });
        //            x.Add(new { Title = "أخبار مكة في قديم الدهر وحديثه", Author = "محمد بن إسحاق بن العباس الفاكهي أبو عبد الله" });
        //            x.Add(new { Title = "الآحاد والمثاني", Author = "أحمد بن عمرو بن الضحاك أبو بكر الشيباني" });
        //            x.Add(new { Title = "الألفاظ المختلفة في المعاني المؤتلفة", Author = " محمد بن عبد الملك بن مالك الطائي الجياني أبو عبد الله" });
        //            x.Add(new { Title = " الأوائل", Author = "أحمد بن عمرو بن أبي عاصم الشيباني " });
        //            x.Add(new { Title = "الإعجاز في المدائن القديمة ", Author = "محمد شكور بن محمود الحاجي أمرير" });
        //            x.Add(new { Title = "الإرشاد في معرفة علماء الحديث", Author = "الخليل بن عبد الله بن أحمد الخليلي القزويني أبو يعلى" });
        //            x.Add(new { Title = "الإصابة في تمييز الصحابة", Author = "أحمد بن علي بن حجر أبو الفضل العسقلاني الشافعي" });
        //            x.Add(new { Title = "البدء والتاريخ", Author = " مطهر بن طاهر المقدسي" });

        //            var role = db.Roles.FirstOrDefault(r => r.Name == ArfUserRole.PrincipalResearcher.ToString());

        //            var i = 0;
        //            foreach (var a in x)
        //            {
        //                i++;
        //                var user = new User
        //                {
        //                    Id = Guid.NewGuid().ToString(),
        //                    Roles = new List<Role> { role },
        //                    UserName = "r" + i,
        //                    Email = "user" + i + "@gmail.com",
        //                    SecurityStamp = "a14ee95e-c73e-466e-a786-aaf4378237d3",
        //                    PasswordHash = "AFDn7WXfSXvvQ9X29S0t/tqKGG8lS7To/E7zVj7mbaxOoBmbN2xE6JwwPQTz/Td8Fg==",
        //                    IsActivated = true,
        //                    Created = TM.Now,
        //                    CreatedBy = "91698DCE-BAA8-469E-906D-EF55D528321B",
        //                    Updated = TM.Now,
        //                    UpdatedBy = "91698DCE-BAA8-469E-906D-EF55D528321B",
        //                };

        //                db.Users.Add(user);
        //                db.SaveChanges();
        //                var comps = Utils.GetNameComponents((string)a.Author);

        //                user.Researchers.Add(new Data.DbModels.Researcher()
        //                {
        //                    FirstName = comps.FirstName,
        //                    SecondName = comps.SecondName,
        //                    ThirdName = comps.ThirdName,
        //                    LastName = comps.LastName,
        //                    Email = "abdulrahman@gmail.com",
        //                    MobileNo = "0559072407",
        //                    Address = "الرياض- حي التعاون",
        //                    //Bank = new Data.DbModels.Bank
        //                    //{
        //                    //    NameArabic = "بنك الراجحي"
        //                    //},
        //                    //City = new Data.DbModels.City
        //                    //{
        //                    //    NameArabic = "الرياض",
        //                    //    Country = new Data.DbModels.Country
        //                    //    {
        //                    //        NameArabic = "المملكة العربية السعودية",
        //                    //    },
        //                    //},
        //                    College = new Data.DbModels.College
        //                    {
        //                        NameArabic = "كلية التربية",

        //                        Created = TM.Now,
        //                        CreatedBy = user.Id,
        //                        Updated = TM.Now,
        //                        UpdatedBy = user.Id,
        //                    },

        //                    DateOfBirth = new DateTime(1985, 1, 1),

        //                    Created = TM.Now,
        //                    CreatedBy = user.Id,
        //                    Updated = TM.Now,
        //                    UpdatedBy = user.Id,
        //                });

        //                db.SaveChanges();

        //                db.Requests.Add(new Data.DbModels.Request
        //                {
        //                    UniqueId = "1",
        //                    Type = (byte)Data.RequestType.FundingRequest,
        //                    PublicationType = (byte)PublicationType.Book,
        //                    Status = (byte)RequestStatus.FundingRequestContractAcceptedByResearcher,

        //                    Created = TM.Now,
        //                    CreatedBy = user.Id,
        //                    Updated = TM.Now,
        //                    UpdatedBy = user.Id,

        //                    RequestHistoryLogs = new List<RequestHistoryLog>
        //                {
        //                    new RequestHistoryLog()
        //                    {
        //                        Action = (byte) Data.RequestAction.Submit,
        //                        IpAddress= "192.168.1.1",
        //                        CurrentlyWith =  "91698DCE-BAA8-469E-906D-EF55D528321B",
        //                        Comments = "لا يوجد ملاحظات",
        //                        Created = TM.Now,
        //                        CreatedBy = user.Id,
        //                    }
        //                }
        //                });

        //                db.SaveChanges();
        //            }
        //        }

        //        public static void GenerateStateTransitionMachine(string outputFilePath)
        //        {
        //            var writer = new StreamWriter(outputFilePath, false, Encoding.UTF8);

        //            var roles = Enum.GetValues(typeof(ArfUserRole));
        //            var actions = Enum.GetValues(typeof(Data.RequestAction));
        //            var states = Enum.GetValues(typeof(RequestStatus));

        //            writer.WriteLine("{0}\t{1}\t{2}", "State", "Role", "Action");

        //            foreach (var s in states)
        //            {
        //                foreach (var a in actions)
        //                {
        //                    foreach (var r in roles)
        //                    {
        //                        writer.WriteLine("{0}\t{1}\t{2}", s.ToString(), r.ToString(), a.ToString());
        //                    }
        //                }
        //            }

        //            writer.Close();
        //        }

        //        public static void CreateReferees(string inputFile)
        //        {
        //            var reader = new StreamReader(inputFile);
        //            var lines = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        //            reader.Close();

        //            var db = Utils.db;
        //            var emails = new List<string>();
        //            var names = new List<NameComponents>();
        //            var role = db.Roles.FirstOrDefault(r => r.Name == ArfUserRole.Referee.ToString());

        //            var countries = db.Countries.ToList();
        //            var referees = db.Referees.ToList();
        //            var majors = db.Majors.ToList().Select(x => new { Id = x.Id, Name = Utils.NormalizeString(x.NameArabic) }).ToList();

        //            var lastRefId = db.Users.FirstOrDefault(r => r.UserName.StartsWith("REF_"));
        //            var userIdStart = lastRefId == null ? 1 : int.Parse(lastRefId.UserName.Split('_')[1]) + 1;

        //            for (var i = 0; i < lines.Length; i++)
        //            {
        //                var line = lines[i];
        //                var tabs = line.Split('\t');

        //                if (tabs.Length < 10) continue;

        //                var no = tabs[0];
        //                var name = tabs[1];
        //                var nationality = tabs[2];
        //                var universityOther = tabs[3];
        //                var qualification = tabs[4];
        //                var major = tabs[5];
        //                var mobile = tabs[6];
        //                var email = tabs[7];
        //                var iban = tabs[8];
        //                var comments = tabs[9];

        //                if (string.IsNullOrWhiteSpace(email))
        //                    email = null;
        //                else
        //                {
        //                    email = email.ToLower().Trim();

        //                    if (!Utils.ValidateEmail(email))
        //                    {
        //                        email = null;
        //                    }
        //                }

        //                var comps = Utils.GetNameComponents(name);

        //                if (comps == null || comps.FirstName == null || comps.LastName == null) continue;
        //                if (emails.Contains(email)) continue;

        //                if (referees.Count(r => r.FirstName == comps.FirstName && r.LastName == comps.LastName && (email == null || email == r.Email)) > 0) continue;
        //                if (names.Count(n => n.FirstName == comps.FirstName && n.LastName == comps.LastName && n.ThirdName == comps.ThirdName && n.SecondName == comps.SecondName) > 0) continue;

        //                emails.Add(email);
        //                names.Add(comps);


        //                var majorId = (int?)null;

        //                if (!string.IsNullOrWhiteSpace(major))
        //                {
        //                    var m = majors.FirstOrDefault(x => x.Name == Utils.NormalizeString(major));
        //                    if (m != null)
        //                    {
        //                        majorId = m.Id;
        //                    }

        //                }



        //                if (string.IsNullOrWhiteSpace(universityOther))
        //                {
        //                    universityOther = null;
        //                }
        //                else
        //                {
        //                    universityOther = universityOther.Trim();
        //                }

        //                if (string.IsNullOrWhiteSpace(major))
        //                {
        //                    major = null;
        //                }
        //                else
        //                {
        //                    major = major.Trim();
        //                }


        //                var qualificationType = (QualificationType?)null;
        //                if (string.IsNullOrWhiteSpace(qualification))
        //                {
        //                    qualification = null;
        //                }
        //                else
        //                {

        //                    qualification = qualification.Trim();

        //                    if (qualification.StartsWith("أستاذ مشارك") || qualification.StartsWith("استاذ مشارك"))
        //                    {
        //                        qualificationType = QualificationType.AssociateProfessor;
        //                    }
        //                    else if (qualification.StartsWith("أستاذ مساعد") || qualification.StartsWith("استاذ مساعد"))
        //                    {
        //                        qualificationType = QualificationType.AssistantProfessor;
        //                    }
        //                    else if (qualification.StartsWith("أستاذ") || qualification.StartsWith("استاذ"))
        //                    {
        //                        qualificationType = QualificationType.Professor;
        //                    }
        //                }


        //                if (string.IsNullOrWhiteSpace(iban))
        //                {
        //                    iban = null;
        //                }
        //                else
        //                {
        //                    iban = iban.Trim().ToUpper();

        //                    if (!Utils.ValidateIBAN(iban))
        //                    {
        //                        iban = null;
        //                    }
        //                }


        //                var nationalityId = (int?)null;
        //                if (string.IsNullOrWhiteSpace(nationality))
        //                {
        //                    nationality = null;
        //                }
        //                else
        //                {
        //                    nationality = nationality.Trim().ToUpper();

        //                    nationalityId = countries.Where(c => c.NameArabic == nationality || c.NameEnglish == nationality).Select(c => c.Id).FirstOrDefault();

        //                    if (nationalityId == 0)
        //                        nationalityId = null;
        //                }


        //                var username = "REF_" + (userIdStart++) + "";
        //                var password = Helpers.GeneratePassword(12);

        //                var validPhone = (string)null;

        //                if (!string.IsNullOrWhiteSpace(mobile))
        //                {

        //                    if (!Utils.ValidateMobile(mobile, out validPhone))
        //                    {
        //                        var tabs2 = mobile.Split('/', '-');
        //                        foreach (var t in tabs2)
        //                        {
        //                            if (Utils.ValidateMobile(t, out validPhone))
        //                            {
        //                                mobile = validPhone;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }

        //                var fullName = comps.FirstName;

        //                if (comps.SecondName != null)
        //                    fullName += " " + comps.SecondName;

        //                if (comps.ThirdName != null)
        //                    fullName += " " + comps.ThirdName;

        //                if (comps.LastName != null)
        //                    fullName += " " + comps.LastName;

        //                var user = new User
        //                {
        //                    Id = Guid.NewGuid().ToString(),
        //                    Roles = new List<Role> { role },
        //                    UserName = username,
        //                    Email = email,
        //                    PhoneNumber = validPhone,
        //                    MajorId = majorId,
        //                    MajorOther = majorId > 0 ? null : major,
        //                    UniversityOther = universityOther,
        //                    FullName = fullName,
        //                    SecurityStamp = "a14ee95e-c73e-466e-a786-aaf4378237d3",
        //                    PasswordHash = "AFDn7WXfSXvvQ9X29S0t/tqKGG8lS7To/E7zVj7mbaxOoBmbN2xE6JwwPQTz/Td8Fg==",
        //                    IsActivated = true,
        //                    Created = TM.Now,
        //                    CreatedBy = "00000000-0000-0000-0000-000000000000",
        //                    Updated = TM.Now,
        //                    UpdatedBy = "00000000-0000-0000-0000-000000000000",
        //                };

        //                db.Users.Add(user);
        //                db.SaveChanges();

        //                var userP = new UserProfile
        //                {
        //                    UserId = user.Id,
        //                    FirstName = comps.FirstName,
        //                    MiddleName = comps.SecondName,
        //                    ThirdName = comps.ThirdName,
        //                    LastName = comps.LastName,
        //                    MobileNo = validPhone,
        //                    MobileNumber = validPhone,
        //                    BankIban = iban,
        //                    Created = TM.Now,
        //                    CreatedBy = "00000000-0000-0000-0000-000000000000",
        //                    Updated = TM.Now,
        //                    UpdatedBy = "00000000-0000-0000-0000-000000000000",
        //                };

        //                db.UserProfiles.Add(userP);
        //                db.SaveChanges();

        //                var @ref = new Referee
        //                {
        //                    UserId = user.Id,
        //                    FirstName = comps.FirstName,
        //                    SecondName = comps.SecondName,
        //                    ThirdName = comps.ThirdName,
        //                    LastName = comps.LastName,
        //                    Email = user.Email,
        //                    MobileNo = validPhone,
        //                    MajorId = majorId,
        //                    NationalityId = nationalityId,
        //                    Qualification = (byte?)qualificationType,
        //                    Created = TM.Now,
        //                    CreatedBy = "00000000-0000-0000-0000-000000000000",
        //                    Updated = TM.Now,
        //                    UpdatedBy = "00000000-0000-0000-0000-000000000000",
        //                };

        //                db.Referees.Add(@ref);
        //                db.SaveChanges();
        //            }
        //        }

        //        public static void WriteTerms(string outputFilePath)
        //        {
        //            var dict = new List<string>()
        //            {
        //               "ActionSubmit	إرسال",
        //"ActionApprove	موافقة",
        //"ActionReject	رفض",
        //"ActionApproveWithComments	موافقة مع ملاحظات ",
        //"ActionForward	إعادة توجيه ",
        //"ActionResubmit	إعادة إرسال",
        //"ActionSubmitCoresearcherData	إرسال بيانات الباحث",
        //"ActionCancel	إلغاء",
        //"ActionReturnToResearcher	إعادة للباحث",
        //"ActionForwardToInitialReferee	إرسال للمحكم المبدئي",
        //"ActionPublish	نشر",
        //"ActionReturnToResearchUnitHead	إعادة إلى رئيس وحدة البحوث",
        //"ActionReturnToViceDeanOfAcademicResearch	إعادة إلى وكيل العمادة للنشر العلمي  ",
        //"ActionSelectReferees	اختيار محكمين",
        //"ActionForwardToPressOficer	إرسال إلى موظف مطابع الجامعة",
        //"ActionForwardToProofreader	إرسال إلى المصحح",
        //"ActionForwardToResearchUnitSecretary	إعادة توجيه إلى أمين وحدة البحوث ",
        //"ActionForwardToDeanOfAcademicResearch	إرسال إلى عميد البحث العلمي ",
        //"ActionForwardToSpareReferee	إرسال إلى المحكم الإحتياطي",
        //"ActionApproveWithNoReferees	موافقة من غير محكمين ",
        //"ActionForwardToResearchUnitHead	إرسال إلى رئيس وحدة البحوث ",
        //"ActionApproveAndForwardToResearchUnitHead	موافقة وإرسال إلى رئيس وحدة البحوث",
        //"ActionAccept	موافق ",
        //"ActionSendToCommiteeMembers	إرسال إلى أعضاء اللجنة ",
        //"ActionApproveAndForwardToDeanOfAcademicResearch	موافقة وإرسال إلى عميد البحث العلمي",
        //"ActionRejectAndForwardToDeanOfAcademicResearch	رفض وإرسال إلى عميد البحث العلمي ",
        //"ActionForwardToDeanshipOfAcademicResearchCommiteeMembers	إرسال إلى أعضاء لجنة  عمادة البحث العلمي ",
        //"ActionApproveAndForwardToUnversityRector	موافقة وإعادة توجيه لمدير الجامعة ",
        //"ActionRejectAndForwardToUniversityRector	رفض وإعادة توجيه إلى مدير الجامعة ",
        //"ActionReturnToResearchUnitSecretary	إعادة إلى أمين وحدة البحوث",
        //"ActionReturnToDeanshipOfAcademicResearch	إعادة إلى عميد البحث العلمي ",
        //"ActionApproveApproval	اعتماد الموافقة ",
        //"ActionForwardToPAECommitteeHead	إرسال إلى رئيس لجنة النشر والتحكيم والمعادلات ",
        //"ActionForwardToAcademicCouncilHead	إرسال إلى رئيس المجلس العلمي ",
        //"ActionApproveAndReturnToPAECommitee	موافقة وإعادة للجنة النشر والتحكيم والمعادلات",
        //"ActionRejectAndReturnToPAECommitee	رفض وإعادة إلى أعضاء لجنة النشر والتحكيم و المعادلات ",
        //"ActionForwardToMainReferees	إرسال إلى المحكمان  الأساسيان ",
        //"ActionReturnToAcademdicCouncil	إعادة إلى المجلس العلمي ",
        //"ActionReturnToPAECommittee	إعادة إلى لجنة النشر والتحكيم والمعادلات",
        //"ActionApproveRejection	اعتماد الرفض ",
        //"ActionResendToReferees	إرسال إلى المحكمين",
        //"ActionForwardToAcademicCouncil	إرسال إلى المجلس العلمي ",
        //"ActionApproveAndForwardToViceDeanOfAcademicResearch	موافقة وإرسال إلى وكيل عمادة للبحث العلمي ",
        //"ActionRejectAndForwardToViceDeanOfAcademicResearch	رفض وإرسال وكيل عمادة البحث العلمي",
        //"Action		",
        //"ActionApproveAndForwardToAcademicCouncil	موافقة وإرسال إلى المجلس العلمي ",
        //"ActionRejectAndForwardToAcademicCouncil	رفض وإرسال إلى المجلس العلمي ",
        //"ActionApproveWithCommentsAndForwardToAcademicCouncil	موافقة مع ملاحظات وإرسال إلى المجلس العلمي ",
        //"ActionProvideFeedback	إبداء الرأي",
        //"ActionReturnToPAECommitteeSecretary	إرجاع إلى أمين لجنة النشر والتحكيم والمعادلات ",
        //"ActionForwardToReferees	إرسال للمحكمين ",
        //"ActionApproveModifications	الموافقة على التعديلات ",
        //"ActionRejectModifications	رفض التعديلات ",
        //"ActionApproveRejectionAndForwardToUniversityRector	موافقة وإعادة توجيه لمدير الجامعة  ",
        //"ActionApproveApprovalAndForwardToUniversityRector	اعتماد الموافقة وإرسال إلى مدير الجامعة ",
        //"ActionRejectAndReturnToResearcher	رفض وإرجاع للباحث ",
        //"ActionResubmitAndReturnToCoresearchers	إعادة إرسال للمشاركين في البحث",
        //"ActionReturnToAcademdicCouncilSecretary	إعادة إلى أمين المجلس العلمي ",
        //"ActionResubmitAndForwardToPAECommitee	إعادة إرسال وإرسال إلى لجنة النشر النشر والتحكيم والمعادلات ",
        //"ActionForwardToViceDeanOfAcademicResearch	إرسال إلى وكيل عمادة البحث العلمي ",
        //"ActionReturnToOfficerOfPrintingUnit	إر جاع إلى موظف وحدة التجهيز الطباعي ",
        //"ActionApproveAndForwardToUniversityPressDirector	موافقة وإرسال إلى مدير مطابع الجامعة ",
        //"ActionRejectAndReturnToOfficerOfPrintingUnit	رفض وإرجاع لموظف وحدة التجهيز الطباعي ",
        //"ActionCertifyAndForwardToUniversityPressOfficer	تصديق وإعادة إرسال إلى موظف مطابع الجامعة ",
        //"ActionOperationCommandExecutedAndForwardToPUAP	تنفيذ أمر التشغيل وإرسال إلى أمين لجنة التجهيز الطباعي ",
        //"ActionReturnToUniversityPressDirector	إرجاع لموظف مطابع الجامعة ",
        //"ActionReturnToUniversityPressOfficer	إعادة إلى موظف مطابع الجامعة ",
        //"ActionForwardToResearcher	إرسال إلى الباحث ",
        //"ActionReturnToProofreader	إعادة للمصحح ",
        //"ActionSubmitOperationCommandToViceDeanOfAcademicResearch	إرسال طلب أمر التشغيل إلى وكيل عمادة البحث العلمي ",
        //"ActionReturnToPUAP	إعادة إلى أمين لجنة التجهيز الطباعي ",
        //"ActionCloseRequest	إقفال الطلب",
        //"Action		",
        //"ActionSubmitISBNRequestToKFNL	رفع طلب ردمك إلى مكتبة الملك فهد الوطنية ",
        //"ActionSubmitISBNToImamUniversity	إرسال رقم الردمك إلى جامعة الإمام محمد بن سعود الإسلامية ",
        //"ActionReturnToKFNLOfficer	إرجاع إلى موظف مكتبة الملك فهد ",
        //"ActionRejectAndReturnToPUAP	إعادة إلى أمين لجنة التجهيز الطباعي ",
        //"ActionRejectAndReturnToViceDeanOfAcademicResearch	رفض وإعادة إلى وكيل عمادة البحث العلمي ",
        //"ActionSubmitToPressOfficer	إرسال إلى موظف المطابع ",
        //"ActionPrintingCompleted	تمت الطباعة ",
        //"ActionApproveAndForwardToFiancialDepartmentDirector	موافقة وإرسال إلى مدير الإدارة المالية ",
        //"ActionApproveAndForwardToAccountant	موافقة وإرسال إلى المحاسب ",
        //"ActionRejectAndReturnToDeanOfAcademicResearch	رفض وإرجاع إلى عميد البحث العلمي ",
        //"ActionPaymentCompleted	صرف المكافأة ",
        //"ActionPrepareDistributionList	تجهيز قوائم التوزيع",
        //"ActionConfirmReceivalOfCopies	تأكيد إستلام النسخ ",
        //"ActionConfirmDeliveringCopiesToResaercher	تاكيد إستلام الباحث للنسخ ",
        //"ActionForwardToKFNL	إرسال إلى موظف مكتبة الملك فهد ",
        //"ActionConfirmDeliveringCopiesToKFNL	تأكيد إستلام مكتبة الملك فهد النسخ ",
        //"ActionConfirmDeliveringCopiesToAgencies	تأكيد إستلام نسخ الجهات ",
        //"ActionForwardToUniversityPressDirector	إرسال إلى مدير مطابع الجامعة ",
        //"ActionApproveAndForwardToDeanshipOfLibraryAffairs	موافقة وإرسال إلى عميد شؤون المكتبات ",
        //"ActionApproveAndAndForardToDeanOfAcademicResearch	موافقة وإرسال إلى عميد البحث العلمي ",
        //"ActionReturnToDeanshipOfLibraryAffairs	إعادة إلى عميد شؤون المكتبات ",
        //"ActionCanceled	إلغاء",
        //"HistorySubmit	تم الإرسال ",
        //"HistoryApprove	تمت الموافقة",
        //"HistoryReject	رُفضت",
        //"HistoryApproveWithComments	تمت الموافقة مع تعديلات",
        //"HistoryForward	تم الإرسال ",
        //"HistoryResubmit	تمت إعادة الإرسال",
        //"HistorySubmitCoresearcherData	طلب النشر بانتظار ",
        //"HistoryCancel	تم الإلغاء",
        //"HistoryReturnToResearcher	إعادة للباحث",
        //"HistoryForwardToInitialReferee	إرسال إلى المحكم المبدئي",
        //"HistoryPublish	نشر",
        //"HistoryReturnToResearchUnitHead	إعادة إلى رئيس وحدة البحوث",
        //"HistoryReturnToViceDeanOfAcademicResearch	إعادة إلى وكيل العمادة للنشر العلمي",
        //"HistorySelectReferees	اختيار محكمين",
        //"HistoryForwardToPressOficer	تم الإرسال إلى موظف مطابع الجامعة",
        //"HistoryForwardToProofreader	تم الإرسال إلى المصحح",
        //"HistoryForwardToResearchUnitSecretary	تم الإرسال إلى أمين وحدة البحوث",
        //"HistoryForwardToDeanOfAcademicResearch	تم الإرسال إلى عميد البحث العلمي",
        //"HistoryForwardToSpareReferee	تم الإرسال إلى المحكم الاحتياطي",
        //"HistoryApproveWithNoReferees	تمت الموافقة بدون تحكيم",
        //"HistoryForwardToResearchUnitHead	تم الإرسال إلى رئيس وحدة البحوث",
        //"HistoryApproveAndForwardToResearchUnitHead	تمت الموافقة والإرسال إلى رئيس وحدة البحوث",
        //"HistoryAccept	تمت الموافقة",
        //"HistorySendToCommiteeMembers	تم الإرسال إلى أعضاء اللجنة",
        //"HistoryApproveAndForwardToDeanOfAcademicResearch	تمت الموافقة والإرسال إلى عميد  البحث العلمي",
        //"HistoryRejectAndForwardToDeanOfAcademicResearch	تم الرفض والإرسال إلى عميد البحث العلمي",
        //"HistoryForwardToDeanshipOfAcademicResearchCommiteeMembers	تمت الإرسال إلى مجلس عمادة البحث العلمي",
        //"HistoryApproveAndForwardToUnversityRector	تمت الموافقة والإرسال إلى مدير الجامعة",
        //"HistoryRejectAndForwardToUniversityRector	تم الرفض والإرسال إلى مدير الجامعة",
        //"HistoryReturnToResearchUnitSecretary	تمت الإعادة إلى أمين وحدة البحوث",
        //"HistoryReturnToDeanshipOfAcademicResearch	تمت الإعادة إلى عميد البحث العلمي",
        //"HistoryApproveApproval	تم الاعتماد",
        //"HistoryForwardToPAECommitteeHead	تم الإرسال إلى رئيس لجنة النشر والتحكيم والمعادلات",
        //"HistoryForwardToAcademicCouncilHead	تم الإرسال إلى رئيس المجلس العلمي",
        //"HistoryApproveAndReturnToPAECommitee	تمت الموافقة والإعادة إلى لجنة النشر والتحكيم والمعادلات",
        //"HistoryRejectAndReturnToPAECommitee	تم الرفض والإعادة إلى لجنة النشر والتحكيم والمعادلات",
        //"HistoryForwardToMainReferees	تم الإرسال إلى المحكمين الأساسيين",
        //"HistoryReturnToAcademdicCouncil	تم الإعادة إلى المجلس العلمي",
        //"HistoryReturnToPAECommittee	تم الإعادة إلى لجنة النشر والتحكيم والمعادلات",
        //"HistoryApproveRejection	تم اعتماد الرفض",
        //"HistoryResendToReferees	تم إعادة الإرسال للمحكمين",
        //"HistoryForwardToAcademicCouncil	تم الإرسال إلى المجلس العلمي",
        //"HistoryApproveAndForwardToViceDeanOfAcademicResearch	تمت الموافقة والإرسال إلى وكيل العمادة للنشر العلمي",
        //"HistoryRejectAndForwardToViceDeanOfAcademicResearch	تم الرفض والإرسال إلى وكيل العمادة للنشر العلمي",
        //"HistoryApproveAndForwardToAcademicCouncil	تمت الموافقة والإرسال إلى المجلس العلمي",
        //"HistoryRejectAndForwardToAcademicCouncil	تم الرفض والإرسال إلى المجلس العلمي",
        //"HistoryApproveWithCommentsAndForwardToAcademicCouncil	تمت الموافقة مع تعديلات، والإرسال إلى المجلس العلمي",
        //"HistoryProvideFeedback	تم إبداء الرأي",
        //"HistoryReturnToPAECommitteeSecretary	تم الإعادة إلى أمين لجنة النشر والتحكيم والمعادلات",
        //"HistoryForwardToReferees	تم الإرسال إلى المحكمين ",
        //"HistoryApproveModifications	تمت الموافقة على تعديلات الباحث",
        //"HistoryRejectModifications	تم رفض تعديلات الباحث",
        //"HistoryApproveRejectionAndForwardToUniversityRector	تم اعتماد الرفض والإرسال إلى مدير الجامعة",
        //"HistoryApproveApprovalAndForwardToUniversityRector	تم اعتماد الموافقة والإرسال إلى مدير الجامعة",
        //"HistoryRejectAndReturnToResearcher	تم الرفض والإعادة إلى الباحث",
        //"HistoryResubmitAndReturnToCoresearchers	تم إعادة الإرسال للمشاركين في المادة العلمية",
        //"HistoryReturnToAcademdicCouncilSecretary	تم الإعادة إلى أمين المجلس العلمي",
        //"HistoryResubmitAndForwardToPAECommitee	تم إعادة الإرسال إلى لجنة النشر والتحكيم والمعادلات",
        //"HistoryForwardToViceDeanOfAcademicResearch	تم الإرسال إلى وكيل العمادة للنشر العلمي",
        //"HistoryReturnToOfficerOfPrintingUnit	تم الإعادة إلى موظف وحدة التجهيز الطباعي",
        //"HistoryApproveAndForwardToUniversityPressDirector	تمت الموافقة والإرسال إلى مدير مطابع الجامعة",
        //"HistoryRejectAndReturnToOfficerOfPrintingUnit	تم الرفض والإعادة إلى موظف وحدة التجهيز الطباعي",
        //"HistoryCertifyAndForwardToUniversityPressOfficer	تمت المصادقة، والإرسال إلى موظف مطابع الجامعة",
        //"HistoryOperationCommandExecutedAndForwardToPUAP	تم تنفيذ أمر التشغيل، والإرسال إلى موظف وحدة التجهيز الطباعي",
        //"HistoryReturnToUniversityPressDirector	تم الإعادة إلى مدير مطابع الجامعة",
        //"HistoryReturnToUniversityPressOfficer	تم الإعادة إلى موظف مطابع الجامعة",
        //"HistoryForwardToResearcher	تم الإرسال إلى الباحث",
        //"HistoryReturnToProofreader	تم الإعادة إلى المصحح",
        //"HistorySubmitOperationCommandToViceDeanOfAcademicResearch	تم إرسال أمر التشغيل إلى وكيل العمادة للنشر العلمي",
        //"HistoryReturnToPUAP	تم الإعادة إلى موظف وحدة التجهيز الطباعي",
        //"HistoryAcceptWithComments	تمت الموافقة مع تعديلات",
        //"HistoryCloseRequest	تم إغلاق الطلب",
        //"HistorySubmitISBNRequestToKFNL	تم إرسال طلب الردمك إلى مكتبة الملك الفهد الوطنية",
        //"HistorySubmitISBNToImamUniversity	تم إرسال الردمك إلى جامعة الإمام محمد بن سعود الإسلامية",
        //"HistoryReturnToKFNLOfficer	تمت الإعادة إلى موظف مكتبة الملك فهد الوطنية",
        //"HistoryRejectAndReturnToPUAP	تم الرفض والإعادة إلى موظف وحدة التجهيز الطباعي",
        //"HistoryRejectAndReturnToViceDeanOfAcademicResearch	تم الرفض والإعادة إلى وكيل العمادة للنشر العلمي",
        //"HistorySubmitToPressOfficer	تم الإرسال إلى موظف مطابع الجامعة",
        //"HistoryPrintingCompleted	تمت الطباعة",
        //"HistoryApproveAndForwardToFiancialDepartmentDirector	تمت الموافقة والإرسال إلى مدير الإدارة المالية",
        //"HistoryApproveAndForwardToAccountant	تمت الموافقة والإرسال إلى المحاسب",
        //"HistoryRejectAndReturnToDeanOfAcademicResearch	تم الرفض والإعادة إلى عميد البحث العلمي",
        //"HistoryPaymentCompleted	تم صرف المكافأة",
        //"HistoryPrepareDistributionList	تم إعداد قائمة التوزيع للكتاب",
        //"HistoryConfirmReceivalOfCopies	تم تأكيد استلام نسخ الكتاب",
        //"HistoryConfirmDeliveringCopiesToResaercher	تم تأكيد استلام نسخ الكتاب من قِبل الباحث",
        //"HistoryForwardToKFNL	تم الإرسال إلى مكتبة الملك فهد الوطنية",
        //"HistoryConfirmDeliveringCopiesToKFNL	تم تأكيد استلام نسخ الكتاب من قِبل مكتبة الملك فهد الوطنية",
        //"HistoryConfirmDeliveringCopiesToAgencies	تم تأكيد استلام النسخ لجهات التوزيع",
        //"HistoryForwardToUniversityPressDirector	تم الإرسال إلى مدير مطابع الجامعة",
        //"HistoryApproveAndForwardToDeanshipOfLibraryAffairs	تمت الموافقة والإرسال إلى عميد شؤون المكتبات",
        //"HistoryApproveAndAndForardToDeanOfAcademicResearch	تمت الموافقة والإرسال إلى عميد البحث العلمي",
        //"HistoryReturnToDeanshipOfLibraryAffairs	تمت الإعادة إلى عميد شؤون المكتبات",
        //"HistoryCanceled	تم الإلغاء",
        //"StatePublishRequestNotSubmitted	لم يتم إرسال الطلب ",
        //"StatePublishRequestPendingModificationsAfterRUS	في انتظار تعديلات الباحث ",
        //"StatePublishRequestPendingModificationsAfterReferees	في انتظار تعديلات الباحث بعد التحكيم ",
        //"StatePublishRequestPendingCoresearchers	في انتظار المشاركين في المادة العلمية ",
        //"StatePublishRequestRejectedByCoresearchers	طلب النشر رفض بواسطة المشاركون بالبحث",
        //"StatePublishRequestPendingResearchUnitSecretary	في انتظار أمين وحدة البحوث ",
        //"StatePublishRequestPendingResearchUnitHead	في انتظار  رئيس وحدة البحوث ",
        //"StatePublishRequestPendingResearchUnitCommittee	في انتظار  لجنة النشر",
        //"StatePublishRequestPendingInitialReferee	في انتظار  المحكم المبدئي ",
        //"StatePublishRequestPendingViceDeanOfAcademicResearchPublishApproval	في انتظار  وكيل العمادة للنشر العلمي ",
        //"StatePublishRequestPendingDeanOfAcademicResearchPublishApproval	في انتظار  موافقة عميد البحث العلمي ",
        //"StatePublishRequestPendingDeanshipOfAcademicResearchCommitee	في انتظار لجنة عمادة البحث العلمي ",
        //"StatePublishRequestPendingUniversityRectorPublishApproval	في انتظار موافقة اعتماد مدير الجامعة ",
        //"StatePublishRequestPendingPAERefereeSelection	في انتظار اختيار المحكمين من قبل لجنة النشر والتحكيم والمعادلات ",
        //"StatePublishRequestPendingPAERefereeSelectionViceDean	في انتظار اختيار المحكمين من قبل عميد عمادة النشر العلمي",
        //"StatePublishRequestPendingPAERefereeSelectionHead	في انتظار اختيار رئيس لجنة النشر والتحكيم والمعادلات للمحكمين ",
        //"StatePublishRequestPendingPAERefereeSelectionMembers	في انتظار اختيار أعضاء لجنة النشر والتحكيم والمعادلات للمحكمين ",
        //"StatePublishRequestPendingAcademicCouncilRefereeSelection	في انتظار اختيار المحكمين من قبل المجلس العلمي ",
        //"StatePublishRequestPendingAcademicCouncilRefereeSelectionMembers	في انتظار اختيار المحكمين من قبل أعضاء المجلس العلمي ",
        //"StatePublishRequestPendingAcademicCouncilRefereeSelectionHead	في انتظار اختيار المحكمين من قبل رئيس المجلس العلمي ",
        //"StatePublishRequestPendingUniversityRectorAFterRefereeSelection	في انتظار موافقة مدير الجامعة على اختيار المحكمين  ",
        //"StatePublishRequestPendingPAEAfterRefereeSelectionViceDean	في انتظار اختيار المحكمين من قبل وكيل عمادة البحث العلمي ",
        //"StatePublishRequestPendingPAEAfterRefereeSelection	في انتظار لجنة النشر والتحكيم والمعادلات بعد التحكيم ",
        //"StatePublishRequestPendingReferees	في انتظار المحكمين",
        //"StatePublishRequestPendingPAEAfterReferees	في انتظار لجنة النشر والتحكيم والمعادلات بعد التحكيم ",
        //"StatePublishRequestPendingPAEAfterRefereesViceDean	في انتظار وكيل عمادة النشر العلمي",
        //"StatePublishRequestPendingPAEAfterRefereesHead	في انتظار رئيس لجنة النشر والتحكيم والمعادلات",
        //"StatePublishRequestPendingPAEAfterRefereesMembers	في انتظار أعضاء لجنة النشر والتحكيم والمعادلات ",
        //"StatePublishRequestPendingSpareReferee	في انتظار المحكم الإحتياطي",
        //"StatePublishRequestPendingAcademicCouncilAfterReferees	في انتظار المجلس العلمي بعد التحكيم ",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesHead	في انتظار رئيس المجلس العلمي  ",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesMembers	في انتظار  أعضاء المجلس العلمي ",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesApprove	في انتظار موافقة المجلس العلمي ",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesApproveHead	في انتظار موافقة رئيس المجلس العلمي  ",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesApproveMembers	في انتظار موافقة أعضاء المجلس العلمي ",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesReject	تم الرفض من قبل المجلس العلمي",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesRejectHead	تم الرفض من قبل  رئيس المجلس العلمي",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesRejectMembers	تم الرفض من قبل أعضاء المجلس ",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesApproveWithComments	موافقة المجلس العلمي مع وجود ملاحظات ",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesApproveWithCommentsHead	موافقة رئيس المجلس العلمي مع وجود ملاحظات ",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesApproveWithCommentsMembers	موافقة أعضاء المجلس العلمي مع وجود ملاحظات ",
        //"StatePublishRequestPendingPAECouncilAfterRefereesApproveWithComments	موافقة لجنة النشر والتحكيم والمعادلات ",
        //"StatePublishRequestPendingPAECouncilAfterRefereesApproveWithCommentsViceDean	موافقة وكيل عمادة النشر العلمي ",
        //"StatePublishRequestPendingPAECouncilAfterRefereesApproveWithCommentsHead	موافقة رئيس لجنة النشر والتحكيم والمعادلات مع وجود ملاحظات ",
        //"StatePublishRequestPendingPAEAfterRefereesAndResearcherModifications	في انتظار تعديلات الباحث ",
        //"StatePublishRequestPendingPAEAfterRefereesAndResearcherModificationsViceDean	في انتظار وكيل عمادة  البحث العلمي ",
        //"State	PublishRequestPendingPAEAfterRefereesAndResearcherModificationsHead	",
        //"State	PublishRequestPendingRefereesAfterRefereesAndResearcherModifications	",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesRectorApproved	تمت الموافقة من قبل مدير الجامعة ",
        //"StatePublishRequestPendingAcademicCouncilAfterRefereesRectorRejected	تم الرفض من قبل مدير الجامعة ",
        //"StateOperationCommandPendingOperationCommandInitiation	في انتظار اعطاء امر بدء التشغيل من قبل وكيل العمادة للنشر العلمي",
        //"StateOperationCommandPendingOperationCommandInitiationAfterProofReading	في انتظار اعطاء امر بدء التشغيل من قبل وكيل العمادة للنشر العلمي",
        //"State	OperationCommandPendingOperationCommandPreparation	",
        //"State	OperationCommandPendingOperationCommandApproval	",
        //"State	OperationCommandPendingOperationCommandExecution	",
        //"State	OperationCommandPendingOperationCommandValidation	",
        //"StateOperationCommandPendingResearcherAcceptance	في موافقة الباحث على طلب التشغيل",
        //"StateOperationCommandPendingISBNRequestSubmission	في انتظار إرسال طلب رقم  الردمك ",
        //"StateOperationCommandPendingProofreading	في انتظار المصحح ",
        //"StateOperationCommandPendingOperationCommandFinalApproval	اعتماد طلب التشغيل",
        //"StateOperationCommandPendingPrintingManager	في انتظار مدير المطابع ",
        //"StateOperationCommandPendingOperationCommandForward	إعادة توجيه أمر التشغيل ",
        //"StateOperationCommandPendingPendingCorrection	في انتظار التصحيح",
        //"StateOperationCommandPendingPrinting	في انتظار الطباعة ",
        //"StateOperationCommandPendingResearcherProofreadingAcceptance	في انتظار موافقة المصحح ",
        //"StateOperationCommandPendingOperationCommandForwardResearcherRejection	في انتظار إعادة توجيه رفض الباحث ",
        //"StateOperationCommandPendingResearcherComments	في انتظار تعديلات الباحث ",
        //"State	OperationCommandPendingOperationCommandForward1AndHalf	",
        //"State	OperationCommandPendingResearcherComments2	",
        //"State	OperationCommandPendingOperationCommandExecution2	",
        //"State	OperationCommandPendingOperationCommandForward2	",
        //"State	OperationCommandPendingOperationCommandForward2AndHalf	",
        //"State	OperationCommandPendingOperationCommandExecution3	",
        //"State	OperationCommandPendingOperationCommandForward3	",
        //"State	OperationCommandPendingOperationCommandResearcherRejected	",
        //"State	OperationCommandPendingISBN	",
        //"StateOperationCommandAfterISBN	بدء عملية التسعير",
        //"StateOperationCommandPendingOperationCommandFinalApprovalViceDean	الموافقة النهائية من قبل وكيل عمادة البحث العلمي ",
        //"StatePricingPendingPricingPrepartion	بدء عملية التسعير",
        //"StatePricingPendingPricingApprovalByUniversityPress	في انتظار موافقة مدير مطابع الجامعة ",
        //"StatePricingPendingPricingApprovalByLibraryAffairs	في انتظار موافقة عميد شؤون المكتبات ",
        //"StatePricingPendingPricingApprovalByDean	في انتظار موافقة العميد ",
        //"StateDistributionPendingDistributionListPreparation	إعداد قوائم التوزيع",
        //"StateDistributionPendingDistributionListApproval	اعتماد قوائم التوزيع",
        //"StateDistributionPendingDistributionListApprovalViceDean	في انتظار موافقة وكيل عمادة النشر العلمي ",
        //"StateDistributionPendingAuthorCopiesDistributionByPR	معالجة طلب تسليم نسخ للمؤلف من قبل مدير العلاقات العامة",
        //"State	DistributionPendingKFNLCopiesDistributionByPrintingUnit	",
        //"StateDistributionPendingCopiesDistributionByLibraryAffairs	معالجة طلب التوزيع على الجهات",
        //"StateDistributionPendingDistributionClosure	في انتظار إغلاق طلب التوزيع",
        //"StateDistributionPendingResearcherConfirmation	في انتظار تأكيد استلام النسخ من قِبل الباحث",
        //"StateDistributionPendingCopyDistribution	في انتظار توزيع النسخ",
        //"StateRewardPendingApprovalByViceDean	في انتظار اعتماد المكافأة من قبل وكيل عمادة النشر العلمي ",
        //"StateRewardPendingApprovalByDean	في انتظار اعتماد المكافأة من قبل العميد",
        //"StateRewardPendingAccountant	في انتظار صرف الموافقة من قبل المحاسب",
        //"StateRewardPendingFinancialDepartmentDirector	في انتظار صرف الموافقة من قبل مدير الإدارة المالية",
        //"StatePendingAcademicCouncilApproval	في انتظار موافقة المجلس العلمي",
        //"StatePendingAdvertisement	في انتظار الإعلان",
        //"StatePending	قيد الانتظار",
        //"StateRejected	تم رفض الطلب",
        //"StateConditionalApproval	موافقة مشروطة",
        //"StateApproved	تم الاعتماد ",
        //"StateRejectedByResearcher	تم رفض الطلب بواسطة الباحث",
        //"StateCanceled	تم إلغاء الطلب",
        //"StateCompleted	تم اكتمال الطلب",
        //            };
        //            //var s = Utils.db.RequestActions.ToList();
        //            //foreach (var k in s)
        //            //{
        //            //    var translation = LangHelpers.Instance[k.Key];

        //            //    Utils.db.RequestActions.Where(e => e.Key == k.Key).FirstOrDefault().NameArabic = translation;
        //            //    Console.WriteLine(translation);

        //            //    Console.ReadLine();
        //            //}
        //            //Utils.db.SaveChanges();

        //            return;
        //            var writer = new StreamWriter(outputFilePath, false, Encoding.UTF8);

        //            foreach (var k in dict)
        //            {
        //                var translation = LangHelpers.Instance[k];
        //                writer.WriteLine(translation);
        //                //writer.WriteLine(Utils.PascalCaseToUnderscore(k));
        //            }

        //            writer.Close();
        //        }

        //        public static void PrintStatuses(string outputFilePath)
        //        {
        //            var writer = new StreamWriter(outputFilePath, false, Encoding.UTF8);

        //            writer.WriteLine("{0}\t{1}\t{2}", "Term", "ArabicTranslation", "EnglishTranslation");

        //            //var roles = Enum.GetValues(typeof(ArfUserRole));

        //            //foreach (var r in roles)
        //            //{
        //            //    writer.WriteLine("{0}\t{1}\t{2}",
        //            //            Utils.PascalCaseToUnderscore(r.ToString()),
        //            //            LangHelpers.GetLangValueByString("AR", Utils.PascalCaseToUnderscore(r.ToString()), false),
        //            //            LangHelpers.GetLangValueByString("EN", Utils.PascalCaseToUnderscore(r.ToString()), false)
        //            //    );
        //            //}

        //            //var statuses = Enum.GetValues(typeof(RequestStatus));

        //            //foreach (var r in statuses)
        //            //{
        //            //    writer.WriteLine("{0}\t{1}\t{2}",
        //            //            Utils.PascalCaseToUnderscore(r.ToString()),
        //            //            LangHelpers.GetLangValueByString("AR", Utils.PascalCaseToUnderscore(r.ToString()), false),
        //            //            LangHelpers.GetLangValueByString("EN", Utils.PascalCaseToUnderscore(r.ToString()), false)
        //            //    );
        //            //}

        //            //var types = Enum.GetValues(typeof(Data.RequestType));

        //            //foreach (var r in types)
        //            //{
        //            //    writer.WriteLine("{0}\t{1}\t{2}",
        //            //            Utils.PascalCaseToUnderscore(r.ToString()),
        //            //            LangHelpers.GetLangValueByString("AR", Utils.PascalCaseToUnderscore(r.ToString()), false),
        //            //            LangHelpers.GetLangValueByString("EN", Utils.PascalCaseToUnderscore(r.ToString()), false)
        //            //    );
        //            //}

        //            //var actions = Enum.GetValues(typeof(Data.RequestAction));

        //            //foreach (var r in actions)
        //            //{
        //            //    writer.WriteLine("{0}\t{1}\t{2}",
        //            //            Utils.PascalCaseToUnderscore(r.ToString()),
        //            //            LangHelpers.GetLangValueByString("AR", Utils.PascalCaseToUnderscore(r.ToString()), false),
        //            //            LangHelpers.GetLangValueByString("EN", Utils.PascalCaseToUnderscore(r.ToString()), false)
        //            //    );
        //            //}

        //            //foreach (var s in statuses)
        //            //{
        //            //    foreach (var a in actions)
        //            //    {
        //            //        var term = Utils.PascalCaseToUnderscore(s.ToString()) + "_" + Utils.PascalCaseToUnderscore(a.ToString());

        //            //        writer.WriteLine("{0}\t{1}\t{2}",
        //            //                Utils.PascalCaseToUnderscore(term),
        //            //                LangHelpers.GetLangValueByString("AR", term, false),
        //            //                LangHelpers.GetLangValueByString("EN", term, false)

        //            //        );
        //            //    }
        //            //}

        //            //

        //            var list = new List<Tuple<RequestStatus, Data.RequestAction>>();

        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.RefereeRewardRecievedByDirector_of_Research_Projects_Financing_Program, Data.RequestAction.SendTo_ViceDean_for_research_centers_and_databases));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.RefereeRewardRecievedBy_ViceDean_for_research_centers_and_databases, Data.RequestAction.SendToDeanOfScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.RefereeRewardRecievedByDeanOfScientificResearch, Data.RequestAction.ApproveAndForwardToAccountant));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.RefereeRewardRecievedByAccountant_of_Deanship_of_Scientific_Research, Data.RequestAction.PaidClaims));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MidTermReportPendingSubmission, Data.RequestAction.SendToViceDeanForHigherStudiesAndScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MidTermReportRecievedByResearcher, Data.RequestAction.SendToSecretary_of_Deanship_Research_Centers_and_databases));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearch, Data.RequestAction.SendToSecretary_of_Deanship_Research_Centers_and_databases));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MidTermReportRecievedByVice_of_Deanship_Research_Centers_and_databases, Data.RequestAction.SendToDirector_of_Research_Projects_Financing_Program));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MidTermReportRecievedByDirector_of_Research_Projects_Financing_Program, Data.RequestAction.ApproveAndSendToDeanshOfScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MidTermReportRecievedByDeanOfScientificResearch, Data.RequestAction.ApproveAndForwardToAccountant));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MidTermReportRecievedByAccountant_of_Deanship_of_Scientific_Research, Data.RequestAction.PaidClaims));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MaterialRequestPendingSubmissionByResearcher, Data.RequestAction.SendToViceDeanForHigherStudiesAndScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MaterialRequestReceivedByViceDeanForHigherStudiesAndScientificResearch, Data.RequestAction.SendToDirectorOfProgramOfFinance));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MaterialRequestReceivedByDirectorOfProgramOfFinance, Data.RequestAction.SendToVicePresidentForHigherStudiesAndScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MaterialRequestReceivedByVicePresidentForHigherStudiesAndScientificResearch, Data.RequestAction.ApproveAndForwardToUniversityRector));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MaterialRequestReceivedByUniversityRector, Data.RequestAction.ApproveAndSendToDirectorGeneralOfAdministrationAndFinance));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MaterialRequestReceivedByDirectorGeneralOfAdministrationAndFinance, Data.RequestAction.SendToViceDeanForHigherStudiesAndScientificResearch2));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.MaterialRequestReceivedByViceDeanForHigherStudiesAndScientificResearch2, Data.RequestAction.CompleteTheRequest));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.RefereeRewardRecievedByDirector_of_Research_Projects_Financing_Program, Data.RequestAction.SendTo_ViceDean_for_research_centers_and_databases));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.RefereeRewardRecievedBy_ViceDean_for_research_centers_and_databases, Data.RequestAction.SendToDeanOfScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.RefereeRewardRecievedByDeanOfScientificResearch, Data.RequestAction.ApproveAndForwardToAccountant));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.RefereeRewardRecievedByAccountant_of_Deanship_of_Scientific_Research, Data.RequestAction.PaidClaims));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestPendingSubmission, Data.RequestAction.SendToSecretary_of_Deanship_Research_Centers_and_databases));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestPendingModificationsByResearcher, Data.RequestAction.SendToDirectorOfProgramOfFinance));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies, Data.RequestAction.ApproveAndSendToDeanshOfScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestRecievedByDeanOfScientificResearch, Data.RequestAction.ForwardToViceDean_for_research_centers_and_databases));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestRecivedByViceDeanOfResearchCenterAndDatabases, Data.RequestAction.SendToSecretary_of_Deanship_Research_Centers_and_databases));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestRecievedBySecertaryOfDeanShipResearchCentersAndDatabases, Data.RequestAction.ApproveAndSendToDirectorOfFinancing));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancing, Data.RequestAction.ForwardToSecretary_of_Standing_Committee_for_Financing_Research_Projects));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestRecievedBySecretaryOfCommitteeResearchFinance, Data.RequestAction.ApproveAndSelectReferees));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeSelection, Data.RequestAction.ForwardToMainReferees));

        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterResearcher, Data.RequestAction.ReturnToResearcher));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestReceivedBySpareReferee, Data.RequestAction.Approve));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestWaitingForContractPreparation, Data.RequestAction.SendToDeanOfScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestContractReceivedByDeanOfScientificResearch, Data.RequestAction.Approve));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestReceivedByDirectorOfFinancingAfterDeanApproval, Data.RequestAction.SendToDeanOfScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestContractIssuedWaitingForResearcher, Data.RequestAction.Approve));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestContractReceivedByDeanOfScientificResearchAfterResearcher, Data.RequestAction.Approve));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestContractAcceptedByResearcher, Data.RequestAction.SendToDeanOfScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FundingRequestContractRejectedByResearcher, Data.RequestAction.Reject));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FinalReportPendingSubmission, Data.RequestAction.SendToSecretary_of_Deanship_Research_Centers_and_databases));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FinalReportRecievedByResearcher, Data.RequestAction.SendToSecretary_of_Deanship_Research_Centers_and_databases));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FinalReportRecievedByVice_of_Deanship_Research_Centers_and_databases, Data.RequestAction.ReturnToResearcher));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FinalReportRecievedByDirector_of_Research_Projects_Financing_Program, Data.RequestAction.ForwardToDeanOfScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FinalReportRecievedByDeanOfScientificResearch, Data.RequestAction.ApproveAndForwardToAccountant));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FinalReportRecievedByDeanOfScientificResearchContainer, Data.RequestAction.ForwardToViceDean_for_research_centers_and_databases));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FinalReportRecievedByAccountant_of_Deanship_of_Scientific_Research, Data.RequestAction.PaidClaims));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FinalReportRecievedBy_ViceDean_for_research_centers_and_databases, Data.RequestAction.ForwardToSecretary_of_Standing_Committee_for_Financing_Research_Projects));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.FinalReportRecievedBySecretary_of_Standing_Committee_for_Financing_Research_Projects, Data.RequestAction.ApproveContainer));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.DesignRequestReceivedByDesigner, Data.RequestAction.SendToDeanOfScientificResearch));
        //            list.Add(new Tuple<RequestStatus, Data.RequestAction>(RequestStatus.DesignRequestReceivedByDeanOfScientificResearch, Data.RequestAction.ApproveAndSendToDesignerAndDirectorOfFinancing));

        //            foreach (var t in list)
        //            {
        //                var term = Utils.PascalCaseToUnderscore(t.Item1.ToString()) + "_" + Utils.PascalCaseToUnderscore(t.Item2.ToString());

        //                if (term == "design_request_received_by_dean_of_scientific_research_send_to_dean_of_scientific_research")
        //                {
        //                }

        //                //writer.WriteLine("{0}\t{1}\t{2}",
        //                //        Utils.PascalCaseToUnderscore(term),
        //                //        LangHelpers.GetLangValueByString("AR", term, false),
        //                //        LangHelpers.GetLangValueByString("EN", term, false)

        //                //);
        //            }

        //            writer.Close();
        //        }

        //        public static void WriteTranslations(string inputFile, string outputFilePath)
        //        {
        //            var reader = new StreamReader(inputFile, Encoding.UTF8);
        //            var lines = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        //            var writer = new StreamWriter(outputFilePath, false, Encoding.UTF8);

        //            foreach (var line in lines)
        //            {
        //                writer.WriteLine(Utils.PascalCaseToUnderscore(line));
        //            }

        //            writer.Close();
        //        }

        //        public static void TableHeadingsToTables()
        //        {
        //            var db = Utils.db;

        //            var forms = db.Forms.Where(f => f.FormFields.Count(d => d.FieldType == (byte)FormFieldType.TableHeading) > 0).ToList();

        //            foreach (var form in forms)
        //            {
        //                var sections = form.FormSections.ToList();

        //                foreach (var sec in sections)
        //                {
        //                }
        //            }
        //        }

        //        public static void WriteViewsAndModels(string outputFolder)
        //        {
        //            Directory.CreateDirectory(outputFolder + "/Views/Requests/FundingRequests/");
        //            Directory.CreateDirectory(outputFolder + "/Models/");

        //            var viewsFolder = outputFolder + "/Views/Requests/FundingRequests/";
        //            var modelFile = outputFolder + "/Models/CommonFundingRequestViewModelDelta.cs";
        //            var termsFile = outputFolder + "/termsArabic.text";
        //            var termsFileEnglish = outputFolder + "/termsEnglish.text";

        //            var db = Utils.db;

        //            var modelWriter = new StreamWriter(modelFile, false, Encoding.UTF8);
        //            var termsWriterArabic = new StreamWriter(termsFile, false, Encoding.UTF8);
        //            var termsWriterEnglish = new StreamWriter(termsFileEnglish, false, Encoding.UTF8);

        //            var encounteredFields = new HashSet<long>();

        //            var classModels = "";

        //            foreach (var tstr in Enum.GetValues(typeof(ReqFieldType)))
        //            {
        //                var fieldType = (ReqFieldType)tstr;

        //                if (fieldType == ReqFieldType.Unspecified) continue;

        //                var formKey = (string)null;

        //                switch (fieldType)
        //                {
        //                    case ReqFieldType.ArabicLanguageField:
        //                    case ReqFieldType.IslamicField:
        //                        formKey = "Islamic&ArabicForm";
        //                        break;

        //                    case ReqFieldType.SocialField:
        //                        formKey = "SocialForm";
        //                        break;

        //                    case ReqFieldType.AppliedScienceField:
        //                        formKey = "scientificForm";
        //                        break;
        //                }

        //                var form = db.Forms.FirstOrDefault(f => f.Key == formKey);
        //                if (form == null) continue;

        //                var activeSections = form.FormSections
        //                .Where(s => s.IsActive && s.FormFields.Count(x => x.IsActive) > 0)
        //                .OrderBy(s => s.Key)
        //                .ThenBy(s => s.Id)
        //                .ToList();


        //                for (var i = 1; i <= activeSections.Count; i++)
        //                {
        //                    var step = i;

        //                    if (i > 1) step++;

        //                    var section = activeSections[i - 1];

        //                    var model = new FormFillModel
        //                    {
        //                        FormId = form.Id,
        //                        IsLTR = section.IsLtr,
        //                        SectionId = section.Id,
        //                        Step = step,
        //                        StepCount = activeSections.Count,
        //                        StepTitles = activeSections.Select(s => s.LocalizedName()).ToList(),
        //                        Fields = section.FormFields.Where(f => f.IsActive).OrderBy(f => f.Order).ThenBy(f => f.Id).Select(f => new FormFieldModel
        //                        {
        //                            Id = f.Id,
        //                            FormId = f.FormId,
        //                            LookupId = f.LookupId,
        //                            SectionId = f.SectionId,
        //                            Key = f.Key,
        //                            Name = f.NameArabic,
        //                            NameEnglish = f.NameEnglish,
        //                            IsLTR = f.IsLtr,

        //                            FieldType = (FormFieldType)f.FieldType,
        //                            IsRequired = f.IsRequired,
        //                            IsMultipleSelection = f.IsMultipleSelection,
        //                            IsActive = f.IsActive,
        //                            MinValue = f.MinValue == null ? (int?)null : int.Parse(f.MinValue),
        //                            MaxValue = f.MaxValue == null ? (int?)null : int.Parse(f.MaxValue),
        //                            DependsOnFieldId = f.DependsOnFieldId,
        //                            DependsOnFieldValue = f.DependsOnFieldValue,
        //                            VisibleToRoles = f.VisibleToRoles,
        //                            InvisibleToRoles = f.InvisibleToRoles,
        //                            Order = f.Order,
        //                            IsReadOnly = f.IsReadOnly,
        //                            //Lookups = f.LookupId > 0 ? f.Lookup.LookupValues.Select(v => new SelectListItem
        //                            //{
        //                            //    Value = v.Id + "",
        //                            //    Text = v.LocalizedName()
        //                            //}).ToList() : null,

        //                            OriginalField = f
        //                        }).ToList(),
        //                    };

        //                    var folder = viewsFolder + "/";

        //                    switch (fieldType)
        //                    {
        //                        case ReqFieldType.ArabicLanguageField:
        //                            formKey = "Arabic";
        //                            break;
        //                        case ReqFieldType.IslamicField:
        //                            formKey = "Islamic";
        //                            break;

        //                        case ReqFieldType.SocialField:
        //                            formKey = "Social";
        //                            break;

        //                        case ReqFieldType.AppliedScienceField:
        //                            formKey = "Scientific";
        //                            break;
        //                    }

        //                    Directory.CreateDirectory(folder);

        //                    var viewWriter = new StreamWriter(viewsFolder + "/" + formKey + "/Step" + step + ".cshtml", false, Encoding.UTF8);


        //                    var tablestarted = false;
        //                    var headingstarted = false;
        //                    var index = -1;
        //                    var tableFieldIndex = -1;
        //                    var tableFIeld = (FormFieldModel)null;

        //                    viewWriter.WriteLine("@model CommonFundingRequestViewModel");
        //                    viewWriter.WriteLine("");


        //                    var addedFields = new HashSet<string>();

        //                    var tableModel = (string)null;

        //                    foreach (var fieldM in model.Fields)
        //                    {
        //                        var field = fieldM.OriginalField;

        //                        fieldM.Key = "FIELD_" + field.Id;

        //                        if (fieldM.FieldType == FormFieldType.Table)
        //                        {

        //                            if (!addedFields.Contains("FIELD_" + field.Id))
        //                            {
        //                                modelWriter.WriteLine("\r\n\r\n // FIELD_" + field.Id + ": " + field.LocalizedName() + "");
        //                                modelWriter.WriteLine("[Step(Step = " + step + ", IsRequired = " + (field.IsRequired ? "true" : "false") + ", FieldType = FormFieldType." + ((FormFieldType)field.FieldType).ToString() + ")]");
        //                                modelWriter.WriteLine("[LocalizedDisplay(Name = \"FIELDTERM_" + field.Id + "\", ResourceType = typeof(ArfStrings))]");
        //                                modelWriter.WriteLine("public List<FIELD_" + field.Id + "> FIELD_" + field.Id + " {get; set;} = new List<FIELD_" + field.Id + ">(); ");

        //                                tableModel = "public class FIELD_" + field.Id + " {";

        //                                addedFields.Add("FIELD_" + field.Id);
        //                            }

        //                        }
        //                        else if (tablestarted)
        //                        {

        //                            if (
        //                                fieldM.FieldType != FormFieldType.TableEnd &&
        //                                fieldM.FieldType != FormFieldType.ColumnHeading &&
        //                                fieldM.FieldType != FormFieldType.Table &&
        //                                fieldM.FieldType != FormFieldType.Heading &&
        //                                fieldM.FieldType != FormFieldType.SubHeading
        //                            )
        //                            {

        //                                if (!addedFields.Contains("FIELD_" + field.Id))
        //                                {
        //                                    tableModel += ("\r\n\r\n\r\n // FIELD_" + field.Id + ": " + field.LocalizedName() + "");
        //                                    tableModel += ("\r\n[Step(Step = " + step + ", IsRequired = " + (field.IsRequired ? "true" : "false") + ", FieldType = FormFieldType." + ((FormFieldType)field.FieldType).ToString() + ")]");
        //                                    tableModel += ("\r\n[LocalizedDisplay(Name = \"FIELDTERM_" + field.Id + "\", ResourceType = typeof(ArfStrings))]");
        //                                    tableModel += ("\r\npublic string FIELD_" + field.Id + " {get; set;}");

        //                                    addedFields.Add("FIELD_" + field.Id);
        //                                }

        //                            }
        //                            else if (fieldM.FieldType == FormFieldType.TableEnd)
        //                            {
        //                                classModels += "\r\n" + tableModel + "\r\n}\r\n\r\n";

        //                            }
        //                        }

        //                        if (!encounteredFields.Contains(field.Id) && fieldM.FieldType != FormFieldType.TableEnd &&
        //                            fieldM.FieldType != FormFieldType.ColumnHeading &&
        //                            fieldM.FieldType != FormFieldType.Table &&
        //                            fieldM.FieldType != FormFieldType.Heading &&
        //                            fieldM.FieldType != FormFieldType.SubHeading)
        //                        {
        //                            encounteredFields.Add(field.Id);

        //                            if (!addedFields.Contains("FIELD_" + field.Id))
        //                            {
        //                                modelWriter.WriteLine("\r\n\r\n // FIELD_" + field.Id + ": " + field.LocalizedName() + "");
        //                                modelWriter.WriteLine("[Step(Step = " + step + ", IsRequired = " + (field.IsRequired ? "true" : "false") + ", FieldType = FormFieldType." + ((FormFieldType)field.FieldType).ToString() + ")]");
        //                                modelWriter.WriteLine("[LocalizedDisplay(Name = \"FIELDTERM_" + field.Id + "\", ResourceType = typeof(ArfStrings))]");
        //                                modelWriter.WriteLine("public string FIELD_" + field.Id + " {get; set;}");

        //                                addedFields.Add("FIELD_" + field.Id);
        //                            }

        //                            termsWriterArabic.WriteLine("FIELDTERM_" + field.Id + "\t" + field.NameArabic);
        //                            termsWriterEnglish.WriteLine("FIELDTERM_" + field.Id + "\t" + field.NameEnglish);

        //                            if (field.Lookup != null && field.Lookup.LookupValues.Count > 0)
        //                            {
        //                                foreach (var lookup in field.Lookup.LookupValues)
        //                                {
        //                                    termsWriterArabic.WriteLine("LOOKUP_" + lookup.Id + "\t" + lookup.NameArabic);
        //                                    termsWriterEnglish.WriteLine("LOOKUP_" + lookup.Id + "\t" + lookup.NameEnglish);
        //                                }
        //                            }
        //                        }

        //                        //var str = fieldM.RenderFieldModel(null, true, ref tablestarted, ref headingstarted, ref index, ref tableFieldIndex, ref tableFIeld);
        //                        var str = "";

        //                        viewWriter.WriteLine(str);

        //                        viewWriter.WriteLine("");
        //                    }

        //                    if (tablestarted)
        //                    {
        //                        classModels += tableModel + "\r\n}\r\n\r\n";



        //                    }

        //                    viewWriter.Close();

        //                }



        //            }




        //            modelWriter.WriteLine(classModels);





        //            modelWriter.Close();
        //            termsWriterArabic.Close();
        //            termsWriterEnglish.Close();
        //        }


        //        public static void ImportPriorities(string inputXlsxFile, string outputSqlFile)
        //        {
        //            if (!File.Exists(inputXlsxFile)) return;
        //            var writer = new StreamWriter(outputSqlFile, false, Encoding.UTF8);

        //            var db = Utils.db;

        //            var departments = db.Departments.ToList();

        //            using (var pkg = new ExcelPackage(new FileInfo(inputXlsxFile)))
        //            {
        //                var sheet = pkg.Workbook.Worksheets["اولوليات النشر العلمي"];

        //                var rowCount = sheet.Dimension.End.Row;

        //                for (var row = 2; row <= rowCount; row++)
        //                {
        //                    if (sheet.Cells[row, 2].Value == null) continue;

        //                    var deps = sheet.Cells[row, 2].Value.ToString().Split(new string[] { "\n", "\r\n", "-" }, StringSplitOptions.RemoveEmptyEntries)
        //                        .Where(x => x.Length > 5)
        //                        .Select(x => x.EndsWith(".") ? x.Substring(0, x.Length - 1) : x)
        //                        .Select(x => x.Trim())
        //                        .ToList();

        //                    var priorities = sheet.Cells[row, 3].Value.ToString().Split(new string[] { "\n", "\r\n", "-" }, StringSplitOptions.RemoveEmptyEntries)
        //                        .Where(x => x.Length > 5)
        //                        .Select(x => x.EndsWith(".") ? x.Substring(0, x.Length - 1) : x)
        //                        .Select(x => x.Trim())
        //                        .ToList();

        //                    foreach (var dep2 in deps)
        //                    {
        //                        var dep = dep2;
        //                        if (dep.StartsWith("قسم"))
        //                        {
        //                            dep = dep.Substring("قسم".Length).Trim();
        //                        }
        //                        var depEnt = departments.Where(d => Utils.NormalizeString(d.NameArabic) == Utils.NormalizeString(dep)).FirstOrDefault();
        //                        if (depEnt == null)
        //                        {
        //                            continue;
        //                        }

        //                        foreach (var p2 in priorities)
        //                        {
        //                            var p = p2.Replace("'", "''");
        //                            var sql1 = @"IF NOT EXISTS (select * from Priorities where namearabic = N'" + p + @"') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'" + p + @"', getdate(), N'" + Utils.SystemAdminUserId + @"', getdate(), N'" + Utils.SystemAdminUserId + @"') end";
        //                            var sql2 = @"IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'" + depEnt.NameArabic + @"') and priorityid = (Select id from Priorities where namearabic = N'" + p + @"')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'" + depEnt.NameArabic + @"'),  (Select id from Priorities where namearabic = N'" + p + @"')) end";
        //                            writer.WriteLine(sql1);
        //                            writer.WriteLine("GO");

        //                            writer.WriteLine(sql2);
        //                            writer.WriteLine("GO");
        //                        }
        //                    }

        //                }

        //            }

        //            writer.Close();
        //        }

        //        public static void CleanupTranslations(string inputFilePath, string outputFilePath, bool englishOnly = false, bool arabicOnly = false)
        //        {

        //            var reader = new StreamReader(inputFilePath, Encoding.UTF8, true);
        //            var writer = new StreamWriter(outputFilePath, false, Encoding.UTF8);
        //            var lines = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);


        //            var encountered = new HashSet<string>();

        //            foreach (var line in lines)
        //            {
        //                if (string.IsNullOrWhiteSpace(line)) continue;
        //                var tabs = line.Split('\t');

        //                if (tabs[0] == "step2")
        //                {

        //                }

        //                if (tabs.Length < 2) continue;

        //                var key = Utils.PascalCaseToUnderscore(tabs[0].Trim());

        //                if (string.IsNullOrWhiteSpace(key)) continue;

        //                var val = tabs[1].Trim();

        //                if (key.StartsWith("fieldterm_") || key.EndsWith("1") || key.EndsWith("2") || key.EndsWith("3") || key.EndsWith("4") || key.EndsWith("5")) continue;

        //                if (encountered.Contains(key)) continue;

        //                if (arabicOnly && !englishOnly && !Utils.IsArabicString(val))
        //                {
        //                    continue;
        //                }

        //                if (englishOnly && !arabicOnly && !Utils.IsEnglishString(val))
        //                {
        //                    continue;
        //                }


        //                encountered.Add(key);

        //                writer.WriteLine("{0}\t{1}", key, val);
        //            }

        //            writer.Close();
        //            reader.Close();

        //        }
    }
}