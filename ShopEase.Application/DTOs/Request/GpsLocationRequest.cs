namespace Ecommerce.Application.DTOs.Request
{
    public class GpsLocationRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int OrderId { get; set; }
    }
}
