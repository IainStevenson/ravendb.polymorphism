using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using RavenDb.Polymorphism.Model;

namespace RavenDb.Polymorphism.Tests
{
    public class DenizenIndex : AbstractMultiMapIndexCreationTask<DenizenIndex.Result>
    {
        public DenizenIndex()
        {
            AddMap<Cat>(items => from item in items
                select
                    new Result
                    {
                        WorldId = item.WorldId,
                        Content = new object[] {item.DenizenGroup, item.AnimalGroup, item.MammalGroup, item.Litter}
                    });
            AddMap<Dog>(items => from item in items
                select
                    new Result
                    {
                        WorldId = item.WorldId,
                        Content = new object[] {item.DenizenGroup, item.AnimalGroup, item.MammalGroup, item.Pack}
                    });
            AddMap<Human>(items => from item in items
                select
                    new Result
                    {
                        WorldId = item.WorldId,
                        Content = new object[] {item.DenizenGroup, item.AnimalGroup, item.MammalGroup, item.Clan}
                    });
            AddMap<Ghost>(items => from item in items
                select
                    new Result
                    {
                        WorldId = item.WorldId,
                        Content =
                            new object[]
                            {item.DenizenGroup, item.AnimalGroup, item.MammalGroup, item.Clan, item.Spectre}
                    });

            AddMap<Ant>(items => from item in items
                select
                    new Result
                    {
                        WorldId = item.WorldId,
                        Content = new object[] {item.DenizenGroup, item.AnimalGroup, item.InsectGroup, item.Colony}
                    });
            AddMap<Bee>(items => from item in items
                select
                    new Result
                    {
                        WorldId = item.WorldId,
                        Content = new object[] {item.DenizenGroup, item.AnimalGroup, item.InsectGroup, item.Hive}
                    });
            AddMap<Wasp>(items => from item in items
                select
                    new Result
                    {
                        WorldId = item.WorldId,
                        Content = new object[] {item.DenizenGroup, item.AnimalGroup, item.InsectGroup, item.Nest}
                    });

            AddMap<Carrot>(items => from item in items
                select
                    new Result
                    {
                        WorldId = item.WorldId,
                        Content = new object[] {item.DenizenGroup, item.PlantGroup, item.VegetableGroup, item.Weight}
                    });
            AddMap<Cabbage>(items => from item in items
                select
                    new Result
                    {
                        WorldId = item.WorldId,
                        Content = new object[] {item.DenizenGroup, item.PlantGroup, item.VegetableGroup, item.Colour}
                    });
            AddMap<Onion>(items => from item in items
                select
                    new Result
                    {
                        WorldId = item.WorldId,
                        Content = new object[] {item.DenizenGroup, item.PlantGroup, item.VegetableGroup, item.Set}
                    });

            Index(x => x.WorldId, FieldIndexing.Analyzed);
            Index(x => x.Content, FieldIndexing.Analyzed);
        }

        public class Result
        {
            public Guid WorldId { get; set; }
            public object[] Content { get; set; }
        }
    }
}