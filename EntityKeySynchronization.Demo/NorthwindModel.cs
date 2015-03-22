using EntyTea.EntityKeySynchronization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntyTea.EntityKeySynchronization.Demo.Northwind
{
    [Table("Orders")]
    public class Order
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }

        [Key]
        public int OrderID { get; set; }

        [ForeignKey("Customer")]
        [StringLength(10, MinimumLength = 10)]
        public string CustomerID { get { return customer.Key; } set { customer.Key = value; } }
        public Customer Customer { get { return customer.Entity; } set { customer.Entity = value; } }
        private readonly EntityKeySynchronizer<Customer, string> customer = new EntityKeySynchronizer<Customer, string>(c => c.CustomerID);

        [ForeignKey("Employee")]
        public int? EmployeeID { get { return employee.IdOrNull; } set { employee.IdOrNull = value; } }
        public Employee Employee { get { return employee.Entity; } set { employee.Entity = value; } }
        private readonly EntityIdSynchronizer<Employee, int> employee = new EntityIdSynchronizer<Employee, int>(e => e.EmployeeID);

        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public decimal? Freight { get; set; }

        [StringLength(80)]
        public string ShipName { get; set; }

        [StringLength(120)]
        public string ShipAddress { get; set; }

        [StringLength(30)]
        public string ShipCity { get; set; }

        [StringLength(30)]
        public string ShipRegion { get; set; }

        [StringLength(20)]
        public string ShipPostalCode { get; set; }

        [StringLength(30)]
        public string ShipCountry { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }

    [Table("Products")]
    public class Product
    {
        public Product()
        {
            OrderDetails = new List<OrderDetail>();
        }

        [Key]
        public int ProductID { get; set; }

        [Required]
        [StringLength(30)]
        public string ProductName { get; set; }

        [StringLength(40)]
        public string QuantityPerUnit { get; set; }

        public decimal? UnitPrice { get; set; }

        public short? UnitsInStock { get; set; }

        public short? UnitsOnOrder { get; set; }

        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }

    [Table("Order Details")]
    public class OrderDetail
    {
        [Key, Column(Order = 1)]
        [ForeignKey("Order")]
        public int OrderId { get { return order.Id; } set { order.Id = value; } }
        public Order Order { get { return order.Entity; } set { order.Entity = value; } }
        private readonly EntityIdSynchronizer<Order, int> order = new EntityIdSynchronizer<Order, int>(o => o.OrderID);

        [Key, Column(Order = 2)]
        [ForeignKey("Product")]
        public int ProductId { get { return product.Id; } set { product.Id = value; } }
        public Product Product { get { return product.Entity; } set { product.Entity = value; } }
        private readonly EntityIdSynchronizer<Product, int> product = new EntityIdSynchronizer<Product, int>(p => p.ProductID);

        public decimal UnitPrice { get; set; }

        public short Quantity { get; set; }

        public float Discount { get; set; }
    }

    [Table("Customers")]
    public class Customer
    {
        public Customer()
        {
            Orders = new List<Order>();
        }

        [Key, Required]
        [StringLength(10, MinimumLength = 10)]
        public string CustomerID { get; set; }

        [Required]
        [StringLength(80)]
        public string CompanyName { get; set; }

        [StringLength(60)]
        public string ContactName { get; set; }

        [StringLength(60)]
        public string ContactTitle { get; set; }

        [StringLength(120)]
        public string Address { get; set; }

        [StringLength(30)]
        public string City { get; set; }

        [StringLength(30)]
        public string Region { get; set; }

        [StringLength(20)]
        public string PostalCode { get; set; }

        [StringLength(30)]
        public string Country { get; set; }

        [StringLength(48)]
        public string Phone { get; set; }

        [StringLength(48)]
        public string Fax { get; set; }

        public ICollection<Order> Orders { get; set; }
    }

    [Table("Employees")]
    public class Employee
    {
        public Employee()
        {
            Orders = new List<Order>();
        }

        [Key]
        public int EmployeeID { get; set; }

        [Required]
        [StringLength(40)]
        public string LastName { get; set; }

        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        [StringLength(60)]
        public string Title { get; set; }

        [StringLength(50)]
        public string TitleOfCourtesy { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? HireDate { get; set; }

        [StringLength(120)]
        public string Address { get; set; }

        [StringLength(30)]
        public string City { get; set; }

        [StringLength(30)]
        public string Region { get; set; }

        [StringLength(20)]
        public string PostalCode { get; set; }

        [StringLength(30)]
        public string Country { get; set; }

        [StringLength(48)]
        public string HomePhone { get; set; }

        [StringLength(8)]
        public string Extension { get; set; }

        [Column(TypeName = "image")]
        public byte[] Photo { get; set; }

        public string Notes { get; set; }

        [ForeignKey("ReportsTo")]
        public Employee ReportsToEmployee { get { return reportsTo.Entity; } set { reportsTo.Entity = value; } }
        public int? ReportsTo { get { return reportsTo.IdOrNull; } set { reportsTo.IdOrNull = value; } }
        private readonly EntityIdSynchronizer<Employee, int> reportsTo = new EntityIdSynchronizer<Employee, int>(e => e.EmployeeID);

        [StringLength(510)]
        public string PhotoPath { get; set; }

        public ICollection<Order> Orders { get; set; }
    }

    //public class NorthwindContext : DbContext
    //{
    //    static NorthwindContext()
    //    {
    //        System.Data.Entity.Database.SetInitializer<NorthwindContext>(null);
    //    }

    //    public NorthwindContext()
    //        : base() { Initialize(); }

    //    public NorthwindContext(string nameOrConnectionString)
    //        : base(nameOrConnectionString) { Initialize(); }

    //    public NorthwindContext(DbConnection existingConnection, bool contextOwnsConnection)
    //        : base(existingConnection, contextOwnsConnection) { Initialize(); }

    //    private void Initialize()
    //    {
    //        Configuration.LazyLoadingEnabled = false;
    //        Configuration.ProxyCreationEnabled = false;

    //        var objectContext = ((IObjectContextAdapter)this).ObjectContext;
    //        var contextOptions = objectContext.ContextOptions;
    //        contextOptions.UseCSharpNullComparisonBehavior = true;
    //    }

    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
    //    }

    //    public DbSet<Order> Orders { get; set; }
    //    public DbSet<Product> Products { get; set; }
    //    public DbSet<OrderDetail> OrderDetails { get; set; }
    //    public DbSet<Customer> Customers { get; set; }
    //    public DbSet<Employee> Employees { get; set; }
    //}
}
