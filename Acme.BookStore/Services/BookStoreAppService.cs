using Acme.BookStore.Localization;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Services;

/* Inherit your application services from this class. */
public abstract class BookStoreAppService : ApplicationService
{
    protected BookStoreAppService()
    {
        LocalizationResource = typeof(BookStoreResource);
    }
}