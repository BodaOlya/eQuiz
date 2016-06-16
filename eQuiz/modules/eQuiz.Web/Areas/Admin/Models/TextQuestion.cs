using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Admin.Models
{
    public class TextQuestion : QuestionBase
    {
        public string Answer { get; set; }
        public string RightAnswer { get; set; }
        public bool IsPassed { get; set; }
        public int? EvaluatedBy { get; set; }

        public TextQuestion(int id, byte maxScore, int 
            userScore, string questionText, string answer, string rightAnswer, short? order, bool isPassed, int? evaluatedBy)
            : base(id, maxScore, userScore, questionText, order)
        {
            base.Type = "Text";
            Answer = answer;
            RightAnswer = rightAnswer;
            IsPassed = isPassed;
            EvaluatedBy = evaluatedBy;
        }

        public static string GetAnswer(string answer)
        {
            if (answer.Length == 0)
            {
                return "No right answer in the database";
            }

            return answer;
        }
    }
}