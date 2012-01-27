USE SecurityCrawlerDatabase
GO

CREATE FUNCTION dbo.getLargestCrawlID()
	RETURNS int
BEGIN
DECLARE @result int;
SET @result = (SELECT MAX(CrawlID) FROM CRAWL);
RETURN @result
END