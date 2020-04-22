using FluentNHibernate.Mapping;
using StockManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Mappers
{
    public class MailTemplateMap : ClassMap<MailTemplate>
    {
        public MailTemplateMap()
        {
            Table("MailTemplate");
            Id(x => x.Id);
            Map(x => x.Body);
            Map(x => x.Subject);
        }
    }
}
