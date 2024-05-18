using System;
using System.Collections.Generic;

#nullable disable

namespace FB_Booking.DAL.Models
{
    public partial class FootballPitch
    {
        public FootballPitch()
        {
            Bookings = new HashSet<Booking>();
        }

        public int PitchId { get; set; }
        public string PitchName { get; set; }
        public string Location { get; set; }
        public string Size { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
