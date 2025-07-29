using SocietyMng.Data.Entities;

public interface IAdminService
{
    Task<List<User>> GetAllUsersAsync();
    Task BlockUserAsync(int userId);
    Task DeleteUserAsync(int userId);
    Task<string> GeneratePropertyDocument(string documentType, int propertyId);
    Task UpdateStaffSalary(int staffId, decimal newSalary);
    Task CreateAnnouncement(Announcement announcement);
    Task ResolveComplaint(int complaintId, string resolution);
    //Task<SystemReport> GenerateSystemReport(DateTime startDate, DateTime endDate);
}