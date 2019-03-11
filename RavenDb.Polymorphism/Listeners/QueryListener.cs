using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Raven.Client;
using Raven.Client.Listeners;

namespace RavenDb.Polymorphism.Listeners
{
    [ExcludeFromCodeCoverage]
    public class QueryListener : IDocumentQueryListener
    {
        private readonly List<DateTime> _keys;

        public QueryListener(List<DateTime> keys)
        {
            _keys = keys;
        }

        public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
        {
            Trace.WriteLine(string.Format("Query at {0}", DateTime.Now));
            queryCustomization.WaitForNonStaleResultsAsOfNow();
            _keys.Add(DateTime.Now);
        }
    }
}