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

            Random score = new Random(); // Temporary changes. Must be deleted.

            var query = from u in users
                        join qp in quizPasses on u.Id equals qp.UserId
                        where qp.QuizId == id
                        group new { u, qp } by u.Id into changed
                        select new
                        {
                            id = changed.Key,
                            student = changed.Select(ch => ch.u.FirstName + " " + ch.u.LastName).Distinct(),
                            //student = u.FirstName + " " + u.LastName,
                            email = changed.Select(ch => ch.u.Email).Distinct(),
                            studentScore = score.Next(1, 100),
                            quizStatus = "Not Passed",
                            questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
                        };

            foreach (var item in query)
            {
                res.Add(item);
            }

            #region Temporary region that must be deleted
            res.Add(new
            {
                id = 100,
                student = "First Student",
                email = "FirstStudent@com",
                studentScore = score.Next(1, 100),
                quizStatus = "Not Passed",
                questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
            });
            res.Add(new
            {
                id = 101,
                student = "Second Student",
                email = "SecondStudent@com",
                studentScore = score.Next(1, 100),
                quizStatus = "Not Passed",
                questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
            });
            res.Add(new
            {
                id = 102,
                student = "Third Student",
                email = "ThirdStudent@com",
                studentScore = score.Next(1, 100),
                quizStatus = "Not Passed",
                questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
            });
            res.Add(new
            {
                id = 103,
                student = "Fourth Student",
                email = "FourthStudent@com",
                studentScore = score.Next(1, 100),
                quizStatus = "Not Passed",
                questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
            });
            res.Add(new
            {
                id = 104,
                student = "Fifth Student",
                email = "FifthStudent@com",
                studentScore = score.Next(1, 100),
                quizStatus = "Not Passed",
                questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
            });
            res.Add(new
            {
                id = 105,
                student = "Six Student",
                email = "SixStudent@com",
                studentScore = score.Next(1, 100),
                quizStatus = "Not Passed",
                questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
            });
            res.Add(new
            {
                id = 106,
                student = "Seven Student",
                email = "SevenStudent@com",
                studentScore = score.Next(1, 100),
                quizStatus = "Not Passed",
                questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
            });
            res.Add(new
            {
                id = 107,
                student = "Eight Student",
                email = "EightStudent@com",
                studentScore = score.Next(1, 100),
                quizStatus = "Not Passed",
                questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
            });
            res.Add(new
            {
                id = 108,
                student = "Nine Student",
                email = "NineStudent@com",
                studentScore = score.Next(1, 100),
                quizStatus = "Not Passed",
                questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
            });
            res.Add(new
            {
                id = 109,
                student = "Tenth Student",
                email = "TenthStudent@com",
                studentScore = score.Next(1, 100),
                quizStatus = "Not Passed",
                questionDetails = "{ passed: 0, notPassed: 10, inVerification: 0 }"
            });
            #endregion;

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuiz(int id)
        {
            var result = new List<object>();

            var quizzPasses = _repository.Get<QuizPass>();
            var quizPassScore = _repository.Get<QuizPassScore>();
            var quiz = _repository.Get<Quiz>();
            var ugroup = _repository.Get<UserGroup>();
            var qblock = _repository.Get<QuizBlock>();

            var query = from q in quiz
                        join ug in ugroup on q.GroupId equals ug.Id
                        join qp in quizzPasses on q.Id equals qp.QuizId
                        join qps in quizPassScore on qp.Id equals qps.Id
                        where q.Id == id
                        select new
                        {
                            quizName = q.Name,
                            groupName = ug.Name,
                            quizScore = qps.PassScore
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