using Microsoft.AspNetCore.Mvc;
using Servidor_PI.Models;
using Servidor_PI.Repositories.Interfaces;

namespace Servidor_PI.Controllers
{
    // Controller de Doações
    // Faz CRUD (listar, buscar, criar, atualizar, deletar) das doações
    // Rota base: /api/doacoes
    [ApiController]
    [Route("api/[controller]")]
    public class DoacoesController : ControllerBase
    {
        // Repository: acesso ao banco de dados de doações
        private readonly IDoacaoRepository _repository;
        // Logger: registra erros e eventos importantes
        private readonly ILogger<DoacoesController> _logger;

        public DoacoesController(IDoacaoRepository repository, ILogger<DoacoesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET /api/doacoes - Lista doações com paginação
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doacao>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var doacoes = await _repository.GetAllAsync();
                var total = doacoes.Count();
                var paginated = doacoes.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
                _logger.LogError(ex, "Erro ao buscar todas as doações");
                return StatusCode(500, new { message = "Erro interno ao buscar doações", error = "InternalServerError" });
            }
        }

        // GET /api/doacoes/{id} - Busca uma doação pelo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Doacao>> GetById(int id)
        {
            try
            {
                var doacao = await _repository.GetByIdAsync(id);
                if (doacao == null)
                {
                    return NotFound(new { message = $"Doação {id} não encontrada", error = "NotFound" });
                }

                return Ok(doacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar doação {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao buscar doação", error = "InternalServerError" });
            }
        }

        // GET /api/doacoes/publicar - Retorna todas as doações (sem paginação)
        [HttpGet("publicar")]
        public async Task<ActionResult<IEnumerable<Doacao>>> Publicar()
        {
            try
            {
                var doacoes = await _repository.GetAllAsync();
                return Ok(doacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar doações");
                return StatusCode(500, new { message = "Erro interno ao publicar doações", error = "InternalServerError" });
            }
        }

        // POST /api/doacoes - Cria uma nova doação
        // Recebe os dados via JSON no corpo da requisição
        [HttpPost]
        public async Task<ActionResult<Doacao>> Create([FromBody] Doacao doacao)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dados inválidos", error = "BadRequest", errors = ModelState });
                }

                var created = await _repository.CreateAsync(doacao);
                return CreatedAtAction(nameof(GetById), new { id = created.cd_doacao }, created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao criar doação");
                return BadRequest(new { message = ex.Message, error = "BadRequest" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar doação");
                return StatusCode(500, new { message = "Erro interno ao criar doação", error = "InternalServerError" });
            }
        }

        // PUT /api/doacoes/{id} - Atualiza uma doação existente
        // Confere se o ID da URL é o mesmo do objeto
        [HttpPut("{id}")]
        public async Task<ActionResult<Doacao>> Update(int id, [FromBody] Doacao doacao)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dados inválidos", error = "BadRequest", errors = ModelState });
                }

                if (id != doacao.cd_doacao)
                {
                    return BadRequest(new { message = "ID na URL não corresponde ao ID do corpo", error = "BadRequest" });
                }

                var updated = await _repository.UpdateAsync(id, doacao);
                if (updated == null)
                {
                    return NotFound(new { message = $"Doação {id} não encontrada", error = "NotFound" });
                }

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao atualizar doação {Id}", id);
                return BadRequest(new { message = ex.Message, error = "BadRequest" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar doação {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao atualizar doação", error = "InternalServerError" });
            }
        }

        // DELETE /api/doacoes/{id} - Remove uma doação
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"Doação {id} não encontrada", error = "NotFound" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar doação {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao deletar doação", error = "InternalServerError" });
            }
        }
    }
}
