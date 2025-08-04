using SocietyMng.Areas.Buyer.DTOs;

namespace SocietyMng.Services.Interfaces
{

    public interface IBuyerService
    {
        Task<Profile> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(Profile profile);
        Task<bool> DeleteAccountAsync(int userId);
    }

}
