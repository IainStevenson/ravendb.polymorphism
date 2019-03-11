using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using RavenDb.Polymorphism.Model;

namespace RavenDb.Polymorphism.Tests
{
    public class WhenSearching : GivenAPersistedWorld
    {
        [Test]
        public void ItShouldReturnAllOnions()
        {
            string searchTerms = "Set";
            Trace.WriteLine(String.Format("Search terms: {0}", searchTerms));

            List<Denizen> searchResults = Session.Advanced
                .LuceneQuery<Denizen, DenizenIndex>()
                .Statistics(out Stats)
                .Search("Content", searchTerms).ToList();
            TraceStats(Stats);


            Assert.IsNotEmpty(searchResults);
            foreach (Denizen item in searchResults)
            {
                Assert.IsInstanceOf<Onion>(item);
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
        }


        [Test]
        public void ItShouldReturnAllInsects()
        {
            string searchTerms = "Insects";
            Trace.WriteLine(String.Format("Search terms: {0}", searchTerms));

            List<Insect> searchResults = Session.Advanced
                .LuceneQuery<Insect, DenizenIndex>()
                .Statistics(out Stats)
                .Search("Content", searchTerms)
                .ToList();
            TraceStats(Stats);

            Assert.IsNotEmpty(searchResults);
            foreach (Insect item in searchResults)
            {
                Assert.IsInstanceOf<Insect>(item);
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
        }


        [Test]
        public void ItShouldReturnAllDogs()
        {
            string searchTerms = "Pack";
            Trace.WriteLine(String.Format("Search terms: {0}", searchTerms));

            List<Dog> searchResults = Session.Advanced
                .LuceneQuery<Dog, DenizenIndex>()
                .Statistics(out Stats)
                .Search("Content", searchTerms)
                .ToList();
            TraceStats(Stats);

            Assert.IsNotEmpty(searchResults);
            foreach (Dog item in searchResults)
            {
                Assert.IsInstanceOf<Dog>(item);
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
        }


        [Test]
        public void ItShouldReturnADog()
        {
            string searchTerms = Denizens.OfType<Dog>().First().Name;
            Trace.WriteLine(String.Format("Search terms: {0}", searchTerms));
            List<Dog> searchResults = Session.Advanced
                .LuceneQuery<Dog, DenizenIndex>()
                .Statistics(out Stats)
                .Search("Content", searchTerms)
                .ToList();
            TraceStats(Stats);

            Assert.IsNotEmpty(searchResults);
            foreach (Dog item in searchResults)
            {
                Assert.IsInstanceOf<Dog>(item);
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
        }

        [Test]
        public void ItShouldReturnAnimals()
        {
            string searchTerms = "Dog*";
            Trace.WriteLine(String.Format("Search terms: {0}", searchTerms));

            List<Denizen> searchResults = Session.Advanced
                .LuceneQuery<Denizen, DenizenIndex>()
                .Statistics(out Stats)
                .Search("Content", searchTerms)
                .ToList();
            TraceStats(Stats);

            Assert.IsNotEmpty(searchResults);
            foreach (Denizen item in searchResults)
            {
                Assert.IsInstanceOf<Denizen>(item);
                Trace.WriteLine(String.Format("Item {0} of type {1} Name {2}", item.Id, item.GetType().Name, item.Name));
            }
        }
    }
}