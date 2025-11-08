using Microsoft.AspNetCore.Mvc;
using Servidor_PI.Models;
using Servidor_PI.Repositories.Interfaces;

namespace Servidor_PI.Controllers
{
    // Controller de Relatórios
    // CRUD de relatórios: listar, buscar, criar, atualizar e deletar
    // Rota base: /api/relatorios
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        // Repository: acesso ao banco de dados de relatórios
        private readonly IRelatorioRepository _repository;
        // Logger: registra erros e informações para diagnosticar problemas
        private readonly ILogger<RelatoriosController> _logger;

        public RelatoriosController(IRelatorioRepository repository, ILogger<RelatoriosController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET /api/relatorios - Lista relatórios com paginação
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Relatorio>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var relatorios = await _repository.GetAllAsync();
                var total = relatorios.Count();
                var paginated = relatorios.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
                _logger.LogError(ex, "Erro ao buscar todos os relatórios");
                return StatusCode(500, new { message = "Erro interno ao buscar relatórios", error = "InternalServerError" });
            }
        }

        // GET /api/relatorios/{id} - Busca relatório pelo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Relatorio>> GetById(int id)
        {
            try
            {
                var relatorio = await _repository.GetByIdAsync(id);
                if (relatorio == null)
                {
                    return NotFound(new { message = $"Relatório {id} não encontrado", error = "NotFound" });
                }

                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relatório {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao buscar relatório", error = "InternalServerError" });
            }
        }

        // GET /api/relatorios/publicar - Retorna todos os relatórios (sem paginação)
        [HttpGet("publicar")]
        public async Task<ActionResult<IEnumerable<Relatorio>>> Publicar()
        {
            try
            {
                var relatorios = await _repository.GetAllAsync();
                return Ok(relatorios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar relatórios");
                return StatusCode(500, new { message = "Erro interno ao publicar relatórios", error = "InternalServerError" });
            }
        }

        // POST /api/relatorios - Cria um novo relatório
        // Recebe os dados via JSON no corpo
        [HttpPost]
        public async Task<ActionResult<Relatorio>> Create([FromBody] Relatorio relatorio)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dados inválidos", error = "BadRequest", errors = ModelState });
                }

                var created = await _repository.CreateAsync(relatorio);
                return CreatedAtAction(nameof(GetById), new { id = created.cd_relatorio }, created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao criar relatório");
                return BadRequest(new { message = ex.Message, error = "BadRequest" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar relatório");
                return StatusCode(500, new { message = "Erro interno ao criar relatório", error = "InternalServerError" });
            }
        }

        // PUT /api/relatorios/{id} - Atualiza um relatório
        // Confere se o ID da URL é igual ao do objeto
        [HttpPut("{id}")]
        public async Task<ActionResult<Relatorio>> Update(int id, [FromBody] Relatorio relatorio)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dados inválidos", error = "BadRequest", errors = ModelState });
                }

                if (id != relatorio.cd_relatorio)
                {
                    return BadRequest(new { message = "ID na URL não corresponde ao ID do corpo", error = "BadRequest" });
                }

                var updated = await _repository.UpdateAsync(id, relatorio);
                if (updated == null)
                {
                    return NotFound(new { message = $"Relatório {id} não encontrado", error = "NotFound" });
                }

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao atualizar relatório {Id}", id);
                return BadRequest(new { message = ex.Message, error = "BadRequest" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar relatório {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao atualizar relatório", error = "InternalServerError" });
            }
        }

        // DELETE /api/relatorios/{id} - Remove um relatório pelo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"Relatório {id} não encontrado", error = "NotFound" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar relatório {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao deletar relatório", error = "InternalServerError" });
            }
        }
    }
}
