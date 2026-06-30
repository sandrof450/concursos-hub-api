namespace WebApplication_App_Concurso.Models
{
    public class Concurso
    {
        public Guid ConcursoId { get; set; } = Guid.NewGuid();
        public string? Titulo { get; set; } = string.Empty;
        public string? Estado { get; set; } = string.Empty;
        public string? Cidade { get; set; } = string.Empty;
        public string? Orgao { get; set; } = string.Empty;
        public string? Area { get; set; } = string.Empty;
        public int? Vagas { get; set; }
        public string? Nivel { get; set; } = string.Empty;
        public decimal? Salario { get; set; }
        public string? Status { get; set; } = string.Empty;
        public string? Fonte { get; set; } = string.Empty;
        public DateTime DataPublicacao { get; set; }
        public string? Link { get; set; } = string.Empty;
    }
}
