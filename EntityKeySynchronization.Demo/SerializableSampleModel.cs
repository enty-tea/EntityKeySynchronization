using EntyTea.EntityKeySynchronization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntyTea.EntityKeySynchronization.Demo.SerializableSample
{
    public interface ISampleEntity
    {
        int Id { get; set; }
    }

    [Serializable]
    public class SampleEntityIdSynchronizer<TEntity> : EntityIdSynchronizerBase<TEntity, int>
        where TEntity : class, ISampleEntity
    {
        protected override int GetKeyForEntity(TEntity entity)
        {
            return entity.Id;
        }
    }

    [Serializable]
    public class Person : ISampleEntity
    {
        public int Id { get; set; }

        [Required, StringLength(128)]
        public string Name { get; set; }
    }

    [Serializable]
    public class Order : ISampleEntity
    {
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get { return customer.Id; } set { customer.Id = value; } }
        public Person Customer { get { return customer.Entity; } set { customer.Entity = value; } }
        private readonly SampleEntityIdSynchronizer<Person> customer = new SampleEntityIdSynchronizer<Person>();

        [ForeignKey("AlternateCustomer")]
        public int? AlternateCustomerId { get { return alternateCustomer.IdOrNull; } set { alternateCustomer.IdOrNull = value; } }
        public Person AlternateCustomer { get { return alternateCustomer.Entity; } set { alternateCustomer.Entity = value; } }
        private readonly SampleEntityIdSynchronizer<Person> alternateCustomer = new SampleEntityIdSynchronizer<Person>();

        [ForeignKey("Employee")]
        public int? EmployeeId { get { return employee.IdOrNull; } set { employee.IdOrNull = value; } }
        public Person Employee { get { return employee.Entity; } set { employee.Entity = value; } }
        private readonly SampleEntityIdSynchronizer<Person> employee = new SampleEntityIdSynchronizer<Person>();
    }
}
