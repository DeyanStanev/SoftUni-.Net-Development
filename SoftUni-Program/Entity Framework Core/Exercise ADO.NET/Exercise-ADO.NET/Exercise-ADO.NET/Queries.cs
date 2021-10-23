using System;
using System.Collections.Generic;
using System.Text;

namespace Exercise_ADO.NET
{
    public static class Queries
    {
        public const string ViliansWithMoreThan3Minions =
                        @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                FROM Villains AS v 
                JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
            GROUP BY v.Id, v.Name 
              HAVING COUNT(mv.VillainId) > 3 
            ORDER BY COUNT(mv.VillainId)";

        public const string GetViliansById = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age,
										 V.name as VillianName
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
									join Villains as v on mv.VillainId = V.id
                                   WHERE mv.VillainId = @id
                                ORDER BY m.Name";
                             
    }
}
