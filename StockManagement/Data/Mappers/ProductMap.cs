using FluentNHibernate.Mapping;
using StockManagement.Domain;

namespace StockManagement.Data.Mappers
{
    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Table("Products");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.ProductNumber).Unique().Not.Nullable();
            Map(x => x.Description).Not.Nullable();
            References(x => x.Category).Not.Nullable();
            HasMany(x => x.Items)
                .Inverse()
                .Cascade.All();
        }
    }
}
