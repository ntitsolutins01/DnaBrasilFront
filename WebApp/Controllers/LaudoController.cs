using Azure;
using ClosedXML.Excel;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using WebApp.Authorization;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;
using WebApp.Views;
using Claim = WebApp.Identity.Claim;

namespace WebApp.Controllers
{
    //[Authorize(Policy = ModuloAccess.Laudo)]
    public class LaudoController : BaseController
    {
        #region Parametros

        private readonly IWebHostEnvironment _host;
        private readonly ILog _logger;

        #endregion

        #region Constructor
        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        /// <param name="host">informações da aplicação em execução</param>
        /// <param name="logger"></param>
        public LaudoController(IOptions<UrlSettings> appSettings,
            IWebHostEnvironment host,
            ILog logger)
        {
            _host = host;
            _logger = logger;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
        }
        #endregion

        #region Main Methods
        [ClaimsAuthorize(ClaimType.Laudo, Claim.Consultar)]
        [HttpGet]
        public async Task<IActionResult> Index(int? crud, int? notify, string message = null)
        {
            try
            {
                _logger.Info($"Usuario Logado: {User.Identity.Name}");

                var usuario = User.Identity.Name;

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

                var fomento = ApiClientFactory.Instance.GetFomentoByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));
                var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome", fomento.Id);

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

                SelectList alunos = null;

                if (usu.LocalidadeId != null)
                {
                    var resultAlunos = ApiClientFactory.Instance.GetAlunosByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));

