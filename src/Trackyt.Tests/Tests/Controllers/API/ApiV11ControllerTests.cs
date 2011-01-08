﻿using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Trackyt.Core.DAL.Repositories;
using Trackyt.Core.Services;
using Web.API.v11.Controllers;
using Web.API.v11.Model;
using System;
using Web.Infrastructure.Exceptions;

namespace Trackyt.Core.Tests.Tests.Controllers.API
{
    // API v.1.1 is covered with integration tests (/Scripts/Tests/api/tests.api.v11.js)

    // TODO: API unit tests for all controller methods
    // TODO: API improve test by using IDateTimeProvider to be able to mock it and use mock instead of DateTime.UtcNow (test performance issue)
    [TestFixture]
    public class ApiV11ControllerTests
    {
        [Test]
        public void Smoke()
        {
            // assert
            var repository = new Mock<ITasksRepository>();
            var mapper = new Mock<IMappingEngine>();
            var service = new Mock<IApiService>();
            var api = new ApiV11Controller(service.Object, repository.Object, mapper.Object);

            // act / assert
            Assert.That(api, Is.Not.Null);
        }

        [Test]
        public void Start_TaskStarted_StartedTimeInitialized()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act 
            api.Start(token, 1);

            // assert
            var task = ApiTestsCommonSetup.SubmittedTasks[0];
            Assert.That(task.StartedDate, Is.Not.Null, "stated date has not been set");
            Assert.That(task.StoppedDate, Is.Null, "stopped date has not been reset to null");
            Assert.That(task.Status, Is.EqualTo(1), "task status is not <started>");
        }

        [Test]
        public void Stop_StatedTaskStopped_StoppedInitialized()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act 
            api.Start(token, 1);
            Thread.Sleep(1000);
            api.Stop(token, 1);

