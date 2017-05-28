using ZalandoShop.UWP.ViewModels;

namespace ZalandoShop.UWP.Views
{
    public sealed partial class ArticlesPage : MvvmPage
    {
        public ArticlesPage()
        {
            InitializeComponent();
        }

        private ArticlesViewModel ViewModel => VmBase as ArticlesViewModel;
    }
}
