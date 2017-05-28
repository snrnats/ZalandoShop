namespace ZalandoShop.UWP.Services.CommonApi.Exceptions
{
    public class ServerException : ApiException
    {
        public ServerException(int statusCode, string response)
        {
            StatusCode = statusCode;
            Response = response;
        }

        public int StatusCode { get; set; }
        public string Response { get; set; }

        public override string ToString()
        {
            return $"Server responded with code: {StatusCode}. Response: {Response}";
        }
    }
}