using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using WebApp.Authorization;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers;

/// <summary>
/// Controller de Evento
/// </summary>
[Authorize(Policy = ModuloAccess.Evento)]
public class EventoController : BaseController
{
    #region Constructor

    private readonly IWebHostEnvironment _host;

	/// <summary>
	/// Construtor da página
	/// </summary>
	/// <param name="app">configurações de urls do sistema</param>
	/// <param name="host">informações da aplicação em execução</param>
	public EventoController(IOptions<UrlSettings> appSettings, IWebHostEnvironment host)
    {
	    _host = host;
        ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Main Methods

    /// <summary>
    /// Listagem de Evento
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Evento, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetEventosAll();

        return View(new EventoModel() { Eventos = response });
    }

    /// <summary>
    /// Tela para Inclusão de Evento
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Evento, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");



            return View(new EventoModel()
            {
                ListEstados = estados
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Inclusão do Evento
    /// </summary>
    /// <param name="collection">coleção de dados para Inclusao de Evento</param>
    /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Evento, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new EventoModel.CreateUpdateEventoCommand
            {
                LocalidadeId = Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                Titulo = collection["titulo"].ToString(),
                Descricao = collection["descricao"].ToString(),
                DataEvento = collection["dtEvento"].ToString(),
            };

            await ApiClientFactory.Instance.CreateEvento(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }


    /// <summary>
    /// Ação de alteração do Evento
    /// </summary>
    /// <param name="collection">coleção de dados para alteração de Evento</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Evento, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new EventoModel.CreateUpdateEventoCommand
            {
                Id = Convert.ToInt32(collection["editEventoId"]),
                Titulo = collection["titulo"].ToString(),
                Descricao = collection["descricao"].ToString(),
                DataEvento = collection["dtEvento"].ToString(),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            await ApiClientFactory.Instance.UpdateEvento(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão do Evento
    /// </summary>
    /// <param name="id">Identificador do Evento</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Evento, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            if (ApiClientFactory.Instance.GetControlesPresencasByEventoId(id).Any())
            {
                return RedirectToAction(nameof(IndexControlePresenca), new { eventoId = id, notify = (int)EnumNotify.Error, message = "O evento não pode ser excluído pois existem presenças registradas para o mesmo." });
            }
            ApiClientFactory.Instance.DeleteEvento(id);
            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
        }
        catch
        {
            return RedirectToAction(nameof(Index));
        }
    }
	#endregion

	#region Methods
	/// <summary>
	/// Ação de Upload de fotos do Evento
	/// </summary>
	/// <param name="collection">arquivo de Upload realizado</param>
	/// <returns>retorna mensagem de Upload realizado através do parametro notfy e message</returns>
	[HttpPost]
	[ClaimsAuthorize(ClaimType.Evento, Claim.Incluir)]
	public async Task<ActionResult> Upload(IFormCollection collection)
	{
		try
		{
			string filePath = null;
			string fileName = null;
            
			var list = new List<CreateFotoEventoDto>();

			foreach (var t in collection.Files)
			{
				var file = t;
				if (file.Length <= 0) continue;
				fileName = $"{collection["eventoId"]}-{Guid.NewGuid()}.jpg";
				filePath = Path.Combine(_host.WebRootPath, $"Eventos\\{fileName}");

				if (!Directory.Exists(Path.Combine(_host.WebRootPath, $"Eventos")))
					Directory.CreateDirectory(Path.Combine(_host.WebRootPath, $"Eventos"));

				using Stream fileStream = new FileStream(filePath, FileMode.Create);
				await file.CopyToAsync(fileStream);

				list.Add(new CreateFotoEventoDto()
				{
					EventoId = Convert.ToInt32(collection["eventoId"]),
					NomeArquivo = fileName,
					Url = filePath
				});
			}
            await ApiClientFactory.Instance.CreateFotoEvento(list);

			return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Success, message = "Upload de foto realizado com sucesso." });
		}
		catch (Exception e)
		{
			Console.Write(e.StackTrace);
			return RedirectToAction(nameof(Index), new { notify = EnumNotify.Error, mesage = e.Message });
		}
	}

