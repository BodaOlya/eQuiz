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

        public DefaultController(IRepository repository)
        {
            this._repository = repository;
        }

        #endregion

        #region Web Actions

        [HttpGet]
        public ActionResult Index()
        {
            var today = DateTime.Now; // Todo: implement with ITimeService
            
            ViewBag.QuizzesCount = _repository.Count<Quiz>();
            ViewBag.ActiveQuizzesCount = _repository.Count<Quiz>(quiz => quiz.StartDate <= today && today <= quiz.EndDate);
            ViewBag.InactiveQuizzesCount = _repository.Count<Quiz>(quiz => quiz.StartDate >= today);
            ViewBag.QuestionsCount = _repository.Count<Question>();
            ViewBag.ActiveQuestionsCount = _repository.Count<Question>(question => question.IsActive);
            ViewBag.UserGroupsCount = _repository.Count<UserGroup>();

            var activeStateId = _repository.GetSingle<UserGroupState>(ugs => ugs.Name == "Active").Id;
            ViewBag.ActiveUserGroups = _repository.Count<UserGroup>(userGroup => userGroup.UserGroupStateId == activeStateId);

            var archivedStateId = _repository.GetSingle<UserGroupState>(ugs => ugs.Name == "Archived").Id;
            ViewBag.ArchivedUserGroups = _repository.Count<UserGroup>(userGroup => userGroup.UserGroupStateId == archivedStateId);
            ViewBag.StudentsCount = _repository.Count<User>();

            return View();
        }

        #endregion
    }
}