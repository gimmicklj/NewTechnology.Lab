using System;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;

using WebApi.Entity;

namespace Consumer
{
    public partial class BMSDbContext : DbContext
    {

        private readonly string m_ConnectionString;

        public BMSDbContext(string connectionString)
        {
            m_ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(m_ConnectionString);

            base.OnConfiguring(optionsBuilder);
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

            modelBuilder.Entity<Book>().HasOne(e => e.Inventory).WithOne(e => e.Book).HasForeignKey<Inventory>(e => e.Bookid);

            base.OnModelCreating(modelBuilder);
        }

    }
}
