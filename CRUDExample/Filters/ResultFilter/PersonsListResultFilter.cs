using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFolder
{
    public class PersonsListResultFilterv : IAsyncResultFilter
    {
        private readonly ILogger<PersonsListResultFilterv> _logger;
        public PersonsListResultFilterv(ILogger<PersonsListResultFilterv> logger)
        {
            _logger = logger;
        }
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName}-before",nameof(PersonsListResultFilterv),nameof(OnResultExecutionAsync));
            context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("yyyy-MM-dd-HH:mm");
            await next();
            _logger.LogInformation("{FilterName}.{MethodName}-after", nameof(PersonsListResultFilterv), nameof(OnResultExecutionAsync));

        }
    }
}
