using WebApplication_App_Concurso.Data.Seed;
using WebApplication_App_Concurso.Models;
using WebApplication_App_Concurso.Services.Interfaces;
using System.Text.RegularExpressions;

namespace WebApplication_App_Concurso.Services
{
    public class EstadoNormalizadorService: IEstadoNormalizadorService
    {
        private readonly Dictionary<string, string> _variacoes;

        public EstadoNormalizadorService()
        {
            _variacoes = GetVariacoes(EstadoSeed.GetEstados());
        }

        private Dictionary<string, string> GetVariacoes(List<Estado> estados)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var estado in estados)
            {
                // sigla
                dict[estado.Uf] = estado.Uf;

                // nome completo
                dict[estado.EstadoNome] = estado.Uf;

                // nome sem espaços
                dict[estado.EstadoNome.Replace(" ", "")] = estado.Uf;

                // variações com hífen
                dict[$"{estado.EstadoNome} - {estado.Uf}"] = estado.Uf;
                dict[$"{estado.Uf} - {estado.EstadoNome}"] = estado.Uf;
                // variações com nome + uf colado
                dict[$"{estado.EstadoNome} {estado.Uf}"] = estado.Uf;
                dict[$"{estado.Uf} {estado.EstadoNome}"] = estado.Uf;
            }

            // casos especiais que não são estados
            dict["Todo o Brasil"] = "NACIONAL";
            dict["Brasil"] = "NACIONAL";
            dict["Federal"] = "NACIONAL";

            // casos especiais mapeados para NACIONAL
            dict["Nacional"] = "NACIONAL";
            dict["nacional"] = "NACIONAL";
            dict["NACIONAL"] = "NACIONAL";
            dict["Todo o Brasil"] = "NACIONAL";
            dict["Brasil"] = "NACIONAL";
            dict["Federal"] = "NACIONAL";
            dict["todo brasil"] = "NACIONAL";

            return dict;
        }

        public string? Normalizar(string? estadoBruto)
        {
            if (string.IsNullOrWhiteSpace(estadoBruto)) return null;

            var limpo = estadoBruto.Trim();

            // tenta achar direto
            if (_variacoes.TryGetValue(limpo, out var uf)) return uf;

            // verifica casos especiais antes do Contains
            if (limpo.Contains("Nacional", StringComparison.OrdinalIgnoreCase) ||
                limpo.Contains("Brasil", StringComparison.OrdinalIgnoreCase) ||
                limpo.Contains("Federal", StringComparison.OrdinalIgnoreCase))
                return "Nacional";

            foreach (var variacao in _variacoes
                .Where(v => v.Key.Length > 2) // ignora siglas como ES, SP, RJ...
                .OrderByDescending(v => v.Key.Length))
                    {
                        if (limpo.Contains(variacao.Key, StringComparison.OrdinalIgnoreCase))
                            return variacao.Value;
                    }

            return null;
        }
    }
}
