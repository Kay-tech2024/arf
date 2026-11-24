IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ViewRequestsDetails]'))
BEGIN
	DROP VIEW ViewRequestsDetails
END

GO

Create VIEW ViewRequestsDetails as 
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

where r.type = 1
GO



IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ViewRequestsByTypeReport]'))
BEGIN
	DROP VIEW ViewRequestsByTypeReport
END

GO

Create VIEW ViewRequestsByTypeReport as select * from 
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
				(select count(*) from ViewRequestsDetails where [FieldType] = 1 and status = 351 and IsFullTermFundPaid = 1)	
			
			UNION SELECT
				2,
				'ArabicLanguageField',
				(select count(*) from ViewRequestsDetails where [FieldType] = 2),
				(select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 351),
				(select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 351 and IsMidTermFundApproved = 1),
				(select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 351 and IsFullTermFundApproved = 1),
				(select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 301),
				(select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 351 and IsMidTermFundPaid = 1),
				(select count(*) from ViewRequestsDetails where [FieldType] = 2 and status = 351 and IsFullTermFundPaid = 1)
			UNION SELECT
				3,
				'SocialField',
				(select count(*) from ViewRequestsDetails where [FieldType] = 3),
				(select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 351),
				(select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 351 and IsMidTermFundApproved = 1),
				(select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 351 and IsFullTermFundApproved = 1),
				(select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 301),
				(select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 351 and IsMidTermFundPaid = 1),
				(select count(*) from ViewRequestsDetails where [FieldType] = 3 and status = 351 and IsFullTermFundPaid = 1)

			UNION SELECT
				4,
				'AppliedScienceField',
				(select count(*) from ViewRequestsDetails where [FieldType] = 4),
				(select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 351),
				(select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 351 and IsMidTermFundApproved = 1),
				(select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 351 and IsFullTermFundApproved = 1),
				(select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 301),
				(select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 351 and IsMidTermFundPaid = 1),
				(select count(*) from ViewRequestsDetails where [FieldType] = 4 and status = 351 and IsFullTermFundPaid = 1)

			

	) E(FieldTypeId, FieldType, SubmittedRequests, ApprovedRequests, ApprovedMidTermRequests, ApprovedFullTermRequests, RejectedRequests, PaidMidTermRequests, PaidFullTermRequests)
	
	
go

IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[FuncRequestsByType]'))
BEGIN
	DROP FUNCTION [FuncRequestsByType]
END

GO

CREATE FUNCTION [dbo].[FuncRequestsByType] (@FieldType smallint, @RequestStatus smallint)
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
		
		
		
		(@FieldType = 1 and vr.FieldType = 1 and @RequestStatus = 1) OR
		(@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 2 and vr.Status = 351) OR
		(@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 3 and vr.Status = 351 and IsMidTermFundApproved = 1) OR
		(@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 4 and vr.Status = 351 and IsFullTermFundApproved = 1) OR
		(@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 5 and vr.Status = 301) OR
		(@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 6 and vr.Status = 351 and IsMidTermFundPaid = 1) OR
		(@FieldType = 1 and vr.FieldType = 1 and  @RequestStatus = 7 and vr.Status = 351 and IsFullTermFundPaid = 1) OR
		
		
		(@FieldType = 2 and vr.FieldType = 2 and @RequestStatus = 1) OR
		(@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 2 and vr.Status = 351) OR
		(@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 3 and vr.Status = 351 and IsMidTermFundApproved = 1) OR
		(@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 4 and vr.Status = 351 and IsFullTermFundApproved = 1) OR
		(@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 5 and vr.Status = 301) OR
		(@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 6 and vr.Status = 351 and IsMidTermFundPaid = 1) OR
		(@FieldType = 2 and vr.FieldType = 2 and  @RequestStatus = 7 and vr.Status = 351 and IsFullTermFundPaid = 1) OR
		
		
		(@FieldType = 3 and vr.FieldType = 3 and @RequestStatus = 1) OR
		(@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 2 and vr.Status = 351) OR
		(@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 3 and vr.Status = 351 and IsMidTermFundApproved = 1) OR
		(@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 4 and vr.Status = 351 and IsFullTermFundApproved = 1) OR
		(@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 5 and vr.Status = 301) OR
		(@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 6 and vr.Status = 351 and IsMidTermFundPaid = 1) OR
		(@FieldType = 3 and vr.FieldType = 3 and  @RequestStatus = 7 and vr.Status = 351 and IsFullTermFundPaid = 1) OR
		
		(@FieldType = 4 and vr.FieldType = 4 and @RequestStatus = 1) OR
		(@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 2 and vr.Status = 351) OR
		(@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 3 and vr.Status = 351 and IsMidTermFundApproved = 1) OR
		(@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 4 and vr.Status = 351 and IsFullTermFundApproved = 1) OR
		(@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 5 and vr.Status = 301) OR
		(@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 6 and vr.Status = 351 and IsMidTermFundPaid = 1) OR
		(@FieldType = 4 and vr.FieldType = 4 and  @RequestStatus = 7 and vr.Status = 351 and IsFullTermFundPaid = 1)

