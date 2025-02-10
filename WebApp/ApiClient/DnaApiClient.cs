using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        private readonly HttpClient _httpClient;
        private Uri BaseEndpoint { get; }

        public DnaApiClient(Uri baseEndpoint)
        {
            BaseEndpoint = baseEndpoint ?? throw new ArgumentNullException(nameof(baseEndpoint));
            _httpClient = new HttpClient();
        }

        public T Get<T>(Uri requestUrl)
        {
            addHeaders();
            var response = _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
            var data = response.Result.Content.ReadAsStringAsync();
            //if (data.Exception == null) return JsonConvert.DeserializeObject<T>(data.Result)!;
            //dynamic dataResult = JObject.Parse(data.Result);
            //if (dataResult.status == 409)
            //{
            //    throw new ApplicationException(dataResult.detail.Value);
            //}
            return JsonConvert.DeserializeObject<T>(data.Result)!;
        }

        public Task<T?> GetFiltro<T>(Uri requestUrl, T content)
        {
            addHeaders();
            var response = _httpClient.PostAsync(requestUrl, CreateHttpContent<T>(content));
            var data = response.Result.Content.ReadAsStringAsync();
            return Task.FromResult(JsonConvert.DeserializeObject<T>(data.Result));
        }


        /// <summary>
        /// Common method for making POST calls
        /// </summary>
        public Task<long> Post<T>(Uri requestUrl, T content)
        {
            addHeaders();
            var response = _httpClient.PostAsync(requestUrl.ToString(), CreateHttpContent<T>(content));
            var data = response.Result.Content.ReadAsStringAsync();
            return Task.FromResult(JsonConvert.DeserializeObject<long>(data.Result));
        }

        public Task<T?> PostWithResponseBody<T>(Uri requestUrl, T content)
        {
            addHeaders();
            var response = _httpClient.PostAsync(requestUrl.ToString(), CreateHttpContent<T>(content));
            var data = response.Result.Content.ReadAsStringAsync();
            return Task.FromResult(JsonConvert.DeserializeObject<T>(data.Result));
        }

        /// <summary>
        /// Common method for making PUT calls
        /// </summary>
        public Task<bool> Put<T>(Uri requestUrl, T content)
        {
            addHeaders();
            var response = _httpClient.PutAsync(requestUrl.ToString(), CreateHttpContent<T>(content));
            var data = response.Result.Content.ReadAsStringAsync();
            return Task.FromResult(JsonConvert.DeserializeObject<bool>(data.Result));
        }
        //public Task<bool> Put<T>(Uri requestUrl, T content)
        //{
        //    addHeaders();
        //     var response = _httpClient.PutAsync(requestUrl.ToString(), CreateHttpContent<T>(content));
        //    var data = response.Result.Content.ReadAsStringAsync();
        //    return Task.FromResult(JsonConvert.DeserializeObject<bool>(data.Result));
        //}

        /// <summary>
        /// Common method for making DELETE calls
        /// </summary>
        public Task<T> Delete<T>(Uri requestUrl)
        {
            addHeaders();
            var response = _httpClient.DeleteAsync(requestUrl);

            var data = response.Result.Content.ReadAsStringAsync();

            if (data.Result != "true")
            {
                dynamic dataResult = JObject.Parse(data.Result);
                if (dataResult.status == 409)
                {
                    //throw new ArgumentException(dataResult.detail);
                    //throw new CustomException<MyCustomException>("your error description");
                    throw new ApplicationException(dataResult.detail.Value);
                }
            }
            else if (data.Result == "false")
            {
                throw new ApplicationException("Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema.");

            }

            return Task.FromResult(JsonConvert.DeserializeObject<T>(data.Result));
        }

        public Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint);
            uriBuilder.Query = queryString;
            return uriBuilder.Uri;
        }

        private HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content, MicrosoftDateFormatSettings);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static JsonSerializerSettings MicrosoftDateFormatSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };
            }
        }

        private void addHeaders()
        {
            _httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            //_httpClient.DefaultRequestHeaders.Authorization =
            //    new AuthenticationHeaderValue("Bearer", "Your Oauth token");
        }





    }
}
