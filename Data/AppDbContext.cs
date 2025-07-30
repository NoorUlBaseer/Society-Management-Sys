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
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Asset> Assets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // user + sys code item 
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<SystemCode>().HasData(
                new SystemCode
                {
                    Id = 1,
                    Code = "User_Role",
                    Description = "User roles in the system"
                });

            // seeding codes
            modelBuilder.Entity<SystemCodeItem>().HasData(
                new SystemCodeItem
                {
                    Id = 1,
                    SystemCodeId = 1,
                    Code = "Admin",
                    Description = "System Administrator",
                    SortOrder = 1,
                    IsActive = true
                },
                new SystemCodeItem
                {
                    Id = 2,
                    SystemCodeId = 1,
                    Code = "Resident",
                    Description = "Society Resident",
                    SortOrder = 2,
                    IsActive = true
                },
                new SystemCodeItem
                {
                    Id = 3,
                    SystemCodeId = 1,
                    Code = "Buyer",
                    Description = "Property Buyer",
                    SortOrder = 3,
                    IsActive = true
                });

            //asset + sys code item
            modelBuilder.Entity<Asset>()
               .HasOne(a => a.RoomCount)
               .WithMany()
               .HasForeignKey(a => a.RoomCountId)
               .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Status)
                .WithMany()
                .HasForeignKey(a => a.StatusId)
                .OnDelete(DeleteBehavior.NoAction);


            //asset + user
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.UploadedByUser)
                .WithMany()
                .HasForeignKey(a => a.UploadedByUserId)
                .OnDelete(DeleteBehavior.NoAction);


            //seeding code
            modelBuilder.Entity<SystemCode>().HasData(
               new SystemCode { Id = 2, Code = "Room_No", Description = "Property room counts" },
               new SystemCode { Id = 3, Code = "Asset_Status", Description = "Rental/Sale statuses" }
            );

            modelBuilder.Entity<SystemCodeItem>().HasData(
                new { Id = 4, SystemCodeId = 2, Code = "1_ROOM", Description = "1 Bedroom", SortOrder = 1, IsActive = true },
                new { Id = 5, SystemCodeId = 2, Code = "2_ROOMS", Description = "2 Rooms, 1 Kitchen", SortOrder = 2, IsActive = true },
                new { Id = 6, SystemCodeId = 2, Code = "3_ROOMS", Description = "3 Rooms, 1 Living room & 1 Kitchen", SortOrder = 3, IsActive = true }
            );

            // assets statuses
            modelBuilder.Entity<SystemCodeItem>().HasData(
                // Rental assets
                new { Id = 7, SystemCodeId = 3, Code = "Rental_avail", Description = "Available for Rent", SortOrder = 1, IsActive = true },
                new { Id = 8, SystemCodeId = 3, Code = "Rented", Description = "Rented", SortOrder = 2, IsActive = true },

                // Sale assets
                new { Id = 9, SystemCodeId = 3, Code = "Sale_avail", Description = "Available for Sale", SortOrder = 3, IsActive = true },
                new { Id = 10, SystemCodeId = 3, Code = "Sold", Description = "Sold", SortOrder = 4, IsActive = true }
            );

            // Seeders
            modelBuilder.Entity<User>().HasData(SeedAdmin.AdminUser);
            modelBuilder.Entity<Staff>().HasData(StaffSeedData.GetDummyStaff());
        }
    }
}