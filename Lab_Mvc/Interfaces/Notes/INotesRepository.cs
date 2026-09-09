using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Notes;

namespace Lab_Mvc.Interfaces.Notes
{
    public interface INotesRepository
    {
        Task<NotesUser> RegisterUser(NotesUser user);
        Task<NotesUser> LoginUser(string number, string password);
        Task<NoteItem> CreateNote(NoteItem note);
        Task<IEnumerable<NoteItem>> GetNotesByUserId(int userId, string searchQuery = null);
        Task<bool> UpdateNote(NoteItem note);
        Task<bool> DeleteNote(int noteId, int userId);
    }
}
