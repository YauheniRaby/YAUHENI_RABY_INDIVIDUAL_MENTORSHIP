using System.Threading;

namespace BusinessLayer.Infrastructure
{
    public static class TokenProvider
    {
        public static CancellationToken GetCancellationToken(int timeout)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(timeout);            
            return cancellationTokenSource.Token;
        }
    }
}
