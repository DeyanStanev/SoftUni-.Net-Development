USE SOFTUNI

GO
-- 1. Employees with Salary Above 35000
CREATE PROCEDURE usp_GetEmployeesSalaryAbove35000 
AS
BEGIN
	SELECT 
	FirstName, LastName
	FROM Employees
	WHERE Salary > 35000
END

--EXEC usp_GetEmployeesSalaryAbove35000 
GO
--2 Employees with Salary Above Number

CREATE PROCEDURE usp_GetEmployeesSalaryAboveNumber @inputNum DECIMAL(18,4)
AS
BEGIN
	SELECT FirstName,LastName
	FROM Employees
	WHERE Salary >= @inputNum
END
--EXEC usp_GetEmployeesSalaryAboveNumber 48100
GO
-- 3. Town Names Starting With
CREATE PROCEDURE usp_GetTownsStartingWith @InputString VARCHAR(50)
AS
BEGIN
	SELECT [Name]
	FROM Towns
	WHERE LEFT([Name], LEN(@InputString)) =  @InputString
END
-- EXEC usp_GetTownsStartingWith 'B'
GO

--4. Employees from Town
CREATE PROCEDURE usp_GetEmployeesFromTown  @InputName VARCHAR(50)
AS
BEGIN
	SELECT 
	FirstName as [First Name],
	LastName  as [Last Name]		
	FROM Employees AS e
	INNER JOIN Addresses AS  d ON e.AddressID = d.AddressID
	INNER JOIN Towns AS t ON d.TownID = t.TownID
	WHERE t.[Name] = @InputName
END
--EXEC usp_GetEmployeesFromTown 'Sofia'
GO

--5. Salary Level Function
CREATE FUNCTION ufn_GetSalaryLevel(@salary DECIMAL(18,4)) 
RETURNS VARCHAR(8)
AS
BEGIN
DECLARE @Output VARCHAR(8)

	IF @salary < 30000 
		BEGIN
			SET @Output = 'Low'
		END
	ELSE IF @salary BETWEEN 30000 AND 50000 
		BEGIN
			SET @Output = 'Average'
		END
	ELSE
		BEGIN
			SET @Output = 'High'
		END
	RETURN @Output
END
GO
--SELECT dbo.ufn_GetSalaryLevel(35000) FROM Employees
-- 6.	Employees by Salary Level
CREATE PROCEDURE usp_EmployeesBySalaryLevel (@Salary VARCHAR(8))
AS
BEGIN
	SELECT FirstName AS [First Name],
	        LastName AS [Last Name]
			FROM Employees
			WHERE dbo.ufn_GetSalaryLevel(Salary) = @Salary
END
GO
-- 7. Define Function
CREATE OR ALTER FUNCTION ufn_IsWordComprised(@setOfLetters NVARCHAR(MAX), @word NVARCHAR(MAX))
RETURNS BIT
AS
BEGIN
DECLARE @RESULT BIT 
	SET @RESULT = 0
DECLARE @COUNTER INT
	SET @COUNTER = 1
DECLARE @NUMOFCHARS INT
	SET @NUMOFCHARS = LEN(@word)
WHILE @COUNTER <= LEN(@word)
	BEGIN
		DECLARE @COUNTER1 INT
		SET @COUNTER1 = 1
		WHILE @COUNTER1 <= LEN(@setOfLetters)
		BEGIN
			IF SUBSTRING(@word, @COUNTER,1) = SUBSTRING(@setOfLetters, @COUNTER1,1) 
			BEGIN
			SET @NUMOFCHARS = @NUMOFCHARS-1
			END
		SET @COUNTER1 = @COUNTER1+1
		END
	SET @COUNTER = @COUNTER+1
	END
IF @NUMOFCHARS <= 0 
	BEGIN
		SET @RESULT = 1
	END
RETURN @RESULT
END
GO
 --SELECT dbo.ufn_IsWordComprised('oistmiahf', 'halves') 