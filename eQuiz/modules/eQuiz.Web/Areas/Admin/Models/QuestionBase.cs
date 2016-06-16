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
        public byte MaxScore { get; set; }
        public int? UserScore { get; set; }
        public string QuestionText { get; set; }
        public short? Order { get; set; }

        // Base constructor
        public QuestionBase (int id, byte maxScore, int? userScore, string questionText, short? order)
        {
            Id = id;
            MaxScore = maxScore;
            UserScore = userScore;
            QuestionText = questionText;
            Order = order;
        }
    }
}