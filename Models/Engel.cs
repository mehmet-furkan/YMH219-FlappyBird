using System.Drawing.Drawing2D;

namespace FlappyBird.Models;

/// <summary>
/// Üstte ve altta birer sütundan oluşan, ortasında geçiş boşluğu olan engel.
/// Sağdan sola kayar; ekranın solundan çıkınca sağa yeniden konumlandırılır.
/// </summary>
public class Engel : OyunNesnesi
{
    public const float SutunGenisligi = 64;
    public const float BoslukYuksekligi = 150;

    /// <summary>Boşluğun üst kenarının Y koordinatı.</summary>
    public float BoslukY { get; private set; }

    /// <summary>Kuş bu engeli geçince true olur; aynı engelden iki kez puan alınmasın diye.</summary>
    public bool Gecildi { get; set; }

    public RectangleF UstSutun => new(X, 0, Genislik, BoslukY);
    public RectangleF AltSutun => new(X, BoslukY + BoslukYuksekligi, Genislik, Yukseklik - BoslukY - BoslukYuksekligi);

    public Engel(float x, float boslukY, float alanYuksekligi)
    {
        X = x;
        BoslukY = boslukY;
        Genislik = SutunGenisligi;
        Yukseklik = alanYuksekligi;
    }

    /// <summary>Engeli verilen hız kadar sola kaydırır.</summary>
    public void Kaydir(float hiz) => X -= hiz;

    public bool EkrandanCikti => X + Genislik < 0;

    /// <summary>Engeli sağa taşır ve boşluğun yüksekliğini yeniden belirler.</summary>
    public void YenidenKonumlandir(float yeniX, float yeniBoslukY)
    {
        X = yeniX;
        BoslukY = yeniBoslukY;
        Gecildi = false;
    }

    public bool Carpisiyor(RectangleF hedef) =>
        hedef.IntersectsWith(UstSutun) || hedef.IntersectsWith(AltSutun);

    public override void Ciz(Graphics g)
    {
        SutunCiz(g, UstSutun, ustte: true);
        SutunCiz(g, AltSutun, ustte: false);
    }

    private static void SutunCiz(Graphics g, RectangleF r, bool ustte)
    {
        if (r.Height <= 0) return;

        using var firca = new LinearGradientBrush(r, Color.FromArgb(74, 222, 128), Color.FromArgb(22, 101, 52), 0f);
        g.FillRectangle(firca, r);
        using var kalem = new Pen(Color.FromArgb(20, 83, 45), 2);
        g.DrawRectangle(kalem, r.X, r.Y, r.Width, r.Height);

        // sütunun boşluğa bakan ucunda daha geniş bir başlık
        const float baslikYuksekligi = 22;
        var baslik = new RectangleF(r.X - 5, ustte ? r.Bottom - baslikYuksekligi : r.Y, r.Width + 10, baslikYuksekligi);
        g.FillRectangle(firca, baslik);
        g.DrawRectangle(kalem, baslik.X, baslik.Y, baslik.Width, baslik.Height);
    }
}
