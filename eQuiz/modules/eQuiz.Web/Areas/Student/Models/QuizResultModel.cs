using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Student.Models
{
    public class QuizResultModel
    {
        public int QuizId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime FinishDateTime { get; set; }
        public List<UserAnswer> UserAnswers { get; set; }
    }
}