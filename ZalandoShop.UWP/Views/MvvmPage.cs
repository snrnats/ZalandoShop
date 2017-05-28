using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ZalandoShop.UWP.ViewModels.Common;

namespace ZalandoShop.UWP.Views
{
    public abstract class MvvmPage : Page
    {
        protected ViewModelBase VmBase => (ViewModelBase) DataContext;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            VmBase?.Activate(e.Parameter);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            VmBase?.Deactivate(e.Parameter);
        }
    }
}
