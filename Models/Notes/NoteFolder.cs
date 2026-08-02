using System;

namespace Models.Notes
{
    public class NoteFolder
    {
        public int FolderId { get; set; }
        public int UserId { get; set; }
        public int? ParentFolderId { get; set; }
        public string FolderName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
