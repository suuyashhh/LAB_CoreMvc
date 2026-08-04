using Models.Notes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Notes
{
    public interface INoteFileRepository
    {
        /// <summary>Inserts a file record. StoredFileName, StoragePath, etc. are set by the controller before calling.</summary>
        Task<NoteFile> UploadFile(NoteFile file);

        /// <summary>Returns all files belonging to a folder, scoped to the user.</summary>
        Task<IEnumerable<NoteFile>> GetFilesByFolder(int folderId, int userId);

        /// <summary>Returns all file DTOs for a user (used by tree builder).</summary>
        Task<IEnumerable<NoteFileDto>> GetAllFilesByUser(int userId);

        /// <summary>Returns full file record for a single file, owner-scoped.</summary>
        Task<NoteFile?> GetFileById(int fileId, int userId);

        /// <summary>Renames only the display name; never touches the disk.</summary>
        Task<bool> RenameFile(int fileId, int userId, string newDisplayName);

        /// <summary>Returns the file record so the caller can delete the physical file, then removes the DB row.</summary>
        Task<NoteFile?> DeleteFile(int fileId, int userId);

        /// <summary>Increments the download counter (fire-and-forget safe).</summary>
        Task IncrementDownloadCount(int fileId);

        /// <summary>Searches display names for a user.</summary>
        Task<IEnumerable<NoteFileDto>> SearchFiles(int userId, string query);

        /// <summary>Returns true if a folder has at least one active file.</summary>
        Task<bool> FolderHasFiles(int folderId, int userId);
    }
}
