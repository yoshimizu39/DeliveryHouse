using DeliveryHouse.Common.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Data
{
    public class DataContext : DbContext
    {
        //DbContextOptions invoca a un proveedor de DB, pasa una cadena de conexión como parámetros
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Department> Departments { get; set; }
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
                d.HasMany(d => d.Cities).WithOne(c => c.Department).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<City>(c =>
            {
                c.HasIndex("Name", "DepartmentId").IsUnique();
                c.HasMany(c => c.Stores).WithOne(s => s.City).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Store>(m =>
            {
                m.HasIndex("Name").IsUnique();
            });

            modelBuilder.Entity<Category>(m =>
            {
                m.HasIndex("Name").IsUnique();
            });
        }
    }
}
