using System;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebApi.Entity;

namespace WebApi.Data.DataContext
{
    public partial class BMSDbContext : DbContext
    {
        public BMSDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Inventory> Inventorys { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entiy =>
            {
                entiy.ToTable("TB_Book");
                entiy.HasKey("Id");
                entiy.Property(c => c.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Inventory>(entiy =>
            {
                entiy.ToTable("TB_Inventory");
                entiy.HasKey("Id");
                entiy.Property(c => c.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Book>().HasOne(e => e.Inventory).WithOne(e => e.Book).HasForeignKey<Inventory>(e => e.Bookid).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

    }
}
