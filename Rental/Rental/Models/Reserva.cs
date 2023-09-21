using Microsoft.Build.Framework;
using System.ComponentModel;

namespace Rental.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        [DisplayName("Data do Pedido")] //data em que o cliente efetua a reserva
        public DateTime? DataPedido { get; set; }
        [Required]
        [DisplayName("Data de Levantamento")] //levantamento pelo cliente
        public DateTime DataLevantamento { get; set; }
        [Required]
        [DisplayName("Data de Entrega")] //entrega pelo cliente
        public DateTime DataEntrega { get; set; }
        [DisplayName("Duração em Horas")]
        public double? DuracaoEmHoras { get; set; }
        [DisplayName("Duração em Minutos")]
        public double? DuracaoEmMinutos { get; set; }
        [DisplayName("Custo")]
        public double? Custo { get; set; }
        [DisplayName("Em Trânsito")]
        public bool EmTransito { get; set; } = false;
        public bool Terminado { get; set; } = false;
        public bool Eliminado { get; set; } = false;
        public bool Confirmado { get; set; } = false;
        public bool Rejeitada { get; set; } = false;
        public int VeiculoId { get; set; }
        public Veiculo veiculo { get; set; }
        public string ClienteId { get; set; }
        public ApplicationUser cliente { get; set; }

        public ICollection<EstadoVeiculo>? estadosVeiculo { get; set; }

    }
}
