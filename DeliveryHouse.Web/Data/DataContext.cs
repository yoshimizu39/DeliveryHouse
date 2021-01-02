using DeliveryHouse.Common.Entities;
using DeliveryHouse.Web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeliveryHouse.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        //DbContextOptions invoca a un proveedor de DB, pasa una cadena de conexión como parámetros
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>(c =>
            {
                c.HasIndex("Name").IsUnique();
                c.HasMany(c => c.Departments).WithOne(d => d.Country).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Department>(d =>
            {
                d.HasIndex("Name", "CountryId").IsUnique();
                d.HasOne(d => d.Country).WithMany(c => c.Departments).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<City>(c =>
            {
                c.HasIndex("Name", "DepartmentId").IsUnique();
                c.HasOne(c => c.Department).WithMany(d => d.Cities).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Store>(m =>
            {
                m.HasIndex("Name").IsUnique();
            });

            modelBuilder.Entity<Category>(m =>
            {
                m.HasIndex("Name").IsUnique();
                m.HasMany(p => p.Products).WithOne(c => c.Category).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
