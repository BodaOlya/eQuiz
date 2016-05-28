using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace eQuiz.Web.Areas.Admin.Models
{
    public class QuizInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public int questions { get; set; }
        public string verificationType { get; set; }
        public string otherDetails { get; set; }
        public DateTime? date { get; set; }

        public static string SetVerificationType(int autoQuestionCount, int overallCount)
        {
            if (autoQuestionCount < overallCount)
            {
                return new StringBuilder($"Combined [a: {autoQuestionCount}; m: {overallCount - autoQuestionCount}]").ToString();
            }
            else if (autoQuestionCount == 20)
            {
                return "Auto";
            }
            else { return "Manual"; }
        }
    }
}