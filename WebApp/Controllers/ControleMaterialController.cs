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
/// Controle de Material
/// </summary>
public class ControleMaterialController : BaseController
{
    #region Parametros

    private readonly IOptions<UrlSettings> _appSettings;

    #endregion

    #region Constructor

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">Configurações de urls do sistema</param>
    public ControleMaterialController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion
    
    #region Main Methods
    /// <summary>
    /// Listagem de Controle Material
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.ControleMaterial, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetControlesMateriaisAll();

        return View(new ControleMaterialModel() { ControlesMateriais = response });
    }

    /// <summary>
    /// Tela para Inclusão de Controle Material
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.ControleMaterial, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var linhaAcao = new SelectList(ApiClientFactory.Instance.GetLinhasAcoesAll(), "Id", "Nome");

            return View(new ControleMaterialModel()
            {
                ListLinhasAcoes = linhaAcao
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

	/// <summary>
	/// Ação de Inclusão do Controle Material
	/// </summary>
	/// <param name="collection">Coleção de dados para inclusao de Controle Material</param>
	/// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
	[ClaimsAuthorize(ClaimType.ControleMaterial, Identity.Claim.Incluir)]
	[HttpPost]
	public async Task<ActionResult> Create(IFormCollection collection)
	{
		try
		{
			var command = new ControleMaterialModel.CreateUpdateControleMaterialCommand
			{
				Id = Convert.ToInt32(collection["editControleMaterialId"]),
				LinhaAcaoId = Convert.ToInt32(collection["ddlLinhaAcao"]),
				Descricao = collection["descricao"].ToString(),
				UnidadeMedida = collection["unidademedida"].ToString(),
				Quantidade = Convert.ToInt32(collection["quantidade"]),
				Saida = Convert.ToInt32(collection["saida"]),
				Disponivel = Convert.ToInt32(collection["disponivel"]),
				Status = collection["editStatus"].ToString() == "" ? false : true

			};

			//var possuiControleMaterial = ApiClientFactory.Instance.GetControleMaterialByAlunoIdDisciplinaId(Convert.ToInt32(command.AlunoId), Convert.ToInt32(command.DisciplinaId));

			//if (possuiControleMaterial==null)
			//{
			// return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Warning, message = "Já existe ControleMaterial cadastrada para este aluno na disciplina informada." });
			//}

			await ApiClientFactory.Instance.CreateControleMaterial(command);

			return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
		}
		catch (Exception e)
		{
			return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
		}
	}

	/// <summary>
	/// Ação de Alteração do Controle Material
	/// </summary>
	/// <param name="collection">Coleção de dados para alteração de Controle Material</param>
	/// <returns>Retorna mensagem de alteração através do parametro crud</returns>
	[ClaimsAuthorize(ClaimType.ControleMaterial, Identity.Claim.Alterar)]
	public async Task<ActionResult> Edit(IFormCollection collection)
	{
		try
		{
			var command = new ControleMaterialModel.CreateUpdateControleMaterialCommand
			{
				Id = Convert.ToInt32(collection["editControleMaterialId"]),
				Descricao = collection["descricao"].ToString(),
				UnidadeMedida = collection["unidademedida"].ToString(),
				Quantidade = Convert.ToInt32(collection["quantidade"]),
				Saida = Convert.ToInt32(collection["saida"]),
				Disponivel = Convert.ToInt32(collection["disponivel"]),
				Status = collection["editStatus"].ToString() == "" ? false : true

			};

			await ApiClientFactory.Instance.UpdateControleMaterial(command.Id, command);

			return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
		}
		catch (Exception e)
		{
			return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
		}
	}

    /// <summary>
    /// Ação de Exclusão do Controle Material
    /// </summary>
    /// <param name="id">Identificador do Controle Material</param>
    /// <returns>Retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ControleMaterial, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteControleMaterial(id);
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
    /// Método de busca de Controle de Material por id
    /// </summary>
    /// <param name="id">Id do Material </param>
    /// <returns>Retorna o objeto Controle de Material</returns>
    [ClaimsAuthorize(ClaimType.ControleMaterial, Identity.Claim.Consultar)]
    public Task<ControleMaterialDto> GetControleMaterialById(int id)
    {
        var result = ApiClientFactory.Instance.GetControleMaterialById(id);

        return Task.FromResult(result);
    }
    #endregion
}