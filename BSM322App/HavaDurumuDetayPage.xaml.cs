using BSM322App.Services;
using Microsoft.Maui.Platform;

namespace BSM322App;

public partial class HavaDurumuDetayPage : ContentPage
{
    private SehirHavaDurumu _sehir;
    private HavaDurumuGoruntulemeMode _currentMode = HavaDurumuGoruntulemeMode.Tahmin;

    public HavaDurumuDetayPage(SehirHavaDurumu sehir)
    {
        InitializeComponent();
        _sehir = sehir;
        LoadSehirBilgileri();
        UpdateGoruntulemeModuButtons();
        LoadWeatherView();
    }

    private void LoadSehirBilgileri()
    {
        if (_sehir == null) return;

        lblSehirAdi.Text = _sehir.FormattedName;
        btnFavorite.Text = _sehir.FavoriteIcon;

        var koordinatlar = _sehir.Koordinatlar;
        if (koordinatlar.HasValue)
        {
            var (lat, lon) = koordinatlar.Value;
            lblKoordinatlar.Text = $"{lat:F4}°, {lon:F4}°";
            lblEnlem.Text = $"{lat:F6}°";
            lblBoylam.Text = $"{lon:F6}°";
        }
        else
        {
            lblKoordinatlar.Text = "Koordinat bilgisi yok";
            lblEnlem.Text = "Bilinmiyor";
            lblBoylam.Text = "Bilinmiyor";
        }

        lblSonGuncelleme.Text = _sehir.SonGuncellemeText;
        lblDetaySonGuncelleme.Text = _sehir.SonGuncelleme.ToString("dd.MM.yyyy HH:mm");
        lblEklenmeTarihi.Text = _sehir.EklenmeTarihi.ToString("dd.MM.yyyy HH:mm");
        lblFavoriDurumu.Text = _sehir.IsFavorite ? "⭐ Favori" : "☆ Normal";

        // Sayfa başlığını güncelle
        Title = $"{_sehir.FormattedName} - Detay";
    }

    private void LoadWeatherView()
    {
        if (_sehir == null) return;

        string url = _currentMode switch
        {
            HavaDurumuGoruntulemeMode.Tahmin => _sehir.TahminUrl,
            HavaDurumuGoruntulemeMode.SonDurum => _sehir.SonDurumUrl,
            HavaDurumuGoruntulemeMode.Klasik => _sehir.KlasikUrl,
            _ => _sehir.TahminUrl
        };

        mainWebView.Source = url;
    }

    private void UpdateGoruntulemeModuButtons()
    {
        // Tüm butonları normal yap
        btnTahmin.BackgroundColor = Colors.Transparent;
        btnSonDurum.BackgroundColor = Colors.Transparent;
        btnKlasik.BackgroundColor = Colors.Transparent;

        btnTahmin.TextColor = Color.FromArgb("#2E86AB");
        btnSonDurum.TextColor = Color.FromArgb("#2E86AB");
        btnKlasik.TextColor = Color.FromArgb("#2E86AB");

        // Aktif modu vurgula
        var primaryColor = Color.FromArgb("#2E86AB");
        switch (_currentMode)
        {
            case HavaDurumuGoruntulemeMode.Tahmin:
                btnTahmin.BackgroundColor = primaryColor;
                btnTahmin.TextColor = Colors.White;
                break;
            case HavaDurumuGoruntulemeMode.SonDurum:
                btnSonDurum.BackgroundColor = primaryColor;
                btnSonDurum.TextColor = Colors.White;
                break;
            case HavaDurumuGoruntulemeMode.Klasik:
                btnKlasik.BackgroundColor = primaryColor;
                btnKlasik.TextColor = Colors.White;
                break;
        }
    }

