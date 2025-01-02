using Acme.BookStore.Localization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Acme.BookStore.Services;

public class PermissionDefinition2AppService : BookStoreAppService
{

    private readonly IPermissionGroupDefinitionRecordRepository _permissionGroupRepository;
    private readonly IPermissionDefinitionRecordRepository _permissionRepository;
    private readonly IDistributedCache _distributedCache;
    private readonly DynamicPermissionDefinitionStore _dynamicPermissionDefinitionStore;
    protected readonly AbpDistributedCacheOptions _cacheOptions;
    private readonly IDynamicPermissionDefinitionStoreInMemoryCache _cache;
    private readonly IPermissionDefinitionManager _permissionDefManager;
    public PermissionDefinition2AppService(IPermissionGroupDefinitionRecordRepository permissionGroupRepository,
        IPermissionDefinitionRecordRepository permissionRepository,
        IDistributedCache distributedCache,
        DynamicPermissionDefinitionStore dynamicPermissionDefinitionStore,
        IOptions<AbpDistributedCacheOptions> cacheOptions,
        IDynamicPermissionDefinitionStoreInMemoryCache cache,
        IPermissionDefinitionManager permissionDefManager)
    {
        _permissionGroupRepository = permissionGroupRepository;
        _permissionRepository = permissionRepository;
        _distributedCache = distributedCache;
        _dynamicPermissionDefinitionStore = dynamicPermissionDefinitionStore;
        _cacheOptions = cacheOptions.Value;
        _cache = cache;
        _permissionDefManager = permissionDefManager;
    }


    //[LocalizationResourceName("BookStore")]
    public async Task PostPermissionAsync(string groupName, string parentName, string name, string url, string icon, string target)
    {

        var permissionGroup = new PermissionGroupDefinitionRecord
        {
            Name = groupName,
            DisplayName = GetDisplayName(groupName)
        };
        var group = await _permissionGroupRepository.GetListAsync();
        if (group.All(x => x.Name != groupName))
        {
            await _permissionGroupRepository.InsertAsync(permissionGroup);
        }
        var permissions = await _permissionRepository.GetListAsync();
        if (permissions.All(x => x.Name != name))
        {
            var testPermission = new PermissionDefinitionRecord
            {
                Name = name,
                DisplayName = GetDisplayName(name),
                GroupName = groupName,
                IsEnabled = true,
                ParentName = parentName,
                MultiTenancySide = MultiTenancySides.Both
            };
            if(!string.IsNullOrEmpty(url))
            {
                testPermission.SetProperty("Url", url);
            }
            if (!string.IsNullOrEmpty(icon))
            {
                testPermission.SetProperty("Icon", icon);
            }
            if (!string.IsNullOrEmpty(target))
            {
                testPermission.SetProperty("Target", target);
            }
            await _permissionRepository.InsertAsync(testPermission);

            await _distributedCache.RemoveAsync($"{_cacheOptions.KeyPrefix}_AbpInMemoryPermissionCacheStamp");
        }
    }

    private string GetDisplayName(string name)
    {
        //L:BookStore,Permission:BookStore
        var resourceName = "BookStore";
        var permissionName = $"Permission";
        return $"L:{resourceName},{permissionName}:{name}";
    }

    public Task<PermissionDefinition> GetPermissionAsync(string name)
    {
        //var p1 = _cache.GetPermissionOrNull(name);
        //return p1;
        return _permissionDefManager.GetAsync(name);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BookStoreResource>(name);
    }
}
