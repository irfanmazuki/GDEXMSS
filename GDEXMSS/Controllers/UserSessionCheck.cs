﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GDEXMSS.Controllers
{
    public class UserSessionCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session != null && session["Email"] == null && session["Role"] != "User")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                                { "Controller", "User" },
                                { "Action", "Login" },
                                { "error", "login" }
                                });
            }
        }
    }
}