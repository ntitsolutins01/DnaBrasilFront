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
             var responseFrequenciasEscolares =  ApiClientFactory.Instance.GetControlesFrequenciasEscolaresAll();

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

            return View(new ControleFrequenciaEscolarModel()
            {
                ListEstados = estados,
                ListMunicipios = municipios,
                ListLocalidades = localidades,
                ListEtapas = etapas,
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
    public async Task<IActionResult> Create(IFormCollection collection)
    {
        try
        {
            var listFalta =
                (from item in collection where item.Key.Contains("falta") select item.Key)
                .Select(v => (string)v).ToList();

            var command = new ControleFrequenciaEscolarModel.CreateUpdateControleFrequenciaEscolarCommand()
            {
                ListFaltas = listFalta,
                DisciplinaId = collection["ddlDisciplina"].ToString(),
                SerieId = collection["ddlLocalidade"].ToString(),
                ProfissionalId = collection["ddlProfissional"].ToString(),
                DataFrequencia = DateTime.Now.ToString("dd/MM/yyyy") //na api fica assim: DtNascimento = DateTime.ParseExact(request.DtNascimento, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("pt-BR")),
            };


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
