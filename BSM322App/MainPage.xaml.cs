using Microsoft.Maui.Controls;

namespace BSM322App;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    // Döviz Kurları sayfasına git
    private async void OnKurlarClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new KurlarPage());
    }

    // Haberler sayfasına git
    private async void OnHaberlerClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HaberlerPage1());
    }

    // Hava Durumu sayfasına git
    private async void OnHavaDurumuClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HavaDurumuPage());
    }

    // Yapılacaklar sayfasına git
    private async void OnYapilacaklarClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new YapilacaklarPage());
    }

    // Ayarlar sayfasına git
    private async void OnAyarlarClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AyarlarPage());
    }
}