using Rental.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental.ViewModels
{
    public class ReservaViewModel
    {
        [Display(Name = "Data de Levantamento", Prompt = "yyyy-mm-dd")]
        public DateTime DataLevantamento { get; set; }
        [Display(Name = "Data de Entrega", Prompt = "yyyy-mm-dd")]
        public DateTime DataEntrega { get; set; }
        [DisplayName("Custo")]
        public double? Custo { get; set; }
        [DisplayName("Duração em Dias")]
        public double DuracaoEmDias { get; set; }
        [DisplayName("Duração em Horas")]
        public double DuracaoEmHoras { get; set; }
        [DisplayName("Duração em Minutos")]
        public double DuracaoEmMinutos { get; set; }

        [Display(Name = "Modelo de Veiculo", Prompt = "Escolha o modelo do veiculo que pretende")]
        public int? VeiculoId { get; set; }
        public Veiculo? veiculo { get; set; }
        public string ClienteId { get; set; }
        public ApplicationUser cliente { get; set; }
    }
}
