using AdvertApi.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AdvertApi.HealthChecks
{
    public class StorageHealthCheck : IHealthCheck
    {

        private readonly IAdvertStorageService advertStorageService;

        public StorageHealthCheck(IAdvertStorageService advertStorageService)
        {
            this.advertStorageService = advertStorageService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var isStorageOK = await advertStorageService.CheckHealthAsync();
            return isStorageOK ? HealthCheckResult.Healthy("The check indicates a healthy result.") : HealthCheckResult.Unhealthy("The check indicates an unhealthy result.");
        }
    }
}
