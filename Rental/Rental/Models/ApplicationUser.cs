using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Rental.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Primeiro Nome")]
        public string PrimeiroNome { get; set; }
        [Display(Name = "Ultimo Nome")]
        public string UltimoNome { get; set; }
        [Display(Name = "Data Nascimento")]
        [PersonalData]
        public DateTime DataNascimento { get; set; }
        [Display(Name = "NIF")]
        [PersonalData]
        public int NIF { get; set; }
        [Display(Name = "O meu Avatar")]
        public byte[]? Avatar { get; set; }
        [NotMapped]
        public IFormFile? AvatarFile { get; set; }
        //se for cliente
        public ICollection<Reserva>? Reservas { get; set; }
        public int? EmpresaId { get; set; }
        public Empresa? empresa { get; set; }
        public string? roleEmpresa { get; set; }

        public bool Ativo { get; set; } = true;

        public bool Eliminado { get; set; } = false;

        public DateTime? DataRegisto { get; set; }

    }
}
