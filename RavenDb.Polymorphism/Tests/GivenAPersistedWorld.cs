using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Imports.Newtonsoft.Json;
using RavenDb.Polymorphism.Listeners;
using RavenDb.Polymorphism.Model;

namespace RavenDb.Polymorphism.Tests
{
    public abstract class GivenAPersistedWorld
    {
        private static int _denizenIndex;

        /// <summary>
        ///     A list of item Id's and objects deleted, written by the Delete Listener
        /// </summary>
        protected SortedList<Guid, object> DeleteKeys;

        protected List<Denizen> Denizens;
        protected IDocumentSession Session;

        /// <summary>
        ///     A list of Item Id's and objects stored, written by the Store Listener
        /// </summary>
        protected SortedList<Guid, object> StoreKeys;

        protected World World;
        private DeleteListener _deleteListener;
        private Denizen _denizen;
        private List<DateTime> _queryKeys;
        private QueryListener _queryListener;
        protected RavenQueryStatistics Stats;
        protected IDocumentStore Store;
        private StoreListener _storeListener;

        [TestFixtureSetUp]
        public void Setup()
        {
            World = new World {Id = Guid.NewGuid()};

            StoreKeys = new SortedList<Guid, object>();
            DeleteKeys = new SortedList<Guid, object>();
            _queryKeys = new List<DateTime>();
            _storeListener = new StoreListener(StoreKeys);
            _deleteListener = new DeleteListener(DeleteKeys);
            _queryListener = new QueryListener(_queryKeys);

            Store = new EmbeddableDocumentStore
            {
                //ConnectionStringName = "Raven.Server",
                Configuration =
                {
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    RunInMemory = true,
                },
                Conventions =
                {
                    CustomizeJsonSerializer = serializer => { serializer.TypeNameHandling = TypeNameHandling.All; },
                    DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite,
                    FindTypeTagName = type => type.TypeStoreName()
                }
            };
            Store.Initialize();
            IndexCreation.CreateIndexes(typeof (DenizenIndex).Assembly, Store);

            (Store as DocumentStore).RegisterListener(_storeListener);
            (Store as DocumentStore).RegisterListener(_deleteListener);
            (Store as DocumentStore).RegisterListener(_queryListener);
            Session = Store.OpenSession();

            Act();
            Session.Dispose();
            Session = Store.OpenSession();
        }

        /// <summary>
        ///     Produces a mixed bag of denizens for a new world
        /// </summary>
        private void Act()
        {
            Session.Store(World);
            Denizens = new List<Denizen>();
            int sets = 10;
            int modelTypes = typeof (Denizen).SubClassNames().Count() - 4; // subtact abstract class count

            for (int i = 0; i <= modelTypes*sets; i++)
            {
                int type = i <= 11 ? i : i%11;
                _denizen = Create(type, World.Id);
                Session.Store(_denizen);
                Denizens.Add(_denizen);
            }
            Session.SaveChanges();
        }

