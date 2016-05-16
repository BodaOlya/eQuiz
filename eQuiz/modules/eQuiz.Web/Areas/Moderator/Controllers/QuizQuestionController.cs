using eQuiz.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;
using eQuiz.Web.Code;

namespace eQuiz.Web.Areas.Moderator.Controllers
{
    public class QuizQuestionController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetQuestionTypes()
        {
            using (var context = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            {
                var typesList = context.QuestionTypes.OrderBy(x => x.TypeName).ToList();
                return Json(typesList, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult Save(int id, Question[] questions, QuestionAnswer[][] answers, Tag[][] tags)
        {
            if (questions == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Can not save quiz without question.");
            }
            using (var context = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            {
                var topicId = context.Topics.First().Id;
                var quizVariantId = 1; /*context.QuizVariants.First(x => x.QuizId == id).Id;*/ //we don't have fk on tblQuiz
                var blockId = context.QuizBlocks.First(x => x.QuizId == id).Id;
                var newQuestions = questions.Where(q => q.Id == 0).ToList();

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
                            QuestionId= question.Id,
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
                    var questionAnswer = context.Questions.Include("QuestionAnswers").FirstOrDefault(y => y.Id == questionId).QuestionAnswers.ToList();

                    if (answers[i][0] != null)
                    {
                        for (var qa = 0; qa < answers[i].Length; qa++)
                        {
                            var answer = answers[i][qa];

                            if (answer.Id != 0)
                            {
                                var existedAnswer = context.QuestionAnswers.FirstOrDefault(x => x.Id == answer.Id);

                                if (existedAnswer == null)
                                {
                                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Answer is not found");
                                }

                                if (questionAnswer.Contains(existedAnswer))
                                {
                                    questionAnswer.Remove(existedAnswer);
                                }
                                existedAnswer.AnswerOrder = answer.AnswerOrder;
                                existedAnswer.AnswerText = answer.AnswerText;
                                existedAnswer.IsRight = answer.IsRight;
                                existedAnswer.QuestionId = answer.QuestionId;
                            }
                            else
                            {
                                answer.AnswerOrder = (byte)(qa + 1);
                                answer.QuestionId = questions[i].Id;
                                context.QuestionAnswers.Add(answer);
                            }
                        }
                        if (questionAnswer != null)
                        {
                            foreach (var item in questionAnswer)
                            {
                                context.QuestionAnswers.Remove(item);//todo
                            }
                        }
                    }
                    //todo doesn't delete tags 
                 
                    if (tags[i][0] != null)
                    {
                        for (int qt = 0; qt < tags[i].Length; qt++)
                        {
                            var tag = tags[i][qt];

                           var existedTag = context.Tags.FirstOrDefault(x => x.Name == tag.Name);

                            var question = context.Questions.FirstOrDefault(x => x.Id == questionId);

                            if (existedTag == null)
                            {
                                context.Tags.Add(tag);
                                question.QuestionTags.Add(new QuestionTag
                                {
                                    Tag = tag
                                });
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
            return RedirectToAction("Get", new { id = id });
        }

        public ActionResult Get(int id)
        {
            List<Question> questions = new List<Question>();
            List<List<QuestionAnswer>> answers = null;
            List<List<Tag>> tags = null;
            int quizId = 0;
            using (var context = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            {
                var quiz = context.Quizs.Where(x => x.Id == id).FirstOrDefault();  //check if exists

                if (quiz == null)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Quiz is not found");
                }
                quizId = quiz.Id;
                var quizBlockIds = context.QuizBlocks.Where(b => b.QuizId == quizId).Select(b => b.Id).ToList();
                var quizQuestios = context.QuizQuestions.Where(x => quizBlockIds.Contains(x.QuizBlockId)).OrderBy(x => x.QuestionOrder).ToList();

                foreach (var quizQuestion in quizQuestios)
                {
                    questions.Add(context.Questions.Where(q => q.Id == quizQuestion.QuestionId).Include(q => q.QuestionAnswers).Include(q => q.QuestionTags).First());
                }

                answers = new List<List<QuestionAnswer>>();
                foreach (var item in questions)
                {
                    answers.Add(item.QuestionAnswers.ToList());
                }

                tags = new List<List<Tag>>();
                foreach (var item in questions)
                {
                    var questionTags = context.QuestionTags.Where(x=>x.QuestionId == item.Id).Include("Tag").ToList();

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
        }
    }
}