using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSM322App.HaberApi
{
    /*
     TRT Haber RSS Endpoints - BSM322App için güncellenmiş versiyon
        https://www.trthaber.com/manset_articles.rss
        https://www.trthaber.com/sondakika_articles.rss
        https://www.trthaber.com/koronavirus_articles.rss
        https://www.trthaber.com/gundem_articles.rss
        https://www.trthaber.com/turkiye_articles.rss
        https://www.trthaber.com/dunya_articles.rss
        https://www.trthaber.com/ekonomi_articles.rss
        https://www.trthaber.com/spor_articles.rss
        https://www.trthaber.com/yasam_articles.rss
        https://www.trthaber.com/saglik_articles.rss
        https://www.trthaber.com/kultur_sanat_articles.rss
        https://www.trthaber.com/bilim_teknoloji_articles.rss
        https://www.trthaber.com/guncel_articles.rss
        https://www.trthaber.com/egitim_articles.rss
        https://www.trthaber.com/infografik_articles.rss
        https://www.trthaber.com/interaktif_articles.rss
        https://www.trthaber.com/ozel_haber_articles.rss
        https://www.trthaber.com/dosya_haber_articles.rss
     */

    public class HaberKategori
    {
        public string KategoriAdi { get; set; }
        public string KategoriUrl { get; set; }

        public HaberKategori(string kategoriAdi, string kategoriUrl)
        {
            KategoriAdi = kategoriAdi;
            KategoriUrl = kategoriUrl;
        }

        public override string ToString()
        {
            return KategoriAdi;
        }

        // Kategori rengini döndürür (UI için kullanılabilir)
        public Color GetKategoriRengi()
        {
            return KategoriAdi switch
            {
                var k when k.Contains("Son Dakika") => Colors.Red,
                var k when k.Contains("Spor") => Colors.Green,
                var k when k.Contains("Ekonomi") => Colors.Orange,
                var k when k.Contains("Sağlık") => Colors.Blue,
                var k when k.Contains("Teknoloji") => Colors.Purple,
                var k when k.Contains("Dünya") => Colors.Teal,
                var k when k.Contains("Kültür") => Colors.Pink,
                _ => Colors.Gray
            };
        }
    }
}