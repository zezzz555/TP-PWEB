using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rental.Models
{
	public class Empresa
	{
		public int Id { get; set; }
		[Required]
		[DisplayName("Nome da Empresa")]
		public string Nome { get; set; }
		[Required]
		[DisplayName("Avaliação")]
		public double Avaliacao { get; set; }
		[DisplayName("Subscrição")]
		public bool Susbcricao { get; set; } = true;
        [NotMapped]
        public IFormFile? LogoFile { get; set; }
        [DisplayName("Logotipo da Empresa")]
        public byte[]? Logo { get; set; } = null;
        public ICollection<Veiculo>? veiculos { get; set; }
        //funcionarios são tanto gestores como funcionarios
        public ICollection<ApplicationUser>? funcionarios { get; set; }
        public bool Eliminado { get; set; } = false;

    }


}
