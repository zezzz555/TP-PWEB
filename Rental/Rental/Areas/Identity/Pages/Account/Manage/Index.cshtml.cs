// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rental.Models;

namespace Rental.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            //applicationUser new attributes
            [Display(Name = "Primeiro Nome")]
            public string PrimeiroNome { get; set; }
            [Display(Name = "Ultimo Nome")]
            public string UltimoNome { get; set; }
            [Display(Name = "Data de Nascimento")]
            public DateTime DataNascimento { get; set; }
            [Display(Name = "Número de Identificação Fiscal")]
            public int NIF { get; set; }
            [Display(Name = "O meu Avatar")]
            public byte[]? Avatar { get; set; }
            public IFormFile AvatarFile { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                //applicationUser new attributes
                PrimeiroNome = user.PrimeiroNome,
                UltimoNome = user.UltimoNome,
                DataNascimento = user.DataNascimento,
                NIF = user.NIF,
                Avatar = user.Avatar
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            //applicationUser new attributes
            if (Input.PrimeiroNome != user.PrimeiroNome)
            {
                user.PrimeiroNome = Input.PrimeiroNome;
            }

            if (Input.UltimoNome != user.UltimoNome)
            {
                user.UltimoNome = Input.UltimoNome;
            }

            if (Input.DataNascimento != user.DataNascimento)
            {
                user.DataNascimento = Input.DataNascimento;
            }

            if (Input.NIF != user.NIF)
            {
                user.NIF = Input.NIF;
            }

            if (Input.AvatarFile != null)
            {
                if (Input.AvatarFile.Length > (200 * 1024))
                {
                    StatusMessage = "Error: Ficheiro demasiado grande";
                    return RedirectToPage();
                }
               
                if (!IsValidFileType(Input.AvatarFile.FileName))
                {
                    StatusMessage = "Error: Ficheiro não suportado";
                    return RedirectToPage();
                }
                using (var dataStream = new MemoryStream())
                {
                    await Input.AvatarFile.CopyToAsync(dataStream);
                    user.Avatar = dataStream.ToArray();
                }
                await _userManager.UpdateAsync(user);
            }

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        // verifica se a extensão é .png,.jpg,.jpeg
        static bool IsValidFileType(string fileName)
        {
            if (fileName != null)
            {
                var extensao = fileName.Split(".");
                if (extensao[1].Contains("png") || extensao[1].Contains("jpg") || extensao[1].Contains("jpeg"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