                    alunos = new SelectList(resultAlunos, "Id", "Nome");

                }

                var searchFilter = new LaudosFilterDto
                {
                    MunicipioId = usu.MunicipioId.ToString(),
                    LocalidadeId = usu.LocalidadeId,
#if DEBUG
                    PageSize = 300
#else
                    PageSize = 10000
#endif
                };

                var response = await ApiClientFactory.Instance.GetLaudosByFilter(searchFilter);
                var profissionais = new SelectList(ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(searchFilter.LocalidadeId)), "Id", "Nome");

                var model = new LaudoModel()
                {
                    Laudos = response.Laudos,
                    ListFomentos = fomentos,
                    ListEstados = estados,
                    ListMunicipios = municipios!,
                    ListLocalidades = localidades!,
                    ListAlunos = alunos,
                    ListProfissionais = profissionais,
                    SearchFilter = searchFilter,
                    IdPerfil = usu.Perfil.Id
                };

                return View(model);
            }
            catch (Exception e)
            {
                _logger.Error($"Laudo.Index: {e.StackTrace}");
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error",
                    message = e.Message,
                    stackTrace = e.StackTrace
                });
            }
        }

        [ClaimsAuthorize(ClaimType.Laudo, Claim.Consultar)]
        [HttpPost]
        public async Task<IActionResult> Index(int? crud, int? notify, IFormCollection collection, string message = null)
        {
            try
            {
                var usuario = User.Identity.Name;
                var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                var possuiFoto = collection["possuiFoto"].ToString();
                var finalizado = collection["finalizado"].ToString();

                var searchFilter = new LaudosFilterDto()
                {

                    MunicipioId = collection["ddlMunicipio"].ToString(),
                    LocalidadeId = collection["ddlLocalidade"].ToString(),
                    AlunoId = collection["ddlAluno"].ToString(),
                    PossuiFoto = possuiFoto != "",
                    Finalizado = finalizado != "",
                    PageNumber = 1,
#if DEBUG
                    PageSize = 300
#else
                    PageSize = 10000
#endif
                };

                var response = await ApiClientFactory.Instance.GetLaudosByFilter(searchFilter);

                var fomento = ApiClientFactory.Instance.GetFomentoByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));
                var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome", searchFilter.FomentoId);

                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", searchFilter.Estado);

                SelectList municipios = null;

                if (!string.IsNullOrEmpty(searchFilter.Estado))
                {
                    municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByFomentoId(fomento.Id), "Id", "Nome", searchFilter.MunicipioId);
                }

                SelectList localidades = null;

                if (usu.MunicipioId != null)
                {
                    var resultLocalidades = ApiClientFactory.Instance.GetLocalidadeByMunicipioId(searchFilter.MunicipioId.ToString());

                    if (resultLocalidades != null)
                        localidades = new SelectList(resultLocalidades, "Id", "Nome", searchFilter.LocalidadeId);
                }

                SelectList alunos = null;

                if (usu.LocalidadeId != null)
                {
                    var resultAlunos = ApiClientFactory.Instance.GetAlunosByLocalidadeId(Convert.ToInt32(searchFilter.LocalidadeId));

                    alunos = new SelectList(resultAlunos, "Id", "Nome", searchFilter.AlunoId);

                }

                var model = new LaudoModel()
                {
                    Laudos = response.Laudos,
                    ListFomentos = fomentos,
                    ListEstados = estados,
                    ListMunicipios = municipios!,
                    ListLocalidades = localidades!,
                    ListAlunos = alunos,
                    SearchFilter = searchFilter,
                    IdPerfil = usu.Perfil.Id
                };

                return View(model);
            }
            catch (Exception e)
            {
                _logger.Error(e.StackTrace);
                return RedirectToAction(nameof(Error), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        [ClaimsAuthorize(ClaimType.Laudo, Claim.Detalhar)]
        public async Task<ActionResult> Details(int id)
        {
            var laudo = ApiClientFactory.Instance.GetLaudoById(id);

            var consumoAlimentar = laudo.ConsumoAlimentarId == null ? null : ApiClientFactory.Instance.GetConsumoAlimentarById((int)laudo.ConsumoAlimentarId);
            var saudeBucal = laudo.SaudeBucalId == null ? null : ApiClientFactory.Instance.GetConsumoAlimentarById((int)laudo.SaudeBucalId);
            var educacionalMatematica = laudo.EducacionalMatematicaId == null ? null : ApiClientFactory.Instance.GetConsumoAlimentarById((int)laudo.EducacionalMatematicaId);
            var educacionalPortugues = laudo.EducacionalPortuguesId == null ? null : ApiClientFactory.Instance.GetConsumoAlimentarById((int)laudo.EducacionalPortuguesId);

            var aluno = await ApiClientFactory.Instance.GetAlunoById((int)laudo.AlunoId);
            var profissional = laudo.ProfissionalId == null ? null : ApiClientFactory.Instance.GetProfissionalById(Convert.ToInt32(aluno.ProfissionalId));
            var talentoEsportivo = laudo.TalentoEsportivoId == null ? null : ApiClientFactory.Instance.GetTalentoEsportivoById((int)laudo.TalentoEsportivoId!);
            var encaminhamentoImc = laudo.SaudeId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoBySaudeId(Convert.ToInt32(laudo.SaudeId));
            var qualidadeDeVida = laudo.QualidadeDeVidaId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoByQualidadeDeVidaId((int)laudo.QualidadeDeVidaId);
            var vocacional = laudo.VocacionalId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoByVocacional();
            var encaminhamentoConsumoAlimentar = laudo.ConsumoAlimentarId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)consumoAlimentar.Encaminhamento.Id);
            var encaminhamentoSaudeBucal = laudo.SaudeBucalId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)saudeBucal.Encaminhamento.Id);
            var encaminhamentoMatematica = laudo.EducacionalMatematicaId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)educacionalMatematica.Encaminhamento.Id);
            var encaminhamentoPortugues = laudo.EducacionalPortuguesId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)educacionalPortugues.Encaminhamento.Id);
            var desempenho = ApiClientFactory.Instance.GetDesempenhoByAluno(Convert.ToInt32(laudo.AlunoId));
            var modalidade = laudo.ModalidadeId == null ? null : ApiClientFactory.Instance.GetModalidadeById((int)laudo.ModalidadeId);

            var model = new LaudoModel()
            {
                Laudo = laudo,
                Aluno = aluno,
                Profissional = profissional,
                TalentoEsportivo = talentoEsportivo,
                EncaminhamentoImc = encaminhamentoImc,
                ListQualidadeDeVida = qualidadeDeVida,
                ListVocacional = vocacional,
                EncaminhamentoSaudeBucal = encaminhamentoSaudeBucal,
                EncaminhamentoConsumoAlimentar = encaminhamentoConsumoAlimentar,
                EncaminhamentoMatematica = encaminhamentoMatematica,
                EncaminhamentoPortugues = encaminhamentoPortugues,
                Desempenho = desempenho,
                Modalidade = modalidade
            };
            return View(model);
        }

        public async Task<ActionResult> Report(int id)
        {
            var laudo = ApiClientFactory.Instance.GetLaudoById(id);

            var consumoAlimentar = laudo.ConsumoAlimentarId == null ? null : ApiClientFactory.Instance.GetConsumoAlimentarById((int)laudo.ConsumoAlimentarId);
            var saudeBucal = laudo.SaudeBucalId == null ? null : ApiClientFactory.Instance.GetSaudeBucalById((int)laudo.SaudeBucalId);
            var educacionalMatematica = laudo.EducacionalMatematicaId == null ? null : ApiClientFactory.Instance.GetConsumoAlimentarById((int)laudo.EducacionalMatematicaId);
            var educacionalPortugues = laudo.EducacionalPortuguesId == null ? null : ApiClientFactory.Instance.GetConsumoAlimentarById((int)laudo.EducacionalPortuguesId);

            var aluno = await ApiClientFactory.Instance.GetAlunoById((int)laudo.AlunoId);
            var profissional = laudo.ProfissionalId == null ? null : ApiClientFactory.Instance.GetProfissionalById(Convert.ToInt32(aluno.ProfissionalId));
            var talentoEsportivo = laudo.TalentoEsportivoId == null ? null : ApiClientFactory.Instance.GetTalentoEsportivoById((int)laudo.TalentoEsportivoId!);
            var encaminhamentoImc = laudo.SaudeId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoBySaudeId(Convert.ToInt32(laudo.SaudeId));
            var qualidadeDeVida = laudo.QualidadeDeVidaId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoByQualidadeDeVidaId((int)laudo.QualidadeDeVidaId);
            var vocacional = laudo.VocacionalId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoByVocacional();
            var encaminhamentoConsumoAlimentar = laudo.ConsumoAlimentarId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)consumoAlimentar.Encaminhamento.Id);
            var encaminhamentoSaudeBucal = laudo.SaudeBucalId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)saudeBucal.Encaminhamento.Id);
            var encaminhamentoMatematica = laudo.EducacionalMatematicaId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)educacionalMatematica.Encaminhamento.Id);
            var encaminhamentoPortugues = laudo.EducacionalPortuguesId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)educacionalPortugues.Encaminhamento.Id);
            var desempenho = ApiClientFactory.Instance.GetDesempenhoByAluno(Convert.ToInt32(laudo.AlunoId));
            var modalidade = laudo.ModalidadeId == null ? null : ApiClientFactory.Instance.GetModalidadeById((int)laudo.ModalidadeId);

            var percentual = new PercentualLaudoDto();

            //if (laudo.Ordem != 1)
            //{
            //    var laudoAnterior = await ApiClientFactory.Instance.GetLaudosByFilter(new LaudosFilterDto
            //    {
            //        AlunoId = laudo.AlunoId.ToString(),
            //        Ordem = laudo.Ordem - 1,
            //        PageNumber = 1,
            //        PageSize = 10
            //    });

            //    var talentoEsportivoAnterior = ApiClientFactory.Instance.GetTalentoEsportivoById((int)laudoAnterior.Laudos.Items.First().TalentoEsportivoId);

            //    percentual.PreensaoManual = (decimal)((talentoEsportivoAnterior.PreensaoManual - talentoEsportivo.PreensaoManual) / talentoEsportivoAnterior.PreensaoManual * 100);
            //    percentual.Flexibilidade = (decimal)((talentoEsportivoAnterior.Flexibilidade - talentoEsportivo.Flexibilidade) / talentoEsportivoAnterior.Flexibilidade * 100);
            //    percentual.ImpulsaoHorizontal = (decimal)((talentoEsportivoAnterior.ImpulsaoHorizontal - talentoEsportivo.ImpulsaoHorizontal) / talentoEsportivoAnterior.ImpulsaoHorizontal * 100);
            //    percentual.Velocidade = (decimal)((talentoEsportivoAnterior.Velocidade - talentoEsportivo.Velocidade) / talentoEsportivoAnterior.Velocidade * 100);
            //    percentual.AptidaoFisica = (decimal)((talentoEsportivoAnterior.Vo2Max - talentoEsportivo.Vo2Max) / talentoEsportivoAnterior.Vo2Max * 100);
            //    percentual.Agilidade = (decimal)((talentoEsportivoAnterior.ShuttleRun - talentoEsportivo.ShuttleRun) / talentoEsportivoAnterior.ShuttleRun * 100);
            //    percentual.Imc = (decimal)((talentoEsportivoAnterior.Imc - talentoEsportivo.Imc) / talentoEsportivoAnterior.Imc * 100);


            //}

            var tiposLaudos = ApiClientFactory.Instance.GetTiposLaudoAll();

            var model = new LaudoModel()
            {
                Laudo = laudo,
                Profissional = profissional,
                TalentoEsportivo = talentoEsportivo,
                EncaminhamentoImc = encaminhamentoImc,
                ListQualidadeDeVida = qualidadeDeVida,
                ListVocacional = vocacional,
                EncaminhamentoSaudeBucal = encaminhamentoSaudeBucal,
                EncaminhamentoConsumoAlimentar = encaminhamentoConsumoAlimentar,
                EncaminhamentoMatematica = encaminhamentoMatematica,
                EncaminhamentoPortugues = encaminhamentoPortugues,
                Desempenho = desempenho,
                Modalidade = modalidade,
                Percentual = percentual,
                TipoLaudoQualidadeVidaDescricao = tiposLaudos.First(x => x.Id == (int)EnumTipoLaudo.QualidadeVida).Descricao,
                TipoLaudoConsumoAlimentarDescricao = tiposLaudos.First(x => x.Id == (int)EnumTipoLaudo.ConsumoAlimentar).Descricao,
                TipoLaudoSaudeBucalDescricao = tiposLaudos.First(x => x.Id == (int)EnumTipoLaudo.SaudeBucal).Descricao,
                TipoLaudoVocacionalDescricao = tiposLaudos.First(x => x.Id == (int)EnumTipoLaudo.Vocacional).Descricao,
                TipoLaudoEducacionalDescricao = tiposLaudos.First(x => x.Id == (int)EnumTipoLaudo.Educacional3MT).Descricao
            };
            return View(model);
        }

        [ClaimsAuthorize(ClaimType.Laudo, Claim.Incluir)]
        public async Task<ActionResult> Create(int? crud, int? notify, bool? acessoAluno = false, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                var usuario = User.Identity.Name;

                UsuarioDto usu;
                AlunoDto aln = null;
                if (acessoAluno != null && (bool)acessoAluno)
                {
                    aln = await ApiClientFactory.Instance.GetAlunoById(Convert.ToInt32(usuario));
                    usu = await ApiClientFactory.Instance.GetUsuarioByEmail(aln.Email);
                }
                else
                {
                    usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);
                }


                var questionarioVocacional =
                ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.Vocacional).OrderBy(o => o.Questao).ToList();
                var questionarioQualidadeVida =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.QualidadeVida).OrderBy(o => o.Questao).ToList();
                var questionarioConsumoAlimentar =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.ConsumoAlimentar).OrderBy(o => o.Questao).ToList();
                var questionarioSaudeBucal =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.SaudeBucal).OrderBy(o => o.Questao).ToList();
                var questionarioEducacional3Lp =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.Educacional3LP).OrderBy(o => o.Questao).ToList();

                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", usu.Uf);

                SelectList municipios = null;

                if (!string.IsNullOrEmpty(usu.Uf))
                {
                    municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(usu.Uf), "Id", "Nome", usu.MunicipioId);
                }

                SelectList localidades = null;

                if (usu.MunicipioId != null)
                {
                    var resultLocalidades = ApiClientFactory.Instance.GetLocalidadeByMunicipioId(usu.MunicipioId.ToString());

                    localidades = new SelectList(resultLocalidades, "Id", "Nome", usu.LocalidadeId);
                }

                SelectList alunos = null;

                SelectList profissionais = null;

                if (usu.LocalidadeId != null)
                {
                    IEnumerable<AlunoIndexDto> resultAlunos;

                    if (acessoAluno != null && (bool)acessoAluno)
                    {
                        resultAlunos = ApiClientFactory.Instance
                            .GetAlunosByLocalidadeId(Convert.ToInt32(usu.LocalidadeId))
                            .Where(x => aln != null && x.Id == aln.Id);

                    }
                    else
                    {
                        resultAlunos = ApiClientFactory.Instance
                            .GetAlunosByLocalidadeId(Convert.ToInt32(usu.LocalidadeId))
                            .Where(x => x.PossuiLaudoFinalizado);
                    }


                    alunos = new SelectList(resultAlunos, "Id", "Nome");

                    var resultProfissionais =
                        ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId));

                    profissionais = new SelectList(resultProfissionais, "Id", "Nome");
                }

                var model = new LaudoModel()
                {
                    ListQuestionarioVocacional = questionarioVocacional,
                    ListQuestionarioQualidadeVida = questionarioQualidadeVida,
                    ListQuestionarioConsumoAlimentar = questionarioConsumoAlimentar,
                    ListQuestionarioSaudeBucal = questionarioSaudeBucal,
                    ListQuestionarioEducacional3Lp = questionarioEducacional3Lp,
                    ListEstados = estados,
                    ListMunicipios = municipios!,
                    ListLocalidades = localidades!,
                    ListAlunos = alunos!,
                    ListProfissionais = profissionais!,
                    IdPerfil = usu.Perfil.Id,
                    AcessoAluno = acessoAluno
                };

                return View(model);

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        [HttpPost]
        [ClaimsAuthorize(ClaimType.Laudo, Claim.Incluir)]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                if (string.IsNullOrEmpty(collection["ddlAluno"].ToString()))
                {
                    return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Error, message = "Favor Informar o Aluno." });
                }

                int ordem;

                var existLaudo = ApiClientFactory.Instance.GetLaudoByAluno(Convert.ToInt32(collection["ddlAluno"].ToString()));

                ordem = existLaudo == null ? 1 : (int)(existLaudo.Ordem + 1)!;

                var command = new LaudoModel.CreateUpdateLaudoCommand
                {
                    AlunoId = Convert.ToInt32(collection["ddlAluno"].ToString()),
                    Ordem = ordem
                };

                var listVocacional = (from item in collection where item.Key.Contains("nomeRespVocacional") select item.Value).Select(v => (string)v).ToList();

                var listQualidadeDeVida = (from item in collection where item.Key.Contains("nomeRespQualidadeVida") select item.Value).Select(qv => (string)qv).ToList();

                var listConsumoAlimentar = (from item in collection where item.Key.Contains("nomeRespConsumoAlimentar") select item.Value).Select(ca => (string)ca).ToList();

                var listSaudeBucal = (from item in collection where item.Key.Contains("nomeRespSaudeBucal") select item.Value).Select(sb => (string)sb).ToList();

                if (listVocacional.Any())
                {
                    var totalRespVocacional = ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.Vocacional).Count;

                    if (listVocacional.Count != totalRespVocacional)
                    {
                        return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Error, message = "Favor responder todas as perguntas do questionário vocacional." });
                    }

                    command.VocacionalId = (int)await ApiClientFactory.Instance.CreateVocacional(
                        new VocacionalModel.CreateUpdateVocacionalCommand()
                        {
                            Respostas = string.Join(",", listVocacional),
                            ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                            AlunoId = Convert.ToInt32(collection["ddlAluno"].ToString()),
                            StatusVocacional = listVocacional.Count == totalRespVocacional ? "F" : "A"
                        });
                }

                if (listQualidadeDeVida.Any())
                {
                    var totalRespQualidadeDeVida = ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.QualidadeVida).Count;

                    if (listQualidadeDeVida.Count != totalRespQualidadeDeVida)
                    {
                        return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Error, message = "Favor responder todas as perguntas do questionário de qualidade de vida." });
                    }

                    command.QualidadeDeVidaId = (int)await ApiClientFactory.Instance.CreateQualidadeVida(
                        new QualidadeVidaModel.CreateUpdateQualidadeVidaCommand()
                        {
                            Respostas = string.Join(",", listQualidadeDeVida),
                            ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                            AlunoId = Convert.ToInt32(collection["ddlAluno"].ToString()),
                            StatusQualidadeDeVida = listQualidadeDeVida.Count == totalRespQualidadeDeVida ? "F" : "A"
                        });
                }

                if (listConsumoAlimentar.Any())
                {
                    var totalRespConsumoAlimentar = ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.ConsumoAlimentar).Count;

                    if (listConsumoAlimentar.Count != totalRespConsumoAlimentar)
                    {
                        return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Error, message = "Favor responder todas as perguntas do questionário de consumo alimentar." });
                    }

                    command.ConsumoAlimentarId = (int)await ApiClientFactory.Instance.CreateConsumoAlimentar(
                        new ConsumoAlimentarModel.CreateUpdateConsumoAlimentarCommand()
                        {
                            Respostas = string.Join(",", listConsumoAlimentar),
                            ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                            AlunoId = Convert.ToInt32(collection["ddlAluno"].ToString()),
                            StatusConsumoAlimentar = listConsumoAlimentar.Count == totalRespConsumoAlimentar ? "F" : "A"
                        });
                }

                if (listSaudeBucal.Any())
                {
                    var totalRespSaudeBucal = ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.SaudeBucal).Count;

                    if (listSaudeBucal.Count != totalRespSaudeBucal)
                    {
                        return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Error, message = "Favor responder todas as perguntas do questionário de saúde bucal." });
                    }

                    command.SaudeBucalId = (int)await ApiClientFactory.Instance.CreateSaudeBucal(
                        new SaudeBucalModel.CreateUpdateSaudeBucalCommand()
                        {
                            Respostas = string.Join(",", listSaudeBucal),
                            ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                            AlunoId = Convert.ToInt32(collection["ddlAluno"].ToString()),
                            StatusSaudeBucal = listSaudeBucal.Count == totalRespSaudeBucal ? "F" : "A"
                        });
                }

                var commandSaude = new SaudeModel.CreateUpdateSaudeCommand()
                {
                    ProfissionalId = collection["ddlProfissional"] == ""
                        ? null
                        : Convert.ToInt32(collection["ddlProfissional"].ToString()),
                    AlunoId = collection["ddlAluno"] == ""
                        ? null
                        : Convert.ToInt32(collection["ddlAluno"].ToString()),
                    EnvergaduraSaude = collection["envergaduraSaude"] == ""
                        ? null
                        : decimal.Parse(collection["envergaduraSaude"].ToString().Replace(".", ","),
                            System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    MassaCorporalSaude = collection["massaCorporalSaude"] == ""
                        ? null
                        : decimal.Parse(collection["massaCorporalSaude"].ToString().Replace(".", ","),
                            System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    AlturaSaude = collection["alturaSaude"] == ""
                        ? null
                        : decimal.Parse(collection["alturaSaude"].ToString().Replace(".", ","),
                            System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    StatusSaude = "F"
                };

                if (commandSaude.EnvergaduraSaude != null && commandSaude.MassaCorporalSaude != null && commandSaude.AlturaSaude != null)
                {
                    command.SaudeId = (int)await ApiClientFactory.Instance.CreateSaude(commandSaude);
                }
                else
                {
                    return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Error, message = "Favor informar todos os campos de Saúde." });
                }

                var commandTalentoEsportivo = new TalentoEsportivoModel.CreateUpdateTalentoEsportivoCommand()
                {
                    ProfissionalId = collection["ddlProfissional"] == "" ? null : Convert.ToInt32(collection["ddlProfissional"].ToString()),
                    AlunoId = collection["ddlAluno"] == "" ? null : Convert.ToInt32(collection["ddlAluno"].ToString()),
                    Altura = collection["altura"] == "" ? null : decimal.Parse(collection["altura"].ToString().Replace(".", ","),
                                                         System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    MassaCorporal = collection["massaCorporal"] == "" ? null : decimal.Parse(collection["massaCorporal"].ToString().Replace(".", ","),
                                                             System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    PreensaoManual = collection["preensaoManual"] == "" ? null : decimal.Parse(collection["preensaoManual"].ToString().Replace(".", ","),
                                                                 System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    Flexibilidade = collection["flexibilidade"] == "" ? null : decimal.Parse(collection["flexibilidade"].ToString().Replace(".", ","),
                                                            System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    ImpulsaoHorizontal = collection["impulsaoHorizontal"] == "" ? null : decimal.Parse(collection["impulsaoHorizontal"].ToString().Replace(".", ","),
                                                                      System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    Velocidade = collection["testeVelocidade"] == "" ? null : decimal.Parse(collection["testeVelocidade"].ToString().Replace(".", ","),
                                                           System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    AptidaoFisica = collection["aptidaoFisica"] == "" ? null : decimal.Parse(collection["aptidaoFisica"].ToString().Replace(".", ","),
                                                             System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    Agilidade = collection["agilidade"] == "" ? null : decimal.Parse(collection["agilidade"].ToString().Replace(".", ","),
                                                         System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    Abdominal = Convert.ToBoolean(collection["rdbAbdominal"]),
                    StatusTalentosEsportivos = "F"
                };

                if (commandTalentoEsportivo.Altura != null && commandTalentoEsportivo.MassaCorporal != null && commandTalentoEsportivo.PreensaoManual != null &&
                    commandTalentoEsportivo.Flexibilidade != null && commandTalentoEsportivo.ImpulsaoHorizontal != null && commandTalentoEsportivo.Velocidade != null &&
                    commandTalentoEsportivo.AptidaoFisica != null && commandTalentoEsportivo.Agilidade != null)
                {
                    command.TalentoEsportivoId = (int)await ApiClientFactory.Instance.CreateTalentoEsportivo(commandTalentoEsportivo);

                    var encaminhamento = ApiClientFactory.Instance.GetTalentoEsportivoById((int)command.TalentoEsportivoId)
                        .Encaminhamento;

                    var modalidade = ApiClientFactory.Instance.GetModalidadeAll()
                        .FirstOrDefault(x => encaminhamento.Nome.Contains(x.Nome));

                    command.ModalidadeId = modalidade!.Id;

                }
                //else
                //{
                //    return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Error, message = "Favor informar todos os campos de Talento Esportivo." });
                //}

                if (command.SaudeBucalId != null || command.ConsumoAlimentarId != null || command.QualidadeDeVidaId != null ||
                    command.SaudeId != null || command.TalentoEsportivoId != null || command.VocacionalId != null)
                {
                    await ApiClientFactory.Instance.CreateLaudo(command);
                }


                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = $"Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema.{e.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorize(ClaimType.Laudo, Claim.Alterar)]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                bool acessoAluno = Convert.ToBoolean(collection["hdnAluno"].ToString());

                var laudo = ApiClientFactory.Instance.GetLaudoById(id);

                var command = new LaudoModel.CreateUpdateLaudoCommand
                {
                    Id = id,
                    AlunoId = (int)laudo.AlunoId
                };

                var listSaudePane = (from item in collection where item.Key.Contains("SaudePane") select item.Value).Select(v => (string)v).ToList();

                var listTalentoEsportivoPane = (from item in collection where item.Key.Contains("TalentoEsportivoPane") select item.Value).Select(v => (string)v).ToList();

                var listVocacional = (from item in collection where item.Key.Contains("nomeRespVocacional") select item.Value).Select(v => (string)v).ToList();

                var listQualidadeDeVida = (from item in collection where item.Key.Contains("nomeRespQualidadeVida") select item.Value).Select(qv => (string)qv).ToList();

                var listConsumoAlimentar = (from item in collection where item.Key.Contains("nomeRespConsumoAlimentar") select item.Value).Select(ca => (string)ca).ToList();

                var listSaudeBucal = (from item in collection where item.Key.Contains("nomeRespSaudeBucal") select item.Value).Select(sb => (string)sb).ToList();

                if (listVocacional.Any())
                {
                    var totalRespVocacional = ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.Vocacional).Count;

                    if (listVocacional.Count != totalRespVocacional)
                    {
                        return RedirectToAction(nameof(Edit), new { notify = (int)EnumNotify.Error, message = "Favor responder todas as perguntas do questionário vocacional." });
                    }

                    if (laudo.VocacionalId != null)
                    {
                        await ApiClientFactory.Instance.UpdateVocacional((int)laudo.VocacionalId,
                            new VocacionalModel.CreateUpdateVocacionalCommand()
                            {
                                Id = (int)laudo.VocacionalId,
                                Respostas = string.Join(",", listVocacional),
                                ProfissionalId = (int)laudo.ProfissionalId,
                                AlunoId = (int)laudo.AlunoId,
                                StatusVocacional = listVocacional.Count == totalRespVocacional ? "F" : "A"
                            });
                        command.VocacionalId = laudo.VocacionalId;
                    }
                    else
                    {
                        command.VocacionalId = (int)await ApiClientFactory.Instance.CreateVocacional(
                            new VocacionalModel.CreateUpdateVocacionalCommand()
                            {
                                Respostas = string.Join(",", listVocacional),
                                ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                                AlunoId = (int)laudo.AlunoId,
                                StatusVocacional = listVocacional.Count == totalRespVocacional ? "F" : "A"
                            });
                    }
                }

                if (listQualidadeDeVida.Any())
                {
                    var totalRespQualidadeDeVida = ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.QualidadeVida).Count;

                    if (listQualidadeDeVida.Count != totalRespQualidadeDeVida)
                    {
                        return RedirectToAction(nameof(Edit), new { notify = (int)EnumNotify.Error, message = "Favor responder todas as perguntas do questionário de qualidade de vida." });
                    }

                    if (laudo.QualidadeDeVidaId != null)
                    {
                        await ApiClientFactory.Instance.UpdateQualidadeVida((int)laudo.QualidadeDeVidaId,
                            new QualidadeVidaModel.CreateUpdateQualidadeVidaCommand()
                            {
                                Id = (int)laudo.QualidadeDeVidaId,
                                Respostas = string.Join(",", listQualidadeDeVida),
                                ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                                AlunoId = (int)laudo.AlunoId,
                                StatusQualidadeDeVida =
                                    listQualidadeDeVida.Count == totalRespQualidadeDeVida ? "F" : "A"
                            });
                        command.QualidadeDeVidaId = laudo.QualidadeDeVidaId;
                    }
                    else
                    {
                        command.QualidadeDeVidaId = (int)await ApiClientFactory.Instance.CreateQualidadeVida(
                            new QualidadeVidaModel.CreateUpdateQualidadeVidaCommand()
                            {
                                Respostas = string.Join(",", listQualidadeDeVida),
                                ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                                AlunoId = (int)laudo.AlunoId,
                                StatusQualidadeDeVida =
                                    listQualidadeDeVida.Count == totalRespQualidadeDeVida ? "F" : "A"
                            });
                    }
                }

                if (listConsumoAlimentar.Any())
                {
                    var totalRespConsumoAlimentar = ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.ConsumoAlimentar).Count;

                    if (listConsumoAlimentar.Count != totalRespConsumoAlimentar)
                    {
                        return RedirectToAction(nameof(Edit), new { notify = (int)EnumNotify.Error, message = "Favor responder todas as perguntas do questionário de consumo alimentar." });
                    }

                    if (laudo.ConsumoAlimentarId != null)
                    {
                        await ApiClientFactory.Instance.UpdateConsumoAlimentar((int)laudo.ConsumoAlimentarId,
                            new ConsumoAlimentarModel.CreateUpdateConsumoAlimentarCommand()
                            {
                                Id = (int)laudo.ConsumoAlimentarId,
                                Respostas = string.Join(",", listConsumoAlimentar),
                                ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                                AlunoId = (int)laudo.AlunoId,
                                StatusConsumoAlimentar = listConsumoAlimentar.Count == totalRespConsumoAlimentar ? "F" : "A"
                            });
                        command.ConsumoAlimentarId = laudo.ConsumoAlimentarId;
                    }
                    else
                    {
                        command.ConsumoAlimentarId = (int)await ApiClientFactory.Instance.CreateConsumoAlimentar(
                            new ConsumoAlimentarModel.CreateUpdateConsumoAlimentarCommand()
                            {
                                Respostas = string.Join(",", listConsumoAlimentar),
                                ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                                AlunoId = (int)laudo.AlunoId,
                                StatusConsumoAlimentar = listConsumoAlimentar.Count == totalRespConsumoAlimentar ? "F" : "A"
                            });

                    }
                }

                if (listSaudeBucal.Any())
                {
                    var totalRespSaudeBucal = ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.SaudeBucal).Count;

                    if (listSaudeBucal.Count != totalRespSaudeBucal)
                    {
                        return RedirectToAction(nameof(Edit), new { notify = (int)EnumNotify.Error, message = "Favor responder todas as perguntas do questionário de saúde bucal." });
                    }

                    if (laudo.SaudeBucalId != null)
                    {
                        await ApiClientFactory.Instance.UpdateSaudeBucal((int)laudo.SaudeBucalId,
                            new SaudeBucalModel.CreateUpdateSaudeBucalCommand()
                            {
                                Id = (int)laudo.SaudeBucalId,
                                Respostas = string.Join(",", listSaudeBucal),
                                ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                                AlunoId = (int)laudo.AlunoId,
                                StatusSaudeBucal = listSaudeBucal.Count == totalRespSaudeBucal ? "F" : "A"
                            });
                        command.SaudeBucalId = laudo.SaudeBucalId;
                    }
                    else
                    {
                        command.SaudeBucalId = (int)await ApiClientFactory.Instance.CreateSaudeBucal(
                            new SaudeBucalModel.CreateUpdateSaudeBucalCommand()
                            {
                                Respostas = string.Join(",", listSaudeBucal),
                                ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                                AlunoId = (int)laudo.AlunoId,
                                StatusSaudeBucal = listSaudeBucal.Count == totalRespSaudeBucal ? "F" : "A"
                            });

                    }
                }

                if (laudo.SaudeId != null)
                {
                    if (listSaudePane.Any())
                    {
                        var saude = ApiClientFactory.Instance.GetSaudeById((int)laudo.SaudeId);

                        var commandSaude = new SaudeModel.CreateUpdateSaudeCommand()
                        {
                            Id = (int)laudo.SaudeId,
                            ProfissionalId = saude.ProfissionalId,
                            AlunoId = (int)laudo.AlunoId,
                            EnvergaduraSaude = collection["envergaduraSaudePane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["envergaduraSaudePane"].ToString()),
                            MassaCorporalSaude = collection["massaCorporalSaudePane"] == ""
                                ? null
                                : decimal.Parse(collection["massaCorporalSaudePane"].ToString().Replace(".", ","),
                                    System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                            AlturaSaude = collection["alturaSaudePane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["alturaSaudePane"].ToString()),
                            StatusSaude = "F"
                        };

                        await ApiClientFactory.Instance.UpdateSaude((int)laudo.SaudeId, commandSaude);

                        command.SaudeId = laudo.SaudeId;
                    }
                }
                else
                {
                    if (listSaudePane.Any())
                    {
                        var commandSaude = new SaudeModel.CreateUpdateSaudeCommand()
                        {
                            ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                            AlunoId = (int)laudo.AlunoId,
                            EnvergaduraSaude = collection["envergaduraSaudePane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["envergaduraSaudePane"].ToString()),
                            MassaCorporalSaude = collection["massaCorporalSaudePane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["massaCorporalSaudePane"].ToString()),
                            AlturaSaude = collection["alturaSaudePane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["alturaSaudePane"].ToString()),
                            StatusSaude = "F"
                        };

                        if (!acessoAluno)
                        {
                            if (commandSaude.EnvergaduraSaude == 0 && commandSaude.MassaCorporalSaude == 0 &&
                                commandSaude.AlturaSaude == 0)
                            {
                                command.SaudeId = (int)await ApiClientFactory.Instance.CreateSaude(commandSaude);
                            }
                        }
                    }
                }

                if (laudo.TalentoEsportivoId != null)
                {
                    if (listTalentoEsportivoPane.Any())
                    {
                        var talentoEsportivo =
                            ApiClientFactory.Instance.GetTalentoEsportivoById((int)laudo.TalentoEsportivoId);

                        var commandTalentoEsportivo = new TalentoEsportivoModel.CreateUpdateTalentoEsportivoCommand()
                        {
                            Id = (int)laudo.TalentoEsportivoId,
                            ProfissionalId = talentoEsportivo.ProfissionalId,
                            AlunoId = (int)laudo.AlunoId,
                            Altura = collection["alturaTalentoEsportivoPane"] == ""
                                ? null
                                : decimal.Parse(collection["alturaTalentoEsportivoPane"].ToString().Replace(".", ","),
                                    System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                            MassaCorporal = collection["massaCorporalTalentoEsportivoPane"] == ""
                                ? null
                                : decimal.Parse(
                                    collection["massaCorporalTalentoEsportivoPane"].ToString().Replace(".", ","),
                                    System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                            PreensaoManual = collection["preensaoManualTalentoEsportivoPane"] == ""
                                ? null
                                : decimal.Parse(
                                    collection["preensaoManualTalentoEsportivoPane"].ToString().Replace(".", ","),
                                    System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                            Flexibilidade = collection["flexibilidadeTalentoEsportivoPane"] == ""
                                ? null
                                : decimal.Parse(
                                    collection["flexibilidadeTalentoEsportivoPane"].ToString().Replace(".", ","),
                                    System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                            ImpulsaoHorizontal = collection["impulsaoHorizontalTalentoEsportivoPane"] == ""
                                ? null
                                : decimal.Parse(
                                    collection["impulsaoHorizontalTalentoEsportivoPane"].ToString().Replace(".", ","),
                                    System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                            Velocidade = collection["testeVelocidadeTalentoEsportivoPane"] == ""
                                ? null
                                : decimal.Parse(
                                    collection["testeVelocidadeTalentoEsportivoPane"].ToString().Replace(".", ","),
                                    System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                            AptidaoFisica = collection["aptidaoFisicaTalentoEsportivoPane"] == ""
                                ? null
                                : decimal.Parse(
                                    collection["aptidaoFisicaTalentoEsportivoPane"].ToString().Replace(".", ","),
                                    System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                            Agilidade = collection["agilidadeTalentoEsportivoPane"] == ""
                                ? null
                                : decimal.Parse(
                                    collection["agilidadeTalentoEsportivoPane"].ToString().Replace(".", ","),
                                    System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                            Abdominal = Convert.ToBoolean(collection["rdbAbdominalTalentoEsportivoPane"]),
                            StatusTalentosEsportivos = "F"
                        };

                        await ApiClientFactory.Instance.UpdateTalentoEsportivo((int)laudo.TalentoEsportivoId,
                            commandTalentoEsportivo);

                        command.TalentoEsportivoId = laudo.TalentoEsportivoId;

                        if (laudo.ModalidadeId == null)
                        {
                            var modalidade = ApiClientFactory.Instance.GetModalidadeAll()
                                .FirstOrDefault(x => x.Nome.Contains(laudo.EncaminhamentoTexto));

                            command.ModalidadeId = modalidade!.Id;
                        }
                        else
                        {
                            command.ModalidadeId = laudo.ModalidadeId;
                        }
                    }
                }
                else
                {
                    if (listTalentoEsportivoPane.Any())
                    {
                        var commandTalentoEsportivo = new TalentoEsportivoModel.CreateUpdateTalentoEsportivoCommand()
                        {
                            ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                            AlunoId = (int)laudo.AlunoId,
                            Altura = collection["alturaTalentoEsportivoPane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["alturaTalentoEsportivoPane"].ToString()),
                            MassaCorporal = collection["massaCorporalTalentoEsportivoPane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["massaCorporalTalentoEsportivoPane"].ToString()),
                            PreensaoManual = collection["preensaoManualTalentoEsportivoPane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["preensaoManualTalentoEsportivoPane"].ToString()),
                            Flexibilidade = collection["flexibilidadeTalentoEsportivoPane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["flexibilidadeTalentoEsportivoPane"].ToString()),
                            ImpulsaoHorizontal = collection["impulsaoHorizontalTalentoEsportivoPane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["impulsaoHorizontalTalentoEsportivoPane"].ToString()),
                            Velocidade = collection["testeVelocidadeTalentoEsportivoPane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["testeVelocidadeTalentoEsportivoPane"].ToString()),
                            AptidaoFisica = collection["aptidaoFisicaTalentoEsportivoPane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["aptidaoFisicaTalentoEsportivoPane"].ToString()),
                            Agilidade = collection["agilidadeTalentoEsportivoPane"] == ""
                                ? null
                                : Convert.ToDecimal(collection["agilidadeTalentoEsportivoPane"].ToString()),
                            Abdominal = Convert.ToBoolean(collection["rdbAbdominalTalentoEsportivoPane"]),
                            StatusTalentosEsportivos = "F"
                        };

                        if (!acessoAluno)
                        {
                            if (commandTalentoEsportivo.Altura != null &&
                                commandTalentoEsportivo.MassaCorporal != null &&
                                commandTalentoEsportivo.PreensaoManual != null &&
                                commandTalentoEsportivo.Flexibilidade != null &&
                                commandTalentoEsportivo.ImpulsaoHorizontal != null &&
                                commandTalentoEsportivo.Velocidade != null &&
                                commandTalentoEsportivo.AptidaoFisica != null &&
                                commandTalentoEsportivo.Agilidade != null)
                            {
                                command.TalentoEsportivoId =
                                    (int)await ApiClientFactory.Instance
                                        .CreateTalentoEsportivo(commandTalentoEsportivo);

                                var encaminhamento = ApiClientFactory.Instance
                                    .GetTalentoEsportivoById((int)command.TalentoEsportivoId)
                                    .Encaminhamento;

                                var modalidade = ApiClientFactory.Instance.GetModalidadeAll()
                                    .FirstOrDefault(x => x.Nome.Contains(encaminhamento.Nome));

                                command.ModalidadeId = modalidade!.Id;
                            }
                        }
                    }
                }

                await ApiClientFactory.Instance.UpdateLaudo(command.Id, command);

                if ((bool)acessoAluno)
                {
                    return RedirectToAction(nameof(Edit), new { id = id, acessoAluno = true, crud = (int)EnumCrud.Created });

                }

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });

            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        [ClaimsAuthorize(ClaimType.Laudo, Claim.Alterar)]
        public async Task<ActionResult> Edit(int id, int? crud, int? notify, bool acessoAluno = false, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                var usuario = User.Identity.Name;

                UsuarioDto usu;
                AlunoDto aln = null;
                if (acessoAluno != null && (bool)acessoAluno)
                {
                    aln = await ApiClientFactory.Instance.GetAlunoById(Convert.ToInt32(usuario));
                    usu = await ApiClientFactory.Instance.GetUsuarioByEmail(aln.Email);
                }
                else
                {
                    usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);
                }

                var questionarioVocacional =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.Vocacional).OrderBy(o => o.Questao).ToList();
                var questionarioQualidadeVida =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.QualidadeVida).OrderBy(o => o.Questao).ToList();
                var questionarioConsumoAlimentar =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.ConsumoAlimentar).OrderBy(o => o.Questao).ToList();
                var questionarioSaudeBucal =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.SaudeBucal).OrderBy(o => o.Questao).ToList();

                var laudo = ApiClientFactory.Instance.GetLaudoById(id);

                var aluno = await ApiClientFactory.Instance.GetAlunoById((int)laudo.AlunoId);

                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", aluno.Estado);

                var municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(aluno.Estado!), "Id", "Nome", aluno.MunicipioId);

                var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeByMunicipioId(aluno.MunicipioId.ToString()), "Id", "Nome", aluno.LocalidadeId);

                var profissionais = new SelectList(ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(aluno.LocalidadeId)), "Id", "Nome", aluno.ProfissionalId);

                var alunos = new SelectList(ApiClientFactory.Instance.GetAlunosByLocalidadeId(Convert.ToInt32(aluno.LocalidadeId)), "Id", "Nome", aluno.Id);

                var saude = new SaudeDto();

                if (laudo.SaudeId != null)
                {
                    saude = ApiClientFactory.Instance.GetSaudeById((int)laudo.SaudeId);
                }

                var talentoEsportivo = new TalentoEsportivoDto();

                if (laudo.TalentoEsportivoId != null)
                {
                    talentoEsportivo = ApiClientFactory.Instance.GetTalentoEsportivoById((int)laudo.TalentoEsportivoId);
                }

                var vocacional = new VocacionalDto();


                if (laudo.VocacionalId != null)
                {
                    vocacional = ApiClientFactory.Instance.GetVocacionalById(laudo.VocacionalId);
                }

                var consumoAlimentar = new ConsumoAlimentarDto();

                if (laudo.ConsumoAlimentarId != null)
                {
                    consumoAlimentar = ApiClientFactory.Instance.GetConsumoAlimentarById(laudo.ConsumoAlimentarId);
                }

                var qualidadeVida = new QualidadeVidaDto();

                if (laudo.QualidadeDeVidaId != null)
                {
                    qualidadeVida = ApiClientFactory.Instance.GetQualidadeVidaById(laudo.QualidadeDeVidaId);
                }

                var saudeBucal = new SaudeBucalDto();

                if (laudo.SaudeBucalId != null)
                {
                    saudeBucal = ApiClientFactory.Instance.GetSaudeBucalById(laudo.SaudeBucalId);
                }

                return View(new LaudoModel()
                {
                    ListQuestionarioVocacional = questionarioVocacional,
                    ListQuestionarioQualidadeVida = questionarioQualidadeVida,
                    ListQuestionarioConsumoAlimentar = questionarioConsumoAlimentar,
                    ListQuestionarioSaudeBucal = questionarioSaudeBucal,

                    ListEstados = estados,
                    ListMunicipios = municipios,
                    ListLocalidades = localidades,
                    ListProfissionais = profissionais,
                    ListAlunos = alunos,

                    Aluno = aluno,
                    Saude = saude,
                    TalentoEsportivo = talentoEsportivo,
                    Vocacional = vocacional,
                    ConsumoAlimentar = consumoAlimentar,
                    QualidadeVida = qualidadeVida,
                    SaudeBucal = saudeBucal,
                    IdPerfil = usu.Perfil.Id,
                    AcessoAluno = acessoAluno
                });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        [ClaimsAuthorize(ClaimType.Laudo, Claim.Consultar)]
        public async Task<IActionResult> Print([FromQuery] string ddlFomento, [FromQuery] string ddlEstado,
            [FromQuery] string ddlMunicipio, [FromQuery] string ddlLocalidade,
            [FromQuery] string ddlAluno, [FromQuery] string ddlTipoLaudo,
            [FromQuery] string ddlDeficiencia,
            [FromQuery] string possuiFoto, [FromQuery] string finalizado)
        {
            try
            {
                var usuario = User.Identity.Name;
                bool possuiFotoValue = bool.TryParse(possuiFoto, out var pfoto) ? pfoto : false;
                bool finalizadoValue = bool.TryParse(finalizado, out var fin) ? fin : false;

                var searchFilter = new LaudosResumidosFilterDto
                {
                    UsuarioEmail = usuario,
                    FomentoId = ddlFomento,
                    Estado = ddlEstado,
                    MunicipioId = ddlMunicipio,
                    LocalidadeId = ddlLocalidade,
                    TipoLaudoId = ddlTipoLaudo,
                    AlunoId = ddlAluno,
                    DeficienciaId = ddlDeficiencia,
                    PossuiFoto = possuiFotoValue,
                    Finalizado = finalizadoValue,

                };

                var result = await ApiClientFactory.Instance.GetLaudosResumidosByFilter(searchFilter);

                //var lista = result.LaudosResumidos.Where(x => x.Idade >= 14 && x.StatusLaudo == "F").ToList();

                var model = new LaudoResumidoModel()
                {
                    ListLaudosResumidos = result.LaudosResumidos.ToList()
                };

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        [ClaimsAuthorize(ClaimType.Laudo, Claim.Consultar)]
        public async Task<IActionResult> ExportLaudo([FromQuery] string ddlFomento, [FromQuery] string ddlEstado,
            [FromQuery] string ddlMunicipio, [FromQuery] string ddlLocalidade,
            [FromQuery] string ddlAluno, [FromQuery] string ddlTipoLaudo,
            [FromQuery] string ddlDeficiencia,
            [FromQuery] string possuiFoto, [FromQuery] string finalizado,
            int? crud = null, int? notify = null, string message = null)
        {
            try
            {
                var usuario = User.Identity.Name;

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                bool possuiFotoValue = bool.TryParse(possuiFoto, out var pfoto) ? pfoto : false;
                bool finalizadoValue = bool.TryParse(finalizado, out var fin) ? fin : false;

                var searchFilter = new LaudosFilterDto
                {
                    UsuarioEmail = usuario,
                    FomentoId = ddlFomento,
                    Estado = ddlEstado,
                    MunicipioId = ddlMunicipio,
                    LocalidadeId = ddlLocalidade,
                    TipoLaudoId = ddlTipoLaudo,
                    AlunoId = ddlAluno,
                    DeficienciaId = ddlDeficiencia,
                    PossuiFoto = possuiFotoValue,
                    Finalizado = finalizadoValue,
                    PageNumber = 1,
                    PageSize = 10000
                };

                var result = await ApiClientFactory.Instance.GetLaudosByFilter(searchFilter);

                var workbook = new XLWorkbook();
                workbook.AddWorksheet("sheetName");
                var ws = workbook.Worksheet("sheetName");
                ws.Cell(1, 1).Value = "Matricula";
                ws.Cell(1, 2).Value = "Data Nascimento";
                ws.Cell(1, 3).Value = "Idade";
                ws.Cell(1, 4).Value = "Aluno";
                ws.Cell(1, 5).Value = "Localidade";
                ws.Cell(1, 6).Value = "Email";
                ws.Cell(1, 7).Value = "Telefone";
                ws.Cell(1, 8).Value = "Celular";
                ws.Cell(1, 9).Value = "LaudoId";
                ws.Cell(1, 10).Value = "Saúde";
                ws.Cell(1, 11).Value = "Talento Esportivo";
                ws.Cell(1, 12).Value = "Consumo Alimentar";
                ws.Cell(1, 13).Value = "Saúde Bucal";
                ws.Cell(1, 14).Value = "Qualidade de Vida";
                ws.Cell(1, 15).Value = "Vocacional";
                ws.Cell(1, 16).Value = "Status Laudo";
                int row = 2;
                foreach (var item in result.Laudos.Items.ToList())
                {
                    ws.Cell("A" + row).Value = item.AlunoId;
                    ws.Cell("B" + row).Value = item.DtNascimento;
                    ws.Cell("C" + row).Value = item.Idade;
                    ws.Cell("D" + row).Value = item.NomeAluno;
                    ws.Cell("E" + row).Value = item.NomeLocalidade;
                    ws.Cell("F" + row).Value = item.Email;
                    ws.Cell("G" + row).Value = item.Telefone;
                    ws.Cell("H" + row).Value = item.Celular;
                    ws.Cell("I" + row).Value = item.Id;
                    ws.Cell("J" + row).Value = item.SaudeId != null ? "X" : "";
                    ws.Cell("K" + row).Value = item.TalentoEsportivoId != null ? "X" : "";
                    ws.Cell("L" + row).Value = item.ConsumoAlimentarId != null ? "X" : "";
                    ws.Cell("M" + row).Value = item.SaudeBucalId != null ? "X" : "";
                    ws.Cell("N" + row).Value = item.QualidadeDeVidaId != null ? "X" : "";
                    ws.Cell("O" + row).Value = item.VocacionalId != null ? "X" : "";
                    ws.Cell("P" + row).Value = item.StatusLaudo;
                    row++;
                }

                var exportPath = Path.Combine(_host.ContentRootPath, "Exportacao");
                var fileName = $"laudo_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(exportPath, fileName);

                if (!Directory.Exists(exportPath))
                    Directory.CreateDirectory(exportPath);

                workbook.SaveAs(filePath);

                if (!System.IO.File.Exists(filePath))
                {
                    return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Warning, message = "Arquivo não encontrado." });
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var response = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = DateTime.Now.ToString("ddMMyyyy") + "-laudo.xlsx"
                };

                return response;
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        /// <summary>
        /// Calcula quantidade de anos passdos com base em duas datas, caso encontre qualquer problema retorna 0 
        /// </summary>
        /// <param name="data">Data inicial</param>
        /// <param name="now">Data final ou deixar nula para data atual</param>
        /// <returns>Retorna inteiro com quantiadde de anos</returns>
        private static int GetIdade(DateTime data, DateTime? now = null)
        {
            // Carrega a data do dia para comparação caso data informada seja nula

            now = ((now == null) ? DateTime.Now : now);

            try
            {
                int YearsOld = (now.Value.Year - data.Year);

                if (now.Value.Month < data.Month || (now.Value.Month == data.Month && now.Value.Day < data.Day))
                {
                    YearsOld--;
                }

                return YearsOld > 18 ? 99 : YearsOld < 4 ? 4 : YearsOld;
            }
            catch
            {
                return 0;
            }
        }



        /// <summary>
        /// Impressão de Gabaritos do SAEB para Alunos de uma certa localidade
        /// </summary>
        /// <param name="ddlEstadoGabarito">Gabarito do SAEB</param>
        /// <param name="ddlMunicipioGabarito">Id do município</param>
        /// <param name="ddlLocalidadeGabarito">Id da localidade</param>
        /// <param name="ddlAlunoGabarito">Id do Aluno</param>
        /// <returns>Retorna a lista de Alunos</returns>
        [ClaimsAuthorize(ClaimType.Laudo, Claim.Consultar)]
        public async Task<IActionResult> PrintGabarito(IFormCollection collection)
        {
            try
            {
                var searchFilter = new AlunosFilterDto()
                {
                    Estado = collection["ddlEstadoGabarito"],
                    MunicipioId = collection["ddlMunicipioGabarito"],
                    LocalidadeId = collection["ddlLocalidadeGabarito"],
                    AlunoId = collection["ddlAlunoGabarito"],
                    SerieId = collection["ddlTurma"]
                };

                var result = await ApiClientFactory.Instance.GetAlunosByFilter(searchFilter);

                var textoGabarito = "";
                var anoGabarito = "";

                switch (collection["ddlGabaritoModal"])
                {
                    case "3LP":
                        textoGabarito = "LÍNGUA PORTUGUESA";
                        anoGabarito = " 3ª Série do Ensino Médio";
                        break;
                    case "3MT":
                        textoGabarito = "MATEMÁTICA";
                        anoGabarito = " 3ª Série do Ensino Médio";
                        break;
                    case "5LP":
                        textoGabarito = "LÍNGUA PORTUGUESA";
                        anoGabarito = " 5º Ano do Ensino Médio";
                        break;
                    case "5MT":
                        textoGabarito = "MATEMÁTICA";
                        anoGabarito = " 5º Ano do Ensino Médio";
                        break;
                    case "9LP":
                        textoGabarito = "LÍNGUA PORTUGUESA";
                        anoGabarito = " 9º Ano do Ensino Médio";
                        break;
                    case "9MT":
                        textoGabarito = "MATEMÁTICA";
                        anoGabarito = " 9º Ano do Ensino Médio";
                        break;
                }

                var model = new AlunoModel()
                {
                    Alunos = result.Alunos.ToList(),
                    TextGabarito = textoGabarito,
                    AnoGabarito = anoGabarito
                };

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        /// <summary>
        /// Ação de Processamento de Foto do Gabarito
        /// </summary>
        /// <param name="collection">Arquivo de upload realizado</param>
        /// <returns>Retorna um dicionario em json com as respostas reconhecidas</returns>
        [HttpPost]
        public async Task<IActionResult> ProcessarGabarito(IFormCollection collection)
        {
            try
            {
                _logger.Info($"Ação de processamento do gabarito - Laudo.ProcessarGabarito");

                byte[]? byteImage = null;
                foreach (var file in collection.Files)
                {
                    if (file.Length <= 0) continue;
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    byteImage = ms.ToArray();
                    break;
                }

                if (byteImage == null)
                    return Json(new { erro = "Nenhuma imagem foi enviada." });

                var resultado = await ApiClientFactory.Instance.ProcessarGabarito(byteImage);

                Dictionary<string, string> respostasDict;

                if (resultado["respostas"] is Dictionary<string, object> respostasObj)
                {
                    respostasDict = respostasObj.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.ToString() ?? ""
                    );
                }
                else if (resultado["respostas"] is Newtonsoft.Json.Linq.JObject respostasJObj)
                {
                    respostasDict = respostasJObj.ToObject<Dictionary<string, string>>();
                }
                else if (resultado["respostas"] is string respostasJson)
                {
                    respostasDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(respostasJson);
                }
                else
                {
                    respostasDict = resultado["respostas"] as Dictionary<string, string>;
                }

                return Json(new
                {
                    sucesso = true,
                    matricula = resultado["matricula"],
                    respostas = respostasDict
                });

            }
            catch (Exception e)
            {
                _logger.Error($"Ação de upload de foto do gabarito - Laudo.ProcessarGabarito: {e.Message}");
                return Json(new { sucesso = false, erro = e.Message });
            }
        }

        /// <summary>
        /// Ação de Upload de Foto do Gabarito
        /// </summary>
        /// <param name="collection">Arquivo de upload realizado</param>
        /// <returns>Retorna mensagem de upload realizado através do parametro notfy e message</returns>
        [HttpPost]
        //[ClaimsAuthorize(ClaimType.Laudo, Claim.Upload)]
        public async Task<ActionResult> Upload(IFormCollection collection)
        {
            try
            {
                _logger.Info($"Ação de upload de foto do gabarito - Laudo.Upload");

                var matricula = Convert.ToInt32(collection["matriculaReconhecida"]);
                var gabarito = "Educacional" + collection["ddlGabarito"];

                Enum.TryParse(gabarito, true, out EnumTipoLaudo enumValue);

                var questionario = ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)enumValue);

                var alternativaParaIndice = new Dictionary<string, int> {
                    { "A", 0 },
                    { "B", 1 },
                    { "C", 2 },
                    { "D", 3 },
                    { "E", 4 }
                };

                var respostaIds = new List<int>();
                var respostasJson = collection["respostasReconhecidas"].ToString();
                var respostasDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(respostasJson);

                foreach (var questao in questionario)
                {
                    string numeroQuestao = questao.Questao.ToString();
                    if (!respostasDict.TryGetValue(numeroQuestao, out string alternativaMarcada) ||
                        string.IsNullOrWhiteSpace(alternativaMarcada) ||
                        !alternativaParaIndice.ContainsKey(alternativaMarcada.ToUpper()))
                    {
                        respostaIds.Add(0);
                        continue;
                    }

                    int indice = alternativaParaIndice[alternativaMarcada.ToUpper()];
                    if (questao.Respostas.Count > indice)
                    {
                        var resposta = questao.Respostas[indice];
                        respostaIds.Add(resposta.Id);
                    }
                    else
                    {
                        respostaIds.Add(0);
                    }
                }

                string respostasString = string.Join(",", respostaIds);


                var command = new LaudoModel.CreateUpdateEducacionalCommand
                {
                    ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                    Gabarito = gabarito,
                    AlunoId = matricula,
                    Respostas = respostasString,
                    StatusEducacional = "F",
                };

                string? filePath;
                string? fileName;
                string extension = ".jpg";
                string newFileName = Path.ChangeExtension(
                    Guid.NewGuid().ToString(),
                    extension
                );

                foreach (var file in collection.Files)
                {
                    if (file.Length <= 0) continue;
                    fileName = Path.GetFileName(collection.Files[0].FileName);
                    filePath = Path.Combine(_host.WebRootPath, $"Gabaritos\\{newFileName}");

                    if (!Directory.Exists(Path.Combine(_host.WebRootPath, $"Gabaritos")))
                        Directory.CreateDirectory(Path.Combine(_host.WebRootPath, $"Gabaritos"));

                    command.Imagem = filePath;
                    command.NomeImagem = fileName;

                    using Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                }

                var educacionalId = await ApiClientFactory.Instance.CreateEducacional(command);

                var laudo = ApiClientFactory.Instance.GetLaudoByAluno(matricula);

                var ordem = (laudo?.Ordem ?? 0) + 1;

                if (laudo == null)
                {
                    var createLaudocommand = new LaudoModel.CreateUpdateLaudoCommand
                    {
                        AlunoId = matricula,
                        Ordem = ordem
                    };

                    await ApiClientFactory.Instance.CreateLaudo(createLaudocommand);

                    laudo = ApiClientFactory.Instance.GetLaudoByAluno(matricula);

                }

                var materia = gabarito?.Contains("LP", StringComparison.OrdinalIgnoreCase) == true ? "LP" : "MT";

                var updateLaudoCommand = new LaudoModel.UpdateLaudoEducacionalCommand
                {
                    Id = laudo.Id,
                    AlunoId = matricula,
                    EducacionalId = checked((int)educacionalId),
                    Materia = materia
                };

                await ApiClientFactory.Instance.UpdateLaudoEducacional(updateLaudoCommand.Id, updateLaudoCommand);

                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Success, message = "Upload realizado com sucesso." });
            }
            catch (Exception e)
            {
                _logger.Error($"Ação de upload de foto do gabarito - Laudo.Upload: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        public async Task<ActionResult> VisualizarGabarito(int id)
        {
            try
            {
                _logger.Info("Ação de Visualiza Gabarito do aluno - Laudo.VisualizarGabarito");

                var laudo = ApiClientFactory.Instance.GetLaudoById(id);
                var aluno = await ApiClientFactory.Instance.GetAlunoById((int)laudo.AlunoId);
                var educacionais = ApiClientFactory.Instance.GetEducacionaisByAluno(laudo.AlunoId);

                var alternativasDtos = new List<AlternativasDto>();
                var altDict = new Dictionary<int, string>();

                foreach (var e in educacionais)
                {
                    if (e.Imagem.IsNullOrEmpty())
                    {
                        var dto = ApiClientFactory.Instance.GetAlternativasByEducacionalId(e.Id);
                        if (dto != null)
                        {
                            alternativasDtos.Add(dto);
                            altDict[e.Id] = dto.Alternativas ?? string.Empty;
                        }
                    }
                }

                var model = new LaudoModel
                {
                    Aluno = aluno,
                    AlunoId = laudo.AlunoId.ToString(),
                    Educacionais = educacionais,
                    AlternativasDosEducacionais = alternativasDtos,
                    AlternativasPorEducacional = altDict
                };

                return View(model);
            }
            catch (Exception e)
            {
                _logger.Error($"Ação de visualizar gabarito do aluno - Laudo.VisualizarGabarito: {e}");
                return RedirectToAction(nameof(Index),
                    new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        public async Task<ActionResult> ResponderGabarito(IFormCollection collection)
        {
            try
            {
                _logger.Info($"Ação de Resposta do Gabarito  do aluno - Laudo.ResponderGabarito");

                var aluno = await ApiClientFactory.Instance.GetAlunoById(Convert.ToInt32(collection["alunoId"]));

                var textoGabarito = "";
                var anoGabarito = "";

                switch (collection["ddlGabarito"])
                {
                    case "3LP":
                        textoGabarito = "LÍNGUA PORTUGUESA";
                        anoGabarito = " 3ª Série do Ensino Médio";
                        break;
                    case "3MT":
                        textoGabarito = "MATEMÁTICA";
                        anoGabarito = " 3ª Série do Ensino Médio";
                        break;
                    case "5LP":
                        textoGabarito = "LÍNGUA PORTUGUESA";
                        anoGabarito = " 5º Ano do Ensino Médio";
                        break;
                    case "5MT":
                        textoGabarito = "MATEMÁTICA";
                        anoGabarito = " 5º Ano do Ensino Médio";
                        break;
                    case "9LP":
                        textoGabarito = "LÍNGUA PORTUGUESA";
                        anoGabarito = " 9º Ano do Ensino Médio";
                        break;
                    case "9MT":
                        textoGabarito = "MATEMÁTICA";
                        anoGabarito = " 9º Ano do Ensino Médio";
                        break;
                }

                var model = new AlunoModel()
                {
                    Aluno = aluno,
                    TextGabarito = textoGabarito,
                    AnoGabarito = anoGabarito,
                    SiglaGabarito = collection["ddlResponderGabarito"],
                    ProfissionalId = collection["ddlProfissionalRespostaModal"]
                };

                return View(model);
            }
            catch (Exception e)
            {
                _logger.Error($"Ação de resposta do gabarito do aluno - Laudo.ResponderGabarito: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        /// <summary>
        /// Busca de Laudo por Id
        /// </summary>
        /// <param name="id">Identificador de Laudo</param>
        /// <returns>Retorna o Laudo</returns>
        [HttpGet]
        public Task<LaudoDto> GetLaudoById(int id)
        {
            var result = ApiClientFactory.Instance.GetLaudoById(id);

            return Task.FromResult(result);
        }
        #endregion
    }
}
