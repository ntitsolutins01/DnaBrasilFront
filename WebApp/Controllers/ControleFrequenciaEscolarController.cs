using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApp.Authorization;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;
using System.Linq;
using static WebApp.Models.ControleFrequenciaEscolarModel;
using log4net;

namespace WebApp.Controllers;

/// <summary>
/// Controle de Frequencia Escolar
/// </summary>
public class ControleFrequenciaEscolarController : BaseController
{
    #region Parametros

    private readonly ILog _logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">configurações de urls do sistema</param>
    /// <param name="logger">Log de mensagens da aplicação</param>
    public ControleFrequenciaEscolarController(IOptions<UrlSettings> appSettings,
        ILog logger)
    {
        ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
        _logger = logger;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Controle Frequencia Escolar
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.ControlePresenca, Identity.Claim.Consultar)]
    [HttpGet]
    public async Task<ActionResult> Index(int? crud, int? notify, string message = null)
    {
        try
        {

            _logger.Info($"Usuario Logado em ControleFrequenciaEscolar.Index User.Identity.Name : {User.Identity.Name}");

            var usuario = User.Identity.Name;

            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

            var searchFilter = new ControleFrequenciaEscolarFilterDto
            {
                MunicipioId = usu.MunicipioId.ToString(),
                LocalidadeId = usu.LocalidadeId
            };

            //var response = await ApiClientFactory.Instance.GetControlesFrequenciasEscolaresByFilter(searchFilter);
            var responseFrequenciasEscolares = ApiClientFactory.Instance.GetControlesFrequenciasEscolaresAll();

            var fomento = ApiClientFactory.Instance.GetFomentoByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));

            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", usu.Uf);

            SelectList municipios = null;

            if (!string.IsNullOrEmpty(usu.Uf))
            {
                municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByFomentoId(fomento.Id), "Id", "Nome", usu.MunicipioId);
            }

            SelectList localidades = null;

            if (usu.MunicipioId != null)
            {
                var resultLocalidades = ApiClientFactory.Instance.GetLocalidadeByMunicipioId(usu.MunicipioId.ToString());

                if (resultLocalidades != null)
                    localidades = new SelectList(resultLocalidades, "Id", "Nome", usu.LocalidadeId);
            }

            var etapas = new SelectList(ApiClientFactory.Instance.GetEtapasEnsinoAll(), "Id", "Nome");
            var disciplinas = new SelectList(ApiClientFactory.Instance.GetDisciplinasAll(), "Id", "Nome");

            return View(new ControleFrequenciaEscolarModel()
            {
                ListEstados = estados,
                ListMunicipios = municipios,
                ListLocalidades = localidades,
                ListEtapas = etapas,
                ListDisciplinas = disciplinas,
                SearchFilter = searchFilter,
                IdPerfil = usu.Perfil.Id,
                ControlesFrequenciasEscolares = responseFrequenciasEscolares//response.FrequenciasEscolares
            });

        }
        catch (Exception e)
        {
            _logger.Error($"ControleFrequenciaEscolar.Index: {e.StackTrace}");
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Pesquisa de Alunos
    /// </summary>
    /// <param name="collection">Coleção de dados para pesquisa da Frequencia Escolar</param>
    /// <returns>Retorna a partial view da tabela com os alunos</returns>
    [HttpPost]
    public async Task<ActionResult> Index(IFormCollection collection)
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
            var freq = ApiClientFactory.Instance.GetControlesFrequenciasEscolaresByAlunoId(aluno.Id)
                       ?? new List<ControleFrequenciaEscolarDto>();

            if (int.TryParse(collection["ddlDisciplina"].ToString().Trim(), out var disciplinaId))
            {
                freq = freq.Where(f => f.DisciplinaId == disciplinaId.ToString()).ToList();
            }

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
            ListProfissionais = profissionais,
            TurmaId = collection["ddlTurma"].ToString(),
            LocalidadeId = collection["ddlLocalidade"].ToString(),
            DisciplinaId = collection["ddlDisciplina"].ToString()
        };

        return PartialView("_TabelaFrequenciaEscolar", tabelaFrequenciaEscolar);
    }

    /// <summary>
    /// Ação de criação de registro de Frequências
    /// </summary>
    /// <param name="collection">Coleção de dados para criação dos registros da Frequencia Escolar</param>
    /// <returns>Retorna mensagem de sucesso ou erro da ação</returns>
    [HttpPost]
    public async Task<IActionResult> Create(IFormCollection collection)
    {
        try
        {
            var listFaltas = collection
                .Where(item => item.Key.Contains("#"))
                .Select(item => item.Key.Split("#")[1]).ToList();

            var filter = new AlunosFilterDto()
            {
                SerieId = collection["turma"].ToString(),
                LocalidadeId = collection["localidade"].ToString()
            };

            var alunosFilter = await ApiClientFactory.Instance.GetAlunosByFilter(filter);

            var alunos = alunosFilter?.Alunos.ToList();

            var diaHoje = DateTime.Now;

            if (alunos != null && alunos.Any())
            {
                foreach (var aluno in alunos)
                {
                    var existentes = ApiClientFactory.Instance
                        .GetControlesFrequenciasEscolaresByAlunoMesAno(aluno.Id, diaHoje.Month, diaHoje.Year);

                    var presencaHoje = existentes.FirstOrDefault(e =>
                        DateTimeOffset.TryParse(e.DataFrequencia, out var data) &&
                        data.Date == diaHoje.Date && e.DisciplinaId == collection["disciplina"].ToString() &&
                        e.SerieId == collection["turma"].ToString());

                    var controle = "P";
                    if (listFaltas.Contains(aluno.Id.ToString()))
                    {
                        controle = "F";
                    }

                    if (presencaHoje != null)
                    {
                        var command = new ControleFrequenciaEscolarModel.CreateUpdateControleFrequenciaEscolarCommand()
                        {
                            Id = presencaHoje.Id,
                            Controle = controle,
                            DisciplinaId = presencaHoje.DisciplinaId,
                            AlunoId = presencaHoje.AlunoId,
                            SerieId = presencaHoje.SerieId,
                            ProfissionalId = presencaHoje.ProfissionalId,
                            DataFrequencia = presencaHoje.DataFrequencia
                        };

                        await ApiClientFactory.Instance.UpdateControleFrequenciaEscolar(presencaHoje.Id, command);
                    }
                    else
                    {
                        var command = new ControleFrequenciaEscolarModel.CreateUpdateControleFrequenciaEscolarCommand()
                        {
                            Controle = controle,
                            DisciplinaId = collection["disciplina"].ToString(),
                            AlunoId = aluno.Id.ToString(),
                            SerieId = collection["turma"].ToString(),
                            ProfissionalId = collection["ddlProfissional"].ToString(),
                            DataFrequencia = DateTime
                                .Parse(collection["data"])
                                .ToString("yyyy-MM-dd")
                        };

                        await ApiClientFactory.Instance.CreateControleFrequenciaEscolar(command);
                    }
                }
            }

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Alteração da Frquência Escolar
    /// </summary>
    /// <param name="collection">coleção de dados para Alteração das Frquência Escolar</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Laudo, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var material =
                ApiClientFactory.Instance.GetMaterialById(Convert.ToInt32(collection["editMaterialId"]));

            var command = new MaterialModel.CreateUpdateMaterialCommand
            {
                Id = Convert.ToInt32(collection["editMaterialId"]),
                UnidadeMedida = collection["ddlUnidadeMedida"].ToString(),
                Descricao = collection["descricao"].ToString()
            };

            await ApiClientFactory.Instance.UpdateMaterial(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
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
