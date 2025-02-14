using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Collections.Specialized;
using System.Security.Claims;
using WebApp.Configuration;
using WebApp.Data;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controle de Perfil
    /// </summary>
    public class PerfilController : BaseController
    {

        #region Parametros

        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<UrlSettings> _appSettings;


        #endregion

        #region Constructor

        public PerfilController(ApplicationDbContext db,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager, IOptions<UrlSettings> appSettings)
        {
            _db = db;
            _roleManager = roleManager;
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem de Perfil
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        //[ClaimsAuthorize("Perfil", "Consultar")]
        public ActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var response = ApiClientFactory.Instance.GetPerfilAll();
            return View(new PerfilModel { Perfis = response });
        }

        /// <summary>
        /// Tela para Inclusão de Perfil
        /// </summary>
        /// <returns>returns true false</returns>
        public ActionResult Create()
        {
            var responseModulos = ApiClientFactory.Instance.GetModulosAll();

            var model = new PerfilModel { Modulos = responseModulos };
            return View(model);
        }

        /// <summary>
        /// Ação de Inclusão de Perfil
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao de Perfil</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Perfil", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                ListDictionary list = new ListDictionary();
                var responseModulos = ApiClientFactory.Instance.GetModulosAll();

                foreach (var modulo in responseModulos)
                {
                    var funcionalidades = modulo.Funcionalidades.FirstOrDefault().Nome.Split(',');
                    var listValue = funcionalidades
                        .Where(func => collection[modulo.Nome + func].ToString() == "on").ToList();
                    list.Add(modulo.Nome, string.Join(",", listValue));
                }

                var command = new PerfilModel.CreateUpdateCommand()
                {
                    Nome = collection["nome"].ToString(),
                    Descricao = collection["descricao"].ToString(),
                    Claims = list
                };

                var adminRole = await _roleManager.FindByNameAsync(command.Nome);

                if (adminRole != null) return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
                adminRole = new IdentityRole(command.Nome);
                await _roleManager.CreateAsync(adminRole);
                foreach (var addClaim in from DictionaryEntry claim in command.Claims
                                         select new Claim(claim.Key.ToString()!, claim.Value!.ToString()!))
                {
                    await _roleManager.AddClaimAsync(adminRole, addClaim);
                }

                command.AspNetRoleId = adminRole.Id;

                await ApiClientFactory.Instance.CreatePerfil(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return View();
            }
        }

        /// <summary>
        /// Ação de Alteração de Perfil
        /// </summary>
        /// <param name="id">Identificador de Perfil</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Perfil", "Alterar")]
        public ActionResult Edit(int id)
        {
            // todo: Fábio não edita perfil quando existe modulo sem funcionalidade
            var perfil = ApiClientFactory.Instance.GetPerfilById(id);

            if (id != null)
            {
                var obj = GetPerfilByAspNetRoleId(perfil.AspNetRoleId);
                var responseModulos = ApiClientFactory.Instance.GetModulosAll();

                var listClaim = new List<Claim>();

                foreach (DictionaryEntry item in obj.Claims)
                {
                    listClaim.Add(new Claim(item.Key.ToString(), item.Value.ToString()));
                }

                var model = new PerfilModel { Perfil = perfil, Modulos = responseModulos, Claims = listClaim };
                return View(model);
            }

            return RedirectToAction(nameof(Index),
                new
                {
                    notify = 2,
                    message = "Erro ao alterar o perfil. Favor entrar em contato com o administrador do sistema."
                });
        }

        /// <summary>
        /// Ação de Alteração de Perfil
        /// </summary>
        /// <param name="id">Identificador de Perfil</param>
        /// <param name="collection">coleção de dados para alteração de Perfil</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Perfil", "Alterar")]
        [HttpPost]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                ListDictionary list = new ListDictionary();
                var responseModulos = ApiClientFactory.Instance.GetModulosAll();
                var perfil = ApiClientFactory.Instance.GetPerfilById(id);

                foreach (var modulo in responseModulos)
                {
                    var ListFuncionalidades = modulo.Funcionalidades.FirstOrDefault().Nome.Split(',');
                    var listValue = ListFuncionalidades
                        .Where(func => collection[modulo.Nome + func].ToString() == "on").ToList();
                    list.Add(modulo.Nome, string.Join(",", listValue));
                }

                var command = new PerfilModel.CreateUpdateCommand
                {
                    Id = perfil.Id,
                    Nome = collection["nome"].ToString(),
                    Descricao = collection["descricao"].ToString(),
                    Claims = list,
                    AspNetRoleId = perfil.AspNetRoleId
                };

                var adminRole = await _roleManager.FindByIdAsync(perfil.AspNetRoleId);

                if (adminRole != null)
                {
                    adminRole.Name = command.Nome;
                    var resRole = await _roleManager.UpdateAsync(adminRole);

                    var role = _roleManager.Roles.Single(x => x.Id == command.AspNetRoleId);
                    var claims = _roleManager.GetClaimsAsync(role).Result;

                    foreach (var claim in claims)
                    {
                        var res = await _roleManager.RemoveClaimAsync(adminRole, claim);
                    }

                    foreach (DictionaryEntry claim in command.Claims)
                    {
                        var addClaim = new Claim(claim.Key.ToString(), claim.Value.ToString());
                        var res = await _roleManager.AddClaimAsync(adminRole, addClaim);
                    }

                    await ApiClientFactory.Instance.UpdatePerfil(id, command);

                    return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
                }
                else
                {
                    return RedirectToAction(nameof(Index),
                        new
                        {
                            notify = 2,
                            message = "Erro ao alterar o perfil. Favor entrar em contato com o administrador do sistema."
                        });
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        ///  Ação de Exclusão do Perfil
        /// </summary>
        /// <param name="id">identificador do Perfil</param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Perfil", "Excluir")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                var perfil = ApiClientFactory.Instance.GetPerfilById(id);

                var obj = _db.Roles.Where(x => x.Id == perfil.AspNetRoleId).FirstOrDefault();

                if (obj != null)
                {
                    var result = await _roleManager.DeleteAsync(obj);
                }

                var resultDelete = ApiClientFactory.Instance.DeletePerfil(id);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });

                //if (!result.Succeeded)
                //{
                //    return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Existe  usuário vinculado a esse perfil." });
                //}
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index),
                    new
                    {
                        notify = 2,
                        message = $"Erro ao excluir o perfil. {ex.Message}"
                    });
            }
        }

        #endregion

        #region Get Methods

        /// <summary>
        ///Busca Perfil por ID de função de rede Asp
        /// </summary>
        /// <param name="aspNetRoleId">aspNetRoleId</param>
        /// <returns>Retorna a Perfil</returns>
        private PerfilDto GetPerfilByAspNetRoleId(string aspNetRoleId)
        {
            var entity = _db.Roles.Where(x => x.Id == aspNetRoleId).Select(item => new PerfilDto { Nome = item.Name, AspNetRoleId = item.Id }).FirstOrDefault();
            var role = _roleManager.Roles.Single(x => x.Id == entity.AspNetRoleId);
            var claims = _roleManager.GetClaimsAsync(role).Result;
            ListDictionary list = new ListDictionary();
            foreach (var claim in claims)
            {
                list.Add(claim.Type, claim.Value);
            }

            entity.Claims = list;

            return entity;
        }
    }

    #endregion


}
