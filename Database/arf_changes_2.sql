IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'LastUpdatedStep'
)
BEGIN
	ALTER TABLE Requests add LastUpdatedStep tinyint NULL;
END
	
	
	
IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'TitleEnglish'
)
BEGIN
	ALTER TABLE Requests add TitleEnglish nvarchar(255) NULL;
END
	



IF NOT EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'Categories')
BEGIN

	
	CREATE TABLE [dbo].[Categories](
		[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[NameArabic] [nvarchar](500) NULL,
		[NameEnglish] [nvarchar](500) NULL,
		[ParentId] [bigint] NULL,
		[Level] [bigint] NULL,
		[Created] [datetime] NOT NULL,
		[Updated] [datetime] NOT NULL,
		[CreatedBy] [nvarchar](128) NOT NULL,
		[UpdatedBy] [nvarchar](128) NOT NULL)
	

	ALTER TABLE [dbo].[Categories] ADD  DEFAULT (getdate()) FOR [Created]

	ALTER TABLE [dbo].[Categories] ADD  DEFAULT (getdate()) FOR [Updated]

	ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Categories_ParentId] FOREIGN KEY([ParentId])
	REFERENCES [dbo].[Categories] ([Id])

	ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Users_CreatedBy] FOREIGN KEY(CreatedBy)
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Users_UpdatedBy] FOREIGN KEY([UpdatedBy])
	REFERENCES [dbo].[Users] ([Id])

END


-- relationship to requests

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'PrimaryCategoryId'
)
BEGIN
	ALTER TABLE Requests add PrimaryCategoryId bigint NULL;


		
	ALTER TABLE [dbo].[Requests] ADD  CONSTRAINT [FK_Requests_Categories_Primary] FOREIGN KEY([PrimaryCategoryId])
	REFERENCES [dbo].[Categories] ([Id])
END


IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'SecondaryCategoryId'
)
BEGIN
	ALTER TABLE Requests add SecondaryCategoryId bigint NULL;


		
	ALTER TABLE [dbo].[Requests] ADD  CONSTRAINT [FK_Requests_Categories_Primary] FOREIGN KEY([SecondaryCategoryId])
	REFERENCES [dbo].[Categories] ([Id])
END



IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'PrimaryCategoryOther'
)
BEGIN
	ALTER TABLE Requests add PrimaryCategoryOther nvarchar(255) NULL;

END




IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'SecondaryCategoryOther'
)
BEGIN
	ALTER TABLE Requests add SecondaryCategoryOther nvarchar(255) NULL;

END





IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'ApplicationNumber'
)
BEGIN
	ALTER TABLE Requests add ApplicationNumber nvarchar(255) NULL;

END




IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'IsPreviouslyApplied'
)
BEGIN
	ALTER TABLE Requests add IsPreviouslyApplied bit NULL;

END



IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'NumberOfFundedProjects'
)
BEGIN
	ALTER TABLE Requests add NumberOfFundedProjects int NULL;

END







IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'NumberOfExpectedPublications'
)
BEGIN
	ALTER TABLE Requests add NumberOfExpectedPublications int NULL;

END




IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'RequestCoresearchers'
    AND column_name = 'Order'
)
BEGIN
	ALTER TABLE RequestCoresearchers add [Order] int NULL;

END





IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Researchers'
    AND column_name = 'Qualification'
)
BEGIN
	ALTER TABLE Researchers add [Qualification] tinyint NULL;

END





IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Researchers'
    AND column_name = 'UniversityOther'
)
BEGIN
	ALTER TABLE Researchers add [UniversityOther]  nvarchar(255) NULL;

END


IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Researchers'
    AND column_name = 'CollegeOther'
)
BEGIN
	ALTER TABLE Researchers add [CollegeOther]  nvarchar(255) NULL;

END

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Researchers'
    AND column_name = 'DepartmentOther'
)
BEGIN
	ALTER TABLE Researchers add [DepartmentOther] nvarchar(255) NULL;

END




IF NOT EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'FileTexts')
BEGIN

	
		
	CREATE TABLE [dbo].[FileTexts](
		[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[Text] [ntext] NULL,
		[Created] [datetime] NOT NULL,
		[Updated] [datetime] NOT NULL,
		[CreatedBy] [nvarchar](128) NOT NULL,
		[UpdatedBy] [nvarchar](128) NOT NULL
	);
		
		
	ALTER TABLE [dbo].[RequestFiles] add FileTextId bigint NULL;
	ALTER TABLE [dbo].[UserFiles] add FileTextId bigint NULL;
	
	ALTER TABLE [dbo].[FileTexts] ADD  CONSTRAINT [FK_FileTexts_Users_CreatedBy] FOREIGN KEY([CreatedBy])
	REFERENCES [dbo].[Users] ([Id])
	
	ALTER TABLE [dbo].[FileTexts] ADD  CONSTRAINT [FK_FileTexts_Users_UpdatedBy] FOREIGN KEY([UpdatedBy])
	REFERENCES [dbo].[Users] ([Id])
	
	
	ALTER TABLE [dbo].[RequestFiles] ADD  CONSTRAINT [FK_RequestFiles_FileTexts] FOREIGN KEY([FileTextId])
	REFERENCES [dbo].[FileTexts] ([Id])
	
	ALTER TABLE [dbo].[UserFiles] ADD  CONSTRAINT [FK_UserFiles_FileTexts] FOREIGN KEY([FileTextId])
	REFERENCES [dbo].[FileTexts] ([Id])
	
	
END



IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'RequestFiles'
    AND column_name = 'Updated'
)
BEGIN
	ALTER TABLE [dbo].[RequestFiles] add [Updated] [datetime] NULL;
	ALTER TABLE [dbo].[RequestFiles] add [UpdatedBy] [nvarchar](128) NULL;
	
	
	ALTER TABLE [dbo].[UserFiles] add [Updated] [datetime] NULL;
	ALTER TABLE [dbo].[UserFiles] add [UpdatedBy] [nvarchar](128) NULL;
	
	
	ALTER TABLE [dbo].[RequestFiles] ADD  CONSTRAINT [FK_RequestFiles_Users_CreatedBy] FOREIGN KEY([CreatedBy])
	REFERENCES [dbo].[Users] ([Id])
	
	ALTER TABLE [dbo].[RequestFiles] ADD  CONSTRAINT [FK_RequestFiles_Users_UpdatedBy] FOREIGN KEY([UpdatedBy])
	REFERENCES [dbo].[Users] ([Id])
	
	ALTER TABLE [dbo].[UserFiles] ADD  CONSTRAINT [FK_UserFiles_Users_CreatedBy] FOREIGN KEY([CreatedBy])
	REFERENCES [dbo].[Users] ([Id])
	
	ALTER TABLE [dbo].[UserFiles] ADD  CONSTRAINT [FK_UserFiles_Users_UpdatedBy] FOREIGN KEY([UpdatedBy])
	REFERENCES [dbo].[Users] ([Id])
	
	
	
	
		
	ALTER TABLE [dbo].[RequestFiles] alter column [Updated] [datetime] NOT NULL;
	ALTER TABLE [dbo].[RequestFiles] alter column [UpdatedBy] [nvarchar](128) NOT NULL;
	
	
	ALTER TABLE [dbo].[UserFiles] alter column [Updated] [datetime] NOT NULL;
	ALTER TABLE [dbo].[UserFiles] alter column [UpdatedBy] [nvarchar](128) NOT NULL;
	
END

Update RequestFiles set updated = created, updatedby = createdby where updated is null or updatedby is null
	
Update UserFiles set updated = created, updatedby = createdby where updated is null or updatedby is null



IF EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'DepartmentPriorities')
BEGIN
	
	DROP TABLE [dbo].[DepartmentPriorities];

END

go

IF (OBJECT_ID('FK_Requests_Priorities', 'F') IS NOT NULL) BEGIN ALTER TABLE [dbo].[Requests]  DROP  CONSTRAINT [FK_Requests_Priorities] END



IF EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'Priorities')
BEGIN
	
	DROP TABLE [dbo].[Priorities];

END

go

CREATE TABLE [dbo].[Priorities](
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PriorityNo] [int] NULL,
	[NameArabic] [nvarchar](500) NULL UNIQUE,
	[NameEnglish] [nvarchar](500) NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[UpdatedBy] [nvarchar](128) NOT NULL
)


ALTER TABLE [dbo].[Priorities] ADD  DEFAULT (getdate()) FOR [Created]

ALTER TABLE [dbo].[Priorities] ADD  DEFAULT (getdate()) FOR [Updated]



ALTER TABLE [dbo].[Priorities]  WITH CHECK ADD  CONSTRAINT [FK_Priorities_Users_CreatedBy] FOREIGN KEY(CreatedBy)
REFERENCES [dbo].[Users] ([Id])

ALTER TABLE [dbo].[Priorities]  WITH CHECK ADD  CONSTRAINT [FK_Priorities_Users_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[Users] ([Id])

CREATE TABLE [dbo].[DepartmentPriorities](
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PriorityId] [bigint] NOT NULL,
	[DepartmentId] [int] NOT NULL
)

ALTER TABLE [dbo].[DepartmentPriorities]  WITH CHECK ADD  CONSTRAINT [FK_DepartmentPriorities_Priorities] FOREIGN KEY([PriorityId])
REFERENCES [dbo].[Priorities] ([Id])

ALTER TABLE [dbo].[DepartmentPriorities]  WITH CHECK ADD  CONSTRAINT [FK_DepartmentPriorities_Departments] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([Id])

go
IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'PriorityId'
)
BEGIN
	ALTER TABLE Requests add [PriorityId] bigint NULL;

END



IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'DepartmentId'
)
BEGIN
	ALTER TABLE Requests add [DepartmentId] int NULL;

	
	ALTER TABLE [dbo].[Requests]  WITH CHECK ADD  CONSTRAINT [FK_Requests_Departments] FOREIGN KEY([DepartmentId])
	REFERENCES [dbo].[Departments] ([Id])
END


IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Colleges'
    AND column_name = 'Field'
)
BEGIN
	ALTER TABLE Colleges add [Field] tinyint NULL;
END


IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Colleges'
    AND column_name = 'ResearchUnitCode'
)
BEGIN
	ALTER TABLE Colleges add [ResearchUnitCode] tinyint NULL;
END


update researchers set CollegeId=null, DepartmentId=null, universityid = null
update requests set CollegeId=null, DepartmentId=null
update users set CollegeId = null, departmentid = null
update researchers set collegeid =null, departmentid = null

delete from  DepartmentPriorities
delete from  Priorities
delete from  Departments
delete from  colleges
delete from  COLLEGES
delete from  universities
go

SET IDENTITY_INSERT universities ON
insert into universities (id, NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (1, N'جامعة الإمام محمد بن سعود الإسلامية', 'Al-Imam Mohammad Ibn Saud Islamic University', getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
SET IDENTITY_INSERT universities OFF
go


INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'المعهد العالي للقضاء', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 1, 1);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'المعهد العالي للاحتساب', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 1, 15);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'الشريعة', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 1, 2);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'أصول الدين', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 1, 3);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'الشريعة بالأحساء', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, NULL, 8);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'علوم الحاسب', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 4, 9);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'الطب', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 4, 13);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'العلوم', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 4, 12);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'الهندسة', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 4, 14);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'اللغة العربية', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 2, 4);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'معهد تعليم اللغة العربية', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 2, 10);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'الاعلام والاتصال', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 3, 5);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'العلوم الاجتماعية', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 3, 6);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'اللغات والترجمة', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 3, 7);
INSERT INTO Colleges (NameArabic, NameEnglish, UniversityId, Created, Updated, CreatedBy, UpdatedBy, IsDeleted, Field, ResearchUnitCode) VALUES	(N'الاقتصاد والعلوم الإدارية', NULL, 1, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0, 3, 11);
go


insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم القران الكريم وعلومه', NULL, (select id from colleges where ResearchUnitCode = 3), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم السنة النبوية', NULL, (select id from colleges where ResearchUnitCode = 3), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم العقيدة والمذاهب المعاصرة', NULL, (select id from colleges where ResearchUnitCode = 3), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم القضاء العمالي', NULL, (select id from colleges where ResearchUnitCode = 1), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الفقه المقارن', NULL, (select id from colleges where ResearchUnitCode = 1), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم السياسة الشرعية', NULL, (select id from colleges where ResearchUnitCode = 1), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم القضاء التجاري', NULL, (select id from colleges where ResearchUnitCode = 1), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم القضاء الجزائي', NULL, (select id from colleges where ResearchUnitCode = 1), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم قضاء الأحوال الشخصية', NULL, (select id from colleges where ResearchUnitCode = 1), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم القضاء الإداري', NULL, (select id from colleges where ResearchUnitCode = 1), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'الدراسات الإسلامية المعاصرة', NULL, (select id from colleges where ResearchUnitCode = 15), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الدعوة', NULL, (select id from colleges where ResearchUnitCode = 15), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الحسبة', NULL, (select id from colleges where ResearchUnitCode = 15), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الفقه', NULL, (select id from colleges where ResearchUnitCode = 2), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم أصول الفقه', NULL, (select id from colleges where ResearchUnitCode = 2), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الثقافة الإسلامية', NULL, (select id from colleges where ResearchUnitCode = 2), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الأنظمة', NULL, (select id from colleges where ResearchUnitCode = 2), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم نظم المعلومات', NULL, (select id from colleges where ResearchUnitCode = 9), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم علوم الحاسب', NULL, (select id from colleges where ResearchUnitCode = 9), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم تقنية المعلومات', NULL, (select id from colleges where ResearchUnitCode = 9), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم إدارة المعلومات', NULL, (select id from colleges where ResearchUnitCode = 9), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم طب الأسرة', NULL, (select id from colleges where ResearchUnitCode = 13), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الصحة العامة', NULL, (select id from colleges where ResearchUnitCode = 13), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم العيون', NULL, (select id from colleges where ResearchUnitCode = 13), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم التشريح', NULL, (select id from colleges where ResearchUnitCode = 13), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم علوم الامراض', NULL, (select id from colleges where ResearchUnitCode = 13), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الكيمياء الحيوية', NULL, (select id from colleges where ResearchUnitCode = 13), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الأدوية', NULL, (select id from colleges where ResearchUnitCode = 13), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم وظائف الأعضاء', NULL, (select id from colleges where ResearchUnitCode = 13), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم التعليم الطبي', NULL, (select id from colleges where ResearchUnitCode = 13), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الجلدية', NULL, (select id from colleges where ResearchUnitCode = 13), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الكيمياء', NULL, (select id from colleges where ResearchUnitCode = 12), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الفيزياء', NULL, (select id from colleges where ResearchUnitCode = 12), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الرياضيات والاحصاء', NULL, (select id from colleges where ResearchUnitCode = 12), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الأحياء', NULL, (select id from colleges where ResearchUnitCode = 12), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الهندسة الميكانيكية', NULL, (select id from colleges where ResearchUnitCode = 14), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الهندسة المدنية', NULL, (select id from colleges where ResearchUnitCode = 14), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الهندسة الكهربائية', NULL, (select id from colleges where ResearchUnitCode = 14), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الهندسة الكيميائية', NULL, (select id from colleges where ResearchUnitCode = 14), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الهندسة المعمارية', NULL, (select id from colleges where ResearchUnitCode = 14), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الادب', NULL, (select id from colleges where ResearchUnitCode = 4), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم البلاغة والنقد ومنهج الادب الإسلامي', NULL, (select id from colleges where ResearchUnitCode = 4), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم النحو والصرف وفقه اللغة', NULL, (select id from colleges where ResearchUnitCode = 4), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم اللغة العربية والعلوم الإسلامية', NULL, (select id from colleges where ResearchUnitCode = 10), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم علم اللغة التطبيقي', NULL, (select id from colleges where ResearchUnitCode = 10), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الإعداد اللغوي', NULL, (select id from colleges where ResearchUnitCode = 10), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الصحافة والنشر  الإكتروني', NULL, (select id from colleges where ResearchUnitCode = 5), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الإذاعة والتليفزيون والفيلم', NULL, (select id from colleges where ResearchUnitCode = 5), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم العلاقات العامة', NULL, (select id from colleges where ResearchUnitCode = 5), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الإعلان والإتصال التسويقي', NULL, (select id from colleges where ResearchUnitCode = 5), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الجرفيكس والوسائط المتعددة', NULL, (select id from colleges where ResearchUnitCode = 5), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الإعلام المتخصص', NULL, (select id from colleges where ResearchUnitCode = 5), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الجغرافيا', NULL, (select id from colleges where ResearchUnitCode = 6), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الاجتماع والخدمة الاجتماعية', NULL, (select id from colleges where ResearchUnitCode = 6), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم علم النفس', NULL, (select id from colleges where ResearchUnitCode = 6), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الإدارة والتخطيط التربوي', NULL, (select id from colleges where ResearchUnitCode = 6), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم أصول التربية', NULL, (select id from colleges where ResearchUnitCode = 6), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم المناهج وطرق التدريس', NULL, (select id from colleges where ResearchUnitCode = 6), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم التربية الخاصة', NULL, (select id from colleges where ResearchUnitCode = 6), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم التاريخ والحضارة', NULL, (select id from colleges where ResearchUnitCode = 6), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم اللغات والترجمة', NULL, (select id from colleges where ResearchUnitCode = 7), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الاقتصاد', NULL, (select id from colleges where ResearchUnitCode = 11), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم المحاسبة', NULL, (select id from colleges where ResearchUnitCode = 11), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم إدارة الاعمال', NULL, (select id from colleges where ResearchUnitCode = 11), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم التمويل والاستثمار', NULL, (select id from colleges where ResearchUnitCode = 11), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الإعمال المصرفية', NULL, (select id from colleges where ResearchUnitCode = 11), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم التأمين وإدارة المخاطر', NULL, (select id from colleges where ResearchUnitCode = 11), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الشريعة', NULL, (select id from colleges where ResearchUnitCode = 8), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم أصول الدين', NULL, (select id from colleges where ResearchUnitCode = 8), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم اللغة العربية', NULL, (select id from colleges where ResearchUnitCode = 8), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الإدارة', NULL, (select id from colleges where ResearchUnitCode = 8), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الأنظمة', NULL, (select id from colleges where ResearchUnitCode = 8), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الحاسب الآلي', NULL, (select id from colleges where ResearchUnitCode = 8), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);
insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم اللغة الإنجليزية', NULL, (select id from colleges where ResearchUnitCode = 8), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);


go


insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة أم القرى', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'الجامعة الإسلامية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الملك سعود', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الملك فهد للبترول والمعادن', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الملك فيصل', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الملك خالد', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة القصيم', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة طيبة', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الطائف', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة حائل', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة جازان', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الجوف', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الباحة', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة تبوك', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة نجران', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الحدود الشمالية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الأميرة نورة بنت عبد الرحمن', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الملك سعود بن عبد العزيز للعلوم الصحية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الامام عبد الرحمن الفيصل', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الأمير سطام بن عبد العزيز', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة شقراء', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة المجمعة', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'الجامعة السعودية الإلكترونية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة جدة', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة بيشة', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الأمير سلطان الأهلية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة عفت الأهلية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'الجامعة العربية المفتوحة', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة اليمامة الأهلية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الاعمال والتكنولوجيا الأهلية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الأمير محمد بن فهد الأهلية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الفيصل الأهلية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة دار العلوم الأهلية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة دار الحكمة الأهلية', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
insert into universities (NameArabic, NameEnglish, Created, Updated, CreatedBy, UpdatedBy, IsDeleted) values (N'جامعة الأمير مقرن بن عبد العزيز', NULL, getdate(), getdate(), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);


go


IF NOT EXISTS (select * from countries where namearabic = N'الإمارات العربية المتحدة') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الإمارات العربية المتحدة', N'United Arab Emirates', 1) end
IF NOT EXISTS (select * from countries where namearabic = N'الأردن') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الأردن', N'Jordan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'البحرين') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'البحرين', N'Bahrain', 1) end
IF NOT EXISTS (select * from countries where namearabic = N'الجزائر') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الجزائر', N'Algeria', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'السودان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'السودان', N'Sudan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الصومال') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الصومال', N'Somalia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'العراق') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'العراق', N'Iraq', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الكويت') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الكويت', N'Kuwait', 1) end
IF NOT EXISTS (select * from countries where namearabic = N'المغرب') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'المغرب', N'Morocco', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'المملكة العربية السعودية') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'المملكة العربية السعودية', N'Saudi Arabia', 1) end
IF NOT EXISTS (select * from countries where namearabic = N'اليمن') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'اليمن', N'Yeman', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'تونس') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'تونس', N'Tunis', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جيبوتي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جيبوتي', N'Djibouti', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سوريا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سوريا', N'Syria', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'عمان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'عمان', N'Oman', 1) end
IF NOT EXISTS (select * from countries where namearabic = N'فلسطين') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'فلسطين', N'Palestine', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'قطر') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'قطر', N'Qatar', 1) end
IF NOT EXISTS (select * from countries where namearabic = N'لبنان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'لبنان', N'Lebanon', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ليبيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ليبيا', N'Libya', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'مصر') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'مصر', N'Egypt', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'موريتانيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'موريتانيا', N'Mauritania', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أفغانستان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أفغانستان', N'Afghanistan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ألبانيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ألبانيا', N'Albania', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ساموا-الأمريكي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ساموا-الأمريكي', N'American Samoa', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أندورا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أندورا', N'Andorra', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أنغولا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أنغولا', N'Angola', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أنغويلا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أنغويلا', N'Anguilla', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أنتاركتيكا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أنتاركتيكا', N'Antarctica', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أنتيغوا وبربودا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أنتيغوا وبربودا', N'Antigua and Barbuda', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الأرجنتين') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الأرجنتين', N'Argentina', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أرمينيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أرمينيا', N'Armenia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أروبا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أروبا', N'Aruba', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أستراليا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أستراليا', N'Australia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'النمسا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'النمسا', N'Austria', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أذربيجان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أذربيجان', N'Azerbaijan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الباهاماس') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الباهاماس', N'Bahamas', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بنغلاديش') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بنغلاديش', N'Bangladesh', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بربادوس') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بربادوس', N'Barbados', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'روسيا البيضاء') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'روسيا البيضاء', N'Belarus', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بلجيكا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بلجيكا', N'Belgium', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بيليز') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بيليز', N'Belize', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بنين') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بنين', N'Benin', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جزر برمود') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جزر برمود', N'Bermuda', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بوتان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بوتان', N'Bhutan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بوليفيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بوليفيا', N'Bolivia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'البوسنة و الهرسك') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'البوسنة و الهرسك', N'Bosnia and Herzegovina', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بوتسوانا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بوتسوانا', N'Botswana', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'البرازيل') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'البرازيل', N'Brazil', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بروناي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بروناي', N'Brunei', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بلغاريا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بلغاريا', N'Bulgaria', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بوركينا فاسو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بوركينا فاسو', N'Burkina Faso', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بوروندي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بوروندي', N'Burundi', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كمبوديا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كمبوديا', N'Cambodia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كاميرون') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كاميرون', N'Cameroon', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كندا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كندا', N'Canada', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الرأس الأخضر') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الرأس الأخضر', N'Cape Verde', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جمهورية أفريقيا الوسطى') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جمهورية أفريقيا الوسطى', N'Central African Republic', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'تشاد') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'تشاد', N'Chad', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'تشيلي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'تشيلي', N'Chile', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جمهورية الصين الشعبية') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جمهورية الصين الشعبية', N'China', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كولومبيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كولومبيا', N'Colombia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جزر القمر') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جزر القمر', N'Comoros', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جمهورية الكونغو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جمهورية الكونغو', N'Congo', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جزر كوك') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جزر كوك', N'Cook Islands', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كوستاريكا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كوستاريكا', N'Costa Rica', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ساحل العاج') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ساحل العاج', N'Cote d''Ivoire (Ivory Coast)', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كرواتيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كرواتيا', N'Croatia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كوبا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كوبا', N'Cuba', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'قبرص') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'قبرص', N'Cyprus', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الجمهورية التشيكية') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الجمهورية التشيكية', N'Czech Republic', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الدانمارك') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الدانمارك', N'Denmark', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'دومينيكا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'دومينيكا', N'Dominica', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الجمهورية الدومينيكية') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الجمهورية الدومينيكية', N'Dominican Republic', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'تيمور الشرقية') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'تيمور الشرقية', N'East Timor Timor-Leste', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'إكوادور') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'إكوادور', N'Ecuador', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'إلسلفادور') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'إلسلفادور', N'El Salvador', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'غينيا الاستوائي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'غينيا الاستوائي', N'Equatorial Guinea', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'إريتريا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'إريتريا', N'Eritrea', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'استونيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'استونيا', N'Estonia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أثيوبيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أثيوبيا', N'Ethiopia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جزر فوكلاند') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جزر فوكلاند', N'Falkland Islands', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جزر فارو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جزر فارو', N'Faroe Islands', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'فيجي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'فيجي', N'Fiji', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'فنلندا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'فنلندا', N'Finland', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'فرنسا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'فرنسا', N'France', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'غويانا الفرنسية') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'غويانا الفرنسية', N'French Guiana', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بولينيزيا الفرنسية') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بولينيزيا الفرنسية', N'French Polynesia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الغابون') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الغابون', N'Gabon', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'غامبيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'غامبيا', N'Gambia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جيورجيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جيورجيا', N'Georgia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ألمانيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ألمانيا', N'Germany', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'غانا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'غانا', N'Ghana', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'اليونان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'اليونان', N'Greece', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جرينلاند') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جرينلاند', N'Greenland', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'غرينادا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'غرينادا', N'Grenada', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جزر جوادلوب') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جزر جوادلوب', N'Guadeloupe', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جوام') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جوام', N'Guam', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'غواتيمال') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'غواتيمال', N'Guatemala', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'غينيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'غينيا', N'Guinea', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'غينيا-بيساو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'غينيا-بيساو', N'Guinea-Bissau', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'غيانا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'غيانا', N'Guyana', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'هايتي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'هايتي', N'Haiti', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'هندوراس') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'هندوراس', N'Honduras', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'هونغ كونغ') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'هونغ كونغ', N'Hong Kong', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'المجر') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'المجر', N'Hungary', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'آيسلندا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'آيسلندا', N'Iceland', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الهند') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الهند', N'India', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أندونيسيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أندونيسيا', N'Indonesia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'إيران') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'إيران', N'Iran', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جمهورية أيرلندا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جمهورية أيرلندا', N'Ireland', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'إيطاليا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'إيطاليا', N'Italy', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جمايكا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جمايكا', N'Jamaica', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'اليابان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'اليابان', N'Japan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كازاخستان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كازاخستان', N'Kazakhstan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كينيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كينيا', N'Kenya', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كيريباتي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كيريباتي', N'Kiribati', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كوريا الشمالية') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كوريا الشمالية', N'North Korea', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كوريا الجنوبية') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كوريا الجنوبية', N'South Korea', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'قيرغيزستان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'قيرغيزستان', N'Kyrgyzstan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'لاوس') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'لاوس', N'Lao, PDR', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'لاتفيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'لاتفيا', N'Latvia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ليسوتو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ليسوتو', N'Lesotho', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ليبيريا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ليبيريا', N'Liberia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ليختنشتين') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ليختنشتين', N'Liechtenstein', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'لتوانيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'لتوانيا', N'Lithuania', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'لوكسمبورغ') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'لوكسمبورغ', N'Luxembourg', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ماكاو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ماكاو', N'Macao', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جمهورية مقدونيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جمهورية مقدونيا', N'Macedonia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'مدغشقر') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'مدغشقر', N'Madagascar', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'مالاوي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'مالاوي', N'Malawi', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ماليزيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ماليزيا', N'Malaysia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'المالديف') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'المالديف', N'Maldives', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'مالي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'مالي', N'Mali', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'مالطا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'مالطا', N'Malta', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جزر مارشال') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جزر مارشال', N'Marshall Islands', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'مارتينيك') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'مارتينيك', N'Martinique', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'موريشيوس') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'موريشيوس', N'Mauritius', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'المكسيك') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'المكسيك', N'Mexico', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ولايات ميكرونيسيا المتحدة') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ولايات ميكرونيسيا المتحدة', N'Micronesia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'مولدافيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'مولدافيا', N'Moldova', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'موناكو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'موناكو', N'Monaco', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'منغوليا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'منغوليا', N'Mongolia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الجبل الأسود') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الجبل الأسود', N'Montenegro', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'مونتسيرات') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'مونتسيرات', N'Montserrat', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'موزمبيق') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'موزمبيق', N'Mozambique', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ميانمار') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ميانمار', N'Myanmar', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ناميبيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ناميبيا', N'Namibia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ناورو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ناورو', N'Nauru', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'نيبال') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'نيبال', N'Nepal', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'هولندا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'هولندا', N'Netherlands', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جزر الأنتيل الهولندي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جزر الأنتيل الهولندي', N'Netherlands Antilles', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'كاليدونيا الجديدة') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'كاليدونيا الجديدة', N'New Caledonia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'نيوزيلندا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'نيوزيلندا', N'New Zealand', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'نيكاراجوا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'نيكاراجوا', N'Nicaragua', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'النيجر') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'النيجر', N'Niger', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'نيجيريا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'نيجيريا', N'Nigeria', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'نييوي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'نييوي', N'Niue', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جزر ماريانا الشمالية') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جزر ماريانا الشمالية', N'Northern Mariana Islands', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'النرويج') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'النرويج', N'Norway', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'باكستان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'باكستان', N'Pakistan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بالاو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بالاو', N'Palau', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بنما') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بنما', N'Panama', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بابوا غينيا الجديدة') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بابوا غينيا الجديدة', N'Papua New Guinea', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'باراغواي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'باراغواي', N'Paraguay', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بيرو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بيرو', N'Peru', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الفليبين') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الفليبين', N'Philippines', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بولندا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بولندا', N'Poland', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'البرتغال') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'البرتغال', N'Portugal', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'بورتوريكو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'بورتوريكو', N'Puerto Rico', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ريونيون') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ريونيون', N'Reunion Island', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'رومانيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'رومانيا', N'Romania', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'روسيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'روسيا', N'Russian Federation', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'رواندا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'رواندا', N'Rwanda', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سانت كيتس ونيفس') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سانت كيتس ونيفس', N'Saint Kitts and Nevis', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سانت لوسيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سانت لوسيا', N'Saint Lucia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سانت فنسنت وجزر غرينادين') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سانت فنسنت وجزر غرينادين', N'Saint Vincent and the Grenadines', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ساموا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ساموا', N'Samoa', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سان مارينو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سان مارينو', N'San Marino', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ساو تومي وبرينسيبي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ساو تومي وبرينسيبي', N'Sao Tome and Príncipe', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'السنغال') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'السنغال', N'Senegal', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جمهورية صربيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جمهورية صربيا', N'Serbia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سيشيل') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سيشيل', N'Seychelles', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سيراليون') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سيراليون', N'Sierra Leone', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سنغافورة') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سنغافورة', N'Singapore', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سلوفاكيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سلوفاكيا', N'Slovakia (Slovak Republic)', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سلوفينيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سلوفينيا', N'Slovenia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جزر سليمان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جزر سليمان', N'Solomon Islands', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جنوب أفريقيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جنوب أفريقيا', N'South Africa', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'جنوب السودان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'جنوب السودان', N'South Sudan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'إسبانيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'إسبانيا', N'Spain', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سريلانكا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سريلانكا', N'Sri Lanka', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سورينام') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سورينام', N'Suriname', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سوازيلند') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سوازيلند', N'Swaziland', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'السويد') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'السويد', N'Sweden', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'سويسرا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'سويسرا', N'Switzerland', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'تايوان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'تايوان', N'Taiwan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'طاجيكستان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'طاجيكستان', N'Tajikistan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'تنزانيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'تنزانيا', N'Tanzania', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'تايلندا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'تايلندا', N'Thailand', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'تبت') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'تبت', N'Tibet', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'توغو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'توغو', N'Togo', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'تونغا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'تونغا', N'Tonga', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'ترينيداد وتوباغو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'ترينيداد وتوباغو', N'Trinidad and Tobago', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'تركيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'تركيا', N'Turkey', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'تركمانستان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'تركمانستان', N'Turkmenistan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'توفالو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'توفالو', N'Tuvalu', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أوغندا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أوغندا', N'Uganda', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أوكرانيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أوكرانيا', N'Ukraine', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'المملكة المتحدة') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'المملكة المتحدة', N'United Kingdom', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الولايات المتحدة') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الولايات المتحدة', N'United States', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أورغواي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أورغواي', N'Uruguay', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'أوزباكستان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'أوزباكستان', N'Uzbekistan', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'فانواتو') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'فانواتو', N'Vanuatu', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'دولة مدينة الفاتيكان') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'دولة مدينة الفاتيكان', N'Vatican City State', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'فنزويلا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'فنزويلا', N'Venezuela', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'فيتنام') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'فيتنام', N'Vietnam', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'الجزر العذراء البريطانية') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'الجزر العذراء البريطانية', N'Virgin Islands', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'والس وفوتونا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'والس وفوتونا', N'Wallis and', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'زامبيا') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'زامبيا', N'Zambia', 0) end
IF NOT EXISTS (select * from countries where namearabic = N'زمبابوي') begin insert into countries (NameArabic, NameEnglish, IsGCC) VALUES (N'زمبابوي', N'Zimbabwe', 0) end


