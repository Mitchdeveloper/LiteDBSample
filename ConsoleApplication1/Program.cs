using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using System.Collections;

namespace ConsoleApplication1
{
    
    class Program
    {
        // DbRef to cross references
        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Product
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public double Price { get; set; }
        }

        // DbRef to cross references
        public class Order
        {   
            public ObjectId Id { get; set; }
            public DateTime OrderDate { get; set; }

            [BsonRef("customers")]
            public Customer Customer { get; set; }
            [BsonRef("products")]
            public List<Product> Products   { get; set; }
        }



        static void Main(string[] args)
        {
          

            // Produts and Customer are other collections (not embedded document)
            // you can use [BsonRef("colname")] attribute
          
            using (var db = new LiteDatabase("LiteDB"))
            {
                db.DropCollection("customers");
                db.DropCollection("orders");
                db.DropCollection("products");
                //var mapper = BsonMapper.Global;
                //mapper.Entity<Order>()
                // .DbRef(x => x.Customer, "customers")   // 1 to 1/0 reference
                // .DbRef(x => x.Products, "products");
                
                var customers = db.GetCollection<Customer>("customers");
                var products = db.GetCollection<Product>("products");
                var orders = db.GetCollection<Order>("orders");
               
                var john = new Customer { Name = "John Doe" };
                var tv = new Product {Id =1, Description = "TV Sony 44\"", Price = 799 };
                var iphone = new Product {Id = 2, Description = "iPhone X", Price = 999 };
                List<Product> p = new List<Product>().ToList();
                p.Add(tv);
                var order1 = new Order { OrderDate = new DateTime(2017, 1, 1), Customer = john,Products = p } ;
                var order2 = new Order { OrderDate = new DateTime(2017, 10, 1), Customer = john };
                customers.Insert(john);
                products.Insert(new Product[] { tv, iphone });
                orders.Insert(order1);
               // products.Delete(1);
                var result = orders
                    .Include(x => x.Customer)
                    .Include(x => x.Products).FindAll();


                foreach (var o in result)
                {
                    Console.WriteLine("name:{0}", o.Customer.Name.ToString());

                    foreach (Product p1 in o.Products)
                    {
                        Console.WriteLine(String.Format("{0},{1:0.00}", p1.Description, p1.Price));
                    }

                    Console.ReadLine();
                }


            }
        }
    }
}
