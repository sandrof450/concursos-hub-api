using WebApplication_App_Concurso.Data;
using WebApplication_App_Concurso.DTOs;
using WebApplication_App_Concurso.Exceptions;
using WebApplication_App_Concurso.Models;
using WebApplication_App_Concurso.Models.Filters;
using WebApplication_App_Concurso.Repositories.Interfaces;
using WebApplication_App_Concurso.Scrapers.Interfaces;
using WebApplication_App_Concurso.Services.Interfaces;

namespace WebApplication_App_Concurso.Services
{
    public class ConcursoService : IConcursoService
    {
        private readonly IEnumerable<IScrapingService> _scrapers;
        private readonly IConcursoRepository _concursoRepository;
        private readonly IEstadoNormalizadorService _estadoNormalizador;
        private readonly IFonteProvider _fonteProvider;

        public ConcursoService(
            IEnumerable<IScrapingService> scrapers, 
            IConcursoRepository concursoRepository, 
            IEstadoNormalizadorService estadoNormalizador,
            IFonteProvider fonteProvider
            )
        {
            _scrapers = scrapers;
            _concursoRepository = concursoRepository;
            _estadoNormalizador = estadoNormalizador;
            _fonteProvider = fonteProvider;
        }

        public async Task<List<ConcursoDTO>> GetAllConcursosAsync()
        {
            try
            {
                var concursos = await _concursoRepository.GetAllConcursosAsync();
                if (concursos == null || concursos.Count == 0)
                    throw new ConcursoServiceException("No concursos found in the repository.");
                if (concursos.Any(c => c == null))
                    throw new ConcursoServiceException("One or more concursos in the repository are null.");

                var concursosDTO = concursos.Select(c => new ConcursoDTO
                {
                    ConcursoId = c.ConcursoId.ToString(),
                    Titulo = c.Titulo,
                    Estado = c.Estado,
                    Cidade = c.Cidade,
                    Orgao = c.Orgao,
                    Area = c.Area,
                    Vagas = c.Vagas.ToString(),
                    Nivel = c.Nivel,
                    Salario = c.Salario,
                    Status = c.Status,
                    DataPublicacao = c.DataPublicacao,
                    Link = c.Link,
                    Fonte = c.Fonte,
                }).ToList();
                return concursosDTO;
            }
            catch (Exception ex) when (ex is not ConcursoServiceException)
            {
                throw new ConcursoServiceException("Error while fetching concursos.", ex);
            }
        }

        public async Task<PaginacaoDTO<ConcursoDTO>> GetConcursosAsync(ConcursoFilterDTO filtersDTO)
        {
            try
            {
                var filters = new ConcursoFilter
                {
                    PageNumber = filtersDTO.PageNumber,
                    PageSize = filtersDTO.PageSize,
                    Titulo = filtersDTO.Titulo == null ? null : filtersDTO.Titulo.Trim(),
                    Orgao = filtersDTO.Orgao == null ? null : filtersDTO.Orgao.Trim(),
                    Area = filtersDTO.Area == null ? null : filtersDTO.Area.Trim(),
                    Fonte = filtersDTO.Fonte,
                    Estados = filtersDTO.Estados?
                        .Select(e => e.Equals("Nacional", StringComparison.OrdinalIgnoreCase)
                            ? "NACIONAL"
                            : e.ToUpper())
                        .ToList(),

                };

                var (concursos, totalCounts) = await _concursoRepository.GetConcursosAsync(filters);

                //(totalCounts + tamanho - 1) / tamanho
                var totalPages = (totalCounts + filters.PageSize - 1) / filters.PageSize;

                if (filters.PageNumber > totalPages && totalPages != 0)
                    throw new ConcursoServiceException($"Page number {filters.PageNumber} exceeds total pages {totalPages}.");


                var concursosDTO = concursos.Select(c => new ConcursoDTO
                {
                    ConcursoId = c.ConcursoId.ToString(),
                    Titulo = c.Titulo,
                    Estado = c.Estado,
                    Cidade = c.Cidade,
                    Orgao = c.Orgao,
                    Area = c.Area,
                    Vagas = c.Vagas.ToString(),
                    Nivel = c.Nivel,
                    Salario = c.Salario,
                    Status = c.Status,
                    DataPublicacao = c.DataPublicacao,
                    Link = c.Link,
                    Fonte = c.Fonte,
                }).ToList();

                var concursosPaginados = new PaginacaoDTO<ConcursoDTO>
                {
                    Data = concursosDTO,
                    TotalCount = totalCounts,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize
                };

                return concursosPaginados;
            }
            catch (Exception ex) when (ex is not ConcursoServiceException)
            {
                throw new ConcursoServiceException("Error while fetching concursos.", ex);
            }
        }