go

update UserProfiles set BankId=null
delete from Banks
go

INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'مجموعة سامبا المالية', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'بنك ساب', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'مصرف الراجحي', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'البنك الأول', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'البنك العربي الوطني', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'بنك الجزيرة', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'البنك السعودي الفرنسي', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'البنك الأهلي التجاري', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'بنك الرياض', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'البنك السعودي للاستثمار', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'شركة البلاد المالية', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'بنك مسقط', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'مصرف الانماء', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'بنك الكويت', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'بنك الامارات دبي الوطني    ', 1);
INSERT INTO BANKS (NameArabic, CountryId) VALUES (N'بنك الخليج الدولي', 1);

go


IF NOT EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'Translations')
BEGIN

	
	CREATE TABLE [dbo].[Translations](
		[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[Key] [nvarchar] (2000) NOT NULL,
		[Value] [ntext] NOT NULL,
		[Lang] [char](2) NOT NULL,
		[Created] [datetime] NOT NULL default getdate(),
		[Updated] [datetime] NOT NULL default getdate(),
		[CreatedBy] [nvarchar](128) NOT NULL,
		[UpdatedBy] [nvarchar](128) NOT NULL
	)
	
	
	ALTER TABLE [dbo].[Translations]  WITH CHECK ADD  CONSTRAINT [FK_Translations_Users_CreatedBy] FOREIGN KEY(CreatedBy)
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[Translations]  WITH CHECK ADD  CONSTRAINT [FK_Translations_Users_UpdatedBy] FOREIGN KEY([UpdatedBy])
	REFERENCES [dbo].[Users] ([Id])	
END




IF EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'RequestSuggestedReferees')
BEGIN
	DROP TABLE [dbo].[RequestSuggestedReferees]
END

go

CREATE TABLE [dbo].[RequestSuggestedReferees](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RefereeId] [int] NULL,
	[RequestId] [bigint] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[NationalityId] [int] NULL,
	[CountryId] [int] NULL,
	[RegionId] [int] NULL,
	[CityId] [int] NULL,
	[UniversityId] [int] NULL,
	[CollegeId] [int] NULL,
	[DepartmentId] [int] NULL,
	[Address] [nvarchar](350) NULL,
	[Email] [nvarchar](50) NOT NULL,
	[MobileNo] [nvarchar](50) NULL,
	[Comments] [nvarchar](250) NULL,
	[DateOfBirth] [datetime] NULL,
	[DidResearcherAgree] [bit] NULL,
	[AgreementDate] [datetime] NULL,
	[DegreeId] [int] NULL,
	[Qualification] [tinyint] NULL,
	[UniversityOther] [nvarchar](255) NULL,
	[CollegeOther] [nvarchar](255) NULL,
	[DepartmentOther] [nvarchar](255) NULL,
	[Created] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[Updated] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Order] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[RequestSuggestedReferees] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[RequestSuggestedReferees]  WITH CHECK ADD  CONSTRAINT [FK_RequestSuggestedReferees_Requests] FOREIGN KEY([RequestId])
REFERENCES [dbo].[Requests] ([Id])
GO

ALTER TABLE [dbo].[RequestSuggestedReferees] CHECK CONSTRAINT [FK_RequestSuggestedReferees_Requests]
GO

ALTER TABLE [dbo].[RequestSuggestedReferees]  WITH CHECK ADD  CONSTRAINT [FK_RequestSuggestedReferees_Referees] FOREIGN KEY([RefereeId])
REFERENCES [dbo].[Referees] ([Id])
GO

ALTER TABLE [dbo].[RequestSuggestedReferees] CHECK CONSTRAINT [FK_RequestSuggestedReferees_Referees]
GO



IF EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'RequestActivities')
BEGIN
	DROP TABLE [dbo].[RequestActivities]
	
END


CREATE TABLE [dbo].[RequestActivities](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestId] [bigint] NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[ActivityType] [tinyint] NULL,
	[ActivityTypeOther] [nvarchar] (500) NULL,
	[Address] [nvarchar](350) NULL,
	[Description] [nvarchar](500) NULL,
	[Created] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[Updated] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Order] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[RequestActivities] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[RequestActivities]  WITH CHECK ADD  CONSTRAINT [FK_RequestActivities_Requests] FOREIGN KEY([RequestId])
REFERENCES [dbo].[Requests] ([Id])
GO





IF EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'RequestBeneficiaries')
BEGIN
	DROP TABLE [dbo].[RequestBeneficiaries]
	
END


CREATE TABLE [dbo].[RequestBeneficiaries](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestId] [bigint] NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[Address] [nvarchar](350) NULL,
	[Description] [nvarchar](500) NULL,
	[Created] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[Updated] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Order] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[RequestBeneficiaries] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[RequestBeneficiaries]  WITH CHECK ADD  CONSTRAINT [FK_RequestBeneficiaries_Requests] FOREIGN KEY([RequestId])
REFERENCES [dbo].[Requests] ([Id])
GO




IF EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'RequestContainers')
BEGIN
	DROP TABLE [dbo].[RequestContainers]
	
END


CREATE TABLE [dbo].[RequestContainers](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestId] [bigint] NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[Address] [nvarchar](350) NULL,
	[Description] [nvarchar](500) NULL,
	[Created] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[Updated] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Order] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[RequestContainers] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[RequestContainers]  WITH CHECK ADD  CONSTRAINT [FK_RequestContainers_Requests] FOREIGN KEY([RequestId])
REFERENCES [dbo].[Requests] ([Id])
GO






insert into departments ( NameArabic, NameEnglish, CollegeId, CreatedBy, UpdatedBy, Created, Updated, IsDeleted) values (N'قسم الأدب الإنجليزي', NULL, (select id from colleges where namearabic = N'اللغات والترجمة'), '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', getdate(), getdate(), 0);

go

delete from DepartmentPriorities where DepartmentId in (select id from Departments where NameArabic = N'قسم 1' or  NameArabic = N'قسم 2')
delete from Departments where NameArabic = N'قسم 1' or  NameArabic = N'قسم 2'
go


