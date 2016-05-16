using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Moderator.Models
{
    public class QuizListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte? CountOfQuestions { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public short? Duration { get; set; }
        public bool Active { get; set; }
    }
}