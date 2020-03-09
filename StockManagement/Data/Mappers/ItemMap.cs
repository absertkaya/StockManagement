using FluentNHibernate.Mapping;
using StockManagement.Domain;

namespace StockManagement.Data.Mappers
{
    public class ItemMap : ClassMap<Item>
    {

        public ItemMap()
        {
            Table("Item");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.SerialNumber).UniqueKey("SerialNumberProductNumber");
            Map(x => x.Comment);
            Map(x => x.InStock);
            Map(x => x.IsDefective);
            References(x => x.ADUser);
            References(x => x.Product).UniqueKey("SerialNumberProductNumber").Not.Nullable();
            References(x => x.Supplier);
            HasMany(x => x.ItemUsers)
                .Inverse()
                .Cascade.All();
        }
    }
}
