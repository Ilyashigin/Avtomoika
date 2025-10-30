using Microsoft.EntityFrameworkCore;
using Avtomoika;
using Avtomoika.models;

namespace Avtomoika
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
            Database.EnsureCreated();
        }

        
    }
}