            // assert
            var task = ApiTestsCommonSetup.SubmittedTasks[0];
            Assert.That(task.StartedDate, Is.Not.Null, "stated date has not been set");
            Assert.That(task.StoppedDate, Is.Not.Null, "stopped date has not been set");
            Assert.That(task.Status, Is.EqualTo(2), "task status is not <stopped>");
        }

        [Test]
        public void Stop_StatedTaskStopped_ActualWorkUpdated()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act 
            api.Start(token, 1);
            Thread.Sleep(1000);
            api.Stop(token, 1);

            // assert
            var task = ApiTestsCommonSetup.SubmittedTasks[0];
            Assert.That(task.ActualWork, Is.EqualTo(1), "one second have to be stored as actual work");
        }

        [Test]
        public void Start_IfStoppedTaskIsStartedAgain_StartedDateUpdated()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act 
            api.Start(token, 1);
            Thread.Sleep(1000);
            api.Stop(token, 1);
            Thread.Sleep(1000);
            api.Start(token, 1);

            // assert
            var task = ApiTestsCommonSetup.SubmittedTasks[0];
            Assert.That(task.StartedDate, Is.Not.Null, "stated date has not been set");
            Assert.That(task.StoppedDate, Is.Null, "stopped date has not been reset to null");
            Assert.That(task.Status, Is.EqualTo(1), "task status is not <started>");
        }

        [Test]
        public void Start_IfStoppedTaskIsStartedAgain_ActualWorkStillTheSame()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act 
            api.Start(token, 1);
            Thread.Sleep(1000);
            api.Stop(token, 1);
            Thread.Sleep(1000);
            api.Start(token, 1);

            // assert
            var task = ApiTestsCommonSetup.SubmittedTasks[0];
            Assert.That(task.ActualWork, Is.EqualTo(1), "one second have to be stored as actual work");
        }

        [Test]
        public void Stop_IfStoppedTaskIsStartedAgainAndStopped_StoppedUpdated()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act 
            api.Start(token, 1);
            Thread.Sleep(1000);
            api.Stop(token, 1);
            api.Start(token, 1);
            Thread.Sleep(1000);
            api.Stop(token, 1);

            // assert
            var task = ApiTestsCommonSetup.SubmittedTasks[0];
            Assert.That(task.StoppedDate, Is.Not.Null, "stopped date initialized");
        }

        [Test]
        public void Stop_IfStoppedTaskIsStartedAgainAndStopped_ActualWorkUpdated()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act 
            api.Start(token, 1);
            Thread.Sleep(1000);
            api.Stop(token, 1);
            api.Start(token, 1);
            Thread.Sleep(1000);
            api.Stop(token, 1);

            // assert
            var task = ApiTestsCommonSetup.SubmittedTasks[0];
            Assert.That(task.ActualWork, Is.EqualTo(2), "two seconds have to be stored as actual work");
        }

        [Test]
        public void All_IfTaskStatusIsNone_SpentEqualsToActualWork()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act 
            var results = api.All(token) as JsonResult;
            dynamic data = results.Data;

            var tasksList = data.data.tasks as IList<TaskDescriptor>;
            Assert.That(tasksList, Is.Not.Null);
            Assert.That(tasksList[0].spent, Is.EqualTo(0), "spend equal to task ActualWork field");
        }

        [Test]
        public void All_IfTaskStatusIsStopped_SpentEqualsToActualWork()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act 
            var results = api.All(token) as JsonResult;
            dynamic data = results.Data;

            var tasksList = data.data.tasks as IList<TaskDescriptor>;
            Assert.That(tasksList, Is.Not.Null);
            Assert.That(tasksList[2].spent, Is.EqualTo(20), "spend equal to task ActualWork field");
        }


        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckArguments_ApiTokenWrong()
        {
            // arrange
            var userId = 100;
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken("bad_token")).Returns(userId);

            // act 
            var results = api.All("bad_token") as JsonResult;
            dynamic data = results.Data;

            // assert
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Autenticate_EmailIsNull_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            // act
            api.Authenticate(null, "");
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Autenticate_PasswordIsNull_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            // act
            api.Authenticate("aa", null);
        }

        [Test]
        [ExpectedException(typeof(UserNotAuthorizedException))]
        public void Authenticate_ApiTokenNotLinkedToUser_ExceptionThrow()
        {
            // arrange
            var userId = 100;
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetApiToken("email", "password")).Returns((string)null);

            // act
            api.Authenticate("email", "token");
        }

        [Test]
        [ExpectedException(typeof(UserNotAuthorizedException))]        
        public void All_CheckAuthentication_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.All(token);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_CheckArgumentsBadToken_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Add("bad_token", "");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_CheckArgumentsBadDescription_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Add(token, "");
        }


        [Test]
        [ExpectedException(typeof(UserNotAuthorizedException))]
        public void Add_CheckAuthentication_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Add(token, "desc");
        }


        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Delete_CheckArgumentsBadToken_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Delete("bad_token", 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Delete_CheckArgumentsBadTaskId_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Delete(token, -1);
        }


        [Test]
        [ExpectedException(typeof(UserNotAuthorizedException))]
        public void Delete_CheckAuthentication_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Delete(token, 0);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Delete_TaskWithSuchIdDoesNotExist_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act
            api.Delete(token, 333);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Start_CheckArgumentsBadToken_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Start("bad_token", 0);
        }


        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Start_CheckArgumentsBadTaskId_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Start(token, -1);
        }

        [Test]
        [ExpectedException(typeof(UserNotAuthorizedException))]
        public void Start_CheckAuthentication_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Start(token, 0);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Start_TaskWithSuchIdDoesNotExist_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act
            api.Start(token, 333);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Stop_CheckArgumentsBadToken_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Stop("bad_token", 0);
        }


        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Stop_CheckArgumentsBadTaskId_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Stop(token, -1);
        }

        [Test]
        [ExpectedException(typeof(UserNotAuthorizedException))]
        public void Stop_CheckAuthentication_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.Stop(token, 0);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Stop_TaskWithSuchIdDoesNotExist_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

            // act
            api.Stop(token, 333);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void StartAll_CheckArgumentsBadToken_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.StartAll("bad_token");
        }

        [Test]
        [ExpectedException(typeof(UserNotAuthorizedException))]
        public void StartAll_CheckAuthentication_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.StartAll(token);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void StopAll_CheckArgumentsBadToken_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.StopAll("bad_token");
        }

        [Test]
        [ExpectedException(typeof(UserNotAuthorizedException))]
        public void StopAll_CheckAuthentication_ExceptionThrown()
        {
            // arrange
            var userId = 100;
            var token = "4a891b4d0bb22f83482f9fb5bafeb4b8";
            var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
            var mapper = ApiTestsCommonSetup.SetupMapper();
            var service = new Mock<IApiService>();

            var api = new ApiV11Controller(service.Object, repository.Object, mapper);

            service.Setup(s => s.GetUserIdByApiToken(token)).Returns(0);

            // act
            api.StopAll(token);
        }

        // TODO: API enable (correct) test then DateTimeProvider implmented
        //[Test]
        //public void All_IfTaskStatusIsStarted_SpentEqualsToActualWorkPlusTimeSpanBetweenStartedDateAndCurrentDate()
        //{
        //    // arrange
        //    var userId = 100;
        //    var repository = ApiTestsCommonSetup.SetupMockRepository(userId);
        //    var mapper = ApiTestsCommonSetup.SetupMapper();
        //    var service = new Mock<IApiService>();

        //    var api = new ApiV11Controller(service.Object, repository.Object, mapper);

        //    service.Setup(s => s.GetUserIdByApiToken(token)).Returns(userId);

        //    // act 
        //    var results = api.All(token) as JsonResult;
        //    dynamic data = results.Data;

        //    var difference = GetTimeDifference();

        //    var tasksList = data.data.tasks as IList<TaskDescriptor>;
        //    Assert.That(tasksList, Is.Not.Null);
        //    Assert.That(tasksList[1].spent, Is.EqualTo(20 + difference), "spend equal to task ActualWork field");
        //}

        //private static int GetTimeDifference()
        //{
        //    var difference = Convert.ToInt32(Math.Floor((DateTime.Now - ApiTestsCommonSetup.StartedDate).TotalSeconds));
        //    return difference;
        //}
    }
}
