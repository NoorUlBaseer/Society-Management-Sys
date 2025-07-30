namespace SocietyMng.Data.Entities
{
    public class Asset
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int RoomCountId { get; set; } // links to SystemCodeItem room num
        public int StatusId { get; set; } // Links to SystemCodeItem rent/sold/available
        public string ImagePath { get; set; } 
        public decimal Price { get; set; }
        public DateTime DateUploaded { get; set; }
        public int UploadedByUserId { get; set; } 
        
        // Navigation properties
        public SystemCodeItem RoomCount { get; set; }
        public SystemCodeItem Status { get; set; }
        public User UploadedByUser { get; set; }
    }
}