USE SoftUni
-- 1. Employee Address
SELECT TOP(5)e.EmployeeID, e.JobTitle, e.AddressID, a.AddressText
FROM Employees AS e
LEFT JOIN  Addresses AS a ON e.AddressID = a.AddressID
ORDER BY a.AddressID 
GO
-- 2. Addresses with Towns
SELECT TOP(50) e.FirstName, e.LastName, t.[Name] AS Town, a.AddressText
FROM Employees AS e
LEFT JOIN Addresses AS a ON e.AddressID = a.AddressID
LEFT JOIN Towns AS t ON A.TownID = T.TownID
ORDER BY e.FirstName, e.LastName
GO
-- 3. Employee
SELECT e.EmployeeID, e.FirstName, e.LastName, d.[Name] AS DepartmentName
FROM Employees AS e 
INNER JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
WHERE d.Name = 'Sales'
ORDER BY e.EmployeeID
GO
--4. Employee Departments
SELECT TOP(5) e.EmployeeID, e.FirstName, e.Salary, d.[Name] AS DepartmentName
FROM Employees AS e 
INNER JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
WHERE E.Salary > 15000
ORDER BY D.DepartmentID
GO
--5. Employees Without Project
SELECT TOP(3)	e.EmployeeID, e.FirstName
FROM Employees AS e
LEFT JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
WHERE ep.EmployeeID IS NULL
ORDER BY e.EmployeeID
GO
--6. Employees Hired After
SELECT e.FirstName, e.LastName, e.HireDate, d.[Name] AS DeptName
FROM Employees AS e
INNER JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
WHERE d.Name IN ('Sales', 'Finance')
AND e.HireDate > '01-01-1999'
ORDER BY e.HireDate
GO
--7. Employees with Project
SELECT TOP(5) e.EmployeeID, e.FirstName,p.[Name] AS ProjectName
FROM Employees AS e 
INNER JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
INNER JOIN Projects AS p ON ep.ProjectID = p.ProjectID
WHERE p.StartDate > '08-13-2002' 
AND p.EndDate IS NULL
ORDER BY e.EmployeeID
GO
--8. Employee 24
SELECT TOP(5) e.EmployeeID, e.FirstName,
CASE 
WHEN YEAR(p.StartDate) >= 2005 THEN NULL
ELSE p.[Name]
END AS ProjectName
FROM Employees AS e 
INNER JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
INNER JOIN Projects AS p ON ep.ProjectID = p.ProjectID
WHERE e.EmployeeID = 24
GO
--9. Employee Manager
SELECT e.EmployeeID, e.FirstName, e.ManagerID, em.FirstName AS ManagerName
FROM Employees AS e
FULL OUTER JOIN Employees AS em ON e.ManagerID = em.EmployeeID
WHERE e.ManagerID IN (3,7)
ORDER BY e.EmployeeID
GO
--10. Employee Summary
SELECT TOP(50) e.EmployeeID, e.FirstName + ' '+ E.LastName AS EmployeeName,em.FirstName + ' '+em.LastName AS ManagerName, d.[name] AS DepartmentName
 FROM Employees AS e
FULL OUTER JOIN Employees AS em ON e.ManagerID = em.EmployeeID
JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
ORDER BY e.EmployeeID
GO
--11. Min Average Salary
SELECT TOP(1) MinAverageSalary
FROM 
(SELECT DepartmentID, AVG(Salary) AS MinAverageSalary
FROM Employees
GROUP BY DepartmentID) AS SalaryAverage
ORDER BY MinAverageSalary
GO
USE Geography
--12. Highest Peaks in Bulgaria
SELECT c.CountryCode, m.MountainRange, p.PeakName, p.Elevation FROM 
Countries AS c
INNER JOIN MountainsCountries  AS mc ON c.CountryCode = mc.CountryCode
INNER JOIN Mountains AS m ON mc.MountainId = m.Id
INNER JOIN Peaks AS p ON m.Id = p.MountainId
WHERE c.CountryCode = 'BG' AND p.Elevation > 2835
ORDER BY  p.Elevation DESC
GO
--13. Count Mountain Ranges
SELECT c.CountryCode , COUNT(c.CountryCode) FROM Countries AS  c 
INNER JOIN  MountainsCountries AS mc ON c.CountryCode = mc.CountryCode AND c.CountryCode IN ('BG','US','RU')
INNER JOIN Mountains AS m ON mc.MountainId = m.Id
GROUP BY c.CountryCode
GO
--14. Countries with Rivers
SELECT TOP(5) c.CountryName, r.RiverName 
FROM Countries AS c 
LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode 
LEFT JOIN Rivers AS r ON cr.RiverId = r.Id
WHERE c.ContinentCode = 'AF'
ORDER BY c.CountryName
--15. *Continents and Currencies

SELECT ContinentCode ,CurrencyCode, CurCount AS CurrencyUsage
FROM
(SELECT *,
DENSE_RANK () OVER (PARTITION BY ContinentCode ORDER BY CurCount DESC ) AS  CurrencyRank
FROM
(SELECT *
FROM (SELECT ContinentCode, CurrencyCode, COUNT(CurrencyCode) AS CurCount
FROM Countries
GROUP BY ContinentCode, CurrencyCode) AS CurrencyCounter
WHERE CurCount > 1) AS SubQuerry) AS FinalQuerry
WHERE CurrencyRank = 1
ORDER BY ContinentCode
GO
--16. Countries Without Any Mountains
SELECT COUNT(CountryName) AS [Count]
FROM Countries AS c
LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
WHERE MountainId IS NULL
GO
--17. Highest Peak and Longest River by Country
SELECT TOP(5)
CountryName, HighestPeakElevation, LongestRiverLength
FROM
(SELECT c.CountryName,MAX(p.Elevation) AS HighestPeakElevation, MAX(r.Length) AS LongestRiverLength
FROM Countries AS c 
LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
LEFT JOIN Peaks AS p ON m.Id = p.MountainId
LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
LEFT JOIN Rivers AS r ON cr.RiverId = r.Id
GROUP BY c.CountryName) AS CountryInfo
ORDER BY HighestPeakElevation DESC, LongestRiverLength DESC
--18. Highest Peak Name and Elevation by Country
SELECT TOP(5)
[Country],
ISNULL([Highest Peak Name], '(no highest peak)') AS [Highest Peak Name] ,
ISNULL([Highest Peak Elevation] , 0)  AS [Highest Peak Elevation], 
ISNULL([Mountain], '(no mountain)') AS [Mountain]
FROM 
(SELECT *,
DENSE_RANK() OVER  (PARTITION BY [Country] ORDER BY [Highest Peak Elevation] ) AS PeakRank
FROM 
(SELECT c.CountryName AS [Country] , P.PeakName AS [Highest Peak Name] , p.Elevation AS[Highest Peak Elevation], m.MountainRange AS [Mountain]
FROM Countries AS c 
LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
LEFT JOIN Peaks AS p ON m.Id = p.MountainId)
AS FirstQ) 
AS SecondQ
WHERE PeakRank = 1





