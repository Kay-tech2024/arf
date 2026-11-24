use arf
alter table RequestCoresearchers add ResearcherType smallint null;
go


alter table Requests add [CollegeId] [int] NULL;
go

update requests set collegeId = (select collegeId from researchers r where r.userId = requests.createdby) where collegeId is null;
go

update requests set collegeId = (select collegeId from researchers r where r.userId = (select createdby from requests r2 where r2.id = requests.publicationrequestid)) where collegeId is null;
go


alter table requests add constraint FK_Requests_Colleges FOREIGN KEY (CollegeID) REFERENCES Colleges (Id);
go





alter table RequestReferees add [ReceivedOn] [datetime] NULL;
go
alter table RequestReferees add [SubmittedOn] [datetime] NULL;
go


alter table RequestReferees add [HasVerifiedModifications] [bit] NULL DEFAULT ((0));
go


alter table RequestUsers add [ReceivedOn] [datetime] NULL;
go
alter table RequestUsers add [SubmittedOn] [datetime] NULL;
go

alter table RequestReferees add [NumberOfVotes] [int] NOT NULL DEFAULT ((0));
go



alter table users add NationalId nvarchar(15) null;


CREATE TABLE [dbo].[UserTemporaryUrls](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar] (128) NULL,
	[Key] [nvarchar](255) NOT NULL,
	[ShortCode] [int] NULL,
	[Type] [tinyint] NOT NULL,
	[Generated] [datetime] NULL,
	[GeneratedBy] [nvarchar](128) NOT NULL,
	[Expires] [datetime] NULL,
	[IsUsed] [bit] NOT NULL,
 CONSTRAINT [PK_UserTemporaryUrls] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserTemporaryUrls] ADD  DEFAULT (getdate()) FOR [Generated]
GO

ALTER TABLE [dbo].[UserTemporaryUrls] ADD  DEFAULT (dateadd(day,(1),CONVERT([date],getdate()))) FOR [Expires]
GO

ALTER TABLE [dbo].[UserTemporaryUrls] ADD  DEFAULT ((0)) FOR [IsUsed]
GO


