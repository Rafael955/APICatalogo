using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace APICatalogo.Filters
{
    public class ApiLoggingFilter : IActionFilter
    {
        private readonly ILogger<ApiLoggingFilter> _logger;

        public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
        {
            _logger = logger;
        }

        //Enquanto a ação está sendo executada
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("### Executando -> OnActionExecuting");
            _logger.LogInformation("#############################################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"ModelState : {context.ModelState.IsValid}");
            _logger.LogInformation("#############################################");
        }

        //Depois que a ação foi executada esse método é disparado
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("### Executando -> OnActionExecuted");
            _logger.LogInformation("#############################################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation("#############################################");
        }
    }
}
