using PRN_HotelManagement.Model;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PRN_HotelManagement
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly FuminiHotelManagementContext _context;
        public Login()
        {
            InitializeComponent();
            _context = new FuminiHotelManagementContext();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Customer account = _context.Customers.Where(ac => ac.EmailAddress == txtUser.Text).FirstOrDefault();

            if (txtUser.Text == "Admin")
            {
                this.Hide();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                if (account != null && account.Password.Equals(txtPass.Password))
                {
                    this.Hide();
                    UserWindow userWindow = new UserWindow(account.CustomerId);
                    userWindow.Show();
                }
                else
                {
                    MessageBox.Show("You are not permitted", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
