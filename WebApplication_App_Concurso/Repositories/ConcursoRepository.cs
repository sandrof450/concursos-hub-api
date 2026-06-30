using Microsoft.EntityFrameworkCore;
using WebApplication_App_Concurso.DTOs;
using WebApplication_App_Concurso.Exceptions;
using WebApplication_App_Concurso.Models;
using WebApplication_App_Concurso.Models.Filters;
using WebApplication_App_Concurso.Repositories.Contexts;
using WebApplication_App_Concurso.Repositories.Interfaces;

namespace WebApplication_App_Concurso.Repositories
{
    public class ConcursoRepository : IConcursoRepository
    {
        private readonly DataContext _context;

        public ConcursoRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Concurso>> GetAllConcursosAsync()
        {
            var concurso = await _context.Concursos.AsNoTracking().ToListAsync();
            
            return concurso;
        }

        public async Task<(List<Concurso> concursos, int totalCounts)> GetConcursosAsync(ConcursoFilter filters)
        {
            var page = filters.PageNumber;
            var limit = filters.PageSize;
            var skip = (page - 1) * limit;


            var query = _context.Concursos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(filters.Titulo))
                query = query.Where(c => c.Titulo.Contains(filters.Titulo));
            if (!string.IsNullOrEmpty(filters.Orgao))
                query = query.Where(c => c.Orgao.Contains(filters.Orgao));
            if (!string.IsNullOrEmpty(filters.Area))
                query = query.Where(c => c.Area.Contains(filters.Area));
            if (!string.IsNullOrEmpty(filters.Fonte))
                query = query.Where(c => c.Fonte.Contains(filters.Fonte));
            if (filters.Estados != null && filters.Estados.Any())
                query = query.Where(c => filters.Estados.Contains(c.Estado));


            var totalCount = await query.CountAsync();

            var concursos = await query
                .OrderBy(c => c.Estado)
                .Skip(skip) // Pula N registros
                .Take(limit) // Pega os próximos N registros
                .ToListAsync();// Executa a query no banco e retorna uma List<T>

            return (concursos, totalCount);
        }
        public async Task<List<string>> GetAllFontesConcursoAsync()
        {
            var fontes = await _context.Concursos
                .AsNoTracking()
                .Select(c => c.Fonte)
                .Distinct()
                .ToListAsync();
            return fontes;
        }

        public async Task<int> GetTotalConcursosAsync() =>
            await _context.Concursos.CountAsync();

        public async Task<int> GetTotalVagasAsync() =>
            await _context.Concursos
                .Where(c => c.Vagas != null)
                .SumAsync(c => c.Vagas) ?? 0;

        public async Task<int> GetTotalFontesAsync() =>
            await _context.Concursos
                .Select(c => c.Fonte)
                .Distinct()
                .CountAsync();

        // Repositories/ConcursoRepository.cs
        public async Task<List<string>> GetEstadosDisponiveisAsync()
        {
            return await _context.Concursos
                .Where(c => c.Estado != null && c.Estado != "")
                .AsNoTracking()
                .Select(c => c.Estado!.ToUpper())
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();
        }

        public async Task<List<Concurso>> SaveConcursosAsync(List<Concurso> novosConcursos)
        {
            try
            {
                await _context.Concursos.AddRangeAsync(novosConcursos);

                var result = await _context.SaveChangesAsync();

                if (result == 0)
                    throw new RepositoryException("Não foi possível salvar os concursos no banco.");
                return novosConcursos;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Ocorreu um erro ao limpar os concursos no banco.", ex);
            }


        }

        public async Task ClearAllConcursosAsync()
        {
            try
            {
                //Limpa a tabela de concursos
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM public.\"Concursos\";");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Ocorreu um erro ao limpar os concursos no banco.", ex);
            }
        }
    }
}
