using WebApplication_App_Concurso.Data;
using WebApplication_App_Concurso.Services.Interfaces;

namespace WebApplication_App_Concurso.Services
{
    public class FonteProvider: IFonteProvider
    {
        public Task<List<string>> GetAllFontesAsync() => FonteData.GetAllFontesAsync();
    }
}
