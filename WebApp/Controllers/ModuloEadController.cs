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

namespace WebApp.Controllers;

/// <summary>
/// Contrtole de Modulo Ead
/// </summary>
[Authorize(Policy = ModuloAccess.ConfiguracaoSistemaEad)]
public class ModuloEadController : BaseController
{
    #region Constructor
    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="app">configurações de urls do sistema</param>
    /// <param name="host">informações da aplicação em execução</param>
    public ModuloEadController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Crud Methods
    /// <summary>
    /// Listagem de Modulo Ead
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">lista de filtros selecionados para pesquisa de alunos</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.ModuloEad, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetModulosEadAll();

        return View(new ModuloEadModel() { ModulosEad = response });
    }

    /// <summary>
    /// Tela para Inclusão de Modulo Ead
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.ModuloEad, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var modulosEad = new SelectList(ApiClientFactory.Instance.GetModulosEadAll(), "Id", "Nome");
            var tipoCurso = new SelectList(ApiClientFactory.Instance.GetTipoCursosAll(), "Id", "Nome");
            //var curso = new SelectList(ApiClientFactory.Instance.GetCursosAll(), "Id", "Nome");


            return View(new ModuloEadModel()
            {
                ListModulosEad = modulosEad,
                ListTipoCursos = tipoCurso
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Inclusão do Modulo Ead
    /// </summary>
    /// <param name="collection">coleção de dados para inclusao de Modulo Ead</param>
    /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ModuloEad, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new ModuloEadModel.CreateUpdateModuloEadCommand
            {
	            CursoId = Convert.ToInt32(collection["ddlCurso"].ToString()),
	            Titulo = collection["nome"].ToString(),
	            Descricao = collection["descricao"].ToString(),
				CargaHoraria = Convert.ToInt32(collection["cargaHoraria"].ToString())
			};

            //foreach (var file in collection.Files)
            //{
            //    if (file.Length <= 0) continue;

            //    command.Imagem = Path.GetFileName(collection.Files[0].FileName);

            //    using (var ms = new MemoryStream())
            //    {
            //        file.CopyToAsync(ms);
            //        var byteIMage = ms.ToArray();
            //        command.ByteImage = byteIMage;
            //    }
            //}

            await ApiClientFactory.Instance.CreateModuloEad(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }


    /// <summary>
    /// Ação de Alteração do Modulo Ead
    /// </summary>
    /// <param name="id">identificador do Modulo Ead</param>
    /// <param name="collection">coleção de dados para alteração de Modulo Ead</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ModuloEad, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new ModuloEadModel.CreateUpdateModuloEadCommand
            {
                Id = Convert.ToInt32(collection["editModuloEadId"]),
                Titulo = collection["nome"].ToString(),
                Descricao = collection["descricao"].ToString(),
                CargaHoraria = Convert.ToInt32(collection["cargaHoraria"].ToString()),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            //foreach (var file in collection.Files)
            //{
            //    if (file.Length <= 0) continue;

            //    command.Imagem = Path.GetFileName(collection.Files[0].FileName);

            //    using (var ms = new MemoryStream())
            //    {
            //        file.CopyToAsync(ms);
            //        var byteIMage = ms.ToArray();
            //        command.ByteImage = byteIMage;
            //    }
            //}

            await ApiClientFactory.Instance.UpdateModuloEad(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão do Modulo Ead
    /// </summary>
    /// <param name="id">identificador do Modulo Ead</param>
    /// <param name="collection">coleção de dados para exclusão de Modulo Ead</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ModuloEad, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteModuloEad(id);
            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
        }
        catch (Exception e)
        {
			return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Este módulo não pode ser excluído pois possui aulas vinculadas a ele." });
		}
    }

    
    #endregion

    #region Get Methods

    /// <summary>
    /// Busca Modulo por Id
    /// </summary>
    /// <param name="id">Identificador de Modulo</param>
    /// <returns>Retorna a Modulo</returns>
    public Task<ModuloEadDto> GetModuloEadById(int id)
    {
        var result = ApiClientFactory.Instance.GetModuloEadById(id);

        return Task.FromResult(result);
    }


    /// <summary>
    ///  Busca todos os módulos Ead pelo id do curso
    /// </summary>
    /// <param name="id">Id do curso</param>
    /// <returns>Retorna um json com todos os módulos ead</returns>
    public Task<JsonResult> GetModulosEadAllByCursoId(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("Curso não informado.");
            var resultLocal = ApiClientFactory.Instance.GetModulosEadAllByCursoId(Convert.ToInt32(id));

            return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Titulo")));

        }
        catch (Exception ex)
        {
            return Task.FromResult(Json(ex.Message));
        }
    }

    /// <summary>
    /// Busca todos os Cursos pelo id Tipo Curso 
    /// </summary>
    /// <param name="id">Id do curso</param>
    /// <returns>Retorna um json com todos os Cursos</returns>
    public Task<JsonResult> GetCursosAllByTipoCursoId(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("Tipo de Curso não informado.");
            var resultLocal = ApiClientFactory.Instance.GetCursosAllByTipoCursoId(Convert.ToInt32(id));

            return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Titulo")));

        }
        catch (Exception ex)
        {
            return Task.FromResult(Json(ex.Message));
        }
    }

    #endregion
}