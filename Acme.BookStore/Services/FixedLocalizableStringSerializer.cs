using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;

namespace Acme.BookStore.Services;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(LocalizableStringSerializer), typeof(ILocalizableStringSerializer), typeof(FixedLocalizableStringSerializer))]
public class FixedLocalizableStringSerializer : LocalizableStringSerializer, ITransientDependency
{
    public FixedLocalizableStringSerializer(IOptions<AbpLocalizationOptions> localizationOptions) : base(localizationOptions)
    {
    }

    public override ILocalizableString Deserialize(string? value)
    {
        if (value == null)
        {
            return new FixedLocalizableString("");
        }
        try
        {
            return base.Deserialize(value);
        }
        catch
        {
            return new FixedLocalizableString(value);
        }
        
    }
}