	/// <summary>
	/// Ação de Download de Fotos do Evento
	/// </summary>
	/// <param name="id">Id do Evento</param>
	/// <returns>retorna Fotos para Download</returns>
	[ClaimsAuthorize(ClaimType.Evento, Claim.Incluir)]
	public ActionResult Download(int id)
	{
		var list = new List<FileContentResult>();

		var files = ApiClientFactory.Instance.GetFotosAllByEventoId(id);

		MemoryStream outms = new MemoryStream();

		using (ZipArchive zar = new ZipArchive(outms, ZipArchiveMode.Create, false))
		{
			foreach (var file in files)
			{
				var filePath = Path.Combine(_host.WebRootPath, $"Eventos\\{file.NomeArquivo}");

				if (!System.IO.File.Exists(filePath))
				{
					return RedirectToAction(nameof(Index),
						new { notify = (int)EnumNotify.Warning, message = "Arquivo não encontrado." });
				}

				var fileBytes = System.IO.File.ReadAllBytes(filePath);

				var fileName = file.NomeArquivo;

				byte[] unzipped = fileBytes;
				ZipArchiveEntry entry = zar.CreateEntry(fileName);
				using (Stream str = entry.Open())
				{
					str.Write(unzipped);
				}
			}
		}

		var outdata = outms.ToArray();

		var result = File(outdata, "application/zip", $"evento-{id}.zip");
		return result;
	}
	#endregion

	#region Crud Controle Presenca Evento Methods

	/// <summary>
	/// Listagem de Controle de Presença de Evento
	/// </summary>
	/// <param name="eventoId">Id do Evento</param>
	/// <param name="crud">paramentro que indica o tipo de ação realizado</param>
	/// <param name="notify">parametro que indica o tipo de notificação realizada</param>
	/// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
	[ClaimsAuthorize(ClaimType.Evento, Identity.Claim.Consultar)]
    public IActionResult IndexControlePresenca(int eventoId, int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var listControlePresenca = ApiClientFactory.Instance.GetControlesPresencasByEventoId(eventoId);
        var evento = ApiClientFactory.Instance.GetEventoById(eventoId);

        return View(new EventoModel() { ControlesPresencas = listControlePresenca, Evento = evento });
    }

