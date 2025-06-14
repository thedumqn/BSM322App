using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSM322App.HaberApi
{
    // BSM322App için güncellenmiş Haber API sınıfları
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

    public class Enclosure
    {
        public string link { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
    }

    public class Feed
    {
        public string url { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string link { get; set; } = string.Empty;
        public string author { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;
    }

    public class Item
    {
        public string title { get; set; } = string.Empty;
        public string pubDate { get; set; } = string.Empty;
        public string link { get; set; } = string.Empty;
        public string guid { get; set; } = string.Empty;
        public string author { get; set; } = string.Empty;
        public string thumbnail { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public Enclosure? enclosure { get; set; }
        public List<object> categories { get; set; } = new List<object>();

        // UI için yardımcı özellikler
        public string KisaBaslik => title.Length > 60 ? title.Substring(0, 60) + "..." : title;
        public string KisaAciklama => string.IsNullOrEmpty(description) ? "Açıklama bulunamadı" :
            (description.Length > 100 ? description.Substring(0, 100) + "..." : description);

        public DateTime TarihSaat
        {
            get
            {
                if (DateTime.TryParse(pubDate, out DateTime result))
                    return result;
                return DateTime.Now;
            }
        }

        public string FormatliTarih => TarihSaat.ToString("dd.MM.yyyy HH:mm");

        public bool HasThumbnail => !string.IsNullOrEmpty(thumbnail);
    }

    public class Root
    {
        public string status { get; set; } = string.Empty;
        public Feed? feed { get; set; }
        public List<Item> items { get; set; } = new List<Item>();
    }
}