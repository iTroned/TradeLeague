using Microsoft.Extensions.Hosting;
using StockApplication.Controllers;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class ValueUpdater : BackgroundService
    {
        
        public ValueUpdater()
        {
            
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //private readonly PeriodicTimer _
            while (!stoppingToken.IsCancellationRequested)
            {
                
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