    private async void ToggleFavorite_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (_sehir != null)
            {
                _sehir.ToggleFavorite();
                btnFavorite.Text = _sehir.FavoriteIcon;
                lblFavoriDurumu.Text = _sehir.IsFavorite ? "⭐ Favori" : "☆ Normal";

                string durum = _sehir.IsFavorite ? "favorilere eklendi" : "favorilerden çıkarıldı";
                await DisplayAlert("Başarılı", $"{_sehir.FormattedName} {durum}!", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Favori işlemi sırasında hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void Yenile_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (_sehir != null)
            {
                _sehir.UpdateLastRefresh();
                LoadSehirBilgileri();
                LoadWeatherView();

                await DisplayAlert("Başarılı", $"{_sehir.FormattedName} güncellendi!", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Yenileme sırasında hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void Harita_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (_sehir?.Koordinatlar != null)
            {
                var (lat, lon) = _sehir.Koordinatlar.Value;
                var mapUrl = $"https://www.google.com/maps/@{lat},{lon},12z";

                bool openMap = await DisplayAlert("Harita",
                    $"{_sehir.FormattedName} şehrini Google Maps'te açmak istediğinizden emin misiniz?",
                    "Aç", "İptal");

                if (openMap)
                {
                    await Launcher.OpenAsync(mapUrl);
                }
            }
            else
            {
                await DisplayAlert("Hata", "Bu şehir için konum bilgisi bulunamadı.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Harita açılırken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void Paylas_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (_sehir != null)
            {
                var koordinatBilgisi = _sehir.Koordinatlar?.ToString() ?? "Koordinat yok";
                var paylasilacakMetin = $@"🌤️ {_sehir.FormattedName} Hava Durumu

📍 Konum: {koordinatBilgisi}
🔄 Son Güncelleme: {_sehir.SonGuncelleme:dd.MM.yyyy HH:mm}
⭐ Favori: {(_sehir.IsFavorite ? "Evet" : "Hayır")}

📱 BSM322 Hava Durumu Uygulaması ile paylaşıldı";

                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = paylasilacakMetin,
                    Title = $"{_sehir.FormattedName} Hava Durumu"
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Paylaşım sırasında hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void Ayarlar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var secim = await DisplayActionSheet(
                "Detay Ayarları",
                "İptal",
                null,
                "Şehri Ana Listeden Sil",
                "Favoriye Ekle/Çıkar",
                "MGM Sitesinde Aç",
                "Koordinatları Kopyala");

            switch (secim)
            {
                case "Şehri Ana Listeden Sil":
                    await SehirSil();
                    break;
                case "Favoriye Ekle/Çıkar":
                    ToggleFavorite_Clicked(sender, e);
                    break;
                case "MGM Sitesinde Aç":
                    await MgmSitesiniAc();
                    break;
                case "Koordinatları Kopyala":
                    await KoordinatlariKopyala();
                    break;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Ayarlar işlemi sırasında hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private void GoruntulemeModuDegistir_Clicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var mode = button?.CommandParameter?.ToString();

            if (Enum.TryParse<HavaDurumuGoruntulemeMode>(mode, out var newMode))
            {
                _currentMode = newMode;
                UpdateGoruntulemeModuButtons();
                LoadWeatherView();
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Hata", $"Görüntüleme modu değiştirilirken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async Task SehirSil()
    {
        if (_sehir != null)
        {
            bool onay = await DisplayAlert("Silme Onayı",
                $"{_sehir.FormattedName} şehrini ana listeden silmek istediğinizden emin misiniz? Bu işlem geri alınamaz!",
                "Sil", "İptal");

            if (onay)
            {
                // Ana sayfaya geri dön ve silme işlemini tetikle
                MessagingCenter.Send(this, "DeleteCity", _sehir);
                await Navigation.PopAsync();
            }
        }
    }

    private async Task MgmSitesiniAc()
    {
        try
        {
            var mgmUrl = "https://www.mgm.gov.tr/";
            await Launcher.OpenAsync(mgmUrl);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"MGM sitesi açılırken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async Task KoordinatlariKopyala()
    {
        try
        {
            if (_sehir?.Koordinatlar != null)
            {
                var (lat, lon) = _sehir.Koordinatlar.Value;
                var koordinatMetni = $"{lat:F6}, {lon:F6}";

                await Clipboard.SetTextAsync(koordinatMetni);
                await DisplayAlert("Başarılı",
                    $"{_sehir.FormattedName} koordinatları panoya kopyalandı!\n{koordinatMetni}",
                    "Tamam");
            }
            else
            {
                await DisplayAlert("Hata", "Bu şehir için koordinat bilgisi bulunamadı.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Koordinat kopyalama sırasında hata oluştu: {ex.Message}", "Tamam");
        }
    }
}