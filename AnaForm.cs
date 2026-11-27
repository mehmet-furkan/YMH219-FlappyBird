using System.Drawing.Drawing2D;
using FlappyBird.Services;

namespace FlappyBird;

/// <summary>
/// Oyun penceresi. Timer her ~16 ms'de bir oyunu bir kare ilerletir ve paneli yeniden çizdirir.
/// Klavye: Space / Yukarı ok = zıpla, P = duraklat, Enter / R = yeniden başla.
/// Form sadece girdiyi Oyun'a iletir ve çizer; kurallar Oyun sınıfındadır.
/// </summary>
public partial class AnaForm : Form
{
    private Oyun _oyun = null!;
    private readonly SkorDeposu _skorDeposu = new();

    public AnaForm()
    {
        InitializeComponent();
    }

    private void AnaForm_Load(object? sender, EventArgs e)
    {
        // Panel Dock=Fill ile formu kapladığı için oyun alanı formun iç boyutudur.
        // (Load anında panelin yerleşimi henüz hesaplanmamış olabilir.)
        _oyun = new Oyun(ClientSize.Width, ClientSize.Height, _skorDeposu);
        _oyun.SkorDegisti += Oyun_SkorDegisti;
        _oyun.OyunBitti += Oyun_OyunBitti;
        oyunZamanlayici.Start();
    }

    private void Oyun_SkorDegisti(object? sender, int skor) =>
        Text = $"Flappy Bird — YMH219  |  Skor: {skor}";

    private void Oyun_OyunBitti(object? sender, EventArgs e)
    {
        Text = $"Flappy Bird — YMH219  |  Oyun bitti, skor: {_oyun.Skor}";

        // Skor dosyası yazılamadıysa oyunu durdurmadan kullanıcıya bildir.
        if (_skorDeposu.SonHata is string hata)
            Text += "  (" + hata + ")";
    }

    // ---------- Oyun döngüsü ----------

    private void oyunZamanlayici_Tick(object? sender, EventArgs e)
    {
        _oyun.Tik();
        oyunPaneli.Invalidate(); // Paint olayını tetikler
    }

    private void oyunPaneli_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        ArkaPlaniCiz(g);
        _oyun.Ciz(g);

        if (_oyun.Durum is OyunDurumu.Oynaniyor or OyunDurumu.Duraklatildi)
            SkoruCiz(g);

