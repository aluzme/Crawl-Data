using System;
using System.Collections.Generic;
using System.Linq;
using Crawl_Data.Models;

namespace Crawl_Data.Services
{
    public class PersonService : IPersonService
    {
        private readonly ApplicationContext _context;

        public PersonService() {
            _context = new ApplicationContext();
        }

        public List<Person> GetAll()
        {
            return _context.Person.ToList();
        }

        public void GetById(string id){
           List<Parameter> para = new List<Parameter>();
            para.Add(new Parameter("@countryName", id));
            string result = _context.select("GetOfficeByCountry", para);
            Console.WriteLine(result);
        }
    }
}