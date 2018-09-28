
using Autofac;
using Autofac.Builder;
using Autofac.Integration.Mvc;
using Document_storage_v2._0.Controllers;
using Document_storage_v2.Models;

using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

using Microsoft.Owin;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Event;
using NHibernate.Tool.hbm2ddl;
using Owin;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

[assembly: OwinStartupAttribute(typeof(Document_storage_v2.Startup))]
namespace Document_storage_v2
{

    public partial class Startup
    {
        static IPersistenceConfigurer MsSqlDbConfiguration(string connectionStringId)
        => MsSqlConfiguration.MsSql2012.ConnectionString(ConfigurationManager.ConnectionStrings[connectionStringId]?.ConnectionString ?? "Connection string not found");

        public void Configuration(IAppBuilder app)
        {
            var dbConfiguration = MsSqlDbConfiguration("MSSQL");
            var assembly = typeof(Startup).Assembly;
            var builder = new ContainerBuilder();
            var container = default(IContainer);

            IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle>
                Register<T>(Func<IComponentContext, T> provider)
                    => builder.Register<T>(provider).As<T>();

            Register(x =>
            {
                // конфигурирование FluentNH
                var fluentConfig = Fluently.Configure()
                    .Database(dbConfiguration)
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Users>())
                    .CurrentSessionContext("call")
                    .ExposeConfiguration(hConfig =>
                    {
                        //SchemaMetadataUpdater.QuoteTableAndColumns(hConfig, MsSql2012Dialect.GetDialect());

                        var eventListeners = hConfig.EventListeners;

                        //eventListeners.PreUpdateEventListeners =
                        //    new IPreUpdateEventListener[] { new PreUpdateNoteListener() };

                        //eventListeners.PreInsertEventListeners =
                        //    new IPreInsertEventListener[] { new PreSaveNoteListener() };
                        // TODO: реализовать получение необходимых типов listener'ов

                    });

                // сборка конфигурации
                var hibernateConfig = fluentConfig.BuildConfiguration();
                var schemaExport = new SchemaUpdate(hibernateConfig);

                schemaExport.Execute(useStdOut: true, doUpdate: true);

                return fluentConfig.BuildSessionFactory();
            }).SingleInstance();
            builder.RegisterControllers(typeof(ClientController).Assembly /*, ... Другие сборки*/);
            builder.RegisterModule(new AutofacWebTypesModule());

            builder.RegisterGeneric(typeof(Repository<>));

            // настройка процесса выдачи сессии
            {
                ISession createSession(IComponentContext x)
                    => x.Resolve<ISessionFactory>().OpenSession();

                Register(createSession).InstancePerRequest();
                Register(createSession).InstancePerDependency();
            }

            // настройка разрешения зависимостей
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container = builder.Build()));

            // настройка приложения
            app.UseAutofacMiddleware(container);

            ConfigureAuth(app);
        }
    }
}