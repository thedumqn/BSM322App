using Firebase.Auth;
using Firebase.Auth.Providers;

namespace BSM322App;

public partial class RegisterPage : ContentPage
{
    private FirebaseAuthClient authClient;

    public RegisterPage()
    {
        InitializeComponent();

        // Firebase Auth Client yapılandırması
        var config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyBQfrtA298P6zX_4npMfwvO-PcDTbt3hxU",
            AuthDomain = "gorselodevbsm322-1d6ae.firebaseapp.com", // Firebase Console'dan proje ID'nizi alın
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        authClient = new FirebaseAuthClient(config);
    }

    private async void OnKayitOlClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text?.Trim();
        var passwordConfirm = PasswordConfirmEntry.Text?.Trim();

        // Validasyon
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirm))
        {
            await DisplayAlert("Hata", "Tüm alanları doldurun!", "Tamam");
            return;
        }

        if (password != passwordConfirm)
        {
            await DisplayAlert("Hata", "Şifreler eşleşmiyor!", "Tamam");
            return;
        }

        if (password.Length < 6)
        {
            await DisplayAlert("Hata", "Şifre en az 6 karakter olmalıdır!", "Tamam");
            return;
        }

        try
        {
            // Firebase ile kayıt ol
            var userCredential = await authClient.CreateUserWithEmailAndPasswordAsync(email, password);

            if (userCredential?.User != null)
            {
                await DisplayAlert("Başarılı", "Kayıt başarılı! Şimdi giriş yapabilirsiniz.", "Tamam");
                await Navigation.PopAsync();
            }
        }
        catch (FirebaseAuthException firebaseEx)
        {
            string errorMessage = firebaseEx.Reason switch
            {
                AuthErrorReason.WeakPassword => "Şifre çok zayıf. Daha güçlü bir şifre seçin",
                AuthErrorReason.EmailExists => "Bu e-posta adresi zaten kullanımda",
                AuthErrorReason.InvalidEmailAddress => "Geçersiz e-posta adresi formatı",
                AuthErrorReason.Unknown => "Bilinmeyen bir hata oluştu",
                _ => $"Kayıt hatası: {firebaseEx.Reason}"
            };
            await DisplayAlert("Hata", errorMessage, "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Beklenmeyen hata: {ex.Message}", "Tamam");
        }
    }

    private async void OnGeriDonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}