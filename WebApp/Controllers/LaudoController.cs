using ClosedXML.Excel;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using iText.Kernel.Geom;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Globalization;
using WebApp.Authorization;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;
using Claim = WebApp.Identity.Claim;
using Path = System.IO.Path;

namespace WebApp.Controllers
{
    [Authorize(Policy = ModuloAccess.Laudo)]
    public class LaudoController : BaseController
    {
        private readonly IOptions<UrlSettings> _appSettings;
        private readonly IWebHostEnvironment _host;
        private readonly ILog _logger;

        public LaudoController(IOptions<UrlSettings> appSettings,
            IWebHostEnvironment host,
            ILog logger)
        {
            _appSettings = appSettings;
            _host = host;
            _logger = logger;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        [ClaimsAuthorize(ClaimType.Laudo, Claim.Consultar)]
        public async Task<IActionResult> Index(int? crud, int? notify, IFormCollection collection, string message = null)
        {
            try
            {
                _logger.Info($"Usuario Logado: {User.Identity.Name}");

                var usuario = User.Identity.Name;

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);


                var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome");
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

                    if (resultLocalidades != null)
                        localidades = new SelectList(resultLocalidades, "Id", "Nome", usu.LocalidadeId);
                }

                SelectList alunos = null;

                if (usu.LocalidadeId != null)
                {
                    var resultAlunos = ApiClientFactory.Instance.GetAlunosByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));

                    alunos = new SelectList(resultAlunos, "Id", "Nome");
                }

                var tiposLaudos = new SelectList(ApiClientFactory.Instance.GetTiposLaudoAll(), "Id", "Nome");

                var possuiFoto = collection["possuiFoto"].ToString();
                var finalizado = collection["finalizado"].ToString();

                var searchFilter = new LaudosFilterDto()
                {
                    UsuarioEmail = usuario,
                    FomentoId = collection["ddlFomento"].ToString(),
                    Estado = collection["ddlEstado"].ToString(),
                    MunicipioId = collection["ddlMunicipio"].ToString(),
                    LocalidadeId = collection["ddlLocalidade"].ToString() == "" ? usu.LocalidadeId : collection["ddlLocalidade"].ToString(),
                    TipoLaudoId = collection["ddlTipoLaudo"].ToString(),
                    AlunoId = collection["ddlAluno"].ToString(),
                    DeficienciaId = collection["ddlDeficiencia"].ToString(),
                    PossuiFoto = possuiFoto != "",
                    Finalizado = finalizado != "",
                    PageNumber = 1,
#if DEBUG
                    PageSize = 300
#else
                    PageSize = 1000
#endif
                };

