namespace FlappyBird.Models;

/// <summary>
/// Arka planda yavaşça kayan bulut. Engellerden daha yavaş kaydığı için
/// derinlik hissi verir (paralaks). Çarpışmaya katılmaz, sadece süstür.
/// </summary>
public class Bulut : OyunNesnesi
{
    /// <summary>Bulutun engel hızına göre ne kadar yavaş kaydığı (0.3 = %30'u).</summary>
    public const float HizOrani = 0.3f;

    public Bulut(float x, float y, float olcek)
    {
        X = x;
        Y = y;
        Genislik = 90 * olcek;
        Yukseklik = 36 * olcek;
    }

    public void Kaydir(float oyunHizi) => X -= oyunHizi * HizOrani;

    public bool EkrandanCikti => X + Genislik < 0;

    public void YenidenKonumlandir(float yeniX, float yeniY)
    {
        X = yeniX;
        Y = yeniY;
    }

    public override void Ciz(Graphics g)
    {
        using var firca = new SolidBrush(Color.FromArgb(245, 250, 255)); // opak: daireler üst üste binince iz kalmasın
        // Üç daireyi üst üste bindirerek bulut şekli
        g.FillEllipse(firca, X, Y + Yukseklik * 0.35f, Genislik * 0.5f, Yukseklik * 0.65f);
        g.FillEllipse(firca, X + Genislik * 0.22f, Y, Genislik * 0.5f, Yukseklik);
        g.FillEllipse(firca, X + Genislik * 0.5f, Y + Yukseklik * 0.3f, Genislik * 0.5f, Yukseklik * 0.7f);
    }
}
