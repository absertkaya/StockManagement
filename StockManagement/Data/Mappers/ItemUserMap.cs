using FluentNHibernate.Mapping;
using StockManagement.Domain;

namespace StockManagement.Data.Mappers
{
    public class ItemUserMap : ClassMap<ItemUser>
    {
        public ItemUserMap()
        {
            Table("ItemUser");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.FromDate).Not.Nullable();
            Map(x => x.ToDate);
            References(x => x.User).Not.Nullable();
            References(x => x.Item).Not.Nullable();
            References(x => x.AssignedBy).Not.Nullable();
            References(x => x.ReturnedBy);
        }
    }
}
