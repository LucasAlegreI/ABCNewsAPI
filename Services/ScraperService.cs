using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using ABCNewsAPI.Models;

namespace ABCNewsAPI.Services
{
    public class ScraperService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://www.abc.com.py/buscar/?q=";

        public ScraperService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<List<Noticia>> ObtenerNoticiasAsync(string query)
        {
            var url = $"{BASE_URL}{query}";
            var response = await _httpClient.GetStringAsync(url);

            var noticias = ParsearHTML(response);
            return noticias;
        }

        private List<Noticia> ParsearHTML(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var noticias = new List<Noticia>();
            var nodosNoticias = doc.DocumentNode.SelectNodes("//div[@class='news-item']");
            if (nodosNoticias == null) return noticias;

            foreach (var nodo in nodosNoticias)
            {
                var tituloNodo = nodo.SelectSingleNode(".//h2/a");
                var enlace = tituloNodo?.GetAttributeValue("href", "");
                var titulo = tituloNodo?.InnerText.Trim() ?? "";

                var resumenNodo = nodo.SelectSingleNode(".//p");
                var resumen = resumenNodo?.InnerText.Trim() ?? "";

                var fechaNodo = nodo.SelectSingleNode(".//time");
                var fecha = fechaNodo?.GetAttributeValue("datetime", "");

                var imagenNodo = nodo.SelectSingleNode(".//img");
                var enlaceFoto = imagenNodo?.GetAttributeValue("src", "");

                noticias.Add(new Noticia
                {
                    Fecha = fecha,
                    Enlace = enlace,
                    EnlaceFoto = enlaceFoto,
                    Titulo = titulo,
                    Resumen = resumen
                });
            }

            return noticias;
        }
    }
}
