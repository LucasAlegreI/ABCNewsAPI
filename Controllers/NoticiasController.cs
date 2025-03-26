using Microsoft.AspNetCore.Mvc;
using ABCNewsAPI.Services;
using ABCNewsAPI.Models;
using System.Text.Json;
using System.Xml.Serialization;
using System.Text;
using System.IO;

namespace ABCNewsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NoticiasController : ControllerBase
    {
        private readonly ScraperService _scraperService;

        public NoticiasController(ScraperService scraperService)
        {
            _scraperService = scraperService;
        }

        [HttpGet("consulta")]
        public async Task<IActionResult> Consulta([FromQuery] string q, [FromQuery] bool f = false)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { codigo = "g268", error = "Parámetros inválidos" });

            try
            {
                var noticias = await _scraperService.ObtenerNoticiasAsync(q);

                if (noticias.Count == 0)
                    return NotFound(new { codigo = "g267", error = $"No se encuentran noticias para el texto: {q}" });
                if (f)
                {
                    foreach (var noticia in noticias)
                    {
                        if (!string.IsNullOrEmpty(noticia.EnlaceFoto))
                        {
                            var imageBase64 = await ConvertImageToBase64Async(noticia.EnlaceFoto);
                            noticia.Extra = new Extra { FotoBase64 = imageBase64, ContentType = "image/jpeg" };
                        }
                    }
                }

                var acceptHeader = Request.Headers["Accept"].ToString().ToLower();

                return acceptHeader switch
                {
                    "application/xml" => Content(ToXml(noticias), "application/xml"),
                    "application/json" => Content(JsonSerializer.Serialize(noticias), "application/json"),
                    "text/plain" => Content(ToPlainText(noticias), "text/plain"),
                    "text/html" => Content(ToHtml(noticias), "text/html"),
                    _ => Content(JsonSerializer.Serialize(noticias), "application/json") // JSON por defecto
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { codigo = "g100", error = "Error interno del servidor", detalle = ex.Message });
            }
        }
        private async Task<string> ConvertImageToBase64Async(string imageUrl)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var imageBytes = await client.GetByteArrayAsync(imageUrl);
                    return Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception)
            {
                return null; // Si hay un error, no se incluye la foto base64
            }
        }

        private string ToXml<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }

        private string ToPlainText(List<Noticia> noticias)
        {
            var sb = new StringBuilder();
            foreach (var noticia in noticias)
            {
                sb.AppendLine($"Título: {noticia.Titulo}");
                sb.AppendLine($"Resumen: {noticia.Resumen}");
                sb.AppendLine($"Fecha: {noticia.Fecha}");
                sb.AppendLine($"Enlace: {noticia.Enlace}");
                sb.AppendLine($"Imagen: {noticia.EnlaceFoto}");
                sb.AppendLine("------------------------------------");
            }
            return sb.ToString();
        }

        private string ToHtml(List<Noticia> noticias)
        {
            var sb = new StringBuilder();
            sb.Append("<html><body><h1>Noticias</h1><ul>");
            foreach (var noticia in noticias)
            {
                sb.Append($"<li><h2>{noticia.Titulo}</h2>");
                sb.Append($"<p><strong>Resumen:</strong> {noticia.Resumen}</p>");
                sb.Append($"<p><strong>Fecha:</strong> {noticia.Fecha}</p>");
                sb.Append($"<p><a href='{noticia.Enlace}'>Leer más</a></p>");
                sb.Append($"<img src='{noticia.EnlaceFoto}' alt='Imagen de la noticia' style='max-width:300px;'/>");
                sb.Append("</li>");
            }
            sb.Append("</ul></body></html>");
            return sb.ToString();
        }
    }
}
