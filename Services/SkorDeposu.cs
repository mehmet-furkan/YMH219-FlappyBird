namespace FlappyBird.Services;

/// <summary>
/// En yüksek skoru %AppData%\YMH219-FlappyBird\enyuksek.txt dosyasında saklar.
/// Dosya yoksa, bozuksa veya erişilemiyorsa oyun çökmez: okuma 0 döner, yazma false döner.
/// </summary>
public class SkorDeposu
{
    public string DosyaYolu { get; }

    /// <summary>Son okuma/yazma işleminde oluşan hatanın açıklaması (hata yoksa null).</summary>
    public string? SonHata { get; private set; }

    public SkorDeposu()
        : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "YMH219-FlappyBird", "enyuksek.txt"))
    {
    }

    public SkorDeposu(string dosyaYolu) => DosyaYolu = dosyaYolu;

    public int Oku()
    {
        SonHata = null;
        try
        {
            if (!File.Exists(DosyaYolu))
                return 0;

            string metin = File.ReadAllText(DosyaYolu).Trim();
            if (int.TryParse(metin, out int skor) && skor >= 0)
                return skor;

            SonHata = "Skor dosyası okunamadı (geçersiz içerik), 0 kabul edildi.";
            return 0;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            SonHata = "Skor dosyası okunamadı: " + ex.Message;
            return 0;
        }
    }

    public bool Kaydet(int skor)
    {
        SonHata = null;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DosyaYolu)!);
            File.WriteAllText(DosyaYolu, skor.ToString());
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            SonHata = "En yüksek skor kaydedilemedi: " + ex.Message;
            return false;
        }
    }
}
