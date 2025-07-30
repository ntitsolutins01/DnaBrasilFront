using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApp.Authorization;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    public class CertificadoController : BaseController
    {
        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;
        private readonly IWebHostEnvironment _host;

        #endregion

        #region Constructor

        /// <summary>
        /// Contrutor da página
        /// </summary>
        /// <param name="appSettings">Configurações da aplicação</param>
        /// <param name="host">Informação do ambiente em que a aplicação está rodando</param>
        public CertificadoController(IOptions<UrlSettings> appSettings, IWebHostEnvironment host)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
            _host = host;
        }
        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem de Certificado
        /// </summary>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        [ClaimsAuthorize(ClaimType.Certificado, Identity.Claim.Consultar)]
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            ViewBag.Status = true;
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetCertificadosAll() ?? new List<CertificadoDto>();
            var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome");

            return View(new CertificadoModel()
            {
                Certificados = response,
                ListFomentos = fomentos
            });
        }

        /// <summary>
        /// Tela para Inclusão de Certificado
        /// </summary>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        [ClaimsAuthorize(ClaimType.Certificado, Identity.Claim.Incluir)]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);
                var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome");

                return View(new CertificadoModel
                {
                    ListFomentos = fomentos
                });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        /// <summary>
        ///  Ação de Inclusão de Certificado
        /// </summary>
        /// <param name="collection">Coleção de dados para inclusao de Certificado</param>
        /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
        [ClaimsAuthorize(ClaimType.Certificado, Identity.Claim.Consultar)]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new CertificadoModel.CreateCertificadoCommand
                {
                    FomentoId = Convert.ToInt32(collection["ddlFomento"].ToString()),
                    Status = collection["Status"].ToString().ToLower() == "on"
                };

                // Caminho para salvar as imagens
                string certificadosPath = Path.Combine(_host.WebRootPath, "Certificados");
                if (!Directory.Exists(certificadosPath))
                    Directory.CreateDirectory(certificadosPath);

                string? filePath;
                string? fileName;
                string extension = ".pdf";
                string newFileName = Path.ChangeExtension(
                    Guid.NewGuid().ToString(),
                    extension
                );

                foreach (var file in collection.Files)
                {
                    if (file.Length <= 0) continue;
                    fileName = Path.GetFileName(collection.Files[0].FileName);
                    filePath = Path.Combine(_host.WebRootPath, $"Certificados\\{newFileName}");

                    if (!Directory.Exists(Path.Combine(_host.WebRootPath, $"Certificados")))
                        Directory.CreateDirectory(Path.Combine(_host.WebRootPath, $"Certificados"));

                    command.Url = filePath;
                    command.Nome = fileName;

                    using Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                }

                await ApiClientFactory.Instance.CreateCertificado(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        ///// <summary>
        ///// Tela para Alteração de Certificado
        ///// </summary>
        ///// <param name="id">Identificador de Certificado</param>
        ///// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        ///// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
        ///// <param name="message">Retorna mensagem de alteração através do parametro crud</param>
        ///// <returns></returns>
        //[ClaimsAuthorize(ClaimType.Certificado, Identity.Claim.Alterar)]
        //public ActionResult Edit(int id, int? crud, int? notify, string message = null)
        //{
        //    SetNotifyMessage(notify, message);
        //    SetCrudMessage(crud);

        //    var certificado = ApiClientFactory.Instance.GetCertificadoById(id);
        //    var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome", certificado.FomentoId);

        //    var model = new CertificadoModel
        //    {
        //        Certificado = certificado,
        //        ListFomentos = fomentos
        //    };
        //    return View(model);
        //}

        /// <summary>
        /// Ação de Alteração de Certificado
        /// </summary>
        /// <param name="id">Identificador de Certificado</param>
        /// <param name="collection">Coleção de dados para Alteração de Certificado</param>
        /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
        [ClaimsAuthorize(ClaimType.Certificado, Identity.Claim.Alterar)]
        [HttpPost]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            var command = new CertificadoModel.UpdateCertificadoCommand
            {
                Id = Convert.ToInt32(collection["editCertificadoId"]),
                FomentoId = Convert.ToInt32(collection["ddlFomento"]),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            string? filePath;
            string? fileName;
            string extension = ".pdf";
            string newFileName = Path.ChangeExtension(
                Guid.NewGuid().ToString(),
                extension
            );

            var certificado = ApiClientFactory.Instance.GetCertificadoById(command.Id);

            if (!certificado.Url.IsNullOrEmpty())
                System.IO.File.Delete(certificado.Url);

            if (!collection.Files.Any())
            {
                command.Url = certificado.Url;
                command.Nome = certificado.Nome;
            }

            foreach (var file in collection.Files)
            {
                if (file.Length <= 0) continue;
                fileName = Path.GetFileName(collection.Files[0].FileName);
                filePath = Path.Combine(_host.WebRootPath, $"Certificados\\{newFileName}");

                if (!Directory.Exists(Path.Combine(_host.WebRootPath, $"Certificados")))
                    Directory.CreateDirectory(Path.Combine(_host.WebRootPath, $"Certificados"));

                command.Url = filePath;
                command.Nome = fileName;

                using Stream fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }

            await ApiClientFactory.Instance.UpdateCertificado(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        /// <summary>
        /// Ação de Exclusão de Certificado
        /// </summary>
        /// <param name="id">Identificador do Certificado</param>
        /// <returns>Retorna mensagem de exclusão através do parametro crud</returns>
        [ClaimsAuthorize(ClaimType.Certificado, Identity.Claim.Excluir)]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteCertificado(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Busca de Certificado por Id
        /// </summary>
        /// <param name="id">Identificador de Certificado</param>
        /// <returns>Retorna o Certificado</returns>
        [HttpGet]
        [Route("Certificado/GetCertificadoById")]
        public Task<CertificadoDto> GetCertificadoById(int id)
        {
            var result = ApiClientFactory.Instance.GetCertificadoById(id);
            var fomentos = result.FomentoId == null ? null : new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome", result.FomentoId);
            result.ListFomentos = fomentos;

            return Task.FromResult(result);
        }
        #endregion
    }
}
