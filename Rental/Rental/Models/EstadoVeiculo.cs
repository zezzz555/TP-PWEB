using Microsoft.Build.Framework;
using System.ComponentModel;

namespace Rental.Models
{
    public class EstadoVeiculo
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Nº de Km")]
        public double? Kms { get; set; }
        public bool Danos { get; set; } = false;
        public string? Observações { get; set; }
        public bool Eliminado { get; set; } = false;
        public int? ReservaId { get; set; }
        public Reserva? reserva { get; set; }
        public string FuncionarioId { get; set; }
        public ApplicationUser funcionario { get; set; }
    }
}
