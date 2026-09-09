using System;

namespace Models.Fab
{
    public class DTOFabLogin
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Type { get; set; } // "Admin" or "Helper"
        
        // Response fields
        public int? User_id { get; set; }
        public string? User_name { get; set; }
        public string? User_contact { get; set; }
    }
}
