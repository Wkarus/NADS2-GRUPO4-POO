using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servidor_PI.Data;

namespace Servidor_PI.Controllers
{
    // Controller de Views (consultas)
    // Simula consultas como se fossem views do banco
    // Rota base: /api/views
    [ApiController]
    [Route("api/[controller]")]
    public class ViewsController : ControllerBase
    {
        // Contexto do banco de dados (EF Core)
        private readonly AppDbContext _context;
        // Logger para registrar erros e informações
        private readonly ILogger<ViewsController> _logger;

        public ViewsController(AppDbContext context, ILogger<ViewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET /api/views/buscar-nome?usuario=abc
        // Busca nome completo e nome de usuário a partir do login
        [HttpGet("buscar-nome")]
        public async Task<ActionResult> BuscarNome([FromQuery] string usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario))
                {
                    return BadRequest(new { message = "Parâmetro 'usuario' é obrigatório", error = "BadRequest" });
                }

                // Simula a view buscarNome: SELECT nome_completo, nome_usuario FROM Usuario WHERE nome_usuario = @usuario
                var resultado = await _context.Usuarios
                    .Where(u => u.nome_usuario == usuario)
                    .Select(u => new
                    {
                        nome_completo = u.nome_completo,
                        nome_usuario = u.nome_usuario
                    })
                    .FirstOrDefaultAsync();

                if (resultado == null)
                {
                    return NotFound(new { message = $"Usuário '{usuario}' não encontrado", error = "NotFound" });
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar nome do usuário {Usuario}", usuario);
                return StatusCode(500, new { message = "Erro interno ao buscar nome", error = "InternalServerError" });
            }
        }

        // GET /api/views/doacoes-detalhadas
        // Retorna lista com dados de doações + usuário + campanha
        [HttpGet("doacoes-detalhadas")]
        public async Task<ActionResult> DoacoesDetalhadas()
        {
            try
            {
                // Simula a view vw_doacoes_detalhadas
                var resultado = await _context.Doacoes
                    .Include(d => d.Usuario)
                    .Include(d => d.Campanha)
                    .Select(d => new
                    {
                        cd_doacao = d.cd_doacao,
                        nome_doador = d.Usuario.nome_completo,
                        nome_campanha = d.Campanha.nome_campanha,
                        nome_doacao = d.nome_doacao,
                        tipo_doacao = d.tipo_doacao.ToString(),
                        forma_arrecadacao = d.forma_arrecadacao != null ? d.forma_arrecadacao.ToString() : null,
                        status_arrecadacao = d.status_arrecadacao.ToString()
                    })
                    .ToListAsync();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar doações detalhadas");
                return StatusCode(500, new { message = "Erro interno ao buscar doações detalhadas", error = "InternalServerError" });
            }
        }
    }
}
