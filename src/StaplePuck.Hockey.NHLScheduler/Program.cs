// See https://aka.ms/new-console-template for more information
using StaplePuck.Hockey.NHLScheduler;

Console.WriteLine("Hello, World!");

var date = DateTime.Parse("2024-05-01");
date = date.Date.AddHours(10);
var tokenSource = new CancellationTokenSource();
var scheduler = Scheduler.Init();
await scheduler.CreateSchedulesAsync(tokenSource.Token);