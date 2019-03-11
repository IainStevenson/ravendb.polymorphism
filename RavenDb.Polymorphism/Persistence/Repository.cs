using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Raven.Client;
using Raven.Client.Linq;
using RavenDb.Polymorphism.Model;

namespace RavenDb.Polymorphism.Persistence
{
    /// <summary>
    ///     Repository for classes tuned for the Application model using and abstracting RavenDB persitence
    ///     Designed as a disposable object
    ///     Session changes are saved on disposal, or on demand
    ///     Primary: To store retrieve and delete entities
    ///     Secondary: To retrieve all instances of specifc abstract classes of entity. .eg. All Insects
    /// </summary>
    public class Repository : IDisposable
    {
        private readonly IDocumentSession _session;

        public Repository(IDocumentStore store)
        {
            _session = store.OpenSession();
        }

        /// <summary>
        ///     On disposal the session will auto save.
        ///     If a transaction was externally set it remains open for the client to manage
        /// </summary>
        public void Dispose()
        {
            if (_session.Advanced.HasChanges)
            {
                _session.SaveChanges();
            }
            _session.Dispose();
        }

        public void Save()
        {
            if (_session.Advanced.HasChanges)
            {
                _session.SaveChanges();
            }
        }

        /// <summary>
        ///     Create or modify the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Store<T>(T item) where T : Entity<Guid>
        {
            _session.Store(item);
            return true;
        }

        /// <summary>
        ///     Retrieve one or more items.
        ///     <remarks>
        ///         By deault will return only max 128 items.
        ///         Use Linq paging .Take(x) and Skip(y) via IQueryable to modify results
        ///     </remarks>
        /// </summary>
        /// <typeparam name="T">Entity type requested</typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable Query<T>(Func<T, bool> query)
        {
            return _session.Query<T>().AsProjection<T>().Where(query).AsQueryable();
        }


        public IQueryable Query<TStore,TSubClass>(Func<TStore, bool> query) 
            where TStore: Entity<Guid> 
            where TSubClass: Entity<Guid>
        {
            IEnumerable<string> subClassNames = typeof(TSubClass).SubClassNames();
            return _session.Query<TStore>()
                .Where(query)
                .Where( x=>x.Type.In(subClassNames))
                .AsQueryable();
        }

        /// <summary>
        ///     Delete the item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Delete<T>(T item) where T : Entity<Guid>
        {
            try
            {
                // try a direct delete in case the item was previously loaded by this session.
                _session.Delete(item);
            }
            catch (InvalidOperationException ex)
            {
                var loadedItem = _session.Load<T>(item.Id);
                if (loadedItem != null)
                {
                    _session.Delete(loadedItem);
                    return true;
                }
                return false;
            }

            return true;
        }

        public T Load<T>(Guid id)
        {
            return _session.Load<T>(id);
        }
    }
}