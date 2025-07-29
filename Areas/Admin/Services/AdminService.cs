using SocietyMng.Data;
using SocietyMng.Data.Entities;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public AdminService(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync();
    }

    public async Task BlockUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string> GeneratePropertyDocument(string documentType, int propertyId)
    {
        var property = await _context.Properties.FindAsync(propertyId);
        var templatePath = Path.Combine(_env.WebRootPath, "templates", $"{documentType}Template.docx");

        // Implement document generation logic (using OpenXML, etc.)
        return $"Generated {documentType} document for {property.Name}";
    }

    public async Task UpdateStaffSalary(int staffId, decimal newSalary)
    {
        var staff = await _context.Staff.FindAsync(staffId);
        if (staff != null)
        {
            staff.Salary = newSalary;
            await _context.SaveChangesAsync();
        }
    }

   
}