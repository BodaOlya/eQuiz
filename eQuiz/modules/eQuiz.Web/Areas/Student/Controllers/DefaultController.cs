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
                             // Unix time convertation.
                             StartDate = q.StartDate.HasValue ? (long)(q.StartDate.Value.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds : -1,
                             TimeLimitMinutes = q.TimeLimitMinutes,
                             InternetAccess = q.InternetAccess
                         };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQuestionsByQuizId(int id, int duration)
        {
            try
            {
                // Because duration is in minutes.
                int timeLeft = duration * 60;
                var lastPassedQuiz = _repository.Get<QuizPass>(q => q.QuizId == id
                                                                && q.Quiz.TimeLimitMinutes == duration
                                                                && q.StartTime.AddMinutes(duration) > DateTime.UtcNow)
                                                                .Select(q => new
                                                                {
                                                                    Id = q.Id,
                                                                    QuizId = q.QuizId,
                                                                    UserId = q.UserId,
                                                                    StartTime = q.StartTime,
                                                                    FinishTime = q.FinishTime
                                                                })
                                                                .ToList()
                                                                .LastOrDefault();
                int quizPassId = lastPassedQuiz.Id;

                if (lastPassedQuiz != null && lastPassedQuiz.FinishTime == null)
                {
                    timeLeft = (int)(lastPassedQuiz.StartTime.AddMinutes(duration) - DateTime.UtcNow).TotalSeconds;
                }
                else if (lastPassedQuiz == null || (lastPassedQuiz != null && lastPassedQuiz.FinishTime != null))
                {
                    QuizPass quizPassToInsert = new QuizPass
                    {
                        QuizId = id,
                        UserId = 1,//TODO will be fixed after authentification
                        StartTime = DateTime.UtcNow,
                        FinishTime = null
                    };

                    _repository.Insert<QuizPass>(quizPassToInsert);
                    quizPassId = quizPassToInsert.Id;
                    TempData["doc"] = quizPassToInsert.Id;
                }

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
                                        QuestionOrder = q.QuestionOrder,
                                        QuizPassId = quizPassId
                                    })
                                    .OrderBy(q => q.QuestionOrder)
                                    .ToList();

                return Json(new { remainingTime = timeLeft, questions = quizInfoList }, JsonRequestBehavior.AllowGet);
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

        //public void InsertQuizResult(QuizResultModel passedQuiz)
        //{
        //    QuizPass quizPassToInsert = new QuizPass
        //    {
        //        QuizId = passedQuiz.QuizId,
        //        UserId = 1,
        //        StartTime = passedQuiz.StartDate,
        //        FinishTime = passedQuiz.FinishDate
        //    };
        //    _repository.Insert<QuizPass>(quizPassToInsert);

        //    if (passedQuiz.UserAnswers != null)
        //    {
        //        var lastQuizPassIdentity = quizPassToInsert.Id;

        //        foreach (var userAnswer in passedQuiz.UserAnswers)
        //        {
        //            if (userAnswer != null)
        //            {
        //                QuizPassQuestion quizPassQuestionToInsert = new QuizPassQuestion
        //                {
        //                    QuizPassId = lastQuizPassIdentity,
        //                    QuestionId = userAnswer.QuestionId,
        //                    QuizBlockId = userAnswer.QuizBlock,
        //                    //Remade GetQuestionsByQuizId method to send questionOrder property on the client
        //                    QuestionOrder = userAnswer.QuestionOrder
        //                };

        //                _repository.Insert<QuizPassQuestion>(quizPassQuestionToInsert);

        //                var lastQuizPassQuestionIdentity = quizPassQuestionToInsert.Id;

        //                if (userAnswer.IsAutomatic)
        //                {
        //                    UserAnswer userAnswerToInsert;
        //                    if (userAnswer.AnswerId != null)
        //                    {
        //                        userAnswerToInsert = new UserAnswer
        //                        {
        //                            QuizPassQuestionId = lastQuizPassQuestionIdentity,
        //                            AnswerId = (int)userAnswer.AnswerId,
        //                            AnswerTime = userAnswer.AnswerTime
        //                        };
        //                        _repository.Insert<UserAnswer>(userAnswerToInsert);
        //                    }
        //                    else
        //                    {
        //                        foreach (var answerId in userAnswer.Answers)
        //                        {
        //                            if (answerId != null)
        //                            {
        //                                userAnswerToInsert = new UserAnswer
        //                                {
        //                                    QuizPassQuestionId = lastQuizPassQuestionIdentity,
        //                                    AnswerId = (int) answerId,
        //                                    AnswerTime = userAnswer.AnswerTime
        //                                };
        //                                _repository.Insert<UserAnswer>(userAnswerToInsert);
        //                            }
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    UserTextAnswer userTextAnswerToInsert = new UserTextAnswer
        //                    {
        //                        QuizPassQuestionId = lastQuizPassQuestionIdentity,
        //                        AnswerText = userAnswer.AnswerText,
        //                        AnswerTime = userAnswer.AnswerTime
        //                    };

        //                    _repository.Insert<UserTextAnswer>(userTextAnswerToInsert);
        //                }
        //            }

        //        }
        //    }
        //}

        public void InsertQuestionResult(QuestionResultModel passedQuestion)
        {
            passedQuestion.AnswerTime = DateTime.UtcNow;
            if (passedQuestion.QuestionType == "Descriptive")
            {
                if (passedQuestion.AnswerText == null)
                {
                    passedQuestion.AnswerText = "";
                }
                var textAnswer = _repository.GetSingle<UserTextAnswer>(a => a.QuizPassQuestion.QuestionId == passedQuestion.QuestionId
                                                && a.QuizPassQuestion.QuizPass.UserId == 1
                                                && a.QuizPassQuestion.QuizPassId == passedQuestion.QuizPassId);
              
                if (textAnswer != null)
                {
                    if (string.IsNullOrEmpty(passedQuestion.AnswerText))
                    {
                        _repository.Delete<int, UserTextAnswer>("Id", textAnswer.Id);
                        _repository.Delete<int, QuizPassQuestion>("Id", textAnswer.QuizPassQuestionId);
                    }
                    else
                    {
                        textAnswer.AnswerText = passedQuestion.AnswerText;
                        textAnswer.AnswerTime = passedQuestion.AnswerTime;

                        _repository.Update<UserTextAnswer>(textAnswer);
                    }
                }
                else
                {
                    var quizzPassQuestionToCheck =
                        _repository.GetSingle<QuizPassQuestion>(el => el.QuizPassId == passedQuestion.QuizPassId
                                                                      && el.QuestionId == passedQuestion.QuestionId);

                    QuizPassQuestion quizPassQuestionToInsert;
                    if (quizzPassQuestionToCheck == null)
                    {
                        quizPassQuestionToInsert = new QuizPassQuestion
                        {
                            QuizPassId = passedQuestion.QuizPassId,
                            QuestionId = passedQuestion.QuestionId,
                            QuizBlockId = passedQuestion.QuizBlock,
                            QuestionOrder = passedQuestion.QuestionOrder
                        };
                        _repository.Insert<QuizPassQuestion>(quizPassQuestionToInsert);
                    }
                    else
                    {
                        quizPassQuestionToInsert = quizzPassQuestionToCheck;
                    }
                    var lastGeneratedQuizPassQuestionId = quizPassQuestionToInsert.Id;

                    var userTextAnswerToInsert = new UserTextAnswer
                    {
                        QuizPassQuestionId = lastGeneratedQuizPassQuestionId,
                        AnswerTime = passedQuestion.AnswerTime,
                        AnswerText = passedQuestion.AnswerText
                    };

                    _repository.Insert<UserTextAnswer>(userTextAnswerToInsert);
                }
            }
            else if (passedQuestion.QuestionType == "Select one")
            {
                var radioAnswer = _repository.GetSingle<UserAnswer>(a => a.QuizPassQuestion.QuestionId == passedQuestion.QuestionId
                                                && a.QuizPassQuestion.QuizPass.UserId == 1
                                                && a.QuizPassQuestion.QuizPassId == passedQuestion.QuizPassId);
                if (radioAnswer != null)
                {
                    radioAnswer.AnswerId = (int)passedQuestion.AnswerId;
                    radioAnswer.AnswerTime = passedQuestion.AnswerTime;

                    _repository.Update<UserAnswer>(radioAnswer);
                }
                else
                {
                    var quizzPassQuestionToCheck =
                        _repository.GetSingle<QuizPassQuestion>(el => el.QuizPassId == passedQuestion.QuizPassId
                                                                      && el.QuestionId == passedQuestion.QuestionId);

                    QuizPassQuestion quizPassQuestionToInsert;
                    if (quizzPassQuestionToCheck == null)
                    {
                        quizPassQuestionToInsert = new QuizPassQuestion
                        {
                            QuizPassId = passedQuestion.QuizPassId,
                            QuestionId = passedQuestion.QuestionId,
                            QuizBlockId = passedQuestion.QuizBlock,
                            QuestionOrder = passedQuestion.QuestionOrder
                        };
                        _repository.Insert<QuizPassQuestion>(quizPassQuestionToInsert);
                    }
                    else
                    {
                        quizPassQuestionToInsert = quizzPassQuestionToCheck;
                    }
                   

                    var lastGeneratedQuizPassQuestionId = quizPassQuestionToInsert.Id;
                    if (passedQuestion.AnswerId != null)
                    {
                        var userAnswerToInsert = new UserAnswer
                        {
                            QuizPassQuestionId = lastGeneratedQuizPassQuestionId,
                            AnswerTime = passedQuestion.AnswerTime,
                            AnswerId = (int) passedQuestion.AnswerId
                        };

                        _repository.Insert<UserAnswer>(userAnswerToInsert);
                    }
                }
            }
            else
            {
                var checkAnswers = _repository.Get<UserAnswer>(a => a.QuizPassQuestion.QuestionId == passedQuestion.QuestionId
                                                && a.QuizPassQuestion.QuizPass.UserId == 1
                                                && a.QuizPassQuestion.QuizPassId == passedQuestion.QuizPassId);
                if (checkAnswers != null && checkAnswers.Count != 0)
                {
                    var allNull = true;

                    foreach (var answer in passedQuestion.Answers)
                    {
                        if (answer != null)
                        {
                            allNull = false;
                        }
                    }
                    if (allNull)
                    {
                        foreach (var existingCheckAnswer in checkAnswers)
                        {
                            _repository.Delete<int, UserAnswer>("Id", existingCheckAnswer.Id);
                        }
                        _repository.Delete<int, QuizPassQuestion>("Id", checkAnswers.First().QuizPassQuestionId);
                    }
                    else
                    {
                        var quizPassQuestionId = checkAnswers.First().QuizPassQuestionId;
                        foreach (var existingCheckAnswer in checkAnswers)
                        {
                            _repository.Delete<int, UserAnswer>("Id", existingCheckAnswer.Id);
                        }
                        foreach (var newCheckAnswer in passedQuestion.Answers)
                        {
                            if (newCheckAnswer != null)
                            {
                                var userAnswerToInsert = new UserAnswer
                                {
                                    QuizPassQuestionId = quizPassQuestionId,
                                    AnswerTime = passedQuestion.AnswerTime,
                                    AnswerId = (int)newCheckAnswer
                                };

                                _repository.Insert<UserAnswer>(userAnswerToInsert);
                            }
                        }
                    }
                }
                else
                {
                    var quizzPassQuestionToCheck =
                      _repository.GetSingle<QuizPassQuestion>(el => el.QuizPassId == passedQuestion.QuizPassId
                                                                    && el.QuestionId == passedQuestion.QuestionId);

                    QuizPassQuestion quizPassQuestionToInsert;
                    if (quizzPassQuestionToCheck == null)
                    {
                        quizPassQuestionToInsert = new QuizPassQuestion
                        {
                            QuizPassId = passedQuestion.QuizPassId,
                            QuestionId = passedQuestion.QuestionId,
                            QuizBlockId = passedQuestion.QuizBlock,
                            QuestionOrder = passedQuestion.QuestionOrder
                        };
                        _repository.Insert<QuizPassQuestion>(quizPassQuestionToInsert);
                    }
                    else
                    {
                        quizPassQuestionToInsert = quizzPassQuestionToCheck;
                    }                               
                    var lastGeneratedQuizPassQuestionId = quizPassQuestionToInsert.Id;

                    foreach (var checkAnswer in passedQuestion.Answers)
                    {
                        if (checkAnswer != null)
                        {
                            var userAnswerToInsert = new UserAnswer
                            {
                                QuizPassQuestionId = lastGeneratedQuizPassQuestionId,
                                AnswerTime = passedQuestion.AnswerTime,
                                AnswerId = (int) checkAnswer
                            };

                            _repository.Insert<UserAnswer>(userAnswerToInsert);
                        }
                    }
                }
            }
        }

        public void SetQuizFinishTime(int quizPassId)
        {
            var quizPassWithFinishTime = _repository.GetSingle<QuizPass>(qp => qp.Id == quizPassId);
            quizPassWithFinishTime.FinishTime = DateTime.UtcNow;
            _repository.Update<QuizPass>(quizPassWithFinishTime);

            var userResult = _repository.Get<QuizPassQuestion>(q => q.QuizPassId == quizPassId, q => q.Question.QuestionType);

            foreach(var elem in userResult)
            {
                if(elem.Question.QuestionType.TypeName == "Select one")
                {
                    var userAnswer = _repository.GetSingle<UserAnswer>(ur => ur.QuizPassQuestionId == elem.Id, ur => ur.Answer);

                    if(userAnswer.Answer.IsRight.HasValue && userAnswer.Answer.IsRight.Value)
                    {
                        var userAnswerScoreToInsert = new UserAnswerScore
                        {
                            QuizPassQuestionId = elem.Id,
                            Score = 1,
                            EvaluatedBy = 1,
                            EvaluatedAt = DateTime.UtcNow
                        };

                        _repository.Insert<UserAnswerScore>(userAnswerScoreToInsert);
                    }
                    else
                    {
                        var userAnswerScoreToInsert = new UserAnswerScore
                        {
                            QuizPassQuestionId = elem.Id,
                            Score = 0,
                            EvaluatedBy = 1,
                            EvaluatedAt = DateTime.UtcNow
                        };

                        _repository.Insert<UserAnswerScore>(userAnswerScoreToInsert);
                    }
                }
                else if(elem.Question.QuestionType.TypeName == "Select many")
                {
                    var userAnswers = _repository.Get<UserAnswer>(ur => ur.QuizPassQuestionId == elem.Id, ur => ur.Answer);
                    sbyte mark = 0;

                    foreach(var answer in userAnswers)
                    {
                        if(answer.Answer.IsRight.HasValue && answer.Answer.IsRight.Value)
                        {
                            mark++; 
                        }
                        else
                        {
                            mark--;
                        }
                    }

                    if(mark <= 0)
                    {
                        mark = 0;
                    }

                    var userAnswerScoreToInsert = new UserAnswerScore
                    {
                        QuizPassQuestionId = elem.Id,
                        Score = Convert.ToByte(mark),
                        EvaluatedBy = 1,//TODO
                        EvaluatedAt = DateTime.UtcNow
                    };

                    _repository.Insert<UserAnswerScore>(userAnswerScoreToInsert);
                }
            }
        }
        #endregion
    }
}