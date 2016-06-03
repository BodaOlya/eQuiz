using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Admin.Models
{
    public class SelectQuestion : QuestionBase
    {
        List<TestAnswer> QuestionVariants;

        public SelectQuestion(int id, int maxScore, int userScore, string questionText, List<TestAnswer> questionVariants, short? order)
            : base(id, maxScore, userScore, questionText, order)
        {
            base.Type = "Select";
            QuestionVariants = questionVariants;
        }
    }
}