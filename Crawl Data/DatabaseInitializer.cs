using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crawl_Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Crawl_Data
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync();
    }
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationContext _context;
        public DatabaseInitializer(ApplicationContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync().ConfigureAwait(false);

            CreatePerson();
        }

        private void CreatePerson()
        {
            var peopleToAdd = new List<Person>(){
                new Person { Name = "Nguyen Minh Dung" }
            };
            foreach (var person in peopleToAdd)
            {
                var personExists = _context.Person.SingleOrDefault(e => e.Name == person.Name);

                if (personExists == null)
                {
                    _context.Person.Add(person);
                }
            }

            _context.SaveChanges();
        }
    }
}