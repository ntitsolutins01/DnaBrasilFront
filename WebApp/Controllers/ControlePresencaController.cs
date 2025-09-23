using System.Globalization;
using System.Security.Claims;
using log4net;
using Microsoft.AspNetCore.Identity;
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
using Claim = WebApp.Identity.Claim;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controle de Presença
    /// </summary>
    //[Authorize(Policy = ModuloAccess.ControlePresenca)]
    public class ControlePresencaController : BaseController
    {
        private readonly ILog _logger;

        #region Constructor

        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de url da api</param>
        /// <param name="userManager">gerenciador de identidade de usuários</param>
        /// <param name="logger">Log de mensagens da aplicação</param>
        public ControlePresencaController(IOptions<UrlSettings> appSettings, UserManager<IdentityUser> userManager, ILog logger)
        {
            _userManager = userManager;
            _logger = logger;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem de Controle de Presença 
        /// </summary>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>Returs true false</returns>
        [ClaimsAuthorize(ClaimType.ControlePresenca, Claim.Consultar)]
        public async Task<ActionResult> Index(int? crud, int? notify, string message = null)
        {
            try
            {
                _logger.Info($"Usuario Logado em ControlePresenca.Index User.Identity.Name : {User.Identity.Name}");

                var usuario = User.Identity.Name;

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                //Busca usuario por AspNetUserId
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _logger.Info($"Busca Usuario por AspNetUserId: {userId}");

                if (userId == null)
                {
                    _logger.Warn($"AspNetUserId não encontrado para o email: {User.Identity.Name}");
                    throw new Exception($"AspNetUserId não encontrado para o email: {User.Identity.Name}");
                }

                var usu = await ApiClientFactory.Instance.GetUsuarioByAspNetUserId(userId);

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

                    localidades = new SelectList(resultLocalidades, "Id", "Nome", usu.LocalidadeId);
                }

                SelectList alunos = null;

                SelectList modalidades = null;

                SelectList profissionais = null;

                if (usu.LocalidadeId != null)
                {
                    var resultAlunos = ApiClientFactory.Instance.GetAlunosByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));

                    alunos = new SelectList(resultAlunos, "Id", "Nome");

                    //var listAtividades = await ApiClientFactory.Instance.GetAtividadeByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));

                    // modalidades = new SelectList(listAtividades.Select(s => new { Id = s.ModalidadeId, Nome = s.NomeModalidade }).ToList(), "Id", "Nome");

                    //var profissional = await ApiClientFactory.Instance.GetPro

                    profissionais = new SelectList(ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId)), "Id", "Nome");
                }

                //var listModalidades = new SelectList(ApiClientFactory.Instance.GetModalidadeAll(), "Id", "Nome");
                //var profissionais = 
                //    ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId));

                var searchFilter = new ControlesPresencasFilterDto()
                {
                    UsuarioEmail = usuario,
                    //FomentoId = collection["ddlFomento"].ToString(),
                    //Estado = collection["ddlEstado"].ToString(),
                    //MunicipioId = collection["ddlMunicipio"].ToString(),
                    //LocalidadeId = collection["ddlLocalidade"].ToString() == "" ? usu.LocalidadeId : collection["ddlLocalidade"].ToString(),

                    PageNumber = 1,
#if DEBUG
                    PageSize = 10
#else
                    PageSize = 1000
#endif
                };

                var response = await ApiClientFactory.Instance.GetControlesPresencasByFilter(searchFilter);

                var model = new ControlePresencaModel()
                {
                    ListFomentos = fomentos,
                    ListEstados = estados,
                    ListMunicipios = municipios!,
                    ListLocalidades = localidades!,
                    ListAlunos = alunos,
                    ControlesPresencas = response.ControlesPresencas,
                    //ListAtividadesModalidades = modalidades,
                    ListProfissionais = profissionais!,
                    IdPerfil = usu.Perfil.Id

                };
                return View(model);

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        /// <summary>
        /// Listagem de Controle de Presença 
        /// </summary>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
        /// <param name="collection">Parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>Returs true false</returns>
        [ClaimsAuthorize(ClaimType.ControlePresenca, Claim.Consultar)]
        [HttpPost]
        public async Task<ActionResult> Index(int? crud, int? notify, IFormCollection collection, string message = null)
        {
            try
            {

                var data = DateTime.ParseExact(collection["data"].ToString(), "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("pt-BR"));

                var diasSemana = collection["diaSemana"].ToString().Split("-");

                VerificaDiaSemana(data, diasSemana);

                _logger.Info($"Usuario Logado em ControlePresenca.Index User.Identity.Name : {User.Identity.Name}");

                var usuario = User.Identity.Name;

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                //Busca usuario por AspNetUserId
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _logger.Info($"Busca Usuario por AspNetUserId: {userId}");

                if (userId == null)
                {
                    _logger.Warn($"AspNetUserId não encontrado para o email: {User.Identity.Name}");
                    throw new Exception($"AspNetUserId não encontrado para o email: {User.Identity.Name}");
                }

                var usu = await ApiClientFactory.Instance.GetUsuarioByAspNetUserId(userId);

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

                    localidades = new SelectList(resultLocalidades, "Id", "Nome", usu.LocalidadeId);
                }

                SelectList profissionais = null;
                SelectList modalidades = null;
                SelectList turmas = null;
                List<AtividadeAlunosDto> listAtividadeAlunos = null;


                if (usu.LocalidadeId != null)
                {
                    var profissionalId = collection["ddlProfissional"].ToString();

                    profissionais =
                        new SelectList(
                            ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(usu.LocalidadeId)),
                            "Id", "Nome", profissionalId);

                    var modalidadeId = collection["ddlModalidade"].ToString();

                    modalidades =
                        new SelectList(
                            ApiClientFactory.Instance.GetModalidadesByProfissionalId(Convert.ToInt32(profissionalId)),
                            "Id", "Nome", modalidadeId);

                    var turmaId = collection["ddlTurma"].ToString();

                    turmas = new SelectList(
                        ApiClientFactory.Instance.GetTurmasByModalidadeIdProfissionalId(Convert.ToInt32(modalidadeId),
                            Convert.ToInt32(profissionalId)), "Id", "TurmaHora", turmaId);

                    listAtividadeAlunos = ApiClientFactory.Instance.GetAtividadeAlunosByAtividadeId(Convert.ToInt32(turmaId));
                }

                var listFalta = (from item in collection where item.Key.Contains("falta") select item.Key).Select(v => (string)v).ToList();

                var dic = new Dictionary<string, string>();

                var listJustificativa = collection.Where(item => item.Key.Contains("justificativa")).ToList();

                foreach (var falta in listFalta)
                {
                    var x = listJustificativa.FirstOrDefault(t => t.Key == falta.Replace("falta", "justificativa"));
                }


                if (listFalta.Any())
                {

                    //command.VocacionalId = (int)await ApiClientFactory.Instance.CreateVocacional(
                    //    new VocacionalModel.CreateUpdateVocacionalCommand()
                    //    {
                    //        Respostas = string.Join(",", listVocacional),
                    //        ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString()),
                    //        AlunoId = Convert.ToInt32(collection["ddlAluno"].ToString()),
                    //        StatusVocacional = listVocacional.Count == totalRespVocacional ? "F" : "A"
                    //    });
                }

                var model = new ControlePresencaModel()
                {
                    ListFomentos = fomentos,
                    ListEstados = estados,
                    ListMunicipios = municipios!,
                    ListLocalidades = localidades!,
                    ListAtividadesModalidades = modalidades,
                    ListProfissionais = profissionais!,
                    ListTurmas = turmas,
                    Data = collection["data"].ToString(),
                    Estrutura = collection["estrutura"].ToString(),
                    DiasSemana = collection["diaSemana"].ToString(),
                    Categoria = collection["categoria"].ToString(),
                    ListAtividadeAlunos = listAtividadeAlunos
                };

                return View(model);

            }
            catch (Exception e)
            {
                _logger.Error($"Exception [HttpPost]ControlePresenca.Index : {e.Message} - {e.StackTrace} ");
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        /// <summary>
        /// Tela para Inclusão de Controle de Presença
        /// </summary>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>Returns true love </returns>
        [ClaimsAuthorize(ClaimType.ControlePresenca, Claim.Incluir)]
        public async Task<ActionResult> Create(int? crud, int? notify, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (User.Identity == null) return Redirect("/Identity/Account/Login");
                var usuario = User.Identity.Name;

                if (usuario == null) return Redirect("/Identity/Account/Login");
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

                SelectList alunos = null;

                if (usu.LocalidadeId == null)
                    return View(new ControlePresencaModel()
                    {
                        ListEstados = estados,
                        ListMunicipios = municipios!,
                        ListLocalidades = localidades!,
                        ListAlunos = alunos,
                    });
                var resultAlunos = ApiClientFactory.Instance.GetAlunosByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));

                alunos = new SelectList(resultAlunos, "Id", "Nome");

                return View(new ControlePresencaModel()
                {
                    ListEstados = estados,
                    ListMunicipios = municipios!,
                    ListLocalidades = localidades!,
                    ListAlunos = alunos,
                });

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        /// <summary>
        /// Ação de Inclusão de Controle de Presença
        /// </summary>
        /// <param name="collection">Coleção de dados para Inclusao de Controle de Presença</param>
        /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
        [HttpPost]
        [ClaimsAuthorize(ClaimType.ControlePresenca, Claim.Incluir)]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new ControlePresencaModel.CreateUpdateControlePresencaCommand
                {
                    MunicipioId = collection["ddlMunicipio"] == "" ? null : Convert.ToInt32(collection["ddlMunicipio"].ToString()).ToString(),
                    LocalidadeId = collection["ddlLocalidade"] == "" ? null : Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                    Controle = collection["controle"].ToString(),
                    Justificativa = collection["justificativa"].ToString(),
                    AlunoId = collection["ddlAluno"] == "" ? null : Convert.ToInt32(collection["ddlAluno"].ToString()).ToString(),
                };

                var possuiPresensa = ApiClientFactory.Instance.GetControlePresencaByAlunoId(Convert.ToInt32(command.AlunoId))
                    .Where(x => x.ControlesPresencas.FirstOrDefault().Data == DateTime.Now.ToString("dd/MM/yyyy") && x.ControlesPresencas.FirstOrDefault()?.EventoId == null);

                if (possuiPresensa.Any())
                {
                    return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Warning, message = "Já existe presença cadastrada para este aluno no dia de hoje." });
                }
                await ApiClientFactory.Instance.CreateControlePresenca(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
            }
        }

        /// <summary>
        /// Ação de Alteração de Controle de Presença 
        /// </summary>
        /// <param name="collection">Coleção de dados para alteração de Controle de Presença</param>
        /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
        [ClaimsAuthorize(ClaimType.ControlePresenca, Claim.Alterar)]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            try
            {
                var command = new ControlePresencaModel.CreateUpdateControlePresencaCommand
                {
                    Id = Convert.ToInt32(collection["editControlePresencaId"]),
                    Controle = collection["controle"].ToString(),
                    Justificativa = collection["convidado"].ToString()
                };

                await ApiClientFactory.Instance.UpdateControlePresenca(command.Id, command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
            }
        }

        /// <summary>
        /// Ação de Exclusão de Controle de Presença 
        /// </summary>
        /// <param name="id">Identificador do Controle de Categoria</param>
        /// <returns>Retorna mensagem de exclusão através do parametro crud</returns>
        [ClaimsAuthorize(ClaimType.ControlePresenca, Claim.Excluir)]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteControlePresenca(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Busca Controle de Presença  por Id
        /// </summary>
        /// <param name="id">Identificador de Controle de Presença</param>
        /// <returns>Retorna a Categoria</returns>
        [ClaimsAuthorize(ClaimType.ControlePresenca, Claim.Consultar)]
        public Task<ControlePresencaDto> GetControlePresencaById(int id)
        {
            var result = ApiClientFactory.Instance.GetControlePresencaById(id);

            return Task.FromResult(result);
        }

        /// <summary>
        /// Busca uma lista de modalidades pelo id do profissional
        /// </summary>
        /// <param name="id">Id do profissional a ser buscado</param>
        /// <returns>Retorna uma lista json de Modalidades </returns>
        public Task<JsonResult> GetModalidadesByProfissionalId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Profissional não informado.");

                var resultLocal = ApiClientFactory.Instance.GetModalidadesByProfissionalId(Convert.ToInt32(id));

                return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Nome")));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }

        #endregion

        #region Custom Actions

        /// <summary>
        /// Tela para impressao de relatório de frequência individual
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        [ClaimsAuthorize(ClaimType.ControlePresenca, Claim.Incluir)]
        public ActionResult ImprimirFrequencia(int mes)
        {
            ViewBag.Mes = mes;

            return View("ImprimirFrequencia");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Método para verificar se o dia da data é um dia valido da semana de atividade
        /// </summary>
        /// <param name="data">Data selecionado</param>
        /// <param name="diasSemana">Dias da semana da atividade</param>
        /// <exception cref="Exception">Retorna Exceção de Data inválida</exception>
        private static void VerificaDiaSemana(DateTime data, string[] diasSemana)
        {
            switch (data.DayOfWeek)
            {
                case DayOfWeek.Monday when !diasSemana.Contains("SEG"):
                    throw new Exception($"Data Inválida.");
                case DayOfWeek.Tuesday when !diasSemana.Contains("TER"):
                    throw new Exception($"Data Inválida.");
                case DayOfWeek.Wednesday when !diasSemana.Contains("QUA"):
                    throw new Exception($"Data Inválida.");
                case DayOfWeek.Thursday when !diasSemana.Contains("QUI"):
                    throw new Exception($"Data Inválida.");
                case DayOfWeek.Friday when !diasSemana.Contains("SEX"):
                    throw new Exception($"Data Inválida.");
                case DayOfWeek.Saturday when !diasSemana.Contains("SAB"):
                    throw new Exception($"Data Inválida.");
                case DayOfWeek.Sunday when !diasSemana.Contains("DOM"):
                    throw new Exception($"Data Inválida.");
            }


        }

        #endregion
    }
}
