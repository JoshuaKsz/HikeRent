using HikeRent.Exceptions;
using HikeRent.Services.Interfaces;

namespace HikeRent.Services.Implementations
{
    public class PricingService : IPricingService
    {
        private static readonly TimeZoneInfo JakartaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");

        public int CalculateRentalDays(DateTime localTransactionTime, DateOnly plannedStartDate, DateOnly plannedReturnDate)
        {
            DateOnly today = DateOnly.FromDateTime(localTransactionTime);

            if (plannedReturnDate < plannedStartDate)
            {
                throw new BusinessException(
                    "Tanggal pengembalian tidak boleh sebelum tanggal mulai sewa.");
            }

            // Jika pengembalian di hari yang sama (sebelum maupun sesudah cut-off) selalu 1 hari
            if (plannedStartDate == today && plannedReturnDate == today)
            {
                return 1;
            }

            // Aturan cut-off jam 12.00 WIB: jika transaksi >= jam 12.00, 
            // hitungan biaya dimulai dari hari berikutnya jika plannedStartDate adalah hari ini.
            DateOnly effectiveStartDate = plannedStartDate;
            if (localTransactionTime.TimeOfDay >= new TimeSpan(12, 0, 0) && plannedStartDate == today)
            {
                effectiveStartDate = today.AddDays(1);
            }

            // Durasi minimal 1 hari, termasuk tanggal pengembalian
            int baseDays = plannedReturnDate.DayNumber - effectiveStartDate.DayNumber + 1;
            int calculatedDays = Math.Max(1, baseDays);
            
            return calculatedDays;
        }

        public decimal CalculateSubtotal(int quantity, decimal dailyPrice, int rentalDays)
        {
            decimal subtotal = quantity * dailyPrice * rentalDays;
            return Math.Round(subtotal, 2);
        }
    }
}
