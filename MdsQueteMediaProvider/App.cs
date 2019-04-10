using MdsProvider.ServiceMain.Controller;
using MdsProvider.TestService;
using MdsProvider.TestService.API;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MdsProvider
{
    public class App
    {
        private readonly ITestService _testService;
        private readonly MdsProviderDealer _dealer;

        public App  (
                        ITestService testService, 
                        MdsProviderDealer dealer
                    )
        {
            _testService = testService;
            _dealer = dealer;
        }

        public void Run()
        {
            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            // bootstrap any services here 
            _testService.Run();
            _dealer.Run();

            Log.Logger.Information(@"Press 'Ctrl+c' to quit.");
            exitEvent.WaitOne();
        }
    }
}
