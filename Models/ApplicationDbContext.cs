using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Guide_Me.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<Place> Places { get; set; }
        public DbSet<City> Cities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data source = .; Initial catalog = Guide_Me; integrated security = true");
            optionsBuilder.LogTo(log => Debug.WriteLine(log));
            //optionsBuilder.UseLazyLoadingProxies(true);

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entity mappings here
            modelBuilder.Entity<Place>().HasKey(p => p.Id);
            modelBuilder.Entity<City>().HasKey(c => c.Id);

            // Define relationships (if any)
            modelBuilder.Entity<Place>()
                .HasOne(p => p.City)
                .WithMany(c => c.Places)
                .HasForeignKey(p => p.CityId);
            base.OnModelCreating(modelBuilder);
        }

    }
}
