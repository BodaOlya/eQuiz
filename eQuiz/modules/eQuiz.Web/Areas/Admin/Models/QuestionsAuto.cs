using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Admin.Models
{
    public class QuestionsAuto
    {
        public int QuizId { get; set; }
        public int IsAutomatic { get; set; }
    }
}