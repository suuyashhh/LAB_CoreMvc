using System;

namespace Models.Notes
{
    public class NoteItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
