using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrbitalGuard.API.Data;
using OrbitalGuard.API.Models;

namespace OrbitalGuard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacoesController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public NotificacoesController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var notificacoes = await dbContext.Notificacoes
                .OrderByDescending(n => n.IdNotificacao)
                .ToListAsync();
            return Ok(notificacoes);
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var notificacao = await dbContext.Notificacoes.FindAsync(id);
            if (notificacao == null)
            {
                return NotFound("Notificação não encontrada.");
            }
            return Ok(notificacao);
        }

       
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUsuario(int usuarioId)
        {
            var notificacoes = await dbContext.Notificacoes
                .Where(n => n.IdUsuario == usuarioId)
                .OrderByDescending(n => n.IdNotificacao)
                .ToListAsync();
            return Ok(notificacoes);
        }

        
        [HttpGet("alerta/{alertaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAlerta(int alertaId)
        {
            var notificacoes = await dbContext.Notificacoes
                .Where(n => n.IdAlerta == alertaId)
                .ToListAsync();
            return Ok(notificacoes);
        }

        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Notificacao notificacao)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool usuarioExiste = await dbContext.Usuarios.AnyAsync(u => u.IdUsuario == notificacao.IdUsuario);
            if (!usuarioExiste) return BadRequest("Usuário informado não existe.");

            bool alertaExiste = await dbContext.Alertas.AnyAsync(a => a.IdAlerta == notificacao.IdAlerta);
            if (!alertaExiste) return BadRequest("Alerta informado não existe.");

            dbContext.Notificacoes.Add(notificacao);
            await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = notificacao.IdNotificacao }, notificacao);
        }

        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Notificacao notificacaoAtualizada)
        {
            if (id != notificacaoAtualizada.IdNotificacao)
            {
                return BadRequest("O ID da URL não confere com o ID do corpo da requisição.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existente = await dbContext.Notificacoes.FindAsync(id);
            if (existente == null)
            {
                return NotFound("Notificação não encontrada.");
            }

            existente.Mensagem = notificacaoAtualizada.Mensagem;
            existente.Canal = notificacaoAtualizada.Canal;
            existente.Enviada = notificacaoAtualizada.Enviada;
            existente.DtEnvio = notificacaoAtualizada.DtEnvio;

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var notificacao = await dbContext.Notificacoes.FindAsync(id);
            if (notificacao == null)
            {
                return NotFound("Notificação não encontrada.");
            }
            dbContext.Notificacoes.Remove(notificacao);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpPost("gerar/{alertaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GerarParaAlerta(int alertaId, [FromQuery] string canal = "PUSH")
        {
            var alerta = await dbContext.Alertas.FindAsync(alertaId);
            if (alerta == null)
            {
                return NotFound("Alerta não encontrado.");
            }

            
            var idsUsuarios = await dbContext.UsuariosRegioes
                .Where(ur => ur.IdRegiao == alerta.IdRegiao && ur.Usuario!.Ativo)
                .Select(ur => ur.IdUsuario)
                .ToListAsync();

            if (idsUsuarios.Count == 0)
            {
                return Ok(new { criadas = 0, mensagem = "Nenhum usuário ativo inscrito nessa região." });
            }

            var novas = idsUsuarios.Select(uid => new Notificacao
            {
                Mensagem = $"Novo alerta {alerta.Nivel} ({alerta.Tipo}) na sua região monitorada. Verifique o app OrbitalGuard.",
                Canal = canal,
                Enviada = false,
                IdUsuario = uid,
                IdAlerta = alertaId
            }).ToList();

            dbContext.Notificacoes.AddRange(novas);
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                criadas = novas.Count,
                alertaId,
                canal,
                mensagem = $"{novas.Count} notificação(ões) gerada(s) com sucesso."
            });
        }
    }
}