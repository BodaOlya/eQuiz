using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Admin.Models
{
    public class TestAnswer : IEquatable<TestAnswer>
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Name { get; set; }
        public bool IsRight { get; set; }
        public byte? Order { get; set; }
        public bool ChosenByUser { get; set; }

        public TestAnswer (int id, int questionId, string name, bool isRight, byte? order, bool chosenByUser)
        {
            Id = id;
            QuestionId = questionId;
            Name = name;
            IsRight = isRight;
            Order = order;
            ChosenByUser = chosenByUser;
        }

        public bool Equals(TestAnswer other)
        {
            if (Id == other.Id && QuestionId == other.QuestionId && IsRight == other.IsRight)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int idHash = Id.ToString().GetHashCode();
            int qIdHash = QuestionId.ToString().GetHashCode();
            int nameId = Name.GetHashCode();
            int isRightHash = IsRight.ToString().GetHashCode();

            return idHash + qIdHash + nameId + isRightHash;
        }
    }
}