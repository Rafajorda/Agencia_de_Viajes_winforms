using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VistasWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

    

        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            var w = new ClientesWindow();
            w.Owner = this;
            w.ShowDialog();
        }

        private void BtnViajes_Click(object sender, RoutedEventArgs e)
        {
            var w = new ViajesWindow();
            w.Owner = this;
            w.ShowDialog();
        }

        private void BtnReservas_Click(object sender, RoutedEventArgs e)
        {
            var w = new ReservasWindow();
            w.Owner = this;
            w.ShowDialog();
        }
        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}