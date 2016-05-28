﻿using System;
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

            var studentAmount = from ug in userGroup
                                join utg in userToGroup on ug.Id equals utg.GroupId
                                join u in user on utg.UserId equals u.Id
                                group new { ug, u } by new { ug.Id, ug.Name } into grouped
                                select new
                                {                                    
                                    groupId = grouped.Key,
                                    studentAmount = grouped.Select(item => item.u.Id).Count()
                                };

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
                        join sa in studentAmount on ug.Id equals sa.groupId.Id
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

        #endregion

    }
}