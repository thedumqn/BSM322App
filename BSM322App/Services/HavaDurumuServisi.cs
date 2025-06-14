using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace BSM322App.Services
{
    public static class HavaDurumuServisi
    {
        private static readonly HttpClient httpClient = new HttpClient();

        // Türkiye'nin büyük şehirleri ve koordinatları
        private static readonly Dictionary<string, (double lat, double lon)> SehirKoordinatlari = new()
        {
            {"ANKARA", (39.9334, 32.8597)},
            {"ISTANBUL", (41.0082, 28.9784)},
            {"IZMIR", (38.4237, 27.1428)},
            {"BURSA", (40.1826, 29.0665)},
            {"ANTALYA", (36.8969, 30.7133)},
            {"ADANA", (37.0000, 35.3213)},
            {"KONYA", (37.8667, 32.4833)},
            {"GAZIANTEP", (37.0594, 37.3825)},
            {"KAYSERI", (38.7312, 35.4787)},
            {"TRABZON", (41.0015, 39.7178)}
        };

        public static async Task<string> GetHavaDurumuUrlByCity(string city)
        {
            string url = "";
            city = NormalizeCityName(city);

            // Özel durumlar
            if (city == "KAHRAMANMARAS")
                city = "K.MARAS";
            else if (city == "AFYON")
                city = "AFYONKARAHISAR";

            // Geliştirilmiş URL - daha iyi görsel parametrelerle
            url = $"https://www.mgm.gov.tr/sunum/tahmin-show-2.aspx?m={city}&basla=1&bitir=5&rC=2E86AB&rZ=F8FAFC";

            return url;
        }

        public static string GetSonDurumUrl(string city)
        {
            city = NormalizeCityName(city);

            if (city == "KAHRAMANMARAS")
                city = "K.MARAS";
            else if (city == "AFYON")
                city = "AFYONKARAHISAR";

            return $"https://www.mgm.gov.tr/sunum/sondurum-show-2.aspx?m={city}&rC=2E86AB&rZ=F8FAFC";
        }

        public static string GetKlasikTahminUrl(string city)
        {
            city = NormalizeCityName(city);

            if (city == "KAHRAMANMARAS")
                city = "K.MARAS";
            else if (city == "AFYON")
                city = "AFYONKARAHISAR";

            return $"https://www.mgm.gov.tr/sunum/tahmin-klasik-5070.aspx?m={city}&basla=1&bitir=5&rC=2E86AB&rZ=F8FAFC";
        }

        private static string NormalizeCityName(string city)
        {
            if (string.IsNullOrEmpty(city)) return "";

            city = city.ToUpper().Trim();

            // Türkçe karakterleri normalize et
            city = city.Replace("Ç", "C")
                      .Replace("Ğ", "G")
                      .Replace("İ", "I")
                      .Replace("Ö", "O")
                      .Replace("Ş", "S")
                      .Replace("Ü", "U")
                      .Replace("ç", "c")
                      .Replace("ğ", "g")
                      .Replace("ı", "i")
                      .Replace("ö", "o")
                      .Replace("ş", "s")
                      .Replace("ü", "u");

            return city;
        }

        public static bool IsValidCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return false;

            var normalizedCity = NormalizeCityName(city);
            return SehirKoordinatlari.ContainsKey(normalizedCity) ||
                   normalizedCity.Length >= 2; // En az 2 karakter
        }

        public static List<string> GetPopularCities()
        {
            return SehirKoordinatlari.Keys.ToList();
        }

        public static (double lat, double lon)? GetCityCoordinates(string city)
        {
            var normalizedCity = NormalizeCityName(city);
            return SehirKoordinatlari.TryGetValue(normalizedCity, out var coords) ? coords : null;
        }
    }

    public class SehirHavaDurumu
    {
        public SehirHavaDurumu(string name)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
            EklenmeTarihi = DateTime.Now;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime SonGuncelleme { get; set; } = DateTime.Now;
        public bool IsFavorite { get; set; } = false;

        // Farklı görüntüleme modları için URL'ler
        public string TahminUrl => HavaDurumuServisi.GetHavaDurumuUrlByCity(Name).Result;
        public string SonDurumUrl => HavaDurumuServisi.GetSonDurumUrl(Name);
        public string KlasikUrl => HavaDurumuServisi.GetKlasikTahminUrl(Name);

        // Şehir koordinatları
        public (double lat, double lon)? Koordinatlar => HavaDurumuServisi.GetCityCoordinates(Name);

        // Görüntüleme için formatlanmış isim
        public string FormattedName =>
            System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Name.ToLower());

        // Favori durumu için emoji
        public string FavoriteIcon => IsFavorite ? "⭐" : "☆";

        // Son güncelleme zamanı
        public string SonGuncellemeText =>
            $"Son güncelleme: {SonGuncelleme:HH:mm}";

        public void ToggleFavorite()
        {
            IsFavorite = !IsFavorite;
        }

        public void UpdateLastRefresh()
        {
            SonGuncelleme = DateTime.Now;
        }
    }

    public enum HavaDurumuGoruntulemeMode
    {
        Tahmin,
        SonDurum,
        Klasik
    }
}