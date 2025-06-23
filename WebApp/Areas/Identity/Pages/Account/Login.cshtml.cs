using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using WebApp.Areas.Identity.Models;
using WebApp.Configuration;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IOptions<UrlSettings> _appSettings;

        public LoginModel(SignInManager<IdentityUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
            IOptions<UrlSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        [BindProperty]
        public LoginInput Login { get; set; }


        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class LoginInput : IValidatableObject
        {
            //[RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$", ErrorMessage = "O e-mail informado deve atender um formato padrão válido.")]
            public string Email { get; set; }

            [DataType(DataType.Password)] public string Password { get; set; }

            [Display(Name = "Remember me?")] public bool RememberMe { get; set; }

            public bool Submitted { get; set; } = false;

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                var results = new List<ValidationResult>();

                if (Submitted)
                {
                    if (string.IsNullOrEmpty(Email))
                        results.Add(new ValidationResult("Your email address is required", new[] { "Email" }));

                    if (string.IsNullOrEmpty(Password))
                        results.Add(new ValidationResult("Your password is required", new[] { "Password" }));
                }

                return results;
            }
        }

        public async Task OnGetAsync(int? notify, string message = null, string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            try
            {
#if DEBUG
                returnUrl = Url.Content("~/Usuario/Profile");
#else
			returnUrl = Url.Content("~/Usuario/Profile");
#endif

                if (!ModelState.IsValid) return Page();
                var result =
                    await _signInManager.PasswordSignInAsync(Login.Email, Login.Password, Login.RememberMe, true);

                var user = await _userManager.FindByNameAsync(Login.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuário não cadastrado ou não encontrado.");
                    return Page();
                }

                var roles = await _userManager.GetRolesAsync(user);

                switch (result.Succeeded)
                {
                    case false when result.IsLockedOut:
                        _logger.LogWarning("A sua conta foi bloqueada.");
                        ModelState.AddModelError(string.Empty, "A sua conta foi bloqueada.");
                        return RedirectToPage("./ForgotPassword");
                    case false:
                        ModelState.AddModelError(string.Empty, "Senha inválida.");
                        return Page();
                    case true when !user.EmailConfirmed:
                        {
                            _logger.LogInformation("Primeiro acesso do usuário.");

                            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                            var email = Login.Email;
                            var callbackUrl = Url.Page(
                                "/Account/FirstAccessPassword",
                                null,
                                new { email, code },
                                Request.Scheme);

                            return Redirect($"/Identity/Account/Manage/FirstAccessPassword?email={email}&code={code}");
                        }
                    case true:
                        _logger.LogInformation("User logged in.");

                        var userRole = roles.First();
                        //var userRole = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Role).Value;

                        switch (userRole)
                        {
                            case UserRoles.Aluno:
                                //return RedirectToPage("Login", new
                                //{
                                //    notify = (int)EnumNotify.Success,
                                //    message = $"Este usuário não possui permissão de acesso ao sistema DNA."
                                //});
                                returnUrl = Url.Content("~/Aluno/Profile");
                                break;
                            case UserRoles.AdministradorEad:
                                returnUrl = Url.Content("~/DashboardEad");
                                break;
                            case UserRoles.Administrador:
                            case UserRoles.Coordenador:
                            case UserRoles.Gestor:
                                returnUrl = Url.Content("~/Usuario/Profile");
                                break;
                            case UserRoles.Profissional:
                                returnUrl = Url.Content("~/Profissional/Profile");
                                break;
                        }
                        // append cookie with token to the http response
                        //CookieOptions? cookieOptions = new()
                        //{
                        //    HttpOnly = true,
                        //    SameSite = SameSiteMode.Strict,
                        //    Secure = true,
                        //    Expires = DateTime.Now.AddDays(1)
                        //};

                        var request = new UsuarioModel.LoginUsuarioRequest()
                        {
                            Email = user.Email!,
                            Password = Login.Password
                        };

                        var resultreturn  = await ApiClientFactory.Instance.LoginUsuario(request);

                        //Response.Cookies.Append("token", token.AccessToken, cookieOptions);

                        //if (user != null && await _userManager.CheckPasswordAsync(user, Login.Password))
                        //{
                        //    // append cookie with token to the http response
                        //    CookieOptions? cookieOptions = new()
                        //    {
                        //        HttpOnly = true,
                        //        SameSite = SameSiteMode.Strict,
                        //        Secure = true,
                        //        Expires = DateTime.Now.AddDays(1) 
                        //    };
                        //    Response.Cookies.Append("token", "token", cookieOptions);
                        //}

                        return LocalRedirect(returnUrl);
                    default:
                        // If we got this far, something failed, redisplay form
                        return Page();
                }
            }
            catch (Exception e)
            {
                return Page();
            }
        }
    }
}
