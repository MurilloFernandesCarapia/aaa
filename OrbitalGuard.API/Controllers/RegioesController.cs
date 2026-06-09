using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrbitalGuard.API.Data;
using OrbitalGuard.API.Models;

namespace OrbitalGuard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegioesController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public RegioesController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var regioes = await dbContext.Regioes.ToListAsync();
            return Ok(regioes);
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var regiao = await dbContext.Regioes.FindAsync(id);

            if (regiao == null)
            {
                return NotFound("Região não encontrada.");
            }

            return Ok(regiao);
        }

        
        [HttpGet("estado/{uf}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByEstado(string uf)
        {
            var regioes = await dbContext.Regioes
                .Where(r => r.Estado.ToLower() == uf.ToLower())
                .ToListAsync();
            return Ok(regioes);
        }

       
        [HttpGet("risco/{nivel}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByRisco(string nivel)
        {
            var regioes = await dbContext.Regioes
                .Where(r => r.RiscoNivel.ToLower() == nivel.ToLower())
                .ToListAsync();
            return Ok(regioes);
        }

        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Regiao regiao)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            dbContext.Regioes.Add(regiao);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = regiao.IdRegiao }, regiao);
        }

       
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Regiao regiaoAtualizada)
        {
            if (id != regiaoAtualizada.IdRegiao)
            {
                return BadRequest("O ID da URL não confere com o ID do corpo da requisição.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existente = await dbContext.Regioes.FindAsync(id);
            if (existente == null)
            {
                return NotFound("Região não encontrada.");
            }

            existente.NmRegiao = regiaoAtualizada.NmRegiao;
            existente.Estado = regiaoAtualizada.Estado;
            existente.Bioma = regiaoAtualizada.Bioma;
            existente.AreaKm2 = regiaoAtualizada.AreaKm2;
            existente.Latitude = regiaoAtualizada.Latitude;
            existente.Longitude = regiaoAtualizada.Longitude;
            existente.RiscoNivel = regiaoAtualizada.RiscoNivel;

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id)
        {
            var regiao = await dbContext.Regioes.FindAsync(id);
            if (regiao == null)
            {
                return NotFound("Região não encontrada.");
            }

            try
            {
                dbContext.Regioes.Remove(regiao);
                await dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                
                return Conflict("Não é possível remover esta região porque ela possui alertas vinculados. Remova os alertas antes.");
            }
        }
    }
}