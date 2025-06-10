using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WebApp.Configuration;
using WebApp.Data;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IOptions<UrlSettings> _appSettings;
        private readonly IHostingEnvironment _host;

        public RegisterModel(IOptions<UrlSettings> app,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            IHostingEnvironment host)
        {
            _appSettings = app;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
            _host = host;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public int EstadoId { get; set; }
        public int MunicipioId { get; set; }
        public int LocalidadeId { get; set; }
        public int EtniaId { get; set; }
        public int ModalidadeId { get; set; }
        public int DeficienciaId { get; set; }
        public SelectList ListEstados { get; set; }
        public SelectList ListMunicipios { get; set; }
        public SelectList ListLocalidades { get; set; }
        public SelectList ListEtnias { get; set; }
        public SelectList ListModalidades { get; set; }
        public SelectList ListDeficiencia { get; set; }
        public int NotifyMessage { get; set; }
        public string Notify { get; set; }
        public string FomentoId { get; set; }
        public SelectList ListFomentos { get; set; }

        public async Task OnGetAsync(int? notify, string message = null, string returnUrl = null)
        {
            ReturnUrl = returnUrl;
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

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ListEstados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
            ListFomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome");

            List<SelectListDto> list = new List<SelectListDto>
            {
                new() { IdNome = "PARDO", Nome = "PARDO" },
                new() { IdNome = "BRANCO", Nome = "BRANCO" },
                new() { IdNome = "PRETO", Nome = "PRETO" },
                new() { IdNome = "INDIGENA", Nome = "INDIGENA" },
                new() { IdNome = "AMARELO", Nome = "AMARELO" }
            };

            var etnias = new SelectList(list, "IdNome", "Nome");
            ListEtnias = etnias;

            var modalidades = new SelectList(ApiClientFactory.Instance.GetModalidadeAll(), "Id", "Nome");
            ListModalidades = modalidades;

            var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll().Where(x => x.Status), "Id", "Nome");
            ListDeficiencia = deficiencias;

        }

        public async Task<IActionResult> OnPostAsync(IFormCollection collection)
        {
            var commandAluno = new AlunoModel.CreateUpdateDadosAlunoCommand()
            {
                MunicipioId = collection["ddlMunicipio"] == "" ? null : Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                LocalidadeId = collection["ddlLocalidade"] == "" ? null : Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                DeficienciaId = collection["ddlDeficiencia"] == "" ? null : Convert.ToInt32(collection["ddlDeficiencia"].ToString()),
                Endereco = collection["endereco"] == "" ? null : collection["endereco"].ToString(),
                Nome = collection["nome"] == "" ? null : collection["nome"].ToString(),
                Cpf = collection["cpf"] == "" ? null : collection["cpf"].ToString(),
                Sexo = collection["ddlSexo"] == "" ? null : collection["ddlSexo"].ToString(),
                DtNascimento = collection["dtNasc"] == "" ? null : collection["dtNasc"].ToString(),
                Email = collection["email"] == "" ? null : collection["email"].ToString(),
                Celular = collection["numCelular"] == "" ? null : collection["numCelular"].ToString(),
                Etnia = collection["ddlEtnia"] == "" ? null : collection["ddlEtnia"].ToString(),
                NomeMae = collection["nomeMae"] == "" ? null : collection["nomeMae"].ToString(),
                NomeResponsavel = collection["nomeResp"] == "" ? null : collection["nomeResp"].ToString(),
                DeficienciasIds = collection["ddlDeficiencia"] == "" ? null : collection["ddlDeficiencia"].ToString(),
                Habilitado = true,
                Status = true,
                UtilizacaoImagem = Convert.ToBoolean(collection["utilizacaoImagem"].ToString()),
                ParticipacaoProgramaCompartilhamentoDados = Convert.ToBoolean(collection["participacao"].ToString()),
                AutorizacaoConsentimentoAssentimento = collection["agreeterms"].ToString() != "",
                FomentoId = ApiClientFactory.Instance.GetFomentoByLocalidadeId(Convert.ToInt32(collection["ddlLocalidade"].ToString())).Id,
                ModalidadesIds = collection["ddlModalidadeDesejada"] == "" ? null : collection["ddlModalidadeDesejada"].ToString(),
            };


            var validaAluno = await ApiClientFactory.Instance.GetAlunosByFilter(new AlunosFilterDto()
            {
                Nome = commandAluno.Nome,
                DataNascimento = commandAluno.DtNascimento,
                Cpf = commandAluno.Cpf
            });

            if (validaAluno.Alunos.Any())
            {
                return RedirectToPage("Register", new { notify = (int)EnumNotify.Error, message = $"Já existe um aluno cadastrado com estas informações." });
            }

            //var newUser = new IdentityUser { UserName = commandAluno.Email, Email = commandAluno.Email };
            //var aspNetUser = await _userManager.CreateAsync(newUser, "12345678");

            //StringBuilder msg = new StringBuilder();
            //if (!aspNetUser.Succeeded)
            //{
            //    foreach (var error in aspNetUser.Errors)
            //    {
            //        ModelState.AddModelError(string.Empty, error.Description);
            //        msg.AppendLine(error.Description);
            //    }

            //    // Se chegamos até aqui, algo falhou, exiba novamente o formulário
            //    //return Page();
            //    return RedirectToPage("Register", new { notify = (int)EnumNotify.Error, message = msg });
            //}

            //var includedUserId = _userManager.Users.FirstOrDefault(x => x.Email == newUser.Email).Id;

            //var perfil = ApiClientFactory.Instance.GetPerfilById((int)EnumPerfil.Aluno);

            //var command = new UsuarioModel.CreateUpdateUsuarioCommand
            //{
            //    Email = collection["email"].ToString(),
            //    Nome = collection["nome"].ToString(),
            //    CpfCnpj = collection["cpf"].ToString(),
            //    //AspNetUserId = includedUserId,
            //    AspNetRoleId = perfil.AspNetRoleId,
            //    PerfilId = perfil.Id,
            //    MunicipioId = (int)commandAluno.MunicipioId,
            //    TipoPessoa = "pf"
            //};

            //var usu = await ApiClientFactory.Instance.CreateUsuario(command);

            //var userRole = _roleManager.Roles.FirstOrDefault(x => x.Id == perfil.AspNetRoleId).Name;

            //await _userManager.AddToRoleAsync(newUser, userRole);

            //commandAluno.AspNetUserId = command.AspNetUserId;

            var alunoId = await ApiClientFactory.Instance.CreateDados(commandAluno);

            //SendNewUserEmail(newUser, command.Email, command.Nome);

            string returnUrl = null;
            returnUrl ??= Url.Content("~/");

            return RedirectToPage("Register", new { notify = (int)EnumNotify.Success, message = $"Aluno cadastrado com sucesso. Matrícula: {alunoId}. Para efetivação da matrícula, compareça à unidade com os seguintes documentos: cópia do RG e CPF do aluno e do responsável e laudo médico para Pessoas com Deficiência." });
        }

        private async Task SendNewUserEmail(IdentityUser user, string email, string nome)
        {
            string returnUrl = null;
            returnUrl ??= Url.Content("~/");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = Url.ActionLink("ResetPassword",
                "Identity/Account", new { code, email });

            var message =
                System.IO.File.ReadAllText(Path.Combine(_host.WebRootPath, "emailtemplates/ConfirmEmail.html"));
            message = message.Replace("%NAME%", nome);
            message = message.Replace("%CALLBACK%", HtmlEncoder.Default.Encode(callbackUrl.Replace("%2FAccount", "/Account")));

            await _emailSender.SendEmailAsync(user.Email, "Primeiro acesso sistema Dna do Brasil",
                message);
        }
    }
}
