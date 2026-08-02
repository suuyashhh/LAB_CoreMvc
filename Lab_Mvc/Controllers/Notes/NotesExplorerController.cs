using Lab_Mvc.Interfaces.Notes;
using Microsoft.AspNetCore.Mvc;
using Models.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Notes
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesExplorerController : Controller
    {
        private readonly INotesExplorerRepository _repo;

        public NotesExplorerController(INotesExplorerRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("tree/{userId}")]
        public async Task<IActionResult> GetTree(int userId)
        {
            try
            {
                var folders = (await _repo.GetAllFoldersByUser(userId)).ToList();
                var pages = (await _repo.GetAllPagesByUser(userId)).ToList();

                var rootNodes = new List<FolderNode>();
                var dict = new Dictionary<int, FolderNode>();

                foreach (var f in folders)
                {
                    dict[f.FolderId] = new FolderNode
                    {
                        FolderId = f.FolderId,
                        UserId = f.UserId,
                        ParentFolderId = f.ParentFolderId,
                        FolderName = f.FolderName
                    };
                }

                foreach (var f in dict.Values)
                {
                    if (f.ParentFolderId.HasValue && dict.ContainsKey(f.ParentFolderId.Value))
                    {
                        dict[f.ParentFolderId.Value].SubFolders.Add(f);
                    }
                    else
                    {
                        rootNodes.Add(f);
                    }
                }

                foreach (var p in pages)
                {
                    if (dict.ContainsKey(p.FolderId))
                    {
                        dict[p.FolderId].Pages.Add(p);
                    }
                }

                return Ok(rootNodes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("folders")]
        public async Task<IActionResult> CreateFolder([FromBody] NoteFolder folder)
        {
            try
            {
                if (folder == null || string.IsNullOrWhiteSpace(folder.FolderName))
                    return BadRequest("Folder name is required.");

                var created = await _repo.CreateFolder(folder);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("folders")]
        public async Task<IActionResult> UpdateFolder([FromBody] NoteFolder folder)
        {
            try
            {
                if (folder == null || folder.FolderId <= 0 || string.IsNullOrWhiteSpace(folder.FolderName))
                    return BadRequest("Valid Folder Data is required.");

                var success = await _repo.UpdateFolder(folder);
                if (!success) return NotFound();
                return Ok(new { message = "Folder updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("folders/{id}/user/{userId}")]
        public async Task<IActionResult> DeleteFolder(int id, int userId)
        {
            try
            {
                var pages = await _repo.GetAllPagesByUser(userId);
                if (pages.Any(p => p.FolderId == id))
                {
                    return BadRequest("This folder contains pages. Please delete them first.");
                }

                var success = await _repo.DeleteFolder(id, userId);
                if (!success) return NotFound();
                return Ok(new { message = "Folder deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("pages/{pageId}/user/{userId}")]
        public async Task<IActionResult> GetPage(int pageId, int userId)
        {
            try
            {
                var page = await _repo.GetPageById(pageId, userId);
                if (page == null) return NotFound();
                return Ok(page);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("pages")]
        public async Task<IActionResult> CreatePage([FromBody] NotePage page)
        {
            try
            {
                if (page == null || string.IsNullOrWhiteSpace(page.Title) || page.FolderId <= 0)
                    return BadRequest("Title and Folder are required.");

                var created = await _repo.CreatePage(page);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("pages")]
        public async Task<IActionResult> UpdatePage([FromBody] NotePage page)
        {
            try
            {
                if (page == null || page.PageId <= 0 || string.IsNullOrWhiteSpace(page.Title))
                    return BadRequest("Valid Page Data is required.");

                var success = await _repo.UpdatePage(page);
                if (!success) return NotFound();
                return Ok(new { message = "Page updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("pages/{id}/user/{userId}")]
        public async Task<IActionResult> DeletePage(int id, int userId)
        {
            try
            {
                var success = await _repo.DeletePage(id, userId);
                if (!success) return NotFound();
                return Ok(new { message = "Page deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("search/{userId}")]
        public async Task<IActionResult> Search(int userId, [FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return Ok(new { folders = new List<NoteFolder>(), pages = new List<NotePageDto>() });

                var folders = await _repo.SearchFolders(userId, query);
                var pages = await _repo.SearchPages(userId, query);

                return Ok(new { folders, pages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
