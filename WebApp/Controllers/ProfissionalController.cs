using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    public class ProfissionalController : BaseController
    {

        private readonly IOptions<UrlSettings> _appSettings;

        public ProfissionalController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetProfissionalAll();

            return View(new ProfissionalModel(){Profissionais = response});
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
                var command = new ProfissionalModel.CreateUpdateProfissionalCommand
                {
                    Nome = collection["nome"].ToString(),
                    DtNascimento = collection["dataNascimento"].ToString(),
                    Email = Convert.ToInt32(collection["email"]),
                    Sexo = Convert.ToInt32(collection["sexo"]),
                    Telefone = collection["telefone"].ToString(),
                    Cep = collection["cep"].ToString(),
                    Celular = collection["celular"].ToString(),
                    Cpf = collection["cpf"].ToString(),
                    AspNetUserId = collection["aspnetuserId"].ToString(),
                    Numero = collection["numero"].ToString(),
                    Bairro = collection["bairro"].ToString(),
					Endereco = collection["endereco"].ToString(),
					MunicipioId = collection["municipioId"].ToString(),
					Habilitado = collection["habilitado"].ToString(),
				};

                await ApiClientFactory.Instance.CreateProfissional(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        //[ClaimsAuthorize("Profissional", "Alterar")]
        //public ActionResult Edit(string id)
        //{
        //    var obj = ApiClientFactory.Instance.GetProfissionalById(id);

        //    var model = new ProfissionalModel() { Profissional = obj };

        //    return View(model);
        //}

        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            var command = new ProfissionalModel.CreateUpdateProfissionalCommand
            {
                Id = Convert.ToInt32(id),
                Nome = collection["nome"].ToString(),
                DtNascimento = collection["dataNascimento"].ToString(),
                Email = Convert.ToInt32(collection["email"]),
                Sexo = Convert.ToInt32(collection["sexo"]),
                Telefone = collection["telefone"].ToString(),
                Cep = collection["cep"].ToString(),
                Celular = collection["celular"].ToString(),
                Cpf = collection["cpf"].ToString(),
                AspNetUserId = collection["aspnetuserId"].ToString(),
                Numero = collection["numero"].ToString(),
                Bairro = collection["bairro"].ToString(),
                Endereco = collection["endereco"].ToString(),
                MunicipioId = collection["municipioId"].ToString(),
                Habilitado = collection["habilitado"].ToString()
			};

            await ApiClientFactory.Instance.UpdateProfissional(id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        //[ClaimsAuthorize("Usuario", "Excluir")]
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
    }
}
