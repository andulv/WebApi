using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Builder;
using System.Web.OData.Query;
using System.Web.OData.Extensions;

namespace OData_Sample.Controllers
{
    public class ProductsController : ApiController
    {

        //Her er det mye automatikk.
        //Vi returnerer en IQueryable<>, så kicker mediatypeformatter (eller noe lingnende) inn, lager ODataQueryOptions fra querystring og apply'er den før serializering.
        [EnableQuery]
        public IQueryable<Product> Get1()
        {
            Product[] products = GetProducts();
            return products.AsQueryable();
        }

        //Litt mindre automatikk
        //Vi spesifiserer at vi vil ha ODataQueryOptions som argument, en modelbinder sørger for at den blir opprettet fra querystring
        public IList<Product> Get2(ODataQueryOptions<Product> opts)
        {
            var products = GetProducts().AsQueryable();
            opts.ApplyTo(products);
            return products.ToList();
        }

        //Her er manuell måte å hente ODataQueryOptions fra scratch.
        public IList<Product> Get3()
        {
            ODataQueryOptions<Product> opts = GetOdataQueryOptions<Product>();

            var products = GetProducts().AsQueryable();
            products = opts.ApplyTo(products) as IQueryable<Product>;
            return products.ToList();
        }

        private ODataQueryOptions<T> GetOdataQueryOptions<T>()
        {
            var entityClrType = typeof(T);
            Microsoft.OData.Edm.IEdmModel edmModel = BuildModel(entityClrType);
            var odataPath = this.Request.ODataProperties().Path;
            var odataContext = new ODataQueryContext(edmModel, entityClrType, odataPath);
            var opts = new ODataQueryOptions<T>(odataContext, this.Request);
            return opts;
        }

        //NB! Tror denne er expensive å lage, og forandrer seg aldri. Bør caches.
        private Microsoft.OData.Edm.IEdmModel BuildModel(Type entityClrType)
        {
            var builder = new ODataConventionModelBuilder(this.Configuration, isQueryCompositionMode: true);
            var entityTypeConfiguration = builder.AddEntityType(entityClrType);
            builder.AddEntitySet(entityClrType.Name, entityTypeConfiguration);
            var edmModel = builder.GetEdmModel();
            return edmModel;
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
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeCode { get; set; }
    }
}
