using Microsoft.EntityFrameworkCore;
using Models;
using Models.Lab;

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