go




IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ViewRewardRequestsDetails]'))
BEGIN
	DROP VIEW ViewRewardRequestsDetails
END

GO

IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ViewResearcherRewardRequestsDetails]'))
BEGIN
	DROP VIEW ViewResearcherRewardRequestsDetails
END

GO

IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ViewCoResearcherRewardRequestsDetails]'))
BEGIN
	DROP VIEW ViewCoResearcherRewardRequestsDetails
END

GO


IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ViewRefereeRewardRequestsDetails]'))
BEGIN
	DROP VIEW ViewRefereeRewardRequestsDetails
END

GO

Create VIEW ViewResearcherRewardRequestsDetails as 
select r.Id, r.Type, r.Title, rs.FirstName, rs.SecondName, rs.ThirdName, rs.LastName, r.Status, r.PublicationType, r.Created, r.CreatedBy, r.Updated, r.UpdatedBy, rr.Amount
from requests r 
join RewardRequests rr on rr.Requestid = r.Id
join users u on u.id = rr.UserId
join Researchers rs on rs.UserId = u.Id
where r.type in (3,4,5)

GO



Create VIEW ViewCoResearcherRewardRequestsDetails as 
select r.Id, r.Type, r.Title, rs.FirstName, rs.SecondName, rs.ThirdName, rs.LastName, r.Status, r.PublicationType, r.Created, r.CreatedBy, r.Updated, r.UpdatedBy, rr.Amount, cr.ResearcherType
from requests r 
join RewardRequests rr on rr.Requestid = r.Id
join users u on u.id = rr.UserId
join Researchers rs on rs.UserId = u.Id
join RequestCoresearchers cr on cr.ResearcherId = rs.Id
where r.type in (3,4,5)

GO




Create VIEW ViewRefereeRewardRequestsDetails as 
select r.Id, r.Type, r.Title, rs.FirstName, rs.SecondName, rs.ThirdName, rs.LastName, r.Status, r.PublicationType, r.Created, r.CreatedBy, r.Updated, r.UpdatedBy, rr.Amount
from requests r 
join RewardRequests rr on rr.Requestid = r.Id
join users u on u.id = r.CreatedBy
join Referees rs on rs.UserId = u.Id
where r.type in (7)

GO

create view ViewRewardRequestsDetails as 
(
	select * from 
	(
		(select Id, Type, Title, FirstName, SecondName, ThirdName, LastName, Status, PublicationType, Created, CreatedBy, Updated, UpdatedBy, Amount, 3 as UserType from ViewResearcherRewardRequestsDetails) UNION
		(select Id, Type, Title, FirstName, SecondName, ThirdName, LastName, Status, PublicationType, Created, CreatedBy, Updated, UpdatedBy, Amount, ResearcherType as UserType from ViewCoResearcherRewardRequestsDetails) UNION
		(select Id, Type, Title, FirstName, SecondName, ThirdName, LastName, Status, PublicationType, Created, CreatedBy, Updated, UpdatedBy, Amount, 1 as UserType from ViewRefereeRewardRequestsDetails)
	) as E
)

go

IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ViewRequestsByFinancialTypesReport]'))
BEGIN
	DROP VIEW ViewRequestsByFinancialTypesReport
END

GO

Create VIEW ViewRequestsByFinancialTypesReport as select * from 
	(
		select 
				1,
				'Referee',  
				(select count(*) from ViewRefereeRewardRequestsDetails),
				(select count(*) from ViewRefereeRewardRequestsDetails where status = 351),
				(select count(*) from ViewRefereeRewardRequestsDetails where status = 301),
				(select sum(r.amount) from ViewRefereeRewardRequestsDetails r where r.status = 351)	
				
			UNION SELECT
				2,
				'ResearchTeam',
				(select count(*) from ViewResearcherRewardRequestsDetails),
				(select count(*) from ViewResearcherRewardRequestsDetails where status = 351),
				(select count(*) from ViewResearcherRewardRequestsDetails where status = 301),
				(select sum(r.amount) from ViewResearcherRewardRequestsDetails r where r.status = 351)
				+ (select sum(r.amount) from ViewCoResearcherRewardRequestsDetails r where r.status = 351)
				


	) E(UserRoleId, UserRole, SubmittedRequests, ApprovedRequests, RejectedRequests, TotalAmount)
	
	
