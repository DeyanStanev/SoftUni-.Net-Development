CREATE DATABASE CigarShop

USE CigarShop
GO

--1
CREATE TABLE [Sizes]
(
	[Id] INT PRIMARY KEY IDENTITY NOT NULL,
	[Length] INT NOT NULL,
	CHECK ([Length] BETWEEN 10 AND 25) ,
	[RingRange] DECIMAL (2,1) NOT NULL
	CHECK ([RingRange] BETWEEN 1.5 AND 7.5)

)

CREATE TABLE [Tastes]
(
	[Id] INT PRIMARY KEY IDENTITY NOT NULL,
	[TasteType] NVARCHAR(20) NOT NULL,
	[TasteStrength] NVARCHAR(15) NOT NULL,
	[ImageURL] NVARCHAR(100) NOT NULL,
)

CREATE TABLE [Brands]
(
	[Id] INT PRIMARY KEY IDENTITY NOT NULL,
	[BrandName] NVARCHAR(30) NOT NULL UNIQUE,
	[BrandDescription] VARCHAR(MAX)
)

CREATE TABLE [Cigars]
(
	[Id] INT PRIMARY KEY IDENTITY NOT NULL,
	[CigarName] NVARCHAR(80) NOT NULL ,
	[BrandId] INT FOREIGN KEY REFERENCES [Brands](Id) NOT NULL,
	[TastId]  INT FOREIGN KEY REFERENCES [Tastes](Id) NOT NULL,
	[SizeId]  INT FOREIGN KEY REFERENCES [Sizes](Id) NOT NULL,
	[PriceForSingleCigar] DECIMAL (18,2) NOT NULL,
	[ImageURL] NVARCHAR(100) NOT NULL
)

CREATE TABLE [Addresses]
(
	[Id] INT PRIMARY KEY IDENTITY NOT NULL,
	[Town]  NVARCHAR(30) NOT NULL ,
	[Country] NVARCHAR(30) NOT NULL ,
	[Streat] NVARCHAR(100) NOT NULL ,
	[ZIP] NVARCHAR(20) NOT NULL 
)

CREATE TABLE [Clients]
(
	[Id] INT PRIMARY KEY IDENTITY NOT NULL,
	[FirstName]  NVARCHAR(30) NOT NULL ,
	[LastName]  NVARCHAR(30) NOT NULL ,
	[Email]  NVARCHAR(50) NOT NULL ,
	[AddressId] INT  FOREIGN KEY REFERENCES [Addresses](Id) NOT NULL

)

CREATE TABLE [ClientsCigars]
(
	[ClientId] INT FOREIGN KEY REFERENCES [Clients](Id) NOT NULL,
	[CigarId] INT FOREIGN KEY REFERENCES [Cigars](Id) NOT NULL,
	PRIMARY KEY ([ClientId],[CigarId] )
)

--2

INSERT INTO [Cigars] VALUES
('COHIBA ROBUSTO',	9	, 1	, 5	, 15.50, 	'cohiba-robusto-stick_18.jpg'),
('COHIBA SIGLO I',	9	, 1	, 10, 410.00	, 'cohiba-siglo-i-stick_12.jpg'),
('HOYO DE MONTERREY LE HOYO DU MAIRE',	14	,5	,11	,7.50,	'hoyo-du-maire-stick_17.jpg'),
('HOYO DE MONTERREY LE HOYO DE SAN JUAN',	14	,4	,15	,32.00	, 'hoyo-de-san-juan-stick_20.jpg'),
('TRINIDAD COLONIALES',	2	,3	,8	,85.21	, 'trinidad-coloniales-stick_30.jpg')


INSERT INTO [Addresses] VALUES
('Sofia',	'Bulgaria',	'18 Bul. Vasil levski',	'1000'),
('Athens',	'Greece',	'4342 McDonald Avenue',	'10435'),
('Zagreb',	'Croatia',	'4333 Lauren Drive',	'10000')

