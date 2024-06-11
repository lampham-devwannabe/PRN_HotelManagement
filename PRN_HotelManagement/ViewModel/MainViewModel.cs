using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PRN_HotelManagement.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private object _selectedContent;

        public object SelectedContent
        {
            get { return _selectedContent; }
            set
            {
                _selectedContent = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChangeContentCommand { get; set; }

        public int UserId;

        public MainViewModel(int userId)
        {
            ChangeContentCommand = new RelayCommand(ChangeContent);
            UserId = userId;
        }

        private void ChangeContent(object parameter)
        {
            string pageName = parameter as string;
            if (pageName == "Dashboard")
            {
                SelectedContent = new Dashboard(); // Create an instance of your Dashboard UserControl
            }
            if (pageName == "Customers")
            {
                SelectedContent = new Customers(); // Create an instance of your Dashboard UserControl
            }
            if (pageName == "Room")
            {
                SelectedContent = new Room(); // Create an instance of your Dashboard UserControl
            }
            if (pageName == "Reservation")
            {
                SelectedContent = new Reservation(); // Create an instance of your Dashboard UserControl
            }

            if (pageName == "Booking")
            {
                SelectedContent = new Booking(UserId);
            }

            if (pageName == "History")
            {
                SelectedContent = new History(UserId);
            }

            if (pageName == "Information")
            {
                SelectedContent = new Information(UserId);
            }
            // Add more conditions for other pages if needed
        }
    }
}
