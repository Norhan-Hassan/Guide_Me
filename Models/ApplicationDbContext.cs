using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Guide_Me.Models
{
    public class ApplicationDbContext : IdentityDbContext<Tourist>
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

        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<History> histories { get; set; }
        public DbSet<Tourist> Tourist { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<RatingSuggestion> RatingSuggestions { get; set; }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<Suggestionplacebyuser> Suggestionplacebyusers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure table name for Tourist entity
            builder.Entity<Tourist>().ToTable("Tourist");
        }


    }
}
