﻿using System.Linq;
using System.Web.Mvc;
using Trackyt.Core.DAL.DataModel;
using Trackyt.Core.DAL.Extensions;
using Trackyt.Core.DAL.Repositories;
using Trackyt.Core.Security;

namespace Web.Controllers
{
    public class RegistrationController : Controller
    {
        private IUsersRepository _repository;
        private IFormsAuthentication _forms;

        public RegistrationController(IUsersRepository repository, IFormsAuthentication forms)
        {
            _repository = repository;
            _forms = forms;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Models.RegisterUserModel model)
        {
            if (ModelState.IsValid)
            {
                //check if used already registered
                if (IsAlreadyRegistered(model))
                {
                    ModelState.AddModelError("", "Sorry, user with such email already exist. Please register with different email.");
                    return View("Index", model);                    
                }

                return CreateNewUserAndRedirectToDashboard(model.Email, model.Password);
            }

            return View("Index", model);
        }

        [HttpGet]
        public ActionResult QuickStart()
        {
            var email = GenerateEmail();
            var password = GeneratePassword();

            return CreateNewUserAndRedirectToDashboard(email, password, true);
        }

        // Helpers

        private bool IsAlreadyRegistered(Models.RegisterUserModel model)
        {
            return _repository.Users.WithEmail(model.Email) != null;
        }

        private string GeneratePassword()
        {
            return _forms.GeneratePassword();
        }

        private string GenerateEmail()
        {
            var id = GetLastId();
            var email = "temp" + id + "@trackyt.net";
            
            return email;
        }

        private int GetLastId()
        {
            return _repository.Users.Count();
        }

        private RedirectResult CreateNewUserAndRedirectToDashboard(string email, string password, bool temp = false)
        {
            var user = new User
            {
                Email = email,
                Password = password,
                Temp = temp
            };

            _repository.SaveUser(user);
            _forms.SetAuthCookie(email, false);

            return Redirect("~/User/Dashboard");
        }
    }
}