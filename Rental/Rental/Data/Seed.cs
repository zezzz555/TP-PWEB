using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rental.Models;
using System.Data;
using System.IO;
using System.Numerics;
using System.Text;

namespace Rental.Data
{
    public enum Roles
    {
        Admin,
        Funcionario,
        Cliente,
        Gestor
    }
    public class Seed
    {
        public static async Task Seeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            //Adicionar default Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Funcionario.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Cliente.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Gestor.ToString()));

            if (!context.Categoria.Any())
            {
                var categorias = new List<Categoria>()
                {
                    new Categoria()
                    {
                        Nome = "Carro"
                    },
                    new Categoria()
                    {
                        Nome = "Mota"
                    },
                    new Categoria()
                    {
                        Nome = "Caravana"
                    }
                };
                context.Categoria.AddRange(categorias);
                context.SaveChanges();
            }

            if (!context.Empresa.Any())
            {
                var empresas = new List<Empresa>()
                {
                    new Empresa()
                    {
                        Nome="Ace",
                        Avaliacao =8
                    },
                    new Empresa()
                    {
                        Nome="Budget",
                        Avaliacao =4
                    },
                    new Empresa()
                    {
                        Nome="Hertz",
                        Avaliacao = 6
                    },
                    new Empresa()
                    {
                        Nome="VIP",
                        Avaliacao =10
                    },
                     new Empresa()
                     {
                        Nome="Routes",
                        Avaliacao =7
                     }
                };
                context.Empresa.AddRange(empresas);
                context.SaveChanges();
                foreach (var emp in empresas)
                {
                    var gestor = new ApplicationUser
                    {
                        UserName = "gestor" + emp.Nome + "@localhost.com",
                        Email = "gestor" + emp.Nome + "@localhost.com",
                        PrimeiroNome = "Gestor",
                        UltimoNome = emp.Nome,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        EmpresaId = emp.Id,
                        empresa = emp,
                        roleEmpresa = "Gestor",
                        DataRegisto = DateTime.Now
                    }; 
                    ApplicationUser Gestor = await userManager.FindByEmailAsync(gestor.Email);
                    if (Gestor == null)
                    {
                        await userManager.CreateAsync(gestor, "Gestor..00");
                        await userManager.AddToRoleAsync(gestor, Roles.Gestor.ToString());
                        emp.funcionarios.Add(gestor);
                        context.SaveChanges();
                    }
                    var funcionario = new ApplicationUser
                    {
                        UserName = "funcionario" + emp.Nome + "@localhost.com",
                        Email = "funcionario" + emp.Nome + "@localhost.com",
                        PrimeiroNome = "Funcionario",
                        UltimoNome = emp.Nome,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        EmpresaId = emp.Id,
                        empresa = emp,
                        roleEmpresa = "Funcionario",
                        DataRegisto = DateTime.Now
                    };
                    ApplicationUser Funcionario = await userManager.FindByEmailAsync(gestor.Email);
                    if (Funcionario == null)
                    {
                        await userManager.CreateAsync(funcionario, "Funcionario..00");
                        await userManager.AddToRoleAsync(funcionario, Roles.Gestor.ToString());
                        emp.funcionarios.Add(funcionario);
                        context.SaveChanges();
                    }
                }
            }

