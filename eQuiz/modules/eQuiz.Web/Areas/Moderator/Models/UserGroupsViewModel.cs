using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eQuiz.Web.Areas.Moderator.Models
{
    public class UserGroupsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountOfStudents { get; set; }
        public int CountOfQuizzes { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string StateName { get; set; }
    }
}