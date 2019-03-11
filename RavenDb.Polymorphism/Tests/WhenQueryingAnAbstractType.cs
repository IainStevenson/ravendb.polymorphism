using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Linq;
using RavenDb.Polymorphism.Model;

namespace RavenDb.Polymorphism.Tests
{
    /// <summary>
    ///     This is where it gets interesting:
    ///     In order to extract e.g. Dog's from Denizens by World, it is necessary to be able to
    ///     query on the fact it is a Dog and of that world.
    ///     The World Id is there, so I had to add a Type property to the base because I don't have access
    ///     in Linq to the Raven-Clr-Type ($type) and it may be of the base (root) type anyway.
    ///     Using info from these:
    ///     http://www.timlabonne.com/2013/08/ravendb-survival-tip-3-handling-polymorphism/
    ///     http://ravendb.net/docs/faq/polymorphism
    ///     https://groups.google.com/forum/#!topic/ravendb/otd0JqLzPpg *****
    ///     Oren Eini
    ///     13/01/2013
    ///     Raven-Clr-Type is for the root type.
    ///     What you are doing is probably done with the $type internal value.
    ///     We _might_ be able to do so, but that is pretty hard.
    ///     Might be easier for you to just add a property to the base class (read only) with the type.
    ///     Then you can query on that easily.
    ///     Hence, following that advice, the Entity base abstract class Has a Type property
    ///     And the need for queryng that Type to extract only the necessary sub classes.
    ///     To extract subclasses from a specific ancestor you must know all of the sub class names
    ///     hence the Type extension method SubClassNames
    /// </summary>
    [TestFixture]
    public class WhenQueryingAnAbstractType : GivenAPersistedWorld
    {
        [Test]
        public void ItShouldRetrieveAllAnimalsFromAnimalsAsTheCorrectType()
        {
            List<Animal> items = Session.Query<Animal>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id
                            && x.Type.In(typeof (Animal).SubClassNames()))
                .Take(1024).ToList();
            TraceStats(Stats);
            Assert.IsNotEmpty(items);
            foreach (Animal item in items)
            {
                Assert.IsInstanceOf<Animal>(item);
                Assert.IsTrue(item.GetType() != typeof (Animal));
                Assert.IsTrue(typeof (Animal).SubClassNames().Contains(item.Type));

                Trace.WriteLine(String.Format("Item {0} of type {1}, Name {2}", item.Id, item.GetType().Name, item.Name));
            }
            Assert.AreEqual(Denizens.Count(x => x is Animal), items.Count());
        }

        [Test]
        public void ItShouldRetrieveAllAnimalsFromDenizensAsTheCorrectType()
        {
            List<Denizen> items = Session.Query<Denizen>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id
                            && x.Type.In(typeof (Animal).SubClassNames())).Take(1024).ToList();

