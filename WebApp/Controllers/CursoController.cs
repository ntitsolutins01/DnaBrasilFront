﻿using Microsoft.AspNetCore.Authorization;
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


[Authorize(Policy = ModuloAccess.ConfiguracaoSistemaEad)]
public class CursoController : BaseController
{
    #region Constructor

    /// <summary>
    /// Contrutor da página
    /// </summary>
    /// <param name="appSettings">paramentros para chamada de </param>
    public CursoController(IOptions<UrlSettings> appSettings)
    {
        ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Crud Methods

    /// <summary>
    /// Listagem de Curso
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetCursosAll();

        return View(new CursoModel() { Cursos = response });
    }

    /// <summary>
    /// Tela para inclusão de Curso
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var tiposcursos = new SelectList(ApiClientFactory.Instance.GetTipoCursosAll(), "Id", "Nome");
            var coordenadores = new SelectList(ApiClientFactory.Instance.GetUsuarioAll().Where(x=>x.PerfilId == (int)EnumPerfil.Coordenador), "Id", "Nome");


            return View(new CursoModel()
            {
                ListTiposCursos = tiposcursos,
                ListCoordenadores = coordenadores
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de inclusão do Curso
    /// </summary>
    /// <param name="collection">coleção de dados para inclusao de Curso</param>
    /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new CursoModel.CreateUpdateCursoCommand
            {
	            TipoCursoId = Convert.ToInt32(collection["ddlTipoCurso"].ToString()),
	            CoordenadorId = Convert.ToInt32(collection["ddlCoordenador"].ToString()),
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

            await ApiClientFactory.Instance.CreateCurso(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }


    /// <summary>
    /// Ação de alteração do Curso
    /// </summary>
    /// <param name="id">identificador do Curso</param>
    /// <param name="collection">coleção de dados para alteração de Curso</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new CursoModel.CreateUpdateCursoCommand
            {
                Id = Convert.ToInt32(collection["editCursoId"]),
                CoordenadorId = Convert.ToInt32(collection["ddlCoordenador"].ToString()),
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

            await ApiClientFactory.Instance.UpdateCurso(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de exclusão do Curso
    /// </summary>
    /// <param name="id">identificador do Curso</param>
    /// <param name="collection">coleção de dados para exclusão de Curso</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
	    try
	    {
		    ApiClientFactory.Instance.DeleteCurso(id);
		    return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
	    }
	    catch (Exception e)
	    {
			return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Este curso não pode ser excluído pois possui módulos vinculadas a ele." });
		}
    }
    #endregion

    #region Get Methods

    public Task<CursoDto> GetCursoById(int id)
    {
        var result = ApiClientFactory.Instance.GetCursoById(id);
        var coordenadores = new SelectList(ApiClientFactory.Instance.GetUsuarioAll().Where(x => x.PerfilId == (int)EnumPerfil.Coordenador), "Id", "Nome", result.CoordenadorId);
        result.ListCoordenadores = coordenadores;

		return Task.FromResult(result);
    }
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