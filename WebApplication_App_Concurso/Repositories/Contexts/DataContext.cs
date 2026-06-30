using Microsoft.EntityFrameworkCore;
using WebApplication_App_Concurso.Models;

namespace WebApplication_App_Concurso.Repositories.Contexts
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Concurso> Concursos { get; set; }
    }
}
