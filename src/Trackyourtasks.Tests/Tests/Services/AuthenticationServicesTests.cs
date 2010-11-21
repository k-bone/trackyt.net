﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using Trackyourtasks.Core.DAL.Repositories;
using Web.Infrastructure.Security;
using Web.Services;
using Trackyourtasks.Core.DAL.DataModel;

namespace Trackyourtasks.Core.Tests.Tests.Services
{
    [TestFixture]
    public class AuthenticationServicesTests
    {
        [Test]
        public void Smoke()
        {
            //arrange
            var users = new Mock<IUsersRepository>();
            var forms = new Mock<IFormsAuthentication>();
            var service = new AuthenticationService(users.Object, forms.Object);

            //act / post
            Assert.That(service, Is.Not.Null);
        }

        [Test]
        public void SuccessAuthentication()
        {
            //arrange
            var users = new Mock<IUsersRepository>();
            var forms = new Mock<IFormsAuthentication>();
            var service = new AuthenticationService(users.Object, forms.Object);

            users.Setup(u => u.Users).Returns(new List<User> { new User { Email = "ok@a.com", Password = "111", Id = 1, Temp = false } }.AsQueryable());

            //act
            var auth = service.Authenticate("ok@a.com", "111");

            //post
            Assert.That(auth, Is.True);
            forms.Verify(f => f.SetAuthCookie("ok@a.com", false));
        }

        [Test]
        public void FailedAuthentication()
        {
            //arrange
            var users = new Mock<IUsersRepository>();
            var forms = new Mock<IFormsAuthentication>();
            var service = new AuthenticationService(users.Object, forms.Object);

            users.Setup(u => u.Users).Returns(new List<User> { new User { Email = "ok@a.com", Password = "111", Id = 1, Temp = false } }.AsQueryable());
            forms.Setup(f => f.SetAuthCookie("", false)).Throws(new Exception("SetAuthCookie must not be called for failed authentication"));
            
            //act
            var auth = service.Authenticate("fail@a.com", "111");

            //post
            Assert.That(auth, Is.False);
        }

        [Test]
        public void FailedAuthentication_WrongPassword()
        {
            //arrange
            var users = new Mock<IUsersRepository>();
            var forms = new Mock<IFormsAuthentication>();
            var service = new AuthenticationService(users.Object, forms.Object);

            users.Setup(u => u.Users).Returns(new List<User> { new User { Email = "ok@a.com", Password = "111", Id = 1, Temp = false } }.AsQueryable());
            forms.Setup(f => f.SetAuthCookie("", false)).Throws(new Exception("SetAuthCookie must not be called for failed authentication"));

            //act
            var auth = service.Authenticate("ok@a.com", "1111");

            //post
            Assert.That(auth, Is.False);
        }
    }
}
