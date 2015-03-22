using EntyTea.EntityKeySynchronization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntyTea.EntityKeySynchronization.Demo.Sample
{
    public class Person
    {
        public int Id { get; set; }

        [StringLength(10)]
        public string EmployeeNumber { get; set; }

        [Required, StringLength(128)]
        public string Name { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get { return customer.Id; } set { customer.Id = value; } }
        public Person Customer { get { return customer.Entity; } set { customer.Entity = value; } }
        private readonly EntityIdSynchronizer<Person, int> customer = new EntityIdSynchronizer<Person, int>(p => p.Id);

        [ForeignKey("AlternateCustomer")]
        public int? AlternateCustomerId { get { return alternateCustomer.IdOrNull; } set { alternateCustomer.IdOrNull = value; } }
        public Person AlternateCustomer { get { return alternateCustomer.Entity; } set { alternateCustomer.Entity = value; } }
        private readonly EntityIdSynchronizer<Person, int> alternateCustomer = new EntityIdSynchronizer<Person, int>(p => p.Id);

        [ForeignKey("Employee")]
        public string EmployeeIdentifier { get { return employee.Key; } set { employee.Key = value; } }
        public Person Employee { get { return employee.Entity; } set { employee.Entity = value; } }
        private readonly EntityKeySynchronizer<Person, string> employee = new EntityKeySynchronizer<Person, string>(p => p.EmployeeNumber);
    }
}
