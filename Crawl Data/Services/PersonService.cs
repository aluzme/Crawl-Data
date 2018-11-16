using System.Collections.Generic;
using System.Linq;
using Crawl_Data.Models;

namespace Crawl_Data.Services
{
    public class PersonService : IPersonService
    {
        private readonly ApplicationContext _context;

        public PersonService(ApplicationContext context) {
            _context = context;
        }

        public List<Person> GetAll()
        {
            return _context.Person.ToList();
        }
    }
}