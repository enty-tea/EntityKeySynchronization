using EntyTea.EntityKeySynchronization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntyTea.EntityKeySynchronization.UnitTests
{
    public class TestEntityIdSynchronizer
    {
        [Test]
        public void Construct_NullIdExtractor()
        {
            Assert.Throws<ArgumentNullException>(() => new EntityIdSynchronizer<Person, int>(null));
        }

        [Test]
        public void SetEntityThenId_IdMatchesEntity()
        {
            var s = new EntityIdSynchronizer<Person, int>(p => p.Id);
            var bob = new Person { Id = 1, Name = "Bob" };
            s.Entity = bob;
            s.Id = 1;

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual(1, s.Id);
            Assert.AreEqual(1, s.IdOrNull);
            Assert.AreEqual(1, s.Key);
        }

        [Test]
        public void SetEntityThenId_IdDoesNotMatchEntity()
        {
            var s = new EntityIdSynchronizer<Person, int>(p => p.Id);
            var bob = new Person { Id = 1, Name = "Bob" };
            s.Entity = bob;
            s.Id = 2;

            Assert.AreEqual(2, s.Id);
            Assert.AreEqual(2, s.IdOrNull);
            Assert.AreEqual(2, s.Key);
            Assert.IsNull(s.Entity);
        }

        private class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }

    public class TestEntityIdSynchronizerBase
    {
        [Test]
        public void NothingSet()
        {
            var s = new PersonIdSynchronizer();

            Assert.IsNull(s.Entity);
            Assert.AreEqual(0, s.Id);
            Assert.IsNull(s.IdOrNull);
            Assert.AreEqual(0, s.Key);
        }

        [Test]
        public void SetEntity()
        {
            var s = new PersonIdSynchronizer();
            var bob = new Person { Id = 1, Name = "Bob" };
            s.Entity = bob;

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual(1, s.Id);
            Assert.AreEqual(1, s.IdOrNull);
            Assert.AreEqual(1, s.Key);
        }

        [Test]
        public void SetId()
        {
            var s = new PersonIdSynchronizer();
            s.Id = 1;

            Assert.AreEqual(1, s.Id);
            Assert.AreEqual(1, s.IdOrNull);
            Assert.AreEqual(1, s.Key);
            Assert.IsNull(s.Entity);
        }

        [Test]
        public void SetNullableId()
        {
            var s = new PersonIdSynchronizer();
            s.IdOrNull = 1;

            Assert.AreEqual(1, s.Id);
            Assert.AreEqual(1, s.IdOrNull);
            Assert.AreEqual(1, s.Key);
            Assert.IsNull(s.Entity);
        }

        [Test]
        public void SetEntityThenId_IdMatchesEntity()
        {
            var s = new PersonIdSynchronizer();
            var bob = new Person { Id = 1, Name = "Bob" };
            s.Entity = bob;
            s.Id = 1;

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual(1, s.Id);
            Assert.AreEqual(1, s.IdOrNull);
            Assert.AreEqual(1, s.Key);
        }


        [Test]
        public void SetEntityThenId_IdDoesNotMatchEntity()
        {
            var s = new PersonIdSynchronizer();
            var bob = new Person { Id = 1, Name = "Bob" };
            s.Entity = bob;
            s.Id = 2;

            Assert.AreEqual(2, s.Id);
            Assert.AreEqual(2, s.IdOrNull);
            Assert.AreEqual(2, s.Key);
            Assert.IsNull(s.Entity);
        }

        [Test]
        public void SetIdThenEntity_IdMatchesEntity()
        {
            var s = new PersonIdSynchronizer();
            var bob = new Person { Id = 1, Name = "Bob" };
            s.Id = 1;
            s.Entity = bob;

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual(1, s.Id);
            Assert.AreEqual(1, s.IdOrNull);
            Assert.AreEqual(1, s.Key);
        }

        [Test]
        public void SetIdThenEntity_IdDoesNotMatchEntity()
        {
            var s = new PersonIdSynchronizer();
            var bob = new Person { Id = 1, Name = "Bob" };
            s.Id = 2;
            s.Entity = bob;

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual(1, s.Id);
            Assert.AreEqual(1, s.IdOrNull);
            Assert.AreEqual(1, s.Key);
        }

        [Test]
        public void SetEntityThenClear()
        {
            var s = new PersonIdSynchronizer();
            var bob = new Person { Id = 1, Name = "Bob" };
            s.Entity = bob;
            s.Entity = null;

            Assert.IsNull(s.Entity);
            Assert.AreEqual(1, s.Id);
            Assert.AreEqual(1, s.IdOrNull);
            Assert.AreEqual(1, s.Key);
        }

        [Test]
        public void SetEntityThenIdNull()
        {
            var s = new PersonIdSynchronizer();
            var bob = new Person { Id = 1, Name = "Bob" };
            s.Entity = bob;
            s.IdOrNull = null;

            Assert.IsNull(s.Entity);
            Assert.AreEqual(0, s.Id);
            Assert.IsNull(s.IdOrNull);
            Assert.AreEqual(0, s.Key);
        }

        [Test]
        public void SetEntityThenIdNull_CustomSentinel()
        {
            var s = new ZeroValidPersonIdSynchronizer();
            var bob = new Person { Id = 1, Name = "Bob" };
            s.Entity = bob;
            s.IdOrNull = null;

            Assert.IsNull(s.Entity);
            Assert.AreEqual(-1, s.Id);
            Assert.IsNull(s.IdOrNull);
            Assert.AreEqual(-1, s.Key);
        }

        [Test]
        public void SerializeThenDeserialize()
        {
            var s = new PersonIdSynchronizer();
            var bob = new Person { Id = 1, Name = "Bob" };
            s.Id = 1;
            s.Entity = bob;

            var clone = SerializeUtils.Clone(s);
            Assert.AreNotSame(s, clone);
            Assert.AreEqual(1, clone.Id);
            Assert.AreEqual(bob.Id, clone.Entity.Id);
            Assert.AreEqual(bob.Name, clone.Entity.Name);
            Assert.AreNotSame(bob, clone.Entity);
        }

        #region Test Classes

        [Serializable]
        private class PersonIdSynchronizer : EntityIdSynchronizerBase<Person, int>
        {
            protected override int GetKeyForEntity(Person entity)
            {
                return entity.Id;
            }
        }

        [Serializable]
        private class ZeroValidPersonIdSynchronizer : EntityIdSynchronizerBase<Person, int>
        {
            protected override int GetKeyForEntity(Person entity)
            {
                return entity.Id;
            }

            protected override int IdSentinel
            {
                get
                {
                    return -1;
                }
            }
        }

        [Serializable]
        private class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion
    }
}
