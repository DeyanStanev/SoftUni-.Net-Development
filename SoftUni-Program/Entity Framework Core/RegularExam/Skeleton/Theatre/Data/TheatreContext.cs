namespace Theatre.Data
{
    using Microsoft.EntityFrameworkCore;
    using Theatre.Data.Models;

    public class TheatreContext : DbContext
    {
        public TheatreContext() { }

        public TheatreContext(DbContextOptions options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<Ticket>().HasOne(o => o.Theatre).WithMany(w => w.Tickets).HasForeignKey(f => f.TheatreId);
            modelBuilder.Entity<Ticket>().HasOne(o => o.Play).WithMany(w => w.Tickets).HasForeignKey(f => f.PlayId);
        }
        public virtual DbSet<Cast> Casts{ get; set; }
        public virtual DbSet<Play> Plays{ get; set; }
        public virtual DbSet<Theatre> Theatres{ get; set; }
        public virtual DbSet<Ticket> Tickets{ get; set; }
    }
}
