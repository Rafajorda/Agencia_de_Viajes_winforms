using System;
using System.Linq;

namespace agenciaViajes.Model.Repositorios
{
    /// <summary>
    /// Repositorio de Clientes
    /// Hereda operaciones CRUD base y agrega métodos específicos
    /// </summary>
    public class ClientesRepositorio : RepositorioBase<Clientes>
    {
        public ClientesRepositorio() : base(new AgenciaViajesEntities())
        {
        }

        public ClientesRepositorio(AgenciaViajesEntities context) : base(context)
        {
        }

        public override void Actualizar(Clientes cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            var clienteExistente = _context.Clientes.Find(cliente.IdCliente);
            
            if (clienteExistente != null)
            {
                clienteExistente.Nombre = cliente.Nombre;
                clienteExistente.Apellidos = cliente.Apellidos;
                clienteExistente.Email = cliente.Email;
                
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Verifica si existe un cliente con el email especificado
        /// </summary>
        /// <param name="email">Email a verificar</param>
        /// <param name="excluyendoId">ID a excluir de la búsqueda (útil para ediciones)</param>
        public bool ExisteEmail(string email, int? excluyendoId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var query = _context.Clientes.Where(c => c.Email == email);

            if (excluyendoId.HasValue)
            {
                query = query.Where(c => c.IdCliente != excluyendoId.Value);
            }

            return query.Any();
        }

        /// <summary>
        /// Busca clientes por nombre o apellidos con coincidencia parcial
        /// </summary>
        public System.Collections.Generic.List<Clientes> BuscarPorNombre(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return ObtenerTodos();

            return _context.Clientes
                .Where(c => c.Nombre.Contains(termino) || c.Apellidos.Contains(termino))
                .ToList();
        }
    }
}
