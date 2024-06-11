using Microsoft.IdentityModel.Tokens;
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
    /// Interaction logic for Booking.xaml
    /// </summary>
    public struct booking
    {
        public string RoomNumber { get; set; }
        public DateOnly Start { get; set; }
        public DateOnly End { get; set; }
        public decimal Price { get; set; }

    }
    public partial class Booking : UserControl
    {
        private FuminiHotelManagementContext _context;

        private readonly int _userId;

        public ObservableCollection<booking> bookInfo { get; set; } = new ObservableCollection<booking>();

        public Booking(int userId)
        {
            InitializeComponent();
            DataContext = this;
            LoadRoom();
            _userId = userId;
        }

        private void LoadRoom()
        {
            using (_context = new FuminiHotelManagementContext())
            {
                try
                {
                    var typeList = _context.RoomInformations.ToList();
                    cboRoom.ItemsSource = typeList;
                    cboRoom.DisplayMemberPath = "RoomNumber";
                    cboRoom.SelectedValuePath = "RoomId";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error on load list of types");
                }
            }
        }

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            if (bookInfo.IsNullOrEmpty())
            {
                MessageBox.Show("You must add a room first!");
            }
            else
            {
                try
                {
                    using (_context = new FuminiHotelManagementContext())
                    {
                        // Begin a transaction
                        using (var transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                BookingReservation bookingReservation = new BookingReservation
                                {
                                    BookingReservationId = _context.BookingDetails.Count() * 2 + 1,
                                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                                    TotalPrice = bookInfo.Sum(x => x.Price),
                                    CustomerId = _userId,
                                    BookingStatus = 1
                                };

                                _context.Add(bookingReservation);
                                _context.SaveChanges(); // Save changes to generate BookingReservationId

                                foreach (var book in bookInfo)
                                {
                                    BookingDetail bookingDetail = new BookingDetail
                                    {
                                        RoomId = _context.RoomInformations.Where(r => r.RoomNumber == book.RoomNumber).Select(r => r.RoomId).First(),
                                        StartDate = book.Start,
                                        EndDate = book.End,
                                        ActualPrice = book.Price,
                                        BookingReservationId = bookingReservation.BookingReservationId,
                                        BookingReservation = bookingReservation// Assign the BookingReservationId
                                    };
                                    _context.Add(bookingDetail);
                                }

                                _context.SaveChanges(); // Save changes for BookingDetail entities

                                // Commit the transaction if all changes are applied successfully
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                // Roll back the transaction in case of an error
                                transaction.Rollback();
                                MessageBox.Show("An error occurred during the booking process: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }

                        bookInfo.Clear();
                        ResetInputFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (_context = new FuminiHotelManagementContext())
            {
                DateTime start = DateTime.Parse(txtStart.Text).Date;
                DateTime end = DateTime.Parse(txtEnd.Text).Date;
                if (end <= start)
                {
                    MessageBox.Show("End date must be greater than the start date.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                decimal pricePerDay = Convert.ToDecimal(_context.RoomInformations
                    .Where(r => r.RoomId == Convert.ToInt32(cboRoom.SelectedValue.ToString()))
                    .Select(r => r.RoomPricePerDay)
                    .FirstOrDefault());

                TimeSpan duration = end - start; // Calculate the duration between start and end dates

                int numberOfDays = duration.Days; // Get the number of days from the duration

                booking b = new booking
                {
                    RoomNumber = _context.RoomInformations
                        .Where(r => r.RoomId == Convert.ToInt32(cboRoom.SelectedValue.ToString()))
                        .Select(r => r.RoomNumber)
                        .FirstOrDefault(),
                    Start = new DateOnly(start.Year, start.Month, start.Day), // Convert DateTime to DateOnly
                    End = new DateOnly(end.Year, end.Month, end.Day), // Convert DateTime to DateOnly
                    Price = pricePerDay * numberOfDays, // Calculate the total price
                };


                bookInfo.Add(b);
                ResetInputFields();
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is booking selectedBooking)
            {
                try
                {
                    using (_context = new FuminiHotelManagementContext())
                    {
                        bookInfo.Remove(selectedBooking);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a booking to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetInputFields()
        {
            txtEnd.Text = "";
            txtStart.Text = "";
            cboRoom.SelectedValue = -1;
        }
    }
}
