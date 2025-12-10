using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace agenciaViajes.Model.Repositorios
{
    /// <summary>
    /// Repositorio de Reservas
    /// Hereda operaciones CRUD base y agrega métodos específicos
    /// Incluye eager loading de entidades relacionadas (Clientes, Viajes)
    /// </summary>
    public class ReservasRepositorio : RepositorioBase<Reservas>
    {
        public ReservasRepositorio() : base(new AgenciaViajesEntities())
        {
        }

        public ReservasRepositorio(AgenciaViajesEntities context) : base(context)
        {
        }

        public override void Actualizar(Reservas reserva)
        {
            if (reserva == null)
                throw new ArgumentNullException(nameof(reserva));

            var reservaExistente = _context.Reservas.Find(reserva.IdReserva);
            
            if (reservaExistente != null)
            {
                reservaExistente.FechaReserva = reserva.FechaReserva;
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene todas las reservas incluyendo datos de Cliente y Viaje
        /// </summary>
        public override List<Reservas> ObtenerTodos()
        {
            return _context.Reservas
                .Include(r => r.Clientes)
                .Include(r => r.Viajes)
                .ToList();
        }

        /// <summary>
        /// Obtiene una reserva por ID incluyendo datos de Cliente y Viaje
        /// </summary>
        public override Reservas ObtenerPorId(int id)
        {
            return _context.Reservas
                .Include(r => r.Clientes)
                .Include(r => r.Viajes)
                .FirstOrDefault(r => r.IdReserva == id);
        }

        /// <summary>
        /// Obtiene todas las reservas de un cliente
        /// </summary>
        public List<Reservas> ObtenerPorCliente(int idCliente)
        {
            return _context.Reservas
                .Include(r => r.Clientes)
                .Include(r => r.Viajes)
                .Where(r => r.IdCliente == idCliente)
                .ToList();
        }

        /// <summary>
        /// Obtiene todas las reservas de un viaje
        /// </summary>
        public List<Reservas> ObtenerPorViaje(int idViaje)
        {
            return _context.Reservas
                .Include(r => r.Clientes)
                .Include(r => r.Viajes)
                .Where(r => r.IdViaje == idViaje)
                .ToList();
        }

        /// <summary>
        /// Cuenta las reservas de un cliente sin cargar las entidades completas
        /// </summary>
        public int ContarPorCliente(int idCliente)
        {
            return _context.Reservas.Count(r => r.IdCliente == idCliente);
        }

        /// <summary>
        /// Cuenta las reservas de un viaje sin cargar las entidades completas
        /// </summary>
        public int ContarPorViaje(int idViaje)
        {
            return _context.Reservas.Count(r => r.IdViaje == idViaje);
        }

        /// <summary>
        /// Obtiene reservas dentro de un rango de fechas
        /// </summary>
        public List<Reservas> ObtenerPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            return _context.Reservas
                .Include(r => r.Clientes)
                .Include(r => r.Viajes)
                .Where(r => r.FechaReserva >= fechaInicio && r.FechaReserva <= fechaFin)
                .ToList();
        }

        /// <summary>
        /// Verifica si un cliente ya tiene una reserva para un viaje específico
        /// </summary>
        public bool ClienteTieneReservaEnViaje(int idCliente, int idViaje)
        {
            return _context.Reservas.Any(r => r.IdCliente == idCliente && r.IdViaje == idViaje);
        }
    }
}
