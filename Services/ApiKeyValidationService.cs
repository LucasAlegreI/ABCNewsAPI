using ABCNewsAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ABCNewsAPI.Services
{
    public class ApiKeyValidationService
    {
        private readonly ApplicationDbContext _context;
        private readonly HmacService _hmacService;

        public ApiKeyValidationService(ApplicationDbContext context, HmacService hmacService)
        {
            _context = context;
            _hmacService = hmacService;
        }
        public async Task<bool> IsApiKeyValidAsync(string apiKeyHeader)
        {
            // Obtener todas las firmas HMAC activas en la base de datos
            var apiKeys = await _context.ApiKeys
                .Where(k => k.Activo)
                .Select(k => k.FirmaHmac) // Solo traemos las firmas HMAC
                .ToListAsync();

            // Recalcular la firma esperada usando la clave pública recibida
            var firmaCalculada = _hmacService.GenerarFirma(apiKeyHeader);

            // Verificar si alguna firma en la base de datos coincide con la firma recalculada
            return apiKeys.Contains(firmaCalculada);
        }
    }
}