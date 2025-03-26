namespace ABCNewsAPI.Models
{
    public class Noticia
    {
        public string Fecha { get; set; } = string.Empty;
        public string Enlace { get; set; } = string.Empty;
        public string EnlaceFoto { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Resumen { get; set; } = string.Empty;
        public Extra? Extra { get; set; }
    }
    public class Extra
    {
        public string? FotoBase64 { get; set; }
        public string? ContentType { get; set; }
    }
}
