SELECT MountainS.MountainRange, Peaks.PeakName, Peaks.Elevation
FROM Mountains
JOIN Peaks ON Peaks.MountainID = Mountains.Id
WHERE MountainRange = 'Rila'
ORDER BY Elevation DESC