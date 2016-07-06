CREATE DATABASE [eQuiz];

GO

USE [eQuiz];

CREATE TABLE [dbo].[tblFacebookUser]
(
	[Id] [BIGINT] NOT NULL IDENTITY(1, 1),
	[UserId] [INT] NOT NULL,
	[UserName] [VARCHAR](50) NOT NULL,
	[FirstName] [NVARCHAR](50) NULL,
	[LastName] [NVARCHAR](50) NULL,
	[Email] [VARCHAR](50) NOT NULL,
	[ProfileLink] [VARCHAR](100) NULL,
	[ObtainedDate] [DATETIME] NOT NULL,
	CONSTRAINT [PK_tblFacebookUser_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [UK_tblFacebookUser_Email] UNIQUE ([Email]),
	CONSTRAINT [UK_tblFacebookUser_UserName] UNIQUE ([UserName])
);

--
CREATE TABLE [dbo].[tblAnswer]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[AnswerText] [NVARCHAR](MAX) NOT NULL,
	[AnswerOrder] [TINYINT] NULL,
	[IsRight] [BIT] NULL,
	CONSTRAINT [PK_tblAnswer_Id] PRIMARY KEY ([Id]) 
);

--
CREATE TABLE [dbo].[tblQuestionAnswer]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuestionId] [INT] NOT NULL,
	[AnswerId] [INT] NOT NULL,
	CONSTRAINT [PK_tblQuestionAnswer_Id] PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[tblQuestion]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuestionTypeId] [INT] NOT NULL,
	[TopicId] [INT] NOT NULL,
	[QuestionText] [NVARCHAR](max) NOT NULL,
	[QuestionComplexity] [TINYINT] NOT NULL,
	[IsActive] [BIT] NOT NULL,
	CONSTRAINT [PK_tblQuestion_Id] PRIMARY KEY ([Id]) 
);

CREATE TABLE [dbo].[tblQuestionTag]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuestionId] [INT] NOT NULL,
	[TagId] [INT] NOT NULL,
	CONSTRAINT [PK_tblQuestionTag_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [UK_tblQuestionTag_QuestionId_TagId] UNIQUE ([QuestionId], [TagId]) 
);

CREATE TABLE [dbo].[tblQuestionType]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[TypeName] [VARCHAR](20) NOT NULL,
	[IsAutomatic] [BIT] NOT NULL,
	CONSTRAINT [PK_tblQuestionType_Id] PRIMARY KEY ([Id]), 
	CONSTRAINT [UK_tblQuestionType_TypeName] UNIQUE ([TypeName]) 
);

CREATE TABLE [dbo].[tblQuizBlock]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuizId] [INT] NOT NULL,
	[TopicId] [INT] NOT NULL,
	[BlockOrder] [TINYINT] NULL,
	[IsRandom] [BIT] NOT NULL,
	[QuestionMinComplexity] [TINYINT] NULL,
	[QuestionMaxComplexity] [TINYINT] NULL,
	[QuestionCount] [TINYINT] NULL,
	CONSTRAINT [PK_tblQuizBlock_Id] PRIMARY KEY ([Id]), 
	CONSTRAINT [UK_tblQuizBlock_QuizId_TopicId] UNIQUE ([QuizId], [TopicId])
);

CREATE TABLE [dbo].[tblQuiz]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuizTypeId] [INT] NOT NULL,
	[QuizStateId] [INT] NOT NULL,
	[Name] [NVARCHAR](50) NOT NULL,
	[Description] [NVARCHAR](MAX) NULL,
	[StartDate] [SMALLDATETIME] NULL,
	[EndDate] [SMALLDATETIME] NULL,
	[TimeLimitMinutes] [SMALLINT] NULL,
	[InternetAccess] [BIT] NOT NULL,
	[GroupId] [INT] NULL,
	CONSTRAINT [PK_tblQuiz_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [UK_tblQuiz_Name] UNIQUE ([Name]),
	CONSTRAINT [CK_tblQuiz_StartDate_EndDate] CHECK ([StartDate] <= [EndDate])
 ); 

 CREATE TABLE [dbo].[tblQuizEditHistory]
 (
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuizId] [INT] NOT NULL,
	[UserId] [NVARCHAR](128) NOT NULL,
	[StartDate] [DATETIME] NOT NULL,
	[LastChangeDate] [DATETIME] NOT NULL,
	CONSTRAINT [PK_tblQuizEditHistory_Id] PRIMARY KEY ([Id])
 );

 CREATE TABLE [dbo].[tblQuizState]
 (
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[Name] [NVARCHAR](50) NOT NULL,
	CONSTRAINT [PK_tblQuizState_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [UK_tblQuizState_Name] UNIQUE ([Name])
 );

CREATE TABLE [dbo].[tblQuizPass]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuizId] [INT] NOT NULL,
	[UserId] [INT] NOT NULL,
	[StartTime] [DATETIME] NOT NULL,
	[FinishTime] [DATETIME] NULL,
	CONSTRAINT [PK_tblQuizPass_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [UK_tblQuizPass_QuizId_UserId_StartTime] UNIQUE ([QuizId], [UserId], [StartTime])
);

