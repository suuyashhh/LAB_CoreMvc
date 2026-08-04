using System;

namespace Models.Notes
{
    public class NoteFile
    {
        public int FileId { get; set; }
        public int FolderId { get; set; }
        public int UserId { get; set; }

        /// <summary>User-visible name (can be renamed without touching disk)</summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>Original file name as uploaded</summary>
        public string OriginalFileName { get; set; } = string.Empty;

        /// <summary>GUID-based name stored on disk (prevents traversal / collisions) - Obsolete</summary>
        public string? StoredFileName { get; set; }

        public string Extension { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }

        /// <summary>Relative path from the NotesFiles root (e.g. "12/3/abc.pdf") - Obsolete</summary>
        public string? StoragePath { get; set; }
        
        /// <summary>SQL Server VARBINARY(MAX) contents</summary>
        public byte[]? FileData { get; set; }

        public int DownloadCount { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class NoteFileDto
    {
        public int FileId { get; set; }
        public int FolderId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public int DownloadCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
