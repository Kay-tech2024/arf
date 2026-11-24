using ARF.Core;
using ARF.Core.Managers;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using ARF.Web.App_Start;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ARF.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest()
        {
            if (Utils.IsLocalhost)
            {
                FakeDataManager.Init();
            }
        }

        protected void Application_EndRequest()
        {
            if (Request.Cookies.Count > 0)
            {
                foreach (string s in Request.Cookies.AllKeys)
                {
                    if (s == "id")
                    {
                        Request.Cookies["id"].HttpOnly = true;
                    }
                    //if (!Utils.IsLocalhost)
                    //{
                    //    Response.Cookies[s].Secure = true;
                    //}

                }
            }

            if (this.Context != null && this.Context.AllErrors != null && this.Context.AllErrors.Length > 0)
            {
                foreach (var ex in this.Context.AllErrors)
                {
                    Logger.LogExceptionAsync(ex);
                }
            }
        }

        protected void Application_PreSendRequestHeaders(object source, EventArgs e)
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        private void Session_Start(object sender, EventArgs e)
        {
            string CookieHeaders = HttpContext.Current.Request.Headers["Cookie"];

            if ((null != CookieHeaders) && (CookieHeaders.IndexOf("ASP.NET_SessionId") >= 0))
            {
                // It is existing visitor, but ASP.NET session is expired
            }
            else
            {
                // It is a new visitor, session was not created before
            }
        }

        private bool IsUserAllowedToSeeMiniProfilerUI(HttpRequest httpRequest)
        {
            return true;
        }

        protected void Application_Start()
        {
            //HangfireBootstrapper.Instance.Start();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            using (var db = new ArfDbEntities())
            {
                try
                {
                    {
                        var rolesEnums = Enum.GetValues(typeof(ArfUserRole));
                        var roles = db.Roles.ToList();

                        foreach (var r in rolesEnums)
                        {
                            if (roles.Count(x => x.Name == r.ToString()) > 0) continue;

                            var role = new Role
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = r.ToString()
                            };

                            db.Roles.Add(role);
                        }

                        db.SaveChanges();
                    }

                    // filling out fields
                    {
                        var data = db.RequestDatas.Where(f => f.Name.StartsWith("FIELD_") && f.FormField == null).ToList();

                        foreach (var d in data)
                        {
                            var id = int.Parse(d.Name.Substring("FIELD_".Length));
                            d.FieldId = id;
                        }

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                }

                try
                {
                    {
                        var referees = db.Referees.ToList();

                        var imamUReserchers = db
                            .Users
                            .Where(u => u.Researchers.Count(r => r.UniversityId == 1) > 0) // belongs to Imam university
                            .Where(u => u.Roles.Count(r => r.Name == ArfUserRole.PrincipalResearcher.ToString()) > 0)
                            .Where(u => u.UserExtraRoles.Count(x => x.Role.Name == ArfUserRole.Referee.ToString()) == 0)
                            .ToList();

                        var refereeRoleId = db.Roles.FirstOrDefault(r => r.Name == ArfUserRole.Referee.ToString()).Id;

                        foreach (var u in imamUReserchers)
                        {
                            var extraRole = new UserExtraRole
                            {
                                UserId = u.Id,
                                RoleId = refereeRoleId,
                                IsActive = true,
                                Created = TM.Now,
                                Updated = TM.Now,
                                CreatedBy = Utils.SystemAdminUserId,
                                UpdatedBy = Utils.SystemAdminUserId,

                            };

                            db.UserExtraRoles.Add(extraRole);

                            db.SaveChanges();

                            var researcher = u.Researchers.FirstOrDefault();
                            var referee = referees.FirstOrDefault(r => r.Email == u.Email);
                            if (referee == null)
                            {

                                var userReferee = new Referee
                                {
                                    UserId = u.Id,
                                    Email = researcher.Email,
                                    MobileNo = researcher.MobileNo,
                                    FirstName = researcher.FirstName,
                                    SecondName = researcher.SecondName,
                                    ThirdName = researcher.ThirdName,
                                    LastName = researcher.LastName,
                                    UniversityId = researcher.UniversityId,
                                    CollegeId = researcher.CollegeId,
                                    Address = researcher.Address,
                                    CityId = researcher.CityId,
                                    CountryId = researcher.CountryId,
                                    NationalityId = researcher.NationalityId,
                                    MajorId = researcher.MajorId,
                                    Qualification = researcher.Qualification,
                                    RegionId = researcher.RegionId,
                                    IsDeleted = false,


                                    Created = TM.Now,
                                    Updated = TM.Now,
                                    CreatedBy = Utils.SystemAdminUserId,
                                    UpdatedBy = Utils.SystemAdminUserId,
                                };
                                db.Referees.Add(userReferee);

                                db.SaveChanges();
                            }
                        }

                        db.SaveChanges();
                    }


                }
                catch (Exception ex)
                {
                }



                // adding history for requests without history :)

                try
                {
                    {

                        var requestsWithoutHistory = db
                            .Requests
                            .Where(u => u.Status > 0 && u.Status != (long)RequestStatus.NotSubmitted && u.Type == (byte)RequestType.FundingRequest)
                            .Where(u => u.RequestHistoryLogs.Count() == 0)
                            .ToList();


                        foreach (var request in requestsWithoutHistory)
                        {
                            var date = request.UpdatedBy == request.CreatedBy ? request.Updated : request.Created;


                            db.RequestHistoryLogs.Add(new RequestHistoryLog
                            {
                                RequestId = request.Id,
                                Url = "",
                                Status = request.Status,
                                Action = (byte)(RequestAction.Submit),
                                UserAgent = "",
                                IpAddress = "",
                                Role = (byte)ArfUserRole.PrincipalResearcher,
                                IsImpersonating = false,
                                CollegeId = request.CollegeId,
                                Created = date,
                                CreatedBy = request.CreatedBy,

                            });

                            db.SaveChanges();

                        }

                        db.SaveChanges();
                    }


                }
                catch (Exception ex)
                {
                }


                // filling out new columns for request history logs
                try
                {
                    {

                        var requestsWithoutTitles = db
                            .Requests
                            .Where(u => u.Title == null && u.Type == (byte)RequestType.FundingRequest)
                            .ToList();


                        foreach (var request in requestsWithoutTitles)
                        {
                            try
                            {
                                db.Entry(request).State = System.Data.Entity.EntityState.Deleted;

                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {

                            }

                        }

                        db.SaveChanges();
                    }


                }
                catch (Exception ex)
                {
                }


                // delete empty requests
                try
                {
                    {

                        var logs = db
                            .RequestHistoryLogs
                            .Where(u => u.Role == null)
                            .ToList();


                        foreach (var l in logs)
                        {
                            l.Role = (int)Enum.Parse(typeof(ArfUserRole), l.CreatedByUser.Roles.FirstOrDefault().Name);
                            l.IsImpersonating = false;
                            l.CollegeId = l.CreatedByUser.CollegeId;

                            db.SaveChanges();

                        }

                        db.SaveChanges();
                    }


                }
                catch (Exception ex)
                {
                }

                if (Constants.EnableCoresearchersNotifications)
                {
                    try
                    {
                        {
                            // correcting coresearchers for funding requests

                            var pendingFundingRequests = db.Requests
                                .Where(r => r.Type == (byte)RequestType.FundingRequest)
                                .Where(r => r.Status == (long)RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies)
                                .Where(r => r.RequestCoresearchers.Count(rr => rr.Researcher.UserId != r.CreatedBy && rr.DidResearcherAgree == null && rr.IsDeleted == false) > 0)
                                .ToList();

                            foreach (var r in pendingFundingRequests)
                            {
                                r.Status = (long)RequestStatus.FundingRequestPendingCoresearchers;

                                foreach (var rr in r.RequestCoresearchers)
                                {
                                    if (rr.Notified == null)
                                    {
                                        var key = "";
                                        var err = "";
                                        Utils.SendCoreseacherNotification(rr, false, out err, out key);
                                        rr.Notified = TM.Now;
                                    }
                                }

                                db.SaveChanges();
                            }

                            db.SaveChanges();
                        }

                    }
                    catch (Exception ex)
                    {
                    }

                }
                else
                {
                    try
                    {
                        {
                            // correcting coresearchers for funding requests

                            var pendingFundingRequests = db.Requests
                                .Where(r => r.Type == (byte)RequestType.FundingRequest)
                                .Where(r => r.Status == (long)RequestStatus.FundingRequestPendingCoresearchers)
                                .ToList();

                            foreach (var r in pendingFundingRequests)
                            {
                                r.Status = (long)RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies;

                                db.SaveChanges();
                            }

                            db.SaveChanges();
                        }


                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

        }

        private void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var path = Request.RawUrl;

            if (path.ToLower().StartsWith(Request.ApplicationPath.ToLower() + " / content") || path.ToLower().StartsWith(Request.ApplicationPath.ToLower() + "/bundles"))
                return;

            Utils.SetUserLocale(Context);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            //HangfireBootstrapper.Instance.Stop();
        }
    }
}