using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrbitalGuard.API.Models
{
    /// Usuário da plataforma OrbitalGuard. Pode ser agricultor, integrante da defesa civil, pesquisador ou administrador. Cada usuário se inscreve nas regiões que quer monitorar
    /// e recebe notificações dos alertas dessas regiões.
    [Table("TB_USUARIO")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "O nome do usuário é obrigatório")]
        [MaxLength(150)]
        [Column("NM_USUARIO")]
        public string NmUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [MaxLength(200)]
        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        /// Hash da senha (bcrypt / argon2). Nunca trafega em GETs
        [Required(ErrorMessage = "A senha (hash) é obrigatória")]
        [MaxLength(255)]
        [Column("SENHA_HASH")]
        public string SenhaHash { get; set; } = string.Empty;

        /// Perfil de acesso: AGRICULTOR, DEFESA_CIVIL, PESQUISADOR, ADMIN.
        [MaxLength(30)]
        [Column("PERFIL")]
        public string Perfil { get; set; } = "AGRICULTOR";

        [Column("ATIVO")]
        public bool Ativo { get; set; } = true;

        [Column("DT_CADASTRO")]
        public DateTime DtCadastro { get; set; } = DateTime.UtcNow;

        // 1:N  um usuário vai recebe várias notificações
        public ICollection<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();

        // N:N aqui um usuário monitora várias regiões (via TB_USUARIO_REGIAO)
        public ICollection<UsuarioRegiao> RegioesMonitoradas { get; set; } = new List<UsuarioRegiao>();
    }
}