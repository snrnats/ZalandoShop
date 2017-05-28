using System;
using System.Threading.Tasks;

namespace ZalandoShop.UWP.Helpers
{
    public static class FuncEx
    {
        public static Func<Task<TResult>> Try<TResult, TException>(this Func<Task<TResult>> func)
            where TException : Exception
        {
            return async () =>
            {
                try
                {
                    return await func().ConfigureAwait(false);
                }
                catch (TException)
                {
                    return default(TResult);
                }
            };
        }

        public static Func<Task<TResult>> Make<TResult>(Func<Task<TResult>> func)
        {
            return func;
        }

        public static async Task<TResponse> RetryWhenException<TResponse, TException>(this Func<Task<TResponse>> func, int triesRemaining = 3)
            where TException : Exception
        {
            try
            {
                return await func().ConfigureAwait(false);
            }
            catch (TException)
            {
                if (--triesRemaining > 0)
                {
                    return await RetryWhenException<TResponse, TException>(func, triesRemaining).ConfigureAwait(false);
                }
                throw;
            }
        }
    }
}
