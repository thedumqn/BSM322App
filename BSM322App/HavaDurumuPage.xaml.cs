using BSM322App.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace BSM322App;

public partial class HavaDurumuPage : ContentPage
{
    public ObservableCollection<SehirHavaDurumu> Sehirler { get; set; } = new();
    private HavaDurumuGoruntulemeMode _currentMode = HavaDurumuGoruntulemeMode.Tahmin;

    public HavaDurumuPage()
    {
        InitializeComponent();
        UpdateUI();
        UpdateGoruntulemeModuButtons();

        // MessagingCenter ile şehir silme işlemini dinle
        MessagingCenter.Subscribe<HavaDurumuDetayPage, SehirHavaDurumu>(
            this, "DeleteCity", (sender, sehir) =>
            {
                if (Sehirler.Contains(sehir))
                {
                    Sehirler.Remove(sehir);
                    UpdateUI();
                }
            });
    }

    private void UpdateUI()
    {
        bool hasItems = Sehirler.Count > 0;
        stackEmpty.IsVisible = !hasItems;
        lstSehirler.IsVisible = hasItems;

        if (hasItems)
        {
            lstSehirler.ItemsSource = Sehirler;
        }

        lblToplamSehir.Text = $"{Sehirler.Count} şehir takip ediliyor";
    }

