using FlappyBird.Models;

namespace FlappyBird.Services;

public enum OyunDurumu
{
    Hazir,       // oyun başlamayı bekliyor
    Oynaniyor,
    Duraklatildi,
    Bitti
}

/// <summary>
/// Oyunun tüm durumunu ve kurallarını yönetir: kuş, engeller, hız.
/// Arayüzden bağımsızdır; form her Timer tikinde Tik() çağırır ve Ciz() ile ekrana çizdirir.
/// </summary>
public class Oyun
{
    private const int EngelSayisi = 3;
    private const float EngelAraligi = 230;   // iki engel arasındaki yatay mesafe
    private const float BaslangicHizi = 3.2f;

    private readonly Random _rastgele = new();
    private readonly List<Engel> _engeller = new();

    public float AlanGenisligi { get; }
    public float AlanYuksekligi { get; }
    public float ZeminYuksekligi => 60;
    public float OynanabilirYukseklik => AlanYuksekligi - ZeminYuksekligi;

    public Kus Kus { get; private set; }
    public IReadOnlyList<Engel> Engeller => _engeller;
    public OyunDurumu Durum { get; private set; } = OyunDurumu.Hazir;
    public float Hiz { get; private set; } = BaslangicHizi;

    /// <summary>Zeminin kayma animasyonu için toplam kayma miktarı.</summary>
    public float ZeminKaymasi { get; private set; }

    public Oyun(float alanGenisligi, float alanYuksekligi)
    {
        AlanGenisligi = alanGenisligi;
        AlanYuksekligi = alanYuksekligi;
        Kus = new Kus(0, 0);
        Sifirla();
    }

    /// <summary>Kuşu başlangıç noktasına, engelleri ekranın sağına yerleştirir.</summary>
    public void Sifirla()
    {
        Kus = new Kus(AlanGenisligi * 0.25f, OynanabilirYukseklik / 2 - 13);
        Hiz = BaslangicHizi;

        _engeller.Clear();
        for (int i = 0; i < EngelSayisi; i++)
            _engeller.Add(new Engel(AlanGenisligi + 80 + i * EngelAraligi, RastgeleBoslukY(), OynanabilirYukseklik));

        Durum = OyunDurumu.Hazir;
    }

    /// <summary>Space/yukarı tuşu: oyun hazırsa başlatır, oynanıyorsa kuşu zıplatır.</summary>
    public void Zipla()
    {
        if (Durum == OyunDurumu.Hazir)
            Durum = OyunDurumu.Oynaniyor;

        if (Durum == OyunDurumu.Oynaniyor)
            Kus.Zipla();
    }

    /// <summary>Oyun döngüsünün bir karesi: her şeyi bir adım ilerletir.</summary>
    public void Tik()
    {
        if (Durum != OyunDurumu.Oynaniyor) return;

        Kus.Guncelle();
        Kus.TavandaTut();

        foreach (var engel in _engeller)
        {
            engel.Kaydir(Hiz);

            // Soldan çıkan engeli en sağdaki engelin arkasına taşı.
            if (engel.EkrandanCikti)
                engel.YenidenKonumlandir(_engeller.Max(e => e.X) + EngelAraligi, RastgeleBoslukY());
        }

        ZeminKaymasi = (ZeminKaymasi + Hiz) % 24;
    }

    private float RastgeleBoslukY()
    {
        // Boşluk ne tavana ne zemine fazla yakın olsun.
        const float kenarPayi = 50;
        float enAz = kenarPayi;
        float enCok = OynanabilirYukseklik - Engel.BoslukYuksekligi - kenarPayi;
        return enAz + (float)_rastgele.NextDouble() * (enCok - enAz);
    }

    // ---------- Çizim ----------

    public void Ciz(Graphics g)
    {
        foreach (var engel in _engeller)
            engel.Ciz(g);

        ZeminiCiz(g);
        Kus.Ciz(g);
    }

    private void ZeminiCiz(Graphics g)
    {
        float y = OynanabilirYukseklik;
        using var toprak = new SolidBrush(Color.FromArgb(222, 184, 135));
        using var cim = new SolidBrush(Color.FromArgb(101, 163, 13));
        using var cizgi = new Pen(Color.FromArgb(160, 120, 80), 3);

        g.FillRectangle(toprak, 0, y, AlanGenisligi, ZeminYuksekligi);
        g.FillRectangle(cim, 0, y, AlanGenisligi, 12);
        // kayan çizgiler zeminin hareket ettiği hissini verir
        for (float x = -ZeminKaymasi; x < AlanGenisligi; x += 24)
            g.DrawLine(cizgi, x, y + 22, x + 12, y + 38);
    }
}
