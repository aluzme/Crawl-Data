using FluentScheduler;
using Microsoft.Extensions.Configuration;
using Mynt.Core.Exchanges;
using System;
using System.Collections.Generic;
using System.IO;

namespace Crawl_Data
{
    public class MyJob : IJob
    {
        public String name { get; set; }

        public int count { get; set; }

        public void Execute()
        {   //TODO
            //implement insert db in function RefreshCandleData
            //Program._dataRefresher.RefreshCandleData(Program.CoinsToBacktest, (x) => Program.WriteColoredLine(x, ConsoleColor.Green), Mynt.Core.Enums.Period.Minute).Wait();
            Program._dataRefresher.getAllPrice((x) => Program.WriteColoredLine(x, ConsoleColor.Green)).Wait();
        }
    }

    public class ScheduledJobRegistry : Registry
    {
        public ScheduledJobRegistry()
        {
             var job = new MyJob { };
             JobManager.AddJob(job, s => s.ToRunOnceAt(DateTime.Now.AddSeconds(3)).AndEvery(1).Minutes());
         
        }
    }

    class Program
    {

        public static IConfiguration Configuration { get; set; }
        public static DataRefresher _dataRefresher;
        public static List<string> CoinsToBacktest = new List<string> { }; // The coins to use.

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            Init();
            WriteIntro();
            Console.WriteLine();
            Console.WriteLine();
            
            JobManager.Initialize(new ScheduledJobRegistry());

            // Wait for something
            //WriteAt("Press enter to terminate...",0,, ConsoleColor.Red);

            Console.ReadLine();

            // Stop the scheduler
            JobManager.StopAndBlock();
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
                Console.SetCursorPosition(x,y);
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
