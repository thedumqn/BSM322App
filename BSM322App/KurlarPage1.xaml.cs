using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Maui.Controls;

namespace BSM322App;

public partial class KurlarPage : ContentPage
{
    public ObservableCollection<DovizKuru> Kurlar { get; set; }

    public KurlarPage()
    {
        InitializeComponent();
        Kurlar = new ObservableCollection<DovizKuru>();
        BindingContext = this;
        _ = KurlariYukle(); // Sayfa açıldığında veriyi çek
    }

    private async void OnYenileClicked(object sender, EventArgs e)
    {
        await KurlariYukle();
    }

    private async Task KurlariYukle()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            YenileButton.IsEnabled = false;
            YenileButton.Text = "Yükleniyor...";

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);
            var response = await client.GetStringAsync("https://finans.truncgil.com/today.json");

            var jObject = JObject.Parse(response);
            Kurlar.Clear();

            foreach (var item in jObject)
            {
                // Güncelleme tarihi alanı değilse
                if (item.Key == "Update_Date" || item.Key == "Güncelleme Tarihi")
                    continue;

                try
                {
                    var detay = item.Value.ToObject<KurDetay>();
                    if (detay != null)
                    {
                        Kurlar.Add(new DovizKuru
                        {
                            Ad = item.Key,
                            AlisFiyat = FormatPrice(detay.Alis),
                            SatisFiyat = FormatPrice(detay.Satis),
                            Degisim = detay.Degisim,
                            Fark = detay.Degisim,
                            YonOk = GetYonOk(detay.Degisim),
                            YonRenk = GetYonRenk(detay.Degisim)
                        });
                    }
                }
                catch
                {
                    continue;
                }
            }

            // Güncelleme tarihi varsa göster
            var updateDate = jObject["Update_Date"]?.ToString() ?? jObject["Güncelleme Tarihi"]?.ToString();
            if (!string.IsNullOrEmpty(updateDate))
            {
                GuncellemeTarihiLabel.Text = $"Son Güncelleme: {updateDate}";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Bağlantı Hatası", $"İnternet bağlantınızı kontrol edin.\n{ex.Message}", "Tamam");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            YenileButton.IsEnabled = true;
            YenileButton.Text = "Kurları Yenile";
        }
    }


    private string FormatPrice(string price)
    {
        if (string.IsNullOrWhiteSpace(price)) return "-";
        price = price.Replace(",", "."); // Nokta ondalıklı yap
        if (decimal.TryParse(price, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
            return parsed.ToString("N2") + " ₺";
        return price + " ₺";
    }

    private string GetYonOk(string degisim)
    {
        if (degisim.Contains("-")) return "▼";
        if (degisim.Contains("+")) return "▲";
        return "-";
    }

    private Color GetYonRenk(string degisim)
    {
        if (degisim.Contains("-")) return Colors.Red;
        if (degisim.Contains("+")) return Colors.Green;
        return Colors.Gray;
    }
}

// Döviz ekranında gösterilecek model
public class DovizKuru
{
    public string Ad { get; set; }
    public string AlisFiyat { get; set; }
    public string SatisFiyat { get; set; }
    public string Degisim { get; set; }
    public string Fark { get; set; }
    public string YonOk { get; set; }
    public Color YonRenk { get; set; }
}

// API'den gelen JSON objesini temsil eder
public class KurDetay
{
    [JsonProperty("Alış")]
    public string Alis { get; set; }

    [JsonProperty("Satış")]
    public string Satis { get; set; }

    [JsonProperty("Değişim")]
    public string Degisim { get; set; }
}

