using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    public class PlanoAulaController : BaseController
    {

        private readonly IOptions<UrlSettings> _appSettings;

        public PlanoAulaController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetPlanosAulasAll();

            return View(new PlanoAulaModel() { PlanosAulas = response });
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
                var command = new PlanoAulaModel.CreateUpdatePlanoAulaCommand
                {
                    Nome = collection["nome"].ToString(),
                    Grade = collection["grade"].ToString(),
                    Url = collection["url"].ToString(),
                };

                await ApiClientFactory.Instance.CreatePlanoAula(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        //[ClaimsAuthorize("PlanoAula", "Alterar")]
        public ActionResult Edit(string id)
        {
            var obj = ApiClientFactory.Instance.GetPlanoAulaById(id);

            var model = new PlanoAulaModel() { PlanoAula = obj };

            return View(model);
        }

        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            var command = new PlanoAulaModel.CreateUpdatePlanoAulaCommand
            {
                Id = Convert.ToInt32(id),
                Nome = collection["nome"].ToString(),
                Grade = collection["grade"].ToString(),
                Url = collection["url"].ToString(),
            };

            await ApiClientFactory.Instance.UpdatePlanoAula(id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeletePlanoAula(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
