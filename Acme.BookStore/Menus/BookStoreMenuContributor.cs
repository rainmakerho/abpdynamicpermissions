using Acme.BookStore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Polly;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace Acme.BookStore.Menus;

public class BookStoreMenuContributor : IMenuContributor
{
    private readonly IServiceCollection _services;
    private readonly IPermissionChecker _permissionChecker;
    public BookStoreMenuContributor(IServiceCollection services)
    {
        _services = services;
        _permissionChecker = services.GetRequiredService<IPermissionChecker>();
    }
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private async Task ConfigureMainMenuAsync( MenuConfigurationContext context)
    {
         
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<BookStoreResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                BookStoreMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        
        var rootPermissionName = "BookStore.RootFun";
        var permissionDefManager = _services.GetRequiredService<IPermissionDefinitionManager>();
        var rootPermission = await permissionDefManager.GetOrNullAsync(rootPermissionName);
        var isGrant = await _permissionChecker.IsGrantedAsync(rootPermissionName);
        if (rootPermission != null && isGrant)
        {
            var menu = await ConfigureAPMenuAsync(context, rootPermission);
            context.Menu.Items.Add(menu);
        }
        //context.Menu.AddItem(
        //       new ApplicationMenuItem(
        //           "BooksStore",
        //           l["Menu:BookStore"],
        //           icon: "fas fa-book"
        //       ).AddItem(
        //           new ApplicationMenuItem(
        //               "BooksStore.Books",
        //               l["Menu:Books"],
        //               url: "/books"
        //           )
        //       ).AddItem(
        //           new ApplicationMenuItem(
        //               "BooksStore.Authors",
        //               l["Menu:Authors"],
        //               url: "/authors"
        //           )
        //       )
        //   );
        if (BookStoreModule.IsMultiTenant)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }
 
    }

    private async Task<ApplicationMenuItem> ConfigureAPMenuAsync(MenuConfigurationContext context, PermissionDefinition permission)
    {
        var l = context.GetLocalizer<BookStoreResource>();
        var menu = new ApplicationMenuItem(
                permission.Name,
                 l[$"Permission:{permission.Name}"]
            );
        if (permission.Properties.ContainsKey("Icon"))
        {
            menu.Icon = permission.Properties["Icon"] as string;
        }
        if (permission.Properties.ContainsKey("Url"))
        {
            menu.Url = permission.Properties["Url"] as string;
        }
        if (permission.Properties.ContainsKey("Target"))
        {
            menu.Target = permission.Properties["Target"] as string;
        }
        foreach (var child in permission.Children)
        {
            var grant = await _permissionChecker.IsGrantedAsync(child.Name);
            if (grant)
            {
                var childMenu = await ConfigureAPMenuAsync(context, child);
                menu.Items.Add(childMenu);
            }
            
        }
        return menu;
    }
}
