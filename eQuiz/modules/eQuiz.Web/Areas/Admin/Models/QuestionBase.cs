using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Admin.Models
{
    public class QuestionBase : IQuestion
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int MaxScore { get; set; }
        public int UserScore { get; set; }
        public string QuestionText { get; set; }
        public int Order { get; set; }
        // Base constructor
        public QuestionBase (int id, int maxScore, int userScore, string questionText, int order)
        {
            Id = id;
            MaxScore = maxScore;
            UserScore = userScore;
            QuestionText = questionText;
            Order = order;
        }
    }
}