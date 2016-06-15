ALTER TABLE [dbo].[tblFacebookUser] DROP CONSTRAINT [FK_tblFacebookUser_tblUser];

ALTER TABLE [dbo].[tblQuestionAnswer] DROP CONSTRAINT [FK_tblQuestionAnswer_tblQuestion];

ALTER TABLE [dbo].[tblQuestionAnswer] DROP  CONSTRAINT [FK_tblQuestionAnswer_tblAnswer]; 

ALTER TABLE [dbo].[tblQuestion] DROP CONSTRAINT [FK_tblQuestion_tblQuestionType];

ALTER TABLE [dbo].[tblQuestion] DROP CONSTRAINT [FK_tblQuestion_tblTopic];

ALTER TABLE [dbo].[tblQuestionTag] DROP CONSTRAINT [FK_tblQuestionTag_tblQuestion];

ALTER TABLE [dbo].[tblQuestionTag] DROP CONSTRAINT [FK_tblQuestionTag_tblTag];

ALTER TABLE [dbo].[tblQuizBlock] DROP CONSTRAINT [FK_tblQuizBlock_tblQuiz];

ALTER TABLE [dbo].[tblQuizBlock] DROP CONSTRAINT [FK_tblQuizBlock_Topic];

ALTER TABLE [dbo].[tblQuiz] DROP CONSTRAINT [FK_tblQuiz_tblQuizType];

ALTER TABLE [dbo].[tblQuiz] DROP CONSTRAINT [FK_tblQuiz_tblGroup];

ALTER TABLE [dbo].[tblQuiz] DROP CONSTRAINT [FK_tblQuiz_tblQuizState];

ALTER TABLE [dbo].[tblQuizEditHistory] DROP CONSTRAINT [FK_tblQuizEditHistory_tblUser];

ALTER TABLE [dbo].[tblQuizEditHistory] DROP CONSTRAINT [FK_tblQuizEditHistory_tblQuiz];

ALTER TABLE [dbo].[tblQuizPass] DROP CONSTRAINT [FK_tblQuizPass_tblQuiz];

ALTER TABLE [dbo].[tblQuizPass]  DROP CONSTRAINT [FK_tblQuizPass_tblUser];

ALTER TABLE [dbo].[tblQuizPassQuestion] DROP CONSTRAINT [FK_tblQuizPassQuestion_tblQuestion];

ALTER TABLE [dbo].[tblQuizPassQuestion] DROP CONSTRAINT [FK_tblQuizPassQuestion_QuizBlock];

ALTER TABLE [dbo].[tblQuizPassQuestion] DROP CONSTRAINT [FK_tblQuizPassQuestion_tblQuizPass];

ALTER TABLE [dbo].[tblQuizPassScore] DROP CONSTRAINT [FK_tblQuizPassScore_tblQuizPass];

ALTER TABLE [dbo].[tblQuizPassScore] DROP CONSTRAINT [FK_tblQuizPassScore_tblUser];

ALTER TABLE [dbo].[tblQuizQuestion] DROP CONSTRAINT [FK_tblQuizQuestion_tblQuestion];

ALTER TABLE [dbo].[tblQuizQuestion] DROP CONSTRAINT [FK_tblQuizQuestion_tblQuizBlock];

ALTER TABLE [dbo].[tblQuizQuestion] DROP CONSTRAINT [FK_tblQuizQuestion_tblQuizVariant];

ALTER TABLE [dbo].[tblQuizVariant] DROP CONSTRAINT [FK_tblQuizVariant_tblQuiz];

ALTER TABLE [dbo].[tblUserAnswer] DROP CONSTRAINT [FK_tblUserAnswer_tblAnswer];

ALTER TABLE [dbo].[tblUserAnswer] DROP CONSTRAINT [FK_tblUserAnswer_tblQuizPassQuestion];

ALTER TABLE [dbo].[tblUserAnswerScore] DROP CONSTRAINT [FK_tblUserAnswerScore_tblQuizPassQuestion];

ALTER TABLE [dbo].[tblUserAnswerScore] DROP CONSTRAINT [FK_tblUserAnswerScore_tblUser];

ALTER TABLE [dbo].[tblUserToUserGroup] DROP CONSTRAINT [FK_tblUserToUserGroup_tblUserGroup];

ALTER TABLE [dbo].[tblUserToUserGroup] DROP CONSTRAINT [FK_tblUserToUserGroup_tblUser];

ALTER TABLE [dbo].[tblUserTextAnswer] DROP CONSTRAINT [FK_tblUserTextAnswer_tblQuizPassQuestion];

ALTER TABLE [dbo].[tblUserComment] DROP CONSTRAINT [FK_tblUserComment_AdminId_tblUser];

ALTER TABLE [dbo].[tblUserComment] DROP CONSTRAINT [FK_tblUserComment_UserId_tblUser];

ALTER TABLE [dbo].[tblUserGroup] DROP CONSTRAINT [FK_tblUserGroup_tblUserGroupState];

ALTER TABLE [dbo].[tblUserGroup] DROP CONSTRAINT [FK_tblUserGroup_tblUser];

ALTER TABLE [dbo].[tblUserTextAnswer] DROP CONSTRAINT [FR_tblUserTextAnswer_tblUser];

DROP TABLE [tblUserComment];

DROP TABLE [tblUserTextAnswer];
 
DROP TABLE [tblUserToUserGroup];
 
DROP TABLE [tblUserAnswerScore];
 
DROP TABLE [tblFacebookUser];
 
DROP TABLE [tblQuizPassScore];
 
DROP TABLE [tblUserAnswer];

DROP TABLE [tblAnswer];
 
DROP TABLE [tblQuizPassQuestion];
 
DROP TABLE [tblQuizPass];
 
DROP TABLE [tblUser];
 
DROP TABLE [tblQuestionAnswer];
 
DROP TABLE [tblQuestionTag];
 
DROP TABLE [tblTag];
 
DROP TABLE [tblQuizQuestion];
 
DROP TABLE [tblQuestion];
 
DROP TABLE [tblQuestionType];
 
DROP TABLE [tblQuizBlock];
 
DROP TABLE [tblTopic];
 
DROP TABLE [tblQuizVariant];
 
DROP TABLE [tblQuiz];

DROP TABLE [tblQuizState];
 
DROP TABLE [tblUserGroup];
 
DROP TABLE [tblQuizType];
 
DROP TABLE [tblQuizEditHistory];

DROP TABLE [tblUserGroupState];