                var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll(), "Id", "Nome", searchFilter.DeficienciaId);

                var response = await ApiClientFactory.Instance.GetLaudosByFilter(searchFilter);

                var model = new LaudoModel()
                {
                    Laudos = response.Laudos,
                    ListFomentos = fomentos,
                    ListEstados = estados,
                    ListTiposLaudos = tiposLaudos,
                    ListMunicipios = municipios!,
                    ListLocalidades = localidades!,
                    ListDeficiencias = deficiencias,
                    ListAlunos = alunos,
                    SearchFilter = searchFilter
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
            var laudo = ApiClientFactory.Instance.GetLaudoByAluno(id);

            var consumoAlimentar = laudo.ConsumoAlimentarId == null ? null : ApiClientFactory.Instance.GetConsumoAlimentarById((int)laudo.ConsumoAlimentarId);
            var saudeBucal = laudo.SaudeBucalId == null ? null : ApiClientFactory.Instance.GetConsumoAlimentarById((int)laudo.SaudeBucalId);

            var aluno = await ApiClientFactory.Instance.GetAlunoById(id);
            var profissional = laudo.ProfissionalId == null ? null : ApiClientFactory.Instance.GetProfissionalById(Convert.ToInt32(aluno.ProfissionalId));
            var talentoEsportivo = laudo.TalentoEsportivoId == null ? null : ApiClientFactory.Instance.GetTalentoEsportivoByAluno((int)laudo.AlunoId!);
            var encaminhamentoImc = laudo.SaudeId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoBySaudeId(Convert.ToInt32(laudo.SaudeId));
            var qualidadeDeVida = laudo.QualidadeDeVidaId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoByQualidadeDeVidaId((int)laudo.QualidadeDeVidaId);
            var vocacional = laudo.VocacionalId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoByVocacional();
            var encaminhamentoConsumoAlimentar = laudo.ConsumoAlimentarId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)consumoAlimentar.Encaminhamento.Id);
            var encaminhamentoSaudeBucal = laudo.SaudeBucalId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)saudeBucal.Encaminhamento.Id);
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
                Desempenho = desempenho,
                Modalidade = modalidade
            };
            return View(model);
        }

        //[ClaimsAuthorize(ClaimType.Laudo, Claim.Ver)]
        public async Task<ActionResult> Report(int id)
        {
            var laudo = ApiClientFactory.Instance.GetLaudoByAluno(id);

            var consumoAlimentar = laudo.ConsumoAlimentarId == null ? null : ApiClientFactory.Instance.GetConsumoAlimentarById((int)laudo.ConsumoAlimentarId);
            var saudeBucal = laudo.SaudeBucalId == null ? null : ApiClientFactory.Instance.GetSaudeBucalById((int)laudo.SaudeBucalId);

            var aluno = await ApiClientFactory.Instance.GetAlunoById(id);
            var profissional = laudo.ProfissionalId == null ? null : ApiClientFactory.Instance.GetProfissionalById(Convert.ToInt32(aluno.ProfissionalId));
            var talentoEsportivo = laudo.TalentoEsportivoId == null ? null : ApiClientFactory.Instance.GetTalentoEsportivoByAluno((int)laudo.AlunoId!);
            var encaminhamentoImc = laudo.SaudeId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoBySaudeId(Convert.ToInt32(laudo.SaudeId));
            var qualidadeDeVida = laudo.QualidadeDeVidaId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoByQualidadeDeVidaId((int)laudo.QualidadeDeVidaId);
            var vocacional = laudo.VocacionalId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoByVocacional();
            var encaminhamentoConsumoAlimentar = laudo.ConsumoAlimentarId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)consumoAlimentar.Encaminhamento.Id);
            var encaminhamentoSaudeBucal = laudo.SaudeBucalId == null ? null : ApiClientFactory.Instance.GetEncaminhamentoById((int)saudeBucal.Encaminhamento.Id);
            var desempenho = ApiClientFactory.Instance.GetDesempenhoByAluno(Convert.ToInt32(laudo.AlunoId));
            var modalidade = ApiClientFactory.Instance.GetModalidadeById(Convert.ToInt32(laudo.ModalidadeId));

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
                Desempenho = desempenho,
                Modalidade = modalidade
            };
            return View(model);
        }

        [ClaimsAuthorize(ClaimType.Laudo, Claim.Incluir)]
        public async Task<ActionResult> Create(int? crud, int? notify, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                var usuario = User.Identity.Name;

                var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

                var questionarioVocacional =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.Vocacional).OrderBy(o => o.Questao).ToList();
                var questionarioQualidadeVida =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.QualidadeVida).OrderBy(o => o.Questao).ToList();
                var questionarioConsumoAlimentar =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.ConsumoAlimentar).OrderBy(o => o.Questao).ToList();
                var questionarioSaudeBucal =
                    ApiClientFactory.Instance.GetQuestionarioByTipoLaudo((int)EnumTipoLaudo.SaudeBucal).OrderBy(o => o.Questao).ToList();

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
                    var resultAlunos = ApiClientFactory.Instance.GetAlunosByLocalidadeId(Convert.ToInt32(usu.LocalidadeId)).Where(x => x.PossuiLaudo == false);

                    alunos = new SelectList(resultAlunos, "Id", "Nome");

                    var resultProfissionais =
                        ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId));

                    profissionais = new SelectList(resultProfissionais, "Id", "Nome");
                }

                return View(new LaudoModel()
                {
                    ListQuestionarioVocacional = questionarioVocacional,
                    ListQuestionarioQualidadeVida = questionarioQualidadeVida,
                    ListQuestionarioConsumoAlimentar = questionarioConsumoAlimentar,
                    ListQuestionarioSaudeBucal = questionarioSaudeBucal,
                    ListEstados = estados,
                    ListMunicipios = municipios!,
                    ListLocalidades = localidades!,
                    ListAlunos = alunos!,
                    ListProfissionais = profissionais!
                });

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

                var command = new LaudoModel.CreateUpdateLaudoCommand
                {
                    AlunoId = Convert.ToInt32(collection["ddlAluno"].ToString())
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
                        : Convert.ToDecimal(collection["envergaduraSaude"].ToString()),
                    MassaCorporalSaude = collection["massaCorporalSaude"] == ""
                        ? null
                        : Convert.ToDecimal(collection["massaCorporalSaude"].ToString()),
                    AlturaSaude = collection["alturaSaude"] == ""
                        ? null
                        : Convert.ToDecimal(collection["alturaSaude"].ToString()),
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
                    Altura = collection["altura"] == "" ? null : Convert.ToDecimal(collection["altura"].ToString()),
                    MassaCorporal = collection["massaCorporal"] == "" ? null : Convert.ToDecimal(collection["massaCorporal"].ToString()),
                    PreensaoManual = collection["preensaoManual"] == "" ? null : Convert.ToDecimal(collection["preensaoManual"].ToString()),
                    Flexibilidade = collection["flexibilidade"] == "" ? null : Convert.ToDecimal(collection["flexibilidade"].ToString()),
                    ImpulsaoHorizontal = collection["impulsaoHorizontal"] == "" ? null : Convert.ToDecimal(collection["impulsaoHorizontal"].ToString()),
                    Velocidade = collection["testeVelocidade"] == "" ? null : Convert.ToDecimal(collection["testeVelocidade"].ToString()),
                    AptidaoFisica = collection["aptidaoFisica"] == "" ? null : Convert.ToDecimal(collection["aptidaoFisica"].ToString()),
                    Agilidade = collection["agilidade"] == "" ? null : Convert.ToDecimal(collection["agilidade"].ToString()),
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
                        .FirstOrDefault(x => x.Nome.Contains(encaminhamento.Nome));

                    command.ModalidadeId = modalidade!.Id;

                }
                else
                {
                    return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Error, message = "Favor informar todos os campos de Talento Esportivo." });
                }

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
                var laudo = ApiClientFactory.Instance.GetLaudoById(id);

                var command = new LaudoModel.CreateUpdateLaudoCommand
                {
                    Id = id,
                    AlunoId = (int)laudo.AlunoId
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
                    var saude = ApiClientFactory.Instance.GetSaudeById((int)laudo.SaudeId);

                    var commandSaude = new SaudeModel.CreateUpdateSaudeCommand()
                    {
                        Id = (int)laudo.SaudeId,
                        ProfissionalId = saude.ProfissionalId,
                        AlunoId = (int)laudo.AlunoId,
                        EnvergaduraSaude = collection["envergaduraSaude"] == ""
                            ? null
                            : Convert.ToDecimal(collection["envergaduraSaude"].ToString()),
                        MassaCorporalSaude = collection["massaCorporalSaude"] == ""
                            ? null
                            : Convert.ToDecimal(collection["massaCorporalSaude"].ToString()),
                        AlturaSaude = collection["alturaSaude"] == ""
                            ? null
                            : Convert.ToDecimal(collection["alturaSaude"].ToString()),
                        StatusSaude = "F"
                    };

                    await ApiClientFactory.Instance.UpdateSaude((int)laudo.SaudeId, commandSaude);

                    command.SaudeId = laudo.SaudeId;
                }
                else
                {
                    var commandSaude = new SaudeModel.CreateUpdateSaudeCommand()
                    {
                        ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                        AlunoId = (int)laudo.AlunoId,
                        EnvergaduraSaude = collection["envergaduraSaude"] == ""
                            ? null
                            : Convert.ToDecimal(collection["envergaduraSaude"].ToString()),
                        MassaCorporalSaude = collection["massaCorporalSaude"] == ""
                            ? null
                            : Convert.ToDecimal(collection["massaCorporalSaude"].ToString()),
                        AlturaSaude = collection["alturaSaude"] == ""
                            ? null
                            : Convert.ToDecimal(collection["alturaSaude"].ToString()),
                        StatusSaude = "F"
                    };

                    commandSaude.Id = (int)await ApiClientFactory.Instance.CreateSaude(commandSaude);
                }

                if (laudo.TalentoEsportivoId != null)
                {
                    var talentoEsportivo = ApiClientFactory.Instance.GetTalentoEsportivoById((int)laudo.TalentoEsportivoId);

                    var commandTalentoEsportivo = new TalentoEsportivoModel.CreateUpdateTalentoEsportivoCommand()
                    {
                        Id = (int)laudo.TalentoEsportivoId,
                        ProfissionalId = talentoEsportivo.ProfissionalId,
                        AlunoId = (int)laudo.AlunoId,
                        Altura = collection["altura"] == "" ? null : Convert.ToDecimal(collection["altura"].ToString()),
                        MassaCorporal = collection["massaCorporal"] == "" ? null : Convert.ToDecimal(collection["massaCorporal"].ToString()),
                        PreensaoManual = collection["preensaoManual"] == "" ? null : Convert.ToDecimal(collection["preensaoManual"].ToString()),
                        Flexibilidade = collection["flexibilidade"] == "" ? null : Convert.ToDecimal(collection["flexibilidade"].ToString()),
                        ImpulsaoHorizontal = collection["impulsaoHorizontal"] == "" ? null : Convert.ToDecimal(collection["impulsaoHorizontal"].ToString()),
                        Velocidade = collection["testeVelocidade"] == "" ? null : Convert.ToDecimal(collection["testeVelocidade"].ToString()),
                        AptidaoFisica = collection["aptidaoFisica"] == "" ? null : Convert.ToDecimal(collection["aptidaoFisica"].ToString()),
                        Agilidade = collection["agilidade"] == "" ? null : Convert.ToDecimal(collection["agilidade"].ToString()),
                        Abdominal = Convert.ToBoolean(collection["rdbAbdominal"]),
                        StatusTalentosEsportivos = "F"
                    };

                    await ApiClientFactory.Instance.UpdateTalentoEsportivo((int)laudo.TalentoEsportivoId, commandTalentoEsportivo);

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
                else
                {
                    var commandTalentoEsportivo = new TalentoEsportivoModel.CreateUpdateTalentoEsportivoCommand()
                    {
                        ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                        AlunoId = (int)laudo.AlunoId,
                        Altura = collection["altura"] == "" ? null : Convert.ToDecimal(collection["altura"].ToString()),
                        MassaCorporal = collection["massaCorporal"] == "" ? null : Convert.ToDecimal(collection["massaCorporal"].ToString()),
                        PreensaoManual = collection["preensaoManual"] == "" ? null : Convert.ToDecimal(collection["preensaoManual"].ToString()),
                        Flexibilidade = collection["flexibilidade"] == "" ? null : Convert.ToDecimal(collection["flexibilidade"].ToString()),
                        ImpulsaoHorizontal = collection["impulsaoHorizontal"] == "" ? null : Convert.ToDecimal(collection["impulsaoHorizontal"].ToString()),
                        Velocidade = collection["testeVelocidade"] == "" ? null : Convert.ToDecimal(collection["testeVelocidade"].ToString()),
                        AptidaoFisica = collection["aptidaoFisica"] == "" ? null : Convert.ToDecimal(collection["aptidaoFisica"].ToString()),
                        Agilidade = collection["agilidade"] == "" ? null : Convert.ToDecimal(collection["agilidade"].ToString()),
                        Abdominal = Convert.ToBoolean(collection["rdbAbdominal"]),
                        StatusTalentosEsportivos = "F"
                    };

                    command.TalentoEsportivoId = (int)await ApiClientFactory.Instance.CreateTalentoEsportivo(commandTalentoEsportivo);
                }

                await ApiClientFactory.Instance.UpdateLaudo(command.Id, command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        [ClaimsAuthorize(ClaimType.Laudo, Claim.Alterar)]
        public async Task<ActionResult> Edit(int id, int? crud, int? notify, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

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
                    SaudeBucal = saudeBucal
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
#if DEBUG
                    PageSize = 2000
#else
            PageSize = 1000
#endif
                };

                var result = await ApiClientFactory.Instance.GetLaudosByFilter(searchFilter);
                var laudoModels = new List<LaudoModel>();

                foreach (var laudo in result.Laudos.Items)
                {
                    if (laudo.AlunoId != null)
                    {
                        var aluno = await ApiClientFactory.Instance.GetAlunoById((int)laudo.AlunoId);
                        if (aluno != null)
                        {
                            var profissional = ApiClientFactory.Instance.GetProfissionalById(Convert.ToInt32(aluno.ProfissionalId));
                            var talentoEsportivo = laudo.TalentoEsportivoId == null ? null :
                                ApiClientFactory.Instance.GetTalentoEsportivoByAluno((int)laudo.AlunoId);
                            var encaminhamentoImc = laudo.SaudeId == null ? null :
                                ApiClientFactory.Instance.GetEncaminhamentoBySaudeId(Convert.ToInt32(laudo.SaudeId));
                            var qualidadeDeVida = laudo.QualidadeDeVidaId == null ? null :
                                ApiClientFactory.Instance.GetEncaminhamentoByQualidadeDeVidaId((int)laudo.QualidadeDeVidaId);
                            var vocacional = laudo.VocacionalId == null ? null :
                                ApiClientFactory.Instance.GetEncaminhamentoByVocacional();
                            var encaminhamentoConsumoAlimentar = laudo.ConsumoAlimentarId == null ? null :
                                ApiClientFactory.Instance.GetEncaminhamentoByConsumoAlimentarId((int)laudo.ConsumoAlimentarId);
                            var encaminhamentoSaudeBucal = laudo.SaudeBucalId == null ? null :
                                ApiClientFactory.Instance.GetEncaminhamentoBySaudeBucalId((int)laudo.SaudeBucalId);
                            var desempenho = ApiClientFactory.Instance.GetDesempenhoByAluno(Convert.ToInt32(laudo.AlunoId));
                            var modalidade = ApiClientFactory.Instance.GetModalidadeById(Convert.ToInt32(laudo.ModalidadeId));

                            laudoModels.Add(new LaudoModel
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
                                Desempenho = desempenho,
                                Modalidade = modalidade
                            });
                        }
                    }
                }

                return View(laudoModels);
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        /// <summary>
        /// Gera um PDF contendo os laudos filtrados de acordo com os parâmetros fornecidos.
        /// </summary>
        /// <param name="ddlFomento">Identificador do fomento.</param>
        /// <param name="ddlEstado">Sigla do estado.</param>
        /// <param name="ddlMunicipio">Identificador do município.</param>
        /// <param name="ddlLocalidade">Identificador da localidade.</param>
        /// <param name="ddlAluno">Identificador do aluno.</param>
        /// <param name="ddlTipoLaudo">Identificador do tipo de laudo.</param>
        /// <param name="ddlDeficiencia">Identificador da deficiência.</param>
        /// <param name="possuiFoto">Indica se possui foto.</param>
        /// <param name="finalizado">Indica se está finalizado.</param>
        /// <returns>Retorna um arquivo PDF contendo os laudos filtrados.</returns>
        [ClaimsAuthorize(ClaimType.Laudo, Claim.Consultar)]
        public async Task<IActionResult> GerarPdfLaudos([FromQuery] string ddlFomento, [FromQuery] string ddlEstado,
            [FromQuery] string ddlMunicipio, [FromQuery] string ddlLocalidade,
            [FromQuery] string ddlAluno, [FromQuery] string ddlTipoLaudo,
            [FromQuery] string ddlDeficiencia,
            [FromQuery] string possuiFoto, [FromQuery] string finalizado)
        {
            try
            {
                _logger.Info($"Gerando PDF de laudos em lote - Laudo.GerarPdfLaudos");

                var usuario = User.Identity.Name;
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
#if DEBUG
                    PageSize = 2000
#else
                    PageSize = 1000
#endif
                };

                var result = await ApiClientFactory.Instance.GetLaudosByFilter(searchFilter);
                var laudoModels = new List<LaudoModel>();

                // Obter os dados para cada laudo
                foreach (var laudo in result.Laudos.Items)
                {
                    var aluno = await ApiClientFactory.Instance.GetAlunoById((int)laudo.AlunoId);
                    var profissional = ApiClientFactory.Instance.GetProfissionalById(Convert.ToInt32(aluno.ProfissionalId));
                    var talentoEsportivo = laudo.TalentoEsportivoId == null ? null :
                        ApiClientFactory.Instance.GetTalentoEsportivoByAluno((int)laudo.AlunoId);
                    var encaminhamentoImc = laudo.SaudeId == null ? null :
                        ApiClientFactory.Instance.GetEncaminhamentoBySaudeId(Convert.ToInt32(laudo.SaudeId));
                    var qualidadeDeVida = laudo.QualidadeDeVidaId == null ? null :
                        ApiClientFactory.Instance.GetEncaminhamentoByQualidadeDeVidaId((int)laudo.QualidadeDeVidaId);
                    var vocacional = laudo.VocacionalId == null ? null :
                        ApiClientFactory.Instance.GetEncaminhamentoByVocacional();
                    var encaminhamentoConsumoAlimentar = laudo.ConsumoAlimentarId == null ? null :
                        ApiClientFactory.Instance.GetEncaminhamentoByConsumoAlimentarId((int)laudo.ConsumoAlimentarId);
                    var encaminhamentoSaudeBucal = laudo.SaudeBucalId == null ? null :
                        ApiClientFactory.Instance.GetEncaminhamentoBySaudeBucalId((int)laudo.SaudeBucalId);
                    var desempenho = ApiClientFactory.Instance.GetDesempenhoByAluno(Convert.ToInt32(laudo.AlunoId));
                    var modalidade = ApiClientFactory.Instance.GetModalidadeById(Convert.ToInt32(laudo.ModalidadeId));

                    laudoModels.Add(new LaudoModel
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
                        Desempenho = desempenho,
                        Modalidade = modalidade
                    });
                }

                // Gerar o PDF
                byte[] pdfBytes = await GerarPdfLaudosEmLote(laudoModels);

                // Retornar o arquivo PDF
                return File(
                    pdfBytes,
                    "application/pdf",
                    $"Laudos_{DateTime.Now:yyyyMMdd}.pdf"
                );
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao gerar PDF de laudos: {ex.Message}", ex);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = ex.Message });
            }
        }

        /// <summary>
        /// Gera um PDF contendo os laudos fornecidos em lote.
        /// </summary>
        /// <param name="laudoModels">Lista de modelos de laudo contendo as informações a serem incluídas no PDF.</param>
        /// <returns>Retorna um array de bytes representando o conteúdo do PDF gerado.</returns>
        private async Task<byte[]> GerarPdfLaudosEmLote(List<LaudoModel> laudoModels)
        {
            // Criar memorystream para armazenar o PDF
            using (MemoryStream ms = new MemoryStream())
            {
                // Inicializar writer e documento
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);

                // Definir tamanho A4
                PageSize pageSize = PageSize.A4;
                Document document = new Document(pdf, pageSize);
                document.SetMargins(0, 0, 0, 0);

                // Definir cores principais
                DeviceRgb corHeaderBg = new DeviceRgb(69, 94, 159); // Aproximação do gradient do header

                // DNA Score
                DeviceRgb corDnaScore = new DeviceRgb(0, 234, 188); // #00eabc
                DeviceRgb corDnaScoreBg = new DeviceRgb(192, 250, 238); // #c0faee

                // Saúde
                DeviceRgb corSaude = new DeviceRgb(239, 173, 87); // #efad57
                DeviceRgb corSaudeBg = new DeviceRgb(255, 238, 217); // #ffeed9

                // Saúde Bucal
                DeviceRgb corSaudeBucal = new DeviceRgb(233, 108, 82); // #e96c52
                DeviceRgb corSaudeBucalBg = new DeviceRgb(252, 227, 222); // #fce3de

                // Consumo Alimentar
                DeviceRgb corConsumoAlimentar = new DeviceRgb(115, 220, 105); // #73dc69
                DeviceRgb corConsumoAlimentarBg = new DeviceRgb(222, 255, 220); // #deffdc

                // Vocação
                DeviceRgb corVocacao = new DeviceRgb(91, 92, 156); // #5b5c9c
                DeviceRgb corVocacaoBg = new DeviceRgb(213, 214, 231); // #d5d6e7

                // Qualidade de Vida
                DeviceRgb corVida = new DeviceRgb(35, 191, 255); // #23bfff
                DeviceRgb corVidaBg = new DeviceRgb(200, 238, 255); // #c8eeff

                // Esporte
                DeviceRgb corEsporte = new DeviceRgb(223, 67, 108); // #df436c
                DeviceRgb corEsporteBg = new DeviceRgb(247, 209, 219); // #f7d1db

                // Carregar as fontes Ubuntu
                PdfFont ubuntuRegular = PdfFontFactory.CreateFont(Path.Combine(_host.WebRootPath, "assets/fonts/Ubuntu-Regular.ttf"), PdfEncodings.IDENTITY_H);
                PdfFont ubuntuBold = PdfFontFactory.CreateFont(Path.Combine(_host.WebRootPath, "assets/fonts/Ubuntu-Bold.ttf"), PdfEncodings.IDENTITY_H);

                // Para cada laudo, gerar as páginas
                foreach (var laudo in laudoModels)
                {
                    // Primeira página
                    CriarPrimeiraPagina(document, pdf, laudo, ubuntuRegular, ubuntuBold,
                        corHeaderBg, corDnaScore, corDnaScoreBg,
                        corSaude, corSaudeBg, corSaudeBucal, corSaudeBucalBg,
                        corConsumoAlimentar, corConsumoAlimentarBg, corVocacao, corVocacaoBg);

                    // Segunda página
                    CriarSegundaPagina(document, pdf, laudo, ubuntuRegular, ubuntuBold,
                        corHeaderBg, corVida, corVidaBg, corEsporte, corEsporteBg);
                }

                // Fechar o documento
                document.Close();

                // Retornar os bytes do PDF
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Cria a primeira página do documento PDF com informações do laudo.
        /// </summary>
        /// <param name="document">Documento PDF onde a página será adicionada.</param>
        /// <param name="pdf">Documento PDF.</param>
        /// <param name="laudo">Modelo de laudo contendo as informações a serem exibidas.</param>
        /// <param name="fontRegular">Fonte regular para o texto.</param>
        /// <param name="fontBold">Fonte em negrito para o texto.</param>
        /// <param name="corHeaderBg">Cor de fundo do cabeçalho.</param>
        /// <param name="corDnaScore">Cor principal do DNA Score.</param>
        /// <param name="corDnaScoreBg">Cor de fundo do DNA Score.</param>
        /// <param name="corSaude">Cor principal da seção de saúde.</param>
        /// <param name="corSaudeBg">Cor de fundo da seção de saúde.</param>
        /// <param name="corSaudeBucal">Cor principal da seção de saúde bucal.</param>
        /// <param name="corSaudeBucalBg">Cor de fundo da seção de saúde bucal.</param>
        /// <param name="corConsumoAlimentar">Cor principal da seção de consumo alimentar.</param>
        /// <param name="corConsumoAlimentarBg">Cor de fundo da seção de consumo alimentar.</param>
        /// <param name="corVocacao">Cor principal da seção de vocação.</param>
        /// <param name="corVocacaoBg">Cor de fundo da seção de vocação.</param>
        private void CriarPrimeiraPagina(Document document, PdfDocument pdf, LaudoModel laudo,
            PdfFont fontRegular, PdfFont fontBold,
                    DeviceRgb corHeaderBg, DeviceRgb corDnaScore, DeviceRgb corDnaScoreBg,
            DeviceRgb corSaude, DeviceRgb corSaudeBg, DeviceRgb corSaudeBucal, DeviceRgb corSaudeBucalBg,
            DeviceRgb corConsumoAlimentar, DeviceRgb corConsumoAlimentarBg, DeviceRgb corVocacao, DeviceRgb corVocacaoBg)
        {
            // Controle de paginação (mantém seu código existente)
            if (pdf.GetNumberOfPages() > 0)
            {
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            }
            else
            {
                pdf.AddNewPage();
            }

            // Obter dimensões da página
            float larguraPagina = pdf.GetDefaultPageSize().GetWidth();
            float alturaPagina = pdf.GetDefaultPageSize().GetHeight();

            // --- SEÇÃO 1: CABEÇALHO (HEADER) ---
            // (O header está bom, mantemos seu código existente)
            Rectangle headerRect = new Rectangle(0, alturaPagina - 120, larguraPagina, 120);
            PdfCanvas headerCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));

            headerCanvas.SaveState()
                .SetFillColor(corHeaderBg)
                .Rectangle(headerRect)
                .Fill()
                .RestoreState();

            // Logo DNA
            string logoPath = Path.Combine(_host.WebRootPath, "assets/images/logo2.png");
            if (System.IO.File.Exists(logoPath))
            {
                ImageData logoImageData = ImageDataFactory.Create(logoPath);
                iText.Layout.Element.Image logoImage = new iText.Layout.Element.Image(logoImageData)
                    .ScaleToFit(150, 81)
                    .SetFixedPosition(pdf.GetNumberOfPages(), 20, alturaPagina - 100);
                document.Add(logoImage);
            }

            // Informações do talento
            float inicioLadoDireito = larguraPagina - 250;
            Rectangle talentRect = new Rectangle(inicioLadoDireito, alturaPagina - 110, 230, 95);
            Canvas talentCanvas = new Canvas(headerCanvas, talentRect, true);

            Paragraph talentLabel = new Paragraph("MEU TALENTO")
                .SetFont(fontRegular)
                .SetFontSize(18)
                .SetFontColor(ColorConstants.WHITE);
            talentCanvas.Add(talentLabel);

            string talentName = laudo.Modalidade.Nome ?? "Não Definido";
            Paragraph talentNameParagraph = new Paragraph(talentName)
                .SetFont(fontBold)
                .SetFontSize(35)
                .SetFontColor(ColorConstants.WHITE);
            talentCanvas.Add(talentNameParagraph);

            // Ícone do esporte (ajustado para melhor posicionamento)
            float iconX = larguraPagina - 90;
            float iconY = alturaPagina - 110;

            if (laudo.Modalidade.ByteImage != null)
            {
                ImageData sportIconData = ImageDataFactory.Create(laudo.Modalidade.ByteImage);
                iText.Layout.Element.Image sportIcon = new iText.Layout.Element.Image(sportIconData)
                    .ScaleToFit(69, 95)
                    .SetFixedPosition(pdf.GetNumberOfPages(), iconX, iconY);
                document.Add(sportIcon);
            }
            else
            {
                string defaultIconPath = Path.Combine(_host.WebRootPath, "assets/assets_Laudo/icon_info_esporte.png");
                if (System.IO.File.Exists(defaultIconPath))
                {
                    ImageData defaultIconData = ImageDataFactory.Create(defaultIconPath);
                    iText.Layout.Element.Image defaultIcon = new iText.Layout.Element.Image(defaultIconData)
                        .ScaleToFit(69, 95)
                        .SetFixedPosition(pdf.GetNumberOfPages(), iconX, iconY);
                    document.Add(defaultIcon);
                }
            }

            // --- SEÇÃO 2: INFORMAÇÕES DO ALUNO (AJUSTADA) ---
            // Seção de fundo cinza
            float infoSectionHeight = 150;
            Rectangle infoRect = new Rectangle(0, alturaPagina - 120 - infoSectionHeight, larguraPagina, infoSectionHeight);
            PdfCanvas infoCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));

            infoCanvas.SaveState()
                .SetFillColor(new DeviceRgb(244, 244, 244)) // #f4f4f4
                .Rectangle(infoRect)
                .Fill()
                .RestoreState();

            // Definir coordenadas para informações do aluno (ajustadas)
            float infoSectionY = alturaPagina - 120 - 16; // Topo da seção de info - margem
            float leftSectionX = 16;  // Margem esquerda

            // Foto do aluno (ajustada para ficar bem posicionada)
            float photoSize = 75;
            float photoX = leftSectionX;
            float photoY = infoSectionY - photoSize;

            if (laudo.Laudo.ByteImage != null)
            {
                ImageData photoData = ImageDataFactory.Create(laudo.Laudo.ByteImage);

                // Criar máscara circular para a foto
                PdfCanvas circleCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
                circleCanvas.SaveState();

                // Definir círculo para clipping
                circleCanvas.Arc(photoX, photoY, photoX + photoSize, photoY + photoSize, 0, 360);
                circleCanvas.Clip().EndPath();

                // Adicionar a imagem com escala para cobrir o círculo
                iText.Layout.Element.Image photo = new iText.Layout.Element.Image(photoData)
                    .ScaleToFit(photoSize, photoSize)
                    .SetFixedPosition(pdf.GetNumberOfPages(), photoX, photoY);
                document.Add(photo);

                circleCanvas.RestoreState();
            }
            else
            {
                string defaultPhotoPath = Path.Combine(_host.WebRootPath, "assets/images/user.png");
                if (System.IO.File.Exists(defaultPhotoPath))
                {
                    ImageData defaultPhotoData = ImageDataFactory.Create(defaultPhotoPath);

                    // Criar máscara circular para a foto padrão
                    PdfCanvas circleCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
                    circleCanvas.SaveState();

                    circleCanvas.Arc(photoX, photoY, photoX + photoSize, photoY + photoSize, 0, 360);
                    circleCanvas.Clip().EndPath();

                    iText.Layout.Element.Image defaultPhoto = new iText.Layout.Element.Image(defaultPhotoData)
                        .ScaleToFit(photoSize, photoSize)
                        .SetFixedPosition(pdf.GetNumberOfPages(), photoX, photoY);
                    document.Add(defaultPhoto);

                    circleCanvas.RestoreState();
                }
            }

            // Informações básicas do aluno (ajustadas para alinhamento correto)
            float infoBasicX = photoX + photoSize + 10;
            float infoBasicY = infoSectionY;

            // Nome (aumentado e em negrito)
            Paragraph nameParagraph = new Paragraph(laudo.Laudo.NomeAluno)
                .SetFont(fontBold)
                .SetFontSize(20)
                .SetFixedPosition(pdf.GetNumberOfPages(), infoBasicX, infoBasicY - 20, 300);
            document.Add(nameParagraph);

            // Escola (tamanho médio)
            Paragraph schoolParagraph = new Paragraph(laudo.Laudo.NomeLocalidade)
                .SetFont(fontRegular)
                .SetFontSize(14)
                .SetFixedPosition(pdf.GetNumberOfPages(), infoBasicX, infoBasicY - 45, 300);
            document.Add(schoolParagraph);

            // Email (com sublinhado e cor azul)
            Paragraph emailParagraph = new Paragraph(laudo.Laudo.Email)
                .SetFont(fontRegular)
                .SetFontSize(13)
                .SetFixedPosition(pdf.GetNumberOfPages(), infoBasicX, infoBasicY - 65, 300)
                .SetFontColor(new DeviceRgb(0, 0, 238)); // Cor de link
            document.Add(emailParagraph);

            // Caixa branca de detalhes do aluno (redesenhada)
            float detailsBoxWidth = 380;
            float detailsBoxHeight = 55;
            float detailsBoxX = leftSectionX;
            float detailsBoxY = photoY - detailsBoxHeight - 10; // Abaixo da foto

            // Fundo da caixa branca com cantos arredondados
            PdfCanvas detailsCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
            detailsCanvas.SaveState()
                .SetFillColor(ColorConstants.WHITE)
                .RoundRectangle(detailsBoxX, detailsBoxY, detailsBoxWidth, detailsBoxHeight, 6)
                .Fill()
                .RestoreState();

            // Detalhes em grid (2 colunas)
            float leftColX = detailsBoxX + 10;
            float rightColX = leftColX + 200;
            float detailsTopY = detailsBoxY + detailsBoxHeight - 15;

            // Coluna esquerda
            Paragraph ageDetail = new Paragraph()
                .Add(new iText.Layout.Element.Text("Idade: ")
                    .SetFont(fontBold).SetFontSize(13).SetFontColor(new DeviceRgb(68, 68, 68)))
                .Add(new iText.Layout.Element.Text(laudo.Laudo.Idade.ToString())
                    .SetFont(fontRegular).SetFontSize(13))
                .SetFixedPosition(pdf.GetNumberOfPages(), leftColX, detailsTopY, 180);
            document.Add(ageDetail);

            Paragraph ethnicityDetail = new Paragraph()
                .Add(new iText.Layout.Element.Text("Etnia: ")
                    .SetFont(fontBold).SetFontSize(13).SetFontColor(new DeviceRgb(68, 68, 68)))
                .Add(new iText.Layout.Element.Text(laudo.Laudo.Etnia)
                    .SetFont(fontRegular).SetFontSize(13))
                .SetFixedPosition(pdf.GetNumberOfPages(), leftColX, detailsTopY - 18, 180);
            document.Add(ethnicityDetail);

            if (laudo.Laudo.NomeDeficiencia != "NÃO POSSUI")
            {
                Paragraph disabilityDetail = new Paragraph()
                    .Add(new iText.Layout.Element.Text("Deficiência: ")
                        .SetFont(fontBold).SetFontSize(13).SetFontColor(new DeviceRgb(68, 68, 68)))
                    .Add(new iText.Layout.Element.Text(laudo.Laudo.NomeDeficiencia)
                        .SetFont(fontRegular).SetFontSize(13))
                    .SetFixedPosition(pdf.GetNumberOfPages(), leftColX, detailsTopY - 36, 180);
                document.Add(disabilityDetail);
            }

            // Coluna direita
            Paragraph seriesDetail = new Paragraph()
                .Add(new iText.Layout.Element.Text("Série: ")
                    .SetFont(fontBold).SetFontSize(13).SetFontColor(new DeviceRgb(68, 68, 68)))
                .Add(new iText.Layout.Element.Text("Não definida")
                    .SetFont(fontRegular).SetFontSize(13))
                .SetFixedPosition(pdf.GetNumberOfPages(), rightColX, detailsTopY, 180);
            document.Add(seriesDetail);

            // --- SEÇÃO 3: DNA SCORE (REDESENHADA) ---
            // Caixa do DNA score do lado direito
            float dnaBoxWidth = 300;
            float dnaBoxHeight = 130;
            float dnaBoxX = larguraPagina - dnaBoxWidth - 20; // Margem direita
            float dnaBoxY = infoSectionY - dnaBoxHeight + 20; // Ajuste vertical

            // Fundo branco com cantos arredondados
            PdfCanvas dnaScoreCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
            dnaScoreCanvas.SaveState()
                .SetFillColor(ColorConstants.WHITE)
                .RoundRectangle(dnaBoxX, dnaBoxY, dnaBoxWidth, dnaBoxHeight, 8)
                .Fill()
                .RestoreState();

            // Círculo de progresso do DNA
            float circleX = dnaBoxX + 55;
            float circleY = dnaBoxY + dnaBoxHeight / 2;
            float circleRadius = 45;

            // Círculo de fundo
            dnaScoreCanvas.SaveState()
                .SetStrokeColor(corDnaScoreBg)
                .SetLineWidth(8)
                .Circle(circleX, circleY, circleRadius)
                .Stroke()
                .RestoreState();

            // Arco de progresso
            float scoreValue = laudo.Desempenho.ScoreDna;
            float scoreRatio = scoreValue / 6f; // Normalizado para 6

            if (scoreRatio > 0)
            {
                dnaScoreCanvas.SaveState()
                    .SetStrokeColor(corDnaScore)
                    .SetLineWidth(8);

                // Desenhar o arco (ajustado para a versão correta do iText7)
                float startAngle = 270;
                float endAngle = startAngle + (scoreRatio * 360);

                if (Math.Abs(scoreRatio - 1.0) < 0.001)
                {
                    dnaScoreCanvas.Circle(circleX, circleY, circleRadius).Stroke();
                }
                else
                {
                    dnaScoreCanvas.Arc(circleX - circleRadius, circleY - circleRadius,
                                      circleX + circleRadius, circleY + circleRadius,
                                      startAngle, endAngle - startAngle).Stroke();
                }

                dnaScoreCanvas.RestoreState();
            }

            // Valor do score no centro
            Paragraph scoreText = new Paragraph(scoreValue.ToString())
                .SetFont(fontBold)
                .SetFontSize(32)
                .SetFontColor(corDnaScore)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFixedPosition(pdf.GetNumberOfPages(), circleX - 20, circleY - 10, 40);
            document.Add(scoreText);

            // Label DNA abaixo
            Paragraph dnaLabel = new Paragraph("DNA")
                .SetFont(fontRegular)
                .SetFontSize(20)
                .SetFontColor(new DeviceRgb(128, 128, 128))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFixedPosition(pdf.GetNumberOfPages(), circleX - 20, circleY - 35, 40);
            document.Add(dnaLabel);

            // Texto informativo ao lado direito
            float infoX = circleX + 70;
            float infoY = circleY + 25;

            Paragraph infoTitle = new Paragraph("O que é o índice DNA?")
                .SetFont(fontBold)
                .SetFontSize(14)
                .SetFixedPosition(pdf.GetNumberOfPages(), infoX, infoY, 170);
            document.Add(infoTitle);

            Paragraph infoDesc = new Paragraph("O Score DNA é um sistema que permitirá acompanhar seu rendimento em diferentes momentos.")
                .SetFont(fontRegular)
                .SetFontSize(12)
                .SetFixedPosition(pdf.GetNumberOfPages(), infoX, infoY - 40, 170);
            document.Add(infoDesc);

            // --- SEÇÃO 4: CARDS DE MÉTRICAS (REDESENHADOS) ---
            // Calculando posições para os cards
            float cardsTopY = infoSectionY - infoSectionHeight - 30;
            float cardHeight = 200;
            float cardWidth = (larguraPagina - 60) / 2;
            float leftCardX = 20; // Margem esquerda
            float rightCardX = leftCardX + cardWidth + 20; // Margem entre cards

            // Controle de posicionamento dos cards
            int cardsTop = 0; // Cards na primeira linha
            int cardsBottom = 0; // Cards na segunda linha

            // Card de Saúde
            if (laudo.Laudo.SaudeId != null)
            {
                CriarCardMetrica(document, pdf, "Saúde", laudo.Desempenho.ScoreSaude, corSaude, corSaudeBg,
                    "Índice de massa corporal", laudo.Desempenho.TextoImc, laudo.Laudo.ImcSaude.ToString(),
                    leftCardX, cardsTopY, cardWidth, cardHeight, fontRegular, fontBold, true);
                cardsTop++;
            }

            // Card de Saúde Bucal
            if (laudo.Laudo.EncaminhamentoSaudeBucalId != null)
            {
                float cardX = (cardsTop < 2) ? (cardsTop == 0 ? leftCardX : rightCardX) :
                                              (cardsBottom == 0 ? leftCardX : rightCardX);
                float cardY = (cardsTop < 2) ? cardsTopY : (cardsTopY - cardHeight - 20);

                CriarCardMetrica(document, pdf, "Saúde Bucal", laudo.Desempenho.ScoreSaudeBucal, corSaudeBucal, corSaudeBucalBg,
                    laudo.Desempenho.AvisoSaudeBucal, laudo.EncaminhamentoSaudeBucal.Descricao, null,
                    cardX, cardY, cardWidth, cardHeight, fontRegular, fontBold, false);

                if (cardsTop < 2) cardsTop++;
                else cardsBottom++;
            }

            // Card de Consumo Alimentar
            if (laudo.Laudo.EncaminhamentoConsumoAlimentarId != null)
            {
                float cardX = (cardsTop < 2) ? (cardsTop == 0 ? leftCardX : rightCardX) :
                                              (cardsBottom == 0 ? leftCardX : rightCardX);
                float cardY = (cardsTop < 2) ? cardsTopY : (cardsTopY - cardHeight - 20);

                CriarCardMetrica(document, pdf, "Consumo Alimentar", laudo.Desempenho.ScoreConsumoAlimentar,
                    corConsumoAlimentar, corConsumoAlimentarBg,
                    laudo.Desempenho.AvisoConsumoAlimentar,
                    laudo.EncaminhamentoConsumoAlimentar == null ? "" : laudo.EncaminhamentoConsumoAlimentar.Descricao, null,
                    cardX, cardY, cardWidth, cardHeight, fontRegular, fontBold, false);

                if (cardsTop < 2) cardsTop++;
                else cardsBottom++;
            }

            // Card de Vocação
            if (laudo.Laudo.VocacionalId != null)
            {
                foreach (var vocacional in laudo.ListVocacional)
                {
                    if (vocacional.Id == laudo.Laudo.VocacionalId)
                    {
                        float cardX = (cardsTop < 2) ? (cardsTop == 0 ? leftCardX : rightCardX) :
                                                      (cardsBottom == 0 ? leftCardX : rightCardX);
                        float cardY = (cardsTop < 2) ? cardsTopY : (cardsTopY - cardHeight - 20);

                        CriarCardMetrica(document, pdf, "Vocação", 100, corVocacao, corVocacaoBg,
                            vocacional.Nome, vocacional.Descricao, null,
                            cardX, cardY, cardWidth, cardHeight, fontRegular, fontBold, false);

                        if (cardsTop < 2) cardsTop++;
                        else cardsBottom++;

                        break; // Apenas um vocacional
                    }
                }
            }

            // --- RODAPÉ: DATA ---
            Paragraph dateParagraph = new Paragraph(DateTime.Now.ToString("D", CultureInfo.CreateSpecificCulture("pt-BR")).ToUpper())
                .SetFont(fontRegular)
                .SetFontSize(12)
                .SetFontColor(new DeviceRgb(102, 102, 102))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFixedPosition(pdf.GetNumberOfPages(), 0, 20, larguraPagina);
            document.Add(dateParagraph);
        }

        /// <summary>
        /// Cria a segunda página do documento PDF com informações de qualidade de vida e esporte.
        /// </summary>
        /// <param name="document">Documento PDF onde a página será adicionada.</param>
        /// <param name="pdf">Documento PDF.</param>
        /// <param name="laudo">Modelo de laudo contendo as informações a serem exibidas.</param>
        /// <param name="fontRegular">Fonte regular para o texto.</param>
        /// <param name="fontBold">Fonte em negrito para o texto.</param>
        /// <param name="corHeaderBg">Cor de fundo do cabeçalho.</param>
        /// <param name="corVida">Cor principal da seção de qualidade de vida.</param>
        /// <param name="corVidaBg">Cor de fundo da seção de qualidade de vida.</param>
        /// <param name="corEsporte">Cor principal da seção de esporte.</param>
        /// <param name="corEsporteBg">Cor de fundo da seção de esporte.</param>
        private void CriarSegundaPagina(Document document, PdfDocument pdf, LaudoModel laudo,
            PdfFont fontRegular, PdfFont fontBold,
            DeviceRgb corHeaderBg, DeviceRgb corVida, DeviceRgb corVidaBg, DeviceRgb corEsporte, DeviceRgb corEsporteBg)
        {
            // Adicionar nova página
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            // Definir dimensões da página
            float larguraPagina = pdf.GetDefaultPageSize().GetWidth();
            float alturaPagina = pdf.GetDefaultPageSize().GetHeight();

            // Seção do cabeçalho (header)
            Rectangle headerRect = new Rectangle(0, alturaPagina - 120, larguraPagina, 120);
            PdfCanvas headerCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));

            // Criar gradiente do header (simplificado com cor sólida)
            headerCanvas.SaveState()
                .SetFillColor(corHeaderBg)
                .Rectangle(headerRect)
                .Fill()
                .RestoreState();

            // Adicionar logo DNA
            string logoPath = Path.Combine(_host.WebRootPath, "assets/images/logo2.png");
            if (System.IO.File.Exists(logoPath))
            {
                ImageData logoImageData = ImageDataFactory.Create(logoPath);
                iText.Layout.Element.Image logoImage = new iText.Layout.Element.Image(logoImageData)
                    .ScaleToFit(150, 81)
                    .SetFixedPosition(pdf.GetNumberOfPages(), 20, alturaPagina - 100);
                document.Add(logoImage);
            }

            // Adicionar informações do talento
            float inicioLadoDireito = larguraPagina - 250;
            Rectangle talentRect = new Rectangle(inicioLadoDireito, alturaPagina - 110, 230, 95);
            Canvas talentCanvas = new Canvas(headerCanvas, talentRect, true);

            // Label do talento
            Paragraph talentLabel = new Paragraph("MEU TALENTO")
                .SetFont(fontRegular)
                .SetFontSize(18)
                .SetFontColor(ColorConstants.WHITE);
            talentCanvas.Add(talentLabel);

            // Nome do talento
            string talentName = laudo.Modalidade.Nome ?? "Não Definido";
            Paragraph talentNameParagraph = new Paragraph(talentName)
                .SetFont(fontBold)
                .SetFontSize(35)
                .SetFontColor(ColorConstants.WHITE);
            talentCanvas.Add(talentNameParagraph);

            // Ícone do esporte
            if (laudo.Modalidade.ByteImage != null)
            {
                ImageData sportIconData = ImageDataFactory.Create(laudo.Modalidade.ByteImage);
                iText.Layout.Element.Image sportIcon = new iText.Layout.Element.Image(sportIconData)
                    .ScaleToFit(69, 95)
                    .SetFixedPosition(pdf.GetNumberOfPages(), larguraPagina - 90, alturaPagina - 110);
                document.Add(sportIcon);
            }
            else
            {
                string defaultIconPath = Path.Combine(_host.WebRootPath, "assets/assets_Laudo/icon_info_esporte.png");
                if (System.IO.File.Exists(defaultIconPath))
                {
                    ImageData defaultIconData = ImageDataFactory.Create(defaultIconPath);
                    iText.Layout.Element.Image defaultIcon = new iText.Layout.Element.Image(defaultIconData)
                        .ScaleToFit(69, 95)
                        .SetFixedPosition(pdf.GetNumberOfPages(), larguraPagina - 90, alturaPagina - 110);
                    document.Add(defaultIcon);
                }
            }

            // Conteúdo principal da página
            float inicioConteudo = alturaPagina - 150;
            float margemLateral = 20;
            float larguraConteudo = larguraPagina - (2 * margemLateral);

            // Seção de Qualidade de Vida
            if (laudo.Laudo.QualidadeDeVidaId != null)
            {
                float qualidadeVidaY = inicioConteudo;

                // Cabeçalho da seção
                CriarCabecalhoSecao(document, pdf, "Qualidade de Vida", laudo.Desempenho.ScoreVida, corVida, corVidaBg,
                    margemLateral, qualidadeVidaY, larguraConteudo, 60, fontRegular, fontBold);

                // Grid de cartões de qualidade de vida
                float cardWidth = (larguraConteudo - 30) / 2;
                float cardHeight = 120;
                float startY = qualidadeVidaY - 80;

                var textosVida = new List<string>
                {
                    laudo.Desempenho.TextoBemEstar,
                    laudo.Desempenho.TextoAutoestima,
                    laudo.Desempenho.TextoFamilia,
                    laudo.Desempenho.TextoContexto
                };

                for (var i = 0; i < laudo.ListQualidadeDeVida.Count && i < 4; i++)
                {
                    float cardX = margemLateral + (i % 2 == 0 ? 0 : cardWidth + 10);
                    float cardY = startY - (i / 2 * (cardHeight + 10));

                    CriarCardQualidade(document, pdf, laudo.ListQualidadeDeVida[i].Nome, textosVida[i],
                        cardX, cardY, cardWidth, cardHeight, fontRegular, fontBold);
                }
            }

            // Seção de Esporte
            if (laudo.Laudo.TalentoEsportivoId != null)
            {
                float esporteY = laudo.Laudo.QualidadeDeVidaId != null ?
                    inicioConteudo - 350 : inicioConteudo;

                // Cabeçalho da seção
                CriarCabecalhoSecao(document, pdf, "Esporte", laudo.Desempenho.ScoreTalentoEsportivo, corEsporte, corEsporteBg,
                    margemLateral, esporteY, larguraConteudo, 60, fontRegular, fontBold);

                // Grid de cartões de esporte
                float sportCardWidth = (larguraConteudo - 40) / 3;
                float sportCardHeight = 120;
                float sportStartY = esporteY - 80;

                // IMC
                CriarCardEsporte(document, pdf, "Índice de Massa Corporal", "imc",
                    laudo.Laudo.ImcSaude.ToString(), laudo.Desempenho.AvisoImc, laudo.Desempenho.TextoImc,
                    margemLateral, sportStartY, sportCardWidth, sportCardHeight, fontRegular, fontBold, true, GetProgressBarWidth("imc", laudo.Laudo.ImcSaude.ToString()));

                // Velocidade
                CriarCardEsporte(document, pdf, "Velocidade 20m", "seg",
                    laudo.TalentoEsportivo.Velocidade.ToString(), "", laudo.Desempenho.TextoVelocidade,
                    margemLateral + sportCardWidth + 10, sportStartY, sportCardWidth, sportCardHeight, fontRegular, fontBold, false, "0%");

                // Impulsão
                CriarCardEsporte(document, pdf, "Impulsão Horizontal", "cm",
                    laudo.TalentoEsportivo.ImpulsaoHorizontal.ToString(), laudo.Desempenho.AvisoImpulsao, laudo.Desempenho.TextoImpulsao,
                    margemLateral + 2 * (sportCardWidth + 10), sportStartY, sportCardWidth, sportCardHeight, fontRegular, fontBold, true,
                    GetProgressBarWidth("Impulsão Horizontal", laudo.TalentoEsportivo.ImpulsaoHorizontal.ToString()));

                // Segunda linha de cartões
                float sportSecondRowY = sportStartY - sportCardHeight - 20;

                // Flexibilidade
                CriarCardEsporte(document, pdf, "Flexibilidade", "cm",
                    laudo.TalentoEsportivo.Flexibilidade.ToString(), laudo.Desempenho.AvisoFlexibilidadeMuscular, laudo.Desempenho.TextoFlexibilidadeMuscular,
                    margemLateral, sportSecondRowY, sportCardWidth, sportCardHeight, fontRegular, fontBold, true,
                    GetProgressBarWidth("Flexibilidade", laudo.TalentoEsportivo.Flexibilidade.ToString()));

                // Aptidão Aeróbica
                CriarCardEsporte(document, pdf, "Aptidão Aeróbica", "seg",
                    laudo.TalentoEsportivo.Vo2Max.ToString(), "", laudo.Desempenho.TextoAptidaoCardio,
                    margemLateral + sportCardWidth + 10, sportSecondRowY, sportCardWidth, sportCardHeight, fontRegular, fontBold, false, "0%");

                // Preensão Manual
                CriarCardEsporte(document, pdf, "Preensão Manual", "kg",
                    laudo.TalentoEsportivo.PreensaoManual.ToString(), laudo.Desempenho.AvisoForcaMembrosSup, laudo.Desempenho.TextoForcaMembrosSup,
                    margemLateral + 2 * (sportCardWidth + 10), sportSecondRowY, sportCardWidth, sportCardHeight, fontRegular, fontBold, true,
                    GetProgressBarWidth("Preensão Manual", laudo.TalentoEsportivo.PreensaoManual.ToString()));
            }

            // Adicionar data no rodapé
            Paragraph dateParagraph = new Paragraph(DateTime.Now.ToString("D", CultureInfo.CreateSpecificCulture("pt-BR")).ToUpper())
                .SetFont(fontRegular)
                .SetFontSize(12)
                .SetFontColor(new DeviceRgb(102, 102, 102))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFixedPosition(pdf.GetNumberOfPages(), 0, 20, larguraPagina);
            document.Add(dateParagraph);
        }

        /// <summary>
        /// Cria um card de métrica para exibir informações do laudo.
        /// </summary>
        /// <param name="document">Documento PDF onde o card será adicionado.</param>
        /// <param name="pdf">Documento PDF.</param>
        /// <param name="titulo">Título do card.</param>
        /// <param name="score">Pontuação da métrica.</param>
        /// <param name="corPrincipal">Cor principal do card.</param>
        /// <param name="corSecundaria">Cor secundária do card.</param>
        /// <param name="mensagem">Mensagem principal do card.</param>
        /// <param name="descricao">Descrição detalhada da métrica.</param>
        /// <param name="valorImc">Valor do IMC, se aplicável.</param>
        /// <param name="x">Posição X do card.</param>
        /// <param name="y">Posição Y do card.</param>
        /// <param name="largura">Largura do card.</param>
        /// <param name="altura">Altura do card.</param>
        /// <param name="fontRegular">Fonte regular para o texto.</param>
        /// <param name="fontBold">Fonte em negrito para o texto.</param>
        /// <param name="mostrarBarraImc">Indica se a barra de progresso do IMC deve ser exibida.</param>
        private void CriarCardMetrica(Document document, PdfDocument pdf, string titulo, double score,
            DeviceRgb corPrincipal, DeviceRgb corSecundaria, string mensagem, string descricao, string valorImc,
            float x, float y, float largura, float altura, PdfFont fontRegular, PdfFont fontBold, bool mostrarBarraImc)
        {
            int pageNum = pdf.GetNumberOfPages();

            // --- ESTRUTURA BÁSICA DO CARD ---
            PdfCanvas cardCanvas = new PdfCanvas(pdf.GetPage(pageNum));

            // Sombra (efeito discreto)
            cardCanvas.SaveState()
                .SetFillColor(new DeviceRgb(220, 220, 220))
                .RoundRectangle(x + 3, y - altura - 3, largura, altura, 8)
                .Fill()
                .RestoreState();

            // Fundo branco do card
            cardCanvas.SaveState()
                .SetFillColor(ColorConstants.WHITE)
                .RoundRectangle(x, y - altura, largura, altura, 8)
                .Fill()
                .RestoreState();

            // --- CABEÇALHO DO CARD ---
            float headerHeight = 50;
            float headerMargin = 12;

            // Fundo cinza do cabeçalho
            cardCanvas.SaveState()
                .SetFillColor(new DeviceRgb(244, 244, 244))
                .RoundRectangle(x + headerMargin, y - headerHeight, largura - (2 * headerMargin), headerHeight, 8)
                .Fill()
                .RestoreState();

            // Título (à esquerda no cabeçalho)
            Paragraph titleParagraph = new Paragraph(titulo)
                .SetFont(fontBold)
                .SetFontSize(24)
                .SetFontColor(corPrincipal)
                .SetFixedPosition(pageNum, x + headerMargin + 10, y - headerHeight + 15, largura - 100);
            document.Add(titleParagraph);

            // --- CÍRCULO DE PROGRESSO (NO CABEÇALHO) ---
            float circleX = x + largura - headerMargin - 30;
            float circleY = y - (headerHeight / 2);
            float circleRadius = 20;

            // Fundo do círculo
            cardCanvas.SaveState()
                .SetStrokeColor(corSecundaria)
                .SetLineWidth(5)
                .Circle(circleX, circleY, circleRadius)
                .Stroke()
                .RestoreState();

            // Arco de progresso
            float scoreRatio = (float)(score / 100.0);
            if (scoreRatio > 0)
            {
                cardCanvas.SaveState()
                    .SetStrokeColor(corPrincipal)
                    .SetLineWidth(5);

                // Ângulos para o arco (270° é o topo)
                float startAngle = 270;
                float endAngle = startAngle + (scoreRatio * 360);

                if (Math.Abs(scoreRatio - 1.0) < 0.001)
                {
                    // Círculo completo para 100%
                    cardCanvas.Circle(circleX, circleY, circleRadius).Stroke();
                }
                else
                {
                    // Arco parcial
                    cardCanvas.Arc(circleX - circleRadius, circleY - circleRadius,
                                  circleX + circleRadius, circleY + circleRadius,
                                  startAngle, endAngle - startAngle).Stroke();
                }

                cardCanvas.RestoreState();
            }

            // Valor do score dentro do círculo
            Paragraph scoreText = new Paragraph(score.ToString("0"))
                .SetFont(fontBold)
                .SetFontSize(18)
                .SetFontColor(corPrincipal)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFixedPosition(pageNum, circleX - 15, circleY - 9, 30);
            document.Add(scoreText);

            // --- CONTEÚDO DO CARD ---
            float contentX = x + 20; // Margem lateral para o conteúdo
            float contentWidth = largura - 40; // Largura do conteúdo
            float contentY = y - headerHeight - 15; // Posição inicial do conteúdo

            // Mensagem principal (subtítulo)
            if (!string.IsNullOrEmpty(mensagem))
            {
                Paragraph messageParagraph = new Paragraph(mensagem)
                    .SetFont(fontBold)
                    .SetFontSize(18)
                    .SetFixedPosition(pageNum, contentX, contentY, contentWidth);
                document.Add(messageParagraph);
                contentY -= 30; // Espaço após o subtítulo
            }

            // Barra de IMC (se aplicável)
            if (mostrarBarraImc && !string.IsNullOrEmpty(valorImc))
            {
                // Container para rótulo e valor do IMC
                float imcLabelY = contentY;

                // Rótulo "IMC"
                Paragraph imcLabel = new Paragraph("IMC")
                    .SetFont(fontRegular)
                    .SetFontSize(14)
                    .SetFixedPosition(pageNum, contentX, imcLabelY, 40);
                document.Add(imcLabel);

                // Valor do IMC (à direita, na cor principal)
                Paragraph imcValue = new Paragraph(valorImc)
                    .SetFont(fontBold)
                    .SetFontSize(14)
                    .SetFontColor(corPrincipal)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFixedPosition(pageNum, contentX + contentWidth - 40, imcLabelY, 40);
                document.Add(imcValue);

                contentY -= 20; // Espaço antes da barra

                // Barra de progresso
                float barHeight = 10;
                float barY = contentY - barHeight;

                // Fundo da barra (cor secundária)
                cardCanvas.SaveState()
                    .SetFillColor(corSecundaria)
                    .RoundRectangle(contentX, barY, contentWidth, barHeight, 5)
                    .Fill()
                    .RestoreState();

                // Progresso (cor principal)
                string progressValue = GetProgressBarWidth("imc", valorImc);
                float percentValue = 0;

                if (!string.IsNullOrEmpty(progressValue) && progressValue.EndsWith("%"))
                {
                    // Converter a string de porcentagem para float
                    if (float.TryParse(progressValue.Replace("%", "").Trim(),
                        NumberStyles.Any, CultureInfo.InvariantCulture, out percentValue))
                    {
                        percentValue = Math.Min(percentValue / 100f, 1f);
                        float progressWidth = contentWidth * percentValue;

                        // Desenhar a parte preenchida da barra
                        cardCanvas.SaveState()
                            .SetFillColor(corPrincipal)
                            .RoundRectangle(contentX, barY, progressWidth, barHeight, 5)
                            .Fill()
                            .RestoreState();
                    }
                }

                contentY -= barHeight + 25; // Espaço após a barra
            }

            // Texto de descrição
            if (!string.IsNullOrEmpty(descricao))
            {
                // Calcular altura disponível restante
                float maxDescHeight = (y - altura + 20) - contentY; // +20 para margem inferior

                Paragraph descParagraph = new Paragraph(descricao)
                    .SetFont(fontRegular)
                    .SetFontSize(10)
                    .SetFixedPosition(pageNum, contentX, contentY, contentWidth);

                // Adicionar texto de descrição
                document.Add(descParagraph);
            }
        }

        /// <summary>
        /// Cria um card de qualidade para exibir informações do laudo na segunda página do PDF.
        /// </summary>
        /// <param name="document">Documento PDF onde o card será adicionado.</param>
        /// <param name="pdf">Documento PDF.</param>
        /// <param name="titulo">Título do card.</param>
        /// <param name="descricao">Descrição do card.</param>
        /// <param name="x">Posição X do card.</param>
        /// <param name="y">Posição Y do card.</param>
        /// <param name="largura">Largura do card.</param>
        /// <param name="altura">Altura do card.</param>
        /// <param name="fontRegular">Fonte regular para o texto.</param>
        /// <param name="fontBold">Fonte em negrito para o texto.</param>
        private void CriarCardQualidade(Document document, PdfDocument pdf, string titulo, string descricao,
            float x, float y, float largura, float altura, PdfFont fontRegular, PdfFont fontBold)
        {
            // Fundo do card
            PdfCanvas cardCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
            cardCanvas.SaveState()
                .SetFillColor(ColorConstants.WHITE)
                .RoundRectangle(x, y - altura, largura, altura, 8)
                .Fill()
                .RestoreState();

            // Adicionar sombra (simulação simples)
            cardCanvas.SaveState()
                .SetStrokeColor(new DeviceRgb(220, 220, 220))
                .SetLineWidth(1)
                .RoundRectangle(x, y - altura, largura, altura, 8)
                .Stroke()
                .RestoreState();

            // Título do card
            Paragraph titleParagraph = new Paragraph(titulo)
                .SetFont(fontBold)
                .SetFontSize(16)
                .SetFixedPosition(pdf.GetNumberOfPages(), x + 10, y - 20, largura - 20);
            document.Add(titleParagraph);

            // Descrição
            if (!string.IsNullOrEmpty(descricao))
            {
                Paragraph descParagraph = new Paragraph(descricao)
                    .SetFont(fontRegular)
                    .SetFontSize(12)
                    .SetFixedPosition(pdf.GetNumberOfPages(), x + 10, y - 45, largura - 20);
                document.Add(descParagraph);
            }
        }

        /// <summary>
        /// Método para criar cartão de esporte (para segunda página)
        /// </summary>
        /// <param name="document">Documento PDF onde o cartão será adicionado.</param>
        /// <param name="pdf">Documento PDF.</param>
        /// <param name="titulo">Título do cartão.</param>
        /// <param name="unidade">Unidade de medida do valor.</param>
        /// <param name="valor">Valor métrico a ser exibido no cartão.</param>
        /// <param name="mensagem">Mensagem adicional a ser exibida no cartão.</param>
        /// <param name="descricao">Descrição detalhada do cartão.</param>
        /// <param name="x">Posição X do cartão.</param>
        /// <param name="y">Posição Y do cartão.</param>
        /// <param name="largura">Largura do cartão.</param>
        /// <param name="altura">Altura do cartão.</param>
        /// <param name="fontRegular">Fonte regular para o texto.</param>
        /// <param name="fontBold">Fonte em negrito para o texto.</param>
        /// <param name="mostrarBarra">Indica se a barra de progresso deve ser exibida.</param>
        /// <param name="valorBarra">Valor da barra de progresso.</param>
        private void CriarCardEsporte(Document document, PdfDocument pdf, string titulo, string unidade,
            string valor, string mensagem, string descricao, float x, float y, float largura, float altura,
            PdfFont fontRegular, PdfFont fontBold, bool mostrarBarra, string valorBarra)
        {
            // Fundo do card
            PdfCanvas cardCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
            cardCanvas.SaveState()
                .SetFillColor(ColorConstants.WHITE)
                .RoundRectangle(x, y - altura, largura, altura, 8)
                .Fill()
                .RestoreState();

            // Adicionar sombra (simulação simples)
            cardCanvas.SaveState()
                .SetStrokeColor(new DeviceRgb(220, 220, 220))
                .SetLineWidth(1)
                .RoundRectangle(x, y - altura, largura, altura, 8)
                .Stroke()
                .RestoreState();

            // Título do card
            Paragraph titleParagraph = new Paragraph(titulo)
                .SetFont(fontBold)
                .SetFontSize(16)
                .SetFixedPosition(pdf.GetNumberOfPages(), x + 10, y - 20, largura - 20);
            document.Add(titleParagraph);

            // Mensagem (se houver)
            float currentY = y - 25;
            if (!string.IsNullOrEmpty(mensagem))
            {
                currentY -= 15;
                Paragraph messageParagraph = new Paragraph(mensagem)
                    .SetFont(fontRegular)
                    .SetFontSize(12)
                    .SetFixedPosition(pdf.GetNumberOfPages(), x + 10, currentY, largura - 20);
                document.Add(messageParagraph);
            }

            // Valor métrico
            currentY -= 25;

            // Unidade
            Paragraph unitParagraph = new Paragraph(unidade)
                .SetFont(fontRegular)
                .SetFontSize(14)
                .SetFixedPosition(pdf.GetNumberOfPages(), x + 10, currentY, 40);
            document.Add(unitParagraph);

            // Valor
            DeviceRgb corValor = titulo.Contains("Impulsão") ?
                new DeviceRgb(223, 67, 108) : // cor esporte
                new DeviceRgb(0, 234, 188);   // cor padrão

            Paragraph valueParagraph = new Paragraph(valor)
                .SetFont(fontBold)
                .SetFontSize(14)
                .SetFontColor(corValor)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFixedPosition(pdf.GetNumberOfPages(), x + largura - 50, currentY, 40);
            document.Add(valueParagraph);

            // Borda inferior (linha)
            if (!mostrarBarra)
            {
                cardCanvas.SaveState()
                    .SetStrokeColor(new DeviceRgb(238, 238, 238))
                    .SetLineWidth(1)
                    .MoveTo(x + 10, currentY - 5)
                    .LineTo(x + largura - 10, currentY - 5)
                    .Stroke()
                    .RestoreState();
            }

            // Barra de progresso (se aplicável)
            if (mostrarBarra)
            {
                DeviceRgb corBarraBg = titulo.Contains("Impulsão") ?
                    new DeviceRgb(249, 193, 193) : // #f9c1c1
                    new DeviceRgb(192, 250, 238);  // #c0faee

                DeviceRgb corBarraProgresso = titulo.Contains("Impulsão") ?
                    new DeviceRgb(223, 67, 108) : // #df436c
                    new DeviceRgb(0, 234, 188);   // #00eabc

                CriarBarraProgresso(pdf, x + 10, currentY - 15, largura - 20, 10,
                    corBarraBg, corBarraProgresso, valorBarra);
            }

            // Descrição
            currentY -= mostrarBarra ? 30 : 15;

            if (!string.IsNullOrEmpty(descricao))
            {
                Paragraph descParagraph = new Paragraph(descricao)
                    .SetFont(fontRegular)
                    .SetFontSize(12)
                    .SetFixedPosition(pdf.GetNumberOfPages(), x + 10, currentY, largura - 20);
                document.Add(descParagraph);
            }
            else
            {
                Paragraph defaultDescParagraph = new Paragraph("O aluno não tem essa informação preenchida no laudo de Esporte.")
                    .SetFont(fontRegular)
                    .SetFontSize(12)
                    .SetFixedPosition(pdf.GetNumberOfPages(), x + 10, currentY, largura - 20);
                document.Add(defaultDescParagraph);
            }
        }

        /// <summary>
        /// Cria o cabeçalho de uma seção no documento PDF.
        /// </summary>
        /// <param name="document">Documento PDF onde o cabeçalho será adicionado.</param>
        /// <param name="pdf">Documento PDF.</param>
        /// <param name="titulo">Título da seção.</param>
        /// <param name="score">Pontuação da seção.</param>
        /// <param name="corPrincipal">Cor principal do cabeçalho.</param>
        /// <param name="corSecundaria">Cor secundária do cabeçalho.</param>
        /// <param name="x">Posição X do cabeçalho.</param>
        /// <param name="y">Posição Y do cabeçalho.</param>
        /// <param name="largura">Largura do cabeçalho.</param>
        /// <param name="altura">Altura do cabeçalho.</param>
        /// <param name="fontRegular">Fonte regular para o texto.</param>
        /// <param name="fontBold">Fonte em negrito para o texto.</param>
        private void CriarCabecalhoSecao(Document document, PdfDocument pdf, string titulo, double score,
            DeviceRgb corPrincipal, DeviceRgb corSecundaria, float x, float y, float largura, float altura,
            PdfFont fontRegular, PdfFont fontBold)
        {
            // Fundo do header
            PdfCanvas headerCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
            headerCanvas.SaveState()
                .SetFillColor(new DeviceRgb(244, 244, 244))
                .RoundRectangle(x, y - altura, largura, altura, 8)
                .Fill()
                .RestoreState();

            // Título da seção
            Paragraph titleParagraph = new Paragraph(titulo)
                .SetFont(fontBold)
                .SetFontSize(24)
                .SetFontColor(corPrincipal)
                .SetFixedPosition(pdf.GetNumberOfPages(), x + 20, y - 35, largura - 90);
            document.Add(titleParagraph);

            // Desenhar círculo de score
            DesenharCirculoProgresso(pdf, score, corPrincipal, corSecundaria,
                x + largura - 50, y - 30, 25, fontBold);
        }

        /// <summary>
        /// Desenha um círculo de progresso no PDF.
        /// </summary>
        /// <param name="pdf">Documento PDF onde o círculo será desenhado.</param>
        /// <param name="score">Pontuação a ser exibida no círculo.</param>
        /// <param name="corPrincipal">Cor principal do círculo.</param>
        /// <param name="corSecundaria">Cor secundária do círculo.</param>
        /// <param name="x">Posição X do círculo.</param>
        /// <param name="y">Posição Y do círculo.</param>
        /// <param name="raio">Raio do círculo.</param>
        /// <param name="fontBold">Fonte em negrito para o texto do score.</param>
        private void DesenharCirculoProgresso(PdfDocument pdf, double score, DeviceRgb corPrincipal,
            DeviceRgb corSecundaria, float x, float y, float raio, PdfFont fontBold)
        {
            float scoreRatio = (float)(score / 100.0);

            PdfCanvas circleCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));

            // Círculo de fundo
            circleCanvas.SaveState()
                .SetStrokeColor(corSecundaria)
                .SetLineWidth(5)
                .Circle(x, y, raio)
                .Stroke()
                .RestoreState();

            // Arco de progresso
            if (scoreRatio > 0)
            {
                circleCanvas.SaveState()
                    .SetStrokeColor(corPrincipal)
                    .SetLineWidth(5);

                // Desenhar o arco de progresso
                float startAngle = 270;
                float endAngle = startAngle + (scoreRatio * 360);

                // Tratamento especial para círculo completo
                if (Math.Abs(scoreRatio - 1.0) < 0.001)
                {
                    circleCanvas.Circle(x, y, raio).Stroke();
                }
                else
                {
                    // Corrigindo o método Arc para a versão 7.2.5
                    float startAngleRad = (float)(startAngle * Math.PI / 180);
                    float extentRad = (float)((endAngle - startAngle) * Math.PI / 180);
                    circleCanvas.Arc(x - raio, y - raio, x + raio, y + raio, startAngleRad, extentRad);
                    circleCanvas.Stroke();
                }

                circleCanvas.RestoreState();
            }

            // Adicionar texto do score
            Paragraph scoreText = new Paragraph(score.ToString("0"))
                .SetFont(fontBold)
                .SetFontSize(16)
                .SetFontColor(corPrincipal)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFixedPosition(pdf.GetNumberOfPages(), x - 15, y - 8, 30);

            // Corrigindo a criação do Canvas
            Rectangle rectangle = new Rectangle(x - 15, y - 8, 30, 20);
            Canvas scoreCanvas = new Canvas(new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages())), rectangle);
            scoreCanvas.Add(scoreText);
            scoreCanvas.Close();
        }

        /// <summary>  
        /// Método para criar barra de progresso no PDF.  
        /// </summary>  
        /// <param name="pdf">Documento PDF onde a barra será desenhada.</param>  
        /// <param name="x">Posição X da barra.</param>  
        /// <param name="y">Posição Y da barra.</param>  
        /// <param name="largura">Largura da barra.</param>  
        /// <param name="altura">Altura da barra.</param>  
        /// <param name="corFundo">Cor de fundo da barra.</param>  
        /// <param name="corProgresso">Cor do progresso da barra.</param>  
        /// <param name="porcentagem">Porcentagem de progresso a ser exibida.</param>  
        private void CriarBarraProgresso(PdfDocument pdf, float x, float y, float largura, float altura,
           DeviceRgb corFundo, DeviceRgb corProgresso, string porcentagem)
        {
            // Converter a porcentagem para valor de 0 a 1  
            float percentValue = 0;
            if (!string.IsNullOrEmpty(porcentagem) && porcentagem.EndsWith("%"))
            {
                float.TryParse(porcentagem.Replace("%", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out percentValue);
                percentValue = Math.Min(percentValue / 100f, 1f);
            }

            // Desenhar o fundo da barra  
            PdfCanvas barCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
            barCanvas.SaveState()
                .SetFillColor(corFundo)
                .RoundRectangle(x, y, largura, altura, 5)
                .Fill()
                .RestoreState();

            // Desenhar a parte de progresso  
            if (percentValue > 0)
            {
                float progressWidth = largura * percentValue;

                barCanvas.SaveState()
                    .SetFillColor(corProgresso)
                    .RoundRectangle(x, y, progressWidth, altura, 5)
                    .Fill()
                    .RestoreState();
            }
        }

        /// <summary>
        /// Método para calcular a largura da barra de progresso com base no título e valor fornecidos.
        /// </summary>
        /// <param name="title">Título da métrica.</param>
        /// <param name="value">Valor da métrica.</param>
        /// <returns>Retorna uma string representando a largura da barra de progresso em porcentagem.</returns>
        private string GetProgressBarWidth(string title, string value)
        {
            if (string.IsNullOrEmpty(value)) return "0%";

            double numValue;
            if (!double.TryParse(value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out numValue))
                return "0%";

            double percentage = 0;
            switch (title)
            {
                case "Impulsão Horizontal":
                    percentage = (numValue / 250.0) * 100;
                    break;
                case "Preensão Manual":
                    percentage = (numValue / 80.0) * 100;
                    break;
                case "Flexibilidade":
                    percentage = (numValue / 455.0) * 100;
                    break;
                case "Índice de Massa Corporal":
                case "imc":
                    percentage = (numValue / 37.26) * 100;
                    percentage = Math.Min(percentage, 100);
                    break;
                default:
                    percentage = 0;
                    break;
            }

            return $"{percentage.ToString("F1", CultureInfo.InvariantCulture)}%";
        }

        [ClaimsAuthorize(ClaimType.Laudo, Claim.Consultar)]
        public async Task<IActionResult> ExportLaudo(
    [FromQuery] string ddlFomento, [FromQuery] string ddlEstado,
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
                    PageSize = 1000
                };

                var result = await ApiClientFactory.Instance.GetLaudosByFilter(searchFilter);

                var workbook = new XLWorkbook();
                workbook.AddWorksheet("sheetName");
                var ws = workbook.Worksheet("sheetName");
                ws.Cell(1, 1).Value = "Laudo Id";
                ws.Cell(1, 2).Value = "Idade";
                ws.Cell(1, 3).Value = "Matricula";
                ws.Cell(1, 4).Value = "Aluno";
                ws.Cell(1, 5).Value = "Localidade";
                ws.Cell(1, 6).Value = "Email";
                ws.Cell(1, 7).Value = "Telefone";
                ws.Cell(1, 8).Value = "Celular";
                ws.Cell(1, 9).Value = "Saúde";
                ws.Cell(1, 10).Value = "Talento Esportivo";
                ws.Cell(1, 11).Value = "Consumo Alimentar";
                ws.Cell(1, 12).Value = "Saúde Bucal";
                ws.Cell(1, 13).Value = "Qualidade de Vida";
                ws.Cell(1, 14).Value = "Vocacional";
                ws.Cell(1, 15).Value = "Finalizado";
                int row = 2;
                foreach (var item in result.Laudos.Items.ToList())
                {
                    ws.Cell("A" + row).Value = item.Id;
                    ws.Cell("B" + row).Value = item.Idade;
                    ws.Cell("C" + row).Value = item.AlunoId;
                    ws.Cell("D" + row).Value = item.NomeAluno;
                    ws.Cell("E" + row).Value = item.NomeLocalidade;
                    ws.Cell("F" + row).Value = item.Email;
                    ws.Cell("F" + row).Value = item.Telefone;
                    ws.Cell("H" + row).Value = item.Celular;
                    ws.Cell("I" + row).Value = item.SaudeId != null ? "X" : "" ;
                    ws.Cell("J" + row).Value = item.TalentoEsportivoId != null ? "X" : "" ;
                    ws.Cell("K" + row).Value = item.ConsumoAlimentarId != null ? "X" : "" ;
                    ws.Cell("L" + row).Value = item.SaudeBucalId != null ? "X" : "" ;
                    ws.Cell("M" + row).Value = item.QualidadeDeVidaId != null ? "X" : "" ;
                    ws.Cell("N" + row).Value = item.VocacionalId != null ? "X" : "" ;
                    ws.Cell("O" + row).Value = item.StatusLaudo;
                    row++;
                }

                var exportPath = Path.Combine(_host.WebRootPath, "Exportacao");
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

    }
}
