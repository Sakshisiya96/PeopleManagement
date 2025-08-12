using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseActiomFilter : IAsyncActionFilter,IOrderedFilter
    {
        private readonly ILogger<ResponseActiomFilter> _logger;
        private readonly string _key;
        private readonly string _value;
        public int Order { get; set; }
        public ResponseActiomFilter(ILogger<ResponseActiomFilter> logger,string key,string value,int order)
        {
            _logger = logger;
            _key = key;
           _value = value;
            Order = order;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName}before method", nameof(ResponseActiomFilter), nameof(OnActionExecutionAsync));
            await next();
            _logger.LogInformation("{FilterName}.{MethodName} after method", nameof(ResponseActiomFilter), nameof(OnActionExecutionAsync));
            context.HttpContext.Response.Headers[_key] = _value;
        }
    }
}
