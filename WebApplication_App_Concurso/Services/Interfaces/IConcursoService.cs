using WebApplication_App_Concurso.DTOs;
using WebApplication_App_Concurso.Models;

namespace WebApplication_App_Concurso.Services.Interfaces
{
    public interface IConcursoService
    {
        Task<List<ConcursoDTO>> GetAllConcursosAsync();
        Task<PaginacaoDTO<ConcursoDTO>> GetConcursosAsync(ConcursoFilterDTO filters);
        Task<List<FonteDTO>> GetAllFontesConcursoAsync();
        Task<EstatisticaDTO> GetEstatisticasAsync();
        Task<List<string>> GetEstadosDisponiveisAsync();
        Task<List<ConcursoDTO>> CreateConcursosAsync();
        Task ClearAllConcursosAsync();
    }
}
