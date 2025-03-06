using Microsoft.AspNetCore.Authorization;
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
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebApp.Controllers
{
    /// <summary>
	/// Controle de Plano Aula
	/// </summary>
    [Authorize(Policy = ModuloAccess.PlanoAula)]
    public class PlanoAulaController : BaseController
	{
        #region Constructor
        private readonly IOptions<UrlSettings> _appSettings;
		private readonly IHostingEnvironment _host;

		public PlanoAulaController(IOptions<UrlSettings> appSettings,
			IHostingEnvironment host)
		{
			_appSettings = appSettings;
			ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
			_host = host;
		}
        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem do Plano de Aula
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        [ClaimsAuthorize(ClaimType.PlanoAula, Claim.Consultar)]
        public IActionResult Index(int? crud, int? notify, string message = null)
		{
			SetNotifyMessage(notify, message);
			SetCrudMessage(crud);
			var response = ApiClientFactory.Instance.GetPlanosAulasAll();
			var modalidades = new SelectList(ApiClientFactory.Instance.GetModalidadeAll(), "Nome", "Nome");

			return View(new PlanoAulaModel() { PlanosAulas = response, ListModalidades = modalidades });
		}

        /// <summary>
        /// Tela para Inclusão do Plano de Aula
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        [ClaimsAuthorize(ClaimType.PlanoAula, Claim.Incluir)]
        public ActionResult Create(int? crud, int? notify, string message = null)
		{
			SetNotifyMessage(notify, message);
			SetCrudMessage(crud);

			var modalidades = new SelectList(ApiClientFactory.Instance.GetModalidadeAll(), "Nome", "Nome");

			return View(new PlanoAulaModel() { ListModalidades = modalidades });
		}

        /// <summary>
        /// Ação de Inclusão do Plano  de Aula
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao de Plano Aula</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        [ClaimsAuthorize(ClaimType.PlanoAula, Claim.Incluir)]
        [HttpPost]
		public async Task<ActionResult> Create(IFormCollection collection)
		{
			try
			{
				string filePath = null;
				string fileName = null;

				foreach (var file in collection.Files)
				{
					if (file.Length <= 0) continue;
					fileName = Path.GetFileName(collection.Files[0].FileName);
					filePath = Path.Combine(_host.WebRootPath, $"PlanosAulas/{fileName}");

					if (!Directory.Exists(Path.Combine(_host.WebRootPath, $"PlanosAulas")))
						Directory.CreateDirectory(Path.Combine(_host.WebRootPath, $"PlanosAulas"));

					using Stream fileStream = new FileStream(filePath, FileMode.Create);
					await file.CopyToAsync(fileStream);
				}

				var command = new PlanoAulaModel.CreateUpdatePlanoAulaCommand
				{
					Nome = collection["ddlPlanoAula"].ToString(),
					TipoEscolaridade = collection["ddlTipoEscolaridade"].ToString(),
					Modalidade = collection["ddlModalidade"].ToString(),
					NomeArquivo = fileName,
                    Url = filePath
				};

				await ApiClientFactory.Instance.CreatePlanoAula(command);

				return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
			}
			catch (Exception e)
			{
				return RedirectToAction(nameof(Index));
			}
		}

        /// <summary>
        /// Ação de Alteração do Plano de Aula
        /// </summary>
        /// <param name="collection">coleção de dados para alteração de Plano Aula</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        [ClaimsAuthorize(ClaimType.PlanoAula, Claim.Alterar)]
        public async Task<ActionResult> Edit(IFormCollection collection)
		{
			try
			{
				string filePath = null;

				foreach (var file in collection.Files)
				{
					if (file.Length <= 0) continue;
					var fileName = Path.GetFileName(collection.Files[0].FileName);
					filePath = Path.Combine(_host.WebRootPath, $"PlanosAulas/{fileName}");

					await using Stream fileStream = new FileStream(filePath, FileMode.Create);
					await file.CopyToAsync(fileStream);
				}

				var command = new PlanoAulaModel.CreateUpdatePlanoAulaCommand
				{
					Id = Convert.ToInt32(collection["editPlanoAulaId"]),
					Nome = collection["ddlPlanoAula"].ToString(),
					TipoEscolaridade = collection["ddlTipoEscolaridade"].ToString(),
					Modalidade = collection["ddlModalidade"].ToString(),
					Url = filePath,
					NomeArquivo = collection.Files[0].FileName
				};

				await ApiClientFactory.Instance.UpdatePlanoAula(command.Id, command);

				return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
			}
			catch (Exception e)
			{
				return RedirectToAction(nameof(Index));
			}
		}

        /// <summary>
        /// Ação de Exclusão do Plano de Aula
        /// </summary>
        /// <param name="id">identificador do Plano de Aula</param>
        /// <returns></returns>
        [ClaimsAuthorize(ClaimType.PlanoAula, Claim.Excluir)]
        public ActionResult Delete(int id)
		{
			try
			{
                var result =  ApiClientFactory.Instance.GetPlanoAulaById(id);

                if (result != null)	
                {
                    ApiClientFactory.Instance.DeletePlanoAula(id);
                    System.IO.File.Delete(result.Url);
                }

				return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
			}
			catch
			{
				return RedirectToAction(nameof(Index));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
        [ClaimsAuthorize(ClaimType.PlanoAula, Claim.Download)]
        public ActionResult Download(int id)
        {
            var file = ApiClientFactory.Instance.GetPlanoAulaById(id);

            var filePath = Path.Combine(_host.WebRootPath, $"PlanosAulas/{file.NomeArquivo}");

            if (!System.IO.File.Exists(filePath))
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Warning, message = "Arquivo não encontrado." });
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var response = new FileContentResult(fileBytes, "application/octet-stream")
            {
                FileDownloadName = file.NomeArquivo
            };
            return response;
        }

        #endregion

        #region Get Methods

        /// <summary>
		/// Busca Plano de Aula por Id
		/// </summary>
		/// <param name="id">Identificador de Plano de Aula</param>
		/// <returns>Retorna a Categoria</returns>
        [ClaimsAuthorize(ClaimType.PlanoAula, Claim.Consultar)]
        public Task<PlanoAulaDto> GetPlanoAulaById(int id)
		{
			var result = ApiClientFactory.Instance.GetPlanoAulaById(id);

			return Task.FromResult(result);
		}

        #endregion

	}
}
