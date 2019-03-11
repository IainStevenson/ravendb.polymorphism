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
    public class DeleteListener : IDocumentDeleteListener
    {
        private readonly SortedList<Guid, object> _keys;

        public DeleteListener(SortedList<Guid, object> keys)
        {
            _keys = keys;
        }

        public void BeforeDelete(string key, object entityInstance, RavenJObject metadata)
        {
            Trace.WriteLine(string.Format("(Before) Deleting object with key: {0}", key));
            Guid objectKey = entityInstance.GetType().IdFromKey<Denizen>(key);
            if (_keys.ContainsKey(objectKey)) return;
            _keys.Add(objectKey, entityInstance);
        }
    }
}