using System;

namespace Booking.Core.Users.Domain.Entities
{
    public class Seller : User
    {
        public string? StoreName { get; set; }
        public double? Rating { get; set; }
    }
}