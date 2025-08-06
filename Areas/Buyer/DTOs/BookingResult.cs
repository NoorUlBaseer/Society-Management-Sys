namespace SocietyMng.Areas.Buyer.DTOs
{
    public class BookingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? BookingId { get; set; }
        public DateTime? BookingDate { get; set; }
        public int? AssetId { get; set; }
        public string? AssetDescription { get; set; }
        public string? StatusCode { get; set; }

        private BookingResult() { }

        public static BookingResult SuccessResult(
            string message,
            int bookingId,
            DateTime bookingDate,
            int assetId,
            string assetDescription,
            string statusCode)
        {
            return new BookingResult
            {
                Success = true,
                Message = message,
                BookingId = bookingId,
                BookingDate = bookingDate,
                AssetId = assetId,
                AssetDescription = assetDescription,
                StatusCode = statusCode
            };
        }

        public static BookingResult Failure(string message)
        {
            return new BookingResult
            {
                Success = false,
                Message = message
            };
        }
    }
}