go


IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ViewRequestsByResearchTeamTypesReport]'))
BEGIN
	DROP VIEW ViewRequestsByResearchTeamTypesReport
END

GO

Create VIEW ViewRequestsByResearchTeamTypesReport as select * from 
	(
		select 
				3,
				'MainResearcher',  
				(select count(*) from ViewResearcherRewardRequestsDetails),
				(select count(*) from ViewResearcherRewardRequestsDetails where status = 351),
				(select count(*) from ViewResearcherRewardRequestsDetails where status = 301),
				(select sum(r.amount) from ViewResearcherRewardRequestsDetails r where r.status = 351)	
				
			UNION SELECT
				4,
				'AssistantResearcher',  
				(select count(*) from ViewCoResearcherRewardRequestsDetails where researchertype = 4),
				(select count(*) from ViewCoResearcherRewardRequestsDetails where researchertype = 4 and status = 351),
				(select count(*) from ViewCoResearcherRewardRequestsDetails where researchertype = 4 and status = 301),
				(select sum(r.amount) from ViewCoResearcherRewardRequestsDetails r where researchertype = 4 and r.status = 351)	
				
			UNION SELECT
				5,
				'AssociateResearcher',  
				(select count(*) from ViewCoResearcherRewardRequestsDetails where researchertype = 5),
				(select count(*) from ViewCoResearcherRewardRequestsDetails where researchertype = 5 and status = 351),
				(select count(*) from ViewCoResearcherRewardRequestsDetails where researchertype = 5 and status = 301),
				(select sum(r.amount) from ViewCoResearcherRewardRequestsDetails r where researchertype = 5 and r.status = 351)	
			
			UNION SELECT
				6,
				'Consultant',  
				(select count(*) from ViewCoResearcherRewardRequestsDetails where researchertype = 6),
				(select count(*) from ViewCoResearcherRewardRequestsDetails where researchertype = 6 and status = 351),
				(select count(*) from ViewCoResearcherRewardRequestsDetails where researchertype = 6 and status = 301),
				(select sum(r.amount) from ViewCoResearcherRewardRequestsDetails r where researchertype = 6 and r.status = 351)	


	) E(UserRoleId, UserRole, SubmittedRequests, ApprovedRequests, RejectedRequests, TotalAmount)
	
	
go

IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[FuncRequestsByFinancialTypes]'))
BEGIN
	DROP FUNCTION [FuncRequestsByFinancialTypes]
END

GO

CREATE FUNCTION [dbo].[FuncRequestsByFinancialTypes] (@UserRole smallint, @RequestStatus smallint)
RETURNS TABLE
AS
RETURN
	select * from ViewRewardRequestsDetails vr
	where 
		(@UserRole = 0 and @RequestStatus = 1) OR
		(@UserRole = 0 and @RequestStatus = 2 and vr.Status = 351) OR
		(@UserRole = 0 and @RequestStatus = 3 and vr.Status = 301) OR
		
		(@UserRole = 1 and @RequestStatus = 1 and vr.UserType = 1) OR
		(@UserRole = 1 and @RequestStatus = 2 and vr.UserType = 1 and vr.Status = 351) OR
		(@UserRole = 1 and @RequestStatus = 3 and vr.UserType = 1 and vr.Status = 301) OR
		
		(@UserRole = 2 and @RequestStatus = 1 and vr.UserType in (3,4,5,6)) OR
		(@UserRole = 2 and @RequestStatus = 2 and vr.UserType in (3,4,5,6) and vr.Status = 351) OR
		(@UserRole = 2 and @RequestStatus = 3 and vr.UserType in (3,4,5,6) and vr.Status = 301) OR
		
		(@UserRole = 3 and @RequestStatus = 1 and vr.UserType = 3) OR
		(@UserRole = 3 and @RequestStatus = 2 and vr.UserType = 3 and vr.Status = 351) OR
		(@UserRole = 3 and @RequestStatus = 3 and vr.UserType = 3 and vr.Status = 301) OR
		
		(@UserRole = 4 and @RequestStatus = 1 and vr.UserType = 4) OR
		(@UserRole = 4 and @RequestStatus = 2 and vr.UserType = 4 and vr.Status = 351) OR
		(@UserRole = 4 and @RequestStatus = 3 and vr.UserType = 4 and vr.Status = 301) OR
		
		
		(@UserRole = 5 and @RequestStatus = 1 and vr.UserType = 5) OR
		(@UserRole = 5 and @RequestStatus = 2 and vr.UserType = 5 and vr.Status = 351) OR
		(@UserRole = 5 and @RequestStatus = 3 and vr.UserType = 5 and vr.Status = 301) OR
		
		
		(@UserRole = 6 and @RequestStatus = 1 and vr.UserType = 6) OR
		(@UserRole = 6 and @RequestStatus = 2 and vr.UserType = 6 and vr.Status = 351) OR
		(@UserRole = 6 and @RequestStatus = 3 and vr.UserType = 6 and vr.Status = 301)

