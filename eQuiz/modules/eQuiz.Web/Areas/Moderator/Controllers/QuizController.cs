using eQuiz.Entities;
using eQuiz.Repositories.Abstract;
using eQuiz.Repositories.Concrete;
using eQuiz.Web.Areas.Moderator.Models;
using eQuiz.Web.Code;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace eQuiz.Web.Areas.Moderator.Controllers
{
    public class QuizController : BaseController
    {
        #region Fields

        private readonly IRepository _repository;

        private const int QuizLockDuration = 2;

        #endregion

        #region Constructors

        public QuizController(IRepository repository)
        {
            this._repository = repository;
        }

        #endregion

        #region Web Actions

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return RedirectToAction("Edit");
        }

        public ActionResult Edit(int? id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult IsNameUnique(string name, int? id)
        {
            var result = ValidateQuizName(name, id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Get(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is null");
            }
            var now = DateTime.Now;
            var locked = false;
            var latestChange = _repository.Get<QuizEditHistory>(q => q.QuizId == id, q => q.User).OrderByDescending(q => q.LastChangeDate).Take(1).FirstOrDefault();

            if (latestChange != null)
            {
                var endLock = latestChange.LastChangeDate.AddMinutes(QuizLockDuration);
                if (endLock > now) // && USER != latestEdit.User //UPDATE WHEN AUTH
                {
                    locked = true;
                }
                else
                {
                    latestChange = LockQuiz((int)id, 1, now);
                }
            }
            else
            {
                latestChange = LockQuiz((int)id, 1, now);
            }

            Quiz quiz = _repository.GetSingle<Quiz>(q => q.Id == id, r => r.UserGroup, s => s.QuizState);
            QuizBlock block = _repository.GetSingle<QuizBlock>(b => b.QuizId == id);

            var minQuiz = GetQuizForSerialization(quiz);
            var minQuizBlock = GetQuizBlockForSerialization(block);
            var minLatestChange = GetQuizEditHistoryForSerialization(latestChange);

            var result = new { quiz = minQuiz, block = minQuizBlock, latestChange = minLatestChange, locked = locked };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetQuizzesForCopy()
        {
            IEnumerable<Quiz> quizzes = _repository.Get<Quiz>(q => q.QuizState.Name != "Draft" && q.QuizState.Name != "Archived", q => q.QuizState);

            var minQuizzes = new ArrayList();

            foreach (var quiz in quizzes)
            {
                minQuizzes.Add(new
                {
                    Id = quiz.Id,
                    Name = quiz.Name
                });
            }

            return Json(minQuizzes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetOpenQuizzes()
        {
            var quizzes = _repository.Get<Quiz>(q => q.QuizState.Name == "Opened", q => q.QuizState);

            var minQuizzes = new ArrayList();

            foreach (var quiz in quizzes)
            {
                minQuizzes.Add(new
                {
                    Id = quiz.Id,
                    Name = quiz.Name
                });
            }

            return Json(minQuizzes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStates()
        {
            var states = new List<QuizState>();

            states.Add(_repository.GetSingle<QuizState>(s => s.Name == "Draft"));
            states.Add(_repository.GetSingle<QuizState>(s => s.Name == "Opened"));
            states.Add(_repository.GetSingle<QuizState>(s => s.Name == "Scheduled"));
            states.Add(_repository.GetSingle<QuizState>(s => s.Name == "Archived"));

            var minStates = new ArrayList();

            foreach (var state in states)
            {
                minStates.Add(GetQuizStateForSerialization(state));
            }

            return Json(minStates, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetQuizzesPage(int currentPage = 1, int quizzesPerPage = 3, string predicate = "Name",
                                            bool reverse = false, string searchText = null, string selectedStatus = null)
        {
            IEnumerable<QuizListViewModel> quizzesList = null;
            var quizzesTotal = 0;

            if (selectedStatus == "All")
            {
                selectedStatus = null;
            }

            var quizzes = _repository.Get<Quiz>();
            var quizBlocks = _repository.Get<QuizBlock>();
            var quizStates = _repository.Get<QuizState>();

            //TODO : add method to repository for paging
            quizzesList = (from quiz in quizzes
                           join quizBlock in quizBlocks on quiz.Id equals quizBlock.QuizId
                           join quizState in quizStates on quiz.QuizStateId equals quizState.Id
                           select
                                   new QuizListViewModel
                                   {
                                       Id = quiz.Id,
                                       Name = quiz.Name,
                                       CountOfQuestions = quizBlock.QuestionCount,
                                       StartDate = quiz.StartDate,
                                       Duration = quiz.TimeLimitMinutes,
                                       StateName = quizState.Name
                                   }).Where(item => (searchText == null || item.Name.ToLower().Contains(searchText.ToLower())) &&
                                           (item.StateName == "Opened" || item.StateName == "Draft" || item.StateName == "Scheduled") &&
                                           (selectedStatus == null || item.StateName == selectedStatus))
                                            .OrderBy(q => q.Name);

            quizzesTotal = quizzesList.Count();

            switch (predicate)
            {
                case "Name":
                    quizzesList = reverse ? quizzesList.OrderByDescending(q => q.Name) : quizzesList.OrderBy(q => q.Name);
                    break;
                case "CountOfQuestions":
                    quizzesList = reverse ? quizzesList.OrderByDescending(q => q.CountOfQuestions) : quizzesList.OrderBy(q => q.CountOfQuestions);
                    break;
                case "StartDate":
                    quizzesList = reverse ? quizzesList.OrderBy(q => !q.StartDate.HasValue).ThenByDescending(q => q.StartDate) :
                        quizzesList.OrderBy(q => !q.StartDate.HasValue).ThenBy(q => q.StartDate);
                    break;
                case "StateName":
                    quizzesList = reverse ? quizzesList.OrderByDescending(q => q.StateName) : quizzesList.OrderBy(q => q.StateName);
                    break;
                case "Duration":
                    quizzesList = reverse ? quizzesList.OrderBy(q => !q.Duration.HasValue).ThenByDescending(q => q.Duration) :
                        quizzesList.OrderBy(q => !q.Duration.HasValue).ThenBy(q => q.Duration);
                    break;
                default:
                    quizzesList = reverse ? quizzesList.OrderByDescending(q => q.Name) : quizzesList.OrderBy(q => q.Name);
                    break;
            }

            quizzesList = quizzesList.Skip((currentPage - 1) * quizzesPerPage).Take(quizzesPerPage).ToList();

            return Json(new { Quizzes = quizzesList, QuizzesTotal = quizzesTotal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(Quiz quiz, QuizBlock block, QuizEditHistory latestChange)
        {
            var errorMessages = ValidateQuiz(quiz, block);
            if (errorMessages != null)
            {
                var errorMessage = string.Format("Invalid data: {0}", string.Concat(errorMessages));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, errorMessage);
            }

            var now = DateTime.Now;

            if (quiz.Id != 0)
            {
                quiz.QuizStateId = quiz.QuizState.Id;
                quiz.QuizState = null;
                block.Quiz = null;
                if (quiz.UserGroup != null)
                {
                    quiz.GroupId = quiz.UserGroup.Id;
                    quiz.UserGroup = null;
                }
                latestChange.LastChangeDate = now;
                latestChange.User = null;
                _repository.Update<QuizEditHistory>(latestChange);
                _repository.Update<Quiz>(quiz);
                _repository.Update<QuizBlock>(block);
            }
            else
            {
                quiz.QuizStateId = quiz.QuizState.Id;
                quiz.QuizState = null;
                _repository.Insert<Quiz>(quiz);
                block.TopicId = 1;
                block.QuizId = quiz.Id;
                _repository.Insert<QuizBlock>(block);
                _repository.Insert<QuizVariant>(new QuizVariant() { QuizId = quiz.Id });
                latestChange = LockQuiz(quiz.Id, 1, now);
            }
            quiz.QuizState = _repository.GetSingle<QuizState>(q => q.Id == quiz.QuizStateId);
            quiz.UserGroup = _repository.GetSingle<UserGroup>(g => g.Id == quiz.GroupId);

            var minQuiz = GetQuizForSerialization(quiz);
            var minQuizBlock = GetQuizBlockForSerialization(block);
            var minLatestChange = GetQuizEditHistoryForSerialization(latestChange);

            var result = new { quiz = minQuiz, block = minQuizBlock, latestChange = minLatestChange };

            return Json(result);
        }

        [HttpGet]
        public ActionResult Schedule()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Schedule(Quiz quiz)
        {
            var errorMessages = ValidateSchedule(quiz);
            if (errorMessages != null)
            {
                var errorMessage = string.Format("Invalid data: {0}", string.Concat(errorMessages));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, errorMessage);
            }


            var copy = _repository.GetSingle<Quiz>(q => q.Id == quiz.Id);
            var sheduledStateId = _repository.GetSingle<QuizState>(q => q.Name == "Scheduled").Id;
            var newQuiz = new Quiz()
            {
                Name = String.Format("{0} / {1}", quiz.Name, quiz.UserGroup.Name),
                StartDate = quiz.StartDate,
                EndDate = quiz.EndDate,
                TimeLimitMinutes = quiz.TimeLimitMinutes,
                QuizStateId = sheduledStateId,
                QuizTypeId = copy.QuizTypeId,
                GroupId = quiz.UserGroup.Id
            };
            var quizBlock = _repository.GetSingle<QuizBlock>(q => q.QuizId == quiz.Id);
            var quizQuestions = _repository.Get<QuizQuestion>(q => q.QuizBlockId == quizBlock.Id);
            var questions = _repository.Get<QuizQuestion>(q => q.QuizBlockId == quizBlock.Id, q => q.Question).Select(q => q.Question);
            var questionAnswers = new List<QuestionAnswer>();
            var questionTags = new List<QuestionTag>();
            foreach (var question in questions)
            {
                var currrentAnswers = _repository.Get<QuestionAnswer>(q => q.QuestionId == question.Id, q => q.Answer);
                questionAnswers.AddRange(currrentAnswers);

                var currentTags = _repository.Get<QuestionTag>(q => q.QuestionId == question.Id);
                questionTags.AddRange(currentTags);
            }
            var answers = questionAnswers.Select(q => q.Answer);

            _repository.Insert<Quiz>(newQuiz);

            var newQuizBlock = new QuizBlock()
            {
                QuizId = newQuiz.Id,
                QuestionCount = quizBlock.QuestionCount,
                TopicId = _repository.GetSingle<Topic>().Id
            };

            _repository.Insert<QuizBlock>(newQuizBlock);

            foreach (var question in quizQuestions)
            {
                question.Question = questions.FirstOrDefault(q => q.Id == question.QuestionId);
            }

            foreach (var answer in questionAnswers)
            {
                answer.Question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            }

            foreach (var tag in questionTags)
            {
                tag.Question = questions.FirstOrDefault(q => q.Id == tag.QuestionId);
            }

            foreach (var question in questions)
            {
                question.QuizPassQuestions = null;
                question.QuizQuestions = null;
                _repository.Insert(question);
            }

            foreach (var question in quizQuestions)
            {
                question.QuizBlockId = newQuizBlock.Id;
                question.QuestionId = question.Question.Id;
                question.Question = null;
                _repository.Insert<QuizQuestion>(question);
            }

            foreach (var answer in questionAnswers)
            {

                answer.Answer = answers.FirstOrDefault(a => a.Id == answer.AnswerId);
            }

            foreach (var answer in answers)
            {
                answer.QuestionAnswers = null;
                answer.UserAnswers = null;
                _repository.Insert<Answer>(answer);
            }

            foreach (var answer in questionAnswers)
            {
                answer.AnswerId = answer.Answer.Id;
                answer.Answer = null;
                answer.QuestionId = answer.Question.Id;
                answer.Question = null;
                _repository.Insert<QuestionAnswer>(answer);
            }

            foreach (var tag in questionTags)
            {
                tag.QuestionId = tag.Question.Id;
                tag.Question = null;
                _repository.Insert<QuestionTag>(tag);
            }

            return Json(newQuiz.Id);
        }

        [HttpGet]
        public ActionResult QuizPreview(int id)
        {
            var quiz = _repository.GetSingle<Quiz>(q => q.Id == id);

            if (quiz == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var userGroup = _repository.GetSingle<UserGroup>(ug => ug.Id == quiz.GroupId);
            var quizType = _repository.GetSingle<QuizType>(qt => qt.Id == quiz.QuizTypeId);
            var quizState = _repository.GetSingle<QuizState>(qs => qs.Id == quiz.QuizStateId);

            var quizPreviewModel = new QuizPreviewModel();

            quizPreviewModel.Id = quiz.Id;
            quizPreviewModel.Type = quizType.TypeName;
            quizPreviewModel.Name = quiz.Name;
            quizPreviewModel.Descriprtion = quiz.Description;
            quizPreviewModel.StartDate = quiz.StartDate;
            quizPreviewModel.EndDate = quiz.EndDate;
            quizPreviewModel.TimeLimitMinutes = quiz.TimeLimitMinutes;
            quizPreviewModel.InternetAccess = quiz.InternetAccess;
            quizPreviewModel.Group = userGroup != null ? userGroup.Name : "Not assigned";
            quizPreviewModel.State = quizState.Name;

            var quizBlock = _repository.GetSingle<QuizBlock>(qb => qb.QuizId == id);
            var quizTopic = _repository.GetSingle<Topic>(t => t.Id == quizBlock.TopicId);

            quizPreviewModel.Topic = quizTopic.Name;
            quizPreviewModel.IsRandom = quizBlock.IsRandom;
            quizPreviewModel.QuestionMinComplexity = quizBlock.QuestionMinComplexity;
            quizPreviewModel.QuestionMaxComplexity = quizBlock.QuestionMaxComplexity;
            quizPreviewModel.QuestionCount = quizBlock.QuestionCount;

            var quizQuestions = _repository.Get<QuizQuestion>(qq => qq.QuizBlockId == quizBlock.Id);

            quizPreviewModel.Questions = new List<QuestionInfo>();

            foreach (var quizQuestion in quizQuestions)
            {
                var question = _repository.GetSingle<Question>(q => q.Id == quizQuestion.QuestionId);

                var questionType = _repository.GetSingle<QuestionType>(qt => qt.Id == question.QuestionTypeId);

                var questionAnswers = _repository.Get<QuestionAnswer>(qa => qa.QuestionId == question.Id);

                var answers = questionAnswers.Select(questionAnswer => _repository.GetSingle<Answer>(a => a.Id == questionAnswer.AnswerId)).ToList();

                var questionInfo = new QuestionInfo
                {
                    Answers = answers,
                    QuestionComplexity = question.QuestionComplexity,
                    QuestionScore = quizQuestion.QuestionScore,
                    QuestionOrder = quizQuestion.QuestionOrder,
                    Type = questionType.TypeName,
                    Text = question.QuestionText
                };

                quizPreviewModel.Questions.Add(questionInfo);
            }

            return View(quizPreviewModel);
        }

        [HttpPost]
        public ActionResult DeleteQuizById(int id)
        {
            var quizBlocks = _repository.Get<QuizBlock>(qb => qb.QuizId == id);

            // Deleting QuizBlocks dependent to Quiz
            foreach (var quizBlock in quizBlocks)
            {
                // Deleting QuizQuestions dependent to QuizBlock
                var quizQuestions = _repository.Get<QuizQuestion>(qq => qq.QuizBlockId == quizBlock.Id);
                foreach (var quizQuestion in quizQuestions)
                {
                    _repository.Delete<int, QuizQuestion>("Id", quizQuestion.Id);
                }

                // Deleting QuizPassQuestions dependent to QuizBlock
                var quizPassQuestions = _repository.Get<QuizPassQuestion>(qpq => qpq.QuizBlockId == quizBlock.Id);
                foreach (var quizPassQuestion in quizPassQuestions)
                {
                    // Deleting UserAnswers dependent to QuizPassQuestion
                    var userAnswers =
                        _repository.Get<UserAnswer>(ua => ua.QuizPassQuestionId == quizPassQuestion.Id);
                    foreach (var userAnswer in userAnswers)
                    {
                        _repository.Delete<int, UserAnswer>("Id", userAnswer.Id);
                    }

                    // Deleting UserTextAnswers dependent to QuizPassQuestion
                    var userTextAnswers = _repository.Get<UserTextAnswer>(ua => ua.QuizPassQuestionId == quizPassQuestion.Id);
                    foreach (var userTextAnswer in userTextAnswers)
                    {
                        _repository.Delete<int, UserTextAnswer>("Id", userTextAnswer.Id);
                    }

                    _repository.Delete<int, QuizPassQuestion>("Id", quizPassQuestion.Id);
                }

                _repository.Delete<int?, QuizBlock>("Id", quizBlock.Id);
            }

            // Deleting QuizPasses dependent to Quiz
            var quizPasses = _repository.Get<QuizPass>(qp => qp.QuizId == id);
            foreach (var quizPass in quizPasses)
            {
                // Deleting QuizPassQuestions dependent to QuizPass
                var quizPassQuestions = _repository.Get<QuizPassQuestion>(qpq => qpq.QuizPassId == quizPass.Id);
                foreach (var quizPassQuestion in quizPassQuestions)
                {
                    // Deleting UserAnswers dependent to QuizPassQuestion
                    var userAnswers = _repository.Get<UserAnswer>(ua => ua.QuizPassQuestionId == quizPassQuestion.Id);
                    foreach (var userAnswer in userAnswers)
                    {
                        _repository.Delete<int, UserAnswer>("Id", userAnswer.Id);
                    }

                    // Deleting UserTextAnswers dependent to QuizPassQuestion
                    var userTextAnswers = _repository.Get<UserTextAnswer>(ua => ua.QuizPassQuestionId == quizPassQuestion.Id);
                    foreach (var userTextAnswer in userTextAnswers)
                    {
                        _repository.Delete<int, UserTextAnswer>("Id", userTextAnswer.Id);
                    }

                    _repository.Delete<int, QuizPassQuestion>("Id", quizPassQuestion.Id);
                }

                _repository.Delete<int, QuizPass>("Id", quizPass.Id);
            }

            // Deleting QuizVariants dependent to Quiz
            var quizVariants = _repository.Get<QuizVariant>(qv => qv.QuizId == id);
            foreach (var quizVariant in quizVariants)
            {
                _repository.Delete<int?, QuizVariant>("Id", quizVariant.Id);
            }

            // Deleting QuizEditHistories dependent to Quiz
            var quizEditHistories = _repository.Get<QuizEditHistory>(qeh => qeh.QuizId == id);
            foreach (var quizEditHistory in quizEditHistories)
            {
                _repository.Delete<int, QuizEditHistory>("Id", quizEditHistory.Id);
            }

            // Deleting Quiz
            _repository.Delete<int, Quiz>("Id", id);

            var result = Json(new HttpStatusCodeResult(HttpStatusCode.OK));

            return result;
        }

        #endregion

        #region Helpers

        private bool ValidateQuizName(string name, int? id)
        {
            bool exists = true;

            if (id != null)
            {
                var quiz = _repository.GetSingle<Quiz>(q => q.Name == name);
                if (quiz == null)
                {
                    exists = false;
                }
                else if (quiz.Id == (int)id)
                {
                    exists = false;
                }
            }
            else
            {
                exists = _repository.Exists<Quiz>(q => q.Name == name);
            }

            return !exists;
        }

        private object GetQuizForSerialization(Quiz quiz)
        {
            var minUserGroup = quiz.UserGroup != null ? GetUserGroupForSerialization(quiz.UserGroup) : null;
            var minQuizState = GetQuizStateForSerialization(quiz.QuizState);

            var minQuiz = new
            {
                Id = quiz.Id,
                Name = quiz.Name,
                QuizTypeId = quiz.QuizTypeId,
                StartDate = quiz.StartDate != null ? ((DateTime)quiz.StartDate).ToString("yyyy-MM-ddTHH:mm:ss") : null,
                EndDate = quiz.EndDate != null ? ((DateTime)quiz.EndDate).ToString("yyyy-MM-ddTHH:mm:ss") : null,
                TimeLimitMinutes = quiz.TimeLimitMinutes,
                UserGroup = minUserGroup,
                QuizState = minQuizState
            };

            return minQuiz;
        }

        private object GetQuizBlockForSerialization(QuizBlock block)
        {
            var minQuizBlock = new
            {
                Id = block.Id,
                QuizId = block.QuizId,
                TopicId = block.TopicId,
                QuestionCount = block.QuestionCount
            };

            return minQuizBlock;
        }

        private object GetQuizStateForSerialization(QuizState state)
        {
            var minState = new
            {
                Id = state.Id,
                Name = state.Name
            };

            return minState;
        }

        private object GetUserGroupForSerialization(UserGroup group)
        {
            var minGroup = new
            {
                Id = group.Id,
                Name = group.Name
            };

            return minGroup;
        }

        private object GetQuizEditHistoryForSerialization(QuizEditHistory history)
        {
            object user = null;
            if (history.User!= null)
            {
                user = new
                {
                    FirstName = history.User.FirstName,
                    LastName = history.User.LastName
                };
            }
            var minQuizEditHistory = new
            {
                Id = history.Id,
                UserId = history.UserId,
                QuizId = history.QuizId,
                StartDate = history.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                LastChangeDate = history.LastChangeDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                EndLockDate = history.LastChangeDate.AddMinutes(QuizLockDuration).ToString("dd.MM.yyyy HH:mm:ss"),
                User = user
            };

            return minQuizEditHistory;
        }

        private IEnumerable<string> ValidateSchedule(Quiz quiz)
        {
            var errorMessages = new List<string>();

            var selectedQuiz = _repository.GetSingle<Quiz>(q => q.Id == quiz.Id, q => q.QuizState);

            if (selectedQuiz == null)
            {
                errorMessages.Add("There is no such quiz");
            }
            else if (selectedQuiz.QuizState.Name != "Opened")
            {
                errorMessages.Add("Selected quiz cannot be applied");
            }

            if (quiz.StartDate == null)
            {
                errorMessages.Add("There is no start date");
            }

            if (quiz.StartDate <= DateTime.Now)
            {
                errorMessages.Add("Start date should be greater then current date");
            }

            if (quiz.EndDate == null)
            {
                errorMessages.Add("There is no end date");
            }

            if (quiz.EndDate <= DateTime.Now)
            {
                errorMessages.Add("End date should be greater then current date");
            }

            if (quiz.TimeLimitMinutes == null)
            {
                errorMessages.Add("There is no time limit");
            }
            else if ((byte)quiz.TimeLimitMinutes <= 0)
            {
                errorMessages.Add("Time limit should be greater then 0");
            }

            if (quiz.UserGroup == null)
            {
                errorMessages.Add("There is no user group selected");
            }
            else if (!_repository.Exists<UserGroup>(q => q.Id == quiz.UserGroup.Id))
            {
                errorMessages.Add("There is no such user group in the database");
            }


            return errorMessages.Count > 0 ? errorMessages : null;
        }

        private QuizEditHistory LockQuiz(int quizId, int userId, DateTime date)
        {
            var latestEdit = new QuizEditHistory()
            {
                QuizId = quizId,
                UserId = userId, 
                StartDate = date,
                LastChangeDate = date
            };
            _repository.Insert<QuizEditHistory>(latestEdit);

            return latestEdit;
        }

        private IEnumerable<string> ValidateQuiz(Quiz quiz, QuizBlock block)
        {
            var errorMessages = new List<string>();

            if (quiz.Name == null)
            {
                errorMessages.Add("There is no quiz name");
            }

            if (!ValidateQuizName(quiz.Name, quiz.Id))
            {
                errorMessages.Add("Quiz name is not unique");
            }

            if (block.QuestionCount == null)
            {
                errorMessages.Add("There is no question quantity");
            }
            else if (block.QuestionCount <= 0)
            {
                errorMessages.Add("Question quantity should be greater then 0");
            }

            if (!_repository.Exists<QuizType>(q => q.Id == quiz.QuizTypeId))
            {
                errorMessages.Add("There is no such quiz type in database");
            }

            if (quiz.QuizState == null)
            {
                if (quiz.StartDate != null)
                {
                    errorMessages.Add("There is start date but state isnt Scheduled");
                }
                if (quiz.EndDate != null)
                {
                    errorMessages.Add("There is end date but state isnt Scheduled");
                }
                if (quiz.TimeLimitMinutes != null)
                {
                    errorMessages.Add("There is time limit but state isnt Scheduled");
                }
                if (quiz.UserGroup != null)
                {
                    errorMessages.Add("There is user group selected but state isnt Scheduled");
                }
            }

            if(quiz.Id != 0)
            {
                var latestEdit = _repository.Get<QuizEditHistory>(q => q.QuizId == quiz.Id, q => q.User).OrderByDescending(q => q.LastChangeDate).Take(1).FirstOrDefault();

                //IF latestEdit USER ID != CURRENT USER.ID ERROR
                //if (latestEdit.UserId != 123)
                //{
                //    errorMessages.Add("Quiz is being edited by another user, refresh page");
                //}
            }

            return errorMessages.Count > 0 ? errorMessages : null;
        }

        #endregion

    }
}
