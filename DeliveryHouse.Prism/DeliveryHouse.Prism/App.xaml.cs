using DeliveryHouse.Prism.ViewModels;
using DeliveryHouse.Common.Services;
using DeliveryHouse.Prism.Views;
using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Syncfusion.Licensing;

namespace DeliveryHouse.Prism
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            SyncfusionLicenseProvider.RegisterLicense("MzkzMzE2QDMxMzgyZTM0MmUzMGF1TjR4dFRROTE5dWV4d055ZkQzQnQ0K3B3aVdUTW0yZW1YM3lFMzdFOEU9");
            InitializeComponent();

            await NavigationService.NavigateAsync($"NavigationPage/{nameof(CategoriesPage)}");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<CategoriesPage, CategoriesPageViewModel>();
            containerRegistry.RegisterForNavigation<CategoriesPage, CategoriesPageViewModel>();
        }
    }
}
