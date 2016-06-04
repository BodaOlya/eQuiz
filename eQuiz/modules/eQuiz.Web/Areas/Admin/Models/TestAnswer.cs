using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Admin.Models
{
    public class TestAnswer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRight { get; set; }
        public int Order { get; set; }
        public bool ChosenByUser { get; set; }

        public TestAnswer (int id, string name, bool isRight, int order, bool chosenByUser)
        {
            Id = id;
            Name = name;
            IsRight = isRight;
            Order = order;
            ChosenByUser = ChosenByUser;
        }
    }
}