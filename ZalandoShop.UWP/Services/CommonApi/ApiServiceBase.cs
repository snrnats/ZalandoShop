using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Windows.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using MetroLog;
using Microsoft.Toolkit.Uwp;
using Newtonsoft.Json;
using ZalandoShop.UWP.Services.CommonApi.Exceptions;

namespace ZalandoShop.UWP.Services.CommonApi
{
    public class ApiServiceBase
    {
        private static readonly ILogger Logger = LogManagerFactory.DefaultLogManager.GetLogger<ApiServiceBase>();

        protected readonly HttpClient HttpClient;

        public ApiServiceBase()
        {
            HttpClient = new HttpClient
            {
                DefaultRequestHeaders = {AcceptLanguage = {HttpLanguageRangeWithQualityHeaderValue.Parse(CultureInfo.CurrentUICulture.Name)}}
            };
        }

        private static Uri Endpoint(params string[] args)
        {
            var sb = new StringBuilder();
            foreach (var arg in args)
            {
                sb.Append(arg);
                sb.Append('/');
            }
            sb.Remove(sb.Length - 1, 1);
            return new Uri(sb.ToString());
        }

        private async Task<HttpResponseMessage> SendInternalAsync(HttpMethod httpMethod, Uri uri, IHttpContent content)
        {
            var request = new HttpRequestMessage(httpMethod, uri);
            if (content != null)
            {
                request.Content = content;
            }
            Logger.Info($"Sending {httpMethod} request to {uri} {(content != null ? "\n" + content : "")}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            HttpResponseMessage response;
            try
            {
                response = await HttpClient.SendRequestAsync(request).AsTask().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                var status = WebError.GetStatus(e.HResult);
                Logger.Error($"Caught exception in {stopwatch.ElapsedMilliseconds}ms while sending request to {uri}:\n${status}");
                if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
                {
                    throw new NoInternetException(status);
                }
                throw new NetworkException(status);
            }

            stopwatch.Stop();

            var success = response.IsSuccessStatusCode;
            if (!success)
            {
                var responeText = await response.Content.ReadAsStringAsync().AsTask().ConfigureAwait(false);
                Logger.Error($"Server responded with error code in {stopwatch.ElapsedMilliseconds}ms {uri} {response.StatusCode}:\n{responeText}");
                throw new ServerException((int) response.StatusCode, responeText);
            }
            Logger.Info($"Got response from {uri} in {stopwatch.ElapsedMilliseconds}ms with status code: {response.StatusCode}");
            return response;
        }

        protected async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string[] urlParts, string queryString = null, IHttpContent content = null)
        {
            if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                throw new NoInternetException();
            }
            var endpoint = Endpoint(urlParts);
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                endpoint = new Uri(endpoint.AbsoluteUri + "?" + queryString);
            }
            return await SendInternalAsync(httpMethod, endpoint, null).ConfigureAwait(false);
        }


        protected async Task<T> SendAsync<T>(HttpMethod httpMethod, string[] urlParts, string queryString = null, IHttpContent content = null)
        {
            var response = await SendAsync(httpMethod, urlParts, queryString, content).ConfigureAwait(false);
            var responseText = await response.Content.ReadAsStringAsync().AsTask().ConfigureAwait(false);
            try
            {
                return JsonConvert.DeserializeObject<T>(responseText);
            }
            catch (JsonException e)
            {
                Logger.Error("Failed to deserialize response", e);
                throw new ServerException((int) response.StatusCode, responseText);
            }

        }
    }
}
