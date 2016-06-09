using eQuiz.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;
using eQuiz.Web.Code;
using eQuiz.Repositories.Abstract;
using System.Collections;

namespace eQuiz.Web.Areas.Moderator.Controllers
{
    public class QuizQuestionController : BaseController
    {
        #region Fields

        private readonly IRepository _repository;

        #endregion

        #region Constructor

        public QuizQuestionController(IRepository repository)
        {
            this._repository = repository;
        }

        #endregion

        #region Web Actions

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetQuestionTypes()
        {
            var questionTypes = _repository.Get<QuestionType>().ToList();

            var minQuestionTypes = new ArrayList();

            foreach (var type in questionTypes)
            {
                minQuestionTypes.Add(GetQuestionTypeForSerialization(type));
            }

            return Json(minQuestionTypes, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Save(int id, Question[] questions, Answer[][] answers, Tag[][] tags)
        {
            using (var context = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            {
                var quiz = context.Quizs.FirstOrDefault(x => x.Id == id);
                var quizState = context.QuizStates.FirstOrDefault(state => state.Id == quiz.QuizStateId).Name;
                var quizBlock = context.QuizBlocks.First(x => x.QuizId == id);
                if (questions == null && answers == null && tags == null && quizState == "Draft")
                {
                    return RedirectToAction("Get", new { id = id });
                }

                IEnumerable<string> errors = QuestionsValidate(id, questions, answers, tags);
                if (errors != null)
                {
                    string mergedErrors = string.Join(". ", errors.ToArray());
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, mergedErrors);
                }

                if (quizBlock.QuestionCount != questions.Length && quizState != "Draft")
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Not enough question had created.");
                }
                var topicId = context.Topics.First().Id;
                var quizVariantId = _repository.GetSingle<QuizVariant>(x => x.QuizId == id).Id; 
                var blockId = quizBlock.Id;
                var newQuestions = questions.Where(q => q.Id == 0).ToList();

                DeleteQuestions(id, questions);

                for (int i = 0; i < questions.Length; i++)
                {
                    var question = questions[i];

                    if (question.Id != 0)
                    {
                        var existedQuestion = context.Questions.FirstOrDefault(x => x.Id == question.Id);
                        if (existedQuestion == null)
                        {
                            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Question is not found");
                        }
                        existedQuestion.IsActive = question.IsActive;
                        existedQuestion.QuestionComplexity = question.QuestionComplexity;
                        existedQuestion.QuestionText = question.QuestionText;
                        existedQuestion.QuestionTypeId = question.QuestionTypeId;

                    }
                    else
                    {
                        question.TopicId = topicId;
                        question.IsActive = true;
                        context.Questions.Add(question);
                    }
                }
                context.SaveChanges();

                for (int i = 0; i < questions.Length; i++)
                {
                    var question = questions[i];

                    if (newQuestions.Contains(question))
                    {
                        var quizQuestion = new QuizQuestion
                        {
                            QuestionId = question.Id,
                            QuizVariantId = quizVariantId,
                            QuestionOrder = (short)(i + 1),
                            QuizBlockId = blockId
                        };
                        context.QuizQuestions.Add(quizQuestion);
                    }
                }
                //for delete answer
                for (int i = 0; i < answers.Length; i++)
                {
                    var questionId = questions[i].Id;
                    var questionAnswer = context.QuestionAnswers.Where(y => y.QuestionId == questionId).ToList(); // list of answers for current question 

                    if (answers[i][0] != null)
                    {
                        for (var qa = 0; qa < answers[i].Length; qa++)
                        {
                            var answer = answers[i][qa];

                            if (answer.Id != 0)
                            {
                                var existedAnswer = context.Answers.Include("QuestionAnswers").FirstOrDefault(x => x.Id == answer.Id);

                                if (existedAnswer == null)
                                {
                                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Answer is not found");
                                }
                                var temp = context.QuestionAnswers.Where(x => x.AnswerId == answer.Id && x.QuestionId == questionId).FirstOrDefault();
                                if (temp != null)
                                {
                                    var answerFromList = questionAnswer.Where(a => a.Id == temp.Id).FirstOrDefault();
                                    if (answerFromList != null)
                                    {
                                        questionAnswer.Remove(answerFromList);
                                    }
                                }
                                existedAnswer.AnswerOrder = answer.AnswerOrder;
                                existedAnswer.AnswerText = answer.AnswerText;
                                existedAnswer.IsRight = answer.IsRight;
                            }
                            else
                            {
                                answer.AnswerOrder = (byte)(qa + 1);
                                context.Answers.Add(answer);
                                context.QuestionAnswers.Add(new QuestionAnswer
                                {
                                    QuestionId = questionId,
                                    Answer = answer,
                                });
                                //answer.AnswerOrder = (byte)(qa + 1);
                                //answer.QuestionAnswers.Add(new QuestionAnswer
                                //{
                                //    Answer = answer
                                //});
                            }
                        }
                        if (questionAnswer != null)
                        {
                            foreach (var item in questionAnswer)
                            {
                                context.QuestionAnswers.Remove(item);//test it 
                            }
                        }
                    }
                    //todo doesn't delete tags 

                    if (tags[i][0] != null)
                    {
                        var question = context.Questions.FirstOrDefault(x => x.Id == questionId);
                        var questionTags = context.QuestionTags.Where(x => x.QuestionId == questionId).ToList();

                        for (int qt = 0; qt < tags[i].Length; qt++)
                        {
                            var tag = tags[i][qt];

                            var existedTag = context.Tags.FirstOrDefault(x => x.Name == tag.Name);

                            if (existedTag == null)
                            {
                                context.Tags.Add(tag);
                                question.QuestionTags.Add(new QuestionTag
                                {
                                    Tag = tag
                                });
                            }
                            else
                            {
                                var temp = context.QuestionTags.FirstOrDefault(x => x.TagId == existedTag.Id && x.QuestionId == questionId);
                                if (temp != null)
                                {
                                    if (questionTags.Contains(temp))
                                    {
                                        questionTags.Remove(temp);
                                    }
                                }
                                else
                                {
                                    question.QuestionTags.Add(new QuestionTag
                                    {
                                        Tag = existedTag
                                    });
                                }

                            }
                        }
                        if (questionTags != null)
                        {
                            foreach (var item in questionTags)
                            {
                                context.QuestionTags.Remove(item);//test it 
                            }
                        }
                    }
                    else
                    {
                        var questionTags = context.QuestionTags.Where(x => x.QuestionId == questionId).ToList();
                        foreach (var item in questionTags)
                        {
                            context.QuestionTags.Remove(item);
                        }
                    }
                    context.SaveChanges();
                }
            }
            return RedirectToAction("Get", new { id = id });
        }

        public ActionResult Get(int id)
        {
            List<Question> questions = new List<Question>();
            List<ArrayList> tags = new List<ArrayList>();
            List<ArrayList> answers = new List<ArrayList>();
            int quizId = 0;

            var quiz = _repository.GetSingle<Quiz>(q => q.Id == id);
            if (quiz == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Quiz is not found");
            }

            var quizState = _repository.GetSingle<QuizState>(qs => qs.Id == quiz.QuizStateId).Name;

            if (quizState != "Archived")
            {
                quizId = quiz.Id;
                var quizBlockIds = _repository.Get<QuizBlock>(qb => qb.QuizId == quizId).Select(qb => qb.Id).ToList();
                var quizQuestios = _repository.Get<QuizQuestion>(qq => quizBlockIds.Contains(qq.QuizBlockId)).ToList();

                foreach (var quizQuestion in quizQuestios)
                {
                    questions.Add(_repository.GetSingle<Question>(q => q.Id == quizQuestion.QuestionId, q => q.QuestionAnswers, q => q.QuestionTags));

                }

                foreach (var item in questions)
                {
                    var questionAnswers = _repository.Get<QuestionAnswer>(x => x.QuestionId == item.Id, x => x.Answer).ToList();
                    var answerStorage = new ArrayList();
                    foreach (var answer in questionAnswers)
                    {
                        var tempAnswer = GetAnswerForSerialization(answer.Answer);
                        answerStorage.Add(tempAnswer);
                    }
                    answers.Add(answerStorage);
                }

                foreach (var item in questions)
                {
                    // var questionTags = context.QuestionTags.Where(x => x.QuestionId == item.Id).Include("Tag").ToList();
                    var questionTags = _repository.Get<QuestionTag>(x => x.QuestionId == item.Id, x => x.Tag).ToList();

                    var tagStorage = new ArrayList();
                    foreach (var tag in questionTags)
                    {
                        var tempTag = GetTagForSerialization(tag.Tag);
                        tagStorage.Add(tempTag);
                    }
                    tags.Add(tagStorage);
                }

            }
            var returnQuestion = new ArrayList();
            foreach (var question in questions)
            {
                var tempQuestion = GetQuestionForSerialization(question);
                returnQuestion.Add(tempQuestion);
            }
            var data = JsonConvert.SerializeObject(new { questions = returnQuestion, answers = answers, id = quizId, tags = tags }, Formatting.None,
                                                    new JsonSerializerSettings()
                                                    {
                                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                    });

            return Content(data, "application/json");
        }
        #endregion

        #region Helpers

        private void DeleteQuestions(int quizId, Question[] questions)
        {
            var quiz = _repository.GetSingle<Quiz>(x => x.Id == quizId);
            var blockId = _repository.GetSingle<QuizBlock>(x => x.QuizId == quizId).Id;

            var quizQuestions = _repository.Get<QuizQuestion>(qq => qq.QuizBlockId == blockId).ToList();
            for (int i = 0; i < questions.Length; i++)
            {
                int currentQuestionId = questions[i].Id;
                QuizQuestion matchedQuizQuestion = _repository.GetSingle<QuizQuestion>(qq => qq.QuestionId == currentQuestionId && qq.QuizBlockId == blockId);
                if (matchedQuizQuestion != null)
                {
                    QuizQuestion questionFromList = quizQuestions.Where(qq => qq.Id == matchedQuizQuestion.Id).FirstOrDefault();
                    if (questionFromList != null)
                    {
                        quizQuestions.Remove(questionFromList);
                    }
                }
            }
            if (quizQuestions != null)
            {
                foreach (var item in quizQuestions)
                {
                    _repository.Delete<int, QuizQuestion>("Id", item.Id);
                }
            }
        }

        private IEnumerable<string> QuestionsValidate(int quizId, Question[] questions, Answer[][] answers, Tag[][] tags)
        {
            var errorMessages = new List<string>();

            if (quizId == 0)
            {
                errorMessages.Add("Quiz is not exist");
            }

            if (questions == null)
            {
                errorMessages.Add("No questions");
            }

            if (answers == null)
            {
                errorMessages.Add("No answers");
            }

            if (tags == null)
            {
                errorMessages.Add("No tags");
            }

            if (questions.Length != answers.Length || questions.Length != tags.Length || answers.Length != tags.Length)
            {
                errorMessages.Add("Different length of questions, answers or tags");
            }

            var isAllQuestionsHaveText = true;
            var isExistsAnswers = true;
            var isAllAnswersHaveText = true;
            var isExistsCheckedAnswerForAllQuestions = true;

            for (var i = 0; i < questions.Length; i++)
            {
                if (string.IsNullOrEmpty(questions[i].QuestionText))
                {
                    isAllQuestionsHaveText = false;
                }
                if (questions[i].QuestionTypeId == 2 || questions[i].QuestionTypeId == 3)
                {
                    if (answers[i] == null || answers[i].Length == 0)
                    {
                        isExistsAnswers = false;
                        isExistsCheckedAnswerForAllQuestions = false;
                    }
                    else
                    {
                        var chechedCount = 0;
                        for (var j = 0; j < answers[i].Length; j++)
                        {
                            if (answers[i][j] == null)
                            {
                                isExistsAnswers = false;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(answers[i][j].AnswerText))
                                {
                                    isAllAnswersHaveText = false;
                                }
                                chechedCount = (bool)answers[i][j].IsRight ? chechedCount + 1 : chechedCount;
                            }
                        }
                        if (questions[i].QuestionTypeId == 2 && chechedCount != 1)
                        {
                            isExistsCheckedAnswerForAllQuestions = false;
                        }
                        if (questions[i].QuestionTypeId == 3 && chechedCount < 1)
                        {
                            isExistsCheckedAnswerForAllQuestions = false;
                        }
                    }
                }
            }

            if (!isAllQuestionsHaveText)
            {
                errorMessages.Add("Not all questions have text");
            }

            if (!isExistsAnswers)
            {
                errorMessages.Add("Not all questions with type 'Select one' or 'Select many' have answers");
            }

            if (!isAllAnswersHaveText)
            {
                errorMessages.Add("Not all answers have text");
            }

            if (!isExistsCheckedAnswerForAllQuestions)
            {
                errorMessages.Add("Not all questions with type 'Select one' or 'Select many' have right answers");
            }

            return errorMessages.Count > 0 ? errorMessages.ToArray() : null;
        }

        private object GetAnswerForSerialization(Answer answer)
        {
            var minAnswer = new
            {
                Id = answer.Id,
                AnswerText = answer.AnswerText,
                AnswerOrder = answer.AnswerOrder,
                IsRight = answer.IsRight
            };

            return minAnswer;
        }

        private object GetQuestionForSerialization(Question question)
        {
            var minQuestion = new
            {
                Id = question.Id,
                QuestionTypeId = question.QuestionTypeId,
                TopicId = question.TopicId,
                QuestionText = question.QuestionText,
                QuestionComplexity = question.QuestionComplexity,
                IsActive = question.IsActive
            };

            return minQuestion;
        }

        private object GetTagForSerialization(Tag tag)
        {

            var minTag = new
            {
                Id = tag.Id,
                Name = tag.Name
            };

            return minTag;
        }

        private object GetQuestionTypeForSerialization(QuestionType type)
        {
            var minType = new
            {
                Id = type.Id,
                TypeName = type.TypeName,
                IsAutomatic = type.IsAutomatic
            };

            return minType;
        }

        #endregion
    }
}