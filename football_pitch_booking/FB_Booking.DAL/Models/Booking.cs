using System;
using System.Collections.Generic;

#nullable disable

namespace FB_Booking.DAL.Models
{
    public partial class Booking
    {
        public Booking()
        {
            Bills = new HashSet<Bill>();
        }

        public int BookingId { get; set; }
        public int? UserId { get; set; }
        public int? PitchId { get; set; }
        public DateTime TimeStart { get; set; }
        public double HoursToRent { get; set; }

        public virtual FootballPitch Pitch { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
    }
}
