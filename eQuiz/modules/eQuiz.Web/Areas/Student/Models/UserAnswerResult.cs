using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Student.Models
{
    public class UserAnswerResult
    {
        public int QuestionId { get; set; }
        public int?[] Answers { get; set; }
        public int? AnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool IsAutomatic { get; set; }
        public int QuizBlock { get; set; }
        public short QuestionOrder { get; set; }
        public DateTime AnswerTime { get; set; }
    }
}