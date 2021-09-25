USE SoftUni
--1 TASK Problem 1.	Find Names of All Employees by First Name
SELECT FirstName, LastName FROM Employees
WHERE FirstName LIKE 'Sa%'
GO
--2 TASK Find Names of All employees by Last Name
SELECT FirstName, LastName FROM Employees
WHERE LastName LIKE '%ei%'
GO
--3 TASK Problem 3.	Find First Names of All Employees
SELECT FirstName FROM Employees
WHERE DepartmentID = 3
OR DepartmentID = 10
AND DATEPART(YEAR,HireDate ) >= 1995 
AND DATEPART(YEAR,HireDate ) <=  2005 
--4 TASK Find All Employees Except Engineers
GO
SELECT FirstName, LastName FROM Employees
WHERE JobTitle NOT LIKE '%engineer%'
GO
--5 TASK Find Towns with Name Length
SELECT [Name] FROM Towns
WHERE LEN([Name]) IN (5,6)
ORDER BY [Name]
GO
--6 TASK Find Towns Starting With
SELECT TownID, [Name] FROM Towns
WHERE LEFT([NAME],1) IN ('M','K','B','E')
ORDER BY [Name]
--7 TASK Find Towns Not Starting With
SELECT TownID, [Name] FROM Towns
WHERE LEFT([NAME],1) NOT IN ('R','B','D')
ORDER BY [Name]
--8 TASK Create View Employees Hired After 2000 Year
CREATE VIEW [V_EmployeesHiredAfter2000] AS
SELECT FirstName, LastName
FROM Employees
WHERE YEAR(HireDate) > 2000
GO
--9 TASK Length of Last Name
SELECT FirstName,LastName
FROM Employees
WHERE LEN(LastName) = 5
GO
--10 TASK Rank Employees by Salary
SELECT EmployeeID,FirstName,LastName,Salary,
DENSE_RANK() OVER (PARTITION BY [Salary] ORDER BY [EmployeeID] ) AS [Rank]
FROM Employees
WHERE Salary BETWEEN 10000 AND 50000
ORDER BY Salary DESC
GO
--11 TASK Find All Employees with Rank 2
SELECT  * 
FROM 
	(
SELECT EmployeeID,FirstName,LastName,Salary,
DENSE_RANK() OVER (PARTITION BY [Salary] ORDER BY [EmployeeID]) AS [Rank]
FROM Employees
WHERE Salary BETWEEN 10000 AND 50000
	) 
AS RankingTable
WHERE [Rank] = 2
ORDER BY Salary DESC
GO
USE Geography
--12 TASK Countries Holding ‘A’ 3 or More Times
SELECT CountryName [Country Name] , IsoCode AS [Iso Code]
FROM Countries
WHERE CountryName LIKE '%a%a%a%'
ORDER BY IsoCode
GO
--13 TASK Mix of Peak and River Names
SELECT * FROM
(
SELECT  Peaks.PeakName, Rivers.RiverName,
LOWER(CONCAT(LEFT(PeakName, LEN(PeakName)-1), RiverName )) AS Mix
FROM Peaks
JOIN Rivers ON LOWER(RIGHT(PeakName,1)) = LOWER(LEFT(RiverName,1))
)
AS ResultTable
ORDER BY Mix
GO
USE Diablo
--14 TASK Queries for Diablo Database
SELECT TOP (50) [Name],
FORMAT([Start],'yyyy-MM-dd') AS [Start]
FROM Games
WHERE YEAR([Start]) IN (2011, 2012)
ORDER BY [Start],
[Name]
GO
--15 TASK User Email Providers
SELECT Username,
SUBSTRING([Email],1+CHARINDEX('@',Email), LEN(Email) - CHARINDEX('@',Email)) AS [Email Provider]
FROM Users
ORDER BY [Email Provider],[Username]
GO
--16 TASK Get Users with IPAdress Like Pattern
SELECT Username,
IpAddress AS [IP Address]
FROM Users
WHERE IpAddress LIKE '___.1%._%.___'
ORDER BY [Username]
GO
--17 TASK Show All Games with Duration and Part of the Day
SELECT [Name],
CASE 
WHEN DATEPART(HOUR , Start) >=0 AND DATEPART(HOUR, Start) < 12 THEN 'Morning'
WHEN DATEPART(HOUR , Start) >=12 AND DATEPART(HOUR, Start) < 18 THEN 'Afternoon'
ELSE 'Evening'
END AS [Part of the Day],
CASE 
WHEN Duration <=3 THEN 'Extra Short'
WHEN Duration > 3 AND Duration <=6  THEN 'Short'
WHEN Duration > 6  THEN 'Long'
ELSE 'Extra Long'
END AS [Duration]
FROM Games
ORDER BY [Name], [Duration]
--18 TASK Orders Table
USE Orders
SELECT ProductName,OrderDate, 
DATEADD(DAY,3, OrderDate) AS [Pay Due],
DATEADD(MONTH,1, OrderDate) AS [Deliver Due]
FROM Orders
GO
