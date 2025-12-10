using AgenciaViajes.Controller;
using System;
using System.Windows.Forms;

namespace AgenciaViajes.View
{
    /// <summary>
    /// Formulario de Gestión de Clientes - CRUD completo
    /// Permite Crear, Leer, Actualizar y Eliminar clientes
    /// Implementa el patrón MVC: la Vista solo maneja la interfaz
    /// </summary>
    public partial class frmClientes : Form
    {
        // API del controlador para operaciones de clientes
        // Toda la lógica de negocio está en ClienteAPI, no aquí
        private ClienteAPI _clienteAPI;
        
        // Variable para rastrear el cliente que estamos editando
        // 0 = modo crear nuevo, >0 = modo editar existente
        private int _idClienteSeleccionado = 0;

        /// <summary>
        /// Constructor del formulario
        /// Inicializa componentes y crea la instancia del API
        /// </summary>
        public frmClientes()
        {
            // Inicializar controles visuales
            InitializeComponent();
            
            // Crear instancia del controlador
            // Toda interacción con datos será a través de esta API
            _clienteAPI = new ClienteAPI();
        }

        /// <summary>
        /// Evento Load - Se ejecuta al cargar el formulario
        /// Aquí inicializamos datos y configuramos el estado inicial
        /// </summary>
        private void frmClientes_Load(object sender, EventArgs e)
        {
            // Cargar la lista de clientes en el DataGridView
            CargarClientes();
            
            // Limpiar campos y resetear controles al estado inicial
            LimpiarCampos();
        }

