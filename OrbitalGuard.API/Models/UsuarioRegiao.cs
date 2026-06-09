using System.ComponentModel.DataAnnotations.Schema;

namespace OrbitalGuard.API.Models
{
  
    [Table("TB_USUARIO_REGIAO")]
    public class UsuarioRegiao
    {
        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }

        [Column("ID_REGIAO")]
        public int IdRegiao { get; set; }

        [ForeignKey("IdRegiao")]
        public Regiao? Regiao { get; set; }

        /// Quando o usuário se inscreveu para monitorar essa região.
        [Column("DT_INSCRICAO")]
        public DateTime DtInscricao { get; set; } = DateTime.UtcNow;
    }
}