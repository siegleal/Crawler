USE SecurityCrawlerDatabase

CREATE TABLE Website(
	ID int,
	URL varchar(500),
	Language char(15),
	ServerSoftware char(25),
	Version char(15),
	PRIMARY KEY(ID)
);
	
CREATE TABLE Crawl(
	CrawlID int,
	CrawlTimestamp Timestamp,
	SiteID int,
	RequestEmail varchar(60),
	PRIMARY KEY(CrawlID),
	FOREIGN KEY(SiteID) REFERENCES Website(ID)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

CREATE TABLE ContentPackage(
	SiteID int,
	Name varchar(75),
	Version char(15),
	PRIMARY KEY(SiteID, Name),
	FOREIGN KEY(SiteID) REFERENCES Website(ID)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

CREATE TABLE Vulnerability(
	CrawlID int,
	VulnerabilityID int,
	Details varchar(600),
	PRIMARY KEY(VulnerabilityID),
	FOREIGN KEY(CrawlID) REFERENCES Crawl(CrawlID)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);