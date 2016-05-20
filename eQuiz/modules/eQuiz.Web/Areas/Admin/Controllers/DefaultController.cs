using eQuiz.Web.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eQuiz.Web.Areas.Admin.Controllers
{
    public class DefaultController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetStudentsList()
        {
            var students = new List<object>();

            students.Add(new { id = 1, student = "Ben Gann", userGroup = "User", quizzes = ".Net" });
            students.Add(new { id = 2, student = "Harley Quinn", userGroup = "User", quizzes = ".Net" });

            return Json(students, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentInfo()
        {
            var studentInfo = new
            {
                id = 1,
                firstName = "Eugene",
                lastName = "Smith",
                phone = "555-15-754",
                email = "yevhen.smith@mail.com",
                userGroup = "Student"
            };

            return Json(studentInfo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentQuizzes()
        {
            var studentQuizzes = new List<object>();

            studentQuizzes.Add(new
            {
                id = 1,
                name = "Quiz 1",
                state = "Passed",
                questions = 20,
                verificationType = "Auto",
                otherDetails = "Details 1",
                date = "15.05.2016"
            });
            studentQuizzes.Add(new
            {
                id = 2,
                name = "Quiz 2",
                state = "In Verification",
                questions = 10,
                verificationType = "Manual",
                otherDetails = "Details 2",
                date = "04.04.2016"
            });
            studentQuizzes.Add(new
            {
                id = 3,
                name = "Quiz 3",
                state = "Not passed",
                questions = 20,
                verificationType = "Combined (A:12, M:8)",
                otherDetails = "Details 3",
                date = "10.05.2016"
            }
                );

            return Json(studentQuizzes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentComments()
        {
            var studentComments = new List<object>();

            studentComments.Add(new
            {
                date = "15.04.2016",
                author = "Volodymyr",
                text = "Responsible, initiative student with excellent knowledge of necessary frameworks"
            });
            studentComments.Add(new
            {
                date = "03.05.2016",
                author = "Ivan",
                text = "Demonstrates deep theoretical knowledge"
            });

            return Json(studentComments, JsonRequestBehavior.AllowGet);
        }
    }
}