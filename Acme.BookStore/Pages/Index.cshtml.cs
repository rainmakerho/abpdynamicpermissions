using Acme.BookStore.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Identity;

namespace Acme.BookStore.Pages;

public class IndexModel : AbpPageModel
{
  
    private readonly IdentityUserManager _identityUserManager;
    private readonly PermissionDefinition2AppService _permissionDefAppService;
    public IndexModel(IdentityUserManager identityUserManager
        , PermissionDefinition2AppService permissionDefAppService)
    {
        _identityUserManager = identityUserManager;
        _permissionDefAppService = permissionDefAppService;
    }

    [BindProperty]
    public string UserPassword { get; set; }

    [BindProperty]
    public string GroupName { get; set; }

    [BindProperty]
    public string? ParentPermissionName { get; set; }

    [BindProperty]
    public string PermissionName { get; set; }
    [BindProperty]
    public string? Icon { get; set; }

    [BindProperty]
    public string? Url { get; set; }

    [BindProperty]
    public string? Target { get; set; }
    public async Task OnPostResetPassword()
    {
        var id = "06D21F6D-90BA-1A6F-1578-3A14A00F933D";
        var user = await _identityUserManager.FindByIdAsync(id);
        var token = await _identityUserManager.GeneratePasswordResetTokenAsync(user);
        var result = await _identityUserManager.ResetPasswordAsync(user, token, UserPassword);
    }

    public  Task OnPostAddPermission()
    {
        return _permissionDefAppService.PostPermissionAsync(GroupName, ParentPermissionName, PermissionName, Url, Icon, Target);
    }
}