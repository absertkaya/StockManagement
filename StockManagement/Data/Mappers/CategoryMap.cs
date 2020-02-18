using FluentNHibernate.Mapping;
using StockManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Mappers
{
    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Table("Category");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.CategoryName);
        }
    }
}
