using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;
using WebApp.Authorization;
using Claim = WebApp.Identity.Claim;
using WebApp.Identity;
using Microsoft.AspNetCore.Authorization;
using WebApp.Dto;
using ClosedXML.Excel;
using System.Diagnostics;
using DocumentFormat.OpenXml.Presentation;
using log4net;

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
                            : decimal.Parse(collection["massaCorporalSaude"].ToString().Replace(".", ","),
                                System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
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

                    command.SaudeId = (int)await ApiClientFactory.Instance.CreateSaude(commandSaude);
                }

                if (laudo.TalentoEsportivoId != null)
                {
                    var talentoEsportivo = ApiClientFactory.Instance.GetTalentoEsportivoById((int)laudo.TalentoEsportivoId);

                    var commandTalentoEsportivo = new TalentoEsportivoModel.CreateUpdateTalentoEsportivoCommand()
                    {
                        Id = (int)laudo.TalentoEsportivoId,
                        ProfissionalId = talentoEsportivo.ProfissionalId,
                        AlunoId = (int)laudo.AlunoId,
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

    }
}
