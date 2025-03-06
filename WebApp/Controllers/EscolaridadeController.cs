using Microsoft.AspNetCore.Mvc;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controle de Escolaridade
    /// </summary>
	public class EscolaridadeController : BaseController
    {

        #region Main Methods

        /// <summary>
        /// Listagem de Escolaridade
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>returns true false</returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetEscolaridadeAll();

            return View(new EscolaridadeModel() { Escolaridades = response });
        }

        /// <summary>
        /// Tela para Inclusão de Escolaridade
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>returns a true false</returns>
        //[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            return View();
        }

        /// <summary>
        /// Ação de Inclusão de Escolaridade
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao de Escolaridade</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new EscolaridadeModel.CreateUpdateEscolaridadeCommand
                {

                    Nome = collection["nome"].ToString(),
                    Descricao = collection["descricao"].ToString()

                };

                await ApiClientFactory.Instance.CreateEscolaridade(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        ///  Ação de Alteração de Escolaridade
        /// </summary>
        /// <param name="id">Identificador de Escolaridade</param>
        /// <param name="collection">coleção de dados para alteração de Escolaridade</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public Task<ActionResult> Edit(string id, IFormCollection collection)
        {
            var command = new EscolaridadeModel.CreateUpdateEscolaridadeCommand
            {
                Id = Convert.ToInt32(id),
                Nome = collection["nome"].ToString(),
                Descricao = collection["descricao"].ToString()
            };

            //await ApiClientFactory.Instance.UpdateEscolaridade(command);

            return Task.FromResult<ActionResult>(RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated }));
        }

        /// <summary>
        ///  Ação de exclusão do Escolaridade
        /// </summary>
        /// <param name="id">identificador do Escolaridade</param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(string id)
        {
            try
            {
                //ApiClientFactory.Instance.DeleteEscolaridade(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }

    #endregion

}
