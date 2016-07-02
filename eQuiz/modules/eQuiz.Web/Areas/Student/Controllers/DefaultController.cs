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
    [Authorize(Roles = "Student")]
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
        public ActionResult QuizInRun()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetUserGroups()
        {
            User currentUser = _repository.GetSingle<User>(u => u.Email == User.Identity.Name);
            var userGroups = _repository.Get<UserToUserGroup>(u => u.UserId == currentUser.Id, u => u.UserGroup)
                .Select(u => u.UserGroup.Name).ToList();

            return Json(userGroups, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllQuizzes()
        {
            User currentUser = _repository.GetSingle<User>(u => u.Email == User.Identity.Name);
            IEnumerable<User> users = _repository.Get<User>();
            IEnumerable<UserToUserGroup> userToUserGroup = _repository.Get<UserToUserGroup>();
            IEnumerable<UserGroup> userGroup = _repository.Get<UserGroup>();
            IEnumerable<Quiz> allQuizzes = _repository.Get<Quiz>();

            var result = from u in users
                         where u.Id == currentUser.Id
                         join utug in userToUserGroup on u.Id equals utug.UserId
                         join g in userGroup on utug.GroupId equals g.Id
                         join q in allQuizzes on g.Id equals q.GroupId
                         select new
                         {
                             Id = q.Id,
                             Name = q.Name,
                             // Unix time convertation.
                             StartDate = q.StartDate.HasValue ? (long)(q.StartDate.Value.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds : -1,
                             TimeLimitMinutes = q.TimeLimitMinutes,
                             IsActive = q.StartDate.HasValue && q.StartDate.Value <= DateTime.Now ? true : false
                         };

            return Json(new { quizzes = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQuestionsByQuizId(int id, int duration)
        {
            User currentUser = _repository.GetSingle<User>(u => u.Email == User.Identity.Name);
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
                        UserId = currentUser.Id,//TODO will be fixed after authentification
                        StartTime = DateTime.UtcNow,
                        FinishTime = null
                    };

                    _repository.Insert<QuizPass>(quizPassToInsert);
                    quizPassId = quizPassToInsert.Id;
                    TempData["doc"] = quizPassToInsert.Id;

                    var quizQuestions = _repository.Get<QuizQuestion>(q => q.QuizVariant.QuizId == id && q.QuizBlock.Quiz.TimeLimitMinutes == duration);
                    foreach (var question in quizQuestions)
                    {
                        var quizPassQuestionToInsert = new QuizPassQuestion
                        {
                            QuizPassId = quizPassToInsert.Id,
                            QuestionId = question.QuestionId,
                            QuizBlockId = question.QuizBlockId,
                            QuestionOrder = (short)question.QuestionOrder
                        };

                        _repository.Insert<QuizPassQuestion>(quizPassQuestionToInsert);
                    }
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

        public void InsertQuestionResult(QuestionResultModel passedQuestion)
        {
            User currentUser = _repository.GetSingle<User>(u => u.Email == User.Identity.Name);
            passedQuestion.AnswerTime = DateTime.UtcNow;
            if (passedQuestion.QuestionType == "Descriptive")
            {
                if (passedQuestion.AnswerText == null)
                {
                    passedQuestion.AnswerText = "";
                }
                var textAnswer = _repository.GetSingle<UserTextAnswer>(a => a.QuizPassQuestion.QuestionId == passedQuestion.QuestionId
                                                && a.QuizPassQuestion.QuizPass.UserId == currentUser.Id
                                                && a.QuizPassQuestion.QuizPassId == passedQuestion.QuizPassId);

                if (textAnswer != null)
                {
                    if (string.IsNullOrEmpty(passedQuestion.AnswerText))
                    {
                        _repository.Delete<int, UserTextAnswer>("Id", textAnswer.Id);
                        //_repository.Delete<int, QuizPassQuestion>("Id", textAnswer.QuizPassQuestionId);
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
                    var userTextAnswerToInsert = new UserTextAnswer
                    {
                        QuizPassQuestionId = quizzPassQuestionToCheck.Id,
                        AnswerTime = passedQuestion.AnswerTime,
                        AnswerText = passedQuestion.AnswerText
                    };

                    _repository.Insert<UserTextAnswer>(userTextAnswerToInsert);
                }
            }
            else if (passedQuestion.QuestionType == "Select one")
            {
                var radioAnswer = _repository.GetSingle<UserAnswer>(a => a.QuizPassQuestion.QuestionId == passedQuestion.QuestionId
                                                && a.QuizPassQuestion.QuizPass.UserId == currentUser.Id
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
                    if (passedQuestion.AnswerId != null)
                    {
                        var userAnswerToInsert = new UserAnswer
                        {
                            QuizPassQuestionId = quizzPassQuestionToCheck.Id,
                            AnswerTime = passedQuestion.AnswerTime,
                            AnswerId = (int)passedQuestion.AnswerId
                        };

                        _repository.Insert<UserAnswer>(userAnswerToInsert);
                    }
                }
            }
            else
            {
                var checkAnswers = _repository.Get<UserAnswer>(a => a.QuizPassQuestion.QuestionId == passedQuestion.QuestionId
                                                && a.QuizPassQuestion.QuizPass.UserId == currentUser.Id
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
                    foreach (var checkAnswer in passedQuestion.Answers)
                    {
                        if (checkAnswer != null)
                        {
                            var userAnswerToInsert = new UserAnswer
                            {
                                QuizPassQuestionId = quizzPassQuestionToCheck.Id,
                                AnswerTime = passedQuestion.AnswerTime,
                                AnswerId = (int)checkAnswer
                            };

                            _repository.Insert<UserAnswer>(userAnswerToInsert);
                        }
                    }
                }
            }
        }

        public void SetQuizFinishTime(int quizPassId)
        {
            User currentUser = _repository.GetSingle<User>(u => u.Email == User.Identity.Name);

            var quizPassWithFinishTime = _repository.GetSingle<QuizPass>(qp => qp.Id == quizPassId);
            quizPassWithFinishTime.FinishTime = DateTime.UtcNow;
            _repository.Update<QuizPass>(quizPassWithFinishTime);

            var userResult = _repository.Get<QuizPassQuestion>(q => q.QuizPassId == quizPassId, q => q.Question.QuestionType, q => q.QuizPass);

            foreach (var elem in userResult)
            {
                if (elem.Question.QuestionType.TypeName == "Select one")
                {
                    var userAnswer = _repository.GetSingle<UserAnswer>(ur => ur.QuizPassQuestionId == elem.Id, ur => ur.Answer);
                    UserAnswerScore userAnswerScoreToInsert;
                    if (userAnswer == null)
                    {
                        userAnswerScoreToInsert = new UserAnswerScore
                        {
                            QuizPassQuestionId = elem.Id,
                            Score = 0,
                            EvaluatedBy = currentUser.Id,
                            EvaluatedAt = DateTime.UtcNow
                        };

                        _repository.Insert<UserAnswerScore>(userAnswerScoreToInsert);
                    }
                    else if (userAnswer.Answer.IsRight.HasValue && userAnswer.Answer.IsRight.Value)
                    {
                        userAnswerScoreToInsert = new UserAnswerScore
                        {
                            QuizPassQuestionId = elem.Id,
                            Score = _repository.GetSingle<QuizQuestion>(qq => qq.Id == elem.Question.Id).QuestionScore,
                            EvaluatedBy = currentUser.Id,
                            EvaluatedAt = DateTime.UtcNow
                        };

                        _repository.Insert<UserAnswerScore>(userAnswerScoreToInsert);
                    }
                    else
                    {
                        userAnswerScoreToInsert = new UserAnswerScore
                        {
                            QuizPassQuestionId = elem.Id,
                            Score = 0,
                            EvaluatedBy = currentUser.Id,
                            EvaluatedAt = DateTime.UtcNow
                        };

                        _repository.Insert<UserAnswerScore>(userAnswerScoreToInsert);
                    }
                }
                else if (elem.Question.QuestionType.TypeName == "Select many")
                {
                    var userAnswers = _repository.Get<UserAnswer>(ur => ur.QuizPassQuestionId == elem.Id, ur => ur.Answer, ur => ur.QuizPassQuestion);

                    UserAnswerScore userAnswerScoreToInsert;
                    if (userAnswers == null || userAnswers.Count == 0)
                    {

                        userAnswerScoreToInsert = new UserAnswerScore
                        {
                            QuizPassQuestionId = elem.Id,
                            Score = 0,
                            EvaluatedBy = currentUser.Id,
                            EvaluatedAt = DateTime.UtcNow
                        };
                        _repository.Insert<UserAnswerScore>(userAnswerScoreToInsert);

                    }
                    else
                    {
                        double mark = 0;
                        byte questionScore = _repository.GetSingle<QuizQuestion>(qq => qq.Id == elem.Question.Id).QuestionScore;

                        var quizInfo = _repository.Get<QuizQuestion>(q => q.QuizVariant.QuizId == elem.QuizPass.QuizId && q.QuestionId == elem.QuestionId,
                                                                 q => q.Question,
                                                                 q => q.Question.QuestionAnswers).SingleOrDefault();
                        var questionAnswers = quizInfo.Question.QuestionAnswers;
                        int amountOfTrueAns = 0;

                        foreach (var qa in questionAnswers)
                        {
                            qa.Answer =
                                _repository.GetSingle<QuestionAnswer>(el => el.Id == qa.Id, el => el.Answer)
                                    .Answer;
                            if (qa.Answer.IsRight.HasValue && qa.Answer.IsRight.Value)
                            {
                                amountOfTrueAns++;
                            }
                        }
                        double pointsPerRightAnswer = Convert.ToDouble(questionScore) / Convert.ToDouble(questionAnswers.Count);

                        var checkedTrueAns = 0;

                        foreach (var answer in userAnswers)
                        {
                            if (answer.Answer.IsRight.HasValue && answer.Answer.IsRight.Value)
                            {
                                mark += pointsPerRightAnswer;
                                checkedTrueAns++;
                            }
                        }
                        var notCheckedTrueAns = amountOfTrueAns - checkedTrueAns;

                        var pointsToAdd = ((questionAnswers.Count - userAnswers.Count) - notCheckedTrueAns) * pointsPerRightAnswer;

                        mark += pointsToAdd;

                        userAnswerScoreToInsert = new UserAnswerScore
                        {
                            QuizPassQuestionId = elem.Id,
                            Score = mark, // TODO
                            EvaluatedBy = currentUser.Id,
                            EvaluatedAt = DateTime.UtcNow
                        };

                        _repository.Insert<UserAnswerScore>(userAnswerScoreToInsert);
                    }
                }
            }
        }
        #endregion
    }
}