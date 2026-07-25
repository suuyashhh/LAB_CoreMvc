using Models.BillingApp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.BillingApp
{
    public interface IProductEntries
    {
        Task<IEnumerable<DTOProductEntries>> GetAllEntries();
        Task<bool> SaveProductEntry(DTOProductEntries entry);
        Task<bool> UpdateProductEntry(DTOProductEntries entry);
    }
}
