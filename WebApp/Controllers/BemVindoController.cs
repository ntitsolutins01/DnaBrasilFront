using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebApp.Configuration;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    public class BemVindoController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IOptions<UrlSettings> _appSettings;

        public BemVindoController(UserManager<IdentityUser> userManager, IOptions<UrlSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        public async Task<IActionResult> Index()
        {
            var user = User.Identity.Name;

            var usuario = await ApiClientFactory.Instance.GetUsuarioByEmail(user);

            var result = ApiClientFactory.Instance.GetAlunoByEmail(usuario.Email);

            var model = new AlunoModel()
            {
                Aluno = result,
                NomePerfil = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role).Value
            };

            return View(model);
        }
    }
}
