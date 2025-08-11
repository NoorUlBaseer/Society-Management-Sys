using SocietyMng.Areas.Buyer.ModelView;
using SocietyMng.Data.Entities;

namespace SocietyMng.Services.Interfaces
{
    public interface IBuyerService
    {
        Task<Profile> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(Profile profile);
        Task<bool> DeleteAccountAsync(int userId);
        Task<List<Asset>> GetAllAssetsAsync();
        Task<Asset> GetAssetByIdAsync(int assetId);
        Task<BookingResult> BookAssetAsync(int userId, int assetId);
        Task<BookingResult> CancelBookingAsync(int userId, int bookingId);
        Task<List<Booking>> GetUserBookingsAsync(int userId);
        Task<(long minPrice, long maxPrice)> GetPriceRangeAsync();
        //Task<List<Complaint>> GetUserComplaintsAsync(int userId);
    }
}