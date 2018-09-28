using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;

namespace Document_storage_v2.Models
{
    public interface IRepository<T>  :
        IDisposable
        where T : IEntity, new()
    {
        /// <summary>
        /// Обноволяет состояние указанного объекта
        /// </summary>
        /// <param name="entity">Обноволяемый объект</param>
        void Update(T entity, Action<T> action = default);

        /// <summary>
        /// Сохраняет состояние объекта в репозитории
        /// </summary>
        void Save(T entity, Action<T> action = default);

        /// <summary>
        /// Удаляет указанный объект из репозитория
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// Создает объект в данном репозитории иинициализирует при помощи метода <paramref name="init"/>
        /// </summary>
        /// <param name="init">Метод инициализации объекта</param>
        /// <returns></returns>
        T Create(Action<T> init = default, bool singleTransaction = true);

        /// <summary>
        /// Возвращает объект по его id
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Get(long? id, out T value);

        /// <summary>
        /// Возвращает все элементы, удовлетворяющие заданным критериям
        /// </summary>
        /// <param name="criteriaAlias">Всевдоним критерии</param>
        /// <param name="criterions"></param>
        /// <returns></returns>
        IList<T> Get(string criteriaAlias = default, Func<ICriteria, ICriteria> transform = default, params ICriterion[] criterions);

        /// <summary>
        /// Возвращает количество элементов, удовлетворяющих заданным условиям
        /// </summary>
        /// <param name="criterion">Критерий отбора элементов</param>
        /// <param name="countProjetion">Проекция счетного свойства</param>
        /// <param name="propertyName">Имя счетного свойства</param>
        /// <returns></returns>
        long Count(ICriterion criterion, IProjection countProjetion = default, string propertyName = default);

        /// <summary>
        /// Создать критерий поиска без условий
        /// </summary>
        /// <param name="alias">Псевдоним для критерия поиска</param>
        /// <returns></returns>
        ICriteria CreateCriteria(string alias = default);

        /// <summary>
        /// Выполнить транзакцию, заданную следующим действием
        /// </summary>
        /// <param name="action">Действие, выполняемое как транзакция</param>
        void Transaction(Action<IRepository<T>> action);

        /// <summary>
        /// Возвращает объект по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор искомого объекта</param>
        /// <returns></returns>
        T this[long? id] { get; }

        /// <summary>
        /// Возвращает объект по строковому идентификатору
        /// </summary>
        /// <param name="id">Строковый идентификатор</param>
        /// <returns>Если id не указан, возвращается null</returns>
        T this[string id] { get; }
    }
}