        if (_oyun.Durum == OyunDurumu.Hazir)
            HazirYazisiniCiz(g);
        else if (_oyun.Durum == OyunDurumu.Duraklatildi)
            DuraklatildiYazisiniCiz(g);
        else if (_oyun.Durum == OyunDurumu.Bitti)
            OyunBittiPaneliniCiz(g);
    }

    private void ArkaPlaniCiz(Graphics g)
    {
        var alan = oyunPaneli.ClientRectangle;
        using var gokyuzu = new LinearGradientBrush(alan, Color.FromArgb(125, 211, 252), Color.FromArgb(224, 242, 254), 90f);
        g.FillRectangle(gokyuzu, alan);
    }

    /// <summary>Ekranın üst ortasına büyük, kenarlıklı skor yazısı çizer.</summary>
    private void SkoruCiz(Graphics g)
    {
        string metin = _oyun.Skor.ToString();
        using var yol = new GraphicsPath();
        using var yaziTipi = new FontFamily("Segoe UI");
        var ortala = new StringFormat { Alignment = StringAlignment.Center };
        yol.AddString(metin, yaziTipi, (int)FontStyle.Bold, 64, new RectangleF(0, 30, oyunPaneli.Width, 90), ortala);

        using var kenar = new Pen(Color.FromArgb(60, 40, 0), 6) { LineJoin = LineJoin.Round };
        g.DrawPath(kenar, yol);
        g.FillPath(Brushes.White, yol);

        using var kucuk = new Font("Segoe UI", 12, FontStyle.Bold);
        using var renk = new SolidBrush(Color.FromArgb(200, 30, 41, 59));
        g.DrawString($"Seviye {_oyun.Seviye}", kucuk, renk, new RectangleF(0, 118, oyunPaneli.Width, 24), ortala);
    }

    /// <summary>Oyun başlamadan önce gösterilen başlık ve "Space ile başla" yazısı.</summary>
    private void HazirYazisiniCiz(Graphics g)
    {
        var ortala = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        float orta = oyunPaneli.Height * 0.62f;

        using var baslik = new Font("Segoe UI", 34, FontStyle.Bold);
        using var golge = new SolidBrush(Color.FromArgb(90, 0, 0, 0));
        var baslikAlani = new RectangleF(0, oyunPaneli.Height * 0.18f, oyunPaneli.Width, 70);
        g.DrawString("FLAPPY BIRD", baslik, golge, new RectangleF(baslikAlani.X + 3, baslikAlani.Y + 3, baslikAlani.Width, baslikAlani.Height), ortala);
        g.DrawString("FLAPPY BIRD", baslik, Brushes.Gold, baslikAlani, ortala);

        var kutu = new RectangleF(oyunPaneli.Width / 2f - 140, orta - 28, 280, 56);
        using (var zemin = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            g.FillRectangle(zemin, kutu);
        using var yazi = new Font("Segoe UI", 16, FontStyle.Bold);
        using var renk = new SolidBrush(Color.FromArgb(30, 64, 175));
        g.DrawString("Space ile başla", yazi, renk, kutu, ortala);

        using var kucuk = new Font("Segoe UI", 11, FontStyle.Bold);
        g.DrawString($"En yüksek skor: {_oyun.EnYuksekSkor}", kucuk, renk,
            new RectangleF(0, kutu.Bottom + 8, oyunPaneli.Width, 26), ortala);
    }

    private void DuraklatildiYazisiniCiz(Graphics g)
    {
        using (var karartma = new SolidBrush(Color.FromArgb(90, 0, 0, 0)))
            g.FillRectangle(karartma, oyunPaneli.ClientRectangle);

        var ortala = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var buyuk = new Font("Segoe UI", 28, FontStyle.Bold);
        using var kucuk = new Font("Segoe UI", 12);
        float orta = oyunPaneli.Height / 2f;
        g.DrawString("Duraklatıldı", buyuk, Brushes.White, new RectangleF(0, orta - 40, oyunPaneli.Width, 50), ortala);
        g.DrawString("Devam etmek için P", kucuk, Brushes.White, new RectangleF(0, orta + 12, oyunPaneli.Width, 26), ortala);
    }

    /// <summary>Ekranı karartıp ortada "Game Over" kutusu çizer.</summary>
    private void OyunBittiPaneliniCiz(Graphics g)
    {
        var alan = oyunPaneli.ClientRectangle;
        using (var karartma = new SolidBrush(Color.FromArgb(110, 0, 0, 0)))
            g.FillRectangle(karartma, alan);

        var kutu = new Rectangle(alan.Width / 2 - 150, alan.Height / 2 - 110, 300, 200);
        using (var zemin = new SolidBrush(Color.FromArgb(250, 245, 230)))
            g.FillRectangle(zemin, kutu);
        using (var kenar = new Pen(Color.FromArgb(120, 72, 0), 4))
            g.DrawRectangle(kenar, kutu);

        using var buyuk = new Font("Segoe UI", 30, FontStyle.Bold);
        var ortala = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString("Game Over", buyuk, Brushes.OrangeRed, new RectangleF(kutu.X, kutu.Y + 10, kutu.Width, 60), ortala);

        using var orta = new Font("Segoe UI", 18, FontStyle.Bold);
        using var yaziRengi = new SolidBrush(Color.FromArgb(70, 50, 20));
        g.DrawString($"Skor: {_oyun.Skor}", orta, yaziRengi, new RectangleF(kutu.X, kutu.Y + 75, kutu.Width, 36), ortala);

        using var rekorYazisi = new Font("Segoe UI", 12, FontStyle.Bold);
        string rekor = _oyun.YeniRekor ? $"Yeni rekor! En yüksek: {_oyun.EnYuksekSkor}" : $"En yüksek: {_oyun.EnYuksekSkor}";
        var rekorRengi = _oyun.YeniRekor ? Brushes.SeaGreen : yaziRengi;
        g.DrawString(rekor, rekorYazisi, rekorRengi, new RectangleF(kutu.X, kutu.Y + 112, kutu.Width, 30), ortala);

        using var kucuk = new Font("Segoe UI", 11);
        g.DrawString("Enter / R ile yeniden başla", kucuk, yaziRengi, new RectangleF(kutu.X, kutu.Y + 150, kutu.Width, 30), ortala);
    }

    /// <summary>
    /// Kullanıcı başka pencereye geçerse (Alt+Tab vb.) oyun kendiliğinden duraklar,
    /// geri döndüğünde P ile devam eder; arka planda kuş düşüp ölmez.
    /// </summary>
    private void AnaForm_Deactivate(object? sender, EventArgs e)
    {
        _oyun.Duraklat();
        oyunPaneli.Invalidate();
    }

    // ---------- Girdi ----------

    private void AnaForm_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Space or Keys.Up:
                _oyun.Zipla();
                e.SuppressKeyPress = true;
                break;
            case Keys.P:
                _oyun.DuraklatVeyaDevamEt();
                break;
            case Keys.Enter or Keys.R:
                _oyun.YenidenBaslat();
                e.SuppressKeyPress = true;
                break;
        }
    }

    private void oyunPaneli_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
            _oyun.Zipla();
    }
}
