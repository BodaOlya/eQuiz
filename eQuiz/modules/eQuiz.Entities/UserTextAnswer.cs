//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eQuiz.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserTextAnswer
    {
        public int Id { get; set; }
        public System.DateTime AnswerTime { get; set; }
        public string AnswerText { get; set; }
        public int QuizPassQuestionId { get; set; }
    
        public virtual QuizPassQuestion QuizPassQuestion { get; set; }
    }
}
