using FluentNHibernate.Mapping;
using StockManagement.Domain;

namespace StockManagement.Data.Mappers
{
    public class ADUserMap : ClassMap<ADUser>
    {
        public ADUserMap()
        {
            Table("ADUser");
            Id(x => x.Id);
            Map(x => x.LastName);
            Map(x => x.FirstName);
            Map(x => x.UserRole);
            Map(x => x.Office);
            Map(x => x.OfficeRole);
            HasMany(x => x.Items)
                .Inverse()
                .Cascade
                .SaveUpdate();
            HasMany(x => x.ItemUsers)
                .Inverse()
                .Cascade.All();
        }
    }
}
