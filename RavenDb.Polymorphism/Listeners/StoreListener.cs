using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Raven.Client.Listeners;
using Raven.Json.Linq;
using RavenDb.Polymorphism.Model;

namespace RavenDb.Polymorphism.Listeners
{
    [ExcludeFromCodeCoverage]
    public class StoreListener : IDocumentStoreListener
    {
        private readonly SortedList<Guid, object> _keys;

        public StoreListener(SortedList<Guid, object> keys)
        {
            _keys = keys;
        }

        public bool BeforeStore(string key, object entityInstance, RavenJObject metadata, RavenJObject original)
        {
            // This happens on execution of the Session.Store command
            Trace.WriteLine(string.Format("(Before) Storing object with key: {0} of type {1}", key,
                entityInstance.GetType().Name));
            return true;
        }

        public void AfterStore(string key, object entityInstance, RavenJObject metadata)
        {
            // this happens after execution of the Session.SaveChanges command
            Trace.WriteLine(string.Format("(After) Storing object with key: {0}", key));
            Guid objectKey = entityInstance.GetType().IdFromKey<Denizen>(key);
            if (_keys.ContainsKey(objectKey)) return;
            _keys.Add(objectKey, entityInstance);
        }
    }
}