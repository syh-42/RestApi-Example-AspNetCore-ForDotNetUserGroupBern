using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using RestApi.Model;
using Xunit;
using FluentAssertions;

namespace RestApi.IntegrationTest
{
    public class CustomerControllerTest : IClassFixture<TestServerFixture<Startup, SalesDbContext>>, IDisposable
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

            result.Should().HaveCount(10);
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

            result.First().Name.ShouldBeEquivalentTo(testName);
            result.Should().HaveCount(1);
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

        [Fact]
        public async void CreateCustomer_CreatesTheCustomer()
        {
            var fakeCustomer = GetFakeCustomers(1).First();

            await _testServerFixture.Client.PostAsync("/api/customer", fakeCustomer.ToJsonStringContent());

            _testServerFixture.DbContext.Customers.Should().HaveCount(1);
            _testServerFixture.DbContext.Customers.First().Name.ShouldBeEquivalentTo(fakeCustomer.Name);
        }

        [Fact]
        public async void UpdateCustomer_UpdatesTheCustomer()
        {
            var fakeCustomer = GetFakeCustomers(1).First();
            _testServerFixture.DbContext.Customers.Add(fakeCustomer);
            _testServerFixture.DbContext.SaveChanges();

            fakeCustomer.Name = Guid.NewGuid().ToString();

            await _testServerFixture.Client.PutAsync("/api/customer", fakeCustomer.ToJsonStringContent());

            _testServerFixture.DbContext.Customers.First().Name.ShouldBeEquivalentTo(fakeCustomer.Name);
        }

        [Fact]
        public async void DeleteCustomer_DeletesTheCustomer()
        {
            var fakeCustomer = GetFakeCustomers(1).First();
            _testServerFixture.DbContext.Customers.Add(fakeCustomer);
            _testServerFixture.DbContext.SaveChanges();
            var customer = _testServerFixture.DbContext.Customers.First();

            await _testServerFixture.Client.DeleteAsync($"/api/customer/{customer.Id}");

            _testServerFixture.DbContext.Customers.Should().HaveCount(0);
        }

        private List<Customer> GetFakeCustomers(int amount)
        {
            return new Faker<Customer>()
                .RuleFor(u => u.Name, f => f.Name.LastName())
                .RuleFor(u => u.Vorname, f => f.Name.FirstName())
                .Generate(amount).ToList();
        }

        public void Dispose()
        {
            _testServerFixture.DbContext.Database.EnsureDeleted();
        }
    }
}