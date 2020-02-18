using FluentNHibernate.Mapping;
using StockManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Mappers
{
    public class ADUserMap: ClassMap<ADUser>
    {
        public ADUserMap()
        {
            Table("ADUser");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.LastName);
            Map(x => x.FirstName);
        }
    }
}
