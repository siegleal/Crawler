USE SecurityCrawlerDatabase
GO

CREATE PROCEDURE [dbo].[insertNewWebsite]
	( @url varchar(500),
	@language char(15),
	@serverSoftware char(25),
	@version char(15),
	@output int OUTPUT)
AS
IF @url IS NULL
BEGIN
	RAISERROR('url cannot be null for insertNewWebsite', 15, 1);
	RETURN
END
INSERT INTO Website(URL, Language, ServerSoftware, Version)
VALUES(@url, @language, @serverSoftware, @version);
SET @output = (SELECT MAX(ID) FROM Website WHERE url = @url);





