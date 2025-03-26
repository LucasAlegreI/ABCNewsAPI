using ABCNewsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCNewsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiKeyController : ControllerBase
    {
        private readonly ApiKeyService _apiKeyService;

        public ApiKeyController(ApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddApiKey([FromBody] string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return BadRequest(new { error = "API Key is required." });

            try
            {
                var newApiKey = await _apiKeyService.CrearApiKeyAsync(apiKey);
                return Ok(new { message = "API Key added successfully." });
            }
            catch (InvalidOperationException ex)
            {
                // Si la clave ya existe, devolvemos un error de conflicto
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
