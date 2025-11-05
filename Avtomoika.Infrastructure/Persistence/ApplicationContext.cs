


using Avtomoika.Domain.Entities;
using Avtomoika.Aplication.Orders.Dto;
using Microsoft.EntityFrameworkCore;

namespace Avtomoika.Infrastructure.Persistence
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Car> Cars { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Client -> Car ===
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Car)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // === Client -> Order ===
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(cl => cl.Order)
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            // === Car -> Order ===
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Car)
                .WithMany(c => c.Order)
                .HasForeignKey(o => o.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // === Order -> Service (многие-ко-многим) ===
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Services)
                .WithMany(s => s.Orders)
                .UsingEntity(j => j.ToTable("OrderServices"));

            
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
