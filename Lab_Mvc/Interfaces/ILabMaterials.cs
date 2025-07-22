using Models;

namespace Lab_Mvc.Interfaces
{
    public interface ILabMaterials
    {
        Task<IEnumerable<DTOLabMaterials>> GetLabMaterials(int comId);
        Task<DTOLabMaterials> GetLabMaterialsById(long mat_id);
        Task SaveLabMaterials(DTOLabMaterials objMat);
        Task EditLabMaterials(DTOLabMaterials objMat, long mat_id);
        Task DeleteLabMaterials(long mat_id);
    }
}
