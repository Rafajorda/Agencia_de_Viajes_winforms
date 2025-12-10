using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace agenciaViajes.Model.Repositorios
{
    /// <summary>
    /// Implementación base genérica del patrón Repository
    /// Proporciona funcionalidad CRUD común para todas las entidades
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public abstract class RepositorioBase<T> : IRepositorio<T> where T : class
    {
        protected readonly AgenciaViajesEntities _context;
        protected readonly DbSet<T> _dbSet;

        protected RepositorioBase(AgenciaViajesEntities context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public virtual List<T> ObtenerTodos()
        {
            return _dbSet.ToList();
        }

        public virtual T ObtenerPorId(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual List<T> Buscar(Expression<Func<T, bool>> predicado)
        {
            return _dbSet.Where(predicado).ToList();
        }

        public virtual void Insertar(T entidad)
        {
            if (entidad == null)
                throw new ArgumentNullException(nameof(entidad));

            _dbSet.Add(entidad);
            _context.SaveChanges();
        }

        /// <summary>
        /// Método abstracto - Los repositorios específicos deben implementarlo
        /// para actualizar propiedades específicas de su entidad
        /// </summary>
        public abstract void Actualizar(T entidad);

        public virtual void Eliminar(int id)
        {
            var entidad = ObtenerPorId(id);
            if (entidad != null)
            {
                _dbSet.Remove(entidad);
                _context.SaveChanges();
            }
        }

        public virtual bool Existe(Expression<Func<T, bool>> predicado)
        {
            return _dbSet.Any(predicado);
        }

        public virtual int Contar(Expression<Func<T, bool>> predicado = null)
        {
            return predicado == null ? _dbSet.Count() : _dbSet.Count(predicado);
        }
    }
}
