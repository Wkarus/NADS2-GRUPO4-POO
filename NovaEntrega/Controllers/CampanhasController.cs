using Microsoft.AspNetCore.Mvc;
using Servidor_PI.Models;
using Servidor_PI.Repositories.Interfaces;

namespace Servidor_PI.Controllers
{
    // Controller de Campanhas
    // Responsável por listar, buscar, criar, atualizar e deletar campanhas
    // Rota base: /api/campanhas
    [ApiController]
    [Route("api/[controller]")]
    public class CampanhasController : ControllerBase
    {
        // Repository: camada que fala com o banco de dados
        private readonly ICampanhaRepository _repository;
        // Logger: registra erros e informações para acompanhar a execução
        private readonly ILogger<CampanhasController> _logger;

        public CampanhasController(ICampanhaRepository repository, ILogger<CampanhasController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET /api/campanhas - Lista campanhas com paginação (page e pageSize)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Campanha>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var campanhas = await _repository.GetAllAsync();
                var total = campanhas.Count();
                var paginated = campanhas.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    data = paginated,
                    total = total,
                    page = page,
                    pageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as campanhas");
                return StatusCode(500, new { message = "Erro interno ao buscar campanhas", error = "InternalServerError" });
            }
        }

        // GET /api/campanhas/{id} - Busca uma campanha específica pelo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Campanha>> GetById(int id)
        {
            try
            {
                var campanha = await _repository.GetByIdAsync(id);
                if (campanha == null)
                {
                    return NotFound(new { message = $"Campanha {id} não encontrada", error = "NotFound" });
                }

                return Ok(campanha);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar campanha {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao buscar campanha", error = "InternalServerError" });
            }
        }

        // GET /api/campanhas/publicar - Retorna todas as campanhas (sem paginação)
        [HttpGet("publicar")]
        public async Task<ActionResult<IEnumerable<Campanha>>> Publicar()
        {
            try
            {
                var campanhas = await _repository.GetAllAsync();
                return Ok(campanhas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar campanhas");
                return StatusCode(500, new { message = "Erro interno ao publicar campanhas", error = "InternalServerError" });
            }
        }

        // POST /api/campanhas - Cria uma nova campanha
        // Recebe os dados no corpo da requisição (JSON)
        [HttpPost]
        public async Task<ActionResult<Campanha>> Create([FromBody] Campanha campanha)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dados inválidos", error = "BadRequest", errors = ModelState });
                }

                var created = await _repository.CreateAsync(campanha);
                return CreatedAtAction(nameof(GetById), new { id = created.cd_campanha }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar campanha");
                return StatusCode(500, new { message = "Erro interno ao criar campanha", error = "InternalServerError" });
            }
        }

        // PUT /api/campanhas/{id} - Atualiza uma campanha existente
        // Confere se o ID da URL bate com o ID do objeto
        [HttpPut("{id}")]
        public async Task<ActionResult<Campanha>> Update(int id, [FromBody] Campanha campanha)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dados inválidos", error = "BadRequest", errors = ModelState });
                }

                if (id != campanha.cd_campanha)
                {
                    return BadRequest(new { message = "ID na URL não corresponde ao ID do corpo", error = "BadRequest" });
                }

                var updated = await _repository.UpdateAsync(id, campanha);
                if (updated == null)
                {
                    return NotFound(new { message = $"Campanha {id} não encontrada", error = "NotFound" });
                }

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar campanha {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao atualizar campanha", error = "InternalServerError" });
            }
        }

        // DELETE /api/campanhas/{id} - Remove uma campanha pelo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"Campanha {id} não encontrada", error = "NotFound" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar campanha {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao deletar campanha", error = "InternalServerError" });
            }
        }
    }
}
