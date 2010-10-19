﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.Tracky.Controllers;
using Moq;
using Trackyourtasks.Core.DAL.Repositories;
using Web.Infrastructure.Security;
using Trackyourtasks.Core.DAL.DataModel;
using System.Web.Mvc;
using Web.Infrastructure.Helpers;

namespace Trackyourtasks.Core.Tests.Tests.Controllers.Tracky
{
    [TestFixture]
    public class DashboardTests
    {
        [Test]
        public void Smoke()
        {
            //arrange
            var users = new Mock<IUsersRepository>();
            var forms = new Mock<IFormsAuthentication>();
            var path = new Mock<IPathHelper>();
            var dashboard = new DashboardController(users.Object, forms.Object, path.Object);

            //act/post
            Assert.That(dashboard, Is.Not.Null);
        }

        [Test]
        public void Index_Get_Initialize_UserId()
        {
            //arrange
            var users = new Mock<IUsersRepository>();
            var forms = new Mock<IFormsAuthentication>();
            var path = new Mock<IPathHelper>();

            forms.Setup(f => f.GetLoggedUser()).Returns("logged@tracky.net");
            users.Setup(u => u.GetUsers()).Returns((new List<User> { new User { Id = 100, Email = "logged@tracky.net" } }).AsQueryable());

            var dashboard = new DashboardController(users.Object, forms.Object, path.Object);

            //act
            var result = dashboard.Index() as ViewResult;

            //post
            Assert.That(result.ViewData["UserId"], Is.EqualTo(100));
        }

        [Test]
        public void Index_Get_Initialize_Api_Path()
        {
            //arrange
            var users = new Mock<IUsersRepository>();
            var forms = new Mock<IFormsAuthentication>();
            var path = new Mock<IPathHelper>();

            forms.Setup(f => f.GetLoggedUser()).Returns("logged@tracky.net");
            users.Setup(u => u.GetUsers()).Returns((new List<User> { new User { Id = 100, Email = "logged@tracky.net" } }).AsQueryable());
            path.Setup(p => p.VirtualToAbsolute(It.IsAny<string>())).Returns((string v) => v);

            var dashboard = new DashboardController(users.Object, forms.Object, path.Object);

            //act
            var result = dashboard.Index() as ViewResult;

            //post
            Assert.That(result.ViewData["Api"], Is.EqualTo("~/API/v1"));
        }

    }
}