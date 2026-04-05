using System.Reflection;
using Autofac;
using ShopApi.Application.Common.Interfaces;
using ShopApi.Infrastructure.Data;
using ShopApi.Infrastructure.Repositories;
using ShopApi.Infrastructure.Services;
using ShopApi.Infrastructure.Strategies;

namespace ShopApi.Infrastructure.AutofacModules;

public class InfrastructureModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterAssemblyTypes(typeof(CatalogRepository).GetTypeInfo().Assembly)
            .Where(type => !type.IsAbstract && type.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder
            .Register<IUnitOfWork>(context => context.Resolve<AppDbContext>())
            .InstancePerLifetimeScope();

        builder
            .RegisterAssemblyTypes(typeof(MustHaveAuditableBeforeSavingStrategy).GetTypeInfo().Assembly)
            .As<IDbContextStrategy>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<IdentityService>()
            .As<IIdentityService>()
            .InstancePerLifetimeScope();
    }
}
