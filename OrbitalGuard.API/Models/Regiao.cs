using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrbitalGuard.API.Models
{
    
    /// Região do território brasileiro monitorada pela plataforma OrbitalGuard ,(ex: Vale do Itajaí, Pantanal Norte, Sul da Bahia).
    /// É o ponto de partida do monitoramento: cada região concentra alertas e pode ser acompanhada por vários usuários.
    [Table("TB_REGIAO")]
    public class Regiao
    {
        [Key]
        [Column("ID_REGIAO")]
        public int IdRegiao { get; set; }

        [Required(ErrorMessage = "O nome da região é obrigatório")]
        [MaxLength(150)]
        [Column("NM_REGIAO")]
        public string NmRegiao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado (UF) é obrigatório")]
        [MaxLength(2, ErrorMessage = "Use a sigla da UF com 2 letras (ex: SC)")]
        [Column("ESTADO")]
        public string Estado { get; set; } = string.Empty;

        /// Bioma predominante (AMAZONIA, CERRADO, MATA_ATLANTICA, PANTANAL, CAATINGA, PAMPA)
        [MaxLength(50)]
        [Column("BIOMA")]
        public string? Bioma { get; set; }

        [Column("AREA_KM2")]
        public decimal? AreaKm2 { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude deve estar entre -90 e 90")]
        [Column("LATITUDE")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude deve estar entre -180 e 180")]
        [Column("LONGITUDE")]
        public double? Longitude { get; set; }

        /// Nível de risco classificado (BAIXO, MEDIO, ALTO, CRITICO). Default: BAIXO
        [MaxLength(20)]
        [Column("RISCO_NIVEL")]
        public string RiscoNivel { get; set; } = "BAIXO";

        // Aqui temos um relacionamento 1:N como pedido, once uma região concentra vários alertas
        public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();

        // Aqui temos um relacionamento N:N uma região e monitorada por varios usuarios (via TB_USUARIO_REGIAO)
        public ICollection<UsuarioRegiao> UsuariosMonitorando { get; set; } = new List<UsuarioRegiao>();
    }
}