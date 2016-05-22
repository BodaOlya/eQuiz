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
        public JsonResult GetStudentInfo(int id)
        {
            var student = _repository.GetSingle<User>(s => s.Id == id);
            var uug = _repository.Get<UserToUserGroup>(ug => ug.UserId == id);
            var usergroup = _repository.Get<UserGroup>();

            var data = new
            {
                id = student.Id,
                firstName = student.FirstName,
                lastName = student.LastName,
                phone = student.Phone,
                email = student.Email,
                userGroup = "Student"
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStudentQuizzes(int id)
        {
            var result = new List<object>();

            var quizzes = _repository.Get<Quiz>();
            var userQuizzes = _repository.Get<QuizPass>(qp => qp.UserId == id);

            var query = from q in quizzes
                        join uq in userQuizzes on q.Id equals uq.QuizId
                        where uq.UserId == id
                        select new
                        {
                            id = q.Id,
                            name = q.Name,
                            state = "Passed",
                            questions = 20,
                            verificationType = "Hasn't be here",
                            otherDetails = q.Description,
                            date = uq.FinishTime
                        };

            foreach (var item in query)
            {
                result.Add(item);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}