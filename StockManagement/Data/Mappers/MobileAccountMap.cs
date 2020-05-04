using FluentNHibernate.Mapping;
using StockManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Mappers
{
    public class MobileAccountMap : ClassMap<MobileAccount>
    {
        public MobileAccountMap()
        {
            Table("MobileAccount");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.AccountName).Not.Nullable();
            Map(x => x.AccountNumber).Not.Nullable();
            HasMany(x => x.MobileSubscriptions).Inverse();
        }
    }
}
