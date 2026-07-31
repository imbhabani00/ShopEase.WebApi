namespace ShopEase.Application.DTOs.Response.Customer
{
    public class CustomerResponse
    {
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }

    public class CustomerResponseList
    {
        public List<CustomerResponse> Customers { get; set; }
        public int TotalCount { get; set; }
        public int ReturnValue { get; set; }
    }
}