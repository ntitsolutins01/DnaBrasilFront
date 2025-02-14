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
using Claim = WebApp.Identity.Claim;

namespace WebApp.Controllers;


//Testando PR
/// <summary>
/// Controller de Atividade
/// </summary>
//[Authorize(Policy = ModuloAccess.Atividade)]
public class AtividadeController : BaseController
{

    #region Parametros

    private readonly IOptions<UrlSettings> _appSettings;

    #endregion

    #region Constructor

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">configurações de urls do sistema</param>
    public AtividadeController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Atividade
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Atividade, Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetAtividadesAll();

        return View(new AtividadeModel() { Atividades = response });
    }

    /// <summary>
    /// Tela para Inclusão de Atividade
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Atividade, Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");

            var linhasAcoes = new SelectList(ApiClientFactory.Instance.GetLinhasAcoesAll(), "Id", "Nome");

            var categorias = new SelectList(ApiClientFactory.Instance.GetCategoriasAll(), "Id", "Codigo");

            var model = new AtividadeModel()
            {
                ListEstados = estados,
                ListLinhasAcoes = linhasAcoes,
                ListCategorias = categorias
            };

            return View(model);
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
        }
    }

    /// <summary>
    /// Ação de Inclusão do Atividade
    /// </summary>
    /// <param name="collection">Coleção de dados para inclusao de Atividade</param>
    /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Atividade, Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var listDiasSemana = new List<string>
            {
                collection["segunda"].ToString() != "" ? "SEG" : "",
                collection["terca"].ToString() != "" ? "TER" : "",
                collection["quarta"].ToString() != "" ? "QUA" : "",
                collection["quinta"].ToString() != "" ? "QUI" : "",
                collection["sexta"].ToString() != "" ? "SEX" : "",
                collection["sabado"].ToString() != "" ? "SAB" : ""
            };

            var command = new AtividadeModel.CreateUpdateAtividadeCommand
            {
                EstruturaId = Convert.ToInt32(collection["ddlEstrutura"].ToString()),
                LinhaAcaoId = Convert.ToInt32(collection["ddlLinhaAcao"].ToString()),
                CategoriaId = Convert.ToInt32(collection["ddlCategoria"].ToString()),
                ModalidadeId = Convert.ToInt32(collection["ddlModalidade"].ToString()),
                ProfissionalId = Convert.ToInt32(collection["ddlProfessorProfissional"].ToString()),
                LocalidadeId = Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                Turma = collection["ddlTurma"].ToString(),
                HrInicial = collection["hrInicial"].ToString(),
                HrFinal = collection["hrFinal"].ToString(),
                QuantidadeAluno = Convert.ToInt32(collection["qtAluno"].ToString()),
                DiasSemana = string.Join("-", listDiasSemana.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList())
            };

            await ApiClientFactory.Instance.CreateAtividade(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Alteração do Atividade
    /// </summary>
    /// <param name="id">Identificador do Atividade</param>
    /// <param name="collection">Coleção de dados para alteração de Atividade</param>
    /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Atividade, Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new AtividadeModel.CreateUpdateAtividadeCommand
            {
                Id = Convert.ToInt32(collection["editAtividadeId"]),
                //Codigo = collection["codigo"]
                //    .ToString(),
                //Nome = collection["nome"]
                //    .ToString(),
                //IdadeFinal = Convert.ToInt32(collection["idadeFinal"]),
                //IdadeInicial = Convert.ToInt32(collection["idadeInicial"]),
                //Descricao = collection["descricao"]
                //    .ToString(),
                Status = collection["editStatus"]
                             .ToString() ==
                         ""
                    ? false
                    : true,
                EstruturaId = 0,
                LinhaAcaoId = 0,
                CategoriaId = 0,
                ModalidadeId = 0,
                ProfissionalId = 0,
                LocalidadeId = 0,
                Turma = null,
                HrInicial = null,
                HrFinal = null,
                QuantidadeAluno = 0,
                DiasSemana = null,
            };

            await ApiClientFactory.Instance.UpdateAtividade(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão do Atividade
    /// </summary>
    /// <param name="id">Identificador do Atividade</param>
    /// <param name="collection">Coleção de dados para exclusão de Atividade</param>
    [ClaimsAuthorize(ClaimType.Atividade, Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            //if (ApiClientFactory.Instance.GetAtividadeById(id).Any())
            //{
            //	return RedirectToAction(nameof(Index), new { AtividadeId = id, notify = (int)EnumNotify.Error, message = "O Atividade não pode ser excluído pois existem presenças registradas para o mesmo." });
            //}
            ApiClientFactory.Instance.DeleteAtividade(id);
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
    /// Busca uma Única Atividade
    /// </summary>
    /// <param name="id">Id da Atividade a ser buscada</param>
    /// <returns>Retorna o objeto da Atividade</returns>
    public Task<AtividadeDto> GetAtividadeById(int id)
    {
        var result = ApiClientFactory.Instance.GetAtividadeById(id);

        return Task.FromResult(result);
    }

    /// <summary>
    /// Busca Atividade de Aluno por Atividade por Id
    /// </summary>
    /// <param name="id">Id da Atividade a ser buscada</param>
    /// <returns>Retorna o objeto da Atividade</returns>
    public Task<List<AtividadeAlunosDto>> GetAtividadeAlunosByAtividadeId(int id)
    {
        var result = ApiClientFactory.Instance.GetAtividadeAlunosByAtividadeId(id);

        return Task.FromResult(result);
    }

    
    #endregion
}
