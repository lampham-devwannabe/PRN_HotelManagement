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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace PRN_HotelManagement
{
    /// <summary>
    /// Interaction logic for Information.xaml
    /// </summary>
    public partial class Information : UserControl
    {
        private readonly int _userId;
        private FuminiHotelManagementContext _context;

        public Information(int userId)
        {
            InitializeComponent();
            this.DataContext = this;
            _userId = userId;
            LoadUser();
        }

        private void LoadUser()
        {
            using (_context = new FuminiHotelManagementContext())
            {
                Customer customer = _context.Customers.Where(c => c.CustomerId == _userId).FirstOrDefault();
                txtId.Text = customer.CustomerId.ToString();
                txtBirthday.Text = customer.CustomerBirthday.ToString();
                txtEmail.Text = customer.EmailAddress.ToString();
                txtName.Text = customer.CustomerFullName.ToString();
                txtPass.Text = customer.Password.ToString();
                txtPhone.Text = customer.Telephone.ToString();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            using (_context = new FuminiHotelManagementContext())
            {
                Customer customer = _context.Customers.Find(Convert.ToInt32(txtId.Text));
                customer.Telephone = txtPhone.Text;
                customer.CustomerFullName = txtName.Text;
                customer.Password = txtPass.Text;
                customer.CustomerBirthday = DateOnly.Parse(txtBirthday.Text);
                customer.EmailAddress = txtEmail.Text;
                _context.SaveChanges();
                LoadUser();
            }
        }
    }
}
