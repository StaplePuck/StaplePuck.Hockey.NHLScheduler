using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace StaplePuck.Hockey.NHLScheduler
{
    public class LambdaEntryPoint
    {
        public LambdaEntryPoint()
        {
        }

        public async Task ProcessMessage(string input, ILambdaContext context)
        {
            var tokenSource = new CancellationTokenSource();
            var scheduler = Scheduler.Init();
            await scheduler.CreateSchedulesAsync(tokenSource.Token);
        }
    }
}
