using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

public class HmacService
{
    private readonly string _serverSecret;

    public HmacService(IConfiguration config)
    {
        _serverSecret = config["HmacSecretKey"] ?? throw new Exception("Clave maestra no configurada");
    }

    /// <summary>
    /// Genera una firma HMAC de la API Key usando la clave maestra.
    /// </summary>
    public string GenerarFirma(string apiKey)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_serverSecret)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
            return Convert.ToBase64String(hash);
        }
    }

    /// <summary>
    /// Verifica si la API Key es válida comparando su firma con la almacenada.
    /// </summary>
    public bool VerificarFirma(string apiKey, string firmaEsperada)
    {
        string nuevaFirma = GenerarFirma(apiKey);
        return nuevaFirma == firmaEsperada;
    }
}