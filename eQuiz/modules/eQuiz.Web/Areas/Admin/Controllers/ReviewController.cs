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


        [HttpGet]
        public JsonResult GetStudentInfo(int id)
        {
            var student = _repository.GetSingle<User>(s => s.Id == id);
            var uug = _repository.Get<UserToUserGroup>(ug => ug.UserId == id);
            var usergroup = _repository.Get<UserGroup>();

            var query = from g in usergroup
                        join ug in uug on g.Id equals ug.GroupId
                        where ug.UserId == id
                        select g;

            var gr = new List<object>();

            foreach (var item in query)
            {
                gr.Add(item.Name);
            }


            var data = new
            {
                id = student.Id,
                firstName = student.FirstName,
                lastName = student.LastName,
                fatherName = student.FatheName,
                phone = student.Phone,
                email = student.Email,
                userGroup = gr
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

   /*     public JsonResult GetStudentComments(int id)
        {
            var result = new List<object>();
            var comments = _repository.Get<UserComment>(com => com.UserId == id);

            var query = from c in comments
                        select new
                        {
                            id = c.Id,
                            adminId = c.Name,
                            commentTime = c.CommentTime,
                            commentText = c.CommentText
                        };

            foreach (var item in query)
            {
                result.Add(item);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }*/

        [HttpGet]
        public JsonResult GetQuiz(int studentId, int quizId)
        {
            var quizInfo = new List<object>();
            var quizQuestions = new List<object>();
            var quizzes = _repository.Get<Quiz>();
            var quizPass = _repository.Get<QuizPass>(qp => qp.UserId == studentId && qp.QuizId == quizId);
            var quizPassId = quizPass[0].Id;
            var query = from q in quizzes
                        join qp in quizPass on q.Id equals qp.QuizId
                        select new
                        {
                            id = qp.Id,
                            name = q.Name,
                            startDate = q.StartDate,
                            endDate = q.EndDate,
                            finishTime = qp.FinishTime
                        };

            foreach (var item in query)
            {
                quizInfo.Add(item);
            }

            var quizPassQuestions = _repository.Get<QuizPassQuestion>(qpq => qpq.QuizPassId == quizPassId);
            var questions = _repository.Get<Question>();
            var questionAnswers = _repository.Get<QuestionAnswer>();
            var userAnswers = _repository.Get<UserAnswer>();
            var questionTypes = _repository.Get<QuestionType>();

            var query2 = from qpq in quizPassQuestions
                        join q in questions on qpq.QuestionId equals q.Id
                        join qa in questionAnswers on qpq.QuestionId equals qa.QuestionId
                        join ua in userAnswers on qpq.QuestionId equals ua.QuizPassQuestionId
                        join qt in questionTypes on qpq.QuestionId equals qt.Id
                        select new
                        {
                            id = q.Id,
                            question = q.QuestionText,
                            answer = qa.Answer.AnswerText,
                            questionStatus = 0,
                            questionType = qt.TypeName
                        };

            foreach (var item in query)
            {
                quizQuestions.Add(item);
            }
            quizInfo.Add(quizQuestions);
            return Json(quizInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void UpdateUserInfo(int id, string firstName, string lastName, string phone)
        {
            var user = _repository.GetSingle<User>(u => u.Id == id);
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Phone = phone;

            _repository.Update<User>(user);
        }

        #endregion
    }
}