using System;
using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data;

namespace P03_FootballBetting
{
    class StartUp
    {
        static void Main(string[] args)
        {
            FootballBettingContext footballBettingContext = new FootballBettingContext();

            footballBettingContext.Database.Migrate();
            Console.WriteLine("DB migration complete");

            //footballBettingContext.Database.EnsureCreated();
            //Console.WriteLine("DB creation complete");
            //Console.WriteLine("Do you want to delete this DB enter Y/N ");
            //if (Console.ReadLine().ToUpper() == "Y")
            //{
            //    footballBettingContext.Database.EnsureDeleted();
            //}
            //else
            //{
            //    Console.WriteLine("DB was not deleted");
            //}

        }
    }
}
