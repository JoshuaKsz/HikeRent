namespace HikeRent.DTOs.Customer
{
    public class CustomerResponse
    {
        public long CustomerId { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string IdentityNumber { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }

    }
}
