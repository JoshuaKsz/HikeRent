using Microsoft.AspNetCore.Mvc;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HikeRent.DTOs.Customer;


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


        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerRequest request)
        {
            var created = await this._customerService.CreateAsync(request);
            return Created($"/api/theaters/{created.CustomerId}", created);
        }
    }


}
