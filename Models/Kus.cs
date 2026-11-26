using System.Drawing.Drawing2D;

namespace FlappyBird.Models;

/// <summary>
/// Oyuncunun kontrol ettiği kuş. Her karede yer çekimi dikey hızı artırır,
/// Zipla() ise hızı yukarı yönlü sabit bir değere çeker.
/// </summary>
public class Kus : OyunNesnesi
{
    public const float YerCekimi = 0.45f;   // her karede dikey hıza eklenen değer (piksel/kare²)
    public const float ZiplamaHizi = -7.5f; // zıplayınca dikey hız (negatif = yukarı)
    public const float AzamiDususHizi = 11f;

    public float DikeyHiz { get; private set; }

    public Kus(float x, float y)
    {
        X = x;
        Y = y;
        Genislik = 34;
        Yukseklik = 26;
    }

    // Çarpışma kutusunu çizilen şekilden biraz küçük tutuyoruz; oyun daha adil hissettiriyor.
    public override RectangleF Sinirlar => new(X + 3, Y + 3, Genislik - 6, Yukseklik - 6);

    public void Zipla() => DikeyHiz = ZiplamaHizi;

    /// <summary>Bir kare ilerletir: hıza yer çekimini ekler, konumu hıza göre değiştirir.</summary>
    public void Guncelle()
    {
        DikeyHiz = Math.Min(DikeyHiz + YerCekimi, AzamiDususHizi);
        Y += DikeyHiz;
    }

    /// <summary>Kuşu tavanın altında tutar (tavana çarpmak oyunu bitirmez, sadece durdurur).</summary>
    public void TavandaTut()
    {
        if (Y < 0)
        {
            Y = 0;
            DikeyHiz = 0;
        }
    }

    public override void Ciz(Graphics g)
    {
        // Kuş, dikey hızına göre hafifçe yukarı/aşağı eğilir.
        float aci = Math.Clamp(DikeyHiz * 4f, -25f, 70f);
        var durum = g.Save();
        g.TranslateTransform(X + Genislik / 2, Y + Yukseklik / 2);
        g.RotateTransform(aci);

        var govde = new RectangleF(-Genislik / 2, -Yukseklik / 2, Genislik, Yukseklik);
        using (var firca = new LinearGradientBrush(govde, Color.FromArgb(255, 214, 64), Color.FromArgb(245, 158, 11), 90f))
            g.FillEllipse(firca, govde);
        using (var kalem = new Pen(Color.FromArgb(120, 72, 0), 2))
            g.DrawEllipse(kalem, govde);

        // kanat
        g.FillEllipse(Brushes.White, -12, 0, 14, 9);
        g.DrawEllipse(Pens.SaddleBrown, -12, 0, 14, 9);
        // göz
        g.FillEllipse(Brushes.White, 5, -9, 10, 10);
        g.FillEllipse(Brushes.Black, 10, -6, 4, 4);
        // gaga
        g.FillPolygon(Brushes.OrangeRed, new[] { new PointF(14, -1), new PointF(23, 2), new PointF(14, 6) });

        g.Restore(durum);
    }
}
