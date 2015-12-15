using LuxBio.Library.Models;
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

namespace LuxBio.WindowsApp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // TODO refactor this
            Customer customer = null;

            if (txtUserName.Text == "1")
            {
                customer = new Customer() { ID = 1, Name = "Jørn", Phone = "12345678" };
            } else if (txtUserName.Text == "2")
            {
                customer = new Customer() { ID = 2, Name = "Børge", Phone = "12345678" };
            } else
            {
                customer = new Customer() { ID = 1, Name = "Jørn", Phone = "12345678" };
            }

            MainWindow main = new MainWindow(customer);
            main.Title = string.Format("Customer {0}", customer.ID);
            App.Current.MainWindow = main;
            this.Close();
            main.Show();
        }
    }
}
