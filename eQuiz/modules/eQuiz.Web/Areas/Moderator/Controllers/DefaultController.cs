using eQuiz.Entities;
using eQuiz.Repositories.Abstract;
using eQuiz.Repositories.Concrete;
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
        #region Fields

        private readonly IRepository _repository;

        #endregion

        #region Constructors

        public DefaultController()
        {
            var dataContextSettings = new DefaultDataContextSettings(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString);
            var dataContextFactory = new EFDataContextFactory(dataContextSettings);
            this._repository = new EFRepository(dataContextFactory);
        }

        #endregion

        #region Web Actions

        [HttpGet]
        public ActionResult Index()
        {
            //using (var db = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            //{
            //    ViewBag.QuizzesCount = db.Quizs.Count();
            //    var today = DateTime.Now;
            //    ViewBag.ActiveQuizzesCount = (from quiz in db.Quizs where (quiz.StartDate <= today && today <= quiz.EndDate) select quiz).Count();
            //    ViewBag.InactiveQuizzesCount = (from quiz in db.Quizs where quiz.StartDate >= today select quiz).Count();
            //    ViewBag.QuestionsCount = db.Questions.Count();
            //    ViewBag.ActiveQuestionsCount = (from question in db.Questions where question.IsActive select question).Count();
            //    ViewBag.UserGroupsCount = db.UserGroups.Count();
            //    ViewBag.StudentsCount = db.Users.Count();
            //};

            var today = DateTime.Now; // Todo: implement with ITimeService
            
            ViewBag.QuizzesCount = _repository.Count<Quiz>();
            ViewBag.ActiveQuizzesCount = _repository.Count<Quiz>(quiz => quiz.StartDate <= today && today <= quiz.EndDate);            
            ViewBag.InactiveQuizzesCount = _repository.Count<Quiz>(quiz => quiz.StartDate >= today);            
            ViewBag.QuestionsCount = _repository.Count<Question>();            
            ViewBag.ActiveQuestionsCount = _repository.Count<Question>(question => question.IsActive);            
            ViewBag.UserGroupsCount = _repository.Count<UserGroup>();            
            ViewBag.StudentsCount = _repository.Count<User>();

            return View();
        }

        #endregion
    }
}