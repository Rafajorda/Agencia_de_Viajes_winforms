using AgenciaViajes.Controller;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VistasWPF
{
    /// <summary>
    /// Lógica de interacción para UCBooking.xaml
    /// </summary>
    public partial class UCBooking : UserControl
    {
        private readonly ReservaAPI _reservaAPI = new ReservaAPI();
        private readonly ClienteAPI _clienteAPI = new ClienteAPI();
        private readonly ViajeAPI _viajeAPI = new ViajeAPI();

        public UCBooking()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var reservas = _reservaAPI.ListarReservas();
                var reservasDisplay = reservas.Select(r => new
                {
                    r.IdReserva,
                    r.IdCliente,
                    Cliente = r.Clientes.Nombre + " " + r.Clientes.Apellidos,
                    r.IdViaje,
                    Destino = r.Viajes.Destino,
                    Precio = r.Viajes.Precio,
                    FechaReserva = r.FechaReserva.ToString("dd/MM/yyyy")
                }).ToList();

                DgReservas.ItemsSource = reservasDisplay;
                TxtTotalReservas.Text = $"Total de reservas: {reservas.Count}";

                // Load combos
                CmbClientes.ItemsSource = _clienteAPI.ListarClientes();
                CmbViajes.ItemsSource = _viajeAPI.ListarViajesDisponibles();
                CmbClientes.SelectedIndex = -1;
                CmbViajes.SelectedIndex = -1;

                LblPlazasDisponibles.Text = "Plazas disponibles: -";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCrearReserva_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmbClientes.SelectedValue == null)
                {
                    MessageBox.Show("Debe seleccionar un cliente", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CmbViajes.SelectedValue == null)
                {
                    MessageBox.Show("Debe seleccionar un viaje", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int idCliente = (int)CmbClientes.SelectedValue;
                int idViaje = (int)CmbViajes.SelectedValue;
                DateTime fecha = DpFechaReserva.SelectedDate ?? DateTime.Now;

                string msg;
                if (_reservaAPI.CrearReserva(idCliente, idViaje, fecha, out msg))
                {
                    MessageBox.Show("Reserva creada correctamente", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                else
                {
                    MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear reserva: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelarReserva_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = DgReservas.SelectedItem as dynamic;
                if (item == null)
                {
                    MessageBox.Show("Seleccione una reserva para cancelar", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (MessageBox.Show("¿Está seguro de cancelar esta reserva?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    string msg;
                    if (_reservaAPI.CancelarReserva((int)item.IdReserva, out msg))
                    {
                        MessageBox.Show("Reserva cancelada correctamente", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cancelar reserva: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFiltrarPorCliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmbClientes.SelectedValue == null)
                {
                    MessageBox.Show("Debe seleccionar un cliente", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int idCliente = (int)CmbClientes.SelectedValue;
                var reservas = _reservaAPI.MostrarReservasPorCliente(idCliente);
                var reservasDisplay = reservas.Select(r => new
                {
                    r.IdReserva,
                    r.IdCliente,
                    Cliente = r.Clientes.Nombre + " " + r.Clientes.Apellidos,
                    r.IdViaje,
                    Destino = r.Viajes.Destino,
                    Precio = r.Viajes.Precio,
                    FechaReserva = r.FechaReserva.ToString("dd/MM/yyyy")
                }).ToList();

                DgReservas.ItemsSource = reservasDisplay;
                TxtTotalReservas.Text = $"Reservas del cliente: {reservas.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar por cliente: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFiltrarPorViaje_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmbViajes.SelectedValue == null)
                {
                    MessageBox.Show("Debe seleccionar un viaje", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int idViaje = (int)CmbViajes.SelectedValue;
                var reservas = _reservaAPI.MostrarReservasPorViaje(idViaje);
                var reservasDisplay = reservas.Select(r => new
                {
                    r.IdReserva,
                    r.IdCliente,
                    Cliente = r.Clientes.Nombre + " " + r.Clientes.Apellidos,
                    r.IdViaje,
                    Destino = r.Viajes.Destino,
                    Precio = r.Viajes.Precio,
                    FechaReserva = r.FechaReserva.ToString("dd/MM/yyyy")
                }).ToList();

                DgReservas.ItemsSource = reservasDisplay;
                TxtTotalReservas.Text = $"Reservas del viaje: {reservas.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar por viaje: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnMostrarTodas_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void CmbViajes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CmbViajes.SelectedValue != null && CmbViajes.SelectedValue is int)
                {
                    int idViaje = (int)CmbViajes.SelectedValue;
                    int plazas = _reservaAPI.ObtenerPlazasDisponibles(idViaje);
                    LblPlazasDisponibles.Text = $"Plazas disponibles: {plazas}";
                }
                else
                {
                    LblPlazasDisponibles.Text = "Plazas disponibles: -";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener plazas: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
