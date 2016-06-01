using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Admin.Models
{
    public class SelectQuestion : QuestionBase
    {
        // I don't give a fuck how to do here, will do at 1.06
        List<string> QuestionVariants;

        public SelectQuestion(int id, int maxScore, int userScore, string questionText, List<string> questionVariants, int order)
            : base(id, maxScore, userScore, questionText, order)
        {
            QuestionVariants = questionVariants;
        }
    }
}