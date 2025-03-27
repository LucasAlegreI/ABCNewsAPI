using ABCNewsAPI.Models;
using OpenQA.Selenium.Edge;

namespace ABCNewsAPI.Services
{
    public class ScraperService
    {
        private readonly string BASE_URL = "https://www.abc.com.py/buscador/?query=";
        private readonly string BASE_IMAGE_URL = "https://www.abc.com.py";

        public async Task<List<Noticia>> ObtenerNoticiasAsync(string query)
        {
            var url = $"{BASE_URL}{query}";

            // Configurar Selenium para usar Microsoft Edge
            var options = new EdgeOptions();
            options.AddArgument("--headless");  // Ejecutar en modo sin cabeza (sin interfaz gráfica)

            using var driver = new EdgeDriver(options);
            driver.Navigate().GoToUrl(url);

            // Esperar a que el contenido se cargue (puedes usar una espera explícita si sabes que el contenido tarda un tiempo en cargarse)
            Thread.Sleep(5000);  // Dormir por 5 segundos (ajusta según necesites)

            // Obtener el HTML de la página ya renderizado
            var pageSource = driver.PageSource;
            driver.Quit();

            var noticias = ParsearHTML(pageSource);
            return noticias;
        }

        private List<Noticia> ParsearHTML(string html)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var noticias = new List<Noticia>();
            var nodosNoticias = doc.DocumentNode.SelectNodes("//div[@class='queryly_item_row']");
            if (nodosNoticias == null) return noticias;

            foreach (var nodo in nodosNoticias)
            {
                // Extraer el título
                var tituloNodo = nodo.SelectSingleNode(".//div[@class='queryly_item_title']");
                var titulo = tituloNodo?.InnerText.Trim() ?? "";

                // Extraer el enlace (href)
                var enlaceNodo = nodo.SelectSingleNode(".//a");
                var enlace = enlaceNodo?.GetAttributeValue("href", "");

                // Extraer el resumen
                var resumenNodo = nodo.SelectSingleNode(".//div[@class='queryly_item_description']");
                var resumen = resumenNodo?.InnerText.Trim() ?? "";

                // Extraer la fecha
                var fechaNodo = nodo.SelectSingleNode(".//div[contains(@style, 'font-size:12px')]");
                var fecha = fechaNodo?.InnerText.Trim();

                // Extraer la foto
                var imagenNodo = nodo.SelectSingleNode(".//div[contains(@class, 'queryly_advanced_item_imagecontainer')]");
                var enlaceFoto = imagenNodo?.GetAttributeValue("style", "")
                    .Split(new string[] { "url(" }, StringSplitOptions.None)[1]?.Split(')')[0];
                if (enlaceFoto != null)
                {
                    enlaceFoto = enlaceFoto.Trim('\'', '"');
                    enlaceFoto = enlaceFoto.Replace("&amp;", "&");
                }
                    noticias.Add(new Noticia
                {
                    Fecha = fecha,
                    Enlace = BASE_IMAGE_URL + enlace,
                    EnlaceFoto = BASE_IMAGE_URL+enlaceFoto,
                    Titulo = titulo,
                    Resumen = resumen
                });
            }

            return noticias;
        }
    }
}
