using EntyTea.EntityKeySynchronization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntyTea.EntityKeySynchronization.UnitTests
{
    public class TestEntityKeySynchronizer
    {
        [Test]
        public void Construct_NullKeyExtractor()
        {
            Assert.Throws<ArgumentNullException>(() => new EntityKeySynchronizer<Person, int>(null));
        }

        [Test]
        public void SetEntityThenKey_KeyMatchesEntity()
        {
            var s = new EntityKeySynchronizer<Person, string>(p => p.Name);
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;
            s.Key = "Bob";

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual("Bob", s.Key);
        }

        [Test]
        public void SetEntityThenKey_KeyDoesNotMatchEntity()
        {
            var s = new EntityKeySynchronizer<Person, string>(p => p.Name);
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;
            s.Key = "Robert";

            Assert.AreEqual("Robert", s.Key);
            Assert.IsNull(s.Entity);
        }

        [Test]
        public void SetEntityThenKey_KeyMatchesEntityDifferentCase_DefaultComparer()
        {
            var s = new EntityKeySynchronizer<Person, string>(p => p.Name);
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;
            s.Key = "bob";

            Assert.AreEqual("bob", s.Key);
            Assert.IsNull(s.Entity);
        }

        [Test]
        public void SetEntityThenKey_KeyMatchesEntityDifferentCase_CaseInsensitiveComparer()
        {
            var s = new EntityKeySynchronizer<Person, string>(p => p.Name, new CaseInsensitiveKeyComparer());
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;
            s.Key = "bob";

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual("Bob", s.Key);
        }

        private class CaseInsensitiveKeyComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.ToUpper().GetHashCode();
            }
        }

        private class Person
        {
            public string Name { get; set; }
        }
    }

    public class TestEntityKeySynchronizerBase
    {
        [Test]
        public void NothingSet()
        {
            var s = new PersonNameSynchronizer();

            Assert.IsNull(s.Entity);
            Assert.IsNull(s.Key);
        }

        [Test]
        public void SetEntity()
        {
            var s = new PersonNameSynchronizer();
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual("Bob", s.Key);
        }

        [Test]
        public void SetKey()
        {
            var s = new PersonNameSynchronizer();
            s.Key = "Bob";

            Assert.AreEqual("Bob", s.Key);
            Assert.IsNull(s.Entity);
        }

        [Test]
        public void SetEntityThenKey_KeyMatchesEntity()
        {
            var s = new PersonNameSynchronizer();
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;
            s.Key = "Bob";

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual("Bob", s.Key);
        }

        [Test]
        public void SetEntityThenKey_KeyDoesNotMatchEntity()
        {
            var s = new PersonNameSynchronizer();
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;
            s.Key = "Robert";

            Assert.AreEqual("Robert", s.Key);
            Assert.IsNull(s.Entity);
        }

        [Test]
        public void SetEntityThenKey_KeyMatchesEntityDifferentCase_DefaultComparer()
        {
            var s = new PersonNameSynchronizer();
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;
            s.Key = "bob";

            Assert.AreEqual("bob", s.Key);
            Assert.IsNull(s.Entity);
        }

        [Test]
        public void SetEntityThenKey_KeyMatchesEntityDifferentCase_CaseInsensitiveComparer()
        {
            var s = new CaseInsensitivePersonNameSynchronizer();
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;
            s.Key = "bob";

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual("Bob", s.Key);
        }

        [Test]
        public void SetKeyThenEntity_KeyMatchesEntity()
        {
            var s = new PersonNameSynchronizer();
            var bob = new Person { Name = "Bob" };
            s.Key = "Bob";
            s.Entity = bob;

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual("Bob", s.Key);
        }

        [Test]
        public void SetKeyThenEntity_KeyDoesNotMatchEntity()
        {
            var s = new PersonNameSynchronizer();
            var bob = new Person { Name = "Bob" };
            s.Key = "Robert";
            s.Entity = bob;

            Assert.AreSame(bob, s.Entity);
            Assert.AreEqual("Bob", s.Key);
        }

        [Test]
        public void SetEntityThenClear()
        {
            var s = new PersonNameSynchronizer();
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;
            s.Entity = null;

            Assert.IsNull(s.Entity);
            Assert.AreEqual("Bob", s.Key);
        }

        [Test]
        public void SetEntityThenKeyThenClearEntity_KeyMatchesEntityDifferentCase_CaseInsensitiveComparer()
        {
            var s = new CaseInsensitivePersonNameSynchronizer();
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;
            s.Key = "bob";
            s.Entity = null;
            Assert.AreEqual("Bob", s.Key);
            Assert.IsNull(s.Entity);
        }

        [Test]
        public void SetEntityWithDefaultKey()
        {
            var s = new PersonNameSynchronizer();
            var nullPerson = new Person { Name = null };
            s.Entity = nullPerson;

            Assert.AreSame(nullPerson, s.Entity);
            Assert.IsNull(s.Key);
        }

        [Test]
        public void SetEntityWithDefaultKey_ThenSetKeyToNull()
        {
            var s = new PersonNameSynchronizer();
            var nullPerson = new Person { Name = null };
            s.Entity = nullPerson;
            s.Key = null;

            Assert.AreSame(nullPerson, s.Entity);
            Assert.IsNull(s.Key);
        }

        [Test]
        public void SetEntityWithEmptytKey_ThenSetKeyToNull_NotNullEqualityComparer()
        {
            var s = new NotNullPersonNameSynchronizer();
            var emptyPerson = new Person { Name = string.Empty };
            s.Entity = emptyPerson;
            s.Key = null;

            Assert.AreSame(emptyPerson, s.Entity);
            Assert.AreEqual(string.Empty, s.Key);
        }

        [Test]
        public void SerializeThenDeserialize()
        {
            var s = new PersonNameSynchronizer();
            var bob = new Person { Name = "Bob" };
            s.Entity = bob;
            s.Key = "Bob";

            var clone = SerializeUtils.Clone(s);
            Assert.AreNotSame(s, clone);
            Assert.AreEqual("Bob", clone.Key);
            Assert.AreEqual(bob.Name, clone.Entity.Name);
            Assert.AreNotSame(bob, clone.Entity);
        }

        #region Test Classes

        [Serializable]
        private class PersonNameSynchronizer : EntityKeySynchronizerBase<Person, string>
        {
            protected override string GetKeyForEntity(Person entity)
            {
                return entity.Name;
            }
        }

        private class CaseInsensitivePersonNameSynchronizer : EntityKeySynchronizerBase<Person, string>
        {
            protected override string GetKeyForEntity(Person entity)
            {
                return entity.Name;
            }

            protected override bool KeyEquals(string a, string b)
            {
                return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
            }
        }

        private class NotNullPersonNameSynchronizer : EntityKeySynchronizerBase<Person, string>
        {
            protected override string GetKeyForEntity(Person entity)
            {
                return entity.Name;
            }

            protected override bool KeyEquals(string a, string b)
            {
                return string.Equals(a ?? string.Empty, b ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Serializable]
        private class Person
        {
            public string Name { get; set; }
        }

        #endregion
    }
}
