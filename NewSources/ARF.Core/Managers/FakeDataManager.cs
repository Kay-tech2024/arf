using ARF.TimeMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARF.Core.Managers
{
    public class FakeDataManager : AbstractManager
    {
        public static void Init()
        {
            var db = Utils.db;

            if (db.Categories.Count(c => c.Level == 1) == 0)
            {
                for (var i = 1; i <= 3; i++)
                {
                    var cat = new Data.DbModels.Category
                    {
                        NameArabic = "صنف " + i,
                        NameEnglish = "Category " + i,
                        Created = TM.Now,
                        Updated = TM.Now,
                        CreatedBy = Utils.SystemAdminUserId,
                        UpdatedBy = Utils.SystemAdminUserId,
                        Level = 1,
                    };

                    db.Categories.Add(cat);

                    db.SaveChanges();

                    for (var k = 1; k <= 3; k++)
                    {
                        db.Categories.Add(new Data.DbModels.Category
                        {
                            NameArabic = "صنف فرعي" + i + "/" + k,
                            NameEnglish = "Subcategory " + i + "/" + k,
                            Created = TM.Now,
                            Updated = TM.Now,
                            CreatedBy = Utils.SystemAdminUserId,
                            UpdatedBy = Utils.SystemAdminUserId,
                            Level = 2,
                            ParentId = cat.Id

                        });
                    }

                    db.SaveChanges();
                }
            }


            var colleges = db.Colleges.ToList();

            foreach (var col in colleges)
            {
                var deps = col.Departments.ToList();

                for (var i = deps.Count; i < 3; i++)
                {
                    db.Departments.Add(new Data.DbModels.Department
                    {
                        NameArabic = "قسم " + i,
                        NameEnglish = "Department " + i,
                        Created = TM.Now,
                        Updated = TM.Now,
                        CreatedBy = Utils.SystemAdminUserId,
                        UpdatedBy = Utils.SystemAdminUserId,
                        CollegeId = col.Id,

                    });
                }

                db.SaveChanges();


                //deps = col.Departments.ToList();

                //foreach(var dep in deps)
                //{
                //    var prios = dep.DepartmentPriorities.ToList();

                //    for (var i = prios.Count; i < 3; i++)
                //    {
                //        db.DepartmentPriorities.Add(new Data.DbModels.DepartmentPriority
                //        {
                //            NameArabic = "أولوية " + i,
                //            NameEnglish = "Priority " + i,
                //            Created = TM.Now,
                //            Updated = TM.Now,
                //            CreatedBy = Utils.SystemAdminUserId,
                //            UpdatedBy = Utils.SystemAdminUserId,
                //            DepartmentId = dep.Id,

                //        });
                //    }

                //    db.SaveChanges();

                //}
            }

        }
    }
}
