using System.Collections.Generic;
using System.Linq;
using Crawl_Data.Models;
using Mynt.Core.Models;

namespace Crawl_Data.Services
{
    public class CandleService : ICandleService
    {
        private readonly ApplicationContext _context;

        public CandleService(ApplicationContext context) {
            _context = context;
        }

        public List<Candle> GetAll()
        {
            return _context.Candle.ToList();
        }
    }
}