update Departments set namearabic = replace(namearabic, N'قسم ', N'')
go
IF NOT EXISTS (select * from Priorities where namearabic = N'مقاومة الأفكار المنحرفة كالإرهاب والتكفير') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'مقاومة الأفكار المنحرفة كالإرهاب والتكفير', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'القران الكريم وعلومه') and priorityid = (Select id from Priorities where namearabic = N'مقاومة الأفكار المنحرفة كالإرهاب والتكفير')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'القران الكريم وعلومه'),  (Select id from Priorities where namearabic = N'مقاومة الأفكار المنحرفة كالإرهاب والتكفير')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دفع الشبهات عن القرآن الكريم') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دفع الشبهات عن القرآن الكريم', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'القران الكريم وعلومه') and priorityid = (Select id from Priorities where namearabic = N'دفع الشبهات عن القرآن الكريم')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'القران الكريم وعلومه'),  (Select id from Priorities where namearabic = N'دفع الشبهات عن القرآن الكريم')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'إبراز الإعجاز العلمي للقرآن الكريم') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'إبراز الإعجاز العلمي للقرآن الكريم', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'القران الكريم وعلومه') and priorityid = (Select id from Priorities where namearabic = N'إبراز الإعجاز العلمي للقرآن الكريم')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'القران الكريم وعلومه'),  (Select id from Priorities where namearabic = N'إبراز الإعجاز العلمي للقرآن الكريم')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'نشر علوم القرآن للناطقين بغير اللغة العربية') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'نشر علوم القرآن للناطقين بغير اللغة العربية', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'القران الكريم وعلومه') and priorityid = (Select id from Priorities where namearabic = N'نشر علوم القرآن للناطقين بغير اللغة العربية')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'القران الكريم وعلومه'),  (Select id from Priorities where namearabic = N'نشر علوم القرآن للناطقين بغير اللغة العربية')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'موقف القرآن الكريم من القضايا المعاصرة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'موقف القرآن الكريم من القضايا المعاصرة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'القران الكريم وعلومه') and priorityid = (Select id from Priorities where namearabic = N'موقف القرآن الكريم من القضايا المعاصرة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'القران الكريم وعلومه'),  (Select id from Priorities where namearabic = N'موقف القرآن الكريم من القضايا المعاصرة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'إبراز الهدايات والجوانب التربوية في القرآن الكريم') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'إبراز الهدايات والجوانب التربوية في القرآن الكريم', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'القران الكريم وعلومه') and priorityid = (Select id from Priorities where namearabic = N'إبراز الهدايات والجوانب التربوية في القرآن الكريم')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'القران الكريم وعلومه'),  (Select id from Priorities where namearabic = N'إبراز الهدايات والجوانب التربوية في القرآن الكريم')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'العناية بالموسوعات القرآنية') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'العناية بالموسوعات القرآنية', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'القران الكريم وعلومه') and priorityid = (Select id from Priorities where namearabic = N'العناية بالموسوعات القرآنية')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'القران الكريم وعلومه'),  (Select id from Priorities where namearabic = N'العناية بالموسوعات القرآنية')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاهتمام بالدراسات التأصيلية للفكر الإسلامي وبيان موقف القرآن من الأفكار الأخرى') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاهتمام بالدراسات التأصيلية للفكر الإسلامي وبيان موقف القرآن من الأفكار الأخرى', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'القران الكريم وعلومه') and priorityid = (Select id from Priorities where namearabic = N'الاهتمام بالدراسات التأصيلية للفكر الإسلامي وبيان موقف القرآن من الأفكار الأخرى')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'القران الكريم وعلومه'),  (Select id from Priorities where namearabic = N'الاهتمام بالدراسات التأصيلية للفكر الإسلامي وبيان موقف القرآن من الأفكار الأخرى')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بتأصيل العقيدة السلفية وفق منهج أهل السنة . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث التي تتعلق بتأصيل العقيدة السلفية وفق منهج أهل السنة . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بتأصيل العقيدة السلفية وفق منهج أهل السنة . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بتأصيل العقيدة السلفية وفق منهج أهل السنة . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالقضايا والمسائل العقدية المعاصرة . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث التي تتعلق بالقضايا والمسائل العقدية المعاصرة . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالقضايا والمسائل العقدية المعاصرة . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالقضايا والمسائل العقدية المعاصرة . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالمستجدات في المذاهب المعاصرة . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث التي تتعلق بالمستجدات في المذاهب المعاصرة . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالمستجدات في المذاهب المعاصرة . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالمستجدات في المذاهب المعاصرة . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث التي تتناول الفتن وأثرها على المجتمع . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث التي تتناول الفتن وأثرها على المجتمع . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتناول الفتن وأثرها على المجتمع . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتناول الفتن وأثرها على المجتمع . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالحوار بين الأديان والحضارات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث التي تتعلق بالحوار بين الأديان والحضارات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالحوار بين الأديان والحضارات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالحوار بين الأديان والحضارات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالفرق واألحزاب المعاصرة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث التي تتعلق بالفرق واألحزاب المعاصرة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالفرق واألحزاب المعاصرة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العقيدة والمذاهب المعاصرة'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث التي تتعلق بالفرق واألحزاب المعاصرة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في أبواب العبادات. 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الفقهية التي تتناول المسائل المعاصرة في أبواب العبادات. 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه') and priorityid = (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في أبواب العبادات. 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه'),  (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في أبواب العبادات. 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في المعاملات المالية. 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الفقهية التي تتناول المسائل المعاصرة في المعاملات المالية. 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه') and priorityid = (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في المعاملات المالية. 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه'),  (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في المعاملات المالية. 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في األنكحة وفقه الأسرة. 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الفقهية التي تتناول المسائل المعاصرة في األنكحة وفقه الأسرة. 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه') and priorityid = (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في األنكحة وفقه الأسرة. 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه'),  (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في األنكحة وفقه الأسرة. 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا الطبية والبيئية. 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا الطبية والبيئية. 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه') and priorityid = (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا الطبية والبيئية. 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه'),  (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا الطبية والبيئية. 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في العقوبات والتأديب. 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الفقهية التي تتناول المسائل المعاصرة في العقوبات والتأديب. 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه') and priorityid = (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في العقوبات والتأديب. 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه'),  (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في العقوبات والتأديب. 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في الأطعمة والزينة. 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الفقهية التي تتناول المسائل المعاصرة في الأطعمة والزينة. 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه') and priorityid = (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في الأطعمة والزينة. 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه'),  (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في الأطعمة والزينة. 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا ووسائل الإثبات ودراسة األنظمة. 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا ووسائل الإثبات ودراسة األنظمة. 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه') and priorityid = (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا ووسائل الإثبات ودراسة األنظمة. 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه'),  (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا ووسائل الإثبات ودراسة األنظمة. 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في فقه الأقليات المسلمة. 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الفقهية التي تتناول المسائل المعاصرة في فقه الأقليات المسلمة. 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه') and priorityid = (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في فقه الأقليات المسلمة. 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه'),  (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في فقه الأقليات المسلمة. 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في تحقيق المخطوطات التي تعنى بالمذهب والدليل') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الفقهية التي تتناول المسائل المعاصرة في تحقيق المخطوطات التي تعنى بالمذهب والدليل', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه') and priorityid = (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في تحقيق المخطوطات التي تعنى بالمذهب والدليل')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه'),  (Select id from Priorities where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في تحقيق المخطوطات التي تعنى بالمذهب والدليل')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه المعاملات المالية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث المتعلقة بفقه المعاملات المالية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه المقارن') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه المعاملات المالية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه المقارن'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه المعاملات المالية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه الأسرة . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث المتعلقة بفقه الأسرة . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه المقارن') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه الأسرة . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه المقارن'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه الأسرة . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه الجنايات . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث المتعلقة بفقه الجنايات . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه المقارن') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه الجنايات . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه المقارن'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه الجنايات . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه الحدود . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث المتعلقة بفقه الحدود . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه المقارن') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه الحدود . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه المقارن'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه الحدود . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه التعزيرات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث المتعلقة بفقه التعزيرات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه المقارن') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه التعزيرات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه المقارن'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بفقه التعزيرات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التأصيلية لفقه النوازل . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التأصيلية لفقه النوازل . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه المقارن') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التأصيلية لفقه النوازل . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه المقارن'),  (Select id from Priorities where namearabic = N'الدراسات التأصيلية لفقه النوازل . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بتحقيق المخطوطات الفقهية . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات والبحوث المتعلقة بتحقيق المخطوطات الفقهية . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه المقارن') and priorityid = (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بتحقيق المخطوطات الفقهية . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه المقارن'),  (Select id from Priorities where namearabic = N'الدراسات والبحوث المتعلقة بتحقيق المخطوطات الفقهية . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات الفقهية المقارنة بالأنظمة . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات الفقهية المقارنة بالأنظمة . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه المقارن') and priorityid = (Select id from Priorities where namearabic = N'الدراسات الفقهية المقارنة بالأنظمة . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه المقارن'),  (Select id from Priorities where namearabic = N'الدراسات الفقهية المقارنة بالأنظمة . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات القضائية وتطبيقاتها') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات القضائية وتطبيقاتها', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفقه المقارن') and priorityid = (Select id from Priorities where namearabic = N'الدراسات القضائية وتطبيقاتها')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفقه المقارن'),  (Select id from Priorities where namearabic = N'الدراسات القضائية وتطبيقاتها')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول الحكم الشرعي وأدلة الأحكام ودلالات الألفاظ . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول الحكم الشرعي وأدلة الأحكام ودلالات الألفاظ . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول الفقه') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول الحكم الشرعي وأدلة الأحكام ودلالات الألفاظ . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول الفقه'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول الحكم الشرعي وأدلة الأحكام ودلالات الألفاظ . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول الاجتهاد والتقليد والتعارض والترجيح . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول الاجتهاد والتقليد والتعارض والترجيح . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول الفقه') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول الاجتهاد والتقليد والتعارض والترجيح . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول الفقه'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول الاجتهاد والتقليد والتعارض والترجيح . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول جميع المسائل الأصولوية من غير مظانها ودراستها . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول جميع المسائل الأصولوية من غير مظانها ودراستها . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول الفقه') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول جميع المسائل الأصولوية من غير مظانها ودراستها . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول الفقه'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول جميع المسائل الأصولوية من غير مظانها ودراستها . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول أصول العلماء . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول أصول العلماء . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول الفقه') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول أصول العلماء . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول الفقه'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول أصول العلماء . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول تخريج الفروع على الأصول وغيره من أنواع التخريج . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول تخريج الفروع على الأصول وغيره من أنواع التخريج . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول الفقه') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول تخريج الفروع على الأصول وغيره من أنواع التخريج . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول الفقه'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول تخريج الفروع على الأصول وغيره من أنواع التخريج . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول القواعد الفقهية وجمعها والتطبيق عليها . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول القواعد الفقهية وجمعها والتطبيق عليها . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول الفقه') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول القواعد الفقهية وجمعها والتطبيق عليها . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول الفقه'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول القواعد الفقهية وجمعها والتطبيق عليها . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول مقاصد الشريعة . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول مقاصد الشريعة . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول الفقه') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول مقاصد الشريعة . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول الفقه'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول مقاصد الشريعة . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول تحقيق كتب التراث في أصول الفقه والقواعد الفقهية . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول تحقيق كتب التراث في أصول الفقه والقواعد الفقهية . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول الفقه') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول تحقيق كتب التراث في أصول الفقه والقواعد الفقهية . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول الفقه'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول تحقيق كتب التراث في أصول الفقه والقواعد الفقهية . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول إبراز أثر أصول الفقه في ضبط المصطلحات وتفسير النصوص النظامية . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول إبراز أثر أصول الفقه في ضبط المصطلحات وتفسير النصوص النظامية . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول الفقه') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول إبراز أثر أصول الفقه في ضبط المصطلحات وتفسير النصوص النظامية . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول الفقه'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول إبراز أثر أصول الفقه في ضبط المصطلحات وتفسير النصوص النظامية . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول الفكر الأصولي المعاصر') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول الفكر الأصولي المعاصر', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول الفقه') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول الفكر الأصولي المعاصر')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول الفقه'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول الفكر الأصولي المعاصر')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تجميد األموال . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تجميد األموال . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'السياسة الشرعية') and priorityid = (Select id from Priorities where namearabic = N'تجميد األموال . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'السياسة الشرعية'),  (Select id from Priorities where namearabic = N'تجميد األموال . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'رأس المال المعرفي . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'رأس المال المعرفي . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'السياسة الشرعية') and priorityid = (Select id from Priorities where namearabic = N'رأس المال المعرفي . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'السياسة الشرعية'),  (Select id from Priorities where namearabic = N'رأس المال المعرفي . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الحماية الشرعية والنظامية للمنتجات الصوتية . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الحماية الشرعية والنظامية للمنتجات الصوتية . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'السياسة الشرعية') and priorityid = (Select id from Priorities where namearabic = N'الحماية الشرعية والنظامية للمنتجات الصوتية . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'السياسة الشرعية'),  (Select id from Priorities where namearabic = N'الحماية الشرعية والنظامية للمنتجات الصوتية . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'مسؤولية المرشد السياحي . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'مسؤولية المرشد السياحي . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'السياسة الشرعية') and priorityid = (Select id from Priorities where namearabic = N'مسؤولية المرشد السياحي . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'السياسة الشرعية'),  (Select id from Priorities where namearabic = N'مسؤولية المرشد السياحي . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التوفيق بين نظام العالمات التجارية السعودي والنظام المشترك لدول مجلس التعاون الخليجي بشأن العلامة التجارية المشهورة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التوفيق بين نظام العالمات التجارية السعودي والنظام المشترك لدول مجلس التعاون الخليجي بشأن العلامة التجارية المشهورة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'السياسة الشرعية') and priorityid = (Select id from Priorities where namearabic = N'التوفيق بين نظام العالمات التجارية السعودي والنظام المشترك لدول مجلس التعاون الخليجي بشأن العلامة التجارية المشهورة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'السياسة الشرعية'),  (Select id from Priorities where namearabic = N'التوفيق بين نظام العالمات التجارية السعودي والنظام المشترك لدول مجلس التعاون الخليجي بشأن العلامة التجارية المشهورة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسة مقارنة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسة مقارنة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'السياسة الشرعية') and priorityid = (Select id from Priorities where namearabic = N'دراسة مقارنة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'السياسة الشرعية'),  (Select id from Priorities where namearabic = N'دراسة مقارنة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الحماية النظامية للبث عبر التوابع الصناعية من أعلى البحار – دراسة مقارنة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الحماية النظامية للبث عبر التوابع الصناعية من أعلى البحار – دراسة مقارنة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'السياسة الشرعية') and priorityid = (Select id from Priorities where namearabic = N'الحماية النظامية للبث عبر التوابع الصناعية من أعلى البحار – دراسة مقارنة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'السياسة الشرعية'),  (Select id from Priorities where namearabic = N'الحماية النظامية للبث عبر التوابع الصناعية من أعلى البحار – دراسة مقارنة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أحكام تطبيق مبادئ اتفاقية تربس على البلدان النامية – دراسة مقارنة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أحكام تطبيق مبادئ اتفاقية تربس على البلدان النامية – دراسة مقارنة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'السياسة الشرعية') and priorityid = (Select id from Priorities where namearabic = N'أحكام تطبيق مبادئ اتفاقية تربس على البلدان النامية – دراسة مقارنة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'السياسة الشرعية'),  (Select id from Priorities where namearabic = N'أحكام تطبيق مبادئ اتفاقية تربس على البلدان النامية – دراسة مقارنة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البدائل الموضوعية لحماية العلامة التجارية غير المسجلة بموجب أحكام نظام العلامات التجارية السعودي – دراسة مقارنة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البدائل الموضوعية لحماية العلامة التجارية غير المسجلة بموجب أحكام نظام العلامات التجارية السعودي – دراسة مقارنة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'السياسة الشرعية') and priorityid = (Select id from Priorities where namearabic = N'البدائل الموضوعية لحماية العلامة التجارية غير المسجلة بموجب أحكام نظام العلامات التجارية السعودي – دراسة مقارنة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'السياسة الشرعية'),  (Select id from Priorities where namearabic = N'البدائل الموضوعية لحماية العلامة التجارية غير المسجلة بموجب أحكام نظام العلامات التجارية السعودي – دراسة مقارنة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا الحوار والتعايش العلاقة مع الآخر . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الثقافية التي تتناول قضايا الحوار والتعايش العلاقة مع الآخر . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الثقافة الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا الحوار والتعايش العلاقة مع الآخر . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الثقافة الإسلامية'),  (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا الحوار والتعايش العلاقة مع الآخر . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا الإرهاب والعنف . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الثقافية التي تتناول قضايا الإرهاب والعنف . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الثقافة الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا الإرهاب والعنف . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الثقافة الإسلامية'),  (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا الإرهاب والعنف . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا العولمة والهوية والإنتماء والمواطنة . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الثقافية التي تتناول قضايا العولمة والهوية والإنتماء والمواطنة . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الثقافة الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا العولمة والهوية والإنتماء والمواطنة . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الثقافة الإسلامية'),  (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا العولمة والهوية والإنتماء والمواطنة . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا المرأة والطفولة والشباب . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الثقافية التي تتناول قضايا المرأة والطفولة والشباب . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الثقافة الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا المرأة والطفولة والشباب . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الثقافة الإسلامية'),  (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا المرأة والطفولة والشباب . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا العنصرية . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الثقافية التي تتناول قضايا العنصرية . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الثقافة الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا العنصرية . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الثقافة الإسلامية'),  (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول قضايا العنصرية . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الثقافية التي تتناول الدراسات المستقبلية في المجال الثقافي . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الثقافية التي تتناول الدراسات المستقبلية في المجال الثقافي . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الثقافة الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول الدراسات المستقبلية في المجال الثقافي . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الثقافة الإسلامية'),  (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول الدراسات المستقبلية في المجال الثقافي . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الثقافية التي تتناول المذهبيات الثقافية المعاصرة )اللبرالية، الاستشراق ، التنصير، العلمانية، ..( . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الثقافية التي تتناول المذهبيات الثقافية المعاصرة )اللبرالية، الاستشراق ، التنصير، العلمانية، ..( . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الثقافة الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول المذهبيات الثقافية المعاصرة )اللبرالية، الاستشراق ، التنصير، العلمانية، ..( . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الثقافة الإسلامية'),  (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول المذهبيات الثقافية المعاصرة )اللبرالية، الاستشراق ، التنصير، العلمانية، ..( . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الثقافية التي تتناول ثقافات الشعوب المعاصرة . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الثقافية التي تتناول ثقافات الشعوب المعاصرة . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الثقافة الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول ثقافات الشعوب المعاصرة . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الثقافة الإسلامية'),  (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول ثقافات الشعوب المعاصرة . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث الثقافية التي تتناول أخلاقيات المهن المتعددة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث الثقافية التي تتناول أخلاقيات المهن المتعددة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الثقافة الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول أخلاقيات المهن المتعددة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الثقافة الإسلامية'),  (Select id from Priorities where namearabic = N'البحوث الثقافية التي تتناول أخلاقيات المهن المتعددة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'ِّ إحياء التراث القيم ونشره . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'ِّ إحياء التراث القيم ونشره . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'النحو والصرف وفقه اللغة') and priorityid = (Select id from Priorities where namearabic = N'ِّ إحياء التراث القيم ونشره . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'النحو والصرف وفقه اللغة'),  (Select id from Priorities where namearabic = N'ِّ إحياء التراث القيم ونشره . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسة الواقع اللغوي في المجتمع السعودي. 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسة الواقع اللغوي في المجتمع السعودي. 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'النحو والصرف وفقه اللغة') and priorityid = (Select id from Priorities where namearabic = N'دراسة الواقع اللغوي في المجتمع السعودي. 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'النحو والصرف وفقه اللغة'),  (Select id from Priorities where namearabic = N'دراسة الواقع اللغوي في المجتمع السعودي. 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تطوير الدرس النحوي والصرفي وفقًا لمنجزات العلوم اللغوية الحديثة . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تطوير الدرس النحوي والصرفي وفقًا لمنجزات العلوم اللغوية الحديثة . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'النحو والصرف وفقه اللغة') and priorityid = (Select id from Priorities where namearabic = N'تطوير الدرس النحوي والصرفي وفقًا لمنجزات العلوم اللغوية الحديثة . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'النحو والصرف وفقه اللغة'),  (Select id from Priorities where namearabic = N'تطوير الدرس النحوي والصرفي وفقًا لمنجزات العلوم اللغوية الحديثة . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تأليف مراجع مناسبة للقسم. 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تأليف مراجع مناسبة للقسم. 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'النحو والصرف وفقه اللغة') and priorityid = (Select id from Priorities where namearabic = N'تأليف مراجع مناسبة للقسم. 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'النحو والصرف وفقه اللغة'),  (Select id from Priorities where namearabic = N'تأليف مراجع مناسبة للقسم. 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث المشتركة بين علوم اللغة العربية والعلوم الأخرى . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث المشتركة بين علوم اللغة العربية والعلوم الأخرى . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'النحو والصرف وفقه اللغة') and priorityid = (Select id from Priorities where namearabic = N'البحوث المشتركة بين علوم اللغة العربية والعلوم الأخرى . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'النحو والصرف وفقه اللغة'),  (Select id from Priorities where namearabic = N'البحوث المشتركة بين علوم اللغة العربية والعلوم الأخرى . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تيسير اللغة العربية') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تيسير اللغة العربية', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'النحو والصرف وفقه اللغة') and priorityid = (Select id from Priorities where namearabic = N'تيسير اللغة العربية')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'النحو والصرف وفقه اللغة'),  (Select id from Priorities where namearabic = N'تيسير اللغة العربية')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المعاجم المتخصصة في المصطلحات والمفاهيم اللغوية والنقدية والدراسات المتصلة بها. 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المعاجم المتخصصة في المصطلحات والمفاهيم اللغوية والنقدية والدراسات المتصلة بها. 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الادب') and priorityid = (Select id from Priorities where namearabic = N'المعاجم المتخصصة في المصطلحات والمفاهيم اللغوية والنقدية والدراسات المتصلة بها. 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الادب'),  (Select id from Priorities where namearabic = N'المعاجم المتخصصة في المصطلحات والمفاهيم اللغوية والنقدية والدراسات المتصلة بها. 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث والدراسات الأدبية المقارنة. 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث والدراسات الأدبية المقارنة. 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الادب') and priorityid = (Select id from Priorities where namearabic = N'البحوث والدراسات الأدبية المقارنة. 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الادب'),  (Select id from Priorities where namearabic = N'البحوث والدراسات الأدبية المقارنة. 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'إنجاز دراسات تطبيقية في الأدب الحديث في ضوء مناهج التحليل الحديثة. 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'إنجاز دراسات تطبيقية في الأدب الحديث في ضوء مناهج التحليل الحديثة. 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الادب') and priorityid = (Select id from Priorities where namearabic = N'إنجاز دراسات تطبيقية في الأدب الحديث في ضوء مناهج التحليل الحديثة. 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الادب'),  (Select id from Priorities where namearabic = N'إنجاز دراسات تطبيقية في الأدب الحديث في ضوء مناهج التحليل الحديثة. 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تجديد دراسة التراث الأدبي في ضوء المناهج الحديثة. 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تجديد دراسة التراث الأدبي في ضوء المناهج الحديثة. 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الادب') and priorityid = (Select id from Priorities where namearabic = N'تجديد دراسة التراث الأدبي في ضوء المناهج الحديثة. 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الادب'),  (Select id from Priorities where namearabic = N'تجديد دراسة التراث الأدبي في ضوء المناهج الحديثة. 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول عالقة الأدب بالتحولات االجتماعية 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول عالقة الأدب بالتحولات االجتماعية 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الادب') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول عالقة الأدب بالتحولات االجتماعية 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الادب'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول عالقة الأدب بالتحولات االجتماعية 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحقيق النصوص التراثية القيمة والنادرة تحقيقا علميا جيدا 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحقيق النصوص التراثية القيمة والنادرة تحقيقا علميا جيدا 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الادب') and priorityid = (Select id from Priorities where namearabic = N'تحقيق النصوص التراثية القيمة والنادرة تحقيقا علميا جيدا 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الادب'),  (Select id from Priorities where namearabic = N'تحقيق النصوص التراثية القيمة والنادرة تحقيقا علميا جيدا 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التفاعل بين الأدب العربي والثقافة الرقمية. 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التفاعل بين الأدب العربي والثقافة الرقمية. 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الادب') and priorityid = (Select id from Priorities where namearabic = N'التفاعل بين الأدب العربي والثقافة الرقمية. 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الادب'),  (Select id from Priorities where namearabic = N'التفاعل بين الأدب العربي والثقافة الرقمية. 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الأدب العربي في عصر العولمة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الأدب العربي في عصر العولمة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الادب') and priorityid = (Select id from Priorities where namearabic = N'الأدب العربي في عصر العولمة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الادب'),  (Select id from Priorities where namearabic = N'الأدب العربي في عصر العولمة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'مناهج البحث البالغي. 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'مناهج البحث البالغي. 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي') and priorityid = (Select id from Priorities where namearabic = N'مناهج البحث البالغي. 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي'),  (Select id from Priorities where namearabic = N'مناهج البحث البالغي. 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'النظريات البالغية والنقدية في التراث العربي. 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'النظريات البالغية والنقدية في التراث العربي. 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي') and priorityid = (Select id from Priorities where namearabic = N'النظريات البالغية والنقدية في التراث العربي. 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي'),  (Select id from Priorities where namearabic = N'النظريات البالغية والنقدية في التراث العربي. 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'بالغة الفنون األدبية الحديثة. 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'بالغة الفنون األدبية الحديثة. 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي') and priorityid = (Select id from Priorities where namearabic = N'بالغة الفنون األدبية الحديثة. 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي'),  (Select id from Priorities where namearabic = N'بالغة الفنون األدبية الحديثة. 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'بالغة الخطاب المعاصر. 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'بالغة الخطاب المعاصر. 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي') and priorityid = (Select id from Priorities where namearabic = N'بالغة الخطاب المعاصر. 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي'),  (Select id from Priorities where namearabic = N'بالغة الخطاب المعاصر. 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحقيق التراث البالغي األصيل. 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحقيق التراث البالغي األصيل. 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي') and priorityid = (Select id from Priorities where namearabic = N'تحقيق التراث البالغي األصيل. 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي'),  (Select id from Priorities where namearabic = N'تحقيق التراث البالغي األصيل. 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المناهج النقدية الحديثة. 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المناهج النقدية الحديثة. 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي') and priorityid = (Select id from Priorities where namearabic = N'المناهج النقدية الحديثة. 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي'),  (Select id from Priorities where namearabic = N'المناهج النقدية الحديثة. 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تأصيل منهج األدب اإلسالمي. 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تأصيل منهج األدب اإلسالمي. 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي') and priorityid = (Select id from Priorities where namearabic = N'تأصيل منهج األدب اإلسالمي. 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي'),  (Select id from Priorities where namearabic = N'تأصيل منهج األدب اإلسالمي. 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المصطلحات النقدية الحديثة. 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المصطلحات النقدية الحديثة. 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي') and priorityid = (Select id from Priorities where namearabic = N'المصطلحات النقدية الحديثة. 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي'),  (Select id from Priorities where namearabic = N'المصطلحات النقدية الحديثة. 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أدب الطفل') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أدب الطفل', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي') and priorityid = (Select id from Priorities where namearabic = N'أدب الطفل')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'البلاغة والنقد ومنهج الادب الإسلامي'),  (Select id from Priorities where namearabic = N'أدب الطفل')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'اكتساب اللغة الأم واللغات الثانية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'اكتساب اللغة الأم واللغات الثانية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'اكتساب اللغة الأم واللغات الثانية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية'),  (Select id from Priorities where namearabic = N'اكتساب اللغة الأم واللغات الثانية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحليل الخطاب اللغوي . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحليل الخطاب اللغوي . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'تحليل الخطاب اللغوي . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية'),  (Select id from Priorities where namearabic = N'تحليل الخطاب اللغوي . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية'),  (Select id from Priorities where namearabic = N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسة في الصناعة المعجمية المعاصرة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسة في الصناعة المعجمية المعاصرة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'دراسة في الصناعة المعجمية المعاصرة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية'),  (Select id from Priorities where namearabic = N'دراسة في الصناعة المعجمية المعاصرة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسات لغوية حاسوبية . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسات لغوية حاسوبية . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'دراسات لغوية حاسوبية . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية'),  (Select id from Priorities where namearabic = N'دراسات لغوية حاسوبية . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'معالجة قضايا اللغة في المجتمع') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'معالجة قضايا اللغة في المجتمع', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية') and priorityid = (Select id from Priorities where namearabic = N'معالجة قضايا اللغة في المجتمع')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغة العربية والعلوم الإسلامية'),  (Select id from Priorities where namearabic = N'معالجة قضايا اللغة في المجتمع')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'اكتساب اللغة الأم واللغات الثانية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'اكتساب اللغة الأم واللغات الثانية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم اللغة التطبيقي') and priorityid = (Select id from Priorities where namearabic = N'اكتساب اللغة الأم واللغات الثانية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم اللغة التطبيقي'),  (Select id from Priorities where namearabic = N'اكتساب اللغة الأم واللغات الثانية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحليل الخطاب اللغوي . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحليل الخطاب اللغوي . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم اللغة التطبيقي') and priorityid = (Select id from Priorities where namearabic = N'تحليل الخطاب اللغوي . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم اللغة التطبيقي'),  (Select id from Priorities where namearabic = N'تحليل الخطاب اللغوي . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم اللغة التطبيقي') and priorityid = (Select id from Priorities where namearabic = N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم اللغة التطبيقي'),  (Select id from Priorities where namearabic = N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسة في الصناعة المعجمية المعاصرة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسة في الصناعة المعجمية المعاصرة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم اللغة التطبيقي') and priorityid = (Select id from Priorities where namearabic = N'دراسة في الصناعة المعجمية المعاصرة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم اللغة التطبيقي'),  (Select id from Priorities where namearabic = N'دراسة في الصناعة المعجمية المعاصرة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسات لغوية حاسوبية . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسات لغوية حاسوبية . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم اللغة التطبيقي') and priorityid = (Select id from Priorities where namearabic = N'دراسات لغوية حاسوبية . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم اللغة التطبيقي'),  (Select id from Priorities where namearabic = N'دراسات لغوية حاسوبية . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'معالجة قضايا اللغة في المجتمع') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'معالجة قضايا اللغة في المجتمع', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم اللغة التطبيقي') and priorityid = (Select id from Priorities where namearabic = N'معالجة قضايا اللغة في المجتمع')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم اللغة التطبيقي'),  (Select id from Priorities where namearabic = N'معالجة قضايا اللغة في المجتمع')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'اكتساب اللغة الأم واللغات الثانية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'اكتساب اللغة الأم واللغات الثانية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعداد اللغوي') and priorityid = (Select id from Priorities where namearabic = N'اكتساب اللغة الأم واللغات الثانية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعداد اللغوي'),  (Select id from Priorities where namearabic = N'اكتساب اللغة الأم واللغات الثانية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحليل الخطاب اللغوي . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحليل الخطاب اللغوي . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعداد اللغوي') and priorityid = (Select id from Priorities where namearabic = N'تحليل الخطاب اللغوي . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعداد اللغوي'),  (Select id from Priorities where namearabic = N'تحليل الخطاب اللغوي . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعداد اللغوي') and priorityid = (Select id from Priorities where namearabic = N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعداد اللغوي'),  (Select id from Priorities where namearabic = N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسة في الصناعة المعجمية المعاصرة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسة في الصناعة المعجمية المعاصرة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعداد اللغوي') and priorityid = (Select id from Priorities where namearabic = N'دراسة في الصناعة المعجمية المعاصرة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعداد اللغوي'),  (Select id from Priorities where namearabic = N'دراسة في الصناعة المعجمية المعاصرة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسات لغوية حاسوبية . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسات لغوية حاسوبية . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعداد اللغوي') and priorityid = (Select id from Priorities where namearabic = N'دراسات لغوية حاسوبية . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعداد اللغوي'),  (Select id from Priorities where namearabic = N'دراسات لغوية حاسوبية . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'معالجة قضايا اللغة في المجتمع') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'معالجة قضايا اللغة في المجتمع', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعداد اللغوي') and priorityid = (Select id from Priorities where namearabic = N'معالجة قضايا اللغة في المجتمع')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعداد اللغوي'),  (Select id from Priorities where namearabic = N'معالجة قضايا اللغة في المجتمع')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسات مقارنة الأدب االنجليزي الحديث الأدب السعودي . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسات مقارنة الأدب االنجليزي الحديث الأدب السعودي . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الأدب الإنجليزي') and priorityid = (Select id from Priorities where namearabic = N'دراسات مقارنة الأدب االنجليزي الحديث الأدب السعودي . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الأدب الإنجليزي'),  (Select id from Priorities where namearabic = N'دراسات مقارنة الأدب االنجليزي الحديث الأدب السعودي . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الشرق ووجهة نظر المستشرقين كما تصوره األعمال الأدبية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الشرق ووجهة نظر المستشرقين كما تصوره األعمال الأدبية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الأدب الإنجليزي') and priorityid = (Select id from Priorities where namearabic = N'الشرق ووجهة نظر المستشرقين كما تصوره األعمال الأدبية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الأدب الإنجليزي'),  (Select id from Priorities where namearabic = N'الشرق ووجهة نظر المستشرقين كما تصوره األعمال الأدبية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السرد الشارح وإعادة كتابة التاريخ . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السرد الشارح وإعادة كتابة التاريخ . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الأدب الإنجليزي') and priorityid = (Select id from Priorities where namearabic = N'السرد الشارح وإعادة كتابة التاريخ . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الأدب الإنجليزي'),  (Select id from Priorities where namearabic = N'السرد الشارح وإعادة كتابة التاريخ . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'نظرية الدراما الشارحة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'نظرية الدراما الشارحة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الأدب الإنجليزي') and priorityid = (Select id from Priorities where namearabic = N'نظرية الدراما الشارحة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الأدب الإنجليزي'),  (Select id from Priorities where namearabic = N'نظرية الدراما الشارحة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسات نقدية لإلبداع في الأعمال الحائزة على جائزة نوبل للسلام') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسات نقدية لإلبداع في الأعمال الحائزة على جائزة نوبل للسلام', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الأدب الإنجليزي') and priorityid = (Select id from Priorities where namearabic = N'دراسات نقدية لإلبداع في الأعمال الحائزة على جائزة نوبل للسلام')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الأدب الإنجليزي'),  (Select id from Priorities where namearabic = N'دراسات نقدية لإلبداع في الأعمال الحائزة على جائزة نوبل للسلام')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'نظريات الترجمة الأدبية. 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'نظريات الترجمة الأدبية. 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'نظريات الترجمة الأدبية. 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'نظريات الترجمة الأدبية. 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'طرق تدريس الترجمة . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'طرق تدريس الترجمة . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'طرق تدريس الترجمة . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'طرق تدريس الترجمة . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحديات تعليم الترجمة الفورية. 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحديات تعليم الترجمة الفورية. 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'تحديات تعليم الترجمة الفورية. 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'تحديات تعليم الترجمة الفورية. 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الترجمة الآلية : نظرياتها وتطبيقاتها') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الترجمة الآلية : نظرياتها وتطبيقاتها', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'الترجمة الآلية : نظرياتها وتطبيقاتها')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'الترجمة الآلية : نظرياتها وتطبيقاتها')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'فعالية نظرية الذكاء المتعددة في تدريس المهارات اللغوية المختلفة وتعزيز دافعية و اتجاهات الطالب نحو تعلم اللغة . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'فعالية نظرية الذكاء المتعددة في تدريس المهارات اللغوية المختلفة وتعزيز دافعية و اتجاهات الطالب نحو تعلم اللغة . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'فعالية نظرية الذكاء المتعددة في تدريس المهارات اللغوية المختلفة وتعزيز دافعية و اتجاهات الطالب نحو تعلم اللغة . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'فعالية نظرية الذكاء المتعددة في تدريس المهارات اللغوية المختلفة وتعزيز دافعية و اتجاهات الطالب نحو تعلم اللغة . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دور استراتيجيات التعلم الذاتي والتعلم التعاوني في تنمية المهارات اللغوية المختلفة للطالب . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دور استراتيجيات التعلم الذاتي والتعلم التعاوني في تنمية المهارات اللغوية المختلفة للطالب . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'دور استراتيجيات التعلم الذاتي والتعلم التعاوني في تنمية المهارات اللغوية المختلفة للطالب . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'دور استراتيجيات التعلم الذاتي والتعلم التعاوني في تنمية المهارات اللغوية المختلفة للطالب . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'برامج التعلم عن بعد والبرامج المدمجة لتدريب الطالب المعلمين بالجامعات السعودية على مهارات التدريس وإدارة الصف. 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'برامج التعلم عن بعد والبرامج المدمجة لتدريب الطالب المعلمين بالجامعات السعودية على مهارات التدريس وإدارة الصف. 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'برامج التعلم عن بعد والبرامج المدمجة لتدريب الطالب المعلمين بالجامعات السعودية على مهارات التدريس وإدارة الصف. 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'برامج التعلم عن بعد والبرامج المدمجة لتدريب الطالب المعلمين بالجامعات السعودية على مهارات التدريس وإدارة الصف. 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'برامج تدريب وتطوير أداء معلم اللغة اإلنجليزية ما قبل الخدمة. 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'برامج تدريب وتطوير أداء معلم اللغة اإلنجليزية ما قبل الخدمة. 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'برامج تدريب وتطوير أداء معلم اللغة اإلنجليزية ما قبل الخدمة. 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'برامج تدريب وتطوير أداء معلم اللغة اإلنجليزية ما قبل الخدمة. 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحديات تطبيق التعلم الإلكتروني . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحديات تطبيق التعلم الإلكتروني . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'تحديات تطبيق التعلم الإلكتروني . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'تحديات تطبيق التعلم الإلكتروني . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'برامج تدريب وتطوير أداء معلم اللغة الإنجليزية أثناء الخدمة على المستويين المدرسي والجامعي. 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'برامج تدريب وتطوير أداء معلم اللغة الإنجليزية أثناء الخدمة على المستويين المدرسي والجامعي. 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'برامج تدريب وتطوير أداء معلم اللغة الإنجليزية أثناء الخدمة على المستويين المدرسي والجامعي. 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'برامج تدريب وتطوير أداء معلم اللغة الإنجليزية أثناء الخدمة على المستويين المدرسي والجامعي. 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تأثير اكتساب اللغة العربية على اكتساب اللغة الإنجليزية . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تأثير اكتساب اللغة العربية على اكتساب اللغة الإنجليزية . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'تأثير اكتساب اللغة العربية على اكتساب اللغة الإنجليزية . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'تأثير اكتساب اللغة العربية على اكتساب اللغة الإنجليزية . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعددية اللغوية وتأثيرها على كل من الفرد والمجتمع . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعددية اللغوية وتأثيرها على كل من الفرد والمجتمع . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'التعددية اللغوية وتأثيرها على كل من الفرد والمجتمع . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'التعددية اللغوية وتأثيرها على كل من الفرد والمجتمع . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التحليل اللغوي التقابلي بين اللغة العربية واللغة الإنجليزية. 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التحليل اللغوي التقابلي بين اللغة العربية واللغة الإنجليزية. 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'التحليل اللغوي التقابلي بين اللغة العربية واللغة الإنجليزية. 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'التحليل اللغوي التقابلي بين اللغة العربية واللغة الإنجليزية. 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'وسائل التقنية المساعدة في تعلم وتدريس اللغة الإنجليزية') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'وسائل التقنية المساعدة في تعلم وتدريس اللغة الإنجليزية', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'اللغات والترجمة') and priorityid = (Select id from Priorities where namearabic = N'وسائل التقنية المساعدة في تعلم وتدريس اللغة الإنجليزية')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'اللغات والترجمة'),  (Select id from Priorities where namearabic = N'وسائل التقنية المساعدة في تعلم وتدريس اللغة الإنجليزية')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسة السيرة النبوية الراشدة خاصة الدراسات الموجهة نحو الموضوعات الحضارية المستوحاة من هذين العصرين . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسة السيرة النبوية الراشدة خاصة الدراسات الموجهة نحو الموضوعات الحضارية المستوحاة من هذين العصرين . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التاريخ والحضارة') and priorityid = (Select id from Priorities where namearabic = N'دراسة السيرة النبوية الراشدة خاصة الدراسات الموجهة نحو الموضوعات الحضارية المستوحاة من هذين العصرين . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التاريخ والحضارة'),  (Select id from Priorities where namearabic = N'دراسة السيرة النبوية الراشدة خاصة الدراسات الموجهة نحو الموضوعات الحضارية المستوحاة من هذين العصرين . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات في تاريخ المملكة العربية السعودية والدولتين السعوديتين الأولى والثانية. 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات في تاريخ المملكة العربية السعودية والدولتين السعوديتين الأولى والثانية. 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التاريخ والحضارة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات في تاريخ المملكة العربية السعودية والدولتين السعوديتين الأولى والثانية. 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التاريخ والحضارة'),  (Select id from Priorities where namearabic = N'الدراسات في تاريخ المملكة العربية السعودية والدولتين السعوديتين الأولى والثانية. 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات في تاريخ الحرمين الشريفين والحج عبر العصور الإسلامية . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات في تاريخ الحرمين الشريفين والحج عبر العصور الإسلامية . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التاريخ والحضارة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات في تاريخ الحرمين الشريفين والحج عبر العصور الإسلامية . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التاريخ والحضارة'),  (Select id from Priorities where namearabic = N'الدراسات في تاريخ الحرمين الشريفين والحج عبر العصور الإسلامية . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات في مجال المصادر التاريخية القديمة والحديثة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات في مجال المصادر التاريخية القديمة والحديثة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التاريخ والحضارة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات في مجال المصادر التاريخية القديمة والحديثة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التاريخ والحضارة'),  (Select id from Priorities where namearabic = N'الدراسات في مجال المصادر التاريخية القديمة والحديثة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات في مجال الفكر التاريخي وتطوير مناهج البحث في التاريخ . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات في مجال الفكر التاريخي وتطوير مناهج البحث في التاريخ . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التاريخ والحضارة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات في مجال الفكر التاريخي وتطوير مناهج البحث في التاريخ . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التاريخ والحضارة'),  (Select id from Priorities where namearabic = N'الدراسات في مجال الفكر التاريخي وتطوير مناهج البحث في التاريخ . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تاريخ المدن الإسلامية وخططها العمرانية ومرافقها الخداماتية . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تاريخ المدن الإسلامية وخططها العمرانية ومرافقها الخداماتية . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التاريخ والحضارة') and priorityid = (Select id from Priorities where namearabic = N'تاريخ المدن الإسلامية وخططها العمرانية ومرافقها الخداماتية . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التاريخ والحضارة'),  (Select id from Priorities where namearabic = N'تاريخ المدن الإسلامية وخططها العمرانية ومرافقها الخداماتية . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'مراجعة األعمال الأستشراقية عن التاريخ الإسلامي . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'مراجعة األعمال الأستشراقية عن التاريخ الإسلامي . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التاريخ والحضارة') and priorityid = (Select id from Priorities where namearabic = N'مراجعة األعمال الأستشراقية عن التاريخ الإسلامي . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التاريخ والحضارة'),  (Select id from Priorities where namearabic = N'مراجعة األعمال الأستشراقية عن التاريخ الإسلامي . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات الموجهة نحو مظاهر الإبداع الحضاري وتجليات القيم الخيرية الإنسانية . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات الموجهة نحو مظاهر الإبداع الحضاري وتجليات القيم الخيرية الإنسانية . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التاريخ والحضارة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات الموجهة نحو مظاهر الإبداع الحضاري وتجليات القيم الخيرية الإنسانية . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التاريخ والحضارة'),  (Select id from Priorities where namearabic = N'الدراسات الموجهة نحو مظاهر الإبداع الحضاري وتجليات القيم الخيرية الإنسانية . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات الموجهة نحو معرفة أثر الفتن الأزمات الأمنية على تاريخ المسلمين ومسيرتهم الحضارية. 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات الموجهة نحو معرفة أثر الفتن الأزمات الأمنية على تاريخ المسلمين ومسيرتهم الحضارية. 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التاريخ والحضارة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات الموجهة نحو معرفة أثر الفتن الأزمات الأمنية على تاريخ المسلمين ومسيرتهم الحضارية. 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التاريخ والحضارة'),  (Select id from Priorities where namearabic = N'الدراسات الموجهة نحو معرفة أثر الفتن الأزمات الأمنية على تاريخ المسلمين ومسيرتهم الحضارية. 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات المتعلقة بالرؤى الإصلاحية والإنسانية عند قادة الدولة السعودية وملوكها') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات المتعلقة بالرؤى الإصلاحية والإنسانية عند قادة الدولة السعودية وملوكها', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التاريخ والحضارة') and priorityid = (Select id from Priorities where namearabic = N'الدراسات المتعلقة بالرؤى الإصلاحية والإنسانية عند قادة الدولة السعودية وملوكها')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التاريخ والحضارة'),  (Select id from Priorities where namearabic = N'الدراسات المتعلقة بالرؤى الإصلاحية والإنسانية عند قادة الدولة السعودية وملوكها')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات في مجال الجغرافيا الطبيعية للمملكة سواء في الجغرافيا المناخية أو النباتية أو الجغرافيا الحيوية. 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات في مجال الجغرافيا الطبيعية للمملكة سواء في الجغرافيا المناخية أو النباتية أو الجغرافيا الحيوية. 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجغرافيا') and priorityid = (Select id from Priorities where namearabic = N'الدراسات في مجال الجغرافيا الطبيعية للمملكة سواء في الجغرافيا المناخية أو النباتية أو الجغرافيا الحيوية. 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجغرافيا'),  (Select id from Priorities where namearabic = N'الدراسات في مجال الجغرافيا الطبيعية للمملكة سواء في الجغرافيا المناخية أو النباتية أو الجغرافيا الحيوية. 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات البيئية والتي تركز على دراسة المشاكل التي تتعرض لها بيئة المملكة ودراسة المخاطر التي تنتج عن الكوارث البيئية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات البيئية والتي تركز على دراسة المشاكل التي تتعرض لها بيئة المملكة ودراسة المخاطر التي تنتج عن الكوارث البيئية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجغرافيا') and priorityid = (Select id from Priorities where namearabic = N'الدراسات البيئية والتي تركز على دراسة المشاكل التي تتعرض لها بيئة المملكة ودراسة المخاطر التي تنتج عن الكوارث البيئية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجغرافيا'),  (Select id from Priorities where namearabic = N'الدراسات البيئية والتي تركز على دراسة المشاكل التي تتعرض لها بيئة المملكة ودراسة المخاطر التي تنتج عن الكوارث البيئية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات الجغرافية التي تعنى بتطبيق التقنيات الحديثة نظم المعلومات في الجغرافية والاستشعار عن بعد في مسار الجغرافية البشرية والطبيعية بكافة فروعهما . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات الجغرافية التي تعنى بتطبيق التقنيات الحديثة نظم المعلومات في الجغرافية والاستشعار عن بعد في مسار الجغرافية البشرية والطبيعية بكافة فروعهما . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجغرافيا') and priorityid = (Select id from Priorities where namearabic = N'الدراسات الجغرافية التي تعنى بتطبيق التقنيات الحديثة نظم المعلومات في الجغرافية والاستشعار عن بعد في مسار الجغرافية البشرية والطبيعية بكافة فروعهما . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجغرافيا'),  (Select id from Priorities where namearabic = N'الدراسات الجغرافية التي تعنى بتطبيق التقنيات الحديثة نظم المعلومات في الجغرافية والاستشعار عن بعد في مسار الجغرافية البشرية والطبيعية بكافة فروعهما . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات في مجال الجغرافية الاقتصادية وتشمل التعدين والطاقة والأنشطة البشرية المختلفة في المملكة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات في مجال الجغرافية الاقتصادية وتشمل التعدين والطاقة والأنشطة البشرية المختلفة في المملكة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجغرافيا') and priorityid = (Select id from Priorities where namearabic = N'الدراسات في مجال الجغرافية الاقتصادية وتشمل التعدين والطاقة والأنشطة البشرية المختلفة في المملكة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجغرافيا'),  (Select id from Priorities where namearabic = N'الدراسات في مجال الجغرافية الاقتصادية وتشمل التعدين والطاقة والأنشطة البشرية المختلفة في المملكة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات السكانية التي تركز على دراسة سكان المملكة وتوزيعهم الجغرافي وما يتعلق بذلك من قضايا. 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات السكانية التي تركز على دراسة سكان المملكة وتوزيعهم الجغرافي وما يتعلق بذلك من قضايا. 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجغرافيا') and priorityid = (Select id from Priorities where namearabic = N'الدراسات السكانية التي تركز على دراسة سكان المملكة وتوزيعهم الجغرافي وما يتعلق بذلك من قضايا. 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجغرافيا'),  (Select id from Priorities where namearabic = N'الدراسات السكانية التي تركز على دراسة سكان المملكة وتوزيعهم الجغرافي وما يتعلق بذلك من قضايا. 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات العمرانية التي تركز على التخطيط والتنمية العمرانية والتغير العمراني للتجمعات الحضرية في المملكة . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات العمرانية التي تركز على التخطيط والتنمية العمرانية والتغير العمراني للتجمعات الحضرية في المملكة . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجغرافيا') and priorityid = (Select id from Priorities where namearabic = N'الدراسات العمرانية التي تركز على التخطيط والتنمية العمرانية والتغير العمراني للتجمعات الحضرية في المملكة . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجغرافيا'),  (Select id from Priorities where namearabic = N'الدراسات العمرانية التي تركز على التخطيط والتنمية العمرانية والتغير العمراني للتجمعات الحضرية في المملكة . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تركز على التوزيع الجغرافي للخدمات المختلفة )حكومية وخاصة ( ومدى كفاءة التوزيع ودراسة العناصر المؤثرة في توزيع الخدمات . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تركز على التوزيع الجغرافي للخدمات المختلفة )حكومية وخاصة ( ومدى كفاءة التوزيع ودراسة العناصر المؤثرة في توزيع الخدمات . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجغرافيا') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تركز على التوزيع الجغرافي للخدمات المختلفة )حكومية وخاصة ( ومدى كفاءة التوزيع ودراسة العناصر المؤثرة في توزيع الخدمات . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجغرافيا'),  (Select id from Priorities where namearabic = N'الدراسات التي تركز على التوزيع الجغرافي للخدمات المختلفة )حكومية وخاصة ( ومدى كفاءة التوزيع ودراسة العناصر المؤثرة في توزيع الخدمات . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات الخرائطية التي تعنى بدراسة الأساليب الخرائطية التقليدية والرقمية وتقييمها . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات الخرائطية التي تعنى بدراسة الأساليب الخرائطية التقليدية والرقمية وتقييمها . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجغرافيا') and priorityid = (Select id from Priorities where namearabic = N'الدراسات الخرائطية التي تعنى بدراسة الأساليب الخرائطية التقليدية والرقمية وتقييمها . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجغرافيا'),  (Select id from Priorities where namearabic = N'الدراسات الخرائطية التي تعنى بدراسة الأساليب الخرائطية التقليدية والرقمية وتقييمها . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات في الجغرافية السلوكية والتي تركز على التوزيع الجغرافي للظواهر السلوكية في المجتمع في المملكة العربية السعودية . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات في الجغرافية السلوكية والتي تركز على التوزيع الجغرافي للظواهر السلوكية في المجتمع في المملكة العربية السعودية . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجغرافيا') and priorityid = (Select id from Priorities where namearabic = N'الدراسات في الجغرافية السلوكية والتي تركز على التوزيع الجغرافي للظواهر السلوكية في المجتمع في المملكة العربية السعودية . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجغرافيا'),  (Select id from Priorities where namearabic = N'الدراسات في الجغرافية السلوكية والتي تركز على التوزيع الجغرافي للظواهر السلوكية في المجتمع في المملكة العربية السعودية . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات الجغرافية عن العالم الإسلامي والوطن العربي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات الجغرافية عن العالم الإسلامي والوطن العربي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجغرافيا') and priorityid = (Select id from Priorities where namearabic = N'الدراسات الجغرافية عن العالم الإسلامي والوطن العربي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجغرافيا'),  (Select id from Priorities where namearabic = N'الدراسات الجغرافية عن العالم الإسلامي والوطن العربي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'العدالة االجتماعية والأمن الفكري لدى الشباب . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'العدالة االجتماعية والأمن الفكري لدى الشباب . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية') and priorityid = (Select id from Priorities where namearabic = N'العدالة االجتماعية والأمن الفكري لدى الشباب . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية'),  (Select id from Priorities where namearabic = N'العدالة االجتماعية والأمن الفكري لدى الشباب . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التخطيط لزيادة فاعلية برامج الرعاية الاجتماعية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التخطيط لزيادة فاعلية برامج الرعاية الاجتماعية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية') and priorityid = (Select id from Priorities where namearabic = N'التخطيط لزيادة فاعلية برامج الرعاية الاجتماعية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية'),  (Select id from Priorities where namearabic = N'التخطيط لزيادة فاعلية برامج الرعاية الاجتماعية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسات في المشكلات البيئية في المجتمع السعودي من منظور اجتماعي . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسات في المشكلات البيئية في المجتمع السعودي من منظور اجتماعي . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية') and priorityid = (Select id from Priorities where namearabic = N'دراسات في المشكلات البيئية في المجتمع السعودي من منظور اجتماعي . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية'),  (Select id from Priorities where namearabic = N'دراسات في المشكلات البيئية في المجتمع السعودي من منظور اجتماعي . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الأبعاد االجتماعية والاقتصادية للصنف الرمزي في المجتمع السعودي . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الأبعاد االجتماعية والاقتصادية للصنف الرمزي في المجتمع السعودي . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية') and priorityid = (Select id from Priorities where namearabic = N'الأبعاد االجتماعية والاقتصادية للصنف الرمزي في المجتمع السعودي . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية'),  (Select id from Priorities where namearabic = N'الأبعاد االجتماعية والاقتصادية للصنف الرمزي في المجتمع السعودي . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'سياسات التنمية البشرية في المجتمع السعودي ) سياسات التعليم ،الصحة ، السكان ،..(. 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'سياسات التنمية البشرية في المجتمع السعودي ) سياسات التعليم ،الصحة ، السكان ،..(. 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية') and priorityid = (Select id from Priorities where namearabic = N'سياسات التنمية البشرية في المجتمع السعودي ) سياسات التعليم ،الصحة ، السكان ،..(. 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية'),  (Select id from Priorities where namearabic = N'سياسات التنمية البشرية في المجتمع السعودي ) سياسات التعليم ،الصحة ، السكان ،..(. 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدعم االجتماعي للأسرة عند الأزمات والكوارث . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدعم االجتماعي للأسرة عند الأزمات والكوارث . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية') and priorityid = (Select id from Priorities where namearabic = N'الدعم االجتماعي للأسرة عند الأزمات والكوارث . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية'),  (Select id from Priorities where namearabic = N'الدعم االجتماعي للأسرة عند الأزمات والكوارث . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'ثقافة التطوع وأثرها على المجتمع . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'ثقافة التطوع وأثرها على المجتمع . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية') and priorityid = (Select id from Priorities where namearabic = N'ثقافة التطوع وأثرها على المجتمع . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية'),  (Select id from Priorities where namearabic = N'ثقافة التطوع وأثرها على المجتمع . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الخدمة االجتماعية الوقائية وارتباطها بالوعي التنموي . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الخدمة االجتماعية الوقائية وارتباطها بالوعي التنموي . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية') and priorityid = (Select id from Priorities where namearabic = N'الخدمة االجتماعية الوقائية وارتباطها بالوعي التنموي . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية'),  (Select id from Priorities where namearabic = N'الخدمة االجتماعية الوقائية وارتباطها بالوعي التنموي . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التدخالت المهنية في مجال العمل مع األطفال المعاقين ) ذوي الاحتياجات الخاصة (. 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التدخالت المهنية في مجال العمل مع األطفال المعاقين ) ذوي الاحتياجات الخاصة (. 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية') and priorityid = (Select id from Priorities where namearabic = N'التدخالت المهنية في مجال العمل مع األطفال المعاقين ) ذوي الاحتياجات الخاصة (. 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية'),  (Select id from Priorities where namearabic = N'التدخالت المهنية في مجال العمل مع األطفال المعاقين ) ذوي الاحتياجات الخاصة (. 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'العنف الممارس من قبل الشباب') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'العنف الممارس من قبل الشباب', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية') and priorityid = (Select id from Priorities where namearabic = N'العنف الممارس من قبل الشباب')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاجتماع والخدمة الاجتماعية'),  (Select id from Priorities where namearabic = N'العنف الممارس من قبل الشباب')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تنمية المصحة النفسية لأفراد المجتمع . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تنمية المصحة النفسية لأفراد المجتمع . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'تنمية المصحة النفسية لأفراد المجتمع . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'تنمية المصحة النفسية لأفراد المجتمع . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المشكالت النفسية للأطفال والشباب والراشدين والمسنين . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المشكالت النفسية للأطفال والشباب والراشدين والمسنين . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'المشكالت النفسية للأطفال والشباب والراشدين والمسنين . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'المشكالت النفسية للأطفال والشباب والراشدين والمسنين . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الهوية النفسية وتحدياتها المعاصرة . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الهوية النفسية وتحدياتها المعاصرة . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'الهوية النفسية وتحدياتها المعاصرة . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'الهوية النفسية وتحدياتها المعاصرة . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المنظر الإسلامي للموضوعات النفسية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المنظر الإسلامي للموضوعات النفسية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'المنظر الإسلامي للموضوعات النفسية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'المنظر الإسلامي للموضوعات النفسية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المسؤولية الاجتماعية . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المسؤولية الاجتماعية . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'المسؤولية الاجتماعية . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'المسؤولية الاجتماعية . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'مهارات الحياة الزوجية . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'مهارات الحياة الزوجية . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'مهارات الحياة الزوجية . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'مهارات الحياة الزوجية . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تقنيات التواصل االجتماعي الحديث وتوظيفها الإيجابي . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تقنيات التواصل االجتماعي الحديث وتوظيفها الإيجابي . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'تقنيات التواصل االجتماعي الحديث وتوظيفها الإيجابي . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'تقنيات التواصل االجتماعي الحديث وتوظيفها الإيجابي . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعامل الأسري مع المريض النفسي . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعامل الأسري مع المريض النفسي . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'التعامل الأسري مع المريض النفسي . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'التعامل الأسري مع المريض النفسي . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'مهارات الذات والتواصل الاجتماعي . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'مهارات الذات والتواصل الاجتماعي . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'مهارات الذات والتواصل الاجتماعي . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'مهارات الذات والتواصل الاجتماعي . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تتناول الإرهاب من منظور إسلامي. 11') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تتناول الإرهاب من منظور إسلامي. 11', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تتناول الإرهاب من منظور إسلامي. 11')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'الدراسات التي تتناول الإرهاب من منظور إسلامي. 11')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تناولت الشائعات وسمات مصدقيها . 12') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تناولت الشائعات وسمات مصدقيها . 12', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تناولت الشائعات وسمات مصدقيها . 12')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'الدراسات التي تناولت الشائعات وسمات مصدقيها . 12')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الدراسات التي تناولت المخدرات وآثارها النفسية على الفرد والأسرة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الدراسات التي تناولت المخدرات وآثارها النفسية على الفرد والأسرة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علم النفس') and priorityid = (Select id from Priorities where namearabic = N'الدراسات التي تناولت المخدرات وآثارها النفسية على الفرد والأسرة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علم النفس'),  (Select id from Priorities where namearabic = N'الدراسات التي تناولت المخدرات وآثارها النفسية على الفرد والأسرة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاعتماد الأكاديمي لمؤسسات التعليم العالي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاعتماد الأكاديمي لمؤسسات التعليم العالي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول التربية') and priorityid = (Select id from Priorities where namearabic = N'الاعتماد الأكاديمي لمؤسسات التعليم العالي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول التربية'),  (Select id from Priorities where namearabic = N'الاعتماد الأكاديمي لمؤسسات التعليم العالي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الجودة وتطبيقاتها بالتعليم العالمي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الجودة وتطبيقاتها بالتعليم العالمي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول التربية') and priorityid = (Select id from Priorities where namearabic = N'الجودة وتطبيقاتها بالتعليم العالمي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول التربية'),  (Select id from Priorities where namearabic = N'الجودة وتطبيقاتها بالتعليم العالمي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الكراسي البحثية') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الكراسي البحثية', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول التربية') and priorityid = (Select id from Priorities where namearabic = N'الكراسي البحثية')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول التربية'),  (Select id from Priorities where namearabic = N'الكراسي البحثية')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تطوير برامج الدراسات العليا في ضوء الاعتماد الأكاديمي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تطوير برامج الدراسات العليا في ضوء الاعتماد الأكاديمي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول التربية') and priorityid = (Select id from Priorities where namearabic = N'تطوير برامج الدراسات العليا في ضوء الاعتماد الأكاديمي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول التربية'),  (Select id from Priorities where namearabic = N'تطوير برامج الدراسات العليا في ضوء الاعتماد الأكاديمي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'استشراف مستقبل التعليم العالي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'استشراف مستقبل التعليم العالي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول التربية') and priorityid = (Select id from Priorities where namearabic = N'استشراف مستقبل التعليم العالي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول التربية'),  (Select id from Priorities where namearabic = N'استشراف مستقبل التعليم العالي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'برامج التعليم الموازي مالها وما عليها') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'برامج التعليم الموازي مالها وما عليها', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول التربية') and priorityid = (Select id from Priorities where namearabic = N'برامج التعليم الموازي مالها وما عليها')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول التربية'),  (Select id from Priorities where namearabic = N'برامج التعليم الموازي مالها وما عليها')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'منظمات التعليم وكيفية جعل الجامعات منظمات تعليم') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'منظمات التعليم وكيفية جعل الجامعات منظمات تعليم', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول التربية') and priorityid = (Select id from Priorities where namearabic = N'منظمات التعليم وكيفية جعل الجامعات منظمات تعليم')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول التربية'),  (Select id from Priorities where namearabic = N'منظمات التعليم وكيفية جعل الجامعات منظمات تعليم')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'إدارة المعرفة وتطبيقاتها بالجامعات') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'إدارة المعرفة وتطبيقاتها بالجامعات', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول التربية') and priorityid = (Select id from Priorities where namearabic = N'إدارة المعرفة وتطبيقاتها بالجامعات')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول التربية'),  (Select id from Priorities where namearabic = N'إدارة المعرفة وتطبيقاتها بالجامعات')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تطوير المناهج التعليمية وتقويتها') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تطوير المناهج التعليمية وتقويتها', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'أصول التربية') and priorityid = (Select id from Priorities where namearabic = N'تطوير المناهج التعليمية وتقويتها')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'أصول التربية'),  (Select id from Priorities where namearabic = N'تطوير المناهج التعليمية وتقويتها')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المراجعات العلمية للنظريات اإلعالمية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المنهجيات الحديثة في البحث العلمي . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المتغيرات المستجدة في العملية االتصالية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دور الشبكات االجتماعية في التأثير على الجمهور . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البدائل الحديثة لتمويل العمل اإلعالمي . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحديات المحتوى في اإلعالم الجديد . 11', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'جرائم االتصال اإللكتروني') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'جرائم االتصال اإللكتروني', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحافة والنشر  الإكتروني') and priorityid = (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحافة والنشر  الإكتروني'),  (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المراجعات العلمية للنظريات اإلعالمية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المنهجيات الحديثة في البحث العلمي . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المتغيرات المستجدة في العملية االتصالية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دور الشبكات االجتماعية في التأثير على الجمهور . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البدائل الحديثة لتمويل العمل اإلعالمي . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحديات المحتوى في اإلعالم الجديد . 11', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'جرائم االتصال اإللكتروني') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'جرائم االتصال اإللكتروني', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم') and priorityid = (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإذاعة والتليفزيون والفيلم'),  (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المراجعات العلمية للنظريات اإلعالمية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المنهجيات الحديثة في البحث العلمي . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المتغيرات المستجدة في العملية االتصالية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دور الشبكات االجتماعية في التأثير على الجمهور . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البدائل الحديثة لتمويل العمل اإلعالمي . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحديات المحتوى في اإلعالم الجديد . 11', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'جرائم االتصال اإللكتروني') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'جرائم االتصال اإللكتروني', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العلاقات العامة') and priorityid = (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العلاقات العامة'),  (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المراجعات العلمية للنظريات اإلعالمية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المنهجيات الحديثة في البحث العلمي . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المتغيرات المستجدة في العملية االتصالية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دور الشبكات االجتماعية في التأثير على الجمهور . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البدائل الحديثة لتمويل العمل اإلعالمي . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحديات المحتوى في اإلعالم الجديد . 11', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'جرائم االتصال اإللكتروني') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'جرائم االتصال اإللكتروني', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلان والإتصال التسويقي') and priorityid = (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلان والإتصال التسويقي'),  (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المراجعات العلمية للنظريات اإلعالمية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المنهجيات الحديثة في البحث العلمي . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المتغيرات المستجدة في العملية االتصالية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دور الشبكات االجتماعية في التأثير على الجمهور . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البدائل الحديثة لتمويل العمل اإلعالمي . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحديات المحتوى في اإلعالم الجديد . 11', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'جرائم االتصال اإللكتروني') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'جرائم االتصال اإللكتروني', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة') and priorityid = (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجرفيكس والوسائط المتعددة'),  (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المراجعات العلمية للنظريات اإلعالمية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المنهجيات الحديثة في البحث العلمي . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المتغيرات المستجدة في العملية االتصالية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دور الشبكات االجتماعية في التأثير على الجمهور . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البدائل الحديثة لتمويل العمل اإلعالمي . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحديات المحتوى في اإلعالم الجديد . 11', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'جرائم االتصال اإللكتروني') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'جرائم االتصال اإللكتروني', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعلام المتخصص') and priorityid = (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعلام المتخصص'),  (Select id from Priorities where namearabic = N'جرائم االتصال اإللكتروني')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'القضايا المتعلقة بالاستثمار والتمويل . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'القضايا المتعلقة بالاستثمار والتمويل . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاقتصاد') and priorityid = (Select id from Priorities where namearabic = N'القضايا المتعلقة بالاستثمار والتمويل . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاقتصاد'),  (Select id from Priorities where namearabic = N'القضايا المتعلقة بالاستثمار والتمويل . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسات تطبيقية على الاقتصاد السعودي . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسات تطبيقية على الاقتصاد السعودي . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاقتصاد') and priorityid = (Select id from Priorities where namearabic = N'دراسات تطبيقية على الاقتصاد السعودي . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاقتصاد'),  (Select id from Priorities where namearabic = N'دراسات تطبيقية على الاقتصاد السعودي . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'قضايا الاقتصاد اإلسالمي . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'قضايا الاقتصاد اإلسالمي . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاقتصاد') and priorityid = (Select id from Priorities where namearabic = N'قضايا الاقتصاد اإلسالمي . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاقتصاد'),  (Select id from Priorities where namearabic = N'قضايا الاقتصاد اإلسالمي . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السياسات النقدية والمالية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السياسات النقدية والمالية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاقتصاد') and priorityid = (Select id from Priorities where namearabic = N'السياسات النقدية والمالية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاقتصاد'),  (Select id from Priorities where namearabic = N'السياسات النقدية والمالية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الأسوق المالية . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الأسوق المالية . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاقتصاد') and priorityid = (Select id from Priorities where namearabic = N'الأسوق المالية . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاقتصاد'),  (Select id from Priorities where namearabic = N'الأسوق المالية . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'قضايا البطالة والعمل') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'قضايا البطالة والعمل', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الاقتصاد') and priorityid = (Select id from Priorities where namearabic = N'قضايا البطالة والعمل')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الاقتصاد'),  (Select id from Priorities where namearabic = N'قضايا البطالة والعمل')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'بحوث المحاسبة المالية، وما يتصل بها من مواضيع تتعلق بالأفصاح المحاسبي وطرق القياس والعرض لبنود الموجودات والالتزامات وحقوق المالك والإيردات، والمصروفات، وبناء المعايير المحاسبية وغيرها من مواضيع المحاسبة المالية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'بحوث المحاسبة المالية، وما يتصل بها من مواضيع تتعلق بالأفصاح المحاسبي وطرق القياس والعرض لبنود الموجودات والالتزامات وحقوق المالك والإيردات، والمصروفات، وبناء المعايير المحاسبية وغيرها من مواضيع المحاسبة المالية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'المحاسبة') and priorityid = (Select id from Priorities where namearabic = N'بحوث المحاسبة المالية، وما يتصل بها من مواضيع تتعلق بالأفصاح المحاسبي وطرق القياس والعرض لبنود الموجودات والالتزامات وحقوق المالك والإيردات، والمصروفات، وبناء المعايير المحاسبية وغيرها من مواضيع المحاسبة المالية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'المحاسبة'),  (Select id from Priorities where namearabic = N'بحوث المحاسبة المالية، وما يتصل بها من مواضيع تتعلق بالأفصاح المحاسبي وطرق القياس والعرض لبنود الموجودات والالتزامات وحقوق المالك والإيردات، والمصروفات، وبناء المعايير المحاسبية وغيرها من مواضيع المحاسبة المالية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث المتعلقة بالمحاسبة الدولية وما يتصل بها من مواضيع تتعلق بالشركات ذات الاستثمارات الدولية، والتوافق مع المعايير الدولية وغيرها من المواضيع ذات البعد الدولي في المحاسبة . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث المتعلقة بالمحاسبة الدولية وما يتصل بها من مواضيع تتعلق بالشركات ذات الاستثمارات الدولية، والتوافق مع المعايير الدولية وغيرها من المواضيع ذات البعد الدولي في المحاسبة . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'المحاسبة') and priorityid = (Select id from Priorities where namearabic = N'البحوث المتعلقة بالمحاسبة الدولية وما يتصل بها من مواضيع تتعلق بالشركات ذات الاستثمارات الدولية، والتوافق مع المعايير الدولية وغيرها من المواضيع ذات البعد الدولي في المحاسبة . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'المحاسبة'),  (Select id from Priorities where namearabic = N'البحوث المتعلقة بالمحاسبة الدولية وما يتصل بها من مواضيع تتعلق بالشركات ذات الاستثمارات الدولية، والتوافق مع المعايير الدولية وغيرها من المواضيع ذات البعد الدولي في المحاسبة . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'بحوث المحاسبة الإدارية والتكاليف وما يتصل بها من مواضيع ذات عالقة بتحديد التكاليف المستهدفة وأنظمة التكاليف المختلفة وأساليب قياس الأداءواتخاذ القرارات، ووضع الموازنات وغيرها من مواضيع المحاسبة الإدارية . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'بحوث المحاسبة الإدارية والتكاليف وما يتصل بها من مواضيع ذات عالقة بتحديد التكاليف المستهدفة وأنظمة التكاليف المختلفة وأساليب قياس الأداءواتخاذ القرارات، ووضع الموازنات وغيرها من مواضيع المحاسبة الإدارية . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'المحاسبة') and priorityid = (Select id from Priorities where namearabic = N'بحوث المحاسبة الإدارية والتكاليف وما يتصل بها من مواضيع ذات عالقة بتحديد التكاليف المستهدفة وأنظمة التكاليف المختلفة وأساليب قياس الأداءواتخاذ القرارات، ووضع الموازنات وغيرها من مواضيع المحاسبة الإدارية . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'المحاسبة'),  (Select id from Priorities where namearabic = N'بحوث المحاسبة الإدارية والتكاليف وما يتصل بها من مواضيع ذات عالقة بتحديد التكاليف المستهدفة وأنظمة التكاليف المختلفة وأساليب قياس الأداءواتخاذ القرارات، ووضع الموازنات وغيرها من مواضيع المحاسبة الإدارية . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'بحوث المحاسبة في الجهات غير الربحية سواء كانت حكومية أو جهات أخرى غير هادفة للربح وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح. 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'بحوث المحاسبة في الجهات غير الربحية سواء كانت حكومية أو جهات أخرى غير هادفة للربح وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح. 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'المحاسبة') and priorityid = (Select id from Priorities where namearabic = N'بحوث المحاسبة في الجهات غير الربحية سواء كانت حكومية أو جهات أخرى غير هادفة للربح وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح. 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'المحاسبة'),  (Select id from Priorities where namearabic = N'بحوث المحاسبة في الجهات غير الربحية سواء كانت حكومية أو جهات أخرى غير هادفة للربح وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح. 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'بحوث التحليل المالي، وما يتصل بها من مواضيع تتعلق بالتحليل الأساسي والفني، وبناء المؤشرات المالية، وأثر المعلومات المحاسبية على تعاملات الأسواق المالية، ومشاكل إدارة الأرباح، وغيرها من المواضيع ذات الصلة بالتحليل المالي . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'بحوث التحليل المالي، وما يتصل بها من مواضيع تتعلق بالتحليل الأساسي والفني، وبناء المؤشرات المالية، وأثر المعلومات المحاسبية على تعاملات الأسواق المالية، ومشاكل إدارة الأرباح، وغيرها من المواضيع ذات الصلة بالتحليل المالي . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'المحاسبة') and priorityid = (Select id from Priorities where namearabic = N'بحوث التحليل المالي، وما يتصل بها من مواضيع تتعلق بالتحليل الأساسي والفني، وبناء المؤشرات المالية، وأثر المعلومات المحاسبية على تعاملات الأسواق المالية، ومشاكل إدارة الأرباح، وغيرها من المواضيع ذات الصلة بالتحليل المالي . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'المحاسبة'),  (Select id from Priorities where namearabic = N'بحوث التحليل المالي، وما يتصل بها من مواضيع تتعلق بالتحليل الأساسي والفني، وبناء المؤشرات المالية، وأثر المعلومات المحاسبية على تعاملات الأسواق المالية، ومشاكل إدارة الأرباح، وغيرها من المواضيع ذات الصلة بالتحليل المالي . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'بحوث المحاسبة في المؤسسات المالية الإسلامية ، وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح للأدوات المالية والعمليات المتوافقة مع الشريعة، وبناء المعايير المحاسبية، وغيرها من المواضيع ذات الصلة بالمؤسسات المالية الإسلامية. 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'بحوث المحاسبة في المؤسسات المالية الإسلامية ، وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح للأدوات المالية والعمليات المتوافقة مع الشريعة، وبناء المعايير المحاسبية، وغيرها من المواضيع ذات الصلة بالمؤسسات المالية الإسلامية. 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'المحاسبة') and priorityid = (Select id from Priorities where namearabic = N'بحوث المحاسبة في المؤسسات المالية الإسلامية ، وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح للأدوات المالية والعمليات المتوافقة مع الشريعة، وبناء المعايير المحاسبية، وغيرها من المواضيع ذات الصلة بالمؤسسات المالية الإسلامية. 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'المحاسبة'),  (Select id from Priorities where namearabic = N'بحوث المحاسبة في المؤسسات المالية الإسلامية ، وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح للأدوات المالية والعمليات المتوافقة مع الشريعة، وبناء المعايير المحاسبية، وغيرها من المواضيع ذات الصلة بالمؤسسات المالية الإسلامية. 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'بحوث النظم المحاسبية ، وما يتصل بها من مواضيع تتعلق بتصميم النظم ونقدها، التخصصات الإدارية التخصصات التطبيقية 40 وهياكل الرقابة الداخلية، وغيرها من المواضيع ذات الصلة بالنظم المحاسبية اليدوية والآلية. 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'بحوث النظم المحاسبية ، وما يتصل بها من مواضيع تتعلق بتصميم النظم ونقدها، التخصصات الإدارية التخصصات التطبيقية 40 وهياكل الرقابة الداخلية، وغيرها من المواضيع ذات الصلة بالنظم المحاسبية اليدوية والآلية. 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'المحاسبة') and priorityid = (Select id from Priorities where namearabic = N'بحوث النظم المحاسبية ، وما يتصل بها من مواضيع تتعلق بتصميم النظم ونقدها، التخصصات الإدارية التخصصات التطبيقية 40 وهياكل الرقابة الداخلية، وغيرها من المواضيع ذات الصلة بالنظم المحاسبية اليدوية والآلية. 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'المحاسبة'),  (Select id from Priorities where namearabic = N'بحوث النظم المحاسبية ، وما يتصل بها من مواضيع تتعلق بتصميم النظم ونقدها، التخصصات الإدارية التخصصات التطبيقية 40 وهياكل الرقابة الداخلية، وغيرها من المواضيع ذات الصلة بالنظم المحاسبية اليدوية والآلية. 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'بحوث المراجعة سواء تلك المتعلقة بالمراجعة الداخلية أو المراجعة الخارجية، وما يتصل بها من مواضيع تتعلق بتأهيل المراجع وبناء معايير المراجعة، ولجان المراجعة، وغيرها من المواضيع ذات الصلة بعملية المراجعة . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'بحوث المراجعة سواء تلك المتعلقة بالمراجعة الداخلية أو المراجعة الخارجية، وما يتصل بها من مواضيع تتعلق بتأهيل المراجع وبناء معايير المراجعة، ولجان المراجعة، وغيرها من المواضيع ذات الصلة بعملية المراجعة . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'المحاسبة') and priorityid = (Select id from Priorities where namearabic = N'بحوث المراجعة سواء تلك المتعلقة بالمراجعة الداخلية أو المراجعة الخارجية، وما يتصل بها من مواضيع تتعلق بتأهيل المراجع وبناء معايير المراجعة، ولجان المراجعة، وغيرها من المواضيع ذات الصلة بعملية المراجعة . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'المحاسبة'),  (Select id from Priorities where namearabic = N'بحوث المراجعة سواء تلك المتعلقة بالمراجعة الداخلية أو المراجعة الخارجية، وما يتصل بها من مواضيع تتعلق بتأهيل المراجع وبناء معايير المراجعة، ولجان المراجعة، وغيرها من المواضيع ذات الصلة بعملية المراجعة . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث المتعلقة بالمحاسبة عن الزكاة والضريبة . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث المتعلقة بالمحاسبة عن الزكاة والضريبة . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'المحاسبة') and priorityid = (Select id from Priorities where namearabic = N'البحوث المتعلقة بالمحاسبة عن الزكاة والضريبة . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'المحاسبة'),  (Select id from Priorities where namearabic = N'البحوث المتعلقة بالمحاسبة عن الزكاة والضريبة . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث المتعلقة بحوكمة الشركات، وما يتصل بها من مواضيع تتعلق بآليات الحوكمة ، وآثارها على المعلومات المحاسبية . 11') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث المتعلقة بحوكمة الشركات، وما يتصل بها من مواضيع تتعلق بآليات الحوكمة ، وآثارها على المعلومات المحاسبية . 11', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'المحاسبة') and priorityid = (Select id from Priorities where namearabic = N'البحوث المتعلقة بحوكمة الشركات، وما يتصل بها من مواضيع تتعلق بآليات الحوكمة ، وآثارها على المعلومات المحاسبية . 11')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'المحاسبة'),  (Select id from Priorities where namearabic = N'البحوث المتعلقة بحوكمة الشركات، وما يتصل بها من مواضيع تتعلق بآليات الحوكمة ، وآثارها على المعلومات المحاسبية . 11')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'البحوث المتعلقة بالتعليم المحاسبي والتأهيل المهني وما يتصل بها من مواضيع تتعلق ببناء المناهج والبرامج ومخرجاتها') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'البحوث المتعلقة بالتعليم المحاسبي والتأهيل المهني وما يتصل بها من مواضيع تتعلق ببناء المناهج والبرامج ومخرجاتها', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'المحاسبة') and priorityid = (Select id from Priorities where namearabic = N'البحوث المتعلقة بالتعليم المحاسبي والتأهيل المهني وما يتصل بها من مواضيع تتعلق ببناء المناهج والبرامج ومخرجاتها')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'المحاسبة'),  (Select id from Priorities where namearabic = N'البحوث المتعلقة بالتعليم المحاسبي والتأهيل المهني وما يتصل بها من مواضيع تتعلق ببناء المناهج والبرامج ومخرجاتها')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التمويل الإسلامي. 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التمويل الإسلامي. 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التمويل والاستثمار') and priorityid = (Select id from Priorities where namearabic = N'التمويل الإسلامي. 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التمويل والاستثمار'),  (Select id from Priorities where namearabic = N'التمويل الإسلامي. 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تطوير قطاع التمويل والاستثمارفي المملكة . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تطوير قطاع التمويل والاستثمارفي المملكة . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التمويل والاستثمار') and priorityid = (Select id from Priorities where namearabic = N'تطوير قطاع التمويل والاستثمارفي المملكة . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التمويل والاستثمار'),  (Select id from Priorities where namearabic = N'تطوير قطاع التمويل والاستثمارفي المملكة . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الأسواق المالية . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الأسواق المالية . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التمويل والاستثمار') and priorityid = (Select id from Priorities where namearabic = N'الأسواق المالية . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التمويل والاستثمار'),  (Select id from Priorities where namearabic = N'الأسواق المالية . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'حوكمة الشركات . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'حوكمة الشركات . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التمويل والاستثمار') and priorityid = (Select id from Priorities where namearabic = N'حوكمة الشركات . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التمويل والاستثمار'),  (Select id from Priorities where namearabic = N'حوكمة الشركات . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التأمين وإدارة المخاطر') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التأمين وإدارة المخاطر', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التمويل والاستثمار') and priorityid = (Select id from Priorities where namearabic = N'التأمين وإدارة المخاطر')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التمويل والاستثمار'),  (Select id from Priorities where namearabic = N'التأمين وإدارة المخاطر')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'استراتيجيات تنمية رأس المال الفكري في المنظمات السعودية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'استراتيجيات تنمية رأس المال الفكري في المنظمات السعودية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة الاعمال') and priorityid = (Select id from Priorities where namearabic = N'استراتيجيات تنمية رأس المال الفكري في المنظمات السعودية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة الاعمال'),  (Select id from Priorities where namearabic = N'استراتيجيات تنمية رأس المال الفكري في المنظمات السعودية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'إدارة الجدارات البشرية في المنظمات السعودية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'إدارة الجدارات البشرية في المنظمات السعودية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة الاعمال') and priorityid = (Select id from Priorities where namearabic = N'إدارة الجدارات البشرية في المنظمات السعودية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة الاعمال'),  (Select id from Priorities where namearabic = N'إدارة الجدارات البشرية في المنظمات السعودية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاتجاهات الحديثة في التسويق بالمنظمات السعودية : بين النظرية والتطبيق . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاتجاهات الحديثة في التسويق بالمنظمات السعودية : بين النظرية والتطبيق . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة الاعمال') and priorityid = (Select id from Priorities where namearabic = N'الاتجاهات الحديثة في التسويق بالمنظمات السعودية : بين النظرية والتطبيق . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة الاعمال'),  (Select id from Priorities where namearabic = N'الاتجاهات الحديثة في التسويق بالمنظمات السعودية : بين النظرية والتطبيق . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'إدارة الموارد البشرية في ظل التنوع الثقافي للعاملين بالمنظمات السعودية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'إدارة الموارد البشرية في ظل التنوع الثقافي للعاملين بالمنظمات السعودية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة الاعمال') and priorityid = (Select id from Priorities where namearabic = N'إدارة الموارد البشرية في ظل التنوع الثقافي للعاملين بالمنظمات السعودية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة الاعمال'),  (Select id from Priorities where namearabic = N'إدارة الموارد البشرية في ظل التنوع الثقافي للعاملين بالمنظمات السعودية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'نظم الإنتاج الحديثة وأثرها في تطوير المنظمات الصناعية بالمملكة العربية السعودية . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'نظم الإنتاج الحديثة وأثرها في تطوير المنظمات الصناعية بالمملكة العربية السعودية . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة الاعمال') and priorityid = (Select id from Priorities where namearabic = N'نظم الإنتاج الحديثة وأثرها في تطوير المنظمات الصناعية بالمملكة العربية السعودية . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة الاعمال'),  (Select id from Priorities where namearabic = N'نظم الإنتاج الحديثة وأثرها في تطوير المنظمات الصناعية بالمملكة العربية السعودية . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسة تحليلية لبعض االتجاهات السلوكية للعاملين بالمنظمات السعودية . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسة تحليلية لبعض االتجاهات السلوكية للعاملين بالمنظمات السعودية . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة الاعمال') and priorityid = (Select id from Priorities where namearabic = N'دراسة تحليلية لبعض االتجاهات السلوكية للعاملين بالمنظمات السعودية . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة الاعمال'),  (Select id from Priorities where namearabic = N'دراسة تحليلية لبعض االتجاهات السلوكية للعاملين بالمنظمات السعودية . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'إدارة الجودة الشاملة والتميز في التعليم العالي بالمملكة العربية السعودية . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'إدارة الجودة الشاملة والتميز في التعليم العالي بالمملكة العربية السعودية . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة الاعمال') and priorityid = (Select id from Priorities where namearabic = N'إدارة الجودة الشاملة والتميز في التعليم العالي بالمملكة العربية السعودية . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة الاعمال'),  (Select id from Priorities where namearabic = N'إدارة الجودة الشاملة والتميز في التعليم العالي بالمملكة العربية السعودية . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المشروعات الصغيرة والمتوسطة بالمملكة العربية السعودية :الواقع والتحديات والطموحات . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المشروعات الصغيرة والمتوسطة بالمملكة العربية السعودية :الواقع والتحديات والطموحات . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة الاعمال') and priorityid = (Select id from Priorities where namearabic = N'المشروعات الصغيرة والمتوسطة بالمملكة العربية السعودية :الواقع والتحديات والطموحات . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة الاعمال'),  (Select id from Priorities where namearabic = N'المشروعات الصغيرة والمتوسطة بالمملكة العربية السعودية :الواقع والتحديات والطموحات . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'اإلدارة الاستراتيجية للبحث العلمي بالمملكة العربية السعودية . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'اإلدارة الاستراتيجية للبحث العلمي بالمملكة العربية السعودية . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة الاعمال') and priorityid = (Select id from Priorities where namearabic = N'اإلدارة الاستراتيجية للبحث العلمي بالمملكة العربية السعودية . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة الاعمال'),  (Select id from Priorities where namearabic = N'اإلدارة الاستراتيجية للبحث العلمي بالمملكة العربية السعودية . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الحوكمة في المنظمات السعودية') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الحوكمة في المنظمات السعودية', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة الاعمال') and priorityid = (Select id from Priorities where namearabic = N'الحوكمة في المنظمات السعودية')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة الاعمال'),  (Select id from Priorities where namearabic = N'الحوكمة في المنظمات السعودية')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'ظاهرة التحول من البنوك التقليدية إلى الصيرفة الإسلامية. 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'ظاهرة التحول من البنوك التقليدية إلى الصيرفة الإسلامية. 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعمال المصرفية') and priorityid = (Select id from Priorities where namearabic = N'ظاهرة التحول من البنوك التقليدية إلى الصيرفة الإسلامية. 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعمال المصرفية'),  (Select id from Priorities where namearabic = N'ظاهرة التحول من البنوك التقليدية إلى الصيرفة الإسلامية. 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أثر متغيرات عناصر المركز المالي في ربحية المصارف الإسلامية. 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أثر متغيرات عناصر المركز المالي في ربحية المصارف الإسلامية. 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعمال المصرفية') and priorityid = (Select id from Priorities where namearabic = N'أثر متغيرات عناصر المركز المالي في ربحية المصارف الإسلامية. 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعمال المصرفية'),  (Select id from Priorities where namearabic = N'أثر متغيرات عناصر المركز المالي في ربحية المصارف الإسلامية. 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'كفاية رأس مال البنوك المتخصصة والبنوك الشاملة . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'كفاية رأس مال البنوك المتخصصة والبنوك الشاملة . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعمال المصرفية') and priorityid = (Select id from Priorities where namearabic = N'كفاية رأس مال البنوك المتخصصة والبنوك الشاملة . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعمال المصرفية'),  (Select id from Priorities where namearabic = N'كفاية رأس مال البنوك المتخصصة والبنوك الشاملة . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'درجة أمان البنوك التجارية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'درجة أمان البنوك التجارية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعمال المصرفية') and priorityid = (Select id from Priorities where namearabic = N'درجة أمان البنوك التجارية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعمال المصرفية'),  (Select id from Priorities where namearabic = N'درجة أمان البنوك التجارية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'The effect of applying the Basel standards on Bank''s risk 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'The effect of applying the Basel standards on Bank''s risk 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعمال المصرفية') and priorityid = (Select id from Priorities where namearabic = N'The effect of applying the Basel standards on Bank''s risk 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعمال المصرفية'),  (Select id from Priorities where namearabic = N'The effect of applying the Basel standards on Bank''s risk 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'ربحية المصارف التجارية . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'ربحية المصارف التجارية . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعمال المصرفية') and priorityid = (Select id from Priorities where namearabic = N'ربحية المصارف التجارية . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعمال المصرفية'),  (Select id from Priorities where namearabic = N'ربحية المصارف التجارية . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دور البنوك الربوية / التقليدية والبنوك الإسلامية في دفع عجلة التنمية الاقتصادية. 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دور البنوك الربوية / التقليدية والبنوك الإسلامية في دفع عجلة التنمية الاقتصادية. 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعمال المصرفية') and priorityid = (Select id from Priorities where namearabic = N'دور البنوك الربوية / التقليدية والبنوك الإسلامية في دفع عجلة التنمية الاقتصادية. 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعمال المصرفية'),  (Select id from Priorities where namearabic = N'دور البنوك الربوية / التقليدية والبنوك الإسلامية في دفع عجلة التنمية الاقتصادية. 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تسعير الخدمات في المصارف التجارية . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تسعير الخدمات في المصارف التجارية . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعمال المصرفية') and priorityid = (Select id from Priorities where namearabic = N'تسعير الخدمات في المصارف التجارية . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعمال المصرفية'),  (Select id from Priorities where namearabic = N'تسعير الخدمات في المصارف التجارية . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تطوير السياسة التمويلية لدعم عمليات التمويل في المصارف الإسلامية. 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تطوير السياسة التمويلية لدعم عمليات التمويل في المصارف الإسلامية. 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعمال المصرفية') and priorityid = (Select id from Priorities where namearabic = N'تطوير السياسة التمويلية لدعم عمليات التمويل في المصارف الإسلامية. 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعمال المصرفية'),  (Select id from Priorities where namearabic = N'تطوير السياسة التمويلية لدعم عمليات التمويل في المصارف الإسلامية. 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'حوكمة المؤسسات المالية والمصرفية') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'حوكمة المؤسسات المالية والمصرفية', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الإعمال المصرفية') and priorityid = (Select id from Priorities where namearabic = N'حوكمة المؤسسات المالية والمصرفية')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الإعمال المصرفية'),  (Select id from Priorities where namearabic = N'حوكمة المؤسسات المالية والمصرفية')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الحوسبة في خدمة المجالات الشرعية واللغة العربية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الحوسبة في خدمة المجالات الشرعية واللغة العربية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الحاسب') and priorityid = (Select id from Priorities where namearabic = N'الحوسبة في خدمة المجالات الشرعية واللغة العربية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الحاسب'),  (Select id from Priorities where namearabic = N'الحوسبة في خدمة المجالات الشرعية واللغة العربية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'هندسة البرمجيات . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'هندسة البرمجيات . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الحاسب') and priorityid = (Select id from Priorities where namearabic = N'هندسة البرمجيات . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الحاسب'),  (Select id from Priorities where namearabic = N'هندسة البرمجيات . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'قواعد البيانات وتعدين البيانات . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'قواعد البيانات وتعدين البيانات . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الحاسب') and priorityid = (Select id from Priorities where namearabic = N'قواعد البيانات وتعدين البيانات . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الحاسب'),  (Select id from Priorities where namearabic = N'قواعد البيانات وتعدين البيانات . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمن المعلومات . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمن المعلومات . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الحاسب') and priorityid = (Select id from Priorities where namearabic = N'أمن المعلومات . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الحاسب'),  (Select id from Priorities where namearabic = N'أمن المعلومات . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الشبكات والشبكات المتنقلة . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الشبكات والشبكات المتنقلة . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الحاسب') and priorityid = (Select id from Priorities where namearabic = N'الشبكات والشبكات المتنقلة . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الحاسب'),  (Select id from Priorities where namearabic = N'الشبكات والشبكات المتنقلة . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الحوسبة عالية الأداء. 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الحوسبة عالية الأداء. 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الحاسب') and priorityid = (Select id from Priorities where namearabic = N'الحوسبة عالية الأداء. 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الحاسب'),  (Select id from Priorities where namearabic = N'الحوسبة عالية الأداء. 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الذكاء الصناعي و الروبوت') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الذكاء الصناعي و الروبوت', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الحاسب') and priorityid = (Select id from Priorities where namearabic = N'الذكاء الصناعي و الروبوت')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الحاسب'),  (Select id from Priorities where namearabic = N'الذكاء الصناعي و الروبوت')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'األعمال الإلكترونية. 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'األعمال الإلكترونية. 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'نظم المعلومات') and priorityid = (Select id from Priorities where namearabic = N'األعمال الإلكترونية. 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'نظم المعلومات'),  (Select id from Priorities where namearabic = N'األعمال الإلكترونية. 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'النظم التفاعلية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'النظم التفاعلية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'نظم المعلومات') and priorityid = (Select id from Priorities where namearabic = N'النظم التفاعلية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'نظم المعلومات'),  (Select id from Priorities where namearabic = N'النظم التفاعلية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'نظم دعم اتخاذ القرارات . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'نظم دعم اتخاذ القرارات . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'نظم المعلومات') and priorityid = (Select id from Priorities where namearabic = N'نظم دعم اتخاذ القرارات . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'نظم المعلومات'),  (Select id from Priorities where namearabic = N'نظم دعم اتخاذ القرارات . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'هندسة المتطلبات. 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'هندسة المتطلبات. 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'نظم المعلومات') and priorityid = (Select id from Priorities where namearabic = N'هندسة المتطلبات. 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'نظم المعلومات'),  (Select id from Priorities where namearabic = N'هندسة المتطلبات. 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمن تقنية المعلومات وإدارة المخاطر . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمن تقنية المعلومات وإدارة المخاطر . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'نظم المعلومات') and priorityid = (Select id from Priorities where namearabic = N'أمن تقنية المعلومات وإدارة المخاطر . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'نظم المعلومات'),  (Select id from Priorities where namearabic = N'أمن تقنية المعلومات وإدارة المخاطر . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تقويم نظم المعلومات') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تقويم نظم المعلومات', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'نظم المعلومات') and priorityid = (Select id from Priorities where namearabic = N'تقويم نظم المعلومات')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'نظم المعلومات'),  (Select id from Priorities where namearabic = N'تقويم نظم المعلومات')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'إدارة المعرفة . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'إدارة المعرفة . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'تقنية المعلومات') and priorityid = (Select id from Priorities where namearabic = N'إدارة المعرفة . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'تقنية المعلومات'),  (Select id from Priorities where namearabic = N'إدارة المعرفة . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الأرشفة الإلكترونية. 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الأرشفة الإلكترونية. 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'تقنية المعلومات') and priorityid = (Select id from Priorities where namearabic = N'الأرشفة الإلكترونية. 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'تقنية المعلومات'),  (Select id from Priorities where namearabic = N'الأرشفة الإلكترونية. 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المحتوى الرقمي . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المحتوى الرقمي . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'تقنية المعلومات') and priorityid = (Select id from Priorities where namearabic = N'المحتوى الرقمي . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'تقنية المعلومات'),  (Select id from Priorities where namearabic = N'المحتوى الرقمي . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'النشر الإلكتروني. 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'النشر الإلكتروني. 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'تقنية المعلومات') and priorityid = (Select id from Priorities where namearabic = N'النشر الإلكتروني. 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'تقنية المعلومات'),  (Select id from Priorities where namearabic = N'النشر الإلكتروني. 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'خدمات المعلومات') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'خدمات المعلومات', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'تقنية المعلومات') and priorityid = (Select id from Priorities where namearabic = N'خدمات المعلومات')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'تقنية المعلومات'),  (Select id from Priorities where namearabic = N'خدمات المعلومات')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'إدارة المعرفة . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'إدارة المعرفة . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة المعلومات') and priorityid = (Select id from Priorities where namearabic = N'إدارة المعرفة . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة المعلومات'),  (Select id from Priorities where namearabic = N'إدارة المعرفة . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الأرشفة الإلكترونية. 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الأرشفة الإلكترونية. 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة المعلومات') and priorityid = (Select id from Priorities where namearabic = N'الأرشفة الإلكترونية. 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة المعلومات'),  (Select id from Priorities where namearabic = N'الأرشفة الإلكترونية. 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المحتوى الرقمي . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المحتوى الرقمي . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة المعلومات') and priorityid = (Select id from Priorities where namearabic = N'المحتوى الرقمي . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة المعلومات'),  (Select id from Priorities where namearabic = N'المحتوى الرقمي . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'النشر الإلكتروني. 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'النشر الإلكتروني. 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة المعلومات') and priorityid = (Select id from Priorities where namearabic = N'النشر الإلكتروني. 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة المعلومات'),  (Select id from Priorities where namearabic = N'النشر الإلكتروني. 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'خدمات المعلومات') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'خدمات المعلومات', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'إدارة المعلومات') and priorityid = (Select id from Priorities where namearabic = N'خدمات المعلومات')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'إدارة المعلومات'),  (Select id from Priorities where namearabic = N'خدمات المعلومات')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الجبر ونظرية الأعداد وتطبيقاتهما. 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الجبر ونظرية الأعداد وتطبيقاتهما. 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الرياضيات والاحصاء') and priorityid = (Select id from Priorities where namearabic = N'الجبر ونظرية الأعداد وتطبيقاتهما. 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الرياضيات والاحصاء'),  (Select id from Priorities where namearabic = N'الجبر ونظرية الأعداد وتطبيقاتهما. 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المعادلات التفاضلية وتطبيقاتها. 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المعادلات التفاضلية وتطبيقاتها. 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الرياضيات والاحصاء') and priorityid = (Select id from Priorities where namearabic = N'المعادلات التفاضلية وتطبيقاتها. 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الرياضيات والاحصاء'),  (Select id from Priorities where namearabic = N'المعادلات التفاضلية وتطبيقاتها. 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'نماذج الاحتمالات والإحصاءوتطبيقاتها. 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'نماذج الاحتمالات والإحصاءوتطبيقاتها. 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الرياضيات والاحصاء') and priorityid = (Select id from Priorities where namearabic = N'نماذج الاحتمالات والإحصاءوتطبيقاتها. 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الرياضيات والاحصاء'),  (Select id from Priorities where namearabic = N'نماذج الاحتمالات والإحصاءوتطبيقاتها. 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التحليل العددي والرياضيات الحاسوبية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التحليل العددي والرياضيات الحاسوبية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الرياضيات والاحصاء') and priorityid = (Select id from Priorities where namearabic = N'التحليل العددي والرياضيات الحاسوبية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الرياضيات والاحصاء'),  (Select id from Priorities where namearabic = N'التحليل العددي والرياضيات الحاسوبية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الرياضيات المتقطعة وتطبيقاتها. 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الرياضيات المتقطعة وتطبيقاتها. 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الرياضيات والاحصاء') and priorityid = (Select id from Priorities where namearabic = N'الرياضيات المتقطعة وتطبيقاتها. 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الرياضيات والاحصاء'),  (Select id from Priorities where namearabic = N'الرياضيات المتقطعة وتطبيقاتها. 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التحليل الدالي وتطبيقاته') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التحليل الدالي وتطبيقاته', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الرياضيات والاحصاء') and priorityid = (Select id from Priorities where namearabic = N'التحليل الدالي وتطبيقاته')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الرياضيات والاحصاء'),  (Select id from Priorities where namearabic = N'التحليل الدالي وتطبيقاته')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تقنية النانو . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تقنية النانو . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفيزياء') and priorityid = (Select id from Priorities where namearabic = N'تقنية النانو . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفيزياء'),  (Select id from Priorities where namearabic = N'تقنية النانو . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطاقة المتجددة . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطاقة المتجددة . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفيزياء') and priorityid = (Select id from Priorities where namearabic = N'الطاقة المتجددة . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفيزياء'),  (Select id from Priorities where namearabic = N'الطاقة المتجددة . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطاقة العالية والنووية . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطاقة العالية والنووية . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفيزياء') and priorityid = (Select id from Priorities where namearabic = N'الطاقة العالية والنووية . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفيزياء'),  (Select id from Priorities where namearabic = N'الطاقة العالية والنووية . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الفيزياء البيئية والطبية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الفيزياء البيئية والطبية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفيزياء') and priorityid = (Select id from Priorities where namearabic = N'الفيزياء البيئية والطبية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفيزياء'),  (Select id from Priorities where namearabic = N'الفيزياء البيئية والطبية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الفيزياء النظرية . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الفيزياء النظرية . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفيزياء') and priorityid = (Select id from Priorities where namearabic = N'الفيزياء النظرية . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفيزياء'),  (Select id from Priorities where namearabic = N'الفيزياء النظرية . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الفيزياء التربوية التعليمية . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الفيزياء التربوية التعليمية . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفيزياء') and priorityid = (Select id from Priorities where namearabic = N'الفيزياء التربوية التعليمية . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفيزياء'),  (Select id from Priorities where namearabic = N'الفيزياء التربوية التعليمية . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الأغشية الرقيقة . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الأغشية الرقيقة . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفيزياء') and priorityid = (Select id from Priorities where namearabic = N'الأغشية الرقيقة . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفيزياء'),  (Select id from Priorities where namearabic = N'الأغشية الرقيقة . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'فيزياء البلازما') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'فيزياء البلازما', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الفيزياء') and priorityid = (Select id from Priorities where namearabic = N'فيزياء البلازما')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الفيزياء'),  (Select id from Priorities where namearabic = N'فيزياء البلازما')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الكيمياء البيئية ) تلوث المياه – تلوث الهواء – معالجة النفايات (. 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الكيمياء البيئية ) تلوث المياه – تلوث الهواء – معالجة النفايات (. 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الكيمياء') and priorityid = (Select id from Priorities where namearabic = N'الكيمياء البيئية ) تلوث المياه – تلوث الهواء – معالجة النفايات (. 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الكيمياء'),  (Select id from Priorities where namearabic = N'الكيمياء البيئية ) تلوث المياه – تلوث الهواء – معالجة النفايات (. 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الكيمياء العضوية الصناعية في مجال البلمرات واللدائند – الكيمياء الدوائية الأسمدة العضوية ومعالجة النفايات . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الكيمياء العضوية الصناعية في مجال البلمرات واللدائند – الكيمياء الدوائية الأسمدة العضوية ومعالجة النفايات . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الكيمياء') and priorityid = (Select id from Priorities where namearabic = N'الكيمياء العضوية الصناعية في مجال البلمرات واللدائند – الكيمياء الدوائية الأسمدة العضوية ومعالجة النفايات . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الكيمياء'),  (Select id from Priorities where namearabic = N'الكيمياء العضوية الصناعية في مجال البلمرات واللدائند – الكيمياء الدوائية الأسمدة العضوية ومعالجة النفايات . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الكيمياء غير العضوية التطبيقية في مجال صناعة الأسمدة المركبة الجديدة والمتراكبة وغيرها . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الكيمياء غير العضوية التطبيقية في مجال صناعة الأسمدة المركبة الجديدة والمتراكبة وغيرها . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الكيمياء') and priorityid = (Select id from Priorities where namearabic = N'الكيمياء غير العضوية التطبيقية في مجال صناعة الأسمدة المركبة الجديدة والمتراكبة وغيرها . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الكيمياء'),  (Select id from Priorities where namearabic = N'الكيمياء غير العضوية التطبيقية في مجال صناعة الأسمدة المركبة الجديدة والمتراكبة وغيرها . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'استخدام تكنلوجيا النانو في مجال صناعة المواد المتقدمة ) الأدوية– الكربون النشط – البلمرات ( . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'استخدام تكنلوجيا النانو في مجال صناعة المواد المتقدمة ) الأدوية– الكربون النشط – البلمرات ( . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الكيمياء') and priorityid = (Select id from Priorities where namearabic = N'استخدام تكنلوجيا النانو في مجال صناعة المواد المتقدمة ) الأدوية– الكربون النشط – البلمرات ( . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الكيمياء'),  (Select id from Priorities where namearabic = N'استخدام تكنلوجيا النانو في مجال صناعة المواد المتقدمة ) الأدوية– الكربون النشط – البلمرات ( . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'كتب المقررات الدراسية والدراسات المترجمة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'كتب المقررات الدراسية والدراسات المترجمة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الكيمياء') and priorityid = (Select id from Priorities where namearabic = N'كتب المقررات الدراسية والدراسات المترجمة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الكيمياء'),  (Select id from Priorities where namearabic = N'كتب المقررات الدراسية والدراسات المترجمة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'طب الأسرة') and priorityid = (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'طب الأسرة'),  (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'طب الأسرة') and priorityid = (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'طب الأسرة'),  (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'طب الأسرة') and priorityid = (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'طب الأسرة'),  (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'طب الأسرة') and priorityid = (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'طب الأسرة'),  (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'طب الأسرة') and priorityid = (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'طب الأسرة'),  (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعليم الطبي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعليم الطبي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'طب الأسرة') and priorityid = (Select id from Priorities where namearabic = N'التعليم الطبي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'طب الأسرة'),  (Select id from Priorities where namearabic = N'التعليم الطبي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحة العامة') and priorityid = (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحة العامة'),  (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحة العامة') and priorityid = (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحة العامة'),  (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحة العامة') and priorityid = (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحة العامة'),  (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحة العامة') and priorityid = (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحة العامة'),  (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحة العامة') and priorityid = (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحة العامة'),  (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعليم الطبي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعليم الطبي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الصحة العامة') and priorityid = (Select id from Priorities where namearabic = N'التعليم الطبي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الصحة العامة'),  (Select id from Priorities where namearabic = N'التعليم الطبي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العيون') and priorityid = (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العيون'),  (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العيون') and priorityid = (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العيون'),  (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العيون') and priorityid = (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العيون'),  (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العيون') and priorityid = (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العيون'),  (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العيون') and priorityid = (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العيون'),  (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعليم الطبي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعليم الطبي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'العيون') and priorityid = (Select id from Priorities where namearabic = N'التعليم الطبي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'العيون'),  (Select id from Priorities where namearabic = N'التعليم الطبي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التشريح') and priorityid = (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التشريح'),  (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التشريح') and priorityid = (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التشريح'),  (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التشريح') and priorityid = (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التشريح'),  (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التشريح') and priorityid = (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التشريح'),  (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التشريح') and priorityid = (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التشريح'),  (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعليم الطبي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعليم الطبي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التشريح') and priorityid = (Select id from Priorities where namearabic = N'التعليم الطبي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التشريح'),  (Select id from Priorities where namearabic = N'التعليم الطبي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الامراض') and priorityid = (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الامراض'),  (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الامراض') and priorityid = (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الامراض'),  (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الامراض') and priorityid = (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الامراض'),  (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الامراض') and priorityid = (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الامراض'),  (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الامراض') and priorityid = (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الامراض'),  (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعليم الطبي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعليم الطبي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'علوم الامراض') and priorityid = (Select id from Priorities where namearabic = N'التعليم الطبي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'علوم الامراض'),  (Select id from Priorities where namearabic = N'التعليم الطبي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الكيمياء الحيوية') and priorityid = (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الكيمياء الحيوية'),  (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الكيمياء الحيوية') and priorityid = (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الكيمياء الحيوية'),  (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الكيمياء الحيوية') and priorityid = (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الكيمياء الحيوية'),  (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الكيمياء الحيوية') and priorityid = (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الكيمياء الحيوية'),  (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الكيمياء الحيوية') and priorityid = (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الكيمياء الحيوية'),  (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعليم الطبي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعليم الطبي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الكيمياء الحيوية') and priorityid = (Select id from Priorities where namearabic = N'التعليم الطبي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الكيمياء الحيوية'),  (Select id from Priorities where namearabic = N'التعليم الطبي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الأدوية') and priorityid = (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الأدوية'),  (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الأدوية') and priorityid = (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الأدوية'),  (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الأدوية') and priorityid = (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الأدوية'),  (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الأدوية') and priorityid = (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الأدوية'),  (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الأدوية') and priorityid = (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الأدوية'),  (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعليم الطبي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعليم الطبي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الأدوية') and priorityid = (Select id from Priorities where namearabic = N'التعليم الطبي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الأدوية'),  (Select id from Priorities where namearabic = N'التعليم الطبي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'وظائف الأعضاء') and priorityid = (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'وظائف الأعضاء'),  (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'وظائف الأعضاء') and priorityid = (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'وظائف الأعضاء'),  (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'وظائف الأعضاء') and priorityid = (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'وظائف الأعضاء'),  (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'وظائف الأعضاء') and priorityid = (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'وظائف الأعضاء'),  (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'وظائف الأعضاء') and priorityid = (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'وظائف الأعضاء'),  (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعليم الطبي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعليم الطبي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'وظائف الأعضاء') and priorityid = (Select id from Priorities where namearabic = N'التعليم الطبي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'وظائف الأعضاء'),  (Select id from Priorities where namearabic = N'التعليم الطبي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التعليم الطبي') and priorityid = (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التعليم الطبي'),  (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التعليم الطبي') and priorityid = (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التعليم الطبي'),  (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التعليم الطبي') and priorityid = (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التعليم الطبي'),  (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التعليم الطبي') and priorityid = (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التعليم الطبي'),  (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التعليم الطبي') and priorityid = (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التعليم الطبي'),  (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعليم الطبي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعليم الطبي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'التعليم الطبي') and priorityid = (Select id from Priorities where namearabic = N'التعليم الطبي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'التعليم الطبي'),  (Select id from Priorities where namearabic = N'التعليم الطبي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجلدية') and priorityid = (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجلدية'),  (Select id from Priorities where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجلدية') and priorityid = (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجلدية'),  (Select id from Priorities where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجلدية') and priorityid = (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجلدية'),  (Select id from Priorities where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجلدية') and priorityid = (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجلدية'),  (Select id from Priorities where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجلدية') and priorityid = (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجلدية'),  (Select id from Priorities where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التعليم الطبي') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التعليم الطبي', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الجلدية') and priorityid = (Select id from Priorities where namearabic = N'التعليم الطبي')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الجلدية'),  (Select id from Priorities where namearabic = N'التعليم الطبي')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تحليه المياه . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تحليه المياه . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكيميائية') and priorityid = (Select id from Priorities where namearabic = N'تحليه المياه . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكيميائية'),  (Select id from Priorities where namearabic = N'تحليه المياه . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطاقة المتجددة وتخزين الطاقة . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطاقة المتجددة وتخزين الطاقة . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكيميائية') and priorityid = (Select id from Priorities where namearabic = N'الطاقة المتجددة وتخزين الطاقة . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكيميائية'),  (Select id from Priorities where namearabic = N'الطاقة المتجددة وتخزين الطاقة . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الفصل بالأغشية. 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الفصل بالأغشية. 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكيميائية') and priorityid = (Select id from Priorities where namearabic = N'الفصل بالأغشية. 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكيميائية'),  (Select id from Priorities where namearabic = N'الفصل بالأغشية. 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'مكيفات التبريد بالتبخير . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'مكيفات التبريد بالتبخير . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكيميائية') and priorityid = (Select id from Priorities where namearabic = N'مكيفات التبريد بالتبخير . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكيميائية'),  (Select id from Priorities where namearabic = N'مكيفات التبريد بالتبخير . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'فصل وتخزين ثاني أكسيد الكربون . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'فصل وتخزين ثاني أكسيد الكربون . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكيميائية') and priorityid = (Select id from Priorities where namearabic = N'فصل وتخزين ثاني أكسيد الكربون . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكيميائية'),  (Select id from Priorities where namearabic = N'فصل وتخزين ثاني أكسيد الكربون . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المواد المبلمرة والمركبة . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المواد المبلمرة والمركبة . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكيميائية') and priorityid = (Select id from Priorities where namearabic = N'المواد المبلمرة والمركبة . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكيميائية'),  (Select id from Priorities where namearabic = N'المواد المبلمرة والمركبة . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'هندسة المحفزات . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'هندسة المحفزات . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكيميائية') and priorityid = (Select id from Priorities where namearabic = N'هندسة المحفزات . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكيميائية'),  (Select id from Priorities where namearabic = N'هندسة المحفزات . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'معالجة المياه والمياه العادمة . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'معالجة المياه والمياه العادمة . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكيميائية') and priorityid = (Select id from Priorities where namearabic = N'معالجة المياه والمياه العادمة . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكيميائية'),  (Select id from Priorities where namearabic = N'معالجة المياه والمياه العادمة . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'التلوث البيئي ومعالجته. 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'التلوث البيئي ومعالجته. 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكيميائية') and priorityid = (Select id from Priorities where namearabic = N'التلوث البيئي ومعالجته. 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكيميائية'),  (Select id from Priorities where namearabic = N'التلوث البيئي ومعالجته. 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'معالجة النفايات الكيماوية . 11') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'معالجة النفايات الكيماوية . 11', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكيميائية') and priorityid = (Select id from Priorities where namearabic = N'معالجة النفايات الكيماوية . 11')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكيميائية'),  (Select id from Priorities where namearabic = N'معالجة النفايات الكيماوية . 11')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'مواد النانو') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'مواد النانو', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكيميائية') and priorityid = (Select id from Priorities where namearabic = N'مواد النانو')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكيميائية'),  (Select id from Priorities where namearabic = N'مواد النانو')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'إنتاج ودراسة خصائص خرسانة صديقة للبيئة . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'إنتاج ودراسة خصائص خرسانة صديقة للبيئة . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة المدنية') and priorityid = (Select id from Priorities where namearabic = N'إنتاج ودراسة خصائص خرسانة صديقة للبيئة . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة المدنية'),  (Select id from Priorities where namearabic = N'إنتاج ودراسة خصائص خرسانة صديقة للبيئة . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تطوير مواد وأنظمة إنشائية لتقليل الكلفة في المشاريع السكنية . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تطوير مواد وأنظمة إنشائية لتقليل الكلفة في المشاريع السكنية . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة المدنية') and priorityid = (Select id from Priorities where namearabic = N'تطوير مواد وأنظمة إنشائية لتقليل الكلفة في المشاريع السكنية . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة المدنية'),  (Select id from Priorities where namearabic = N'تطوير مواد وأنظمة إنشائية لتقليل الكلفة في المشاريع السكنية . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تقييم ومراقبة سلوك المنشاءات الخراسانية والمعدنية في المملكة. 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تقييم ومراقبة سلوك المنشاءات الخراسانية والمعدنية في المملكة. 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة المدنية') and priorityid = (Select id from Priorities where namearabic = N'تقييم ومراقبة سلوك المنشاءات الخراسانية والمعدنية في المملكة. 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة المدنية'),  (Select id from Priorities where namearabic = N'تقييم ومراقبة سلوك المنشاءات الخراسانية والمعدنية في المملكة. 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تطوير واستخدام خرسانة ذاتية المعالجة للظروف المناخية المحلية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تطوير واستخدام خرسانة ذاتية المعالجة للظروف المناخية المحلية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة المدنية') and priorityid = (Select id from Priorities where namearabic = N'تطوير واستخدام خرسانة ذاتية المعالجة للظروف المناخية المحلية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة المدنية'),  (Select id from Priorities where namearabic = N'تطوير واستخدام خرسانة ذاتية المعالجة للظروف المناخية المحلية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تخطيط وتصميم الطرق في المملكة العربية السعودية . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تخطيط وتصميم الطرق في المملكة العربية السعودية . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة المدنية') and priorityid = (Select id from Priorities where namearabic = N'تخطيط وتصميم الطرق في المملكة العربية السعودية . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة المدنية'),  (Select id from Priorities where namearabic = N'تخطيط وتصميم الطرق في المملكة العربية السعودية . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'دراسة مسببات تدهور طبقات رصف الطرق السريعة في المملكة العربية السعودية والطرق المثلى لمعالجتها . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'دراسة مسببات تدهور طبقات رصف الطرق السريعة في المملكة العربية السعودية والطرق المثلى لمعالجتها . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة المدنية') and priorityid = (Select id from Priorities where namearabic = N'دراسة مسببات تدهور طبقات رصف الطرق السريعة في المملكة العربية السعودية والطرق المثلى لمعالجتها . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة المدنية'),  (Select id from Priorities where namearabic = N'دراسة مسببات تدهور طبقات رصف الطرق السريعة في المملكة العربية السعودية والطرق المثلى لمعالجتها . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أنظمة النقل العام داخل المدن في المملكة العربية السعودية . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أنظمة النقل العام داخل المدن في المملكة العربية السعودية . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة المدنية') and priorityid = (Select id from Priorities where namearabic = N'أنظمة النقل العام داخل المدن في المملكة العربية السعودية . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة المدنية'),  (Select id from Priorities where namearabic = N'أنظمة النقل العام داخل المدن في المملكة العربية السعودية . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الإدارةالمتكاملة للمصادر المائية . 9') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الإدارةالمتكاملة للمصادر المائية . 9', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة المدنية') and priorityid = (Select id from Priorities where namearabic = N'الإدارةالمتكاملة للمصادر المائية . 9')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة المدنية'),  (Select id from Priorities where namearabic = N'الإدارةالمتكاملة للمصادر المائية . 9')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'معالجة المياه العادمة وإعادة استخدامها . 10') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'معالجة المياه العادمة وإعادة استخدامها . 10', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة المدنية') and priorityid = (Select id from Priorities where namearabic = N'معالجة المياه العادمة وإعادة استخدامها . 10')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة المدنية'),  (Select id from Priorities where namearabic = N'معالجة المياه العادمة وإعادة استخدامها . 10')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'مخاطر السيول في المناطق الحضرية والريفية في المملكة . 11') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'مخاطر السيول في المناطق الحضرية والريفية في المملكة . 11', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة المدنية') and priorityid = (Select id from Priorities where namearabic = N'مخاطر السيول في المناطق الحضرية والريفية في المملكة . 11')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة المدنية'),  (Select id from Priorities where namearabic = N'مخاطر السيول في المناطق الحضرية والريفية في المملكة . 11')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'إدارة النفايات الصلبة والخطرة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'إدارة النفايات الصلبة والخطرة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة المدنية') and priorityid = (Select id from Priorities where namearabic = N'إدارة النفايات الصلبة والخطرة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة المدنية'),  (Select id from Priorities where namearabic = N'إدارة النفايات الصلبة والخطرة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'اتصالات النطاق العريض ) الالياف البصرية –الأجيالالحديثة من الاتصالات المتنقلة ( . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'اتصالات النطاق العريض ) الالياف البصرية –الأجيالالحديثة من الاتصالات المتنقلة ( . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكهربائية') and priorityid = (Select id from Priorities where namearabic = N'اتصالات النطاق العريض ) الالياف البصرية –الأجيالالحديثة من الاتصالات المتنقلة ( . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكهربائية'),  (Select id from Priorities where namearabic = N'اتصالات النطاق العريض ) الالياف البصرية –الأجيالالحديثة من الاتصالات المتنقلة ( . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'معالجة الإشارات والصور الرقمية وتطبيقاتها . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'معالجة الإشارات والصور الرقمية وتطبيقاتها . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكهربائية') and priorityid = (Select id from Priorities where namearabic = N'معالجة الإشارات والصور الرقمية وتطبيقاتها . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكهربائية'),  (Select id from Priorities where namearabic = N'معالجة الإشارات والصور الرقمية وتطبيقاتها . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تطبيقات شبكات الاتصالات والحاسب الآلي. 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تطبيقات شبكات الاتصالات والحاسب الآلي. 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكهربائية') and priorityid = (Select id from Priorities where namearabic = N'تطبيقات شبكات الاتصالات والحاسب الآلي. 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكهربائية'),  (Select id from Priorities where namearabic = N'تطبيقات شبكات الاتصالات والحاسب الآلي. 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'استخدامات النانو في الإلكترونيات الضوئية . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'استخدامات النانو في الإلكترونيات الضوئية . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكهربائية') and priorityid = (Select id from Priorities where namearabic = N'استخدامات النانو في الإلكترونيات الضوئية . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكهربائية'),  (Select id from Priorities where namearabic = N'استخدامات النانو في الإلكترونيات الضوئية . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تطبيقات أنظمة التحكم والربوت. 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تطبيقات أنظمة التحكم والربوت. 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكهربائية') and priorityid = (Select id from Priorities where namearabic = N'تطبيقات أنظمة التحكم والربوت. 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكهربائية'),  (Select id from Priorities where namearabic = N'تطبيقات أنظمة التحكم والربوت. 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تطبيقات أنظمة القياس واالستشعار الحديثة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تطبيقات أنظمة القياس واالستشعار الحديثة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الكهربائية') and priorityid = (Select id from Priorities where namearabic = N'تطبيقات أنظمة القياس واالستشعار الحديثة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الكهربائية'),  (Select id from Priorities where namearabic = N'تطبيقات أنظمة القياس واالستشعار الحديثة')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'المواد المتقدمة . 2') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'المواد المتقدمة . 2', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الميكانيكية') and priorityid = (Select id from Priorities where namearabic = N'المواد المتقدمة . 2')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الميكانيكية'),  (Select id from Priorities where namearabic = N'المواد المتقدمة . 2')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أساليب التصنيع الحديث . 3') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أساليب التصنيع الحديث . 3', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الميكانيكية') and priorityid = (Select id from Priorities where namearabic = N'أساليب التصنيع الحديث . 3')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الميكانيكية'),  (Select id from Priorities where namearabic = N'أساليب التصنيع الحديث . 3')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'الطاقة النظيفة . 4') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'الطاقة النظيفة . 4', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الميكانيكية') and priorityid = (Select id from Priorities where namearabic = N'الطاقة النظيفة . 4')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الميكانيكية'),  (Select id from Priorities where namearabic = N'الطاقة النظيفة . 4')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'علم الحراريات وتحويل الطاقة . 5') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'علم الحراريات وتحويل الطاقة . 5', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الميكانيكية') and priorityid = (Select id from Priorities where namearabic = N'علم الحراريات وتحويل الطاقة . 5')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الميكانيكية'),  (Select id from Priorities where namearabic = N'علم الحراريات وتحويل الطاقة . 5')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'ترشيد استهالك الطاقة . 6') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'ترشيد استهالك الطاقة . 6', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الميكانيكية') and priorityid = (Select id from Priorities where namearabic = N'ترشيد استهالك الطاقة . 6')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الميكانيكية'),  (Select id from Priorities where namearabic = N'ترشيد استهالك الطاقة . 6')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'تقنيات لتحلية وتنقية المياه . 7') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'تقنيات لتحلية وتنقية المياه . 7', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الميكانيكية') and priorityid = (Select id from Priorities where namearabic = N'تقنيات لتحلية وتنقية المياه . 7')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الميكانيكية'),  (Select id from Priorities where namearabic = N'تقنيات لتحلية وتنقية المياه . 7')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'نظم التكييف المتقدمة . 8') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'نظم التكييف المتقدمة . 8', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الميكانيكية') and priorityid = (Select id from Priorities where namearabic = N'نظم التكييف المتقدمة . 8')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الميكانيكية'),  (Select id from Priorities where namearabic = N'نظم التكييف المتقدمة . 8')) end
GO
IF NOT EXISTS (select * from Priorities where namearabic = N'أنظمة الديناميكا والتحكم المتقدمة') begin insert into Priorities (NameArabic, Created, CreatedBy, Updated, UpdatedBy) VALUES (N'أنظمة الديناميكا والتحكم المتقدمة', getdate(), N'00000000-0000-0000-0000-000000000000', getdate(), N'00000000-0000-0000-0000-000000000000') end
GO
IF NOT EXISTS (select * from DepartmentPriorities where departmentid = (select id from departments where namearabic = N'الهندسة الميكانيكية') and priorityid = (Select id from Priorities where namearabic = N'أنظمة الديناميكا والتحكم المتقدمة')) begin insert into DepartmentPriorities (DepartmentId, PriorityId) VALUES ((select id from departments where namearabic = N'الهندسة الميكانيكية'),  (Select id from Priorities where namearabic = N'أنظمة الديناميكا والتحكم المتقدمة')) end
GO





