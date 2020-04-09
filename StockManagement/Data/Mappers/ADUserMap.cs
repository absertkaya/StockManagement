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
            Map(x => x.LastName).Not.Nullable();
            Map(x => x.FirstName).Not.Nullable();
            Map(x => x.Mail).Not.Nullable();
            Map(x => x.MobilePhone);
            Map(x => x.Office).Not.Nullable();
            Map(x => x.OfficeRole).Not.Nullable();
            Map(x => x.StockRole);
            HasMany(x => x.Items)
                .Inverse()
                .Cascade
                .SaveUpdate();
            HasMany(x => x.ItemUsers)
                .Inverse()
                .Cascade.All();
            HasMany(x => x.MobileSubscriptions)
                .Inverse()
                .Cascade
                .Delete();
        }
    }
}
