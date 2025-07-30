namespace SocietyMng.Data.Entities
{
    public class SystemCode
    {
        public int Id { get; set; } //primary key
        public string Code { get; set; } //identifier -> "User Role"
        public string Description { get; set; } // "User roles in the system"
        public List<SystemCodeItem> Items { get; set; } = new();    //Admin, Resident, Buyer
    }

    public class SystemCodeItem
    {
        public int Id { get; set; } //primary key
        public int SystemCodeId { get; set; }   //link to SystemCode
        public SystemCode SystemCode { get; set; }  //parent category
        public string Code { get; set; } // Admin, Resident, Buyer
        public string Description { get; set; } // Administrator of site, Society Resident, Property Buyer
        public int SortOrder { get; set; }  //to display in dropdowns
        public bool IsActive { get; set; } = true;
    }
}