            TraceStats(Stats);
            Assert.IsNotEmpty(items);
            foreach (Denizen item in items)
            {
                Assert.IsInstanceOf<Animal>(item);
                Assert.IsTrue(item.GetType() != typeof (Animal));
                Assert.IsTrue(typeof (Animal).SubClassNames().Contains(item.Type));

                Trace.WriteLine(String.Format("Item {0} of type {1}, Name {2}", item.Id, item.GetType().Name, item.Name));
            }
            Assert.AreEqual(Denizens.Count(x => x is Animal), items.Count());
        }

        [Test]
        public void ItShouldRetrieveAllDenizensAsTheCorrectType()
        {
            List<Denizen> items = Session.Query<Denizen>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id).Take(1024).ToList();
            TraceStats(Stats);
            Assert.IsNotEmpty(items);
            foreach (Denizen item in items)
            {
                Assert.IsInstanceOf<Denizen>(item);
                Assert.IsTrue(item.GetType() != typeof (Denizen));
                Assert.IsTrue(typeof (Denizen).SubClassNames().Contains(item.Type));
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
            Assert.AreEqual(Denizens.Count, items.Count());
        }

        [Test]
        public void ItShouldRetrieveAllDogsFromDenizensAsDog()
        {
            List<Dog> items = Session.Query<Denizen>()
                .AsProjection<Dog>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id
                            && x.Type == typeof (Dog).Name).Take(1024).ToList();

            TraceStats(Stats);
            Assert.IsNotEmpty(items);
            foreach (Dog item in items)
            {
                Assert.IsInstanceOf<Dog>(item);
                Assert.IsTrue(item.GetType() != typeof (Denizen));
                Assert.IsTrue(typeof (Dog).SubClassNames().Contains(item.Type));
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
            Assert.AreEqual(Denizens.Count(x => x is Dog), items.Count());
        }

        [Test]
        public void ItShouldRetrieveAllDogsFromDogsAsDog()
        {
            List<Dog> items = Session.Query<Dog>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id
                            && x.Type == typeof (Dog).Name).Take(1024).ToList();

            IEnumerable<string> subClassNames = typeof (Dog).SubClassNames();
            TraceStats(Stats);
            Assert.IsNotEmpty(items);
            foreach (Dog item in items)
            {
                Assert.IsInstanceOf<Dog>(item);
                Assert.IsTrue(item.GetType() != typeof (Denizen));
                Assert.IsTrue(subClassNames.Contains(item.Type));
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
            Assert.AreEqual(Denizens.Count(x => x is Dog), items.Count());
        }

        [Test]
        public void ItShouldRetrieveAllInsectsromInsectsAsTheCorrectType()
        {
            List<Insect> items = Session.Query<Insect>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id
                            && x.Type.In(typeof (Insect).SubClassNames())).Take(1024).ToList();

            TraceStats(Stats);
            Assert.IsNotEmpty(items);
            foreach (Insect item in items)
            {
                Assert.IsInstanceOf<Insect>(item);
                Assert.IsTrue(item.GetType() != typeof (Insect));
                Assert.IsTrue(typeof (Insect).SubClassNames().Contains(item.Type));
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
            Assert.AreEqual(Denizens.Count(x => x is Insect), items.Count());
        }

        [Test]
        public void ItShouldRetrieveAllMammalsFromMammalsAsTheCorrectType()
        {
            List<Mammal> items = Session.Query<Mammal>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id
                            && x.Type.In(typeof (Mammal).SubClassNames())).Take(1024).ToList();

            TraceStats(Stats);
            Assert.IsNotEmpty(items);
            foreach (Mammal item in items)
            {
                Assert.IsInstanceOf<Mammal>(item);
                Assert.IsTrue(item.GetType() != typeof (Mammal));
                Assert.IsTrue(typeof (Mammal).SubClassNames().Contains(item.Type));
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
            Assert.AreEqual(Denizens.Count(x => x is Mammal), items.Count());
        }

        [Test]
        public void ItShouldRetrieveAllPlantsFromPlantsAsTheCorrectType()
        {
            List<Plant> items = Session.Query<Plant>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id
                            && x.Type.In(typeof (Plant).SubClassNames())).Take(1024).ToList();

            TraceStats(Stats);
            Assert.IsNotEmpty(items);
            foreach (Plant item in items)
            {
                Assert.IsInstanceOf<Plant>(item);
                Assert.IsTrue(item.GetType() != typeof (Plant));
                Assert.IsTrue(typeof (Plant).SubClassNames().Contains(item.Type));
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
            Assert.AreEqual(Denizens.Count(x => x is Plant), items.Count());
        }

        [Test]
        public void ItShouldRetrieveAllVegetablesFromVegetablesAsTheCorrectType()
        {
            List<Vegetable> items = Session.Query<Vegetable>()
                .Statistics(out Stats)
                .Where(x => x.WorldId == World.Id
                            && x.Type.In(typeof (Vegetable).SubClassNames())).Take(1024).ToList();

            TraceStats(Stats);
            Assert.IsNotEmpty(items);
            foreach (Vegetable item in items)
            {
                Assert.IsInstanceOf<Vegetable>(item);
                Assert.IsTrue(item.GetType() != typeof (Vegetable));
                Assert.IsTrue(typeof (Vegetable).SubClassNames().Contains(item.Type));
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
            Assert.AreEqual(Denizens.Count(x => x is Vegetable), items.Count());
        }
    }
}