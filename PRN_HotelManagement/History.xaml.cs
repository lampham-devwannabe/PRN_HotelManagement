using Microsoft.EntityFrameworkCore;
using PRN_HotelManagement.Model;
using PRN_HotelManagement;
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
    /// Interaction logic for History.xaml
    /// </summary>

    public partial class History : UserControl
    {
        private readonly int _userId;
        private FuminiHotelManagementContext _context;
        public ObservableCollection<reservation> Bookings { get; set; } = new ObservableCollection<reservation>();
        public History(int userId)
        {
            InitializeComponent();
            _userId = userId;
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
                        if (booking.BookingReservation.Customer.CustomerId == _userId)
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
