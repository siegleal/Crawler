USE SecurityCrawlerDatabase
GO

CREATE PROCEDURE dbo.getWebsiteVulnerabilities
	( @url varchar(500) )
AS
IF @url IS NULL
BEGIN
	RAISERROR('url for getWebsiteVulnerabilities is null', 14, 1)
	RETURN
END
IF @url NOT IN (SELECT URL FROM Website)
BEGIN
	RAISERROR('url not in database', 14, 2)
	RETURN
END
DECLARE @crawlID int;
SET @crawlID = dbo.getLatestCrawl(@url);
SELECT Details FROM Vulnerability WHERE CrawlID = @crawlID