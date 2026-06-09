namespace OrbitalGuard.API.DTOs
{
    
    public class ResumoRegiaoDTO
    {
        public string Regiao { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? Bioma { get; set; }
        public string RiscoNivel { get; set; } = string.Empty;
        public int TotalAlertas { get; set; }
        public int Criticos { get; set; }
        public int Altos { get; set; }
        public int Ativos { get; set; }
        public DateTime? UltimoAlerta { get; set; }
    }
}