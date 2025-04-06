using Microsoft.EntityFrameworkCore;
using Models;

namespace Lab_Mvc.Contest
{
    public class LabMvcDBContext : DbContext
    {
        public LabMvcDBContext(DbContextOptions<LabMvcDBContext> options) : base(options)
        {
        }
        public DbSet<DTOTest> dTOTests { get; set; }
    }
}
