using Rental.Models;
using System.ComponentModel;

namespace Rental.ViewModels
{
    public class EmpresaFuncionariosViewmodel
    {
        [DisplayName("Empresa")]
        public Empresa? empresa { get; set; }
        [DisplayName("Funcionarios")]
        public List<ApplicationUser>? funcionarios { get; set; }
        [DisplayName("Gestores")]

        public List<ApplicationUser>? gestores { get; set; }
    }
    public class ListaEmpresasFuncionariosViewModel
    {
        [DisplayName("Empresas")]
        public List<EmpresaFuncionariosViewmodel>? empresas { get; set; }
        [DisplayName("Empresa")]
        public int? empresaId { get; set; }
        [DisplayName("Subscrição")]
        public bool? subscricao { get; set; } = null;
    }

}
