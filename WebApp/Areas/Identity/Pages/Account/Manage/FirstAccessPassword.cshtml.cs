using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Areas.Identity.Pages.Account.Manage
{
    [AllowAnonymous]
    public class FirstAccessPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public FirstAccessPasswordModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty] public InputModel Input { get; set; }

        public IActionResult OnGet(string email = null, string code = null)
        {
            if (email == null)
            {
                return BadRequest("Usuário não cadastrado.");
            }
            if (code == null)
            {
                return BadRequest("Um código deve ser fornecido para alteração de senha.");
            }
            Input = new InputModel
            {
                Email = email,
                Code = code
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByNameAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário não cadastrado.");
                return Page();
            }

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded) return Redirect("/Identity/Account/Manage/FirstAccessPasswordConfirmation");

            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        public class InputModel
        {
            [Required]
            //[RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$", ErrorMessage = "O e-mail informado deve atender um formato padrão válido.")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Formato de senha inválido, a senha deve conter no mínimo 8 digitos.",
                MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "As senhas informadas não conferem.")]
            public string ConfirmPassword { get; set; }
            public string Code { get; set; }
        }
    }
}