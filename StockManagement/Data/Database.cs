using Azure.Security.KeyVault.Secrets;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using StockManagement.Domain.IServices;

namespace StockManagement.Data
{
    public class Database
    {
        private static ISessionFactory _sessionFactory;
        public IConfiguration Configuration { get; set; }
        public IKeyVaultService KeyVaultService { get; set; }
        private ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    var connectionString = KeyVaultService.GetSecret("ConnectionString");
                    _sessionFactory = Fluently.Configure()
                                .Database(MsSqlConfiguration.MsSql2012
                                .ConnectionString(connectionString))
                                .Mappings(m => m.FluentMappings.AddFromAssembly(new Database().GetType().Assembly))
                                .CurrentSessionContext("call")
                                .ExposeConfiguration(cfg => BuildSchema(cfg, false, false))
                                .BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }

        public Database(IConfiguration configuration, IKeyVaultService keyVaultService)
        {
            Configuration = configuration;
            KeyVaultService = keyVaultService;
        }

        private Database() { }

        private void BuildSchema(NHibernate.Cfg.Configuration config, bool create, bool update)
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
        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
