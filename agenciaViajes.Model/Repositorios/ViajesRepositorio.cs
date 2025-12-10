using System;
using System.Collections.Generic;
using System.Linq;

namespace agenciaViajes.Model.Repositorios
{
    /// <summary>
    /// Repositorio de Viajes
    /// Hereda operaciones CRUD base y agrega métodos específicos para gestión de plazas
    /// </summary>
    public class ViajesRepositorio : RepositorioBase<Viajes>
    {
        public ViajesRepositorio() : base(new AgenciaViajesEntities())
        {
        }

        public ViajesRepositorio(AgenciaViajesEntities context) : base(context)
        {
        }

        public override void Actualizar(Viajes viaje)
        {
            if (viaje == null)
                throw new ArgumentNullException(nameof(viaje));

            var viajeExistente = _context.Viajes.Find(viaje.IdViaje);
            
            if (viajeExistente != null)
            {
                viajeExistente.Destino = viaje.Destino;
                viajeExistente.Precio = viaje.Precio;
                viajeExistente.PlazasDisponibles = viaje.PlazasDisponibles;
                
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene solo viajes con plazas disponibles (> 0)
        /// </summary>
        public List<Viajes> ObtenerDisponibles()
        {
            return _context.Viajes
                .Where(v => v.PlazasDisponibles > 0)
                .ToList();
        }

        /// <summary>
        /// Actualiza las plazas disponibles de un viaje
        /// </summary>
        /// <param name="idViaje">ID del viaje</param>
        /// <param name="cantidad">Cantidad a sumar/restar (negativo para reservar, positivo para cancelar)</param>
        public void ActualizarPlazas(int idViaje, int cantidad)
        {
            var viaje = _context.Viajes.Find(idViaje);
            
            if (viaje != null)
            {
                viaje.PlazasDisponibles += cantidad;
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene las plazas disponibles de un viaje sin cargar la entidad completa
        /// </summary>
        public int ObtenerPlazasDisponibles(int idViaje)
        {
            var viaje = _context.Viajes.Find(idViaje);
            return viaje != null ? viaje.PlazasDisponibles : 0;
        }

        /// <summary>
        /// Busca viajes por destino con coincidencia parcial
        /// </summary>
        public List<Viajes> BuscarPorDestino(string destino)
        {
            if (string.IsNullOrWhiteSpace(destino))
                return ObtenerTodos();

            return _context.Viajes
                .Where(v => v.Destino.Contains(destino))
                .ToList();
        }

        /// <summary>
        /// Obtiene viajes dentro de un rango de precios
        /// </summary>
        public List<Viajes> ObtenerPorRangoPrecio(decimal precioMin, decimal precioMax)
        {
            return _context.Viajes
                .Where(v => v.Precio >= precioMin && v.Precio <= precioMax)
                .ToList();
        }
    }
}
