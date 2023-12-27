﻿using System.Text.Encodings.Web;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
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

namespace WebApp.Controllers
{
	public class ProfissionalController : BaseController
	{
		private readonly IOptions<UrlSettings> _appSettings;
		private readonly IEmailSender _emailSender;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IHostingEnvironment _host;

		public ProfissionalController(IOptions<UrlSettings> appSettings,
			IEmailSender emailSender,
			UserManager<IdentityUser> userManager, IHostingEnvironment host, RoleManager<IdentityRole> roleManager)
		{
			_appSettings = appSettings;
			_emailSender = emailSender;
			_userManager = userManager;
			_host = host;
			_roleManager = roleManager;
			ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
		}

		public IActionResult Index(int? crud, int? notify, string message = null)
		{

			try
			{
				SetNotifyMessage(notify, message);
				SetCrudMessage(crud); var response = ApiClientFactory.Instance.GetProfissionalAll();
				var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");

				return View(new ProfissionalModel() { Profissionais = response, ListEstados = estados });

			}
			catch (Exception e)
			{
				Console.Write(e.StackTrace);
				return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

			}
		}

		//[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
		public ActionResult Create(int? crud, int? notify, string message = null)
		{
			try
			{
				SetNotifyMessage(notify, message);
				SetCrudMessage(crud);

				var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
				var ambientes = new SelectList(ApiClientFactory.Instance.GetAmbienteAll(), "Id", "Nome");

				return View(new ProfissionalModel() { ListEstados = estados, ListAmbientes = ambientes });

			}
			catch (Exception e)
			{
				Console.Write(e.StackTrace);
				return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Error, message = e.Message });

			}
		}

		//[ClaimsAuthorize("Profissional", "Incluir")]
		[HttpPost]
		public async Task<ActionResult> Create(IFormCollection collection)
		{
			try
			{
				var status = collection["status"].ToString();
				var habilitado = collection["habilitado"].ToString();
				var ambientesIds = collection["arrAmbientes"];

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
					//AspNetUserId = collection["aspnetuserId"].ToString(),
					Numero = collection["numero"] == "" ? null : Convert.ToInt32(collection["numero"].ToString()),
					Bairro = collection["bairro"] == "" ? null : collection["bairro"].ToString(),
					Endereco = collection["endereco"] == "" ? null : collection["endereco"].ToString(),
					MunicipioId = collection["ddlMunicipio"] == "" ? null : Convert.ToInt32(collection["ddlMunicipio"].ToString()),
					Habilitado = habilitado != "",
					Status = status != "",
					AmbientesIds = collection["arrAmbientes"] == "" ? null : collection["arrAmbientes"].ToString()
				};

				await ApiClientFactory.Instance.CreateProfissional(command);

				return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
			}
			catch (Exception e)
			{
				Console.Write(e.StackTrace);
				return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

			}
		}

		//[ClaimsAuthorize("Profissional", "Alterar")]
		public async Task<ActionResult> Edit(IFormCollection collection)
		{
			try
			{
				var status = collection["status"].ToString();
				var habilitado = collection["habilitado"].ToString();
				var command = new ProfissionalModel.CreateUpdateProfissionalCommand
				{
					Nome = collection["nome"].ToString(),
					DtNascimento = collection["DtNascimento"].ToString(),
					Email = collection["email"].ToString(),
					Sexo = collection["ddlSexo"].ToString(),
					Telefone = collection["numTelefone"].ToString(),
					Cep = collection["cep"].ToString(),
					Celular = collection["numCelular"].ToString(),
					Cpf = collection["cpf"].ToString(),
					//AspNetUserId = collection["aspnetuserId"].ToString(),
					//Numero = Convert.ToInt32(collection["numero"].ToString()),
					Bairro = collection["bairro"].ToString(),
					Endereco = collection["endereco"].ToString(),
					//MunicipioId = Convert.ToInt32(collection["ddlMunicipio"].ToString()),
					Habilitado = habilitado == "",
					Status = status != ""
				};

				await ApiClientFactory.Instance.UpdateProfissional(command.Id, command);

				return RedirectToAction(nameof(Edit), new { crud = (int)EnumCrud.Updated });
			}
			catch (Exception e)
			{
				Console.Write(e.StackTrace);
				return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

			}
		}

		//[ClaimsAuthorize("Profissional", "Alterar")]
		[HttpPost]
		public async Task<ActionResult> CreateAmbiente(IFormCollection collection)
		{
			try
			{
				var ambiente = collection["ambiente"].ToString();

				var command = new AmbienteModel.CreateUpdateAmbienteCommand
				{
					Nome = ambiente
				};

				await ApiClientFactory.Instance.CreateAmbiente(command);

				return RedirectToAction(nameof(Create), new { crud = (int)EnumCrud.Created });
			}
			catch (Exception e)
			{
				Console.Write(e.StackTrace);
				return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

			}
		}

		//[ClaimsAuthorize("Profissional", "Excluir")]
		public ActionResult Delete(int id)
		{
			try
			{
				ApiClientFactory.Instance.DeleteProfissional(id);
				return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
			}
			catch
			{
				return RedirectToAction(nameof(Index));
			}
		}

		public async Task<ProfissionalDto> GetProfissionalById(int id)
		{
			var result = ApiClientFactory.Instance.GetProfissionalById(id);

			return result;
		}
		//[ClaimsAuthorize("Profissional", "Consultar")]
		public async Task<bool> GetProfissionalByEmail(string email)
		{
			if (string.IsNullOrEmpty(email)) throw new Exception("Email não informado.");
			var result = ApiClientFactory.Instance.GetProfissionalByEmail(email);

			if (result == null)
			{
				return true;
			}

			return false;
		}

		public async Task<bool> GetProfissionalByCpf(string cpf)
		{
			if (string.IsNullOrEmpty(cpf)) throw new Exception("Cpf não informado.");
			var result = ApiClientFactory.Instance.GetProfissionalByCpf(Regex.Replace(cpf, "[^0-9a-zA-Z]+", ""));

			if (result == null)
			{
				return true;
			}

			return false;
		}
		[HttpPost]
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

				var result2 = ApiClientFactory.Instance.GetUsuarioByEmail(collection["email"].ToString().Trim());

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

				command.PerfilId = result2.PerfilId;
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
	}

}
