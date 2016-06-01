using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Admin.Models
{
    public class SelectOneQuestion : SelectQuestion
    {
        public SelectOneQuestion(int id, int maxScore, int userScore, string questionText, List<string> questionVariants, int order) : base(id, maxScore, userScore, questionText, questionVariants, order)
        {
        }
    }
}