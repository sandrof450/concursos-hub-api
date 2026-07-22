namespace WebApplication_App_Concurso.Services.Interfaces
{
    public interface IFonteProvider
    {
        Task<List<string>> GetAllFontesAsync();
    }
}
