using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;
using System.Text;
using System.Text.RegularExpressions;
using WebApp.Configuration;
using WebApp.Identity;
using WebApp.Authorization;
using log4net;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controler de Usuário
    /// </summary>
    //[Authorize(Policy = ModuloAccess.ControleAcesso)]
    public class UsuarioController : BaseController
    {
        #region Constructor

        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _host;
        private readonly ILog _logger;

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="app">configurações de urls do sistema</param>
        /// <param name="emailSender">imlementacao de infraestrutura de identidade possa enviar emails de confirmação e redefinição de senha.</param>
        /// <param name="userManager">gerenciador de identidade de usuários</param>
        /// <param name="host">informações da aplicação em execução</param>
        /// <param name="roleManager">gerenciador de regras de permissoes</param>
        /// <param name="logger">gerenciador de log</param>
        public UsuarioController(IOptions<UrlSettings> app,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment host,
            RoleManager<IdentityRole> roleManager,
            ILog logger)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _host = host;
            _roleManager = roleManager;
            _logger = logger;
            ApplicationSettings.WebApiUrl = app.Value.WebApiBaseUrl;
        }

        #endregion

        #region Crud Methods

        /// <summary>
        /// Listagem de usuário
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        [ClaimsAuthorize(ClaimType.Usuario, Identity.Claim.Consultar)]
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetUsuarioAll(); var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeAll(), "Id", "Nome");
            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");

            return View(new UsuarioModel()
            {
                Usuarios = response,
                ListEstados = estados
            });
        }

        /// <summary>
        /// Tela para inclusão de Usuario
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        [ClaimsAuthorize(ClaimType.Usuario, Identity.Claim.Incluir)]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var resultPerfil = ApiClientFactory.Instance.GetPerfilAll();
            var estados = ApiClientFactory.Instance.GetEstadosAll();

            var model = new UsuarioModel
            {
                ListEstados = new SelectList(estados, "Sigla", "Nome"),
                ListPerfis = new SelectList(resultPerfil, "Id", "Nome")
            };
            return View(model);
        }

        /// <summary>
        /// Ação de inclusão do Usuario
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao de Usuario</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        [ClaimsAuthorize(ClaimType.Usuario, Identity.Claim.Incluir)]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var cpf = collection["cpf"].ToString();

                var result = ApiClientFactory.Instance.GetUsuarioByCpf(cpf); //Regex.Replace(cpf, "[^0-9a-zA-Z]+", "")

                if (result != null)
                {
                    return RedirectToAction(nameof(Create),
                        new
                        {
                            notify = (int)EnumNotify.Error,
                            message = "Já existe um usuário cadastrado com esse cpf."
                        });
                }

                var result2 = ApiClientFactory.Instance.GetUsuarioByEmail(collection["email"].ToString().Trim());

                if (result2 != null)
                {
                    return RedirectToAction(nameof(Create),
                        new
                        {
                            notify = (int)EnumNotify.Error,
                            message = "Já existe usuários com o E-mail informado cadastrado na base de dados!"
                        });
                }

                var command = new UsuarioModel.CreateUpdateUsuarioCommand
                {
                    Email = collection["email"].ToString(),
                    Nome = collection["nome"].ToString(),
                    CpfCnpj = collection["cpf"].ToString(),
                    TipoPessoa = collection["tipoPessoa"].ToString(),
                    MunicipioId = Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                    LocalidadeId = Convert.ToInt32(collection["ddlLocalidade"].ToString())
                };


                command.PerfilId = int.Parse(collection["ddlPerfil"].ToString());
                var perfil = ApiClientFactory.Instance.GetPerfilById(command.PerfilId);

                var newUser = new IdentityUser { UserName = command.Email, Email = command.Email };
                var userCreated = await _userManager.CreateAsync(newUser, "12345678");

                if (userCreated.Succeeded)
                {
                    var userRole = _roleManager.Roles.FirstOrDefault(x => x.Id == perfil.AspNetRoleId).Name;

                    command.AspNetUserId = newUser.Id;
                    command.AspNetRoleId = perfil.AspNetRoleId;
                    command.PerfilId = perfil.Id;

                    var usuarioId = await ApiClientFactory.Instance.CreateUsuario(command);

                    if (usuarioId != 0)
                    {
                        await _userManager.AddToRoleAsync(newUser, userRole);
                    }

                    SendNewUserEmail(newUser, command.Email, command.Nome);

                    return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        notify = (int)EnumNotify.Error,
                        message = $"Erro ao criar usuário. Favor entrar em contato com o administrador do sistema."
                    });

            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index),
                    new
                    {
                        notify = (int)EnumNotify.Error,
                        message = "Erro ao criar usuário. Favor entrar em contato com o administrador do sistema."
                    });
            }
        }

        /// <summary>
        /// Tela de alteração de Usuario
        /// </summary>
        /// <param name="id">id do usuario</param>
        /// <exception cref="ArgumentNullException">Mensagem de erro ao alterar o tentar acessar tela de alteração do Usuario</exception>
        [ClaimsAuthorize(ClaimType.Usuario, Identity.Claim.Alterar)]
        public async Task<ActionResult> Edit(string id, int? crud, int? notify, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                UsuarioModel model = new UsuarioModel();

                var obj = await ApiClientFactory.Instance.GetUsuarioById(id) ?? throw new ArgumentNullException("Usuário não encontrado.");

                var resultPerfil = ApiClientFactory.Instance.GetPerfilAll();

                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", obj.Uf);
                var municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(obj.Uf!), "Id", "Nome", obj.MunicipioId);
                var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeByMunicipio(obj.MunicipioId.ToString()), "Id", "Nome", obj.LocalidadeId);

                model = new UsuarioModel
                {
                    ListPerfis = new SelectList(resultPerfil, "Id", "Nome", obj.Perfil.Id),
                    Usuario = obj,
                    ListMunicipios = municipios,
                    ListEstados = estados,
                    ListLocalidades = localidades
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index),
                    new
                    {
                        notify = (int)EnumNotify.Error,
                        message = $"Erro ao alterar usuário. Favor entrar em contato com o administrador do sistema. {ex.Message}"
                    });
            }
        }

        /// <summary>
        /// Ação de alteração do Usuario
        /// </summary>
        /// <param name="id">identificador do Usuario</param>
        /// <param name="collection">coleção de dados para alteração de Usuario</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Alterar)]
        [HttpPost]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var usuario = await ApiClientFactory.Instance.GetUsuarioById(id.ToString());

                var perfil = ApiClientFactory.Instance.GetPerfilById(Convert.ToInt32(collection["ddlPerfis"].ToString()));

                var role = await _roleManager.FindByIdAsync(perfil.AspNetRoleId);//find passed in role id
                var currentRole = await _roleManager.FindByIdAsync(usuario.AspNetRoleId);//find the role of current user
                var user = await _userManager.FindByIdAsync(usuario.AspNetUserId);

                await _userManager.RemoveFromRoleAsync(user, currentRole.Name);
                await _userManager.AddToRoleAsync(user, role.Name);


                var command = new UsuarioModel.CreateUpdateUsuarioCommand
                {
                    Id = id,
                    Email = collection["email"].ToString(),
                    Nome = collection["nome"].ToString(),
                    TipoPessoa = collection["tipoPessoa"].ToString(),
                    CpfCnpj = collection["cpf"].ToString() == "" ? collection["cnpj"].ToString() : collection["cpf"].ToString(),
                    PerfilId = perfil.Id,
                    AspNetUserId = usuario.AspNetUserId,
                    AspNetRoleId = perfil.AspNetRoleId,
                    MunicipioId = Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                    LocalidadeId = Convert.ToInt32(collection["ddlLocalidade"].ToString())
                };

                await ApiClientFactory.Instance.UpdateUsuario(id, command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index),
                    new
                    {
                        notify = (int)EnumNotify.Error,
                        message = $"Erro ao alterar usuário. Favor entrar em contato com o administrador do sistema. {ex.Message}"
                    });
            }
        }

        /// <summary>
        /// Ação de exclusão do Usuario
        /// </summary>
        /// <param name="id">identificador do Usuario</param>
        /// <param name="collection">coleção de dados para exclusão de Usuario</param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        [ClaimsAuthorize(ClaimType.Usuario, Identity.Claim.Excluir)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var usuario = await ApiClientFactory.Instance.GetUsuarioById(id.ToString()) ?? throw new ArgumentNullException("Usuário não encontrado.");

                var user = _userManager.Users.FirstOrDefault(x => x.Email == usuario.Email);

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    await _userManager.RemoveFromRoleAsync(user, _roleManager.Roles.FirstOrDefault(x => x.Id == usuario.AspNetRoleId).Name);

                    ApiClientFactory.Instance.DeleteUsuario(id);

                    return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
                }

                StringBuilder str = new StringBuilder();

                // Handle failure
                foreach (var error in result.Errors)
                {
                    str.AppendLine(error.Description);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        notify = (int)EnumNotify.Error,
                        message = $"Erro ao criar usuário. Favor entrar em contato com o administrador do sistema. {str}"
                    });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index),
                    new
                    {
                        notify = (int)EnumNotify.Error,
                        message = $"Erro ao excluir usuário. Favor entrar em contato com o administrador do sistema. {ex.Message}"
                    });
            }
        }

        /// <summary>
        /// Tela de Visualização do Profile do Usuário Logado
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        public async Task<ActionResult> Profile(int? crud, int? notify, string message = null)
        {
            try
            {
                _logger.Info($"Usuario Logado User.Identity.Name: {User.Identity.Name}");

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                //Busca usuario por email
                //var usuario = User.Identity.Name;
                //if (usuario == null) return Redirect("/Account/Logout");
                //_logger.Info($"Busca Usuario por email: {usuario}");
                //var usu1 = ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

                //Busca usuario por AspNetUserId
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.Info($"Busca Usuario por AspNetUserId: {userId}");
                if (userId != null)
                {
                    var usu = await ApiClientFactory.Instance.GetUsuarioByAspNetUserId(userId);

                    _logger.Info($"Retorno de GetUsuarioByAspNetUserId");
                    _logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(usu));

                    var model = new UsuarioModel
                    {
                        Usuario = usu
                    };
                    return View(model);
                }
                else
                {
                    _logger.Warn($"AspNetUserId não encontrado para o email: {User.Identity.Name}");
                    throw new Exception($"AspNetUserId não encontrado para o email: {User.Identity.Name}");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.StackTrace);
                throw;
            }

        }


        #endregion

        #region Get Methods

        /// <summary>
        /// Método para envio de email
        /// </summary>
        /// <param name="user">identidade do usuário</param>
        /// <param name="email">email a ser enviado</param>
        /// <param name="nome">nome da pessoa que receberá o email</param>
        [ClaimsAuthorize(ClaimType.Usuario, Identity.Claim.Excluir)]
        private async Task SendNewUserEmail(IdentityUser user, string email, string nome)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.ActionLink("ResetPassword", "Identity/Account", new { code, email });

            var message =
                System.IO.File.ReadAllText(Path.Combine(_host.WebRootPath, "emailtemplates/ConfirmEmail.html"));
            message = message.Replace("%NAME%", nome);
            message = message.Replace("%CALLBACK%", HtmlEncoder.Default.Encode(callbackUrl.Replace("%2FAccount", "/Account")));

            await _emailSender.SendEmailAsync(user.Email, "Primeiro acesso sistema Dna do Brasil",
                message);
        }

        /// <summary>
        /// Método de busca de Usuario por email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>retorna objeto Usuario</returns>
        [ClaimsAuthorize(ClaimType.Usuario, Identity.Claim.Consultar)]
        public Task<JsonResult> GetUsuarioByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) throw new Exception("Email não informado.");
                var result = ApiClientFactory.Instance.GetUsuarioByEmail(email);

                if (result == null)
                {
                    return Task.FromResult(Json(true));
                }

                return Task.FromResult(Json(false));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex.Message));
            }

        }

        /// <summary>
        /// Método de busca de Usuario por cpf
        /// </summary>
        /// <param name="cpf">cpf do Usuario</param>
        /// <returns>retorna objeto Usuario</returns>
        [ClaimsAuthorize(ClaimType.Usuario, Identity.Claim.Consultar)]
        public Task<JsonResult> GetUsuarioByCpf(string cpf)
        {
            try
            {
                if (string.IsNullOrEmpty(cpf)) throw new Exception("Cpf não informado.");
                var result = ApiClientFactory.Instance.GetUsuarioByCpf(Regex.Replace(cpf, "[^0-9a-zA-Z]+", ""));

                if (result == null)
                {
                    return Task.FromResult(Json(true));
                }

                return Task.FromResult(Json(false));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex.Message));
            }
        }

        #endregion
    }
}
