using Microsoft.EntityFrameworkCore;
using SocietyMng.Data.Entities;
using SocietyMng.Data.SeedData;

namespace SocietyMng.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<SystemCode> SystemCodes { get; set; }
        public DbSet<SystemCodeItem> SystemCodeItems { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //  User -> Role 
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(u => u.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            //  Asset relations
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Block)
                .WithMany(b => b.BlockAssets)
                .HasForeignKey(a => a.BlockId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.PropertyType)
                .WithMany(p => p.TypeAssets)
                .HasForeignKey(a => a.PropertyTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Status)
                .WithMany(s => s.StatusAssets)
                .HasForeignKey(a => a.StatusId)
                .OnDelete(DeleteBehavior.NoAction);

            //  Booking relations
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Asset)
                .WithMany(a => a.Bookings)
                .HasForeignKey(b => b.AssetId)
                .OnDelete(DeleteBehavior.NoAction);

            //  Complaint relations
            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.User)
                .WithMany(u => u.Complaints)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Asset)
                .WithMany(a => a.Complaints)
                .HasForeignKey(c => c.AssetId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Booking)
                .WithMany(b => b.Complaints)
                .HasForeignKey(c => c.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            // seeding sys codes
            modelBuilder.Entity<SystemCode>().HasData(
                new SystemCode { Id = 1, Code = "User_Role", Description = "User roles in the system" },
                new SystemCode { Id = 2, Code = "Block", Description = "Society blocks" },
                new SystemCode { Id = 3, Code = "Property_Type", Description = "Property types" },
                new SystemCode { Id = 4, Code = "Asset_Status", Description = "Asset statuses" }
            );

            // sys code items seeding
            modelBuilder.Entity<SystemCodeItem>().HasData(
                // User Roles
                new SystemCodeItem { Id = 1, SystemCodeId = 1, Code = "Admin", Description = "System Administrator", SortOrder = 1, IsActive = true },
                new SystemCodeItem { Id = 2, SystemCodeId = 1, Code = "Buyer", Description = "Property Buyer", SortOrder = 2, IsActive = true },
                new SystemCodeItem { Id = 3, SystemCodeId = 1, Code = "Sales", Description = "Society Sales", SortOrder = 3, IsActive = true },

                // Blocks 
                new SystemCodeItem { Id = 4, SystemCodeId = 2, Code = "BLOCK_A", Description = "Block A", SortOrder = 1, IsActive = true },
                new SystemCodeItem { Id = 5, SystemCodeId = 2, Code = "BLOCK_B", Description = "Block B", SortOrder = 2, IsActive = true },
                new SystemCodeItem { Id = 6, SystemCodeId = 2, Code = "BLOCK_C", Description = "Block C", SortOrder = 2, IsActive = true },
                new SystemCodeItem { Id = 7, SystemCodeId = 2, Code = "BLOCK_D", Description = "Block D", SortOrder = 2, IsActive = true },

                // Property Types
                new SystemCodeItem { Id = 8, SystemCodeId = 3, Code = "APARTMENT", Description = "Apartment", SortOrder = 1, IsActive = true },
                new SystemCodeItem { Id = 9, SystemCodeId = 3, Code = "COMMERCIAL", Description = "Commercial", SortOrder = 2, IsActive = true },
                new SystemCodeItem { Id = 10, SystemCodeId = 3, Code = "LAND", Description = "Land/Plot", SortOrder = 2, IsActive = true },

                // Asset Statuses
                new SystemCodeItem { Id = 11, SystemCodeId = 4, Code = "AVAILABLE", Description = "Available", SortOrder = 1, IsActive = true },
                new SystemCodeItem { Id = 12, SystemCodeId = 4, Code = "BOOKED", Description = "Booked", SortOrder = 2, IsActive = true },
                new SystemCodeItem { Id = 13, SystemCodeId = 4, Code = "SOLD", Description = "Sold", SortOrder = 3, IsActive = true }
            );

            
            modelBuilder.Entity<User>().HasData(SeedAdmin.AdminUser);
        }
    }
}