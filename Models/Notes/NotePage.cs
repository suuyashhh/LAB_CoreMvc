using System;

namespace Models.Notes
{
    public class NotePage
    {
        public int PageId { get; set; }
        public int FolderId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
