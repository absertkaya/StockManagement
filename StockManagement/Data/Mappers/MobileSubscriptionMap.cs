using FluentNHibernate.Mapping;
using StockManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Mappers
{
    public class MobileSubscriptionMap : ClassMap<MobileSubscription>
    {
        public MobileSubscriptionMap()
        {
            Table("MobileSubscription");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.MobileNumber).Not.Nullable();
            Map(x => x.SubscriptionType).Not.Nullable();
            References(x => x.User).Not.Nullable();
            References(x => x.MobileAccount).Not.Nullable();
        }
    }
}
