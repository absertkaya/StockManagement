﻿using FluentNHibernate.Mapping;
using StockManagement.Domain;

namespace StockManagement.Data.Mappers
{
    public class ItemMap : ClassMap<Item>
    {

        public ItemMap()
        {
            Table("Item");
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.SerialNumber).UniqueKey("SerialNumberProductNumber").Not.Nullable();
            Map(x => x.Comment);
            Map(x => x.ItemStatus).Not.Nullable();
            Map(x => x.DeliveryDate);
            Map(x => x.InvoiceDate);
            Map(x => x.Imei);
            Map(x => x.Hostname);
            Map(x => x.License);
            Map(x => x.Carepack);
            Map(x => x.VGDNumber);
            References(x => x.ADUser);
            References(x => x.Product).UniqueKey("SerialNumberProductNumber").Not.Nullable();
            References(x => x.Supplier);
            HasMany(x => x.ItemUsers)
                .Inverse()
                .Cascade.All();
            HasMany(x => x.IncludedItems)
                .Cascade.None();
        }
    }
}