        /// <summary>
        /// Carga todos los clientes desde la API y los muestra en el DataGridView
        /// </summary>
        /// <remarks>
        /// DataGridView se vincula automáticamente a la lista de objetos
        /// Cada propiedad pública de Cliente se convierte en una columna
        /// Configuramos manualmente las columnas para mejorar la presentación
        /// </remarks>
        private void CargarClientes()
        {
            try
            {
                // Limpiar el DataSource antes de asignar nuevos datos
                // Esto evita problemas de visualización
                dgvClientes.DataSource = null;
                
                // Obtener clientes del controlador y vincular al DataGridView
                // ListarClientes() ya los trae ordenados por apellidos
                dgvClientes.DataSource = _clienteAPI.ListarClientes();
                
                // Ocultar la columna de navegación de Reservas
                // Esta propiedad existe por la relación 1-N pero no queremos mostrarla
                if (dgvClientes.Columns.Contains("Reservas"))
                    dgvClientes.Columns["Reservas"].Visible = false;
                
                // Configurar encabezados de columnas con nombres más amigables
                if (dgvClientes.Columns.Contains("IdCliente"))
                    dgvClientes.Columns["IdCliente"].HeaderText = "ID";
                if (dgvClientes.Columns.Contains("Nombre"))
                    dgvClientes.Columns["Nombre"].HeaderText = "Nombre";
                if (dgvClientes.Columns.Contains("Apellidos"))
                    dgvClientes.Columns["Apellidos"].HeaderText = "Apellidos";
                if (dgvClientes.Columns.Contains("Email"))
                    dgvClientes.Columns["Email"].HeaderText = "Email";
            }
            catch (Exception ex)
            {
                // Capturar errores de carga (BD no disponible, etc.)
                // Mostrar mensaje amigable al usuario
                MessageBox.Show("Error al cargar clientes: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Limpia todos los campos del formulario y resetea el estado
        /// </summary>
        /// <remarks>
        /// Vuelve el formulario al "modo crear nuevo"
        /// Se llama después de guardar/cancelar o al iniciar
        /// </remarks>
        private void LimpiarCampos()
        {
            // Limpiar todos los TextBox
            txtNombre.Clear();
            txtApellidos.Clear();
            txtEmail.Clear();
            
            // Resetear el ID (0 = modo crear nuevo)
            _idClienteSeleccionado = 0;
            
            // Cambiar el texto del botón a "Guardar" (modo crear)
            btnGuardar.Text = "Guardar";
            
            // Deshabilitar el botón Cancelar (no hay nada que cancelar)
            btnCancelar.Enabled = false;
        }

        /// <summary>
        /// Maneja el clic en el botón Nuevo
        /// Prepara el formulario para crear un nuevo cliente
        /// </summary>
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            // Limpiar y resetear campos
            LimpiarCampos();
            
            // Poner el foco en el primer campo para facilitar el ingreso
            txtNombre.Focus();
        }

        /// <summary>
        /// Maneja el clic en el botón Guardar/Actualizar
        /// Crea un nuevo cliente o actualiza uno existente según el modo
        /// </summary>
        /// <remarks>
        /// Este botón tiene doble función:
        /// - Si _idClienteSeleccionado == 0: CREAR nuevo cliente
        /// - Si _idClienteSeleccionado > 0: ACTUALIZAR cliente existente
        /// </remarks>
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Variables para manejar el resultado
            string mensajeError;
            bool resultado;

            // Decidir si crear o editar según el ID
            if (_idClienteSeleccionado == 0)
            {
                // MODO CREAR: Crear nuevo cliente
                resultado = _clienteAPI.CrearCliente(
                    txtNombre.Text,      // Nombre del TextBox
                    txtApellidos.Text,   // Apellidos del TextBox
                    txtEmail.Text,       // Email del TextBox
                    out mensajeError     // Mensaje de error por referencia
                );
            }
            else
            {
                // MODO EDITAR: Actualizar cliente existente
                resultado = _clienteAPI.EditarCliente(
                    _idClienteSeleccionado,  // ID del cliente a editar
                    txtNombre.Text,
                    txtApellidos.Text,
                    txtEmail.Text,
                    out mensajeError
                );
            }

            // Verificar el resultado de la operación
            if (resultado)
            {
                // ÉXITO: Mostrar mensaje de confirmación
                MessageBox.Show("Cliente guardado correctamente", "Éxito", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Recargar la lista para mostrar los cambios
                CargarClientes();
                
                // Limpiar campos y volver al modo crear
                LimpiarCampos();
            }
            else
            {
                // ERROR: Mostrar el mensaje de error del controlador
                // El controlador ya validó y generó un mensaje descriptivo
                MessageBox.Show(mensajeError, "Error de validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Maneja el clic en el botón Editar
        /// Carga los datos del cliente seleccionado en los campos para editar
        /// </summary>
        /// <remarks>
        /// Este método NO guarda, solo prepara el formulario para edición
        /// Cambia el formulario al "modo editar"
        /// </remarks>
        private void btnEditar_Click(object sender, EventArgs e)
        {
            // Verificar que hay una fila seleccionada en el DataGridView
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un cliente para editar", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // Salir si no hay selección
            }

            // Obtener el ID del cliente seleccionado
            // CurrentRow.Cells["nombre_columna"].Value obtiene el valor de una celda
            _idClienteSeleccionado = Convert.ToInt32(dgvClientes.CurrentRow.Cells["IdCliente"].Value);
            
            // Cargar los datos en los TextBox para edición
            txtNombre.Text = dgvClientes.CurrentRow.Cells["Nombre"].Value.ToString();
            txtApellidos.Text = dgvClientes.CurrentRow.Cells["Apellidos"].Value.ToString();
            txtEmail.Text = dgvClientes.CurrentRow.Cells["Email"].Value.ToString();
            
            // Cambiar el texto del botón a "Actualizar" (modo editar)
            btnGuardar.Text = "Actualizar";
            
            // Habilitar el botón Cancelar (para poder deshacer la edición)
            btnCancelar.Enabled = true;
            
            // Poner el foco en el primer campo
            txtNombre.Focus();
        }

        /// <summary>
        /// Maneja el clic en el botón Eliminar
        /// Elimina el cliente seleccionado con confirmación previa
        /// </summary>
        /// <remarks>
        /// IMPORTANTE: El controlador valida que no tenga reservas
        /// No se puede eliminar un cliente con historial de reservas
        /// </remarks>
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Verificar que hay una fila seleccionada
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un cliente para eliminar", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Pedir confirmación antes de eliminar
            // Es una buena práctica siempre confirmar operaciones destructivas
            if (MessageBox.Show("¿Está seguro de eliminar este cliente?", "Confirmar eliminación", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Obtener el ID del cliente a eliminar
                int idCliente = Convert.ToInt32(dgvClientes.CurrentRow.Cells["IdCliente"].Value);
                string mensajeError;

                // Intentar eliminar a través del controlador
                if (_clienteAPI.EliminarCliente(idCliente, out mensajeError))
                {
                    // ÉXITO: Cliente eliminado
                    MessageBox.Show("Cliente eliminado correctamente", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Recargar la lista
                    CargarClientes();
                    
                    // Limpiar campos
                    LimpiarCampos();
                }
                else
                {
                    // ERROR: Probablemente tiene reservas asociadas
                    MessageBox.Show(mensajeError, "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // Si el usuario presionó "No", no hacer nada
        }

        /// <summary>
        /// Maneja el clic en el botón Cancelar
        /// Cancela la edición actual y vuelve al modo crear
        /// </summary>
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // Simplemente limpiar campos y volver al estado inicial
            LimpiarCampos();
        }

        /// <summary>
        /// Maneja el clic en el botón Cerrar
        /// Cierra el formulario actual
        /// </summary>
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            // Close() cierra solo este formulario, no toda la aplicación
            this.Close();
        }

        /// <summary>
        /// Maneja el doble clic en una celda del DataGridView
        /// Permite editar un cliente haciendo doble clic en su fila
        /// </summary>
        /// <param name="sender">El DataGridView</param>
        /// <param name="e">Argumentos con información de la celda clickeada</param>
        /// <remarks>
        /// Atajo de usabilidad: doble clic = editar
        /// Es más intuitivo que tener que seleccionar y luego presionar "Editar"
        /// </remarks>
        private void dgvClientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar que no sea la fila de encabezados (e.RowIndex >= 0)
            if (e.RowIndex >= 0)
            {
                // Llamar al método de editar
                // Reutilizamos la lógica existente
                btnEditar_Click(sender, e);
            }
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
