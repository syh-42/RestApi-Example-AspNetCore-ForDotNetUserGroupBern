using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using RestApi.Model;
using Xunit;
using FluentAssertions;

namespace RestApi.IntegrationTest
{
    public class CustomerControllerTest : IClassFixture<TestServerFixture<Startup, SalesDbContext>>
    {
        private readonly TestServerFixture<Startup, SalesDbContext> _testServerFixture;

        public CustomerControllerTest(TestServerFixture<Startup, SalesDbContext> testServerFixture)
        {
            _testServerFixture = testServerFixture;
        }

        [Fact]
        public async void GetCustomers_WithoutFilter_ReturnsAllCustomers()
        {
            var fakeCustomers = GetFakeCustomers(10);
            _testServerFixture.DbContext.Customers.AddRange(fakeCustomers);
            _testServerFixture.DbContext.SaveChanges();

            var response = await _testServerFixture.Client.GetAsync("/api/customer");
            var result = await response.Content.ReadAsJsonAsync<List<Customer>>();
            var totalCount = _testServerFixture.DbContext.Customers.Count();

            result.Should().HaveCount(totalCount);
        }

        [Fact]
        public async void GetCustomers_WithFilter_ReturnsFilteredCustomers()
        {
            var testName = Guid.NewGuid().ToString();
            var fakeCustomers = GetFakeCustomers(10);
            fakeCustomers.First().Name = testName;
            _testServerFixture.DbContext.Customers.AddRange(fakeCustomers);
            _testServerFixture.DbContext.SaveChanges();

            var response = await _testServerFixture.Client.GetAsync($"/api/customer?name={testName}");
            var result = await response.Content.ReadAsJsonAsync<List<Customer>>();

            result.FirstOrDefault(el => el.Name == testName).Should().NotBeNull();
        }

        [Fact]
        public async void GetCustomers_ById_ReturnsTheCustomer()
        {
            var fakeCustomer = GetFakeCustomers(1).First();
            _testServerFixture.DbContext.Customers.Add(fakeCustomer);
            _testServerFixture.DbContext.SaveChanges();
            var customer = _testServerFixture.DbContext.Customers.First();

            var response = await _testServerFixture.Client.GetAsync($"/api/customer/{customer.Id}");
            var result = await response.Content.ReadAsJsonAsync<Customer>();

            result.Id.ShouldBeEquivalentTo(customer.Id);
        }


        private List<Customer> GetFakeCustomers(int amount)
        {
            return new Faker<Customer>()
                .RuleFor(u => u.Name, f => f.Name.LastName())
                .RuleFor(u => u.Vorname, f => f.Name.FirstName())
                .Generate(amount).ToList();
        }

        [Fact]
        public async void CreateCustomer_CreatesTheCustomer()
        {
            var fakeCustomer = GetFakeCustomers(1).First();

            var response = await _testServerFixture.Client.PostAsync("/api/customer", fakeCustomer.ToJsonStringContent());
            var result = await response.Content.ReadAsJsonAsync<Customer>();

            result.Should().NotBeNull();
            result.Name.Should().BeEquivalentTo(fakeCustomer.Name);
        }

        [Fact]
        public async void UpdateCustomer_UpdatesTheCustomer()
        {
            var fakeCustomer = GetFakeCustomers(1).First();
            _testServerFixture.DbContext.Customers.Add(fakeCustomer);
            _testServerFixture.DbContext.SaveChanges();

            fakeCustomer.Name = Guid.NewGuid().ToString();

            await _testServerFixture.Client.PutAsync($"/api/customer/{fakeCustomer.Id}", fakeCustomer.ToJsonStringContent());

            _testServerFixture.DbContext.Customers.First(el => el.Id == fakeCustomer.Id).Name.ShouldBeEquivalentTo(fakeCustomer.Name);
        }

        [Fact]
        public async void DeleteCustomer_DeletesTheCustomer()
        {
            var fakeCustomer = GetFakeCustomers(1).First();
            _testServerFixture.DbContext.Customers.Add(fakeCustomer);
            _testServerFixture.DbContext.SaveChanges();
            var customer = _testServerFixture.DbContext.Customers.First();

            await _testServerFixture.Client.DeleteAsync($"/api/customer/{customer.Id}");

            _testServerFixture.DbContext.Customers.FirstOrDefault(el => el.Id == customer.Id).Should().BeNull();
        }
    }
}