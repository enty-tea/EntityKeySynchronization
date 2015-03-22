# Entity Key Synchronization [![Build status](https://ci.appveyor.com/api/projects/status/w8xly9tppg3n1550?svg=true)](https://ci.appveyor.com/project/enty-tea/entitykeysynchronization)

A lightweight .NET library for keeping an entity and its key synchronized.


## Overview

In an Entity Framework model, you can choose to model a one-to-many relationship as an [independent relationtion or a foreign key relationship](https://msdn.microsoft.com/en-us/data/jj713564.aspx).

For example, here's what an Order class might look like in a model where a customer can have many orders:

```c#
public class Order
{
	public int Id { get; set; }
	public int CustomerId { get; set; }
	public Person Customer { get; set; }
}
```

You can choose to have either a `CustomerId` or `Customer` property or, as in this example, you can have both.  And you often want both, because they are useful in different situations when using Entity Framework:

* The entity property lets you easily join with the Customer table and select properties from it
* The foreign key property lets you easily load just then foreign key value without querying the Customer table 

But if you have both of these properties, you can end up in situations where the id and the entity properties are not synchronized.  If you set the `Customer` property, then you want to make sure the `CustomerId` property is always the same as the id in the `Customer` object.  And you should be able to set the `CustomerId` property even if there is no `Customer` object.

So the goal of this library is to help with that synchronization problem.


## Use

The main class in this library is `EntityIdSynchronizer`, which is a wrapper around an entity and its foreign key:

```c#
var s = new EntityIdSynchronizer<Person, int>(p => p.Id);
s.Id = 1;
s.Entity = new Person { Id = 1, Name = "Bob" };
```

You can set just the ID and the entity will remain null:

```c#
var s = new EntityIdSynchronizer<Person, int>(p => p.Id);
s.Id = 1;
// s.Entity == null
```

If you set the ID and then set the Entity to an object with a different ID value, then the Entity's ID will overwrite the value you set:

```c#
var s = new EntityIdSynchronizer<Person, int>(p => p.Id);
s.Id = 1;
s.Entity = new Person { Id = 2, Name = "Bob" };
// s.Id == 2
```

If you set the Entity first and then change the ID, then the Entity property will be set to null:

```c#
var s = new EntityIdSynchronizer<Person, int>(p => p.Id);
s.Entity = new Person { Id = 2, Name = "Bob" };
s.Id = 1;
// s.Entity == null
```

There is also support for optional associations through the IdOrNull property:

```c#
var s = new EntityIdSynchronizer<Person, int>(p => p.Id);
s.Entity = new Person { Id = 1, Name = "Bob" };
s.IdOrNull = null;
// s.Entity == null;
// s.Id == 0
```

If you have an entity with a non-primitive key (like a string), then you can use the `EntityKeySynchronizer` class instead:

```c#
var s = new EntityKeySynchronizer<Person, string>(p => p.Name);
s.Key = "Bob";
s.Entity = new Person { Name = "Bob" };
```

## Examples

Here's an example of how these synchronizer classes can be used in a model:

```c#
public class Person
{
	public int Id { get; set; }
	public string EmployeeNumber { get; set; }
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
```

And here's an example that supports serialization by avoiding the use of delegates in the synchronizer constructors:

```c#
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
```








