using System.Threading.Tasks;

namespace WebApplication_App_Concurso.Data
{
    public class FonteData
    {
        public static async Task<List<string>> GetAllFontesAsync() => new()
        {
            "PCI Concursos",
            "Concursos no Brasil",
        };
    }
}
