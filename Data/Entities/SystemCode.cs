namespace SocietyMng.Data.Entities
{
    public class SystemCode
    {
        public int Id { get; set; } //primary key
        public string Code { get; set; } //identifier -> User role, blocks, property types
        public string Description { get; set; } // what the code stands for
        public List<SystemCodeItem> Items { get; set; } = new();    //Admin, Sales, Buyer
    }

    public class SystemCodeItem
    {
        public int Id { get; set; } //primary key
        public int SystemCodeId { get; set; }   //link to SystemCode
        public SystemCode SystemCode { get; set; }  //parent category
        public string Code { get; set; } // Admin, Sales, Buyer
        public string Description { get; set; } // Administrator of site, Society Resident, Property Buyer
        public int SortOrder { get; set; }  //to display in dropdowns
        public bool IsActive { get; set; } = true;

        public List<User> Users { get; set; } = new();       // For UserRole
        public List<Asset> BlockAssets { get; set; } = new();    // For Block
        public List<Asset> TypeAssets { get; set; } = new();     // For PropertyType
        public List<Asset> StatusAssets { get; set; } = new();
    }

}
