using WebApplication_App_Concurso.Models;

namespace WebApplication_App_Concurso.Data.Seed
{
    public class EstadoSeed
    {
        public static List<Estado> GetEstados() => new()
        {
            new Estado { EstadoNome = "Nacional",              Uf = "NACIONAL"},
            new Estado { EstadoNome = "Acre",                  Uf = "AC" },
            new Estado { EstadoNome = "Alagoas",               Uf = "AL" },
            new Estado { EstadoNome = "Amapá",                 Uf = "AP" },
            new Estado { EstadoNome = "Amazonas",              Uf = "AM" },
            new Estado { EstadoNome = "Bahia",                 Uf = "BA" },
            new Estado { EstadoNome = "Ceará",                 Uf = "CE" },
            new Estado { EstadoNome = "Distrito Federal",      Uf = "DF" },
            new Estado { EstadoNome = "Espírito Santo",        Uf = "ES" },
            new Estado { EstadoNome = "Goiás",                 Uf = "GO" },
            new Estado { EstadoNome = "Maranhão",              Uf = "MA" },
            new Estado { EstadoNome = "Mato Grosso",           Uf = "MT" },
            new Estado { EstadoNome = "Mato Grosso do Sul",    Uf = "MS" },
            new Estado { EstadoNome = "Minas Gerais",          Uf = "MG" },
            new Estado { EstadoNome = "Pará",                  Uf = "PA" },
            new Estado { EstadoNome = "Paraíba",               Uf = "PB" },
            new Estado { EstadoNome = "Paraná",                Uf = "PR" },
            new Estado { EstadoNome = "Pernambuco",            Uf = "PE" },
            new Estado { EstadoNome = "Piauí",                 Uf = "PI" },
            new Estado { EstadoNome = "Rio de Janeiro",        Uf = "RJ" },
            new Estado { EstadoNome = "Rio Grande do Norte",   Uf = "RN" },
            new Estado { EstadoNome = "Rio Grande do Sul",     Uf = "RS" },
            new Estado { EstadoNome = "Rondônia",              Uf = "RO" },
            new Estado { EstadoNome = "Roraima",               Uf = "RR" },
            new Estado { EstadoNome = "Santa Catarina",        Uf = "SC" },
            new Estado { EstadoNome = "São Paulo",             Uf = "SP" },
            new Estado { EstadoNome = "Sergipe",               Uf = "SE" },
            new Estado { EstadoNome = "Tocantins",             Uf = "TO" },
        };
    }
}
