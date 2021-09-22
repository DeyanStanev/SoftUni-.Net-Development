USE [SoftUni]

--2 TASK 
SELECT [DepartmentID] ,[Name] ,[ManagerID]
FROM [Departments]
GO
-- 3 TASK
SELECT [Name]
FROM [Departments]
GO
--4 TASK 
SELECT [FirstName], [LastName], [Salary]
FROM [Employees]
GO
--5 TASK
SELECT [FirstName], [MiddleName], [LastName]
FROM [Employees]
GO
--6 TASK 
SELECT CONCAT([FirstName], '.', [LastName], '@' , 'softuni.bg')
FROM [Employees]
AS [Full Email Address]
GO
--7 TASK
SELECT DISTINCT [Salary] FROM [Employees]
GO
--8 TASK
SELECT * FROM [Employees]
WHERE [JobTitle] = 'Sales Representative'
GO
--9 TASK
SELECT [FirstName], [Lastname], [JobTitle]
FROM [Employees]
WHERE [Salary] >= 20000 AND [Salary] <= 30000
GO
--10 TASK
SELECT CONCAT ([FirstName],' ', [MiddleName],' ',[LastName])
AS [Full Name ]
FROM [Employees] 
WHERE [Salary] IN (25000, 14000, 12500, 23600)
GO
--11 TASK
SELECT [FirstName],[LastName]
FROM [Employees] 
WHERE [ManagerID] IS NULL
GO
--12 TASK
SELECT TOP (5)[FirstName],[LastName], [Salary]
FROM [Employees] 
WHERE [Salary] > 50000
ORDER BY [Salary] DESC
GO
--13 TASK
SELECT TOP (5)[FirstName],[LastName]
FROM [Employees] 
ORDER BY [Salary] DESC
GO
--14 TASK
SELECT [FirstName],[LastName]
FROM [Employees] 
WHERE NOT [DepartmentID] = 4
GO
--15 TASK
SELECT * FROM [Employees]
ORDER BY [Salary] DESC,
[FirstName] ASC,
[LastName] DESC,
[MiddleName] ASC
GO
--16 TASK
CREATE VIEW [V_EmployeesSalaries] AS(
SELECT  [FirstName], [LastName], [Salary]
FROM [Employees])
GO
--17 TASK
CREATE VIEW [V_EmployeeNameJobTitle] AS
SELECT CONCAT ([FirstName],' ', [MiddleName],' ', [LastName] ) AS [Full Name],  [JobTitle]
FROM [Employees]
GO
--18 TASK
SELECT DISTINCT  [JobTitle]
FROM [Employees]
GO
--19 TASK
SELECT TOP(10) [ProjectId] AS [ID], [Name], [Description],[StartDate], [EndDate]
FROM [Projects]
ORDER BY [StartDate],
[NAME]
GO
--20 TASK
SELECT TOP(7) [FirstName], [LastName], [HireDate]
FROM [Employees]
ORDER BY [HireDate] DESC
GO
--21 TASK
UPDATE [Employees] 
SET [Salary] += [Salary]*0.12
WHERE [DepartmentID] IN (1,2,4,11)
SELECT [Salary] FROM [Employees]
GO
--22 TASK
USE [Geography]
SELECT [PeakName]
FROM [Peaks]
ORDER BY [PeakName] ASC
GO
--23 TASK
SELECT TOP(30) [CountryName], [Population]
FROM [Countries]
WHERE [ContinentCode] = 'EU'
ORDER BY [Population] DESC,
[CountryName] ASC
GO
--TASK 24 
SELECT [CountryName], [CountryCode],
CASE 
  WHEN [CurrencyCode] = 'EUR' THEN 'Euro'
  ELSE 'NotEuro'
END AS [Currency]
FROM [Countries]
ORDER BY[CountryName] ASC
GO
--TASK 25 
USE [Diablo]
SELECT [Name]
FROM [Characters]
ORDER BY [Name] ASC
