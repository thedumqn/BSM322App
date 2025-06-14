using System.Collections.ObjectModel;
using Firebase.Database;
using Firebase.Database.Query;

namespace BSM322App;

public partial class YapilacaklarPage : ContentPage
{
    public ObservableCollection<Gorev> Gorevler { get; set; }
    private FirebaseClient firebase;

    public YapilacaklarPage()
    {
        InitializeComponent();
        Gorevler = new ObservableCollection<Gorev>();
        BindingContext = this;

        // Firebase bağlantısı - Kendi Firebase URL'inizi buraya yazın
        firebase = new FirebaseClient("https://gorselodevbsm322-default-rtdb.firebaseio.com/");

        GorevleriYukle();
    }

    private async void OnYeniGorevClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GorevDetayPage(null, firebase));
    }

    private async void OnDuzenleClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var gorev = (Gorev)button.CommandParameter;
        await Navigation.PushAsync(new GorevDetayPage(gorev, firebase));
    }

    private async void OnSilClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var gorev = (Gorev)button.CommandParameter;

        bool silmeOnay = await DisplayAlert("Silme Onayı",
            $"'{gorev.Baslik}' görevi silinsin mi?", "Evet", "Hayır");

        if (silmeOnay)
        {
            await GorevSil(gorev);
        }
    }

    private async void OnTamamlandiChanged(object sender, CheckedChangedEventArgs e)
    {
        var checkBox = (CheckBox)sender;
        var gorev = (Gorev)checkBox.BindingContext;
        gorev.Tamamlandi = e.Value;

        await GorevGuncelle(gorev);
    }

    private async Task GorevleriYukle()
    {
        try
        {
            var gorevler = await firebase
                .Child("gorevler")
                .OnceAsync<Gorev>();

            Gorevler.Clear();
            foreach (var item in gorevler)
            {
                var gorev = item.Object;
                gorev.Id = item.Key;
                Gorevler.Add(gorev);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Görevler yüklenirken hata: {ex.Message}", "Tamam");
        }
    }

    private async Task GorevSil(Gorev gorev)
    {
        try
        {
            await firebase
                .Child("gorevler")
                .Child(gorev.Id)
                .DeleteAsync();

            Gorevler.Remove(gorev);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Görev silinirken hata: {ex.Message}", "Tamam");
        }
    }

    private async Task GorevGuncelle(Gorev gorev)
    {
        try
        {
            await firebase
                .Child("gorevler")
                .Child(gorev.Id)
                .PutAsync(gorev);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Görev güncellenirken hata: {ex.Message}", "Tamam");
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GorevleriYukle();
    }
}

// Görev model sınıfı
public class Gorev
{
    public string Id { get; set; }
    public string Baslik { get; set; }
    public string Detay { get; set; }
    public DateTime TarihSaat { get; set; }
    public bool Tamamlandi { get; set; }
}