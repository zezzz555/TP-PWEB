using Rental.Models;
using System.ComponentModel;

namespace Rental.ViewModels
{
    public class ReservasCategoriaClienteDatasViewModel
    {
        public List<Reserva>? ListaDeReservas { get; set; }
        [DisplayName("Categoria")]
        public int? CategoriaId { get; set; }
       
        [DisplayName("Cliente")]
        public string? ClienteId { get; set; }
        [DisplayName("Data de Levantamento")]
        public DateTime? DataLevantamento { get; set; }
        [DisplayName("Data de Entrega")]
        public DateTime? DataEntrega { get; set; }

    }
}
