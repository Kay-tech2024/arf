-- Date: 07/29/2018
-- Added new columns to Requests Table
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[RequestTimeline]') 
         AND name = 'CompletionRate'
)
ALTER TABLE RequestTimeline ADD [CompletionRate] [tinyint] NULL;
GO
-- Date: 07/29/2018
-- Added new columns to Requests Table
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[RequestTimeline]') 
         AND name = 'Reasons'
)
ALTER TABLE RequestTimeline ADD [Reasons] [text] NULL;
GO
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
ALTER TABLE Requests ADD [MeetingNumber] [NVARCHAR](200) NULL;
GO

-- Date: 03/04/2018
-- Added Forieng Key FK_RequestSuggestedReferees_Users 
IF NOT EXISTS 
(
    SELECT * FROM sys.foreign_key_columns fk 
    INNER JOIN sys.columns pc ON pc.object_id = fk.parent_object_id AND pc.column_id = fk.parent_column_id 
    INNER JOIN sys.columns rc ON rc.object_id = fk.referenced_object_id AND rc.column_id = fk.referenced_column_id
    WHERE fk.parent_object_id = object_id('RequestSuggestedReferees') AND pc.name = 'CreatedBy'
    AND fk.referenced_object_id = object_id('Users') AND rc.NAME = 'Id'
)
ALTER TABLE RequestSuggestedReferees 
ADD CONSTRAINT FK_RequestSuggestedReferees_Users FOREIGN KEY (CreatedBy)
REFERENCES Users(Id)

-- Date: 03/05/2018
-- Added new columns to Researchers Table
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Researchers]') 
         AND name = 'Specialization'
)
ALTER TABLE Researchers ADD [Specialization] [NVARCHAR](100) NULL;
GO




IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionObjectives' ) BEGIN 	ALTER TABLE Requests add SectionObjectives nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionRelevance' ) BEGIN 	ALTER TABLE Requests add SectionRelevance nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionBackground' ) BEGIN 	ALTER TABLE Requests add SectionBackground nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionMethodology' ) BEGIN 	ALTER TABLE Requests add SectionMethodology nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'SectionAppendices' ) BEGIN 	ALTER TABLE Requests add SectionAppendices nvarchar(max) NULL; END
IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Requests' AND column_name = 'BudgetRationale' ) BEGIN 	ALTER TABLE Requests add BudgetRationale nvarchar(max) NULL; END





IF EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo'  AND TABLE_NAME = N'RequestFundingSources')
BEGIN
	DROP TABLE [dbo].[RequestFundingSources]
END

go

CREATE TABLE [dbo].[RequestFundingSources](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestId] [bigint] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[Amount] [float] NULL,
	[Duration] [int] NULL,
	[Status] [tinyint] NULL,
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


ALTER TABLE [dbo].[RequestFundingSources] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[RequestFundingSources]  WITH CHECK ADD  CONSTRAINT [FK_RequestFundingSources_Requests] FOREIGN KEY([RequestId])
REFERENCES [dbo].[Requests] ([Id])
GO

ALTER TABLE [dbo].[RequestFundingSources] CHECK CONSTRAINT [FK_RequestFundingSources_Requests]
GO





IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'UserProfiles' AND column_name = 'CvSectionAcademicHistory' ) BEGIN 	ALTER TABLE UserProfiles add CvSectionAcademicHistory nvarchar(max) NULL; END



IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[FK_Researchers_Cities]'))
BEGIN
ALTER TABLE Researchers DROP CONSTRAINT FK_Researchers_Cities
END

-- Date: 03/07/2018
-- Added new columns to Researchers Table

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Requests]') 
         AND name = 'MeetingDate'
)
ALTER TABLE Requests ADD [MeetingDate] DATETIME NULL;
GO



------------



IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[RequestSuggestedReferees]') 
         AND name = 'Specialization'
)
ALTER TABLE RequestSuggestedReferees ADD [Specialization] nvarchar(500) NULL;
GO




IF OBJECT_ID('dbo.RemoveUniqueConstraints') IS NOT  NULL
	DROP PROCEDURE dbo.[RemoveUniqueConstraints]
go


CREATE PROCEDURE [dbo].[RemoveUniqueConstraints]

	 @tableName nvarchar(255),
	 @columnName nvarchar(255)

AS
BEGIN
	
	DECLARE @constraintName nvarchar(255)
	DECLARE cur2 CURSOR FOR select TC.Constraint_Name from information_schema.table_constraints TC
			inner join information_schema.constraint_column_usage CC on TC.Constraint_Name = CC.Constraint_Name
			where TC.constraint_type = 'Unique'

			and tc.TABLE_NAME = @tableName
			and COLUMN_NAME = @columnName
			order by TC.Constraint_Name
	OPEN cur2

	FETCH NEXT FROM cur2 INTO @constraintName

	WHILE @@FETCH_STATUS = 0 BEGIN
		EXEC ('ALTER TABLE '+@tableName+' DROP CONSTRAINT ' + @constraintName)
		FETCH NEXT FROM cur2 INTO @constraintName
	END
	CLOSE cur2    
	DEALLOCATE cur2
		
	RETURN 0
	
END

GO

exec [dbo].[RemoveUniqueConstraints] 'Users', 'Email';


delete from cities
go

set identity_insert Cities on
go

