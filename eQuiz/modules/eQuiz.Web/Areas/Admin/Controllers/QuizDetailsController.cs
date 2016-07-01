using eQuiz.Entities;
using eQuiz.Repositories.Abstract;
using eQuiz.Web.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eQuiz.Web.Areas.Admin.Controllers
{
    public class QuizDetailsController : BaseController
    {
        #region Fields

        private readonly IRepository _repository;

        #endregion

        #region Constructors

        public QuizDetailsController(IRepository repository)
        {
            this._repository = repository;
        }

        #endregion

        #region Web actions

        [HttpGet]
        public JsonResult GetQuizPasses(int id)
        {
            var res = new List<object>();

            var quizPasses = _repository.Get<QuizPass>();
            var users = _repository.Get<User>();
            var quizScores = _repository.Get<QuizPassScore>();
            var quizBlock = _repository.Get<QuizBlock>();
            var quizPassQuestions = _repository.Get<QuizPassQuestion>();
            var userAnswerScores = _repository.Get<UserAnswerScore>();
            var quizQuestions = _repository.Get<QuizQuestion>();

            var questionCountQuery = from qpq in quizPassQuestions
                                     join qp in quizPasses on qpq.QuizPassId equals qp.Id
                                     group new { qpq, qp } by qp.Id into changed
                                     select new
                                     {
                                         id = changed.Key,
                                         questions = changed.Count(ch => ch.qpq.Id > 0)
                                     };

            var userScores = from uas in userAnswerScores
                             join qpq in quizPassQuestions on uas.QuizPassQuestionId equals qpq.Id
                             join qp in quizPasses on qpq.QuizPassId equals qp.Id
                             join qcq in questionCountQuery on qp.Id equals qcq.id
                             group new { qp, uas, qcq } by qp.Id into changed
                             select new
                             {
                                 id = changed.Key,
                                 scores = changed.Sum(ch => ch.uas.Score),
                                 passed = changed.Count(ch => ch.uas.Score > 0),
                                 notPassed = changed.Count(ch => ch.uas.Score == 0),
                                 inVerification = (int)changed.Select(ch => ch.qcq.questions).First() - changed.Count(ch => ch.uas.Score > 0) - changed.Count(ch => ch.uas.Score == 0)
                             };

            var query = from qp in quizPasses
                        join u in users on qp.UserId equals u.Id
                        where qp.QuizId == id
                        join us in userScores on qp.Id equals us.id
                        join qs in quizScores on qp.Id equals qs.QuizPassId into temp
                        from t in temp.DefaultIfEmpty()
                        select new
                        {
                            id = u.Id,
                            quizPassId = qp.Id,
                            student = u.FirstName + " " + u.LastName,
                            email = u.Email,
                            studentScore = us.scores,
                            quizStatus = t == null ? "In Verification" : "Passed",
                            questionDetails = new {
                                passed = us.passed,
                                notPassed = us.notPassed,
                                inVerification = us.inVerification
                            },
                        };

            foreach (var item in query)
            {
                res.Add(item);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuiz(int id)
        {
            var result = new List<object>();

            var quizzPasses = _repository.Get<QuizPass>();            
            var quiz = _repository.Get<Quiz>();
            var ugroup = _repository.Get<UserGroup>();            
            var quizBlock = _repository.Get<QuizBlock>();
            var quizQuestions = _repository.Get<QuizQuestion>();

            var query = from q in quiz
                        join ug in ugroup on q.GroupId equals ug.Id
                        join qp in quizzPasses on q.Id equals qp.QuizId
                        where qp.QuizId == id
                        join qb in quizBlock on qp.QuizId equals qb.QuizId
                        join qq in quizQuestions on qb.Id equals qq.QuizBlockId
                        group new { q, ug, qq, qp } by qp.Id into changed
                        select new
                        {
                            quizId = changed.Select(ch => ch.q.Id).Distinct(),
                            quizName = changed.Select(ch => ch.q.Name).Distinct(),
                            groupName = changed.Select(ch => ch.ug.Name).Distinct(),
                            quizScore = changed.Sum(ch => ch.qq.QuestionScore),
                        };


            foreach (var item in query)
            {
                result.Add(item);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetQuizStudents(int quizId)
        {
            var res = new List<object>();

            var quizPasses = _repository.Get<QuizPass>(qp => qp.Id == quizId);
            var users = _repository.Get<User>();

            var query = from u in users
                        join qp in quizPasses on u.Id equals qp.UserId
                        select new
                        {
                            id = u.Id,
                            student = u.FirstName + " " + u.LastName,
                            studentScore = 0,
                            quizStatus = "Not Passed",
                            questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
                        };

            foreach (var item in query)
            {
                res.Add(item);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}