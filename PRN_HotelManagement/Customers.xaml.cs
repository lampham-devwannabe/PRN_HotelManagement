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
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customers : UserControl
    {
        private FuminiHotelManagementContext _context;

        public ObservableCollection<Customer> CustomersList { get; set; } = new ObservableCollection<Customer>();

        public Customers()
        {
            InitializeComponent();
            DataContext = this;
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                using (_context = new FuminiHotelManagementContext())
                {
                    CustomersList.Clear();
                    var customers = _context.Customers.ToList();
                    foreach (var customer in customers)
                    {
                        CustomersList.Add(customer);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ResetInputFields();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Customer newCustomer = new Customer
                {
                    CustomerFullName = txtCustomerName.Text,
                    Telephone = txtPhone.Text,
                    EmailAddress = txtEmail.Text,
                    CustomerBirthday = DateOnly.Parse(txtBirthday.Text),
                    CustomerStatus = 1,
                    Password = "123"
                };
                using (_context = new FuminiHotelManagementContext())
                {
                    _context.Customers.Add(newCustomer);
                    _context.SaveChanges();
                }
                LoadCustomers();
                ResetInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is Customer selectedCustomer)
            {
                try
                {
                    using (_context = new FuminiHotelManagementContext())
                    {
                        // Find the selected customer in the database
                        var customerToUpdate = _context.Customers.Find(selectedCustomer.CustomerId);
                        if (customerToUpdate != null)
                        {
                            // Update the customer properties
                            customerToUpdate.CustomerFullName = txtCustomerName.Text;
                            customerToUpdate.Telephone = txtPhone.Text;
                            customerToUpdate.EmailAddress = txtEmail.Text;
                            customerToUpdate.CustomerBirthday = DateOnly.Parse(txtBirthday.Text); // Assuming CustomerBirthday is a DateTime property

                            _context.SaveChanges();
                            ResetInputFields();
                            LoadCustomers();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is Customer selectedCustomer)
            {
                try
                {
                    using (_context = new FuminiHotelManagementContext())
                    {
                        // Find the selected customer in the database
                        var customerToDelete = _context.Customers.Find(selectedCustomer.CustomerId);
                        if (customerToDelete != null)
                        {
                            _context.Customers.Remove(customerToDelete);
                            _context.SaveChanges();
                            LoadCustomers();
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
            if (dgData.SelectedItem is Customer selectedCustomer)
            {
                txtCustomerName.Text = selectedCustomer.CustomerFullName;
                txtPhone.Text = selectedCustomer.Telephone.ToString();
                txtEmail.Text = selectedCustomer.EmailAddress;
                txtBirthday.Text = selectedCustomer.CustomerBirthday?.ToString();
            }
            else
            {
                ResetInputFields();
            }
        }

        private void ResetInputFields()
        {
            txtCustomerName.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtBirthday.Text = "";
        }
    }
}