IF EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'RequestTimeline')
BEGIN
	DROP TABLE [dbo].[RequestTimeline]
	
END


CREATE TABLE [dbo].[RequestTimeline](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestId] [bigint] NOT NULL,
	[TaskName] [nvarchar](1000) NOT NULL,
	[ResearcherId] [bigint] NOT NULL,
	[StartingMonth] [int] NULL,
	[EndingMonth] [int] NULL,
	[Created] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[Updated] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Order] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



ALTER TABLE [dbo].[RequestTimeline]  WITH CHECK ADD  CONSTRAINT [FK_RequestTimeline_Requests] FOREIGN KEY([RequestId])
REFERENCES [dbo].[Requests] ([Id])
GO

ALTER TABLE [dbo].[RequestTimeline]  WITH CHECK ADD  CONSTRAINT [FK_RequestTimeline_Researchers] FOREIGN KEY([ResearcherId])
REFERENCES [dbo].[Researchers] ([Id])
GO

ALTER TABLE [dbo].[RequestTimeline]  WITH CHECK ADD  CONSTRAINT [FK_RequestTimeline_Users_CreatedBy] FOREIGN KEY(CreatedBy)
REFERENCES [dbo].[Users] ([Id])

ALTER TABLE [dbo].[RequestTimeline]  WITH CHECK ADD  CONSTRAINT [FK_RequestTimeline_Users_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[Users] ([Id])








IF EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'RequestMaterials')
BEGIN
	DROP TABLE [dbo].[RequestMaterials]
	
