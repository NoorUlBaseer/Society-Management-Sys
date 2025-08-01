namespace SocietyMng.Data.Entities
{
    public class Asset
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Address { get; set; } = null!;
        public string PlotNumber { get; set; } = null!;
        public string ImagePath { get; set; } 
        public decimal Price { get; set; }
        public DateTime DateUploaded { get; set; }

        //from sys code items
        public int BlockId { get; set; }
        public SystemCodeItem Block { get; set; }
        public int PropertyTypeId { get; set; } 
        public SystemCodeItem PropertyType { get; set; } = null!;
        public int StatusId { get; set; } 
        public SystemCodeItem Status { get; set; }

        public List<Booking> Bookings { get; set; } = new();
        public List<Complaint> Complaints { get; set; } = new();
    }
}