using System;

namespace Models.Notes
{
    public class NotesUser
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string Number { get; set; }
        public string Password { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
