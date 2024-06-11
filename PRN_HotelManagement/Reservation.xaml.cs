using Microsoft.EntityFrameworkCore;
using PRN_HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Reservation.xaml
    /// </summary>
    public struct reservation
    {
        public int BookingReservationId;
        public int RoomId { get; set; }
        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }
        public decimal? ActualPrice { get; set; }
        public string CustomerName { get; set; }

    }
    public partial class Reservation : UserControl
    {
        private FuminiHotelManagementContext _context;

        public ObservableCollection<reservation> Bookings { get; set; } = new ObservableCollection<reservation>();
        public Reservation()
        {
            InitializeComponent();
            LoadReservation();
            DataContext = this;
        }

        private void LoadReservation()
        {
            try
            {
                using (_context = new FuminiHotelManagementContext())
                {
                    Bookings.Clear();
                    var bookings = _context.BookingDetails.Include(b => b.BookingReservation).ThenInclude(b => b.Customer).ToList();
                    foreach (var booking in bookings)
                    {
                        Bookings.Add(new reservation
                        {
                            BookingReservationId = booking.BookingReservationId,
                            RoomId = booking.RoomId,
                            StartDate = booking.StartDate,
                            EndDate = booking.EndDate,
                            ActualPrice = booking.ActualPrice,
                            CustomerName = booking.BookingReservation.Customer.CustomerFullName
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ResetInputFields();
        }

        private void ResetInputFields()
        {
            txtEnd.Text = "";
            txtPrice.Text = "";
            txtRoomId.Text = "";
            txtStart.Text = "";
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is reservation selectedReservation)
            {
                txtEnd.Text = selectedReservation.EndDate.ToString();
                txtPrice.Text = selectedReservation.ActualPrice.ToString();
                txtRoomId.Text = selectedReservation.RoomId.ToString();
                txtStart.Text = selectedReservation.StartDate.ToString();
            }
            else
            {
                ResetInputFields();
            }
        }
    }
}
