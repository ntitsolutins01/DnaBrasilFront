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
using Microsoft.AspNetCore.Hosting;
using log4net;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;

namespace WebApp.Controllers;

/// <summary>
/// Controle de ArquivosInventario
/// </summary>
[Authorize(Policy = ModuloAccess.ConfiguracaoSistemaEad)]
public class ArquivosInventarioController : BaseController
{
    #region Parametros

    private readonly IOptions<UrlSettings> _appSettings;
    private readonly IWebHostEnvironment _host;

    #endregion

    #region Constructor


    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="app">configurações de urls do sistema</param>
    /// <param name="host">informações da aplicação em execução</param>
    public ArquivosInventarioController(IOptions<UrlSettings> appSettings, IWebHostEnvironment host,
    ILog logger)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        _host = host;
    }
    #endregion

    #region Main Methods

    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteArquivosInventario(id);
            return RedirectToAction("Index", "Inventario", new { crud = (int)EnumCrud.Deleted });
        }
        catch (Exception e)
        {
            return RedirectToAction("Index", "Inventario", new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ClaimsAuthorize(ClaimType.Material, Claim.Alterar)]
    public ActionResult Download(int id)
    {
        try
        {
            var arquivo = ApiClientFactory.Instance.GetArquivosInventarioById(id);
            if (arquivo == null)
            {
                return NotFound("Arquivo não encontrado.");
            }

            var index = arquivo.PathArquivo.IndexOf("wwwroot", StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                return NotFound("Caminho do arquivo inválido.");
            }
            var relativePath = arquivo.PathArquivo.Substring(index + "wwwroot".Length)
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            var caminhoArquivo = Path.Combine(_host.WebRootPath, relativePath);

            if (!System.IO.File.Exists(caminhoArquivo))
            {
                return NotFound("Arquivo não existe no servidor: " + caminhoArquivo);
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(arquivo.NomeArquivo, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            return PhysicalFile(caminhoArquivo, contentType, arquivo.NomeArquivo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao baixar arquivo: {ex.Message}");
            return RedirectToAction("Index", "Inventario");
        }
    }

    #endregion

    #region Get Methods

    /// <summary>
    /// Busca Materail por Id
    /// </summary>
    /// <param name="id">Identificador de Materail</param>
    /// <returns>Retorna a ArquivosInventario</returns>
    public Task<ArquivosInventarioDto> GetArquivosInventarioById(int id)
    {
        var result = ApiClientFactory.Instance.GetArquivosInventarioById(id);

        return Task.FromResult(result);
    }

    /// <summary>
    /// Método de busca todos os Arquivos pelo id do inventario
    /// </summary>
    /// <param name="id">Id do inventario</param>
    /// <returns>Retorna um json com todos os Arquivos</returns>
    public Task<JsonResult> GetArquivosInventariosByInventarioId(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("ArquivosInventario não informado.");
            var resultLocal = ApiClientFactory.Instance.GetArquivosInventariosByInventarioId(Convert.ToInt32(id));

            return Task.FromResult(Json(new SelectList(resultLocal, "Id", "NomeArquivo")));

        }
        catch (Exception ex)
        {
            return Task.FromResult(Json(ex.Message));
        }
    }
    #endregion
}