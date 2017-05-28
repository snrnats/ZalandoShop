using Windows.Web;

namespace ZalandoShop.UWP.Services.CommonApi.Exceptions
{
    public class NetworkException : ApiException
    {
        public NetworkException(WebErrorStatus status) : base($"Network exception: {status}")
        {
        }
    }
}
