using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Lab_Mvc.Interfaces.Admin;
using Models.Admin;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;

namespace Lab_Mvc.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioProjectsController : ControllerBase
    {
        private readonly IPortfolioProjectRepository _repository;
        private readonly IWebHostEnvironment _env;

        public PortfolioProjectsController(IPortfolioProjectRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = true)
        {
            var projects = await _repository.GetAllProjectsAsync(onlyActive);
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _repository.GetProjectByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PortfolioProject project)
        {
            var id = await _repository.AddProjectAsync(project);
            return Ok(new { ProjectId = id, Message = "Project added successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PortfolioProject project)
        {
            project.ProjectId = id;
            await _repository.UpdateProjectAsync(project);
            return Ok(new { Message = "Project updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteProjectAsync(id);
            return Ok(new { Message = "Project deleted successfully." });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string type)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var folderName = type.ToLower() switch
            {
                "apk" => "projects/apk",
                "desktop" => "projects/desktop",
                "image" => "projects/images",
                _ => "projects/other"
            };

            var uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path suitable for frontend URL
            var relativePath = $"/uploads/{folderName}/{uniqueFileName}";
            return Ok(new { path = relativePath });
        }
    }
}
