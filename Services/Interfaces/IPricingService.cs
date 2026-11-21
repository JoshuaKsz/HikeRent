using HikeRent.DTOs.Rentals;

namespace HikeRent.Services.Interfaces
{
    public interface IPricingService
    {
        int CalculateRentalDays(DateTime localTransactionTime, DateOnly plannedStartDate, DateOnly plannedReturnDate);
        
        decimal CalculateSubtotal(int quantity, decimal dailyPrice, int rentalDays);
    }
}
