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

        public TextQuestion(int id, byte maxScore, int? userScore, string questionText, string answer, string rightAnswer, short? order)
            : base(id, maxScore, userScore, questionText, order)
        {
            base.Type = "Text";
            Answer = answer;
            RightAnswer = rightAnswer;
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