--3
UPDATE Cigars	
SET PriceForSingleCigar = PriceForSingleCigar *1.2 WHERE [TastId] = 1

UPDATE Brands
SET BrandDescription = 'New description' WHERE BrandDescription IS NULL
--4

DELETE FROM Clients
WHERE AddressId IN (SELECT Id FROM [Addresses] WHERE Country LIKE 'C%')

DELETE FROM Addresses WHERE Country LIKE 'C%'

--5

SELECT CigarName, PriceForSingleCigar, ImageURL FROM Cigars
ORDER BY PriceForSingleCigar, CigarName DESC

--6
SELECT c.Id, c.CigarName, c.PriceForSingleCigar, t.TasteType, t.TasteStrength FROM Cigars AS c
JOIN Tastes AS t ON c.TastId = t.Id
WHERE t.TasteType IN ('Earthy', 'Woody')
ORDER BY c.PriceForSingleCigar DESC
--7
SELECT Id, CONCAT(FirstName, ' ', LastName) AS [Full Name], Email FROM Clients AS c
LEFT JOIN ClientsCigars AS cc ON c.Id = cc.ClientId
WHERE CigarId IS NULL
ORDER BY [Full Name]


--8
SELECT TOP(5) c.CigarName, PriceForSingleCigar, ImageURL FROM Cigars AS c 
LEFT JOIN Sizes AS s ON c.SizeId = s.Id
WHERE s.[Length] >= 12 AND (C.CigarName LIKE '%ci%' OR c.PriceForSingleCigar > 50)  AND s.RingRange > 2.55
ORDER BY c.CigarName, c.PriceForSingleCigar DESC


--9f
SELECT [Full Name], Country, ZIP, CigarPrice from
(SELECT CONCAT (FirstName, ' ', LastName) AS [Full Name], Country, ZIP, 
CONCAT('$',PriceForSingleCigar) as CigarPrice,
DENSE_RANK() over (partition by c.id order by cg.PriceForSingleCigar desc)   as [Rank]
FROM ClientsCigars AS cc
JOIN Clients as c on cc.ClientId = c.Id
JOIN Cigars as cg on cc.CigarId = cg.Id
JOIN Addresses as a on a.Id = c.AddressId
where isnumeric(ZIP) = 1 
 ) as q
where [Rank] = 1
order by [Full Name]
--10

SELECT  [LastName] , AVG(s.Length) AS [CiagrLength] , CEILING(AVG(s.RingRange)) AS [CiagrRingRange] from ClientsCigars as cc
JOIN Clients as c on cc.ClientId = c.Id
JOIN Cigars as cg on cc.CigarId = cg.Id
JOIN Sizes as s on cg.SizeId = s.Id
GROUP BY  c.LastName
ORDER BY [CiagrLength] DESC

--11
GO
CREATE FUNCTION udf_ClientWithCigars(@name VARCHAR(MAX)) 
RETURNS INT
BEGIN
	RETURN(SELECT COUNT(CC.CigarId) FROM ClientsCigars AS CC
	LEFT JOIN  Clients AS C ON CC.ClientId = C.Id
	WHERE C.FirstName = @name)
END
GO
--12
CREATE PROCEDURE usp_SearchByTaste(@taste VARCHAR(50))
AS
BEGIN
SELECT 
C.CigarName,
CONCAT('$', C.PriceForSingleCigar) AS Price,
T.TasteType,
BrandName,
CONCAT(S.Length,' ', 'cm') as CigarLength,
CONCAT(S.RingRange, ' ' ,'cm') AS CigarRingRange
FROM Cigars AS C 
JOIN Tastes AS T ON C.TastId = T.Id
JOIN Sizes AS S ON S.Id = C.SizeId
JOIN Brands AS B ON B.Id = C.BrandId
WHERE T.TasteType = @taste
ORDER BY CigarLength, CigarRingRange DESC
END
GO
EXEC usp_SearchByTaste 'Woody'
--

