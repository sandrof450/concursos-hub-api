using HtmlAgilityPack;
using WebApplication_App_Concurso.DTOs;
using WebApplication_App_Concurso.Exceptions;
using WebApplication_App_Concurso.Scrapers.Interfaces;

namespace WebApplication_App_Concurso.Scrapers
{
    public class PciConcursosScraper: IScrapingService
    {
        private readonly IConfiguration _configuration;

        public PciConcursosScraper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<ConcursoDTO>> GetAllConcursosAsync()
        {
            var url = _configuration["ScrapingSettings:PciUrl"];

            if (string.IsNullOrEmpty(url))
                throw new ScrapingException("A url do PCI Concursos não foi configurada");

            try
            {
                using var http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", _configuration["ScrapingSettings:UserAgent"]);

                var html = await http.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var lista = new List<ConcursoDTO>();

                var container = doc.DocumentNode.SelectSingleNode("//div[@id='concursos']");
                if (container == null) return lista;

                string estadoAtual = "";

                var nodes = container.ChildNodes;

                foreach (var node in nodes)
                {
                    // 🔹 quando encontrar a div de estado
                    if (node.Name == "div" && node.GetAttributeValue("class", "") == "ua")
                    {
                        estadoAtual = node.GetAttributeValue("id", "").ToUpper();
                        continue;
                    }

                    // 🔹 quando encontrar concurso
                    if (node.Name == "div" && node.GetAttributeValue("class", "") == "da")
                    {
                        var link = node.GetAttributeValue("data-url", "");

                        var tituloNode = node.SelectSingleNode(".//div[@class='ca']");
                        var titulo = tituloNode?.InnerText.Trim();

                        var infoNode = node.SelectSingleNode(".//span[contains(@class,'l_ap2')]");
                        var infoExtra = infoNode?.InnerText.Trim();

                        if (string.IsNullOrWhiteSpace(titulo))
                            continue;

                        lista.Add(new ConcursoDTO
                        {
                            Titulo = HtmlEntity.DeEntitize(titulo),
                            Estado = estadoAtual,
                            Fonte = "PCIConcursos",
                            Link = link,
                            Extras = new Dictionary<string, string>
                        {
                            { "info", infoExtra ?? "" }
                        }
                        });
                    }
                }

                return lista;
            }
            catch(TaskCanceledException ex)
            {
                throw new ScrapingConnectionException(
                    "Tempo limite excedido ao acessar o PCI concursos.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new ScrapingConnectionException(
                    "Falha de conexão ao acessar PCI concursos", ex);
            }
            

        }
    }
}
