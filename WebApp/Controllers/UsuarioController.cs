using Infraero.Relprev.CrossCutting.Enumerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using WebApp.Areas.Identity.Models;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Security.Claims;
using System.Text.RegularExpressions;
using WebApp.Configuration;

namespace WebApp.Controllers
{
    public class UsuarioController : BaseController
    {
        private readonly IOptions<UrlSettings> _appSettings;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHostingEnvironment _host;

        public UsuarioController(IOptions<UrlSettings> app, IEmailSender emailSender,
            UserManager<IdentityUser> userManager, IHostingEnvironment host, RoleManager<IdentityRole> roleManager)
        {
            _appSettings = app;
            _emailSender = emailSender;
            _userManager = userManager;
            _host = host;
            _roleManager = roleManager;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }
        //[ClaimsAuthorize("Usuario", "Consultar")]
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetUsuarioAll();

            return View(new UsuarioModel() { Usuarios = response });
        }

        //[ClaimsAuthorize("Usuario", "Incluir")]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var resultPerfil = ApiClientFactory.Instance.GetPerfilAll();

            var model = new UsuarioModel
            {
                ListPerfis = new SelectList(resultPerfil, "Id", "Nome")
            };
            return View(model);
        }

        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var cpf = collection["cpf"].ToString();

                var result = ApiClientFactory.Instance.GetUsuarioByCpf(Regex.Replace(cpf, "[^0-9a-zA-Z]+", ""));

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
                    Cpf = collection["cpf"].ToString()
                };

                var newUser = new IdentityUser { UserName = command.Email, Email = command.Email };
                await _userManager.CreateAsync(newUser, "12345678");

                command.PerfilId = int.Parse(collection["ddlPerfil"].ToString());
                var perfil = ApiClientFactory.Instance.GetPerfilById(command.PerfilId);

                var includedUserId = _userManager.Users.FirstOrDefault(x => x.Email == newUser.Email).Id;

                command.AspNetUserId = includedUserId;
                command.AspNetRoleId = perfil.AspNetRoleId;

                ApiClientFactory.Instance.CreateUsuario(command);

                SendNewUserEmail(newUser, command.Email, command.Nome);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
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

        private async Task SendNewUserEmail(IdentityUser user, string email, string nome)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(new IdentityUser(user.Email));

            var callbackUrl = Url.ActionLink("ResetPassword",
                "Identity/Account", new { code, email });

            var message =
                System.IO.File.ReadAllText(Path.Combine(_host.WebRootPath, "emailtemplates/ConfirmEmail.html"));
            message = message.Replace("%NAME%", nome);
            message = message.Replace("%CALLBACK%", HtmlEncoder.Default.Encode(callbackUrl.Replace("%2FAccount", "/Account")));

            await _emailSender.SendEmailAsync(user.Email, "Primeiro acesso sistema Dna Brasil",
                message);
        }

        //[ClaimsAuthorize("Usuario", "Alterar")]
        public ActionResult Edit(string id)
        {
            UsuarioModel model = new UsuarioModel();

            var obj = ApiClientFactory.Instance.GetUsuarioById(id);

            if (obj != null)
            {
                var resultPerfil = ApiClientFactory.Instance.GetPerfilAll();

                model = new UsuarioModel
                {
                    ListPerfis = new SelectList(resultPerfil, "PerfilId", "Nome", obj.PerfilId),
                    Usuario = obj
                };

                return View(model);
            }

            return View(model);
        }

        //[ClaimsAuthorize("Usuario", "Alterar")]
        [HttpPost]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var perfilId = int.Parse(collection["ddlPerfil"].ToString());
                var aspNetRoleId = _roleManager.FindByNameAsync(collection["ddlPerfil"].ToString()).Result?.Id;

                var command = new UsuarioModel.CreateUpdateUsuarioCommand
                {

                    Email = collection["EndEmail"].ToString(),
                    Nome = collection["NomUsuario"].ToString(),
                    Cpf = collection["cpf"].ToString(),
                    PerfilId = perfilId,
                    AspNetUserId = userId,
                    AspNetRoleId = aspNetRoleId
                };

                var result = await ApiClientFactory.Instance.UpdateUsuario(id, command);

                if (false)
                {
                    return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
                }
                else
                {
                    return RedirectToAction(nameof(Index),
                        new
                        {
                            notify = (int)EnumNotify.Error,
                            message = "Erro ao criar usuário. Favor entrar em contato com o administrador do sistema."
                        });
                }

            }
            catch
            {
                return View();
            }
        }

        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteUsuario(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        //[ClaimsAuthorize("Usuario", "Consultar")]
        public async Task<bool> GetUsuarioByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new Exception("Email não informado.");
            var result = ApiClientFactory.Instance.GetUsuarioByEmail(email);

            if (result == null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> GetUsuarioByCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf)) throw new Exception("Cpf não informado.");
            var result = ApiClientFactory.Instance.GetUsuarioByCpf(Regex.Replace(cpf, "[^0-9a-zA-Z]+", ""));

            if (result == null)
            {
                return true;
            }

            return false;
        }
    }
}