ALTER TABLE [dbo].[UserTemporaryUrls]  WITH CHECK ADD  CONSTRAINT [FK_UserTemporaryUrls_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO


ALTER TABLE [dbo].[UserTemporaryUrls]  WITH CHECK ADD  CONSTRAINT [FK_UserTemporaryUrls_Users_GeneratedBy] FOREIGN KEY([GeneratedBy])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[UserTemporaryUrls] CHECK CONSTRAINT [FK_UserTemporaryUrls_Users_GeneratedBy]
GO





alter table userProfiles add [BankId] [int] NULL;
go
alter table userProfiles add [BankName] [nvarchar](255) NULL;
go
alter table userProfiles add [BankBranch] [nvarchar](255) NULL;
go
alter table userProfiles add [BankAccountHolder] [nvarchar](500) NULL;
go
alter table userProfiles add [BankIban] [nvarchar](128) NULL;
go

alter table userProfiles add constraint [FK_UserProfiles_Banks] FOREIGN KEY([BankId]) REFERENCES [dbo].[Banks] ([Id]);
go

alter table researchers drop constraint FK_Researchers_Banks
go

alter table researchers drop column BankId;
go
alter table researchers drop column iban;
go

alter table UserSessions add VerifiedMobile nvarchar(255) NULL;
go



drop table ThesisRequests;
go

drop table BookRequests;
go

drop table ReviewRequests;
go

drop table HandbookRequests;
go

drop table EncyclopediaRequests;
go

drop table DictionaryRequests;
go


alter table RequestFiles add PageCount int null
go

update requests set CollegeId = (select top 1 id from colleges)
where CollegeId not in (select id from Colleges)
go

alter table Requests add constraint [FK_Requests_Colleges] FOREIGN KEY([CollegeId]) REFERENCES [dbo].[Colleges] ([Id]);
go

alter table RequestUsers add Comments [nvarchar] (4000) null;
go




alter table RequestUsers add FileId [uniqueidentifier]  null;
go

alter table RequestUsers add FileName [nvarchar] (255) null;
go


alter table RequestUsers add constraint [FK_RequestUsers_RequestFiles] FOREIGN KEY([FileId]) REFERENCES [dbo].[RequestFiles] ([Id]);
go


alter table Templates alter column [SubjectArabic] [nvarchar](200) NULL;
go
alter table Templates alter column [SubjectEnglish] [nvarchar](200) NULL;
go
alter table Templates alter column [BodyArabic] [nvarchar](max) NULL;
go
alter table Templates alter column [BodyEnglish] [nvarchar](max) NULL;
go
alter table Templates alter column [IsHtml] [bit] NOT NULL;
go


alter table RequestHistoryLogs add CanSelectReferees bit NOT NULL DEFAULT ((0));
go


alter table Requests add RefereesApproved bit NOT NULL DEFAULT ((0));
go

alter table Referees add IsDeleted bit NOT NULL DEFAULT ((0));
go


alter table Researchers add IsDeleted bit NOT NULL DEFAULT ((0));
go


alter table Users add FullName nvarchar(255) NULL;
go

update users set fullname = (select firstname + ' '+ MiddleName + ' ' + thirdName + ' ' lastName from userProfiles up where up.userid = users.id) where fullname is null;

go


ALTER TABLE [dbo].[UserFiles] DROP CONSTRAINT [FK_UserFiles_Users_UserId]
GO

ALTER TABLE [dbo].[UserFiles] DROP CONSTRAINT [DF__UserFiles__Creat__07C12930]
GO

/****** Object:  Table [dbo].[UserFiles]    Script Date: 12/21/2017 8:45:40 AM ******/
DROP TABLE [dbo].[UserFiles]
GO

/****** Object:  Table [dbo].[UserFiles]    Script Date: 12/21/2017 8:45:40 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserFiles](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FilePath] [nvarchar](1000) NULL,
	[Type] [tinyint] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Extension] [nvarchar](5) NOT NULL,
	[Created] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_UserFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserFiles] ADD  CONSTRAINT [DF__UserFiles__Creat__07C12930]  DEFAULT ((0)) FOR [Created]
GO

ALTER TABLE [dbo].[UserFiles]  WITH CHECK ADD  CONSTRAINT [FK_UserFiles_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[UserFiles] CHECK CONSTRAINT [FK_UserFiles_Users_UserId]
GO

alter table Majors ADD CollegeId int null;
go

alter table Majors ADD CONSTRAINT [FK_Majors_Collges] FOREIGN KEY([Collegeid])
REFERENCES [dbo].[Colleges] ([Id])
GO

alter table Users add UniversityOther nvarchar(255) NULL;
go

alter table Users add CollegeOther nvarchar(255) NULL;
go

alter table Users add MajorOther nvarchar(255) NULL;
go



CREATE TABLE [dbo].[Messages](
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Subject] [nvarchar] (500) NULL,
	[Body] [nvarchar] (1000) NULL,
	[ToUsers] [nvarchar] (2000) NULL,
	[ToRoles] [nvarchar] (2000) NULL,
	[ToColleges] [nvarchar] (2000) NULL,
	[IsAnnouncement] [bit] NOT NULL default ((0)),
	[InReplyTo] [bigint] NULL,
	[Sent] [datetime] NOT NULL default getdate(),
	[SentBy] [nvarchar] (128) NOT NULL,
	[IsDeleted] [bit] NOT NULL default ((0)),
)
go



alter table Messages add constraint FK_Messages_Messages FOREIGN KEY (InReplyTo) REFERENCES Messages (Id);
go


alter table Messages add constraint FK_Messages_Users FOREIGN KEY (SentBy) REFERENCES Users (Id);
go





CREATE TABLE [dbo].[MessageUsers](
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[MessageId] [bigint] NULL,
	[UserId] [nvarchar] (128) NOT NULL,
	[Viewed] [datetime] NOT NULL
)
go



alter table MessageUsers add constraint FK_MessageUsers_Messages FOREIGN KEY (MessageId) REFERENCES Messages (Id);
go




alter table MessageUsers add constraint FK_MessageUsers_Users FOREIGN KEY (UserId) REFERENCES Users (Id);
go


CREATE TABLE [dbo].[MessageFiles](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[MessageId] [bigint] NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FilePath] [nvarchar](1000) NULL,
	[Type] [tinyint] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Extension] [nvarchar](5) NOT NULL,
	[Created] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
)
go






alter table MessageFiles add constraint FK_MessageFiles_Messages FOREIGN KEY (MessageId) REFERENCES Messages (Id);
go




alter table MessageFiles add constraint FK_MessageFiles_Users FOREIGN KEY (CreatedBy) REFERENCES Users (Id);
go


alter table UserSessions add ConnectionId nvarchar(255) NULL;
go

alter table UserSessions drop column ConnectionId;
go

create table UserConnections
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[UserId] [nvarchar] (128) NOT NULL,
	[ConnectionId] [nvarchar](255) NOT NULL,
	[IsConnected] [bit] NOT NULL default((0)),
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL
);

go



alter table UserConnections add constraint FK_UserConnections_Users FOREIGN KEY (UserId) REFERENCES Users (Id);
go

--- Dynamic Forms & Workflows


alter table Roles add NameArabic nvarchar(255) NULL;
go

alter table Roles add NameEnglish nvarchar(255) NULL;
go

alter table Roles add IsActive bit NOT NULL DEFAULT ((1));
go

create table RequestActions
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Key] [nvarchar] (256) NOT NULL UNIQUE,
	[NameArabic] [nvarchar](255) NULL,
	[NameEnglish] [nvarchar](255) NULL,
	IsActive bit NOT NULL DEFAULT ((1))
);
go

create table RequestTypes
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Key] [nvarchar] (256) NOT NULL UNIQUE,
	[NameArabic] [nvarchar](255) NULL,
	[NameEnglish] [nvarchar](255) NULL,
	IsActive bit NOT NULL DEFAULT ((1))
);
go

create table WorkFlows
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Key] [nvarchar] (256) NOT NULL UNIQUE,
	[NameArabic] [nvarchar](255) NULL,
	[NameEnglish] [nvarchar](255) NULL,
	[RequestTypeId] [bigint] NOT NULL, 
	IsActive bit NOT NULL DEFAULT ((1))
);
go


alter table WorkFlows add constraint FK_WorkFlows_RequestTypes FOREIGN KEY (RequestTypeId) REFERENCES RequestTypes (Id);
go


create table WorkflowStates
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[WorkflowId] [bigint] NOT NULL, 
	[Key] [nvarchar] (256) NOT NULL,
	[NameArabic] [nvarchar](255) NULL,
	[NameEnglish] [nvarchar](255) NULL,
	IsActive bit NOT NULL DEFAULT ((1))
);
go


alter table WorkflowStates add constraint FK_WorkflowStates_WorkFlows FOREIGN KEY (WorkflowId) REFERENCES WorkFlows (Id);
go




create table WorkflowStateActions
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[StateId] [bigint] NOT NULL, 
	[RoleId] [nvarchar] (128) NOT NULL, 
	[CollegeId] [int] NULL, 
	[ActionId] [bigint] NOT NULL, 
	[NextStateId] [bigint] NOT NULL, 
	[FormId] [bigint] NULL, 
	[FormScope] [tinyint] NULL,
	[VisibleToRoles] [nvarchar] (MAX) NULL,
	[InvisibleToRoles] [nvarchar] (MAX) NULL,
	IsActive bit NOT NULL DEFAULT ((1))
);
go


alter table WorkflowStateActions add constraint FK_WorkflowStateActions_WorkflowStates FOREIGN KEY (StateId) REFERENCES WorkFlowStates (Id);
go

alter table WorkflowStateActions add constraint FK_WorkflowStateActions_WorkflowStates_Next FOREIGN KEY (NextStateId) REFERENCES WorkFlowStates (Id);
go

alter table WorkflowStateActions add constraint FK_WorkflowStateActions_Roles FOREIGN KEY (RoleId) REFERENCES Roles (Id);
go

alter table WorkflowStateActions add constraint FK_WorkflowStateActions_Colleges FOREIGN KEY (CollegeId) REFERENCES Colleges (Id);
go

alter table WorkflowStateActions add constraint FK_WorkflowStateActions_RequestActions FOREIGN KEY (ActionId) REFERENCES RequestActions (Id);
go


create table Forms
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Key] [nvarchar] (256) NOT NULL,
	[NameArabic] [nvarchar](255) NULL,
	[NameEnglish] [nvarchar](255) NULL,
	IsActive bit NOT NULL DEFAULT ((1)),
	[VisibleToRoles] [nvarchar] (MAX) NULL,
	[InvisibleToRoles] [nvarchar] (MAX) NULL
);
go


alter table WorkflowStateActions add constraint FK_WorkflowStateActions_Forms FOREIGN KEY (FormId) REFERENCES Forms (Id);
go




create table FormSections
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[FormId] [bigint] NULL, 
	[Key] [nvarchar] (256) NOT NULL,
	[NameArabic] [nvarchar](255) NULL,
	[NameEnglish] [nvarchar](255) NULL,
	IsActive bit NOT NULL DEFAULT ((1)),
	[DependsOnFieldId] [bigint] NULL,
	[DependsOnFieldValue] [nvarchar] (MAX) NULL,
	[VisibleToRoles] [nvarchar] (MAX) NULL,
	[InvisibleToRoles] [nvarchar] (MAX) NULL
);
go


alter table FormSections add constraint FK_FormSections_Forms FOREIGN KEY (FormId) REFERENCES Forms (Id);
go


create table FormFields
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[FormId] [bigint] NULL, 
	[LookupId] [bigint] NULL,
	[SectionId] [bigint] NULL,
	[Key] [nvarchar] (256) NOT NULL,
	[NameArabic] [nvarchar](255) NULL,
	[NameEnglish] [nvarchar](255) NULL,
	[FieldType] [tinyint] NOT NULL,
	
	[IsRequired] bit NOT NULL DEFAULT ((0)),
	[IsMultipleSelection] bit NOT NULL DEFAULT ((0)),
	[IsActive] bit NOT NULL DEFAULT ((1)),
	[MinValue] [nvarchar] (MAX) NULL,
	[MaxValue] [nvarchar] (MAX) NULL,
	[DependsOnFieldId] [bigint] NULL,
	[DependsOnFieldValue] [nvarchar] (MAX) NULL,
	[VisibleToRoles] [nvarchar] (MAX) NULL,
	[InvisibleToRoles] [nvarchar] (MAX) NULL
);
go



alter table FormFields add constraint FK_FormFields_Forms FOREIGN KEY (FormId) REFERENCES Forms (Id);
go


alter table FormFields add constraint FK_FormFields_FormSections FOREIGN KEY (SectionId) REFERENCES FormSections (Id);
go

alter table FormFields add constraint FK_FormFields_FormFields FOREIGN KEY (DependsOnFieldId) REFERENCES FormFields (Id);
go

alter table FormSections add constraint FK_FormSections_FormFields FOREIGN KEY (DependsOnFieldId) REFERENCES FormFields (Id);
go


create table Lookups
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Key] [nvarchar] (256) NOT NULL UNIQUE,
	[IsActive] bit NOT NULL DEFAULT ((1)),
);
go


create table LookupValues
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[LookupId] [bigint] NOT NULL,
	[Key] [nvarchar] (256) NOT NULL,
	[NameArabic] [nvarchar](255) NULL,
	[NameEnglish] [nvarchar](255) NULL,
	[ParentId] [bigint] NULL,
	[IsActive] bit NOT NULL DEFAULT ((1)),
);
go

alter table FormFields add constraint FK_FormFields_Lookups FOREIGN KEY (LookupId) REFERENCES Lookups (Id);
go


alter table LookupValues add constraint FK_LookupValues_Lookups FOREIGN KEY (LookupId) REFERENCES Lookups (Id);
go


alter table LookupValues add constraint FK_LookupValues_LookupValues FOREIGN KEY (ParentId) REFERENCES LookupValues (Id);
go

alter table WorkflowStateActions add ActionScope tinyint NULL;
go
alter table WorkflowStateActions add NumberResponded int NULL;
go
alter table WorkflowStateActions add NumberAgreed int NULL;
go
alter table WorkflowStateActions add Negate bit NULL;
go

alter table Roles add IsCommiteeHead [bit] NOT NULL DEFAULT ((0));
go
alter table Roles add IsCommiteeMember [bit] NOT NULL DEFAULT ((0));
go
alter table FormFields add [Order] int NOT NULL;
go
alter table FormFields add [IsReadOnly] bit NOT NULL default ((0));
go


alter table FormFields alter column [key] [nvarchar](max) NULL;
go
alter table FormFields alter column [NameArabic] [nvarchar](max) NULL;
go

alter table FormFields alter column [NameEnglish] [nvarchar](max) NULL;
go

alter table WorkflowStateActions add [FormVisibleToRoles] [nvarchar] (MAX) NULL;
go
alter table WorkflowStateActions add [InvisibleToRoles] [nvarchar] (MAX) NULL;
go