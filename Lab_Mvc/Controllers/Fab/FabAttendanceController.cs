using Lab_Mvc.Interfaces.Fab;
using Microsoft.AspNetCore.Mvc;
using Models.Fab;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Controllers.Fab
{
    [ApiController]
    [Route("api/Fab")]
    public class FabAttendanceController : ControllerBase
    {
        private readonly IFabAttendanceRepository _attendanceRepository;

        public FabAttendanceController(IFabAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        [HttpGet("Attendance/Check")]
        public async Task<IActionResult> CheckAttendanceExists([FromQuery] int userId, [FromQuery] string userName, [FromQuery] string date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                bool exists = await _attendanceRepository.CheckAttendanceExists(userId, userName, dt);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Attendance")]
        public async Task<IActionResult> MarkAttendance([FromBody] DTOFabHelperAtt attendance)
        {
            try
            {
                bool success = await _attendanceRepository.MarkAttendance(attendance);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("Attendance")]
        public async Task<IActionResult> UpdateAttendance([FromBody] DTOFabHelperAtt attendance)
        {
            try
            {
                bool success = await _attendanceRepository.UpdateAttendance(attendance);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("Attendance/{hId}")]
        public async Task<IActionResult> DeleteAttendance(int hId)
        {
            try
            {
                bool success = await _attendanceRepository.DeleteAttendance(hId);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Attendance/Date")]
        public async Task<IActionResult> GetAttendanceByDate([FromQuery] string date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                var attendance = await _attendanceRepository.GetAttendanceByDate(dt);
                return Ok(attendance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Attendance/Range")]
        public async Task<IActionResult> GetAttendanceByRange([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to = DateTime.Parse(toDate);
                var attendance = await _attendanceRepository.GetAttendanceByDateRange(from, to);
                return Ok(attendance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Attendance/Helper/Month")]
        public async Task<IActionResult> GetHelperAttendanceForMonth([FromQuery] int userId, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var attendance = await _attendanceRepository.GetHelperAttendanceForMonth(userId, month, year);
                return Ok(attendance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Attendance/Helper/Summary")]
        [HttpGet("Helper/{userId}/Summary")]
        public async Task<IActionResult> GetHelperAttendanceSummary(int userId, [FromQuery] string fromDate, [FromQuery] string toDate)
        {
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to = DateTime.Parse(toDate);
                var summary = await _attendanceRepository.GetHelperAttendanceSummary(userId, from, to);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Helper/{userId}/Attendance")]
        public async Task<IActionResult> GetHelperAttendanceByRange(int userId, [FromQuery] string fromDate, [FromQuery] string toDate)
        {
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to = DateTime.Parse(toDate);
                var attendance = await _attendanceRepository.GetHelperAttendanceByRange(userId, from, to);
                return Ok(attendance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Attendance/AutoMarkOff")]
        public async Task<IActionResult> AutoMarkOffDays([FromQuery] string date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                int count = await _attendanceRepository.AutoMarkOffDaysMissingAttendance(dt);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
