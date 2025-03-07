using DocumentFormat.OpenXml.Office2010.Excel;
using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Aluno Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceAlunos = "Alunos";

        #region Main Methods
        /// <summary>
        /// Inclusão de Aluno
        /// </summary>
        /// <param name="command">Objeto para inclusão do Aluno</param>
        /// <returns>ID do Aluno inserido</returns>
        public Task<long> CreateDados(AlunoModel.CreateUpdateDadosAlunoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração do Aluno
        /// </summary>
        /// <param name="id">ID da alteração do Aluno</param>
        /// <param name="command">Objeto de alteração do Aluno</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateDados(int id, AlunoModel.CreateUpdateDadosAlunoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Alteração do QR Code do Aluno
        /// </summary>
        /// <param name="id">ID da alteração do QR Code do Aluno</param>
        /// <param name="command">Objeto para alteração do QR Code do Aluno</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateQrCode(int id, AlunoModel.CreateUpdateDadosAlunoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}/QrCode/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Alteração da foto do Aluno
        /// </summary>
        /// <param name="id">ID da alteração da foto do Aluno</param>
        /// <param name="command">Objeto para alteração da foto do Aluno</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateAlunoFoto(int id, AlunoModel.CreateUpdateDadosAlunoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}/UploadFoto/{id}"));
            return Put(requestUrl, command);
        }
        /// <summary>
        /// Exclusão de Aluno
        /// </summary>
        /// <param name="id">ID da exclusão do Aluno </param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteDados(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Buscar todos os Alunos
        /// </summary>
        /// <returns>Retorna a lista de Aluno</returns>
        public List<AlunoDto> GetAlunosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}"));
            return Get<List<AlunoDto>>(requestUrl);
        }

        /// <summary>
        /// Buscar um único Aluno
        /// </summary>
        /// <param name="id">ID do Aluno a ser buscado</param>
        /// <returns>Retorna o objeto do Aluno</returns>
        public async Task<AlunoDto> GetAlunoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}/{id}"));
            return Get<AlunoDto>(requestUrl);
        }

        /// <summary>
        /// Buscar Aluno por e-mail
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>Retorna uma lista de email</returns>
        public AlunoDto GetAlunoByEmail(string email)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}/Email/{email}"));
            return Get<AlunoDto>(requestUrl);
        }

        /// <summary>
        /// Busca o Aluno por rede ASP
        /// </summary>
        /// <param name="aspNetUserId">AspNetUserId</param>
        /// <returns>Retorna o aluno por rede</returns>
        public AlunoDto GetAlunoByAspNetUser(string aspNetUserId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}/AspNetUserId/{aspNetUserId}"));
            return Get<AlunoDto>(requestUrl);
        }

        /// <summary>
        /// Busca o Aluno por localidade
        /// </summary>
        /// <param name="id">ID do Aluno a ser buscado</param>
        /// <returns>Retorna a localidade</returns>
        public List<AlunoIndexDto> GetAlunosByLocalidadeId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}/Localidade/{id}"));
            return Get<List<AlunoIndexDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todos os nomes de Alunos
        /// </summary>
        /// <param name="id">ID da localidade a ser buscada</param>
        /// <returns>Retorna todos os Alunos</returns>
        public async Task<List<SelectListDto>> GetNomeAlunosByLocalidadeId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}/NomeAlunos/Localidade/{id}"));
            return Get<List<SelectListDto>>(requestUrl);
        }

        /// <summary>
        /// Busca o Aluno por filtro
        /// </summary>
        /// <param name="searchFilter">Filtro para pesquisa de Aluno</param>
        /// <returns>Retorna a lista de Alunos</returns>
        public Task<AlunosFilterDto?> GetAlunosByFilter(AlunosFilterDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}/Filter"));
            return GetFiltro(requestUrl, searchFilter);
        }

        /// <summary>
        /// Busca todos os nomes de Alunos
        /// </summary>
        /// <param name="id">ID dos Aluno</param>
        /// <returns>Retorna lista dos Alunos</returns>
        public List<SelectListDto> GetNomeAlunosByProfissionalId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunos}/Profissional/{id}"));
            return Get<List<SelectListDto>>(requestUrl);
        }

        /// <summary>
        /// Busca o Modelo de Carteirinha Pelo ID do Fomento
        /// </summary>
        /// <param name="fomentoId">Id do Fomento</param>
        /// <returns>Retorna Modelo de Carteirinha</returns>
        public async Task<ModeloCarteirinhaDto> GetModeloCarteirinhaByFomentoId(int fomentoId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"ModelosCarteirinhas/Fomento/{fomentoId}"));
            return Get<ModeloCarteirinhaDto>(requestUrl);
        }


        #endregion
    }
}