using System.Collections.Generic;
using RestApi.Model;

namespace RestApi.Swagger
{
    public class CustomerExamples : IProvideExamples
    {
        public object GetExamples()
        {
            var customer1 = new Customer
            {
                Id = 1,
                Vorname = "Hans",
                Name = "Muster",
                Adresse = "Haupstrasse 2",
                Plz = 5000,
                Ort = "Aarau"
            };

            var customer2 = new Customer
            {
                Id = 2,
                Vorname = "John",
                Name = "Doe",
                Adresse = "Hauptgasse 2",
                Plz = 3000,
                Ort = "Bern"
            };

            return new[] {customer1, customer2};
        }
    }
}