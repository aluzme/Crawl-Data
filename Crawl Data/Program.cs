using Crawl_Data.Models;
using Crawl_Data.Services;
using FluentScheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mynt.Core.Exchanges;
using System;
using System.Collections.Generic;
using System.IO;

namespace Crawl_Data
{
    public class MyJob : IJob
    {
        public void Execute()
        {   //TODO
            //implement insert db in function RefreshCandleData
            // lay du lien nen 1phut
           // Program._dataRefresher.RefreshCandleData(Program.CoinsToBacktest, (x) => Program.WriteColoredLine(x, ConsoleColor.Green), Mynt.Core.Enums.Period.Minute).Wait();
            //lay gia cua tat ca cac coin tren 1 san
            //Program._dataRefresher.getAllPrice((x) => Program.WriteColoredLine(x, ConsoleColor.Green)).Wait();
            //lay volume
            //Program._dataRefresher.getAskBid(Program.CoinsToBacktest, (x) => Program.WriteColoredLine(x, ConsoleColor.Green)).Wait();
            for(int i =0;i<1000;i++){
            PersonService person = new PersonService();

person.GetById("asd");
Console.WriteLine(i);
            }
        }
    }

    public class TestJob01 : IJob
    {
        public void Execute()
        {
            Console.WriteLine("TestJob01");
        }
    }

    public class TestJob02 : IJob
    {
        public void Execute()
        {
            Console.WriteLine("TestJob02");
            // throw new Exception("Loi roi");
        }
    }

    public class ScheduledJobRegistry : Registry
    {
        public ScheduledJobRegistry()
        {
            var job = new MyJob { };
            JobManager.AddJob(job, s => s.WithName("MyJob").ToRunEvery(1).Seconds());

            // var job01 = new TestJob01 {};
            // var job02 = new TestJob02 {};
            // JobManager.AddJob(job01, s => s.WithName("TestJob01").ToRunOnceAt(DateTime.Now.AddSeconds(3)).AndEvery(1).Minutes());
            // JobManager.AddJob(job02, s => s.WithName("TestJob02").ToRunOnceAt(DateTime.Now.AddSeconds(3)).AndEvery(1).Minutes());
        }
    }

    class Program
    {

        public static IConfiguration Configuration { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }
        public static DataRefresher _dataRefresher;
        public static List<string> CoinsToBacktest = new List<string> { }; // The coins to use.

        static void Main(string[] args)
        {
            Init();
            WriteIntro();

            Console.WriteLine();
            Console.WriteLine();

            SetupDI();
            DatabaseInitializer();

            JobManager.Initialize(new ScheduledJobRegistry());
//JobManager.UseUtcTime();
            JobManager.JobException += info =>
            {
                Console.WriteLine($"Error occurred in job: {info.Name}, {info.Exception}");
            };
            JobManager.JobStart += info =>
            {
                Console.WriteLine($"Start job: {info.Name}. Duration: {info.StartTime}");
            };
            JobManager.JobEnd += info =>
            {
                Console.WriteLine($"End job: {info.Name}. Duration: {info.Duration}. NextRun: {info.NextRun}.");
                Console.WriteLine();
            };

            // Wait for something
            // WriteAt("Press enter to terminate...",0,, ConsoleColor.Red);

            Console.ReadLine();

            // Stop the scheduler
            JobManager.StopAndBlock();
        }

        private static void SetupDI()
        {
            ServiceProvider = new ServiceCollection()
                            .AddTransient<ApplicationContext>()
                            .AddSingleton<IPersonService, PersonService>()
                            .AddSingleton<ICandleService, CandleService>()
                            .AddTransient<IDatabaseInitializer, DatabaseInitializer>()
                            .AddSingleton<IConfiguration>(Configuration)
                            .BuildServiceProvider();
        }

        private static void DatabaseInitializer()
        {
            try
            {
                var databaseInitializer = ServiceProvider.GetRequiredService<IDatabaseInitializer>();
                databaseInitializer.SeedAsync().Wait();
            }
            catch (Exception)
            {
                Console.WriteLine("Lỗi rồi");
            }
        }

        public static void WriteColoredLine(string line, ConsoleColor color, bool padded = false)
        {
            Console.ForegroundColor = color;
            if (padded) Console.WriteLine();
            Console.WriteLine(line);
            if (padded) Console.WriteLine();
            Console.ResetColor();
        }

        public static void WriteAt(string s, int x, int y, ConsoleColor color, bool padded = false)
        {
            try
            {
                Console.SetCursorPosition(x, y);
                WriteColoredLine(s, color, padded);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        private static void Init()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true);
            Configuration = builder.Build();

            var exchangeOptions = Configuration.Get<ExchangeOptions>();
            var backtestOptions = Configuration.Get<BacktestOptions>();
            CoinsToBacktest = backtestOptions.Coins;
            _dataRefresher = new DataRefresher(exchangeOptions);
        }

        private static void WriteIntro()
        {
            Console.WriteLine();

            Console.WriteLine(@" $$$$$$\                                $$\                                  $$\   $$$$$$\ $$$$$$$$\ ");
            Console.WriteLine(@"$$  __$$\                               $$ |                               $$$$ | $$  __$$\\____$$  |");
            Console.WriteLine(@"$$ /  \__| $$$$$$\  $$\   $$\  $$$$$$\$$$$$$\   $$$$$$\   $$$$$$\   $$$$$$\\_$$ | \__/  $$ |   $$  / ");
            Console.WriteLine(@"$$ |      $$  __$$\ $$ |  $$ |$$  __$$\_$$  _| $$  __$$\ $$  __$$\ $$  __$$\ $$ |  $$$$$$  |  $$  /  ");
            Console.WriteLine(@"$$ |      $$ |  \__|$$ |  $$ |$$ /  $$ |$$ |   $$ /  $$ |$$$$$$$$ |$$ |  \__|$$ | $$  ____/  $$  /   ");
            Console.WriteLine(@"$$ |  $$\ $$ |      $$ |  $$ |$$ |  $$ |$$ |$$\$$ |  $$ |$$   ____|$$ |      $$ | $$ |      $$  /    ");
            Console.WriteLine(@"\$$$$$$  |$$ |      \$$$$$$$ |$$$$$$$  |\$$$$  \$$$$$$  |\$$$$$$$\ $$ |    $$$$$$\$$$$$$$$\$$  /     ");
            Console.WriteLine(@"\______/ \__|       \____$$ |$$  ____/  \____/ \______/  \_______|\__|    \______\________\__/      ");
            Console.WriteLine(@"                    $$\   $$ |$$ |                                                                   ");
            Console.WriteLine(@"                    \$$$$$$  |$$ |                                                                   ");
            Console.WriteLine(@"                     \______/ \__|                                                                   ");

        }
    }
}