END


CREATE TABLE [dbo].[RequestMaterials](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestId] [bigint] NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[Type] [tinyint] NOT NULL,
	[Amount] [float] NOT NULL,
	[Cost] [float] NOT NULL,
	[Created] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[Updated] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](128) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Order] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



ALTER TABLE [dbo].[RequestMaterials]  WITH CHECK ADD  CONSTRAINT [FK_RequestMaterials_Requests] FOREIGN KEY([RequestId])
REFERENCES [dbo].[Requests] ([Id])
GO


ALTER TABLE [dbo].[RequestMaterials]  WITH CHECK ADD  CONSTRAINT [FK_RequestMaterials_Users_CreatedBy] FOREIGN KEY(CreatedBy)
REFERENCES [dbo].[Users] ([Id])

ALTER TABLE [dbo].[RequestMaterials]  WITH CHECK ADD  CONSTRAINT [FK_RequestMaterials_Users_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[Users] ([Id])


go

update priorities set namearabic = N'The effect of applying the Basel standards on Bank''s risk 6' where namearabic = N'The effect of applying the Basel standards on Bank''s risk 6';
update priorities set namearabic = N'إبراز الإعجاز العلمي للقرآن الكريم' where namearabic = N'إبراز الإعجاز العلمي للقرآن الكريم';
update priorities set namearabic = N'إبراز الهدايات والجوانب التربوية في القرآن الكريم' where namearabic = N'إبراز الهدايات والجوانب التربوية في القرآن الكريم';
update priorities set namearabic = N'أثر متغيرات عناصر المركز المالي في ربحية المصارف الإسلامية' where namearabic = N'أثر متغيرات عناصر المركز المالي في ربحية المصارف الإسلامية. 3';
update priorities set namearabic = N'أحكام تطبيق مبادئ اتفاقية تربس على البلدان النامية – دراسة مقارنة' where namearabic = N'أحكام تطبيق مبادئ اتفاقية تربس على البلدان النامية – دراسة مقارنة';
update priorities set namearabic = N'إدارة الجدارات البشرية في المنظمات السعودية' where namearabic = N'إدارة الجدارات البشرية في المنظمات السعودية . 3';
update priorities set namearabic = N'إدارة الجودة الشاملة والتميز في التعليم العالي بالمملكة العربية السعودية' where namearabic = N'إدارة الجودة الشاملة والتميز في التعليم العالي بالمملكة العربية السعودية . 8';
update priorities set namearabic = N'إدارة المعرفة' where namearabic = N'إدارة المعرفة . 2';
update priorities set namearabic = N'إدارة المعرفة وتطبيقاتها بالجامعات' where namearabic = N'إدارة المعرفة وتطبيقاتها بالجامعات';
update priorities set namearabic = N'إدارة الموارد البشرية في ظل التنوع الثقافي للعاملين بالمنظمات السعودية' where namearabic = N'إدارة الموارد البشرية في ظل التنوع الثقافي للعاملين بالمنظمات السعودية . 5';
update priorities set namearabic = N'إدارة النفايات الصلبة والخطرة' where namearabic = N'إدارة النفايات الصلبة والخطرة';
update priorities set namearabic = N'أدب الطفل' where namearabic = N'أدب الطفل';
update priorities set namearabic = N'أساليب التصنيع الحديث' where namearabic = N'أساليب التصنيع الحديث . 3';
update priorities set namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية' where namearabic = N'أمراض القلب والشرايين : الأبحاث في أمراض القلب والأوعية الدموية . أ. العالج الدوائي الجيني . ب. الاستعداد الوراثي الجيني لأمراض القلب والأوعية الدموية . 3';
update priorities set namearabic = N'أمن المعلومات' where namearabic = N'أمن المعلومات . 5';
update priorities set namearabic = N'أمن تقنية المعلومات وإدارة المخاطر' where namearabic = N'أمن تقنية المعلومات وإدارة المخاطر . 6';
update priorities set namearabic = N'إنتاج ودراسة خصائص خرسانة صديقة للبيئة' where namearabic = N'إنتاج ودراسة خصائص خرسانة صديقة للبيئة . 2';
update priorities set namearabic = N'إنجاز دراسات تطبيقية في الأدب الحديث في ضوء مناهج التحليل الحديثة' where namearabic = N'إنجاز دراسات تطبيقية في الأدب الحديث في ضوء مناهج التحليل الحديثة. 4';
update priorities set namearabic = N'أنظمة الديناميكا والتحكم المتقدمة' where namearabic = N'أنظمة الديناميكا والتحكم المتقدمة';
update priorities set namearabic = N'أنظمة النقل العام داخل المدن في المملكة العربية السعودية' where namearabic = N'أنظمة النقل العام داخل المدن في المملكة العربية السعودية . 8';
update priorities set namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير' where namearabic = N'أنماط التعرض لإلعالم الجديد وعالقته بنظريات التأثير . 10';
update priorities set namearabic = N'اإلدارة الاستراتيجية للبحث العلمي بالمملكة العربية السعودية' where namearabic = N'اإلدارة الاستراتيجية للبحث العلمي بالمملكة العربية السعودية . 10';
update priorities set namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي' where namearabic = N'األسس الفكرية واأليديولوجية للعمل اإلعالمي . 4';
update priorities set namearabic = N'األعمال الإلكترونية' where namearabic = N'األعمال الإلكترونية. 2';
update priorities set namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها' where namearabic = N'االتصاالت التسويقية المتكاملة وتطبيقاتها . 8';
update priorities set namearabic = N'الأجيال الحديثة من الاتصالات المتنقلة - الالياف البصرية (اتصالات النطاق العريض )' where namearabic = N'اتصالات النطاق العريض ) الالياف البصرية –الأجيالالحديثة من الاتصالات المتنقلة ( . 2';
update priorities set namearabic = N'استخدام تكنلوجيا النانو في مجال صناعة المواد المتقدمة ( الأدوية– الكربون النشط – البلمرات)' where namearabic = N'استخدام تكنلوجيا النانو في مجال صناعة المواد المتقدمة ) الأدوية– الكربون النشط – البلمرات ( . 5';
update priorities set namearabic = N'استخدامات النانو في الإلكترونيات الضوئية' where namearabic = N'استخدامات النانو في الإلكترونيات الضوئية . 5';
update priorities set namearabic = N'استراتيجيات تنمية رأس المال الفكري في المنظمات السعودية' where namearabic = N'استراتيجيات تنمية رأس المال الفكري في المنظمات السعودية . 2';
update priorities set namearabic = N'استشراف مستقبل التعليم العالي' where namearabic = N'استشراف مستقبل التعليم العالي';
update priorities set namearabic = N'اكتساب اللغة الأم واللغات الثانية' where namearabic = N'اكتساب اللغة الأم واللغات الثانية . 2';
update priorities set namearabic = N'الأبعاد االجتماعية والاقتصادية للصنف الرمزي في المجتمع السعودي' where namearabic = N'الأبعاد االجتماعية والاقتصادية للصنف الرمزي في المجتمع السعودي . 5';
update priorities set namearabic = N'الإدارةالمتكاملة للمصادر المائية' where namearabic = N'الإدارةالمتكاملة للمصادر المائية . 9';
update priorities set namearabic = N'الأدب العربي في عصر العولمة' where namearabic = N'الأدب العربي في عصر العولمة';
update priorities set namearabic = N'الأرشفة الإلكترونية' where namearabic = N'الأرشفة الإلكترونية. 3';
update priorities set namearabic = N'الأسواق المالية' where namearabic = N'الأسواق المالية . 4';
update priorities set namearabic = N'الأسوق المالية' where namearabic = N'الأسوق المالية . 6';
update priorities set namearabic = N'الأغشية الرقيقة' where namearabic = N'الأغشية الرقيقة . 9';
update priorities set namearabic = N'الاتجاهات الحديثة في التسويق بالمنظمات السعودية : بين النظرية والتطبيق' where namearabic = N'الاتجاهات الحديثة في التسويق بالمنظمات السعودية : بين النظرية والتطبيق . 4';
update priorities set namearabic = N'الاعتماد الأكاديمي لمؤسسات التعليم العالي' where namearabic = N'الاعتماد الأكاديمي لمؤسسات التعليم العالي';
update priorities set namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات' where namearabic = N'الاكتئاب القوي ، والإدمان على المخدرات : أ. الأسس الجزيئية وراء الاكتئاب القوي ، والإدمان على المخدرات . ب. العالج الدوائي الجيني للإكتئاب القوي ، والإدمان على المخدرات . 6';
update priorities set namearabic = N'الاهتمام بالدراسات التأصيلية للفكر الإسلامي وبيان موقف القرآن من الأفكار الأخرى' where namearabic = N'الاهتمام بالدراسات التأصيلية للفكر الإسلامي وبيان موقف القرآن من الأفكار الأخرى';
update priorities set namearabic = N'البحوث الثقافية التي تتناول أخلاقيات المهن المتعددة' where namearabic = N'البحوث الثقافية التي تتناول أخلاقيات المهن المتعددة';
update priorities set namearabic = N'البحوث الثقافية التي تتناول الدراسات المستقبلية في المجال الثقافي' where namearabic = N'البحوث الثقافية التي تتناول الدراسات المستقبلية في المجال الثقافي . 7';
update priorities set namearabic = N'البحوث الثقافية التي تتناول المذهبيات الثقافية المعاصرة (اللبرالية، الاستشراق ، التنصير، العلمانية)' where namearabic = N'البحوث الثقافية التي تتناول المذهبيات الثقافية المعاصرة )اللبرالية، الاستشراق ، التنصير، العلمانية، ..( . 8';
update priorities set namearabic = N'البحوث الثقافية التي تتناول ثقافات الشعوب المعاصرة' where namearabic = N'البحوث الثقافية التي تتناول ثقافات الشعوب المعاصرة . 9';
update priorities set namearabic = N'البحوث الثقافية التي تتناول قضايا الإرهاب والعنف' where namearabic = N'البحوث الثقافية التي تتناول قضايا الإرهاب والعنف . 3';
update priorities set namearabic = N'البحوث الثقافية التي تتناول قضايا الحوار والتعايش العلاقة مع الآخر' where namearabic = N'البحوث الثقافية التي تتناول قضايا الحوار والتعايش العلاقة مع الآخر . 2';
update priorities set namearabic = N'البحوث الثقافية التي تتناول قضايا العنصرية' where namearabic = N'البحوث الثقافية التي تتناول قضايا العنصرية . 6';
update priorities set namearabic = N'البحوث الثقافية التي تتناول قضايا العولمة والهوية والإنتماء والمواطنة' where namearabic = N'البحوث الثقافية التي تتناول قضايا العولمة والهوية والإنتماء والمواطنة . 4';
update priorities set namearabic = N'البحوث الثقافية التي تتناول قضايا المرأة والطفولة والشباب' where namearabic = N'البحوث الثقافية التي تتناول قضايا المرأة والطفولة والشباب . 5';
update priorities set namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في أبواب العبادات' where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في أبواب العبادات. 2';
update priorities set namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في األنكحة وفقه الأسرة' where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في األنكحة وفقه الأسرة. 4';
update priorities set namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في الأطعمة والزينة' where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في الأطعمة والزينة. 7';
update priorities set namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في العقوبات والتأديب' where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في العقوبات والتأديب. 6';
update priorities set namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا الطبية والبيئية' where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا الطبية والبيئية. 5';
update priorities set namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا ووسائل الإثبات ودراسة األنظمة' where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في القضايا ووسائل الإثبات ودراسة األنظمة. 8';
update priorities set namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في المعاملات المالية' where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في المعاملات المالية. 3';
update priorities set namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في تحقيق المخطوطات التي تعنى بالمذهب والدليل' where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في تحقيق المخطوطات التي تعنى بالمذهب والدليل';
update priorities set namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في فقه الأقليات المسلمة' where namearabic = N'البحوث الفقهية التي تتناول المسائل المعاصرة في فقه الأقليات المسلمة. 9';
update priorities set namearabic = N'البحوث المتعلقة بالتعليم المحاسبي والتأهيل المهني وما يتصل بها من مواضيع تتعلق ببناء المناهج والبرامج ومخرجاتها' where namearabic = N'البحوث المتعلقة بالتعليم المحاسبي والتأهيل المهني وما يتصل بها من مواضيع تتعلق ببناء المناهج والبرامج ومخرجاتها';
update priorities set namearabic = N'البحوث المتعلقة بالمحاسبة الدولية وما يتصل بها من مواضيع تتعلق بالشركات ذات الاستثمارات الدولية، والتوافق مع المعايير الدولية وغيرها من المواضيع ذات البعد الدولي في المحاسبة' where namearabic = N'البحوث المتعلقة بالمحاسبة الدولية وما يتصل بها من مواضيع تتعلق بالشركات ذات الاستثمارات الدولية، والتوافق مع المعايير الدولية وغيرها من المواضيع ذات البعد الدولي في المحاسبة . 3';
update priorities set namearabic = N'البحوث المتعلقة بالمحاسبة عن الزكاة والضريبة' where namearabic = N'البحوث المتعلقة بالمحاسبة عن الزكاة والضريبة . 10';
update priorities set namearabic = N'البحوث المتعلقة بحوكمة الشركات، وما يتصل بها من مواضيع تتعلق بآليات الحوكمة ، وآثارها على المعلومات المحاسبية' where namearabic = N'البحوث المتعلقة بحوكمة الشركات، وما يتصل بها من مواضيع تتعلق بآليات الحوكمة ، وآثارها على المعلومات المحاسبية . 11';
update priorities set namearabic = N'البحوث المشتركة بين علوم اللغة العربية والعلوم الأخرى' where namearabic = N'البحوث المشتركة بين علوم اللغة العربية والعلوم الأخرى . 6';
update priorities set namearabic = N'البحوث والدراسات الأدبية المقارنة' where namearabic = N'البحوث والدراسات الأدبية المقارنة. 3';
update priorities set namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي' where namearabic = N'البدائل الحديثة لتمويل العمل اإلعالمي . 9';
update priorities set namearabic = N'البدائل الموضوعية لحماية العلامة التجارية غير المسجلة بموجب أحكام نظام العلامات التجارية السعودي – دراسة مقارنة' where namearabic = N'البدائل الموضوعية لحماية العلامة التجارية غير المسجلة بموجب أحكام نظام العلامات التجارية السعودي – دراسة مقارنة';
update priorities set namearabic = N'التأمين وإدارة المخاطر' where namearabic = N'التأمين وإدارة المخاطر';
update priorities set namearabic = N'التحليل الدالي وتطبيقاته' where namearabic = N'التحليل الدالي وتطبيقاته';
update priorities set namearabic = N'التحليل العددي والرياضيات الحاسوبية' where namearabic = N'التحليل العددي والرياضيات الحاسوبية . 5';
update priorities set namearabic = N'التحليل اللغوي التقابلي بين اللغة العربية واللغة الإنجليزية' where namearabic = N'التحليل اللغوي التقابلي بين اللغة العربية واللغة الإنجليزية. 10';
update priorities set namearabic = N'التخطيط لزيادة فاعلية برامج الرعاية الاجتماعية' where namearabic = N'التخطيط لزيادة فاعلية برامج الرعاية الاجتماعية . 3';
update priorities set namearabic = N'التدخالت المهنية في مجال العمل مع األطفال المعاقين (ذوي الاحتياجات الخاصة)' where namearabic = N'التدخالت المهنية في مجال العمل مع األطفال المعاقين ) ذوي الاحتياجات الخاصة (. 10';
update priorities set namearabic = N'الترجمة الآلية : نظرياتها وتطبيقاتها' where namearabic = N'الترجمة الآلية : نظرياتها وتطبيقاتها';
update priorities set namearabic = N'التعامل الأسري مع المريض النفسي' where namearabic = N'التعامل الأسري مع المريض النفسي . 9';
update priorities set namearabic = N'التعددية اللغوية وتأثيرها على كل من الفرد والمجتمع' where namearabic = N'التعددية اللغوية وتأثيرها على كل من الفرد والمجتمع . 9';
update priorities set namearabic = N'التعليم الطبي' where namearabic = N'التعليم الطبي';
update priorities set namearabic = N'التفاعل بين الأدب العربي والثقافة الرقمية' where namearabic = N'التفاعل بين الأدب العربي والثقافة الرقمية. 8';
update priorities set namearabic = N'التلوث البيئي ومعالجته' where namearabic = N'التلوث البيئي ومعالجته. 10';
update priorities set namearabic = N'التمويل الإسلامي' where namearabic = N'التمويل الإسلامي. 2';
update priorities set namearabic = N'التوفيق بين نظام العالمات التجارية السعودي والنظام المشترك لدول مجلس التعاون الخليجي بشأن العلامة التجارية المشهورة' where namearabic = N'التوفيق بين نظام العالمات التجارية السعودي والنظام المشترك لدول مجلس التعاون الخليجي بشأن العلامة التجارية المشهورة';
update priorities set namearabic = N'الجبر ونظرية الأعداد وتطبيقاتهما' where namearabic = N'الجبر ونظرية الأعداد وتطبيقاتهما. 2';
update priorities set namearabic = N'الجودة وتطبيقاتها بالتعليم العالمي' where namearabic = N'الجودة وتطبيقاتها بالتعليم العالمي';
update priorities set namearabic = N'الحماية الشرعية والنظامية للمنتجات الصوتية' where namearabic = N'الحماية الشرعية والنظامية للمنتجات الصوتية . 4';
update priorities set namearabic = N'الحماية النظامية للبث عبر التوابع الصناعية من أعلى البحار – دراسة مقارنة' where namearabic = N'الحماية النظامية للبث عبر التوابع الصناعية من أعلى البحار – دراسة مقارنة';
update priorities set namearabic = N'الحوسبة عالية الأداء' where namearabic = N'الحوسبة عالية الأداء. 7';
update priorities set namearabic = N'الحوسبة في خدمة المجالات الشرعية واللغة العربية' where namearabic = N'الحوسبة في خدمة المجالات الشرعية واللغة العربية . 2';
update priorities set namearabic = N'الحوكمة في المنظمات السعودية' where namearabic = N'الحوكمة في المنظمات السعودية';
update priorities set namearabic = N'الخدمة االجتماعية الوقائية وارتباطها بالوعي التنموي' where namearabic = N'الخدمة االجتماعية الوقائية وارتباطها بالوعي التنموي . 9';
update priorities set namearabic = N'الدراسات البيئية والتي تركز على دراسة المشاكل التي تتعرض لها بيئة المملكة ودراسة المخاطر التي تنتج عن الكوارث البيئية' where namearabic = N'الدراسات البيئية والتي تركز على دراسة المشاكل التي تتعرض لها بيئة المملكة ودراسة المخاطر التي تنتج عن الكوارث البيئية . 3';
update priorities set namearabic = N'الدراسات التأصيلية لفقه النوازل' where namearabic = N'الدراسات التأصيلية لفقه النوازل . 8';
update priorities set namearabic = N'الدراسات التي تتناول إبراز أثر أصول الفقه في ضبط المصطلحات وتفسير النصوص النظامية' where namearabic = N'الدراسات التي تتناول إبراز أثر أصول الفقه في ضبط المصطلحات وتفسير النصوص النظامية . 10';
update priorities set namearabic = N'الدراسات التي تتناول أصول العلماء' where namearabic = N'الدراسات التي تتناول أصول العلماء . 5';
update priorities set namearabic = N'الدراسات التي تتناول الإرهاب من منظور إسلامي' where namearabic = N'الدراسات التي تتناول الإرهاب من منظور إسلامي. 11';
update priorities set namearabic = N'الدراسات التي تتناول الاجتهاد والتقليد والتعارض والترجيح' where namearabic = N'الدراسات التي تتناول الاجتهاد والتقليد والتعارض والترجيح . 3';
update priorities set namearabic = N'الدراسات التي تتناول الحكم الشرعي وأدلة الأحكام ودلالات الألفاظ' where namearabic = N'الدراسات التي تتناول الحكم الشرعي وأدلة الأحكام ودلالات الألفاظ . 2';
update priorities set namearabic = N'الدراسات التي تتناول الفكر الأصولي المعاصر' where namearabic = N'الدراسات التي تتناول الفكر الأصولي المعاصر';
update priorities set namearabic = N'الدراسات التي تتناول القواعد الفقهية وجمعها والتطبيق عليها' where namearabic = N'الدراسات التي تتناول القواعد الفقهية وجمعها والتطبيق عليها . 7';
update priorities set namearabic = N'الدراسات التي تتناول تحقيق كتب التراث في أصول الفقه والقواعد الفقهية' where namearabic = N'الدراسات التي تتناول تحقيق كتب التراث في أصول الفقه والقواعد الفقهية . 9';
update priorities set namearabic = N'الدراسات التي تتناول تخريج الفروع على الأصول وغيره من أنواع التخريج' where namearabic = N'الدراسات التي تتناول تخريج الفروع على الأصول وغيره من أنواع التخريج . 6';
update priorities set namearabic = N'الدراسات التي تتناول جميع المسائل الأصولوية من غير مظانها ودراستها' where namearabic = N'الدراسات التي تتناول جميع المسائل الأصولوية من غير مظانها ودراستها . 4';
update priorities set namearabic = N'الدراسات التي تتناول عالقة الأدب بالتحولات االجتماعية' where namearabic = N'الدراسات التي تتناول عالقة الأدب بالتحولات االجتماعية 6';
update priorities set namearabic = N'الدراسات التي تتناول مقاصد الشريعة' where namearabic = N'الدراسات التي تتناول مقاصد الشريعة . 8';
update priorities set namearabic = N'الدراسات التي تركز على التوزيع الجغرافي للخدمات المختلفة ومدى كفاءة التوزيع ودراسة العناصر المؤثرة في توزيع الخدمات (حكومية وخاصة )' where namearabic = N'الدراسات التي تركز على التوزيع الجغرافي للخدمات المختلفة )حكومية وخاصة ( ومدى كفاءة التوزيع ودراسة العناصر المؤثرة في توزيع الخدمات . 8';
update priorities set namearabic = N'الدراسات التي تناولت الشائعات وسمات مصدقيها' where namearabic = N'الدراسات التي تناولت الشائعات وسمات مصدقيها . 12';
update priorities set namearabic = N'الدراسات التي تناولت المخدرات وآثارها النفسية على الفرد والأسرة' where namearabic = N'الدراسات التي تناولت المخدرات وآثارها النفسية على الفرد والأسرة';
update priorities set namearabic = N'الدراسات الجغرافية التي تعنى بتطبيق التقنيات الحديثة نظم المعلومات في الجغرافية والاستشعار عن بعد في مسار الجغرافية البشرية والطبيعية بكافة فروعهما' where namearabic = N'الدراسات الجغرافية التي تعنى بتطبيق التقنيات الحديثة نظم المعلومات في الجغرافية والاستشعار عن بعد في مسار الجغرافية البشرية والطبيعية بكافة فروعهما . 4';
update priorities set namearabic = N'الدراسات الجغرافية عن العالم الإسلامي والوطن العربي' where namearabic = N'الدراسات الجغرافية عن العالم الإسلامي والوطن العربي';
update priorities set namearabic = N'الدراسات الخرائطية التي تعنى بدراسة الأساليب الخرائطية التقليدية والرقمية وتقييمها' where namearabic = N'الدراسات الخرائطية التي تعنى بدراسة الأساليب الخرائطية التقليدية والرقمية وتقييمها . 9';
update priorities set namearabic = N'الدراسات السكانية التي تركز على دراسة سكان المملكة وتوزيعهم الجغرافي وما يتعلق بذلك من قضايا' where namearabic = N'الدراسات السكانية التي تركز على دراسة سكان المملكة وتوزيعهم الجغرافي وما يتعلق بذلك من قضايا. 6';
update priorities set namearabic = N'الدراسات العمرانية التي تركز على التخطيط والتنمية العمرانية والتغير العمراني للتجمعات الحضرية في المملكة' where namearabic = N'الدراسات العمرانية التي تركز على التخطيط والتنمية العمرانية والتغير العمراني للتجمعات الحضرية في المملكة . 7';
update priorities set namearabic = N'الدراسات الفقهية المقارنة بالأنظمة' where namearabic = N'الدراسات الفقهية المقارنة بالأنظمة . 10';
update priorities set namearabic = N'الدراسات القضائية وتطبيقاتها' where namearabic = N'الدراسات القضائية وتطبيقاتها';
update priorities set namearabic = N'الدراسات المتعلقة بالرؤى الإصلاحية والإنسانية عند قادة الدولة السعودية وملوكها' where namearabic = N'الدراسات المتعلقة بالرؤى الإصلاحية والإنسانية عند قادة الدولة السعودية وملوكها';
update priorities set namearabic = N'الدراسات الموجهة نحو مظاهر الإبداع الحضاري وتجليات القيم الخيرية الإنسانية' where namearabic = N'الدراسات الموجهة نحو مظاهر الإبداع الحضاري وتجليات القيم الخيرية الإنسانية . 9';
update priorities set namearabic = N'الدراسات الموجهة نحو معرفة أثر الفتن الأزمات الأمنية على تاريخ المسلمين ومسيرتهم الحضارية.' where namearabic = N'الدراسات الموجهة نحو معرفة أثر الفتن الأزمات الأمنية على تاريخ المسلمين ومسيرتهم الحضارية. 10';
update priorities set namearabic = N'الدراسات في الجغرافية السلوكية والتي تركز على التوزيع الجغرافي للظواهر السلوكية في المجتمع في المملكة العربية السعودية' where namearabic = N'الدراسات في الجغرافية السلوكية والتي تركز على التوزيع الجغرافي للظواهر السلوكية في المجتمع في المملكة العربية السعودية . 10';
update priorities set namearabic = N'الدراسات في تاريخ الحرمين الشريفين والحج عبر العصور الإسلامية' where namearabic = N'الدراسات في تاريخ الحرمين الشريفين والحج عبر العصور الإسلامية . 4';
update priorities set namearabic = N'الدراسات في تاريخ المملكة العربية السعودية والدولتين السعوديتين الأولى والثانية' where namearabic = N'الدراسات في تاريخ المملكة العربية السعودية والدولتين السعوديتين الأولى والثانية. 3';
update priorities set namearabic = N'الدراسات في مجال الجغرافيا الطبيعية للمملكة سواء في الجغرافيا المناخية أو النباتية أو الجغرافيا الحيوية' where namearabic = N'الدراسات في مجال الجغرافيا الطبيعية للمملكة سواء في الجغرافيا المناخية أو النباتية أو الجغرافيا الحيوية. 2';
update priorities set namearabic = N'الدراسات في مجال الجغرافية الاقتصادية وتشمل التعدين والطاقة والأنشطة البشرية المختلفة في المملكة' where namearabic = N'الدراسات في مجال الجغرافية الاقتصادية وتشمل التعدين والطاقة والأنشطة البشرية المختلفة في المملكة . 5';
update priorities set namearabic = N'الدراسات في مجال الفكر التاريخي وتطوير مناهج البحث في التاريخ' where namearabic = N'الدراسات في مجال الفكر التاريخي وتطوير مناهج البحث في التاريخ . 6';
update priorities set namearabic = N'الدراسات في مجال المصادر التاريخية القديمة والحديثة' where namearabic = N'الدراسات في مجال المصادر التاريخية القديمة والحديثة . 5';
update priorities set namearabic = N'الدراسات والبحوث التي تتعلق بالحوار بين الأديان والحضارات' where namearabic = N'الدراسات والبحوث التي تتعلق بالحوار بين الأديان والحضارات . 6';
update priorities set namearabic = N'الدراسات والبحوث التي تتعلق بالفرق واألحزاب المعاصرة' where namearabic = N'الدراسات والبحوث التي تتعلق بالفرق واألحزاب المعاصرة';
update priorities set namearabic = N'الدراسات والبحوث التي تتعلق بالقضايا والمسائل العقدية المعاصرة' where namearabic = N'الدراسات والبحوث التي تتعلق بالقضايا والمسائل العقدية المعاصرة . 3';
update priorities set namearabic = N'الدراسات والبحوث التي تتعلق بالمستجدات في المذاهب المعاصرة' where namearabic = N'الدراسات والبحوث التي تتعلق بالمستجدات في المذاهب المعاصرة . 4';
update priorities set namearabic = N'الدراسات والبحوث التي تتعلق بتأصيل العقيدة السلفية وفق منهج أهل السنة' where namearabic = N'الدراسات والبحوث التي تتعلق بتأصيل العقيدة السلفية وفق منهج أهل السنة . 2';
update priorities set namearabic = N'الدراسات والبحوث التي تتناول الفتن وأثرها على المجتمع' where namearabic = N'الدراسات والبحوث التي تتناول الفتن وأثرها على المجتمع . 5';
update priorities set namearabic = N'الدراسات والبحوث المتعلقة بتحقيق المخطوطات الفقهية' where namearabic = N'الدراسات والبحوث المتعلقة بتحقيق المخطوطات الفقهية . 9';
update priorities set namearabic = N'الدراسات والبحوث المتعلقة بفقه الأسرة' where namearabic = N'الدراسات والبحوث المتعلقة بفقه الأسرة . 3';
update priorities set namearabic = N'الدراسات والبحوث المتعلقة بفقه التعزيرات' where namearabic = N'الدراسات والبحوث المتعلقة بفقه التعزيرات . 6';
update priorities set namearabic = N'الدراسات والبحوث المتعلقة بفقه الجنايات' where namearabic = N'الدراسات والبحوث المتعلقة بفقه الجنايات . 4';
update priorities set namearabic = N'الدراسات والبحوث المتعلقة بفقه الحدود' where namearabic = N'الدراسات والبحوث المتعلقة بفقه الحدود . 5';
update priorities set namearabic = N'الدراسات والبحوث المتعلقة بفقه المعاملات المالية' where namearabic = N'الدراسات والبحوث المتعلقة بفقه المعاملات المالية . 2';
update priorities set namearabic = N'الدعم االجتماعي للأسرة عند الأزمات والكوارث' where namearabic = N'الدعم االجتماعي للأسرة عند الأزمات والكوارث . 7';
update priorities set namearabic = N'الذكاء الصناعي و الروبوت' where namearabic = N'الذكاء الصناعي و الروبوت';
update priorities set namearabic = N'الرياضيات المتقطعة وتطبيقاتها' where namearabic = N'الرياضيات المتقطعة وتطبيقاتها. 6';
update priorities set namearabic = N'السرد الشارح وإعادة كتابة التاريخ' where namearabic = N'السرد الشارح وإعادة كتابة التاريخ . 4';
update priorities set namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية' where namearabic = N'السرطان : البحث في مجال الثدي ، القولون والمستقيم ، والبروستات: أ. الأسس الجزيئية وراء الإصابة بداء السرطان وانتشاره . ب. التحليل الكلي والجزئي لعوامل الخطر الجينية والبيئية . 2';
update priorities set namearabic = N'السياسات النقدية والمالية' where namearabic = N'السياسات النقدية والمالية . 5';
update priorities set namearabic = N'الشبكات والشبكات المتنقلة' where namearabic = N'الشبكات والشبكات المتنقلة . 6';
update priorities set namearabic = N'الشرق ووجهة نظر المستشرقين كما تصوره األعمال الأدبية' where namearabic = N'الشرق ووجهة نظر المستشرقين كما تصوره األعمال الأدبية . 3';
update priorities set namearabic = N'الطاقة العالية والنووية' where namearabic = N'الطاقة العالية والنووية . 4';
update priorities set namearabic = N'الطاقة المتجددة' where namearabic = N'الطاقة المتجددة . 3';
update priorities set namearabic = N'الطاقة المتجددة وتخزين الطاقة' where namearabic = N'الطاقة المتجددة وتخزين الطاقة . 3';
update priorities set namearabic = N'الطاقة النظيفة' where namearabic = N'الطاقة النظيفة . 4';
update priorities set namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة' where namearabic = N'الطب الإسلامي والنبوي : أ. بحث آليات عالج الأمراض استنادًا إلى القران الكريم . ب. بحث آليات عالج الأمراض استنادًا إلى السنة النبوية الشريفة . 5';
update priorities set namearabic = N'العدالة االجتماعية والأمن الفكري لدى الشباب' where namearabic = N'العدالة االجتماعية والأمن الفكري لدى الشباب . 2';
update priorities set namearabic = N'العناية بالموسوعات القرآنية' where namearabic = N'العناية بالموسوعات القرآنية';
update priorities set namearabic = N'العنف الممارس من قبل الشباب' where namearabic = N'العنف الممارس من قبل الشباب';
update priorities set namearabic = N'الفصل بالأغشية' where namearabic = N'الفصل بالأغشية. 4';
update priorities set namearabic = N'الفيزياء البيئية والطبية' where namearabic = N'الفيزياء البيئية والطبية . 5';
update priorities set namearabic = N'الفيزياء التربوية التعليمية' where namearabic = N'الفيزياء التربوية التعليمية . 8';
update priorities set namearabic = N'الفيزياء النظرية' where namearabic = N'الفيزياء النظرية . 6';
update priorities set namearabic = N'القضايا المتعلقة بالاستثمار والتمويل' where namearabic = N'القضايا المتعلقة بالاستثمار والتمويل . 2';
update priorities set namearabic = N'الكراسي البحثية' where namearabic = N'الكراسي البحثية';
update priorities set namearabic = N'الكيمياء البيئية(تلوث المياه – تلوث الهواء – معالجة النفايات)' where namearabic = N'الكيمياء البيئية ) تلوث المياه – تلوث الهواء – معالجة النفايات (. 2';
update priorities set namearabic = N'الكيمياء العضوية الصناعية في مجال البلمرات واللدائند – الكيمياء الدوائية الأسمدة العضوية ومعالجة النفايات' where namearabic = N'الكيمياء العضوية الصناعية في مجال البلمرات واللدائند – الكيمياء الدوائية الأسمدة العضوية ومعالجة النفايات . 3';
update priorities set namearabic = N'الكيمياء غير العضوية التطبيقية في مجال صناعة الأسمدة المركبة الجديدة والمتراكبة وغيرها' where namearabic = N'الكيمياء غير العضوية التطبيقية في مجال صناعة الأسمدة المركبة الجديدة والمتراكبة وغيرها . 4';
update priorities set namearabic = N'المتغيرات المستجدة في العملية الأتصالية' where namearabic = N'المتغيرات المستجدة في العملية االتصالية . 5';
update priorities set namearabic = N'المحتوى الرقمي' where namearabic = N'المحتوى الرقمي . 4';
update priorities set namearabic = N'المراجعات العلمية للنظريات العالمية' where namearabic = N'المراجعات العلمية للنظريات اإلعالمية . 2';
update priorities set namearabic = N'المسؤولية الاجتماعية' where namearabic = N'المسؤولية الاجتماعية . 6';
update priorities set namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود' where namearabic = N'المسؤولية القانونية لممارسات اإلعالم العابر للحدود . 13';
update priorities set namearabic = N'المشروعات الصغيرة والمتوسطة بالمملكة العربية السعودية :الواقع والتحديات والطموحات' where namearabic = N'المشروعات الصغيرة والمتوسطة بالمملكة العربية السعودية :الواقع والتحديات والطموحات . 9';
update priorities set namearabic = N'المشكالت النفسية للأطفال والشباب والراشدين والمسنين' where namearabic = N'المشكالت النفسية للأطفال والشباب والراشدين والمسنين . 3';
update priorities set namearabic = N'المصطلحات النقدية الحديثة' where namearabic = N'المصطلحات النقدية الحديثة. 9';
update priorities set namearabic = N'المعاجم المتخصصة في المصطلحات والمفاهيم اللغوية والنقدية والدراسات المتصلة بها' where namearabic = N'المعاجم المتخصصة في المصطلحات والمفاهيم اللغوية والنقدية والدراسات المتصلة بها. 2';
update priorities set namearabic = N'المعادلات التفاضلية وتطبيقاتها' where namearabic = N'المعادلات التفاضلية وتطبيقاتها. 3';
update priorities set namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة' where namearabic = N'المعايير والقيم المهنية للممارسات اإلعالمية الحديثة . 6';
update priorities set namearabic = N'المناهج النقدية الحديثة' where namearabic = N'المناهج النقدية الحديثة. 7';
update priorities set namearabic = N'المنظر الإسلامي للموضوعات النفسية' where namearabic = N'المنظر الإسلامي للموضوعات النفسية . 5';
update priorities set namearabic = N'المنهجيات الحديثة في البحث العلمي' where namearabic = N'المنهجيات الحديثة في البحث العلمي . 3';
update priorities set namearabic = N'المواد المبلمرة والمركبة' where namearabic = N'المواد المبلمرة والمركبة . 7';
update priorities set namearabic = N'المواد المتقدمة' where namearabic = N'المواد المتقدمة . 2';
update priorities set namearabic = N'النشر الإلكتروني' where namearabic = N'النشر الإلكتروني. 5';
update priorities set namearabic = N'النظريات البالغية والنقدية في التراث العربي' where namearabic = N'النظريات البالغية والنقدية في التراث العربي. 3';
update priorities set namearabic = N'النظم التفاعلية' where namearabic = N'النظم التفاعلية . 3';
update priorities set namearabic = N'الهوية النفسية وتحدياتها المعاصرة' where namearabic = N'الهوية النفسية وتحدياتها المعاصرة . 4';
update priorities set namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4' where namearabic = N'الوزن الزائد : السمنة ) متالزمة الأيض – داء السكري من النوع 2 ،وداء السكري من النوع 1 .) أ. الاستعداد الوراثي مقابل العوامل البيئية . ب. العالج الدوائي الجيني لمكافحة السمنة وداء السكري . ج. الأسس الجزيئية وراء متالزمة الأيض وداء السكري . 4';
update priorities set namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته' where namearabic = N'انعكاسات البيئة اإلعالمية الحديثة على تشريعات اإلعالم وأخالقياته. 12';
update priorities set namearabic = N'بالغة الخطاب المعاصر' where namearabic = N'بالغة الخطاب المعاصر. 5';
update priorities set namearabic = N'بالغة الفنون األدبية الحديثة' where namearabic = N'بالغة الفنون األدبية الحديثة. 4';
update priorities set namearabic = N'بحوث التحليل المالي، وما يتصل بها من مواضيع تتعلق بالتحليل الأساسي والفني، وبناء المؤشرات المالية، وأثر المعلومات المحاسبية على تعاملات الأسواق المالية، ومشاكل إدارة الأرباح، وغيرها من المواضيع ذات الصلة بالتحليل المالي' where namearabic = N'بحوث التحليل المالي، وما يتصل بها من مواضيع تتعلق بالتحليل الأساسي والفني، وبناء المؤشرات المالية، وأثر المعلومات المحاسبية على تعاملات الأسواق المالية، ومشاكل إدارة الأرباح، وغيرها من المواضيع ذات الصلة بالتحليل المالي . 6';
update priorities set namearabic = N'بحوث المحاسبة الإدارية والتكاليف وما يتصل بها من مواضيع ذات عالقة بتحديد التكاليف المستهدفة وأنظمة التكاليف المختلفة وأساليب قياس الأداءواتخاذ القرارات، ووضع الموازنات وغيرها من مواضيع المحاسبة الإدارية' where namearabic = N'بحوث المحاسبة الإدارية والتكاليف وما يتصل بها من مواضيع ذات عالقة بتحديد التكاليف المستهدفة وأنظمة التكاليف المختلفة وأساليب قياس الأداءواتخاذ القرارات، ووضع الموازنات وغيرها من مواضيع المحاسبة الإدارية . 4';
update priorities set namearabic = N'بحوث المحاسبة المالية، وما يتصل بها من مواضيع تتعلق بالأفصاح المحاسبي وطرق القياس والعرض لبنود الموجودات والالتزامات وحقوق المالك والإيردات، والمصروفات، وبناء المعايير المحاسبية وغيرها من مواضيع المحاسبة المالية' where namearabic = N'بحوث المحاسبة المالية، وما يتصل بها من مواضيع تتعلق بالأفصاح المحاسبي وطرق القياس والعرض لبنود الموجودات والالتزامات وحقوق المالك والإيردات، والمصروفات، وبناء المعايير المحاسبية وغيرها من مواضيع المحاسبة المالية . 2';
update priorities set namearabic = N'بحوث المحاسبة في الجهات غير الربحية سواء كانت حكومية أو جهات أخرى غير هادفة للربح وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح' where namearabic = N'بحوث المحاسبة في الجهات غير الربحية سواء كانت حكومية أو جهات أخرى غير هادفة للربح وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح. 5';
update priorities set namearabic = N'بحوث المحاسبة في المؤسسات المالية الإسلامية ، وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح للأدوات المالية والعمليات المتوافقة مع الشريعة، وبناء المعايير المحاسبية، وغيرها من المواضيع ذات الصلة بالمؤسسات المالية الإسلامية' where namearabic = N'بحوث المحاسبة في المؤسسات المالية الإسلامية ، وما يتصل بها من مواضيع تتعلق بالقياس والعرض والإفصاح للأدوات المالية والعمليات المتوافقة مع الشريعة، وبناء المعايير المحاسبية، وغيرها من المواضيع ذات الصلة بالمؤسسات المالية الإسلامية. 7';
update priorities set namearabic = N'بحوث المراجعة سواء تلك المتعلقة بالمراجعة الداخلية أو المراجعة الخارجية، وما يتصل بها من مواضيع تتعلق بتأهيل المراجع وبناء معايير المراجعة، ولجان المراجعة، وغيرها من المواضيع ذات الصلة بعملية المراجعة' where namearabic = N'بحوث المراجعة سواء تلك المتعلقة بالمراجعة الداخلية أو المراجعة الخارجية، وما يتصل بها من مواضيع تتعلق بتأهيل المراجع وبناء معايير المراجعة، ولجان المراجعة، وغيرها من المواضيع ذات الصلة بعملية المراجعة . 9';
update priorities set namearabic = N'بحوث النظم المحاسبية ، وما يتصل بها من مواضيع تتعلق بتصميم النظم ونقدها، التخصصات الإدارية التخصصات التطبيقية 40 وهياكل الرقابة الداخلية، وغيرها من المواضيع ذات الصلة بالنظم المحاسبية اليدوية والآلية' where namearabic = N'بحوث النظم المحاسبية ، وما يتصل بها من مواضيع تتعلق بتصميم النظم ونقدها، التخصصات الإدارية التخصصات التطبيقية 40 وهياكل الرقابة الداخلية، وغيرها من المواضيع ذات الصلة بالنظم المحاسبية اليدوية والآلية. 8';
update priorities set namearabic = N'برامج التعلم عن بعد والبرامج المدمجة لتدريب الطالب المعلمين بالجامعات السعودية على مهارات التدريس وإدارة الصف' where namearabic = N'برامج التعلم عن بعد والبرامج المدمجة لتدريب الطالب المعلمين بالجامعات السعودية على مهارات التدريس وإدارة الصف. 4';
update priorities set namearabic = N'برامج التعليم الموازي مالها وما عليها' where namearabic = N'برامج التعليم الموازي مالها وما عليها';
update priorities set namearabic = N'برامج تدريب وتطوير أداء معلم اللغة اإلنجليزية ما قبل الخدمة' where namearabic = N'برامج تدريب وتطوير أداء معلم اللغة اإلنجليزية ما قبل الخدمة. 5';
update priorities set namearabic = N'برامج تدريب وتطوير أداء معلم اللغة الإنجليزية أثناء الخدمة على المستويين المدرسي والجامعي' where namearabic = N'برامج تدريب وتطوير أداء معلم اللغة الإنجليزية أثناء الخدمة على المستويين المدرسي والجامعي. 7';
update priorities set namearabic = N'تأثير اكتساب اللغة العربية على اكتساب اللغة الإنجليزية' where namearabic = N'تأثير اكتساب اللغة العربية على اكتساب اللغة الإنجليزية . 8';
update priorities set namearabic = N'تأصيل منهج الأدب الأسلامي' where namearabic = N'تأصيل منهج األدب اإلسالمي. 8';
update priorities set namearabic = N'تأليف مراجع مناسبة للقسم' where namearabic = N'تأليف مراجع مناسبة للقسم. 5';
update priorities set namearabic = N'تاريخ المدن الإسلامية وخططها العمرانية ومرافقها الخداماتية' where namearabic = N'تاريخ المدن الإسلامية وخططها العمرانية ومرافقها الخداماتية . 7';
update priorities set namearabic = N'تجديد دراسة التراث الأدبي في ضوء المناهج الحديثة. 5' where namearabic = N'تجديد دراسة التراث الأدبي في ضوء المناهج الحديثة. 5';
update priorities set namearabic = N'تجميد الأموال' where namearabic = N'تجميد األموال . 2';
update priorities set namearabic = N'تحديات المحتوى في الإعلام الجديد' where namearabic = N'تحديات المحتوى في اإلعالم الجديد . 11';
update priorities set namearabic = N'تحديات تطبيق التعلم الإلكتروني' where namearabic = N'تحديات تطبيق التعلم الإلكتروني . 6';
update priorities set namearabic = N'تحديات تعليم الترجمة الفورية' where namearabic = N'تحديات تعليم الترجمة الفورية. 4';
update priorities set namearabic = N'تحقيق التراث البالغي الأصيل' where namearabic = N'تحقيق التراث البالغي األصيل. 6';
update priorities set namearabic = N'تحقيق النصوص التراثية القيمة والنادرة تحقيقا علميا جيدا' where namearabic = N'تحقيق النصوص التراثية القيمة والنادرة تحقيقا علميا جيدا 7';
update priorities set namearabic = N'تحليل الخطاب اللغوي' where namearabic = N'تحليل الخطاب اللغوي . 3';
update priorities set namearabic = N'تحليه المياه' where namearabic = N'تحليه المياه . 2';
update priorities set namearabic = N'تخطيط وتصميم الطرق في المملكة العربية السعودية' where namearabic = N'تخطيط وتصميم الطرق في المملكة العربية السعودية . 6';
update priorities set namearabic = N'ترشيد استهالك الطاقة' where namearabic = N'ترشيد استهالك الطاقة . 6';
update priorities set namearabic = N'تسعير الخدمات في المصارف التجارية' where namearabic = N'تسعير الخدمات في المصارف التجارية . 9';
update priorities set namearabic = N'تطبيقات أنظمة التحكم والربوت' where namearabic = N'تطبيقات أنظمة التحكم والربوت. 6';
update priorities set namearabic = N'تطبيقات أنظمة القياس واالستشعار الحديثة' where namearabic = N'تطبيقات أنظمة القياس واالستشعار الحديثة';
update priorities set namearabic = N'تطبيقات شبكات الاتصالات والحاسب الآلي' where namearabic = N'تطبيقات شبكات الاتصالات والحاسب الآلي. 4';
update priorities set namearabic = N'تطوير الدرس النحوي والصرفي وفقًا لمنجزات العلوم اللغوية الحديثة' where namearabic = N'تطوير الدرس النحوي والصرفي وفقًا لمنجزات العلوم اللغوية الحديثة . 4';
update priorities set namearabic = N'تطوير السياسة التمويلية لدعم عمليات التمويل في المصارف الإسلامية.' where namearabic = N'تطوير السياسة التمويلية لدعم عمليات التمويل في المصارف الإسلامية. 10';
update priorities set namearabic = N'تطوير المناهج التعليمية وتقويتها' where namearabic = N'تطوير المناهج التعليمية وتقويتها';
update priorities set namearabic = N'تطوير برامج الدراسات العليا في ضوء الاعتماد الأكاديمي' where namearabic = N'تطوير برامج الدراسات العليا في ضوء الاعتماد الأكاديمي';
update priorities set namearabic = N'تطوير قطاع التمويل والاستثمارفي المملكة' where namearabic = N'تطوير قطاع التمويل والاستثمارفي المملكة . 3';
update priorities set namearabic = N'تطوير مواد وأنظمة إنشائية لتقليل الكلفة في المشاريع السكنية' where namearabic = N'تطوير مواد وأنظمة إنشائية لتقليل الكلفة في المشاريع السكنية . 3';
update priorities set namearabic = N'تطوير واستخدام خرسانة ذاتية المعالجة للظروف المناخية المحلية' where namearabic = N'تطوير واستخدام خرسانة ذاتية المعالجة للظروف المناخية المحلية . 5';
update priorities set namearabic = N'تقنيات التواصل االجتماعي الحديث وتوظيفها الإيجابي' where namearabic = N'تقنيات التواصل االجتماعي الحديث وتوظيفها الإيجابي . 8';
update priorities set namearabic = N'تقنيات لتحلية وتنقية المياه' where namearabic = N'تقنيات لتحلية وتنقية المياه . 7';
update priorities set namearabic = N'تقنية النانو' where namearabic = N'تقنية النانو . 2';
update priorities set namearabic = N'تقويم نظم المعلومات' where namearabic = N'تقويم نظم المعلومات';
update priorities set namearabic = N'تقييم ومراقبة سلوك المنشاءات الخراسانية والمعدنية في المملكة' where namearabic = N'تقييم ومراقبة سلوك المنشاءات الخراسانية والمعدنية في المملكة. 4';
update priorities set namearabic = N'تنمية المصحة النفسية لأفراد المجتمع' where namearabic = N'تنمية المصحة النفسية لأفراد المجتمع . 2';
update priorities set namearabic = N'تيسير اللغة العربية' where namearabic = N'تيسير اللغة العربية';
update priorities set namearabic = N'ثقافة التطوع وأثرها على المجتمع' where namearabic = N'ثقافة التطوع وأثرها على المجتمع . 8';
update priorities set namearabic = N'جرائم الأتصال الألكتروني' where namearabic = N'جرائم االتصال اإللكتروني';
update priorities set namearabic = N'حوكمة الشركات' where namearabic = N'حوكمة الشركات . 5';
update priorities set namearabic = N'حوكمة المؤسسات المالية والمصرفية' where namearabic = N'حوكمة المؤسسات المالية والمصرفية';
update priorities set namearabic = N'خدمات المعلومات' where namearabic = N'خدمات المعلومات';
update priorities set namearabic = N'دراسات تطبيقية على الاقتصاد السعودي' where namearabic = N'دراسات تطبيقية على الاقتصاد السعودي . 3';
update priorities set namearabic = N'دراسات في المشكلات البيئية في المجتمع السعودي من منظور اجتماعي' where namearabic = N'دراسات في المشكلات البيئية في المجتمع السعودي من منظور اجتماعي . 4';
update priorities set namearabic = N'دراسات لغوية حاسوبية' where namearabic = N'دراسات لغوية حاسوبية . 6';
update priorities set namearabic = N'دراسات مقارنة الأدب الإنجليزي الحديث الأدب السعودي' where namearabic = N'دراسات مقارنة الأدب االنجليزي الحديث الأدب السعودي . 2';
update priorities set namearabic = N'دراسات نقدية لإلبداع في الأعمال الحائزة على جائزة نوبل للسلام' where namearabic = N'دراسات نقدية لإلبداع في الأعمال الحائزة على جائزة نوبل للسلام';
update priorities set namearabic = N'دراسة السيرة النبوية الراشدة خاصة الدراسات الموجهة نحو الموضوعات الحضارية المستوحاة من هذين العصرين' where namearabic = N'دراسة السيرة النبوية الراشدة خاصة الدراسات الموجهة نحو الموضوعات الحضارية المستوحاة من هذين العصرين . 2';
update priorities set namearabic = N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية' where namearabic = N'دراسة العربية المعاصرة :دراسة دلالية وتركيبية . 4';
update priorities set namearabic = N'دراسة الواقع اللغوي في المجتمع السعودي' where namearabic = N'دراسة الواقع اللغوي في المجتمع السعودي. 3';
update priorities set namearabic = N'دراسة تحليلية لبعض االتجاهات السلوكية للعاملين بالمنظمات السعودية' where namearabic = N'دراسة تحليلية لبعض االتجاهات السلوكية للعاملين بالمنظمات السعودية . 7';
update priorities set namearabic = N'دراسة في الصناعة المعجمية المعاصرة' where namearabic = N'دراسة في الصناعة المعجمية المعاصرة . 5';
update priorities set namearabic = N'دراسة مسببات تدهور طبقات رصف الطرق السريعة في المملكة العربية السعودية والطرق المثلى لمعالجتها' where namearabic = N'دراسة مسببات تدهور طبقات رصف الطرق السريعة في المملكة العربية السعودية والطرق المثلى لمعالجتها . 7';
update priorities set namearabic = N'دراسة مقارنة' where namearabic = N'دراسة مقارنة';
update priorities set namearabic = N'درجة أمان البنوك التجارية' where namearabic = N'درجة أمان البنوك التجارية . 5';
update priorities set namearabic = N'دفع الشبهات عن القرآن الكريم' where namearabic = N'دفع الشبهات عن القرآن الكريم';
update priorities set namearabic = N'دور استراتيجيات التعلم الذاتي والتعلم التعاوني في تنمية المهارات اللغوية المختلفة للطالب' where namearabic = N'دور استراتيجيات التعلم الذاتي والتعلم التعاوني في تنمية المهارات اللغوية المختلفة للطالب . 3';
update priorities set namearabic = N'دور البنوك الربوية / التقليدية والبنوك الإسلامية في دفع عجلة التنمية الاقتصادية' where namearabic = N'دور البنوك الربوية / التقليدية والبنوك الإسلامية في دفع عجلة التنمية الاقتصادية. 8';
update priorities set namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور' where namearabic = N'دور الشبكات االجتماعية في التأثير على الجمهور . 7';
update priorities set namearabic = N'رأس المال المعرفي' where namearabic = N'رأس المال المعرفي . 3';
update priorities set namearabic = N'ربحية المصارف التجارية' where namearabic = N'ربحية المصارف التجارية . 7';
update priorities set namearabic = N'سياسات التنمية البشرية في المجتمع السعودي (سياسات التعليم ،الصحة ، السكان)' where namearabic = N'سياسات التنمية البشرية في المجتمع السعودي ) سياسات التعليم ،الصحة ، السكان ،..(. 6';
update priorities set namearabic = N'طرق تدريس الترجمة' where namearabic = N'طرق تدريس الترجمة . 3';
update priorities set namearabic = N'ظاهرة التحول من البنوك التقليدية إلى الصيرفة الإسلامية' where namearabic = N'ظاهرة التحول من البنوك التقليدية إلى الصيرفة الإسلامية. 2';
update priorities set namearabic = N'علم الحراريات وتحويل الطاقة' where namearabic = N'علم الحراريات وتحويل الطاقة . 5';
update priorities set namearabic = N'فصل وتخزين ثاني أكسيد الكربون' where namearabic = N'فصل وتخزين ثاني أكسيد الكربون . 6';
update priorities set namearabic = N'فعالية نظرية الذكاء المتعددة في تدريس المهارات اللغوية المختلفة وتعزيز دافعية و اتجاهات الطالب نحو تعلم اللغة' where namearabic = N'فعالية نظرية الذكاء المتعددة في تدريس المهارات اللغوية المختلفة وتعزيز دافعية و اتجاهات الطالب نحو تعلم اللغة . 2';
update priorities set namearabic = N'فيزياء البلازما' where namearabic = N'فيزياء البلازما';
update priorities set namearabic = N'قضايا الاقتصاد الإسلامي' where namearabic = N'قضايا الاقتصاد اإلسالمي . 4';
update priorities set namearabic = N'قضايا البطالة والعمل' where namearabic = N'قضايا البطالة والعمل';
update priorities set namearabic = N'قواعد البيانات وتعدين البيانات' where namearabic = N'قواعد البيانات وتعدين البيانات . 4';
update priorities set namearabic = N'كتب المقررات الدراسية والدراسات المترجمة' where namearabic = N'كتب المقررات الدراسية والدراسات المترجمة';
update priorities set namearabic = N'كفاية رأس مال البنوك المتخصصة والبنوك الشاملة' where namearabic = N'كفاية رأس مال البنوك المتخصصة والبنوك الشاملة . 4';
update priorities set namearabic = N'مخاطر السيول في المناطق الحضرية والريفية في المملكة' where namearabic = N'مخاطر السيول في المناطق الحضرية والريفية في المملكة . 11';
update priorities set namearabic = N'مراجعة الأعمال الأستشراقية عن التاريخ الإسلامي' where namearabic = N'مراجعة األعمال الأستشراقية عن التاريخ الإسلامي . 8';
update priorities set namearabic = N'مسؤولية المرشد السياحي' where namearabic = N'مسؤولية المرشد السياحي . 5';
update priorities set namearabic = N'معالجة الإشارات والصور الرقمية وتطبيقاتها' where namearabic = N'معالجة الإشارات والصور الرقمية وتطبيقاتها . 3';
update priorities set namearabic = N'معالجة المياه العادمة وإعادة استخدامها .' where namearabic = N'معالجة المياه العادمة وإعادة استخدامها . 10';
update priorities set namearabic = N'معالجة المياه والمياه العادمة' where namearabic = N'معالجة المياه والمياه العادمة . 9';
update priorities set namearabic = N'معالجة النفايات الكيماوية' where namearabic = N'معالجة النفايات الكيماوية . 11';
update priorities set namearabic = N'معالجة قضايا اللغة في المجتمع' where namearabic = N'معالجة قضايا اللغة في المجتمع';
update priorities set namearabic = N'مقاومة الأفكار المنحرفة كالإرهاب والتكفير' where namearabic = N'مقاومة الأفكار المنحرفة كالإرهاب والتكفير';
update priorities set namearabic = N'مكيفات التبريد بالتبخير' where namearabic = N'مكيفات التبريد بالتبخير . 5';
update priorities set namearabic = N'مناهج البحث البالغي' where namearabic = N'مناهج البحث البالغي. 2';
update priorities set namearabic = N'منظمات التعليم وكيفية جعل الجامعات منظمات تعليم' where namearabic = N'منظمات التعليم وكيفية جعل الجامعات منظمات تعليم';
update priorities set namearabic = N'مهارات الحياة الزوجية' where namearabic = N'مهارات الحياة الزوجية . 7';
update priorities set namearabic = N'مهارات الذات والتواصل الاجتماعي' where namearabic = N'مهارات الذات والتواصل الاجتماعي . 10';
update priorities set namearabic = N'مواد النانو' where namearabic = N'مواد النانو';
update priorities set namearabic = N'موقف القرآن الكريم من القضايا المعاصرة' where namearabic = N'موقف القرآن الكريم من القضايا المعاصرة';
update priorities set namearabic = N'نشر علوم القرآن للناطقين بغير اللغة العربية' where namearabic = N'نشر علوم القرآن للناطقين بغير اللغة العربية';
update priorities set namearabic = N'نظريات الترجمة الأدبية' where namearabic = N'نظريات الترجمة الأدبية. 2';
update priorities set namearabic = N'نظرية الدراما الشارحة' where namearabic = N'نظرية الدراما الشارحة . 5';
update priorities set namearabic = N'نظم الإنتاج الحديثة وأثرها في تطوير المنظمات الصناعية بالمملكة العربية السعودية' where namearabic = N'نظم الإنتاج الحديثة وأثرها في تطوير المنظمات الصناعية بالمملكة العربية السعودية . 6';
update priorities set namearabic = N'نظم التكييف المتقدمة' where namearabic = N'نظم التكييف المتقدمة . 8';
update priorities set namearabic = N'نظم دعم اتخاذ القرارات' where namearabic = N'نظم دعم اتخاذ القرارات . 4';
update priorities set namearabic = N'نماذج الاحتمالات والإحصاء وتطبيقاتها' where namearabic = N'نماذج الاحتمالات والإحصاءوتطبيقاتها. 4';
update priorities set namearabic = N'هندسة البرمجيات' where namearabic = N'هندسة البرمجيات . 3';
update priorities set namearabic = N'هندسة المتطلبات' where namearabic = N'هندسة المتطلبات. 5';
update priorities set namearabic = N'هندسة المحفزات' where namearabic = N'هندسة المحفزات . 8';
update priorities set namearabic = N'وسائل التقنية المساعدة في تعلم وتدريس اللغة الإنجليزية' where namearabic = N'وسائل التقنية المساعدة في تعلم وتدريس اللغة الإنجليزية';
update priorities set namearabic = N'إحياء التراث القيم ونشره' where namearabic = N'ِّ إحياء التراث القيم ونشره . 2';

