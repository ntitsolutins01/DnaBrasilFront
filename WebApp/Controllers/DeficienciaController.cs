using Infraero.Relprev.CrossCutting.Enumerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Areas.Identity.Models;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers;

public class DeficienciaController : BaseController
{
    #region Constructor

    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="appSettings"></param>
    public DeficienciaController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }


    #endregion


    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetDeficienciaAll();

        return View(new DeficienciaModel(){Deficiencias = response});
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
            var command = new DeficienciaModel.CreateUpdateDeficienciaCommand
            {
                Nome = collection["nome"].ToString()
            };

            await ApiClientFactory.Instance.CreateDeficiencia(command);

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
        var command = new DeficienciaModel.CreateUpdateDeficienciaCommand
        {
            Id = Convert.ToInt32(collection["editDeficienciaId"]),
            Nome = collection["nome"].ToString(),
            Status = collection["editStatus"].ToString() == "" ? false : true
        };

        await ApiClientFactory.Instance.UpdateDeficiencia(command.Id, command);

        return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
    }

    //[ClaimsAuthorize("Usuario", "Excluir")]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteDeficiencia(id);
            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
        }
        catch
        {
            return RedirectToAction(nameof(Index));
        }
    }

    public Task<DeficienciaDto> GetDeficienciaById(int id)
    {
        var result = ApiClientFactory.Instance.GetDeficienciaById(id);

        return Task.FromResult(result);
    }
}