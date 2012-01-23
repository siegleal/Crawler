USE SecurityCrawlerDatabase
GO

CREATE PROCEDURE dbo.insertNewWebsite
	( @url varchar(500),
	@language char(15),
	@serverSoftware char(25),
	@version char(15))
AS
IF @url IS NULL
BEGIN
	RAISERROR('url cannot be null for insertNewWebsite', 15, 1);
	RETURN
END
DECLARE @newID int;
SET @newID = dbo.getLargestWebsiteID() + 1;
INSERT INTO Website(ID, URL, Language, ServerSoftware, Version)
VALUES(@newID, @url, @language, @serverSoftware, @version)


