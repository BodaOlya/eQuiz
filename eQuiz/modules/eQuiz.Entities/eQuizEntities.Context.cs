﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class eQuizEntities : DbContext
    {
        public eQuizEntities()
            : base("name=eQuizEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<FacebookUser> FacebookUsers { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        public virtual DbSet<QuestionTag> QuestionTags { get; set; }
        public virtual DbSet<QuestionType> QuestionTypes { get; set; }
        public virtual DbSet<Quiz> Quizs { get; set; }
        public virtual DbSet<QuizBlock> QuizBlocks { get; set; }
        public virtual DbSet<QuizEditHistory> QuizEditHistories { get; set; }
        public virtual DbSet<QuizPass> QuizPasses { get; set; }
        public virtual DbSet<QuizPassQuestion> QuizPassQuestions { get; set; }
        public virtual DbSet<QuizPassScore> QuizPassScores { get; set; }
        public virtual DbSet<QuizQuestion> QuizQuestions { get; set; }
        public virtual DbSet<QuizState> QuizStates { get; set; }
        public virtual DbSet<QuizType> QuizTypes { get; set; }
        public virtual DbSet<QuizVariant> QuizVariants { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAnswer> UserAnswers { get; set; }
        public virtual DbSet<UserAnswerScore> UserAnswerScores { get; set; }
        public virtual DbSet<UserComment> UserComments { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserGroupState> UserGroupStates { get; set; }
        public virtual DbSet<UserTextAnswer> UserTextAnswers { get; set; }
        public virtual DbSet<UserToUserGroup> UserToUserGroups { get; set; }
    }
}
