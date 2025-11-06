using Avtomoika.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Avtomoika.Infrastructure.Persistence
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Service> Services => Set<Service>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Car)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Order)
                .WithOne(o => o.Client)
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Car>()
                .HasMany(c => c.Order)
                .WithOne(o => o.Car)
                .HasForeignKey(o => o.CarId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Services)
                .WithMany(s => s.Orders)
                .UsingEntity(j => j.ToTable("OrderServices"));
            
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);
        }
    }
}