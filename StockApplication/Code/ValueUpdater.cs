using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockApplication.Code.DAL;
using StockApplication.Code.Handlers;
using StockApplication.Controllers;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class ValueUpdater : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ValueUpdater> _logger;
        public ValueUpdater(IServiceProvider serviceProvider, ILogger<ValueUpdater> logger) =>
        (_serviceProvider, _logger) = (serviceProvider, logger);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    IStockRepository rep = scope.ServiceProvider.GetRequiredService<IStockRepository>();
                    await rep.UpdateValues();
                }
                //await Task.Delay(60000, stoppingToken);
                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}
