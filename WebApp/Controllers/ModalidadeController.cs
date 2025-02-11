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

namespace WebApp.Controllers
{
    /// <summary>
    /// Controle de Modalidade
    /// </summary>
    public class ModalidadeController : BaseController
    {

        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        public ModalidadeController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem de Modalidade
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetModalidadeAll();
            var linhasAcoes = new SelectList(ApiClientFactory.Instance.GetLinhasAcoesAll(), "Id", "Nome");

            return View(new ModalidadeModel() { Modalidades = response, ListLinhasAcoes = linhasAcoes });
        }

        /// <summary>
        /// Tela para Inclusão de Modalidade
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        //[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var linhasAcoes = new SelectList(ApiClientFactory.Instance.GetLinhasAcoesAll(), "Id", "Nome");

            return View(new ModalidadeModel { ListLinhasAcoes = linhasAcoes });
        }

        /// <summary>
        /// Ação de Inclusão de Modalidade
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao de Modalidade</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new ModalidadeModel.CreateUpdateModalidadeCommand
                {
                    Nome = collection["nome"].ToString(),
                    LinhaAcaoId = Convert.ToInt32(collection["ddlLinhaAcao"].ToString()),
                    Vo2MaxIni = Convert.ToInt32(collection["vo2MaxIni"].ToString()),
                    Vo2MaxFim = Convert.ToInt32(collection["vo2MaxFim"].ToString()),
                    VinteMetrosIni = Convert.ToInt32(collection["vinteMetrosIni"].ToString()),
                    VinteMetrosFim = Convert.ToInt32(collection["vinteMetrosFim"].ToString()),
                    ShutlleRunIni = Convert.ToInt32(collection["shutlleRunIni"].ToString()),
                    ShutlleRunFim = Convert.ToInt32(collection["shutlleRunFim"].ToString()),
                    FlexibilidadeIni = Convert.ToInt32(collection["flexibilidadeIni"].ToString()),
                    FlexibilidadeFim = Convert.ToInt32(collection["flexibilidadeFim"].ToString()),
                    PreensaoManualIni = Convert.ToInt32(collection["preensaoManualIni"].ToString()),
                    PreensaoManualFim = Convert.ToInt32(collection["preensaoManualFim"].ToString()),
                    AbdominalPranchaIni = Convert.ToInt32(collection["abdominalPranchaIni"].ToString()),
                    AbdominalPranchaFim = Convert.ToInt32(collection["abdominalPranchaFim"].ToString()),
                    ImpulsaoIni = Convert.ToInt32(collection["impulsaoIni"].ToString()),
                    ImpulsaoFim = Convert.ToInt32(collection["impulsaoFim"].ToString()),
                    EnvergaduraIni = Convert.ToInt32(collection["envergaduraIni"].ToString()),
                    EnvergaduraFim = Convert.ToInt32(collection["envergaduraFim"].ToString()),
                    PesoIni = Convert.ToInt32(collection["pesoIni"].ToString()),
                    PesoFim = Convert.ToInt32(collection["pesoFim"].ToString()),
                    AlturaIni = Convert.ToInt32(collection["alturaIni"].ToString()),
                    AlturaFim = Convert.ToInt32(collection["alturaFim"].ToString()),
                    Status = true
                };

                foreach (var file in collection.Files)
                {
                    if (file.Length <= 0) continue;

                    command.NomeByteImage = Path.GetFileName(collection.Files[0].FileName);

                    using (var ms = new MemoryStream())
                    {
                        file.CopyToAsync(ms);
                        var byteIMage = ms.ToArray();
                        command.ByteImage = byteIMage;
                    }
                }


                await ApiClientFactory.Instance.CreateModalidade(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Ação de Alteração de Modalidade
        /// </summary>
        /// <param name="collection">coleção de dados para alteração de Modalidade</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            var command = new ModalidadeModel.CreateUpdateModalidadeCommand
            {
                Id = Convert.ToInt32(collection["editModalidadeId"]),
                Nome = collection["nome"].ToString(),
                Vo2MaxIni = Convert.ToInt32(collection["vo2MaxIni"].ToString()),
                LinhaAcaoId = Convert.ToInt32(collection["ddlLinhaAcao"].ToString()),
                Vo2MaxFim = Convert.ToInt32(collection["vo2MaxFim"].ToString()),
                VinteMetrosIni = Convert.ToInt32(collection["vinteMetrosIni"].ToString()),
                VinteMetrosFim = Convert.ToInt32(collection["vinteMetrosFim"].ToString()),
                ShutlleRunIni = Convert.ToInt32(collection["shutlleRunIni"].ToString()),
                ShutlleRunFim = Convert.ToInt32(collection["shutlleRunFim"].ToString()),
                FlexibilidadeIni = Convert.ToInt32(collection["flexibilidadeIni"].ToString()),
                FlexibilidadeFim = Convert.ToInt32(collection["flexibilidadeFim"].ToString()),
                PreensaoManualIni = Convert.ToInt32(collection["preensaoManualIni"].ToString()),
                PreensaoManualFim = Convert.ToInt32(collection["preensaoManualFim"].ToString()),
                AbdominalPranchaIni = Convert.ToInt32(collection["abdominalPranchaIni"].ToString()),
                AbdominalPranchaFim = Convert.ToInt32(collection["abdominalPranchaFim"].ToString()),
                ImpulsaoIni = Convert.ToInt32(collection["impulsaoIni"].ToString()),
                ImpulsaoFim = Convert.ToInt32(collection["impulsaoFim"].ToString()),
                EnvergaduraIni = Convert.ToInt32(collection["envergaduraIni"].ToString()),
                EnvergaduraFim = Convert.ToInt32(collection["envergaduraFim"].ToString()),
                PesoIni = Convert.ToInt32(collection["pesoIni"].ToString()),
                PesoFim = Convert.ToInt32(collection["pesoFim"].ToString()),
                AlturaIni = Convert.ToInt32(collection["alturaIni"].ToString()),
                AlturaFim = Convert.ToInt32(collection["alturaFim"].ToString()),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            foreach (var file in collection.Files)
            {
                if (file.Length <= 0) continue;

                command.NomeByteImage = Path.GetFileName(collection.Files[0].FileName);

                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    command.ByteImage = ms.ToArray();
                }
            }

            if (!collection.Files.Any())
            {
                var currentModalidade = ApiClientFactory.Instance.GetModalidadeById(command.Id);
                command.ByteImage = currentModalidade.ByteImage;
            }

            await ApiClientFactory.Instance.UpdateModalidade(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        /// <summary>
        /// Ação de Exclusão do Modalidade
        /// </summary>
        /// <param name="id">identificador do Modalidade</param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteModalidade(id);
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
        /// Busca  Modalidade por Id
        /// </summary>
        /// <param name="id">Identificador de Modalidade</param>
        /// <returns>Retorna a Modalidade</returns>
        /// <exception cref="Exception"></exception>
        public Task<ModalidadeDto> GetModalidadeById(int id)
        {
            try
            {
                var result = ApiClientFactory.Instance.GetModalidadeById(id);
                var linhasAcoes = new SelectList(ApiClientFactory.Instance.GetLinhasAcoesAll(), "Id", "Nome", result.LinhaAcaoId);
                result.ListLinhasAcoes = linhasAcoes;

                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Busca Modalidade por id de Linha de Acao
        /// </summary>
        /// <param name="id">Identificador de Modalidade</param>
        /// <returns>Retorna a Modalidade</returns>
        [ClaimsAuthorize(ClaimType.Modalidade, Claim.Consultar)]
        public Task<JsonResult> GetModalidadesByLinhaAcaoId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Linha de ação não informada.");
                var resultLocal = ApiClientFactory.Instance.GetModalidadesByLinhaAcaoId(Convert.ToInt32(id));

                return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Nome")));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }
    }

    #endregion


}
