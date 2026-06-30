namespace WebApplication_App_Concurso.Models.Filters
{
    public class ConcursoFilter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Titulo { get; set; }
        public string? Orgao { get; set; }
        public string? Area { get; set; }
        public string? Fonte { get; set; }
        public List<string>? Estados { get; set; }
    }
}
