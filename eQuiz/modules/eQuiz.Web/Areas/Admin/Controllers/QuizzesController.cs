using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eQuiz.Repositories.Abstract;
using eQuiz.Web.Code;
using eQuiz.Entities;

namespace eQuiz.Web.Areas.Admin.Controllers
{
    public class QuizzesController : BaseController
    {
        #region Fields

        private readonly IRepository _repository;

        #endregion

        #region Constructors

        public QuizzesController(IRepository repository)
        {
            this._repository = repository;
        }

        #endregion

        #region Web Actions
        [HttpGet]
        public JsonResult GetQuizzesList()
        {
            var result = new List<object>();

            var quiz = _repository.Get<Quiz>();
            var quizBlock = _repository.Get<QuizBlock>();
            var userGroup = _repository.Get<UserGroup>();
            var quizPass = _repository.Get<QuizPass>();
            var user = _repository.Get<User>();

            var query = from passq in quizPass
                        join q in quiz on passq.QuizId equals q.Id
                        join ug in userGroup on q.GroupId equals ug.Id
                        join qb in quizBlock on q.Id equals qb.QuizId                                                
                        select new
                        {
                            id = passq.Id,
                            quiz_name = q.Name,
                            group_name = ug.Name,
                            questions_amount = qb.QuestionCount
                        }; /// only gets first 3 columns

            foreach (var item in query)
            {
                result.Add(item);
            }           

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}