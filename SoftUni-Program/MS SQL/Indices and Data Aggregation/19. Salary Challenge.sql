--19. **Salary Challenge
SELECT TOP(10)FirstName , LastName , e.DepartmentID  FROM Employees AS e
JOIN (SELECT DepartmentID, AVG (Salary) AS AvgSalary
FROM Employees
GROUP BY DepartmentID) AS q ON e.DepartmentID = q.DepartmentID
WHERE e.Salary > q.AvgSalary
ORDER BY DepartmentID