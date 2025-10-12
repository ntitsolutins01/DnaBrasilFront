using System.Text.Encodings.Web;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Authorization;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;
using Claim = WebApp.Identity.Claim;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebApp.Controllers
{
    [Authorize(Policy = ModuloAccess.SistemaSocioeconomico)]
    public class SistemaSocioeconomicoController : BaseController
    {
        #region Parametros
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHostingEnvironment _host;
        private readonly ILog _logger;
        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">Configurações de urls do sistema</param>
        /// <param name="emailSender">Gerenciador de Email</param>
        /// <param name="userManager">Gerenciador de Usuario</param>
        /// <param name="host">Informações da aplicação em execução</param>
        /// <param name="roleManager">Gerenciador de regras de permissoes</param>
        /// <param name="logger">Log de mensagens da aplicação</param>
        public SistemaSocioeconomicoController(
            IOptions<UrlSettings> appSettings,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            IHostingEnvironment host,
            RoleManager<IdentityRole> roleManager,
            ILog logger)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _host = host;
            _roleManager = roleManager;
            _logger = logger;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
        }
        #endregion

        #region Crud Methods

        [ClaimsAuthorize(ClaimType.SistemaSocioeconomico, Claim.Consultar)]
        public IActionResult Parceiro(int? crud, int? notify, string message = null)
        {
            try
            {
                _logger.Info($"Usuario Logado em SistemaSocioeconomico.Index User.Identity.Name : {User.Identity.Name}");

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                var response = ApiClientFactory.Instance.GetParceiroAll();
                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");

                return View(new ParceiroModel() { Parceiros = response, ListEstados = estados });

            }
            catch (Exception e)
            {
                _logger.Error($"SistemaSocioeconomico.Index: {e.StackTrace}");
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error",
                    message = e.Message,
                    stackTrace = e.StackTrace
                });

            }

        }

        [ClaimsAuthorize(ClaimType.SistemaSocioeconomico, Claim.Incluir)]
        public ActionResult CreateParceiro(int? crud, int? notify, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);


                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");


                var tiposParcerias = new SelectList(ApiClientFactory.Instance.GetTipoParceriaAll().Where(x => x.Parceria == 1), "Id", "Nome");

                return View(new ParceiroModel() { ListEstados = estados, ListTiposParcerias = tiposParcerias });

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Parceiro), new { notify = (int)EnumNotify.Error, message = $"Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema. {e.Message}" });

            }
        }

        [HttpPost]
        [ClaimsAuthorize(ClaimType.SistemaSocioeconomico, Claim.Incluir)]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var status = collection["status"].ToString();
                var habilitado = collection["habilitado"].ToString();

                var command = new ParceiroModel.CreateUpdateParceiroCommand
                {

                    Nome = collection["nome"].ToString(),
                    RazaoSocial = collection["razaoSocial"].ToString(),
                    NomeContato = collection["nomeContato"].ToString(),
                    TipoPessoa = collection["tipoPessoa"].ToString(),
                    CpfCnpj = collection["tipoPessoa"].ToString() == "pf" ? collection["cpf"].ToString() : collection["cnpj"].ToString(),
                    Telefone = collection["numTelefone"].ToString() == "" ? null : collection["numTelefone"].ToString(),
                    Celular = collection["numCelular"].ToString() == "" ? null : collection["numCelular"].ToString(),
                    Cep = collection["cep"].ToString() == "" ? null : collection["cep"].ToString(),
                    Endereco = collection["endereco"].ToString() == "" ? null : collection["endereco"].ToString(),
                    Numero = collection["numero"].ToString() == "" ? 0 : Convert.ToInt32(collection["numero"].ToString()),
                    Bairro = collection["bairro"].ToString() == "" ? null : collection["bairro"].ToString(),
                    MunicipioId = Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                    Habilitado = habilitado != "",
                    Status = status != "",
                    Email = collection["email"].ToString(),
                    TipoParceriaId = Convert.ToInt32(collection["ddlTipoParceria"].ToString()),

                };

                await ApiClientFactory.Instance.CreateParceiro(command);

                return RedirectToAction(nameof(Parceiro), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Parceiro), new { notify = (int)EnumNotify.Error, message = $"Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema. {e.Message}" });

            }
        }

        [ClaimsAuthorize(ClaimType.SistemaSocioeconomico, Claim.Alterar)]
        public async Task<ActionResult> EditParceiro(int id)
        {
            try
            {
                ParceiroModel model = new ParceiroModel();
                {
                    var parceiro = await ApiClientFactory.Instance.GetParceiroById(id);
                    var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", parceiro.Uf);
                    var municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(parceiro.Uf), "Id", "Nome", parceiro.MunicipioId);

                    return View(new ParceiroModel() { ListEstados = estados, Parceiro = parceiro, ListMunicipios = municipios });
                }

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Parceiro), new { notify = (int)EnumNotify.Error, message = $"Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema. {e.Message}" });

            }
        }

        [HttpPost]
        [ClaimsAuthorize(ClaimType.SistemaSocioeconomico, Claim.Alterar)]
        public async Task<ActionResult> EditParceiro(int id, IFormCollection collection)
        {
            try
            {
                // Verifica se os campos existem no form
                bool status = false;
                bool habilitado = false;

                // Verifica se as chaves existem e têm valor
                if (collection.ContainsKey("status"))
                {
                    status = !string.IsNullOrEmpty(collection["status"].ToString());
                }

                if (collection.ContainsKey("habilitado"))
                {
                    habilitado = !string.IsNullOrEmpty(collection["habilitado"].ToString());
                }

                string cpfCnpj;
                if (collection["tipoPessoa"] == "pf")
                {
                    cpfCnpj = collection["cpf"].ToString();
                }
                else
                {
                    cpfCnpj = collection["cnpj"].ToString();
                }

                var command = new ParceiroModel.CreateUpdateParceiroCommand
                {
                    Id = id,
                    Nome = collection["nome"].ToString(),
                    RazaoSocial = collection["razaoSocial"].ToString(),
                    TipoPessoa = collection["tipoPessoa"].ToString(),
                    CpfCnpj = cpfCnpj,
                    Telefone = collection["numTelefone"] == "" ? null : collection["numTelefone"].ToString(),
                    Celular = collection["numCelular"] == "" ? null : collection["numCelular"].ToString(),
                    Cep = collection["cep"] == "" ? null : collection["cep"].ToString(),
                    Endereco = collection["endereco"] == "" ? null : collection["endereco"].ToString(),
                    Numero = collection["numero"] == "" ? null : Convert.ToInt32(collection["numero"].ToString()),
                    Bairro = collection["bairro"] == "" ? null : collection["bairro"].ToString(),
                    MunicipioId = Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                    Habilitado = habilitado,
                    Status = status,
                    Email = collection["email"].ToString()
                };

                await ApiClientFactory.Instance.UpdateParceiro(command.Id, command);

                return RedirectToAction(nameof(Parceiro), new { crud = (int)EnumCrud.Updated });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Parceiro), new { notify = (int)EnumNotify.Error, message = $"Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema. {e.Message}" });

            }
        }

        [ClaimsAuthorize(ClaimType.SistemaSocioeconomico, Claim.Excluir)]
        public ActionResult DeleteParceiro(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteParceiro(id);
                return RedirectToAction(nameof(Parceiro), new { crud = (int)EnumCrud.Deleted });
            }
            catch
            {
                return RedirectToAction(nameof(Parceiro));
            }
        }

        [HttpPost]
        [ClaimsAuthorize(ClaimType.SistemaSocioeconomico, Claim.Habilitar)]
        public async Task<ActionResult> Habilitar(IFormCollection collection)
        {
            try
            {
                var parceiroId = collection["habilitarParceiroId"].ToString();

                var result = await ApiClientFactory.Instance.GetParceiroById(Convert.ToInt32(parceiroId));

                if (result.Habilitado)
                {
                    return RedirectToAction(nameof(Parceiro),
                        new
                        {
                            notify = (int)EnumNotify.Error,
                            message = "Já existe parceiro/usuário habilitado com o E-mail cadastrado na base de dados!"
                        });
                }

                var result2 = ApiClientFactory.Instance.GetUsuarioByEmail(collection["email"].ToString().Trim());

                if (result2 != null)
                {
                    return RedirectToAction(nameof(Parceiro),
                        new
                        {
                            notify = (int)EnumNotify.Error,
                            message = "Já existe parceiro/usuário habilitado com o E-mail cadastrado na base de dados!"
                        });
                }

                var command = new UsuarioModel.CreateUpdateUsuarioCommand
                {
                    Email = collection["email"].ToString(),
                    Nome = collection["nome"].ToString(),
                    CpfCnpj = collection["cpfCnpj"].ToString()
                };

                var newUser = new IdentityUser { UserName = command.Email, Email = command.Email };
                var aspNetUser = await _userManager.CreateAsync(newUser, "12345678");

                if (aspNetUser.Succeeded)
                {
                    var perfil = ApiClientFactory.Instance.GetPerfilById((int)EnumPerfil.Parceiro);

                    var includedUserId = _userManager.Users.FirstOrDefault(x => x.Email == newUser.Email).Id;

                    command.AspNetUserId = includedUserId;
                    command.AspNetRoleId = perfil.AspNetRoleId;
                    command.PerfilId = perfil.Id;
                    command.Status = true;
                    command.MunicipioId = (int)result.MunicipioId;
                    command.TipoPessoa = result.TipoPessoa;
                    command.CpfCnpj = result.CpfCnpj;

                    var usu = await ApiClientFactory.Instance.CreateUsuario(command);

                    if (usu != 0)
                    {
                        var res = await ApiClientFactory.Instance.UpdateParceiro(result.Id,
                            new ParceiroModel.CreateUpdateParceiroCommand()
                            { AspNetUserId = command.AspNetUserId, Habilitado = true, Id = result.Id, Nome = result.Nome, CpfCnpj = result.CpfCnpj, Email = result.Email });
                    }

                    SendNewUserEmail(newUser, command.Email, command.Nome);

                    return RedirectToAction(nameof(Parceiro), new { crud = (int)EnumCrud.Created });
                }
                else
                {
                    return RedirectToAction(nameof(Parceiro),
                        new
                        {
                            notify = (int)EnumNotify.Error,
                            message = "Erro ao habilitar usuário. Favor entrar em contato com o administrador do sistema."
                        });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Parceiro),
                    new
                    {
                        notify = (int)EnumNotify.Error,
                        message = "Erro ao habilitar usuário. Favor entrar em contato com o administrador do sistema."
                    });
            }
        }
        #endregion

        #region Get Methods

        [ClaimsAuthorize(ClaimType.SistemaSocioeconomico, Claim.Consultar)]
        public async Task<ParceiroDto> GetParceiroById(int id)
        {
            var result = await ApiClientFactory.Instance.GetParceiroById(id);

            return result;
        }

        [ClaimsAuthorize(ClaimType.SistemaSocioeconomico, Claim.Consultar)]
        public async Task<JsonResult> GetTiposParceriasByParceria(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Parceria não informado.");
                var resultLocal = ApiClientFactory.Instance.GetTipoParceriaAll()
                    .Where(x => x.Parceria == Convert.ToInt32(id) && x.Status == true);

                return Json(new SelectList(resultLocal, "Id", "Nome"));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        #endregion

        #region Private Methods

        private async Task SendNewUserEmail(IdentityUser user, string email, string nome)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.ActionLink("ResetPassword",
                "Identity/Account", new { code, email });

            var message =
                System.IO.File.ReadAllText(Path.Combine(_host.WebRootPath, "emailtemplates/ConfirmEmail.html"));
            message = message.Replace("%NAME%", nome);
            message = message.Replace("%CALLBACK%", HtmlEncoder.Default.Encode(callbackUrl.Replace("%2FAccount", "/Account")));

            await _emailSender.SendEmailAsync(user.Email, "Primeiro acesso sistema Dna do Brasil",
                message);
        }

        #endregion
















        public IActionResult Estudantes()
        {
            var response = ApiClientFactory.Instance.GetLaudosAll();

            return View();
        }
        public async Task<IActionResult> SolicitacaoContato()
        {
            try
            {


                var alunos = await ApiClientFactory.Instance.GetAlunosByFilter(new AlunosFilterDto()
                { MunicipioId = "5570" });

                var model = new ParceiroModel() { Alunos = alunos!.Alunos! };

                return View(model);
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Parceiro), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }
        public IActionResult Details()
        {
            return View();
        }
    }
}
