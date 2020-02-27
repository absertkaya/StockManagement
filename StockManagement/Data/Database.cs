using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace StockManagement.Data
{
    public class Database
    {
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = Fluently.Configure()
                                .Database(MsSqlConfiguration.MsSql2012
                                .ConnectionString("Server=tcp:vgd-stockmanagement.database.windows.net,1433;Initial Catalog=stockmanagement-test;Persist Security Info=False;User ID=vgd-stockmanagement;Password=fwVhpN73gJ2gZJLvD25h;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
                                .Mappings(m => m.FluentMappings.AddFromAssembly(new Database().GetType().Assembly))
                                .CurrentSessionContext("call")
                                .ExposeConfiguration(cfg => BuildSchema(cfg, true, true))
                                .BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }
        private static void BuildSchema(NHibernate.Cfg.Configuration config, bool create, bool update)
        {
            if (create)
            {
                new SchemaExport(config).Create(false, true);
            }
            else
            {
                new SchemaUpdate(config).Execute(false, update);
            }
        }
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
