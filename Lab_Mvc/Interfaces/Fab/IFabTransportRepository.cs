using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Fab;

namespace Lab_Mvc.Interfaces.Fab
{
    public interface IFabTransportRepository
    {
        Task<IEnumerable<DTOFabExpanse>> GetTransportHistory(DateTime fromDate, DateTime toDate);
    }
}
