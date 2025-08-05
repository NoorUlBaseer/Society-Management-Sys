using SocietyMng.Areas.Admin.DTOs;
using SocietyMng.Areas.Buyer.DTOs;
using SocietyMng.Data.Entities;

namespace SocietyMng.Services.Interfaces
{

    public interface IBuyerService
    {
        Task<Profile> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(Profile profile);
        //Task<bool> ChangePasswordAsync(int userId, string newPassword);
        Task<bool> DeleteAccountAsync(int userId);
        Task<List<Asset>> GetAllAssetsAsync();
    }

}
