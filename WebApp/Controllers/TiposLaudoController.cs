using Infraero.Relprev.CrossCutting.Enumerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Areas.Identity.Models;
using WebApp.Configuration;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    public class TiposLaudoController : BaseController
    {
        private readonly IOptions<UrlSettings> _appSettings;

        public TiposLaudoController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetTiposLaudoAll();

            return View(new TiposLaudoModel(){TiposLaudos = response});
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
                var command = new TiposLaudoModel.CreateUpdateTiposLaudoCommand
                {

                    Nome = collection["nome"].ToString(),
                    Descricao = collection["descricao"].ToString()

                };

                await ApiClientFactory.Instance.CreateTiposLaudo(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        //[ClaimsAuthorize("TiposLaudo", "Alterar")]
        //public ActionResult Edit(string id)
        //{
        //    var obj = ApiClientFactory.Instance.GetTiposLaudoById(id);

        //    var model = new TiposLaudoModel() { TiposLaudo = obj };

        //    return View(model);
        //}

        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(string id, IFormCollection collection)
        {
                var command = new TiposLaudoModel.CreateUpdateTiposLaudoCommand
                {

                    Nome = collection["nome"].ToString(),
                    Descricao = collection["descricao"].ToString()
                };

                //await ApiClientFactory.Instance.UpdateTiposLaudo(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(string id)
        {
            try
            {
                //ApiClientFactory.Instance.DeleteTiposLaudo(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