insert into cities (Id, NameArabic, NameEnglish, CountryId) values (1, N'الرياض', N'Riyadh', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (2, N'الرميلة', N'Rumailah', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (3, N'الزيمة', N'Az Zaimah', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (4, N'السليل', N'السليل', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (5, N'الطائف', N'الطائف', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (6, N'الطريف', N'طريف', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (7, N'الظهران', N'الظهران', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (8, N'العضيلية', N'العضيلية', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (9, N'العقير', N'ميناء العقير', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (10, N'العلا', N'العلا', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (11, N'العمران', N'الأحساء', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (12, N'العيينة', N'العيينة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (13, N'القريات', N'Gurayat', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (14, N'القضيمة', N'القضيمة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (15, N'القطيف', N'القطيف', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (16, N'القنفذة', N'القنفذة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (17, N'القويعية', N'محافظة القويعية', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (18, N'القيصومة', N'القيصومة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (19, N'الليث', N'الليث', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (20, N'المجمعة', N'المجمعة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (21, N'المزاحمية', N'محافظة المزاحمية', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (22, N'النماص', N'محافظة النماص', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (23, N'الهفوف', N'الهفوف', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (24, N'الوجه', N'الوجه', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (25, N'بدر حنين', N'بدر', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (26, N'بقيق', N'بقيق', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (27, N'بلجرشي', N'محافظة بلجرشي', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (28, N'بيشة', N'محافظة بيشة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (29, N'تاروت', N'تاروت', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (30, N'تبوك', N'Tabuk', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (31, N'تربة البقوم', N'تربة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (32, N'تنومة', N'تنومة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (33, N'تيماء', N'تيماء', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (34, N'ثادق', N'محافظة ثادق', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (35, N'ثقبة', N'الثقبة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (36, N'ثول', N'ثول', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (37, N'جازان', N'جازان', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (38, N'جلاجل', N'جلاجل', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (39, N'حائل', N'حائل', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (40, N'حفر الباطن', N'حفر الباطن', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (41, N'حقل', N'محافظة حقل', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (42, N'حوطة بني تميم', N'حوطة بني تميم', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (43, N'خميس مشيط', N'خميس مشيط', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (44, N'دهبان', N'ذهبان', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (45, N'دومة الجندل', N'محافظة دومة الجندل', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (46, N'رابغ', N'رابغ', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (47, N'رأس تنورة', N'رأس تنورة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (48, N'رفحا', N'رفحاء', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (49, N'زلفي', N'محافظة الزلفي', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (50, N'سكاكا', N'محافظة سكاكا', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (51, N'سيهات', N'سيهات', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (52, N'شرورة', N'شرورة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (53, N'شيبه', N'Shaybah', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (54, N'صفوى', N'صفوى', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (55, N'ضبا', N'Duba', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (56, N'ضرما', N'ضرما', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (57, N'عرعر', N'مصطفى وهبي التل', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (58, N'عفيف', N'محافظة عفيف', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (59, N'عنيزة', N'عنيزة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (60, N'فرسان', N'Farasan city', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (61, N'فيفاء', N'faifa', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (62, N'ليلى', N'Layla', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (63, N'مدينة الملك عبد الله الاقتصادية', N'K. Abdullah Economic City', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (64, N'نجران', N'نجران (مدينة)', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (65, N'وادي الدواسر', N'وادي الدواسر', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (66, N'ينبع', N'ينبع', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (67, N'بلومنغتون', N'Bloomington', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (68, N'مكة المكرمة', N'Makkah Al-Mukarramah', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (69, N'المندق', N'Almandaq', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (70, N'محافظة القريات', N'Qurayyat', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (71, N'جدة_المملكة العربية السعوودية', N'Jeddah', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (72, N'شقراء', N'Shaqraa', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (73, N'بريدة ‏', N'Buraydaah', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (74, N'الخفجي', N'Khafji', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (75, N'المدينة المنورة', N'Medina', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (76, N'ابو ظبى', N'Abu Dhabi', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (77, N'الشارقة', N'Sharjah', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (78, N'الرويس', N'الرويس', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (79, N'الفجيرة', N'الفجيرة', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (80, N'أم القيوين', N'أم القيوين', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (81, N'جزيرة الحمراء', N'Jazirat Al Hamra', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (82, N'خور فكان', N'خورفكان', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (83, N'دبا', N'دبا', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (84, N'دبى', N'إمارة دبي', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (85, N'رأس الخيمة', N'رأس الخيمة', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (86, N'صير بني ياس', N'Sir Banyas Island', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (87, N'عجمان', N'عجمان', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (88, N'غليلة', N'Ghalilah', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (89, N'ليوا', N'Liwa', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (90, N'العين', N'Al Ain', (select id from countries where namearabic = N'الإمارات العربية المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (91, N'الزرقاء', N'Zarqa', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (92, N'الأزرق', N'الأزرق', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (93, N'البتراء', N'البتراء', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (94, N'الرمثا', N'نادي الرمثا', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (95, N'السلط', N'السلط', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (96, N'العقبة', N'Al'' Aqabah', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (97, N'الفحيص', N'الفحيص', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (98, N'القويسمة', N'القويسمة', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (99, N'الكرك', N'الكرك', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (100, N'إربد', N'إربد', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (101, N'جحفية', N'جحفيه', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (102, N'جرش', N'Jarash', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (103, N'خريبة السوق', N'Khirbat as Souk', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (104, N'سحاب', N'Sahab', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (105, N'عجلون', N'عجلون', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (106, N'كفرنجة', N'kufranja', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (107, N'مادبا', N'مادبا', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (108, N'عمان', N'Amman', (select id from countries where namearabic = N'الأردن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (109, N'المنامة', N'Manama', (select id from countries where namearabic = N'البحرين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (110, N'البديع', N'البديع (قرية)', (select id from countries where namearabic = N'البحرين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (111, N'الرفاع الغربي', N'الرفاع', (select id from countries where namearabic = N'البحرين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (112, N'المالكية', N'المالكية', (select id from countries where namearabic = N'البحرين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (113, N'المحرق', N'المحرق', (select id from countries where namearabic = N'البحرين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (114, N'جد حفص', N'جد حفص', (select id from countries where namearabic = N'البحرين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (115, N'سترة', N'سترة', (select id from countries where namearabic = N'البحرين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (116, N'عالي', N'علي بن أبي طالب', (select id from countries where namearabic = N'البحرين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (117, N'قرية سار', N'Sar', (select id from countries where namearabic = N'البحرين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (118, N'مدينة حمد', N'مدينة حمد', (select id from countries where namearabic = N'البحرين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (119, N'مدينة عيسى', N'مدينة عيسى', (select id from countries where namearabic = N'البحرين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (120, N'الأغواط', N'الأغواط', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (121, N'البليدة', N'البليدة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (122, N'الجزائر', N'الجزائر (مدينة)', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (123, N'الجلفة', N'الجلفة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (124, N'الخروب', N'الخروب', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (125, N'الشلف', N'الشلف', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (126, N'العلمة', N'El Eulma-Snt. A.', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (127, N'المدية', N'المدية', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (128, N'المسيلة', N'المسيلة (الجزائر)', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (129, N'الوادي', N'الوادي', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (130, N'باب الزوار', N'باب الزوار', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (131, N'باتنة', N'Batna', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (132, N'بجاية', N'بجاية', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (133, N'براقي', N'Baraki', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (134, N'برج الكيفان', N'برج الكيفان', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (135, N'برج بو عريريج', N'برج بوعريريج', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (136, N'بريكة', N'بريكة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (137, N'بسكرة', N'بسكرة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (138, N'بشار', N'بشار', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (139, N'بو سعادة', N'بوسعادة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (140, N'تبسة', N'تبسة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (141, N'تقورت', N'تقرت', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (142, N'تلمسان', N'تلمسان', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (143, N'تيارت', N'تيارت', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (144, N'جيجل', N'جيجل', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (145, N'خنشلة', N'ولاية خنشلة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (146, N'سطيف', N'سطيف', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (147, N'سعيدة', N'صيدا', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (148, N'سكيكدة', N'سكيكدة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (149, N'سوق أهراس', N'سوق أهراس', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (150, N'سيدي بلعباس', N'سيدي بلعباس', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (151, N'شلالة العذاورة', N'شلالة العذاورة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (152, N'عنابة', N'عنابة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (153, N'عين اعبيد', N'Ain Abid', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (154, N'عين البيضاء', N'عين البيضاء', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (155, N'عين وسارة', N'عين وسارة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (156, N'غرداية', N'غرداية', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (157, N'غليزان', N'غليزان', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (158, N'قالمة', N'قالمة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (159, N'قسنطينة', N'قسطنطين', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (160, N'مسعد', N'مسعد', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (161, N'ورقلة', N'ورقلة', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (162, N'وهران', N'وهران', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (163, N'الجزائر العاصمة', N'Algiers', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (164, N'مستغانم', N'Mostaganem', (select id from countries where namearabic = N'الجزائر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (165, N'الخرطوم', N'Khartoum', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (166, N'الأبيض', N'الأبيض (مدينة)', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (167, N'الجنينة', N'al-Dschunaina', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (168, N'الدامر', N'الدامر', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (169, N'الدويم', N'الدويم', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (170, N'الضعين', N'ad-Du''ain', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (171, N'الفاشر', N'الفاشر', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (172, N'القضارف', N'القضارف (مدينة)', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (173, N'المناقل', N'al-Managil', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (174, N'النهود', N'النهود', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (175, N'أم درمان', N'أم درمان', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (176, N'أم رواب', N'Umm Rawaba', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (177, N'بورتسودان', N'Port-Soudan', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (178, N'حلفا الجديدة', N'مشروع حلفا الجديدة الزراعي', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (179, N'دمازين', N'الدمازين', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (180, N'ربك', N'ربك', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (181, N'سنار', N'سنار', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (182, N'سنجة', N'Sindscha', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (183, N'شندي', N'شندي', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (184, N'عطبرة', N'عطبرة', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (185, N'كادوقلي', N'كادوقلي', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (186, N'كسلا', N'كسلا', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (187, N'كوستي', N'Kusti', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (188, N'ملكال', N'ملكال', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (189, N'نيالا', N'نيالا', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (190, N'وادي حلفا', N'وادي حلفا', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (191, N'واو', N'Waw', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (192, N'ودمدني', N'ود مدني', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (193, N'يامبيو', N'يامبيو', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (194, N'يي', N'ياي', (select id from countries where namearabic = N'السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (195, N'اوبيو', N'Hoby', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (196, N'بربرة', N'بربرة', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (197, N'برديرا', N'Baardheere', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (198, N'بليث وين', N'بلد وين', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (199, N'بورو', N'Borco', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (200, N'جاردو', N'Gardho', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (201, N'جالكاسيو', N'جالكعيو', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (202, N'جلب', N'Gilib', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (203, N'غارو', N'Garoowe', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (204, N'كسماوي', N'كيسمايو', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (205, N'لوك', N'luuq', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (206, N'مقديشو', N'مقديشو', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (207, N'هرجيسة', N'هرجيسا', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (208, N'مقديشيو', N'Mogadishu', (select id from countries where namearabic = N'الصومال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (209, N'بغداد', N'Baghdad', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (210, N'البصرة', N'البصرة', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (211, N'البغدادي', N'بغدادي', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (212, N'التاجي', N'التاجي', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (213, N'الحلة', N'الحلة', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (214, N'الدور', N'الدور', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (215, N'الديوانية', N'الديوانية', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (216, N'الرطبة', N'الرطبة', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (217, N'الرمادي', N'الرمادي', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (218, N'السليمانية', N'السليمانية', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (219, N'السماوة', N'السماوة', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (220, N'الشامية', N'الشامية', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (221, N'العمارة', N'العمارة', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (222, N'العوجا', N'العوجا', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (223, N'الفلوجة', N'الفلوجة', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (224, N'القائم', N'Al-Qa''im', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (225, N'الكاظمية', N'الكاظمية', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (226, N'الكوت', N'الكوت', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (227, N'المقدادية', N'المقدادية', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (228, N'المنطقة الخضراء', N'المنطقة الخضراء', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (229, N'الموصل', N'الموصل', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (230, N'الناصرية', N'الناصرية', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (231, N'النجف', N'النجف', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (232, N'أربيل', N'أربيل', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (233, N'أم قصر', N'أم قصر', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (234, N'بلد', N'بلد', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (235, N'بيجي', N'Baiji', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (236, N'تكريت', N'تكريت', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (237, N'تل كيف', N'تلكيف', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (238, N'تلعفر', N'تلعفر', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (239, N'حديثة', N'حديثة', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (240, N'حلبجة', N'حلبجة', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (241, N'خانقين', N'خانقين', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (242, N'دهوك', N'دهوك', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (243, N'زاخو', N'زاخو', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (244, N'سامراء', N'سامراء', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (245, N'عفك', N'عفك', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (246, N'عقوبة', N'بعقوبة', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (247, N'كركوك', N'كركوك', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (248, N'مدينة الصدر', N'الصدر', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (249, N'هيت', N'هيت', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (250, N'كربلاء', N'Karbala', (select id from countries where namearabic = N'العراق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (251, N'الفحيحيل', N'الفحيحيل', (select id from countries where namearabic = N'الكويت'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (252, N'قرية الجهراء', N'الجهراء', (select id from countries where namearabic = N'الكويت'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (253, N'مدينة الحرير', N'مدينة الحرير', (select id from countries where namearabic = N'الكويت'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (254, N'مدينة الكويت', N'مدينة الكويت', (select id from countries where namearabic = N'الكويت'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (255, N'الكويت', N'Kuwait', (select id from countries where namearabic = N'الكويت'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (256, N'الرِباط', N'Rabat', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (257, N'فاس', N'Fes', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (258, N'القنيطرة', N'Kenitra', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (259, N'الجديدة', N'الجديدة (المغرب)', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (260, N'الحسيمة', N'الحسيمة', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (261, N'الخميسات', N'الخميسات', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (262, N'الدار البيضاء', N'الدار البيضاء', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (263, N'الراشيدية', N'الرشيدية', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (264, N'الرباط', N'رباط (مدينة)', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (265, N'السبت اولاد النمة', N'Sebt Oulad Nemma', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (266, N'الصويرة', N'الصويرة', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (267, N'العرائش', N'العرائش', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (268, N'العيون', N'العيون', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (269, N'الفقيه بنصالح', N'Faki ben-Sala', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (270, N'الفنيدق', N'الفنيدق', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (271, N'القصر الكبير', N'القصر الكبير', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (272, N'المحمدية', N'المحمدية', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (273, N'الناظور', N'الناظور (المغرب)', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (274, N'اليوسفية', N'إقليم اليوسفية', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (275, N'اولاد التايمة', N'Oulad-Teïma', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (276, N'أسفي', N'Safi', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (277, N'أغادير', N'أغادير', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (278, N'برشيد', N'برشيد', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (279, N'بركان', N'بركان', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (280, N'بنجرير', N'بن جرير', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (281, N'بني ملال', N'بني ملال', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (282, N'تارودانت', N'تارودانت', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (283, N'تازة', N'تازة', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (284, N'تزنيت', N'تزنيت', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (285, N'تطوان', N'تطوان', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (286, N'توريرت', N'Taourirt', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (287, N'تيفلت', N'تيفلت', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (288, N'جرسيف', N'جرسيف', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (289, N'خريبكة', N'خريبكة', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (290, N'خنيفرة', N'خنيفرة', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (291, N'سطات', N'سطات', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (292, N'سيدي سليمان', N'Sidi Slimane', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (293, N'سيدي قاسم', N'سيدي قاسم', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (294, N'صفرو', N'صفرو', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (295, N'طانطان', N'طانطان', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (296, N'طنجة', N'طنجة', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (297, N'قلعة السراغنة', N'El-Kelâa-des-Sraghna', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (298, N'كلميم', N'كلميم', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (299, N'مراكش', N'مراكش', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (300, N'مكناس', N'مكناس', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (301, N'وادي زم', N'وادي زم', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (302, N'وجدة', N'وجدة', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (303, N'ورزازات', N'ورززات', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (304, N'وزان', N'وزان', (select id from countries where namearabic = N'المغرب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (305, N'ابها', N'أبها', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (306, N'الباحة', N'الباحة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (307, N'الجبيل', N'محافظة الجبيل', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (308, N'الجفر', N'Al Jafer', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (309, N'الجوف', N'الجوف', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (310, N'الحجرة', N'الحجرة', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (311, N'الحريق', N'مـحافظة الحريق', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (312, N'الخبر', N'محافظة الخبر', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (313, N'الخرج', N'الخرج', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (314, N'الدرعية', N'الدرعية', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (315, N'الدمام', N'الدمام', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (316, N'الدوادمي', N'الدوادمي', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (317, N'الرس', N'الرس', (select id from countries where namearabic = N'المملكة العربية السعودية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (318, N'صنعاء', N'Sanaa', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (319, N'البيضاء', N'البيضاء', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (320, N'الحديدة', N'الحديدة', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (321, N'الشحر', N'ash-Shir', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (322, N'المراوعة', N'al-Marawi''a', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (323, N'المكلا', N'المكلا', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (324, N'إب', N'إب', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (325, N'باجل', N'bajil', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (326, N'بيت الفقيه', N'بيت الفقية', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (327, N'تعز', N'تعز', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (328, N'جعار', N'Chanffar', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (329, N'حجة', N'حجة', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (330, N'ذمار', N'Dhamar', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (331, N'ذي سفال', N'Dhi Sufal', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (332, N'رداع', N'رضا', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (333, N'زبيد', N'زبيد (مدينة)', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (334, N'سحار', N'Sahar', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (335, N'سيان', N'سيان', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (336, N'عدن', N'عدن', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (337, N'عمران', N'Amran', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (338, N'لحج', N'الحوطة', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (339, N'يريم', N'يريم', (select id from countries where namearabic = N'اليمن'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (340, N'القيروان', N'القيروان', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (341, N'المرسى', N'المرسى', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (342, N'المنستير', N'المنستير', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (343, N'بن عروس', N'بن عروس', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (344, N'بنزرت', N'بنزرت', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (345, N'سوسه', N'سوسة', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (346, N'صفاقس', N'صفاقس', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (347, N'قابس', N'قابس', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (348, N'قصرين', N'القصرين', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (349, N'مدنين', N'مدنين', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (350, N'نابل', N'نابل', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (351, N'تونس‎', N'Tunis', (select id from countries where namearabic = N'تونس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (352, N'جيبوتى العاصمة', N'Djibouti', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (353, N'أرتا', N'Arta', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (354, N'أسيلا', N'As Ela', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (355, N'أوبوك', N'أوبوك', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (356, N'بالهو', N'Balho', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (357, N'جلافي', N'غالافي', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (358, N'جيبوتي', N'جيبوتي', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (359, N'خور أنجار', N'Khor `Angar', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (360, N'دخيل', N'دخيل', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (361, N'دورا', N'درة', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (362, N'رندا', N'Randa', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (363, N'علي صبيح', N'علي صبيح', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (364, N'هولهول', N'هلهول', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (365, N'يوبوكي', N'يوبوكي', (select id from countries where namearabic = N'جيبوتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (366, N'ارمناز', N'ARMANAZ', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (367, N'ازرع', N'AZRAA', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (368, N'الأتارب', N'ALATARB', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (369, N'الباب', N'الباب (سوريا)', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (370, N'التل', N'At-Tall', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (371, N'الثورة', N'الطبقة (الرقة)', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (372, N'الحسكة', N'الحسكة', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (373, N'الديماس', N'al dimas', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (374, N'الرستن', N'الرستن', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (375, N'الرقة', N'الرقة', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (376, N'السفيرة', N'السفيرة', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (377, N'السقيلبية', N'ALSEKELBIAH', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (378, N'السويداء', N'سويداء', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (379, N'القامشلي', N'القامشلي', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (380, N'القحطانية', N'ALKahtanieah', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (381, N'القدموس', N'Al kadmus', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (382, N'القرداحة', N'AL Kardaha', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (383, N'القطيفة', N'Qatifh', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (384, N'الكسوة', N'AL kaswah', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (385, N'اللاذقية', N'اللاذقية', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (386, N'الميادين', N'الميادين', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (387, N'النبك', N'النبك', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (388, N'إدلب', N'إدلب', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (389, N'أبو كمال', N'البوكمال', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (390, N'أريحا', N'أريحا', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (391, N'أعزاز', N'أعزاز', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (392, N'بانياس', N'بانياس', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (393, N'تدمر', N'تدمر', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (394, N'تل رفعت', N'Tel reafat', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (395, N'تلكلخ', N'Tal kalak', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (396, N'جبلة', N'جبلة', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (397, N'جرابلس', N'جرابلس', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (398, N'جسر الشغور', N'JAESR ALSHAGUR', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (399, N'حارم', N'حرملك', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (400, N'حماة', N'حماة', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (401, N'حمص', N'حمص', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (402, N'خان شيخون', N'خان شيخون', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (403, N'داريا', N'داريا', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (404, N'درعا', N'Dar''a', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (405, N'دوما', N'دوما', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (406, N'دير الزور', N'دير الزور', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (407, N'دير عطية', N'Deer Atiah', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (408, N'راس العين', N'Ras Alaen', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (409, N'سلمية', N'سلمية', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (410, N'شهبا', N'شهبا', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (411, N'صافيتا', N'SAFITAH', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (412, N'صلخد', N'SALKED', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (413, N'صلنفة', N'SALNFAH', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (414, N'صوران', N'SURAN', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (415, N'طرطوس', N'طرطوس', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (416, N'طيبة الامام', N'Teabt Alamam', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (417, N'عامودا', N'AMUDAH', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (418, N'عفرين', N'Afrin', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (419, N'عين العرب', N'عين العرب', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (420, N'مصياف', N'MSIAF', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (421, N'معرة النعمان', N'معرة النعمان', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (422, N'منبج', N'منبج', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (423, N'نوى', N'Nawa', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (424, N'يبرود', N'YABRUD', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (425, N'دمشق', N'Damascus', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (426, N'حلب', N'Aleppo', (select id from countries where namearabic = N'سوريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (427, N'مسقط', N'Muscat', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (428, N'ابراء', N'ولاية إبراء', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (429, N'البريمي', N'البريمي', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (430, N'الحمرا', N'Al Hamra', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (431, N'الدقم', N'الدقم', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (432, N'الرستاق', N'ولاية الرستاق', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (433, N'السويق', N'ولاية السويق', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (434, N'السيب', N'ولاية السيب', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (435, N'إزكي', N'ولاية إزكي', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (436, N'أدم', N'آدم', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (437, N'بدبد', N'ولاية بدبد', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (438, N'بركاء', N'Barka', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (439, N'بهلا', N'بهلا', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (440, N'خصب', N'ولاية خصب', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (441, N'ريسوت', N'Raysut', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (442, N'سمائل', N'ولاية سمائل', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (443, N'شناص', N'ولاية شناص', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (444, N'صحار', N'ولاية صحار', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (445, N'صحم', N'ولاية صحم', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (446, N'صلالة', N'صلالة', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (447, N'صور', N'Sur', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (448, N'عبري', N'عبري', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (449, N'قريات', N'Quriyat', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (450, N'مدحاء', N'ولاية مدحاء', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (451, N'مصيرة', N'ولاية مصيرة', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (452, N'مطرح', N'ولاية مطرح', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (453, N'منح', N'Manah', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (454, N'نزوى', N'ولاية نزوى', (select id from countries where namearabic = N'عمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (455, N'البيرة', N'البيرة', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (456, N'الجش', N'الجش', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (457, N'الخليل', N'الخليل', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (458, N'الرملة', N'الرملة', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (459, N'اللد', N'اللد', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (460, N'الناصرة', N'الناصرة', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (461, N'الناعورة', N'Naorah', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (462, N'أم الفحم', N'أم الفحم', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (463, N'باقة الغربية', N'باقة الغربية', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (464, N'بيت حانون', N'بيت حانون', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (465, N'بيت لاهيا', N'بيت لاهيا', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (466, N'بيت لحم', N'بيت لحم', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (467, N'جباليا', N'جباليا', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (468, N'جت', N'Jet', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (469, N'جنين', N'جنين', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (470, N'حيفا', N'حيفا', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (471, N'خان يونس', N'خان يونس', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (472, N'دير البلح', N'دير البلح', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (473, N'رفح', N'رفح', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (474, N'سخنين', N'سخنين', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (475, N'سلفيت', N'سلفيت', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (476, N'شفاعمرو', N'شفا عمرو', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (477, N'طرعان', N'Turaan', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (478, N'طوباس', N'طوباس', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (479, N'طولكرم', N'طولكرم', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (480, N'عكا', N'آكر', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (481, N'قلقيلية', N'قلقيلية', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (482, N'كفر قاسم', N'كفر قاسم', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (483, N'كفر كنا', N'كفر كنا', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (484, N'يافا', N'يافا', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (485, N'غزة', N'Gaza', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (486, N'رام الله', N'Ramallah', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (487, N'نابلس', N'Nablus', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (488, N'القدس', N'Jerusalem', (select id from countries where namearabic = N'فلسطين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (489, N'الدوحة', N'Doha', (select id from countries where namearabic = N'قطر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (490, N'الوكرة', N'الوكرة', (select id from countries where namearabic = N'قطر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (491, N'دخان', N'دخان', (select id from countries where namearabic = N'قطر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (492, N'رأس لفان', N'رأس لفان', (select id from countries where namearabic = N'قطر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (493, N'ريان', N'الريان', (select id from countries where namearabic = N'قطر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (494, N'لؤلؤة قطر', N'مشروع اللؤلؤة', (select id from countries where namearabic = N'قطر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (495, N'مسيعيد', N'Ümm Said', (select id from countries where namearabic = N'قطر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (496, N'الدامور', N'الدامور', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (497, N'الشوف', N'Alshouf', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (498, N'النبطية', N'النبطية', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (499, N'بعبدا', N'بعبدا', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (500, N'بعلبك', N'Baalbak', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (501, N'بنت جبيل', N'بنت جبيل', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (502, N'جزين', N'جزين', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (503, N'جونية', N'Junyiah', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (504, N'حلبا', N'حلبا', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (505, N'دير القمر', N'Deer El Qamar', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (506, N'زحلة', N'زحلة', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (507, N'شمسطار', N'Shamustar', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (508, N'كفرشوبا', N'kfarshouba', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (509, N'بيروت', N'Beirut', (select id from countries where namearabic = N'لبنان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (510, N'طرابلس', N'Tripoli', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (511, N'الخمس', N'الخمس', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (512, N'الزاوية', N'Al Zawia', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (513, N'العزيزية', N'العزيزية (ليبيا)', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (514, N'القبة', N'القبة', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (515, N'المرج', N'المرج', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (516, N'إجدابيا', N'أجدابيا', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (517, N'بنغازي', N'بنغازي', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (518, N'بني وليد', N'بني وليد', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (519, N'ترهونة', N'ترهونة', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (520, N'درنة', N'Darnah', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (521, N'زليطن', N'زليتن', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (522, N'زوارة', N'زوارة', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (523, N'سبها', N'Sabha', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (524, N'سرت', N'سرت', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (525, N'صبراتة', N'صبراتة', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (526, N'صرمان', N'صرمان', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (527, N'طبرق', N'طبرق', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (528, N'غريان', N'Ghiryen', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (529, N'مرزق', N'مرزق', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (530, N'مصراتة', N'مصراتة', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (531, N'نالوت', N'نالوت', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (532, N'يفرن', N'يفرن', (select id from countries where namearabic = N'ليبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (533, N'الاسماعيلية', N'Al Isma`iliyah', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (534, N'الأقصر', N'الأقصر', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (535, N'الحوامدية', N'Alhawamdeya', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (536, N'الزقازيق', N'الزقازيق', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (537, N'السويس', N'السويس', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (538, N'العريش', N'Al `Arish', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (539, N'الغردقة', N'الغردقة', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (540, N'الفيوم', N'الفيوم', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (541, N'المحلة الكبرى', N'Al Mahallah al Kubra', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (542, N'المطرية', N'Al Matariyyah', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (543, N'المنصورة', N'المنصورة', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (544, N'إدفو', N'إدفو', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (545, N'أبو كبير', N'Abu Kabeer', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (546, N'أخميم', N'أخميم', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (547, N'أسوان', N'أسوان', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (548, N'أسيوط', N'أسيوط', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (549, N'بلبيس', N'بلبيس', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (550, N'بور سعيد', N'Bur Said', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (551, N'جرجا', N'جرجا', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (552, N'دسوق', N'دسوق', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (553, N'دمنهور', N'دمنهور', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (554, N'دمياط', N'دمياط', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (555, N'سوهاج', N'سوهاج', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (556, N'شبرا الخيمة', N'شبرا الخيمة', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (557, N'شبين الكوم', N'شبين الكوم', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (558, N'عشرة رمضان', N'city 10th Of Ramadan', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (559, N'قليوب', N'قليوب', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (560, N'قنا', N'قنا', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (561, N'كفر الدوار', N'كفر الدوار', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (562, N'كفر الشيخ', N'كفر الشيخ', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (563, N'مدينة ستة أكتوبر', N'City 6 October', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (564, N'مرسى مطروح', N'مرسى مطروح', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (565, N'ملوى', N'ملوي', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (566, N'ميت غمر', N'ميت غمر', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (567, N'القاهرة', N'Cairo', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (568, N'الجيزة', N'الجيزة', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (569, N'طنطا', N'Tanta', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (570, N'بنها', N'Banha', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (571, N'بني سويف', N'Beni Suef', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (572, N'المنيا', N'Minya', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (573, N'الإسكندرية', N'Alexandria', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (574, N'فايد', N'Fayed', (select id from countries where namearabic = N'مصر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (575, N'نواكشوط', N'Nouakchott', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (576, N'ازويرات', N'الزويرات', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (577, N'اكجوجت', N'أكجوجت', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (578, N'الطينطان', N'طينطان', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (579, N'النعمة', N'النعمة', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (580, N'انواديبو', N'نواذيبو', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (581, N'أطار', N'Atar', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (582, N'آلاك', N'ألاك', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (583, N'بابابى', N'Bababé', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (584, N'بوتلميت', N'بوتلميت', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (585, N'بوكادوم', N'Boû Gâdoûm', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (586, N'بوكى', N'Bogué', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (587, N'تجكجة', N'تجكجة', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (588, N'تنبدغة', N'تمبدغة', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (589, N'روصو', N'روصو', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (590, N'سيلبابي', N'سيليبابي', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (591, N'عدل بكرو', N'عدل بكرو', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (592, N'كرو', N'Guérou', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (593, N'كيفة', N'كيفة', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (594, N'كيهيدي', N'كيهيدي', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (595, N'مال', N'Mâl', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (596, N'مقطع لحجار', N'Magta Lahjar', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (597, N'ولاتة', N'ولاتة', (select id from countries where namearabic = N'موريتانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (598, N'كابول', N'Kabul', (select id from countries where namearabic = N'أفغانستان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (599, N'تيرانا', N'Tirana', (select id from countries where namearabic = N'ألبانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (600, N'اندورا لا فيلا', N'Andorra la Vella', (select id from countries where namearabic = N'أندورا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (601, N'لواندا', N'Luanda', (select id from countries where namearabic = N'أنغولا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (602, N'سان جونز', N'Saint John''s', (select id from countries where namearabic = N'أنتيغوا وبربودا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (603, N'بوينس آيرس', N'Buenos Aires', (select id from countries where namearabic = N'الأرجنتين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (604, N'يريفان', N'Yerevan', (select id from countries where namearabic = N'أرمينيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (605, N'أورنجستاد', N'Oranjestad', (select id from countries where namearabic = N'أروبا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (606, N'كانبرا', N'Canberra', (select id from countries where namearabic = N'أستراليا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (607, N'فيينا', N'Vienna', (select id from countries where namearabic = N'النمسا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (608, N'باكو', N'Baku', (select id from countries where namearabic = N'أذربيجان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (609, N'ناساو', N'Nassau', (select id from countries where namearabic = N'الباهاماس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (610, N'دكا', N'Dhaka', (select id from countries where namearabic = N'بنغلاديش'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (611, N'بريدجتاون', N'Bridgetown', (select id from countries where namearabic = N'بربادوس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (612, N'مينسك', N'Minsk', (select id from countries where namearabic = N'روسيا البيضاء'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (613, N'بروكسل', N'Brussels', (select id from countries where namearabic = N'بلجيكا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (614, N'بلموبان', N'Belmopan', (select id from countries where namearabic = N'بيليز'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (615, N'بورتو نوفو', N'Porto-Novo', (select id from countries where namearabic = N'بنين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (616, N'هاميلتون', N'Hamilton', (select id from countries where namearabic = N'جزر برمود'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (617, N'ثيمفو', N'Thimphu', (select id from countries where namearabic = N'بوتان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (618, N'لاباز', N'La Paz', (select id from countries where namearabic = N'بوليفيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (619, N'سراييفو', N'Sarajevo', (select id from countries where namearabic = N'البوسنة و الهرسك'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (620, N'جابورون', N'Gaborone', (select id from countries where namearabic = N'بوتسوانا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (621, N'برازيليا', N'Brasilia', (select id from countries where namearabic = N'البرازيل'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (622, N'بندر سرى بيجاوان', N'Bandar Seri Begawan', (select id from countries where namearabic = N'بروناي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (623, N'صوفيا', N'Sofia', (select id from countries where namearabic = N'بلغاريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (624, N'واجادوجو', N'Ouagadougou', (select id from countries where namearabic = N'بوركينا فاسو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (625, N'بوجومبورا', N'Bujumbura', (select id from countries where namearabic = N'بوروندي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (626, N'بنوم بنـه', N'Phnom Penh', (select id from countries where namearabic = N'كمبوديا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (627, N'ياوندى', N'Yaoundé', (select id from countries where namearabic = N'كاميرون'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (628, N'أوتاوا', N'Ottawa', (select id from countries where namearabic = N'كندا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (629, N'برايا', N'Praia', (select id from countries where namearabic = N'الرأس الأخضر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (630, N'بانجى', N'Bangui', (select id from countries where namearabic = N'جمهورية أفريقيا الوسطى'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (631, N'نجامينا', N'N''Djamena', (select id from countries where namearabic = N'تشاد'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (632, N'سانتياجو', N'Santiago', (select id from countries where namearabic = N'تشيلي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (633, N'بكين', N'Beijing', (select id from countries where namearabic = N'جمهورية الصين الشعبية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (634, N'بوجوتا', N'Bogotá', (select id from countries where namearabic = N'كولومبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (635, N'مورونى', N'Moroni', (select id from countries where namearabic = N'جزر القمر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (636, N'برازافيل', N'Brazzaville', (select id from countries where namearabic = N'جمهورية الكونغو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (637, N'سان خوسيه', N'San José', (select id from countries where namearabic = N'كوستاريكا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (638, N'ياموسوكرو', N'Yamoussoukro', (select id from countries where namearabic = N'ساحل العاج'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (639, N'زغرب', N'Zagreb', (select id from countries where namearabic = N'كرواتيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (640, N'هافانا', N'Havana', (select id from countries where namearabic = N'كوبا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (641, N'نيقوسيا', N'Nicosia', (select id from countries where namearabic = N'قبرص'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (642, N'براغ', N'Prague', (select id from countries where namearabic = N'الجمهورية التشيكية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (643, N'كوبنهاجن', N'Copenhagen', (select id from countries where namearabic = N'الدانمارك'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (644, N'روسو', N'Roseau', (select id from countries where namearabic = N'دومينيكا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (645, N'سان دومينجو', N'Santo Domingo', (select id from countries where namearabic = N'الجمهورية الدومينيكية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (646, N'ديلي', N'Dili', (select id from countries where namearabic = N'تيمور الشرقية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (647, N'كيتو', N'Quito', (select id from countries where namearabic = N'إكوادور'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (648, N'سان سلفادور', N'San Salvador', (select id from countries where namearabic = N'إلسلفادور'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (649, N'ملابو', N'Malabo', (select id from countries where namearabic = N'غينيا الاستوائي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (650, N'اسمرة', N'Asmara', (select id from countries where namearabic = N'إريتريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (651, N'تالين', N'Tallinn', (select id from countries where namearabic = N'استونيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (652, N'اديس ابابا', N'Addis Ababa', (select id from countries where namearabic = N'أثيوبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (653, N'تورشافن', N'Tórshavn', (select id from countries where namearabic = N'جزر فارو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (654, N'سوفا', N'Suva', (select id from countries where namearabic = N'فيجي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (655, N'هلسنكى', N'Helsinki', (select id from countries where namearabic = N'فنلندا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (656, N'باريس', N'Paris', (select id from countries where namearabic = N'فرنسا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (657, N'كايين', N'Cayenne', (select id from countries where namearabic = N'غويانا الفرنسية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (658, N'بابيتي', N'Papeete', (select id from countries where namearabic = N'بولينيزيا الفرنسية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (659, N'ليبرفيل', N'Libreville', (select id from countries where namearabic = N'الغابون'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (660, N'بانجول', N'Banjul', (select id from countries where namearabic = N'غامبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (661, N'تبِليسى', N'T''bilisi', (select id from countries where namearabic = N'جيورجيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (662, N'برلين', N'Berlin', (select id from countries where namearabic = N'ألمانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (663, N'اكرا', N'Accra', (select id from countries where namearabic = N'غانا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (664, N'أثينا', N'Athens', (select id from countries where namearabic = N'اليونان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (665, N'نوك', N'Nuuk', (select id from countries where namearabic = N'جرينلاند'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (666, N'سان جورج', N'Saint George''s', (select id from countries where namearabic = N'غرينادا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (667, N'باس تير', N'Basse-Terre', (select id from countries where namearabic = N'جزر جوادلوب'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (668, N'جواتيمالا سيتى', N'Guatemala', (select id from countries where namearabic = N'غواتيمال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (669, N'كوناكرى', N'Conakry', (select id from countries where namearabic = N'غينيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (670, N'بيساو', N'Bissau', (select id from countries where namearabic = N'غينيا-بيساو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (671, N'جورج تاون', N'Georgetown', (select id from countries where namearabic = N'غيانا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (672, N'بورت أو برنس', N'Port-au-Prince', (select id from countries where namearabic = N'هايتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (673, N'تيجوسيجالبا', N'Tegucigalpa', (select id from countries where namearabic = N'هندوراس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (674, N'بودابست', N'Budapest', (select id from countries where namearabic = N'المجر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (675, N'ريكيافيك', N'Reykjavik', (select id from countries where namearabic = N'آيسلندا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (676, N'نيودلهي', N'New Delhi', (select id from countries where namearabic = N'الهند'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (677, N'جاكارتا', N'Jakarta', (select id from countries where namearabic = N'أندونيسيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (678, N'طهران', N'Tehran', (select id from countries where namearabic = N'إيران'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (679, N'دبلن', N'Dublin', (select id from countries where namearabic = N'جمهورية أيرلندا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (680, N'روما', N'Rome', (select id from countries where namearabic = N'إيطاليا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (681, N'كينجستون', N'Kingston', (select id from countries where namearabic = N'جمايكا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (682, N'طوكيو', N'Tokyo', (select id from countries where namearabic = N'اليابان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (683, N'أستانا', N'Astana', (select id from countries where namearabic = N'كازاخستان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (684, N'نيروبى', N'Nairobi', (select id from countries where namearabic = N'كينيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (685, N'تاراوا', N'Tarawa', (select id from countries where namearabic = N'كيريباتي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (686, N'بيونغيانغ', N'P''yongyang', (select id from countries where namearabic = N'كوريا الشمالية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (687, N'سيئول', N'Seoul', (select id from countries where namearabic = N'كوريا الجنوبية'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (688, N'بيشكك', N'Bishkek', (select id from countries where namearabic = N'قيرغيزستان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (689, N'فينتيان', N'Vientiane', (select id from countries where namearabic = N'لاوس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (690, N'ريجا', N'Riga', (select id from countries where namearabic = N'لاتفيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (691, N'ماسيرو', N'Maseru', (select id from countries where namearabic = N'ليسوتو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (692, N'منروفيا', N'Monrovia', (select id from countries where namearabic = N'ليبيريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (693, N'فادوز', N'Vaduz', (select id from countries where namearabic = N'ليختنشتين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (694, N'فيلنيوس', N'Vilnius', (select id from countries where namearabic = N'لتوانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (695, N'لوكسمبورج', N'Luxembourg', (select id from countries where namearabic = N'لوكسمبورغ'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (696, N'سكوبيه', N'Skopje', (select id from countries where namearabic = N'جمهورية مقدونيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (697, N'أنتاناناريفو', N'Antananarivo', (select id from countries where namearabic = N'مدغشقر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (698, N'ليلونغوي', N'Lilongwe', (select id from countries where namearabic = N'مالاوي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (699, N'كوالا لامبور', N'Kuala Lumpur', (select id from countries where namearabic = N'ماليزيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (700, N'ماليه', N'Male', (select id from countries where namearabic = N'المالديف'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (701, N'باماكو', N'Bamako', (select id from countries where namearabic = N'مالي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (702, N'فاليتا', N'Valletta', (select id from countries where namearabic = N'مالطا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (703, N'ماجورو', N'Majuro', (select id from countries where namearabic = N'جزر مارشال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (704, N'فور دو فرانس', N'Fort-de-France', (select id from countries where namearabic = N'مارتينيك'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (705, N'بورت لويس', N'Port Louis', (select id from countries where namearabic = N'موريشيوس'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (706, N'مكسيكو سيتى', N'Mexico City', (select id from countries where namearabic = N'المكسيك'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (707, N'باليكير', N'Palikir', (select id from countries where namearabic = N'ولايات ميكرونيسيا المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (708, N'كيشيناو', N'Chisinau', (select id from countries where namearabic = N'مولدافيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (709, N'موناكو', N'Monaco', (select id from countries where namearabic = N'موناكو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (710, N'اولان باتور', N'Ulaanbaatar', (select id from countries where namearabic = N'منغوليا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (711, N'بودغوريتسا', N'Podgorica', (select id from countries where namearabic = N'الجبل الأسود'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (712, N'مابوتو', N'Maputo', (select id from countries where namearabic = N'موزمبيق'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (713, N'نايبييدو (يانغون)', N'Naypyidaw', (select id from countries where namearabic = N'ميانمار'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (714, N'ويندهوك', N'Windhoek', (select id from countries where namearabic = N'ناميبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (715, N'مقاطعة يارين', N'Yaren District', (select id from countries where namearabic = N'ناورو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (716, N'كاتمندو', N'Kathmandu', (select id from countries where namearabic = N'نيبال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (717, N'امستردام', N'Amsterdam', (select id from countries where namearabic = N'هولندا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (718, N'لايدن', N'Leiden', (select id from countries where namearabic = N'هولندا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (719, N'نوميا', N'Nouméa', (select id from countries where namearabic = N'كاليدونيا الجديدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (720, N'ويلينجتون', N'Wellington', (select id from countries where namearabic = N'نيوزيلندا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (721, N'ماناغوا', N'Managua', (select id from countries where namearabic = N'نيكاراجوا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (722, N'نيامى', N'Niamey', (select id from countries where namearabic = N'النيجر'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (723, N'ابوجا', N'Abuja', (select id from countries where namearabic = N'نيجيريا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (724, N'اوسلو', N'Oslo', (select id from countries where namearabic = N'النرويج'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (725, N'اسلام اباد', N'Islamabad', (select id from countries where namearabic = N'باكستان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (726, N'ميلكيوك', N'Melekeok', (select id from countries where namearabic = N'بالاو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (727, N'بنما ستى', N'Panama (City)', (select id from countries where namearabic = N'بنما'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (728, N'بورت مورزبي', N'Port Moresby', (select id from countries where namearabic = N'بابوا غينيا الجديدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (729, N'اسنسيون', N'Asunción', (select id from countries where namearabic = N'باراغواي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (730, N'ليما', N'Lima', (select id from countries where namearabic = N'بيرو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (731, N'مانيلا', N'Manila', (select id from countries where namearabic = N'الفليبين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (732, N'وارسو', N'Warsaw', (select id from countries where namearabic = N'بولندا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (733, N'لشبونة', N'Lisbon', (select id from countries where namearabic = N'البرتغال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (734, N'سان خوان', N'San Juan', (select id from countries where namearabic = N'بورتوريكو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (735, N'بوخارست', N'Bucharest', (select id from countries where namearabic = N'رومانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (736, N'موسكو', N'Moscow', (select id from countries where namearabic = N'روسيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (737, N'كيجالى', N'Kigali', (select id from countries where namearabic = N'رواندا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (738, N'كاستريس', N'Castries', (select id from countries where namearabic = N'سانت لوسيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (739, N'كينغستاون', N'Kingstown', (select id from countries where namearabic = N'سانت فنسنت وجزر غرينادين'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (740, N'أبيا', N'Apia', (select id from countries where namearabic = N'ساموا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (741, N'سان مارينو', N'San Marino', (select id from countries where namearabic = N'سان مارينو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (742, N'ساو توميه', N'São Tomé', (select id from countries where namearabic = N'ساو تومي وبرينسيبي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (743, N'دكار', N'Dakar', (select id from countries where namearabic = N'السنغال'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (744, N'بلغراد', N'Belgrade', (select id from countries where namearabic = N'جمهورية صربيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (745, N'فيكتوريا', N'Victoria', (select id from countries where namearabic = N'سيشيل'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (746, N'فرى تاون', N'Freetown', (select id from countries where namearabic = N'سيراليون'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (747, N'سنغافورة', N'Singapore', (select id from countries where namearabic = N'سنغافورة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (748, N'براتسلافا', N'Bratislava', (select id from countries where namearabic = N'سلوفاكيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (749, N'لوبليانا', N'Ljubljana', (select id from countries where namearabic = N'سلوفينيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (750, N'هونيارا', N'Honiara', (select id from countries where namearabic = N'جزر سليمان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (751, N'كيب تاون', N'Cape Town', (select id from countries where namearabic = N'جنوب أفريقيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (752, N'جوبا', N'Juba', (select id from countries where namearabic = N'جنوب السودان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (753, N'مدريد', N'Madrid', (select id from countries where namearabic = N'إسبانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (754, N'سري جاياواردنابورا كوتي', N'Sri Jayewardenepura-Kotte; see: Colombo', (select id from countries where namearabic = N'سريلانكا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (755, N'باراماريبو', N'Paramaribo', (select id from countries where namearabic = N'سورينام'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (756, N'مبابان', N'Mbabane (administrative)', (select id from countries where namearabic = N'سوازيلند'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (757, N'استكهولم', N'Stockholm', (select id from countries where namearabic = N'السويد'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (758, N'برن', N'Bern', (select id from countries where namearabic = N'سويسرا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (759, N'جنيف', N'Geneva', (select id from countries where namearabic = N'سويسرا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (760, N'بازل', N'Basel', (select id from countries where namearabic = N'سويسرا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (761, N'تايبيه', N'Taipei', (select id from countries where namearabic = N'تايوان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (762, N'دوشنبه', N'Dushanbe', (select id from countries where namearabic = N'طاجيكستان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (763, N'دار السلام', N'Dar es Salaam', (select id from countries where namearabic = N'تنزانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (764, N'بانكوك', N'Bangkok', (select id from countries where namearabic = N'تايلندا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (765, N'لاسا', N'Lhasa', (select id from countries where namearabic = N'تبت'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (766, N'لومى', N'Lomé', (select id from countries where namearabic = N'توغو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (767, N'نوكوالوفا', N'Nuku''alofa', (select id from countries where namearabic = N'تونغا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (768, N'بورت اوف سبين', N'Port-of-Spain', (select id from countries where namearabic = N'ترينيداد وتوباغو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (769, N'انقرة', N'Ankara', (select id from countries where namearabic = N'تركيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (770, N'عشق آباد', N'Ashgabat', (select id from countries where namearabic = N'تركمانستان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (771, N'فونافوتي', N'Funafuti', (select id from countries where namearabic = N'توفالو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (772, N'كمبالا', N'Kampala', (select id from countries where namearabic = N'أوغندا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (773, N'كييف', N'Kiev', (select id from countries where namearabic = N'أوكرانيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (774, N'لندن', N'London', (select id from countries where namearabic = N'المملكة المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (775, N'واشنطن العاصمة', N'Washington D.C.', (select id from countries where namearabic = N'الولايات المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (776, N'كانساس', N'Kansas', (select id from countries where namearabic = N'الولايات المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (777, N'ميسا', N'Mesa', (select id from countries where namearabic = N'الولايات المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (778, N'Chicago', N'Chicago', (select id from countries where namearabic = N'الولايات المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (779, N'سان فرانسيسكو', N'San Francisco', (select id from countries where namearabic = N'الولايات المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (780, N'بوسطن', N'Boston', (select id from countries where namearabic = N'الولايات المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (781, N'نيويورك', N'New York', (select id from countries where namearabic = N'الولايات المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (782, N'ساراسوتا', N'Sarasota', (select id from countries where namearabic = N'الولايات المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (783, N'سارانك ليك', N'Saranac Lake', (select id from countries where namearabic = N'الولايات المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (784, N'تشامبيغن', N'Champaign', (select id from countries where namearabic = N'الولايات المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (785, N'واشنطن', N'Washington', (select id from countries where namearabic = N'الولايات المتحدة'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (786, N'مونتفيديو', N'Montevideo', (select id from countries where namearabic = N'أورغواي'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (787, N'طشقند', N'Tashkent', (select id from countries where namearabic = N'أوزباكستان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (788, N'بورت فيلا', N'Port-Vila', (select id from countries where namearabic = N'فانواتو'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (789, N'فاتيكان سيتى', N'Vatican City', (select id from countries where namearabic = N'دولة مدينة الفاتيكان'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (790, N'كراكاس', N'Caracas', (select id from countries where namearabic = N'فنزويلا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (791, N'هانوى', N'Hanoi', (select id from countries where namearabic = N'فيتنام'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (792, N'لوساكا', N'Lusaka', (select id from countries where namearabic = N'زامبيا'));
insert into cities (Id, NameArabic, NameEnglish, CountryId) values (793, N'هرارى', N'Harare', (select id from countries where namearabic = N'زمبابوي'));


set identity_insert Cities off
go




IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Referees'
    AND column_name = 'Qualification'
)
BEGIN
	ALTER TABLE Referees add [Qualification] tinyint NULL;

END


IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Referees'
    AND column_name = 'NationalityId'
)
BEGIN
	ALTER TABLE Referees add [NationalityId] int NULL;

	
	
	ALTER TABLE [dbo].[Referees]  WITH CHECK ADD  CONSTRAINT [FK_Referees_Nationality] FOREIGN KEY(NationalityId)
	REFERENCES [dbo].[Countries] ([Id])
	
END



IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'PriorityOther'
)
BEGIN
	ALTER TABLE Requests add [PriorityOther] nvarchar(255) NULL;
	
END



IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'UserSessions'
    AND column_name = 'CaptchaVerified'
)
BEGIN
	ALTER TABLE UserSessions add [CaptchaVerified] bit NULL;
	
END






IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'RequestMaterials'
    AND column_name = 'OtherType'
)
BEGIN
	ALTER TABLE RequestMaterials add OtherType nvarchar(255) NULL;

END



delete from departments where namearabic = N'قسم 1'
delete from departments where namearabic = N'قسم 2'
delete from departments where namearabic = N'قسم 3'

update departments set nameenglish = N'Information Technology' where namearabic = N'تقنية المعلومات';
update departments set nameenglish = N'Information Management' where namearabic = N'إدارة المعلومات';
update departments set nameenglish = N'Family Medicine' where namearabic = N'طب الأسرة';
update departments set nameenglish = N'Ophthalmology' where namearabic = N'العيون';
update departments set nameenglish = N'Pharmaceutical' where namearabic = N'الأدوية';
update departments set nameenglish = N'Physiology' where namearabic = N'وظائف الأعضاء';
update departments set nameenglish = N'Electrical Engineering' where namearabic = N'الهندسة الكهربائية';
update departments set nameenglish = N'Insurance and Risk Management' where namearabic = N'التأمين وإدارة المخاطر';
update departments set nameenglish = N'Computer Science' where namearabic = N'الحاسب الآلي';
update departments set nameenglish = N'English Literature' where namearabic = N'الأدب الإنجليزي';


delete from Departments where NameArabic = N'الأنظمة' and (Select NameArabic from colleges c where c.id = Departments.CollegeId) = N'الشريعة'

go


update departments set nameenglish = N'Fiqh' where namearabic = N'الفقه'
update departments set nameenglish = N'Biochemistry' where namearabic = N'الكيمياء الحيوية'

go


update settings set [value] = N'omar.shaaban@kay.sa' where [key] = N'smtp:EmailCC';
update settings set [value] = N'omar.s.shaaban4@gmail.com' where [key] = N'smtp:FromEmail';
update settings set [value] = N'ARF' where [key] = N'smtp:FromName';
update settings set [value] = N'smtp.gmail.com' where [key] = N'smtp:Host';
update settings set [value] = N'5t6405wV402D1oc' where [key] = N'smtp:Password';
update settings set [value] = N'587' where [key] = N'smtp:Port';
update settings set [value] = N'TRUE' where [key] = N'smtp:SSL';
update settings set [value] = N'omar.s.shaaban4@gmail.com' where [key] = N'smtp:Username';

go


-- Date: 03/14/2018
-- Added New Table ContactedUs
IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ContactedUs]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[ContactedUs](
		[Id] [BIGINT] IDENTITY(1,1) NOT NULL,
		[FullName] [NVARCHAR](255) NOT NULL,
		[EmailAddress] [NVARCHAR](255) NOT NULL,
		[Subject] [NVARCHAR](255) NULL,
		[Message] [NVARCHAR](MAX) NOT NULL,
		[Created] [DATETIME] NOT NULL,
		[IsDeleted] BIT DEFAULT((0)) NOT NULL
	 CONSTRAINT [PK_ContactedUs] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

-- Date: 03/14/2018
-- Added Forieng Key FK_Researchers_Users 
IF NOT EXISTS 
(
    SELECT * FROM sys.foreign_key_columns fk 
    INNER JOIN sys.columns pc ON pc.object_id = fk.parent_object_id AND pc.column_id = fk.parent_column_id 
    INNER JOIN sys.columns rc ON rc.object_id = fk.referenced_object_id AND rc.column_id = fk.referenced_column_id
    WHERE fk.parent_object_id = object_id('Researchers') AND pc.name = 'UserId'
    AND fk.referenced_object_id = object_id('Users') AND rc.NAME = 'Id'
)
ALTER TABLE Researchers 
ADD CONSTRAINT FK_Researchers_Users FOREIGN KEY (UserId)
REFERENCES Users(Id)


update Settings set [Value] = 'ARF System' where [Key] = 'sms:sender:malath'
delete from templates;

go


update Settings set [Value] = 'ARF System' where [Key] = 'smtp:FromName'

go

-- Date: 03/18/2018

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[usersessions]') 
         AND name = 'Id'
)
BEGIN
		DECLARE @table NVARCHAR(512), @sql NVARCHAR(MAX);
		SELECT @table = N'dbo.usersessions';

		SELECT @sql = 'ALTER TABLE ' + @table 
		+ ' DROP CONSTRAINT ' + name + ';'
		FROM sys.key_constraints 
		WHERE [type] = 'PK'
			AND [parent_object_id] = OBJECT_ID(@table);
		EXEC sp_executeSQL @sql;

		ALTER TABLE usersessions ADD Id BIGINT IDENTITY(1,1);
		ALTER TABLE usersessions add primary key (ID)
END


IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'UserSessions'
    AND column_name = 'EmailVerified'
)
BEGIN
	ALTER TABLE UserSessions add EmailVerified nvarchar(255) NULL;

END


IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[UserSessionData]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[UserSessionData](
		[Id] [BIGINT] IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[SessionId] [BIGINT] NOT NULL FOREIGN KEY REFERENCES UserSessions (Id),
		[Key] [NVARCHAR](255) NOT NULL,
		[Type] [nvarchar] (255) NOT NULL,
		[Value] [NTEXT] NULL
	)

END
GO

IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[UserSessionFiles]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[UserSessionFiles](
		[Id] [uniqueidentifier] NOT NULL,
		[SessionId] [BIGINT] NOT NULL FOREIGN KEY REFERENCES UserSessions (Id),
		[FileName] [nvarchar](255) NOT NULL,
		[FilePath] [nvarchar](1000) NULL,
		[Type] [tinyint] NOT NULL,
		[Description] [nvarchar](500) NULL,
		[Extension] [nvarchar](5) NOT NULL,
		[Created] [datetime] NOT NULL,
		[CreatedBy] [nvarchar](128) NOT NULL
	)

END
GO




IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'UserSessions'
    AND column_name = 'LastSmsCodeGenerated'
)
BEGIN
	ALTER TABLE UserSessions add LastSmsCodeGenerated datetime NULL;

END




IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'UserSessions'
    AND column_name = 'LastEmailCodeGenerated'
)
BEGIN
	ALTER TABLE UserSessions add LastEmailCodeGenerated datetime NULL;

END





IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'UserTemporaryUrls'
    AND column_name = 'SessionId'
)
BEGIN
	ALTER TABLE UserTemporaryUrls add SessionId bigint NULL FOREIGN KEY REFERENCES UserSessions(Id);

END

-- Date: 03/21/2018

IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[Advertisements]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[Advertisements](
		[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[TitleArabic] [nvarchar](1000) NULL,
		[TitleEnglish] [nvarchar](100) NULL,
		[ContentArabic] [nvarchar](max) NULL,
		[ContentEnglish] [varchar](max) NULL,
		[IsActivated] [bit] NOT NULL,
		[VisibleFrom] [datetime] NULL,
		[VisibleUntil] [datetime] NULL,
		[Created] [datetime] NOT NULL,
		[CreatedBy] [nvarchar](128) NULL
	)

END
GO






IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[AdvertisementViews]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[AdvertisementViews](
		[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[AdvertisementId] [int] NOT NULL FOREIGN KEY REFERENCES Advertisements (Id),
		[UserId] [nvarchar](128) NULL FOREIGN KEY REFERENCES Users (Id),
		[SessionId] [bigint] NOT NULL FOREIGN KEY REFERENCES UserSessions (Id),
		[IpAddress] [varchar](50) NOT NULL,
		[Browser] [varchar](500) NOT NULL,
		[Date] [datetime] NOT NULL,
	)

END
GO



IF  NOT EXISTS (SELECT * FROM dbo.Advertisements 
WHERE ContentArabic like '%<img src="~/Content/img/intro.jpg" />%')
BEGIN
	insert into dbo.Advertisements values (
		N'',
		N'',
		N'<img class="img-responsive" src="~/Content/img/intro.jpg" />',
		N'<img class="img-responsive" src="~/Content/img/intro.jpg" />',
		1,
		NULL,
		NULL,
		getdate(),
		'00000000-0000-0000-0000-000000000000'
	);

END
GO



IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'RequestCoresearchers'
    AND column_name = 'Notified'
)
BEGIN
	ALTER TABLE RequestCoresearchers add Notified datetime NULL;

END

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'RequestCoresearchers'
    AND column_name = 'Clicked'
)
BEGIN
	ALTER TABLE RequestCoresearchers add Clicked datetime NULL;

END

-- Date: 03/26/2018

IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[UserExtraRoles]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[UserExtraRoles](
		[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[UserId] [nvarchar](128) NULL FOREIGN KEY REFERENCES Users (Id),
		[RoleId] [nvarchar](128) NULL FOREIGN KEY REFERENCES Roles (Id),
		[CollegeId] [int] NULL FOREIGN KEY REFERENCES Colleges (Id),
		[IsActive] [bit] NOT NULL DEFAULT ((0)),
		[EffectiveFrom] [datetime] NULL,
		[EffectiveTo] [datetime] NULL,
		[Created] [datetime] NOT NULL,
		[Updated] [datetime] NOT NULL,
		[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES Users (Id),
		[UpdatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES Users (Id)
	)

END
GO

-- Added on 26h March, 2018

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Colleges'
    AND column_name = 'RoleGroupCode'
)
BEGIN
	ALTER TABLE Colleges ADD RoleGroupCode TINYINT NULL;
END

GO

UPDATE Colleges SET RoleGroupCode = 0 WHERE NameArabic NOT IN (N'الطب', N'العلوم', N'الهندسة', N'المعهد العالي للقضاء', N'المعهد العالي للدعوةو الإحتساب', N'معهد تعليم اللغة العربية');
UPDATE Colleges SET RoleGroupCode = 1 WHERE NameArabic IN (N'الطب', N'العلوم', N'الهندسة');
UPDATE Colleges SET RoleGroupCode = 2 WHERE NameArabic IN (N'المعهد العالي للقضاء', N'المعهد العالي للدعوةو الإحتساب', N'معهد تعليم اللغة العربية');

GO



IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'RequestHistoryLogs'
    AND column_name = 'Role'
)
BEGIN
	ALTER TABLE RequestHistoryLogs add Role int NULL;

END


IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'RequestHistoryLogs'
    AND column_name = 'CollegeId'
)
BEGIN
	ALTER TABLE RequestHistoryLogs add CollegeId int NULL FOREIGN KEY REFERENCES COlleges (Id);

END




IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'RequestHistoryLogs'
    AND column_name = 'IsImpersonating'
)
BEGIN
	ALTER TABLE RequestHistoryLogs add IsImpersonating bit NULL;

END

-- Added on 2nd April, 2018

GO

IF NOT EXISTS (SELECT * FROM Settings WHERE [Key] = 'arf.referee.external.acceptance.days')
BEGIN
	INSERT INTO Settings VALUES('arf.referee.external.acceptance.days', '5', GETDATE(),'00000000-0000-0000-0000-000000000000', GETDATE(), '00000000-0000-0000-0000-000000000000');
END

GO

IF NOT EXISTS (SELECT * FROM Settings WHERE [Key] = 'arf.referee.response.days')
BEGIN
	INSERT INTO Settings VALUES('arf.referee.response.days', '14', GETDATE(),'00000000-0000-0000-0000-000000000000', GETDATE(), '00000000-0000-0000-0000-000000000000');
END

GO

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'RequestReferees'
    AND column_name = 'RequestReviewOrderNumber'
)
BEGIN
	ALTER TABLE RequestReferees ADD RequestReviewOrderNumber INT NULL;
END

GO

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'RequestReferees'
    AND column_name = 'RequestReviewSentOn'
)
BEGIN
	ALTER TABLE RequestReferees ADD RequestReviewSentOn DATETIME NULL;
END

GO

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'RequestReferees'
    AND column_name = 'RequestReviewAcceptOn'
)
BEGIN
	ALTER TABLE RequestReferees ADD RequestReviewAcceptOn DATETIME NULL;
END

GO  


IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'RequestRevisionId'
)
BEGIN
	ALTER TABLE Requests ADD RequestRevisionId bigint NULL;
END


GO

ALTER VIEW [dbo].[ViewCoResearcherRewardRequestsDetails] as   
SELECT r.Id, r.Type, r.Title, rs.FirstName, rs.SecondName, rs.ThirdName, rs.LastName, r.Status, r.PublicationType, r.Created, r.CreatedBy, r.Updated, r.UpdatedBy, rr.Amount, cr.ResearcherType  
FROM requests r   
JOIN RewardRequests rr on rr.Requestid = r.Id  
JOIN users u on u.id = rr.UserId  
JOIN Researchers rs on rs.UserId = u.Id  
JOIN RequestCoresearchers cr on cr.ResearcherId = rs.Id  
WHERE r.type in (3,4,5) AND r.IsDeleted = 0 AND r.RequestRevisionId IS NULL

GO

  
ALTER VIEW [dbo].[ViewMaterialRequestsDetails] as   
select r.Id, r.Type, r.Title, r.CollegeId, cl.NameArabic as CollegeNameArabic, cl.NameEnglish as CollegeNameEnglish, rs.FirstName, rs.SecondName, rs.ThirdName, rs.LastName, r.Status, r.FieldType, r.ProjectStartDate, r.ProjectEndDate, r.AmountSAR as Amount
, r.Created, r.CreatedBy, r.Updated, r.UpdatedBy,  
  
(case when r.Id IS NULL then 0  
 else  
  case r.Status  
   when 906 then 1  
   else 0  
  end  
end)  
IsApproved,  
  
(case when r.Id IS NULL then 0  
 else  
  case r.Status  
   when 351 then 1  
   else 0  
  end  
end)  
isPaid,  
(case when r.Id IS NULL then 0  
 else  
  case r.Status  
   when 301 then 1  
   else 0  
  end  
end)  
IsRejected  
  
from requests r   
join users u on u.id = r.CreatedBy  
join Researchers rs on rs.UserId = u.Id  
join Colleges cl on cl.Id = r.CollegeId  
  
WHERE r.type = 5 AND r.IsDeleted = 0 AND r.RequestRevisionId IS NULL

GO  
  
ALTER VIEW [dbo].[ViewRefereeRewardRequestsDetails] as   
SELECT r.Id, r.Type, r.Title, rs.FirstName, rs.SecondName, rs.ThirdName, rs.LastName, r.Status, r.PublicationType, r.Created, r.CreatedBy, r.Updated, r.UpdatedBy, rr.Amount  
FROM requests r   
JOIN RewardRequests rr on rr.Requestid = r.Id  
JOIN users u on u.id = r.CreatedBy  
JOIN Referees rs on rs.UserId = u.Id  
WHERE r.type in (7)  AND r.isDeleted = 0 AND r.RequestRevisionId IS NULL
  
GO

  
ALTER VIEW [dbo].[ViewRequestsDetails] as   
select r.Id, r.Type, r.Title, rs.FirstName, rs.SecondName, rs.ThirdName, rs.LastName, r.Status, r.FieldType, r.ProjectStartDate, r.ProjectEndDate, r.AmountSAR as Amount, r.Created, r.CreatedBy, r.Updated, r.UpdatedBy,  
  
(case when r2.Id IS NULL then 0  
 else  
  case r2.Status  
   when 708 then 1  
   else 0  
  end  
end)  
IsMidTermFundApproved,  
  
  
(case when r4.Id IS NULL then 0  
 else  
  case r4.Status  
   when 605 then 1  
   else 0  
  end  
end)  
IsFullTermFundApproved,  
  
  
(case when r2.Id IS NULL then 0  
 else  
  case r2.Status  
   when 351 then 1  
   else 0  
  end  
end)  
IsMidTermFundPaid,  
  
  
(case when r4.Id IS NULL then 0  
 else  
  case r4.Status  
   when 351 then 1  
   else 0  
  end  
end)  
IsFullTermFundPaid  
  
  
  
from requests r   
join users u on u.id = r.CreatedBy  
join Researchers rs on rs.UserId = u.Id  
left join requests r2 on r2.id = (select top 1 id from requests r3 where r3.PublicationRequestId = r.Id and r3.type = 3 order by id desc)  
left join requests r4 on r4.id = (select top 1 id from requests r5 where r5.PublicationRequestId = r.Id and r5.type = 4 order by id desc)  
  
where r.type = 1  AND r.IsDeleted = 0 AND r.RequestRevisionId IS NULL
  
GO

  
ALTER VIEW [dbo].[ViewResearcherRewardRequestsDetails] as   
select r.Id, r.Type, r.Title, rs.FirstName, rs.SecondName, rs.ThirdName, rs.LastName, r.Status, r.PublicationType, r.Created, r.CreatedBy, r.Updated, r.UpdatedBy, rr.Amount  
from requests r   
join RewardRequests rr on rr.Requestid = r.Id  
join users u on u.id = rr.UserId  
join Researchers rs on rs.UserId = u.Id  
where r.type in (3,4,5)  AND r.IsDeleted = 0 AND r.RequestRevisionId IS NULL
  
GO

   
ALTER VIEW [dbo].[ViewRequestsByTypeReport] as select * from   
 (  
  select   
    1,  
    'IslamicField',    
    (select count(*) from ViewRequestsDetails where [FieldType] = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 1 and status = 351),	
    (select count(*) from ViewRequestsDetails where [FieldType] = 1 and status = 351 and IsMidTermFundApproved = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 1 and status = 351 and IsFullTermFundApproved = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 1 and status = 301),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 1 and status = 351 and IsMidTermFundPaid = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 1 and status = 351 and IsFullTermFundPaid = 1),
    (select count(*) from ViewRequestsDetails where [FieldType] = 1 and status = 1) 
   UNION SELECT  
    2,  
    'ArabicLanguageField',  
    (select count(*) from ViewRequestsDetails where [FieldType] = 2),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 351),  	
    (select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 351 and IsMidTermFundApproved = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 351 and IsFullTermFundApproved = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 301),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 351 and IsMidTermFundPaid = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 351 and IsFullTermFundPaid = 1),
    (select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 1)   
   UNION SELECT  
    3,  
    'SocialField',  
    (select count(*) from ViewRequestsDetails where [FieldType] = 3),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 351),  	
    (select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 351 and IsMidTermFundApproved = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 351 and IsFullTermFundApproved = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 301),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 351 and IsMidTermFundPaid = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 351 and IsFullTermFundPaid = 1),
    (select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 1)   
  
   UNION SELECT  
    4,  
    'AppliedScienceField',  
    (select count(*) from ViewRequestsDetails where [FieldType] = 4),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 351),  	
    (select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 351 and IsMidTermFundApproved = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 351 and IsFullTermFundApproved = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 301),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 351 and IsMidTermFundPaid = 1),  
    (select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 351 and IsFullTermFundPaid = 1),
    (select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 1) 
  
     
  
 ) E(FieldTypeId, FieldType, SubmittedRequests, ApprovedRequests, ApprovedMidTermRequests, ApprovedFullTermRequests, RejectedRequests, PaidMidTermRequests, PaidFullTermRequests, ReceivedByViceDean)
   
     
GO    

ALTER FUNCTION [dbo].[FuncRequestsByType] (@FieldType smallint, @RequestStatus smallint)  
RETURNS TABLE  
AS  
RETURN  
 select * from viewrequestsdetails vr  
 where   
  (@FieldType = 0 and @RequestStatus = 1) OR  
  (@FieldType = 0 and @RequestStatus = 2 and vr.Status = 351) OR  
  (@FieldType = 0 and @RequestStatus = 3 and vr.Status = 351 and IsMidTermFundApproved = 1) OR  
  (@FieldType = 0 and @RequestStatus = 4 and vr.Status = 351 and IsFullTermFundApproved = 1) OR  
  (@FieldType = 0 and @RequestStatus = 5 and vr.Status = 301) OR  
  (@FieldType = 0 and @RequestStatus = 6 and vr.Status = 351 and IsMidTermFundPaid = 1) OR  
  (@FieldType = 0 and @RequestStatus = 7 and vr.Status = 351 and IsFullTermFundPaid = 1) OR  
  (@FieldType = 0 and @RequestStatus = 8 and vr.Status = 1) OR
    
    
  (@FieldType = 1 and vr.FieldType = 1 and @RequestStatus = 1) OR  
  (@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 2 and vr.Status = 351) OR  
  (@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 3 and vr.Status = 351 and IsMidTermFundApproved = 1) OR  
  (@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 4 and vr.Status = 351 and IsFullTermFundApproved = 1) OR  
  (@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 5 and vr.Status = 301) OR  
  (@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 6 and vr.Status = 351 and IsMidTermFundPaid = 1) OR  
  (@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 7 and vr.Status = 351 and IsFullTermFundPaid = 1) OR  
  (@FieldType = 1 and vr.FieldType = 1 and @RequestStatus = 8 and vr.Status = 1) OR  
    
  (@FieldType = 2 and vr.FieldType = 2 and @RequestStatus = 1) OR  
  (@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 2 and vr.Status = 351) OR  
  (@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 3 and vr.Status = 351 and IsMidTermFundApproved = 1) OR  
  (@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 4 and vr.Status = 351 and IsFullTermFundApproved = 1) OR  
  (@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 5 and vr.Status = 301) OR  
  (@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 6 and vr.Status = 351 and IsMidTermFundPaid = 1) OR  
  (@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 7 and vr.Status = 351 and IsFullTermFundPaid = 1) OR  
  (@FieldType = 2 and vr.FieldType = 2 and @RequestStatus = 8 and vr.Status = 1) OR    
    
  (@FieldType = 3 and vr.FieldType = 3 and @RequestStatus = 1) OR  
  (@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 2 and vr.Status = 351) OR  
  (@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 3 and vr.Status = 351 and IsMidTermFundApproved = 1) OR  
  (@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 4 and vr.Status = 351 and IsFullTermFundApproved = 1) OR  
  (@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 5 and vr.Status = 301) OR  
  (@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 6 and vr.Status = 351 and IsMidTermFundPaid = 1) OR  
  (@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 7 and vr.Status = 351 and IsFullTermFundPaid = 1) OR  
  (@FieldType = 3 and vr.FieldType = 3 and @RequestStatus = 8 and vr.Status = 1) OR  
    
  (@FieldType = 4 and vr.FieldType = 4 and @RequestStatus = 1) OR  
  (@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 2 and vr.Status = 351) OR  
  (@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 3 and vr.Status = 351 and IsMidTermFundApproved = 1) OR  
  (@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 4 and vr.Status = 351 and IsFullTermFundApproved = 1) OR  
  (@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 5 and vr.Status = 301) OR  
  (@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 6 and vr.Status = 351 and IsMidTermFundPaid = 1) OR  
  (@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 7 and vr.Status = 351 and IsFullTermFundPaid = 1)  OR
  (@FieldType = 4 and vr.FieldType = 4 and @RequestStatus = 8 and vr.Status = 1)   
  
GO


IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefereeRequestFeedbackFormQuestions]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[RefereeRequestFeedbackFormQuestions](
		[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[TextEnglish] [nvarchar](2048) NULL,
		[TextArabic] [nvarchar](2048) NOT NULL,
		[OrderNumber] [int] NOT NULL,
		[MaxMarks] [int] NOT NULL,
		[IslamicFieldEn] [bit] NOT NULL,
		[ArabicLanguageFieldEn] [bit] NOT NULL,
		[SocialFieldEn] [bit] NOT NULL,
		[AppliedScienceFieldEn] [bit] NOT NULL,
		[Created] [datetime] NOT NULL,
		[UpdatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES Users (Id),
		[Updated] [datetime] NOT NULL,
		[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES Users (Id),
		[IsDeleted] [bit] NOT NULL
	 )

END

GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefereeRequestFeedback]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[RefereeRequestFeedback](
		[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[FeedbackFormQuestionId] [int] NOT NULL FOREIGN KEY REFERENCES RefereeRequestFeedbackFormQuestions (Id),
		[RefereeId] [int] NOT NULL  FOREIGN KEY REFERENCES Referees (Id),
		[RequestId] [bigint] NOT NULL  FOREIGN KEY REFERENCES Requests (Id),
		[Comments] nvarchar(MAX) NULL,
		[GivenMarks] [int] NOT NULL,		
		[Created] [datetime] NOT NULL,
		[UpdatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES Users (Id),
		[Updated] [datetime] NOT NULL,
		[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES Users (Id),
		[IsDeleted] [bit] NOT NULL
	 )

END

GO


IF NOT EXISTS (SELECT * FROM Settings WHERE [Key] = 'arf.referee.request_feedback.reject_cutoff')
BEGIN
	INSERT INTO Settings VALUES('arf.referee.request_feedback.reject_cutoff', '70', GETDATE(),'00000000-0000-0000-0000-000000000000', GETDATE(), '00000000-0000-0000-0000-000000000000');
END

GO

-- Added on 2018-05-08

IF NOT EXISTS (SELECT * FROM Settings WHERE [Key] = 'arf.request.allowed_to_submit')
BEGIN
	INSERT INTO Settings VALUES('arf.request.allowed_to_submit', '444', GETDATE(),'00000000-0000-0000-0000-000000000000', GETDATE(), '00000000-0000-0000-0000-000000000000');
END
ELSE
BEGIN
	UPDATE Settings SET value = '444', Updated = GETDATE() WHERE [key] =  'arf.request.allowed_to_submit'	
END

GO

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'SecretaryMeetingDate'
)
BEGIN
	ALTER TABLE Requests ADD SecretaryMeetingDate DATETIME NULL;
END


GO

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'SecretaryMeetingNumber'
)
BEGIN
	ALTER TABLE Requests ADD SecretaryMeetingNumber [NVARCHAR](200) NULL;
END

GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RequestCoresearcherResponse]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[RequestCoresearcherResponse](
		[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[RequestCoresearcherId] bigint NOT NULL FOREIGN KEY REFERENCES RequestCoresearchers (Id),		
		[RequestId] [bigint] NOT NULL  FOREIGN KEY REFERENCES Requests (Id),
		[AgreementType] [smallint] NOT NULL,
		[Status] [smallint] NULL,
		[Comments] nvarchar(MAX) NULL,
		[ReceivedOn] [datetime] NULL,
		[SubmittedOn] [datetime] NULL,
		[SentON] [datetime] NULL,
		[Created] [datetime] NOT NULL,
		[UpdatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES Users (Id),
		[Updated] [datetime] NOT NULL,
		[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES Users (Id),
		[IsDeleted] [bit] NOT NULL
	 )

END

GO

IF NOT EXISTS (SELECT * FROM Settings WHERE [Key] = 'arf.coresearcher.response.days')
BEGIN
	INSERT INTO Settings VALUES('arf.coresearcher.response.days', '5', GETDATE(),'00000000-0000-0000-0000-000000000000', GETDATE(), '00000000-0000-0000-0000-000000000000');
END

GO

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'ResearcherContractText'
)
BEGIN
	ALTER TABLE Requests ADD ResearcherContractText [NVARCHAR](MAX) NULL;
END

GO

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'ResearcherContractStartDate'
)
BEGIN
	ALTER TABLE Requests ADD ResearcherContractStartDate [DateTime] NULL;
END

GO

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'ContractApprovalDate'
)
BEGIN
	ALTER TABLE Requests ADD ContractApprovalDate [DateTime] NULL;
END

GO

-- Added on 1st July, 2018

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Requests'
    AND column_name = 'EditRequestType'
)
BEGIN
	ALTER TABLE Requests ADD EditRequestType [int] NULL;
END

GO