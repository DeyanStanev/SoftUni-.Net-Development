CREATE DATABASE Service
USE Service

CREATE TABLE [Status](
Id INT PRIMARY KEY IDENTITY,
[Label] VARCHAR(30) NOT NULL
)
CREATE TABLE [Users](
Id INT PRIMARY KEY IDENTITY,
[Username] VARCHAR(30) NOT NULL UNIQUE,
[Password] VARCHAR(50) NOT NULL ,
[Name] VARCHAR(50) ,
[Birthdate] DATETIME2,
[Age] INT NOT NULL,
CHECK([Age] BETWEEN 14 AND 110),
[Email] VARCHAR(50) NOT NULL
)
CREATE TABLE [Departments](
Id INT PRIMARY KEY IDENTITY,
[Name] VARCHAR(50) NOT NULL
)
CREATE TABLE [Categories](
Id INT PRIMARY KEY IDENTITY,
[Name] VARCHAR(50) NOT NULL,
[DepartmentId] INT REFERENCES [Departments](Id) NOT NULL
)
CREATE TABLE [Employees](
Id INT PRIMARY KEY IDENTITY,
[FirstName] VARCHAR(25),
[LastName] VARCHAR(25),
[Birthdate] DATETIME2,
[Age] INT ,
CHECK([Age] BETWEEN 18 AND 110),
[DepartmentId] INT REFERENCES [Departments](Id) NOT NULL
)
CREATE TABLE [Reports](
Id INT PRIMARY KEY IDENTITY,
[CategoryId] INT REFERENCES [Categories](Id) NOT NULL,
[StatusId] INT REFERENCES [Status](Id) NOT NULL,
[OpenDate] DATETIME2 NOT NULL,
[CloseDate] DATETIME2 ,
[Description] VARCHAR(200) NOT NULL,
[UserId] INT REFERENCES [Users](Id) NOT NULL,
[EmployeeId] INT REFERENCES [Employees](Id)
)


INSERT INTO [Employees] VALUES
('Marlo','O''Malley','1958-9-21',NULL,1),
('Niki','Stanaghan','1969-11-26',NULL,4),
('Ayrton','Senna', '1960-03-21',NULL,9),
('Ronnie', 'Peterson','1944-02-14',NULL,9),
('Giovanna', 'Amati', '1959-07-20',NULL,5)

INSERT INTO Reports ([CategoryId],[StatusId],[OpenDate],[CloseDate],[Description],[UserId],[EmployeeId]) VALUES
('1','1','2017-04-13', NULL, 'Stuck Road on Str.133',	'6','2'),
('6','3','2015-09-05', '2015-12-06', 'Charity trail running', '3','5'),
('14', '2',	'2015-09-07', NULL,'Falling bricks on Str.58','5','2'),
('4','3','2017-07-03', '2017-07-06','Cut off streetlight on Str.11','1','1')

UPDATE Reports
SET CloseDate = GETDATE() WHERE CloseDate IS NULL


DELETE FROM Reports
WHERE [StatusId] = 4



SELECT [Description],FORMAT(OpenDate, 'dd-MM-yyyy')
FROM Reports
WHERE EmployeeId IS NULL
ORDER BY OpenDate, [Description]

SELECT r.Description, c.Name AS CategoryName
FROM Reports as r
JOIN Categories AS  c on r.CategoryId = c.Id
ORDER BY r.Description, c.Name

SELECT TOP(5) [Name] AS CategoryName , q.ReportsNumber
FROM Categories AS c
JOIN
(SELECT CategoryId, COUNT(CategoryId) AS ReportsNumber
FROM Reports
GROUP BY CategoryId) AS Q ON C.Id = Q.CategoryId
ORDER BY ReportsNumber DESC, CategoryName

SELECT Username, c.Name as CategoryName
FROM Users AS u
join Reports as r on u.Id = r.UserId
join Categories as c on r.CategoryId = c.Id
where day(u.Birthdate) = day(r.OpenDate)
order by u.Username , c.Name

select CONCAT(FirstName, ' ', LastName) as FullName, COUNT(u.Id) as UsersCount
from Employees as e
left join Reports as r on e.Id = r.EmployeeId
left join Users as u on r.UserId = u.Id
group by e.FirstName, e.LastName
order by UsersCount desc, FullName asc


SELECT
CASE
WHEN COALESCE(e.Firstname,e.Lastname) IS NOT NULL
THEN CONCAT(e.FirstName, ' ', e.LastName)
ELSE 'None'
END AS [Employee],
ISNULL(d.Name , 'None') AS [Department],
ISNULL(c.Name, 'None') as [Category],
ISNULL(r.Description, 'None') as [Description],
ISNULL(format(OpenDate, 'dd.MM.yyyy'), 'None' ) as [OpenDate],
ISNULL(s.Label , 'None') as [Status],
ISNULL(u.Name, 'None') as [User]
from Reports as r
left join Categories as c on r.CategoryId = c.Id
left join Employees as e on r.EmployeeId = e.Id
LEFT join Departments as d on e.DepartmentId = d.Id
left join [Status] as s on r.StatusId = s.Id
left join [Users] as u on r.UserId = u.Id
order by e.FirstName desc, e.LastName desc , d.Name , c.Name , r.Description , r.OpenDate, s.Label , u.Name

CREATE FUNCTION  udf_HoursToComplete (@StartDate DATETIME, @EndDate DATETIME)
RETURNS INT
BEGIN
DECLARE @HourDiff INT
	IF @StartDate IS NULL AND @EndDate IS NULL
		BEGIN 
		SET @HourDiff = 0
		END
	ELSE
		BEGIN
		SET @HourDiff =  DATEDIFF(HOUR, @StartDate, @EndDate) 
		END
	RETURN @HourDiff
END

CREATE PROCEDURE usp_AssignEmployeeToReport(@EmployeeId INT, @ReportId INT)
AS
BEGIN
	if (SELECT E.Id FROM Employees AS E
	JOIN Departments AS D ON E.DepartmentId = D.Id
	JOIN Categories AS C ON D.Id = C.DepartmentId
	JOIN Reports AS R ON C.Id = R.CategoryId
	WHERE E.Id = @EmployeeId AND R.Id = @ReportId) is null
	
	THROW 500001, 'Employee doesn''t belong to the appropriate department!',1;
	UPDATE Reports
	sET [EmployeeId] = @EmployeeId
	WHERE [Id] = @ReportId
	
END
