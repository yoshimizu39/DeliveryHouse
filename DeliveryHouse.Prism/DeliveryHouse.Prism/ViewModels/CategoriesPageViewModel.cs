using DeliveryHouse.Common.Entities;
using DeliveryHouse.Common.Responses;
using DeliveryHouse.Common.Services;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;

namespace DeliveryHouse.Prism.ViewModels
{
    public class CategoriesPageViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private ObservableCollection<Category> _categories;
        private bool _isRunning;
        private string _search;
        private List<Category> _mycategiries;
        private DelegateCommand _serachCommand;

        public CategoriesPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            Title = "Categories";
            _apiService = apiService;
            LoadCategoryAsync();
        }

        public string Search
        {
            get => _search;
            set
            {
                SetProperty(ref _search, value);
                ShowCategories();
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public DelegateCommand SearchCommand => _serachCommand ?? (_serachCommand = new DelegateCommand(ShowCategories));

        private async void LoadCategoryAsync()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await App.Current.MainPage.DisplayAlert("Erro", "No tiene conexión a Internet", "Accept");
                return;
            }

            IsRunning = true;
            string url = App.Current.Resources["UrlAPI"].ToString();
            Response response = await _apiService.GetListAsync<Category>(url, "/api", "/Categories");
            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Erro", response.Message, "Accept");
                return;
            }

            _mycategiries = (List<Category>)response.Result;
            ShowCategories();
            ////Categories = (List<Category>)response.Result;
            //List<Category> listCategories = (List<Category>)response.Result;
            //Categories = new ObservableCollection<Category>(listCategories.Select(c => new Category
            //{
            //    Id = c.Id,
            //    ImageCategory = c.ImageCategory,
            //    //ImageFullPath = c.ImageFullPath,
            //    Name = c.Name,
            //    Products = c.Products
            //})
            //    .OrderBy(c => c.Name)
            //    .ToList());                
        }

        private void ShowCategories()
        {
            if (string.IsNullOrEmpty(Search))
            {
                Categories = new ObservableCollection<Category>(_mycategiries);
            }
            else
            {
                Categories = new ObservableCollection<Category>(_mycategiries.Where(c => c.Name.ToLower().Contains(Search.ToLower())));
            }
        }

    }
}
