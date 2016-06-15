using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace eQuiz.Web.Areas.Admin.Models
{
    public class QuizInfo : IEquatable<QuizInfo>
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
            else if (autoQuestionCount == overallCount)
            {
                return "Auto";
            }
            else { return "Manual"; }
        }

        public bool Equals(QuizInfo other)
        {
            if (id == other.id && name == other.name)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int idHash = id.ToString().GetHashCode();
            int nameHash = name.GetHashCode();
            int questionsHash = questions.ToString().GetHashCode();
            int otherDetailsHash = otherDetails.GetHashCode();
            int dateHash = date.ToString().GetHashCode();

            return idHash + nameHash + questionsHash + otherDetailsHash + dateHash;
        }
    }
}