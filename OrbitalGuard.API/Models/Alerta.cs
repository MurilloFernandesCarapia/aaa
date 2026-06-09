using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrbitalGuard.API.Models
{
    
    /// Alerta climático ou de desastre gerado para uma região específica
    /// (enchente, seca, queimada, tempestade, deslizamento etc.)
    [Table("TB_ALERTA")]
    public class Alerta
    {
        [Key]
        [Column("ID_ALERTA")]
        public int IdAlerta { get; set; }

        /// Tipo do alerta: ENCHENTE, SECA, QUEIMADA, TEMPESTADE, DESLIZAMENTO
        [Required(ErrorMessage = "O tipo do alerta é obrigatório")]
        [MaxLength(50)]
        [Column("TIPO")]
        public string Tipo { get; set; } = string.Empty;

        /// Nível de severidade: BAIXO, MEDIO, ALTO, CRITICO
        [Required(ErrorMessage = "O nível do alerta é obrigatório")]
        [MaxLength(20)]
        [Column("NIVEL")]
        public string Nivel { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("DESCRICAO")]
        public string? Descricao { get; set; }

        [Column("DT_HORA")]
        public DateTime DtHora { get; set; } = DateTime.UtcNow;

        [Range(-90, 90, ErrorMessage = "Latitude deve estar entre -90 e 90")]
        [Column("LATITUDE")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude deve estar entre -180 e 180")]
        [Column("LONGITUDE")]
        public double? Longitude { get; set; }

        [Column("ATIVO")]
        public bool Ativo { get; set; } = true;

        
        [Required]
        [Column("ID_REGIAO")]
        public int IdRegiao { get; set; }

        [ForeignKey("IdRegiao")]
        public Regiao? Regiao { get; set; }

        // 1:N isso aqui serve para um alerta pode gerar várias notificações (uma por usuário inscrito na região)
        public ICollection<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();
    }
}