using System;
using System.Linq;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;
using Raven.Imports.Newtonsoft.Json;
using RavenDb.Polymorphism.Model;
using RavenDb.Polymorphism.Persistence;

namespace RavenDb.Polymorphism.Tests
{
    [TestFixture]
    public class RepositoryTests: GivenAPersistedWorld
    {
        [SetUp]
        public void Setup()
        {
            Store = new DocumentStore
            {
                ConnectionStringName = "Raven.Server.Persitence",
                Conventions =
                {
                    CustomizeJsonSerializer = serializer => { serializer.TypeNameHandling = TypeNameHandling.All; },
                    DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite,
                    FindTypeTagName = type => type.TypeStoreName()
                }
            };
            Store.Initialize();
            _unitUnderTest = new Repository(Store);
            _itemUnderTest = new Human {WorldId = _worldId};

            Act();
        }

        [TearDown]
        public void Teardown()
        {
            _unitUnderTest.Dispose();
        }

        private Repository _unitUnderTest;
        private Human _itemUnderTest;
        private readonly Guid _worldId = Guid.NewGuid();


        private Boolean _storeResult = true;

        protected virtual void Act()
        {
            _storeResult &= _unitUnderTest.Store(_itemUnderTest);
            _unitUnderTest.Save();
            _unitUnderTest = new Repository(Store);
        }

        [Test]
        public void ItShouldDeleteTheItem()
        {
            Assert.IsTrue(_unitUnderTest.Delete(_itemUnderTest));
            Assert.DoesNotThrow(() => _unitUnderTest.Save());
        }

        [Test]
        public void ItShouldHaveStoredTheItem()
        {
            Assert.IsTrue(_storeResult);
        }

        [Test]
        public void ItShouldQueryTheItem()
        {
            IQueryable items = _unitUnderTest.Query<Human>(x => x.WorldId == _worldId);
            Assert.IsNotEmpty(items);
        }

        [Test]
        public void ItShouldQuerySubClassItems()
        {
            IQueryable items = _unitUnderTest.Query<Denizen, Animal>(x => x.WorldId == _worldId);
            Assert.IsNotEmpty(items);
            foreach (var item in items)
            {
                Assert.IsInstanceOf<Animal>(item);
            }
        }


        [Test]
        public void ItShouldRetrieveTheItem()
        {
            var item = _unitUnderTest.Load<Human>(_itemUnderTest.Id);
            Assert.IsNotNull(item);
        }

        [Test]
        public void ItShouldUpdateTheItem()
        {
            _itemUnderTest.Name = "Updated";
            Assert.IsTrue(_unitUnderTest.Store(_itemUnderTest));
            Assert.DoesNotThrow(() => _unitUnderTest.Save());
        }
    }
}