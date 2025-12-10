using AgenciaViajes.Controller;
using System;
using System.Windows.Forms;

namespace AgenciaViajes.View
{
    /// <summary>
    /// Formulario de Gestión de Viajes - CRUD completo
    /// Permite administrar los viajes disponibles en la agencia
    /// Funcionalidad similar a frmClientes pero con controles específicos para viajes
    /// </summary>
    public partial class frmViajes : Form
    {
        // API del controlador para operaciones de viajes
        private ViajeAPI _viajeAPI;
        
        // Variable para rastrear si estamos editando un viaje existente
        // 0 = crear nuevo, >0 = editar existente
        private int _idViajeSeleccionado = 0;

        /// <summary>
        /// Constructor del formulario
        /// </summary>
        public frmViajes()
        {
            InitializeComponent();
            _viajeAPI = new ViajeAPI();
        }

        /// <summary>
        /// Evento Load - Inicialización del formulario
        /// </summary>
        private void frmViajes_Load(object sender, EventArgs e)
        {
            // Cargar viajes en el DataGridView
            CargarViajes();
            
            // Limpiar y resetear campos
            LimpiarCampos();
        }

        /// <summary>
        /// Carga todos los viajes y los muestra en el DataGridView
        /// </summary>
        /// <remarks>
        /// Configura formato especial para el precio (N2 = dos decimales)
        /// Incluye viajes con plazas = 0 (agotados)
        /// </remarks>
        private void CargarViajes()
        {
            try
            {
                // Limpiar DataSource
                dgvViajes.DataSource = null;
                
                // Obtener viajes del controlador (ya vienen ordenados por destino)
                dgvViajes.DataSource = _viajeAPI.ListarViajes();
                
                // Ocultar columna de navegación de Reservas
                if (dgvViajes.Columns.Contains("Reservas"))
                    dgvViajes.Columns["Reservas"].Visible = false;
                
                // Configurar encabezados personalizados
                if (dgvViajes.Columns.Contains("IdViaje"))
                    dgvViajes.Columns["IdViaje"].HeaderText = "ID";
                if (dgvViajes.Columns.Contains("Destino"))
                    dgvViajes.Columns["Destino"].HeaderText = "Destino";
                if (dgvViajes.Columns.Contains("Precio"))
                {
                    dgvViajes.Columns["Precio"].HeaderText = "Precio (€)";
                    // Formato N2 = número con 2 decimales (ej: 150.00)
                    dgvViajes.Columns["Precio"].DefaultCellStyle.Format = "N2";
                }
                if (dgvViajes.Columns.Contains("PlazasDisponibles"))
                    dgvViajes.Columns["PlazasDisponibles"].HeaderText = "Plazas Disponibles";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar viajes: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Limpia todos los campos y resetea el formulario al modo crear
        /// </summary>
        private void LimpiarCampos()
        {
            // Limpiar TextBox
            txtDestino.Clear();
            txtPrecio.Clear();
            
            // Resetear NumericUpDown a 0
            numPlazas.Value = 0;
            
            // Resetear variables de estado
            _idViajeSeleccionado = 0;
            btnGuardar.Text = "Guardar";
            btnCancelar.Enabled = false;
        }

        /// <summary>
        /// Botón Nuevo - Prepara para crear un nuevo viaje
        /// </summary>
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            txtDestino.Focus(); // Foco en primer campo
        }

        /// <summary>
        /// Botón Guardar - Crea o actualiza un viaje
        /// </summary>
        /// <remarks>
        /// Incluye validación de formato del precio antes de enviar al controlador
        /// El controlador hace validaciones adicionales de negocio
        /// </remarks>
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensajeError;
            bool resultado;

            // VALIDACIÓN LOCAL: Verificar que el precio sea un número válido
            // TryParse intenta convertir el texto a decimal
            // Devuelve true si es válido, false si no
            decimal precio;
            if (!decimal.TryParse(txtPrecio.Text, out precio))
            {
                // Si no es un número válido, mostrar error y salir
                MessageBox.Show("El precio debe ser un número válido", "Error de validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir sin guardar
            }

            // Decidir si crear o editar
            if (_idViajeSeleccionado == 0)
            {
                // MODO CREAR
                resultado = _viajeAPI.CrearViaje(
                    txtDestino.Text,
                    precio,
                    (int)numPlazas.Value,  // Convertir de decimal a int
                    out mensajeError
                );
            }
            else
            {
                // MODO EDITAR
                resultado = _viajeAPI.ModificarViaje(
                    _idViajeSeleccionado,
                    txtDestino.Text,
                    precio,
                    (int)numPlazas.Value,
                    out mensajeError
                );
            }

            // Procesar resultado
            if (resultado)
            {
                MessageBox.Show("Viaje guardado correctamente", "Éxito", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarViajes();
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show(mensajeError, "Error de validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Botón Editar - Carga los datos del viaje seleccionado para edición
        /// </summary>
        private void btnEditar_Click(object sender, EventArgs e)
        {
            // Verificar selección
            if (dgvViajes.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un viaje para editar", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Obtener ID del viaje seleccionado
            _idViajeSeleccionado = Convert.ToInt32(dgvViajes.CurrentRow.Cells["IdViaje"].Value);
            
            // Cargar datos en los controles
            txtDestino.Text = dgvViajes.CurrentRow.Cells["Destino"].Value.ToString();
            txtPrecio.Text = dgvViajes.CurrentRow.Cells["Precio"].Value.ToString();
            
            // NumericUpDown requiere conversión a int
            numPlazas.Value = Convert.ToInt32(dgvViajes.CurrentRow.Cells["PlazasDisponibles"].Value);
            
            // Cambiar a modo editar
            btnGuardar.Text = "Actualizar";
            btnCancelar.Enabled = true;
            txtDestino.Focus();
        }

        /// <summary>
        /// Botón Eliminar - Elimina el viaje seleccionado
        /// </summary>
        /// <remarks>
        /// El controlador valida que no tenga reservas asociadas
        /// No se pueden eliminar viajes con reservas activas
        /// </remarks>
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Verificar selección
            if (dgvViajes.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un viaje para eliminar", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Pedir confirmación
            if (MessageBox.Show("¿Está seguro de eliminar este viaje?", "Confirmar eliminación", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int idViaje = Convert.ToInt32(dgvViajes.CurrentRow.Cells["IdViaje"].Value);
                string mensajeError;

                if (_viajeAPI.EliminarViaje(idViaje, out mensajeError))
                {
                    MessageBox.Show("Viaje eliminado correctamente", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarViajes();
                    LimpiarCampos();
                }
                else
                {
                    // Probablemente tiene reservas asociadas
                    MessageBox.Show(mensajeError, "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Botón Cancelar - Cancela la edición y vuelve al modo crear
        /// </summary>
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        /// <summary>
        /// Botón Cerrar - Cierra el formulario
        /// </summary>
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Doble clic en el DataGridView - Atajo para editar
        /// </summary>
        /// <remarks>
        /// Mejora la usabilidad: doble clic = modo editar
        /// </remarks>
        private void dgvViajes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnEditar_Click(sender, e);
            }
        }
    }
}