go





	
IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'WasProposalAccepted'
)
BEGIN
	ALTER TABLE Requests add WasProposalAccepted bit NULL;
END



IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'RefusalReason'
)
BEGIN
	ALTER TABLE Requests add RefusalReason nvarchar(500) NULL;
END
	
	



IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'Abstract' ) BEGIN 	ALTER TABLE Requests add Abstract nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'Keywords' ) BEGIN 	ALTER TABLE Requests add Keywords nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'AbstractEnglish' ) BEGIN 	ALTER TABLE Requests add AbstractEnglish nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'KeywordsEnglish' ) BEGIN 	ALTER TABLE Requests add KeywordsEnglish nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionImportance' ) BEGIN 	ALTER TABLE Requests add SectionImportance nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionProblem' ) BEGIN 	ALTER TABLE Requests add SectionProblem nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionGoals' ) BEGIN 	ALTER TABLE Requests add SectionGoals nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionAssumptions' ) BEGIN 	ALTER TABLE Requests add SectionAssumptions nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionTerms' ) BEGIN 	ALTER TABLE Requests add SectionTerms nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionObjectiveLimitations' ) BEGIN 	ALTER TABLE Requests add SectionObjectiveLimitations nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionSpatialLimitations' ) BEGIN 	ALTER TABLE Requests add SectionSpatialLimitations nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionTemporalLimitations' ) BEGIN 	ALTER TABLE Requests add SectionTemporalLimitations nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionTheoriticalFramework' ) BEGIN 	ALTER TABLE Requests add SectionTheoriticalFramework nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionPreviousStudies' ) BEGIN 	ALTER TABLE Requests add SectionPreviousStudies nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionProcedures' ) BEGIN 	ALTER TABLE Requests add SectionProcedures nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionSociety' ) BEGIN 	ALTER TABLE Requests add SectionSociety nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionTools' ) BEGIN 	ALTER TABLE Requests add SectionTools nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionChapters' ) BEGIN 	ALTER TABLE Requests add SectionChapters nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionReferences' ) BEGIN 	ALTER TABLE Requests add SectionReferences nvarchar(max) NULL; END



IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Departments'
    AND column_name = 'Field'
)
BEGIN
	ALTER TABLE Departments add [Field] tinyint NULL;
END

go

update departments set field = 1 where  id =  (select id from departments where namearabic = N'الشريعة' and collegeid = (select id from Colleges where NameArabic = N'الشريعة بالأحساء'));
update departments set field = 1 where  id =  (select id from departments where namearabic = N'أصول الدين' and collegeid = (select id from Colleges where NameArabic = N'الشريعة بالأحساء'));
update departments set field = 2 where  id =  (select id from departments where namearabic = N'اللغة العربية' and collegeid = (select id from Colleges where NameArabic = N'الشريعة بالأحساء'));
update departments set field = 3 where  id =  (select id from departments where namearabic = N'الإدارة' and collegeid = (select id from Colleges where NameArabic = N'الشريعة بالأحساء'));
update departments set field = 3 where  id =  (select id from departments where namearabic = N'الأنظمة' and collegeid = (select id from Colleges where NameArabic = N'الشريعة بالأحساء'));
update departments set field = 4 where  id =  (select id from departments where namearabic = N'الحاسب الآلي' and collegeid = (select id from Colleges where NameArabic = N'الشريعة بالأحساء'));
update departments set field = 3 where  id =  (select id from departments where namearabic = N'اللغة الإنجليزية' and collegeid = (select id from Colleges where NameArabic = N'الشريعة بالأحساء'));

