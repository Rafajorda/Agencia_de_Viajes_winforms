using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace agenciaViajes.Model.Repositorios
{
    /// <summary>
    /// Interfaz genérica para repositorios
    /// Define el contrato común de operaciones CRUD
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public interface IRepositorio<T> where T : class
    {
        /// <summary>
        /// Obtiene todas las entidades
        /// </summary>
        List<T> ObtenerTodos();

        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        T ObtenerPorId(int id);

        /// <summary>
        /// Busca entidades que cumplan una condición
        /// </summary>
        /// <param name="predicado">Expresión lambda con la condición</param>
        List<T> Buscar(Expression<Func<T, bool>> predicado);

        /// <summary>
        /// Inserta una nueva entidad
        /// </summary>
        void Insertar(T entidad);

        /// <summary>
        /// Actualiza una entidad existente
        /// </summary>
        void Actualizar(T entidad);

        /// <summary>
        /// Elimina una entidad por su ID
        /// </summary>
        void Eliminar(int id);

        /// <summary>
        /// Verifica si existe alguna entidad que cumpla una condición
        /// </summary>
        bool Existe(Expression<Func<T, bool>> predicado);

        /// <summary>
        /// Cuenta el número de entidades que cumplen una condición
        /// </summary>
        int Contar(Expression<Func<T, bool>> predicado = null);
    }
}