    /// <summary>
    /// Tela para Inclusão de Controle de Presença do Evento
    /// </summary>
    /// <param name="eventoId">Id do Evento </param>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Evento, Identity.Claim.Incluir)]
    public async Task<ActionResult> CreateControlePresenca(int eventoId, int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            long alunoId = 0;

            var evento = ApiClientFactory.Instance.GetEventoById(eventoId);

            var listAlunos = ApiClientFactory.Instance.GetAlunosByLocalidade(Convert.ToInt32(evento.LocalidadeId));

            var alunos = new SelectList(listAlunos, "Id", "Nome"); 

            var convidado = listAlunos.FirstOrDefault(x => x.Convidado);


            if (convidado == null)
            {
                var command = new AlunoModel.CreateUpdateDadosAlunoCommand
                {
                    MunicipioId = Convert.ToInt32(evento.MunicipioId),
                    Nome = "Convidado",
                    Email = "convidado@convidado.com",
                    Sexo = "G",
                    DtNascimento = DateTime.Now.ToString("dd/MM/yyyy"),
                    LocalidadeId = Convert.ToInt32(evento.LocalidadeId),
                    FomentoId = Convert.ToInt32(ApiClientFactory.Instance.GetFomentoByLocalidadeId(Convert.ToInt32(evento.LocalidadeId)).Id),
                    Etnia = "NA",
                    Convidado = true


                };

                await ApiClientFactory.Instance.CreateDados(command);

                convidado = ApiClientFactory.Instance.GetAlunosByLocalidade(Convert.ToInt32(evento.LocalidadeId))
                    .FirstOrDefault(x => x.Convidado);
            }

            return View(new EventoModel()
            {
                Evento = evento,
                ListAlunos = alunos,
                Convidado = convidado
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(IndexControlePresenca), new { eventoId = eventoId, notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação para Inclusão de Controle de Presença do Evento
    /// </summary>
    /// <param name="collection">coleção de dados para Inclusao de Controle de Presença do Evento</param>
    /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
    [HttpPost]
    [ClaimsAuthorize(ClaimType.Evento, Identity.Claim.Alterar)]
    public async Task<ActionResult> CreateControlePresenca(IFormCollection collection)
    {
        try
        {
            var command = new ControlePresencaModel.CreateUpdateControlePresencaCommand()
            {
                EventoId = Convert.ToInt32(collection["eventoId"]),
                MunicipioId = collection["municipioId"] == "" ? null : Convert.ToInt32(collection["municipioId"].ToString()).ToString(),
                LocalidadeId = collection["localidadeId"] == "" ? null : Convert.ToInt32(collection["localidadeId"].ToString()),
                Controle = collection["controle"].ToString(),
                Justificativa = collection["alunoConvidadoEvento"] == "A" ? "" : collection["convidado"].ToString(),
                AlunoId = collection["alunoConvidadoEvento"] == "A" ? collection["ddlAluno"].ToString() : collection["convidadoId"].ToString(),
            };

            if (collection["alunoConvidadoEvento"] == "A")
            {
                var possuiPrecensa = ApiClientFactory.Instance.GetControlePresencaByAlunoId(Convert.ToInt32(command.AlunoId)).Where(x => x.Data == DateTime.Now.ToString("dd/MM/yyyy"));

                if (possuiPrecensa.Any())
                {
                    return RedirectToAction(nameof(CreateControlePresenca), new { eventoId = command.EventoId, notify = (int)EnumNotify.Warning, message = "Já existe presença cadastrada para este aluno no dia de hoje." });
                }
            }

            await ApiClientFactory.Instance.CreateControlePresenca(command);

            return RedirectToAction(nameof(IndexControlePresenca), new { eventoId = command.EventoId, crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(IndexControlePresenca), new { eventoId = Convert.ToInt32(collection["eventoId"]), notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Alteração do Controle de Presença do Evento
    /// </summary>
    /// <param name="collection">coleção de dados para alteração de controle de presença do Evento</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Evento, Identity.Claim.Alterar)]
    public async Task<ActionResult> EditControlePresenca(IFormCollection collection)
    {
        try
        {
            var command = new ControlePresencaModel.CreateUpdateControlePresencaCommand()
            {
                Id = Convert.ToInt32(collection["editControlePresencaEventoId"]),
                Controle = collection["controle"].ToString(),
                Justificativa = collection["convidado"].ToString(),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            await ApiClientFactory.Instance.UpdateControlePresenca(command.Id, command);

            return RedirectToAction(nameof(IndexControlePresenca), new { eventoId = Convert.ToInt32(collection["editEventoId"]), crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(IndexControlePresenca), new { eventoId = Convert.ToInt32(collection["editEventoId"]), notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }
    [ClaimsAuthorize(ClaimType.ControlePresenca, Claim.Excluir)]
    public ActionResult DeleteControlePresenca(int id, int eventoId)
    {
        try
        {
            ApiClientFactory.Instance.DeleteControlePresenca(id);
            return RedirectToAction(nameof(IndexControlePresenca), new { eventoId = eventoId, crud = (int)EnumCrud.Deleted });
        }
        catch
        {
            return RedirectToAction(nameof(IndexControlePresenca), new { eventoId = eventoId, notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }
    #endregion

    #region Get Methods
    /// <summary>
    /// Busca um Evento por Id
    /// </summary>
    /// <param name="id">Id do Evento</param>
    /// <returns>Entidade Evento</returns>
    public Task<EventoDto> GetEventoById(int id)
    {
        var result = ApiClientFactory.Instance.GetEventoById(id);

        return Task.FromResult(result);
    }
    #endregion
}