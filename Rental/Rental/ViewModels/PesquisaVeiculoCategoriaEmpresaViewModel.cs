using Rental.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rental.ViewModels
{ 
        public class PesquisaVeiculoCategoriaEmpresaViewModel
        {
            public List<Veiculo>? ListaDeVeiculos { get; set; }
            [DisplayName("Categoria")]
            public int? CategoriaId { get; set; }
            [DisplayName("Empresa")]
            public int? EmpresaId { get; set; }
            [DisplayName("Tipo de Veículo")]
            public string? TipoVeiculo { get; set; }
            [Required]
            [DisplayName("Localização")]
            public string? Localizacao { get; set; }
            [Required]
            [DisplayName("Data de Levantamento")]
            public DateTime DataLevantamento { get; set; }
            [Required]
            [DisplayName("Data de Entrega")]
            public DateTime DataEntrega { get; set; }
            [DisplayName("Número de resultados")]
            public int NumResultados { get; set; } = 0;
            public bool? Estado { get; set; } = null;
        }
    }
