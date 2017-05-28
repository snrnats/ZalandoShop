using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ZalandoShop.UWP.Platform;
using ZalandoShop.UWP.Services;
using ZalandoShop.UWP.Services.CommonApi;
using ZalandoShop.UWP.Services.ZalandoShop;
using ZalandoShop.UWP.Views;

namespace ZalandoShop.UWP.ViewModels
{
    public class ViewModelLocator
    {
        private readonly NavigationServiceEx _navigationService = new NavigationServiceEx();
        private ProgressService _progressService;

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register(() => _navigationService);
            SimpleIoc.Default.Register(() => _progressService ?? (_progressService = (ProgressService) Application.Current.Resources["ProgressService"]));
            SimpleIoc.Default.Register<IShopApiService, ZalandoShopService>();
            Register<SearchViewModel, SearchPage>();
            Register<ArticlesViewModel, ArticlesPage>();
        }

        public ArticlesViewModel ArticlesViewModel => ServiceLocator.Current.GetInstance<ArticlesViewModel>();

        public SearchViewModel SearchViewModel => ServiceLocator.Current.GetInstance<SearchViewModel>();

        public void Register<VM, V>() where VM : class
        {
            SimpleIoc.Default.Register<VM>();
            _navigationService.Configure(typeof(VM).FullName, typeof(V));
        }
    }
}
