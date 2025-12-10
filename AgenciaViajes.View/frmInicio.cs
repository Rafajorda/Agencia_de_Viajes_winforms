using System;
using System.Windows.Forms;

namespace AgenciaViajes.View
{
    /// <summary>
    /// Formulario de Inicio - Menú Principal de la aplicación
    /// Proporciona navegación a los diferentes módulos del sistema
    /// Este es el punto de entrada de la aplicación después de Program.cs
    /// </summary>
    public partial class frmInicio : Form
    {
        /// <summary>
        /// Constructor del formulario
        /// Inicializa los componentes visuales generados por el diseñador
        /// </summary>
        public frmInicio()
        {
            // InitializeComponent() es generado automáticamente por el diseñador
            // Configura todos los controles, propiedades y eventos del formulario
            InitializeComponent();
        }

        /// <summary>
        /// Evento que se dispara cuando el formulario se carga por primera vez
        /// </summary>
        /// <param name="sender">El objeto que disparó el evento (el formulario)</param>
        /// <param name="e">Argumentos del evento</param>
        /// <remarks>
        /// Se ejecuta una sola vez cuando el formulario aparece en pantalla
        /// Ideal para inicializaciones, cargar datos iniciales, etc.
        /// </remarks>
        private void frmInicio_Load(object sender, EventArgs e)
        {
            // Configuración inicial del formulario
            // Por ahora está vacío, pero podría incluir:
            // - Verificar permisos de usuario
            // - Cargar configuraciones
            // - Inicializar servicios
        }

        /// <summary>
        /// Maneja el clic en el botón de Clientes
        /// Abre el formulario de gestión de clientes
        /// </summary>
        /// <param name="sender">El botón que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        /// <remarks>
        /// ShowDialog() abre el formulario de manera MODAL
        /// Esto significa que bloquea el formulario padre hasta que se cierre
        /// El usuario no puede interactuar con frmInicio hasta cerrar frmClientes
        /// </remarks>
        private void btnClientes_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario de clientes
            frmClientes formClientes = new frmClientes();
            
            // Mostrar el formulario de manera modal (bloquea el formulario padre)
            // Alternativa: Show() lo mostraría de manera no modal
            formClientes.ShowDialog();
        }

        /// <summary>
        /// Maneja el clic en el botón de Viajes
        /// Abre el formulario de gestión de viajes
        /// </summary>
        /// <param name="sender">El botón que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void btnViajes_Click(object sender, EventArgs e)
        {
            // Crear y mostrar formulario de viajes de manera modal
            frmViajes formViajes = new frmViajes();
            formViajes.ShowDialog();
        }

        /// <summary>
        /// Maneja el clic en el botón de Reservas
        /// Abre el formulario de gestión de reservas
        /// </summary>
        /// <param name="sender">El botón que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void btnReservas_Click(object sender, EventArgs e)
        {
            // Crear y mostrar formulario de reservas de manera modal
            frmReservas formReservas = new frmReservas();
            formReservas.ShowDialog();
        }

        /// <summary>
        /// Maneja el clic en el botón Salir
        /// Cierra la aplicación completa con confirmación del usuario
        /// </summary>
        /// <param name="sender">El botón que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        /// <remarks>
        /// Muestra un MessageBox de confirmación antes de salir
        /// Application.Exit() cierra TODA la aplicación, no solo este formulario
        /// </remarks>
        private void btnSalir_Click(object sender, EventArgs e)
        {
            // Mostrar mensaje de confirmación antes de salir
            // MessageBox.Show devuelve el botón que el usuario presionó
            if (MessageBox.Show("¿Está seguro que desea salir?", // Mensaje
                "Confirmar salida",                              // Título de la ventana
                MessageBoxButtons.YesNo,                         // Botones a mostrar
                MessageBoxIcon.Question)                         // Icono de pregunta
                == DialogResult.Yes)                             // Si presionó "Sí"
            {
                // Cerrar toda la aplicación
                // Esto finalizará el proceso y cerrará todos los formularios
                Application.Exit();
            }
            // Si presionó "No", simplemente no hace nada y permanece en la aplicación
        }

        private void lblTitulo_Click(object sender, EventArgs e)
        {

        }
    }
}
