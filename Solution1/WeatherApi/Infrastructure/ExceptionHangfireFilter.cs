using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging;

namespace WeatherApi.Infrastructure
{
    public class ExceptionHangfireFilter : JobFilterAttribute, IElectStateFilter
    {
        private readonly ILogger _logger;
        public ExceptionHangfireFilter (ILogger<BackgroundJob> logger)
        {
            _logger = logger;
        }
        public void OnStateElection(ElectStateContext context)
        {
            var failedState = context.CandidateState as FailedState;
            if (failedState != null  )
            {
                _logger.LogError(failedState.Exception.Message);
            }
        }
    }
}
