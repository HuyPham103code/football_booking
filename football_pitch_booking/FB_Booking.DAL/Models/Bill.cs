using System;
using System.Collections.Generic;

#nullable disable

namespace FB_Booking.DAL.Models
{
    public partial class Bill
    {
        public int BillId { get; set; }
        public int? BookingId { get; set; }
        public decimal Cost { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentState { get; set; }

        public virtual Booking Booking { get; set; }
    }
}