        public async Task<List<FonteDTO>> GetAllFontesConcursoAsync()
        {
            try
            {
                var fontes = await _fonteProvider.GetAllFontesAsync();
                if (fontes == null || fontes.Count == 0)
                    throw new ConcursoServiceException("No fontes found in the repository.");

                if (fontes.Any(f => string.IsNullOrEmpty(f)))
                    throw new ConcursoServiceException("One or more fontes in the repository are null or empty.");

                var listFontes = fontes.Select(fonte => new FonteDTO
                {
                    FonteNome = fonte,
                }).ToList();

                return listFontes;
            }
            catch (Exception ex) when (ex is not ConcursoServiceException)
            {
                throw new ConcursoServiceException("Error while fetching fontes.", ex);
            }
        }
        public async Task<EstatisticaDTO> GetEstatisticasAsync()
        {
            try
            {
                var totalConcursos = await _concursoRepository.GetTotalConcursosAsync();
                var totalVagas = await _concursoRepository.GetTotalVagasAsync();
                var totalFontes = await _concursoRepository.GetTotalFontesAsync();

                if (totalConcursos < 0 || totalVagas < 0 || totalFontes < 0)
                    throw new ConcursoServiceException("Statistics values cannot be negative.");

                var estatisticaDTO = new EstatisticaDTO
                {
                    TotalDeConcursos = totalConcursos,
                    TotalDeVagas = totalVagas,
                    TotalDeFontes = totalFontes
                };
                return estatisticaDTO;
            }
            catch (Exception ex) when (ex is not ConcursoServiceException)
            {
                throw new ConcursoServiceException("Error while fetching statistics.", ex);
            }
        }

        // Services/ConcursoService.cs
        public async Task<List<string>> GetEstadosDisponiveisAsync()
        {
            try
            {
                return await _concursoRepository.GetEstadosDisponiveisAsync();
            }
            catch (Exception ex) when (ex is not ConcursoServiceException)
            {
                throw new ConcursoServiceException("Error while fetching estados.", ex);
            }
        }

        public async Task<List<ConcursoDTO>> CreateConcursosAsync()
        {
            await ClearAllConcursosAsync();

            try
            {
                var novosConcursos = new List<ConcursoDTO>();


                foreach (var scraper in _scrapers)
                {
                    //Busca todos os concursos do scraper
                    var concurso = await scraper.GetAllConcursosAsync();

                    //Normaliza o estado do concurso
                    concurso.ForEach(c => c.Estado = _estadoNormalizador.Normalizar(c.Estado));

                    //Adiciona os concursos novos à lista
                    novosConcursos.AddRange(concurso);

                }

                //Verifica se existem concursos novos
                if (novosConcursos.Count == 0)
                    throw new ConcursoServiceException("No concursos found from any scraper.");

                var entities = novosConcursos.Select(c => new Concurso
                {
                    ConcursoId = Guid.NewGuid(),
                    Titulo = c.Titulo,
                    Estado = c.Estado,
                    Cidade = c.Cidade,
                    Orgao = c.Orgao,
                    Area = c.Area,
                    Vagas = int.TryParse(c.Vagas, out var vagas) ? vagas : 0,
                    Nivel = c.Nivel,
                    Salario = c.Salario,
                    Status = c.Status,
                    DataPublicacao = c.DataPublicacao,
                    Link = c.Link,
                    Fonte = c.Fonte
                }).ToList();

                var savedConcursos = await _concursoRepository.SaveConcursosAsync(entities);

                return savedConcursos.Select(c => new ConcursoDTO
                {
                    ConcursoId = c.ConcursoId.ToString(),
                    Titulo = c.Titulo,
                    Estado = c.Estado,
                    Cidade = c.Cidade,
                    Orgao = c.Orgao,
                    Area = c.Area,
                    Vagas = c.Vagas.ToString(),
                    Nivel = c.Nivel,
                    Salario = c.Salario,
                    Status = c.Status,
                    DataPublicacao = c.DataPublicacao,
                    Link = c.Link,
                    Fonte = c.Fonte
                }).ToList();
            }
            catch (Exception ex) when (ex is not ConcursoServiceException)
            {
                throw new ConcursoServiceException("Error while fetching concursos.", ex);
            }
        }

        public async Task ClearAllConcursosAsync()
        {
            try
            {
                //Limpa os concursos antigos
                await _concursoRepository.ClearAllConcursosAsync();

            }
            catch (Exception ex)
            {
                throw new ConcursoServiceException("Error while clearing concursos.", ex);
            }

        }
    }
}
