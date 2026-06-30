namespace WebApplication_App_Concurso.Models
{
    public class Estado
    {
        public int EstadoId { get; set; }
        public string EstadoNome { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
    }
}
