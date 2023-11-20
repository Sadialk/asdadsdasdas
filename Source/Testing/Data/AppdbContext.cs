using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Testing.Auth.model;
using Testing.Data.Entities;

namespace Testing.Data
{
    public class AppdbContext : IdentityDbContext<RentUser>
    {
        public readonly IConfiguration _configuration;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<City>().ToTable("Cities");
            modelBuilder.Entity<Region>().ToTable("Regions");
            modelBuilder.Entity<Location>().ToTable("Locations");
        }
        public DbSet<City> cities { get; set; }
        public DbSet<Region> regions { get; set; }
        public DbSet<Location> locations { get; set; }
        public AppdbContext(DbContextOptions<AppdbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DigitalOceanDBConnection"));
        }
    }
}
