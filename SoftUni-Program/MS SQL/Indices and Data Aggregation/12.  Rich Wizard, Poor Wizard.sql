--12. * Rich Wizard, Poor Wizard
SELECT 
SUM([Host Wizard Deposit] - [Guest Wizard Deposit]) AS SumDifference FROM
(SELECT
FirstName AS [Host Wizzard],
DepositAmount AS [Host Wizard Deposit],
LEAD (FirstName) OVER ( ORDER BY Id) AS [Guest Wizard],
LEAD (DepositAmount) OVER (ORDER By id) AS [Guest Wizard Deposit]
FROM WizzardDeposits) AS DepositTable