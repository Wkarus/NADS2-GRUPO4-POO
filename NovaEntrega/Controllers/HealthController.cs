using Microsoft.AspNetCore.Mvc;

namespace Servidor_PI.Controllers
{
    // Controller de Health Check - verifica se a API esta funcionando
    // E util para monitorar se o servidor esta online
    [ApiController]
    [Route("api/[controller]")]  // Rota: /api/health
    public class HealthController : ControllerBase
    {
        // Endpoint GET simples que retorna se a API esta OK
        // Quando alguem acessa /api/health, retorna { status: ok }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "ok" });
        }
    }
}

