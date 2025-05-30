using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Voucher Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceVoucher = "Vouchers";
        #region Main Methods

        /// <summary>
        /// Inclusão de Voucher
        /// </summary>
        /// <param name="command">Objeto de inclusão de Voucher</param>
        /// <returns>Id do Voucher inserido</returns>
        public Task<long> CreateVoucher(VoucherModel.CreateUpdateVoucherCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceVoucher}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Voucher
        /// </summary>
        /// <param name="id">Id de alteração de Voucher</param>
        /// <param name="command">Objeto de alteração de Voucher</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateVoucher(int id, VoucherModel.CreateUpdateVoucherCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceVoucher}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Voucher
        /// </summary>
        /// <param name="id">Id de exclusão de Voucher</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteVoucher(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceVoucher}/{id}"));
            return Delete<bool>(requestUrl);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Voucher
        /// </summary>
        /// <param name="id">Id de Voucher a ser buscado</param>
        /// <returns>Retorna o objeto de Voucher</returns>
        public VoucherDto GetVoucherById(string id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceVoucher}/{id}"));
            return Get<VoucherDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Voucher cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Voucher</returns>
        public List<VoucherDto> GetVoucherAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceVoucher}"));
            return Get<List<VoucherDto>>(requestUrl);
        }

        #endregion
    }
}