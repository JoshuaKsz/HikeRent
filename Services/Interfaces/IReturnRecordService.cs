using HikeRent.DTOs.ReturnRecord;
using Microsoft.AspNetCore.Mvc;

namespace HikeRent.Services.Interfaces
{
    public interface IReturnRecordService
    {
        Task<IEnumerable<ReturnRecordResponse>> GetAllAsync();

        Task<ReturnRecordResponse?> GetByIdAsync(int id);

        Task<ReturnRecordResponse> CreateAsync(ReturnRecordRequest request);


        //Task<ReturnRecordResponse?> UpdateNotesAsync(int id, string Notes);   

        Task<ReturnRecordResponse?> GetByRentalDetailIdAsync(int rentalDetailId);

        //Task<bool> CancelAsync(int id);

        //Task<RentalCustomerReturnRecordResponse?> GetReturnPageAsync(int id);

    }
}
