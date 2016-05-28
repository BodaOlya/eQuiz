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
    public class ReviewController : BaseController
    {
        #region Fields

        private readonly IRepository _repository;

        #endregion

        #region Constructors

        public ReviewController(IRepository repository)
        {
            this._repository = repository;
        }

        #endregion

        #region Web Actions
        [HttpGet]
        public JsonResult GetStudentsList()
        {
            var res = new List<object>();

            var users = _repository.Get<User>();
            var userGroups = _repository.Get<UserGroup>();
            var userToUserGroups = _repository.Get<UserToUserGroup>();
            var quizzPasses = _repository.Get<QuizPass>();
            var quizzes = _repository.Get<Quiz>();

            var query = from u in users
                        join uug in userToUserGroups on u.Id equals uug.UserId
                        join ug in userGroups on uug.GroupId equals ug.Id
                        join qp in quizzPasses on u.Id equals qp.UserId
                        join q in quizzes on qp.QuizId equals q.Id
                        group new { u, ug, q } by new { u.Id } into grouped
                        select new
                        {
                            id = grouped.Key,
                            student = grouped.Select(g => g.u.FirstName + " " + g.u.LastName).Distinct(),
                            userGroup = grouped.Select(g => g.ug.Name).Distinct(),
                            quizzes = grouped.Select(g => g.q.Name).Distinct()
                        };

            foreach (var item in query)
            {
                res.Add(item);
            }

            //res.Add(new { id = 1, student = "Ben Gann", userGroup = "Student", quizzes = ".Net" });

            return Json(res, JsonRequestBehavior.AllowGet);
        }



        #endregion
    }
}