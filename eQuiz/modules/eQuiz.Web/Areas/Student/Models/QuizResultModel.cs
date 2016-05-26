using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Student.Models
{
    public class QuizResultModel
    {
        public int QuizId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public List<UserAnswerResult> UserAnswers { get; set; }
    }
}