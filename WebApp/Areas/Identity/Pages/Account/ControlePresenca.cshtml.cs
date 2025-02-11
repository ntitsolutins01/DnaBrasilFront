using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Areas.Identity.Models;
using WebApp.Configuration;
using WebApp.Data;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ControlePresencaModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ControlePresencaModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IOptions<UrlSettings> _appSettings;

        public ControlePresencaModel(IOptions<UrlSettings> app,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ControlePresencaModel> logger,
            IEmailSender emailSender, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
		{
			_appSettings = app;
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
			_emailSender = emailSender;
			_roleManager = roleManager;
			_db = db;
			ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
		}
		public string ReturnUrl { get; set; }
		public int NotifyMessage { get; set; }
		public string Notify { get; set; }
        public int AlunoId { get; set; }

        public async Task OnGetAsync(int alunoId, int? notify, string message = null, string returnUrl = null)
        {
            AlunoId = alunoId;

			switch (notify)
			{
				case (int)EnumNotify.Success:
					NotifyMessage = (int)EnumNotify.Success;
					Notify = message;
					break;
				case (int)EnumNotify.Error:
					NotifyMessage = (int)EnumNotify.Error;
					Notify = message;
					break;
				case (int)EnumNotify.Warning:
					NotifyMessage = (int)EnumNotify.Warning;
					Notify = message;
					break;
				default:
					NotifyMessage = -1;
					Notify = "null";
					break;
			}

		}

		public async Task<IActionResult> OnPostAsync( IFormCollection collection)
		{
			string returnUrl = null;
			returnUrl ??= Url.Content("~/");

			if (!ModelState.IsValid) return Page();

            if (collection["alunoId"] == "0")
            {
                return RedirectToPage("ControlePresenca", new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Leia o QRCode novamente, caso o erro persista, favor entrar em contato com o administrador do sistema. " });
            }

            var usuario = await ApiClientFactory.Instance.GetUsuarioByEmail(collection["email"].ToString().Trim());

            if (usuario.Perfil.Nome == UserRoles.Administrador || usuario.Perfil.Nome == UserRoles.Profissional)
            {
                var result = await _signInManager.PasswordSignInAsync(collection["email"], collection["senha"], true, false);

                var aluno = await ApiClientFactory.Instance.GetAlunoById(Convert.ToInt32(collection["alunoId"]));

                if (result.Succeeded)
                {

                    var command = new WebApp.Models.ControlePresencaModel.CreateUpdateControlePresencaCommand()
                    {
                        MunicipioId = aluno.MunicipioId,
                        LocalidadeId = Convert.ToInt32(aluno.LocalidadeId),
                        Controle = Convert.ToBoolean((collection["justificativa"] == "").ToString()) ? "P" : "F",
                        Justificativa = collection["justificativa"].ToString(),
                        AlunoId = collection["alunoId"].ToString(),
                    };

                    var possuiPrecensa = ApiClientFactory.Instance.GetControlePresencaByAlunoId(Convert.ToInt32(command.AlunoId)).Where(x => x.Data == DateTime.Now.ToString("dd/MM/yyyy") && x.EventoId == null);

                    if (possuiPrecensa.Any())
                    {
                        return RedirectToPage("ControlePresenca", new { notify = (int)EnumNotify.Warning, message = "Já existe presença cadastrada para este aluno no dia de hoje." });
                    }

                    await ApiClientFactory.Instance.CreateControlePresenca(command);
                }
                return RedirectToPage("ControlePresenca", new { notify = (int)EnumNotify.Success, message = "Controle de presença realizado com sucesso" });
            }
            
            return RedirectToPage("ControlePresenca", new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Perfil de login informado é inválido." });
        }
	}
}
