namespace BSM322App;

public partial class AyarlarPage : ContentPage
{
    public AyarlarPage()
    {
        InitializeComponent();

        // Sayfa açıldığında tema ayarını switch'e yansıt
        bool mevcutTema = Preferences.Get("KoyuTema", false);
        KoyuTemaSwitch.IsToggled = mevcutTema;
    }

    private void OnKoyuTemaToggled(object sender, ToggledEventArgs e)
    {
        bool koyuTema = e.Value;

        // Tema tercihini kaydet
        Preferences.Set("KoyuTema", koyuTema);

        // Uygulama temasını uygula
        Application.Current.UserAppTheme = koyuTema ? AppTheme.Dark : AppTheme.Light;

        DisplayAlert("Bilgi", "Tema değiştirildi!", "Tamam");
    }

    private async void OnCikisYapClicked(object sender, EventArgs e)
    {
        bool onay = await DisplayAlert("Çıkış Onayı", "Uygulamadan çıkmak istiyor musunuz?", "Evet", "Hayır");
        if (onay)
        {
            // Giriş ekranına dönmek istersen:
            // await Shell.Current.GoToAsync("//LoginPage");

            // Uygulamayı kapat
            Application.Current.Quit();
        }
    }
}
