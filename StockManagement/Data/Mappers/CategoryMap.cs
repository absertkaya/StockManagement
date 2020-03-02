using FluentNHibernate.Mapping;
using StockManagement.Domain;

namespace StockManagement.Data.Mappers
{
    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Table("Category");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.CategoryName).Not.Nullable();
            HasMany(x => x.Products)
                .Inverse()
                .Cascade.All();
        }
    }
}
