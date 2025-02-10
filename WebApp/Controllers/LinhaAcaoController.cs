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
    public class LinhaAcaoController : BaseController
    {
        private readonly IOptions<UrlSettings> _appSettings;

        public LinhaAcaoController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetLinhasAcoesAll();

            return View(new LinhaAcaoModel() { LinhasAcoes = response });
        }

        //[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);


            return View();
        }

        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new LinhaAcaoModel.CreateUpdateLinhaAcaoCommand
                {
                    Nome = collection["tipoParceria"].ToString(),
                    Status = true
                };

                await ApiClientFactory.Instance.CreateLinhaAcao(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            var command = new LinhaAcaoModel.CreateUpdateLinhaAcaoCommand
            {
                Id = Convert.ToInt32(collection["editLinhaAcaoId"]),
                Nome = collection["nome"].ToString(),
                //Parceria = Convert.ToInt32(collection["ddlParceria"].ToString()),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            await ApiClientFactory.Instance.UpdateLinhaAcao(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteLinhaAcao(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public Task<LinhaAcaoDto> GetLinhaAcaoById(int id)
        {
            var result = ApiClientFactory.Instance.GetLinhaAcaoById(id);
            
            return Task.FromResult(result);
        }
    }

    
}
