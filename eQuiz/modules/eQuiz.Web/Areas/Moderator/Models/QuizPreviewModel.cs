using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eQuiz.Entities;

namespace eQuiz.Web.Areas.Moderator.Models
{
    public class QuizPreviewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Descriprtion { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public short? TimeLimitMinutes { get; set; }
        public bool InternetAccess { get; set; }
        public string Group { get; set; }
        public string State { get; set; }
        public string Topic { get; set; }
        public bool IsRandom { get; set; }
        public byte? QuestionMinComplexity { get; set; }
        public byte? QuestionMaxComplexity { get; set; }
        public byte? QuestionCount { get; set; }
        public List<QuestionInfo> Questions { get; set; }
    }

    public class QuestionInfo
    {
        public byte QuestionScore { get; set; }
        public short? QuestionOrder { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public byte QuestionComplexity { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
