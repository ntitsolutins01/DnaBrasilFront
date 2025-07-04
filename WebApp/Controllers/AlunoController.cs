using System.Drawing;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
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
        /// <param name="collection">Lista de filtros selecionados para pesquisa de alunos</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Consultar)]
        public async Task<ActionResult> Index(int? crud, int? notify, IFormCollection collection, string message = null)
        {
            try
            {

                _logger.Info($"Usuario Logado em Aluno.Index User.Identity.Name : {User.Identity.Name}");

                var usuario = User.Identity.Name;

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                _logger.Info($"GetUsuarioByEmail");
                //var aluno = await ApiClientFactory.Instance.GetAlunoById(Convert.ToInt32(usuario));

                var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

                var possuiFoto = collection["possuiFoto"].ToString();

                var searchFilter = new AlunosFilterDto
                {
                    FomentoId = collection["ddlFomento"].ToString(),
                    Estado = collection["ddlEstado"].ToString(),
                    MunicipioId = collection["ddlMunicipio"].ToString(),
                    LocalidadeId = collection["ddlLocalidade"].ToString() == "" ? usu.LocalidadeId : collection["ddlLocalidade"].ToString(),
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

                bool filtroVazio = string.IsNullOrEmpty(searchFilter.MunicipioId)
                    ? string.IsNullOrEmpty(searchFilter.FomentoId)
                        ? string.IsNullOrEmpty(searchFilter.LocalidadeId)
                            ? string.IsNullOrEmpty(searchFilter.Sexo)
                                ? string.IsNullOrEmpty(searchFilter.DeficienciaId)
                                    ? string.IsNullOrEmpty(searchFilter.Estado)
                                        ? string.IsNullOrEmpty(searchFilter.Etnia)
                                            ? string.IsNullOrEmpty(searchFilter.Nome)
                                                ? string.IsNullOrEmpty(searchFilter.Matricula)
                                                    ? string.IsNullOrEmpty(searchFilter.ProfissionalId)
                                                    : false
                                                : false
                                            : false
                                        : false
                                    : false
                                : false
                            : false
                        : false
                    : false;

                if (filtroVazio)
                {
                    result.Alunos = (List<AlunoIndexDto>?)result.Alunos.ToList()
                        .Where(x => x.MunicipioId == usu.MunicipioId.ToString()).ToList();
                }

                //var listFomentos = ApiClientFactory.Instance.GetFomentosAll();
                //var fomentos = new SelectList(listFomentos, "Id", "Nome", searchFilter.FomentoId);
                var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome", searchFilter.FomentoId);

                var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll().Where(x => x.Status), "Id", "Nome", searchFilter.DeficienciaId);
                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", usu.Uf);
                var profissionais = new SelectList(ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId)), "Id", "Nome");

                List<SelectListDto> listSexo = new List<SelectListDto>
                {
                    new() { IdNome = "M", Nome = "MASCULINO" },
                    new() { IdNome = "F", Nome = "FEMININO" }
                };

                var sexos = new SelectList(listSexo, "IdNome", "Nome", searchFilter.Sexo);

                List<SelectListDto> list = new List<SelectListDto>
                {
                    new() { IdNome = "PARDO", Nome = "PARDO" },
                    new() { IdNome = "BRANCO", Nome = "BRANCO" },
                    new() { IdNome = "PRETO", Nome = "PRETO" },
                    new() { IdNome = "INDIGENA", Nome = "INDIGENA" },
                    new() { IdNome = "AMARELO", Nome = "AMARELO" }
                };

                var etnias = new SelectList(list, "IdNome", "Nome", searchFilter.Etnia);

                SelectList municipios = null;

                if (!string.IsNullOrEmpty(usu.Uf))
                {
                    municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(usu.Uf), "Id", "Nome", usu.MunicipioId);
                }

                SelectList localidades = null;

                //if (usu.MunicipioId != null)
                //{
                //    var fomento = ApiClientFactory.Instance.GetFomentoLocalidadesByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));

                //    IEnumerable<LocalidadeDto> resultLocalidades;

                //    resultLocalidades = usu.Perfil.Id != (int)EnumPerfil.Administrador
                //        ? ApiClientFactory.Instance.GetLocalidadeByMunicipio(usu.MunicipioId.ToString())
                //            .Where(x => fomento.LocalidadesIds.Contains(x.Id))
                //        : ApiClientFactory.Instance.GetLocalidadeByMunicipio(usu.MunicipioId.ToString());

                //    if (resultLocalidades != null)
                //        localidades = new SelectList(resultLocalidades, "Id", "Nome", usu.LocalidadeId);

                //    fomentos = new SelectList(listFomentos, "Id", "Nome", fomento.Id);
                //}

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
                    ListProfissionais = profissionais

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
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var usuario = User.Identity.Name;

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

            List<SelectListDto> list = new List<SelectListDto>
            {
                new() { IdNome = "PARDO", Nome = "PARDO" },
                new() { IdNome = "BRANCO", Nome = "BRANCO" },
                new() { IdNome = "PRETO", Nome = "PRETO" },
                new() { IdNome = "INDIGENA", Nome = "INDIGENA" },
                new() { IdNome = "AMARELO", Nome = "AMARELO" }
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
                UsuarioLogado = usu
            });
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
                    turmas = new SelectList(
                       ApiClientFactory.Instance
                           .GetTurmasByLocalidadeIdEtapaIdSerie(Convert.ToInt32(aluno.LocalidadeId), Convert.ToInt32(aluno.EtapaId), aluno.SerieNome)
                           .Select(s => new { Id = s.Id, Turma = s.Turma }).ToList(), "Id", "Turma", aluno.SerieId);
                }


                List<SelectListDto> list = new List<SelectListDto>
                {
                    new() { IdNome = "PARDO", Nome = "PARDO" },
                    new() { IdNome = "BRANCO", Nome = "BRANCO" },
                    new() { IdNome = "PRETO", Nome = "PRETO" },
                    new() { IdNome = "INDIGENA", Nome = "INDIGENA" },
                    new() { IdNome = "AMARELO", Nome = "AMARELO" }
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
                    ListTurmas = turmas

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
                    ProfissionalId = collection["ddlProfissionalAluno"] == "" ? null : Convert.ToInt32(collection["ddlProfissionalAluno"].ToString()),
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
                    CopiaDocAlunoResponsavel = Convert.ToBoolean(collection["copiaDoc"].ToString()),
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
                await ApiClientFactory.Instance.CreateDocumentosAluno(list);

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

                string filePath = null;

                var status = collection["status"].ToString();
                var habilitado = collection["habilitado"].ToString();


                var command = new AlunoModel.CreateUpdateDadosAlunoCommand
                {
                    Id = Convert.ToInt32(id),
                    Etnia = collection["ddlEtnia"] == "" ? null : collection["ddlEtnia"].ToString(),
                    MunicipioId = collection["ddlMunicipio"] == "" ? null : Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                    ProfissionalId = collection["ddlProfissionalAluno"] == "" ? null : Convert.ToInt32(collection["ddlProfissionalAluno"].ToString()),
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
                    DeficienciasIds = collection["arrDeficiencias"] == "" ? null : collection["arrDeficiencias"].ToString(),
                    Habilitado = habilitado != "",
                    Status = status != "",
                    NomeFoto = filePath,
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
                return RedirectToAction(nameof(Index), new { notify = EnumNotify.Error, mesage = e.Message });
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

                    // Modalidades
                    AddFullWidthText(document, aluno.Modalidades, leftMargin, currentY, labelWidth + valueWidth, corAzul, false);

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
                    new() { IdNome = "PARDO", Nome = "PARDO" },
                    new() { IdNome = "BRANCO", Nome = "BRANCO" },
                    new() { IdNome = "PRETO", Nome = "PRETO" },
                    new() { IdNome = "INDIGENA", Nome = "INDIGENA" },
                    new() { IdNome = "AMARELO", Nome = "AMARELO" }
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
                //var alunoId = collection["habilitarAlunoId"].ToString();

                var arr = new int[]
                {

48438,
48439,
48440,
48441,
48442,
48443,
48444,
48445,
48446,
48447,
48448,
48449,
48450,
48451,
48452,
48453,
48454,
48455,
48456,
48457,
48458,
48459,
48460,
48461,
48462,
48463,
48464,
48465,
48466,
48467,
48468,
48469,
48470,
48471,
48472,
48473,
48474,
48475,
48476,
48477,
48478,
48479,
48480,
48481,
48482,
48483,
48484,
48485,
48486,
48487,
48488,
48489,
48490,
48491,
48492,
48493,
48494,
48495,
48496,
48497,
48498,
48499,
48500,
48501,
48502,
48503,
48504,
48505,
48506,
48507,
48508,
48509,
48510,
48511,
48512,
48513,
48514,
48515,
48516,
48517,
48518,
48519,
48520,
48521,
48522,
48523,
48524,
48525,
48526,
48527,
48528,
48529,
48530,
48531,
48532,
48533,
48534,
48535,
48536,
48537,
48538,
48539,
48540,
48541,
48542,
48543,
48544,
48545,
48546,
48547,
48548,
48549,
48550,
48551,
48552,
48553,
48554,
48555,
48556,
48557,
48558,
48559,
48560,
48561,
48562,
48563,
48564,
48565,
48566,
48567,
48568,
48569,
48570,
48571,
48572,
48573,
48574,
48575,
48576,
48577,
48578,
48579,
48580,
48581,
48582,
48583,
48584,
48585,
48586,
48587,
48588,
48589,
48590,
48591,
48592,
48593,
48594,
48595,
48596,
48597,
48598,
48599,
48600,
48601,
48602,
48603,
48604,
48605,
48606,
48607,
48608,
48609,
48610,
48611,
48612,
48613,
48614,
48615,
48616,
48617,
48618,
48619,
48620,
48621,
48622,
48623,
48624,
48625,
48626,
48627,
48628,
48629,
48630,
48631,
48632,
48633,
48634,
48635,
48636,
48637,
48638,
48639,
48640,
48641,
48642,
48643,
48644,
48645,
48646,
48647,
48648,
48649,
48650,
48651,
48652,
48653,
48654,
48655,
48656,
48657,
48658,
48659,
48660,
48661,
48662,
48663,
48664,
48665,
48666,
48667,
48668,
48669,
48670,
48671,
48672,
48673,
48674,
48675,
48676,
48677,
48678,
48679,
48680,
48681,
48682,
48683,
48684,
48685,
48686,
48687,
48688,
48689,
48690,
48691,
48692,
48693,
48694,
48695,
48696,
48697,
48698,
48699,
48700,
48701,
48702,
48703,
48704,
48705,
48706,
48707,
48708,
48709,
48710,
48711,
48712,
48713,
48714,
48715,
48716,
48717,
48718,
48719,
48720,
48721,
48722,
48723,
48724,
48725,
48726,
48727,
48728,
48729,
48730,
48731,
48732,
48733,
48734,
48735,
48736,
48737,
48738,
48739,
48740,
48741,
48742,
48743,
48744,
48745,
48746,
48747,
48748,
48749,
48750,
48751,
48752,
48753,
48754,
48755,
48756,
48757,
48758,
48759,
48760,
48761,
48762,
48763,
48764,
48765,
48766,
48767,
48768,
48769,
48770,
48771,
48772,
48773,
48774,
48775,
48776,
48777,
48778,
48779,
48780,
48781,
48782,
48783,
48784,
48785,
48786,
48787,
48788,
48789,
48790,
48791,
48792,
48793,
48794,
48795,
48796,
48797,
48798,
48799,
48800,
48801,
48802,
48803,
48804,
48805,
48806,
48807,
48808,
48809,
48810,
48811,
48812,
48813,
48814,
48815,
48816,
48817,
48818,
48819,
48820,
48821,
48822,
48823,
48824,
48825,
48826,
48827,
48828,
48829,
48830,
48831,
48832,
48833,
48834,
48835,
48836,
48837,
48838,
48839,
48840,
48841,
48842,
48843,
48844,
48845,
48846,
48847,
48848,
48849,
48850,
48851,
48852,
48853,
48854,
48855,
48856,
48857,
48858,
48859,
48860,
48861,
48862,
48863,
48864,
48865,
48866,
48867,
48868,
48869,
48870,
48871,
48872,
48873,
48874,
48875,
48876,
48877,
48878,
48879,
48880,
48881,
48882,
48883,
48884,
48885,
48886,
48887,
48888,
48889,
48890,
48891,
48892,
48893,
48894,
48895,
48896,
48897,
48898,
48899,
48900,
48901,
48902,
48903,
48904,
48905,
48906,
48907,
48908,
48909,
48910,
48911,
48912,
48913,
48914,
48915,
48916,
48917,
48918,
48919,
48920,
48921,
48922,
48923,
48924,
48925,
48926,
48927,
48928,
48929,
48930,
48931,
48932,
48933,
48934,
48935,
48936,
48937,
48938,
48939,
48940,
48941,
48942,
48943,
48944,
48945,
48946,
48947,
48948,
48949,
48950,
48951,
48952,
48953,
48954,
48955,
48956,
48957,
48958,
48959,
48960,
48961,
48962,
48963,
48964,
48965,
48966,
48967,
48968,
48969,
48970,
48971,
48972,
48973,
48974,
48975,
48976,
48977,
48978,
48979,
48980,
48981,
48982,
48983,
48984,
48985,
48986,
48987,
48988,
48989,
48990,
48991,
48992,
48993,
48994,
48995,
48997,
48998,
48999,
49000,
49001,
49002,
49003,
49004,
49005,
49006,
49007,
49008,
49009,
49010,
49011,
49012,
49013,
49014,
49015,
49016,
49017,
49018,
49019,
49020,
49021,
49022,
49023,
49024,
49025,
49026,
49027,
49028,
49029,
49030,
49031,
49032,
49033,
49034,
49035,
49036,
49037,
49038,
49039,
49040,
49041,
49042,
49043,
49044,
49045,
49046,
49047,
49048,
49049,
49050,
49051,
49052,
49053,
49054,
49055,
49056,
49057,
49058,
49059,
49060,
49061,
49062,
49063,
49064,
49065,
49066,
49067,
49068,
49069,
49070,
49071,
49072,
49073,
49074,
49075,
49076,
49077,
49078,
49079,
49080,
49081,
49082,
49083,
49084,
49085,
49086,
49087,
49088,
49089,
49090,
49091,
49092,
49093,
49094,
49095,
49096,
49097,
49098,
49099,
49100,
49101,
49102,
49103,
49104,
49105,
49106,
49107,
49108,
49109,
49110,
49111,
49112,
49113,
49114,
49115,
49116,
49117,
49118,
49119,
49120,
49121,
49122,
49123,
49124,
49125,
49126,
49127,
49128,
49129,
49130,
49131,
49132,
49133,
49134,
49135,
49136,
49137,
49138,
49139,
49140,
49141,
49142,
49143,
49144,
49145,
49146,
49147,
49148,
49149,
49150,
49152,
49153,
49154,
49155,
49156,
49157,
49158,
49159,
49160,
49161,
49162,
49163,
49164,
49165,
49166,
49167,
49168,
49169,
49170,
49171,
49172,
49173,
49174,
49175,
49176,
49177,
49178,
49179,
49180,
49181,
49182,
49183,
49184,
49185,
49186,
49187,
49188,
49189,
49190,
49191,
49192,
49193,
49194,
49195,
49196,
49197,
49198,
49199,
49200,
49201,
49202,
49203,
49204,
49205,
49206,
49207,
49208,
49209,
49210,
49212,
49213,
49214,
49215,
49216,
49217,
49218,
49219,
49220,
49221,
49222,
49223,
49224,
49225,
49226,
49227,
49228,
49229,
49230,
49231,
49232,
49233,
49234,
49235,
49236,
49237,
49238,
49239,
49240,
49241,
49242,
49243,
49244,
49245,
49246,
49247,
49248,
49249,
49250,
49251,
49252,
49253,
49254,
49255,
49256,
49257,
49258,
49260,
49261,
49263,
49264,
49266,
49267,
49268,
49269,
49270,
49271,
49272,
49273,
49274,
49275,
49276,
49278,
49279,
49280,
49281,
49282,
49283,
49284,
49285,
49286,
49287,
49288,
49289,
49290,
49291,
49292,
49293,
49294,
49295,
49296,
49297,
49298,
49299,
49300,
49301,
49302,
49303,
49304,
49305,
49306,
49307,
49308,
49309,
49310,
49311,
49312,
49313,
49314,
49315,
49316,
49317,
49318,
49319,
49320,
49321,
49322,
49323,
49324,
49325,
49326,
49327,
49328,
49329,
49330,
49331,
49332,
49333,
49334,
49335,
49337,
49338,
49339,
49340,
49341,
49342,
49343,
49344,
49345,
49346,
49347,
49348,
49349,
49350,
49351,
49352,
49353,
49354,
49355,
49356,
49357,
49358,
49359,
49360,
49361,
49362,
49363,
49364,
49365,
49366,
49367,
49368,
49369,
49371,
49372,
49373,
49375,
49376,
49377,
49378,
49379,
49380,
49382,
49383,
49384,
49386,
49387,
49389,
49390,
49391,
49392,
49393,
49394,
49396,
49397,
49398,
49399,
49400,
49401,
49402,
49403,
49404,
49405,
49406,
49407,
49408,
49409,
49410,
49411,
49412,
49413,
49414,
49415,
49416,
49417,
49418,
49419,
49420,
49421,
49422,
49423,
49424,
49425,
49426,
49427,
49428,
49429,
49430,
49431,
49432,
49433,
49434,
49435,
49436,
49437,
49438,
49439,
49440,
49441,
49442,
49443,
49444,
49445,
49446,
49447,
49448,
49449,
49450,
49451,
49452,
49453,
49454,
49455,
49456,
49457,
49458,
49459,
49460,
49461,
49462,
49463,
49464,
49465,
49466,
49467,
49468,
49469,
49470,
49471,
49472,
49473,
49474,
49475,
49476,
49477,
49478,
49479,
49480,
49481,
49482,
49483,
49484,
49486,
49487,
49488,
49489,
49490,
49491,
49492,
49494,
49496,
49497,
49502,
49504,
49505,
49506,
49507,
49508,
49513,
49516,
49517,
49518,
49519,
49520,
49521,
49522,
49523,
49524,
49525,
49526,
49527,
49530,
49531,
49532,
49533,
49535,
49536,
49537,
49538,
49539,
49540,
49541,
49542,
49543,
49544,
49545,
49546,
49547,
49548,
49550,
49551,
49552,
49553,
49554,
49555,
49556,
49557,
49558,
49559,
49560,
49561,
49562,
49563,
49564,
49565,
49566,
49567,
49568,
49569,
49570,
49571,
49572,
49573,
49574,
49575,
49576,
49577,
49578,
49579,
49580,
49581,
49582,
49583,
49584,
49585,
49586,
49587,
49588,
49589,
49590,
49591,
49592,
49594,
49595,
49596,
49597,
49598,
49599,
49600,
49601,
49602,
49603,
49605,
49606,
49607,
49608,
49609,
49610,
49612,
49613,
49614,
49615,
49616,
49617,
49618,
49619,
49620,
49621,
49622,
49623,
49624,
49625,
49626,
49627,
49628,
49629,
49630,
49635,
49637,
49638,
49639,
49640,
49641,
49642,
49643,
49644,
49646,
49647,
49649,
49650,
49651,
49652,
49653,
49654,
49655,
49656,
49657,
49658,
49659,
49660,
49661,
49662,
49663,
49664,
49666,
49667,
49668,
49669,
49670,
49671,
49677,
49680,
49681,
49682,
49684,
49685,
49686,
49687,
49689,
49690,
49691,
49692,
49693,
49694,
49695,
49696,
49698,
49699,
49700,
49701,
49702,
49703,
49705,
49706,
49707,
49709,
49710,
49711,
49712,
49713,
49714,
49715,
49716,
49721,
49736,
49738,
49740,
49742,
49743,
49744,
49745,
49746,
49747,
49749,
49750,
49752,
49753,
49754,
49755,
49757,
49758,
49759,
49760,
49761,
49762,
49763,
49764,
49765,
49766,
49767,
49768,
49771,
49772,
49773,
49774,
49777,
49778,
49779,
49781,
49782,
49783,
49784,
49786,
49787,
49788,
49789,
49793,
49795,
49796,
49797,
49799,
49801,
49802,
49803,
49804,
49805,
49806,
49808,
49809,
49810,
49811,
49812,
49813,
49814,
49815,
49816,
49819,
49820,
49821,
49822,
49823,
49824,
49825,
49827,
49828,
49829,
49831,
49832,
49833,
49834,
49835,
49836,
49837,
49838,
49839,
49859,
49863,
49865,
49866,
49867,
49868,
49869,
49870,
49871,
49872,
49875,
49879,
49881,
49884,
49885,
49886,
49887,
49888,
49889,
49890,
49891,
49892,
49893,
49894,
49895,
49896,
49897,
49898,
49899,
49900,
49901,
49902,
49903,
49904,
49905,
49906,
49907,
49908,
49909,
49910,
49911,
49912,
49913,
49914,
49915,
49916,
49917,
49918,
49919,
49920,
49921,
49922,
49923,
49924,
49925,
49926,
49927,
49928,
49929,
49930,
49931,
49932,
49933,
49934,
49937,
49938,
49939,
49940,
49941,
49942,
49943,
49944,
49945,
49946,
49948,
49949,
49950,
49951,
49952,
49953,
49954,
49955,
49956,
49957,
49958,
49959,
49960,
49961,
49962,
49963,
49964,
49965,
49966,
49967,
49969,
49970,
49971,
49972,
49974,
49975,
49976,
49977,
49978,
49979,
49980,
49981,
49982,
49983,
49984,
49985,
49986,
49987,
49988,
49989,
49990,
49991,
49992,
49993,
49994,
49995,
49996,
49997,
49998,
49999,
50000,
50001,
50002,
50003,
50004,
50005,
50006,
50007,
50008,
50009,
50010,
50011,
50012,
50013,
50014,
50015,
50016,
50017,
50018,
50019,
50020,
50021,
50022,
50023,
50024,
50025,
50026,
50027,
50028,
50029,
50030,
50031,
50032,
50033,
50034,
50035,
50036,
50037,
50038,
50039,
50040,
50041,
50042,
50043,
50044,
50045,
50046,
50047,
50048,
50049,
50050,
50051,
50052,
50054,
50055,
50056,
50057,
50058,
50060,
50061,
50062,
50063,
50064,
50065,
50066,
50067,
50068,
50069,
50070,
50071,
50072,
50073,
50074,
50075,
50076,
50078,
50080,
50083,
50084,
50085,
50087,
50089,
50090,
50091,
50092,
50093,
50094,
50095,
50096,
50097,
50098,
50099,
50100,
50101,
50102,
50103,
50104,
50105,
50106,
50107,
50108,
50109,
50110,
50111,
50112,
50113,
50114,
50115,
50116,
50117,
50118,
50119,
50120,
50121,
50122,
50123,
50124,
50125,
50126,
50127,
50128,
50129,
50130,
50131,
50132,
50133,
50134,
50135,
50136,
50137,
50138,
50139,
50140,
50141,
50142,
50143,
50144,
50145,
50147,
50148,
50149,
50150,
50152,
50154,
50155,
50156,
50158,
50159,
50160,
50161,
50162,
50163,
50164,
50166,
50171,
50172,
50173,
50175,
50176,
50178,
50179,
50181,
50182,
50183,
50184,
50185,
50186,
50187,
50188,
50189,
50190,
50191,
50192,
50193,
50194,
50195,
50196,
50197,
50198,
50199,
50201,
50202,
50203,
50204,
50205,
50206,
50207,
50208,
50209,
50210,
50211,
50212,
50213,
50214,
50215,
50216,
50217,
50218,
50219,
50220,
50221,
50222,
50223,
50224,
50225,
50226,
50227,
50228,
50229,
50230,
50231,
50232,
50233,
50234,
50235,
50236,
50237,
50238,
50239,
50240,
50241,
50242,
50243,
50244,
50245,
50246,
50247,
50248,
50249,
50250,
50251,
50252,
50253,
50254,
50255,
50256,
50257,
50258,
50259,
50260,
50261,
50262,
50263,
50264,
50265,
50266,
50267,
50268,
50269,
50270,
50271,
50272,
50273,
50274,
50275,
50276,
50277,
50278,
50279,
50280,
50281,
50282,
50283,
50284,
50286,
50287,
50288,
50289,
50290,
50291,
50292,
50293,
50294,
50295,
50296,
50297,
50298,
50299,
50300,
50301,
50302,
50303,
50304,
50305,
50306,
50307,
50308,
50309,
50310,
50311,
50312,
50313,
50315,
50316,
50317,
50318,
50319,
50320,
50322,
50323,
50324,
50325,
50326,
50327,
50328,
50329,
50330,
50331,
50332,
50333,
50334,
50335,
50336,
50337,
50339,
50340,
50341,
50342,
50343,
50344,
50345,
50346,
50347,
50348,
50349,
50350,
50351,
50352,
50353,
50354,
50355,
50356,
50357,
50358,
50359,
50361,
50362,
50363,
50364,
50365,
50366,
50367,
50368,
50369,
50370,
50371,
50372,
50373,
50375,
50376,
50377,
50378,
50379,
50380,
50381,
50382,
50383,
50384,
50385,
50386,
50387,
50388,
50389,
50390,
50391,
50392,
50393,
50394,
50395,
50396,
50397,
50398,
50399,
50400,
50401,
50402,
50403,
50404,
50405,
50406,
50407,
50408,
50409,
50410,
50411,
50412,
50413,
50414,
50415,
50416,
50417,
50418,
50419,
50420,
50421,
50422,
50423,
50424,
50425,
50426,
50427,
50428,
50429,
50430,
50431,
50432,
50433,
50434,
50435,
50436,
50437,
50438,
50439,
50440,
50441,
50442,
50443,
50444,
50445,
50446,
50447,
50448,
50449,
50450,
50451,
50452,
50453,
50454,
50455,
50456,
50457,
50458,
50459,
50460,
50461,
50462,
50463,
50464,
50465,
50466,
50467,
50468,
50469,
50470,
50471,
50472,
50473,
50474,
50475,
50476,
50477,
50478,
50479,
50480,
50481,
50482,
50483,
50484,
50485,
50486,
50487,
50488,
50489,
50490,
50491,
50492,
50493,
50494,
50495,
50496,
50501,
50502,
50503,
50504,
50505,
50506,
50507,
50508,
50513,
50514,
50515,
50517,
50518,
50519,
50520,
50521,
50522,
50523,
50524,
50525,
50526,
50528,
50529,
50530,
50532,
50535,
50536,
50537,
50538,
50545,
50546,
50547,
50548,
50549,
50550,
50551,
50552,
50553,
50554,
50555,
50556,
50557,
50558,
50559,
50560,
50561,
50562,
50563,
50564,
50565,
50566,
50567,
50568,
50569,
50570,
50571,
50572,
50573,
50574,
50575,
50576,
50577,
50578,
50579,
50580,
50581,
50582,
50583,
50584,
50585,
50586,
50587,
50588,
50589,
50590,
50591,
50592,
50593,
50594,
50595,
50596,
50597,
50598,
50599,
50600,
50601,
50602,
50603,
50604,
50605,
50606,
50607,
50608,
50609,
50610,
50611,
50613,
50615,
50616,
50617,
50621,
50624,
50625,
50626,
50627,
50628,
50629,
50630,
50631,
50632,
50633,
50634,
50635,
50636,
50637,
50638,
50639,
50640,
50641,
50642,
50643,
50644,
50645,
50646,
50647,
50648,
50649,
50650,
50651,
50652,
50653,
50654,
50655,
50656,
50657,
50658,
50659,
50660,
50661,
50662,
50663,
50664,
50665,
50666,
50667,
50668,
50669,
50670,
50671,
50672,
50673,
50674,
50675,
50676,
50677,
50678,
50679,
50680,
50681,
50682,
50683,
50684,
50685,
50687,
50688,
50689,
50691,
50692,
50693,
50694,
50695,
50696,
50697,
50698,
50699,
50700,
50701,
50702,
50703,
50704,
50705,
50706,
50707,
50708,
50709,
50710,
50711,
50712,
50713,
50714,
50715,
50716,
50717,
50718,
50719,
50720,
50721,
50722,
50723,
50724,
50725,
50726,
50728,
50729,
50730,
50731,
50732,
50734,
50735,
50736,
50737,
50738,
50739,
50740,
50741,
50742,
50743,
50744,
50745,
50746,
50747,
50748,
50749,
50751,
50752,
50753,
50754,
50755,
50756,
50757,
50758,
50759,
50760,
50761,
50762,
50763,
50764,
50765,
50766,
50767,
50768,
50771,
50772,
50773,
50774,
50775,
50776,
50777,
50778,
50780,
50782,
50784,
50785,
50787,
50788,
50789,
50791,
50792,
50793,
50794,
50795,
50796,
50797,
50799,
50800,
50801,
50802,
50803,
50804,
50805,
50806,
50807,
50808,
50809,
50810,
50811,
50812,
50813,
50814,
50815,
50816,
50817,
50818,
50820,
50821,
50822,
50823,
50824,
50825,
50826,
50827,
50828,
50829,
50830,
50831,
50832,
50833,
50834,
50836,
50837,
50838,
50840,
50842,
50843,
50844,
50845,
50846,
50847,
50848,
50850,
50851,
50852,
50874,
50879,
50882,
50883,
50884,
50885,
50886,
50887,
50888,
50889,
50890,
50894,
50895,
50896,
50897,
50898,
50899,
50900,
50901,
50902,
50903,
50904,
50905,
50906,
50907,
50908,
50909,
50910,
50911,
50912,
50913,
50914,
50915,
50916,
50917,
50918,
50919,
50920,
50921,
50922,
50923,
50924,
50925,
50926,
50927,
50928,
50929,
50930,
50931,
50932,
50933,
50934,
50935,
50936,
50937,
50938,
50939,
50940,
50941,
50942,
50943,
50944,
50945,
50946,
50947,
50948,
50949,
50950,
50951,
50952,
50953,
50954,
50955,
50956,
50957,
50958,
50959,
50960,
50961,
50962,
50963,
50964,
50965,
50966,
50967,
50968,
50969,
50970,
50971,
50972,
50973,
50974,
50975,
50976,
50977,
50978,
50979,
50980,
50981,
50982,
50983,
50984,
50985,
50986,
50987,
50988,
50989,
50990,
50991,
50992,
50995,
50996,
50997,
50998,
50999,
51000,
51001,
51002,
51003,
51004,
51005,
51006,
51007,
51008,
51009,
51010,
51011,
51012,
51013,
51014,
51015,
51016,
51017,
51018,
51019,
51021,
51022,
51023,
51024,
51025,
51026,
51027,
51028,
51029,
51030,
51031,
51032,
51033,
51034,
51035,
51036,
51037,
51038,
51039,
51040,
51041,
51042,
51044,
51045,
51046,
51047,
51048,
51049,
51050,
51051,
51052,
51053,
51054,
51055,
51056,
51057,
51058,
51059,
51060,
51061,
51062,
51063,
51064,
51065,
51066,
51067,
51068,
51069,
51070,
51071,
51072,
51073,
51074,
51075,
51076,
51077,
51078,
51079,
51080,
51081,
51082,
51083,
51084,
51085,
51086,
51087,
51088,
51089,
51090,
51091,
51092,
51093,
51094,
51095,
51096,
51097,
51098,
51099,
51100,
51101,
51102,
51103,
51104,
51105,
51106,
51107,
51108,
51109,
51110,
51111,
51112,
51113,
51114,
51115,
51116,
51117,
51119,
51120,
51121,
51122,
51123,
51124,
51125,
51126,
51127,
51128,
51226
                };




                foreach (var alunoId in arr)
                {
                    var result = await ApiClientFactory.Instance.GetAlunoById(Convert.ToInt32(alunoId));

                    var command = new UsuarioModel.CreateUpdateUsuarioCommand
                    {
                        Email = result.Email,
                        Nome = result.Nome,
                        CpfCnpj = result.Cpf == null ? "000.000.000-00" : result.Cpf ,
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

                            await ApiClientFactory.Instance.UpdateHabilitarAluno(Convert.ToInt32(alunoId), new AlunoModel.UpdateHabilitarAlunoCommand() { AlunoId = Convert.ToInt32(alunoId), AspNetUserId = newUser.Id });
                        }

                        //SendNewUserEmail(newUser, command.Email, command.Nome);
                    }
                }





                //else
                //{
                //    return RedirectToAction(nameof(Index),
                //        new
                //        {
                //            notify = (int)EnumNotify.Error,
                //            message = "Erro ao criar usuário. Favor entrar em contato com o administrador do sistema."
                //        });
                //}

                return RedirectToAction(nameof(Index), new
                {
                    notify = (int)EnumNotify.Success,
                    message = "Aluno habilitado com sucesso."
                });
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
        }

        public async Task<ActionResult> Carteirinha()
        {
            return View();
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

                // Modalidades - alterei para "Conhecimento" conforme o modelo
                AddFullWidthText(document, aluno.Modalidades, leftMargin, currentY, labelWidth + valueWidth, corAzul, false);

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
        private void AddFullWidthText(Document document, string text, float x, float y, float totalWidth, DeviceCmyk color, bool isBold = false)
        {
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

            document.Add(paragraph);
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
        #endregion

        
    }
}
