using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrbitalGuard.API.Models
{
    /// Notificação enviada a um usuário a partir de um alerta.
    /// Carrega o canal (PUSH/EMAIL/SMS) e o status de envio.
    [Table("TB_NOTIFICACAO")]
    public class Notificacao
    {
        [Key]
        [Column("ID_NOTIFICACAO")]
        public int IdNotificacao { get; set; }

        [Required(ErrorMessage = "A mensagem é obrigatória")]
        [MaxLength(500)]
        [Column("MENSAGEM")]
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>Canal de entrega: PUSH, EMAIL ou SMS.</summary>
        [MaxLength(30)]
        [Column("CANAL")]
        public string Canal { get; set; } = "PUSH";

        [Column("ENVIADA")]
        public bool Enviada { get; set; } = false;

        [Column("DT_ENVIO")]
        public DateTime? DtEnvio { get; set; }

   
        [Required]
        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }

        
        [Required]
        [Column("ID_ALERTA")]
        public int IdAlerta { get; set; }

        [ForeignKey("IdAlerta")]
        public Alerta? Alerta { get; set; }
    }
}