namespace SocietyMng.Configurations
{
    public class AppSettings
    {
        public UserRole User_Role { get; set; } = new(); 
        public Block Blocks { get; set; } = new();

        public class UserRole  
        {
            public string Admin { get; set; } = string.Empty;
            public string Buyer { get; set; } = string.Empty;
            public string Sales { get; set; } = string.Empty;
        }

        public class Block
        {
            public string BLOCK_A { get; set; } = string.Empty;
            public string BLOCK_B { get; set; } = string.Empty;
            public string BLOCK_C { get; set; } = string.Empty;
            public string BLOCK_D { get; set; } = string.Empty;
        }
    }
}