using Acme.BookStore.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.PermissionManagement;

namespace Acme.BookStore.Services;

public class PermissionDefinitionAppService : BookStoreAppService
{
    private readonly IPermissionDefinitionRecordRepository _permissionRepository;
    private readonly IDynamicPermissionDefinitionStoreInMemoryCache _cache;
    private readonly IPermissionDefinitionManager _permissionDefinitionManager;
    private readonly IStaticPermissionDefinitionStore _staticPermissionDefinitionStore;

    public PermissionDefinitionAppService(IPermissionDefinitionRecordRepository permissionRepository
        , IDynamicPermissionDefinitionStoreInMemoryCache cache
        , IPermissionDefinitionManager permissionDefinitionManager
        , IStaticPermissionDefinitionStore staticPermissionDefinitionStore)
    {
        _permissionRepository = permissionRepository;
        _cache = cache;
        _permissionDefinitionManager = permissionDefinitionManager;
        _staticPermissionDefinitionStore = staticPermissionDefinitionStore;
    }


    //https://abp.io/support/questions/8410/Unable-to-save-the-permission-after-creating-it-in-an-App-Service?CurrentPage=1
    public async Task PostPermission(string name, string displayName)
    {
        var groupName = "BookStore";
        var parentName = "BookStore.xBooks";
        var book2Permission = await _permissionDefinitionManager.GetAsync(parentName);
        if (book2Permission != null)
        {
            book2Permission.AddChild(name, L(displayName));
            var exStore = _staticPermissionDefinitionStore as IExtendedStaticPermissionDefinitionStore;
            await exStore.AddPermissionDefinitionAsync(name, book2Permission);
            PermissionDefinitionRecord record = new PermissionDefinitionRecord
            {
                GroupName = groupName,
                Name = name,
                DisplayName = $"L:{displayName}",
                ParentName = parentName,
                IsEnabled = true,
                MultiTenancySide = Volo.Abp.MultiTenancy.MultiTenancySides.Both
            };
            await _permissionRepository.InsertAsync(record);
            _cache.LastCheckTime = null;
            _cache.CacheStamp = Guid.NewGuid().ToString();
            var p2 = await _permissionDefinitionManager.GetAsync(name);
        }
    }

    public PermissionDefinition GetPermission(string name)
    {
        var p1 = _cache.GetPermissionOrNull(name);
        return p1;
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BookStoreResource>(name);
    }
}
