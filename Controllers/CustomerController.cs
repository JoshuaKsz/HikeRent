using HikeRent.DTOs.Customer;
using HikeRent.Models;
using HikeRent.Services.Implementations;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HikeRent.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            this._customerService = customerService;
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCustomers(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            //var customers = await this._customerService.GetAllAsync(search, page, limit);
            var customers = await this._customerService.GetAllAsync();
            return Ok(customers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await this._customerService.GetByIdAsync(id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer tidak ditemukan."
                });
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerRequest request)
        {
            try
            {
                var customer = await this._customerService.CreateAsync(request);

                //return ($"/api/customer/{created.CustomerId}", created);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = customer.CustomerId },
                    customer
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message,
                });
            }
        }

        //public async
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerRequest request)
        {
            try
            {
                var result = await this._customerService.UpdateAsync(id, request);
                if (result == null) return NotFound(new
                {
                    message = "Customer tidak ditemukan."
                });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message,
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await this._customerService.DeleteAsync(id);
                if (!success) return NotFound(new
                {
                    message = "Customer tidak ditemukan."
                });
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Customer tidak bisa didelete karena digunakan oleh data lain."
                });
            }
        }

    }


}
