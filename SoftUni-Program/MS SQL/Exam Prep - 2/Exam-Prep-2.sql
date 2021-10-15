CREATE DATABASE School
GO
USE School
GO

CREATE TABLE [Students]
(
	[Id] INT PRIMARY KEY IDENTITY,
	[FirstName] VARCHAR(30) NOT NULL,
	[MiddleName] VARCHAR(25),
	[LastName] VARCHAR(30) NOT NULL,
	[Age] INT NOT NULL,
	CHECK([Age] > 0),
	[Address] VARCHAR(50),
	[Phone] CHAR(10),
	CHECK(LEN([Phone]) = 10)
)

CREATE TABLE [Subjects]
(
	[Id] INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(25) NOT NULL,
	[Lessons] INT NOT NULL,
	CHECK([Lessons] > 0)
)
CREATE TABLE [StudentsSubjects]
(
	[Id] INT PRIMARY KEY IDENTITY,
	[StudentId] INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	[SubjectId] INT FOREIGN KEY REFERENCES Subjects(ID) NOT NULL,
	[Grade] DECIMAL (3,2) NOT NULL,
	CHECK ([Grade] BETWEEN 2 AND 6) 
)

CREATE TABLE [Exams]
(
	[Id] INT PRIMARY KEY IDENTITY,
	[Date] DATETIME, 
	[SubjectId] INT FOREIGN KEY REFERENCES Subjects(ID) NOT NULL,
)
CREATE TABLE [StudentsExams]
(
	[StudentId] INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	[ExamId]	INT FOREIGN KEY REFERENCES Exams(Id) NOT NULL,
	[Grade] DECIMAL (3,2) NOT NULL,
	CHECK ([Grade] BETWEEN 2 AND 6),
	PRIMARY KEY ([StudentId],[ExamId] )
)

CREATE TABLE [Teachers]
(
	[Id] INT PRIMARY KEY IDENTITY,
	[FirstName] VARCHAR(20) NOT NULL,
	[LastName] VARCHAR(20) NOT NULL,
	[Address] VARCHAR(20) NOT NULL,
	[Phone] CHAR(10),
	CHECK(LEN([Phone]) = 10),
	[SubjectId] INT FOREIGN KEY REFERENCES Subjects(ID) NOT NULL,
)
CREATE TABLE [StudentsTeachers]
(
	[StudentId] INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	[TeacherId] INT FOREIGN KEY REFERENCES Teachers(Id) NOT NULL,
	PRIMARY KEY ([StudentId],[TeacherId])
)
GO
-- 2
INSERT INTO [Subjects] VALUES
('Geometry',	12),
('Health',		10),
('Drama',		7),
('Sports',		9)

INSERT INTO [Teachers] VALUES
('Ruthanne',	'Bamb', '84948 Mesta Junction',	'3105500146',	6),
('Gerrard',	'Lowin', '370 Talisman Plaza',	'3324874824',	2),
('Merrile',	'Lambdin',	'81 Dahle Plaza',	'4373065154',	5),
('Bert',	'Ivie',	'2 Gateway Circle',	'4409584510',	4)

--3
UPDATE [StudentsSubjects]
SET [Grade] = 6 WHERE SubjectId IN (1,2) AND [Grade] >= 5.50
--4
DELETE FROM [StudentsTeachers] 
WHERE  [TeacherId] IN (SELECT Id FROM Teachers WHERE [Phone] LIKE '%72%')

DELETE FROM Teachers WHERE [Phone] LIKE '%72%'

--5
SELECT 
[FirstName], 
[LastName],
[Age]
FROM Students WHERE [Age] >= 12
ORDER BY [FirstName], [LastName]

--6
SELECT s.FirstName, s.LastName, COUNT(st.StudentId) AS TeachersCount FROM [Students] AS s 
JOIN StudentsTeachers AS st ON s.Id = st.StudentId
GROUP BY s.Id, s.FirstName, s.LastName
ORDER BY LastName
--7
SELECT CONCAT(FirstName,' ', LastName) AS [Full Name]
FROM Students AS s
LEFT JOIN StudentsExams AS SE ON s.Id = SE.StudentId
WHERE Grade IS NULL
ORDER BY FirstName

--8
SELECT TOP(10) FirstName, LastName , FORMAT(AVG(es.Grade),'n2') AS [Grade] FROM Students AS s
JOIN StudentsExams AS es on s.Id = es.StudentId
GROUP BY s.FirstName, s.LastName
ORDER BY [Grade] DESC, FirstName, LastName
--9
SELECT 
CASE 
WHEN s.MiddleName IS NULL THEN CONCAT(FirstName, ' ', LastName)
ELSE CONCAT(FirstName,' ', MiddleName, ' ', LastName)
END AS [Full name]
FROM Students AS s
LEFT JOIN StudentsSubjects AS ss ON s.Id = ss.StudentId
WHERE SubjectId IS NULL
ORDER BY [Full name]

--10
SELECT s.[Name], AVG(Grade) AS[AverageGrade] FROM Subjects AS s
JOIN StudentsSubjects AS ss ON s.Id = ss.SubjectId
GROUP BY s.Name, s.Id
ORDER BY s.Id

--11
GO
CREATE FUNCTION udf_ExamGradesToUpdate(@studentId INT, @grade DECIMAL(15,2))
RETURNS VARCHAR(MAX)
AS
BEGIN

DECLARE @StudentName VARCHAR(MAX)
DECLARE @StudentMaxGrade DECIMAL(15,2)
DECLARE @GradeCount INT

IF NOT EXISTS (SELECT 1 FROM Students WHERE @studentId = Id)
	RETURN 'The student with provided id does not exist in the school!'
IF @grade > 6
	RETURN 'Grade cannot be above 6.00!'
SET @StudentName = (SELECT TOP(1) FirstName FROM Students WHERE Id = @studentId)
SET @StudentMaxGrade = @grade+0.5
SET @GradeCount = (SELECT Count(Grade)  FROM StudentsExams
					WHERE StudentId = @studentId AND  Grade between @grade and @StudentMaxGrade)
RETURN CONCAT('You have to update',' ',@GradeCount,' ', 'grades for the student', ' ',@StudentName)

END
GO
--SELECT dbo.udf_ExamGradesToUpdate(12, 6.20)
--SELECT dbo.udf_ExamGradesToUpdate(121, 5.50)
--SELECT dbo.udf_ExamGradesToUpdate(12, 5.50)

--12
CREATE PROCEDURE usp_ExcludeFromSchool(@StudentId INT)
AS
BEGIN 
IF NOT EXISTS(SELECT 1 FROM Students WHERE @StudentId = Id)
THROW 500001, 'This school has no student with the provided id!', 1;

DELETE FROM StudentsTeachers WHERE StudentId =  @StudentId
DELETE FROM StudentsSubjects WHERE StudentId =  @StudentId
DELETE FROM StudentsExams  WHERE StudentId =  @StudentId
DELETE FROM Students WHERE Id = @StudentId
END