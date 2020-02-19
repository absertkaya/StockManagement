using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using StockManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Mappers
{
    public class ItemMap : ClassMap<Item>
    {
        
        public ItemMap()
        {
            Table("Item");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.SerialNumber);
            Map(x => x.Comment);
            Map(x => x.InStock);
            References(x => x.ADUser);
            References(x => x.Product);
        }
    }
}
