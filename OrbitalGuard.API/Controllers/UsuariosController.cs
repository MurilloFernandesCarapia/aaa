using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrbitalGuard.API.Data;
using OrbitalGuard.API.Models;

namespace OrbitalGuard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public UsuariosController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

       
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await dbContext.Usuarios.ToListAsync();
            return Ok(usuarios);
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await dbContext.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            return Ok(usuario);
        }

        
        [HttpGet("perfil/{perfil}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPerfil(string perfil)
        {
            var usuarios = await dbContext.Usuarios
                .Where(u => u.Perfil.ToLower() == perfil.ToLower())
                .ToListAsync();
            return Ok(usuarios);
        }

     
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                dbContext.Usuarios.Add(usuario);
                await dbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = usuario.IdUsuario }, usuario);
            }
            catch (DbUpdateException)
            {
                // Violação do índice único de e-mail
                return Conflict("Já existe um usuário cadastrado com este e-mail.");
            }
        }

        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int id, [FromBody] Usuario usuarioAtualizado)
        {
            if (id != usuarioAtualizado.IdUsuario)
            {
                return BadRequest("O ID da URL não confere com o ID do corpo da requisição.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existente = await dbContext.Usuarios.FindAsync(id);
            if (existente == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            existente.NmUsuario = usuarioAtualizado.NmUsuario;
            existente.Email = usuarioAtualizado.Email;
            existente.SenhaHash = usuarioAtualizado.SenhaHash;
            existente.Perfil = usuarioAtualizado.Perfil;
            existente.Ativo = usuarioAtualizado.Ativo;

            try
            {
                await dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Já existe outro usuário com este e-mail.");
            }
        }

       
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await dbContext.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            dbContext.Usuarios.Remove(usuario);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

       
        [HttpGet("{id}/regioes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRegioesMonitoradas(int id)
        {
            bool usuarioExiste = await dbContext.Usuarios.AnyAsync(u => u.IdUsuario == id);
            if (!usuarioExiste)
            {
                return NotFound("Usuário não encontrado.");
            }

            var regioes = await dbContext.UsuariosRegioes
                .Where(ur => ur.IdUsuario == id)
                .Include(ur => ur.Regiao)
                .Select(ur => new
                {
                    ur.IdRegiao,
                    Nome = ur.Regiao!.NmRegiao,
                    ur.Regiao.Estado,
                    ur.Regiao.Bioma,
                    ur.Regiao.RiscoNivel,
                    ur.DtInscricao
                })
                .ToListAsync();

            return Ok(regioes);
        }

        
        [HttpPost("{id}/regioes/{regiaoId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> InscreverEmRegiao(int id, int regiaoId)
        {
            bool usuarioExiste = await dbContext.Usuarios.AnyAsync(u => u.IdUsuario == id);
            if (!usuarioExiste)
            {
                return NotFound("Usuário não encontrado.");
            }

            bool regiaoExiste = await dbContext.Regioes.AnyAsync(r => r.IdRegiao == regiaoId);
            if (!regiaoExiste)
            {
                return NotFound("Região não encontrada.");
            }

            bool jaInscrito = await dbContext.UsuariosRegioes
                .AnyAsync(ur => ur.IdUsuario == id && ur.IdRegiao == regiaoId);
            if (jaInscrito)
            {
                return Conflict("Este usuário já está inscrito nesta região.");
            }

            var inscricao = new UsuarioRegiao
            {
                IdUsuario = id,
                IdRegiao = regiaoId,
                DtInscricao = DateTime.UtcNow
            };

            dbContext.UsuariosRegioes.Add(inscricao);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRegioesMonitoradas), new { id }, inscricao);
        }

        
        [HttpDelete("{id}/regioes/{regiaoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DesinscreverDeRegiao(int id, int regiaoId)
        {
            var inscricao = await dbContext.UsuariosRegioes
                .FirstOrDefaultAsync(ur => ur.IdUsuario == id && ur.IdRegiao == regiaoId);

            if (inscricao == null)
            {
                return NotFound("Esta inscrição não existe.");
            }

            dbContext.UsuariosRegioes.Remove(inscricao);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}