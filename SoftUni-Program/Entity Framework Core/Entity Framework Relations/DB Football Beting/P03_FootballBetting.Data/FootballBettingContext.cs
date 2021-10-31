using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data.Congigure;
using P03_FootballBetting.Data.Models;
using System;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext: DbContext
    {
        public FootballBettingContext()
        {
                
        }
        public FootballBettingContext(DbContextOptions options)
           :base(options)
        {

        }
        public virtual DbSet<Bet> Bets { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<Town> Towns { get; set; }
        public virtual DbSet<User> Users { get; set; }




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Connection.CONNECTION);
            }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().HasOne(c => c.PrimaryKitColor)
                 .WithMany(t => t.PrimaryKitTeams)
                 .HasForeignKey(fk => fk.PrimaryKitColorId)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Team>().HasOne(c => c.SecondaryKitColor)
                .WithMany(t => t.SecondaryKitTeams)
                .HasForeignKey(fk => fk.SecondaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>().HasOne(h => h.HomeTeam)
                .WithMany(g => g.HomeGames)
                .HasForeignKey(fk => fk.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>().HasOne(a => a.AwayTeam)
                .WithMany(g => g.AwayGames)
                .HasForeignKey(fk => fk.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerStatistic>().HasKey(k => new { k.GameId, k.PlayerId });

            modelBuilder.Entity<Bet>().Property(p => p.Prediction).IsRequired();
        }
    }
}
