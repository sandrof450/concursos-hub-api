using HtmlAgilityPack;
using System.Buffers.Text;
using WebApplication_App_Concurso.DTOs;
using WebApplication_App_Concurso.Exceptions;
using WebApplication_App_Concurso.Scrapers.Interfaces;

namespace WebApplication_App_Concurso.Scrapers
{
    public class ConcursosNoBrasilScraper: IScrapingService
    {
        private readonly IConfiguration _configuration;

        public ConcursosNoBrasilScraper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<ConcursoDTO>> GetAllConcursosAsync()
        {
            var url = _configuration["ScrapingSettings:BaseUrl"];
            var userAgent = _configuration["ScrapingSettings:UserAgent"] ?? throw new Exception("UserAgent Vazia");

            if (string.IsNullOrEmpty(url))
                throw new ScrapingException("A url do concursos brasil não foi configurada");

            try
            {
                using var http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", userAgent);
                http.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml");
                http.DefaultRequestHeaders.Add("Accept-Language", "pt-BR,pt;q=0.9");

                var html = await http.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // concursosnobrasil
                var concursos = GetPortalConcursosNobrasil(doc, url);

                return concursos;
            }
            catch (TaskCanceledException ex)
            {
                throw new ScrapingConnectionException(
                    "Tempo limite excedido ao acessar o concursos brasil.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new ScrapingConnectionException(
                    "Falha de conexão ao acessar concursos brasil", ex);
            }

            
        }

        public List<ConcursoDTO> GetPortalConcursosNobrasil(HtmlDocument doc, string baseUrl)
        {
            var lista = new List<ConcursoDTO>();

            var regionDivs = doc.DocumentNode.SelectNodes("//main[@id='conteudo']//div[@id='nacional' or @id='centro-oeste' or @id='nordeste' or @id='norte' or @id='sudeste' or @id='sul']");

            if (regionDivs == null)
                return lista;

            foreach (var region in regionDivs)
            {
                var nomeRegiao = region.GetAttributeValue("id", "");

                var nodes = region.SelectNodes(".//h4 | .//table");

                string estadoAtual = "";

                foreach (var node in nodes ?? Enumerable.Empty<HtmlNode>())
                {
                    if (node.Name == "h4")
                    {
                        estadoAtual = node.InnerText.Trim();
                        continue;
                    }

                    if (node.Name == "table" && !string.IsNullOrEmpty(estadoAtual))
                    {
                        var rows = node.SelectNodes(".//tbody/tr");

                        foreach (var row in rows ?? Enumerable.Empty<HtmlNode>())
                        {
                            var tds = row.SelectNodes("td");
                            if (tds == null || tds.Count < 2) continue;

                            if (tds[0].InnerText.Contains("Veja todos")) continue;

                            var linkNode = tds[0].SelectSingleNode(".//a");

                            var titulo = linkNode?.InnerText.Trim();
                            var link = linkNode?.GetAttributeValue("href", "");

                            // 🔥 Corrige link relativo
                            if (!string.IsNullOrEmpty(link) && !link.StartsWith("http"))
                                link = baseUrl + link;

                            var vagasTexto = tds[1].InnerText.Trim().Replace(".", "");
                            //int? vagas = int.TryParse(vagasTexto, out var v) ? v : null;

                            if (string.IsNullOrWhiteSpace(titulo))
                                continue;

                            lista.Add(new ConcursoDTO
                            {
                                Titulo = titulo,
                                Orgao = titulo, // pode refinar depois
                                Estado = estadoAtual,
                                Vagas = vagasTexto,
                                Fonte = "ConcursosBrasil",
                                Link = link,
                                Extras = new Dictionary<string, string>
                        {
                            { "regiao", nomeRegiao },
                        }
                            });
                        }
                    }
                }
            }

            // 🔥 deduplicação melhor
            lista = lista
                .GroupBy(x => x.Titulo + x.Estado)
                .Select(g => g.First())
                .ToList();

            return lista;
        }
    }
}
