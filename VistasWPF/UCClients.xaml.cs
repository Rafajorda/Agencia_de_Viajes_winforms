using AgenciaViajes.Controller;
using System;
using System.Windows;
using System.Windows.Controls;

namespace VistasWPF
{
    /// <summary>
    /// Lógica de interacción para UCClients.xaml
    /// </summary>
    public partial class UCClients : UserControl
    {
        private readonly ClienteAPI _api = new ClienteAPI();
        private int _idSeleccionado = 0;

        public UCClients()
        {
            InitializeComponent();
            LoadData();
            ResetState();
        }

        private void LoadData()
        {
            try
            {
                DgClientes.ItemsSource = _api.ListarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DgClientes.ItemsSource = Array.Empty<object>();
            }
        }

        private void ResetState()
        {
            TxtNombre.Clear();
            TxtApellidos.Clear();
            TxtEmail.Clear();
            _idSeleccionado = 0;
            BtnGuardar.Content = "Guardar";
            BtnCancelar.IsEnabled = false;
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            ResetState();
            TxtNombre.Focus();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg;
                bool ok;

                var nombre = (TxtNombre.Text ?? string.Empty).Trim();
                var apellidos = (TxtApellidos.Text ?? string.Empty).Trim();
                var email = (TxtEmail.Text ?? string.Empty).Trim();

                if (_idSeleccionado == 0)
                {
                    ok = _api.CrearCliente(nombre, apellidos, email, out msg);
                }
                else
                {
                    ok = _api.EditarCliente(_idSeleccionado, nombre, apellidos, email, out msg);
                }

                if (ok)
                {
                    MessageBox.Show("Cliente guardado correctamente.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ResetState();
                }
                else
                {
                    MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el cliente:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var item = DgClientes.SelectedItem as dynamic;
            if (item == null)
            {
                MessageBox.Show("Seleccione un cliente.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                _idSeleccionado = (int)item.IdCliente;
            }
            catch
            {
                _idSeleccionado = 0;
            }

            TxtNombre.Text = item.Nombre;
            TxtApellidos.Text = item.Apellidos;
            TxtEmail.Text = item.Email;

            BtnGuardar.Content = "Actualizar";
            BtnCancelar.IsEnabled = true;
            TxtNombre.Focus();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var item = DgClientes.SelectedItem as dynamic;
            if (item == null)
            {
                MessageBox.Show("Seleccione un cliente.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Confirmar eliminación?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string msg;
                    if (_api.EliminarCliente((int)item.IdCliente, out msg))
                    {
                        MessageBox.Show("Cliente eliminado.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                        ResetState();
                    }
                    else
                    {
                        MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el cliente:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ResetState();
        }
    }
}
