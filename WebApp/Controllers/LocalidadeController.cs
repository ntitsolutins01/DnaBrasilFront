using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    public class LocalidadeController : BaseController
    {
	    private readonly IOptions<UrlSettings> _appSettings;

	    public LocalidadeController(IOptions<UrlSettings> appSettings)
	    {
		    _appSettings = appSettings;
		    ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
	    }

		public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetLocalidadeAll();

            return View(new LocalidadeModel() { Localidades = response});
		}

        //[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");

            return View(new LocalidadeModel() { ListEstados = estados });
        }

        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new LocalidadeModel.CreateUpdateLocalidadeCommand
                {
                    Nome = collection["nome"].ToString(),
                    Descricao = collection["descricao"].ToString(),
                    MunicipioId = collection["ddlMunicipio"].ToString()

                };

                await ApiClientFactory.Instance.CreateLocalidade(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        //[ClaimsAuthorize("Localidade", "Alterar")]
        public ActionResult Edit(string id)
        {
            var obj = ApiClientFactory.Instance.GetLocalidadeById(id);

            var model = new LocalidadeModel() { Localidade = obj };

            return View(model);
        }

        //[ClaimsAuthorize("Usuario", "Alterar")]
        [HttpPost]
        public async Task<ActionResult> Edit(string id, IFormCollection collection)
        {
                var command = new LocalidadeModel.CreateUpdateLocalidadeCommand
                {
	                Nome = collection["nome"].ToString(),
	                Descricao = collection["descricao"].ToString()
				};

                //await ApiClientFactory.Instance.UpdateLocalidade(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteLocalidade(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
