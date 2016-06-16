using eQuiz.Entities;
using eQuiz.Repositories.Abstract;
using eQuiz.Web.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eQuiz.Web.Areas.Admin.Models;

namespace eQuiz.Web.Areas.Admin.Controllers
{
    public class StudentController : BaseController
    {
        #region Fields

        private readonly IRepository _repository;

        #endregion

        #region Constructors

        public StudentController(IRepository repository)
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
            var passedQuizes = new List<QuizInfo>();
            var result = new List<QuizInfo>();
            var types = new List<object>();

            var quizzes = _repository.Get<Quiz>();
            var userQuizzes = _repository.Get<QuizPass>(qp => qp.UserId == id);
            var quizBlocks = _repository.Get<QuizBlock>();
            var quizQuestions = _repository.Get<QuizQuestion>();
            var questions = _repository.Get<Question>();
            var questionTypes = _repository.Get<QuestionType>();
            var quizStates = _repository.Get<QuizState>();

            var quizPasses = _repository.Get<QuizPass>();
            var quizPassScores = _repository.Get<QuizPassScore>();

            var autoQuestions = from qz in quizzes
                                join qb in quizBlocks on qz.Id equals qb.QuizId
                                join qq in quizQuestions on qb.Id equals qq.QuizBlockId
                                join q in questions on qq.QuestionId equals q.Id
                                join qt in questionTypes on q.QuestionTypeId equals qt.Id
                                where qt.IsAutomatic
                                group new { qz, qt } by qz.Id into grouped
                                select new QuestionsAuto
                                {
                                    QuizId = grouped.Key,
                                    IsAutomatic = grouped.Select(q => q.qt.IsAutomatic).Count()
                                };

            foreach (var item in autoQuestions)
            {
                types.Add(item);
            }

            var passed = from q in quizzes
                         join uq in userQuizzes on q.Id equals uq.QuizId
                         join qb in quizBlocks on q.Id equals qb.QuizId
                         join aq in autoQuestions on q.Id equals aq.QuizId
                         join qp in quizPasses on q.Id equals qp.QuizId
                         join qps in quizPassScores on qp.Id equals qps.QuizPassId
                         where uq.UserId == id
                         select new QuizInfo
                         {
                             id = uq.Id,
                             name = q.Name,
                             state = "Passed",
                             questions = (int)qb.QuestionCount,
                             verificationType = QuizInfo.SetVerificationType(aq.IsAutomatic, (int)qb.QuestionCount),
                             otherDetails = q.Description,
                             date = uq.FinishTime
                         };

            var notPassed = from q in quizzes
                            join uq in userQuizzes on q.Id equals uq.QuizId
                            join qb in quizBlocks on q.Id equals qb.QuizId
                            join aq in autoQuestions on q.Id equals aq.QuizId
                            where uq.UserId == id
                            select new QuizInfo
                            {
                                id = uq.Id,
                                name = q.Name,
                                state = "In verification",
                                questions = (int)qb.QuestionCount,
                                verificationType = QuizInfo.SetVerificationType(aq.IsAutomatic, (int)qb.QuestionCount),
                                otherDetails = q.Description,
                                date = uq.FinishTime
                            };

            foreach (var item in passed)
            {
                passedQuizes.Add(item);
            }

            //passedQuizes = passedQuizes.Distinct();

            foreach (var item in notPassed)
            {
                result.Add(item);
            }

            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 0; j < passedQuizes.Count; j++)
                {
                    if (result[i].Equals(passedQuizes[j]))
                    {
                        result[i] = passedQuizes[j];
                        break;
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStudentComments(int id)
        {
            var result = new List<object>();
            var comments = _repository.Get<UserComment>(com => com.UserId == id);
            var user = _repository.GetSingle<User>(u => u.Id == id);

            var query = from c in comments
                        select new
                        {
                            date = c.CommentTime.ToString(),
                            author = "Admin",
                            text = c.CommentText
                        };

            foreach (var item in query)
            {
                result.Add(item);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void AddComment(int studentId, int adminId, string comment)
        {
            var comm = new UserComment();
            comm.UserId = studentId;
            comm.AdminId = adminId;
            comm.CommentText = comment;
            comm.CommentTime = DateTime.Now;

            _repository.Insert<UserComment>(comm);
        }

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