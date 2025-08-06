namespace SocietyMng.Configurations
{
    public class AppSettings
    {
        public UserRole User_Role { get; set; } = new();
        public Block Blocks { get; set; } = new();
        public FileUploadPaths FileUploadPath { get; set; } = new();

        public AsssetStatus Asset_Status { get; set; } = new();

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

        public class FileUploadPaths
        {
            public string AssetImages { get; set; }
            public string[] AllowedExtensions { get; set; }
            public int MaxFileSizeMB { get; set; }
        }

        public class AsssetStatus
        {
            public string AVAILABLE { get; set; } = string.Empty;
            public string BOOKED { get; set; } = string.Empty;
            public string SOLD { get; set; } = string.Empty;
        }

    }
}