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
using Microsoft.AspNetCore.Hosting;
using log4net;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Controllers;

/// <summary>
/// Controle de Inventario
/// </summary>
[Authorize(Policy = ModuloAccess.ConfiguracaoSistemaEad)]
public class InventarioController : BaseController
{
    #region Parametros

    private readonly IOptions<UrlSettings> _appSettings;
    private readonly IWebHostEnvironment _host;

    #endregion

    #region Constructor


    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="app">configurações de urls do sistema</param>
    /// <param name="host">informações da aplicação em execução</param>
    public InventarioController(IOptions<UrlSettings> appSettings, IWebHostEnvironment host,
    ILog logger)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        _host = host;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Inventario
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">lista de filtros selecionados para pesquisa de materiais</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    //[ClaimsAuthorize(ClaimType.Inventario, Identity.Claim.Consultar)]
    public async Task<ActionResult> Index(int? crud, int? notify, IFormCollection collection, string message = null)
    {
        var usuario = User.Identity.Name;

        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);

        var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

        var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeAll(), "Id", "Nome");
        var gruposMateriais = new SelectList(ApiClientFactory.Instance.GetGruposMateriaisAll(), "Id", "Nome");
        var arquivosInventarios = ApiClientFactory.Instance.GetArquivosInventariosAll();

        var searchFilter = new InventariosFilterDto
        {
            Id = collection["material"].ToString(),
            LocalidadeId = collection["ddlLocalidade"].ToString() == "" ? usu.LocalidadeId : collection["ddlLocalidade"].ToString(),
            NomeMaterial = collection["nomeMaterial"].ToString(),
            MaterialId = collection["ddlMaterial"].ToString(),
        };
        var result = await ApiClientFactory.Instance.GetInventariosByFilter(searchFilter);

        var model = new InventarioModel
        {
            ListLocalidades = localidades,
            ListGruposMateriais = gruposMateriais,
            LocalidadeId = Convert.ToInt32(!string.IsNullOrEmpty(searchFilter.LocalidadeId) ? searchFilter.LocalidadeId : usu.LocalidadeId),
            ArquivosInventarios = arquivosInventarios,
            Inventarios = result.Inventarios,
            SearchFilter = searchFilter

        };
        return View(model);
    }

    /// <summary>
    /// Tela para Inclusão de Inventario
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    //[ClaimsAuthorize(ClaimType.Inventario, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeAll(), "Id", "Nome");
            var gruposMateriais = new SelectList(ApiClientFactory.Instance.GetGruposMateriaisAll(), "Id", "Nome");

            return View(new InventarioModel()
            {
                ListLocalidades = localidades,
                ListGruposMateriais = gruposMateriais
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Inclusão do Inventario
    /// </summary>
    /// <param name="collection">coleção de dados para Inclusao de Inventario</param>
    /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
    //[ClaimsAuthorize(ClaimType.Inventario, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new InventarioModel.CreateUpdateInventarioCommand
            {
                LocalidadeId = Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                MaterialId = Convert.ToInt32(collection["ddlMaterial"].ToString()),
                Quantidade = Convert.ToInt32(collection["quantidade"].ToString())
            };

            await ApiClientFactory.Instance.CreateInventario(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }


    /// <summary>
    /// Ação de Alteração do Inventario
    /// </summary>
    /// <param name="id">Identificador do Inventario</param>
    /// <param name="collection">coleção de dados para Alteração de Inventario</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    //[ClaimsAuthorize(ClaimType.Inventario, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var inventario = ApiClientFactory.Instance.GetInventarioById(Convert.ToInt32(collection["editInventarioId"]));

            var command = new InventarioModel.CreateUpdateInventarioCommand
            {
                Id = Convert.ToInt32(collection["editInventarioId"]),
            };

            await ApiClientFactory.Instance.UpdateInventario(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Alteração do Inventario
    /// </summary>
    /// <param name="id">Identificador do Inventario</param>
    /// <param name="collection">coleção de dados para Alteração de Inventario</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    //[ClaimsAuthorize(ClaimType.Inventario, Identity.Claim.Alterar)]
    public async Task<ActionResult> Files(IFormCollection collection)
    {
        try
        {
            var command = new ArquivosInventarioModel.CreateUpdateArquivosInventarioCommand
            {
                InventarioId = Convert.ToInt32(collection["editInventarioId"]),
            };

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
                filePath = Path.Combine(_host.WebRootPath, $"ArquivosInventarios\\{newFileName}");

                if (!Directory.Exists(Path.Combine(_host.WebRootPath, $"ArquivosInventarios")))
                    Directory.CreateDirectory(Path.Combine(_host.WebRootPath, $"ArquivosInventarios"));

                command.PathArquivo = filePath;
                command.NomeArquivo = fileName;

                await using Stream fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }

            await ApiClientFactory.Instance.CreateArquivosInventario(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão do Inventario
    /// </summary>
    /// <param name="id">Identificador do Inventario</param>
    /// <param name="collection">coleção de dados para exclusão de Inventario</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
    //[ClaimsAuthorize(ClaimType.Inventario, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteInventario(id);
            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao excluir registro." });
        }
    }

    public Task<JsonResult> GetInventariosByMaterialId(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("Tipo de Inventario não informado.");
            var resultLocal = ApiClientFactory.Instance.GetInventariosByMaterialId(Convert.ToInt32(id));

            return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Descricao")));

        }
        catch (Exception ex)
        {
            return Task.FromResult(Json(ex.Message));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    //[ClaimsAuthorize(ClaimType.Inventario, Claim.Alterar)]
    public ActionResult Download(int id)
    {
        var file = ApiClientFactory.Instance.GetArquivosInventarioById(id);

        var filePath = Path.Combine(_host.WebRootPath, $"ArquivosInventario/{file.NomeArquivo}");

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
    /// Busca Materail por Id
    /// </summary>
    /// <param name="id">Identificador de Materail</param>
    /// <returns>Retorna a Inventario</returns>
    public Task<InventarioDto> GetInventarioById(int id)
    {
        var result = ApiClientFactory.Instance.GetInventarioById(id);

        return Task.FromResult(result);
    }
    #endregion
}