using eQuiz.Entities;
using eQuiz.Web.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eQuiz.Web.Areas.Moderator.Controllers
{
    public class DefaultController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            using (var db = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            {
                ViewBag.QuizzesCount = db.Quizs.Count();
                var today = DateTime.Now;
                ViewBag.ActiveQuizzesCount = (from quiz in db.Quizs where (quiz.StartDate <= today && today <= quiz.EndDate) select quiz).Count();
                ViewBag.InactiveQuizzesCount = (from quiz in db.Quizs where quiz.StartDate >= today select quiz).Count();
                ViewBag.QuestionsCount = db.Questions.Count();
                ViewBag.ActiveQuestionsCount = (from question in db.Questions where question.IsActive select question).Count();
                ViewBag.UserGroupsCount = db.UserGroups.Count();
                ViewBag.StudentsCount = db.Users.Count();
            };
            return View();
        }
    }
}