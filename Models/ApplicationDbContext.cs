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
        public DbSet<PlaceMedia> placeMedias { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entity mappings here
            modelBuilder.Entity<Place>().HasKey(p => p.Id);
            modelBuilder.Entity<City>().HasKey(c => c.Id);
            modelBuilder.Entity<PlaceMedia>().HasKey(placemedia => placemedia.Id);

            // Define relationships (if any)
            modelBuilder.Entity<Place>()
                .HasOne(p => p.City)
                .WithMany(c => c.Places)
                .HasForeignKey(p => p.CityId);

            modelBuilder.Entity<Place>()
                .HasMany(p=>p.PlaceMedias)
                .WithOne(placemedia=> placemedia.Place)
                .HasForeignKey(pm => pm.PlaceId);




            base.OnModelCreating(modelBuilder);
        }

    }
}
