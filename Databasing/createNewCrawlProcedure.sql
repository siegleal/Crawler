USE SecurityCrawlerDatabase
GO

CREATE PROCEDURE dbo.createNewCrawl
	@url varchar(500),
	@reqEmail varchar(60),
	@newID int OUTPUT
AS
IF @url IS NULL
BEGIN
	RAISERROR('URL cannot be null when creating a new crawl', 14, 1);
	RETURN
END
IF @reqEmail IS NULL
BEGIN
	RAISERROR('Request Email cannot be null when creatinga  new crawl', 14, 2);
	RETURN
END
SET @newID = dbo.getLargestCrawl() + 1;
DECLARE @webID int;
SET @webID = (SELECT ID FROM Website WHERE URL = @url);
INSERT INTO Crawl(CrawlID, CrawlTimestamp, SiteID, RequestEmail)
VALUES(@newID, DEFAULT, @webID, @reqEmail)
