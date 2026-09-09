using Lab_Mvc.Interfaces.Notes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Notes;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Notes
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteFileController : ControllerBase
    {
        private readonly INoteFileRepository _repo;
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        // 50 MB hard cap
        private const long MaxFileSizeBytes = 52_428_800;

        // Allowed extensions (lowercase, with dot)
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".txt", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
            ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg",
            ".zip", ".rar", ".7z",
            ".csv", ".json", ".xml",
            ".mp4", ".mp3",
            ".odt", ".ods", ".odp", ".rtf", ".md"
        };

        // Basic MIME whitelist to prevent disguised executables
        private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "text/plain", "text/csv", "text/xml", "application/xml",
            "text/markdown",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-powerpoint",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "application/vnd.oasis.opendocument.text",
            "application/vnd.oasis.opendocument.spreadsheet",
            "application/vnd.oasis.opendocument.presentation",
            "application/rtf",
            "image/jpeg", "image/png", "image/webp", "image/gif", "image/svg+xml",
            "application/zip", "application/x-rar-compressed", "application/x-7z-compressed",
            "application/json",
            "video/mp4",
            "audio/mpeg",
            // Some browsers/OS report these for zip/rar
            "application/x-zip-compressed", "application/octet-stream"
        };

        public NoteFileController(INoteFileRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
            _connectionString = _config.GetConnectionString("connString");
        }

        // ── Upload ────────────────────────────────────────────────────────────
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] int folderId, [FromForm] int userId, IList<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                    return BadRequest("No files provided.");
                if (folderId <= 0 || userId <= 0)
                    return BadRequest("Valid folderId and userId are required.");

                var results = new List<NoteFile>();

                foreach (var formFile in files)
                {
                    // Size check
                    if (formFile.Length > MaxFileSizeBytes)
                        return BadRequest($"File '{formFile.FileName}' exceeds the 50 MB limit.");

                    // Extension check
                    var originalName = Path.GetFileName(formFile.FileName); // strips path traversal
                    var ext = Path.GetExtension(originalName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext))
                        return BadRequest($"File type '{ext}' is not allowed.");

                    // MIME check
                    var mimeType = formFile.ContentType?.ToLowerInvariant() ?? "application/octet-stream";
                    if (!AllowedMimeTypes.Contains(mimeType))
                        return BadRequest($"MIME type '{mimeType}' is not allowed.");

                    byte[] fileBytes;
                    using (var ms = new MemoryStream())
                    {
                        await formFile.CopyToAsync(ms);
                        fileBytes = ms.ToArray();
                    }

                    // Strip extension from original name to use as display name
                    var displayName = Path.GetFileNameWithoutExtension(originalName);

                    var nf = new NoteFile
                    {
                        FolderId = folderId,
                        UserId = userId,
                        DisplayName = displayName,
                        OriginalFileName = originalName,
                        Extension = ext,
                        MimeType = mimeType,
                        SizeBytes = formFile.Length,
                        FileData = fileBytes
                    };

                    var saved = await _repo.UploadFile(nf);
                    results.Add(saved);
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Get files by folder ───────────────────────────────────────────────
        [HttpGet("folder/{folderId}/user/{userId}")]
        public async Task<IActionResult> GetByFolder(int folderId, int userId)
        {
            try
            {
                var files = await _repo.GetFilesByFolder(folderId, userId);
                return Ok(files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Get single file metadata ──────────────────────────────────────────
        [HttpGet("{fileId}/user/{userId}")]
        public async Task<IActionResult> GetFile(int fileId, int userId)
        {
            try
            {
                var file = await _repo.GetFileById(fileId, userId);
                if (file == null) return NotFound();
                return Ok(file);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Download / Stream ─────────────────────────────────────────────────
        [HttpGet("download/{fileId}/user/{userId}")]
        public async Task<IActionResult> Download(int fileId, int userId)
        {
            try
            {
                var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                
                var cmd = new SqlCommand("SELECT MimeType, DisplayName, Extension, FileData FROM Notes_Files WHERE FileId=@FileId AND UserId=@UserId AND Status='Active'", conn);
                cmd.Parameters.AddWithValue("@FileId", fileId);
                cmd.Parameters.AddWithValue("@UserId", userId);

                var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                if (!await reader.ReadAsync())
                {
                    conn.Dispose();
                    return NotFound("File not found in database.");
                }

                // Increment download counter (fire and forget via repo wrapper)
                _ = _repo.IncrementDownloadCount(fileId);

                var mimeType = reader.GetString(0);
                var displayName = reader.GetString(1);
                var ext = reader.GetString(2);
                var stream = reader.GetStream(3);

                // Tie connection and reader to Response lifecycle so they close when download completes
                Response.RegisterForDispose(reader);
                Response.RegisterForDispose(conn);

                var downloadName = WebUtility.UrlEncode($"{displayName}{ext}");
                return File(stream, mimeType, $"{displayName}{ext}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Stream for inline preview (PDF / images / video / audio) ─────────
        [HttpGet("preview/{fileId}/user/{userId}")]
        public async Task<IActionResult> Preview(int fileId, int userId)
        {
            try
            {
                var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                
                var cmd = new SqlCommand("SELECT MimeType, DisplayName, Extension, FileData FROM Notes_Files WHERE FileId=@FileId AND UserId=@UserId AND Status='Active'", conn);
                cmd.Parameters.AddWithValue("@FileId", fileId);
                cmd.Parameters.AddWithValue("@UserId", userId);

                var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                if (!await reader.ReadAsync())
                {
                    conn.Dispose();
                    return NotFound("File not found in database.");
                }

                var mimeType = reader.GetString(0);
                var displayName = reader.GetString(1);
                var ext = reader.GetString(2);
                var stream = reader.GetStream(3);

                Response.RegisterForDispose(reader);
                Response.RegisterForDispose(conn);

                Response.Headers.Append("Content-Disposition", $"inline; filename=\"{displayName}{ext}\"");
                return File(stream, mimeType, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Rename (display name only) ────────────────────────────────────────
        [HttpPut("rename")]
        public async Task<IActionResult> Rename([FromBody] RenameFileRequest req)
        {
            try
            {
                if (req == null || req.FileId <= 0 || req.UserId <= 0 || string.IsNullOrWhiteSpace(req.NewDisplayName))
                    return BadRequest("FileId, UserId and NewDisplayName are required.");

                // Sanitise: no path characters allowed in display name
                var clean = req.NewDisplayName.Trim();
                foreach (var c in Path.GetInvalidFileNameChars())
                    clean = clean.Replace(c.ToString(), "");

                if (string.IsNullOrEmpty(clean))
                    return BadRequest("Invalid display name.");

                var success = await _repo.RenameFile(req.FileId, req.UserId, clean);
                if (!success) return NotFound();
                return Ok(new { message = "File renamed.", newDisplayName = clean });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Delete ────────────────────────────────────────────────────────────
        [HttpDelete("{fileId}/user/{userId}")]
        public async Task<IActionResult> Delete(int fileId, int userId)
        {
            try
            {
                // Delete removes the database row entirely (along with the VARBINARY payload)
                var file = await _repo.DeleteFile(fileId, userId);
                if (file == null) return NotFound();

                return Ok(new { message = "File deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Search ────────────────────────────────────────────────────────────
        [HttpGet("search/{userId}")]
        public async Task<IActionResult> Search(int userId, [FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return Ok(new List<NoteFileDto>());

                var results = await _repo.SearchFiles(userId, query);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Migration Tool (One-Time Run) ─────────────────────────────────────
        [HttpPost("migrate-physical-to-db")]
        public async Task<IActionResult> MigratePhysicalToDb([FromServices] IWebHostEnvironment env)
        {
            try
            {
                int migrated = 0;
                int failed = 0;

                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    
                    // Find files that haven't been migrated yet (FileData IS NULL)
                    var query = "SELECT FileId, StoragePath FROM Notes_Files WHERE FileData IS NULL AND StoragePath IS NOT NULL AND StoragePath != ''";
                    var filesToMigrate = new List<(int FileId, string StoragePath)>();

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            filesToMigrate.Add((reader.GetInt32(0), reader.GetString(1)));
                        }
                    }

                    foreach (var file in filesToMigrate)
                    {
                        var absolutePath = Path.Combine(
                            env.ContentRootPath, 
                            "NotesFiles", 
                            file.StoragePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar));

                        if (System.IO.File.Exists(absolutePath))
                        {
                            var bytes = await System.IO.File.ReadAllBytesAsync(absolutePath);
                            
                            using (var updateCmd = new SqlCommand("UPDATE Notes_Files SET FileData = @FileData WHERE FileId = @FileId", conn))
                            {
                                updateCmd.Parameters.AddWithValue("@FileId", file.FileId);
                                updateCmd.Parameters.Add("@FileData", SqlDbType.VarBinary, -1).Value = bytes;
                                await updateCmd.ExecuteNonQueryAsync();
                            }
                            
                            // Delete physical file safely since it's stored in DB now
                            try { System.IO.File.Delete(absolutePath); } catch { /* ignore if locked */ }
                            
                            migrated++;
                        }
                        else
                        {
                            failed++;
                        }
                    }
                }

                return Ok(new { message = $"Migration complete.", migratedCount = migrated, failedCount = failed });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class RenameFileRequest
    {
        public int FileId { get; set; }
        public int UserId { get; set; }
        public string NewDisplayName { get; set; } = string.Empty;
    }
}
