﻿using System.Web.Mvc;
using Trackyt.Core.Services;
using Web.Models;

namespace Web.Controllers
{
    public class LoginController : Controller
    {
        private IAuthenticationService _authentication;

        public LoginController(IAuthenticationService authentication)
        {
            _authentication = authentication;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if(ModelState.IsValid) 
            {
                if (_authentication.Authenticate(model.Email, model.Password))
                {
                    return Redirect(returnUrl ?? "~/User/Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            return View("Index", model);
        }

    }
}