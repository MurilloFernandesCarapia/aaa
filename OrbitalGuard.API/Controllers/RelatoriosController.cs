using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrbitalGuard.API.Data;
using OrbitalGuard.API.DTOs;

namespace OrbitalGuard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public RelatoriosController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

       
        [HttpGet("resumo")]
        [ProducesResponseType(typeof(IEnumerable<ResumoRegiaoDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetResumo()
        {
            var resumo = await dbContext.Regioes
                .Select(r => new ResumoRegiaoDTO
                {
                    Regiao = r.NmRegiao,
                    Estado = r.Estado,
                    Bioma = r.Bioma,
                    RiscoNivel = r.RiscoNivel,
                    TotalAlertas = r.Alertas.Count,
                    Criticos = r.Alertas.Count(a => a.Nivel == "CRITICO"),
                    Altos = r.Alertas.Count(a => a.Nivel == "ALTO"),
                    Ativos = r.Alertas.Count(a => a.Ativo),
                    UltimoAlerta = r.Alertas.Max(a => (DateTime?)a.DtHora)
                })
                .OrderByDescending(x => x.TotalAlertas)
                .ToListAsync();

            return Ok(resumo);
        }

       
        [HttpGet("por-tipo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPorTipo()
        {
            var porTipo = await dbContext.Alertas
                .GroupBy(a => a.Tipo)
                .Select(g => new
                {
                    Tipo = g.Key,
                    Total = g.Count(),
                    Ativos = g.Count(a => a.Ativo),
                    Criticos = g.Count(a => a.Nivel == "CRITICO")
                })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            return Ok(porTipo);
        }

       
        [HttpGet("totais")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotais()
        {
            var totais = new
            {
                Regioes = await dbContext.Regioes.CountAsync(),
                RegioesCriticas = await dbContext.Regioes.CountAsync(r => r.RiscoNivel == "CRITICO"),
                Alertas = await dbContext.Alertas.CountAsync(),
                AlertasAtivos = await dbContext.Alertas.CountAsync(a => a.Ativo),
                Usuarios = await dbContext.Usuarios.CountAsync(),
                UsuariosAtivos = await dbContext.Usuarios.CountAsync(u => u.Ativo),
                Notificacoes = await dbContext.Notificacoes.CountAsync(),
                NotificacoesEnviadas = await dbContext.Notificacoes.CountAsync(n => n.Enviada),
                NotificacoesPendentes = await dbContext.Notificacoes.CountAsync(n => !n.Enviada)
            };
            return Ok(totais);
        }
    }
}