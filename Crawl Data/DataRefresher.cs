using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mynt.Core.Enums;
using Mynt.Core.Exchanges;
using Mynt.Core.Models;
using Newtonsoft.Json;

namespace Crawl_Data
{
    internal class DataRefresher
    {
        private readonly ExchangeOptions _exchangeOptions;
        private readonly BaseExchange _api;

        public DataRefresher(ExchangeOptions exchangeOptions)
        {
            _exchangeOptions = exchangeOptions;
            _api = new BaseExchange(_exchangeOptions);
        }

        private string GetDataDirectory()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(basePath, "data");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public bool CheckForCandleData()
        {
            return Directory.GetFiles(GetDataDirectory(), "*.json", SearchOption.TopDirectoryOnly).Count() != 0;
        }

        private string GetJsonFilePath(string pair)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(GetDataDirectory(), $"{pair}.json");
        }

        public async Task RefreshCandleData(List<string> coinsToRefresh, Action<string> callback, Period period)
        {
            List<string> writtenFiles = new List<string>();

            foreach (var coinToBuy in coinsToRefresh)
            {
               
                var jsonPath = GetJsonFilePath(coinToBuy);

                // Delete an existing file.
                if (File.Exists(jsonPath)) File.Delete(jsonPath);
                var endDate = DateTime.UtcNow;
                var startDate = DateTime.UtcNow.AddMinutes(-15);
                var totalCandles = new List<Candle>();
                var candles = await _api.GetTickerHistory(coinToBuy, period, startDate, endDate);


                // Add the last bit in...
                totalCandles.AddRange(candles);
                totalCandles = totalCandles.OrderBy(x => x.Timestamp).ToList();

                // Write all the text.
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(totalCandles));
                writtenFiles.Add(jsonPath);

                callback($"{DateTime.UtcNow}: Refreshed data for {coinToBuy}...");
            }

            // Delete everything that's not refreshed
            foreach (FileInfo fi in new DirectoryInfo(GetDataDirectory()).EnumerateFiles())
            {
                if (!writtenFiles.Contains(fi.FullName))
                    File.Delete(fi.FullName);
            }
        }

        public async Task getAllPrice(Action<string> callback)
        {
            List<string> writtenFiles = new List<string>();


            var basePath = AppDomain.CurrentDomain.BaseDirectory;            
            var jsonPath = Path.Combine(GetDataDirectory(), $"summary.json");

            // Delete an existing file.
            if (File.Exists(jsonPath)) File.Delete(jsonPath);
                var totalMarketSummary = new List<MarketSummary>();
                var marketSummary = await _api.GetMarketSummaries();


                // Add the last bit in...
                totalMarketSummary.AddRange(marketSummary);
                //totalCandles = totalCandles.OrderBy(x => x.Timestamp).ToList();

                // Write all the text.
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(totalMarketSummary));
                writtenFiles.Add(jsonPath);

                callback($"{DateTime.UtcNow}: Refreshed data for ...");
            

            // Delete everything that's not refreshed
            foreach (FileInfo fi in new DirectoryInfo(GetDataDirectory()).EnumerateFiles())
            {
                if (!writtenFiles.Contains(fi.FullName))
                    File.Delete(fi.FullName);
            }
        }

        public async Task getAskBid(List<string> coinsToRefresh, Action<string> callback)
        {
            List<string> writtenFiles = new List<string>();

            foreach (var coinToBuy in coinsToRefresh)
            {

                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var jsonPath = Path.Combine(GetDataDirectory(), $"{coinToBuy}volume.json");

                // Delete an existing file.
                if (File.Exists(jsonPath)) File.Delete(jsonPath);
                var orderBook = await _api.GetOrderBook(coinToBuy);
                var ask = orderBook.Asks;
                var bid = orderBook.Bids;

                // Add the last bit in...
                //totalCandles.AddRange(candles);
               // totalCandles = totalCandles.OrderBy(x => x.Timestamp).ToList();

                // Write all the text.
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(ask));
                writtenFiles.Add(jsonPath);

                callback($"{DateTime.UtcNow}: Refreshed data for {coinToBuy}...");
            }

            // Delete everything that's not refreshed
            foreach (FileInfo fi in new DirectoryInfo(GetDataDirectory()).EnumerateFiles())
            {
                if (!writtenFiles.Contains(fi.FullName))
                    File.Delete(fi.FullName);
            }
        }

        private TimeSpan GetCacheAge()
        {
            string dataFolder = Path.GetDirectoryName(GetJsonFilePath("dummy-dummy"));

            if (Directory.GetFiles(dataFolder).Length == 0)
                return TimeSpan.MinValue;

            var fileInfo = new DirectoryInfo(dataFolder).GetFileSystemInfos()
                                                        .OrderBy(fi => fi.CreationTime)
                                                        .First();

            return DateTime.Now - fileInfo.LastWriteTime;
        }
        //////////////////////////////////

        public async Task<OrderBook> getOrderDetail(String name)
        {
            OrderBook orderBook = await _api.GetOrderBook(name);
            foreach (var ask in orderBook.Asks)
            {
                Console.WriteLine(ask.Quantity + "--" + ask.Price);
            }
            Console.WriteLine(orderBook.Asks);
            //Console.WriteLine(orderBook.Asks);
            return orderBook;
        }
    }
}
