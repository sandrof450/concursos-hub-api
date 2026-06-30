namespace WebApplication_App_Concurso.DTOs
{
    public class ConcursoDTO
    {
        public string ConcursoId { get; set; }
        public string? Titulo { get; set; } = string.Empty;
        public string? Estado {  get; set; } = string.Empty;
        public string? Cidade { get; set; } = string.Empty;
        public string? Orgao { get; set; } = string.Empty;
        public string? Area { get; set; } = string.Empty;
        public string? Vagas { get; set; } = string.Empty;
        public string? Nivel { get; set; } = string.Empty;
        public decimal? Salario { get; set; }
        public string? Status { get; set; } = string.Empty;
        public DateTime DataPublicacao { get; set; }
        public string Link { get; set; } = string.Empty;     
        public string Fonte { get; set; } = string.Empty;

        public Dictionary<string, string> Extras { get; set; } = new();
    }
}