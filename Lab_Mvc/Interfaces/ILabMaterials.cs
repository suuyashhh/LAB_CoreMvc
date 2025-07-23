using Models;

namespace Lab_Mvc.Interfaces
{
    public interface ILabMaterials
    {
        Task<IEnumerable<DTOLabMaterials>> GetLabMaterials(int comId);
        Task<DTOLabMaterials> GetLabMaterialsById(long mat_id);
        Task<List<DTOLabMaterials>> GetDateWiseLabMaterials(string from_date, string to_date);
        Task SaveLabMaterials(DTOLabMaterials objMat);
        Task EditLabMaterials(DTOLabMaterials objMat, long mat_id);
        Task DeleteLabMaterials(long mat_id);
    }
}
