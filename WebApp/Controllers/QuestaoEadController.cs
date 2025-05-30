using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
/// Controle DE Questionario Ead
/// </summary>
[Authorize(Policy = ModuloAccess.ConfiguracaoSistemaEad)]
public class QuestaoEadController : BaseController
{

    #region Parametros

    private readonly IWebHostEnvironment _host;

    #endregion

    #region Constructor

    /// <summary>
    /// Contrutor da página
    /// </summary>
    /// <param name="appSettings">Configurações da aplicação</param>
    /// <param name="host">Informação do ambiente em que a aplicação está rodando</param>
    public QuestaoEadController(IOptions<UrlSettings> appSettings, IWebHostEnvironment host)
    {
        ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
        _host = host;
    }
    #endregion

    #region Mian Methods
    /// <summary>
    /// Listagem de Questao Ead
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">lista de filtros selecionados para pesquisa de alunos</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.QuestaoEad, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetQuestaoEadAll();

        return View(new QuestaoEadModel() { QuestoesEad = response });
    }

    /// <summary>
    /// Tela para Inclusão de Questao Ead
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.QuestaoEad, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var tipoCurso = new SelectList(ApiClientFactory.Instance.GetTipoCursosAll(), "Id", "Nome");

            return View(new QuestaoEadModel()
            {
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
    /// Ação de Inclusão de Questao Ead
    /// </summary>
    /// <param name="collection">coleção de dados para inclusao de Questao Ead</param>
    /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
    //[ClaimsAuthorize(ClaimType.QuestaoEad, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            #region objeto questão

            var commandQuestaoEad = new QuestaoEadModel.CreateUpdateQuestaoEadCommand
            {
                AulaId = Convert.ToInt32(collection["ddlAula"].ToString()),
                Pergunta = collection["pergunta"].ToString(),
                Questao = Convert.ToInt32(collection["questao"].ToString()),
                Referencia = collection["referencia"].ToString()
            };

            var listDdlTipoTextoImagem = (from item in collection where item.Key.Contains("texto") select item.Value).Select(v => (string)v).ToList();

            if (listDdlTipoTextoImagem.Any())
            {
                commandQuestaoEad.ListTextos = listDdlTipoTextoImagem;
            }

            if (collection.Files.Any())
            {
                var listImagens = new List<string?>();
                string? filePath;
                string extension = ".jpg";
                string newFileName = Path.ChangeExtension(
                    Guid.NewGuid().ToString(),
                    extension
                );

                foreach (var file in collection.Files)
                {
                    if (file.Length <= 0) continue;
                    filePath = Path.Combine(_host.WebRootPath, $"QuestoesEad\\{newFileName}");

                    if (!Directory.Exists(Path.Combine(_host.WebRootPath, $"QuestoesEad")))
                        Directory.CreateDirectory(Path.Combine(_host.WebRootPath, $"QuestoesEad"));

                    listImagens.Add(filePath);

                    using Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                }

                if (listImagens.Any())
                {
                    commandQuestaoEad.ListImagens = listImagens;
                }
            }

            #endregion

            //await ApiClientFactory.Instance.CreateQuestaoEad(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }


    /// <summary>
    /// Ação de Alteração de Questao Ead
    /// </summary>
    /// <param name="id">identificador do Questao Ead</param>
    /// <param name="collection">coleção de dados para alteração de Questao Ead</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.QuestaoEad, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            List<RespostaEadDto> respostasList = new List<RespostaEadDto>();

            var command = new QuestaoEadModel.CreateUpdateQuestaoEadCommand
            {
                Id = Convert.ToInt32(collection["editQuestaoEadId"]),
                Referencia = collection["referencia"].ToString(),
                Pergunta = collection["pergunta"].ToString(),
                Respostas = JsonConvert.DeserializeObject<List<RespostaEadDto>>(collection["respostas"]),
                Questao = Convert.ToInt32(collection["questao"].ToString()),
            };

            await ApiClientFactory.Instance.UpdateQuestaoEad(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão de Questao Ead
    /// </summary>
    /// <param name="id">identificador do Questao Ead</param>
    /// <param name="collection">coleção de dados para exclusão de Questao Ead</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.QuestaoEad, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteQuestaoEad(id);
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
    ///  Busca de Questão por id
    /// </summary>
    /// <param name="id">id da Questao</param>
    /// <returns>Retorna o objeto Questao</returns>
    public Task<QuestaoEadDto> GetQuestaoEadById(int id)
    {
        var result = ApiClientFactory.Instance.GetQuestaoEadById(id);

        return Task.FromResult(result);
    }
    #endregion
}