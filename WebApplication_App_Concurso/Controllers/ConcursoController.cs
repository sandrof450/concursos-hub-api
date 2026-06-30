using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication_App_Concurso.DTOs;
using WebApplication_App_Concurso.Exceptions;
using WebApplication_App_Concurso.Services.Interfaces;

namespace WebApplication_App_Concurso.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConcursoController : ControllerBase
    {
        private readonly IConcursoService _concursoService;

        public ConcursoController(IConcursoService concursoService)
        {
            _concursoService = concursoService;
        }

        [HttpPost(Name = "CreateConcurso")]
        public async Task<IActionResult> CreateConcurso()
        {
            var response = await _concursoService.CreateConcursosAsync();
            return Ok(response);
        }
        [HttpGet("All",Name = "GetAllConcursos")]
        public async Task<IActionResult> GetAllConcursos()
        {
            var response = await _concursoService.GetAllConcursosAsync();
            return Ok(response);
        }

        [HttpGet(Name = "GetConcursos")]
        public async Task<IActionResult> GetConcursos([FromQuery] ConcursoFilterDTO filters)
        {
            var response = await _concursoService.GetConcursosAsync(filters);
            return Ok(response);
        }

        [HttpGet("ConcursoFontes", Name = "GetAllFontesConcurso")]
        public async Task<IActionResult> GetAllFontesConcurso()
        {
            var response = await _concursoService.GetAllFontesConcursoAsync();
            return Ok(response);
        }

        [HttpGet("Estatisticas", Name = "GetEstatisticas")]
        public async Task<IActionResult> GetEstatisticas()
        {
            var response = await _concursoService.GetEstatisticasAsync();
            return Ok(response);
        }

        [HttpGet("estados")]
        public async Task<IActionResult> GetEstadosDisponiveis()
        {
            var estados = await _concursoService.GetEstadosDisponiveisAsync();
            return Ok(estados);
        }
    }

}
