GO
DECLARE @DuplicateRequestUniqueCodesTable TABLE(    
	UniqueIdPart varchar(12) NOT NULL,
	Processed INT NOT NULL
);

DECLARE @DuplicateRequestsTable TABLE(    
	RequestId BIGINT NOT NULL,
	UniqueId varchar(12) NOT NULL,
	NewUniqueId varchar(12) NULL,	
	RequestCreated DateTime NOT NULL,
	RequestSubmitted DateTime NOT NULL,
	RequestUpdatedBy varchar(40) NOT NULL,
	GroupId INT NOT NULL,
	Processed INT NOT NULL
);

DECLARE @UniqueIdPart VARCHAR(12);
DECLARE @RequestId BIGINT;
DECLARE @Counter INT;
DECLARE @GroupId INT;

INSERT INTO @DuplicateRequestUniqueCodesTable
SELECT DISTINCT( SUBSTRING(UniqueId, 1, 9)) AS UniqueIdPart, 0
FROM requests
WHERE UniqueId IN (
	SELECT UniqueId
	FROM requests
	WHERE UniqueId is not null	
	GROUP BY UniqueId
	HAVING COUNT(*) > 1
)
ORDER BY UniqueIdPart



SET @GroupId = 1;
WHILE (SELECT COUNT(*) FROM @DuplicateRequestUniqueCodesTable WHERE Processed = 0) > 0
BEGIN
    SELECT TOP 1 @UniqueIdPart = UniqueIdPart FROM @DuplicateRequestUniqueCodesTable WHERE Processed = 0
	
	INSERT INTO @DuplicateRequestsTable

	SELECT tb1.Id, tb1.UniqueId, NULL, tb1.Created, tb1.Updated, tb1.UpdatedBy, @GroupId, 0
	FROM requests tb1	
	WHERE UniqueId LIKE @UniqueIdPart + '%' AND tb1.Status = 1 AND Type = 1	
	
	SET @Counter = 1;
	

	WHILE (SELECT COUNT(*) FROM @DuplicateRequestsTable WHERE Processed = 0) > 0
	BEGIN
		SELECT TOP 1 @RequestId = RequestId FROM @DuplicateRequestsTable WHERE Processed = 0 ORDER BY RequestSubmitted ASC

		UPDATE @DuplicateRequestsTable SET NewUniqueId = @UniqueIdPart + FORMAT(@Counter, '000'),  Processed = 1 WHERE RequestId = @RequestId 

		SET @Counter = @Counter + 1;
	End



    UPDATE @DuplicateRequestUniqueCodesTable SET Processed = 1 WHERE UniqueIdPart = @UniqueIdPart 
	SET @GroupId = @GroupId + 1;
End

SELECT * FROM @DuplicateRequestsTable ORDER BY NewUniqueId, RequestSubmitted ASC



-- Need to uncomment once output is verified
IF (SELECT COUNT(*) FROM @DuplicateRequestsTable WHERE Processed = 1) > 0
BEGIN

	UPDATE Requests 
	SET UniqueId = tb2.NewUniqueId
	FROM Requests tb1 INNER JOIN @DuplicateRequestsTable tb2
		ON tb1.Id = tb2.RequestId
END

GO