using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rental.Models
{
	public class Categoria
	{
		public int Id { get; set; }
		[Required]
		[DisplayName("Nome da Categoria")]
		public string Nome { get; set; }
		public ICollection<Veiculo>? veiculos { get; set; }
        public bool Eliminado { get; set; } = false;
    }
}
