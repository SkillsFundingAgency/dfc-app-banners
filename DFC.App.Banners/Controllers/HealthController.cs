using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DFC.App.Banners.Extensions;
using DFC.App.Banners.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Controllers
{
    public class HealthController : Controller
    {
        public const string HealthViewCanonicalName = "health";

        private readonly ILogger<HealthController> logger;
        private readonly string resourceName = typeof(Program).Namespace!;

        public HealthController(ILogger<HealthController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("banners/health")]
        public IActionResult HealthView()
        {
            var result = Health();

            return result;
        }

        [HttpGet]
        [Route("health")]
        public IActionResult Health()
        {
            logger.LogInformation("Generating Health report");

            var isHealthy = true;

            if (isHealthy)
            {
                const string message = "Document store is available";
                logger.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

                var viewModel = CreateHealthViewModel(message);

                logger.LogInformation("Generated Health report");

                return this.NegotiateContentResult(viewModel, viewModel.HealthItems);
            }

            logger.LogError($"{nameof(Health)}: Ping to {resourceName} has failed");

            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        [HttpGet]
        [Route("health/ping")]
        public IActionResult Ping()
        {
            logger.LogInformation($"{nameof(Ping)} has been called");

            return Ok();
        }

        private HealthViewModel CreateHealthViewModel(string message)
        {
            return new HealthViewModel
            {
                HealthItems = new List<HealthItemViewModel>
                {
                    new HealthItemViewModel
                    {
                        Service = resourceName,
                        Message = message,
                    },
                },
            };
        }
    }
}