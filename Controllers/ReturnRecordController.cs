using HikeRent.DTOs.Category;
using HikeRent.DTOs.Customer;
using HikeRent.DTOs.ReturnRecord;
using HikeRent.Models;
using HikeRent.Services.Implementations;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HikeRent.Controllers
{
    [ApiController]
    [Route("api/ReturnDetail/")]
    //[Authorize]
    public class ReturnRecordController : ControllerBase
    {
        private readonly IReturnRecordService _returnRecordService;

        public ReturnRecordController(IReturnRecordService returnRecordService)
        {
            this._returnRecordService = returnRecordService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<ReturnRecordResponse> returnRecords = await this._returnRecordService.GetAllAsync();
            return Ok(returnRecords);
        }

        //GET: api/return/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdWithDetailsForUpdateAsync(int id)
        {
            var result = await this._returnRecordService.GetByRentalDetailIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Rental Detail tidak ditemukan."
                });
            }

            return Ok(new
            {
                status = "success",
                message = "Data Rental Detail berhasil ditemukan.",
                data = result
            });

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReturnRecordRequest request)
        {
            try
            {
                var returnRecord = await _returnRecordService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = returnRecord.ReturnRecordId },
                    returnRecord
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                });
            }
        }


        [HttpGet("return-record/{id}", Name = "GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _returnRecordService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Return Record tidak ditemukan."
                });
            }

            return Ok(result);
        }
    }
}
