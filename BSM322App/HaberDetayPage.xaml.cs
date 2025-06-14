using BSM322App.HaberApi;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BSM322App
{
    public partial class HaberDetayPage : ContentPage, INotifyPropertyChanged
    {
        private Item _haber;
        private bool _isLoading = true;

        public Item Haber
        {
            get => _haber;
            set
            {
                _haber = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                UpdateLoadingState();
            }
        }

        public HaberDetayPage(Item haber)
        {
            InitializeComponent();
            Haber = haber;
            BindingContext = this;

            InitializePage();
        }

        private void InitializePage()
        {
            try
            {
                // Sayfa başlığını güncelle
                Title = $"📖 {Haber.KisaBaslik}";

                // WebView'e haber linkini yükle
                LoadHaberContent();

                // WebView olaylarını dinle
                haberDetayWebView.Navigating += WebView_Navigating;
                haberDetayWebView.Navigated += WebView_Navigated;
            }
            catch (Exception ex)
            {
                ShowError($"Sayfa başlatılırken hata oluştu: {ex.Message}");
            }
        }

        private void LoadHaberContent()
        {
            try
            {
                if (!string.IsNullOrEmpty(Haber.link))
                {
                    IsLoading = true;
                    haberDetayWebView.Source = Haber.link;
                }
                else
                {
                    ShowError("Haber linki bulunamadı");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Haber yüklenirken hata oluştu: {ex.Message}");
            }
        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            IsLoading = true;
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            IsLoading = false;

            if (e.Result != WebNavigationResult.Success)
            {
                ShowError("Haber sayfası yüklenemedi. İnternet bağlantınızı kontrol edin.");
            }
        }

        private void UpdateLoadingState()
        {
            loadingFrame.IsVisible = IsLoading;
            loadingIndicator.IsRunning = IsLoading;
        }

        private void ShowError(string message)
        {
            IsLoading = false;
            errorFrame.IsVisible = true;
            errorMessageLabel.Text = message;
            haberDetayWebView.IsVisible = false;
        }

        private void HideError()
        {
            errorFrame.IsVisible = false;
            haberDetayWebView.IsVisible = true;
        }

        private async void HaberPaylas_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShareHaber(Haber.link, Share.Default);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Paylaşım sırasında hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private async void TarayicidaAc_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Haber.link))
                {
                    var uri = new Uri(Haber.link);
                    await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                }
                else
                {
                    await DisplayAlert("Hata", "Haber linki bulunamadı", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Tarayıcı açılırken hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private async void HaberYenile_Clicked(object sender, EventArgs e)
        {
            try
            {
                HideError();
                LoadHaberContent();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Yenileme sırasında hata oluştu: {ex.Message}", "Tamam");
            }
        }

        public async Task ShareHaber(string uri, IShare share)
        {
            try
            {
                await share.RequestAsync(new ShareTextRequest
                {
                    Uri = uri,
                    Title = Haber.title,
                    Text = $"📰 {Haber.title}\n\n{Haber.KisaAciklama}\n\n🔗 {uri}\n\n📱 BSM322App ile paylaşıldı"
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Paylaşım Hatası", $"Paylaşım yapılırken hata oluştu: {ex.Message}", "Tamam");
            }
        }

        // Back button davranışını özelleştir
        protected override bool OnBackButtonPressed()
        {
            try
            {
                // WebView'de geri gidebilir miyiz kontrol et
                if (haberDetayWebView.CanGoBack)
                {
                    haberDetayWebView.GoBack();
                    return true; // Geri tuşunu işledi, sayfadan çıkma
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Back button hata: {ex.Message}");
            }

            return base.OnBackButtonPressed(); // Normal geri işlemi
        }

        // Sayfa kaybolurken temizlik
        protected override void OnDisappearing()
        {
            try
            {
                // WebView olaylarını temizle
                if (haberDetayWebView != null)
                {
                    haberDetayWebView.Navigating -= WebView_Navigating;
                    haberDetayWebView.Navigated -= WebView_Navigated;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnDisappearing hata: {ex.Message}");
            }

            base.OnDisappearing();
        }

        // INotifyPropertyChanged implementation
        public new event PropertyChangedEventHandler? PropertyChanged;

        protected new virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}