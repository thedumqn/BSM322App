using Firebase.Database;
using Firebase.Database.Query;

namespace BSM322App;

public partial class GorevDetayPage : ContentPage
{
    private Gorev mevcutGorev;
    private FirebaseClient firebase;
    private bool yeniGorev;

    public GorevDetayPage(Gorev gorev, FirebaseClient firebaseClient)
    {
        InitializeComponent();
        firebase = firebaseClient;
        mevcutGorev = gorev;
        yeniGorev = gorev == null;

        if (!yeniGorev)
        {
            // Mevcut görevi form alanlarına yükle
            BaslikEntry.Text = gorev.Baslik;
            DetayEditor.Text = gorev.Detay;
            TarihPicker.Date = gorev.TarihSaat.Date;
            SaatPicker.Time = gorev.TarihSaat.TimeOfDay;
            TamamlandiCheckBox.IsChecked = gorev.Tamamlandi;
            Title = "Görev Düzenle";
        }
        else
        {
            // Yeni görev için varsayılan değerler
            TarihPicker.Date = DateTime.Today;
            SaatPicker.Time = DateTime.Now.TimeOfDay;
            Title = "Yeni Görev";
        }
    }

    private async void OnKaydetClicked(object sender, EventArgs e)
    {
        // Validasyon
        if (string.IsNullOrWhiteSpace(BaslikEntry.Text))
        {
            await DisplayAlert("Hata", "Görev başlığı boş olamaz!", "Tamam");
            return;
        }

        try
        {
            var tarihSaat = TarihPicker.Date.Add(SaatPicker.Time);

            if (yeniGorev)
            {
                // Yeni görev oluştur
                var yeniGorev = new Gorev
                {
                    Baslik = BaslikEntry.Text,
                    Detay = DetayEditor.Text ?? "",
                    TarihSaat = tarihSaat,
                    Tamamlandi = TamamlandiCheckBox.IsChecked
                };

                await firebase
                    .Child("gorevler")
                    .PostAsync(yeniGorev);
            }
            else
            {
                // Mevcut görevi güncelle
                mevcutGorev.Baslik = BaslikEntry.Text;
                mevcutGorev.Detay = DetayEditor.Text ?? "";
                mevcutGorev.TarihSaat = tarihSaat;
                mevcutGorev.Tamamlandi = TamamlandiCheckBox.IsChecked;

                await firebase
                    .Child("gorevler")
                    .Child(mevcutGorev.Id)
                    .PutAsync(mevcutGorev);
            }

            await DisplayAlert("Başarılı", "Görev kaydedildi!", "Tamam");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Görev kaydedilirken hata: {ex.Message}", "Tamam");
        }
    }

    private async void OnIptalClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}