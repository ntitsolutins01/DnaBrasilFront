using System.Globalization;
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
/// Controle de Nota
/// </summary>
[Authorize(Policy = ModuloAccess.Nota)]
public class NotaController : BaseController
{
    #region Constructor
    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="app">configurações de urls do sistema</param>
    /// <param name="host">informações da aplicação em execução</param>
    public NotaController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Mian Methods
    /// <summary>
    /// Listagem de Nota
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">lista de filtros selecionados para pesquisa de alunos</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Nota, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetNotasAll();

        return View(new NotaModel() { Notas = response });
    }

    /// <summary>
    /// Tela para Inclusão de Nota
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Nota, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
            var disciplinas = new SelectList(ApiClientFactory.Instance.GetDisciplinasAll(), "Id", "Nome");



            return View(new NotaModel()
            {
                ListEstados = estados,
                ListDisciplinas = disciplinas
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Inclusão do Nota
    /// </summary>
    /// <param name="collection">coleção de dados para inclusao de Nota</param>
    /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Nota, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new NotaModel.CreateUpdateNotaCommand
            {
                AlunoId = collection["ddlAluno"] == "" ? null : Convert.ToInt32(collection["ddlAluno"].ToString()).ToString(),
                DisciplinaId = collection["ddlDisciplina"] == "" ? null : Convert.ToInt32(collection["ddlDisciplina"].ToString()).ToString(),
                PrimeiroBimestre = collection["notaPrimeiroBimestre"].ToString() == "" ? 0 : Convert.ToDecimal(collection["notaPrimeiroBimestre"].ToString(), new CultureInfo("pt-BR", true)),
                SegundoBimestre = collection["notaSegundoBimestre"].ToString() == "" ? 0 : Convert.ToDecimal(collection["notaSegundoBimestre"].ToString(), new CultureInfo("pt-BR", true)),
                TerceiroBimestre = collection["notaTerceiroBimestre"].ToString() == "" ? 0 : Convert.ToDecimal(collection["notaTerceiroBimestre"].ToString(), new CultureInfo("pt-BR", true)),
                QuartoBimestre = collection["notaQuartoBimestre"].ToString() == "" ? 0 : Convert.ToDecimal(collection["notaQuartoBimestre"].ToString(), new CultureInfo("pt-BR", true))
            };

            //var possuiNota = ApiClientFactory.Instance.GetNotaByAlunoIdDisciplinaId(Convert.ToInt32(command.AlunoId), Convert.ToInt32(command.DisciplinaId));

            //if (possuiNota==null)
            //{
	           // return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Warning, message = "Já existe nota cadastrada para este aluno na disciplina informada." });
            //}

			await ApiClientFactory.Instance.CreateNota(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Alteração do Nota
    /// </summary>
    /// <param name="id">identificador do Nota</param>
    /// <param name="collection">coleção de dados para alteração de Nota</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Nota, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new NotaModel.CreateUpdateNotaCommand
            {
                Id = Convert.ToInt32(collection["editNotaId"]),
                PrimeiroBimestre = collection["notaPrimeiroBimestre"].ToString() == "" ? null : Convert.ToDecimal(collection["notaPrimeiroBimestre"].ToString()),
                SegundoBimestre = collection["notaSegundoBimestre"].ToString() == "" ? null : Convert.ToDecimal(collection["notaSegundoBimestre"].ToString()),
                TerceiroBimestre = collection["notaTerceiroBimestre"].ToString() == "" ? null : Convert.ToDecimal(collection["notaTerceiroBimestre"].ToString()),
                QuartoBimestre = collection["notaQuartoBimestre"].ToString() == "" ? null : Convert.ToDecimal(collection["notaQuartoBimestre"].ToString()),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            await ApiClientFactory.Instance.UpdateNota(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão do Nota
    /// </summary>
    /// <param name="id">identificador do Nota</param>
    /// <param name="collection">coleção de dados para exclusão de Nota</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Nota, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteNota(id);
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
    /// Busca Nota por Id
    /// </summary>
    /// <param name="id">Identificador de Nota</param>
    /// <returns>Retorna a Nota</returns>
    public Task<NotaDto> GetNotaById(int id)
    {
        var result = ApiClientFactory.Instance.GetNotaById(id);

        return Task.FromResult(result);
    }
    #endregion
}