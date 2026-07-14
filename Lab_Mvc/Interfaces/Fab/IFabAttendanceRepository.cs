using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Fab;

namespace Lab_Mvc.Interfaces.Fab
{
    public interface IFabAttendanceRepository
    {
        Task<bool> CheckAttendanceExists(int userId, string userName, DateTime date);
        Task<bool> MarkAttendance(DTOFabHelperAtt attendance);
        Task<bool> UpdateAttendance(DTOFabHelperAtt attendance);
        Task<bool> DeleteAttendance(int hId);
        Task<IEnumerable<DTOFabHelperAtt>> GetAttendanceByDate(DateTime date);
        Task<IEnumerable<DTOFabHelperAtt>> GetAttendanceByDateRange(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<DTOFabHelperAtt>> GetHelperAttendanceForMonth(int userId, int month, int year);
        Task<DTOFabAttendanceSummary?> GetHelperAttendanceSummary(int userId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<DTOFabHelperAtt>> GetHelperAttendanceByRange(int userId, DateTime fromDate, DateTime toDate);
        Task<int> AutoMarkOffDaysMissingAttendance(DateTime date);
    }
}
