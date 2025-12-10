using agenciaViajes.Model;
using agenciaViajes.Model.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgenciaViajes.Controller
{
    /// <summary>
    /// API de Viajes - Capa de Lógica de Negocio (BLL)
    /// Gestiona operaciones CRUD de viajes aplicando validaciones de negocio
    /// </summary>
    public class ViajeAPI
    {
        private readonly ViajesRepositorio _repositorio;

        public ViajeAPI()
        {
            _repositorio = new ViajesRepositorio();
        }

        /// <summary>
        /// Obtiene todos los viajes ordenados por destino
        /// </summary>
        public List<Viajes> ListarViajes()
        {
            return _repositorio.ObtenerTodos().OrderBy(v => v.Destino).ToList();
        }

        /// <summary>
        /// Obtiene un viaje por su ID
        /// </summary>
        public Viajes ObtenerViaje(int idViaje)
        {
            return _repositorio.ObtenerPorId(idViaje);
        }

        /// <summary>
        /// Crea un nuevo viaje con validaciones de negocio
        /// Valida: destino obligatorio, precio > 0, plazas >= 0
        /// </summary>
        public bool CrearViaje(string destino, decimal precio, int plazasDisponibles, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(destino))
            {
                mensajeError = "El destino es obligatorio.";
                return false;
            }

            if (precio <= 0)
            {
                mensajeError = "El precio debe ser mayor que 0.";
                return false;
            }

            if (plazasDisponibles < 0)
            {
                mensajeError = "Las plazas disponibles deben ser un número entero mayor o igual a 0.";
                return false;
            }

            try
            {
                var viaje = new Viajes
                {
                    Destino = destino.Trim(),
                    Precio = precio,
                    PlazasDisponibles = plazasDisponibles
                };

                _repositorio.Insertar(viaje);
                return true;
            }
            catch (Exception ex)
            {
                mensajeError = "Error al crear el viaje: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Modifica un viaje existente con validaciones
        /// </summary>
        public bool ModificarViaje(int idViaje, string destino, decimal precio, int plazasDisponibles, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(destino))
            {
                mensajeError = "El destino es obligatorio.";
                return false;
            }

            if (precio <= 0)
            {
                mensajeError = "El precio debe ser mayor que 0.";
                return false;
            }

            if (plazasDisponibles < 0)
            {
                mensajeError = "Las plazas disponibles deben ser un número entero mayor o igual a 0.";
                return false;
            }

            try
            {
                var viaje = new Viajes
                {
                    IdViaje = idViaje,
                    Destino = destino.Trim(),
                    Precio = precio,
                    PlazasDisponibles = plazasDisponibles
                };

                _repositorio.Actualizar(viaje);
                return true;
            }
            catch (Exception ex)
            {
                mensajeError = "Error al modificar el viaje: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Elimina un viaje validando que no tenga reservas asociadas
        /// </summary>
        public bool EliminarViaje(int idViaje, out string mensajeError)
        {
            mensajeError = string.Empty;

            try
            {
                var reservasRepo = new ReservasRepositorio();
                int numReservas = reservasRepo.ContarPorViaje(idViaje);
                
                if (numReservas > 0)
                {
                    mensajeError = "No se puede eliminar el viaje porque tiene reservas asociadas.";
                    return false;
                }

                _repositorio.Eliminar(idViaje);
                return true;
            }
            catch (Exception ex)
            {
                mensajeError = "Error al eliminar el viaje: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtiene solo los viajes que tienen plazas disponibles
        /// Filtrado realizado en la base de datos
        /// </summary>
        public List<Viajes> ListarViajesDisponibles()
        {
            return _repositorio.ObtenerDisponibles().OrderBy(v => v.Destino).ToList();
        }
    }
}
