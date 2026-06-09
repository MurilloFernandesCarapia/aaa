using Microsoft.EntityFrameworkCore;
using OrbitalGuard.API.Models;

namespace OrbitalGuard.API.Data
{
    
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Regiao> Regioes { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<UsuarioRegiao> UsuariosRegioes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            // E-mail de usuário é único no sistema (não pode ter dois cadastros com o mesmo e-mail)
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Nome da região é único POR estado (pode existir "Vale Verde" em SP e em MG sem conflito)
            modelBuilder.Entity<Regiao>()
                .HasIndex(r => new { r.NmRegiao, r.Estado })
                .IsUnique();

           

            // Área em km quadadro — até 10 dígitos inteiros e 2 casas decimais
            modelBuilder.Entity<Regiao>()
                .Property(r => r.AreaKm2)
                .HasPrecision(12, 2);

            // Regiao 1 ---N Alerta
            // não dá pra apagar uma região que tem alertas (preserva o histórico de eventos)
            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.Regiao)
                .WithMany(r => r.Alertas)
                .HasForeignKey(a => a.IdRegiao)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario 1 ---N Notificacao
            // ao apagar um usuário, as notificações dele são removidas junto (não fazem mais sentido)
            modelBuilder.Entity<Notificacao>()
                .HasOne(n => n.Usuario)
                .WithMany(u => u.Notificacoes)
                .HasForeignKey(n => n.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // Alerta 1 ---N Notificacao
            // não apaga alerta que já gerou notificações (preserva trilha de auditoria)
            modelBuilder.Entity<Notificacao>()
                .HasOne(n => n.Alerta)
                .WithMany(a => a.Notificacoes)
                .HasForeignKey(n => n.IdAlerta)
                .OnDelete(DeleteBehavior.Restrict);

            // um par (usuario, regiao) só pode aparecer uma vez
            modelBuilder.Entity<UsuarioRegiao>()
                .HasKey(ur => new { ur.IdUsuario, ur.IdRegiao });

            // UsuarioRegiao --N->1 Usuario  (ao deletar usuário, suas inscriçoes saem junto)
            modelBuilder.Entity<UsuarioRegiao>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.RegioesMonitoradas)
                .HasForeignKey(ur => ur.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // UsuarioRegiao --N->1 Regiao   (ao deletar região, as inscriçoes saem junto)
            modelBuilder.Entity<UsuarioRegiao>()
                .HasOne(ur => ur.Regiao)
                .WithMany(r => r.UsuariosMonitorando)
                .HasForeignKey(ur => ur.IdRegiao)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}