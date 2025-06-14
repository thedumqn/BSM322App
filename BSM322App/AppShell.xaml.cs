namespace BSM322App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Sayfa rotalarını kaydet
        Routing.RegisterRoute("HaberDetayPage", typeof(HaberDetayPage));
        Routing.RegisterRoute("GorevDetayPage", typeof(GorevDetayPage));
    }
}