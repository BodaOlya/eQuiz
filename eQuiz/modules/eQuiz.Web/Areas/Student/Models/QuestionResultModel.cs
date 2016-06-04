using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Student.Models
{
    public class QuestionResultModel
    {
        public int QuestionId { get; set; }
        public string QuestionType { get; set; }
        public short QuestionOrder { get; set; }
        public int QuizBlock { get; set; }
        public int QuizPassId { get; set; }
        public int? AnswerId { get; set; }
        public string AnswerText { get; set; }
        public List<int?> Answers { get; set; }
        public DateTime AnswerTime { get; set; }
    }
}