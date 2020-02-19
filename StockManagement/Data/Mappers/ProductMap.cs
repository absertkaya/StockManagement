using FluentNHibernate.Mapping;
using StockManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Mappers
{
    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Table("ItemDescription");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.ProductNumber);
            Map(x => x.Description);
            References(x => x.Category);
            HasMany(x => x.Items)
                .Inverse()
                .Cascade.All();
        }
    }
}
