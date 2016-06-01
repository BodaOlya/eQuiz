using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eQuiz.Repositories.Abstract;
using System.Data.Entity;
using System.Web.Mvc;
using eQuiz.Entities;
using AdminReviewController = eQuiz.Web.Areas.Admin.Controllers.ReviewController;
using Moq;

namespace eQuiz.Web.Tests.Areas.Admin.Controllers
{
    [TestClass]
    class ReviewControllerTest
    {
        private AdminReviewController _controller;

        [TestMethod]
        public void Test_GetStudentsList_Returns_JSONResult()
        {
            var users = new List<User>
            {
                new User
                {
                    FirstName = "John",
                    LastName = "Smith",
                    FatheName = "Jacob",
                    Email = "john@mail.com",
                    Id = 1,
                    Phone = "86786867867",
                    IsEmailConfirmed = true
                },
                new User
                {
                    FirstName = "Caleb",
                    LastName = "Smith",
                    FatheName = "Jacob",
                    Email = "calrb@mail.com",
                    Id = 2,
                    Phone = "86786867947",
                    IsEmailConfirmed = true
                },
                new User
                {
                    FirstName = "Jack",
                    LastName = "Smith",
                    FatheName = "Jacob",
                    Email = "jack@mail.com",
                    Id = 3,
                    Phone = "86782137867",
                    IsEmailConfirmed = true
                },
            };

            var userGroups = new List<UserGroup>
            {
                new UserGroup
                {
                    Id = 1,
                    Name = ".Net"
                },
                new UserGroup
                {
                    Id = 2,
                    Name = "Java"
                },
                new UserGroup
                {
                    Id = 3,
                    Name = "UX"
                },
            };

            var userToUserGroups = new List<UserToUserGroup>
            {
                new UserToUserGroup
                {
                    Id = 1,
                    GroupId = 1,
                    UserId = 2
                },
                new UserToUserGroup
                {
                    Id = 1,
                    GroupId = 2,
                    UserId = 1
                },
                new UserToUserGroup
                {
                    Id = 1,
                    GroupId = 3,
                    UserId = 3
                },
            };

            var quizPasses = new List<QuizPass>
            {
                new QuizPass
                {
                    Id = 1,
                    UserId = 1,
                    QuizId = 1,
                    StartTime = DateTime.Now
                },
                new QuizPass
                {
                    Id = 2,
                    UserId = 3,
                    QuizId = 2,
                    StartTime = DateTime.Now
                },
                new QuizPass
                {
                    Id = 3,
                    UserId = 2,
                    QuizId = 3,
                    StartTime = DateTime.Now
                },
            };

            var quizzes = new List<Quiz>
            {
                new Quiz
                {
                    Id = 1,
                    Name = ".NET Theory",
                },
                new Quiz
                {
                    Id = 1,
                    Name = "Java Theory",
                },
                new Quiz
                {
                    Id = 1,
                    Name = "UX Theory",
                },
            };


            var mockRepository = new Mock<IRepository>();
            mockRepository.Setup(m => m.Get<User>(x=>false)).Returns(users);
            mockRepository.Setup(m => m.Get<UserGroup>(x => false)).Returns(userGroups);
            mockRepository.Setup(m => m.Get<UserToUserGroup>(x => false)).Returns(userToUserGroups);
            mockRepository.Setup(m => m.Get<QuizPass>(x => false)).Returns(quizPasses);
            mockRepository.Setup(m => m.Get<Quiz>(x => false)).Returns(quizzes);

            _controller = new AdminReviewController(mockRepository.Object);

            string expectedData = "";
            ActionResult result = _controller.GetStudentsList();
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            JsonResult jsonResult = result as JsonResult;
            Assert.AreEqual(expectedData, jsonResult.Data);
        }
    }
}
