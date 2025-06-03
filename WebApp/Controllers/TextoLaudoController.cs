using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controle Texto de Laudo 
    /// </summary>
    public class TextoLaudoController : BaseController
    {

        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        public TextoLaudoController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem do Texto de Laudo 
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetTextosLaudosAll();

            return View(new TextoLaudoModel() { TextosLaudos = response });
        }

        /// <summary>
        /// Tela para Inclusão do Texto de Laudo 
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        //[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);
                var tipoLaudos = new SelectList(ApiClientFactory.Instance.GetTiposLaudoAll(), "Id", "Nome");

                return View(new TiposLaudoModel() { ListTiposLaudos = tipoLaudos });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        /// <summary>
        /// Ação de Inclusão do Texto de Laudo 
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao do Texto de Laudo </param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new TextoLaudoModel.CreateUpdateTextoLaudoCommand
                {
                    TipoLaudoId = Convert.ToInt32(collection["ddlTipoLaudo"].ToString()),
                    Sexo = collection["ddlSexo"] == "" ? null : collection["ddlSexo"].ToString(),
                    Idade = collection["idade"] == "" ? null : Convert.ToInt32(collection["idade"].ToString()),
                    Classificacao = collection["classificacao"].ToString(),
                    PontoInicial = collection["pontoInicial"] == "" ? null : Convert.ToDecimal(collection["pontoInicial"].ToString()),
                    PontoFinal = collection["pontoFinal"] == "" ? null : Convert.ToDecimal(collection["pontoFinal"].ToString()),
                    Aviso = collection["aviso"].ToString(),
                    Texto = collection["texto"].ToString(),
                    Quadrante = Convert.ToInt32(collection["quadrante"].ToString())
                };

                await ApiClientFactory.Instance.CreateTextoLaudo(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
            }
        }

        /// <summary>
        /// Ação de Alteração do Texto de Laudo 
        /// </summary>
        /// <param name="collection">coleção de dados para alteração do Texto de Laudo </param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            try
            {
                var command = new TextoLaudoModel.CreateUpdateTextoLaudoCommand
                {
                    Id = Convert.ToInt32(collection["editTextoLaudoId"]),
                    Idade = Convert.ToInt32(collection["idade"].ToString()),
                    Classificacao = collection["classificacao"].ToString(),
                    PontoInicial = Convert.ToDecimal(collection["pontoInicial"].ToString()),
                    PontoFinal = Convert.ToDecimal(collection["pontoFinal"].ToString()),
                    Aviso = collection["aviso"].ToString(),
                    Texto = collection["texto"].ToString(),
                    Quadrante = Convert.ToInt32(collection["quadrante"].ToString())
                };

                await ApiClientFactory.Instance.UpdateTextoLaudo(command.Id, command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
            }
        }

        /// <summary>
        /// Ação de Exclusão do Texto de Laudo 
        /// </summary>
        /// <param name="id">identificador do Texto de Laudo </param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteTextoLaudo(id);
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
        /// Busca Texto de Laudo  por Id
        /// </summary>
        /// <param name="id">Identificador do Texto de Laudo</param>
        /// <returns>Retorna a Texto de Laudo</returns>
        public Task<TextoLaudoDto> GetTextoLaudoById(int id)
        {
            var result = ApiClientFactory.Instance.GetTextoLaudoById(id);

            return Task.FromResult(result);
        }
    }

    #endregion


}
