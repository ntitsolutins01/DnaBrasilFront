using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using WebApp.Authorization;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;
using static WebApp.Models.ControleFrequenciaEscolarModel;

namespace WebApp.Controllers;

/// <summary>
/// Controle de Frequencia Escolar
/// </summary>
public class ControleFrequenciaEscolarController : BaseController
{
    #region Parametros

    private readonly IOptions<UrlSettings> _appSettings;

    #endregion

    #region Constructor

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">Configurações de urls do sistema</param>
    public ControleFrequenciaEscolarController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Controle Frequencia Escolar
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Laudo, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
        var etapas = new SelectList(ApiClientFactory.Instance.GetEtapasEnsinoAll(), "Id", "Nome");
        var response = ApiClientFactory.Instance.GetControlesFrequenciasEscolaresAll();

        return View(new ControleFrequenciaEscolarModel()
        {
            ListEstados = estados,
            ListEtapas = etapas,
            ControlesFrequenciasEscolares = response
        });
    }

    /// <summary>
    /// Ação de Pesquisa de Alunos
    /// </summary>
    /// <param name="collection">Coleção de dados para pesquisa da Frequencia Escolar</param>
    /// <returns>Retorna a partial view da tabela com os alunos</returns>
    [HttpPost]
    public async Task<ActionResult> PesquisarTabela(IFormCollection collection)
    {
        var filter = new AlunosFilterDto()
        {
            SerieId = collection["ddlTurma"].ToString(),
            LocalidadeId = collection["ddlLocalidade"].ToString(),
        };

        var result = await ApiClientFactory.Instance.GetAlunosByFilter(filter);
        var alunos = result.Alunos;
        alunos = alunos.OrderBy(a => a.Nome.Split('-').Last().Trim()).ToList();

        var disciplinas = new SelectList(ApiClientFactory.Instance.GetDisciplinasAll(), "Id", "Nome");

        var profissionais = new SelectList(new List<object>(), "Id", "Nome");

        if (!filter.LocalidadeId.IsNullOrEmpty())
        {
            var listaProfissionais = ApiClientFactory.Instance
                .GetProfissionaisByLocalidade(Convert.ToInt32(filter.LocalidadeId));

            profissionais = new SelectList((IEnumerable)listaProfissionais ?? new List<object>(), "Id", "Nome");
        }

        var presencas = new List<ControleFrequenciaEscolarDto>();

        foreach (var aluno in alunos)
        {
            var freq = ApiClientFactory.Instance.GetControlesFrequenciasEscolaresByAlunoId(aluno.Id);

            if (freq != null && freq.Any())
            {
                foreach (var registro in freq)
                {
                    presencas.Add(new ControleFrequenciaEscolarDto
                    {
                        Id = registro.Id,
                        AlunoId = aluno.Id.ToString(),
                        DataFrequencia = registro.DataFrequencia,
                        Controle = registro.Controle,
                        DisciplinaId = registro.DisciplinaId
                    });
                }
            }
        }

        var tabelaFrequenciaEscolar = new TabelaFrequenciasEscolares
        {
            Alunos = alunos,
            AlunosComPresencas = presencas,
            ListDisciplinas = disciplinas,
            ListProfissionais = profissionais
        };

        return PartialView("_TabelaFrequenciaEscolar", tabelaFrequenciaEscolar);
    }

    /// <summary>
    /// Ação de criação de registro de Frequências
    /// </summary>
    /// <param name="collection">Coleção de dados para criação dos registros da Frequencia Escolar</param>
    /// <returns>Retorna mensagem de sucesso ou erro da ação</returns>
    [HttpPost]
    public async Task<IActionResult> SalvarFrequencias(IFormCollection collection)
    {
        try
        {
            var listPresença =
                (from item in collection where item.Key.Contains("presenca") select item.Value)
                .Select(v => (string)v).ToList();

            //foreach (var freq in frequencias)
            //{
            //    if (!DateTime.TryParse(freq.DataFrequencia, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataFrequencia))
            //        continue;

            //    var freqExistentes = ApiClientFactory.Instance
            //        .GetControlesFrequenciasEscolaresByAlunoId(Convert.ToInt32(freq.AlunoId)) ?? new List<ControleFrequenciaEscolarDto>();

            //    var existente = freqExistentes
            //        .FirstOrDefault(f =>
            //            f.SerieId == freq.SerieId &&
            //            f.DisciplinaId == freq.DisciplinaId &&
            //            DateTime.Parse(f.DataFrequencia).Date == dataFrequencia.Date
            //        );

            //    if (existente == null)
            //    {
            //        var command = new ControleFrequenciaEscolarModel.CreateUpdateControleFrequenciaEscolarCommand()
            //        {
            //            Controle = freq.Controle,
            //            DisciplinaId = freq.DisciplinaId,
            //            AlunoId = freq.AlunoId,
            //            SerieId = freq.SerieId,
            //            ProfissionalId = freq.ProfissionalId,
            //            DataFrequencia = dataFrequencia.ToString("yyyy-MM-dd")
            //        };
            //        await ApiClientFactory.Instance.CreateControleFrequenciaEscolar(command);
            //    }
            //    else if (existente.Controle != freq.Controle)
            //    {
            //        var command = new ControleFrequenciaEscolarModel.CreateUpdateControleFrequenciaEscolarCommand()
            //        {
            //            Id = (int)existente.Id,
            //            Controle = freq.Controle,
            //            DisciplinaId = existente.DisciplinaId,
            //            AlunoId = existente.AlunoId,
            //            SerieId = existente.SerieId,
            //            ProfissionalId = existente.ProfissionalId,
            //            DataFrequencia = dataFrequencia.ToString("yyyy-MM-dd")
            //        };
            //        await ApiClientFactory.Instance.UpdateControleFrequenciaEscolar(command.Id, command);
            //    }
            //}

            return Json(new { success = true, message = "Frequências salvas com sucesso!" });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = "Erro ao salvar frequências." });
        }
    }

    #endregion

    #region Get Methods

    /// <summary>
    /// Método de busca de Controle de Frequencia Escolar por id
    /// </summary>
    /// <param name="id">Id do Frequencia Escolar </param>
    /// <returns>Retorna o objeto Controle de Frequencia Escolar</returns>
    [ClaimsAuthorize(ClaimType.Laudo, Identity.Claim.Consultar)]
    public Task<ControleFrequenciaEscolarDto> GetControleFrequenciaEscolarById(int id)
    {
        var result = ApiClientFactory.Instance.GetControleFrequenciaEscolarById(id);

        return Task.FromResult(result);
    }
    #endregion
}
