using eQuiz.Entities;
using eQuiz.Repositories.Abstract;
using eQuiz.Web.Code;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

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
            var quizScores = _repository.Get<QuizPassScore>();
            var quizBlock = _repository.Get<QuizBlock>();
            var quizPassQuestions = _repository.Get<QuizPassQuestion>();
            var userAnswerScores = _repository.Get<UserAnswerScore>();
            var quizQuestions = _repository.Get<QuizQuestion>();

            var questionCountQuery = from qpq in quizPassQuestions
                                     join qp in quizPasses on qpq.QuizPassId equals qp.Id
                                     group new { qpq, qp } by qp.Id into changed
                                     select new
                                     {
                                         id = changed.Key,
                                         questions = changed.Count(ch => ch.qpq.Id > 0)
                                     };

            var userScores = from uas in userAnswerScores
                             join qpq in quizPassQuestions on uas.QuizPassQuestionId equals qpq.Id
                             join qp in quizPasses on qpq.QuizPassId equals qp.Id
                             join qcq in questionCountQuery on qp.Id equals qcq.id
                             group new { qp, uas, qcq } by qp.Id into changed
                             select new
                             {
                                 id = changed.Key,
                                 scores = changed.Sum(ch => ch.uas.Score),
                                 passed = changed.Count(ch => ch.uas.Score > 0),
                                 notPassed = changed.Count(ch => ch.uas.Score == 0),
                                 inVerification = (int)changed.Select(ch => ch.qcq.questions).First() - changed.Count(ch => ch.uas.Score > 0) - changed.Count(ch => ch.uas.Score == 0)
                             };

            var query = from qp in quizPasses
                        join u in users on qp.UserId equals u.Id
                        where qp.QuizId == id
                        join us in userScores on qp.Id equals us.id
                        join qs in quizScores on qp.Id equals qs.QuizPassId into temp
                        from t in temp.DefaultIfEmpty()
                        select new
                        {
                            id = u.Id,
                            quizPassId = qp.Id,
                            student = u.FirstName + " " + u.LastName,
                            email = u.Email,
                            studentScore = us.scores,
                            quizStatus = t == null ? "In Verification" : "Passed",
                            questionDetails = new {
                                passed = us.passed,
                                notPassed = us.notPassed,
                                inVerification = us.inVerification
                            },
                        };

            foreach (var item in query)
            {
                res.Add(item);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuiz(int id)
        {
            var result = new List<object>();

            var quizzPasses = _repository.Get<QuizPass>();            
            var quiz = _repository.Get<Quiz>();
            var ugroup = _repository.Get<UserGroup>();            
            var quizBlock = _repository.Get<QuizBlock>();
            var quizQuestions = _repository.Get<QuizQuestion>();

            var query = from q in quiz
                        join ug in ugroup on q.GroupId equals ug.Id
                        join qp in quizzPasses on q.Id equals qp.QuizId
                        where qp.QuizId == id
                        join qb in quizBlock on qp.QuizId equals qb.QuizId
                        join qq in quizQuestions on qb.Id equals qq.QuizBlockId
                        group new { q, ug, qq, qp } by qp.Id into changed
                        select new
                        {
                            quizId = changed.Select(ch => ch.q.Id).Distinct(),
                            quizName = changed.Select(ch => ch.q.Name).Distinct(),
                            groupName = changed.Select(ch => ch.ug.Name).Distinct(),
                            quizScore = changed.Sum(ch => ch.qq.QuestionScore),
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

        [HttpGet]
        [WebMethod]
        public void ExportToExcel(string nameOfFile, string pathToFile, string currentUrl, string[] data)
        {
            if (data != null && data.Length > 0)
            {
                // Removeing of symbols, which may lead to error
                while (nameOfFile.IndexOf(':') != -1)
                {
                    nameOfFile = nameOfFile.Remove(nameOfFile.IndexOf(':'), 1);
                }
                while (nameOfFile.IndexOf('?') != -1)
                {
                    nameOfFile = nameOfFile.Remove(nameOfFile.IndexOf('?'), 1);
                }
                while (nameOfFile.IndexOf('#') != -1)
                {
                    nameOfFile = nameOfFile.Replace("#", "Sharp");
                }
                nameOfFile += ".xlsx";

                // Creating a variable with path to file on server side
                string fileName = Path.Combine(Server.MapPath("~/Areas/Admin/"), nameOfFile);
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }

                // Sorting data
                var dataForExport = new JObject[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    dataForExport[i] = JObject.Parse(data[i]);
                }
                dataForExport.OrderBy(obj => obj["student"]);

                // Creating an excel file and filling it with data
                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0 Xml;HDR=Yes'";
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string createTable = "Create table Rezults1 (" +
                        "[Student] varchar(200), " +
                        "[Email] varchar(50), " +
                        "[Score] int)";
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    OleDbCommand command = new OleDbCommand(createTable, connection);
                    command.ExecuteNonQuery();

                    string insertData = "Insert into Rezults1([Student], [Email], [Score]) values(?,?,?)";
                    OleDbCommand insertCommand = new OleDbCommand(insertData, connection);
                    insertCommand.Parameters.Add("?", OleDbType.VarChar, 200);
                    insertCommand.Parameters.Add("?", OleDbType.VarChar, 50);
                    insertCommand.Parameters.Add("?", OleDbType.Integer);

                    for (int i = 0; i < dataForExport.Length; i++)
                    {
                        insertCommand.Parameters[0].Value = (string)dataForExport[i]["student"];
                        insertCommand.Parameters[1].Value = (string)dataForExport[i]["email"];
                        insertCommand.Parameters[2].Value = (int)dataForExport[i]["score"];

                        insertCommand.ExecuteNonQuery();
                    }
                }

                // Downloading created file
                currentUrl = currentUrl.Remove(currentUrl.IndexOf("Admin/")) + "/Areas/Admin/";
                Uri fileUri = new Uri(currentUrl + nameOfFile);
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(fileUri,
                        pathToFile + nameOfFile);
                }

                // Deleting created file from server
                System.IO.File.Delete(fileName);
            }
        }

        #endregion
    }
}