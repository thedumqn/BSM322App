using Firebase.Auth;
using Firebase.Auth.Providers;

namespace BSM322App;

public partial class LoginPage : ContentPage
{
    private FirebaseAuthClient authClient;

    public LoginPage()
    {
        InitializeComponent();

        // Firebase Auth Client yapılandırması
        var config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyBQfrtA298P6zX_4npMfwvO-PcDTbt3hxU",
            // AuthDomain'i doğru formatta yazın - proje ID'niz gerekli
            AuthDomain = "gorselodevbsm322-1d6ae.firebaseapp.com", // Bu kısmı değiştirin
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        authClient = new FirebaseAuthClient(config);
    }

    private async void OnGirisYapClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text?.Trim();

        // Validasyon
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Hata", "E-posta ve şifre alanları boş olamaz!", "Tamam");
            return;
        }

        try
        {
            // Firebase ile giriş yap
            var userCredential = await authClient.SignInWithEmailAndPasswordAsync(email, password);

            if (userCredential?.User != null)
            {
                await DisplayAlert("Başarılı", $"Giriş başarılı! Hoş geldiniz {userCredential.User.Info.Email}", "Tamam");
                // Ana sayfaya git
                Application.Current.MainPage = new AppShell();
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
        catch (FirebaseAuthException firebaseEx)
        {
            // Detaylı hata mesajı için
            await DisplayAlert("Firebase Hatası", $"Hata: {firebaseEx.Message}\nReason: {firebaseEx.Reason}", "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Genel Hata", $"Beklenmeyen hata: {ex.Message}", "Tamam");
        }
    }

    private async void OnKayitOlClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}