using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ARF.Web.App_Start.Code
{
    public class LocalizedDisplayAttribute : DisplayNameAttribute
    {
        private readonly string resourcekey;

        public LocalizedDisplayAttribute(string resourceId)
            : base(GetMessageFromResource(resourceId))
        {

        }

        public static string GetMessageFromResource(string resourceId)
        {
            return LangHelpers.Instance[resourceId];
        }
    }
}