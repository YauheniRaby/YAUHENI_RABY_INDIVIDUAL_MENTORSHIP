using System.Threading;

namespace BusinessLayer.Infrastructure
{
    public static class TokenGenerator
    {
        public static CancellationToken GetCancellationToken(int? timeout)
        {
            return timeout.HasValue ? new CancellationTokenSource(timeout.Value).Token : CancellationToken.None;             
        }
    }
}