CREATE TABLE [dbo].[tblQuizPassQuestion]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuizPassId] [INT] NOT NULL,
	[QuestionId] [INT] NOT NULL,
	[QuizBlockId] [INT] NOT NULL,
	[QuestionOrder] [SMALLINT] NOT NULL,
	CONSTRAINT [PK_tblQuizPassQuestion_Id] PRIMARY KEY ([Id]), 
	CONSTRAINT [UK_tblQuizPassQuestion_QuizPassId_QuestionId] UNIQUE ([QuizPassId], [QuestionId])
); 

CREATE TABLE [dbo].[tblQuizPassScore]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuizPassId] [INT] NOT NULL,
	[PassScore] [SMALLINT] NOT NULL,
	[EvaluatedBy] [INT] NOT NULL,
	[EvaluatedAt] [SMALLDATETIME] NOT NULL,
	CONSTRAINT [PK_tblQuizPassScore_Id] PRIMARY KEY ([Id]), 
	CONSTRAINT [UK_tblQuizPassScore_QuizPassId] UNIQUE ([QuizPassId])
);

CREATE TABLE [dbo].[tblQuizQuestion]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuizBlockId] [INT] NOT NULL,
	[QuizVariantId] [INT] NULL, --May be not null
	[QuestionId] [INT] NOT NULL,
	[QuestionScore] [TINYINT] NOT NULL,
	[QuestionOrder] [SMALLINT] NULL,
	CONSTRAINT [PK_tblQuizQuestion] PRIMARY KEY ([Id], [QuizBlockId]) 
);

CREATE TABLE [dbo].[tblQuizType]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[TypeName] [VARCHAR](50) NOT NULL,
	CONSTRAINT [PK_tblQuizType_Id] PRIMARY KEY ([Id]) 
);

CREATE TABLE [dbo].[tblQuizVariant]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuizId] [INT] NOT NULL,
	[VariantNumber] [TINYINT] NOT NULL,
	CONSTRAINT [PK_tblQuizVariant_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [UK_tblQuizVariant_QuizId_VariantNumber] UNIQUE ([QuizId], [VariantNumber])
);

CREATE TABLE [dbo].[tblTag]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[Name] [NVARCHAR](20) NOT NULL,
	CONSTRAINT [PK_tblTag_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [UK_tblTag_Name] UNIQUE ([Name])
);

CREATE TABLE [dbo].[tblTopic]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[Name] [NVARCHAR](30) NOT NULL,
	[Description] [NVARCHAR](250) NULL,
	CONSTRAINT [PK_tblTopic_Id] PRIMARY KEY ([Id]), 
	CONSTRAINT [UK_tblTopic_Name] UNIQUE ([Name]) 
);

CREATE TABLE [dbo].[tblUserAnswer]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuizPassQuestionId] [INT] NOT NULL,
	[AnswerId] [INT] NOT NULL,
	[AnswerTime] [DATETIME] NOT NULL,
	CONSTRAINT [PK_tblUserAnswer_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [UK_tblUserAnswer_QuizPassQuestionId_AnswerId] UNIQUE ([QuizPassQuestionId], [AnswerId]) 
);

