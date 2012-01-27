USE SecurityCrawlerDatabase
GO

/* Get the latest crawl from a URL's ID.
	Michael Eaton 1/22/2011 */

CREATE FUNCTION dbo.getLatestCrawl
	( @url varchar(500) )
	RETURNS int
BEGIN
DECLARE @result int;
/* Get the website's ID */
DECLARE @websiteID int;
SET @websiteID = (SELECT ID FROM Website WHERE URL = @url);
/* Get the latest timestamp */
DECLARE @latestTimestamp Timestamp;
SET @latestTimestamp = (SELECT MAX(CrawlTimestamp) FROM Crawl WHERE SiteID = @websiteID);
SET @result = (
	SELECT CrawlID FROM Crawl
	WHERE SiteID = @websiteID AND CrawlTimestamp = @latestTimestamp
);
RETURN @result;
END