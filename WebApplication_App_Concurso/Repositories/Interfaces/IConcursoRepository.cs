using WebApplication_App_Concurso.DTOs;
using WebApplication_App_Concurso.Models;
using WebApplication_App_Concurso.Models.Filters;

namespace WebApplication_App_Concurso.Repositories.Interfaces
{
    public interface IConcursoRepository
    {
        Task<List<Concurso>> GetAllConcursosAsync();
        Task<(List<Concurso> concursos, int totalCounts)> GetConcursosAsync(ConcursoFilter filters);
        Task<List<string>> GetAllFontesConcursoAsync();
        Task<int> GetTotalConcursosAsync();
        Task<int> GetTotalVagasAsync();
        Task<int> GetTotalFontesAsync();
        Task<List<string>> GetEstadosDisponiveisAsync();
        Task<List<Concurso>> SaveConcursosAsync(List<Concurso> novosConcursos);
        Task ClearAllConcursosAsync();
    }
}
