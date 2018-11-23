using System.Collections.Generic;
using Crawl_Data.Models;
using Mynt.Core.Models;

namespace Crawl_Data.Services
{
    public interface ICandleService
    {
        List<Candle> GetAll();

    }
}