using PRN_HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PRN_HotelManagement
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        private FuminiHotelManagementContext _context;
        public Dashboard()
        {
            InitializeComponent();
            LoadTotalCustomers();
            LoadRevenue();
            LoadRoom();
        }
        private void LoadTotalCustomers()
        {
            int totalCustomers = 0;
            string cusName = "";
            using (_context = new FuminiHotelManagementContext())
            {
                totalCustomers = _context.Customers.Count();
                var topCustomer = _context.BookingReservations
                                    .GroupBy(br => br.CustomerId)
                                    .Select(group => new
                                    {
                                        CustomerId = group.Key,
                                        BookingCount = group.Count()
                                    })
                                    .OrderByDescending(x => x.BookingCount)
                                    .FirstOrDefault();

                if (topCustomer != null)
                {
                    cusName = _context.Customers.FirstOrDefault(c => c.CustomerId == topCustomer.CustomerId).CustomerFullName;

                }
            }
            customerHasMostBooking.Text = $"Customer book most room: {cusName}";
            totalCustomersTextBlock.Text = $"Total customer: {totalCustomers.ToString()}";
        }
        private void LoadRevenue()
        {
            decimal? totalRevenue = 0;
            int totalBooking = 0;
            using (_context = new FuminiHotelManagementContext())
            {
                var month = DateTime.Now.Month;
                Debug.WriteLine("*****" + _context.BookingReservations.Count());

                totalRevenue = _context.BookingReservations
                    .Where(x => x.BookingDate.Value.Month == month && x.BookingDate.Value.Year == DateTime.Now.Year)
                    .Sum(x => x.TotalPrice);

                totalBooking = _context.BookingReservations.Where(x => x.BookingDate.Value.Month == month && x.BookingDate.Value.Year == DateTime.Now.Year).Count();
            }
            totalRevenueThisMonth.Text = $"Total revenue this month: {totalRevenue?.ToString("N0")}$";
            totalBookingThisMonth.Text = $"Total booking this month: {totalBooking}";

        }

        private void LoadRoom()
        {
            int total = 0;
            decimal? maxPrice = 0;
            decimal? minPrice = 0;
            using (_context = new FuminiHotelManagementContext())
            {
                total = _context.RoomInformations.Count();
                maxPrice = _context.RoomInformations.Max(r => r.RoomPricePerDay);
                minPrice = _context.RoomInformations.Min(r => r.RoomPricePerDay);
            }

            totalRoom.Text = $"Total room: {total}";
            min.Text = $"Min price: {minPrice?.ToString("N0")}$";
            max.Text = $"Max price: {maxPrice?.ToString("N0")}$";
        }
    }
}
