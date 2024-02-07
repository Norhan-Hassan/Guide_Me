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
        }

    }
}
