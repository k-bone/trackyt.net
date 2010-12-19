﻿using System.Web.Mvc;
using Trackyt.Core.DAL.Extensions;
using Trackyt.Core.DAL.Repositories;
using Trackyt.Core.Security;
using Web.Infrastructure.Helpers;
using Web.Infrastructure.Security;

namespace Web.Areas.User.Controllers
{
    [TrackyAuthorizeAttribute(LoginController = "Login")]
    public class DashboardController : Controller
    {
        private IUsersRepository _users;
        private IFormsAuthentication _authentication;
        private IPathHelper _path;

        public DashboardController(IUsersRepository repository, IFormsAuthentication authentication, IPathHelper path)
        {
            _users = repository;
            _authentication = authentication;
            _path = path;
        }

        public ActionResult Index()
        {
            var userEmail = _authentication.GetLoggedUser();
            var user = _users.Users.WithEmail(userEmail);
            //ViewData["UserId"] = user.Id;;
            ViewData["Api"] = _path.VirtualToAbsolute("~/API/v1");
            ViewData["Email"] = userEmail;
            ViewData["ApiToken"] = user.ApiToken;

            return View();
        }

    }
}
