using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;
using WebApp.Authorization;
using WebApp.Identity;
using Microsoft.AspNetCore.Authorization;
using Claim = WebApp.Identity.Claim;
using log4net;
using log4net.Config;
using System.Reflection;

namespace WebApp.Controllers
{
    [Authorize(Policy = ModuloAccess.Profissional)]
    public class ProfissionalController : BaseController
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _host;
        private readonly ILog _logger;

        public ProfissionalController(IOptions<UrlSettings> appSettings,
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
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
        }

        [ClaimsAuthorize(ClaimType.Profissional, Claim.Consultar)]
        public IActionResult Index(int? crud, int? notify, string message = null)
        {

            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud); var response = ApiClientFactory.Instance.GetProfissionalAll();
                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");

                return View(new ProfissionalModel()
                {
                    Profissionais = response,
                    ListEstados = estados,
                });

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }


        [ClaimsAuthorize(ClaimType.Profissional, Claim.Incluir)]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
                var modalidades = new SelectList(ApiClientFactory.Instance.GetModalidadeAll(), "Id", "Nome");
                var perfis = new[] { (int)EnumPerfil.Profissional, (int)EnumPerfil.GestorPedagogico, (int)EnumPerfil.GestorProjeto };
                //var resultPerfil = ApiClientFactory.Instance.GetPerfilAll().Where(x=>perfis.Contains(x.Id));
                List<SelectListDto> list = new List<SelectListDto>
                {
                    new() { IdNome = "Professor Ed. Física", Nome = "Professor Ed. Física" },
                    new() { IdNome = "Instrutor Ativ. Diversas", Nome = "Instrutor Ativ. Diversas" },
                    new() { IdNome = "Instrutor Artes", Nome = "Instrutor Artes" },
                    new() { IdNome = "Professor Reforço", Nome = "Professor Reforço" },
                    new() { IdNome = "Professor Disc. Diversas", Nome = "Professor Disc. Diversas" },
                    new() { IdNome = "Monitor Regular", Nome = "Monitor Regular" },
                    new() { IdNome = "Monitor A. Especializado", Nome = "Monitor A. Especializado" },
                    new() { IdNome = "Professor Informática", Nome = "Professor Informática" },
                    new() { IdNome = "Psicólogo", Nome = "Psicólogo" },
                    new() { IdNome = "Assistente Social", Nome = "Assistente Social" },
                    new() { IdNome = "Estagiário", Nome = "Estagiário" },
                    new() { IdNome = "Profissional Impressão", Nome = "Profissional Impressão" }
                };

                var cargos = new SelectList(list, "IdNome", "Nome");


                return View(new ProfissionalModel()
                {
                    ListEstados = estados,
                    ListModalidades = modalidades,
                    ListCargos = cargos,
                });

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        [HttpPost]
        [ClaimsAuthorize(ClaimType.Profissional, Claim.Incluir)]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var status = collection["status"].ToString();
                var habilitado = collection["habilitado"].ToString();

                var command = new ProfissionalModel.CreateUpdateProfissionalCommand
                {
                    Nome = collection["nome"] == "" ? null : collection["nome"].ToString(),
                    DtNascimento = collection["DtNascimento"] == "" ? null : collection["DtNascimento"].ToString(),
                    Email = collection["email"] == "" ? null : collection["email"].ToString(),
                    Sexo = collection["ddlSexo"] == "" ? null : collection["ddlSexo"].ToString(),
                    Telefone = collection["numTelefone"] == "" ? null : collection["numTelefone"].ToString(),
                    Cep = collection["cep"] == "" ? null : collection["cep"].ToString(),
                    Celular = collection["numCelular"] == "" ? null : collection["numCelular"].ToString(),
                    Cpf = collection["cpf"] == "" ? null : collection["cpf"].ToString(),
                    PerfilId = Convert.ToInt32(EnumPerfil.Profissional),
                    Numero = collection["numero"] == "" ? null : Convert.ToInt32(collection["numero"].ToString()),
                    Bairro = collection["bairro"] == "" ? null : collection["bairro"].ToString(),
                    Endereco = collection["endereco"] == "" ? null : collection["endereco"].ToString(),
                    MunicipioId = collection["ddlMunicipio"] == "" ? null : Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                    LocalidadeId = collection["ddlLocalidade"] == "" ? null : Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                    Habilitado = habilitado != "",
                    Status = status != "",
                    ModalidadesIds = collection["ddlModalidades"].ToString(),
                    Cargo = collection["ddlCargo"].ToString()

                };

                var newUser = new IdentityUser { UserName = command.Email, Email = command.Email };
                var aspNetUser = await _userManager.CreateAsync(newUser, "12345678");

                StringBuilder msg = new StringBuilder();
                if (!aspNetUser.Succeeded)
                {
                    foreach (var error in aspNetUser.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        msg.AppendLine(error.Description);
                    }

                    // Se chegamos até aqui, algo falhou, exiba novamente o formulário
                    //return Page();
                    return RedirectToAction("Index", new { notify = (int)EnumNotify.Error, message = msg });
                }



                var commandUsuario = new UsuarioModel.CreateUpdateUsuarioCommand
                {
                    Email = collection["email"].ToString(),
                    Nome = collection["nome"].ToString(),
                    CpfCnpj = collection["cpf"].ToString(),
                    TipoPessoa = "pf",
                    MunicipioId = collection["ddlMunicipio"] == "" ? 0 : Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                    LocalidadeId = Convert.ToInt32(collection["ddlLocalidade"].ToString())
                };

                var perfil = ApiClientFactory.Instance.GetPerfilById(Convert.ToInt32(EnumPerfil.Profissional));

                var includedUserId = _userManager.Users.FirstOrDefault(x => x.Email == newUser.Email).Id;

                commandUsuario.AspNetUserId = includedUserId;
                commandUsuario.AspNetRoleId = perfil.AspNetRoleId;
                commandUsuario.PerfilId = perfil.Id;

                var usuarioId = await ApiClientFactory.Instance.CreateUsuario(commandUsuario);

                if (usuarioId != 0)
                {
                    var userRole = _roleManager.Roles.FirstOrDefault(x => x.Id == perfil.AspNetRoleId).Name;
                    await _userManager.AddToRoleAsync(newUser, userRole);
                    await ApiClientFactory.Instance.CreateProfissional(command);
                }

                await SendNewUserEmail(newUser, commandUsuario.Email, commandUsuario.Nome);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }


        [ClaimsAuthorize(ClaimType.Profissional, Claim.Alterar)]
        public ActionResult Edit(int id, int? crud, int? notify, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                var profissional = ApiClientFactory.Instance.GetProfissionalById(id);
                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", profissional.Uf);
                var municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(profissional.Uf!), "Id", "Nome", profissional.MunicipioId);
                var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeByMunicipio(profissional.MunicipioId.ToString()), "Id", "Nome", profissional.LocalidadeId);
                var listModalidades = new SelectList(ApiClientFactory.Instance.GetModalidadeAll(), "Id", "Nome", profissional.ModalidadesIds);

                List<SelectListDto> list = new List<SelectListDto>
                {
                    new() { IdNome = "Professor Ed. Física", Nome = "Professor Ed. Física" },
                    new() { IdNome = "Instrutor Ativ. Diversas", Nome = "Instrutor Ativ. Diversas" },
                    new() { IdNome = "Instrutor Artes", Nome = "Instrutor Artes" },
                    new() { IdNome = "Professor Reforço", Nome = "Professor Reforço" },
                    new() { IdNome = "Professor Disc. Diversas", Nome = "Professor Disc. Diversas" },
                    new() { IdNome = "Monitor Regular", Nome = "Monitor Regular" },
                    new() { IdNome = "Monitor A. Especializado", Nome = "Monitor A. Especializado" },
                    new() { IdNome = "Professor Informática", Nome = "Professor Informática" },
                    new() { IdNome = "Psicólogo", Nome = "Psicólogo" },
                    new() { IdNome = "Assistente Social", Nome = "Assistente Social" },
                    new() { IdNome = "Estagiário", Nome = "Estagiário" },
                    new() { IdNome = "Profissional Impressão", Nome = "Profissional Impressão" }
                };

                var cargos = new SelectList(list, "IdNome", "Nome", profissional.Cargo);

                return View(new ProfissionalModel()
                {
                    ListEstados = estados,
                    ListModalidades = listModalidades,
                    Profissional = profissional,
                    ListMunicipios = municipios,
                    ListLocalidades = localidades,
                    ListCargos = cargos
                });

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Edit), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        [HttpPost]
        [ClaimsAuthorize(ClaimType.Profissional, Claim.Alterar)]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                var status = collection["status"].ToString();
                var habilitado = collection["habilitado"].ToString();

                var command = new ProfissionalModel.CreateUpdateProfissionalCommand
                {
                    Id = id,
                    Nome = collection["nome"] == "" ? null : collection["nome"].ToString(),
                    DtNascimento = collection["DtNascimento"] == "" ? null : collection["DtNascimento"].ToString(),
                    Email = collection["email"] == "" ? null : collection["email"].ToString(),
                    Sexo = collection["ddlSexo"] == "" ? null : collection["ddlSexo"].ToString(),
                    Telefone = collection["numTelefone"] == "" ? null : collection["numTelefone"].ToString(),
                    Cep = collection["cep"] == "" ? null : collection["cep"].ToString(),
                    Celular = collection["numCelular"] == "" ? null : collection["numCelular"].ToString(),
                    Cpf = collection["cpf"] == "" ? null : collection["cpf"].ToString(),
                    //AspNetUserId = collection["aspnetuserId"].ToString(),
                    Numero = collection["numero"] == "" ? null : Convert.ToInt32(collection["numero"].ToString()),
                    Bairro = collection["bairro"] == "" ? null : collection["bairro"].ToString(),
                    Endereco = collection["endereco"] == "" ? null : collection["endereco"].ToString(),
                    MunicipioId = collection["ddlMunicipio"] == "" ? null : Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                    LocalidadeId = collection["ddlLocalidade"] == "" ? null : Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                    Habilitado = habilitado != "",
                    Status = status != "",
                    ModalidadesIds = collection["ddlModalidades"].ToString(),
                    Cargo = collection["ddlCargo"].ToString()
                };

                await ApiClientFactory.Instance.UpdateProfissional(command.Id, command);

                // var profissional = ApiClientFactory.Instance.GetProfissionalById(id);

                //           if (profissional.Email.Trim()!=command.Email.Trim())
                //           {
                //               //atualiza email na aspnetuser e o username

                ////atualiza o email na tabela usuários
                //               var usuario = ApiClientFactory.Instance.GetUsuarioByEmail(profissional.Email);

                //usuario.Email = command.Email
                //           }

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        [HttpPost]
        [ClaimsAuthorize(ClaimType.Profissional, Claim.Incluir)]
        public async Task<ActionResult> CreateModalidade(IFormCollection collection)
        {
            try
            {
                var modalidade = collection["modalidade"].ToString();

                var command = new ModalidadeModel.CreateUpdateModalidadeCommand
                {
                    Nome = modalidade
                };

                await ApiClientFactory.Instance.CreateModalidade(command);

                return RedirectToAction(nameof(Create), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        [ClaimsAuthorize(ClaimType.Profissional, Claim.Excluir)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                //select* delete from Profissionais where email like '%thaislmoliveira2@gmail.com%'
                ApiClientFactory.Instance.DeleteProfissional(id);

                var profissional = ApiClientFactory.Instance.GetProfissionalById(id);

                //select* delete from[dbo].[AspNetUsers] where email like '%thaislmoliveira2@gmail.com'
                var user = _userManager.Users.FirstOrDefault(x => x.Email == profissional.Email);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);

                    if (result.Succeeded)
                    {
                        //select* delete from[dbo].[AspNetUserRoles] where userid = '1c9e9c35-7ef3-478d-aba1-b8ec72a8400c'
                        var name = _roleManager.Roles.FirstOrDefault(x => x.Id == profissional.AspNetRoleId)?.Name;
                        if (name != null)
                            await _userManager.RemoveFromRoleAsync(user, name);

                        //select* delete from Usuarios where email like '%amaralsakarina@gmail.com%'
                        var usuario = ApiClientFactory.Instance.GetUsuarioByEmail(profissional.Email);
                        ApiClientFactory.Instance.DeleteUsuario(usuario.Id);

                        return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
                    }
                }
            }
            catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Index),
                    new
                    {
                        notify = (int)EnumNotify.Warning,
                        message = e.Message
                    });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index),
                    new
                    {
                        notify = (int)EnumNotify.Error,
                        message =
                            $"Erro ao excluir Profissional. Favor entrar em contato com o administrador do sistema. {e.Message}"
                    });
            }

            return null;
        }

        [ClaimsAuthorize(ClaimType.Profissional, Claim.Consultar)]
        public Task<ProfissionalDto> GetProfissionalById(int id)
        {
            var result = ApiClientFactory.Instance.GetProfissionalById(id);

            return Task.FromResult(result);
        }

        [ClaimsAuthorize(ClaimType.Profissional, Claim.Consultar)]
        public Task<bool> GetProfissionalByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new Exception("Email não informado.");
            var result = ApiClientFactory.Instance.GetProfissionalByEmail(email);

            if (result == null)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        [ClaimsAuthorize(ClaimType.Profissional, Claim.Consultar)]
        public Task<bool> GetProfissionalByCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf)) throw new Exception("Cpf não informado.");
            var result = ApiClientFactory.Instance.GetProfissionalByCpf(Regex.Replace(cpf, "[^0-9a-zA-Z]+", ""));

            if (result == null)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        [HttpPost]
        [ClaimsAuthorize(ClaimType.Profissional, Claim.Habilitar)]
        public async Task<ActionResult> Habilitar(IFormCollection collection)
        {
            try
            {
                var profissionalId = collection["habilitarProfissionalId"].ToString();

                var result = ApiClientFactory.Instance.GetProfissionalById(Convert.ToInt32(profissionalId));

                if (result.Email != null && result.Email.Equals(collection["email"].ToString().Trim()))
                {
                    return RedirectToAction(nameof(Index),
                        new
                        {
                            notify = (int)EnumNotify.Error,
                            message = "Já existe um profissional cadastrado com esse email."
                        });
                }

                var result2 = await ApiClientFactory.Instance.GetUsuarioByEmail(collection["email"].ToString().Trim());

                if (result2 != null)
                {
                    return RedirectToAction(nameof(Create),
                        new
                        {
                            notify = (int)EnumNotify.Error,
                            message = "Já existe profissional com o E-mail cadastrado na base de dados!"
                        });
                }

                var command = new UsuarioModel.CreateUpdateUsuarioCommand
                {
                    Email = collection["email"].ToString(),
                    Nome = collection["nome"].ToString()
                };

                var newUser = new IdentityUser { UserName = command.Email, Email = command.Email };
                await _userManager.CreateAsync(newUser, "12345678");

                command.PerfilId = result2.Perfil.Id;
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

        [ClaimsAuthorize(ClaimType.Profissional, Claim.Consultar)]
        public Task<JsonResult> GetProfissionaisByLocalidade(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Localidadee não informada.");
                var resultLocal = ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(id));

                return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Nome")));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }
        private async Task SendNewUserEmail(IdentityUser user, string email, string nome)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.ActionLink("ResetPassword",
                "Identity/Account", new { code, email });

            var message =
                System.IO.File.ReadAllText(Path.Combine(_host.WebRootPath, "emailtemplates/ConfirmEmail.html"));
            message = message.Replace("%NAME%", nome);
            message = message.Replace("%CALLBACK%", HtmlEncoder.Default.Encode(callbackUrl.Replace("%2FAccount", "/Account")));

            await _emailSender.SendEmailAsync(user.Email, "Primeiro acesso sistema Dna Brasil",
                message);
        }

        public Task<JsonResult> GetProfissionaisByLocalidadeId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Localidade não informada.");
                var resultLocal = ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(id));

                return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Titulo")));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex.Message));
            }
        }

        /// <summary>
        /// Tela de Visualização do Profile do Profissional Logado
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        [ClaimsAuthorize(ClaimType.Profissional, Claim.Consultar)]
        public async Task<ActionResult> Profile(int? crud, int? notify, string message = null)
        {
            try
            {
                _logger.Info($"Usuario Logado em Profissional.Profile User.Identity.Name : {User.Identity.Name}");

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                //Busca usuario por AspNetUserId
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.Info($"Busca Usuario por AspNetUserId: {userId}");

                var usuario = User.Identity.Name;
                _logger.Info($"Busca Usuario por email: {usuario}");

                if (userId == null)
                {
                    _logger.Warn($"AspNetUserId não encontrado para o email: {User.Identity.Name}");
                    throw new Exception($"AspNetUserId não encontrado para o email: {User.Identity.Name}");
                }

                if (usuario == null)
                {
                    _logger.Warn($"User.Identity.Name não encontrado para o email: {User.Identity.Name}");
                    throw new Exception($"User.Identity.Name não encontrado para o email: {User.Identity.Name}");
                }

                var usu = await ApiClientFactory.Instance.GetUsuarioByAspNetUserId(userId);

                var profissional = await ApiClientFactory.Instance.GetProfissionalByEmail(usuario);

                _logger.Info($"ProfissionalId: {profissional.Id}");

                var listModalidades = new SelectList(
                    ApiClientFactory.Instance.GetModalidadesByProfissionalId(profissional.Id), "Id", "Nome",
                    profissional.ModalidadesIds);

                var listAlunos =
                    new SelectList(ApiClientFactory.Instance.GetNomeAlunosByProfissionalId(profissional.Id), "Id",
                        "Nome");

                return View(new ProfissionalModel()
                {
                    ListAtividadesModalidades = listModalidades,
                    Profissional = profissional,
                    ListAlunos = listAlunos,
                    Usuario = usu,
                });
            }
            catch (Exception e)
            {
                _logger.Error(e.StackTrace);
                throw;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ClaimsAuthorize(ClaimType.Profissional, Claim.AlterarProfile)]
        public async Task<ActionResult> Profile(IFormCollection collection)
        {
            try
            {
                var status = collection["status"].ToString();
                var habilitado = collection["habilitado"].ToString();

                var command = new ProfissionalModel.CreateUpdateProfissionalCommand
                {
                    Id = Convert.ToInt32(collection["ProfissionalId"].ToString()),
                    Nome = collection["nome"] == "" ? null : collection["nome"].ToString(),
                    DtNascimento = collection["DtNascimento"] == "" ? null : collection["DtNascimento"].ToString(),
                    Email = collection["email"] == "" ? null : collection["email"].ToString(),
                    Sexo = collection["ddlSexo"] == "" ? null : collection["ddlSexo"].ToString(),
                    Telefone = collection["numTelefone"] == "" ? null : collection["numTelefone"].ToString(),
                    Cep = collection["cep"] == "" ? null : collection["cep"].ToString(),
                    Celular = collection["numCelular"] == "" ? null : collection["numCelular"].ToString(),
                    Cpf = collection["cpf"] == "" ? null : collection["cpf"].ToString(),
                    //AspNetUserId = collection["aspnetuserId"].ToString(),
                    Numero = collection["numero"] == "" ? null : Convert.ToInt32(collection["numero"].ToString()),
                    Bairro = collection["bairro"] == "" ? null : collection["bairro"].ToString(),
                    Endereco = collection["endereco"] == "" ? null : collection["endereco"].ToString(),
                    MunicipioId = collection["ddlMunicipio"] == "" ? null : Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                    LocalidadeId = collection["ddlLocalidade"] == "" ? null : Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                    Habilitado = habilitado != "",
                    Status = status != "",
                    ModalidadesIds = collection["ddlModalidades"].ToString(),
                    Cargo = collection["ddlCargo"].ToString()
                };

                await ApiClientFactory.Instance.UpdateProfissional(command.Id, command);

                var user = await _userManager.FindByEmailAsync(command.Email);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                await _userManager.ResetPasswordAsync(user, token, collection["perfilNovaSenha"].ToString());

                // var profissional = ApiClientFactory.Instance.GetProfissionalById(id);

                //           if (profissional.Email.Trim()!=command.Email.Trim())
                //           {
                //               //atualiza email na aspnetuser e o username

                ////atualiza o email na tabela usuários
                //               var usuario = ApiClientFactory.Instance.GetUsuarioByEmail(profissional.Email);

                //usuario.Email = command.Email
                //           }

                return RedirectToAction(nameof(Profile), new { crud = (int)EnumCrud.Updated });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Profile), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }


        [ClaimsAuthorize(ClaimType.Profissional, Claim.Consultar)]
        public async Task<ActionResult> MinhasTurmas(IFormCollection collection)
        {
            try
            {
                var command = new AtividadeModel.CreateUpdateAtividadeAlunosCommand
                {
                    AtividadeId = Convert.ToInt32(collection["ddlTurma"].ToString()),
                    AlunosIds = collection["arrAlunos"].ToString()
                };

                if (Convert.ToBoolean(collection["Update"].ToString()))
                {
                    await ApiClientFactory.Instance.UpdateAtividadeAluno(Convert.ToInt32(collection["ddlTurma"].ToString()), command);

                    return RedirectToAction(nameof(Profile), new { crud = (int)EnumCrud.Updated });
                }

                await ApiClientFactory.Instance.CreateAtividadeAluno(command);

                return RedirectToAction(nameof(Profile), new { crud = (int)EnumCrud.Created });

            }
            catch (Exception e)
            {
                return Redirect("/Identity/Account/Login");

            }
        }

        /// <summary>
        /// Busca lista de turmas pelo id da modalidade e id do profissional 
        /// </summary>
        /// <param name="modalidadeId">Id da modalidade</param>
        /// <param name="profissionalId">Id do profissional</param>
        /// <returns>Retorna um json com todas as turmas</returns>
        public Task<JsonResult> GetTurmasByModalidadeIdProfissionalId(string modalidadeId, string profissionalId)
        {
            try
            {
                if (string.IsNullOrEmpty(modalidadeId)) throw new Exception("Modalidade não informada.");

                var resultLocal = ApiClientFactory.Instance.GetTurmasByModalidadeIdProfissionalId(Convert.ToInt32(modalidadeId), Convert.ToInt32(profissionalId));

                return Task.FromResult(Json(new SelectList(resultLocal, "Id", "TurmaHora")));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }


    }

}
