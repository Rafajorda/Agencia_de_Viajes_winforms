using AgenciaViajes.Controller;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AgenciaViajes.View
{
    /// <summary>
    /// Formulario de Gestión de Reservas - El más complejo del sistema
    /// Funcionalidades principales:
    /// - Crear reservas (validando plazas disponibles)
    /// - Cancelar reservas (devolviendo plazas)
    /// - Filtrar reservas por cliente o viaje
    /// - Mostrar plazas disponibles en tiempo real
    /// - Actualización automática de plazas al reservar/cancelar
    /// </summary>
    public partial class frmReservas : Form
    {
        // Tres APIs necesarias para gestionar reservas
        // ReservaAPI: para operaciones de reservas
        // ClienteAPI: para listar clientes disponibles
        // ViajeAPI: para listar viajes disponibles
        private ReservaAPI _reservaAPI;
        private ClienteAPI _clienteAPI;
        private ViajeAPI _viajeAPI;

        /// <summary>
        /// Constructor - Inicializa las tres APIs necesarias
        /// </summary>
        /// <remarks>
        /// Este formulario necesita acceso a tres entidades:
        /// - Reservas (la principal)
        /// - Clientes (para seleccionar quién reserva)
        /// - Viajes (para seleccionar qué se reserva)
        /// </remarks>
        public frmReservas()
        {
            InitializeComponent();
            
            // Inicializar las tres APIs
            _reservaAPI = new ReservaAPI();
            _clienteAPI = new ClienteAPI();
            _viajeAPI = new ViajeAPI();
        }

        /// <summary>
        /// Evento Load - Carga inicial de datos
        /// </summary>
        /// <remarks>
        /// Orden de carga:
        /// 1. Reservas (tabla principal)
        /// 2. Clientes (ComboBox)
        /// 3. Viajes (ComboBox)
        /// 4. Fecha actual
        /// </remarks>
        private void frmReservas_Load(object sender, EventArgs e)
        {
            // Cargar todas las listas
            CargarReservas();
            CargarClientes();
            CargarViajes();
            
            // Inicializar fecha a hoy
            dtpFechaReserva.Value = DateTime.Now;
        }

        /// <summary>
        /// Carga todas las reservas y las muestra en el DataGridView
        /// </summary>
        /// <remarks>
        /// IMPORTANTE: Las reservas vienen con objetos relacionados (Clientes, Viajes)
        /// Creamos una proyección anónima para mostrar solo los datos necesarios
        /// Esto mejora el rendimiento y la presentación
        /// </remarks>
        private void CargarReservas()
        {
            try
            {
                // Obtener reservas del controlador
                var reservas = _reservaAPI.ListarReservas();
                
                // PROYECCIÓN: Crear una lista con solo los datos que queremos mostrar
                // Select crea objetos anónimos con las propiedades seleccionadas
                var reservasDisplay = reservas.Select(r => new
                {
                    r.IdReserva,                                          // ID de la reserva
                    r.IdCliente,                                          // FK (oculto en UI)
                    Cliente = r.Clientes.Nombre + " " + r.Clientes.Apellidos,  // Nombre completo
                    r.IdViaje,                                            // FK (oculto en UI)
                    Destino = r.Viajes.Destino,                          // Destino del viaje
                    Precio = r.Viajes.Precio,                            // Precio del viaje
                    FechaReserva = r.FechaReserva.ToString("dd/MM/yyyy") // Formato español
                }).ToList();

                // Vincular al DataGridView
                dgvReservas.DataSource = null;
                dgvReservas.DataSource = reservasDisplay;
                
                // Configurar columnas
                if (dgvReservas.Columns.Contains("IdReserva"))
                    dgvReservas.Columns["IdReserva"].HeaderText = "ID Reserva";
                
                // Ocultar las FKs (no son útiles para el usuario)
                if (dgvReservas.Columns.Contains("IdCliente"))
                    dgvReservas.Columns["IdCliente"].Visible = false;
                if (dgvReservas.Columns.Contains("IdViaje"))
                    dgvReservas.Columns["IdViaje"].Visible = false;
                
                // Configurar columnas visibles
                if (dgvReservas.Columns.Contains("Cliente"))
                    dgvReservas.Columns["Cliente"].HeaderText = "Cliente";
                if (dgvReservas.Columns.Contains("Destino"))
                    dgvReservas.Columns["Destino"].HeaderText = "Destino";
                if (dgvReservas.Columns.Contains("Precio"))
                {
                    dgvReservas.Columns["Precio"].HeaderText = "Precio (€)";
                    dgvReservas.Columns["Precio"].DefaultCellStyle.Format = "N2";
                }
                if (dgvReservas.Columns.Contains("FechaReserva"))
                    dgvReservas.Columns["FechaReserva"].HeaderText = "Fecha Reserva";

                // Actualizar contador de reservas
                // Interpolación de strings: $"{variable}" inserta el valor en el texto
                lblTotalReservas.Text = $"Total de reservas: {reservas.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reservas: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Carga la lista de clientes en el ComboBox
        /// </summary>
        /// <remarks>
        /// DisplayMember: propiedad que se muestra al usuario (Apellidos)
        /// ValueMember: valor real que se obtiene al seleccionar (IdCliente)
        /// SelectedIndex = -1: ninguno seleccionado inicialmente
        /// </remarks>
        private void CargarClientes()
        {
            try
            {
                cmbClientes.DataSource = null;
                cmbClientes.DataSource = _clienteAPI.ListarClientes();
                
                // Configurar qué se muestra y qué valor se obtiene
                cmbClientes.DisplayMember = "Apellidos";  // Mostrar apellidos
                cmbClientes.ValueMember = "IdCliente";     // Valor = ID
                
                // No seleccionar ninguno por defecto
                cmbClientes.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Carga la lista de viajes DISPONIBLES en el ComboBox
        /// </summary>
        /// <remarks>
        /// IMPORTANTE: Solo carga viajes con plazas disponibles > 0
        /// No tiene sentido mostrar viajes agotados en el combo de reservas
        /// </remarks>
        private void CargarViajes()
        {
            try
            {
                cmbViajes.DataSource = null;
                
                // ListarViajesDisponibles() ya filtra por PlazasDisponibles > 0
                cmbViajes.DataSource = _viajeAPI.ListarViajesDisponibles();
                
                cmbViajes.DisplayMember = "Destino";  // Mostrar destino
                cmbViajes.ValueMember = "IdViaje";     // Valor = ID
                cmbViajes.SelectedIndex = -1;          // Ninguno seleccionado
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar viajes: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Botón Crear Reserva - Crea una nueva reserva
        /// </summary>
        /// <remarks>
        /// PROCESO COMPLETO:
        /// 1. Validar que hay un cliente seleccionado
        /// 2. Validar que hay un viaje seleccionado
        /// 3. Llamar al controlador para crear (valida plazas)
        /// 4. Si es exitoso, recargar listas y limpiar
        /// 
        /// El controlador se encarga de:
        /// - Validar plazas disponibles
        /// - Crear la reserva en BD
        /// - Restar 1 plaza del viaje automáticamente
        /// </remarks>
        private void btnCrearReserva_Click(object sender, EventArgs e)
        {
            // VALIDACIÓN 1: Verificar que hay un cliente seleccionado
            if (cmbClientes.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un cliente", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir si no hay cliente
            }

            // VALIDACIÓN 2: Verificar que hay un viaje seleccionado
            if (cmbViajes.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un viaje", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir si no hay viaje
            }

            // Obtener valores seleccionados
            int idCliente = Convert.ToInt32(cmbClientes.SelectedValue);
            int idViaje = Convert.ToInt32(cmbViajes.SelectedValue);
            DateTime fechaReserva = dtpFechaReserva.Value;

            // Intentar crear la reserva a través del controlador
            string mensajeError;
            if (_reservaAPI.CrearReserva(idCliente, idViaje, fechaReserva, out mensajeError))
            {
                // ÉXITO: Reserva creada
                MessageBox.Show("Reserva creada correctamente", "Éxito", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Recargar reservas para mostrar la nueva
                CargarReservas();
                
                // Recargar viajes porque las plazas cambiaron
                CargarViajes();
                
                // Limpiar campos para otra reserva
                LimpiarCampos();
            }
            else
            {
                // ERROR: Mostrar mensaje (probablemente sin plazas)
                MessageBox.Show(mensajeError, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Botón Cancelar Reserva - Cancela una reserva existente
        /// </summary>
        /// <remarks>
        /// PROCESO:
        /// 1. Validar que hay una reserva seleccionada
        /// 2. Pedir confirmación
        /// 3. Llamar al controlador para cancelar
        /// 
        /// El controlador se encarga de:
        /// - Eliminar la reserva de BD
        /// - Devolver 1 plaza al viaje automáticamente
        /// </remarks>
        private void btnCancelarReserva_Click(object sender, EventArgs e)
        {
            // Validar que hay una fila seleccionada
            if (dgvReservas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una reserva para cancelar", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Pedir confirmación antes de cancelar
            if (MessageBox.Show("¿Está seguro de cancelar esta reserva?", "Confirmar cancelación", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Obtener ID de la reserva
                int idReserva = Convert.ToInt32(dgvReservas.CurrentRow.Cells["IdReserva"].Value);
                string mensajeError;

                // Intentar cancelar
                if (_reservaAPI.CancelarReserva(idReserva, out mensajeError))
                {
                    // ÉXITO: Informar que la plaza fue devuelta
                    MessageBox.Show("Reserva cancelada correctamente. La plaza ha sido devuelta al viaje.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Recargar listas
                    CargarReservas();
                    
                    // IMPORTANTE: Recargar viajes porque las plazas cambiaron
                    CargarViajes();
                }
                else
                {
                    MessageBox.Show(mensajeError, "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Botón Filtrar por Cliente - Muestra solo reservas de un cliente
        /// </summary>
        /// <remarks>
        /// El filtrado se hace en el controlador, no en la Vista
        /// Reutilizamos la lógica de proyección para mostrar datos
        /// </remarks>
        private void btnFiltrarPorCliente_Click(object sender, EventArgs e)
        {
            // Validar selección
            if (cmbClientes.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un cliente", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener ID del cliente
            int idCliente = Convert.ToInt32(cmbClientes.SelectedValue);
            
            // Obtener reservas filtradas del controlador
            var reservas = _reservaAPI.MostrarReservasPorCliente(idCliente);

            // Proyectar datos (igual que en CargarReservas)
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

            // Mostrar en DataGridView
            dgvReservas.DataSource = null;
            dgvReservas.DataSource = reservasDisplay;

            // Actualizar etiqueta con el número de reservas del cliente
            lblTotalReservas.Text = $"Reservas del cliente: {reservas.Count}";
        }

        /// <summary>
        /// Botón Filtrar por Viaje - Muestra solo reservas de un viaje
        /// </summary>
        /// <remarks>
        /// Útil para ver qué clientes han reservado un viaje específico
        /// Permite analizar la ocupación del viaje
        /// </remarks>
        private void btnFiltrarPorViaje_Click(object sender, EventArgs e)
        {
            // Validar selección
            if (cmbViajes.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un viaje", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener ID del viaje
            int idViaje = Convert.ToInt32(cmbViajes.SelectedValue);
            
            // Obtener reservas filtradas
            var reservas = _reservaAPI.MostrarReservasPorViaje(idViaje);

            // Proyectar datos
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

            // Mostrar en DataGridView
            dgvReservas.DataSource = null;
            dgvReservas.DataSource = reservasDisplay;

            // Actualizar etiqueta
            lblTotalReservas.Text = $"Reservas del viaje: {reservas.Count}";
        }

        /// <summary>
        /// Botón Mostrar Todas - Quita filtros y muestra todas las reservas
        /// </summary>
        private void btnMostrarTodas_Click(object sender, EventArgs e)
        {
            // Simplemente recargar todas las reservas
            CargarReservas();
        }

        /// <summary>
        /// Limpia los campos de entrada
        /// </summary>
        private void LimpiarCampos()
        {
            // Deseleccionar ComboBoxes
            cmbClientes.SelectedIndex = -1;
            cmbViajes.SelectedIndex = -1;
            
            // Resetear fecha a hoy
            dtpFechaReserva.Value = DateTime.Now;
        }

        /// <summary>
        /// Botón Cerrar - Cierra el formulario
        /// </summary>
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Evento SelectedIndexChanged del ComboBox de Viajes
        /// Se dispara cada vez que el usuario selecciona un viaje
        /// Actualiza el label de plazas disponibles en tiempo real
        /// </summary>
        /// <remarks>
        /// FUNCIONALIDAD IMPORTANTE:
        /// - Muestra plazas disponibles del viaje seleccionado
        /// - Cambia el color según disponibilidad:
        ///   * Rojo: Sin plazas (0)
        ///   * Naranja: Pocas plazas (<= 5)
        ///   * Verde: Plazas suficientes (> 5)
        /// - Esto ayuda al usuario a tomar decisiones rápidas
        /// </remarks>
        private void cmbViajes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Verificar que hay un valor seleccionado y es válido
            // SelectedValue puede ser null si no hay selección
            // También verificamos que sea un int (por si hay errores de binding)
            if (cmbViajes.SelectedValue != null && cmbViajes.SelectedValue is int)
            {
                // Obtener ID del viaje seleccionado
                int idViaje = (int)cmbViajes.SelectedValue;
                
                // Obtener plazas disponibles del controlador
                int plazas = _reservaAPI.ObtenerPlazasDisponibles(idViaje);
                
                // Actualizar texto del label
                lblPlazasDisponibles.Text = $"Plazas disponibles: {plazas}";
                
                // SISTEMA DE COLORES según disponibilidad
                if (plazas == 0)
                {
                    // ROJO: Sin plazas (no se puede reservar)
                    lblPlazasDisponibles.ForeColor = Color.Red;
                }
                else if (plazas <= 5)
                {
                    // NARANJA: Pocas plazas (alerta)
                    lblPlazasDisponibles.ForeColor = Color.Orange;
                }
                else
                {
                    // VERDE: Plazas suficientes (ok)
                    lblPlazasDisponibles.ForeColor = Color.Green;
                }
            }
            else
            {
                // Si no hay selección válida, mostrar guion y color negro
                lblPlazasDisponibles.Text = "Plazas disponibles: -";
                lblPlazasDisponibles.ForeColor = Color.Black;
            }
        }
    }
}
