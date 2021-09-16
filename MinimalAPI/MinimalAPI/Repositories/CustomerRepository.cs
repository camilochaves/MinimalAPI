using MinimalAPI.Repositories;
using MinimalAPI.Models;

namespace MinimalAPI.Repositories
{
    internal class CustomerRepository
    {
        private Dictionary<Guid, Customer> _customers = new();

        public void Create(Customer customer)
        {
            if (customer is null) return;
            _customers[customer.Id] = customer;
        }

        public Customer GetById(Guid Id)
        {
            var customer = _customers.FirstOrDefault(x => x.Value.Id == Id).Value;
            if (customer is null) return null;
            return customer;
        }

        public List<Customer> GetAll() => _customers.Values.ToList();
        public void Update(Customer customer) => _customers[customer.Id] = customer;
        public void Delete(Guid Id) => _customers.Remove(Id);


    }
}