CREATE TABLE [dbo].[tblUserAnswerScore]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1), 
	[QuizPassQuestionId] [INT] NOT NULL, 
	[Score] [FLOAT] NOT NULL,
	[EvaluatedBy] [INT] NOT NULL,
	[EvaluatedAt] [SMALLDATETIME] NOT NULL,
	CONSTRAINT [PK_tblUserAnswerScore_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [UK_tblUserAnswerScore_QuizPassQuestionId] UNIQUE ([QuizPassQuestionId])
);

CREATE TABLE [dbo].[tblUserGroupState]
(
	[Id] [TINYINT] NOT NULL IDENTITY(1, 1),
	[Name] [NVARCHAR](50) NOT NULL,
	CONSTRAINT [PK_tblUserGroupState_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [UK_tblUserGroupState_Name] UNIQUE ([Name])
);

CREATE TABLE [dbo].[tblUserGroup]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[UserGroupStateId] [TINYINT] NOT NULL,
	[CreatedByUserId] [NVARCHAR](128) NOT NULL,
	[Name] [NVARCHAR](50) NOT NULL,
	[CreatedDate] [SMALLDATETIME] NOT NULL,
	CONSTRAINT [PK_tblUserGroup_Id] PRIMARY KEY ([Id]), 
	CONSTRAINT [UK_tblUserGroups_Name] UNIQUE ([Name]) 
);

CREATE TABLE [dbo].[tblUser]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[FirstName] [NVARCHAR](50) NOT NULL,
	[LastName] [NVARCHAR](50) NOT NULL,
	[FatheName] [NVARCHAR](50) NULL,
	[Email] [VARCHAR](50) NOT NULL,
	[Phone] [VARCHAR](20) NOT NULL,
	[IsEmailConfirmed] [BIT] NOT NULL,
	[PasswordHash] [VARCHAR](MAX) NULL,
	[SecurityStamp] [VARCHAR](MAX) NULL,
	[AspNetUserId] [NVARCHAR](128) NULL,
	CONSTRAINT [PK_tblUser_Id] PRIMARY KEY ([Id]), 
	CONSTRAINT [UK_tblUser_Email] UNIQUE ([Email])
); 

CREATE TABLE [dbo].[tblUserToUserGroup]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[UserId] [INT] NOT NULL,
	[GroupId] [INT] NOT NULL,
	CONSTRAINT [PK_tblUserToUserGroup_Id] PRIMARY KEY ([Id]), 
	CONSTRAINT [UK_UserToUserGroup_UserId_GroupId] UNIQUE ([UserId], [GroupId]) 
);

CREATE TABLE [dbo].[tblUserTextAnswer]
(
	[Id] [INT] NOT NULL IDENTITY(1, 1),
	[QuizPassQuestionId] [INT] NOT NULL,
	[AnswerTime] [DATETIME] NOT NULL,
	[AnswerText] [NVARCHAR](MAX) NOT NULL,
	CONSTRAINT [PK_tblUserTextAnswer_Id] PRIMARY KEY ([Id]) 
);

CREATE TABLE [dbo].[tblUserComment](
    Id int IDENTITY(1,1) NOT NULL,
    UserId int NOT NULL,
    AdminId [nvarchar](128) NOT NULL,
    CommentTime datetime NOT NULL,
    CommentText nvarchar(max) NOT NULL,     
    CONSTRAINT PK_tblUserComment_Id PRIMARY KEY ([Id]),
)

GO
 
ALTER TABLE [dbo].[tblUserComment] ADD CONSTRAINT [FK_tblUserComment_UserId_tblUser] FOREIGN KEY([UserId]) REFERENCES [dbo].[tblUser] ([Id]);

ALTER TABLE [dbo].[tblFacebookUser] ADD CONSTRAINT [FK_tblFacebookUser_tblUser] FOREIGN KEY([UserId]) REFERENCES [dbo].[tblUser] ([Id]);

ALTER TABLE [dbo].[tblQuestionAnswer] ADD CONSTRAINT [FK_tblQuestionAnswer_tblQuestion] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[tblQuestion] ([Id]);

ALTER TABLE [dbo].[tblQuestion] ADD CONSTRAINT [FK_tblQuestion_tblQuestionType] FOREIGN KEY([QuestionTypeId]) REFERENCES [dbo].[tblQuestionType] ([Id]);

