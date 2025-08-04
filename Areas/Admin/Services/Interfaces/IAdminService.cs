using SocietyMng.Areas.Admin.DTOs;
using SocietyMng.Data.Entities;

public interface IAdminService
{
    // User Management
    Task<List<User>> GetAllUsersAsync();
    Task BlockUserAsync(int userId);
    Task DeleteUserAsync(int userId);

    //// Asset Management
    Task AddAssetAsync(AssetCreateView model);
    Task<Asset> UpdateAssetAsync(int id, AssetUpdateView model);
    Task DeleteAssetAsync(int assetId);
    Task<List<Asset>> GetAllAssetsAsync();


    // Staff Management
    //Task<List<Staff>> GetAllStaffAsync();
    //Task AddStaffAsync(StaffView model);
    //Task UpdateStaffSalaryAsync(SalaryUpdate model);
    //Task ToggleStaffStatusAsync(int staffId);

    //// Complaint Management
    //Task<List<Complaint>> GetAllComplaintsAsync();
    ////Task ResolveComplaintAsync(int complaintId, string resolution);


}