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
    }

    private void ArkaPlaniCiz(Graphics g)
    {
        var alan = oyunPaneli.ClientRectangle;
        using var gokyuzu = new LinearGradientBrush(alan, Color.FromArgb(125, 211, 252), Color.FromArgb(224, 242, 254), 90f);
        g.FillRectangle(gokyuzu, alan);
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
