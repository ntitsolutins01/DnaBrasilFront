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
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using QRCoder;
using Claim = WebApp.Identity.Claim;
using log4net;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace WebApp.Controllers
{
    [Authorize(Policy = ModuloAccess.Aluno)]
    public class AlunoController : BaseController
    {
        #region Constructor

        private readonly IOptions<UrlSettings> _appSettings;
        private readonly IHostingEnvironment _host;
        private readonly ILog _logger;

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="app">configurações de urls do sistema</param>
        /// <param name="host">informações da aplicação em execução</param>
        public AlunoController(IOptions<UrlSettings> app,
            IHostingEnvironment host,
            ILog logger)
        {
            _appSettings = app;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
            _host = host;
            _logger = logger;
        }
        #endregion

        #region Main Methods
        /// <summary>
        /// Listagem de Alunos
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="collection">lista de filtros selecionados para pesquisa de alunos</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
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

                var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome", searchFilter.FomentoId);
                var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll().Where(x => x.Status), "Id", "Nome", searchFilter.DeficienciaId);
                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", usu.Uf);


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
                    SearchFilter = searchFilter

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
        /// Tela para inclusão de aluno
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
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
        /// Tela para alteração de aluno
        /// </summary>
        /// <param name="id">identificador do aluno</param>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
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
        /// Ação de inclusao do aluno
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao de aluno</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
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
        /// Ação de alteração do aluno
        /// </summary>
        /// <param name="id">identificador do aluno</param>
        /// <param name="collection">coleção de dados para alteração de aluno</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
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
        /// Ação de upload de foto do aluno
        /// </summary>
        /// <param name="collection">arquivo de upload realizado</param>
        /// <returns>retorna mensagem de upload realizado através do parametro notfy e message</returns>
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
        /// Ação de exclusão do aluno
        /// </summary>
        /// <param name="id">Id de exclusão de Aluno</param>
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
        /// Tela para impressao de carteirinha
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


        [ClaimsAuthorize(ClaimType.Aluno, Claim.Incluir)]
        public async Task<ActionResult> ImprimirCarteirinhasLote(string ids, string fomentoId = null, string estadoId = null,
            string municipioId = null, string localidadeId = null, string deficienciaId = null, string etniaId = null, string sexoId = null)
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
                        DeficienciaId = deficienciaId,
                        Etnia = etniaId,
                        Sexo = sexoId
                    };

                    _logger.Info($"Filtros aplicados: Sexo={searchFilter.Sexo}, Fomento={searchFilter.FomentoId}");
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
                        }
                    }).ToList(), // Convertendo para List<AlunoIndexDto>
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
        /// Busca de alunos por localidade
        /// </summary>
        /// <param name="id">identificador da localidade</param>
        /// <returns>retorna a lista de alunos</returns>
        [ClaimsAuthorize(ClaimType.Aluno, Claim.Consultar)]
        public async Task<JsonResult> GetAlunosByLocalidade(string id)
        {
            try
            {
                _logger.Info($"Busca de alunos por localidade GetAlunosByLocalidade: {id}");

                if (string.IsNullOrEmpty(id)) throw new Exception("Localidade não informada.");
                var resultLocal = await ApiClientFactory.Instance.GetNomeAlunosAll(id);

                return new JsonResult(new SelectList(resultLocal, "Id", "Nome"));

            }
            catch (Exception ex)
            {
                _logger.Error($"Busca de alunos por localidade GetAlunosByLocalidade: {ex.StackTrace}");
                return new JsonResult(ex.StackTrace);
            }
        }

        /// <summary>
        /// Busca de idade do Aluno por Id
        /// </summary>
        /// <param name="id">identificador do aluno</param>
        /// <returns>retorna a idade do aluno</returns>
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
        /// Busca de aluno por id
        /// </summary>
        /// <param name="id">identificador do aluno</param>
        /// <returns>retorna o aluno</returns>
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
        private static byte[]? GeraQrCode(long alunoId)
        {
            var text = $"http://dnadobrasil.org.br/Identity/Account/ControlePresenca?alunoId={alunoId}";

            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(60);

            return BitmapToBytes(QrBitmap);
        }

        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

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
