using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NHibernate;
using NHibernate.Criterion;

namespace Document_storage_v2.Models
{
    public class Repository<T> :
        IRepository<T>, IEnumerable<T>
        where T : class, IEntity, new()
    {

        #region [Свойства]

        /// <summary>
        /// Текущая выполняемая транзакция
        /// </summary>
        public ITransaction CurrentTransaction
            => session?.Transaction;

        /// <summary>
        /// Все элементы данного репозитория
        /// </summary>
        public ICriteria Elements
            => CreateCriteria();

        #endregion

        protected ISession session;

        /// <summary>
        /// Выполняет указанное действие как транзакцию с данным репозиторием
        /// </summary>
        /// <param name="action">Действие для выполнения</param>
        public void Transaction(Action<IRepository<T>> action)
        {
            if (action != null)
            {
                if (CurrentTransaction?.IsActive ?? false)
                    throw new InvalidOperationException("This NHibernate session has active transaction already");

                using (var transaction = session.BeginTransaction())
                {
                    action(this);
                    transaction.Commit();
                }
            }
        }
        public ICriteria CreateCriteria(string alias = default)
           => alias == default ?
               session.CreateCriteria<T>() :
               session.CreateCriteria<T>(alias);

        public void Save(T entity, Action<T> action = default)
            => entity.ApplyTo(e => session.Save(e), action);

        public void Update(T entity, Action<T> action = default)
            => entity.ApplyTo(e => session.Update(e), action);


        public void Delete(T entity)
           => entity.ApplyTo(e => session.Delete(e), default);

        public T Create(Action<T> init = default, bool singleTransaction = true)
        {
            var obj = new T();

            void SaveInit(IRepository<T> repo)
            {
                init?.Invoke(obj);
                session.SaveOrUpdate(obj);
            }

            if (singleTransaction)
                Transaction(SaveInit);
            else
                SaveInit(this);
           
            return obj;
        }

        public bool Get(long? id, out T value)
        {
            value = this[id];
            return !(value == default(T));
        }

        public IList<T> Get(string criteriaAlias = default, Func<ICriteria, ICriteria> transform = default, params ICriterion[] criterions)
            =>  // выбор преобразования
                (transform ?? ((ICriteria c) => c)) 
                // добавление критериев и применение преобразования
                (criterions.Aggregate(CreateCriteria(criteriaAlias), 
                    (criteria, criterion) => criterion == default(ICriterion)?
                        criteria : criteria.Add(criterion)))
                // представление в виде списка
                ?.List<T>();

        public long Count(ICriterion criterion, IProjection countProjetion = default, string propertyName = default)
           => CreateCriteria()
                   .Add(criterion)
                   .SetProjection(countProjetion ?? Projections.Count(propertyName))
                   .UniqueResult<long>();

       



        #region [IEnumerable<T>]

        public IEnumerator<T> GetEnumerator()
           => Elements.List<T>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion

        #region [Индексаторы]

        public T this[long? id]
          => id.HasValue ?
                session.Get<T>(id) :
                  default;

        public T this[string id]
            => long.TryParse(id, out var numericId) ?
                    this[numericId] :
                    default;

        #endregion

        public virtual void Dispose()
        {
            if (session.IsOpen)
            {
                if (CurrentTransaction.IsActive)
                    CurrentTransaction.Dispose();

                session.Close();
                session.Dispose();
            }
        }

        public Repository(ISession nhSession)
            => session = nhSession;
    }
}