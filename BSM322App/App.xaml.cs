using Microsoft.Maui.Controls;
using Microsoft.Maui;
using BSM322App; // AppShell için gerekli

namespace BSM322App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent(); // Bu metod App.xaml ile bağlantılıdır

            // Tema ayarını yükle ve uygula
            var koyuTema = Preferences.Get("KoyuTema", false);
            UserAppTheme = koyuTema ? AppTheme.Dark : AppTheme.Light;

            MainPage = new AppShell();
        }
    }
}
