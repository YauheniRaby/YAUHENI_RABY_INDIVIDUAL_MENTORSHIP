using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Filter
{
    public class ExceptionHangfireFilter : JobFilterAttribute, IElectStateFilter
    {
        private readonly ILogger _logger;
        public ExceptionHangfireFilter(ILogger<BackgroundJob> logger)
        {
            _logger = logger;
        }

        public void OnStateElection(ElectStateContext context)
        {
            if (context.CandidateState is FailedState failedState)
            {
                _logger.LogError(failedState.Exception.Message);
            }
        }
    }
}
