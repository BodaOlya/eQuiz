using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Admin.Models
{
    public class SelectQuestion : QuestionBase
    {
        public IEnumerable<TestAnswer> QuestionVariants;

        public SelectQuestion(int id, byte maxScore, double? userScore, string questionText, IEnumerable<TestAnswer> questionVariants, short? order, bool wasChanged, bool wasNull)
            : base(id, maxScore, userScore, questionText, order, wasChanged, wasNull)
        {
            base.Type = "Select";
            QuestionVariants = questionVariants;
        }
    }
}