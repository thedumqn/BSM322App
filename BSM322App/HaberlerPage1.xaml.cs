using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BSM322App.HaberApi;

namespace BSM322App;

public partial class HaberlerPage1 : ContentPage, INotifyPropertyChanged
{
    private ObservableCollection<Item> _haberler = new();
    private bool _isRefreshing = false;
    private bool _isLoading = false;

    public ObservableCollection<Item> Haberler
    {
        get => _haberler;
        set { _haberler = value; OnPropertyChanged(); }
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; OnPropertyChanged(); }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
            loadingIndicator.IsVisible = value;
            loadingIndicator.IsRunning = value;
        }
    }

    public HaberlerPage1()
    {
        InitializeComponent();
        BindingContext = this;
        InitializeApp();
    }

    private void InitializeApp()
    {
        try
        {
            kategoriPicker.ItemsSource = HaberServisi.HaberKategorileri;
            if (HaberServisi.HaberKategorileri.Count > 0)
                kategoriPicker.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            DisplayAlert("Hata", $"Başlatma hatası: {ex.Message}", "Tamam");
        }
    }

    private async void KategoriPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (kategoriPicker.SelectedItem is HaberKategori secilenKategori)
            await HaberleriYukle(secilenKategori);
    }

    private async Task HaberleriYukle(HaberKategori kategori)
    {
        if (IsLoading) return;
        try
        {
            IsLoading = true;
            Haberler.Clear();

            var haberListesi = await HaberServisi.GetHaberler(kategori);

            if (haberListesi?.Count > 0)
                foreach (var haber in haberListesi)
                    Haberler.Add(haber);
            else
                await DisplayAlert("Bilgi", "Bu kategoride haber yok.", "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Haberler alınamadı: {ex.Message}", "Tamam");
        }
        finally
        {
            IsLoading = false;
            IsRefreshing = false;
        }
    }

    private async void HaberlerCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Item secilenHaber)
        {
            await HaberDetayinAc(secilenHaber);
            haberlerCollectionView.SelectedItem = null;
        }
    }

    private async void HaberItem_Tapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Item haber)
            await HaberDetayinAc(haber);
    }

    private async Task HaberDetayinAc(Item haber)
    {
        try
        {
            var detayPage = new HaberDetayPage(haber);
            await Navigation.PushAsync(detayPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Detay sayfası açılırken hata: {ex.Message}", "Tamam");
        }
    }

    public Command RefreshCommand => new(async () =>
    {
        if (kategoriPicker.SelectedItem is HaberKategori kategori)
            await HaberleriYukle(kategori);
    });

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected new virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Haberler.Count == 0 && kategoriPicker.SelectedItem is HaberKategori kategori)
            Task.Run(async () => await HaberleriYukle(kategori));
    }
}
