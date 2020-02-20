using StockManagement.Domain;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Mappers
{
    public class ItemUserMap : ClassMap<ItemUser>
    {
        public ItemUserMap()
        {
            Table("ItemUserMap");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.FromDate);
            Map(x => x.ToDate);
            References(x => x.User);
            References(x => x.Item);
        }
    }
}
