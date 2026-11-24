using ARF.Auth;
using ARF.Data;
using ARF.Data.DbModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace ARF.TimeMachine
{

    public static class TM
    {
 
        public static DateTime Now
        {
            get
            {
                return TmUtils.CurrentDateTimeStatic;
            }
        }
    }
}
