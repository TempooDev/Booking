using System;

namespace Booking.Core.Users.Domain.Entities
{
    public class Buyer : User
    {
        public string? PreferredPaymentMethod { get; set; }
    }
}