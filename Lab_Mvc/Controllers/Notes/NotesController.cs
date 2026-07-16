using Lab_Mvc.Interfaces.Notes;
using Microsoft.AspNetCore.Mvc;
using Models.Notes;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Notes
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        private readonly INotesRepository _notesRepository;

        public NotesController(INotesRepository notesRepository)
        {
            _notesRepository = notesRepository;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetNotes(int userId, [FromQuery] string searchQuery = null)
        {
            try
            {
                var notes = await _notesRepository.GetNotesByUserId(userId, searchQuery);
                return Ok(notes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NoteItem note)
        {
            try
            {
                if (note == null || string.IsNullOrEmpty(note.Title) || string.IsNullOrEmpty(note.Content) || note.UserId <= 0)
                {
                    return BadRequest("Title, Content, and valid UserId are required.");
                }

                var createdNote = await _notesRepository.CreateNote(note);
                return Ok(createdNote);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NoteItem note)
        {
            try
            {
                if (note == null || note.Id <= 0 || string.IsNullOrEmpty(note.Title) || string.IsNullOrEmpty(note.Content) || note.UserId <= 0)
                {
                    return BadRequest("Id, Title, Content, and UserId are required for update.");
                }

                var success = await _notesRepository.UpdateNote(note);
                if (!success)
                {
                    return NotFound("Note not found or unauthorized to update.");
                }

                return Ok(new { message = "Note updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}/user/{userId}")]
        public async Task<IActionResult> Delete(int id, int userId)
        {
            try
            {
                var success = await _notesRepository.DeleteNote(id, userId);
                if (!success)
                {
                    return NotFound("Note not found or unauthorized to delete.");
                }

                return Ok(new { message = "Note deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
