using ZalandoShop.UWP.ViewModels;

namespace ZalandoShop.UWP.Views
{
    public sealed partial class SearchPage : MvvmPage
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        private SearchViewModel ViewModel => VmBase as SearchViewModel;
    }
}
