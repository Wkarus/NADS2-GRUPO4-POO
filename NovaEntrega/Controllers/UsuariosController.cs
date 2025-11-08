using Microsoft.AspNetCore.Mvc;
using Servidor_PI.Models;
using Servidor_PI.Repositories.Interfaces;

namespace Servidor_PI.Controllers
{
    // Controller de Usuarios - gerencia todas as operacoes com usuarios
    // Endpoints: GET, POST, PUT, DELETE para /api/usuarios
    [ApiController]
    [Route("api/[controller]")]  // Rota base: /api/usuarios
    public class UsuariosController : ControllerBase
    {
        // Repository = camada que acessa o banco de dados
        private readonly IUsuarioRepository _repository;
        // Logger = para registrar erros e informacoes
        private readonly ILogger<UsuariosController> _logger;

        // Construtor - recebe o repository e logger via injecao de dependencia
        // O .NET automaticamente cria essas instancias quando precisa
        public UsuariosController(IUsuarioRepository repository, ILogger<UsuariosController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET /api/usuarios - Lista todos os usuarios com paginacao
        // page = pagina atual (padrao 1)
        // pageSize = quantos itens por pagina (padrao 10)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Busca todos os usuarios do banco
                var usuarios = await _repository.GetAllAsync();
                // Conta quantos tem no total
                var total = usuarios.Count();
                // Faz a paginacao: pula os da pagina anterior e pega so os da pagina atual
                // Exemplo: pagina 2, 10 por pagina = pula 10 e pega 10
                var paginated = usuarios.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                // Retorna os dados paginados com informacoes da paginacao
                return Ok(new
                {
                    data = paginated,      // Lista de usuarios da pagina atual
                    total = total,          // Total de usuarios no banco
                    page = page,            // Pagina atual
                    pageSize = pageSize     // Itens por pagina
                });
            }
            catch (Exception ex)
            {
                // Se der erro, registra no log e retorna erro 500
                _logger.LogError(ex, "Erro ao buscar todos os usuários");
                return StatusCode(500, new { message = "Erro interno ao buscar usuários", error = "InternalServerError" });
            }
        }

        // GET /api/usuarios/{id} - Busca um usuario especifico pelo ID
        // Exemplo: GET /api/usuarios/1 busca o usuario com ID 1
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetById(int id)
        {
            try
            {
                // Busca o usuario no banco pelo ID
                var usuario = await _repository.GetByIdAsync(id);
                // Se nao encontrar, retorna 404 (nao encontrado)
                if (usuario == null)
                {
                    return NotFound(new { message = $"Usuário {id} não encontrado", error = "NotFound" });
                }

                // Se encontrar, retorna o usuario (status 200 OK)
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                // Se der erro, registra e retorna erro 500
                _logger.LogError(ex, "Erro ao buscar usuário {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao buscar usuário", error = "InternalServerError" });
            }
        }

        // GET /api/usuarios/publicar - Retorna TODOS os usuarios sem paginacao
        [HttpGet("publicar")]
        public async Task<ActionResult<IEnumerable<Usuario>>> Publicar()
        {
            try
            {
                // Busca todos os usuarios (sem paginacao)
                var usuarios = await _repository.GetAllAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar usuários");
                return StatusCode(500, new { message = "Erro interno ao publicar usuários", error = "InternalServerError" });
            }
        }

        // POST /api/usuarios - Cria um novo usuario
        // Recebe os dados do usuario no corpo da requisicao (JSON)
        [HttpPost]
        public async Task<ActionResult<Usuario>> Create([FromBody] Usuario usuario)
        {
            try
            {
                // Valida se os dados estao corretos (campos obrigatorios, formato de email, etc)
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dados inválidos", error = "BadRequest", errors = ModelState });
                }

                // Cria o usuario no banco de dados
                var created = await _repository.CreateAsync(usuario);
                // Retorna status 201 (Created) com o usuario criado e a URL onde ele pode ser acessado
                return CreatedAtAction(nameof(GetById), new { id = created.cd_cliente }, created);
            }
            catch (InvalidOperationException ex)
            {
                // Erro de validacao (ex: email ja existe)
                _logger.LogWarning(ex, "Erro de validação ao criar usuário");
                return Conflict(new { message = ex.Message, error = "Conflict" });  // Status 409
            }
            catch (Exception ex)
            {
                // Erro inesperado
                _logger.LogError(ex, "Erro ao criar usuário");
                return StatusCode(500, new { message = "Erro interno ao criar usuário", error = "InternalServerError" });
            }
        }

        // PUT /api/usuarios/{id} - Atualiza um usuario existente
        // Recebe o ID na URL e os dados atualizados no corpo da requisicao
        [HttpPut("{id}")]
        public async Task<ActionResult<Usuario>> Update(int id, [FromBody] Usuario usuario)
        {
            try
            {
                // Valida os dados
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dados inválidos", error = "BadRequest", errors = ModelState });
                }

                // Verifica se o ID da URL bate com o ID do objeto
                // Seguranca: evita atualizar o usuario errado
                if (id != usuario.cd_cliente)
                {
                    return BadRequest(new { message = "ID na URL não corresponde ao ID do corpo", error = "BadRequest" });
                }

                // Atualiza o usuario no banco
                var updated = await _repository.UpdateAsync(id, usuario);
                // Se nao encontrar, retorna 404
                if (updated == null)
                {
                    return NotFound(new { message = $"Usuário {id} não encontrado", error = "NotFound" });
                }

                // Retorna o usuario atualizado
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                // Erro de validacao (ex: email ja existe em outro usuario)
                _logger.LogWarning(ex, "Erro de validação ao atualizar usuário {Id}", id);
                return Conflict(new { message = ex.Message, error = "Conflict" });
            }
            catch (Exception ex)
            {
                // Erro inesperado
                _logger.LogError(ex, "Erro ao atualizar usuário {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao atualizar usuário", error = "InternalServerError" });
            }
        }

        // DELETE /api/usuarios/{id} - Deleta um usuario
        // Exemplo: DELETE /api/usuarios/1 deleta o usuario com ID 1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Tenta deletar o usuario
                var deleted = await _repository.DeleteAsync(id);
                // Se nao encontrar, retorna 404
                if (!deleted)
                {
                    return NotFound(new { message = $"Usuário {id} não encontrado", error = "NotFound" });
                }

                // Se deletou com sucesso, retorna 204 (No Content) - sem conteudo
                return NoContent();
            }
            catch (Exception ex)
            {
                // Erro inesperado
                _logger.LogError(ex, "Erro ao deletar usuário {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao deletar usuário", error = "InternalServerError" });
            }
        }
    }
}
