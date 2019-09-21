using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using TradingPlaces.WebApi.Dtos;
using TradingPlaces.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace TradingPlaces.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class StrategyController : ControllerBase
    {
        private readonly IHostedServiceAccessor<IStrategyManagementService> _strategyManagementService;
        private readonly ILogger<StrategyController> _logger;

        public StrategyController(IHostedServiceAccessor<IStrategyManagementService> strategyManagementService, ILogger<StrategyController> logger)
        {
            _strategyManagementService = strategyManagementService;
            _logger = logger;
            _strategyManagementService = strategyManagementService;
        }

        [HttpPost]
        [SwaggerOperation(nameof(RegisterStrategy))]
        [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(string))]
        public IActionResult RegisterStrategy(StrategyDetailsDto strategyDetails)
        {
            StrategyManagementService service = _strategyManagementService.Service as StrategyManagementService;
            service.ResgisterStrategy(strategyDetails);
            return Ok(strategyDetails);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(nameof(UnregisterStrategy))]
        [SwaggerResponse(StatusCodes.Status200OK, "OK")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public IActionResult UnregisterStrategy(string id)
        {
            return Ok(id);
        }
    }
}
