using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Guide_Me.Models
{
    public class ApplicationDbContext :IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<Place> Places { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<PlaceMedia> placeMedias { get; set; }

        public DbSet<PlaceItem> placeItem { get; set; }

        public DbSet<PlaceItemMedia> placeItemMedias { get; set; }
        public DbSet<History> Histories { get; set; }

        public DbSet<Favorites> Favorites { get; set; }

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

            modelBuilder.Entity<Place>()
                .HasMany(p => p.PlaceItems)
                .WithOne(placeItem => placeItem.place)
                .HasForeignKey(placeItem => placeItem.placeID);

            modelBuilder.Entity<PlaceItem>()
                .HasMany(p => p.PlaceItemMedias)
                .WithOne(placeItemMedia=>placeItemMedia.placeItem)
                .HasForeignKey(placeItemMedia=>placeItemMedia.placeItemID);
            modelBuilder.Entity<Favorites>()
                .HasOne(p => p.tourist)
                .WithMany() // Assuming a Tourist can have multiple preferences
                .HasForeignKey(p => p.TouristID);

            base.OnModelCreating(modelBuilder);
        }

    }
}
