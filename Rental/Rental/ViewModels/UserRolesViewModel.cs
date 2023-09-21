using Rental.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rental.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public string UserName { get; set; }
        public byte[]? Avatar { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataRegisto { get; set; }

        public IEnumerable<string>? Roles { get; set; }
        //  !!Apenas funciona com o nome RoleId apesar de ser o nome do role a ser guardado
        public string? RoleId { get; set; }
        public int? EmpresaId { get; set; }
    }
}
