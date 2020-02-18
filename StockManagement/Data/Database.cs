using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                                .ConnectionString("Server=tcp:vgdtest.database.windows.net,1433;Initial Catalog=StockManagement;Persist Security Info=False;User ID=vgdtest;Password=ugorKGiNOHULOdkq7nAL;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
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
