using ARF.Core;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ARF.TestConsole
{
    internal class GenerateProgram
    {
        public static int SAVE_EVERY = 1000;

        public static string OUTPUT_FOLDER = @"c:\Temp\";

        public static string OUTPUT_FILE_NAME = "RCS_TEST_DATA3.sql";

        private static StreamWriter writer = null;

        public static bool UseEntityFramework = false;

        public static void GenerateMain(params string[] args)
        {
            if (!UseEntityFramework && !Directory.Exists(OUTPUT_FOLDER))
                Directory.CreateDirectory(OUTPUT_FOLDER);

            writer = new StreamWriter(OUTPUT_FOLDER + OUTPUT_FILE_NAME, false, Encoding.UTF8);

            var db = new ArfDbEntities();
            var rand = new Random();

            DbResetter.ResetDatabase();

            var roles = db.Roles.ToList();
            var regions = db.Regions.ToList();
            var cities = db.Cities.ToList();
            var users = new List<User>() { db.Users.First(u => u.UserName == "system") };
            var max_department_id = db.Departments.Count() == 0 ? 0 : db.Departments.Max(p => p.Id);
            var max_group_id = db.Departments.Count() == 0 ? 0 : db.Departments.Max(p => p.Id);

            var colleges = new List<College>();
            var groups = new List<Department>();

            var departmentNamesArabic = new string[] { "شؤون الموظفين", "الادارة المالية", "العلاقات العامة", "إدارة الـ IT" };

            writer.WriteLine("use ARF;\r\n");
            var i = 0;

            for (i = 0; i < departmentNamesArabic.Length; i++)
            {

                var college = new College
                {
                    Id = max_department_id + (i + 1),
                    NameArabic = departmentNamesArabic[i],
                    NameEnglish = departmentNamesArabic[i],

                    Created = TM.Now,
                    CreatedBy = users[0].Id,
                    Updated = TM.Now,
                    UpdatedBy = users[0].Id,
                };

                colleges.Add(college);

                if (UseEntityFramework)
                {
                    db.Colleges.Add(college);
                }
                else
                {
                    writer.WriteLine(Helpers.ToInsertSql(college, lookForIdProperty: true));
                }
            }

            if (UseEntityFramework)
            {
                db.SaveChanges();
            }
            else
            {
                FlushWriter();
            }

            i = 0;
            // DepartmentGroups
            foreach (var p in colleges)
            {
                for (var k = 0; k < 3; k++)
                {

                    var dep = new Department
                    {
                        Id = max_group_id + (i + 1),
                        CollegeId = p.Id,
                        NameArabic = "قسم " + (k + 1),
                        NameEnglish = "Group " + (k + 1),
                        Created = TM.Now,
                        CreatedBy = users[0].Id,
                        Updated = TM.Now,
                        UpdatedBy = users[0].Id,
                    };

                    groups.Add(dep);

                    if (UseEntityFramework)
                    {
                        //p.Departments.Add(dep);
                    }
                    else
                    {
                        writer.WriteLine(Helpers.ToInsertSql(dep, lookForIdProperty: true));
                    }

                    i++;

                }
            }

            if (UseEntityFramework)
            {
                db.SaveChanges();
            }
            else
            {
                FlushWriter();
            }

            i = (int)max_department_id + 1;

            var userNo = 0;

            {
                var gender = ' ';
                var name = FakeGenerator.GenerateName(out gender);
                var cmps = Utils.GetNameComponents(name);


                var userDG = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "dg",
                    PasswordHash = users[0].PasswordHash,
                    SecurityStamp = users[0].SecurityStamp,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    ForcePasswordChange = false,
                    IsActivated = true,
                    Roles = roles.Where(r => r.Name == ArfUserRole.DeanofDeanshipofScientificResearch.ToString()).ToList(),
                    UserProfile = new UserProfile
                    {
                        FirstName = cmps.FirstName,
                        MiddleName = cmps.SecondName,
                        LastName = cmps.LastName,
                        Gender = gender == 'M' ? false : true,
                        NationalId = FakeGenerator.GenerateNIN(),
                        Created = TM.Now,
                        CreatedBy = users[0].Id,
                        Updated = TM.Now,
                        UpdatedBy = users[0].Id,
                    },
                    Created = TM.Now,
                    CreatedBy = users[0].Id,
                    Updated = TM.Now,
                    UpdatedBy = users[0].Id,
                };
                userDG.Email = userDG.UserName + "@gmail.com";
                userDG.UserProfile.UserId = userDG.Id;

                users.Add(userDG);

                var fin = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "fin",
                    PasswordHash = users[0].PasswordHash,
                    SecurityStamp = users[0].SecurityStamp,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    ForcePasswordChange = false,
                    IsActivated = true,
                    Roles = roles.Where(r => r.Name == ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch.ToString()).ToList(),
                    UserProfile = new UserProfile
                    {
                        FirstName = cmps.FirstName,
                        MiddleName = cmps.SecondName,
                        LastName = cmps.LastName,
                        Gender = gender == 'M' ? false : true,
                        NationalId = FakeGenerator.GenerateNIN(),
                        Created = TM.Now,
                        CreatedBy = users[0].Id,
                        Updated = TM.Now,
                        UpdatedBy = users[0].Id,
                    },
                    Created = TM.Now,
                    CreatedBy = users[0].Id,
                    Updated = TM.Now,
                    UpdatedBy = users[0].Id,
                };
                fin.Email = fin.UserName + "@gmail.com";
                fin.UserProfile.UserId = fin.Id;

                users.Add(userDG);
                users.Add(fin);


                if (UseEntityFramework)
                {
                    db.Users.Add(userDG);
                    db.Users.Add(fin);
                }
                else
                {
                    writer.WriteLine(Helpers.ToInsertSql(userDG, lookForIdProperty: false));
                    writer.WriteLine(Helpers.ToInsertSql(userDG.Roles.Select(r => new UserRole { UserId = userDG.Id, RoleId = r.Id }).Single(), lookForIdProperty: false));
                    writer.WriteLine(Helpers.ToInsertSql(userDG.UserProfile, lookForIdProperty: false));

                    writer.WriteLine(Helpers.ToInsertSql(fin, lookForIdProperty: false));
                    writer.WriteLine(Helpers.ToInsertSql(fin.Roles.Select(r => new UserRole { UserId = fin.Id, RoleId = r.Id }).Single(), lookForIdProperty: false));
                    writer.WriteLine(Helpers.ToInsertSql(fin.UserProfile, lookForIdProperty: false));
                }
            }

            var j = 0;
            foreach (var c in colleges)
            {
                var gender = ' ';
                var name = FakeGenerator.GenerateName(out gender);
                var cmps = Utils.GetNameComponents(name);

                var user1 = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "user_" + ((++userNo) + "").PadLeft(2, '0'),
                    PasswordHash = users[0].PasswordHash,
                    CollegeId = c.Id,
                    SecurityStamp = users[0].SecurityStamp,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    ForcePasswordChange = false,
                    IsActivated = true,
                    Roles = roles.Where(r => r.Name == ArfUserRole.PrincipalResearcher.ToString()).ToList(),
                    UserProfile = new UserProfile
                    {
                        FirstName = cmps.FirstName,
                        MiddleName = cmps.SecondName,
                        LastName = cmps.LastName,
                        Gender = gender == 'M' ? false : true,
                        NationalId = FakeGenerator.GenerateNIN(),
                        Created = TM.Now,
                        CreatedBy = users[0].Id,
                        Updated = TM.Now,
                        UpdatedBy = users[0].Id,
                    },
                    Created = TM.Now,
                    CreatedBy = users[0].Id,
                    Updated = TM.Now,
                    UpdatedBy = users[0].Id,
                };

                if (userNo == 1) user1.UserName = "dep";
                user1.Email = user1.UserName + "@gmail.com";
                user1.UserProfile.UserId = user1.Id;

                users.Add(user1);

                var id1 = 0;
                var id2 = 0;

                foreach (var group in groups.Where(x => x.CollegeId == c.Id))
                {
                    if (id1 % 2 == 0)
                        id1 = 0;

                    if (id2 % 2 == 0)
                        id2 = 0;

                    var juser1 = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = "user_" + ((++userNo) + "").PadLeft(2, '0'),
                        PasswordHash = users[0].PasswordHash,
                        CollegeId = c.Id,
                        SecurityStamp = users[0].SecurityStamp,
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        ForcePasswordChange = false,
                        IsActivated = true,
                        Roles = roles.Where(r => r.Name == ArfUserRole.Accountant.ToString()).ToList(),
                        UserProfile = new UserProfile
                        {
                            FirstName = cmps.FirstName,
                            MiddleName = cmps.SecondName,
                            LastName = cmps.LastName,
                            Gender = gender == 'M' ? false : true,
                            NationalId = FakeGenerator.GenerateNIN(),
                            Created = TM.Now,
                            CreatedBy = users[0].Id,
                            Updated = TM.Now,
                            UpdatedBy = users[0].Id,
                        },
                        Created = TM.Now,
                        CreatedBy = users[0].Id,
                        Updated = TM.Now,
                        UpdatedBy = users[0].Id,
                    };

                    if (userNo == 2) juser1.UserName = "grp";
                    juser1.Email = juser1.UserName + "@gmail.com";
                    juser1.UserProfile.UserId = juser1.Id;

                    users.Add(juser1);

                    if (UseEntityFramework)
                    {
                        db.Users.Add(juser1);
                    }
                    else
                    {
                        c.Users.Add(juser1);

                        writer.WriteLine(Helpers.ToInsertSql(juser1, lookForIdProperty: false));
                        writer.WriteLine(Helpers.ToInsertSql(juser1.Roles.Select(r => new UserRole { UserId = juser1.Id, RoleId = r.Id }).Single(), lookForIdProperty: false));
                        writer.WriteLine(Helpers.ToInsertSql(juser1.UserProfile, lookForIdProperty: false));
                    }

                    for (var ug = 0; ug < 5; ug++)
                    {
                        var juser2 = new User
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = "user_" + ((++userNo) + "").PadLeft(2, '0'),
                            PasswordHash = users[0].PasswordHash,
                            CollegeId = c.Id,

                            SecurityStamp = users[0].SecurityStamp,
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            ForcePasswordChange = false,
                            IsActivated = true,
                            Roles = roles.Where(r => r.Name == ArfUserRole.PrincipalResearcher.ToString()).ToList(),
                            UserProfile = new UserProfile
                            {
                                FirstName = cmps.FirstName,
                                MiddleName = cmps.SecondName,
                                LastName = cmps.LastName,
                                Gender = gender == 'M' ? false : true,
                                NationalId = FakeGenerator.GenerateNIN(),
                                Created = TM.Now,
                                CreatedBy = users[0].Id,
                                Updated = TM.Now,
                                UpdatedBy = users[0].Id,
                            },
                            Created = TM.Now,
                            CreatedBy = users[0].Id,
                            Updated = TM.Now,
                            UpdatedBy = users[0].Id,
                        };

                        if (userNo == 3) juser2.UserName = "emp";

                        juser2.Email = juser2.UserName + "@gmail.com";
                        juser2.UserProfile.UserId = juser2.Id;
                        users.Add(juser2);

                        if (UseEntityFramework)
                        {

                            db.Users.Add(juser2);
                        }
                        else
                        {
                            c.Users.Add(juser2);


                            writer.WriteLine(Helpers.ToInsertSql(juser2, lookForIdProperty: false));
                            writer.WriteLine(Helpers.ToInsertSql(juser2.Roles.Select(r => new UserRole { UserId = juser2.Id, RoleId = r.Id }).Single(), lookForIdProperty: false));
                            writer.WriteLine(Helpers.ToInsertSql(juser2.UserProfile, lookForIdProperty: false));
                        }
                    }


                }

                if (UseEntityFramework)
                {
                    db.Users.Add(user1);
                }
                else
                {
                    c.Users.Add(user1);

                    writer.WriteLine(Helpers.ToInsertSql(user1, lookForIdProperty: false));
                    writer.WriteLine(Helpers.ToInsertSql(user1.Roles.Select(r => new UserRole { UserId = user1.Id, RoleId = r.Id }).Single(), lookForIdProperty: false));
                    writer.WriteLine(Helpers.ToInsertSql(user1.UserProfile, lookForIdProperty: false));
                }


                i++;
                j++;
            }

            if (UseEntityFramework)
            {
                db.SaveChanges();
            }
            else
            {
                FlushWriter();
            }

            if (UseEntityFramework)
            {
                db.SaveChanges();
            }
            else
            {
                FlushWriter();
            }
        }

        public static void FlushWriter()
        {
            writer.WriteLine("GO");
            writer.Close();

            writer = new StreamWriter(OUTPUT_FOLDER + OUTPUT_FILE_NAME, true, Encoding.UTF8);
        }
    }

    public class Helpers
    {
        public static string GeneratePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public static string GetUniversityType(int? type)
        {
            if (type == null) return null;

            switch (type)
            {
                case 1:
                    return "GOVERNMENT";

                case 2:
                    return "PRIVATE";

                case 3:
                    return "SACM";

                case 4:
                    return "OTHERS";

                default:
                    return null;
            }
        }

        public static string CleanStr(string str)
        {
            if (str == null) return null;
            return str.Trim().Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
        }

        public static bool IsShittyName(string str)
        {
            if (str == null) return false;
            str = str.Replace(" ", "").Replace("-", "").Trim();

            if (str.StartsWith("?") && str.EndsWith("?")) return true;
            return false;
        }

        public static string ToInsertSql(object obj, string schema = "dbo", bool lookForIdProperty = true)
        {
            var dict = new SortedDictionary<string, string>();

            Type type = obj.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(type.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                var propValue = prop.GetValue(obj, null);

                if (lookForIdProperty && prop.Name.ToLower() == "id")
                {
                    if (propValue is int || propValue is short || propValue is long)
                    {
                        continue;
                    }
                    else if (propValue is Guid)
                    {
                        if (((Guid)propValue) == Guid.Empty)
                        {
                            dict.Add(prop.Name, "newid()");
                            continue;
                        }
                    }
                    else if (propValue is string)
                    {
                        Guid value = Guid.Empty;
                        if (string.IsNullOrEmpty((string)propValue) || (Guid.TryParse((string)propValue, out value) && value == Guid.Empty))
                        {
                            dict.Add(prop.Name, "newid()");
                            continue;
                        }
                    }
                }

                if (propValue is string)
                {
                    dict.Add(prop.Name, "N'" + ((string)propValue).Replace("'", "''") + "'");
                }
                else if (propValue is DateTime)
                {
                    if (((DateTime)propValue) < new DateTime(1900, 1, 1))
                        dict.Add(prop.Name, "'" + new DateTime(1900, 1, 1).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "'");
                    else
                        dict.Add(prop.Name, "'" + ((DateTime)propValue).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "'");
                }
                else if (propValue is DateTime?)
                {
                    if (propValue == null)
                        dict.Add(prop.Name, "null");
                    else if (((DateTime?)propValue) < new DateTime(1900, 1, 1))
                        dict.Add(prop.Name, "'" + new DateTime(1900, 1, 1).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "'");
                    else
                        dict.Add(prop.Name, "'" + ((DateTime)propValue).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "'");
                }
                else if (propValue is bool)
                {
                    dict.Add(prop.Name, (((bool)propValue) ? "1" : "0"));
                }
                else if (propValue is bool?)
                {
                    if (propValue == null)
                        dict.Add(prop.Name, "null");
                    else
                        dict.Add(prop.Name, (((bool?)propValue).Value ? "1" : "0"));
                }
                else if (propValue is int || propValue is short || propValue is long)
                {
                    dict.Add(prop.Name, "" + propValue.ToString() + "");
                }
                else if (propValue is Guid)
                {
                    dict.Add(prop.Name, "'" + propValue.ToString().ToLower() + "'");
                }
                else if (propValue is Nullable)
                {
                    if (propValue == null)
                        dict.Add(prop.Name, "null");
                    else
                        dict.Add(prop.Name, "'" + propValue.ToString() + "'");
                }
                else if (IsSimple(prop.PropertyType))
                {
                    if (propValue == null)
                        dict.Add(prop.Name, "null");
                    else
                        dict.Add(prop.Name, "'" + propValue.ToString() + "'");
                }
            }

            var tableName = type.Name + "s";
            if (tableName.ToLower().EndsWith("ys") && !tableName.ToLower().EndsWith("journeys")) tableName = tableName.Substring(0, tableName.Length - 2) + "ies";

            var statement = string.Format("INSERT INTO [{0}].[{1}] (", schema, tableName);

            var columns = dict.Keys.ToArray();
            for (var i = 0; i < columns.Length; i++)
            {
                statement += "[" + columns[i] + "]";
                if (i < columns.Length - 1)
                    statement += ", ";
                else
                    statement += ") VALUES (";
            }

            var values = dict.Values.ToArray();
            for (var i = 0; i < values.Length; i++)
            {
                statement += values[i];
                if (i < values.Length - 1)
                    statement += ", ";
                else
                    statement += ");";
            }

            return statement;
        }

        private static bool IsSimple(Type type)
        {
            return type.IsPrimitive || type.Equals(typeof(string));
        }
    }

    public class UserRole

    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}