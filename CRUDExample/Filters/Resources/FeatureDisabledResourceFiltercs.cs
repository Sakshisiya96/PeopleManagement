using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.Resources
{
    public class FeatureDisabledResourceFiltercs : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisabledResourceFiltercs> _logger;
        private readonly bool _isDisabled;
        public FeatureDisabledResourceFiltercs(ILogger<FeatureDisabledResourceFiltercs> logger, bool isDisabled=true)
        {
            _logger = logger;
            _isDisabled = isDisabled;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName}-before",nameof(FeatureDisabledResourceFiltercs),nameof(OnResourceExecutionAsync));
            if (_isDisabled)
            {
                //  context.Result = new NotFoundResult(); 404 - Not found
                context.Result = new StatusCodeResult(501);
            }
            else
            {
                await next();
            }
            _logger.LogInformation("{FilterName}.{MethodName}-after", nameof(FeatureDisabledResourceFiltercs), nameof(OnResourceExecutionAsync));
        }
    }
}
