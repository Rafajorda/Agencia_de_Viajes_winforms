using agenciaViajes.Model;
using agenciaViajes.Model.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AgenciaViajes.Controller
{
    /// <summary>
    /// API de Clientes - Capa de Lógica de Negocio (BLL)
    /// Gestiona operaciones CRUD de clientes aplicando validaciones de negocio
    /// </summary>
    public class ClienteAPI
    {
        private readonly ClientesRepositorio _repositorio;

        public ClienteAPI()
        {
            _repositorio = new ClientesRepositorio();
        }

        /// <summary>
        /// Obtiene todos los clientes ordenados por apellidos
        /// </summary>
        public List<Clientes> ListarClientes()
        {
            return _repositorio.ObtenerTodos().OrderBy(c => c.Apellidos).ToList();
        }

        /// <summary>
        /// Obtiene un cliente por su ID
        /// </summary>
        public Clientes ObtenerCliente(int idCliente)
        {
            return _repositorio.ObtenerPorId(idCliente);
        }

        /// <summary>
        /// Crea un nuevo cliente con validaciones de negocio
        /// Valida: nombre, apellidos, formato de email y unicidad de email
        /// </summary>
        public bool CrearCliente(string nombre, string apellidos, string email, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                mensajeError = "El nombre es obligatorio.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(apellidos))
            {
                mensajeError = "Los apellidos son obligatorios.";
                return false;
            }

            if (!ValidarEmail(email))
            {
                mensajeError = "El formato del email no es válido.";
                return false;
            }

            string emailNormalizado = email.Trim().ToLower();
            if (_repositorio.ExisteEmail(emailNormalizado))
            {
                mensajeError = "Ya existe un cliente registrado con este email.";
                return false;
            }

            try
            {
                var cliente = new Clientes
                {
                    Nombre = nombre.Trim(),
                    Apellidos = apellidos.Trim(),
                    Email = emailNormalizado
                };

                _repositorio.Insertar(cliente);
                return true;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
            {
                if (dbEx.InnerException?.InnerException?.Message.Contains("UNIQUE") == true)
                {
                    mensajeError = "El email ya está registrado en el sistema.";
                }
                else
                {
                    mensajeError = "Error de base de datos: " + dbEx.Message;
                }
                return false;
            }
            catch (Exception ex)
            {
                mensajeError = "Error al crear el cliente: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Edita un cliente existente con validaciones de negocio
        /// Verifica que el email no esté en uso por otro cliente
        /// </summary>
        public bool EditarCliente(int idCliente, string nombre, string apellidos, string email, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                mensajeError = "El nombre es obligatorio.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(apellidos))
            {
                mensajeError = "Los apellidos son obligatorios.";
                return false;
            }

            if (!ValidarEmail(email))
            {
                mensajeError = "El formato del email no es válido.";
                return false;
            }

            string emailNormalizado = email.Trim().ToLower();
            if (_repositorio.ExisteEmail(emailNormalizado, idCliente))
            {
                mensajeError = "El email ya está siendo utilizado por otro cliente.";
                return false;
            }

            try
            {
                var cliente = new Clientes
                {
                    IdCliente = idCliente,
                    Nombre = nombre.Trim(),
                    Apellidos = apellidos.Trim(),
                    Email = emailNormalizado
                };

                _repositorio.Actualizar(cliente);
                return true;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
            {
                if (dbEx.InnerException?.InnerException?.Message.Contains("UNIQUE") == true)
                {
                    mensajeError = "El email ya está registrado en el sistema.";
                }
                else
                {
                    mensajeError = "Error de base de datos: " + dbEx.Message;
                }
                return false;
            }
            catch (Exception ex)
            {
                mensajeError = "Error al editar el cliente: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Elimina un cliente validando que no tenga reservas asociadas
        /// </summary>
        public bool EliminarCliente(int idCliente, out string mensajeError)
        {
            mensajeError = string.Empty;

            try
            {
                var reservasRepo = new ReservasRepositorio();
                int numReservas = reservasRepo.ContarPorCliente(idCliente);
                
                if (numReservas > 0)
                {
                    mensajeError = "No se puede eliminar el cliente porque tiene reservas asociadas.";
                    return false;
                }

                _repositorio.Eliminar(idCliente);
                return true;
            }
            catch (Exception ex)
            {
                mensajeError = "Error al eliminar el cliente: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Valida el formato de un email usando expresiones regulares
        /// Patrón: texto@texto.texto
        /// </summary>
        private bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, patron, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
