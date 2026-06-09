using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrbitalGuard.API.Data;
using OrbitalGuard.API.Models;

namespace OrbitalGuard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertasController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public AlertasController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int pagina = 1, [FromQuery] int tamanho = 10)
        {
            if (pagina < 1) pagina = 1;
            if (tamanho < 1 || tamanho > 100) tamanho = 10;

            var query = dbContext.Alertas.OrderByDescending(a => a.DtHora);
            var total = await query.CountAsync();
            var dados = await query
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync();

            return Ok(new
            {
                pagina,
                tamanho,
                total,
                totalPaginas = (int)Math.Ceiling(total / (double)tamanho),
                dados
            });
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var alerta = await dbContext.Alertas.FindAsync(id);
            if (alerta == null)
            {
                return NotFound("Alerta não encontrado.");
            }
            return Ok(alerta);
        }

       
        [HttpGet("regiao/{regiaoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByRegiao(int regiaoId)
        {
            var alertas = await dbContext.Alertas
                .Where(a => a.IdRegiao == regiaoId)
                .OrderByDescending(a => a.DtHora)
                .ToListAsync();
            return Ok(alertas);
        }

       
        [HttpGet("nivel/{nivel}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByNivel(string nivel)
        {
            var alertas = await dbContext.Alertas
                .Where(a => a.Nivel.ToLower() == nivel.ToLower())
                .OrderByDescending(a => a.DtHora)
                .ToListAsync();
            return Ok(alertas);
        }

        
        [HttpGet("ativos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAtivos()
        {
            var alertas = await dbContext.Alertas
                .Where(a => a.Ativo)
                .OrderByDescending(a => a.DtHora)
                .ToListAsync();
            return Ok(alertas);
        }

       
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Alerta alerta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

          
            bool regiaoExiste = await dbContext.Regioes.AnyAsync(r => r.IdRegiao == alerta.IdRegiao);
            if (!regiaoExiste)
            {
                return BadRequest("A região informada (idRegiao) não existe.");
            }

            dbContext.Alertas.Add(alerta);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = alerta.IdAlerta }, alerta);
        }

        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Alerta alertaAtualizado)
        {
            if (id != alertaAtualizado.IdAlerta)
            {
                return BadRequest("O ID da URL não confere com o ID do corpo da requisição.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existente = await dbContext.Alertas.FindAsync(id);
            if (existente == null)
            {
                return NotFound("Alerta não encontrado.");
            }

            // Se mudaram a região, valida que a nova existe
            if (existente.IdRegiao != alertaAtualizado.IdRegiao)
            {
                bool regiaoExiste = await dbContext.Regioes.AnyAsync(r => r.IdRegiao == alertaAtualizado.IdRegiao);
                if (!regiaoExiste)
                {
                    return BadRequest("A região informada (idRegiao) não existe.");
                }
            }

            existente.Tipo = alertaAtualizado.Tipo;
            existente.Nivel = alertaAtualizado.Nivel;
            existente.Descricao = alertaAtualizado.Descricao;
            existente.DtHora = alertaAtualizado.DtHora;
            existente.Latitude = alertaAtualizado.Latitude;
            existente.Longitude = alertaAtualizado.Longitude;
            existente.Ativo = alertaAtualizado.Ativo;
            existente.IdRegiao = alertaAtualizado.IdRegiao;

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

       
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id)
        {
            var alerta = await dbContext.Alertas.FindAsync(id);
            if (alerta == null)
            {
                return NotFound("Alerta não encontrado.");
            }

            try
            {
                dbContext.Alertas.Remove(alerta);
                await dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Este alerta já gerou notificações e não pode ser removido (preserva trilha de auditoria). Considere desativá-lo (PUT com Ativo = false).");
            }
        }
    }
}