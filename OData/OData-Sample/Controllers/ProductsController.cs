using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

namespace OData_Sample.Controllers
{
    public class ProductsController : ApiController
    {
        // GET api/values
        public IList<Product> Get1(ODataQueryOptions<Product> opts)
        {
            var products = GetProducts().AsQueryable();
            opts.ApplyTo(products);
            return products.ToList();
        }

        // GET api/values
        [EnableQuery]
        public IQueryable<Product> Get2()
        {
            Product[] products = GetProducts();
            return products.AsQueryable();
        }


        private static Product[] GetProducts()
        {
            return new[]
            {
                new Product { Id=1, Name="Produkt 1", TypeCode=1},
                new Product { Id=2, Name="Produkt 2", TypeCode=1},
                new Product { Id=3, Name="Produkt 2", TypeCode=0},
            };
        }

        // GET api/values/5
        public Product Get(int id)
        {
            return GetProducts().SingleOrDefault(x => x.Id == id);
        }

    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeCode { get; set; }
    }
}