            if (!context.Veiculo.Any())
            {
                var carro = context.Categoria.Where(c => c.Eliminado == false && c.Nome.Equals("Carro")).FirstOrDefault();
                var mota = context.Categoria.Where(c => c.Eliminado == false && c.Nome.Equals("Mota")).FirstOrDefault();
                var caravana = context.Categoria.Where(c => c.Eliminado == false && c.Nome.Equals("Caravana")).FirstOrDefault();
                var empresas = context.Empresa.Where(c => c.Eliminado == false).ToArray();
                if(carro!=null && mota != null && caravana!=null && empresas != null)
                {
                var veiculos = new List<Veiculo>()
                {
                    new Veiculo()
                    {
                        Tipo ="Economico",
                        Marca ="Mitsubishi",
                        Modelo = "Mirage",
                        Preco = 13.0,
                        Localizacao = "Lisboa",
                        FotoURL = "~/images/Veiculos/Carros/mitsubishi-mirage.jpg",
                        Km = 300,
                        NumAssentos = 5,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Gasolina ",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[0],
                        EmpresaId = empresas[0].Id

                    },      
                    new Veiculo()
                    {
                        Tipo ="Compacto",
                        Marca ="Ford",
                        Modelo = "Focus",
                        Preco = 14.0,
                        Localizacao = "Coimbra",
                        FotoURL = "~/images/Veiculos/Carros/ford-focus.jpeg",
                        Km = 250,
                        NumAssentos = 5,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Hibidro ",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[1],
                        EmpresaId = empresas[1].Id
                    },                 
                    new Veiculo()
                    {
                        Tipo ="Intermedio",
                        Marca ="Toyota",
                        Modelo = "Corolla",
                        Preco = 19.0,
                        Localizacao = "Porto",
                        FotoURL = "~/images/Veiculos/Carros/toyota-corolla.jpg",
                        Km = 350,
                        NumAssentos = 5,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Hibidro ",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[2],
                        EmpresaId = empresas[2].Id
                    },                
                    new Veiculo()
                    {
                        Tipo ="Intermedio",
                        Marca ="Hyundai",
                        Modelo = "Elantra",
                        Preco = 15.0,
                        Localizacao = "Braga",
                        FotoURL = "~/images/Veiculos/Carros/hyundai-elantra.jpg",
                        Km = 150,
                        NumAssentos = 5,
                        Transmissao = "Manual",
                        TipoCombustivel= "Gasolina",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[0],
                        EmpresaId = empresas[0].Id
                    },  
                    new Veiculo()
                    {
                        Tipo ="Grande",
                        Marca ="Nissan",
                        Modelo = "Altima",
                        Preco = 28.0,
                        Localizacao = "Porto",
                        FotoURL = "~/images/Veiculos/Carros/nissan-atima.jpg",
                        Km = 450,
                        NumAssentos = 5,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Hibidro ",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[1],
                        EmpresaId = empresas[1].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Grande",
                        Marca ="Toyota",
                        Modelo = "Camry",
                        Preco = 50.0,
                        Localizacao = "Faro",
                        FotoURL = "~/images/Veiculos/Carros/TOYOTA-Camry.jpg",
                        Km = 250,
                        NumAssentos = 5,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Hibidro ",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[2],
                        EmpresaId = empresas[2].Id
                    },             
                    new Veiculo()
                    {
                        Tipo ="Luxo",
                        Marca ="BMW",
                        Modelo = "5 series",
                        Preco = 97.0,
                        Localizacao = "Lisboa",
                        FotoURL = "~/images/Veiculos/Carros/bmw-5-series.jpg",
                        Km = 150,
                        NumAssentos = 5,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Hibidro ",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Luxo",
                        Marca ="BMW",
                        Modelo = "5 series",
                        Preco = 97.0,
                        Localizacao = "Porto",
                        FotoURL = "~/images/Veiculos/Carros/bmw-5-series.jpg",
                        Km = 150,
                        NumAssentos = 5,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Hibidro ",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Luxo",
                        Marca ="BMW",
                        Modelo = "5 series",
                        Preco = 97.0,
                        Localizacao = "Faro",
                        FotoURL = "~/images/Veiculos/Carros/bmw-5-series.jpg",
                        Km = 150,
                        NumAssentos = 5,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Hibidro ",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Luxo",
                        Marca ="Audi",
                        Modelo = "A5",
                        Preco = 105.0,
                        Localizacao = "Porto",
                        FotoURL = "~/images/Veiculos/Carros/audi-a5.jpg",
                        Km = 250,
                        NumAssentos = 4,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Gasolina ",
                        NumPortas= 3,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                        new Veiculo()
                    {
                        Tipo ="Luxo",
                        Marca ="Audi",
                        Modelo = "A5",
                        Preco = 105.0,
                        Localizacao = "Aveiro",
                        FotoURL = "~/images/Veiculos/Carros/audi-a5.jpg",
                        Km = 250,
                        NumAssentos = 4,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Gasolina ",
                        NumPortas= 3,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                        new Veiculo()
                    {
                        Tipo ="Luxo",
                        Marca ="Audi",
                        Modelo = "A5",
                        Preco = 105.0,
                        Localizacao = "Coimbra",
                        FotoURL = "~/images/Veiculos/Carros/audi-a5.jpg",
                        Km = 250,
                        NumAssentos = 4,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Gasolina ",
                        NumPortas= 3,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                        new Veiculo()
                    {
                        Tipo ="Luxo",
                        Marca ="Audi",
                        Modelo = "A5",
                        Preco = 105.0,
                        Localizacao = "Braga",
                        FotoURL = "~/images/Veiculos/Carros/audi-a5.jpg",
                        Km = 250,
                        NumAssentos = 4,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Gasolina ",
                        NumPortas= 3,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Classico",
                        Marca ="Ford",
                        Modelo = "Mustang Coupee",
                        Preco = 70.0,
                        Localizacao = "Coimbra",
                        FotoURL = "~/images/Veiculos/Carros/ford-mustang-classico.jpg",
                        Km = 550,
                        NumAssentos = 2,
                        Transmissao = "Manual",
                        TipoCombustivel= "Gasolina",
                        NumPortas= 3,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[0],
                        EmpresaId = empresas[0].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Classico",
                        Marca ="Ford",
                        Modelo = "Mustang Coupee",
                        Preco = 70.0,
                        Localizacao = "Braga",
                        FotoURL = "~/images/Veiculos/Carros/ford-mustang-classico.jpg",
                        Km = 550,
                        NumAssentos = 2,
                        Transmissao = "Manual",
                        TipoCombustivel= "Gasolina",
                        NumPortas= 3,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[1],
                        EmpresaId = empresas[1].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Classico",
                        Marca ="Ford",
                        Modelo = "Mustang Coupee",
                        Preco = 70.0,
                        Localizacao = "Lisboa",
                        FotoURL = "~/images/Veiculos/Carros/ford-mustang-classico.jpg",
                        Km = 550,
                        NumAssentos = 2,
                        Transmissao = "Manual",
                        TipoCombustivel= "Gasolina",
                        NumPortas= 3,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[2],
                        EmpresaId = empresas[2].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="SUV",
                        Marca ="Nissan",
                        Modelo = "Rogue",
                        Preco = 38.0,
                        Localizacao = "Lisboa",
                        FotoURL = "~/images/Veiculos/Carros/nissan-rogue.jpg",
                        Km = 250,
                        NumAssentos = 5,
                        Transmissao = "Manual",
                        TipoCombustivel= "Diesel",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[0],
                        EmpresaId = empresas[0].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="SUV",
                        Marca ="Nissan",
                        Modelo = "Rogue",
                        Preco = 38.0,
                        Localizacao = "Coimbra",
                        FotoURL = "~/images/Veiculos/Carros/nissan-rogue.jpg",
                        Km = 250,
                        NumAssentos = 5,
                        Transmissao = "Manual",
                        TipoCombustivel= "Diesel",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[1],
                        EmpresaId = empresas[1].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="SUV",
                        Marca ="Nissan",
                        Modelo = "Rogue",
                        Preco = 38.0,
                        Localizacao = "Aveiro",
                        FotoURL = "~/images/Veiculos/Carros/nissan-rogue.jpg",
                        Km = 250,
                        NumAssentos = 5,
                        Transmissao = "Manual",
                        TipoCombustivel= "Diesel",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[2],
                        EmpresaId = empresas[2].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="SUV",
                        Marca ="Nissan",
                        Modelo = "Rogue",
                        Preco = 38.0,
                        Localizacao = "Braga",
                        FotoURL = "~/images/Veiculos/Carros/nissan-rogue.jpg",
                        Km = 250,
                        NumAssentos = 5,
                        Transmissao = "Manual",
                        TipoCombustivel= "Diesel",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[0],
                        EmpresaId = empresas[0].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="SUV",
                        Marca ="Tesla",
                        Modelo = "Y",
                        Preco = 96.0,
                        Localizacao = "Porto",
                        FotoURL = "~/images/Veiculos/Carros/tesla-model-y.jpg",
                        Km = 100,
                        NumAssentos = 5,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Eletrico",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="SUV",
                        Marca ="Tesla",
                        Modelo = "Y",
                        Preco = 96.0,
                        Localizacao = "Lisboa",
                        FotoURL = "~/images/Veiculos/Carros/tesla-model-y.jpg",
                        Km = 100,
                        NumAssentos = 5,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Eletrico",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="SUV",
                        Marca ="Tesla",
                        Modelo = "Y",
                        Preco = 96.0,
                        Localizacao = "Faro",
                        FotoURL = "~/images/Veiculos/Carros/tesla-model-y.jpg",
                        Km = 100,
                        NumAssentos = 5,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Eletrico",
                        NumPortas= 5,
                        categoria = carro,
                        CategoriaId = carro.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="EcoLine",
                        Marca ="Sunlight",
                        Modelo = "Active T65",
                        Preco = 195.0,
                        Localizacao = "Faro",
                        FotoURL = "~/images/Veiculos/Caravanas/sunlight-active-t65.jpg",
                        Km = 400,
                        NumAssentos = 4,
                        NumCamas = 2,
                        Transmissao = "Manual",
                        TipoCombustivel= "Diesel",
                        NumPortas= 4,
                        categoria = caravana,
                        CategoriaId = caravana.Id,
                        empresa = empresas[4],
                        EmpresaId = empresas[4].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="EcoLine",
                        Marca ="Sunlight",
                        Modelo = "Active T65",
                        Preco = 195.0,
                        Localizacao = "Lisboa",
                        FotoURL = "~/images/Veiculos/Caravanas/sunlight-active-t65.jpg",
                        Km = 400,
                        NumAssentos = 4,
                        NumCamas = 2,
                        Transmissao = "Manual",
                        TipoCombustivel= "Diesel",
                        NumPortas= 4,
                        categoria = caravana,
                        CategoriaId = caravana.Id,
                        empresa = empresas[4],
                        EmpresaId = empresas[4].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Urban",
                        Marca ="Sunlight",
                        Modelo = "Cliff 601",
                        Preco = 95.0,
                        Localizacao = "Aveiro",
                        FotoURL = "~/images/Veiculos/Caravanas/sunlight-cliff-601.jpg",
                        Km = 500,
                        NumAssentos = 4,
                        NumCamas = 2,
                        Transmissao = "Manual",
                        TipoCombustivel= "Gasolina",
                        NumPortas= 4,
                        categoria = caravana,
                        CategoriaId = caravana.Id,
                        empresa = empresas[4],
                        EmpresaId = empresas[4].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Urban",
                        Marca ="Sunlight",
                        Modelo = "Cliff 601",
                        Preco = 95.0,
                        Localizacao = "Coimbra",
                        FotoURL = "~/images/Veiculos/Caravanas/sunlight-cliff-601.jpg",
                        Km = 500,
                        NumAssentos = 4,
                        NumCamas = 2,
                        Transmissao = "Manual",
                        TipoCombustivel= "Gasolina",
                        NumPortas= 4,
                        categoria = caravana,
                        CategoriaId = caravana.Id,
                        empresa = empresas[4],
                        EmpresaId = empresas[4].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Comfornt",
                        Marca ="Dethleffs",
                        Modelo = "Trend Edition",
                        Preco = 115.0,
                        Localizacao = "Braga",
                        FotoURL = "~/images/Veiculos/Caravanas/dethleffs-trend-edition.jpg",
                        Km = 200,
                        NumAssentos = 2,
                        NumCamas = 2,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Diesel",
                        NumPortas= 4,
                        categoria = caravana,
                        CategoriaId = caravana.Id,
                        empresa = empresas[1],
                        EmpresaId = empresas[1].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Comfornt",
                        Marca ="Dethleffs",
                        Modelo = "Trend Edition",
                        Preco = 115.0,
                        Localizacao = "Faro",
                        FotoURL = "~/images/Veiculos/Caravanas/dethleffs-trend-edition.jpg",
                        Km = 200,
                        NumAssentos = 2,
                        NumCamas = 2,
                        Transmissao = "Automatico",
                        TipoCombustivel= "Diesel",
                        NumPortas= 4,
                        categoria = caravana,
                        CategoriaId = caravana.Id,
                        empresa = empresas[0],
                        EmpresaId = empresas[0].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Trilho",
                        Marca ="BMW",
                        Modelo = "G GS",
                        Preco = 87.0,
                        Localizacao = "Lisboa",
                        FotoURL = "~/images/Veiculos/Motas/bmw-g-310-gs.jpg",
                        Km = 300,
                        IdadeMinima=21,
                        Licenca = "A,A2",
                        Cilindrada = 310,
                        TipoCombustivel= "Gasolina",
                        categoria = mota,
                        CategoriaId = mota.Id,
                        empresa = empresas[2],
                        EmpresaId = empresas[2].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Estrada",
                        Marca ="Yamaha",
                        Modelo = "Tracer GT",
                        Preco = 101.0,
                        Localizacao = "Coimbra",
                        FotoURL = "~/images/Veiculos/Motas/Yamaha-Tracer-700-GT.jpg",
                        Km = 200,
                        IdadeMinima=23,
                        Licenca = "A",
                        Cilindrada = 700,
                        TipoCombustivel= "Gasolina",
                        categoria = mota,
                        CategoriaId = mota.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Estrada",
                        Marca ="Yamaha",
                        Modelo = "Tracer GT",
                        Preco = 101.0,
                        Localizacao = "Braga",
                        FotoURL = "~/images/Veiculos/Motas/Yamaha-Tracer-700-GT.jpg",
                        Km = 200,
                        IdadeMinima=23,
                        Licenca = "A",
                        Cilindrada = 700,
                        TipoCombustivel= "Gasolina",
                        categoria = mota,
                        CategoriaId = mota.Id,
                        empresa = empresas[0],
                        EmpresaId = empresas[0].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Estrada",
                        Marca ="BMW",
                        Modelo = "R RT LC",
                        Preco = 101.0,
                        Localizacao = "Faro",
                        FotoURL = "~/images/Veiculos/Motas/BMW-R-1200-RT.jpg",
                        Km = 400,
                        IdadeMinima=23,
                        Licenca = "A",
                        Cilindrada = 1250,
                        TipoCombustivel= "Gasolina",
                        categoria = mota,
                        CategoriaId = mota.Id,
                        empresa = empresas[3],
                        EmpresaId = empresas[3].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Scooter",
                        Marca ="Honda",
                        Modelo = "Vision",
                        Preco = 101.0,
                        Localizacao = "Porto",
                        FotoURL = "~/images/Veiculos/Motas/honda-vision-110.jpg",
                        Km = 400,
                        IdadeMinima=18,
                        Licenca = "A,A1,A2",
                        Cilindrada = 110,
                        TipoCombustivel= "Diesel",
                        categoria = mota,
                        CategoriaId = mota.Id,
                        empresa = empresas[1],
                        EmpresaId = empresas[1].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Scooter",
                        Marca ="Yamaha",
                        Modelo = "N Max",
                        Preco = 39.00,
                        Localizacao = "Porto",                    
                        FotoURL = "~/images/Veiculos/Motas/yamaha-n-max-125.jpg",
                        Km = 500,
                        IdadeMinima=21,
                        Licenca = "A,A1,A2",
                        Cilindrada = 125,
                        TipoCombustivel= "Gasolina",
                        categoria = mota,
                        CategoriaId = mota.Id,
                        empresa = empresas[2],
                        EmpresaId = empresas[2].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Scooter",
                        Marca ="Yamaha",
                        Modelo = "N Max",
                        Preco = 39.00,
                        Localizacao = "Faro",                      
                        FotoURL = "~/images/Veiculos/Motas/yamaha-n-max-125.jpg",
                        Km = 500,
                        IdadeMinima=21,
                        Licenca = "A,A1,A2",
                        Cilindrada = 125,
                        TipoCombustivel= "Gasolina",
                        categoria = mota,
                        CategoriaId = mota.Id,
                        empresa = empresas[0],
                        EmpresaId = empresas[0].Id
                    },
                    new Veiculo()
                    {
                        Tipo ="Scooter",
                        Marca ="Yamaha",
                        Modelo = "N Max",
                        Preco = 39.00,
                        Localizacao = "Lisboa",                       
                        FotoURL = "~/images/Veiculos/Motas/yamaha-n-max-125.jpg",
                        Km = 500,
                        IdadeMinima=21,
                        Licenca = "A,A1,A2",
                        Cilindrada = 125,
                        TipoCombustivel= "Gasolina",
                        categoria = mota,
                        CategoriaId = mota.Id,
                        empresa = empresas[2],
                        EmpresaId = empresas[2].Id
                    },
                };

                    context.Veiculo.AddRange(veiculos);
                    context.SaveChanges();
                }
            }
      
            //Adicionar Default Users
            var admin = new ApplicationUser
            {
                UserName = "admin@localhost.com",
                Email = "admin@localhost.com",
                PrimeiroNome = "Administrador",
                UltimoNome = "Local",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                DataRegisto = DateTime.Now
            };
            var user = await userManager.FindByEmailAsync(admin.Email);
            if (user == null)
            {
                await userManager.CreateAsync(admin, "Admin..00");
                await userManager.AddToRoleAsync(admin,
                Roles.Admin.ToString());
            }
            //Add cliente
            var cliente = new ApplicationUser
            {
                UserName = "cliente@localhost.com",
                Email = "cliente@localhost.com",
                PrimeiroNome = "Cliente",
                UltimoNome = "Local",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                DataRegisto = new DateTime(2022,12,22,12,00,00)
            };
            var userCliente = await userManager.FindByEmailAsync(cliente.Email);
            if (userCliente == null)
            {
                await userManager.CreateAsync(cliente, "Cliente..00");
                await userManager.AddToRoleAsync(cliente,
                Roles.Cliente.ToString());
            }
            //Add cliente
            var cliente2 = new ApplicationUser
            {
                UserName = "cliente2@localhost.com",
                Email = "cliente2@localhost.com",
                PrimeiroNome = "Cliente2",
                UltimoNome = "Local",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                DataRegisto = new DateTime(2022, 08, 22, 12, 00, 00)
            };
            var userCliente2 = await userManager.FindByEmailAsync(cliente2.Email);
            if (userCliente2 == null)
            {
                await userManager.CreateAsync(cliente2, "Cliente..00");
                await userManager.AddToRoleAsync(cliente2,
                Roles.Cliente.ToString());
            }
            //Add cliente
            var cliente3 = new ApplicationUser
            {
                UserName = "cliente3@localhost.com",
                Email = "cliente3@localhost.com",
                PrimeiroNome = "Cliente3",
                UltimoNome = "Local",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                DataRegisto = new DateTime(2022, 12, 22, 12, 00, 00)
            };
            var userCliente3 = await userManager.FindByEmailAsync(cliente3.Email);
            if (userCliente3 == null)
            {
                await userManager.CreateAsync(cliente3, "Cliente..00");
                await userManager.AddToRoleAsync(cliente3,
                Roles.Cliente.ToString());
            }
            //Add cliente
            var cliente4 = new ApplicationUser
            {
                UserName = "cliente4@localhost.com",
                Email = "cliente4@localhost.com",
                PrimeiroNome = "Cliente4",
                UltimoNome = "Local",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                DataRegisto = new DateTime(2020, 12, 22, 12, 00, 00)
            };
            var userCliente4 = await userManager.FindByEmailAsync(cliente4.Email);
            if (userCliente4 == null)
            {
                await userManager.CreateAsync(cliente4, "Cliente..00");
                await userManager.AddToRoleAsync(cliente4,
                Roles.Cliente.ToString());
            }
            //Add cliente
            var cliente5 = new ApplicationUser
            {
                UserName = "cliente5@localhost.com",
                Email = "cliente5@localhost.com",
                PrimeiroNome = "Cliente5",
                UltimoNome = "Local",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                DataRegisto = new DateTime(2022, 11, 15, 12, 00, 00)
            };
            var userCliente5 = await userManager.FindByEmailAsync(cliente5.Email);
            if (userCliente5 == null)
            {
                await userManager.CreateAsync(cliente5, "Cliente..00");
                await userManager.AddToRoleAsync(cliente5,
                Roles.Cliente.ToString());
            }
        }
    }
}
