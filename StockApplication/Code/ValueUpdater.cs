using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class ValueUpdater : IHostedService
    {
        private Timer _timer;
        
        public static void updateAllValues()
        {
            //Code that runs every given time
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer((e) =>
            {
                updateAllValues();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
