using eQuiz.Web.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eQuiz.Repositories.Abstract;
using eQuiz.Repositories.Concrete;
using eQuiz.Entities;
using Newtonsoft.Json;
using eQuiz.Web.Areas.Student.Models;
using System.Data.Entity.Infrastructure;

namespace eQuiz.Web.Areas.Student.Controllers
{
    public class DefaultController : BaseController
    {
        #region Private Members

        private readonly IRepository _repository;

        #endregion

        #region Constructors

        public DefaultController(IRepository repository)
        {
            this._repository = repository;
        }
        #endregion

        #region Action Methods

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAllQuizzes()
        {
            IEnumerable<Quiz> allQuizzes = _repository.Get<Quiz>();

            var result = from q in allQuizzes
                         select new
                         {
                             Id = q.Id,
                             Name = q.Name,
                             StartDate = q.StartDate.HasValue ? q.StartDate.Value.ToString() : "No start date",
                             TimeLimitMinutes = q.TimeLimitMinutes,
                             InternetAccess = q.InternetAccess
                         };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQuestionsByQuizId(int id, int duration)
        {
            try
            {
                QuizPass quizPassToInsert = new QuizPass
                {
                    QuizId = id,
                    UserId = 1,//TODO will be fixed after authentification
                    StartTime = DateTime.UtcNow,
                    FinishTime = DateTime.UtcNow
                };
                _repository.Insert<QuizPass>(quizPassToInsert);
                TempData["doc"] = quizPassToInsert.Id;

                var quizInfo = _repository.Get<QuizQuestion>(q => q.QuizVariant.QuizId == id && q.QuizBlock.Quiz.TimeLimitMinutes == duration,
                                                                 q => q.Question,
                                                                 q => q.Question.QuestionType,
                                                                 q => q.Question.QuestionAnswers,
                                                                 q => q.QuizBlock.Quiz);
                var quizInfoList = quizInfo
                                            .Select(q => new
                                            {
                                                Id = q.Question.Id,
                                                Text = q.Question.QuestionText,
                                                IsAutomatic = q.Question.QuestionType.IsAutomatic,
                                                QuestionType = q.Question.QuestionType.TypeName,
                                                Answers = q.Question.QuestionAnswers.Select(a => new
                                                {
                                                    Id = a.Id,
                                                    Text = _repository.GetSingle<Answer>(ans => ans.Id == a.Id).AnswerText
                                                }),
                                                QuizBlock = q.QuizBlockId,
                                                QuestionOrder = q.QuestionOrder
                                            })
                                            .OrderBy(q => q.QuestionOrder)
                                            .ToList();

                return Json(quizInfoList, JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException)
            {
                return Json("SaveChangeException", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("Exception", JsonRequestBehavior.AllowGet);
            }
           

        }

        public void InsertQuizResult(QuizResultModel passedQuiz)
        {
            QuizPass quizPassToInsert = new QuizPass
            {
                QuizId = passedQuiz.QuizId,
                UserId = 1,
                StartTime = passedQuiz.StartDate,
                FinishTime = passedQuiz.FinishDate
            };
            _repository.Insert<QuizPass>(quizPassToInsert);

            if (passedQuiz.UserAnswers != null)
            {
                var lastQuizPassIdentity = quizPassToInsert.Id;

                foreach (var userAnswer in passedQuiz.UserAnswers)
                {
                    if (userAnswer != null)
                    {
                        QuizPassQuestion quizPassQuestionToInsert = new QuizPassQuestion
                        {
                            QuizPassId = lastQuizPassIdentity,
                            QuestionId = userAnswer.QuestionId,
                            QuizBlockId = userAnswer.QuizBlock,
                            //Remade GetQuestionsByQuizId method to send questionOrder property on the client
                            QuestionOrder = userAnswer.QuestionOrder
                        };

                        _repository.Insert<QuizPassQuestion>(quizPassQuestionToInsert);

                        var lastQuizPassQuestionIdentity = quizPassQuestionToInsert.Id;

                        if (userAnswer.IsAutomatic)
                        {
                            UserAnswer userAnswerToInsert;
                            if (userAnswer.AnswerId != null)
                            {
                                userAnswerToInsert = new UserAnswer
                                {
                                    QuizPassQuestionId = lastQuizPassQuestionIdentity,
                                    AnswerId = (int)userAnswer.AnswerId,
                                    AnswerTime = userAnswer.AnswerTime
                                };
                                _repository.Insert<UserAnswer>(userAnswerToInsert);
                            }
                            else
                            {
                                foreach (var answerId in userAnswer.Answers)
                                {
                                    if (answerId != null)
                                    {
                                        userAnswerToInsert = new UserAnswer
                                        {
                                            QuizPassQuestionId = lastQuizPassQuestionIdentity,
                                            AnswerId = (int) answerId,
                                            AnswerTime = userAnswer.AnswerTime
                                        };
                                        _repository.Insert<UserAnswer>(userAnswerToInsert);
                                    }
                                }
                            }

                        }
                        else
                        {
                            UserTextAnswer userTextAnswerToInsert = new UserTextAnswer
                            {
                                QuizPassQuestionId = lastQuizPassQuestionIdentity,
                                AnswerText = userAnswer.AnswerText,
                                AnswerTime = userAnswer.AnswerTime
                            };

                            _repository.Insert<UserTextAnswer>(userTextAnswerToInsert);
                        }
                    }

                }
            }
        }
        #endregion
    }
}