USE SecurityCrawlerDatabase
GO

CREATE FUNCTION dbo.getLargestWebsiteID
()
	RETURNS int
BEGIN
DECLARE @result int;
SET @result = (SELECT MAX(ID) FROM Website);
RETURN @result;
END