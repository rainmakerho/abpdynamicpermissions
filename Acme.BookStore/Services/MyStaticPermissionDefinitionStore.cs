using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp;
using System.Collections;

namespace Acme.BookStore.Services;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IStaticPermissionDefinitionStore), typeof(IExtendedStaticPermissionDefinitionStore))]
public class MyStaticPermissionDefinitionStore : StaticPermissionDefinitionStore, IExtendedStaticPermissionDefinitionStore
{
    public MyStaticPermissionDefinitionStore(IServiceProvider serviceProvider, IOptions<AbpPermissionOptions> options) : base(serviceProvider, options)
    {
        
    }
    public Task AddPermissionDefinitionAsync(string key, PermissionDefinition permissionDefinition)
    {
        if (!PermissionDefinitions.ContainsKey(key))
        {
            PermissionDefinitions.Add(key, permissionDefinition);
        }
        return Task.CompletedTask;
    }
}


public interface IExtendedStaticPermissionDefinitionStore : IStaticPermissionDefinitionStore
{
    Task AddPermissionDefinitionAsync(string key, PermissionDefinition permissionDefinition);
}

