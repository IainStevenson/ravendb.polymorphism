using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using RavenDb.Polymorphism.Model;

namespace RavenDb.Polymorphism.Tests
{
    [TestFixture]
    public class WhenQueryingAConcreteType : GivenAPersistedWorld
    {
        [Test]
        public void ItShouldRetrieveAllDogsFromAnimal()
        {
            List<Animal> items = Session.Query<Animal>().Statistics(out Stats)
                .Where(x => x.WorldId == World.Id && x.Type == (typeof (Dog).Name)).ToList();
            TraceStats(Stats);
            Assert.AreEqual(Denizens.Count(x => x is Dog), items.Count());
            foreach (Animal item in items)
            {
                Trace.WriteLine(String.Format("Retrieved item of type {0}", item.GetType().Name));
            }
        }

        [Test]
        public void ItShouldRetrieveAllDogsFromDenizens()
        {
            List<Denizen> items = Session.Query<Denizen>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id &&
                            x.Type == (typeof (Dog).Name)).ToList();
            TraceStats(Stats);
            Assert.AreEqual(Denizens.Count(x => x is Dog), items.Count());
            foreach (Denizen item in items)
            {
                Trace.WriteLine(String.Format("Retrieved item of type {0}", item.GetType().Name));
            }
        }


        [Test]
        public void ItShouldRetrieveAllDogsFromDogs()
        {
            List<Dog> items = Session.Query<Dog>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id
                            && x.Type == (typeof (Dog).Name))
                .ToList();
            TraceStats(Stats);
            Assert.AreEqual(Denizens.Count(x => x is Dog), items.Count());
            foreach (Dog item in items)
            {
                Trace.WriteLine(String.Format("Retrieved item of type {0}", item.GetType().Name));
            }
        }

        [Test]
        public void ItShouldRetrieveAllDogsFromMammals()
        {
            List<Mammal> items = Session.Query<Mammal>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id &&
                            x.Type == (typeof (Dog).Name)).ToList();
            TraceStats(Stats);
            Assert.AreEqual(Denizens.Count(x => x is Dog), items.Count());
            foreach (Mammal item in items)
            {
                Trace.WriteLine(String.Format("Retrieved item of type {0}", item.GetType().Name));
            }
        }
    }
}