go


IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ViewMaterialRequestsDetails]'))
BEGIN
	DROP VIEW ViewMaterialRequestsDetails
END

GO

Create VIEW ViewMaterialRequestsDetails as 
select r.Id, r.Type, r.Title, r.CollegeId, cl.NameArabic as CollegeNameArabic, cl.NameEnglish as CollegeNameEnglish, rs.FirstName, rs.SecondName, rs.ThirdName, rs.LastName, r.Status, r.FieldType, r.ProjectStartDate, r.ProjectEndDate, r.AmountSAR as Amount, r.Created, r.CreatedBy, r.Updated, r.UpdatedBy,

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

where r.type = 5
GO




IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[FuncMaterialRequestsByCollege]'))
BEGIN
	DROP FUNCTION [FuncMaterialRequestsByCollege]
END

GO

CREATE FUNCTION [dbo].[FuncMaterialRequestsByCollege] (@collegeId int, @RequestStatus smallint)
RETURNS TABLE
AS
RETURN
	select * from ViewMaterialRequestsDetails vr
	where 
		(@collegeId = 0 and @RequestStatus = 1) OR
		(@collegeId = 0 and @RequestStatus = 2 and vr.IsApproved = 1) OR
		(@collegeId = 0 and @RequestStatus = 3 and vr.IsApproved = 1 and vr.IsPaid = 0) OR
		(@collegeId = 0 and @RequestStatus = 4 and vr.IsRejected = 1) OR
		(@collegeId = 0 and @RequestStatus = 5 and vr.IsApproved = 1 and vr.IsPaid = 1) OR

		(@collegeId > 0 and vr.CollegeId = @collegeId and @RequestStatus = 1) OR
		(@collegeId > 0 and vr.CollegeId = @collegeId and @RequestStatus = 2 and vr.IsApproved = 1) OR
		(@collegeId > 0 and vr.CollegeId = @collegeId and @RequestStatus = 3 and vr.IsApproved = 1 and vr.IsPaid = 0) OR
		(@collegeId > 0 and vr.CollegeId = @collegeId and @RequestStatus = 4 and vr.IsRejected = 1) OR
		(@collegeId > 0 and vr.CollegeId = @collegeId and @RequestStatus = 5 and vr.IsApproved = 1 and vr.IsPaid = 1)

go

		
		
		
		

		
		
IF  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ViewMaterialRequests]'))
BEGIN
	DROP VIEW ViewMaterialRequests
END

GO

Create VIEW ViewMaterialRequests as 
(
	select Id as CollegeId, NameArabic as CollegeNameArabic, NameEnglish as CollegeNameEnglish,
			(select count(*) from ViewMaterialRequestsDetails m2 where m2.CollegeId = m1.Id ) as SubmittedRequests,
			(select count(*) from ViewMaterialRequestsDetails m2 where m2.CollegeId = m1.Id and m2.Isapproved = 1) as ApprovedRequests,
			(select count(*) from ViewMaterialRequestsDetails m2 where m2.CollegeId = m1.Id and m2.IsPaid = 0 and m2.IsApproved = 1) as ApprovedUnpaidRequests,
			(select count(*) from ViewMaterialRequestsDetails m2 where m2.CollegeId = m1.Id and m2.isRejected = 1) as RejectedRequests,
			(select count(*) from ViewMaterialRequestsDetails m2 where m2.CollegeId = m1.Id and m2.IsPaid = 1 and m2.IsApproved = 1) as PaidRequests,
			(select sum(amount) from ViewMaterialRequestsDetails m2 where m2.CollegeId = m1.Id and m2.IsPaid = 1 and m2.IsApproved = 1) as TotalAmount
	from colleges m1
)

go
