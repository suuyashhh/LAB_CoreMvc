using Lab_Mvc.Interfaces.Notes;
using Microsoft.AspNetCore.Mvc;
using Models.Notes;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Notes
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesUserController : Controller
    {
        private readonly INotesRepository _notesRepository;

        public NotesUserController(INotesRepository notesRepository)
        {
            _notesRepository = notesRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] NotesUser user)
        {
            try
            {
                if (user == null || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Number) || string.IsNullOrEmpty(user.Password))
                {
                    return BadRequest("Name, Number, and Password are required.");
                }

                var registeredUser = await _notesRepository.RegisterUser(user);
                return Ok(new
                {
                    id = registeredUser.Id,
                    name = registeredUser.Name,
                    number = registeredUser.Number
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] NotesUser loginDto)
        {
            try
            {
                if (loginDto == null || string.IsNullOrEmpty(loginDto.Number) || string.IsNullOrEmpty(loginDto.Password))
                {
                    return BadRequest("Number and Password are required.");
                }

                var user = await _notesRepository.LoginUser(loginDto.Number, loginDto.Password);
                if (user == null)
                {
                    return Unauthorized("Invalid number or password.");
                }

                return Ok(new
                {
                    id = user.Id,
                    name = user.Name,
                    number = user.Number
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