ALTER TABLE [dbo].[tblQuestion] ADD CONSTRAINT [FK_tblQuestion_tblTopic] FOREIGN KEY([TopicId]) REFERENCES [dbo].[tblTopic] ([Id]);

ALTER TABLE [dbo].[tblQuestionTag] ADD CONSTRAINT [FK_tblQuestionTag_tblQuestion] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[tblQuestion] ([Id]);

ALTER TABLE [dbo].[tblQuestionTag] ADD CONSTRAINT [FK_tblQuestionTag_tblTag] FOREIGN KEY([TagId]) REFERENCES [dbo].[tblTag] ([Id]);

ALTER TABLE [dbo].[tblQuizBlock] ADD CONSTRAINT [FK_tblQuizBlock_tblQuiz] FOREIGN KEY([QuizId]) REFERENCES [dbo].[tblQuiz] ([Id]);

ALTER TABLE [dbo].[tblQuizBlock] ADD CONSTRAINT [FK_tblQuizBlock_Topic] FOREIGN KEY([TopicId]) REFERENCES [dbo].[tblTopic] ([Id]);

ALTER TABLE [dbo].[tblQuiz] ADD CONSTRAINT [FK_tblQuiz_tblQuizType] FOREIGN KEY([QuizTypeId]) REFERENCES [dbo].[tblQuizType] ([Id]);

ALTER TABLE [dbo].[tblQuiz] ADD CONSTRAINT [FK_tblQuiz_tblGroup] FOREIGN KEY([GroupId]) REFERENCES [dbo].[tblUserGroup] ([Id]);

ALTER TABLE [dbo].[tblQuiz] ADD CONSTRAINT [FK_tblQuiz_tblQuizState] FOREIGN KEY([QuizStateId]) REFERENCES [dbo].[tblQuizState] ([Id]);

ALTER TABLE [dbo].[tblQuizEditHistory] ADD CONSTRAINT [FK_tblQuizEditHistory_tblQuiz] FOREIGN KEY([QuizId]) REFERENCES [dbo].[tblQuiz] ([Id]);

ALTER TABLE [dbo].[tblQuizPass] ADD CONSTRAINT [FK_tblQuizPass_tblQuiz] FOREIGN KEY([QuizId]) REFERENCES [dbo].[tblQuiz] ([Id]);

ALTER TABLE [dbo].[tblQuizPass]  ADD CONSTRAINT [FK_tblQuizPass_tblUser] FOREIGN KEY([UserId]) REFERENCES [dbo].[tblUser] ([Id]);

ALTER TABLE [dbo].[tblQuizPassQuestion] ADD CONSTRAINT [FK_tblQuizPassQuestion_tblQuestion] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[tblQuestion] ([Id]);

ALTER TABLE [dbo].[tblQuizPassQuestion] ADD CONSTRAINT [FK_tblQuizPassQuestion_QuizBlock] FOREIGN KEY([QuizBlockId]) REFERENCES [dbo].[tblQuizBlock] ([Id]);

ALTER TABLE [dbo].[tblQuizPassQuestion] ADD CONSTRAINT [FK_tblQuizPassQuestion_tblQuizPass] FOREIGN KEY([QuizPassId]) REFERENCES [dbo].[tblQuizPass] ([Id]);

ALTER TABLE [dbo].[tblQuizPassScore] ADD CONSTRAINT [FK_tblQuizPassScore_tblQuizPass] FOREIGN KEY([QuizPassId]) REFERENCES [dbo].[tblQuizPass] ([Id]);

ALTER TABLE [dbo].[tblQuizPassScore] ADD  CONSTRAINT [FK_tblQuizPassScore_tblUser] FOREIGN KEY([EvaluatedBy]) REFERENCES [dbo].[tblUser] ([Id]);

ALTER TABLE [dbo].[tblQuizQuestion] ADD  CONSTRAINT [FK_tblQuizQuestion_tblQuestion] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[tblQuestion] ([Id]); --Remade

ALTER TABLE [dbo].[tblQuizQuestion] ADD  CONSTRAINT [FK_tblQuizQuestion_tblQuizBlock] FOREIGN KEY([QuizBlockId]) REFERENCES [dbo].[tblQuizBlock] ([Id]);

