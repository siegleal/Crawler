USE SecurityCrawlerDatabase
GO

CREATE TABLE Website(
	ID int IDENTITY,
	URL varchar(500) NOT NULL,
	language varchar(25),
	serverSoftware varchar(25),
	version varchar(15)
	PRIMARY KEY(ID)
);


CREATE TABLE Crawl(
	CrawlID int IDENTITY,
	SiteID int NOT NULL,
	Timestamp Timestamp NOT NULL,
	requestEmail varchar(75) NOT NULL,
	PRIMARY KEY(CrawlID),
	FOREIGN KEY(SiteID) REFERENCES Website(ID)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);



CREATE TABLE Vulnerability(
	CrawlID int,
	VulnerabilityID int IDENTITY,
	Details varchar(500),
	FOREIGN KEY(CrawlID) REFERENCES Crawl(CrawlID)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);
CREATE TABLE ContentPackage(
	SiteID int,
	Name varchar(75),
	Version varchar(75),
	PRIMARY KEY(Name, SiteID),
	FOREIGN KEY(SiteID) REFERENCES Website(ID)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);
	
	