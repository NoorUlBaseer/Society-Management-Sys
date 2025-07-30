using SocietyMng.Data.Entities;

namespace SocietyMng.Data.SeedData
{
    public static class StaffSeedData
    {
        public static List<Staff> GetDummyStaff()
        {
            return new List<Staff>
            {
                new Staff
                {
                    Id = 1,
                    FullName = "John Smith",
                    Email = "john.smith@example.com",
                    ContactNumber = "555-0101",
                    Position = "Manager",
                    Salary = 65000.00m,
                    HireDate = new DateTime(2018, 5, 15),
                    IsActive = true,
                    BankAccount = "US1234567890123456"
                },
                new Staff
                {
                    Id = 2,
                    FullName = "Emily Johnson",
                    Email = "emily.j@example.com",
                    ContactNumber = "555-0102",
                    Position = "Accountant",
                    Salary = 55000.00m,
                    HireDate = new DateTime(2019, 3, 22),
                    IsActive = true,
                    BankAccount = "US2345678901234567"
                },
                new Staff
                {
                    Id = 3,
                    FullName = "Michael Williams",
                    Email = "michael.w@example.com",
                    ContactNumber = "555-0103",
                    Position = "Maintenance Supervisor",
                    Salary = 48000.00m,
                    HireDate = new DateTime(2020, 7, 10),
                    IsActive = true,
                    BankAccount = "US3456789012345678"
                },
                new Staff
                {
                    Id = 4,
                    FullName = "Sarah Brown",
                    Email = "sarah.b@example.com",
                    ContactNumber = "555-0104",
                    Position = "Administrative Assistant",
                    Salary = 42000.00m,
                    HireDate = new DateTime(2021, 1, 5),
                    IsActive = true,
                    BankAccount = "US4567890123456789"
                },
                new Staff
                {
                    Id = 5,
                    FullName = "Robert Davis",
                    Email = "robert.d@example.com",
                    ContactNumber = "555-0105",
                    Position = "Security Officer",
                    Salary = 38000.00m,
                    HireDate = new DateTime(2017, 11, 30),
                    IsActive = false,
                    BankAccount = "US5678901234567890"
                },
                new Staff
                {
                    Id = 6,
                    FullName = "Jennifer Miller",
                    Email = "jennifer.m@example.com",
                    ContactNumber = "555-0106",
                    Position = "Community Manager",
                    Salary = 58000.00m,
                    HireDate = new DateTime(2022, 2, 14),
                    IsActive = true,
                    BankAccount = "US6789012345678901"
                },
                new Staff
                {
                    Id = 7,
                    FullName = "David Wilson",
                    Email = "david.w@example.com",
                    ContactNumber = "555-0107",
                    Position = "Maintenance Technician",
                    Salary = 45000.00m,
                    HireDate = new DateTime(2020, 9, 8),
                    IsActive = true,
                    BankAccount = "US7890123456789012"
                },
                new Staff
                {
                    Id = 8,
                    FullName = "Lisa Taylor",
                    Email = "lisa.t@example.com",
                    ContactNumber = "555-0108",
                    Position = "Accountant",
                    Salary = 53000.00m,
                    HireDate = new DateTime(2019, 6, 17),
                    IsActive = true,
                    BankAccount = "US8901234567890123"
                }
            };
        }
    }
}