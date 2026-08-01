using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Infrastructure
{
    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<RestaurantCategory> RestaurantCategories { get; set; }
        public DbSet<Ratings> Ratings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=.;Database=MyRestaurantDb;Trusted_Connection=True;TrustServerCertificate=True;"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RestaurantCategory>()
                .HasKey(rc => new { rc.RestaurantId, rc.CategoryId });

            modelBuilder.Entity<Ratings>()
                .Property(r => r.Rating)
                .HasDefaultValue(5);

           modelBuilder.Entity<Ratings>()
    .ToTable(t => t.HasCheckConstraint("CK_Ratings_Rating", "[Rating] >= 1 AND [Rating] <= 5"));


    // to prevent cascade delete path for Order and Restaurant relationship
    modelBuilder.Entity<Order>()
    .HasOne(o => o.Customer)
    .WithMany()
    .HasForeignKey(o => o.CustomerId)
    .OnDelete(DeleteBehavior.Restrict); // أو NoAction

  modelBuilder.Entity<Order>()
    .HasOne(o => o.Restaurant)
    .WithMany()
    .HasForeignKey(o => o.RestaurantId)
    .OnDelete(DeleteBehavior.Restrict);


//to prevent cascade delete from Rating to User and Restaurant
modelBuilder.Entity<Ratings>()
    .HasOne(r => r.User)
    .WithMany()
    .HasForeignKey(r => r.UserId)
    .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Ratings>()
    .HasOne(r => r.Restaurant)
    .WithMany()
    .HasForeignKey(r => r.RestaurantId)
    .OnDelete(DeleteBehavior.Restrict);


        }
    }
}