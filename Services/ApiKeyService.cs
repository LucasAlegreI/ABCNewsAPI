using ABCNewsAPI.Data;
using ABCNewsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ABCNewsAPI.Services
{
    public class ApiKeyService
    {
        private readonly ApplicationDbContext _context;
        private readonly HmacService _hmacService;

        public ApiKeyService(ApplicationDbContext context, HmacService hmacService)
        {
            _context = context;
            _hmacService = hmacService;
        }

        public async Task<string> CrearApiKeyAsync(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("API Key no puede ser nula o vacía.");
            }   
            var firmaHmac = _hmacService.GenerarFirma(apiKey);

            // 3. Guardar solo la firma en la base de datos
            var newApiKey = new ApiKey
            {
                FirmaHmac = firmaHmac,
                Activo = true
            };

            _context.ApiKeys.Add(newApiKey);
            await _context.SaveChangesAsync();

            // 4. Devolver la API Key pública al usuario
            return apiKey;
        }
    }
}