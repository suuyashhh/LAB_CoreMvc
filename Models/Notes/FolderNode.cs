using System.Collections.Generic;

namespace Models.Notes
{
    public class FolderNode
    {
        public int FolderId { get; set; }
        public int UserId { get; set; }
        public int? ParentFolderId { get; set; }
        public string FolderName { get; set; }
        
        public List<FolderNode> SubFolders { get; set; } = new List<FolderNode>();
        public List<NotePageDto> Pages { get; set; } = new List<NotePageDto>();
    }

    public class NotePageDto
    {
        public int PageId { get; set; }
        public int FolderId { get; set; }
        public string Title { get; set; }
    }
}
