using System;

namespace RavenDb.Polymorphism.Model
{
    public abstract class Entity<T>
    {
        protected Entity()
        {
            Created = DateTime.UtcNow;
        }

        public T Id { get; set; }
        public DateTime Created { get; set; }

        public String Type
        {
            get { return GetType().Name; }
        }
    }

    public class World : Entity<Guid>
    {
    }

    public abstract class Denizen : Entity<Guid>
    {
        protected Denizen()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        ///     Found in world
        /// </summary>
        public Guid WorldId { get; set; }

        public String Name { get; set; }
        public String DenizenGroup { get; set; }
    }

    public abstract class Plant : Denizen
    {
        public String PlantGroup { get; set; }
    }

    public abstract class Vegetable : Plant
    {
        public string VegetableGroup { get; set; }
    }

    public abstract class Animal : Denizen
    {
        public string AnimalGroup { get; set; }
    }

    public abstract class Insect : Animal
    {
        public string InsectGroup { get; set; }
    }

    public abstract class Mammal : Animal
    {
        public string MammalGroup { get; set; }
    }

    public class Cabbage : Vegetable
    {
        public string Colour { get; set; }
    }

    public class Carrot : Vegetable
    {
        public string Weight { get; set; }
    }

    public class Onion : Vegetable
    {
        public string Set { get; set; }
    }

    public class Bee : Insect
    {
        public string Hive { get; set; }
    }

    public class Ant : Insect
    {
        public string Colony { get; set; }
    }

    public class Wasp : Insect
    {
        public string Nest { get; set; }
    }

    public class Cat : Mammal
    {
        public string Litter { get; set; }
    }

    public class Dog : Mammal
    {
        public string Pack { get; set; }
    }

    public class Human : Mammal
    {
        public string Clan { get; set; }
    }

    public class Ghost : Human
    {
        public Ghost()
        {
            Deceased = DateTime.UtcNow.AddDays(-1);
        }

        public DateTime? Deceased { get; set; }
        public string Spectre { get; set; }
    }
}