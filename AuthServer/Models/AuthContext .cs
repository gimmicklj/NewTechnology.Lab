using System;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AuthServer.Models
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entiy =>
            {
                entiy.ToTable("TB_User");
                entiy.HasKey("Id");
                entiy.Property(c => c.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
