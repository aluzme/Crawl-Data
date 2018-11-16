using System.Collections.Generic;
using Crawl_Data.Models;

namespace Crawl_Data.Services
{
    public interface IPersonService
    {
        List<Person> GetAll();
    }
}