    private async void SehirEkle_Clicked(object sender, EventArgs e)
    {
        try
        {
            var sehir = await DisplayPromptAsync(
                "Şehir Ekle",
                "Lütfen şehir ismini giriniz:",
                "Tamam",
                "İptal",
                placeholder: "Örn: Ankara, İstanbul...");

            if (!string.IsNullOrWhiteSpace(sehir))
            {
                // Şehir ismini validate et
                if (!HavaDurumuServisi.IsValidCity(sehir))
                {
                    await DisplayAlert("Hata", "Lütfen geçerli bir şehir ismi giriniz.", "Tamam");
                    return;
                }

                // Aynı şehir zaten eklenmiş mi kontrol et
                var normalizedInput = sehir.ToUpper().Trim();
                bool alreadyExists = Sehirler.Any(s =>
                    s.Name.ToUpper().Trim() == normalizedInput);

                if (alreadyExists)
                {
                    await DisplayAlert("Bilgi", "Bu şehir zaten eklenmiş.", "Tamam");
                    return;
                }

                var yeniSehir = new SehirHavaDurumu(sehir);
                Sehirler.Add(yeniSehir);
                UpdateUI();

                // Başarı mesajı
                await DisplayAlert("Başarılı", $"{sehir} başarıyla eklendi!", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Şehir eklenirken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void PopulerSehirler_Clicked(object sender, EventArgs e)
    {
        try
        {
            var populerSehirler = HavaDurumuServisi.GetPopularCities();
            var secim = await DisplayActionSheet(
                "Popüler Şehirler",
                "İptal",
                null,
                populerSehirler.ToArray());

            if (!string.IsNullOrEmpty(secim) && secim != "İptal")
            {
                // Aynı şehir zaten eklenmiş mi kontrol et
                bool alreadyExists = Sehirler.Any(s =>
                    s.Name.ToUpper().Trim() == secim.ToUpper().Trim());

                if (alreadyExists)
                {
                    await DisplayAlert("Bilgi", "Bu şehir zaten eklenmiş.", "Tamam");
                    return;
                }

                var yeniSehir = new SehirHavaDurumu(secim);
                Sehirler.Add(yeniSehir);
                UpdateUI();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Popüler şehirler yüklenirken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void Update_Clicked(object sender, EventArgs e)
    {
        try
        {
            var seh = (sender as Button)?.CommandParameter as SehirHavaDurumu;
            if (seh != null)
            {
                seh.UpdateLastRefresh();

                // WebView'i yeniden yükle
                await RefreshWebViewForCity(seh);

                await DisplayAlert("Başarılı", $"{seh.FormattedName} güncellendi!", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Güncelleme sırasında hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        try
        {
            var seh = (sender as Button)?.CommandParameter as SehirHavaDurumu;
            if (seh != null)
            {
                bool onay = await DisplayAlert("Silme Onayı",
                    $"{seh.FormattedName} şehrini listeden silmek istediğinizden emin misiniz?",
                    "Evet", "Hayır");

                if (onay)
                {
                    Sehirler.Remove(seh);
                    UpdateUI();
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Silme işlemi sırasında hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private void Favorite_Clicked(object sender, EventArgs e)
    {
        try
        {
            var seh = (sender as Button)?.CommandParameter as SehirHavaDurumu;
            if (seh != null)
            {
                seh.ToggleFavorite();

                // UI'yi güncelle (ObservableCollection otomatik güncelleme yapmayabilir)
                var index = Sehirler.IndexOf(seh);
                if (index >= 0)
                {
                    Sehirler[index] = seh;
                }
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Hata", $"Favori işlemi sırasında hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void ShowDetail_Clicked(object sender, EventArgs e)
    {
        try
        {
            var seh = (sender as Button)?.CommandParameter as SehirHavaDurumu;
            if (seh != null)
            {
                var detayPage = new HavaDurumuDetayPage(seh);
                await Navigation.PushAsync(detayPage);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Detay sayfası açılırken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void TumunuYenile_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (Sehirler.Count == 0)
            {
                await DisplayAlert("Bilgi", "Yenilenecek şehir bulunamadı.", "Tamam");
                return;
            }

            bool onay = await DisplayAlert("Toplu Yenileme",
                $"{Sehirler.Count} şehir yenilenecek. Devam etmek istediğinizden emin misiniz?",
                "Evet", "Hayır");

            if (onay)
            {
                foreach (var sehir in Sehirler)
                {
                    sehir.UpdateLastRefresh();
                }

                // Tüm WebView'ları yenile
                await RefreshAllWebViews();

                await DisplayAlert("Başarılı", "Tüm şehirler güncellendi!", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Toplu yenileme sırasında hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void Ayarlar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var secim = await DisplayActionSheet(
                "Ayarlar",
                "İptal",
                "Tüm Şehirleri Sil",
                "Favorileri Göster",
                "Uygulama Hakkında");

            switch (secim)
            {
                case "Tüm Şehirleri Sil":
                    await TumSehirleriSil();
                    break;
                case "Favorileri Göster":
                    await FavorileriGoster();
                    break;
                case "Uygulama Hakkında":
                    await UygulamaHakkindaGoster();
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
                RefreshAllWebViews();
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Hata", $"Görüntüleme modu değiştirilirken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private void UpdateGoruntulemeModuButtons()
    {
        // Tüm butonları normal yap
        btnTahmin.BackgroundColor = Colors.Transparent;
        btnSonDurum.BackgroundColor = Colors.Transparent;
        btnKlasik.BackgroundColor = Colors.Transparent;

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

    private async Task RefreshWebViewForCity(SehirHavaDurumu sehir)
    {
        // WebView'ı güncelle - bu implementasyon platform-spesifik olabilir
        await Task.Delay(100); // UI update için kısa bekleme
    }

    private async Task RefreshAllWebViews()
    {
        // Tüm WebView'ları güncelle
        foreach (var sehir in Sehirler)
        {
            await RefreshWebViewForCity(sehir);
        }
    }

    private async Task TumSehirleriSil()
    {
        if (Sehirler.Count == 0)
        {
            await DisplayAlert("Bilgi", "Silinecek şehir bulunamadı.", "Tamam");
            return;
        }

        bool onay = await DisplayAlert("Dikkat!",
            $"Tüm şehirler ({Sehirler.Count} adet) silinecek. Bu işlem geri alınamaz!",
            "Sil", "İptal");

        if (onay)
        {
            Sehirler.Clear();
            UpdateUI();
            await DisplayAlert("Başarılı", "Tüm şehirler silindi.", "Tamam");
        }
    }

    private async Task FavorileriGoster()
    {
        var favoriler = Sehirler.Where(s => s.IsFavorite).ToList();

        if (favoriler.Count == 0)
        {
            await DisplayAlert("Bilgi", "Henüz favori şehir eklenmemiş.", "Tamam");
            return;
        }

        var favoriListesi = string.Join("\n", favoriler.Select(f => $"⭐ {f.FormattedName}"));
        await DisplayAlert("Favori Şehirler", favoriListesi, "Tamam");
    }

    private async Task UygulamaHakkindaGoster()
    {
        var hakkinda = @"BSM322 Hava Durumu Uygulaması

📱 Versiyon: 1.0.0
👨‍💻 Geliştirici: BSM322 Öğrencisi
🌤️ Veri Kaynağı: Meteoroloji Genel Müdürlüğü

✨ Özellikler:
• 5 günlük hava durumu tahmini
• Anlık hava durumu bilgisi
• Klasik görüntüleme modu
• Favori şehirler
• Harita entegrasyonu
• Modern ve kullanıcı dostu arayüz

📧 Destek: BSM322 dersi kapsamında geliştirilmiştir.";

        await DisplayAlert("Uygulama Hakkında", hakkinda, "Tamam");
    }
}