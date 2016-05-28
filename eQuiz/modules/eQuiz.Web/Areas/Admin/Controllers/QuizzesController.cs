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

            var query = from q in quiz
                        join qb in quizBlock on q.Id equals qb.QuizId
                        join ug in userGroup on q.GroupId equals ug.Id
                        join qp in quizPass on q.Id equals qp.QuizId
                        join u in user on qp.UserId equals u.Id
                        group new { q, ug, qb } by new { quizName = q.Name, groupName = ug.Name } into grouped
                        select new
                        {
                            quiz_name = grouped.Select(item => item.q.Name).Distinct(),
                            group_name = grouped.Select(item => item.ug.Name).Distinct(),
                            questions_amount = grouped.Select(item => item.qb.QuestionCount).Distinct()
                        };

            result.Add(new { quiz_name = "Quiz1", group_name = "Group1", questions_amount = 20, students_amount = 10, verification_type = "Auto" });

            foreach (var item in query)
            {
                result.Add(item);
            }           

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}