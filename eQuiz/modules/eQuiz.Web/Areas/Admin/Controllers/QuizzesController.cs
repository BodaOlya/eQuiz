using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eQuiz.Repositories.Abstract;
using eQuiz.Web.Code;
using eQuiz.Entities;
using eQuiz.Web.Areas.Admin.Models;


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
            var userToGroup = _repository.Get<UserToUserGroup>();
            var quizQuestions = _repository.Get<QuizQuestion>();
            var questions = _repository.Get<Question>();
            var questionTypes = _repository.Get<QuestionType>();

            var autoQuestions = from qz in quiz
                                join qb in quizBlock on qz.Id equals qb.QuizId
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

            var studentAmountReal = from q in quiz
                                    join qp in quizPass on q.Id equals qp.QuizId
                                    join u in user on qp.UserId equals u.Id
                                    group new { q, u } by new { q.Id } into grouped
                                    select new
                                    {
                                        quizId = grouped.Key,
                                        studentAmount = grouped.Select(item => item.u.Id).Count()
                                    };
            //var studentAmount = from ug in userGroup
            //                    join utg in userToGroup on ug.Id equals utg.GroupId
            //                    join u in user on utg.UserId equals u.Id
            //                    group new { ug, u } by new { ug.Id, ug.Name } into grouped
            //                    select new
            //                    {
            //                        groupId = grouped.Key,
            //                        studentAmount = grouped.Select(item => item.u.Id).Count()
            //                    };

            //var query = from passq in quizPass
            //            join q in quiz on passq.QuizId equals q.Id
            //            join ug in userGroup on q.GroupId equals ug.Id
            //            join qb in quizBlock on q.Id equals qb.QuizId
            //            join aq in autoQuestions on passq.QuizId equals aq.QuizId
            //            join sa in studentAmount on ug.Id equals sa.groupId.Id
            //            select new
            //            {
            //                id = passq.Id,
            //                quiz_name = q.Name,
            //                group_name = ug.Name,
            //                questions_amount = qb.QuestionCount,
            //                students_amount = sa.studentAmount,
            //                verification_type = QuizInfo.SetVerificationType(aq.IsAutomatic, (int)qb.QuestionCount)
            //            }; 

            var query = from q in quiz
                        join ug in userGroup on q.GroupId equals ug.Id
                        join qb in quizBlock on q.Id equals qb.QuizId
                        join aq in autoQuestions on q.Id equals aq.QuizId
                        // join sa in studentAmount on ug.Id equals sa.groupId.Id
                        join sa in studentAmountReal on q.Id equals sa.quizId.Id
                        select new
                        {
                            id = q.Id,
                            quiz_name = q.Name,
                            group_name = ug.Name,
                            questions_amount = qb.QuestionCount,
                            students_amount = sa.studentAmount,
                            verification_type = QuizInfo.SetVerificationType(aq.IsAutomatic, (int)qb.QuestionCount)
                        };


            foreach (var item in query)
            {
                result.Add(item);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStudentQuiz(int quizPassId)
        {
            List<IQuestion> questionsList = new List<IQuestion>();

            var quizPass = _repository.GetSingle<QuizPass>(qp => qp.Id == quizPassId);
            var quizId = quizPass.QuizId;

            var quiz = _repository.GetSingle<Quiz>(q => q.Id == quizId);

            var quizBlock = _repository.GetSingle<QuizBlock>(qb => qb.QuizId == quizId);
            var quizQuestions = _repository.Get<QuizQuestion>(qq => qq.QuizBlockId == quizBlock.Id);

            var quizPassQuestions = _repository.Get<QuizPassQuestion>(qp => qp.QuizPassId == quizPassId);
            var questions = _repository.Get<Question>();
            var userTextAnswers = _repository.Get<UserTextAnswer>();
            var questionTypes = _repository.Get<QuestionType>();

            var userAnswers = _repository.Get<UserAnswer>();
            var questionAnswers = _repository.Get<QuestionAnswer>();
            var answers = _repository.Get<Answer>();

            var userAnswerScore = _repository.Get<UserAnswerScore>();

            // gets all text answers
            var textAnswers = from q in questions
                              join qt in questionTypes on q.QuestionTypeId equals qt.Id
                              join qpq in quizPassQuestions on q.Id equals qpq.QuestionId
                              join uta in userTextAnswers on qpq.Id equals uta.QuizPassQuestionId
                              join qq in quizQuestions on q.Id equals qq.QuestionId
                              join qa in questionAnswers on q.Id equals qa.QuestionId
                              join a in answers on qa.AnswerId equals a.Id
                              join uas in userAnswerScore on qpq.Id equals uas.QuizPassQuestionId into result
                              from res in result.DefaultIfEmpty()
                              where qt.IsAutomatic == false                       
                              select new TextQuestion(q.Id, qq.QuestionScore, res == null ? (int?)null : res.Score, q.QuestionText, uta.AnswerText, TextQuestion.GetAnswer(a.AnswerText), qq.QuestionOrder);
                            

            //gets all user answers
            var testAnswers = from q in questions
                              join qt in questionTypes on q.QuestionTypeId equals qt.Id
                              join qa in questionAnswers on q.Id equals qa.QuestionId
                              join a in answers on qa.AnswerId equals a.Id
                              join qq in quizQuestions on q.Id equals qq.QuestionId
                              join ua in userAnswers on a.Id equals ua.AnswerId
                              where qt.IsAutomatic == true
                              select new TestAnswer(a.Id, q.Id, a.AnswerText, (bool)a.IsRight, a.AnswerOrder, true);

            // gest all answer variants
            var testQuestions = from q in questions
                                join qt in questionTypes on q.QuestionTypeId equals qt.Id
                                join qa in questionAnswers on q.Id equals qa.QuestionId
                                join a in answers on qa.AnswerId equals a.Id
                                join qq in quizQuestions on q.Id equals qq.QuestionId
                                where qt.IsAutomatic == true
                                select new TestAnswer(a.Id, q.Id, a.AnswerText, (bool)a.IsRight, a.AnswerOrder, false);


            var tests = new List<TestAnswer>();
            var testanswers = new List<TestAnswer>();


            foreach (var item in textAnswers)
            {
                questionsList.Add(item);
            }

            foreach (var item in testAnswers)
            {
                testanswers.Add(item);
            }

            testanswers = testanswers.Distinct().ToList();

            foreach (var item in testQuestions)
            {
                tests.Add(item);
            }

            // Point tests clicked by user
            for (var i = 0; i < tests.Count; i++)
            {
                for (var j = 0; j < testanswers.Count; j++)
                {
                    if (tests[i].Equals(testanswers[j]))
                    {
                        tests[i] = testanswers[j];
                        break;
                    }
                }
            }

            // Generate test questions
            var generateQuestions = from q in questions
                                    join qt in questionTypes on q.QuestionTypeId equals qt.Id
                                    join qpq in quizPassQuestions on q.Id equals qpq.QuestionId
                                    join uas in userAnswerScore on qpq.Id equals uas.QuizPassQuestionId
                                    join qq in quizQuestions on q.Id equals qq.QuestionId
                                    join t in tests on q.Id equals t.QuestionId
                                    where qt.IsAutomatic == true
                                    group new { qpq, qq, q, t, uas } by q.Id into grouped
                                    select new SelectQuestion(grouped.Key,
                                                              grouped.Select(g => g.qq.QuestionScore).FirstOrDefault(),
                                                              grouped.Select(g => g.uas.Score).FirstOrDefault(),
                                                              grouped.Select(g => g.q.QuestionText).FirstOrDefault(),
                                                              grouped.Select(g => g.t).ToList(),
                                                              grouped.Select(g => g.qq.QuestionOrder).FirstOrDefault()
                                                              );

            foreach (var item in generateQuestions)
            {
                questionsList.Add(item);
            }

            var ordered = questionsList.OrderBy(q => q.Order).ToList();

            return Json(ordered, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuizInfo(int quizPassId)
        {
            var result = new List<object>();

            var quizPass = _repository.Get<QuizPass>(item => item.Id == quizPassId);            
            var quiz = _repository.Get<Quiz>();
            var userGroup = _repository.Get<UserGroup>();            
            var quizBlock = _repository.Get<QuizBlock>();
            var quizQuestions = _repository.Get<QuizQuestion>();

            var query = from quizp in quizPass
                        join q in quiz on quizp.QuizId equals q.Id
                        join ugroup in userGroup on q.GroupId equals ugroup.Id
                        join quizb in quizBlock on q.Id equals quizb.QuizId
                        join quizq in quizQuestions on quizb.Id equals quizq.QuizBlockId
                        group new { quizp, q, ugroup, quizq } by quizp.Id into grouped
                        select new
                        {
                            quiz_name = grouped.Select(item => item.q.Name).FirstOrDefault(),
                            group_name = grouped.Select(item => item.ugroup.Name).FirstOrDefault(),
                            start_date = grouped.Select(item => item.quizp.StartTime).FirstOrDefault(),
                            end_date = grouped.Select(item => item.quizp.FinishTime).FirstOrDefault(),
                            quiz_score = grouped.Sum(item => item.quizq.QuestionScore)
                        };
                        

            foreach(var item in query)
            {
                result.Add(item);
            }

            //SELECT quizp.Id, SUM(quizq.QuestionScore)
            //FROM tblQuizPass quizp
            //INNER JOIN tblQuiz quiz ON quizp.QuizId = quiz.Id
            //INNER JOIN tblUserGroup ugroup ON quiz.GroupId = ugroup.Id
            //INNER JOIN tblQuizBlock quizb ON quiz.Id = quizb.QuizId
            //INNER JOIN tblQuizQuestion quizq ON quizb.Id = quizq.QuizBlockId
            //WHERE quizp.Id = 13
            //GROUP BY quizp.Id
            //ORDER BY quizp.Id


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}