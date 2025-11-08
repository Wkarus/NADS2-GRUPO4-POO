using Microsoft.AspNetCore.Mvc;
using Servidor_PI.Models;
using Servidor_PI.Repositories.Interfaces;

namespace Servidor_PI.Controllers
{
    // Controller de Notícias
    // Responsável por operações de listar, buscar, criar, atualizar e deletar notícias
    // Rota base: /api/noticias
    [ApiController]
    [Route("api/[controller]")]
    public class NoticiasController : ControllerBase
    {
        // Repository: camada que acessa o banco de dados de notícias
        private readonly INoticiasRepository _repository;
        // Logger: registra erros e informações úteis
        private readonly ILogger<NoticiasController> _logger;

        public NoticiasController(INoticiasRepository repository, ILogger<NoticiasController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET /api/noticias - Lista notícias com paginação (page e pageSize)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Noticias>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var noticias = await _repository.GetAllAsync();
                var total = noticias.Count();
                var paginated = noticias.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
                _logger.LogError(ex, "Erro ao buscar todas as notícias");
                return StatusCode(500, new { message = "Erro interno ao buscar notícias", error = "InternalServerError" });
            }
        }

        // GET /api/noticias/{id} - Busca uma notícia pelo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Noticias>> GetById(int id)
        {
            try
            {
                var noticia = await _repository.GetByIdAsync(id);
                if (noticia == null)
                {
                    return NotFound(new { message = $"Notícia {id} não encontrada", error = "NotFound" });
                }

                return Ok(noticia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar notícia {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao buscar notícia", error = "InternalServerError" });
            }
        }

        // GET /api/noticias/publicar - Retorna todas as notícias (sem paginação)
        [HttpGet("publicar")]
        public async Task<ActionResult<IEnumerable<Noticias>>> Publicar()
        {
            try
            {
                var noticias = await _repository.GetAllAsync();
                return Ok(noticias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar notícias");
                return StatusCode(500, new { message = "Erro interno ao publicar notícias", error = "InternalServerError" });
            }
        }

        // POST /api/noticias - Cria uma nova notícia
        // Recebe os dados em JSON no corpo da requisição
        [HttpPost]
        public async Task<ActionResult<Noticias>> Create([FromBody] Noticias noticias)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dados inválidos", error = "BadRequest", errors = ModelState });
                }

                var created = await _repository.CreateAsync(noticias);
                return CreatedAtAction(nameof(GetById), new { id = created.cd_noticias }, created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao criar notícia");
                return BadRequest(new { message = ex.Message, error = "BadRequest" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar notícia");
                return StatusCode(500, new { message = "Erro interno ao criar notícia", error = "InternalServerError" });
            }
        }

        // PUT /api/noticias/{id} - Atualiza uma notícia existente
        // Verifica se o ID da URL corresponde ao ID do objeto
        [HttpPut("{id}")]
        public async Task<ActionResult<Noticias>> Update(int id, [FromBody] Noticias noticias)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dados inválidos", error = "BadRequest", errors = ModelState });
                }

                if (id != noticias.cd_noticias)
                {
                    return BadRequest(new { message = "ID na URL não corresponde ao ID do corpo", error = "BadRequest" });
                }

                var updated = await _repository.UpdateAsync(id, noticias);
                if (updated == null)
                {
                    return NotFound(new { message = $"Notícia {id} não encontrada", error = "NotFound" });
                }

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao atualizar notícia {Id}", id);
                return BadRequest(new { message = ex.Message, error = "BadRequest" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar notícia {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao atualizar notícia", error = "InternalServerError" });
            }
        }

        // DELETE /api/noticias/{id} - Remove uma notícia pelo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"Notícia {id} não encontrada", error = "NotFound" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar notícia {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao deletar notícia", error = "InternalServerError" });
            }
        }
    }
}
