namespace ABCNewsAPI.Models
{
    public class ApiKey
    {
        public int Id { get; set; }  // ID de la clave API
        public string FirmaHmac { get; set; } = string.Empty; // Firma HMAC-SHA256
        public bool Activo { get; set; } = true;
    }
}
