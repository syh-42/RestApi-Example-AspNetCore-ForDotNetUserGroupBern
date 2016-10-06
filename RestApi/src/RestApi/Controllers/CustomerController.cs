using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RestApi.Model;
using System.Linq;
using RestApi.Swagger;

namespace RestApi.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly SalesDbContext _salesDbContext;

        public CustomerController(SalesDbContext salesDbContext)
        {
            _salesDbContext = salesDbContext;
        }


        /// <summary>
        /// Holt sämtliche Kunden. Kann gefiltert werden nach Name, Vorname, PLZ und Ort
        /// </summary>
        /// <param name="name">Filtert den Namen</param>
        /// <param name="vorname">Filtert den Vornamen</param>
        /// <param name="plz">Filtert nach der PLZ</param>
        /// <param name="ort">Filtert nach dem Ort</param>
        /// <returns>Kunden</returns>
        [HttpGet]
        [SwaggerResponseExamples(typeof(IEnumerable<Customer>), typeof(CustomerExamples))]
        public IEnumerable<Customer> Get([FromQuery]string name, [FromQuery]string vorname, [FromQuery]int plz, [FromQuery]string ort)
        {
            var query = from cus in _salesDbContext.Customers
                            where
                                (string.IsNullOrEmpty(name) || cus.Name.Contains(name))
                                && (string.IsNullOrEmpty(vorname) || cus.Vorname.Contains(vorname))
                                && (plz == 0 || cus.Plz == plz)
                                && (string.IsNullOrEmpty(ort) || cus.Ort.Contains(ort))
                            select cus;

            return query.ToList();
        }

        [HttpGet]
        [Route("getByOrt/{ort}")]

        public IEnumerable<Customer> GetByOrt([FromRoute]string ort)
        {
            var query = from cus in _salesDbContext.Customers
                        where
                            (string.IsNullOrEmpty(ort) || cus.Ort.Contains(ort))
                        select cus;

            return query.ToList();
        }


        [HttpGet("{id}")]
        public Customer Get(long id)
        {
            return _salesDbContext.Customers.Single(cus => cus.Id == id);
        }

        [HttpPost]
        public Customer Post([FromBody]Customer customer)
        {
            _salesDbContext.Customers.Add(customer);
            _salesDbContext.SaveChanges();

            return customer;
        }

        [HttpPut("{id}")]
        public void Put(long id, [FromBody]Customer customer)
        {
            var original = _salesDbContext.Customers.Single(cus => cus.Id == id);
            original.Name = customer.Name;
            original.Vorname = customer.Vorname;
            original.Adresse = customer.Adresse;
            original.Plz = customer.Plz;
            original.Ort = customer.Ort;

            _salesDbContext.SaveChanges();
        }

        [HttpDelete("{id}")]
        public void Delete(long id)
        {
            var entityToDelete = _salesDbContext.Customers.FirstOrDefault(cus => cus.Id == id);
            _salesDbContext.Customers.Remove(entityToDelete);
            _salesDbContext.SaveChanges();
        }
    }
}