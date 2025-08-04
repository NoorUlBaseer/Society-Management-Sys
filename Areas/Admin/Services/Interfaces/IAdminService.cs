using SocietyMng.Areas.Admin.DTOs;
using SocietyMng.Data.Entities;

public interface IAdminService
{
    // User Management
    Task<List<User>> GetAllUsersAsync();
    Task BlockUserAsync(int userId);
    Task DeleteUserAsync(int userId);

    // Staff Management
    //Task<List<Staff>> GetAllStaffAsync();
    //Task AddStaffAsync(StaffView model);
    //Task UpdateStaffSalaryAsync(SalaryUpdate model);
    //Task ToggleStaffStatusAsync(int staffId);

    //// Complaint Management
    //Task<List<Complaint>> GetAllComplaintsAsync();
    ////Task ResolveComplaintAsync(int complaintId, string resolution);

    //// Asset Management
    //Task<List<Asset>> GetAllAssetsAsync();
    Task AddAssetAsync(AssetCreateView model);
    //Task UpdateAssetAsync(AssetUpdateView model);
    //Task DeleteAssetAsync(int assetId);
    //Task ToggleAssetStatusAsync(int assetId);

}