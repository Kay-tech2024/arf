using ARF.Auth;
using ARF.Core;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;

[assembly: OwinStartupAttribute(typeof(ARF.Web.Startup))]

namespace ARF.Web
{
    public class Startup
    {
        public bool IsDebug { get { return ((CompilationSection)ConfigurationManager.GetSection(@"system.web/compilation")).Debug; } }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var options = new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = true,
            };

            GlobalConfiguration.Configuration.UseSqlServerStorage("ArfDB", options);

            app.UseHangfireServer();


            app.MapSignalR();







        }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            Database.SetInitializer<ApplicationDbContext>(null);
            Database.SetInitializer<ArfDbEntities>(null);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = ctx =>
                    {
                        var ret = Task.Run(() =>
                        {
                            var claim = ctx.Identity.Claims.Where(c => c.Type == "SESSION_ID").FirstOrDefault();

                            string sessionid = null;
                            if (claim != null)
                            {
                                sessionid = claim.Value;

                                var context = ctx.OwinContext.Get<ArfDbEntities>(typeof(ArfDbEntities).FullName);
                                if (context == null)
                                {
                                    context = new ArfDbEntities();
                                    ctx.OwinContext.Set<ArfDbEntities>(typeof(ArfDbEntities).FullName, context);
                                }

                                var session = context.UserSessions.Where(r => r.Sessionid == sessionid).FirstOrDefault();
                                if (session == null || (session.Locked != null && session.Locked.Value) || (session.Expires != null && session.Expires.Value < DateTime.UtcNow))
                                {
                                    if (ctx != null && ctx.Request != null && ctx.Request.Uri != null && ctx.Request.Uri.AbsoluteUri.Contains("ajax/heartbeat"))
                                    {

                                    }
                                    else
                                    {
                                        ctx.RejectIdentity();
                                        ctx.OwinContext.Authentication.SignOut(ctx.Options.AuthenticationType);
                                    }
                                }
                                else
                                {
                                    if (ctx != null && ctx.Request != null && ctx.Request.Uri != null && ctx.Request.Uri.AbsoluteUri.Contains("ajax/heartbeat"))
                                    {

                                    }
                                    else
                                    {
                                        var diff = (session.Expires == null ? DateTime.UtcNow.AddMinutes(30) : session.Expires.Value) - (session.Created == null ? DateTime.UtcNow : session.Created);

                                        var newEXPIRES = DateTime.UtcNow.AddMinutes(60);
                                        if (session.Expires == null || newEXPIRES > session.Expires)
                                        {
                                            session.Expires = newEXPIRES;
                                        }

                                        var url = ctx.OwinContext.Request.Uri.ToString();

                                        var controller = (string)null;
                                        var action = Utils.GetActionName(url, out controller);

                                        if (action == "Index" && !url.ToLower().Contains("getfile"))
                                        {
                                            session.LastIndexUrl = url;
                                        }

                                        session.Updated = TM.Now;
                                        context.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                ctx.RejectIdentity();
                            }
                        });
                        return ret;
                    }
                }
            });
        }
    }
}