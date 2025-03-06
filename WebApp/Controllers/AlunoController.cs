using System.Drawing;
using Microsoft.AspNetCore.Authorization;
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
using QRCoder;
using Claim = WebApp.Identity.Claim;
using log4net;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controle de Aluno
    /// </summary>
    [Authorize(Policy = ModuloAccess.Aluno)]
    public class AlunoController : BaseController
    {
        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;
        private readonly IWebHostEnvironment _host;
        private readonly ILog _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        /// <param name="host">informações da aplicação em execução</param>
        public AlunoController(IOptions<UrlSettings> appSettings,
            IWebHostEnvironment host,
            ILog logger)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
            _host = host;
            _logger = logger;
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
                var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

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
                    Matricula = collection["matricula"].ToString()
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
                    var resultLocalidades = ApiClientFactory.Instance.GetLocalidadeByMunicipio(usu.MunicipioId.ToString());

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
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome");
            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
            var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll().Where(x => x.Status), "Id", "Nome");
            var modalidades = new SelectList(ApiClientFactory.Instance.GetModalidadeAll(), "Id", "Nome");

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
                ListDeficiencias = deficiencias,
                ListModalidades = modalidades,
                ListEtnias = etnias,
                ListFomentos = fomentos
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
                var municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(aluno.Estado!), "Id", "Nome", aluno.MunicipioId);
                var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeByMunicipio(aluno.MunicipioId.ToString()), "Id", "Nome", aluno.LocalidadeId);
                var profissionais = new SelectList(ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(aluno.LocalidadeId)), "Id", "Nome", aluno.ProfissionalId);
                var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome", aluno.FomentoId);
                var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll(), "Id", "Nome", aluno.DeficienciaId); 
                var listModalidades = new SelectList(ApiClientFactory.Instance.GetModalidadeAll(), "Id", "Nome", aluno.ModalidadesIds);

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
                    ListModalidades = listModalidades

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
                    AutorizacaoConsentimentoAssentimento = collection["agreeterms"].ToString() != ""

                };

                foreach (var file in collection.Files)
                {
                    if (file.Length <= 0) continue;

                    command.NomeFoto = Path.GetFileName(collection.Files[0].FileName);

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

                return RedirectToAction(nameof(Index), new { id = alunoId, crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                _logger.Error($"Ação de inclusao do aluno - Aluno.CreateDados: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Este cpf ou email já pertencem a outro aluno." });
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
                };

                foreach (var file in collection.Files)
                {
                    if (file.Length <= 0) continue;

                    command.NomeFoto = Path.GetFileName(collection.Files[0].FileName);

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

                    command.NomeFoto = Path.GetFileName(collection.Files[0].FileName);

                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var byteIMage = ms.ToArray();
                    command.ByteImage = byteIMage;
                }

                await ApiClientFactory.Instance.UpdateAlunoFoto(command.Id, command);

                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Success, mesage = "Upload realizado com sucesso." });
            }
            catch (Exception e)
            {
                _logger.Error($"Ação de upload de foto do aluno - Aluno.Upload: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, mesage = e.Message });
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
        /// Tela para Impressao de Carteirinha
        /// </summary>
        /// <param name="id">Id do Aluno</param>
        /// <param name="fomentoId">Id do Fomento</param>
        /// <returns>Retorna o objeto AlunoModel</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        public async Task<ActionResult> ImprimirCarteirinha(int id, int fomentoId)
        {
            _logger.Info($"Tela para impressao de carteirinha - Aluno.ImprimirCarteirinha");

            var aluno = await ApiClientFactory.Instance.GetAlunoById(id);
            var modeloCarteirinha = await ApiClientFactory.Instance.GetModeloCarteirinhaByFomentoId(fomentoId);

            return View(new AlunoModel()
            {
                Aluno = aluno,
                ModeloCarteirinha = modeloCarteirinha
            });
        }


        /// <summary>
        /// Acao de Imprimir Carteirinha por Lote
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
        /// <returns>Retorna impresao de carteirinha</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        public async Task<ActionResult> ImprimirCarteirinhasLote(string ids, string fomentoId = null, string estadoId = null,
            string municipioId = null, string localidadeId = null, string profissionalId = null, string deficienciaId = null,
            string etniaId = null, string sexoId = null)
        {
            try
            {
                _logger.Info($"Tela para impressao de carteirinha - Aluno.ImprimirCarteirinhasLote");

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

                    // Esse trecho comentado está bugando algo, deixa comentado por enquanto
                    /*if (!string.IsNullOrEmpty(sexoId))
                    {
                        result.Alunos = result.Alunos.Where(a =>
                            !string.IsNullOrEmpty(a.Sexo) &&
                            a.Sexo.Equals(sexoId, StringComparison.OrdinalIgnoreCase)
                        ).ToList();
                    }*/

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
                _logger.Error($"Ação de imprimir carteirinha em lote - Aluno.ImprimirCarteirinhasLote: {e.StackTrace}");
                _logger.Error($"Erro ao aplicar filtros: {e.Message}");

                return RedirectToAction(nameof(Index), new
                {
                    notify = (int)EnumNotify.Error,
                    message = "Erro ao gerar impressão em lote: " + e.Message
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
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Alterar)]
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
                var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeByMunicipio(aluno.MunicipioId.ToString()), "Id", "Nome", aluno.LocalidadeId);
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
                    ModalidadesIds = collection["ddlModalidades"].ToString()

                };

                foreach (var file in collection.Files)
                {
                    if (file.Length <= 0) continue;

                    command.NomeFoto = Path.GetFileName(collection.Files[0].FileName);

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

                return RedirectToAction(nameof(Index), new { id = alunoId, crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                _logger.Error($"Ação de atualização do profile do aluno - Aluno.Profile: {e.StackTrace}");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
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
        #endregion
    }
}
