using ARF.Core;
using System;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.App_Start
{
    public sealed class NonceFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            return;
            var context = filterContext.HttpContext.GetOwinContext();
            var controller = HttpContextManager.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();
            var action = HttpContextManager.Current.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
            var rng = new RNGCryptoServiceProvider();
            var nonceBytes = new byte[32];
            rng.GetBytes(nonceBytes);
            var nonce = Convert.ToBase64String(nonceBytes);
            context.Set("ScriptNonce", nonce);

            if (action == "publicationdetails" && controller == "publications")
            {
                context.Response.Headers.Add("Content-Security-Policy",
                    new[] { string.Format("script-src 'unsafe-eval' 'self' 'nonce-{0}' https://www.google.com https://ajax.googleapis.com https://oss.maxcdn.com https://www.googletagmanager.com https://v2.zopim.com https://use.fontawesome.com;", nonce) });
            }
            else
            {
                context.Response.Headers.Add("Content-Security-Policy",
                    new[] { string.Format("script-src 'self' 'nonce-{0}' https://www.google.com https://ajax.googleapis.com https://oss.maxcdn.com https://www.googletagmanager.com https://v2.zopim.com https://use.fontawesome.com;", nonce) });
            }
        }


        public void OnActionExecuting2(AuthorizationContext filterContext)
        {
            return;
            var context = filterContext.HttpContext.GetOwinContext();
            var controller = HttpContextManager.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();
            var action = HttpContextManager.Current.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
            var rng = new RNGCryptoServiceProvider();
            var nonceBytes = new byte[32];
            rng.GetBytes(nonceBytes);
            var nonce = Convert.ToBase64String(nonceBytes);
            context.Set("ScriptNonce", nonce);

            if (action == "publicationdetails" && controller == "publications")
            {
                context.Response.Headers.Add("Content-Security-Policy",
                    new[] { string.Format("script-src 'unsafe-eval' 'self' 'nonce-{0}' https://www.google.com https://ajax.googleapis.com https://oss.maxcdn.com https://www.googletagmanager.com https://v2.zopim.com https://use.fontawesome.com;", nonce) });
            }
            else
            {
                context.Response.Headers.Add("Content-Security-Policy",
                    new[] { string.Format("script-src 'self' 'nonce-{0}' https://www.google.com https://ajax.googleapis.com https://oss.maxcdn.com https://www.googletagmanager.com https://v2.zopim.com https://use.fontawesome.com;", nonce) });
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}