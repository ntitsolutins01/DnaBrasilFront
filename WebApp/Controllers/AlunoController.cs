using System.Diagnostics;
using System.Drawing;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Spreadsheet;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol;
using QRCoder;
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
using Rectangle = iText.Kernel.Geom.Rectangle;
using Text = iText.Layout.Element.Text;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controle de Aluno
    /// </summary>
    //[Authorize(Policy = ModuloAccess.Aluno)]
    //[Authorize(Policy = ModuloAccess.ProfileAluno)]
    public class AlunoController : BaseController
    {
        #region Parametros

        private readonly IWebHostEnvironment _host;
        private readonly ILog _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        /// <param name="host">informações da aplicação em execução</param>
        /// <param name="logger">Log de mensagens da aplicação</param>
        /// <param name="userManager">Gerenciador de Usuario</param>
        /// <param name="emailSender">imlementacao de infraestrutura de identidade possa enviar emails de confirmação e redefinição de senha.</param>
        /// <param name="roleManager">gerenciador de regras de permissoes</param>
        public AlunoController(IOptions<UrlSettings> appSettings,
            IWebHostEnvironment host,
            ILog logger,
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
            _host = host;
            _logger = logger;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }
        #endregion

        #region Main Methods
        /// <summary>
        /// Listagem de Alunos
        /// </summary>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Consultar)]
        [HttpGet]
        public async Task<ActionResult> Index(int? crud, int? notify, string message = null)
        {
            try
            {

                _logger.Info($"Usuario Logado em Aluno.Index User.Identity.Name : {User.Identity.Name}");
                var usuario = User.Identity.Name;

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                _logger.Info($"Busca usuário por email: {usuario}");
                var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

                var searchFilter = new AlunosFilterDto
                {
                    MunicipioId = usu.MunicipioId.ToString(),
                    LocalidadeId = usu.LocalidadeId
                };

                _logger.Info($"Busca alunos por filtro do usuário logado: {searchFilter.ToJson()}");
                var result = await ApiClientFactory.Instance.GetAlunosByFilter(searchFilter);

                var fomento = ApiClientFactory.Instance.GetFomentoByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));
                var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome", fomento.Id);

                var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll().Where(x => x.Status), "Id", "Nome");
                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", usu.Uf);


                SelectList profissionais;

                if (usu.Perfil.Id == (int)EnumPerfil.Profissional)
                {
                    var profissional = await ApiClientFactory.Instance.GetProfissionalByEmail(usu.Email);

                    profissionais = new SelectList(ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId)), "Id", "Nome", profissional.Id);
                }
                else
                {
                    profissionais = new SelectList(ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId)), "Id", "Nome");
                }


                List<SelectListDto> listSexo = new List<SelectListDto>
                {
                    new() { IdNome = "M", Nome = "MASCULINO" },
                    new() { IdNome = "F", Nome = "FEMININO" }
                };

                var sexos = new SelectList(listSexo, "IdNome", "Nome", searchFilter.Sexo);

                List<SelectListDto> list = new List<SelectListDto>
                {
                    new() { IdNome = "NAODECLARADA", Nome = "NÃO DECLARADA" },
                    new() { IdNome = "PARDA", Nome = "PARDA" },
                    new() { IdNome = "BRANCA", Nome = "BRANCA" },
                    new() { IdNome = "PRETA", Nome = "PRETA" },
                    new() { IdNome = "INDIGENA", Nome = "INDÍGENA" },
                    new() { IdNome = "AMARELA", Nome = "AMARELA" }
                };

                var etnias = new SelectList(list, "IdNome", "Nome", searchFilter.Etnia);

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

                var model = new AlunoModel
                {
                    ListFomentos = fomentos,
                    ListEstados = estados,
                    ListDeficiencias = deficiencias,
                    ListMunicipios = municipios!,
                    ListEtnias = etnias,
                    ListSexos = sexos,
                    ListLocalidades = localidades!,
                    Alunos = result.Alunos,
                    SearchFilter = searchFilter,
                    ListProfissionais = profissionais,
                    NomePerfil = usu.Perfil.Nome,
                    IdPerfil = usu.Perfil.Id

                };
                return View(model);

            }
            catch (Exception e)
            {
                _logger.Error($"Aluno.Index: {e.StackTrace}");
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error",
                    message = e.Message,
                    stackTrace = e.StackTrace
                });

            }
        }

        [HttpPost]
        public async Task<ActionResult> Index(int? crud, int? notify, IFormCollection collection, string message = null)
        {
            try
            {
                var usuario = User.Identity.Name;
                var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                _logger.Info($"Usuario Logado em Aluno.Index User.Identity.Name : {User.Identity.Name}");

                var possuiFoto = collection["possuiFoto"].ToString();

                var searchFilter = new AlunosFilterDto
                {
                    MunicipioId = collection["ddlMunicipio"].ToString(),
                    LocalidadeId = collection["ddlLocalidade"].ToString(),
                    ProfissionalId = collection["ddlProfissional"].ToString(),
                    DeficienciaId = collection["ddlDeficiencia"].ToString(),
                    Etnia = collection["ddlEtnia"].ToString(),
                    Sexo = collection["ddlSexo"].ToString(),
                    Nome = collection["nome"].ToString(),
                    Matricula = collection["matricula"].ToString(),
                    PossuiFoto = possuiFoto != "",
                };

                _logger.Info($"GetAlunosByFilter");
                var result = await ApiClientFactory.Instance.GetAlunosByFilter(searchFilter);


                var fomento = ApiClientFactory.Instance.GetFomentoByLocalidadeId(Convert.ToInt32(searchFilter.LocalidadeId));
                var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome", fomento.Id);

                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", usu.Uf);

                var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll().Where(x => x.Status), "Id", "Nome", searchFilter.DeficienciaId);
                var profissionais = new SelectList(ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(searchFilter.LocalidadeId)), "Id", "Nome");

                List<SelectListDto> listSexo = new List<SelectListDto>
                {
                    new() { IdNome = "M", Nome = "MASCULINO" },
                    new() { IdNome = "F", Nome = "FEMININO" }
                };

                var sexos = new SelectList(listSexo, "IdNome", "Nome", searchFilter.Sexo);

                List<SelectListDto> list = new List<SelectListDto>
                {
                    new() { IdNome = "NAODECLARADA", Nome = "NÃO DECLARADA" },
                    new() { IdNome = "PARDA", Nome = "PARDA" },
                    new() { IdNome = "BRANCA", Nome = "BRANCA" },
                    new() { IdNome = "PRETA", Nome = "PRETA" },
                    new() { IdNome = "INDIGENA", Nome = "INDIGENA" },
                    new() { IdNome = "AMARELA", Nome = "AMARELA" }
                };

                var etnias = new SelectList(list, "IdNome", "Nome", searchFilter.Etnia);

                SelectList municipios = null;

                if (!string.IsNullOrEmpty(usu.Uf))
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

                var model = new AlunoModel
                {
                    ListFomentos = fomentos,
                    ListEstados = estados,
                    ListDeficiencias = deficiencias,
                    ListMunicipios = municipios!,
                    ListEtnias = etnias,
                    ListSexos = sexos,
                    ListLocalidades = localidades!,
                    Alunos = result.Alunos,
                    SearchFilter = searchFilter,
                    ListProfissionais = profissionais,
                    IdPerfil = usu.Perfil.Id

                };
                return View(model);

            }
            catch (Exception e)
            {
                _logger.Error($"Aluno.Index: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        /// <summary>
        /// Tela para Inclusão de Aluno
        /// </summary>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        public async Task<ActionResult> Create(int? crud, int? notify, string message = null)
        {
            try
            {
                _logger.Info($"Usuario Logado em Aluno.Create User.Identity.Name : {User.Identity.Name}");

                var usuario = User.Identity.Name;

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                _logger.Info($"GetUsuarioByEmail");
                //var aluno = await ApiClientFactory.Instance.GetAlunoById(Convert.ToInt32(usuario));

                var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);
                
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

                SelectList fomentos = null;
                SelectList profissionais = null;

                if (usu.LocalidadeId != null)
                {
                    var resultFomento = ApiClientFactory.Instance.GetFomentoByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));
                    fomentos = new SelectList(new List<FomentoDto>() { resultFomento }, "Id", "Nome", resultFomento.Id);

                    if (usu.Perfil.Id == (int)EnumPerfil.Profissional)
                    {
                        var profissionalId = ApiClientFactory.Instance.GetProfissionalByEmail(usu.Email).Result.Id;

                        profissionais =
                            new SelectList(
                                ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId)), "Id",
                                "Nome", profissionalId);
                    }
                    else
                    {
                        profissionais =
                            new SelectList(
                                ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId)), "Id",
                                "Nome");
                    }
                }
                else
                {
                    fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome");

                    if (usu.Perfil.Id == (int)EnumPerfil.Profissional)
                    {
                        var profissionalId = ApiClientFactory.Instance.GetProfissionalByEmail(usu.Email).Result.Id;

                        profissionais =
                            new SelectList(
                                ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId)), "Id",
                                "Nome", profissionalId);
                    }
                    else
                    {
                        profissionais =
                            new SelectList(
                                ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId)), "Id",
                                "Nome");
                    }
                }


                var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll().Where(x => x.Status), "Id", "Nome");
                var modalidades = new SelectList(ApiClientFactory.Instance.GetModalidadeAll(), "Id", "Nome");
                var etapas = new SelectList(ApiClientFactory.Instance.GetEtapasEnsinoAll(), "Id", "Nome");
                var grauParentescos = new SelectList(ApiClientFactory.Instance.GetGrauParentescosAll(), "Id", "Nome");

                List<SelectListDto> list = new List<SelectListDto>
            {
                new() { IdNome = "NAODECLARADA", Nome = "NÃO DECLARADA" },
                new() { IdNome = "PARDA", Nome = "PARDA" },
                new() { IdNome = "BRANCA", Nome = "BRANCA" },
                new() { IdNome = "PRETA", Nome = "PRETA" },
                new() { IdNome = "INDIGENA", Nome = "INDÍGENA" },
                new() { IdNome = "AMARELA", Nome = "AMARELA" }
            };

                var etnias = new SelectList(list, "IdNome", "Nome");

                return View(new AlunoModel()
                {
                    ListEstados = estados,
                    ListMunicipios = municipios!,
                    ListLocalidades = localidades!,
                    ListDeficiencias = deficiencias,
                    ListModalidades = modalidades,
                    ListEtnias = etnias,
                    ListFomentos = fomentos,
                    ListEtapas = etapas,
                    ListProfissionais = profissionais,
                    UsuarioLogado = usu,
                    IdPerfil = usu.Perfil.Id,
                    ListGrauParentescos = grauParentescos
                });
            }
            catch (Exception e)
            {
                _logger.Error($"Aluno.Create: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }


        }

        /// <summary>
        /// Tela para Alteração de Aluno
        /// </summary>
        /// <param name="id">Identificador do aluno</param>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Alterar)]
        public async Task<ActionResult> Edit(int id, int? crud, int? notify, string message = null)
        {
            try
            {
                _logger.Info($"Tela para alteração de aluno - Aluno.Edit: {id}");

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);


                var aluno = await ApiClientFactory.Instance.GetAlunoById(id);

                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", aluno.Estado);

                SelectList municipios = null;

                if (!string.IsNullOrEmpty(aluno.Estado))
                {
                    municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(aluno.Estado), "Id", "Nome", aluno.MunicipioId);
                }
                SelectList localidades = null;

                if (aluno.MunicipioId != null)
                {
                    var resultLocalidades = ApiClientFactory.Instance.GetLocalidadeByMunicipioId(aluno.MunicipioId.ToString());

                    localidades = new SelectList(resultLocalidades, "Id", "Nome", aluno.LocalidadeId);
                }

                SelectList fomentos = null;

                if (aluno.LocalidadeId != null)
                {
                    var resultFomento = ApiClientFactory.Instance.GetFomentoByLocalidadeId(Convert.ToInt32(aluno.LocalidadeId));
                    fomentos = new SelectList(new List<FomentoDto>() { resultFomento }, "Id", "Nome", resultFomento.Id);
                }
                else
                {
                    fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome");
                }

                var profissionais = new SelectList(ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(aluno.LocalidadeId)), "Id", "Nome", aluno.ProfissionalId);
                var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll(), "Id", "Nome", aluno.DeficienciaId);
                var listModalidades = new SelectList(ApiClientFactory.Instance.GetModalidadeAll(), "Id", "Nome", aluno.ModalidadesIds);
                var etapas = new SelectList(ApiClientFactory.Instance.GetEtapasEnsinoAll(), "Id", "Nome", aluno.EtapaId);

                var series = new SelectList(
                    ApiClientFactory.Instance
                        .GetSeriesByLocalidadeIdEtapaId(Convert.ToInt32(aluno.LocalidadeId),
                            Convert.ToInt32(aluno.EtapaId)).Select(s => new { Id = s.Nome, Nome = s.Nome }).Distinct()
                        .ToList(), "Nome", "Nome", aluno.SerieNome);


                SelectList turmas = null;

                if (aluno.EtapaId == null)
                {
                    var resultTurmas =
                        ApiClientFactory.Instance.GetTurmasByLocalidadeId(Convert.ToInt32(aluno.LocalidadeId));
                    if (resultTurmas.Count == 0)
                    {
                        return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Não existem séries/turmas cadastradas para a localidade deste aluno." });
                    }

                }
                else
                {
                    if (aluno.SerieNome != null)
                        turmas = new SelectList(
                            ApiClientFactory.Instance
                                .GetTurmasByLocalidadeIdEtapaIdSerie(Convert.ToInt32(aluno.LocalidadeId),
                                    Convert.ToInt32(aluno.EtapaId), aluno.SerieNome)
                                .Select(s => new { Id = s.Id, Turma = s.Turma }).ToList(), "Id", "Turma",
                            aluno.SerieId);
                }


                List<SelectListDto> list = new List<SelectListDto>
                {
                    new() { IdNome = "NAODECLARADA", Nome = "NÃO DECLARADA" },
                    new() { IdNome = "PARDA", Nome = "PARDA" },
                    new() { IdNome = "BRANCA", Nome = "BRANCA" },
                    new() { IdNome = "PRETA", Nome = "PRETA" },
                    new() { IdNome = "INDIGENA", Nome = "INDÍGENA" },
                    new() { IdNome = "AMARELA", Nome = "AMARELA" }
                };

                var etnias = new SelectList(list, "IdNome", "Nome", aluno.Etnia);

                
                return View(new AlunoModel()
                {
                    ListEstados = estados,
                    Modalidades = aluno.ListModalidades,
                    Aluno = aluno,
                    ListMunicipios = municipios,
                    ListLocalidades = localidades,
                    ListProfissionais = profissionais,
                    ListEtnias = etnias,
                    ListFomentos = fomentos,
                    ListDeficiencias = deficiencias,
                    ListModalidades = listModalidades,
                    ListEtapas = etapas,
                    ListSeries = series,
                    ListTurmas = turmas,


                });

            }
            catch (Exception e)
            {
                _logger.Error($"Tela para alteração de aluno - Aluno.Edit: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        /// <summary>
        /// Ação de Inclusao do Aluno
        /// </summary>
        /// <param name="collection">Coleção de dados para inclusao de aluno</param>
        /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
        [HttpPost]
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        public async Task<ActionResult> CreateDados(IFormCollection collection)
        {
            try
            {
                _logger.Info($"Ação de inclusao do aluno - Aluno.CreateDados");

                string filePath = null;

                var status = collection["status"].ToString();
                var habilitado = collection["habilitado"].ToString();

                var command = new AlunoModel.CreateUpdateDadosAlunoCommand()
                {
                    Etnia = collection["ddlEtnia"] == "" ? null : collection["ddlEtnia"].ToString(),
                    MunicipioId = collection["ddlMunicipio"] == "" ? null : Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                    //ProfissionalId = collection["ddlProfissionalAluno"] == "" ? null : Convert.ToInt32(collection["ddlProfissionalAluno"].ToString()),
                    FomentoId = collection["ddlFomento"] == "" ? null : Convert.ToInt32(collection["ddlFomento"].ToString()),
                    DeficienciaId = collection["ddlDeficiencia"] == "" ? null : Convert.ToInt32(collection["ddlDeficiencia"].ToString()),
                    LocalidadeId = collection["ddlLocalidade"] == "" ? null : Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                    ModalidadesIds = collection["ddlModalidades"].ToString(),
                    Nome = collection["nome"] == "" ? null : collection["nome"].ToString(),
                    DtNascimento = collection["DtNascimento"] == "" ? null : collection["DtNascimento"].ToString(),
                    Email = collection["email"] == "" ? null : collection["email"].ToString(),
                    Sexo = collection["ddlSexo"] == "" ? null : collection["ddlSexo"].ToString(),
                    NomeMae = collection["nomeMae"] == "" ? null : collection["nomeMae"].ToString(),
                    NomePai = collection["nomePai"] == "" ? null : collection["nomePai"].ToString(),
                    Telefone = collection["numTelefone"] == "" ? null : collection["numTelefone"].ToString(),
                    Cep = collection["cep"] == "" ? null : collection["cep"].ToString(),
                    Celular = collection["numCelular"] == "" ? null : collection["numCelular"].ToString(),
                    Cpf = collection["cpf"] == "" ? null : collection["cpf"].ToString(),
                    Endereco = collection["endereco"] == "" ? null : collection["endereco"].ToString(),
                    Numero = collection["numero"] == "" ? null : collection["numero"].ToString(),
                    Bairro = collection["bairro"] == "" ? null : collection["bairro"].ToString(),
                    DeficienciasIds = collection["arrDeficiencias"] == "" ? null : collection["arrDeficiencias"].ToString(),
                    Habilitado = habilitado != "",
                    Status = status != "",
                    NomeFoto = filePath,
                    AutorizacaoSaida = Convert.ToBoolean(collection["autorizado"].ToString()),
                    UtilizacaoImagem = Convert.ToBoolean(collection["utilizacaoImagem"].ToString()),
                    ParticipacaoProgramaCompartilhamentoDados = Convert.ToBoolean(collection["participacao"].ToString()),
                    CopiaDocAlunoResponsavel = false,
                    AutorizacaoConsentimentoAssentimento = collection["agreeterms"].ToString() != "",
                    SerieId = collection["ddlTurma"] == "" ? null : Convert.ToInt32(collection["ddlTurma"].ToString())

                };

                foreach (var file in collection.Files)
                {
                    if (file.Length <= 0) continue;

                    command.NomeFoto = System.IO.Path.GetFileName(collection.Files[0].FileName);

                    using (var ms = new MemoryStream())
                    {
                        file.CopyToAsync(ms);
                        var byteIMage = ms.ToArray();
                        command.ByteImage = byteIMage;
                    }
                }

                var alunoId = await ApiClientFactory.Instance.CreateDados(command);

                var updateCommand = command;

                updateCommand.Id = (int)alunoId;
                command.QrCode = GeraQrCode(alunoId);

                await ApiClientFactory.Instance.UpdateDados((int)alunoId, updateCommand);

                string filePathDocumento = null;
                string fileNameDocumento = null;

                var list = new List<CreateDocumentoAlunoDto>();

                foreach (var t in collection.Files)
                {
                    var file = t;
                    if (file.Length <= 0) continue;
                    fileNameDocumento = $"{alunoId}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    filePathDocumento = Path.Combine(_host.WebRootPath, $"Documentos\\{fileNameDocumento}");

                    if (!Directory.Exists(Path.Combine(_host.WebRootPath, $"Documentos")))
                        Directory.CreateDirectory(Path.Combine(_host.WebRootPath, $"Documentos"));

                    using Stream fileStream = new FileStream(filePathDocumento, FileMode.Create);
                    await file.CopyToAsync(fileStream);

                    list.Add(new CreateDocumentoAlunoDto()
                    {
                        AlunoId = (int)alunoId,
                        NomeDocumento = fileNameDocumento,
                        Url = filePathDocumento
                    });
                }

                if (list.Count > 0) await ApiClientFactory.Instance.CreateDocumentosAluno(list);

                return RedirectToAction(nameof(Index), new { id = alunoId, crud = (int)EnumCrud.Created });

            }
            catch (Exception e)
            {
                _logger.Error($"Ação de inclusão do aluno - Aluno.Edit: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new
                {
                    notify = EnumNotify.Error,
                    mesage = e.Message
                });
            }
        }

        /// <summary>
        /// Ação de Alteração do Aluno
        /// </summary>
        /// <param name="id">Identificador do aluno</param>
        /// <param name="collection">Coleção de dados para alteração de aluno</param>
        /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
        [HttpPost]
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Alterar)]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                _logger.Info($"Ação de alteração do aluno - Aluno.Edit: {id}");


                var status = collection["status"].ToString();
                var habilitado = collection["habilitado"].ToString();


                if (string.IsNullOrEmpty(collection["email"]))
                {
                    return RedirectToAction(nameof(Edit), new { id=id, notify = (int)EnumNotify.Warning, message = "É necessário informar um email." });
                }

                var result = ApiClientFactory.Instance.GetAlunoByEmail(collection["email"]);

                if (result!=null && result.Id != id)
                {
                    return RedirectToAction(nameof(Edit), new { id = id, notify = (int)EnumNotify.Error, message = "Já existe um aluno cadastrado com este email." });
                }



                var command = new AlunoModel.CreateUpdateDadosAlunoCommand
                    {
                        Id = Convert.ToInt32(id),
                        Etnia = collection["ddlEtnia"] == "" ? null : collection["ddlEtnia"].ToString(),
                        MunicipioId = collection["ddlMunicipio"] == "" ? null : Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                        FomentoId = collection["ddlFomento"] == "" ? null : Convert.ToInt32(collection["ddlFomento"].ToString()),
                        DeficienciaId = collection["ddlDeficiencia"] == "" ? null : Convert.ToInt32(collection["ddlDeficiencia"].ToString()),
                        LocalidadeId = collection["ddlLocalidade"] == "" ? null : Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                        Nome = collection["nome"] == "" ? null : collection["nome"].ToString(),
                        DtNascimento = collection["DtNascimento"] == "" ? null : collection["DtNascimento"].ToString(),
                        Email = collection["email"] == "" ? null : collection["email"].ToString(),
                        Sexo = collection["ddlSexo"] == "" ? null : collection["ddlSexo"].ToString(),
                        NomeMae = collection["nomeMae"] == "" ? null : collection["nomeMae"].ToString(),
                        NomePai = collection["nomePai"] == "" ? null : collection["nomePai"].ToString(),
                        Telefone = collection["numTelefone"] == "" ? null : collection["numTelefone"].ToString(),
                        Cep = collection["cep"] == "" ? null : collection["cep"].ToString(),
                        Celular = collection["numCelular"] == "" ? null : collection["numCelular"].ToString(),
                        Cpf = collection["cpf"] == "" ? null : collection["cpf"].ToString(),
                        Endereco = collection["endereco"] == "" ? null : collection["endereco"].ToString(),
                        Numero = collection["numero"] == "" ? null : collection["numero"].ToString(),
                        Bairro = collection["bairro"] == "" ? null : collection["bairro"].ToString(),
                        Habilitado = habilitado != "",
                        Status = status != "",
                        ModalidadesIds = collection["ddlModalidades"].ToString(),
                        SerieId = collection["ddlTurma"] == "" ? null : Convert.ToInt32(collection["ddlTurma"].ToString())
                    };




                foreach (var file in collection.Files)
                {
                    if (file.Length <= 0) continue;

                    command.NomeFoto = System.IO.Path.GetFileName(collection.Files[0].FileName);

                    using (var ms = new MemoryStream())
                    {
                        file.CopyToAsync(ms);
                        var byteIMage = ms.ToArray();
                        command.ByteImage = byteIMage;
                    }
                }

                await ApiClientFactory.Instance.UpdateDados(id, command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
            }
            catch (Exception e)
            {
                _logger.Error($"Ação de alteração do aluno - Aluno.Edit: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, mesage = e.Message });
            }
        }

        /// <summary>
        /// Ação de Upload de Foto do Aluno
        /// </summary>
        /// <param name="collection">Arquivo de upload realizado</param>
        /// <returns>Retorna mensagem de upload realizado através do parametro notfy e message</returns>
        [HttpPost]
        //[ClaimsAuthorize(ClaimType.Aluno, Claim.Upload)]
        public async Task<ActionResult> Upload(IFormCollection collection)
        {
            try
            {
                _logger.Info($"Ação de upload de foto do aluno - Aluno.Upload");

                string filePath = null;

                var command = new AlunoModel.CreateUpdateDadosAlunoCommand
                {
                    Id = Convert.ToInt32(collection["alunoId"])
                };

                foreach (var file in collection.Files)
                {
                    if (file.Length <= 0) continue;

                    command.NomeFoto = System.IO.Path.GetFileName(collection.Files[0].FileName);

                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var byteIMage = ms.ToArray();
                    command.ByteImage = byteIMage;
                }

                await ApiClientFactory.Instance.UpdateAlunoFoto(command.Id, command);

                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Success, message = "Upload realizado com sucesso." });
            }
            catch (Exception e)
            {
                _logger.Error($"Ação de upload de foto do aluno - Aluno.Upload: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        /// <summary>
        /// Ação de Exclusão do Aluno
        /// </summary>
        /// <param name="id">Id de exclusão de aluno</param>
        /// <returns>Retorna true ou false</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Excluir)]
        public ActionResult Delete(int id)
        {
            try
            {
                _logger.Info($"Ação de exclusão do aluno - Aluno.Delete: {id}");

                ApiClientFactory.Instance.DeleteDados(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch (Exception e)
            {
                _logger.Error($"Ação de exclusão do aluno - Aluno.Delete: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = $"ATENÇÃO. {e.Message}" });
            }
        }

        /// <summary>
        /// Gera um PDF da carteirinha do aluno em formato CMYK
        /// </summary>
        /// <param name="id">Id do Aluno</param>
        /// <param name="fomentoId">Id do Fomento</param>
        /// <returns>Arquivo PDF da carteirinha</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        public async Task<IActionResult> GerarPdfCarteirinha(int id, int fomentoId)
        {
            try
            {
                _logger.Info($"Gerando PDF CMYK da carteirinha - Aluno.GerarPdfCarteirinha - AlunoId: {id}");

                // Verificar o ambiente
                if (!VerificarAmbientePdfCmyk())
                {
                    return StatusCode(500, "Configurações necessárias para geração de PDF CMYK não encontradas");
                }

                // Obter os dados do aluno e do modelo de carteirinha
                var aluno = await ApiClientFactory.Instance.GetAlunoById(id);
                var modeloCarteirinha = await ApiClientFactory.Instance.GetModeloCarteirinhaByFomentoId(fomentoId);

                if (aluno == null)
                {
                    _logger.Error($"Aluno não encontrado - ID: {id}");
                    return NotFound("Aluno não encontrado");
                }

                if (modeloCarteirinha == null)
                {
                    _logger.Error($"Modelo de carteirinha não encontrado - FomentoId: {fomentoId}");
                    return NotFound("Modelo de carteirinha não encontrado");
                }

                // Gerar o PDF
                byte[] pdfBytes = await GerarPdfCmyk(aluno, modeloCarteirinha);

                // Retornar o arquivo PDF
                return File(
                    pdfBytes,
                    "application/pdf",
                    $"Carteirinha_{aluno.Nome.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf"
                );
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao gerar PDF da carteirinha: {ex.Message}", ex);
                return StatusCode(500, "Erro ao gerar o PDF da carteirinha: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtém a contagem de alunos com base nos filtros informados
        /// </summary>
        /// <param name="ids">IDs dos alunos selecionados (opcional)</param>
        /// <param name="fomentoId">ID do Fomento (obrigatório)</param>
        /// <param name="estadoId">ID do Estado (opcional)</param>
        /// <param name="municipioId">ID do Município (opcional)</param>
        /// <param name="localidadeId">ID da Localidade (opcional)</param>
        /// <param name="profissionalId">ID do Profissional (opcional)</param>
        /// <param name="deficienciaId">ID da Deficiência (opcional)</param>
        /// <param name="etniaId">ID da Etnia (opcional)</param>
        /// <param name="sexoId">ID do Sexo (opcional)</param>
        /// <param name="possuiFoto">Filtro de alunos que possuem foto (opcional)</param>
        /// <returns>JSON com a contagem total de alunos</returns>
        [HttpGet]
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Consultar)]
        public async Task<IActionResult> ObterContagemAlunos(string ids, int fomentoId, string estadoId = "",
            string municipioId = "", string localidadeId = "", string profissionalId = "",
            string deficienciaId = "", string etniaId = "", string sexoId = "", bool possuiFoto = false)
        {
            try
            {
                _logger.Info($"Obtendo contagem de alunos - Aluno.ObterContagemAlunos - FomentoId: {fomentoId}");

                // Verificar se o fomento foi informado
                if (fomentoId <= 0)
                {
                    return BadRequest(new { success = false, message = "É necessário selecionar um Fomento." });
                }

                int totalAlunos = 0;

                // Se IDs específicos foram informados
                if (!string.IsNullOrEmpty(ids))
                {
                    var alunoIds = ids.Split(',').Select(int.Parse).ToList();
                    totalAlunos = alunoIds.Count;

                    // Se tiver filtro de foto, precisamos verificar cada aluno
                    if (possuiFoto)
                    {
                        totalAlunos = 0;
                        foreach (var id in alunoIds)
                        {
                            var aluno = await ApiClientFactory.Instance.GetAlunoById(id);
                            if (aluno != null && aluno.ByteImage != null && aluno.ByteImage.Length > 0)
                            {
                                totalAlunos++;
                            }
                        }
                    }
                }
                else
                {
                    // Criar filtro para buscar os alunos
                    var searchFilter = new AlunosFilterDto
                    {
                        FomentoId = fomentoId.ToString(),
                        Estado = estadoId,
                        MunicipioId = municipioId,
                        LocalidadeId = localidadeId,
                        ProfissionalId = profissionalId,
                        DeficienciaId = deficienciaId,
                        Etnia = etniaId,
                        Sexo = sexoId,
                        PossuiFoto = possuiFoto
                    };

                    // Buscar apenas a contagem de alunos
                    var resultado = await ApiClientFactory.Instance.GetAlunosByFilter(searchFilter);
                    if (resultado != null && resultado.Alunos != null)
                    {
                        totalAlunos = resultado.Alunos.Count;
                    }
                }

                return Json(new { success = true, total = totalAlunos });
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao obter contagem de alunos: {ex.Message}", ex);
                return StatusCode(500, new { success = false, message = "Erro ao obter contagem de alunos: " + ex.Message });
            }
        }

        /// <summary>
        /// Gera um PDF com múltiplas carteirinhas de alunos com base nos filtros ou IDs informados
        /// </summary>
        /// <param name="ids">IDs dos alunos selecionados (opcional)</param>
        /// <param name="fomentoId">ID do Fomento (obrigatório)</param>
        /// <param name="estadoId">ID do Estado (opcional)</param>
        /// <param name="municipioId">ID do Município (opcional)</param>
        /// <param name="localidadeId">ID da Localidade (opcional)</param>
        /// <param name="profissionalId">ID do Profissional (opcional)</param>
        /// <param name="deficienciaId">ID da Deficiência (opcional)</param>
        /// <param name="etniaId">ID da Etnia (opcional)</param>
        /// <param name="sexoId">ID do Sexo (opcional)</param>
        /// <param name="possuiFoto">Filtro de alunos que possuem foto (opcional)</param>
        /// <param name="pagina">Número da página atual (começando em 1)</param>
        /// <param name="itensPorPagina">Quantidade de itens por página</param>
        /// <returns>Arquivo PDF com múltiplas carteirinhas</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        public async Task<IActionResult> ImprimirCarteirinhasLote(string ids, int fomentoId, string estadoId = "",
            string municipioId = "", string localidadeId = "", string profissionalId = "",
            string deficienciaId = "", string etniaId = "", string sexoId = "", bool possuiFoto = false,
            int pagina = 1, int itensPorPagina = 20)
        {
            try
            {
                _logger.Info($"Gerando PDF CMYK de carteirinhas em lote - Aluno.ImprimirCarteirinhasLote - FomentoId: {fomentoId}, Página: {pagina}");

                // Verificar o ambiente
                if (!VerificarAmbientePdfCmyk())
                {
                    return StatusCode(500, "Configurações necessárias para geração de PDF CMYK não encontradas");
                }

                // Verificar se o fomento foi informado
                if (fomentoId <= 0)
                {
                    _logger.Error("Fomento não informado para impressão em lote");
                    return BadRequest("É necessário selecionar um Fomento para a impressão em lote");
                }

                // Obter o modelo de carteirinha
                var modeloCarteirinha = await ApiClientFactory.Instance.GetModeloCarteirinhaByFomentoId(fomentoId);
                if (modeloCarteirinha == null)
                {
                    _logger.Error($"Modelo de carteirinha não encontrado - FomentoId: {fomentoId}");
                    return NotFound("Modelo de carteirinha não encontrado");
                }

                // Lista para armazenar todos os alunos
                List<AlunoDto> todosAlunos = new List<AlunoDto>();

                // Lista para armazenar os alunos da página atual
                List<AlunoDto> alunosPaginados = new List<AlunoDto>();

                // Se IDs específicos foram informados
                if (!string.IsNullOrEmpty(ids))
                {
                    var alunoIds = ids.Split(',').Select(int.Parse).ToList();

                    // Buscar cada aluno individualmente
                    foreach (var id in alunoIds)
                    {
                        var aluno = await ApiClientFactory.Instance.GetAlunoById(id);
                        if (aluno != null)
                        {
                            // Filtrar por possuiFoto se o filtro estiver ativo
                            if (possuiFoto)
                            {
                                if (aluno.ByteImage != null && aluno.ByteImage.Length > 0)
                                {
                                    todosAlunos.Add(aluno);
                                }
                            }
                            else
                            {
                                todosAlunos.Add(aluno);
                            }
                        }
                    }
                }
                else
                {
                    // Criar filtro para buscar os alunos
                    var searchFilter = new AlunosFilterDto
                    {
                        FomentoId = fomentoId.ToString(),
                        Estado = estadoId,
                        MunicipioId = municipioId,
                        LocalidadeId = localidadeId,
                        ProfissionalId = profissionalId,
                        DeficienciaId = deficienciaId,
                        Etnia = etniaId,
                        Sexo = sexoId,
                        PossuiFoto = possuiFoto
                    };

                    // Buscar alunos com base nos filtros
                    var resultado = await ApiClientFactory.Instance.GetAlunosByFilter(searchFilter);

                    if (resultado != null && resultado.Alunos != null && resultado.Alunos.Any())
                    {
                        // Para cada AlunoIndexDto, buscar o AlunoDto completo com todas as informações necessárias
                        foreach (var alunoIndex in resultado.Alunos)
                        {
                            var alunoCompleto = await ApiClientFactory.Instance.GetAlunoById(alunoIndex.Id);
                            if (alunoCompleto != null)
                            {
                                // Se o filtro possuiFoto estiver ativo e o aluno tiver imagem, ou se o filtro não estiver ativo
                                if (!possuiFoto || (alunoCompleto.ByteImage != null && alunoCompleto.ByteImage.Length > 0))
                                {
                                    todosAlunos.Add(alunoCompleto);
                                }
                            }
                        }
                    }
                }

                // Verificar se encontrou alunos
                if (todosAlunos == null || !todosAlunos.Any())
                {
                    _logger.Warn("Nenhum aluno encontrado com os filtros informados");
                    return NotFound("Nenhum aluno encontrado com os filtros informados");
                }

                // Aplicar paginação
                int totalPaginas = (int)Math.Ceiling(todosAlunos.Count / (double)itensPorPagina);

                // Ajustar a página caso esteja fora dos limites
                if (pagina < 1) pagina = 1;
                if (pagina > totalPaginas) pagina = totalPaginas;

                // Calcular índices de início e fim para a página atual
                int indiceInicio = (pagina - 1) * itensPorPagina;
                int indiceFim = Math.Min(indiceInicio + itensPorPagina, todosAlunos.Count);

                // Obter os alunos da página atual
                alunosPaginados = todosAlunos.Skip(indiceInicio).Take(indiceFim - indiceInicio).ToList();

                // Gerar o PDF com as carteirinhas da página atual
                byte[] pdfBytes = await GerarPdfCarteirinhasLote(alunosPaginados, modeloCarteirinha);

                // Retornar o arquivo PDF
                return File(
                    pdfBytes,
                    "application/pdf",
                    $"Carteirinhas_Lote_Pagina{pagina}_{DateTime.Now:yyyyMMdd}.pdf"
                );
            }
            catch (OutOfMemoryException ex)
            {
                _logger.Error($"Erro de memória ao gerar PDF das carteirinhas em lote: {ex.Message}", ex);
                return StatusCode(500, "Erro de memória ao gerar o PDF. Por favor, reduza o número de alunos selecionados.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao gerar PDF das carteirinhas em lote: {ex.Message}", ex);
                return StatusCode(500, "Erro ao gerar o PDF das carteirinhas em lote: " + ex.Message);
            }
        }

        /// <summary>
        /// Gera um PDF em formato CMYK contendo múltiplas carteirinhas de alunos.
        /// </summary>
        /// <param name="alunos">Lista de alunos para gerar as carteirinhas</param>
        /// <param name="modeloCarteirinha">Modelo de carteirinha utilizado para gerar o PDF</param>
        /// <returns>Retorna os bytes do arquivo PDF gerado</returns>
        private async Task<byte[]> GerarPdfCarteirinhasLote(List<AlunoDto> alunos, ModeloCarteirinhaDto modeloCarteirinha)
        {
            // Caminho dos arquivos de background
            string frenteCaminhoRelativo = $"assets/styles_Carteirinha/modelos/{modeloCarteirinha.NomeImagemFrente}.tif";
            string versoCaminhoRelativo = $"assets/styles_Carteirinha/modelos/{modeloCarteirinha.NomeImagemVerso}.tif";

            // Converter caminhos relativos para absolutos
            string frenteCaminhoAbsoluto = System.IO.Path.Combine(_host.WebRootPath, frenteCaminhoRelativo.Replace("/", System.IO.Path.DirectorySeparatorChar.ToString()));
            string versoCaminhoAbsoluto = System.IO.Path.Combine(_host.WebRootPath, versoCaminhoRelativo.Replace("/", System.IO.Path.DirectorySeparatorChar.ToString()));

            // Verificar se os arquivos existem
            if (!System.IO.File.Exists(frenteCaminhoAbsoluto))
            {
                _logger.Error($"Arquivo de fundo da frente não encontrado: {frenteCaminhoAbsoluto}");
                throw new FileNotFoundException("Arquivo de fundo da frente não encontrado", frenteCaminhoAbsoluto);
            }

            if (!System.IO.File.Exists(versoCaminhoAbsoluto))
            {
                _logger.Error($"Arquivo de fundo do verso não encontrado: {versoCaminhoAbsoluto}");
                throw new FileNotFoundException("Arquivo de fundo do verso não encontrado", versoCaminhoAbsoluto);
            }

            // Criar memorystream para armazenar o PDF
            using (MemoryStream ms = new MemoryStream())
            {
                // Definição da sangria de 3mm (em pontos)
                float sangriaEmPontos = 3f / 10f * 28.35f; // 3mm em pontos

                // Dimensões originais do documento
                float larguraOriginal = 8.5f * 28.35f;
                float alturaOriginal = 5.4f * 28.35f;

                // Dimensões com sangria
                float larguraComSangria = larguraOriginal + (2 * sangriaEmPontos);
                float alturaComSangria = alturaOriginal + (2 * sangriaEmPontos);

                // Configuração das cores CMYK para todo o documento
                DeviceCmyk corPreto = new DeviceCmyk(0.75f, 0.68f, 0.67f, 0.9f); // "Rich black" CMYK
                DeviceCmyk corAzul = new DeviceCmyk(0.75f, 0.68f, 0, 0.2f);
                DeviceCmyk corBranca = new DeviceCmyk(0, 0, 0, 0); // Branco em CMYK

                // Criar documento PDF com suporte a CMYK
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);

                // Configurar Output Intent para CMYK
                PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "",
                    "http://www.color.org", "FOGRA39", null);
                pdf.AddOutputIntent(outputIntent);

                // Configurações para o documento com tamanho ajustado para incluir sangria
                Document document = new Document(pdf, new PageSize(larguraComSangria, alturaComSangria));
                document.SetMargins(0, 0, 0, 0);

                // Definir o TrimBox que será aplicado a cada página
                Rectangle trimBox = new Rectangle(
                    sangriaEmPontos,
                    sangriaEmPontos,
                    larguraComSangria - (2 * sangriaEmPontos),
                    alturaComSangria - (2 * sangriaEmPontos)
                );

                // Para cada aluno na lista, criar uma frente e um verso (duas páginas por aluno)
                foreach (var aluno in alunos)
                {
                    // === FRENTE DA CARTEIRINHA ===
                    // Se não for a primeira página, adicionar uma quebra de página
                    if (pdf.GetNumberOfPages() > 0)
                    {
                        document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    }

                    // Adicionar imagem de fundo da frente
                    ImageData imgDataFrente = ImageDataFactory.Create(frenteCaminhoAbsoluto);
                    iText.Layout.Element.Image backgroundFrente = new iText.Layout.Element.Image(imgDataFrente);
                    backgroundFrente.SetFixedPosition(0, 0);
                    backgroundFrente.SetWidth(larguraComSangria);
                    backgroundFrente.SetHeight(alturaComSangria);
                    document.Add(backgroundFrente);

                    // Adicionar informações do aluno - com a margem de sangria incluída nas posições
                    float leftMargin = 0.5f * 28.35f + sangriaEmPontos; // 0.5cm do original + sangria
                    float textWidth = 5.5f * 28.35f;
                    float labelWidth = 2.5f * 28.35f; // Largura da label
                    float valueWidth = 3.0f * 28.35f; // Largura do valor

                    // Ajustando a posição vertical incluindo a sangria
                    float startY = alturaComSangria - 2.3f * 28.35f - sangriaEmPontos - 10;
                    float currentY = startY;
                    float alturaItem;

                    // Nome
                    alturaItem = AddInfoRow(document, "NOME DO ESTUDANTE:", aluno.Nome, leftMargin, currentY, labelWidth, valueWidth, corAzul);
                    currentY -= alturaItem;

                    // Data de nascimento
                    alturaItem = AddInfoRow(document, "DATA DE NASCIMENTO:", aluno.DtNascimento, leftMargin, currentY, labelWidth, valueWidth, corAzul);
                    currentY -= alturaItem;

                    // Telefone
                    alturaItem = AddInfoRow(document, "TELEFONE:", aluno.Celular, leftMargin, currentY, labelWidth, valueWidth, corAzul);
                    currentY -= alturaItem;

                    // CPF
                    alturaItem = AddInfoRow(document, "CPF:", aluno.Cpf, leftMargin, currentY, labelWidth, valueWidth, corAzul);
                    currentY -= alturaItem;

                    // Matrícula
                    alturaItem = AddInfoRow(document, "MATRÍCULA:", aluno.Id.ToString(), leftMargin, currentY, labelWidth, valueWidth, corAzul);
                    currentY -= alturaItem;

                    // Modalidades - agora ajustando o currentY com base na altura ocupada
                    alturaItem = AddFullWidthText(document, aluno.Modalidades, leftMargin, currentY, labelWidth + valueWidth, corAzul, false);

                    // Adicionar foto do aluno
                    float rightMargin = 0.91f * 28.35f;
                    float topMargin = 0.795f * 28.35f + sangriaEmPontos / 3;
                    float fotoWidth = 48.5f;
                    float fotoHeight = 69.6f;

                    float fotoX = larguraComSangria - rightMargin - fotoWidth;
                    float fotoY = alturaComSangria - topMargin - fotoHeight;
                    float radioBorda = 4; // 4 pontos (aprox. 1.4mm)

                    if (aluno.ByteImage != null && aluno.ByteImage.Length > 0)
                    {
                        float fotoInternalX = fotoX;
                        float fotoInternalY = fotoY;
                        float fotoInternalWidth = fotoWidth;
                        float fotoInternalHeight = fotoHeight;

                        // Desenhar fundo branco com cantos arredondados
                        PdfCanvas canvasFundoFoto = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
                        canvasFundoFoto.SaveState();
                        canvasFundoFoto.SetFillColor(corBranca);
                        canvasFundoFoto.RoundRectangle(fotoInternalX, fotoInternalY, fotoInternalWidth, fotoInternalHeight, radioBorda);
                        canvasFundoFoto.Fill();
                        canvasFundoFoto.RestoreState();

                        ImageData imgDataFoto = ImageDataFactory.Create(aluno.ByteImage);
                        float imgOriginalWidth = imgDataFoto.GetWidth();
                        float imgOriginalHeight = imgDataFoto.GetHeight();
                        float imgRatio = imgOriginalWidth / imgOriginalHeight;
                        float containerRatio = fotoInternalWidth / fotoInternalHeight;

                        iText.Layout.Element.Image foto = new iText.Layout.Element.Image(imgDataFoto);

                        // Implementando comportamento similar ao background-size: cover
                        if (imgRatio > containerRatio)
                        {
                            foto.SetHeight(fotoInternalHeight);
                            float newWidth = fotoInternalHeight * imgRatio;
                            foto.SetWidth(newWidth);
                            float offsetX = (newWidth - fotoInternalWidth) / 2;
                            foto.SetFixedPosition(fotoInternalX - offsetX, fotoInternalY);
                        }
                        else
                        {
                            foto.SetWidth(fotoInternalWidth);
                            float newHeight = fotoInternalWidth / imgRatio;
                            foto.SetHeight(newHeight);
                            float offsetY = (newHeight - fotoInternalHeight) / 2;
                            foto.SetFixedPosition(fotoInternalX, fotoInternalY - offsetY);
                        }

                        // Aplicar o recorte em formato arredondado na imagem
                        PdfCanvas clipCanvas = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
                        clipCanvas.SaveState();
                        clipCanvas.SetFillColor(corPreto);
                        clipCanvas.SetStrokeColor(corPreto);
                        clipCanvas.RoundRectangle(fotoInternalX, fotoInternalY, fotoInternalWidth, fotoInternalHeight, radioBorda);
                        clipCanvas.Clip().EndPath();

                        document.Add(foto);
                        clipCanvas.RestoreState();
                    }
                    else
                    {
                        // Se não tiver foto, usar imagem padrão
                        string fotoDefaultPath = aluno.Sexo == "Feminino"
                            ? System.IO.Path.Combine(_host.WebRootPath, "assets", "images", "menina.png")
                            : System.IO.Path.Combine(_host.WebRootPath, "assets", "images", "menino.png");

                        float fotoInternalX = fotoX;
                        float fotoInternalY = fotoY;
                        float fotoInternalWidth = fotoWidth;
                        float fotoInternalHeight = fotoHeight;

                        // Desenhar fundo branco com cantos arredondados
                        PdfCanvas canvasFundoFotoDefault = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
                        canvasFundoFotoDefault.SaveState();
                        canvasFundoFotoDefault.SetFillColor(corBranca);
                        canvasFundoFotoDefault.RoundRectangle(fotoInternalX, fotoInternalY, fotoInternalWidth, fotoInternalHeight, radioBorda);
                        canvasFundoFotoDefault.Fill();
                        canvasFundoFotoDefault.RestoreState();

                        ImageData imgDataDefault = ImageDataFactory.Create(fotoDefaultPath);
                        float imgOriginalWidth = imgDataDefault.GetWidth();
                        float imgOriginalHeight = imgDataDefault.GetHeight();
                        float imgRatio = imgOriginalWidth / imgOriginalHeight;
                        float containerRatio = fotoInternalWidth / fotoInternalHeight;

                        iText.Layout.Element.Image fotoDefault = new iText.Layout.Element.Image(imgDataDefault);

                        if (imgRatio > containerRatio)
                        {
                            fotoDefault.SetHeight(fotoInternalHeight);
                            float newWidth = fotoInternalHeight * imgRatio;
                            fotoDefault.SetWidth(newWidth);
                            float offsetX = (newWidth - fotoInternalWidth) / 2;
                            fotoDefault.SetFixedPosition(fotoInternalX - offsetX, fotoInternalY);
                        }
                        else
                        {
                            fotoDefault.SetWidth(fotoInternalWidth);
                            float newHeight = fotoInternalWidth / imgRatio;
                            fotoDefault.SetHeight(newHeight);
                            float offsetY = (newHeight - fotoInternalHeight) / 2;
                            fotoDefault.SetFixedPosition(fotoInternalX, fotoInternalY - offsetY);
                        }

                        PdfCanvas clipCanvasDefault = new PdfCanvas(pdf.GetPage(pdf.GetNumberOfPages()));
                        clipCanvasDefault.SaveState();
                        clipCanvasDefault.SetFillColor(corPreto);
                        clipCanvasDefault.SetStrokeColor(corPreto);
                        clipCanvasDefault.RoundRectangle(fotoInternalX, fotoInternalY, fotoInternalWidth, fotoInternalHeight, radioBorda);
                        clipCanvasDefault.Clip().EndPath();

                        document.Add(fotoDefault);
                        clipCanvasDefault.RestoreState();
                    }

                    // Adicionar QR Code
                    if (aluno.QrCode != null && aluno.QrCode.Length > 0)
                    {
                        float qrRightMargin = 0.62f * 28.35f + sangriaEmPontos;
                        float qrBottomMargin = 0.4f * 28.35f + sangriaEmPontos;
                        float qrWidth = 48.5f;
                        float qrHeight = 48.5f;

                        float qrX = larguraComSangria - qrRightMargin - qrWidth;
                        float qrY = qrBottomMargin;

                        ImageData imgDataQr = ImageDataFactory.Create(aluno.QrCode);
                        iText.Layout.Element.Image qrCode = new iText.Layout.Element.Image(imgDataQr);
                        qrCode.SetFixedPosition(qrX, qrY);
                        qrCode.SetHeight(qrHeight);
                        qrCode.SetWidth(qrWidth);
                        qrCode.ScaleToFit(qrWidth, qrHeight);

                        document.Add(qrCode);
                    }

                    // Aplicar TrimBox imediatamente após concluir a página da frente
                    try
                    {
                        int paginaAtual = pdf.GetNumberOfPages();
                        if (paginaAtual > 0) // Verificação adicional de segurança
                        {
                            PdfPage pagina = pdf.GetPage(paginaAtual);
                            if (pagina != null)
                            {
                                pagina.SetTrimBox(trimBox);
                                pagina.SetBleedBox(new Rectangle(0, 0, larguraComSangria, alturaComSangria));
                                _logger.Info($"TrimBox aplicado com sucesso à página {paginaAtual} (frente)");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Erro ao definir TrimBox para página (frente): {ex.Message}", ex);
                        // Continuar mesmo se falhar
                    }

                    // === VERSO DA CARTEIRINHA ===
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                    // Adicionar imagem de fundo do verso
                    ImageData imgDataVerso = ImageDataFactory.Create(versoCaminhoAbsoluto);
                    iText.Layout.Element.Image backgroundVerso = new iText.Layout.Element.Image(imgDataVerso);
                    backgroundVerso.SetFixedPosition(0, 0);
                    backgroundVerso.SetWidth(larguraComSangria);
                    backgroundVerso.SetHeight(alturaComSangria);
                    document.Add(backgroundVerso);

                    // Adicionar informações do verso
                    float versoLeftMargin = 0.6f * 28.35f + sangriaEmPontos;
                    float versoStartY = alturaComSangria - 2.3f * 28.35f - sangriaEmPontos - 10;
                    float versoY = versoStartY;

                    // Município/Estado
                    alturaItem = AddInfoRow(document, "MUNICÍPIO/ESTADO:", aluno.MunicipioEstado, versoLeftMargin, versoY, labelWidth, valueWidth, corAzul, 8f);
                    versoY -= alturaItem;

                    // Unidade Escolar
                    alturaItem = AddInfoRow(document, "UNIDADE ESCOLAR:", aluno.NomeLocalidade, versoLeftMargin, versoY, labelWidth, valueWidth, corAzul, 8f);

                    // Aplicar TrimBox imediatamente após concluir a página do verso
                    try
                    {
                        int paginaAtual = pdf.GetNumberOfPages();
                        if (paginaAtual > 0) // Verificação adicional de segurança
                        {
                            PdfPage pagina = pdf.GetPage(paginaAtual);
                            if (pagina != null)
                            {
                                pagina.SetTrimBox(trimBox);
                                pagina.SetBleedBox(new Rectangle(0, 0, larguraComSangria, alturaComSangria));
                                _logger.Info($"TrimBox aplicado com sucesso à página {paginaAtual} (verso)");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Erro ao definir TrimBox para página (verso): {ex.Message}", ex);
                        // Continuar mesmo se falhar
                    }
                }

                // Fechar documento
                document.Close();

                // Retornar os bytes do PDF
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Acao de Imprimir Carteirinhas em Formato A4
        /// </summary>
        /// <param name="ids">ids</param>
        /// <param name="fomentoId">Id de fomento</param>
        /// <param name="estadoId">Id de estudo</param>
        /// <param name="municipioId">Id de municipio</param>
        /// <param name="localidadeId">Id de localidade</param>
        /// <param name="profissionalId">Id do profissional</param>
        /// <param name="deficienciaId">Id de deficiencia</param>
        /// <param name="etniaId">Id de etnia</param>
        /// <param name="sexoId">Id de sexo</param>
        /// <returns>Retorna impressao de carteirinhas em formato A4</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        public async Task<ActionResult> ImprimirCarteirinhasA4(string ids, string fomentoId = null, string estadoId = null,
            string municipioId = null, string localidadeId = null, string profissionalId = null, string deficienciaId = null,
            string etniaId = null, string sexoId = null)
        {
            try
            {
                _logger.Info($"Tela para impressao de carteirinha em formato A4 - Aluno.ImprimirCarteirinhasA4");

                // Buscar o modelo da carteirinha baseado no fomentoId
                ModeloCarteirinhaDto modeloCarteirinha = null;
                if (!string.IsNullOrEmpty(fomentoId))
                {
                    modeloCarteirinha = await ApiClientFactory.Instance.GetModeloCarteirinhaByFomentoId(int.Parse(fomentoId));
                }

                IEnumerable<AlunoDto> alunos;
                if (!string.IsNullOrEmpty(ids))
                {
                    // Se IDs específicos foram selecionados
                    var idList = ids.Split(',').Select(int.Parse).ToList();

                    alunos = idList.Select(async id =>
                    {
                        var aluno = await ApiClientFactory.Instance.GetAlunoById(id);
                        if (aluno.QrCode == null)
                        {
                            aluno.QrCode = GeraQrCode(aluno.Id);
                            ApiClientFactory.Instance.UpdateDados(aluno.Id, new AlunoModel.CreateUpdateDadosAlunoCommand
                            {
                                Id = aluno.Id,
                                QrCode = aluno.QrCode
                            });
                        }
                        return aluno;
                    }) as IEnumerable<AlunoDto>;
                }
                else
                {
                    // Usa filtros para obter alunos
                    var searchFilter = new AlunosFilterDto
                    {
                        FomentoId = fomentoId,
                        Estado = estadoId,
                        MunicipioId = municipioId,
                        LocalidadeId = localidadeId,
                        ProfissionalId = profissionalId,
                        DeficienciaId = deficienciaId,
                        Etnia = etniaId,
                        Sexo = sexoId
                    };

                    _logger.Info($"Filtros aplicados: Sexo={searchFilter.Sexo}, Fomento={searchFilter.FomentoId}, Profissional={searchFilter.ProfissionalId}");
                    var result = await ApiClientFactory.Instance.GetAlunosByFilter(searchFilter);

                    // Converte AlunoIndexDto para AlunoDto completo
                    var alunosCompletos = result.Alunos.Select(async a =>
                    {
                        var alunoCompleto = await ApiClientFactory.Instance.GetAlunoById(a.Id);
                        if (alunoCompleto.QrCode == null)
                        {
                            alunoCompleto.QrCode = GeraQrCode(alunoCompleto.Id);
                            await ApiClientFactory.Instance.UpdateQrCode(alunoCompleto.Id, new AlunoModel.CreateUpdateDadosAlunoCommand
                            {
                                Id = alunoCompleto.Id,
                                QrCode = alunoCompleto.QrCode
                            });
                        }
                        return alunoCompleto;
                    });
                    alunos = await Task.WhenAll(alunosCompletos);
                }

                var alunosList = alunos.ToList();
                if (!alunosList.Any())
                {
                    return RedirectToAction(nameof(Index), new
                    {
                        notify = (int)EnumNotify.Warning,
                        message = "Nenhum aluno encontrado com os filtros selecionados."
                    });
                }

                // Se necessário, converte a imagem em base64
                foreach (var aluno in alunosList.Where(a => a.ByteImage != null && a.Image == null))
                {
                    aluno.Image = aluno.ByteImage;
                }

                return View(new AlunoModel
                {
                    Alunos = alunosList.Select(a => new AlunoIndexDto
                    {
                        Id = a.Id,
                        Nome = a.Nome,
                        Email = a.Email,
                        DtNascimento = a.DtNascimento,
                        Status = a.Status,
                        Cpf = a.Cpf,
                        Telefone = a.Telefone,
                        Celular = a.Celular,
                        ByteImage = a.ByteImage,
                        QrCode = a.QrCode,
                        Sexo = a.Sexo,
                        ModalidadeLinhaAcao = a.ModalidadeLinhaAcao,
                        MunicipioEstado = a.MunicipioEstado,
                        NomeLocalidade = a.NomeLocalidade,
                        // Adicionando os campos de navegação
                        Municipio = new MunicipioDto
                        {
                            Id = int.TryParse(a.MunicipioId, out var mid) ? mid : 0,
                            Nome = a.NomeMunicipio
                        },
                        Localidade = new LocalidadeDto
                        {
                            Id = a.LocalidadeId,
                            Nome = a.NomeLocalidade
                        },
                        Modalidades = a.Modalidades
                    }).ToList(),
                    ModeloCarteirinha = modeloCarteirinha
                });
            }
            catch (Exception e)
            {
                _logger.Error($"Ação de imprimir carteirinha em formato A4 - Aluno.ImprimirCarteirinhasA4: {e.StackTrace}");
                _logger.Error($"Erro ao aplicar filtros: {e.Message}");

                return RedirectToAction(nameof(Index), new
                {
                    notify = (int)EnumNotify.Error,
                    message = "Erro ao gerar impressão em formato A4: " + e.Message
                });
            }
        }

        /// <summary>
        /// Tela de Visualizasao do Profile Aluno
        /// </summary>
        /// <param name="id">Identificador do aluno</param>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize(ClaimType.Aluno, Claim.Consultar)]
        public async Task<ActionResult> Profile(int id, int? crud, int? notify, string message = null)
        {
            try
            {
                _logger.Info($"Tela para visualização do profile do aluno - Aluno.Profile: {id}");


                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);
                var aluno = await ApiClientFactory.Instance.GetAlunoById(id);
                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", aluno.Estado);
                var municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(aluno.Estado!), "Id", "Nome", aluno.MunicipioId);
                var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeByMunicipioId(aluno.MunicipioId.ToString()), "Id", "Nome", aluno.LocalidadeId);
                var profissionais = new SelectList(ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(aluno.LocalidadeId)), "Id", "Nome", aluno.ProfissionalId);
                var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome", aluno.FomentoId);
                var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll(), "Id", "Nome", aluno.DeficienciaId);
                var listModalidades = new SelectList(ApiClientFactory.Instance.GetModalidadeAll(), "Id", "Nome",
                    aluno.ModalidadesIds);

                List<SelectListDto> list = new List<SelectListDto>
                {
                    new() { IdNome = "NAODECLARADA", Nome = "NÃO DECLARADA" },
                    new() { IdNome = "PARDA", Nome = "PARDA" },
                    new() { IdNome = "BRANCA", Nome = "BRANCA" },
                    new() { IdNome = "PRETA", Nome = "PRETA" },
                    new() { IdNome = "INDIGENA", Nome = "INDÍGENA" },
                    new() { IdNome = "AMARELA", Nome = "AMARELA" }
                };

                var etnias = new SelectList(list, "IdNome", "Nome", aluno.Etnia);


                var model = new AlunoModel()
                {
                    ListEstados = estados,
                    Modalidades = aluno.ListModalidades,
                    Aluno = aluno,
                    ListMunicipios = municipios,
                    ListLocalidades = localidades,
                    ListProfissionais = profissionais,
                    ListEtnias = etnias,
                    ListFomentos = fomentos,
                    ListDeficiencias = deficiencias,
                    ListModalidades = listModalidades
                };
                return View(model);

            }
            catch (Exception e)
            {
                _logger.Error($"Tela para visualização do profile do aluno - Aluno.Profile: {e.StackTrace}");

                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        /// <summary>
        /// Tela de Visualizasao do Profile Aluno
        /// </summary>
        /// <param name="collection">Coleção de dados para alteração de aluno</param>
        /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
        [HttpPost]
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        public async Task<ActionResult> Profile(IFormCollection collection)
        {
            try
            {
                _logger.Info($"Ação de atualização do profile do aluno - Aluno.Profile");

                string filePath = null;

                var alunoId = collection["alunoId"].ToString();

                var updateCommand = new AlunoModel.CreateUpdateProfileAlunoCommand()
                {
                    Endereco = collection["endereco"] == "" ? null : collection["endereco"].ToString(),
                    Cep = collection["cep"] == "" ? null : collection["cep"].ToString(),
                    Numero = collection["numero"] == "" ? null : collection["numero"].ToString(),
                    Bairro = collection["bairro"] == "" ? null : collection["bairro"].ToString(),
                    Telefone = collection["numTelefone"] == "" ? null : collection["numTelefone"].ToString(),
                    Celular = collection["numCelular"] == "" ? null : collection["numCelular"].ToString(),
                    //Email = collection["email"] == "" ? null : collection["numCelular"].ToString()
                };

                await ApiClientFactory.Instance.UpdateProfile(Convert.ToInt32(alunoId), updateCommand);

                return RedirectToAction(nameof(Index), new { id = alunoId, crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                _logger.Error($"Ação de atualização do profile do aluno - Aluno.Profile: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        /// <summary>
        /// Função de Criação de AlunoAula
        /// </summary>
        /// <param name="collection">Coleção de dados para criação de aluno</param>
        /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateAlunoAula([FromForm] AlunoModel.CreateUpdateAlunoAulaCommand model)
        {
            try
            {
                var command = new AlunoModel.CreateUpdateAlunoAulaCommand
                {
                    AlunoId = Convert.ToInt32(model.AlunoId),
                    AulaId = model.AulaId,
                    Progresso = model.Progresso
                };

                await ApiClientFactory.Instance.CreateAlunoAula(command);

                return NoContent();
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
            }
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Busca de Alunos por Localidade
        /// </summary>
        /// <param name="id">Identificador da localidade</param>
        /// <returns>Retorna a lista de alunos</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Consultar)]
        public async Task<JsonResult> GetAlunosByLocalidadeId(string id)
        {
            try
            {
                _logger.Info($"Busca de alunos por localidade GetAlunosByLocalidadeId: {id}");

                if (string.IsNullOrEmpty(id)) throw new Exception("Localidade não informada.");
                var resultLocal = await ApiClientFactory.Instance.GetNomeAlunosByLocalidadeId(Convert.ToInt32(id));

                return new JsonResult(new SelectList(resultLocal, "Id", "Nome"));

            }
            catch (Exception ex)
            {
                _logger.Error($"Busca de alunos por localidade GetAlunosByLocalidadeId: {ex.StackTrace}");
                return new JsonResult(ex.StackTrace);
            }
        }

        /// <summary>
        /// Busca de Alunos por Série
        /// </summary>
        /// <param name="id">Identificador da série</param>
        /// <returns>Retorna a lista de alunos</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Consultar)]
        public async Task<JsonResult> GetAlunosBySerieId(string id)
        {
            try
            {
                _logger.Info($"Busca de alunos por serie GetAlunosBySerieId: {id}");

                if (string.IsNullOrEmpty(id)) throw new Exception("Serie não informada.");
                var resultLocal = await ApiClientFactory.Instance.GetNomeAlunosBySerieId(Convert.ToInt32(id));

                return new JsonResult(new SelectList(resultLocal, "Id", "Nome"));

            }
            catch (Exception ex)
            {
                _logger.Error($"Busca de alunos por serie GetAlunosBySerieId: {ex.StackTrace}");
                return new JsonResult(ex.StackTrace);
            }
        }

        /// <summary>
        /// Busca de Alunos por Localidade
        /// </summary>
        /// <param name="id">Identificador da localidade</param>
        /// <returns>Retorna a lista de alunos</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Consultar)]
        public async Task<JsonResult> GetFomentoByLocalidadeId(string id)
        {
            try
            {
                _logger.Info($"Busca de fomento por localidade GetFomentoByLocalidadeId: {id}");

                if (string.IsNullOrEmpty(id)) throw new Exception("Localidade não informada.");
                var resultFomento = ApiClientFactory.Instance.GetFomentoByLocalidadeId(Convert.ToInt32(id));

                return new JsonResult(new SelectList(new List<FomentoDto>() { resultFomento }, "Id", "Nome"));

            }
            catch (Exception ex)
            {
                _logger.Error($"Busca de alunos por localidade GetFomentoByLocalidadeId: {ex.StackTrace}");
                return new JsonResult(ex.StackTrace);
            }
        }

        /// <summary>
        /// Busca de Idade do Aluno por Id
        /// </summary>
        /// <param name="id">Identificador do aluno</param>
        /// <returns>Retorna a idade do aluno</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Consultar)]
        public async Task<JsonResult> GetAlunoIdadeById(string id)
        {
            try
            {
                _logger.Info($"Busca de idade do Aluno por Id - GetAlunoIdadeById: {id}");

                if (string.IsNullOrEmpty(id)) throw new Exception("Aluno não informado.");
                var usu = await ApiClientFactory.Instance.GetAlunoById(Convert.ToInt32(id));

                return new JsonResult(usu.Idade);

            }
            catch (Exception ex)
            {
                _logger.Error($"Busca de idade do Aluno por Id - GetAlunoIdadeById: {ex.StackTrace}");
                return new JsonResult(ex.StackTrace);
            }
        }

        /// <summary>
        /// Busca de Aluno por id
        /// </summary>
        /// <param name="id">Identificador do aluno</param>
        /// <returns>Retorna o aluno</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Consultar)]
        public async Task<JsonResult> GetAlunoById(string id)
        {
            try
            {
                _logger.Info($"Busca de aluno por id - GetAlunoById: {id}");

                if (string.IsNullOrEmpty(id)) throw new Exception("Id do Aluno não informado.");
                var result = await ApiClientFactory.Instance.GetAlunoById(Convert.ToInt32(id));

                if (result.ByteImage != null)
                {
                    result.Image = GetImage(Convert.ToBase64String(result.ByteImage!));
                }

                if (result.QrCode != null) return new JsonResult(result);
                result.QrCode = GeraQrCode(result.Id);
                await ApiClientFactory.Instance.UpdateQrCode(result.Id, new AlunoModel.CreateUpdateDadosAlunoCommand()
                {
                    Id = result.Id,
                    QrCode = result.QrCode
                });
                return new JsonResult(result);

            }
            catch (Exception ex)
            {

                _logger.Error($"Busca de aluno por id - GetAlunoById: {ex.StackTrace}");

                return new JsonResult(ex);
            }
        }

        /// <summary>
        /// Busca aluno por Id
        /// </summary>
        /// <param name="id">Id do Aluno</param>
        /// <returns>retorna o aluno</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Consultar)]
        public async Task<JsonResult> GetAlunoTurmaById(int id)
        {
            var aluno = await ApiClientFactory.Instance.GetAlunoTurmaById(id);
            return Json(aluno);
        }

        /// <summary>
        /// Busca carteirinha por fomentoId
        /// </summary>
        /// <param name="fomentoId">Id do Fomento</param>
        /// <returns>retorna o modelo da carteirinha</returns>
        [HttpGet]
        public async Task<JsonResult> GetModeloCarteirinhaByFomento(int fomentoId)
        {
            var modeloCarteirinha = await ApiClientFactory.Instance.GetModeloCarteirinhaByFomentoId(fomentoId);
            return Json(modeloCarteirinha);
        }

        /// <summary>
        /// Método de busca de Aluno por cpf
        /// </summary>
        /// <param name="cpf">cpf do Aluno</param>
        /// <returns>retorna true ou false</returns>
        [ClaimsAuthorize(ClaimType.Usuario, Identity.Claim.Consultar)]
        public Task<JsonResult> GetAlunoByCpf(string cpf)
        {
            try
            {
                if (string.IsNullOrEmpty(cpf)) throw new Exception("Cpf não informado.");
                var result = ApiClientFactory.Instance.GetAlunoByCpf(Regex.Replace(cpf, "[^0-9a-zA-Z]+", ""));

                return Task.FromResult(Json(result));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex.Message));
            }
        }

        /// <summary>
        /// Método de busca de Aluno por email
        /// </summary>
        /// <param name="email">email do Aluno</param>
        /// <returns>retorna true ou false</returns>
        [ClaimsAuthorize(ClaimType.Usuario, Identity.Claim.Consultar)]
        public Task<JsonResult> GetAlunoByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) throw new Exception("Email não informado.");
                var result = ApiClientFactory.Instance.GetAlunoByEmail(email);

                return Task.FromResult(result == null ? Json(true) : Json(false));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex.Message));
            }
        }

        /// <summary>
        /// Busca lista de etapa de ensino
        /// </summary>
        /// <returns>Retorna lista de etaoa de ebsino</returns>
        [HttpGet]
        public Task<JsonResult> GetEtapasEnsinoAll()
        {
            try
            {
                var result = ApiClientFactory.Instance.GetEtapasEnsinoAll();

                return Task.FromResult(Json(new SelectList(result.Where(x => x.Id == "1"), "Id", "Nome")));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex.Message));
            }
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Ação de Upload de documentos do aluno
        /// </summary>
        /// <param name="collection">Lista de documentos a serem cadastrados</param>
        /// <returns>Retorna mensagem de Upload realizado através do parametro notfy e message</returns>
        [HttpPost]
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Upload)]
        public async Task<ActionResult> UploadDocumentos(IFormCollection collection)
        {
            try
            {
                string filePath = null;
                string fileName = null;

                var list = new List<CreateDocumentoAlunoDto>();

                foreach (var t in collection.Files)
                {
                    var file = t;
                    if (file.Length <= 0) continue;
                    fileName = $"{collection["alunoId"]}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    filePath = Path.Combine(_host.WebRootPath, $"Documentos\\{fileName}");

                    if (!Directory.Exists(Path.Combine(_host.WebRootPath, $"Documentos")))
                        Directory.CreateDirectory(Path.Combine(_host.WebRootPath, $"Documentos"));

                    using Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);

                    list.Add(new CreateDocumentoAlunoDto()
                    {
                        AlunoId = Convert.ToInt32(collection["alunoId"]),
                        NomeDocumento = fileName,
                        Url = filePath
                    });
                }
                await ApiClientFactory.Instance.CreateDocumentoAluno(list);

                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Success, message = "Upload de documentos realizado com sucesso." });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = EnumNotify.Error, mesage = e.Message });
            }
        }

        /// <summary>
        /// Ação de Download de documentos do aluno
        /// </summary>
        /// <param name="id">Id do aluno</param>
        /// <returns>Retorna Documentos para Download</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Download)]
        public ActionResult Download(int id)
        {
            var list = new List<FileContentResult>();

            var files = ApiClientFactory.Instance.GetDocumentosAllByAlunoId(id);

            MemoryStream outms = new MemoryStream();

            using (ZipArchive zar = new ZipArchive(outms, ZipArchiveMode.Create, false))
            {
                foreach (var file in files)
                {
                    var filePath = Path.Combine(_host.WebRootPath, $"Documentos\\{file.NomeDocumento}");

                    if (!System.IO.File.Exists(filePath))
                    {
                        return RedirectToAction(nameof(Index),
                            new { notify = (int)EnumNotify.Warning, message = "Documentos não encontrado." });
                    }

                    var fileBytes = System.IO.File.ReadAllBytes(filePath);

                    var fileName = file.NomeDocumento;

                    byte[] unzipped = fileBytes;
                    ZipArchiveEntry entry = zar.CreateEntry(fileName);
                    using (Stream str = entry.Open())
                    {
                        str.Write(unzipped);
                    }
                }
            }

            var outdata = outms.ToArray();

            var result = File(outdata, "application/zip", $"aluno-{id}.zip");
            return result;
        }

        /// <summary>
        /// Açao de Habiliatar um aluno no sistema
        /// </summary>
        /// <param name="collection">Coleção de dados para Habiliatr um aluno</param>
        /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
        [HttpPost]
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Habilitar)]
        public async Task<ActionResult> Habilitar(IFormCollection collection)
        {
            try
            {
                if (!String.IsNullOrEmpty(collection["lote"].ToString()))
                {
                    var lote = collection["lote"].ToString().Split(",");

                    foreach (string id in lote)
                    {
                        //var alunoId = collection["habilitarAlunoId"].ToString();
                        var alunoId = id;

                        var result = await ApiClientFactory.Instance.GetAlunoById(Convert.ToInt32(id));

                        var command = new UsuarioModel.CreateUpdateUsuarioCommand
                        {
                            Email = result.Email,
                            Nome = result.Nome,
                            CpfCnpj = result.Cpf == null ? "000.000.000-00" : result.Cpf,
                            LocalidadeId = Convert.ToInt32(result.LocalidadeId),
                            TipoPessoa = "PF",
                            MunicipioId = Convert.ToInt32(result.MunicipioId),
                            Status = true
                        };

                        var newUser = new IdentityUser { UserName = result.Id.ToString(), Email = command.Email, EmailConfirmed = true};
                        var userCreated = await _userManager.CreateAsync(newUser, $"senha{result.Id.ToString()}");

                        command.PerfilId = (int)EnumPerfil.Aluno;
                        var perfil = ApiClientFactory.Instance.GetPerfilById(command.PerfilId);

                        if (userCreated.Succeeded)
                        {
                            var userRole = _roleManager.Roles.FirstOrDefault(x => x.Id == perfil.AspNetRoleId).Name;

                            command.AspNetUserId = newUser.Id;
                            command.AspNetRoleId = perfil.AspNetRoleId;
                            command.PerfilId = perfil.Id;
                            command.Status = true;

                            var usuarioId = await ApiClientFactory.Instance.CreateUsuario(command);

                            if (usuarioId != 0)
                            {
                                await _userManager.AddToRoleAsync(newUser, userRole);

                                await ApiClientFactory.Instance.UpdateHabilitarAluno(Convert.ToInt32(alunoId), new AlunoModel.UpdateHabilitarAlunoCommand() { AlunoId = Convert.ToInt32(alunoId), AspNetUserId = newUser.Id });
                            }
                        }
                    }
                }
                else
                {
                    var id = collection["habilitarAlunoId"].ToString();

                    var result = await ApiClientFactory.Instance.GetAlunoById(Convert.ToInt32(id));

                    var command = new UsuarioModel.CreateUpdateUsuarioCommand
                    {
                        Email = result.Email,
                        Nome = result.Nome,
                        CpfCnpj = result.Cpf == null ? "000.000.000-00" : result.Cpf,
                        LocalidadeId = Convert.ToInt32(result.LocalidadeId),
                        TipoPessoa = "PF",
                        MunicipioId = Convert.ToInt32(result.MunicipioId),
                        Status = true
                    };

                    var newUser = new IdentityUser { UserName = result.Id.ToString(), Email = command.Email };
                    var userCreated = await _userManager.CreateAsync(newUser, $"senha{result.Id.ToString()}");

                    command.PerfilId = (int)EnumPerfil.Aluno;
                    var perfil = ApiClientFactory.Instance.GetPerfilById(command.PerfilId);

                    if (userCreated.Succeeded)
                    {
                        var userRole = _roleManager.Roles.FirstOrDefault(x => x.Id == perfil.AspNetRoleId).Name;

                        command.AspNetUserId = newUser.Id;
                        command.AspNetRoleId = perfil.AspNetRoleId;
                        command.PerfilId = perfil.Id;
                        command.Status = true;

                        var usuarioId = await ApiClientFactory.Instance.CreateUsuario(command);

                        if (usuarioId != 0)
                        {
                            await _userManager.AddToRoleAsync(newUser, userRole);

                            await ApiClientFactory.Instance.UpdateHabilitarAluno(Convert.ToInt32(id),
                                new AlunoModel.UpdateHabilitarAlunoCommand()
                                { AlunoId = Convert.ToInt32(id), AspNetUserId = newUser.Id });
                        }
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index), new
                        {
                            notify = (int)EnumNotify.Success,
                            message = "Este Aluno ja está habilitado a utilizar o sistema."
                        });
                    }
                        return RedirectToAction(nameof(Index), new
                        {
                            notify = (int)EnumNotify.Success,
                            message = "Aluno habilitado com sucesso."
                        });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index),
                    new
                    {
                        notify = (int)EnumNotify.Error,
                        message = $"Erro ao criar usuário. {e.Message}."
                    });
            }

            return null;
        }

        public async Task<ActionResult> Carteirinha()
        {
            return View();
        }



        /// <summary>
        /// Açao de inclusão de etapa de ensino
        /// </summary>
        /// <param name="collection">Coleção de dados para etapa de ensino</param>
        /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
        [HttpPost]
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        public async Task<JsonResult> CreateEtapaEnsino(string nome)
        {
            try
            {
                var command = new AlunoModel.CreateUpdateEtapaEnsinoCommand
                {
                    Nome = nome
                };

                await ApiClientFactory.Instance.CreateEtapaEnsino(command);

                var result = ApiClientFactory.Instance.GetEtapasEnsinoAll();

                return await Task.FromResult(Json(new SelectList(result, "Id", "Nome")));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex.Message));
            }
        }


        /// <summary>
        /// Açao de inclusão de grau de parentêsco
        /// </summary>
        /// <param name="collection">Coleção de dados para grau de parentêsco</param>
        /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
        [HttpGet]
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        public async Task<JsonResult> CreateGrauParentesco(string nome)
        {
            try
            {
                var command = new GrauParentescoModel.CreateUpdateGrauParentescoCommand()
                {
                    Nome = nome
                };

                await ApiClientFactory.Instance.CreateGrauParentesco(command);

                var result = ApiClientFactory.Instance.GetGrauParentescosAll();

                return await Task.FromResult(Json(new SelectList(result, "Id", "Nome")));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex.Message));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gera Qr Code
        /// </summary>
        /// <param name="alunoId">Id de aluno</param>
        /// <returns>Retorna o qrCode</returns>
        private static byte[]? GeraQrCode(long alunoId)
        {
            var text = $"http://dnadobrasil.org.br/Identity/Account/ControlePresenca?alunoId={alunoId}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeInfo = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeInfo);
            Bitmap qrBitmap = qrCode.GetGraphic(60);

            return BitmapToBytes(qrBitmap);
        }

        /// <summary>
        /// Busca os Bytes de Bitmap
        /// </summary>
        /// <param name="img">Bytemap de imagem </param>
        /// <returns>Retrona um stream array</returns>
        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using MemoryStream stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }

        /// <summary>
        /// Busca Imagem
        /// </summary>
        /// <param name="sBase64String">sBase64String</param>
        /// <returns>Retorna a imagem</returns>
        private byte[] GetImage(string sBase64String)
        {
            byte[] bytes = null;
            if (!string.IsNullOrEmpty(sBase64String))
            {
                bytes = Convert.FromBase64String(sBase64String);
            }

            return bytes;
        }

        /// <summary>
        /// Gera um PDF em formato CMYK contendo a carteirinha do aluno.
        /// </summary>
        /// <param name="aluno">Dados do aluno para preenchimento da carteirinha.</param>
        /// <param name="modeloCarteirinha">Modelo de carteirinha utilizado para gerar o PDF.</param>
        /// <returns>Retorna os bytes do arquivo PDF gerado.</returns>
        private async Task<byte[]> GerarPdfCmyk(AlunoDto aluno, ModeloCarteirinhaDto modeloCarteirinha)
        {
            // Caminho dos arquivos de background
            string frenteCaminhoRelativo = $"assets/styles_Carteirinha/modelos/{modeloCarteirinha.NomeImagemFrente}.tif";
            string versoCaminhoRelativo = $"assets/styles_Carteirinha/modelos/{modeloCarteirinha.NomeImagemVerso}.tif";

            // Converter caminhos relativos para absolutos
            string frenteCaminhoAbsoluto = System.IO.Path.Combine(_host.WebRootPath, frenteCaminhoRelativo.Replace("/", System.IO.Path.DirectorySeparatorChar.ToString()));
            string versoCaminhoAbsoluto = System.IO.Path.Combine(_host.WebRootPath, versoCaminhoRelativo.Replace("/", System.IO.Path.DirectorySeparatorChar.ToString()));

            // Verificar se os arquivos existem
            if (!System.IO.File.Exists(frenteCaminhoAbsoluto))
            {
                _logger.Error($"Arquivo de fundo da frente não encontrado: {frenteCaminhoAbsoluto}");
                throw new FileNotFoundException("Arquivo de fundo da frente não encontrado", frenteCaminhoAbsoluto);
            }

            if (!System.IO.File.Exists(versoCaminhoAbsoluto))
            {
                _logger.Error($"Arquivo de fundo do verso não encontrado: {versoCaminhoAbsoluto}");
                throw new FileNotFoundException("Arquivo de fundo do verso não encontrado", versoCaminhoAbsoluto);
            }

            // Criar memorystream para armazenar o PDF
            using (MemoryStream ms = new MemoryStream())
            {
                // Definição da sangria de 3mm (em pontos)
                float sangriaEmPontos = 3f / 10f * 28.35f; // 3mm em pontos

                // Dimensões originais do documento
                float larguraOriginal = 8.5f * 28.35f;
                float alturaOriginal = 5.4f * 28.35f;

                // Dimensões com sangria
                float larguraComSangria = larguraOriginal + (2 * sangriaEmPontos);
                float alturaComSangria = alturaOriginal + (2 * sangriaEmPontos);

                // Configuração das cores CMYK para todo o documento
                // Cores básicas em CMYK para uso em todo o documento - usando "rich black" para evitar conversão para GRAY
                DeviceCmyk corPreto = new DeviceCmyk(0.75f, 0.68f, 0.67f, 0.9f); // "Rich black" CMYK
                DeviceCmyk corAzul = new DeviceCmyk(0.75f, 0.68f, 0, 0.2f);
                DeviceCmyk corBranca = new DeviceCmyk(0, 0, 0, 0); // Branco em CMYK

                // Criar documento PDF com suporte a CMYK
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);

                // Configurar Output Intent para CMYK - isso já deve forçar o uso de cores CMYK
                PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "",
                    "http://www.color.org", "FOGRA39", null);
                pdf.AddOutputIntent(outputIntent);

                // Configurações para o documento com tamanho ajustado para incluir sangria
                Document document = new Document(pdf, new PageSize(larguraComSangria, alturaComSangria));
                document.SetMargins(0, 0, 0, 0);

                // === FRENTE DA CARTEIRINHA ===

                // Adicionar imagem de fundo da frente cobrindo toda a área incluindo sangria
                ImageData imgDataFrente = ImageDataFactory.Create(frenteCaminhoAbsoluto);
                iText.Layout.Element.Image backgroundFrente = new iText.Layout.Element.Image(imgDataFrente);
                backgroundFrente.SetFixedPosition(0, 0);
                backgroundFrente.SetWidth(larguraComSangria);
                backgroundFrente.SetHeight(alturaComSangria);
                document.Add(backgroundFrente);

                // Adicionar informações do aluno - agora com a margem de sangria incluída nas posições
                float leftMargin = 0.5f * 28.35f + sangriaEmPontos; // 0.5cm do original + sangria
                float textWidth = 5.5f * 28.35f;
                float labelWidth = 2.5f * 28.35f; // Largura da label
                float valueWidth = 3.0f * 28.35f; // Largura do valor

                // Ajustando a posição vertical incluindo a sangria
                float startY = alturaComSangria - 2.3f * 28.35f - sangriaEmPontos - 10; // Começando 2.3cm do topo + ajuste pela sangria - 10 pontos aproximadamente 0.33333 cm

                // Starting Y position com padding-top ajustado
                float currentY = startY;
                float alturaItem;

                // Nome
                alturaItem = AddInfoRow(document, "NOME DO ESTUDANTE:", aluno.Nome, leftMargin, currentY, labelWidth, valueWidth, corAzul);
                currentY -= alturaItem;

                // Data de nascimento
                alturaItem = AddInfoRow(document, "DATA DE NASCIMENTO:", aluno.DtNascimento, leftMargin, currentY, labelWidth, valueWidth, corAzul);
                currentY -= alturaItem;

                // Telefone
                alturaItem = AddInfoRow(document, "TELEFONE:", aluno.Celular, leftMargin, currentY, labelWidth, valueWidth, corAzul);
                currentY -= alturaItem;

                // CPF
                alturaItem = AddInfoRow(document, "CPF:", aluno.Cpf, leftMargin, currentY, labelWidth, valueWidth, corAzul);
                currentY -= alturaItem;

                // Matrícula
                alturaItem = AddInfoRow(document, "MATRÍCULA:", aluno.Id.ToString(), leftMargin, currentY, labelWidth, valueWidth, corAzul);
                currentY -= alturaItem;

                // Modalidades - agora ajustando o currentY com base na altura ocupada
                alturaItem = AddFullWidthText(document, aluno.Modalidades, leftMargin, currentY, labelWidth + valueWidth, corAzul, false);
                currentY -= alturaItem;

                // Adicionar foto do aluno - ajustado para incluir sangria
                float rightMargin = 0.91f * 28.35f;
                float topMargin = 0.795f * 28.35f + sangriaEmPontos / 3;
                float fotoWidth = 48.5f;
                float fotoHeight = 69.6f; // Altura reduzida para melhor proporção

                // Calculando posição X a partir da direita (incluindo sangria)
                float fotoX = larguraComSangria - rightMargin - fotoWidth;
                // Calculando posição Y a partir do topo (incluindo sangria)
                float fotoY = alturaComSangria - topMargin - fotoHeight;

                // Definir o raio para as bordas arredondadas (equivalente a border-radius: 4px)
                float radioBorda = 4; // 4 pontos (aproximadamente 1.4mm)

                if (aluno.ByteImage != null && aluno.ByteImage.Length > 0)
                {
                    // Manter as dimensões originais já que não temos mais a borda visível
                    float fotoInternalX = fotoX;
                    float fotoInternalY = fotoY;
                    float fotoInternalWidth = fotoWidth;
                    float fotoInternalHeight = fotoHeight;

                    // Antes de adicionar a imagem, desenhar um fundo branco com cantos arredondados
                    PdfCanvas canvasFundoFoto = new PdfCanvas(pdf.GetPage(1));
                    canvasFundoFoto.SaveState();

                    // Definir um fundo branco em CMYK
                    canvasFundoFoto.SetFillColor(corBranca);
                    canvasFundoFoto.RoundRectangle(fotoInternalX, fotoInternalY, fotoInternalWidth, fotoInternalHeight, radioBorda);
                    canvasFundoFoto.Fill();
                    canvasFundoFoto.RestoreState();

                    ImageData imgDataFoto = ImageDataFactory.Create(aluno.ByteImage);

                    // Obter as dimensões originais da imagem
                    float imgOriginalWidth = imgDataFoto.GetWidth();
                    float imgOriginalHeight = imgDataFoto.GetHeight();

                    // Calcular razões de aspecto
                    float imgRatio = imgOriginalWidth / imgOriginalHeight;
                    float containerRatio = fotoInternalWidth / fotoInternalHeight;

                    iText.Layout.Element.Image foto = new iText.Layout.Element.Image(imgDataFoto);

                    // Implementando comportamento similar ao background-size: cover
                    if (imgRatio > containerRatio)
                    {
                        // Ajustar pela altura e permitir que a largura transborde
                        foto.SetHeight(fotoInternalHeight);
                        // Calcular a nova largura mantendo a proporção
                        float newWidth = fotoInternalHeight * imgRatio;
                        foto.SetWidth(newWidth);
                        // Centralizar horizontalmente
                        float offsetX = (newWidth - fotoInternalWidth) / 2;
                        foto.SetFixedPosition(fotoInternalX - offsetX, fotoInternalY);
                    }
                    else
                    {
                        // Ajustar pela largura e permitir que a altura transborde
                        foto.SetWidth(fotoInternalWidth);
                        // Calcular a nova altura mantendo a proporção
                        float newHeight = fotoInternalWidth / imgRatio;
                        foto.SetHeight(newHeight);
                        // Centralizar verticalmente
                        float offsetY = (newHeight - fotoInternalHeight) / 2;
                        foto.SetFixedPosition(fotoInternalX, fotoInternalY - offsetY);
                    }

                    // Aplicar o recorte em formato arredondado na imagem
                    PdfCanvas clipCanvas = new PdfCanvas(pdf.GetPage(1));
                    clipCanvas.SaveState();

                    // Forçar uso de CMYK para o clipping path
                    clipCanvas.SetFillColor(corPreto);
                    clipCanvas.SetStrokeColor(corPreto);

                    // Aplicar clipping
                    clipCanvas.RoundRectangle(fotoInternalX, fotoInternalY, fotoInternalWidth, fotoInternalHeight, radioBorda);
                    clipCanvas.Clip().EndPath();

                    document.Add(foto);

                    clipCanvas.RestoreState();
                }
                else
                {
                    // Se não tiver foto, usar imagem padrão com bordas arredondadas
                    string fotoDefaultPath = aluno.Sexo == "Feminino"
                        ? System.IO.Path.Combine(_host.WebRootPath, "assets", "images", "menina.png")
                        : System.IO.Path.Combine(_host.WebRootPath, "assets", "images", "menino.png");

                    // Manter as dimensões originais 
                    float fotoInternalX = fotoX;
                    float fotoInternalY = fotoY;
                    float fotoInternalWidth = fotoWidth;
                    float fotoInternalHeight = fotoHeight;

                    // Antes de adicionar a imagem, desenhar um fundo branco com cantos arredondados
                    PdfCanvas canvasFundoFotoDefault = new PdfCanvas(pdf.GetPage(1));
                    canvasFundoFotoDefault.SaveState();

                    // Definir um fundo branco em CMYK
                    canvasFundoFotoDefault.SetFillColor(corBranca);
                    canvasFundoFotoDefault.RoundRectangle(fotoInternalX, fotoInternalY, fotoInternalWidth, fotoInternalHeight, radioBorda);
                    canvasFundoFotoDefault.Fill();
                    canvasFundoFotoDefault.RestoreState();

                    ImageData imgDataDefault = ImageDataFactory.Create(fotoDefaultPath);

                    // Obter as dimensões originais da imagem
                    float imgOriginalWidth = imgDataDefault.GetWidth();
                    float imgOriginalHeight = imgDataDefault.GetHeight();

                    // Calcular razões de aspecto
                    float imgRatio = imgOriginalWidth / imgOriginalHeight;
                    float containerRatio = fotoInternalWidth / fotoInternalHeight;

                    iText.Layout.Element.Image fotoDefault = new iText.Layout.Element.Image(imgDataDefault);

                    // Implementando comportamento similar ao background-size: cover para imagem padrão
                    if (imgRatio > containerRatio)
                    {
                        // Ajustar pela altura e permitir que a largura transborde
                        fotoDefault.SetHeight(fotoInternalHeight);
                        // Calcular a nova largura mantendo a proporção
                        float newWidth = fotoInternalHeight * imgRatio;
                        fotoDefault.SetWidth(newWidth);
                        // Centralizar horizontalmente
                        float offsetX = (newWidth - fotoInternalWidth) / 2;
                        fotoDefault.SetFixedPosition(fotoInternalX - offsetX, fotoInternalY);
                    }
                    else
                    {
                        // Ajustar pela largura e permitir que a altura transborde
                        fotoDefault.SetWidth(fotoInternalWidth);
                        // Calcular a nova altura mantendo a proporção
                        float newHeight = fotoInternalWidth / imgRatio;
                        fotoDefault.SetHeight(newHeight);
                        // Centralizar verticalmente
                        float offsetY = (newHeight - fotoInternalHeight) / 2;
                        fotoDefault.SetFixedPosition(fotoInternalX, fotoInternalY - offsetY);
                    }

                    // Aplicar o recorte em formato arredondado na imagem padrão com cores CMYK
                    PdfCanvas clipCanvasDefault = new PdfCanvas(pdf.GetPage(1));
                    clipCanvasDefault.SaveState();

                    // Forçar uso de CMYK para o clipping path
                    clipCanvasDefault.SetFillColor(corPreto);
                    clipCanvasDefault.SetStrokeColor(corPreto);

                    clipCanvasDefault.RoundRectangle(fotoInternalX, fotoInternalY, fotoInternalWidth, fotoInternalHeight, radioBorda);
                    clipCanvasDefault.Clip().EndPath();

                    document.Add(fotoDefault);

                    clipCanvasDefault.RestoreState();
                }

                // Adicionar QR Code - ajustado para incluir sangria
                if (aluno.QrCode != null && aluno.QrCode.Length > 0)
                {
                    // Cálculo correto baseado no CSS: right: 0.98cm, bottom: 0.8cm
                    float qrRightMargin = 0.62f * 28.35f + sangriaEmPontos;
                    float qrBottomMargin = 0.4f * 28.35f + sangriaEmPontos;
                    float qrWidth = 48.5f;
                    float qrHeight = 48.5f;

                    // Calculando posição X a partir da direita (incluindo sangria)
                    float qrX = larguraComSangria - qrRightMargin - qrWidth;
                    // Calculando posição Y a partir do fundo (incluindo sangria)
                    float qrY = qrBottomMargin;

                    ImageData imgDataQr = ImageDataFactory.Create(aluno.QrCode);
                    iText.Layout.Element.Image qrCode = new iText.Layout.Element.Image(imgDataQr);
                    qrCode.SetFixedPosition(qrX, qrY);
                    qrCode.SetHeight(qrHeight);
                    qrCode.SetWidth(qrWidth);

                    // Garantir que o QR Code mantenha sua proporção
                    qrCode.ScaleToFit(qrWidth, qrHeight);

                    document.Add(qrCode);
                }

                // Nova página para o verso
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                // === VERSO DA CARTEIRINHA ===

                // Adicionar imagem de fundo do verso cobrindo toda a área incluindo sangria
                ImageData imgDataVerso = ImageDataFactory.Create(versoCaminhoAbsoluto);
                iText.Layout.Element.Image backgroundVerso = new iText.Layout.Element.Image(imgDataVerso);
                backgroundVerso.SetFixedPosition(0, 0);
                backgroundVerso.SetWidth(larguraComSangria);
                backgroundVerso.SetHeight(alturaComSangria);
                document.Add(backgroundVerso);

                // Adicionar informações do verso - ajustado para incluir sangria
                float versoLeftMargin = 0.6f * 28.35f + sangriaEmPontos;

                // Usando a mesma lógica de posicionamento vertical da frente
                float versoStartY = alturaComSangria - 2.3f * 28.35f - sangriaEmPontos - 10;

                // Movendo os textos para a mesma posição inicial da frente
                float versoY = versoStartY;

                // Município/Estado
                alturaItem = AddInfoRow(document, "MUNICÍPIO/ESTADO:", aluno.MunicipioEstado, versoLeftMargin, versoY, labelWidth, valueWidth, corAzul, 8f);
                versoY -= alturaItem;

                // Unidade Escolar
                alturaItem = AddInfoRow(document, "UNIDADE ESCOLAR:", aluno.NomeLocalidade, versoLeftMargin, versoY, labelWidth, valueWidth, corAzul, 8f);
                versoY -= alturaItem;

                // Adicionar sangria
                DefinirTrimBox(pdf, sangriaEmPontos, larguraComSangria, alturaComSangria);

                // Fechar documento
                document.Close();

                // Retornar os bytes do PDF
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Método auxiliar da carteirinha para adicionar uma linha de informação com suporte adequado a textos longos
        /// </summary>
        /// <param name="document">O documento onde adicionar os elementos</param>
        /// <param name="label">O texto da label</param>
        /// <param name="value">O valor a ser exibido</param>
        /// <param name="x">Posição X inicial</param>
        /// <param name="y">Posição Y inicial</param>
        /// <param name="labelWidth">Largura da label</param>
        /// <param name="valueWidth">Largura máxima do valor</param>
        /// <param name="color">Cor do texto</param>
        /// <param name="spacing">Espaçamento vertical adicional (opcional)</param>
        /// <returns>A altura total ocupada pelo elemento, incluindo espaçamento</returns>
        private float AddInfoRow(Document document, string label, string value, float x, float y,
                                 float labelWidth, float valueWidth, DeviceCmyk color, float spacing = 4f)
        {
            // Adicionar a label
            Paragraph labelParagraph = new Paragraph(label)
                .SetFontSize(6)
                .SetBold()
                .SetFontColor(color);
            labelParagraph.SetFixedPosition(x, y, labelWidth);
            document.Add(labelParagraph);

            // Adicionar o valor
            Paragraph valueParagraph = new Paragraph(value)
                .SetFontSize(6)
                .SetFontColor(color);

            // Estimar o número de linhas que o texto ocupará
            float charsPerLine = valueWidth / 3.5f;
            int linhasEstimadas = (int)Math.Ceiling(value.Length / charsPerLine);
            float alturaLinha = 8f;

            // Posicionar e adicionar o valor
            if (linhasEstimadas > 1)
            {
                // Para texto multilinha, precisamos forçar quebra de linha
                valueParagraph.SetWidth(valueWidth);

                // Criar um elemento Text que permite quebra de linha
                Text textoValue = new Text(value);
                valueParagraph = new Paragraph().Add(textoValue)
                    .SetFontSize(6)
                    .SetFontColor(color)
                    .SetWidth(valueWidth);

                // Posicionar o valor na mesma altura da label
                valueParagraph.SetFixedPosition(x + labelWidth, y - (linhasEstimadas - 1) * alturaLinha, valueWidth);
                document.Add(valueParagraph);

                // Retornar a altura que este item ocupou (com espaçamento adicional)
                return Math.Max(alturaLinha, linhasEstimadas * alturaLinha) + spacing - 1;
            }
            else
            {
                // Para texto de uma linha, posicionamento simples
                valueParagraph.SetFixedPosition(x + labelWidth, y, valueWidth);
                document.Add(valueParagraph);
                return alturaLinha + spacing;
            }
        }

        /// <summary>
        /// Método auxiliar para adicionar a sangria no documento com tratamento de erro aprimorado
        /// </summary>
        private void DefinirTrimBox(PdfDocument pdf, float sangria, float larguraComSangria, float alturaComSangria)
        {
            try
            {
                if (pdf == null)
                {
                    _logger.Error("Documento PDF nulo ao definir TrimBox");
                    return;
                }

                // Calcular as dimensões do TrimBox (área após o corte)
                Rectangle trimBox = new Rectangle(
                    sangria,             // x - início da área de corte (sangria)
                    sangria,             // y - início da área de corte (sangria)
                    larguraComSangria - (2 * sangria),  // largura da área de corte
                    alturaComSangria - (2 * sangria)    // altura da área de corte
                );

                // Obter o número de páginas com verificação de segurança
                int numPaginas = pdf.GetNumberOfPages();
                _logger.Info($"Definindo TrimBox para {numPaginas} páginas");

                // Aplicar o TrimBox a cada página do documento com verificação adicional
                for (int i = 1; i <= numPaginas; i++)
                {
                    try
                    {
                        // Obter a página com verificação de nulo
                        PdfPage pagina = pdf.GetPage(i);
                        if (pagina != null)
                        {
                            // Definir o TrimBox para a página
                            pagina.SetTrimBox(trimBox);

                            // Definir também BleedBox para compatibilidade com diferentes softwares
                            pagina.SetBleedBox(new Rectangle(0, 0, larguraComSangria, alturaComSangria));
                        }
                        else
                        {
                            _logger.Warn($"Página {i} nula ao definir TrimBox");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Logar o erro mas continuar para as próximas páginas
                        _logger.Error($"Erro ao definir TrimBox para página {i}: {ex.Message}", ex);
                        // Não relançar a exceção para permitir que o resto do documento seja processado
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro geral ao definir TrimBox: {ex.Message}", ex);
                // Registrar o erro, mas não lançar exceção para não interromper a geração do PDF
            }
        }

        /// <summary>
        /// Método auxiliar da carteirinha para adicionar texto com largura total (ocupando espaço de label + value)
        /// </summary>
        /// <param name="document">O documento onde adicionar o texto</param>
        /// <param name="text">O texto a ser adicionado</param>
        /// <param name="x">Posição X</param>
        /// <param name="y">Posição Y</param>
        /// <param name="totalWidth">Largura total disponível</param>
        /// <param name="color">Cor do texto</param>
        /// <param name="isBold">Se o texto deve ser negrito</param>
        /// <returns>A altura total ocupada pelo texto, incluindo espaçamento</returns>
        private float AddFullWidthText(Document document, string text, float x, float y, float totalWidth, DeviceCmyk color, bool isBold = false)
        {
            if (string.IsNullOrEmpty(text))
                return 8f; // Altura mínima se não houver texto

            Paragraph paragraph = new Paragraph(text);
            paragraph.SetFontSize(6); // Mantendo o tamanho da fonte em 6px
            if (isBold)
            {
                paragraph.SetBold();
            }
            paragraph.SetFontColor(color);
            paragraph.SetFixedPosition(x, y, totalWidth);

            // Permitir quebra de linha caso o texto seja muito longo
            paragraph.SetMultipliedLeading(1.2f); // Espaçamento entre linhas
            paragraph.SetTextAlignment(TextAlignment.LEFT);

            // Estimar o número de linhas que o texto ocupará
            float charsPerLine = totalWidth / 3.5f; // Aproximação baseada no tamanho da fonte
            int linhasEstimadas = (int)Math.Ceiling(text.Length / charsPerLine);
            float alturaLinha = 8f; // Altura base de uma linha com fonte tamanho 6

            // Se o texto ocupar mais de uma linha, ajustar a posição Y
            if (linhasEstimadas > 1)
            {
                // Ajustar a posição Y para cima para acomodar as linhas extras
                float adjustedY = y - ((linhasEstimadas - 1) * alturaLinha * 0.8f);
                paragraph.SetFixedPosition(x, adjustedY, totalWidth);
            }

            document.Add(paragraph);

            // Retornar a altura total ocupada (incluindo um pequeno espaçamento)
            return Math.Max(alturaLinha, linhasEstimadas * alturaLinha * 0.8f) + 4f;
        }

        /// <summary>
        /// Verifica se os recursos necessários para geração do PDF estão disponíveis
        /// </summary>
        private bool VerificarAmbientePdfCmyk()
        {
            try
            {
                // Verificar se os diretórios de templates existem
                string templatesPath = System.IO.Path.Combine(_host.WebRootPath, "assets", "styles_Carteirinha", "modelos");
                if (!System.IO.Directory.Exists(templatesPath))
                {
                    _logger.Error($"Diretório de modelos de carteirinha não encontrado: {templatesPath}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao verificar ambiente para geração de PDF CMYK: {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// Método para envio de email
        /// </summary>
        /// <param name="user">identidade do usuário</param>
        /// <param name="email">email a ser enviado</param>
        /// <param name="nome">nome da pessoa que receberá o email</param>
        [ClaimsAuthorize(ClaimType.Usuario, Identity.Claim.Excluir)]
        private async Task SendNewUserEmail(IdentityUser user, string email, string nome)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.ActionLink("ResetPassword", "Identity/Account", new { code, email });

            var message =
                System.IO.File.ReadAllText(Path.Combine(_host.WebRootPath, "emailtemplates/ConfirmEmail.html"));
            message = message.Replace("%NAME%", nome);
            message = message.Replace("%CALLBACK%", HtmlEncoder.Default.Encode(callbackUrl.Replace("%2FAccount", "/Account")));

            await _emailSender.SendEmailAsync(user.Email, "Primeiro acesso sistema Dna do Brasil",
                message);
        }

        /// <summary>
        /// Busca Raça Cor para popular a combo
        /// </summary>
        /// <returns>Retorna lista de Etnias Raça Cor</returns>
        private SelectList GetEtniasRacaCor()
        {
            List<SelectListDto> list = new List<SelectListDto>
            {
                new() { IdNome = "NAODECLARADA", Nome = "NÃO DECLARADA" },
                new() { IdNome = "PARDA", Nome = "PARDA" },
                new() { IdNome = "BRANCA", Nome = "BRANCA" },
                new() { IdNome = "PRETA", Nome = "PRETA" },
                new() { IdNome = "INDIGENA", Nome = "INDÍGENA" },
                new() { IdNome = "AMARELA", Nome = "AMARELA" }
            };

            return new SelectList(list, "IdNome", "Nome");
        }
        #endregion


    }
}
