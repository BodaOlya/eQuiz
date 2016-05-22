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
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetQuestionTypes()
        {
            //using (var context = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            //{
            //    var typesList = context.QuestionTypes.OrderBy(x => x.TypeName).ToList();
            //    return Json(typesList, JsonRequestBehavior.AllowGet);
            //}
            var questionTypes = _repository.Get<QuestionType>().ToList();
            return Json(questionTypes, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Save(int id, Question[] questions, Answer[][] answers, Tag[][] tags)
        {
            string[] errors = QuestionsValidation(id, questions, answers, tags);
            if (errors != null)
            {
                string mergedErrors = string.Join("\n", errors);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, mergedErrors);
            }
            using (var context = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            {
                var quiz = context.Quizs.FirstOrDefault(x => x.Id == id);
                var quizState = context.QuizStates.FirstOrDefault(state => state.Id == quiz.QuizStateId).Name;
                var quizBlock = context.QuizBlocks.First(x => x.QuizId == id);
                if(quizBlock.QuestionCount != questions.Length && quizState != "Draft")
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Not enough question had created.");
                }
                var topicId = context.Topics.First().Id;
                var quizVariantId = 1; /*context.QuizVariants.First(x => x.QuizId == id).Id;*/ //we don't have fk on tblQuiz
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
                    var questionAnswer = context.QuestionAnswers.Where(y => y.Id == questionId).ToList(); // list of answers for current question 

                    if (answers[i][0] != null)
                    {
                        for (var qa = 0; qa < answers[i].Length; qa++)
                        {
                            var answer = answers[i][qa];

                            if (answer.Id != 0)
                            {
                                var existedAnswer = context.Answers.Include("QuestionAnswer").FirstOrDefault(x => x.Id == answer.Id);

                                if (existedAnswer == null)
                                {
                                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Answer is not found");
                                }
                                var temp = context.QuestionAnswers.FirstOrDefault(x => x.AnswerId == existedAnswer.Id && x.QuestionId == questionId);

                                if (questionAnswer.Contains(temp))
                                {
                                    questionAnswer.Remove(temp);
                                }
                                existedAnswer.AnswerOrder = answer.AnswerOrder;
                                existedAnswer.AnswerText = answer.AnswerText;
                                existedAnswer.IsRight = answer.IsRight;
                            }
                            else
                            {
                                answer.AnswerOrder = (byte)(qa + 1);
                                answer.QuestionAnswers.Add(new QuestionAnswer
                                {
                                    Answer = answer
                                });
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
                }
                context.SaveChanges();
            }
            return RedirectToAction("Get", new { id = id });
        }

        private void DeleteQuestions(int quizId, Question[] questions)
        {
            //using (var context = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            //{
            //    var quiz = context.Quizs.FirstOrDefault(x => x.Id == quizId);
            //    var blockId = context.QuizBlocks.First(x => x.QuizId == quizId).Id;

            //    // deleting questions
            //    var quizQuestions = context.QuizQuestions.Where(qq => qq.QuizBlockId == blockId).ToList();
            //    for (int i = 0; i < questions.Length; i++)
            //    {
            //        int currentQuestionId = questions[i].Id;
            //        QuizQuestion matchedQuizQuestion = context.QuizQuestions.Where(qq => qq.QuestionId == currentQuestionId && qq.QuizBlockId == blockId).FirstOrDefault();
            //        if (quizQuestions.Contains(matchedQuizQuestion))
            //        {
            //            quizQuestions.Remove(matchedQuizQuestion);
            //        }
            //        if (quizQuestions != null)
            //        {
            //            foreach (var item in quizQuestions)
            //            {
            //                context.QuizQuestions.Remove(item); //test it 
            //            }
            //        }
            //    }
            //    context.SaveChanges();
            //}
            var quiz = _repository.GetSingle<Quiz>(x => x.Id == quizId);
            var blockId = _repository.GetSingle<QuizBlock>(x => x.QuizId == quizId).Id;

            var quizQuestions = _repository.Get<QuizQuestion>(qq => qq.QuizBlockId == blockId).ToList();
            for (int i = 0; i < questions.Length; i++)
            {
                int currentQuestionId = questions[i].Id;
                QuizQuestion matchedQuizQuestion = _repository.GetSingle<QuizQuestion>(qq => qq.QuestionId == currentQuestionId && qq.QuizBlockId == blockId);
                if (quizQuestions.Contains(matchedQuizQuestion))
                {
                    quizQuestions.Remove(matchedQuizQuestion);
                }
                if (quizQuestions != null)
                {
                    foreach (var item in quizQuestions)
                    {
                        _repository.Delete<int, QuizQuestion>("Id", item.Id);
                    }
                }
            }
        }

        private string[] QuestionsValidation(int quizId, Question[] questions, Answer[][] answers, Tag[][] tags)
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
                            if (string.IsNullOrEmpty(answers[i][j].AnswerText))
                            {
                                isAllAnswersHaveText = false;
                            }
                            chechedCount = (bool)answers[i][j].IsRight ? chechedCount++ : chechedCount;
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

        public ActionResult Get(int id)
        {
            List<Question> questions = new List<Question>();
            List<List<Answer>> answers = null;
            List<List<Tag>> tags = null;
            int quizId = 0;
            //using (var context = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            //{
                //var quiz = context.Quizs.Where(x => x.Id == id).FirstOrDefault();  //check if exists
                var quiz = _repository.GetSingle<Quiz>(q => q.Id == id);
                if (quiz == null)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Quiz is not found");
                }

                // var quizState = context.QuizStates.FirstOrDefault(state => state.Id == quiz.QuizStateId).Name;
                var quizState = _repository.GetSingle<QuizState>(qs=>qs.Id==quiz.QuizStateId).Name;

                if (quizState != "Archived")
                {
                    quizId = quiz.Id;
                    //var quizBlockIds = context.QuizBlocks.Where(b => b.QuizId == quizId).Select(b => b.Id).ToList();
                    //var quizQuestios = context.QuizQuestions.Where(x => quizBlockIds.Contains(x.QuizBlockId)).OrderBy(x => x.QuestionOrder).ToList();
                    var quizBlockIds = _repository.Get<QuizBlock>(qb => qb.Id == quizId).Select(qb=>qb.Id).ToList();
                    var quizQuestios = _repository.Get<QuizQuestion>(qq => quizBlockIds.Contains(qq.QuizBlockId)).ToList();

                    foreach (var quizQuestion in quizQuestios)
                    {
                        // questions.Add(context.Questions.Where(q => q.Id == quizQuestion.QuestionId).Include(q => q.QuestionAnswers).Include(q => q.QuestionTags).First());
                        questions.Add(_repository.GetSingle<Question>(q => q.Id == quizQuestion.QuestionId, q => q.QuestionAnswers, q => q.QuestionTags));

                    }

                    answers = new List<List<Answer>>();
                    foreach (var item in questions)
                    {
                        // var questionAnswers = context.QuestionAnswers.Where(x => x.QuestionId == item.Id).Include("Answer").ToList();
                        var questionAnswers = _repository.Get<QuestionAnswer>(x => x.QuestionId == item.Id, x=>x.Answer).ToList();
                        var answerStorage = new List<Answer>();
                        foreach (var answer in questionAnswers)
                        {
                            answerStorage.Add(answer.Answer);
                        }
                        answers.Add(answerStorage);
                    }

                    tags = new List<List<Tag>>();
                    foreach (var item in questions)
                    {
                       // var questionTags = context.QuestionTags.Where(x => x.QuestionId == item.Id).Include("Tag").ToList();
                        var questionTags = _repository.Get<QuestionTag>(x => x.QuestionId == item.Id, x=>x.Tag).ToList();

                        var tagStorage = new List<Tag>();
                        foreach (var tag in questionTags)
                        {
                            tagStorage.Add(tag.Tag);
                        }
                        tags.Add(tagStorage);
                    }

                }
                var data = JsonConvert.SerializeObject(new { questions = questions, answers = answers, id = quizId, tags = tags }, Formatting.None,
                                                        new JsonSerializerSettings()
                                                        {
                                                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                        });

                return Content(data, "application/json");

           // }
        }
    }
}