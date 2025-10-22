
CREATE TABLE dbo.Countries(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CommonName NVARCHAR(200) NOT NULL CONSTRAINT UQ_Countries_CommonName UNIQUE,
    Capital NVARCHAR(200) NULL
);

CREATE TABLE dbo.CountryBorders(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CountryId INT NOT NULL
        CONSTRAINT FK_CountryBorders_Countries
        REFERENCES dbo.Countries(Id) ON DELETE CASCADE,
    BorderCode NVARCHAR(10) NOT NULL,
    CONSTRAINT UQ_CountryBorders UNIQUE (CountryId, BorderCode)
);

-- optional indexes, faster joins. 
CREATE INDEX IX_CountryBorders_CountryId ON dbo.CountryBorders(CountryId);

--querying: 
SELECT 
    t.CommonName, 
    t.Capital, 
    t.Id,   
    u.BorderCode
FROM dbo.Countries AS t
INNER JOIN dbo.CountryBorders AS u 
    ON t.Id = u.CountryId
ORDER BY 
    t.CommonName ASC, u.BorderCode ASC;
			
--check for duplicates 
	SELECT CountryId, BorderCode, COUNT(*) AS C
FROM dbo.CountryBorders
GROUP BY CountryId, BorderCode
HAVING COUNT(*) > 1;

--null capitals 
	SELECT Id, CommonName
FROM dbo.Countries
WHERE Capital IS NULL OR LTRIM(RTRIM(Capital)) = '';

--empty tables
DELETE FROM  dbo.CountryBorders;
DELETE FROM dbo.Countries;