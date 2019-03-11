using System;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Linq;
using RavenDb.Polymorphism.Model;

namespace RavenDb.Polymorphism.Tests
{
    [TestFixture]
    public class WhenPersistingAConcreteType : GivenAPersistedWorld
    {
        [Test]
        public void ItShouldDelete()
        {
            Dog dog = Denizens.OfType<Dog>().First();
            Assert.IsFalse(DeleteKeys.ContainsKey(dog.Id));
            var existing = Session.Load<Dog>(dog.Id);
            Session.Delete(existing);
            Session.SaveChanges();
            Assert.IsTrue(DeleteKeys.ContainsKey(dog.Id));
        }

        [Test]
        public void ItShouldHaveStored()
        {
            Guid guid = Denizens.OfType<Dog>().First().Id;
            Assert.IsTrue(StoreKeys.ContainsKey(guid));
        }

        [Test]
        public void ItShouldLoad()
        {
            Guid id = Denizens.OfType<Dog>().First(x => !x.Id.In(DeleteKeys.Select(dk => dk.Key))).Id;
            var dog = Session.Load<Dog>(id);
            Assert.IsNotNull(dog);
        }

        [Test]
        public void ItShouldUpdate()
        {
            Dog dog = Denizens.OfType<Dog>().First(x => !x.Id.In(DeleteKeys.Select(dk => dk.Key)));
            Assert.IsFalse(dog.Name.Contains("Super"));
            var existing = Session.Load<Dog>(dog.Id);
            existing.Name = "Super Dog!";
            Session.Store(existing);
            Session.SaveChanges();
            Session = Store.OpenSession();
            Assert.AreEqual("Super Dog!", Session.Load<Dog>(dog.Id).Name);
        }
    }
}