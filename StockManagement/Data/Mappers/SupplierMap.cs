﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using StockManagement.Domain;
namespace StockManagement.Data.Mappers
{
    public class SupplierMap : ClassMap<Supplier>
    {
        public SupplierMap()
        {
            Table("Supplier");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.SupplierName);
        }
    }
}
