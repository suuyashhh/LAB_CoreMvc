using Models.Notes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Notes
{
    public interface INotesExplorerRepository
    {
        Task<NoteFolder> CreateFolder(NoteFolder folder);
        Task<bool> UpdateFolder(NoteFolder folder);
        Task<bool> DeleteFolder(int folderId, int userId);
        
        Task<NotePage> CreatePage(NotePage page);
        Task<bool> UpdatePage(NotePage page);
        Task<bool> DeletePage(int pageId, int userId);
        
        Task<NotePage> GetPageById(int pageId, int userId);
        
        Task<IEnumerable<NoteFolder>> GetAllFoldersByUser(int userId);
        Task<IEnumerable<NotePageDto>> GetAllPagesByUser(int userId);
        Task<IEnumerable<NotePageDto>> SearchPages(int userId, string query);
        Task<IEnumerable<NoteFolder>> SearchFolders(int userId, string query);
    }
}
