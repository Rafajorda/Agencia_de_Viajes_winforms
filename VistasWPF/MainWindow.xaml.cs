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

namespace VistasWPF
{
    /// <summary>
    /// Lógica de interacción para MainWindow2.xaml
    /// </summary>
    public partial class MainWindow2 : Window
    {
        public MainWindow2()
        {
            InitializeComponent();
        }

        private void RbtnClientes_Checked(object sender, RoutedEventArgs e)
        {
            var uc = new UCClients();
            MainContent.Content = uc;
        }

        private void RbtnViajes_Checked(object sender, RoutedEventArgs e)
        {
            var uc = new UCTravel();
            MainContent.Content = uc;
        }

        private void RbtnReservas_Checked(object sender, RoutedEventArgs e)
        {
            var uc = new UCBooking();
            MainContent.Content = uc;
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
