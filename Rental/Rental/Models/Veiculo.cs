using Microsoft.Build.Framework;
using System.ComponentModel;

namespace Rental.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        [Required]
        public string Tipo { get; set; }
        [Required]
        public string Marca { get; set; }
        [Required]
        public string Modelo { get; set; }
        [Required]
        [DisplayName("Preço")] //Preço por dia
        public double Preco { get; set; }
        [Required]
        [DisplayName("Localização")]
        public string Localizacao { get; set; }
        [DisplayName("Fotografia")]
        public string? FotoURL { get; set; }
        [Required]
        [DisplayName("Kms Totais")]
        public double Km { get; set; }
        [DisplayName("Transmissão")]
        public string? Transmissao { get; set; }
        [DisplayName("Tipo de Combustível")]
        public string? TipoCombustivel { get; set; }
        [DisplayName("Número de Portas")]
        public int? NumPortas { get; set; }
        [DisplayName("Número de Aseentos")]
        public int? NumAssentos { get; set; }
        [DisplayName("Numero de Camas")]
        public int? NumCamas { get; set; }
        [DisplayName("Idade Minima")]
        public int? IdadeMinima { get; set; }
        [DisplayName("Licença")]
        public string? Licenca { get; set; }
        [DisplayName("Cilindrada")]
        public int? Cilindrada { get; set; }

        //se o veiculo se encontra ativo (estado == true)
        public bool Estado { get; set; } = true;
        public bool Eliminado { get; set; } = false;
        public Categoria? categoria { get; set; }
        public int? CategoriaId { get; set; }
        public Empresa? empresa { get; set; }
        public int? EmpresaId { get; set; }
        public ICollection<Reserva>? reservas { get; set; }

    }
}
