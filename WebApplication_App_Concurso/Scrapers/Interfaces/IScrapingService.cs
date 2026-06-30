using WebApplication_App_Concurso.DTOs;

namespace WebApplication_App_Concurso.Scrapers.Interfaces
{
    public interface IScrapingService
    {
        Task<List<ConcursoDTO>> GetAllConcursosAsync();
    }
}
