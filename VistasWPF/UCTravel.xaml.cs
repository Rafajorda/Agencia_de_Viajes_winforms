using AgenciaViajes.Controller;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VistasWPF
{
    public partial class UCTravel : UserControl
    {
        private readonly ViajeAPI _api = new ViajeAPI();
        private int _idSeleccionado = 0;

        public UCTravel()
        {
            InitializeComponent();
            LoadData();
            ResetState();
        }

        private void LoadData()
        {
            try
            {
                DgViajes.ItemsSource = _api.ListarViajes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar viajes:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DgViajes.ItemsSource = Array.Empty<object>();
            }
        }

        private void ResetState()
        {
            TxtDestino.Clear();
            TxtPrecio.Clear();
            TxtPlazas.Text = "0";
            _idSeleccionado = 0;
            BtnGuardar.Content = "Guardar";
            BtnCancelar.IsEnabled = false;
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            ResetState();
            TxtDestino.Focus();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal precio;
                if (!decimal.TryParse(TxtPrecio.Text, out precio))
                {
                    MessageBox.Show("El precio debe ser un número válido", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int plazas;
                if (!int.TryParse(TxtPlazas.Text, out plazas))
                {
                    MessageBox.Show("Las plazas deben ser un número entero", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string msg;
                bool ok;
                if (_idSeleccionado == 0)
                {
                    ok = _api.CrearViaje(TxtDestino.Text.Trim(), precio, plazas, out msg);
                }
                else
                {
                    ok = _api.ModificarViaje(_idSeleccionado, TxtDestino.Text.Trim(), precio, plazas, out msg);
                }

                if (ok)
                {
                    MessageBox.Show("Viaje guardado correctamente.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("Error al guardar viaje:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var item = DgViajes.SelectedItem as dynamic;
            if (item == null)
            {
                MessageBox.Show("Seleccione un viaje.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                _idSeleccionado = (int)item.IdViaje;
            }
            catch
            {
                _idSeleccionado = 0;
            }

            TxtDestino.Text = item.Destino;
            TxtPrecio.Text = item.Precio.ToString();
            TxtPlazas.Text = item.PlazasDisponibles.ToString();

            BtnGuardar.Content = "Actualizar";
            BtnCancelar.IsEnabled = true;
            TxtDestino.Focus();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var item = DgViajes.SelectedItem as dynamic;
            if (item == null)
            {
                MessageBox.Show("Seleccione un viaje.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("¿Confirmar eliminación?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string msg;
                    if (_api.EliminarViaje((int)item.IdViaje, out msg))
                    {
                        MessageBox.Show("Viaje eliminado.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    MessageBox.Show("Error al eliminar viaje:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ResetState();
        }
    }
}