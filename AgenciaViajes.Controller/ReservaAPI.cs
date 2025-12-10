using agenciaViajes.Model;
using agenciaViajes.Model.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgenciaViajes.Controller
{
    /// <summary>
    /// API de Reservas - Capa de Lógica de Negocio (BLL)
    /// Gestiona operaciones de reservas incluyendo validación de plazas
    /// y actualización automática de plazas disponibles
    /// </summary>
    public class ReservaAPI
    {
        private readonly ReservasRepositorio _reservasRepo;
        private readonly ViajesRepositorio _viajesRepo;
        private readonly ClientesRepositorio _clientesRepo;

        public ReservaAPI()
        {
            _reservasRepo = new ReservasRepositorio();
            _viajesRepo = new ViajesRepositorio();
            _clientesRepo = new ClientesRepositorio();
        }

        /// <summary>
        /// Obtiene todas las reservas ordenadas por fecha descendente
        /// </summary>
        public List<Reservas> ListarReservas()
        {
            return _reservasRepo.ObtenerTodos().OrderByDescending(r => r.FechaReserva).ToList();
        }

        /// <summary>
        /// Obtiene una reserva por su ID
        /// </summary>
        public Reservas ObtenerReserva(int idReserva)
        {
            return _reservasRepo.ObtenerPorId(idReserva);
        }

        /// <summary>
        /// Crea una reserva validando existencia de cliente, viaje y plazas disponibles
        /// Actualiza automáticamente las plazas del viaje restando 1
        /// </summary>
        public bool CrearReserva(int idCliente, int idViaje, DateTime fechaReserva, out string mensajeError)
        {
            mensajeError = string.Empty;

            try
            {
                var cliente = _clientesRepo.ObtenerPorId(idCliente);
                if (cliente == null)
                {
                    mensajeError = "El cliente seleccionado no existe.";
                    return false;
                }

                var viaje = _viajesRepo.ObtenerPorId(idViaje);
                if (viaje == null)
                {
                    mensajeError = "El viaje seleccionado no existe.";
                    return false;
                }

                if (viaje.PlazasDisponibles <= 0)
                {
                    mensajeError = "No hay plazas disponibles para este viaje.";
                    return false;
                }

                if (fechaReserva == default(DateTime))
                {
                    fechaReserva = DateTime.Now;
                }

                var reserva = new Reservas
                {
                    IdCliente = idCliente,
                    IdViaje = idViaje,
                    FechaReserva = fechaReserva
                };

                _reservasRepo.Insertar(reserva);
                _viajesRepo.ActualizarPlazas(idViaje, -1);

                return true;
            }
            catch (Exception ex)
            {
                mensajeError = "Error al crear la reserva: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Cancela una reserva y devuelve la plaza al viaje sumando 1
        /// </summary>
        public bool CancelarReserva(int idReserva, out string mensajeError)
        {
            mensajeError = string.Empty;

            try
            {
                var reserva = _reservasRepo.ObtenerPorId(idReserva);
                if (reserva == null)
                {
                    mensajeError = "La reserva no existe.";
                    return false;
                }

                int idViaje = reserva.IdViaje;

                _reservasRepo.Eliminar(idReserva);
                _viajesRepo.ActualizarPlazas(idViaje, 1);

                return true;
            }
            catch (Exception ex)
            {
                mensajeError = "Error al cancelar la reserva: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtiene todas las reservas de un cliente ordenadas por fecha
        /// </summary>
        public List<Reservas> MostrarReservasPorCliente(int idCliente)
        {
            return _reservasRepo.ObtenerPorCliente(idCliente)
                .OrderByDescending(r => r.FechaReserva)
                .ToList();
        }

        /// <summary>
        /// Obtiene todas las reservas de un viaje ordenadas por fecha
        /// </summary>
        public List<Reservas> MostrarReservasPorViaje(int idViaje)
        {
            return _reservasRepo.ObtenerPorViaje(idViaje)
                .OrderByDescending(r => r.FechaReserva)
                .ToList();
        }

        /// <summary>
        /// Verifica si un viaje tiene plazas disponibles
        /// </summary>
        public bool VerificarPlazasDisponibles(int idViaje)
        {
            var viaje = _viajesRepo.ObtenerPorId(idViaje);
            return viaje != null && viaje.PlazasDisponibles > 0;
        }

        /// <summary>
        /// Obtiene el número exacto de plazas disponibles de un viaje
        /// </summary>
        public int ObtenerPlazasDisponibles(int idViaje)
        {
            return _viajesRepo.ObtenerPlazasDisponibles(idViaje);
        }
    }
}
