using Windows.Web;

namespace ZalandoShop.UWP.Services.CommonApi.Exceptions
{
    public class NoInternetException : NetworkException
    {
        public NoInternetException(WebErrorStatus status) : base(status)
        {
        }

        public NoInternetException() : base(WebErrorStatus.CannotConnect)
        {
        }
    }
}