        private static Denizen Create(int type, Guid worldId)
        {
            _denizenIndex++;

            switch (type)
            {
                case 1:
                    return new Onion
                    {
                        DenizenGroup = String.Format("Plants {0}", _denizenIndex),
                        PlantGroup = String.Format("Vegetables {0}", _denizenIndex),
                        VegetableGroup = String.Format("Onions {0}", _denizenIndex),
                        Set = String.Format("Set {0}", _denizenIndex),
                        Name = String.Format("Onion {0}", _denizenIndex),
                        WorldId = worldId,
                    };

                case 2:

                    return new Dog
                    {
                        DenizenGroup = String.Format("Animals {0}", _denizenIndex),
                        AnimalGroup = String.Format("Mammals {0}", _denizenIndex),
                        MammalGroup = String.Format("Dogs {0}", _denizenIndex),
                        Pack = String.Format("Pack {0}", _denizenIndex),
                        Name = String.Format("This is Dog {0}", _denizenIndex),
                        WorldId = worldId,
                    };

                case 3:

                    return new Cat
                    {
                        DenizenGroup = String.Format("Animals {0}", _denizenIndex),
                        AnimalGroup = String.Format("Mammals {0}", _denizenIndex),
                        MammalGroup = String.Format("Cats {0}", _denizenIndex),
                        Litter = String.Format("Litter {0}", _denizenIndex),
                        Name = String.Format("This is Cat {0}", _denizenIndex),
                        WorldId = worldId,
                    };

                case 4:

                    return new Human
                    {
                        DenizenGroup = String.Format("Animals {0}", _denizenIndex),
                        AnimalGroup = String.Format("Mammals {0}", _denizenIndex),
                        MammalGroup = String.Format("Humans {0}", _denizenIndex),
                        Clan = String.Format("Clan {0}", _denizenIndex),
                        Name = String.Format("Human {0}", _denizenIndex),
                        WorldId = worldId,
                    };

                case 5:

                    return new Cabbage
                    {
                        DenizenGroup = String.Format("Plants {0}", _denizenIndex),
                        PlantGroup = String.Format("Vegetables {0}", _denizenIndex),
                        VegetableGroup = String.Format("Cabbages {0}", _denizenIndex),
                        Colour = String.Format("Colour {0}", _denizenIndex),
                        Name = String.Format("Cabbage {0}", _denizenIndex),
                        WorldId = worldId,
                    };


                case 6:

                    return new Bee
                    {
                        DenizenGroup = String.Format("Animals {0}", _denizenIndex),
                        AnimalGroup = String.Format("Insects {0}", _denizenIndex),
                        InsectGroup = String.Format("Bees {0}", _denizenIndex),
                        Hive = String.Format("Hive {0}", _denizenIndex),
                        Name = String.Format("Bee {0}", _denizenIndex),
                        WorldId = worldId,
                    };

                case 7:

                    return new Ant
                    {
                        DenizenGroup = String.Format("Animals {0}", _denizenIndex),
                        AnimalGroup = String.Format("Insects {0}", _denizenIndex),
                        InsectGroup = String.Format("Ants {0}", _denizenIndex),
                        Colony = String.Format("Colony {0}", _denizenIndex),
                        Name = String.Format("Ant {0}", _denizenIndex),
                        WorldId = worldId,
                    };

                case 8:

                    return new Wasp
                    {
                        DenizenGroup = String.Format("Animals {0}", _denizenIndex),
                        AnimalGroup = String.Format("Insects {0}", _denizenIndex),
                        InsectGroup = String.Format("Wasps {0}", _denizenIndex),
                        Nest = String.Format("Nest {0}", _denizenIndex),
                        Name = String.Format("Wasp {0}", _denizenIndex),
                        WorldId = worldId,
                    };

                case 9:

                    return new Carrot
                    {
                        DenizenGroup = String.Format("Plants {0}", _denizenIndex),
                        PlantGroup = String.Format("Vegetables {0}", _denizenIndex),
                        VegetableGroup = String.Format("Carrots {0}", _denizenIndex),
                        Weight = String.Format("Weight {0}", _denizenIndex),
                        Name = String.Format("Carrot {0}", _denizenIndex),
                        WorldId = worldId,
                    };

                case 10:
                case 0:
                default:
                    return new Ghost
                    {
                        WorldId = worldId,
                        DenizenGroup = String.Format("Animals {0}", _denizenIndex),
                        AnimalGroup = String.Format("Mammals {0}", _denizenIndex),
                        MammalGroup = String.Format("Former-Humans {0}", _denizenIndex),
                        Name = String.Format("Ghost {0}", _denizenIndex),
                        Spectre = String.Format("Spectre {0}", _denizenIndex)
                    };
            }
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            Session.Dispose();
            Store.Dispose();
        }

        protected static void TraceStats(RavenQueryStatistics stats)
        {
            Trace.WriteLine(
                String.Format(
                    "\r\n\r\nStatistics DurationMilliseconds {0} IndexEtag {1} IndexName {2} IndexTimestamp {3} Timestamp {4} Total Results {5} Is Stale {6}, Last Query Time {7}\r\n\r\n",
                    stats.DurationMilliseconds, stats.IndexEtag, stats.IndexName, stats.IndexTimestamp
                    , stats.Timestamp, stats.TotalResults, stats.IsStale, stats.LastQueryTime));
        }
    }
}