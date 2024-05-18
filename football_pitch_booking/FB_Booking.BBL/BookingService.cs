using FB_Booking.DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FB_Booking.BBL
{
    public class BookingService
    {
        private readonly FootballPitchBookingContext _dbContext;
        private readonly IConfiguration _configuration;


        public BookingService(FootballPitchBookingContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<bool> CheckAndCreateBooking(int userId, int pitchId, DateTime timeStart, float hoursToRent, string paymentMethod, string paymentState)
        {
            var endTime = timeStart.AddHours(hoursToRent);
            var cost = hoursToRent * 100;  // Assuming the cost is 100 per hour
            var existingBooking = await _dbContext.Bookings
                .AnyAsync(b => b.PitchId == pitchId && timeStart < b.TimeStart.AddHours(b.HoursToRent) && endTime > b.TimeStart);
            
            if (!existingBooking)
            {
                var booking = new Booking
                {
                    UserId = userId,
                    PitchId = pitchId,
                    TimeStart = timeStart,
                    HoursToRent = hoursToRent
                };
                await _dbContext.Bookings.AddAsync(booking);
                await _dbContext.SaveChangesAsync();

                var bill = new Bill
                {
                    BookingId = booking.BookingId,
                    Cost = (decimal)cost,
                    PaymentMethod = paymentMethod,
                    PaymentState = paymentState
                };
                await _dbContext.Bills.AddAsync(bill);
                await _dbContext.SaveChangesAsync();

                return true;
            }

            return false;

        }

        //  get all user's bookings by user id
        public async Task<List<get_booking>> get_by_userID(int userId)
        {
            //var bookings = await _dbContext.Bookings
            //    .Where(b => b.UserId == userId)
            //    .OrderBy(b => b.TimeStart)
            //    .ToListAsync();
            var bookings = await _dbContext.Bookings
                .Where(b => b.UserId == userId)
                .Join(
                    _dbContext.Bills,
                    booking => booking.BookingId,
                    bill => bill.BookingId,
                    (booking, bill) => new get_booking
                    {
                        bookingID = booking.BookingId,
                        UserId = booking.UserId,
                        PitchId = booking.PitchId,
                        TimeStart = booking.TimeStart,
                        HoursToRent = booking.HoursToRent,
                        Cost = bill.Cost,
                        PaymentMethod = bill.PaymentMethod,
                        PaymentState = bill.PaymentState
                    }
                )
                .ToListAsync();
            return bookings;
        }

        //  delete a booking by booking id
        public async Task<Boolean> delete(int bookingID)
        {
            try
            {
                var booking = await _dbContext.Bookings.FindAsync(bookingID);
                //  check if the booking exists
                if (booking == null)
                {
                    return false;
                }

                var bill = await _dbContext.Bills.FirstOrDefaultAsync(b => b.BookingId == bookingID);
                //  check if the booking exists
                if (bill != null)
                {
                    _dbContext.Bills.Remove(bill);
                    await _dbContext.SaveChangesAsync();
                }
                _dbContext.Bookings.Remove(booking);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }

    public class get_booking
    {
        public int bookingID { get; set; }
        public int? UserId { get; set; }
        public int? PitchId { get; set; }
        public DateTime TimeStart { get; set; }
        public double HoursToRent { get; set; }
        public decimal Cost { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentState { get; set; }
    }
}
