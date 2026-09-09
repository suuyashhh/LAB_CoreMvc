namespace Models.Admin
{
    public class MainAdmin
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class DTOMainAdminLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
