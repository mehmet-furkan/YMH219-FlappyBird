using System.Drawing.Drawing2D;
using FlappyBird.Services;

namespace FlappyBird;

/// <summary>
/// Oyun penceresi. Timer her ~16 ms'de bir oyunu bir kare ilerletir ve paneli yeniden çizdirir.
/// Klavye: Space / Yukarı ok = zıpla.
/// </summary>
public partial class AnaForm : Form
{
    private Oyun _oyun = null!;

    public AnaForm()
    {
        InitializeComponent();
    }

    private void AnaForm_Load(object? sender, EventArgs e)
    {
        // Panel Dock=Fill ile formu kapladığı için oyun alanı formun iç boyutudur.
        // (Load anında panelin yerleşimi henüz hesaplanmamış olabilir.)
        _oyun = new Oyun(ClientSize.Width, ClientSize.Height);
        oyunZamanlayici.Start();
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

        if (_oyun.Durum == OyunDurumu.Bitti)
            OyunBittiPaneliniCiz(g);
    }

    private void ArkaPlaniCiz(Graphics g)
    {
        var alan = oyunPaneli.ClientRectangle;
        using var gokyuzu = new LinearGradientBrush(alan, Color.FromArgb(125, 211, 252), Color.FromArgb(224, 242, 254), 90f);
        g.FillRectangle(gokyuzu, alan);
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
    }

    // ---------- Girdi ----------

    private void AnaForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.Space or Keys.Up)
        {
            _oyun.Zipla();
            e.SuppressKeyPress = true;
        }
    }

    private void oyunPaneli_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
            _oyun.Zipla();
    }
}
