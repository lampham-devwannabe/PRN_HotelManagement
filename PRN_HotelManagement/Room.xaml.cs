using PRN_HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Interaction logic for Room.xaml
    /// </summary>
    public partial class Room : UserControl
    {
        private FuminiHotelManagementContext _context;

        public ObservableCollection<RoomInformation> RoomInformation { get; set; } = new ObservableCollection<RoomInformation>();
        public Room()
        {
            InitializeComponent();
            DataContext = this;
            LoadRoom();
            LoadType();
        }

        private void LoadRoom()
        {
            try
            {
                using (_context = new FuminiHotelManagementContext())
                {
                    RoomInformation.Clear();
                    var rooms = _context.RoomInformations.ToList();
                    foreach (var room in rooms)
                    {
                        RoomInformation.Add(room);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ResetInputFields();
        }

        private void LoadType()
        {
            using (_context = new FuminiHotelManagementContext())
            {
                try
                {
                    var typeList = _context.RoomTypes.ToList();
                    cboType.ItemsSource = typeList;
                    cboType.DisplayMemberPath = "RoomTypeName";
                    cboType.SelectedValuePath = "RoomTypeId";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error on load list of types");
                }
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RoomInformation room = new RoomInformation
                {
                    RoomNumber = txtRoomNumber.Text,
                    RoomMaxCapacity = Convert.ToInt32(txtMaxCapacity.Text),
                    RoomPricePerDay = Convert.ToInt32(txtMaxCapacity.Text),
                    RoomDetailDescription = txtDescription.Text,
                    RoomStatus = 1
                };
                if (int.TryParse(cboType.SelectedValue.ToString(), out int typeId))
                {
                    room.RoomTypeId = typeId;
                }
                using (_context = new FuminiHotelManagementContext())
                {
                    _context.RoomInformations.Add(room);
                    _context.SaveChanges();
                }
                LoadRoom();
                ResetInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is RoomInformation selectedRoom)
            {
                try
                {
                    using (_context = new FuminiHotelManagementContext())
                    {
                        // Find the selected customer in the database
                        var roomToUpdate = _context.RoomInformations.Find(selectedRoom.RoomId);
                        if (roomToUpdate != null)
                        {
                            // Update the customer properties
                            roomToUpdate.RoomDetailDescription = txtDescription.Text;
                            roomToUpdate.RoomMaxCapacity = Convert.ToInt32(txtMaxCapacity.Text);
                            roomToUpdate.RoomPricePerDay = Convert.ToInt32(txtPrice.Text);
                            roomToUpdate.RoomNumber = txtRoomNumber.Text; // Assuming CustomerBirthday is a DateTime property
                            roomToUpdate.RoomTypeId = Convert.ToInt32(cboType.SelectedValue);
                            _context.SaveChanges();
                            ResetInputFields();
                            LoadRoom();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a room to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            if (dgData.SelectedItem is RoomInformation selectedRoom)
            {
                try
                {
                    using (_context = new FuminiHotelManagementContext())
                    {
                        // Find the selected customer in the database
                        var roomToDelete = _context.RoomInformations.Find(selectedRoom.RoomId);
                        if (roomToDelete != null)
                        {
                            _context.RoomInformations.Remove(roomToDelete);
                            _context.SaveChanges();
                            LoadRoom();
                            ResetInputFields();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is RoomInformation selectedRoom)
            {
                txtDescription.Text = selectedRoom.RoomDetailDescription?.ToString();
                txtMaxCapacity.Text = selectedRoom.RoomMaxCapacity.ToString();
                txtPrice.Text = selectedRoom.RoomPricePerDay.ToString();
                txtRoomNumber.Text = selectedRoom.RoomNumber.ToString();
                cboType.SelectedValue = selectedRoom.RoomTypeId;
            }
            else
            {
                ResetInputFields();
            }
        }

        private void ResetInputFields()
        {
            txtDescription.Text = "";
            txtMaxCapacity.Text = "";
            txtPrice.Text = "";
            txtRoomNumber.Text = "";
            cboType.SelectedValue = -1;
        }
    }
}