go


IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'RequestMaterials' AND column_name = 'Usage' ) BEGIN 	ALTER TABLE RequestMaterials add Usage nvarchar(2000) NULL; END
go


update colleges set nameenglish = 'Higher Judicial Institute' where id = (select top 1 id from colleges c where c.namearabic like N'%المعهد العالي للقضاء%' or N'المعهد العالي للقضاء' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Higher Judicial Institute')));
update colleges set nameenglish = 'Dawah Institute' where id = (select top 1 id from colleges c where c.namearabic like N'%المعهد العالي للاحتساب%' or N'المعهد العالي للاحتساب' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Dawah Institute')));
update colleges set nameenglish = 'Sharia College' where id = (select top 1 id from colleges c where c.namearabic like N'%كلية الشريعة%' or N'كلية الشريعة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Sharia College')));
update colleges set nameenglish = 'College of Religion Basics' where id = (select top 1 id from colleges c where c.namearabic like N'%كلية أصول الدين%' or N'كلية أصول الدين' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('College of Religion Basics')));
update colleges set nameenglish = 'Sharia College in Ahsa' where id = (select top 1 id from colleges c where c.namearabic like N'%الشريعة بالأحساء%' or N'الشريعة بالأحساء' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Sharia College in Ahsa')));
update colleges set nameenglish = 'College of Computer and Information Sciences' where id = (select top 1 id from colleges c where c.namearabic like N'%كلية علوم الحاسب والمعلومات%' or N'كلية علوم الحاسب والمعلومات' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('College of Computer and Information Sciences')));
update colleges set nameenglish = 'College of  Medicine' where id = (select top 1 id from colleges c where c.namearabic like N'%كلية الطب%' or N'كلية الطب' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('College of  Medicine')));
update colleges set nameenglish = 'College of Science' where id = (select top 1 id from colleges c where c.namearabic like N'%كلية العلوم%' or N'كلية العلوم' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('College of Science')));
update colleges set nameenglish = 'College of Engineering' where id = (select top 1 id from colleges c where c.namearabic like N'%كلية الهندسة%' or N'كلية الهندسة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('College of Engineering')));
update colleges set nameenglish = 'College of Arabic Language' where id = (select top 1 id from colleges c where c.namearabic like N'%كلية اللغة العربية%' or N'كلية اللغة العربية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('College of Arabic Language')));
update colleges set nameenglish = 'Institute of Arabic Language Teaching' where id = (select top 1 id from colleges c where c.namearabic like N'%معهد تعليم اللغة العربية%' or N'معهد تعليم اللغة العربية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Institute of Arabic Language Teaching')));
update colleges set nameenglish = 'College of Media and Communication' where id = (select top 1 id from colleges c where c.namearabic like N'%كلية الاعلام والاتصال%' or N'كلية الاعلام والاتصال' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('College of Media and Communication')));
update colleges set nameenglish = 'College of Social Science' where id = (select top 1 id from colleges c where c.namearabic like N'%كلية العلوم الاجتماعية%' or N'كلية العلوم الاجتماعية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('College of Social Science')));
update colleges set nameenglish = 'College of Languages and Translation' where id = (select top 1 id from colleges c where c.namearabic like N'%كلية اللغات والترجمة%' or N'كلية اللغات والترجمة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('College of Languages and Translation')));
update colleges set nameenglish = 'College of Economics and Management Sciences' where id = (select top 1 id from colleges c where c.namearabic like N'%كلية الاقتصاد والعلوم الإدارية%' or N'كلية الاقتصاد والعلوم الإدارية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('College of Economics and Management Sciences')));




go


update departments set nameenglish = 'Quran and its Sciences' where id = (select top 1 id from departments c where c.namearabic like N'%القران الكريم وعلومه%' or N'القران الكريم وعلومه' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Quran and its Sciences')));
update departments set nameenglish = 'Sunnah' where id = (select top 1 id from departments c where c.namearabic like N'%السنة النبوية%' or N'السنة النبوية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Sunnah')));
update departments set nameenglish = 'Creed and contemporary doctrines' where id = (select top 1 id from departments c where c.namearabic like N'%العقيدة والمذاهب المعاصرة%' or N'العقيدة والمذاهب المعاصرة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Creed and contemporary doctrines')));
update departments set nameenglish = 'Labor judiciary' where id = (select top 1 id from departments c where c.namearabic like N'%القضاء العمالي%' or N'القضاء العمالي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Labor judiciary')));
update departments set nameenglish = 'Comparative Jurisprudence' where id = (select top 1 id from departments c where c.namearabic like N'%الفقه المقارن%' or N'الفقه المقارن' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Comparative Jurisprudence')));
update departments set nameenglish = 'Legal Policy' where id = (select top 1 id from departments c where c.namearabic like N'%السياسة الشرعية%' or N'السياسة الشرعية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Legal Policy')));
update departments set nameenglish = 'Commercial Judiciary' where id = (select top 1 id from departments c where c.namearabic like N'%القضاء التجاري%' or N'القضاء التجاري' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Commercial Judiciary')));
update departments set nameenglish = 'Criminal Justice' where id = (select top 1 id from departments c where c.namearabic like N'%القضاء الجزائي%' or N'القضاء الجزائي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Criminal Justice')));
update departments set nameenglish = 'Censorship of personal status' where id = (select top 1 id from departments c where c.namearabic like N'%قضاء الأحوال الشخصية%' or N'قضاء الأحوال الشخصية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Censorship of personal status')));
update departments set nameenglish = 'Administrative judiciary' where id = (select top 1 id from departments c where c.namearabic like N'%القضاء الإداري%' or N'القضاء الإداري' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Administrative judiciary')));
update departments set nameenglish = 'Contemporary Islamic Studies' where id = (select top 1 id from departments c where c.namearabic like N'%الدراسات الإسلامية المعاصرة%' or N'الدراسات الإسلامية المعاصرة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Contemporary Islamic Studies')));
update departments set nameenglish = 'the invitation' where id = (select top 1 id from departments c where c.namearabic like N'%الدعوة%' or N'الدعوة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('the invitation')));
update departments set nameenglish = 'Hessba' where id = (select top 1 id from departments c where c.namearabic like N'%الحسبة%' or N'الحسبة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Hessba')));
update departments set nameenglish = 'Jurisprudence' where id = (select top 1 id from departments c where c.namearabic like N'%الفقه%' or N'الفقه' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Jurisprudence')));
update departments set nameenglish = 'Principles of Fiqh' where id = (select top 1 id from departments c where c.namearabic like N'%أصول الفقه%' or N'أصول الفقه' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Principles of Fiqh')));
update departments set nameenglish = 'Islamic culture' where id = (select top 1 id from departments c where c.namearabic like N'%الثقافة الإسلامية%' or N'الثقافة الإسلامية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Islamic culture')));
update departments set nameenglish = 'Systems' where id = (select top 1 id from departments c where c.namearabic like N'%الأنظمة%' or N'الأنظمة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Systems')));
update departments set nameenglish = 'Information Systems' where id = (select top 1 id from departments c where c.namearabic like N'%نظم المعلومات%' or N'نظم المعلومات' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Information Systems')));
update departments set nameenglish = 'Computer Science' where id = (select top 1 id from departments c where c.namearabic like N'%علوم الحاسب%' or N'علوم الحاسب' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Computer Science')));
update departments set nameenglish = 'information technology' where id = (select top 1 id from departments c where c.namearabic like N'%تقنية المعلومات%' or N'تقنية المعلومات' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('information technology')));
update departments set nameenglish = 'information management' where id = (select top 1 id from departments c where c.namearabic like N'%إدارة المعلومات%' or N'إدارة المعلومات' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('information management')));
update departments set nameenglish = 'family Medicine' where id = (select top 1 id from departments c where c.namearabic like N'%طب الأسرة%' or N'طب الأسرة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('family Medicine')));
update departments set nameenglish = 'Public Health' where id = (select top 1 id from departments c where c.namearabic like N'%الصحة العامة%' or N'الصحة العامة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Public Health')));
update departments set nameenglish = 'the eyes' where id = (select top 1 id from departments c where c.namearabic like N'%العيون%' or N'العيون' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('the eyes')));
update departments set nameenglish = 'Anatomy' where id = (select top 1 id from departments c where c.namearabic like N'%التشريح%' or N'التشريح' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Anatomy')));
update departments set nameenglish = 'Pathology' where id = (select top 1 id from departments c where c.namearabic like N'%علوم الامراض%' or N'علوم الامراض' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Pathology')));
update departments set nameenglish = 'Biochemistry' where id = (select top 1 id from departments c where c.namearabic like N'%الكيمياء الحيوية%' or N'الكيمياء الحيوية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Biochemistry')));
update departments set nameenglish = 'pharmaceutical' where id = (select top 1 id from departments c where c.namearabic like N'%الأدوية%' or N'الأدوية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('pharmaceutical')));
update departments set nameenglish = 'functions of organs' where id = (select top 1 id from departments c where c.namearabic like N'%وظائف الأعضاء%' or N'وظائف الأعضاء' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('functions of organs')));
update departments set nameenglish = 'Medical Education' where id = (select top 1 id from departments c where c.namearabic like N'%التعليم الطبي%' or N'التعليم الطبي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Medical Education')));
update departments set nameenglish = 'Cutaneous' where id = (select top 1 id from departments c where c.namearabic like N'%الجلدية%' or N'الجلدية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Cutaneous')));
update departments set nameenglish = 'Chemistry' where id = (select top 1 id from departments c where c.namearabic like N'%الكيمياء%' or N'الكيمياء' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Chemistry')));
update departments set nameenglish = 'Physics' where id = (select top 1 id from departments c where c.namearabic like N'%الفيزياء%' or N'الفيزياء' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Physics')));
update departments set nameenglish = 'Mathematics and Statistics' where id = (select top 1 id from departments c where c.namearabic like N'%الرياضيات والاحصاء%' or N'الرياضيات والاحصاء' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Mathematics and Statistics')));
update departments set nameenglish = 'Biology' where id = (select top 1 id from departments c where c.namearabic like N'%الأحياء%' or N'الأحياء' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Biology')));
update departments set nameenglish = 'Mechanical engineering' where id = (select top 1 id from departments c where c.namearabic like N'%الهندسة الميكانيكية%' or N'الهندسة الميكانيكية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Mechanical engineering')));
update departments set nameenglish = 'Civil engineering' where id = (select top 1 id from departments c where c.namearabic like N'%الهندسة المدنية%' or N'الهندسة المدنية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Civil engineering')));
update departments set nameenglish = 'electrical engineering' where id = (select top 1 id from departments c where c.namearabic like N'%الهندسة الكهربائية%' or N'الهندسة الكهربائية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('electrical engineering')));
update departments set nameenglish = 'Chemical Engineering' where id = (select top 1 id from departments c where c.namearabic like N'%الهندسة الكيميائية%' or N'الهندسة الكيميائية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Chemical Engineering')));
update departments set nameenglish = 'Architecture Engineering' where id = (select top 1 id from departments c where c.namearabic like N'%الهندسة المعمارية%' or N'الهندسة المعمارية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Architecture Engineering')));
update departments set nameenglish = 'Literature' where id = (select top 1 id from departments c where c.namearabic like N'%الادب%' or N'الادب' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Literature')));
update departments set nameenglish = 'The rhetoric and criticism and the approach of Islamic literature' where id = (select top 1 id from departments c where c.namearabic like N'%البلاغة والنقد ومنهج الادب الإسلامي%' or N'البلاغة والنقد ومنهج الادب الإسلامي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('The rhetoric and criticism and the approach of Islamic literature')));
update departments set nameenglish = 'Arabic Grammar and the Understanding of Language' where id = (select top 1 id from departments c where c.namearabic like N'%النحو والصرف وفقه اللغة%' or N'النحو والصرف وفقه اللغة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Arabic Grammar and the Understanding of Language')));
update departments set nameenglish = 'Arabic and Islamic Sciences' where id = (select top 1 id from departments c where c.namearabic like N'%اللغة العربية والعلوم الإسلامية%' or N'اللغة العربية والعلوم الإسلامية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Arabic and Islamic Sciences')));
update departments set nameenglish = 'Applied Linguistics Sciences' where id = (select top 1 id from departments c where c.namearabic like N'%علم اللغة التطبيقي%' or N'علم اللغة التطبيقي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Applied Linguistics Sciences')));
update departments set nameenglish = 'Language Preparation' where id = (select top 1 id from departments c where c.namearabic like N'%الإعداد اللغوي%' or N'الإعداد اللغوي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Language Preparation')));
update departments set nameenglish = 'Press and electronic publishing' where id = (select top 1 id from departments c where c.namearabic like N'%الصحافة والنشر  الإكتروني%' or N'الصحافة والنشر  الإكتروني' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Press and electronic publishing')));
update departments set nameenglish = 'Radio, television and film' where id = (select top 1 id from departments c where c.namearabic like N'%الإذاعة والتليفزيون والفيلم%' or N'الإذاعة والتليفزيون والفيلم' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Radio, television and film')));
update departments set nameenglish = 'Public Relations' where id = (select top 1 id from departments c where c.namearabic like N'%العلاقات العامة%' or N'العلاقات العامة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Public Relations')));
update departments set nameenglish = 'Advertising and Marketing Communication' where id = (select top 1 id from departments c where c.namearabic like N'%الإعلان والإتصال التسويقي%' or N'الإعلان والإتصال التسويقي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Advertising and Marketing Communication')));
update departments set nameenglish = 'Graphics and multimedia' where id = (select top 1 id from departments c where c.namearabic like N'%الجرفيكس والوسائط المتعددة%' or N'الجرفيكس والوسائط المتعددة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Graphics and multimedia')));
update departments set nameenglish = 'Specialized media' where id = (select top 1 id from departments c where c.namearabic like N'%الإعلام المتخصص%' or N'الإعلام المتخصص' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Specialized media')));
update departments set nameenglish = 'Geography' where id = (select top 1 id from departments c where c.namearabic like N'%الجغرافيا%' or N'الجغرافيا' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Geography')));
update departments set nameenglish = 'Social Service' where id = (select top 1 id from departments c where c.namearabic like N'%الاجتماع والخدمة الاجتماعية%' or N'الاجتماع والخدمة الاجتماعية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Social Service')));
update departments set nameenglish = 'Psychology' where id = (select top 1 id from departments c where c.namearabic like N'%علم النفس%' or N'علم النفس' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Psychology')));
update departments set nameenglish = 'Management and Educational Planning' where id = (select top 1 id from departments c where c.namearabic like N'%الإدارة والتخطيط التربوي%' or N'الإدارة والتخطيط التربوي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Management and Educational Planning')));
update departments set nameenglish = 'Education Basics' where id = (select top 1 id from departments c where c.namearabic like N'%أصول التربية%' or N'أصول التربية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Education Basics')));
update departments set nameenglish = 'Curriculum and Instruction' where id = (select top 1 id from departments c where c.namearabic like N'%المناهج وطرق التدريس%' or N'المناهج وطرق التدريس' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Curriculum and Instruction')));
update departments set nameenglish = 'Special Education' where id = (select top 1 id from departments c where c.namearabic like N'%التربية الخاصة%' or N'التربية الخاصة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Special Education')));
update departments set nameenglish = 'History and Civilization' where id = (select top 1 id from departments c where c.namearabic like N'%التاريخ والحضارة%' or N'التاريخ والحضارة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('History and Civilization')));
update departments set nameenglish = 'Languages ​​and Translation' where id = (select top 1 id from departments c where c.namearabic like N'%اللغات والترجمة%' or N'اللغات والترجمة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Languages ​​and Translation')));
update departments set nameenglish = 'Economy' where id = (select top 1 id from departments c where c.namearabic like N'%الاقتصاد%' or N'الاقتصاد' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Economy')));
update departments set nameenglish = 'Accounting' where id = (select top 1 id from departments c where c.namearabic like N'%المحاسبة%' or N'المحاسبة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Accounting')));
update departments set nameenglish = 'Business Management' where id = (select top 1 id from departments c where c.namearabic like N'%إدارة الاعمال%' or N'إدارة الاعمال' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Business Management')));
update departments set nameenglish = 'Finance and investment' where id = (select top 1 id from departments c where c.namearabic like N'%التمويل والاستثمار%' or N'التمويل والاستثمار' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Finance and investment')));
update departments set nameenglish = 'Banking' where id = (select top 1 id from departments c where c.namearabic like N'%الإعمال المصرفية%' or N'الإعمال المصرفية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Banking')));
update departments set nameenglish = 'Lnsurance and Risk Management' where id = (select top 1 id from departments c where c.namearabic like N'%التأمين وإدارة المخاطر%' or N'التأمين وإدارة المخاطر' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Lnsurance and Risk Management')));
update departments set nameenglish = 'Sharia' where id = (select top 1 id from departments c where c.namearabic like N'%الشريعة%' or N'الشريعة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Sharia')));
update departments set nameenglish = 'Religion Basics' where id = (select top 1 id from departments c where c.namearabic like N'%أصول الدين%' or N'أصول الدين' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Religion Basics')));
update departments set nameenglish = 'Arabic Language' where id = (select top 1 id from departments c where c.namearabic like N'%اللغة العربية%' or N'اللغة العربية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Arabic Language')));
update departments set nameenglish = 'Administration' where id = (select top 1 id from departments c where c.namearabic like N'%الإدارة%' or N'الإدارة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Administration')));
update departments set nameenglish = 'Systems' where id = (select top 1 id from departments c where c.namearabic like N'%الأنظمة%' or N'الأنظمة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Systems')));
update departments set nameenglish = 'computer' where id = (select top 1 id from departments c where c.namearabic like N'%الحاسب الآلي%' or N'الحاسب الآلي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('computer')));
update departments set nameenglish = 'English Language' where id = (select top 1 id from departments c where c.namearabic like N'%اللغة الإنجليزية%' or N'اللغة الإنجليزية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('English Language')));
update departments set nameenglish = 'English literature' where id = (select top 1 id from departments c where c.namearabic like N'%الأدب الإنجليزي%' or N'الأدب الإنجليزي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('English literature')));


go


IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'IsWithinPriority' ) BEGIN 	ALTER TABLE Requests add IsWithinPriority bit NULL; END


go




update universities set nameenglish = 'Al-Imam Mohammad Ibn Saud Islamic University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الإمام محمد بن سعود الإسلامية%' or N'جامعة الإمام محمد بن سعود الإسلامية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Al-Imam Mohammad Ibn Saud Islamic University')));
update universities set nameenglish = 'Umm Al Qura University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة أم القرى%' or N'جامعة أم القرى' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Umm Al Qura University')));
update universities set nameenglish = 'Islamic University' where id = (select top 1 id from universities c where c.namearabic like N'%الجامعة الإسلامية%' or N'الجامعة الإسلامية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Islamic University')));
update universities set nameenglish = 'King Saud University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الملك سعود%' or N'جامعة الملك سعود' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('King Saud University')));
update universities set nameenglish = 'King Fahd University of Petroleum and Minerals' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الملك فهد للبترول والمعادن%' or N'جامعة الملك فهد للبترول والمعادن' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('King Fahd University of Petroleum and Minerals')));
update universities set nameenglish = 'King Faisal University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الملك فيصل%' or N'جامعة الملك فيصل' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('King Faisal University')));
update universities set nameenglish = 'King Khalid University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الملك خالد%' or N'جامعة الملك خالد' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('King Khalid University')));
update universities set nameenglish = 'Al Qussaim university' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة القصيم%' or N'جامعة القصيم' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Al Qussaim university')));
update universities set nameenglish = 'Taiba University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة طيبة%' or N'جامعة طيبة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Taiba University')));
update universities set nameenglish = 'Taif University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الطائف%' or N'جامعة الطائف' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Taif University')));
update universities set nameenglish = 'Hael University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة حائل%' or N'جامعة حائل' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Hael University')));
update universities set nameenglish = 'Gazan University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة جازان%' or N'جامعة جازان' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Gazan University')));
update universities set nameenglish = 'Al-Jouf University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الجوف%' or N'جامعة الجوف' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Al-Jouf University')));
update universities set nameenglish = 'Al Baha university' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الباحة%' or N'جامعة الباحة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Al Baha university')));
update universities set nameenglish = 'Tabouk university' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة تبوك%' or N'جامعة تبوك' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Tabouk university')));
update universities set nameenglish = 'Najran university' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة نجران%' or N'جامعة نجران' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Najran university')));
update universities set nameenglish = 'Northern Border University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الحدود الشمالية%' or N'جامعة الحدود الشمالية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Northern Border University')));
update universities set nameenglish = 'Princess Nora bint AbdulRahman University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الأميرة نورة بنت عبد الرحمن%' or N'جامعة الأميرة نورة بنت عبد الرحمن' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Princess Nora bint AbdulRahman University')));
update universities set nameenglish = 'King Saud bin Abdulaziz University for Health Sciences' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الملك سعود بن عبد العزيز للعلوم الصحية%' or N'جامعة الملك سعود بن عبد العزيز للعلوم الصحية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('King Saud bin Abdulaziz University for Health Sciences')));
update universities set nameenglish = 'Imam Abdul Rahman Al-Faisal University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الامام عبد الرحمن الفيصل%' or N'جامعة الامام عبد الرحمن الفيصل' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Imam Abdul Rahman Al-Faisal University')));
update universities set nameenglish = 'Prince Sattam University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الأمير سطام بن عبد العزيز%' or N'جامعة الأمير سطام بن عبد العزيز' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Prince Sattam University')));
update universities set nameenglish = 'Shakra University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة شقراء%' or N'جامعة شقراء' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Shakra University')));
update universities set nameenglish = 'Majmaah University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة المجمعة%' or N'جامعة المجمعة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Majmaah University')));
update universities set nameenglish = 'the Saudi electronic university' where id = (select top 1 id from universities c where c.namearabic like N'%الجامعة السعودية الإلكترونية%' or N'الجامعة السعودية الإلكترونية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('the Saudi electronic university')));
update universities set nameenglish = 'Jeddah University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة جدة%' or N'جامعة جدة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Jeddah University')));
update universities set nameenglish = 'Bisha University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة بيشة%' or N'جامعة بيشة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Bisha University')));
update universities set nameenglish = 'Prince Sultan Private University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الأمير سلطان الأهلية%' or N'جامعة الأمير سلطان الأهلية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Prince Sultan Private University')));
update universities set nameenglish = 'Effat University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة عفت الأهلية%' or N'جامعة عفت الأهلية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Effat University')));
update universities set nameenglish = 'Arabic Open University' where id = (select top 1 id from universities c where c.namearabic like N'%الجامعة العربية المفتوحة%' or N'الجامعة العربية المفتوحة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Arabic Open University')));
update universities set nameenglish = 'Al -alyamamah University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة اليمامة الأهلية%' or N'جامعة اليمامة الأهلية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Al -alyamamah University')));
update universities set nameenglish = 'Business and Technology University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الاعمال والتكنولوجيا الأهلية%' or N'جامعة الاعمال والتكنولوجيا الأهلية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Business and Technology University')));
update universities set nameenglish = 'Prince Mohammed bin Fahd National University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الأمير محمد بن فهد الأهلية%' or N'جامعة الأمير محمد بن فهد الأهلية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Prince Mohammed bin Fahd National University')));
update universities set nameenglish = 'Al - Faisal National University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الفيصل الأهلية%' or N'جامعة الفيصل الأهلية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Al - Faisal National University')));
update universities set nameenglish = 'Dar Al-Uloom University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة دار العلوم الأهلية%' or N'جامعة دار العلوم الأهلية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Dar Al-Uloom University')));
update universities set nameenglish = 'Dar Al-Hekma University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة دار الحكمة الأهلية%' or N'جامعة دار الحكمة الأهلية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Dar Al-Hekma University')));
update universities set nameenglish = 'Prince Muqrin bin Abdulaziz University' where id = (select top 1 id from universities c where c.namearabic like N'%جامعة الأمير مقرن بن عبد العزيز%' or N'جامعة الأمير مقرن بن عبد العزيز' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Prince Muqrin bin Abdulaziz University')));


go


update banks set nameenglish = 'SAMBA Financial Group' where id = (select top 1 id from banks c where c.namearabic like N'%مجموعة سامبا المالية%' or N'مجموعة سامبا المالية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('SAMBA Financial Group')));
update banks set nameenglish = 'SABB Bank' where id = (select top 1 id from banks c where c.namearabic like N'%بنك ساب%' or N'بنك ساب' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('SABB Bank')));
update banks set nameenglish = 'Al Rajhi Bank' where id = (select top 1 id from banks c where c.namearabic like N'%مصرف الراجحي%' or N'مصرف الراجحي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Al Rajhi Bank')));
update banks set nameenglish = 'First Bank' where id = (select top 1 id from banks c where c.namearabic like N'%البنك الأول%' or N'البنك الأول' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('First Bank')));
update banks set nameenglish = 'Arab national Bank' where id = (select top 1 id from banks c where c.namearabic like N'%البنك العربي الوطني%' or N'البنك العربي الوطني' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Arab national Bank')));
update banks set nameenglish = 'Al Jazeera Bank' where id = (select top 1 id from banks c where c.namearabic like N'%بنك الجزيرة%' or N'بنك الجزيرة' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Al Jazeera Bank')));
update banks set nameenglish = 'Saudi French Bank' where id = (select top 1 id from banks c where c.namearabic like N'%البنك السعودي الفرنسي%' or N'البنك السعودي الفرنسي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Saudi French Bank')));
update banks set nameenglish = 'National Commercial Bank' where id = (select top 1 id from banks c where c.namearabic like N'%البنك الأهلي التجاري%' or N'البنك الأهلي التجاري' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('National Commercial Bank')));
update banks set nameenglish = 'Riyad Bank' where id = (select top 1 id from banks c where c.namearabic like N'%بنك الرياض%' or N'بنك الرياض' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Riyad Bank')));
update banks set nameenglish = 'The Saudi Investment Bank' where id = (select top 1 id from banks c where c.namearabic like N'%البنك السعودي للاستثمار%' or N'البنك السعودي للاستثمار' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('The Saudi Investment Bank')));
update banks set nameenglish = 'Al Bilad Finance Company' where id = (select top 1 id from banks c where c.namearabic like N'%شركة البلاد المالية%' or N'شركة البلاد المالية' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Al Bilad Finance Company')));
update banks set nameenglish = 'Muscat bank' where id = (select top 1 id from banks c where c.namearabic like N'%بنك مسقط%' or N'بنك مسقط' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Muscat bank')));
update banks set nameenglish = 'Alinma Bank' where id = (select top 1 id from banks c where c.namearabic like N'%مصرف الانماء%' or N'مصرف الانماء' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Alinma Bank')));
update banks set nameenglish = 'Kuwait Bank' where id = (select top 1 id from banks c where c.namearabic like N'%بنك الكويت%' or N'بنك الكويت' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Kuwait Bank')));
update banks set nameenglish = 'Emirates NBD Bank' where id = (select top 1 id from banks c where c.namearabic like N'%بنك الامارات دبي الوطني    %' or N'بنك الامارات دبي الوطني    ' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Emirates NBD Bank')));
update banks set nameenglish = 'Gulf International Bank' where id = (select top 1 id from banks c where c.namearabic like N'%بنك الخليج الدولي%' or N'بنك الخليج الدولي' like N'%' + c.namearabic + N'%' order by abs(len(c.namearabic) - len('Gulf International Bank')));


go


IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'UserProfiles' AND column_name = 'CvSectionPublishedPapers' ) BEGIN 	ALTER TABLE UserProfiles add CvSectionPublishedPapers nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'UserProfiles' AND column_name = 'CvSectionInterests' ) BEGIN 	ALTER TABLE UserProfiles add CvSectionInterests nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'UserProfiles' AND column_name = 'CvSectionCareerHistory' ) BEGIN 	ALTER TABLE UserProfiles add CvSectionCareerHistory nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'UserProfiles' AND column_name = 'CvSectionTrainingCourses' ) BEGIN 	ALTER TABLE UserProfiles add CvSectionTrainingCourses nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'UserProfiles' AND column_name = 'CvSectionAwards' ) BEGIN 	ALTER TABLE UserProfiles add CvSectionAwards nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'UserProfiles' AND column_name = 'CvSectionMemberships' ) BEGIN 	ALTER TABLE UserProfiles add CvSectionMemberships nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'UserProfiles' AND column_name = 'CvSectionIbanPhotocopy' ) BEGIN 	ALTER TABLE UserProfiles add CvSectionIbanPhotocopy nvarchar(max) NULL; END





go


IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'RequestCoresearchers' AND column_name = 'FirstPeriodActiveMonths' ) BEGIN 	ALTER TABLE RequestCoresearchers add FirstPeriodActiveMonths int NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'RequestCoresearchers' AND column_name = 'FirstPeriodCost' ) BEGIN 	ALTER TABLE RequestCoresearchers add FirstPeriodCost float NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'RequestCoresearchers' AND column_name = 'SecondPeriodActiveMonths' ) BEGIN 	ALTER TABLE RequestCoresearchers add SecondPeriodActiveMonths int NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'RequestCoresearchers' AND column_name = 'SecondPeriodCost' ) BEGIN 	ALTER TABLE RequestCoresearchers add SecondPeriodCost float NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'RequestCoresearchers' AND column_name = 'TotalActiveMonths' ) BEGIN 	ALTER TABLE RequestCoresearchers add TotalActiveMonths int NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'RequestCoresearchers' AND column_name = 'TotalCost' ) BEGIN 	ALTER TABLE RequestCoresearchers add TotalCost float NULL; END

IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'RequestCoresearchers' AND column_name = 'CountryId' ) BEGIN 	ALTER TABLE RequestCoresearchers add CountryId int NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'RequestCoresearchers' AND column_name = 'CityId' ) BEGIN 	ALTER TABLE RequestCoresearchers add CityId int NULL; END





go

	
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'FirstPeriodTotalCost' ) BEGIN 	ALTER TABLE Requests add FirstPeriodTotalCost float NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SecondPeriodTotalCost' ) BEGIN 	ALTER TABLE Requests add SecondPeriodTotalCost float NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'MaterialsTotalCost' ) BEGIN 	ALTER TABLE Requests add MaterialsTotalCost float NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'TotalCostForProject' ) BEGIN 	ALTER TABLE Requests add TotalCostForProject float NULL; END

IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'RequestTimeline' AND column_name = 'NumberOfVisits' ) BEGIN 	ALTER TABLE RequestTimeline add NumberOfVisits int NULL; END




go


IF (OBJECT_ID('FK_Requests_Priorities', 'F') IS NULL) BEGIN ALTER TABLE [dbo].[Requests]  WITH NOCHECK ADD  CONSTRAINT [FK_Requests_Priorities] FOREIGN KEY([PriorityId]) REFERENCES [dbo].[Priorities] ([Id]) END


-- Date: 02/28/2018
-- Added new columns to Requests Table
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Requests]') 
         AND name = 'ProjectAdvantages'
)
ALTER TABLE Requests ADD [ProjectAdvantages] [NVARCHAR](1000) NULL;
GO

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Requests]') 
         AND name = 'ProjectDisadvantages'
)
ALTER TABLE Requests ADD [ProjectDisadvantages] [NVARCHAR](1000) NULL;
GO

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Requests]') 
         AND name = 'Recommendation'
)
ALTER TABLE Requests ADD [Recommendation] BIGINT NULL;
GO

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Requests]') 
         AND name = 'ReasonForInvalidRecommendation'
)
ALTER TABLE Requests ADD [ReasonForInvalidRecommendation] [NVARCHAR](1000) NULL;
GO

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Requests]') 
         AND name = 'MeetingNumber'
)
ALTER TABLE Requests ADD [MeetingNumber] BIGINT NULL;
GO