ALTER TABLE [dbo].[tblQuizQuestion] ADD  CONSTRAINT [FK_tblQuizQuestion_tblQuizVariant] FOREIGN KEY([QuizVariantId]) REFERENCES [dbo].[tblQuizVariant] ([Id]);

ALTER TABLE [dbo].[tblQuizVariant] ADD  CONSTRAINT [FK_tblQuizVariant_tblQuiz] FOREIGN KEY([QuizId]) REFERENCES [dbo].[tblQuiz] ([Id]);  ---------

ALTER TABLE [dbo].[tblUserAnswer] ADD  CONSTRAINT [FK_tblUserAnswer_tblAnswer] FOREIGN KEY([AnswerId]) REFERENCES [dbo].[tblAnswer] ([Id]); ----------

ALTER TABLE [dbo].[tblUserAnswer] ADD  CONSTRAINT [FK_tblUserAnswer_tblQuizPassQuestion] FOREIGN KEY([QuizPassQuestionId]) REFERENCES [dbo].[tblQuizPassQuestion] ([Id]);

ALTER TABLE [dbo].[tblUserAnswerScore] ADD  CONSTRAINT [FK_tblUserAnswerScore_tblQuizPassQuestion] FOREIGN KEY([QuizPassQuestionId]) REFERENCES [dbo].[tblQuizPassQuestion] ([Id]);

ALTER TABLE [dbo].[tblUserAnswerScore] ADD  CONSTRAINT [FK_tblUserAnswerScore_tblUser] FOREIGN KEY([EvaluatedBy]) REFERENCES [dbo].[tblUser] ([Id]);

ALTER TABLE [dbo].[tblUserToUserGroup] ADD  CONSTRAINT [FK_tblUserToUserGroup_tblUserGroup] FOREIGN KEY([GroupId]) REFERENCES [dbo].[tblUserGroup] ([Id]);

ALTER TABLE [dbo].[tblUserToUserGroup] ADD  CONSTRAINT [FK_tblUserToUserGroup_tblUser] FOREIGN KEY([UserId]) REFERENCES [dbo].[tblUser] ([Id]);

ALTER TABLE [dbo].[tblUserTextAnswer] ADD  CONSTRAINT [FK_tblUserTextAnswer_tblQuizPassQuestion] FOREIGN KEY([QuizPassQuestionId]) REFERENCES [dbo].[tblQuizPassQuestion] ([Id]);

ALTER TABLE [dbo].[tblQuestionAnswer] ADD  CONSTRAINT [FK_tblQuestionAnswer_tblAnswer] FOREIGN KEY([AnswerId]) REFERENCES [dbo].[tblAnswer] ([Id]); ---------------

ALTER TABLE [dbo].[tblUserGroup] ADD  CONSTRAINT [FK_tblUserGroup_tblUserGroupState] FOREIGN KEY([UserGroupStateId]) REFERENCES [dbo].[tblUserGroupState] ([Id]);



GO

USE [eQuiz];
GO

SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO
 
SET ANSI_PADDING ON
GO
 
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 
GO
 
SET ANSI_PADDING OFF
GO

USE [eQuiz];
GO

SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO
 
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Hometown] [nvarchar](max) NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 
GO

--ALTER TABLE [dbo].[tblUserComment] ADD CONSTRAINT [FK_tblUserComment_AdminId_AspNetUsers] FOREIGN KEY([AdminId]) REFERENCES [dbo].[AspNetUsers] ([Id]);
--GO

USE [eQuiz];
GO

SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO
 
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
 
GO

USE [eQuiz];
GO

SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO
 
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 
GO
 
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
 
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO

USE [eQuiz];
GO

SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO
 
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
 
GO
 
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
 
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO

USE [eQuiz];
GO

SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO
 
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
 
GO
 
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
 
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
 
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
 
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [FK_tblUser_AspNetUsers] FOREIGN KEY([AspNetUserid]) REFERENCES [dbo].[AspNetUsers] ([Id]);
GO

ALTER TABLE [dbo].[tblUserGroup] ADD  CONSTRAINT [FK_tblUserGroup_AspNetUsers] FOREIGN KEY([CreatedByUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]);
GO

ALTER TABLE [dbo].[tblQuizEditHistory] ADD CONSTRAINT [FK_tblQuizEditHistory_AspNetUsers] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]);
GO