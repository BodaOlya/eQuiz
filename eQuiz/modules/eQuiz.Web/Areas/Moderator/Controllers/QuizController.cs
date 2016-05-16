using eQuiz.Entities;
using eQuiz.Web.Areas.Moderator.Models;
using eQuiz.Web.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eQuiz.Web.Areas.Moderator.Controllers
{
    public class QuizController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return RedirectToAction("Edit");
        }

        public ActionResult Edit()
        {
            return View();
        }

        [HttpGet]
        public ActionResult IsNameUnique(string name)
        {
            Quiz quiz = null;
            using(eQuizEntities model = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            {
                quiz = model.Quizs.FirstOrDefault(q => q.Name == name);
            }

            return Json(quiz == null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            Quiz quiz = null;
            QuizBlock block = null;
            using (eQuizEntities model = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            {
                quiz = model.Quizs.Include("UserGroup").FirstOrDefault(q => q.Id == id);
                block = model.QuizBlocks.FirstOrDefault(b => b.QuizId == id);
            }

            var data = JsonConvert.SerializeObject(new { quiz = quiz, block = block }, Formatting.None,
                                                    new JsonSerializerSettings()
                                                    {
                                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                    });

            return Content(data, "application/json");
        }

        [HttpGet]
        public ActionResult GetQuizzesPage(int currentPage = 1, int quizzesPerPage = 3, string predicate = "Name", bool reverse = false)
        {
            IEnumerable<QuizListModel> quizzesList = null;
            var quizzesTotal = 0;

            using (var context = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            {
                quizzesList = context.Quizs.Join(context.QuizBlocks,
                    quiz => quiz.Id,
                    quizBlock => quizBlock.QuizId,
                    (quiz, quizBlock) =>
                        new QuizListModel
                        {
                            Id = quiz.Id,
                            Name = quiz.Name,
                            CountOfQuestions = quizBlock.QuestionCount,
                            StartDate = quiz.StartDate,
                            Duration = quiz.TimeLimitMinutes,
                            Active = false
                        }).OrderBy(q => q.Name);

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
                        quizzesList = reverse ? quizzesList.OrderByDescending(q => q.StartDate) : quizzesList.OrderBy(q => q.StartDate);
                        break;
                    case "Active":
                        quizzesList = reverse ? quizzesList.OrderByDescending(q => q.Active) : quizzesList.OrderBy(q => q.Active);
                        break;
                    case "Duration":
                        quizzesList = reverse ? quizzesList.OrderByDescending(q => q.Duration) : quizzesList.OrderBy(q => q.Duration);
                        break;                    
                    default:
                        quizzesList = reverse ? quizzesList.OrderByDescending(q => q.Name) : quizzesList.OrderBy(q => q.Name);
                        break;
                }

                quizzesList = quizzesList.Skip((currentPage - 1) * quizzesPerPage).Take(quizzesPerPage).ToList();
            }
            return Json(new { Quizzes = quizzesList, QuizzesTotal = quizzesTotal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(Quiz quiz, QuizBlock block)
        {
            if (quiz.Id != 0)
            {
                using (eQuizEntities model = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
                {
                    var updateQuiz = model.Quizs.FirstOrDefault(q => q.Id == quiz.Id);
                    if (updateQuiz == null)
                    {
                        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Quiz not found");
                    }

                    var updateBlock = model.QuizBlocks.FirstOrDefault(q => q.Id == quiz.Id);
                    if (updateBlock == null)
                    {
                        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "QuizBlock not found");
                    }

                    updateQuiz.Name = quiz.Name;
                    updateQuiz.QuizTypeId = quiz.QuizTypeId;
                    updateQuiz.StartDate = quiz.StartDate;
                    updateQuiz.EndDate = quiz.EndDate;
                    updateQuiz.TimeLimitMinutes = quiz.TimeLimitMinutes;
                    updateQuiz.GroupId = quiz.GroupId;

                    updateBlock.QuestionCount = block.QuestionCount;

                    model.SaveChanges();
                    quiz = updateQuiz;
                    quiz.UserGroup = model.UserGroups.FirstOrDefault(g => g.Id == quiz.GroupId);
                    block = updateBlock;
                }
            }
            else
            {
                block.TopicId = 1;
                block.Quiz = quiz;

                using (eQuizEntities model = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
                {
                    quiz.UserGroup = model.UserGroups.Where(g => g.Id == quiz.UserGroup.Id).First();
                    model.Quizs.Add(quiz);
                    model.QuizBlocks.Add(block);
                  //  model.QuizVariants.Add(new QuizVariant() { QuizId = quiz.Id });   UPDATE DB
                    model.SaveChanges();
                }
            }
            var data = JsonConvert.SerializeObject(new { quiz = quiz, block = block }, Formatting.None,
                                                    new JsonSerializerSettings()
                                                    {
                                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                    });

            return Content(data, "application/json");
        }
    }
}
