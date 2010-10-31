﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Infrastructure.Security
{
    public class TrackyAuthorizeAttribute : AuthorizeAttribute
    {
        public string LoginArea { set; get; }
        public string LoginController { set; get; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                if (LoginController != null)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            { "area", LoginArea ?? "Public" },
                            { "controller", LoginController },
                            { "ReturnUrl", filterContext.HttpContext.Request.RawUrl }
                        }
                    );
                }
            }
        }
    }
}