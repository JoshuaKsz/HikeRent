using HikeRent.DTOs.Category;
using HikeRent.DTOs.Customer;
using HikeRent.Models;
using HikeRent.Repositories.Implementations;
using HikeRent.Repositories.Interfaces;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;


namespace HikeRent.Services.Implementations
{
    public class CustomerService: ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork)
        {
            this._customerRepository = customerRepository;
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
        {
            IEnumerable<Customer> customers = await this._customerRepository.GetAllAsync();

            return customers.Select(customer => new CustomerResponse
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                IdentityNumber = customer.IdentityNumber,
                CreatedAt = customer.CreatedAt
            });
        }
        public async Task<CustomerResponse?> GetByIdAsync(int id)
        {
            Customer? customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                return null;
            }

            return new CustomerResponse
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                IdentityNumber = customer.IdentityNumber,
                CreatedAt = customer.CreatedAt
            };
        }

        public async Task<CustomerResponse?> CreateAsync(CustomerRequest request)
        {
            string identityNumber = request.IdentityNumber.Trim();

            bool existingIdentity = await this._customerRepository.IdentityNumberExistsAsync(identityNumber);

            if (existingIdentity)
            {
                throw new InvalidOperationException("NIK sudah digunakan.");
            }

            Customer customer = new Customer
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                IdentityNumber = identityNumber,
                //CreatedAt = DateTime.Now,
                CreatedAt = DateTime.UtcNow,
            };

            await this._customerRepository.AddAsync(customer);

            await this._unitOfWork.SaveChangesAsync();

            return new CustomerResponse
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                IdentityNumber = customer.IdentityNumber,
                CreatedAt = customer.CreatedAt
            };
        }

        public async Task<CustomerResponse?> UpdateAsync(int id, CustomerRequest request)
        {
            Customer? customer = await this._customerRepository.GetByIdAsync(id);
            
            if (customer == null)
            {
                return null;
            }

            string identityNumber = request.IdentityNumber.Trim();

            Customer? existingCustomer = await this._customerRepository.GetByNameAsync(identityNumber);

            if (existingCustomer != null && existingCustomer.CustomerId != id)
            {
                throw new InvalidOperationException("NIK sudah digunakan.");
            }


            customer.FullName = request.FullName;
            customer.Email = request.Email;
            customer.PhoneNumber = request.PhoneNumber;
            customer.Address = request.Address;
            customer.IdentityNumber = identityNumber;
            //customer.CreatedAt = DateTime.Now;

            this._customerRepository.Update(customer);

            await this._unitOfWork.SaveChangesAsync();

            return new CustomerResponse
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                IdentityNumber = customer.IdentityNumber,
                CreatedAt = customer.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Customer? customer = await this._customerRepository.GetByIdAsync(id);

            if (customer == null) { return false; }

            this._customerRepository.Delete(customer);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
